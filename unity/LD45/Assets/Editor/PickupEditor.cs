using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    Pickup pickup = (Pickup)target;

    if (GUILayout.Button("Set points to 2")) {
      pickup.SetPoints(2);
    }
    if (GUILayout.Button("Set points to 5")) {
      pickup.SetPoints(5);
    }
    if (GUILayout.Button("Set points to 10")) {
      pickup.SetPoints(10);
    }
    if (GUILayout.Button("Set points to 20")) {
      pickup.SetPoints(20);
    }
    if (GUILayout.Button("Set points to 100")) {
      pickup.SetPoints(100);
    }
  }
}
