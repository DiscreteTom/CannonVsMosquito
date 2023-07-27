using System.Web;
using DT.UniStart;
using UnityEngine;

public class Battle : Entry {
  [SerializeField] Config config;

  void Awake() {
    this.Add(this.config);
    var eb = this.Add<IEventBus>(new DebugEventBus());
    var model = this.Add<Model>();

    eb.AddListener("game.start", (GameStartEvent _) => {
      model.state.Value = GameState.PLAYING;
    });
    eb.AddListener("game.over", (GameOverEvent _) => {
      model.state.Value = GameState.OVER;
    });
  }
}