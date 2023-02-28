using System;

namespace DT.General {
  /// <summary>
  /// Lazy will create the value if the condition is met.
  /// </summary>
  public class Lazy<T> {
    T value;
    Func<T> factory;
    Func<T, bool> condition;

    public T Value {
      get {
        if (this.condition.Invoke(this.value)) {
          this.value = this.factory.Invoke();
        }
        return this.value;
      }
      set => this.value = value;
    }

    public T RawValue => this.value;

    public Lazy(Func<T> factory, Func<T, bool> condition, T initialValue = default(T)) {
      this.value = initialValue;
      this.factory = factory;
      this.condition = condition;
    }
  }

  /// <summary>
  /// LazyRef will create the value if the current value is null.
  /// </summary>
  public class LazyRef<T> : Lazy<T> where T : class {
    public LazyRef(Func<T> factory, T initialValue = null) : base(factory, (v) => v == null, initialValue) { }
  }

  /// <summary>
  /// LazyNew will new the object if the current value is null.
  /// </summary>
  public class LazyNew<T> : LazyRef<T> where T : class, new() {
    public LazyNew(T initialValue = null) : base(() => new T(), initialValue) { }
  }
}