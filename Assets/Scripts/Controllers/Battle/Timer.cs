using DT.General;
using TMPro;
using UnityEngine;

public class Timer : CBC {
  void Start() {
    var eb = this.Get<EventBus>();
    var text = this.GetComponent<TMP_Text>();
    var config = this.Get<Config>();

    eb.AddListener("game.start", (GameStartEvent _) => {
      var timeout = (float)config.timeout;
      text.text = timeout.ToString();
      this.OnUpdate.AddListener(() => {
        timeout -= Time.deltaTime;
        text.text = Mathf.CeilToInt(timeout).ToString();
      });
    });
  }
}