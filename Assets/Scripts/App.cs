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
        // get url params
        var ss = Application.absoluteURL.Split('?'); // strings
        if (ss.Length < 2) return;
        var queryString = ss[1];
        if (queryString == null) return;
        var urlParams = HttpUtility.ParseQueryString(queryString);

        // auto load server url
        var serverUrl = urlParams.Get("serverUrl");
        if (serverUrl != null) eb.Invoke("set.input.serverUrl", serverUrl);
      });
    }
  }
}