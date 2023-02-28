namespace DT.General {
  public class PanicManager {
    public static void Panic() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      UnityEngine.Application.Quit();
#endif
    }
  }
}