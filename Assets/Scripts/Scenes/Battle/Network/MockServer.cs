using System.Collections.Generic;
using DT.UniStart;
using UnityEngine;

namespace Project.Scene.Battle {
  public class MockServer {
    public void Start(Config config, IEventBus eb, ComposableBehaviour entry) {
      Debug.Log("Using mock server");
      var targets = new Dictionary<int, Target>();
      var targetId = 0;

      // init targets
      for (var i = 0; i < config.initTargetCount; ++i) {
        targets[i] = new Target {
          x = Random.Range(-5f, 5f),
          y = Random.Range(-3f, 3f),
          id = targetId,
        };
        ++targetId;
      }

      // start game
      entry.Invoke(() => {
        eb.Invoke(new GameStartEvent {
          targets = targets.Values.Map(v => v) // to array
        });
      }, config.mockServerLatency);

      // handle shoot events
      eb.AddListener((LocalShootEvent e) => {
        var (x, y, angle, playerId) = e;
        entry.Invoke(() => {
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
      entry.InvokeRepeating(() => {
        if (!generate) return;
        var newTargets = new Target[config.newTargetCount];
        for (var i = 0; i < config.newTargetCount; ++i) {
          targets[targetId] = new Target {
            x = Random.Range(-5f, 5f),
            y = Random.Range(-3f, 3f),
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
      entry.Invoke(() => {
        eb.Invoke(new GameOverEvent {
          winner = config.localPlayerId,
        });
        generate = false;
      }, config.mockServerLatency + config.gameTimeout);
    }
  }
}