using System.Web;
using DT.UniStart;
using UnityEngine;

public class App : Entry {
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

    // read server url from query string
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      // use next update to wait for listener to be registered
      this.onNextUpdate.AddListener(() => {
        eb.Invoke("set.input.serverUrl", HttpUtility.ParseQueryString(Application.absoluteURL.Split('?')[1]).Get("serverUrl"));
      });
    }
  }
}