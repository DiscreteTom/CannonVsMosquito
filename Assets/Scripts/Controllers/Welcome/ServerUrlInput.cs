using DT.General;
using TMPro;

public class ServerUrlInput : CBC {
  void Start() {
    var config = this.Get<Config>();

    this.GetComponent<TMP_InputField>().onEndEdit.AddListener((string serverUrl) => {
      config.serverUrl = serverUrl;
    });
  }
}