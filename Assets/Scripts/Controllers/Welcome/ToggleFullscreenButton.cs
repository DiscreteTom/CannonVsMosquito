using DT.UniStart;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreenButton : CBC {
  void Start() {
    this.GetComponent<Button>().onClick.AddListener(() => Screen.fullScreen = !Screen.fullScreen);
  }
}