using DT.UniStart;
using TMPro;
using UnityEngine;

public class Timer : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();
    var text = this.GetComponent<TMP_Text>();
    var config = this.Get<Config>();

    this.Watch(eb, "game.start", (GameStartEvent _) => {
      var timeout = (float)config.gameTimeout;
      text.text = timeout.ToString();
      this.onUpdate.AddListener(() => {
        timeout -= Time.deltaTime;
        if (timeout > -1)
          text.text = Mathf.CeilToInt(timeout).ToString();
      });
    });
  }
}