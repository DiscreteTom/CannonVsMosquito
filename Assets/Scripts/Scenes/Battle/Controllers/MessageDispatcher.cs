using System;
using System.Collections.Generic;
using DT.UniStart;

using UnityEngine;

public class MessageDispatcher : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();
    var config = this.Get<Config>();
    var useMockServer = config.serverUrl == "";

    if (!useMockServer) {
      // create or join server when the websocket is connected
      this.Watch(eb, "ws.connected", () => {
        if (config.localPlayerId == 0) {
          // create room
          eb.Invoke("ws.send", JsonUtility.ToJson(new CreateRoomAction {
            action = "create",
            room = config.roomId,
          }));
        } else {
          // join room
          eb.Invoke("ws.send", JsonUtility.ToJson(new JoinRoomAction {
            action = "join",
            room = config.roomId,
          }));
        }
      });

      // process game events
      this.Watch(eb, "local.shoot", (float x, float y, float angle, int playerId) => {
        eb.Invoke("ws.send", JsonUtility.ToJson(new ShootAction {
          action = "shoot",
          origin = new Origin {
            x = x,
            y = y,
          },
          angle = angle,
        }));
      });

      // listen for messages from the server and invoke game events
      this.Watch(eb, "ws.message", (string str) => {
        var raw = TryDeserialize<ServerMessage>(str);
        if (raw.type == "error") {
          var msg = TryDeserialize<ErrorEvent>(raw.msg);
          eb.Invoke("game.error", msg.type);
          Debug.LogError(msg.type);
        } else if (raw.type == "game start") {
          var msg = TryDeserialize<GameStartEvent>(raw.msg);
          eb.Invoke("game.start", msg);
        } else if (raw.type == "player shoot") {
          var msg = TryDeserialize<PlayerShootEvent>(raw.msg);
          eb.Invoke("game.shoot", msg);
        } else if (raw.type == "new target") {
          var msg = TryDeserialize<NewTargetEvent>(raw.msg);
          eb.Invoke("game.newTarget", msg);
        } else if (raw.type == "game over") {
          var msg = TryDeserialize<GameOverEvent>(raw.msg);
          eb.Invoke("game.over", msg);
        } else {
          Debug.LogError("Unknown message type: " + raw.type);
          eb.Invoke("game.error", "Unknown message type: " + raw.type);
        }
      });
    } else {
      print("Using mock server");
      var targets = new Dictionary<int, Target>();
      var targetId = 0;

      // init targets
      for (var i = 0; i < config.initTargetCount; ++i) {
        targets[i] = new Target {
          x = UnityEngine.Random.Range(-5f, 5f),
          y = UnityEngine.Random.Range(-3f, 3f),
          id = targetId,
        };
        ++targetId;
      }

      // start game
      this.Invoke(() => {
        eb.Invoke("game.start", new GameStartEvent {
          targets = targets.Values.Map(v => v) // to array
        });
      }, config.mockServerLatency);

      // handle shoot events
      this.Watch(eb, "local.shoot", (float x, float y, float angle, int playerId) => {
        this.Invoke(() => {
          // calculate hit
          var hit = new List<int>();
          foreach (var target in targets.Values) {
            var targetPos = new Vector2(target.x, target.y);
            var origin = new Vector2(x, y);
            // calculate the distance between the target and the laser with origin and angle
            var distance = Vector2.Distance(targetPos, origin) * Mathf.Abs(Mathf.Sin((Vector2.Angle(targetPos - origin, new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)))) * Mathf.Deg2Rad));
            if (distance < (config.laserWidth + config.targetPrefab.transform.localScale.x) / 2) {
              hit.Add(target.id);
            }
          }

          // remove hit targets
          foreach (var id in hit) {
            targets.Remove(id);
          }

          eb.Invoke("game.shoot", (new PlayerShootEvent {
            player = playerId,
            hit = hit.ToArray(),
            origin = new Origin {
              x = x,
              y = y,
            },
            angle = angle,
          }));
        }, config.mockServerLatency);
      });

      // generate new target
      var generate = true;
      this.InvokeRepeating(() => {
        if (!generate) return;
        var newTargets = new Target[config.newTargetCount];
        for (var i = 0; i < config.newTargetCount; ++i) {
          targets[targetId] = new Target {
            x = UnityEngine.Random.Range(-5f, 5f),
            y = UnityEngine.Random.Range(-3f, 3f),
            id = targetId,
          };
          newTargets[i] = targets[targetId];
          ++targetId;
        }
        eb.Invoke("game.newTarget", new NewTargetEvent {
          targets = newTargets
        });
      }, config.mockServerLatency + config.newTargetInterval, config.newTargetInterval);

      // mock game over event
      this.Invoke(() => {
        eb.Invoke("game.over", new GameOverEvent {
          winner = config.localPlayerId,
        });
        generate = false;
      }, config.mockServerLatency + config.gameTimeout);
    }
  }

  static T TryDeserialize<T>(string str) {
    try {
      return JsonUtility.FromJson<T>(str);
    } catch (Exception e) {
      Debug.LogError("Failed to deserialize string: " + str + " to type " + typeof(T).Name + ", returning default value.");
      Debug.LogError(e);
      return default(T);
    }
  }
}
