using DT.General;
using TMPro;
using UnityEngine;

public class Player : CBC {
  [SerializeField] int playerId;

  void Start() {
    var eb = this.Get<EventBus>();
    var config = this.Get<Config>();
    var cannon = this.transform.Find("Cannon");

    eb.AddListener("game.start", (GameStartEvent _) => {
      bool rotate = true;
      bool clockwise = false;
      // start rotation within angle range
      this.OnUpdate.AddListener(() => {
        if (!rotate) return;

        if (clockwise) {
          cannon.Rotate(0, 0, -config.cannonRotationSpeed * Time.deltaTime);
        } else {
          cannon.Rotate(0, 0, config.cannonRotationSpeed * Time.deltaTime);
        }
        var angle = cannon.localEulerAngles.z;
        if (!clockwise && angle < 180 && angle > config.angleRange / 2) {
          clockwise = true;
        } else if (clockwise && angle > 180 && angle < -config.angleRange / 2 + 360) {
          clockwise = false;
        }
      });

      // shoot if this player is the local player and when space is pressed
      if (this.playerId == config.localPlayerId) {
        this.OnUpdate.AddListener(() => {
          if (rotate && Input.GetKeyDown(KeyCode.Space)) {
            // stop rotation until we got the server ack
            rotate = false;

            var angle = cannon.localEulerAngles.z;
            eb.Invoke("local.shoot", this.transform.position.x, this.transform.position.y, (angle + 90) % 360);
          }
        });

        eb.AddListener("game.shoot", (PlayerShootEvent e) => {
          if (e.player == this.playerId) {
            // start rotation again
            rotate = true;
          }
        });
      }
    });

    // update player score text
    var text = this.transform.Find("Canvas/ScoreText").GetComponent<TMP_Text>();
    var score = 0;
    eb.AddListener("game.shoot", (PlayerShootEvent e) => {
      if (e.player == this.playerId) {
        score += e.hit.Length;
        text.text = score.ToString();
      }
    });
  }
}