using DT.UniStart;
using DT.UniUtils;
using TMPro;
using UnityEngine;

public class Player : CBC {
  [SerializeField] int playerId;

  void Start() {
    var eb = this.Get<IEventBus>();
    var config = this.Get<Config>();
    var model = this.Get<Model>();
    var cannon = this.transform.Find("Cannon");
    var animator = cannon.GetComponent<Animator>();
    var rotate = new Watch<bool>(true);
    var laserPower = cannon.Find("LaserPower");
    var laserPowerScale = laserPower.transform.localScale;
    var laserPowerSr = laserPower.GetComponent<SpriteRenderer>();
    var laserPowerInitLocalPos = laserPower.localPosition;
    laserPowerSr.enabled = false; // disable at start

    // if this player is not the local player, set color to light gray
    if (this.playerId != config.localPlayerId) {
      var cannonSr = cannon.GetComponent<SpriteRenderer>();
      cannonSr.color = Color.gray;
    }

    // update animator when rotate value changed
    rotate.AddListener(v => {
      animator.SetBool("rotating", rotate.Value);
    });

    // handle game start
    this.Watch(model.state, (GameState state) => {
      if (state != GameState.PLAYING) return;
      animator.SetBool("rotating", true); // start rotating

      bool clockwise = false;
      // start rotation within angle range
      this.onUpdate.AddListener(() => {
        if (!rotate.Value || model.state.Value != GameState.PLAYING) return;

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
        this.onUpdate.AddListener(() => {
          if (rotate.Value && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && model.state.Value == GameState.PLAYING) {
            // stop rotation until we got the server ack
            rotate.Value = false;

            // enable laser power
            laserPowerSr.enabled = true;
            laserPower.transform.localScale = Vector3.zero;

            var angle = cannon.localEulerAngles.z;
            eb.Invoke("local.shoot", this.transform.position.x, this.transform.position.y, (angle + 90) % 360, this.playerId);
          }
        });
      }

      // handle game over
      this.Watch(eb, "game.over", (GameOverEvent e) => {
        rotate.Value = false;
      });

      // mock player shoot
      var useMockServer = config.serverUrl == "";
      if (useMockServer && this.playerId != config.localPlayerId) {
        var timeout = config.mockPlayerShootInterval;
        this.onUpdate.AddListener(() => {
          timeout -= Time.deltaTime;
          if (timeout < 0 && rotate.Value) {
            var angle = cannon.localEulerAngles.z;
            eb.Invoke("local.shoot", this.transform.position.x, this.transform.position.y, (angle + 90) % 360, this.playerId);
            timeout = config.mockPlayerShootInterval;
          }
        });
      }
    });

    // update player score text
    var text = this.transform.Find("Canvas/ScoreText").GetComponent<TMP_Text>();
    var textScale = text.transform.localScale; // save initial scale
    this.Watch(model.scores, () => {
      text.text = model.scores[this.playerId].ToString();
    });
    this.Watch(eb, "game.shoot", (PlayerShootEvent e) => {
      if (e.player == this.playerId) {
        model.scores[this.playerId] += e.hit.Length;
        text.transform.localScale = textScale * config.scoreShakeScale;
        // calibrate shooter angle
        cannon.localEulerAngles = new Vector3(0, 0, e.angle - 90);
      }
    });
    // text shake
    this.onUpdate.AddListener(() => {
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
    this.Watch(eb, "game.shoot", (PlayerShootEvent e) => {
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
          rotate.Value = true;
        }, config.laserWidth / config.laserFadeSpeed); // clear laser points
      }
    });
    // laser fade
    this.onUpdate.AddListener(() => {
      if (lr.positionCount > 0 && lr.startWidth > 0) {
        lr.startWidth -= Time.deltaTime * config.laserFadeSpeed;
        lr.endWidth -= Time.deltaTime * config.laserFadeSpeed;
      }
    });

    // laser power
    this.onUpdate.AddListener(() => {
      if (laserPowerSr.enabled) {
        // grow
        if (laserPower.transform.localScale.x >= laserPowerScale.x) {
          laserPower.transform.localScale = laserPowerScale;
        }
        laserPower.transform.localScale += laserPowerScale * config.laserPowerGrowSpeed * Time.deltaTime;
      }
    });
    this.Watch(eb, "game.shoot", (PlayerShootEvent e) => {
      if (e.player == this.playerId) {
        // disable laser power
        laserPowerSr.enabled = false;
      }
    });
    this.Watch(eb, "game.over", (GameOverEvent e) => {
      laserPowerSr.enabled = false;
    });
  }
}