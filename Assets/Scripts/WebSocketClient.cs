using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected
    }

    [SerializeField]
    private Text statusText;

    [SerializeField]
    private Image statusImage;

    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public string url = "ws://localhost:8080";
    private WebSocket ws;

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }

        UpdateState(ConnectionState.Connecting);
        ws = new WebSocket(url);
        ws.OnOpen += (s, e) => UpdateState(ConnectionState.Connected);
        ws.OnClose += (s, e) => UpdateState(ConnectionState.Disconnected);
        ws.OnError += (s, e) => UpdateState(ConnectionState.Disconnected);
        ws.ConnectAsync();
    }

    private void UpdateState(ConnectionState newState)
    {
        State = newState;
        if (statusText != null)
        {
            statusText.text = State.ToString();
        }
        if (statusImage != null)
        {
            switch (State)
            {
                case ConnectionState.Connected:
                    statusImage.color = Color.green;
                    break;
                case ConnectionState.Connecting:
                    statusImage.color = Color.yellow;
                    break;
                default:
                    statusImage.color = Color.red;
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
