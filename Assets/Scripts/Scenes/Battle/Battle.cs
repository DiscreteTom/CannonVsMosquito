using DT.UniStart;
using UnityEngine;

namespace Project.Scene.Battle {
  public class Battle : Entry {
    [SerializeField] Config config;

    void Awake() {
      var eb = new DebugEventBus();
      var cb = new DebugCommandBus();
      var model = new Model();

      this.Add(this.config);
      this.Add<IEventBus>(eb);
      this.Add<ICommandBus>(cb);
      this.Add(model);

      eb.AddListener((GameStartEvent _) => {
        model.state.Value = GameState.PLAYING;
      });
      eb.AddListener((GameOverEvent _) => {
        model.state.Value = GameState.OVER;
      });
    }
  }
}