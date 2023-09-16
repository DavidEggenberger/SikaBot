package com.example.hackzurich2023.network

import io.reactivex.schedulers.Schedulers
import java.util.concurrent.TimeUnit
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.adapter.rxjava2.RxJava2CallAdapterFactory
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface ChatApi {

    @POST("api/Chats")
    suspend fun createChat(@Body body: MessageResponse): MessageResponse

    @POST("api/Chats/{chatId}/messages")
    suspend fun sendUserMessage(
        @Path("chatId") chatId: String,
        @Body body: MessageItem
    ): Unit


    @GET("api/Chats/{chatId}")
    suspend fun getChatMessages(@Path("chatId") chatId: String): MessageResponse
}

fun createChatApi(): ChatApi {

    val interceptor = HttpLoggingInterceptor()
    interceptor.level = HttpLoggingInterceptor.Level.BODY

    val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(60, TimeUnit.SECONDS)
        .readTimeout(60, TimeUnit.SECONDS)
        .writeTimeout(60, TimeUnit.SECONDS)
        .addInterceptor(interceptor)
        .build()

    return Retrofit.Builder().baseUrl("https://sikabot.azurewebsites.net/")
        .addConverterFactory(GsonConverterFactory.create())
        .client(okHttpClient)
        .addCallAdapterFactory(RxJava2CallAdapterFactory.createWithScheduler(Schedulers.io()))
        .build().create(ChatApi::class.java)


}