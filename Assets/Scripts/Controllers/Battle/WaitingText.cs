using DT.General;

public class WaitingText : CBC {
  void Start() {
    var eb = this.Get<EventBus>();

    eb.AddListener("game.start", (GameStartEvent _) => this.gameObject.SetActive(false));
  }
}