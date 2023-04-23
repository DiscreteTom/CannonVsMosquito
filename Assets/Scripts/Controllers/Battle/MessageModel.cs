
[System.Serializable]
public struct ServerMessage {
  public string type;
  public string msg;
}
[System.Serializable]
public struct ErrorEvent {
  public string type;
}
[System.Serializable]
public struct Origin {
  public float x;
  public float y;
}
[System.Serializable]
public struct Target {
  public float x;
  public float y;
  public int id;
}
[System.Serializable]
public struct GameStartEvent {
  public Target[] targets;
}
[System.Serializable]
public struct PlayerShootEvent {
  public int[] hit;
  public int player;
  public Origin origin;
  public float angle;
}
[System.Serializable]
public struct NewTargetEvent {
  public Target[] targets;
}
[System.Serializable]
public struct GameOverEvent {
  public int winner;
}
[System.Serializable]
public struct CreateRoomAction {
  public string action;
  public string room;
}
[System.Serializable]
public struct JoinRoomAction {
  public string action;
  public string room;
}
[System.Serializable]
public struct ShootAction {
  public string action;
  public Origin origin;
  public float angle;
}