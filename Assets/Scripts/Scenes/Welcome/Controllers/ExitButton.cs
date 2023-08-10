using DT.UniStart;

using UnityEngine.UI;

namespace Project.Scene.Welcome {
  public class ExitButton : CBC {
    void Start() {
      this.GetComponent<Button>().onClick.AddListener(UniStart.ExitGame);
    }
  }
}