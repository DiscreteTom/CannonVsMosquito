using DT.General;
using TMPro;

public class ErrorText : CBC {
  void Start() {
    var eb = this.Get<EventBus>();
    var text = this.GetComponent<TMP_Text>();

    text.text = "";

    eb.AddListener("game.error", (string msg) => text.text = $"Error: {msg}");
    eb.AddListener("ws.error", (string msg) => text.text = $"Error: {msg}");
  }
}