using DT.UniStart;
using UnityEngine;

namespace Project.Scene.Battle {
  public class BattleEntry : Entry {
    [SerializeField] Config config;

    void Awake() {
      var eb = new DebugEventBus();
      var cb = new DebugCommandBus();
      var model = new ModelManager(cb, eb);
      var ws = new WebSocketManager();
      var md = new MessageDispatcher(config, cb, eb);
      var ms = new MockServer();

      this.Add(this.config);
      this.Add<IEventListener>(eb);
      this.Add<ICommandBus>(cb);
      this.Add<Model>(model);

      eb.AddListener((GameStartEvent _) => {
        cb.Push(new GameStartCommand());
      });
      eb.AddListener((GameOverEvent _) => {
        cb.Push(new GameOverCommand());
      });

      ws.Start(config, eb, this);
      ms.Start(config, eb, this);
    }
  }
}