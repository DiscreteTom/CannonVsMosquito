using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject {
  [SerializeField] string serverUrl;
  [SerializeField] string roomId;
  [SerializeField] int playerId;
}