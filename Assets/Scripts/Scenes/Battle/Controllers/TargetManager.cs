using System.Collections.Generic;
using DT.UniStart;

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
        Debug.Log("TargetManager: target created with id " + t.id);
      });
    });

    this.Watch(eb, "game.newTarget", (NewTargetEvent e) => {
      e.targets.ForEach(t => {
        var target = Instantiate(config.targetPrefab);
        target.transform.position = new Vector3(t.x, t.y, 0);
        targetDict[t.id] = target;
        Debug.Log("TargetManager: target created with id " + t.id);
      });
    });

    this.Watch(eb, "game.shoot", (PlayerShootEvent e) => {
      e.hit.ForEach(id => {
        var go = targetDict[id];
        if (go == null) {
          Debug.LogError("TargetManager: target not found for id: " + id);
          return;
        }
        Instantiate(config.deadTargetPrefab, go.transform.position, Quaternion.identity);
        Destroy(go);
        targetDict.Remove(id);
        Debug.Log("TargetManager: target destroyed with id " + id);
      });
    });

    this.Watch(eb, "game.over", (GameOverEvent _) => {
      targetDict.Values.ForEach((g) => Destroy(g));
      targetDict.Clear();
    });
  }
}