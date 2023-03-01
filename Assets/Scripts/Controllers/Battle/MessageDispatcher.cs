using DT.General;
using UnityEngine;

public class MessageDispatcher : CBC {
  void Start() {
    var eb = this.Get<EventBus>();
    var config = this.Get<Config>();
    var useMockServer = config.serverUrl == "";

    if (!useMockServer) {
      // create or join server when the websocket is connected
      eb.AddListener("ws.connected", () => {
        if (config.localPlayerId == 0) {
          // create room
          eb.Invoke("ws.send", JsonUtility.ToJson(new {
            action = "create",
            room = config.roomId,
          }));
        } else {
          // join room
          eb.Invoke("ws.send", JsonUtility.ToJson(new {
            action = "join",
            room = config.roomId,
          }));
        }
      });

      // process game events
      eb.AddListener("local.shoot", (float x, float y, float angle) => {
        eb.Invoke("ws.send", JsonUtility.ToJson(new {
          action = "shoot",
          origin = new {
            x = x,
            y = y,
          },
          angle = angle,
        }));
      });

      // listen for messages from the server and invoke game events
      eb.AddListener("ws.message", (string str) => {
        var raw = JsonUtility.FromJson<ServerMessage>(str);
        if (raw.type == "error") {
          var msg = JsonUtility.FromJson<ErrorEvent>(str);
          eb.Invoke("game.error", msg);
          Debug.LogError(msg.type);
        } else if (raw.type == "game start") {
          var msg = JsonUtility.FromJson<GameStartEvent>(str);
          eb.Invoke("game.start", msg);
        } else if (raw.type == "player shoot") {
          var msg = JsonUtility.FromJson<PlayerShootEvent>(str);
          eb.Invoke("game.shoot", msg);
        } else if (raw.type == "new target") {
          var msg = JsonUtility.FromJson<NewTargetEvent>(str);
          eb.Invoke("game.newTarget", msg);
        } else if (raw.type == "game over") {
          var msg = JsonUtility.FromJson<GameOverEvent>(str);
          eb.Invoke("game.over", msg);
        } else {
          Debug.LogError("Unknown message type: " + raw.type);
        }
      });
    } else {
      print("Using mock server");
      this.Invoke(() => {
        eb.Invoke("game.start", new GameStartEvent {
          targets = new Target[] {
            new Target {
              x = Random.Range(-5f, 0f),
              y = Random.Range(-3, 3),
              id = 0,
            },
            new Target {
              x = Random.Range(0f, 5f),
              y = Random.Range(-3, 3),
              id = 1,
            },
          },
        });
      }, config.mockServerLatency);

      eb.AddListener("local.shoot", (float x, float y, float angle) => {
        this.Invoke(() => {
          eb.Invoke("game.shoot", (new PlayerShootEvent {
            player = config.localPlayerId,
            hit = new int[] { 0, 1 },
            origin = new Origin {
              x = x,
              y = y,
            },
            angle = angle,
          }));
        }, config.mockServerLatency);
      });
    }
  }
}

public struct ServerMessage {
  public string type;
  public string msg;
}
public struct ErrorEvent {
  public string type;
}
public struct Origin {
  public float x;
  public float y;
}
public struct Target {
  public float x;
  public float y;
  public int id;
}
public struct GameStartEvent {
  public Target[] targets;
}
public struct PlayerShootEvent {
  public int[] hit;
  public int player;
  public Origin origin;
  public float angle;
}
public struct NewTargetEvent {
  public Target[] targets;
}
public struct GameOverEvent {
  public int winner;
}