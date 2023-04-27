using DT.UniStart;

public enum GameState {
  IDLE,
  PLAYING,
  OVER,
}

public class Model {
  public Watch<GameState> state { get; private set; } = new Watch<GameState>(GameState.IDLE);
  public WatchArray<int> scores { get; private set; } = new WatchArray<int>(2);
}