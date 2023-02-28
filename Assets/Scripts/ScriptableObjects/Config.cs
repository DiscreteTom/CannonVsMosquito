using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject {
  public string serverUrl;
  public string roomId;
  public int localPlayerId;
  public int timeout = 60;
  public float angleRange = 160f;
  public float cannonRotationSpeed = 180f;
}