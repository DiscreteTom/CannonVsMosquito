using DT.General;
using UnityEngine.UI;

public class RoomIdInput : CBC {
  void Start() {
    var config = this.Get<Config>();

    this.GetComponent<InputField>().onEndEdit.AddListener((string roomId) => {
      config.roomId = roomId;
    });
  }
}