using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject {
  [Header("Server")]
  public string serverUrl;

  [Header("Game")]
  public string roomId;
  public int localPlayerId;
  public int timeout = 60;

  [Header("Cannon")]
  public float angleRange = 160f;
  public float cannonRotationSpeed = 180f;
}