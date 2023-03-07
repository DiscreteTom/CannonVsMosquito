using DT.General;
using TMPro;
using UnityEngine;

public class TargetObject : CBC {
  void Start() {
    var config = this.Get<Config>();

    // grow up when created with config.initTargetGrowSpeed
    this.transform.localScale = Vector3.zero;
    this.OnUpdate.AddListener(() => {
      if (this.transform.localScale.x < 1)
        this.transform.localScale += Vector3.one * config.initTargetGrowSpeed * Time.deltaTime;
      else this.transform.localScale = Vector3.one;
    });

    // float
    var position = this.transform.position;
    var randomX = Random.Range(-3f, 3f);
    var randomY = Random.Range(-3f, 3f);
    this.OnUpdate.AddListener(() => {
      this.transform.position = position + new Vector3(Mathf.Sin(Time.time * 4 + randomX) * 0.03f, Mathf.Sin(Time.time * 4 + randomY) * 0.03f, 0);
    });
  }
}