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
        var ss = Application.absoluteURL.Split('?');
        if (ss.Length < 2) return;
        var queryString = ss[1];
        if (queryString == null) return;
        var serverUrl = HttpUtility.ParseQueryString(queryString).Get("serverUrl");
        if (serverUrl == null) return;
        eb.Invoke("set.input.serverUrl", serverUrl);
      });
    }
  }
}