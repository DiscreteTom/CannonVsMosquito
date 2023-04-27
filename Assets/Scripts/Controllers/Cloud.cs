using DT.UniStart;
using DT.UniUtils;
using UnityEngine;

public class Cloud : CBC {
  void Start() {
    // float up and down
    this.onUpdate.AddListener(() => {
      this.transform.SetPositionY(this.transform.position.y + Mathf.Sin(Time.time * 2) * 0.001f);
    });

    // from left to right, from -15 to 15
    this.transform.SetPositionX(-15);
    this.onUpdate.AddListener(() => {
      if (this.transform.position.x < 15) {
        this.transform.SetPositionX(this.transform.position.x + 0.005f);
      } else {
        this.transform.SetPositionX(-15);
      }
    });

    // in play mode, reset position when game start
    var eb = this.Get<IEventBus>();
    eb.AddListener("game.start", (GameStartEvent _) => {
      this.transform.SetPositionX(-15);
    });
  }
}
