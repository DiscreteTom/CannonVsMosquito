using DT.UniStart;
using DT.UniUtils;
using UnityEngine;

public class MockPlayer : CBC {
  void Start() {
    var cannon = this.transform.Find("Cannon");
    var config = this.Get<Config>();
    bool clockwise = false;

    // start rotation within angle range
    this.onUpdate.AddListener(() => {
      if (clockwise) {
        cannon.Rotate(0, 0, -config.cannonRotationSpeed * Time.deltaTime);
      } else {
        cannon.Rotate(0, 0, config.cannonRotationSpeed * Time.deltaTime);
      }
      var angle = cannon.localEulerAngles.z;
      if (!clockwise && angle < 180 && angle > config.angleRange / 2) {
        clockwise = true;
      } else if (clockwise && angle > 180 && angle < -config.angleRange / 2 + 360) {
        clockwise = false;
      }
    });
  }
}
