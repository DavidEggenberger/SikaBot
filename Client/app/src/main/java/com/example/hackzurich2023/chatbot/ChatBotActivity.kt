package com.example.hackzurich2023.chatbot

import android.annotation.SuppressLint
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.activity.viewModels
import androidx.lifecycle.Observer
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView.VERTICAL
import com.example.hackzurich2023.databinding.ActivityChatBotBinding
import com.google.android.material.snackbar.Snackbar
import com.tinder.scarlet.Lifecycle
import com.tinder.scarlet.Message
import com.tinder.scarlet.Scarlet
import com.tinder.scarlet.StreamAdapter
import com.tinder.scarlet.WebSocket
import com.tinder.scarlet.lifecycle.android.AndroidLifecycle
import com.tinder.scarlet.streamadapter.rxjava2.RxJava2StreamAdapterFactory
import com.tinder.scarlet.websocket.okhttp.newWebSocketFactory
import io.reactivex.android.schedulers.AndroidSchedulers
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import com.tinder.scarlet.Message as MessageScarlet

class ChatBotActivity : AppCompatActivity() {
    companion object {
        private const val ECHO_URL = ""
    }

    private lateinit var webSocketService: EchoService
    private lateinit var binding: ActivityChatBotBinding
    private val adapter = ChatRecyclerViewAdapter()

    private val viewModel: ChatBotViewModel by viewModels()


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityChatBotBinding.inflate(layoutInflater)
        val view = binding.root
        setContentView(view)
        setupRecyclerView()
        setupSendMessageButton()
        createChat()
        observeUiStates()
    }

    private fun createChat() {
        viewModel.createChat()
    }

    private fun observeUiStates() {
        viewModel.uiStateObserver.observe(this, Observer { uiStates ->
            when (uiStates) {
                is ChatBotViewModel.UiState.Loading -> {}
                is ChatBotViewModel.UiState.OnChatCreatedSuccessfully -> {
                    Toast.makeText(this, "Chat Created successfully", Toast.LENGTH_LONG).show()
                }
                is ChatBotViewModel.UiState.OnUserMessageSendSuccessfully -> {
                    Toast.makeText(this, "User Message sent successfully", Toast.LENGTH_LONG).show()
                }
                is ChatBotViewModel.UiState.OnChatMessagesUpdates -> {
                    adapter.setChatMessages(uiStates.messages.map {
                        var senderType = SenderType.BOT
                        if (!it.botGenerated)
                            senderType = SenderType.USER

                        ChatMessage(it.text, senderType)
                    })

                }

            }

        })
    }

    private fun setupSendMessageButton() {
        binding.layout.imageButton.setOnClickListener {
            val message = binding.layout.edtMessage.text.toString()
            if (message.isNotEmpty() && message.isNotBlank()) {
                adapter.addMessage(ChatMessage(message, SenderType.USER))
                binding.layout.edtMessage.text.clear()
                viewModel.sendUserMessage(message)
            }

        }
    }

    private fun setupWebSocketService() {
        webSocketService = provideWebSocketService(
            scarlet = provideScarlet(
                client = provideOkhttp(),
                lifecycle = provideLifeCycle(),
                streamAdapterFactory = provideStreamAdapterFactory(),
            )
        )
    }

    private fun setupRecyclerView() {
        val recyclerView = binding.rclChat
        val linearLayout = LinearLayoutManager(this)
        linearLayout.orientation = VERTICAL
        recyclerView.layoutManager = linearLayout
        recyclerView.adapter = adapter

        adapter.addMessage(ChatMessage("Hello I am here ", SenderType.BOT))
    }

    @SuppressLint("CheckResult")
    private fun observeConnection() {
        webSocketService.observeConnection()
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe({ response ->
                Log.d("observeConnection", response.toString())
                onReceiveResponseConnection(response)
            }, { error ->
                Log.e("observeConnection", error.message.orEmpty())
                Snackbar.make(binding.root, error.message.orEmpty(), Snackbar.LENGTH_SHORT).show()
            })
    }

    private fun onReceiveResponseConnection(response: WebSocket.Event) {
        when (response) {
            is WebSocket.Event.OnConnectionOpened<*> -> changeToolbarTitle("connection opened")
            is WebSocket.Event.OnConnectionClosed -> changeToolbarTitle("connection closed")
            is WebSocket.Event.OnConnectionClosing -> changeToolbarTitle("closing connection..")
            is WebSocket.Event.OnConnectionFailed -> changeToolbarTitle("connection failed")
            is WebSocket.Event.OnMessageReceived -> handleOnMessageReceived(response.message)
        }
    }


    private fun handleOnMessageReceived(message: MessageScarlet) {
        adapter.addMessage(ChatMessage(message.toValue(), SenderType.BOT))
//        adapter.addItem(Message(message.toValue(), false))
//        binding.etMessage.setText("")
    }

    private fun MessageScarlet.toValue(): String {
        return when (this) {
            is Message.Text -> value
            is Message.Bytes -> value.toString()
        }
    }

    private fun changeToolbarTitle(title: String) {
        binding.toolbar.title = title
    }

    private fun provideWebSocketService(scarlet: Scarlet) = scarlet.create(EchoService::class.java)

    private fun provideScarlet(
        client: OkHttpClient,
        lifecycle: Lifecycle,
        streamAdapterFactory: StreamAdapter.Factory,
    ) =
        Scarlet.Builder()
            .webSocketFactory(client.newWebSocketFactory(ECHO_URL))
            .lifecycle(lifecycle)
            .addStreamAdapterFactory(streamAdapterFactory)
            .build()


    private fun provideOkhttp() =
        OkHttpClient.Builder()
            .addInterceptor(HttpLoggingInterceptor().setLevel(HttpLoggingInterceptor.Level.BASIC))
            .build()

    private fun provideLifeCycle() = AndroidLifecycle.ofApplicationForeground(application)

    private fun provideStreamAdapterFactory() = RxJava2StreamAdapterFactory()
}