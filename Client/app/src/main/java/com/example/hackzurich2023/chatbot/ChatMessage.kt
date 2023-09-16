package com.example.hackzurich2023.chatbot

data class ChatMessage(val content: String, val senderType: SenderType)

enum class SenderType {
    BOT,
    USER
}