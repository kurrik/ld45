using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    PlayerController playerController = (PlayerController)target;

    if (playerController.pointsCollectedPrefab) {
      if (GUILayout.Button("Spawn PointsCollected")) {
        playerController.SpawnPoints(Random.Range(1, 10));
      }
    }
  }
}
