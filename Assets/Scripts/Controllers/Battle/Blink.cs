using DT.UniStart;
using TMPro;
using UnityEngine;

public class Blink : CBC {
  [SerializeField] float blinkSpeed = 1f;

  void Start() {
    var text = this.GetComponent<TMP_Text>();
    this.onUpdate.AddListener(() => {
      text.alpha = Mathf.PingPong(Time.time * this.blinkSpeed, 1);
    });
  }
}