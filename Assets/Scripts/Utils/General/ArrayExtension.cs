using System;
using System.Collections;
using System.Collections.Generic;

namespace DT.General {
  static class ArrayExtension {
    public static int BinarySearch<T>(this T[] array, T value, IComparer<T> comparer) {
      return Array.BinarySearch(array, value, comparer);
    }
    public static int BinarySearch<T>(this T[] array, T value) {
      return Array.BinarySearch(array, value);
    }
    public static int BinarySearch<T>(this T[] array, int index, int length, T value) {
      return Array.BinarySearch(array, index, length, value);
    }
    public static int BinarySearch(this Array array, object value, IComparer comparer) {
      return Array.BinarySearch(array, value, comparer);
    }
    public static int BinarySearch(this Array array, object value) {
      return Array.BinarySearch(array, value);
    }
    public static int BinarySearch(this Array array, int index, int length, object value, IComparer comparer) {
      return Array.BinarySearch(array, index, length, value, comparer);
    }
    public static int BinarySearch<T>(this T[] array, int index, int length, T value, IComparer<T> comparer) {
      return Array.BinarySearch<T>(array, index, length, value, comparer);
    }
    public static int BinarySearch(this Array array, int index, int length, object value) {
      return Array.BinarySearch(array, index, length, value);
    }

    public static bool Contains<T>(this T[] array, T value) {
      return Array.BinarySearch(array, value) >= 0;
    }
  }
}