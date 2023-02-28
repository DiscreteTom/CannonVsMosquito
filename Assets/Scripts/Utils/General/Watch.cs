using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;

namespace DT.General {
  public interface IWatchable {
    void AddListener(UnityAction f);
    void RemoveListener(UnityAction f);
  }

  /// <summary>
  /// Watch a **value** type for changes.
  /// This class should be used for immutable types (int, float, bool, string, etc) only.
  /// </summary>
  public class Watch<T> : IWatchable {
    protected T value;
    UnityEvent onChange0;
    UnityEvent<T> onChange1;
    UnityEvent<T, T> onChange2;

    public Watch(T value) {
      this.value = value;
      this.onChange0 = new UnityEvent();
      this.onChange1 = new UnityEvent<T>();
      this.onChange2 = new UnityEvent<T, T>();
    }

    public T Value {
      get => this.value;
      set {
        var previous = this.value;
        this.value = value;
        this.onChange0.Invoke();
        this.onChange1.Invoke(value);
        this.onChange2.Invoke(value, previous);
      }
    }

    /// <summary>
    /// Add a listener that will be called when the value changes.
    /// </summary>
    public void AddListener(UnityAction f) => this.onChange0.AddListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes, and invoke it immediately.
    /// </summary>
    public void AddListenerInvoke(UnityAction f) {
      this.onChange0.AddListener(f);
      f.Invoke();
    }
    public void RemoveListener(UnityAction f) => this.onChange0.RemoveListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes.
    /// The parameter is the new value.
    /// </summary>
    public void AddListener(UnityAction<T> f) => this.onChange1.AddListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes, and invoke it immediately.
    /// The parameter is the new value.
    /// </summary>
    public void AddListenerInvoke(UnityAction<T> f) {
      this.onChange1.AddListener(f);
      f.Invoke(this.value);
    }
    public void RemoveListener(UnityAction<T> f) => this.onChange1.RemoveListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes.
    /// The parameter is the new value and the previous value.
    /// </summary>
    public void AddListener(UnityAction<T, T> f) => this.onChange2.AddListener(f);
    public void RemoveListener(UnityAction<T, T> f) => this.onChange2.RemoveListener(f);
  }

  /// <summary>
  /// Watch a **reference** type for changes.
  /// </summary>
  public class WatchRef<T> : IWatchable {
    protected T value;
    UnityEvent onChange0;
    UnityEvent<WatchRef<T>> onChange;

    public WatchRef(T value) {
      this.value = value;
      this.onChange0 = new UnityEvent();
      this.onChange = new UnityEvent<WatchRef<T>>();
    }

    /// <summary>
    /// Set the value and trigger the onChange event.
    /// </summary>
    public void SetValue(T value) {
      this.value = value;
      this.InvokeEvent();
    }

    /// <summary>
    /// Apply a function to the value and trigger the onChange event.
    /// </summary>
    public void Apply(UnityAction<T> f) {
      f(this.value);
      this.InvokeEvent();
    }

    /// <summary>
    /// Apply a function to the value and trigger the onChange event.
    /// </summary>
    public R Apply<R>(Func<T, R> f) {
      var result = f(this.value);
      this.InvokeEvent();
      return result;
    }

    /// <summary>
    /// Apply a function to the value without trigger the onChange event.
    /// </summary>
    public void ReadOnlyApply(UnityAction<T> f) {
      f(this.value);
    }

    /// <summary>
    /// Apply a function to the value without trigger the onChange event.
    /// </summary>
    public R ReadOnlyApply<R>(Func<T, R> f) {
      return f(this.value);
    }

    /// <summary>
    /// Add a listener that will be called when the value changes.
    /// </summary>
    public void AddListener(UnityAction<WatchRef<T>> f) => this.onChange.AddListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes, and invoke it immediately.
    /// </summary>
    public void AddListenerInvoke(UnityAction<WatchRef<T>> f) {
      this.onChange.AddListener(f);
      f.Invoke(this);
    }
    public void RemoveListener(UnityAction<WatchRef<T>> f) => this.onChange.RemoveListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes.
    /// </summary>
    public void AddListener(UnityAction f) => this.onChange0.AddListener(f);
    /// <summary>
    /// Add a listener that will be called when the value changes, and invoke it immediately.
    /// </summary>
    public void AddListenerInvoke(UnityAction f) {
      this.onChange0.AddListener(f);
      f.Invoke();
    }
    public void RemoveListener(UnityAction f) => this.onChange0.RemoveListener(f);

    /// <summary>
    /// Invoke all events.
    /// </summary>
    protected void InvokeEvent() {
      this.onChange0.Invoke();
      this.onChange.Invoke(this);
    }
  }

  /// <summary>
  /// Immediately calculate a value when a watchable changes.
  /// The result should be immutable.
  /// </summary>
  public class Computed<T> : Watch<T> {
    Func<T> compute;

    public new T Value {
      get => this.value;
    }

    public Computed(Func<T> compute) : base(compute()) {
      this.compute = compute;
      this.value = this.compute();
    }

    /// <summary>
    /// Compute the value when a watchable changes.
    /// </summary>
    public Computed<T> Watch(IWatchable target) {
      target.AddListener(this.Update);
      return this;
    }

    void Update() {
      // use parent class's value setter to trigger events
      base.Value = this.compute();
    }
  }

  /// <summary>
  /// Auto calculate a value when the value is used after a watchable changes.
  /// The result should be immutable.
  /// </summary>
  public class LazyComputed<T> {
    T value;
    bool needUpdate = true;
    Func<T> compute { get; set; }

    public T Value {
      get {
        if (this.needUpdate) {
          this.value = this.compute();
          this.needUpdate = false;
        }
        return this.value;
      }
    }

    public LazyComputed(Func<T> compute) {
      this.compute = compute;
      this.needUpdate = true;
    }

    /// <summary>
    /// Mark current value as dirty when a watchable changes.
    /// </summary>
    public LazyComputed<T> Watch(IWatchable target) {
      target.AddListener(this.Update);
      return this;
    }

    void Update() {
      this.needUpdate = true;
    }
  }

  /// <summary>
  /// Watch a list-like type for changes.
  /// </summary>
  public class WatchIList<L, T> : WatchRef<L>, IList<T> where L : IList<T> {
    LazyComputed<ReadOnlyCollection<T>> readOnlyList;

    public WatchIList(L value) : base(value) {
      this.readOnlyList = new LazyComputed<ReadOnlyCollection<T>>(() => new ReadOnlyCollection<T>(this.value)).Watch(this);
    }

    /// <summary>
    /// Get the list as a read-only list and cache it for future calls.
    /// </summary>
    public ReadOnlyCollection<T> Value => this.readOnlyList.Value;

    #region re-expose methods from the list interface
    public void Add(T item) {
      this.value.Add(item);
      this.InvokeEvent();
    }
    public void Clear() {
      this.value.Clear();
      this.InvokeEvent();
    }
    public bool Contains(T item) => this.value.Contains(item);
    public bool Remove(T item) {
      var result = this.value.Remove(item);
      this.InvokeEvent();
      return result;
    }
    public int Count => this.value.Count;
    public bool IsReadOnly => this.value.IsReadOnly;
    public T this[int index] {
      get => this.value[index];
      set {
        this.value[index] = value;
        this.InvokeEvent();
      }
    }
    public int IndexOf(T item) => this.value.IndexOf(item);
    public void Insert(int index, T item) {
      this.value.Insert(index, item);
      this.InvokeEvent();
    }
    public void RemoveAt(int index) {
      this.value.RemoveAt(index);
      this.InvokeEvent();
    }
    public void CopyTo(T[] array, int arrayIndex) {
      this.value.CopyTo(array, arrayIndex);
    }
    public IEnumerator<T> GetEnumerator() {
      return this.value.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return this.value.GetEnumerator();
    }
    #endregion
  }
  /// <summary>
  /// Watch a dictionary-like type for changes.
  /// </summary>
  public class WatchIDictionary<D, K, V> : WatchRef<D>, IDictionary<K, V> where D : IDictionary<K, V> {
    LazyComputed<ReadOnlyDictionary<K, V>> readOnlyDictionary;

    public WatchIDictionary(D value) : base(value) {
      this.readOnlyDictionary = new LazyComputed<ReadOnlyDictionary<K, V>>(() => new ReadOnlyDictionary<K, V>(this.value)).Watch(this);
    }

    /// <summary>
    /// Get the dictionary as a read-only dictionary and cache it for future calls.
    /// </summary>
    public ReadOnlyDictionary<K, V> Value => this.readOnlyDictionary.Value;

    #region re-expose methods from the dictionary interface
    public void Add(K key, V value) {
      this.value.Add(key, value);
      this.InvokeEvent();
    }
    public void Clear() {
      this.value.Clear();
      this.InvokeEvent();
    }
    public bool ContainsKey(K key) => this.value.ContainsKey(key);
    public bool Remove(K key) {
      var result = this.value.Remove(key);
      this.InvokeEvent();
      return result;
    }
    public int Count => this.value.Count;
    public V this[K key] {
      get => this.value[key];
      set {
        this.value[key] = value;
        this.InvokeEvent();
      }
    }
    public bool TryGetValue(K key, out V value) => this.value.TryGetValue(key, out value);
    public void Add(KeyValuePair<K, V> item) {
      this.value.Add(item);
      this.InvokeEvent();
    }

    public bool Contains(KeyValuePair<K, V> item) {
      return this.value.Contains(item);
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
      this.value.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<K, V> item) {
      var result = this.value.Remove(item);
      this.InvokeEvent();
      return result;
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
      return this.value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this.value.GetEnumerator();
    }

    public ICollection<K> Keys => this.value.Keys;
    public ICollection<V> Values => this.value.Values;
    public bool IsReadOnly => this.value.IsReadOnly;
    #endregion
  }

  /// <summary>
  /// Watch a list for changes.
  /// </summary>
  public class WatchList<T> : WatchIList<List<T>, T> {
    public WatchList() : base(new List<T>()) { }
    public WatchList(List<T> value) : base(value) { }

    // re-expose methods from the list
    public int BinarySearch(T item) => this.value.BinarySearch(item);
    public int BinarySearch(T item, IComparer<T> comparer) => this.value.BinarySearch(item, comparer);
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => this.value.BinarySearch(index, count, item, comparer);
  }
  /// <summary>
  /// Watch an array for changes.
  /// </summary>
  public class WatchArray<T> : WatchIList<T[], T> {
    public WatchArray(int n) : base(new T[n]) { }
    public WatchArray(T[] value) : base(value) { }

    // re-expose methods from the array
    public int BinarySearch(T item) => Array.BinarySearch(this.value, item);
    public int BinarySearch(T item, IComparer<T> comparer) => Array.BinarySearch(this.value, item, comparer);
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => Array.BinarySearch(this.value, index, count, item, comparer);
  }
  /// <summary>
  /// Watch a dictionary for changes.
  /// </summary>
  public class WatchDictionary<K, V> : WatchIDictionary<Dictionary<K, V>, K, V> {
    public WatchDictionary() : base(new Dictionary<K, V>()) { }
    public WatchDictionary(Dictionary<K, V> value) : base(value) { }
  }
}