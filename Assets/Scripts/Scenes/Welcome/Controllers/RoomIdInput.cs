using DT.UniStart;
using TMPro;

namespace Project.Scene.Welcome {
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
      eb.AddListener((SetInputRoomId e) => {
        input.text = e.roomId;
        config.roomId = e.roomId;
      });
    }
  }
}