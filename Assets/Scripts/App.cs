using DT.General;
using UnityEngine;

public class App : Entry {
  [SerializeField] Config config;

  void Awake() {
    this.Add(this.config);
    this.Add<EventBus>();
  }
}