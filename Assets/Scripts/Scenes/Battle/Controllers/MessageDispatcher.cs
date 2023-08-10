using System;
using System.Collections.Generic;
using DT.UniStart;

using UnityEngine;

namespace Project.Scene.Battle {
  public class MessageDispatcher : CBC {
    void Start() {
      var eb = this.Get<IEventBus>();
      var config = this.Get<Config>();
      var useMockServer = config.serverUrl == "";

      if (!useMockServer) {
        // create or join server when the websocket is connected
        this.Watch(eb, (WebSocketConnected _) => {
          if (config.localPlayerId == 0) {
            // create room
            eb.Invoke(new WebSocketSendEvent(JsonUtility.ToJson(new CreateRoomAction {
              action = "create",
              room = config.roomId,
            })));
          } else {
            // join room
            eb.Invoke(new WebSocketSendEvent(JsonUtility.ToJson(new JoinRoomAction {
              action = "join",
              room = config.roomId,
            })));
          }
        });

        // process game events
        this.Watch(eb, (LocalShootEvent e) => {
          var (x, y, angle, playerId) = e;
          eb.Invoke(new WebSocketSendEvent(JsonUtility.ToJson(new ShootAction {
            action = "shoot",
            origin = new Origin {
              x = x,
              y = y,
            },
            angle = angle,
          })));
        });

        // listen for messages from the server and invoke game events
        this.Watch(eb, (WebSocketMessageEvent e) => {
          var str = e.str;
          var raw = TryDeserialize<ServerMessage>(str);
          if (raw.type == "error") {
            var msg = TryDeserialize<ErrorEvent>(raw.msg);
            eb.Invoke(new GameErrorEvent(msg.type));
            Debug.LogError(msg.type);
          } else if (raw.type == "game start") {
            var msg = TryDeserialize<GameStartEvent>(raw.msg);
            eb.Invoke(msg);
          } else if (raw.type == "player shoot") {
            var msg = TryDeserialize<PlayerShootEvent>(raw.msg);
            eb.Invoke(new GameShootEvent(msg));
          } else if (raw.type == "new target") {
            var msg = TryDeserialize<NewTargetEvent>(raw.msg);
            eb.Invoke(msg);
          } else if (raw.type == "game over") {
            var msg = TryDeserialize<GameOverEvent>(raw.msg);
            eb.Invoke(msg);
          } else {
            Debug.LogError("Unknown message type: " + raw.type);
            eb.Invoke(new GameErrorEvent("Unknown message type: " + raw.type));
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
          eb.Invoke(new GameStartEvent {
            targets = targets.Values.Map(v => v) // to array
          });
        }, config.mockServerLatency);

        // handle shoot events
        this.Watch(eb, (LocalShootEvent e) => {
          var (x, y, angle, playerId) = e;
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

            eb.Invoke(new GameShootEvent(new PlayerShootEvent {
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
          eb.Invoke(new NewTargetEvent {
            targets = newTargets
          });
        }, config.mockServerLatency + config.newTargetInterval, config.newTargetInterval);

        // mock game over event
        this.Invoke(() => {
          eb.Invoke(new GameOverEvent {
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
        return default;
      }
    }
  }
}