using DT.General;
using TMPro;
using UnityEngine;

public class Player : CBC {
  [SerializeField] int playerId;

  void Start() {
    var eb = this.Get<EventBus>();
    var config = this.Get<Config>();
    var cannon = this.transform.Find("Cannon");
    var animator = cannon.GetComponent<Animator>();
    bool rotate = true;

    eb.AddListener("game.start", (GameStartEvent _) => {
      animator.SetBool("rotating", rotate);

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
            animator.SetBool("rotating", rotate);

            var angle = cannon.localEulerAngles.z;
            eb.Invoke("local.shoot", this.transform.position.x, this.transform.position.y, (angle + 90) % 360);
          }
        });
      }

      // handle game over
      eb.AddListener("game.over", (GameOverEvent e) => {
        rotate = false;
        animator.SetBool("rotating", rotate);
      });
    });

    // update player score text
    var text = this.transform.Find("Canvas/ScoreText").GetComponent<TMP_Text>();
    var score = 0;
    var textScale = text.transform.localScale; // save initial scale
    eb.AddListener("game.shoot", (PlayerShootEvent e) => {
      if (e.player == this.playerId) {
        score += e.hit.Length;
        text.text = score.ToString();
        text.transform.localScale = textScale * config.scoreShakeScale;
      }
    });
    // text shake
    this.OnUpdate.AddListener(() => {
      if (text.transform.localScale.x > textScale.x) {
        text.transform.localScale -= textScale * config.scoreShakeSpeed * Time.deltaTime;
      } else {
        text.transform.localScale = textScale;
      }
    });

    // draw laser
    var lr = this.gameObject.AddComponent<LineRenderer>();
    lr.material = config.laserMaterial;
    lr.sortingOrder = -1;
    eb.AddListener("game.shoot", (PlayerShootEvent e) => {
      if (e.player == this.playerId) {
        lr.positionCount = 2;
        lr.SetPosition(0, this.transform.position);
        lr.SetPosition(1, new Vector3(this.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * e.angle) * 100, this.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * e.angle) * 100, 0));
        lr.startWidth = config.laserWidth;
        lr.endWidth = config.laserWidth;
        lr.startColor = Color.white;
        lr.endColor = Color.white;

        this.Invoke(() => {
          lr.positionCount = 0;
          // start rotation again
          rotate = true;
          animator.SetBool("rotating", rotate);
        }, config.laserWidth / config.laserFadeSpeed); // clear laser points
      }
    });
    // laser fade
    this.OnUpdate.AddListener(() => {
      if (lr.positionCount > 0 && lr.startWidth > 0) {
        lr.startWidth -= Time.deltaTime * config.laserFadeSpeed;
        lr.endWidth -= Time.deltaTime * config.laserFadeSpeed;
      }
    });
  }
}