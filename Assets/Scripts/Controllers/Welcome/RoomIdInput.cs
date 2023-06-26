using DT.UniStart;
using TMPro;

public class RoomIdInput : CBC {
  void Start() {
    var config = this.Get<Config>();
    var eb = this.Get<IEventBus>();
    var input = this.GetComponent<TMP_InputField>();

    // set initial value
    input.text = config.roomId;

    // register listener
    input.onEndEdit.AddListener((roomId) => config.roomId = roomId);

    // listen event bus
    eb.AddListener("set.input.roomId", (string roomId) => {
      input.text = roomId;
      config.roomId = roomId;
    });
  }
}