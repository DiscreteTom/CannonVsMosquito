using DT.General;
using TMPro;

public class RoomIdInput : CBC {
  void Start() {
    var config = this.Get<Config>();

    this.GetComponent<TMP_InputField>().onEndEdit.AddListener((string roomId) => {
      config.roomId = roomId;
    });
  }
}