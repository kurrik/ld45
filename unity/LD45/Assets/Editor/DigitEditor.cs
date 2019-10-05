using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Digit))]
public class DigitEditor : Editor {
  public override void OnInspectorGUI() { 
    DrawDefaultInspector();

    Digit myScript = (Digit)target;
    if (GUILayout.Button("Increment")) {
      myScript.Value += 1;
    }
  }
}
