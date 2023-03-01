using DT.General;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : CBC {
  void Start() {
    this.GetComponent<Button>().onClick.AddListener(() => {
      SceneManager.LoadScene("Welcome");
    });
  }
}