using System;
using System.Collections;
using UnityEngine;

namespace DT.General {
  public static class MonoBehaviourExtension {
    public static void Invoke(this MonoBehaviour mb, Action f, float delay) {
      static IEnumerator InvokeRoutine(System.Action f, float delay) {
        yield return new WaitForSeconds(delay);
        f();
      }
      mb.StartCoroutine(InvokeRoutine(f, delay));
    }
  }
}