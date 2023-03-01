using DT.General;
using NativeWebSocket;
using UnityEngine;

public class WebSocketManager : CBC {
  WebSocket websocket = null;

  async void Start() {
    var eb = this.Get<EventBus>();
    var config = this.Get<Config>();

    if (config.serverUrl == "") return; // using mock server

    this.websocket = new WebSocket(config.serverUrl);
    this.OnUpdate.AddListener(() => {
#if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
#endif
    });

    this.websocket.OnOpen += () => {
      eb.Invoke("ws.connected");
      Debug.Log("Connection open!");
    };
    this.websocket.OnError += (e) => {
      eb.Invoke("ws.error", e);
      Debug.Log("Error! " + e);
    };
    this.websocket.OnClose += (e) => {
      eb.Invoke("ws.disconnected", e);
      Debug.Log("Connection closed!");
    };
    this.websocket.OnMessage += (bytes) => {
      // getting the message as a string
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      eb.Invoke("ws.message", message);
      Debug.Log("OnMessage! " + message);
    };

    eb.AddListener("ws.send", (string message) => {
      if (websocket.State == WebSocketState.Open) {
        websocket.SendText(message);
      }
    });

    // waiting for messages
    await this.websocket.Connect();
  }

  private async void OnApplicationQuit() {
    if (websocket != null) {
      await websocket.Close();
    }
  }
}