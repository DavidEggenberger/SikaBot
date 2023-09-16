package com.example.hackzurich2023.network

data class MessageResponse(val id: String, val messages: List<MessageItem>) {
}

data class MessageItem(val sentAt: String, val text: String, val botGenerated: Boolean)