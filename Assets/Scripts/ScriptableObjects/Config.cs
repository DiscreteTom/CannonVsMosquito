using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject {
  public string serverUrl;
  public string roomId;
  public int localPlayerId;
}