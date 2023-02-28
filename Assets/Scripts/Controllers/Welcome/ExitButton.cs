using DT.General;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : CBC {
  void Start() {
    this.GetComponent<Button>().onClick.AddListener(PanicManager.Panic);
  }
}