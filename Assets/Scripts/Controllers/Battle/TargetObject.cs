using DT.General;
using TMPro;
using UnityEngine;

public class TargetObject : CBC {
  void Start() {
    // float
    var position = this.transform.position;
    var randomX = Random.Range(-3f, 3f);
    var randomY = Random.Range(-3f, 3f);
    this.OnUpdate.AddListener(() => {
      this.transform.position = position + new Vector3(Mathf.Sin(Time.time * 4 + randomX) * 0.03f, Mathf.Sin(Time.time * 4 + randomY) * 0.03f, 0);
    });
  }
}