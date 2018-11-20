/*
 * Copyright 2018 ICON Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

package foundation.icon.icx;

import foundation.icon.icx.crypto.IconKeys;
import foundation.icon.icx.data.*;
import foundation.icon.icx.transport.jsonrpc.*;
import foundation.icon.icx.transport.jsonrpc.RpcConverter.RpcConverterFactory;

import java.math.BigInteger;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * IconService which provides APIs of ICON network.
 */
@SuppressWarnings("WeakerAccess")
public class IconService {

    private Provider provider;
    private List<RpcConverter.RpcConverterFactory> converterFactories = new ArrayList<>();
    private Map<Class, RpcConverter<?>> converterMap = new HashMap<>();

    /**
     * Creates IconService instance
     *
     * @param provider the worker that transporting requests
     */
    @SuppressWarnings("unchecked")
    public IconService(Provider provider) {
        this.provider = provider;
        addConverterFactory(Converters.newFactory(BigInteger.class, Converters.BIG_INTEGER));
        addConverterFactory(Converters.newFactory(Boolean.class, Converters.BOOLEAN));
        addConverterFactory(Converters.newFactory(String.class, Converters.STRING));
        addConverterFactory(Converters.newFactory(Bytes.class, Converters.BYTES));
        addConverterFactory(Converters.newFactory(byte[].class, Converters.BYTE_ARRAY));
        addConverterFactory(Converters.newFactory(Block.class, Converters.BLOCK));
        addConverterFactory(Converters.newFactory(
                ConfirmedTransaction.class, Converters.CONFIRMED_TRANSACTION));
        addConverterFactory(Converters.newFactory(
                TransactionResult.class, Converters.TRANSACTION_RESULT));
        Class<List<ScoreApi>> listClass = ((Class) List.class);
        addConverterFactory(Converters.newFactory(listClass, Converters.SCORE_API_LIST));
        addConverterFactory(Converters.newFactory(RpcItem.class, Converters.RPC_ITEM));
    }

    /**
     * Get the total number of issued coins.
     *
     * @return A BigNumber instance of the total number of coins.
     */
    public Request<BigInteger> getTotalSupply() {
        long requestId = System.currentTimeMillis();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getTotalSupply", null);
        return provider.request(request, findConverter(BigInteger.class));
    }

    /**
     * Get the balance of an address.
     *
     * @param address The address to get the balance of.
     * @return A BigNumber instance of the current balance for the given address in loop.
     */
    public Request<BigInteger> getBalance(Address address) {
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("address", new RpcValue(address))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getBalance", params);
        return provider.request(request, findConverter(BigInteger.class));
    }

    /**
     * Get a block matching the block number.
     *
     * @param height The block number
     * @return The Block object
     */
    public Request<Block> getBlock(BigInteger height) {
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("height", new RpcValue(height))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getBlockByHeight", params);
        return provider.request(request, findConverter(Block.class));
    }

    /**
     * Get a block matching the block hash.
     *
     * @param hash The block hash (without hex prefix) or the string 'latest'
     * @return The Block object
     */
    public Request<Block> getBlock(Bytes hash) {
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("hash", new RpcValue(hash))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getBlockByHash", params);
        return provider.request(request, findConverter(Block.class));
    }


    /**
     * Get the latest block.
     *
     * @return The Block object
     */
    public Request<Block> getLastBlock() {
        long requestId = System.currentTimeMillis();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getLastBlock", null);
        return provider.request(request, findConverter(Block.class));
    }

    /**
     * Get information about api function in score
     *
     * @param scoreAddress The address to get APIs
     * @return The ScoreApi object
     */
    @SuppressWarnings("unchecked")
    public Request<List<ScoreApi>> getScoreApi(Address scoreAddress) {
        if (!IconKeys.isContractAddress(scoreAddress))
            throw new IllegalArgumentException("Only the contract address can be called.");
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("address", new RpcValue(scoreAddress))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getScoreApi", params);
        Class<List<ScoreApi>> listClass = ((Class) List.class);
        return provider.request(request, findConverter(listClass));
    }


    /**
     * Get a transaction matching the given transaction hash.
     *
     * @param hash The transaction hash
     * @return The Transaction object
     */
    public Request<ConfirmedTransaction> getTransaction(Bytes hash) {
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("txHash", new RpcValue(hash))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getTransactionByHash", params);
        return provider.request(request, findConverter(ConfirmedTransaction.class));
    }

    /**
     * Get the result of a transaction by transaction hash.
     *
     * @param hash The transaction hash
     * @return The TransactionResult object
     */
    public Request<TransactionResult> getTransactionResult(Bytes hash) {
        long requestId = System.currentTimeMillis();
        RpcObject params = new RpcObject.Builder()
                .put("txHash", new RpcValue(hash))
                .build();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_getTransactionResult", params);
        return provider.request(request, findConverter(TransactionResult.class));
    }

    /**
     * Calls a SCORE API just for reading
     *
     * @param call instance of Call
     * @param <O> responseType
     * @return the Request object can execute a request
     */
    public <O> Request<O> call(Call<O> call) {
        long requestId = System.currentTimeMillis();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(requestId, "icx_call", call.getProperties());
        return provider.request(request, findConverter(call.responseType()));
    }

    /**
     * Sends a transaction that changes the states of account
     *
     * @param signedTransaction parameters including signatures
     * @return the Request object can execute a request (result type is txHash)
     */
    public Request<Bytes> sendTransaction(SignedTransaction signedTransaction) {
        long requestId = System.currentTimeMillis();
        foundation.icon.icx.transport.jsonrpc.Request request = new foundation.icon.icx.transport.jsonrpc.Request(
                requestId, "icx_sendTransaction", signedTransaction.getProperties());
        return provider.request(request, findConverter(Bytes.class));
    }

    @SuppressWarnings("unchecked")
    private <T> RpcConverter<T> findConverter(Class<T> type) {
        RpcConverter converter = converterMap.get(type);
        if (converter != null) return converter;

        for (RpcConverterFactory factory : converterFactories) {
            converter = factory.create(type);
            if (converter != null) {
                converterMap.put(type, converter);
                return converter;
            }
        }

        if (type.isAnnotationPresent(AnnotationConverter.class)) {
            if (type.getAnnotation(AnnotationConverter.class).use()) {
                return new AnnotatedConverterFactory().create(type);
            }
        }

        throw new IllegalArgumentException("Could not locate response converter for:'" + type + "'");
    }

    /**
     * Adds Converter factory.
     * It has a create function that creates the converter of the specific type.
     *
     * @param factory a converter factory
     */
    public void addConverterFactory(RpcConverterFactory factory) {
        converterFactories.add(factory);
    }

}
