using DT.UniStart;

public class WaitingText : CBC {
  void Start() {
    var eb = this.Get<IEventBus>();

    eb.AddListener("game.start", (GameStartEvent _) => this.gameObject.SetActive(false));
  }
}