using DT.UniStart;
using DT.UniUtils;
using UnityEngine;

public class TitleText : CBC {
  void Start() {
    // float up and down
    this.onUpdate.AddListener(() => {
      this.transform.SetPositionY(this.transform.position.y + Mathf.Sin(Time.time * 2) * 0.001f);
    });
  }
}
