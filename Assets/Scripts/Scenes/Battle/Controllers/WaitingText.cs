using DT.UniStart;

namespace Project.Scene.Battle {
  public class WaitingText : CBC {
    void Start() {
      var eb = this.Get<IEventBus>();

      this.Watch(eb, (GameStartEvent _) => this.gameObject.SetActive(false));
    }
  }
}