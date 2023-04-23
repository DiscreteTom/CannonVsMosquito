using DT.General;
using DT.UniStart;
using UnityEngine.UI;

public class ExitButton : CBC {
  void Start() {
    this.GetComponent<Button>().onClick.AddListener(PanicManager.Panic);
  }
}