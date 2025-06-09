using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    [Tooltip("WebSocket server address (e.g. ws://localhost:12345)")]
    public string websocketUrl = "ws://localhost:12345";

    [Tooltip("UI Text component to display incoming messages")]
    public Text outputText;

    private WebSocket ws;
    private string latestMessage;

    void Start()
    {
        ws = new WebSocket(websocketUrl);
        ws.OnMessage += OnMessageReceived;
        ws.ConnectAsync();
    }

    void OnMessageReceived(object sender, MessageEventArgs e)
    {
        latestMessage = e.Data;
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            if (outputText != null)
            {
                outputText.text = latestMessage;
            }
            latestMessage = null;
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
