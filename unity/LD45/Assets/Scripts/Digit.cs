using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour {
  public GameObject platformPrefab = null;
  public float pendingSeconds = 1.0f;
  public Color defaultColor;
  public Color pendingColor;
  public Color movingColor;

  private int value = 0;
  private int digitIndex = 0;
  private Platform[,] objects = new Platform[Heightmap.digitHeight, Heightmap.digitWidth];

  public void SetDigitIndex(int i) {
    digitIndex = i;
    if (i == 0) {
      SetValue(0, 0.6f);
    }
  }

  public void StartAnimateValue(int newValue, Gameboard gameboard) {
    StartCoroutine(AnimateValue(newValue, gameboard));
  }

  public Platform FindPickupPlatform() {
    for (int x = 0; x < Heightmap.digitWidth; x++) {
      for (int y = 0; y < Heightmap.digitHeight; y++) {
        Platform obj = objects[y, x];
        if (Random.Range(0.0f, 1.0f) > 0.7f) {
          return obj;
        }
      }
    }
    return null;
  }

  public void OnCellStateChanged(Heightmap.Cell cell) {
    // Debug.LogFormat("OnCellStateChanged {3} {0} {1},{2}", cell.State, cell.DigitX, cell.DigitY, cell.DigitIndex);
    Platform obj = objects[cell.DigitY, cell.DigitX];
    obj.OnCellStateChanged(cell);
  }

  private void Awake() {
    for (int x = 0; x < Heightmap.digitWidth; x++) {
      for (int y = 0; y < Heightmap.digitHeight; y++) {
        float height = -0.2f;
        Vector3 offset = new Vector3(Heightmap.digitWidth - x - 1, height, y);
        Platform obj = Instantiate<GameObject>(
          platformPrefab,
          transform.position + offset,
          Quaternion.identity,
          transform
        ).GetComponent<Platform>();
        obj.defaultColor = defaultColor;
        obj.pendingColor = pendingColor;
        obj.movingColor = movingColor;
        objects[y, x] = obj;
      }
    }
  }

  private void Update() {
    // Game.instance.Gameboard.Heightmap.Log();
    // Debug.LogFormat("digitIndex {0}", digitIndex);
    foreach (Heightmap.Cell h in Game.instance.Gameboard.Heightmap.DigitCells(digitIndex)) {
      Platform obj = objects[h.DigitY, h.DigitX];
      // Debug.LogFormat("Setting fixed platform height of {0},{1} (global: {4},{5}) to {2} (target: {3})", h.DigitX, h.DigitY, h.Height, h.Target, h.IndexX, h.IndexY);
      obj.SetHeight(h.Height);
    }
  }

  private void SetValue(int newValue, float height) {
    Game.instance.Gameboard.Heightmap.SetHeight(digitIndex, newValue, height);
  }

  private IEnumerator AnimateValue(int newValue, Gameboard gameboard) {
    newValue = newValue % 10;
    value = newValue;
    Game.instance.Gameboard.Heightmap.SetPending(digitIndex, newValue);
    yield return new WaitForSeconds(pendingSeconds);
    Game.instance.Gameboard.Heightmap.Grow(digitIndex, newValue);
  }

#if UNITY_EDITOR
  private Color gizmoColor_ = new Color(1.0f, 0.0f, 1.0f, 0.5f);

  public void OnDrawGizmosSelected() {
    Gizmos.color = gizmoColor_;
    Vector3 dimensions = new Vector3(Heightmap.digitWidth, 1, Heightmap.digitHeight);
    Gizmos.DrawCube(transform.position + dimensions / 2 + new Vector3(0, -0.9f, 0), dimensions);
  }
#endif
}