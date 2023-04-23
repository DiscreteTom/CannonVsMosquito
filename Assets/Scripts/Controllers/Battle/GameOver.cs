using DT.UniStart;
using TMPro;

public class GameOver : CBC {
  void Start() {
    var eb = this.Get<EventBus>();

    eb.AddListener("game.over", (GameOverEvent e) => {
      var textTransform = this.transform.transform.Find("GameOverText");
      textTransform.gameObject.SetActive(true);
      textTransform.GetComponent<TMP_Text>().text = "Player " + (e.winner + 1) + " wins";
      this.transform.Find("BackButton").gameObject.SetActive(true);
    });
  }
}