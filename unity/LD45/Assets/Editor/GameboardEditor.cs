using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Gameboard))]
public class GameboardEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    Gameboard gameboard = (Gameboard)target;

    if (GUILayout.Button("Increment")) {
      gameboard.Value += 1;
    }

    if (GUILayout.Button("Bake Navigation")) {
      gameboard.BakeNavigation();
    }
  }
}
