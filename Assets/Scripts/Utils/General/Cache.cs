using System;
using UnityEngine;

namespace DT.General {
  /// <summary>
  /// Store a cached value, and only update the value when the condition is met.
  /// </summary>
  public class ConditionalCache<T> {
    T value;
    protected Func<T> factory;
    protected Func<T, bool> condition;

    public ConditionalCache(Func<T> factory, Func<T, bool> condition) {
      this.factory = factory;
      this.condition = condition;
    }

    public T Value {
      get {
        if (this.condition.Invoke(this.value)) this.Refresh();
        return this.value;
      }
    }

    /// <summary>
    /// Manually refresh the value.
    /// </summary>
    public void Refresh() {
      this.value = this.factory.Invoke();
    }
  }

  /// <summary>
  /// Store a cached value, and only update the value when new frame is reached.
  /// </summary>
  public class FrameBasedCache<T> : ConditionalCache<T> {
    int lastFrame = -1;

    public FrameBasedCache(Func<T> factory) : base(factory, null) {
      this.condition = (value) => {
        var result = Time.frameCount != this.lastFrame;
        if (result) this.lastFrame = Time.frameCount;
        return result;
      };
    }
  }
}
