package com.example.hackzurich2023.chatbot

import android.util.Log
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.hackzurich2023.network.ChatApi
import com.example.hackzurich2023.network.MessageItem
import com.example.hackzurich2023.network.MessageResponse
import com.example.hackzurich2023.network.createChatApi
import kotlinx.coroutines.launch

class ChatBotViewModel(private val chatApi: ChatApi = createChatApi()) : ViewModel() {

    private var userId = "df426d5f-83a5-4af9-a8de-9faf14bf8a0f"
    private var messagesList = mutableListOf<MessageItem>()

    val uiStateObserver = MutableLiveData<UiState>()
    var isPolling = false

    fun sendUserMessage(message: String) {
        viewModelScope.launch {
            try {
                uiStateObserver.postValue(UiState.Loading)

                val messageItem = MessageItem("2023-09-16T21:13:53.480Z", message, false)
                messagesList.add(messageItem)
                val response = chatApi.sendUserMessage(userId, messageItem)
                uiStateObserver.postValue(UiState.OnUserMessageSendSuccessfully)
                if (!isPolling) {
                    getChatMessages()
                    isPolling = true
                }
            } catch (exception: Exception) {
                Log.e(exception.message, exception.message.toString())
            }
        }
    }

    fun createChat() {
        viewModelScope.launch {
            try {
                uiStateObserver.postValue(UiState.Loading)
                val messageItem =
                    MessageItem("2023-09-16T21:13:53.480Z", "Connect to the chat bot", false)

                val response =
                    chatApi.createChat(MessageResponse(userId, listOf<MessageItem>(messageItem)))
                userId = response.id

                uiStateObserver.postValue(UiState.OnChatCreatedSuccessfully)
            } catch (exception: Exception) {
                Log.e(exception.message, exception.message.toString())
            }
        }
    }

    fun getChatMessages() {
        viewModelScope.launch {
            while (true) {
                try {
                    val response = chatApi.getChatMessages(userId)
                    uiStateObserver.postValue(UiState.OnChatMessagesUpdates(response.messages))
                } catch (exception: Exception) {
                    Log.e(exception.message, exception.message.toString())
                }
            }
        }
    }

    sealed class UiState {
        object Loading : UiState()
        object OnChatCreatedSuccessfully : UiState()
        object OnUserMessageSendSuccessfully : UiState()
        data class OnChatMessagesUpdates(val messages: List<MessageItem>) : UiState()
    }
}