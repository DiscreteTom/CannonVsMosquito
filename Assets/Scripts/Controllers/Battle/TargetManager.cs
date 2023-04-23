using System.Collections.Generic;
using DT.UniStart;
using DT.UniUtils;
using UnityEngine;

public class TargetManager : CBC {
  void Start() {
    var config = this.Get<Config>();
    var eb = this.Get<IEventBus>();
    var targetDict = new Dictionary<int, GameObject>(); // id -> target

    this.Watch(eb, "game.start", (GameStartEvent e) => {
      e.targets.ForEach(t => {
        var target = Instantiate(config.targetPrefab);
        target.transform.position = new Vector3(t.x, t.y, 0);
        targetDict[t.id] = target;
      });
    });

    this.Watch(eb, "game.newTarget", (NewTargetEvent e) => {
      e.targets.ForEach(t => {
        var target = Instantiate(config.targetPrefab);
        target.transform.position = new Vector3(t.x, t.y, 0);
        targetDict[t.id] = target;
      });
    });

    this.Watch(eb, "game.shoot", (PlayerShootEvent e) => {
      e.hit.ForEach(id => {
        var go = targetDict[id];
        Instantiate(config.deadTargetPrefab, go.transform.position, Quaternion.identity);
        Destroy(go);
        targetDict.Remove(id);
      });
    });

    this.Watch(eb, "game.over", (GameOverEvent _) => {
      targetDict.Values.ForEach((g) => Destroy(g));
      targetDict.Clear();
    });
  }
}