using DT.UniStart;
using TMPro;

public class ErrorText : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();
    var text = this.GetComponent<TMP_Text>();

    text.text = "";

    this.Watch(eb, "game.error", (string msg) => text.text = $"Error: {msg}");
    this.Watch(eb, "ws.error", (string msg) => text.text = $"Error: {msg}");
  }
}