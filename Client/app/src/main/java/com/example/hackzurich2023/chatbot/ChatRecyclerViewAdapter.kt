package com.example.hackzurich2023.chatbot

import android.icu.text.DateFormat
import android.icu.text.SimpleDateFormat
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.hackzurich2023.R
import java.util.*


class ChatRecyclerViewAdapter : RecyclerView.Adapter<ChatViewHolder>() {
    private val ITEM_LEFT = 1
    private val ITEM_RIGHT = 2

    private var chatMessages = mutableListOf<ChatMessage>()
    var timeStampStr: String? = null

    override fun getItemViewType(position: Int): Int {
        return if (chatMessages[position].senderType == SenderType.BOT) 1 else 2
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ChatViewHolder {
        return when (viewType) {
            ITEM_LEFT -> LeftChatViewHolder(
                LayoutInflater.from(parent.context).inflate(R.layout.chatitem_left, parent, false)
            )
            else -> RightChatViewHolder(
                LayoutInflater.from(parent.context).inflate(R.layout.chatitem_right, parent, false)
            )
        }
    }

    override fun onBindViewHolder(holder: ChatViewHolder, position: Int) {
        val chatMessage: ChatMessage = chatMessages[position]
        if (holder.itemViewType == ITEM_LEFT) {
            val viewHolder: LeftChatViewHolder = holder as LeftChatViewHolder
            viewHolder.contents.text = chatMessage.content
//            timeStampStr = chatMessage.getTime()
//            viewHolder.time.setText(timeStampStr)
        } else {
            val viewHolder: RightChatViewHolder = holder as RightChatViewHolder
            viewHolder.contents.text = chatMessage.content
//            timeStampStr = chatMessage.getTime()
//            viewHolder.time.setText(timeStampStr)
        }
    }


    override fun getItemCount(): Int {
        return chatMessages.size
    }

    fun setChatMessages(chatMessages: List<ChatMessage>) {
        this.chatMessages.clear()
        this.chatMessages.addAll(chatMessages)
        notifyDataSetChanged()
    }

    fun addMessage(chatMessage: ChatMessage) {
        chatMessages.add(chatMessage)
        notifyDataSetChanged()
    }

    fun deleteMessage(position: Int) {
        chatMessages.removeAt(position)
        notifyDataSetChanged()
    }

    fun getRequiredTime(timeStampStr: String): String? {
        return try {
            val sdf: DateFormat = SimpleDateFormat("yyyy-MM-dd HH:mm:ss")
            val netDate = Date(timeStampStr.toLong())
            sdf.format(netDate)
        } catch (ignored: Exception) {
            "xx"
        }
    }
}