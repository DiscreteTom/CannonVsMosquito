using DT.UniStart;

public class WaitingText : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();

    this.Watch(eb, "game.start", (GameStartEvent _) => this.gameObject.SetActive(false));
  }
}