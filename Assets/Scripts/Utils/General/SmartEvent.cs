using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DT.General {
  public interface ISmartEventListener {
    List<Func<bool>> conditions { get; set; }
  }

  public class AddListenerHandle {
    ISmartEventListener listener;

    public AddListenerHandle(ISmartEventListener listener) {
      this.listener = listener;
    }

    /// <summary>
    /// Remove the listener from the event when the condition is not met.
    /// </summary>
    public AddListenerHandle When(Func<bool> condition) {
      this.listener.conditions.Add(condition);
      return this;
    }

    /// <summary>
    /// Remove the listener from the event if obj is null.
    /// </summary>
    public AddListenerHandle Require(UnityEngine.Object obj) {
      return this.When(() => obj != null);
    }
  }

  /// <summary>
  /// Template of smart event.
  /// </summary>
  public class TSmartEvent<ActionType> where ActionType : class {
    protected class Listener : ISmartEventListener {
      public ActionType action;
      public List<Func<bool>> conditions { get; set; }
    }
    protected List<Listener> listeners;

    public TSmartEvent() {
      this.listeners = new List<Listener>();
    }

    public AddListenerHandle AddListener(ActionType action, params Func<bool>[] conditions) {
      Listener l = new Listener();
      l.action = action;
      l.conditions = new List<Func<bool>>(conditions);
      this.listeners.Add(l);
      return new AddListenerHandle(l);
    }

    public void RemoveListener(ActionType action) {
      this.listeners.RemoveAll((l) => l.action == action);
    }

    protected void InvokeActions(UnityAction<ActionType> callback) {
      foreach (var listener in this.listeners) {
        if (listener.conditions.TrueForAll((c) => c())) {
          callback.Invoke(listener.action);
        } else {
          // Remove listener if any of the conditions are not met.
          this.listeners.Remove(listener);
        }
      }
    }
  }

  /// <summary>
  /// Event that can auto remove listeners.
  /// </summary>
  public class SmartEvent : TSmartEvent<UnityAction> {
    public void Invoke() => this.InvokeActions((action) => action.Invoke());
  }
  /// <summary>
  /// Event that can auto remove listeners.
  /// </summary>
  public class SmartEvent<T0> : TSmartEvent<UnityAction<T0>> {
    public void Invoke(T0 arg) => this.InvokeActions((action) => action.Invoke(arg));
  }
  /// <summary>
  /// Event that can auto remove listeners.
  /// </summary>
  public class SmartEvent<T0, T1> : TSmartEvent<UnityAction<T0, T1>> {
    public void Invoke(T0 arg0, T1 arg1) => this.InvokeActions((action) => action.Invoke(arg0, arg1));
  }
  /// <summary>
  /// Event that can auto remove listeners.
  /// </summary>
  public class SmartEvent<T0, T1, T2> : TSmartEvent<UnityAction<T0, T1, T2>> {
    public void Invoke(T0 arg0, T1 arg1, T2 arg2) => this.InvokeActions((action) => action.Invoke(arg0, arg1, arg2));
  }
  /// <summary>
  /// Event that can auto remove listeners.
  /// </summary>
  public class SmartEvent<T0, T1, T2, T3> : TSmartEvent<UnityAction<T0, T1, T2, T3>> {
    public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) => this.InvokeActions((action) => action.Invoke(arg0, arg1, arg2, arg3));
  }
}
