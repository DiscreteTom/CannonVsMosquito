using DT.UniStart;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreenButton : CBC {
  void Start() {
    this.onMouseDown.AddListener(() => Screen.fullScreen = !Screen.fullScreen);
  }
}