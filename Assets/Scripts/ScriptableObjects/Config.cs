using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject {
  [Header("Server")]
  public string serverUrl;
  public float mockServerLatency = 0.2f;

  [Header("Game")]
  public string roomId;
  public int localPlayerId;
  public int timeout = 60;
  public float laserWidth = 0.6f;
  public int initTargetCount = 5;

  [Header("Cannon")]
  public float angleRange = 160f;
  public float cannonRotationSpeed = 180f;

  [Header("Prefabs")]
  public GameObject targetPrefab;
}