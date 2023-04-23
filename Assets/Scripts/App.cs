using DT.UniStart;
using UnityEngine;

public class App : Entry {
  [SerializeField] Config config;

  void Awake() {
    this.Add(this.config);
    this.Add<IEventBus>(new DebugEventBus());
  }
}