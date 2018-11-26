using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Lykke.Icon.Sdk
{
    /**
     * Builder for the transaction to send<br>
     * There are four builder types.<br>
     * Builder is a basic builder to send ICXs.<br>
     * CallBuilder, DeployBuilder, MessageBuilder is an extended builder for each purpose.
     * They can be initiated from Builder.
     *
     * @see <a href="https://github.com/icon-project/icon-rpc-server/blob/develop/docs/icon-json-rpc-v3.md#icx_sendtransaction" target="_blank">ICON JSON-RPC API</a>
     */
    public class TransactionBuilder
    {
        /**
         * Creates a builder for the given network ID
         *
         * @param nid network ID
         * @return new builder
         * @deprecated This method can be replaced by {@link #newBuilder()}
         */
        public static Builder Of(NetworkId nid)
        {
            Builder builder = newBuilder();
            return builder.Nid(nid.GetValue());
        }

        /**
         * Creates a builder for the given network ID
         *
         * @param nid network ID in BigInteger
         * @return new builder
         * @deprecated This method can be replaced by {@link #newBuilder()}
         */
        public static Builder Of(BigInteger nid)
        {
            Builder builder = newBuilder();
            return builder.nid(nid);
        }

        /**
         * Creates a builder to make a transaction to send
         *
         * @return new builder
         */
        public static Builder newBuilder()
        {
            return new Builder();
        }

        /**
         * A Builder for the simple icx sending transaction.
         */
        public static class Builder
        {
            private TransactionData transactionData;

            private Builder()
            {
                this.transactionData = new TransactionData();
            }

            /**
             * Sets the Network ID
             *
             * @param nid Network ID ("0x1" for Mainnet, etc)
             * @return self
             */
            public Builder nid(BigInteger nid)
            {
                transactionData.nid = nid;
                return this;
            }

            /**
             * Sets the Network ID
             *
             * @param nid Network ID ("0x1" for Mainnet, etc)
             * @return self
             */
            public Builder nid(NetworkId nid)
            {
                transactionData.nid = nid.getValue();
                return this;
            }

            /**
             * Sets the sender address
             *
             * @param from EOA address that created the transaction
             * @return self
             */
            public Builder from(Address from)
            {
                transactionData.from = from;
                return this;
            }

            /**
             * Sets the receiver address
             *
             * @param to EOA address to receive coins, or SCORE address to execute the transaction.
             * @return self
             */
            public Builder to(Address to)
            {
                transactionData.to = to;
                return this;
            }

            /**
             * Sets the value to send ICXs
             *
             * @param value Amount of ICX coins in loop to transfer. (1 icx = 1 ^ 18 loop)
             * @return self
             */
            public Builder value(BigInteger value)
            {
                transactionData.value = value;
                return this;
            }

            /**
             * Sets the Maximum step
             *
             * @param stepLimit Maximum step allowance that can be used by the transaction.
             * @return self
             */
            public Builder stepLimit(BigInteger stepLimit)
            {
                transactionData.stepLimit = stepLimit;
                return this;
            }

            /**
             * Sets the timestamp
             *
             * @param timestamp Transaction creation time, in microsecond.
             * @return self
             */
            public Builder timestamp(BigInteger timestamp)
            {
                transactionData.timestamp = timestamp;
                return this;
            }

            /**
             * Sets the nonce
             *
             * @param nonce An arbitrary number used to prevent transaction hash collision.
             * @return self
             */
            public Builder nonce(BigInteger nonce)
            {
                transactionData.nonce = nonce;
                return this;
            }

            /**
             * Converts the builder to CallBuilder with the calling method name
             *
             * @param method calling method name
             * @return {@link CallBuilder}
             */
            public CallBuilder call(String method)
            {
                return new CallBuilder(transactionData, method);
            }

            /**
             * Converts the builder to DeployBuilder with the deploying content
             *
             * @param contentType content type
             * @param content     deploying content
             * @return {@link DeployBuilder}
             */
            public DeployBuilder deploy(String contentType, byte[] content)
            {
                return new DeployBuilder(transactionData, contentType, content);
            }

            /**
             * Converts the builder to MessageBuilder with the message
             *
             * @param message message
             * @return {@link MessageBuilder}
             */
            public MessageBuilder message(String message)
            {
                return new MessageBuilder(transactionData, message);
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction build()
            {
                return transactionData.build();
            }

        }

        /**
         * A Builder for the calling SCORE transaction.
         */
        public static class CallBuilder
        {

            private TransactionData transactionData;
            private RpcObject.Builder dataBuilder;

            private CallBuilder(TransactionData transactionData, String method)
            {
                this.transactionData = transactionData;
                this.transactionData.dataType = "call";

                dataBuilder = new RpcObject.Builder()
                        .put("method", new RpcValue(method));
            }

            /**
             * Sets the params
             *
             * @param params Function parameters
             * @return self
             */
            public CallBuilder Params(RpcObject @params)
            {
                dataBuilder.put("params", @params);
                return this;
            }

            /**
             * Sets the params
             *
             * @param params Function parameters
             * @return self
             */
            public CallBuilder Params<T>(T @params)
            {
                dataBuilder.put("params", RpcItemCreator.create(@params));
                return this;
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction build()
            {
                transactionData.data = dataBuilder.build();
                CheckArgument(((RpcObject)transactionData.data).GetItem("method"), "method not found");

                return transactionData.build();
            }
        }

        /**
         * A Builder for the message transaction.
         */
        public static class MessageBuilder
        {
            private TransactionData transactionData;

            private MessageBuilder(TransactionData transactionData, String message)
            {
                this.transactionData = transactionData;
                this.transactionData.dataType = "message";
                this.transactionData.data = new RpcValue(message.getBytes(StandardCharsets.UTF_8));
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction build()
            {
                return transactionData.build();
            }

        }

        /**
         * A Builder for the deploy transaction.
         */
        public static class DeployBuilder
        {

            private TransactionData transactionData;
            private RpcObject.Builder dataBuilder;

            private DeployBuilder(TransactionData transactionData, String contentType, byte[] content)
            {
                this.transactionData = transactionData;
                this.transactionData.dataType = "deploy";

                dataBuilder = new RpcObject.Builder()
                        .put("contentType", new RpcValue(contentType))
                        .put("content", new RpcValue(content));
            }

            /**
             * Sets the params
             *
             * @param params Function parameters will be delivered to on_install() or on_update()
             * @return self
             */
            public DeployBuilder Params(RpcObject @params)
            {
                dataBuilder.put("params", @params);
                return this;
            }

            /**
             * Make a new transaction using given properties
             *
             * @return a transaction to send
             */
            public Transaction build()
            {
                transactionData.data = dataBuilder.build();
                CheckArgument(((RpcObject)transactionData.data).GetItem("contentType"), "contentType not found");
                CheckArgument(((RpcObject)transactionData.data).GetItem("content"), "content not found");

                return transactionData.build();
            }
        }

        private static class TransactionData
        {
            private BigInteger version = new BigInteger("3");
            private Address from;
            private Address to;
            private BigInteger value;
            private BigInteger stepLimit;
            private BigInteger timestamp;
            private BigInteger nid = NetworkId.MAIN.getValue();
            private BigInteger nonce;
            private String dataType;
            private RpcItem data;

            private Transaction build()
            {
                checkAddress(from, "from not found");
                checkAddress(to, "to not found");
                CheckArgument(version, "version not found");
                CheckArgument(stepLimit, "stepLimit not found");
                return new SendingTransaction(this);
            }

            void checkAddress(Address address, String message)
            {
                CheckArgument(address, message);
                if (address.isMalformed())
                {
                    throw new IllegalArgumentException("Invalid address");
                }
            }
        }

        private static class SendingTransaction : Transaction
        {
            private BigInteger version;
            private Address from;
            private Address to;
            private BigInteger value;
            private BigInteger stepLimit;
            private BigInteger timestamp;
            private BigInteger nid;
            private BigInteger nonce;
            private String dataType;
            private RpcItem data;

            private SendingTransaction(TransactionData transactionData)
            {
                version = transactionData.version;
                from = transactionData.from;
                to = transactionData.to;
                value = transactionData.value;
                stepLimit = transactionData.stepLimit;
                timestamp = transactionData.timestamp;
                nid = transactionData.nid;
                nonce = transactionData.nonce;
                dataType = transactionData.dataType;
                data = transactionData.data;
            }

            public BigInteger getVersion()
            {
                return version;
            }

            public Address getFrom()
            {
                return from;
            }

            public Address getTo()
            {
                return to;
            }

            public BigInteger getValue()
            {
                return value;
            }

            public BigInteger getStepLimit()
            {
                return stepLimit;
            }

            public BigInteger getTimestamp()
            {
                return timestamp;
            }

            public BigInteger getNid()
            {
                return nid;
            }

            public BigInteger getNonce()
            {
                return nonce;
            }

            public String getDataType()
            {
                return dataType;
            }

            public RpcItem getData()
            {
                return data;
            }
        }

        static void CheckArgument<T>(T @object, String message)
        {
            if (@object == null)
            {
                throw new ArgumentException(message);
            }
        }
    }
}
