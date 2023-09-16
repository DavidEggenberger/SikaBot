package com.example.hackzurich2023.chatbot

import android.view.View
import android.widget.TextView
import com.example.hackzurich2023.R


class LeftChatViewHolder(itemView: View) : ChatViewHolder(itemView) {
    var contents: TextView
   // var time: TextView

    init {
        contents = itemView.findViewById(R.id.chatItem_left_text) as TextView
        //time = itemView.findViewById(R.id.chatItem_left_time) as TextView
    }
}