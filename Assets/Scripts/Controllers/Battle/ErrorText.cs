using DT.UniStart;
using TMPro;

public class ErrorText : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();
    var text = this.GetComponent<TMP_Text>();

    text.text = "";

    eb.AddListener("game.error", (string msg) => text.text = $"Error: {msg}");
    eb.AddListener("ws.error", (string msg) => text.text = $"Error: {msg}");
  }
}