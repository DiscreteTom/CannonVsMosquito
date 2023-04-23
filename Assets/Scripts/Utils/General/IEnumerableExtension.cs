using System;
using System.Collections.Generic;

namespace DT.UniUtils {
  public static class IEnumerableExtension {
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action) {
      int i = 0;
      foreach (T item in enumerable) {
        action(item, i);
        i++;
      }
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
      enumerable.ForEach((T item, int i) => action(item));
    }

    public static R[] Map<T, R>(this IEnumerable<T> enumerable, Func<T, int, R> action) {
      List<R> list = new List<R>();
      int i = 0;
      foreach (T item in enumerable) {
        list.Add(action(item, i));
        i++;
      }
      return list.ToArray();
    }

    public static R[] Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> action) {
      return enumerable.Map((T item, int i) => action(item));
    }

    public static T[] Shuffle<T>(this IEnumerable<T> enumerable) {
      List<T> list = new List<T>(enumerable);
      for (int i = 0; i < list.Count; i++) {
        int j = UnityEngine.Random.Range(i, list.Count);
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
      }
      return list.ToArray();
    }
  }
}