using UnityEngine;
using UnityEngine.Events;

namespace DT.General {
  public class Debouncer {
    float debounceTimeMs;
    float debounceTimeLeft;
    bool debouncing;
    UnityAction onDebounceFinished;

    public Debouncer(float debounceTimeMs, UnityAction onDebounceFinished) {
      this.debounceTimeMs = debounceTimeMs;
      this.onDebounceFinished = onDebounceFinished;
    }

    public void Update(float deltaTime) {
      // check debounce
      if (!this.debouncing) {
        // start debounce
        this.debouncing = true;
        this.debounceTimeLeft = this.debounceTimeMs;
      } else {
        // debounce
        this.debounceTimeLeft -= deltaTime * 1000f;
        if (this.debounceTimeLeft <= 0f) {
          // debounce finished
          this.onDebounceFinished?.Invoke();
          // cancel debounce
          this.debouncing = false;
        }
      }
    }

    public void Update() {
      this.Update(Time.deltaTime);
    }

    public void Cancel() {
      this.debouncing = false;
    }
  }
}