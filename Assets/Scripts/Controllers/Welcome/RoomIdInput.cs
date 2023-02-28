using DT.General;
using TMPro;

public class RoomIdInput : CBC {
  void Start() {
    var config = this.Get<Config>();
    var input = this.GetComponent<TMP_InputField>();

    // set initial value
    input.text = config.roomId;

    // register listener
    input.onEndEdit.AddListener((roomId) => config.roomId = roomId);
  }
}