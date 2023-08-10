using DT.UniStart;
using TMPro;

namespace Project.Scene.Welcome {
  public class ServerUrlInput : CBC {
    void Start() {
      var config = this.Get<Config>();
      var eb = this.Get<IEventBus>();
      var input = this.GetComponent<TMP_InputField>();

      // set initial value
      input.text = config.serverUrl;

      // register listener
      input.onEndEdit.AddListener((serverUrl) => config.serverUrl = serverUrl);

      // listen event bus
      eb.AddListener((SetInputServerUrlEvent e) => {
        input.text = e.serverUrl;
        config.serverUrl = e.serverUrl;
      });
    }
  }
}