using DT.UniStart;
using NativeWebSocket;
using UnityEngine;

public class WebSocketManager : CBC {
  async void Start() {
    var eb = this.Get<IEventBus>();
    var config = this.Get<Config>();

    if (config.serverUrl == "") return; // using mock server

    var websocket = new WebSocket(config.serverUrl);
    this.onUpdate.AddListener(() => {
#if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
#endif
    });

    websocket.OnOpen += () => {
      eb.Invoke("ws.connected");
      Debug.Log("Connection open!");
    };
    websocket.OnError += (e) => {
      eb.Invoke("ws.error", e);
      Debug.Log("Error! " + e);
    };
    websocket.OnClose += (e) => {
      eb.Invoke("ws.disconnected", e);
      Debug.Log("Connection closed!");
    };
    websocket.OnMessage += (bytes) => {
      // getting the message as a string
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      eb.Invoke("ws.message", message);
      Debug.Log("OnMessage! " + message);
    };

    eb.AddListener("ws.send", (string message) => {
      if (websocket.State == WebSocketState.Open) {
        Debug.Log("Send: " + message);
        websocket.SendText(message);
      }
    });

    this.onApplicationQuit.AddListener(() => {
      if (websocket != null) {
        websocket.Close();
      }
    });

    // waiting for messages
    await websocket.Connect();
  }
}