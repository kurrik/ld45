using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour {
  private const int letterWidth = 5;
  private const int letterHeight = 9;

  public GameObject platformPrefab = null;
  public float pendingSeconds = 1.0f;
  public float transitionSeconds = 0.5f;
  public float bumpIncrement = 0.2f;
  public float descendDelay = 5f;
  public float descendRate = 0.05f;
  public Color defaultColor;
  public Color pendingColor;
  public Color movingColor;
  public bool useNewHeightmap = true;

  private int value = 0;
  private int digitIndex = 0;
  private float descendDelayElapsed = 0.0f;
  private Platform[,] objects = new Platform[Heightmap.digitHeight, Heightmap.digitWidth];

  public void SetDigitIndex(int i) {
    digitIndex = i;
    if (i == 0) {
      SetValue(0, bumpIncrement * 3);
    }
  }

  public void StartAnimateValue(int newValue, Gameboard gameboard) {
    StartCoroutine(AnimateValue(newValue, gameboard));
  }

  public Platform FindPickupPlatform() {
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        Platform obj = objects[y, x];
        if (Random.Range(0.0f, 1.0f) > 0.7f) {
          return obj;
        }
      }
    }
    return null;
  }

  private void Awake() {
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        float height = -0.2f;
        Vector3 offset = new Vector3(letterWidth - x - 1, height, y);
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
    if (useNewHeightmap) {
      Game.instance.Gameboard.Heightmap.Log();
      Debug.LogFormat("digitIndex {0}", digitIndex);
      foreach (Heightmap.Cell h in Game.instance.Gameboard.Heightmap.DigitCells(digitIndex)) {
        Platform obj = objects[h.DigitY, h.DigitX];
        Debug.LogFormat("Setting fixed platform height of {0},{1} (global: {4},{5}) to {2} (target: {3})", h.DigitX, h.DigitY, h.Height, h.Target, h.IndexX, h.IndexY);
        obj.SetHeight(h.Height);
      }
    }
  }

  private void FixedUpdate() {
    if (!useNewHeightmap) {
      if (descendDelayElapsed < descendDelay) {
        descendDelayElapsed += Time.fixedDeltaTime;
        return;
      }
      for (int x = 0; x < letterWidth; x++) {
        for (int y = 0; y < letterHeight; y++) {
          Platform obj = objects[y, x];
          obj.Adjust(-descendRate * Time.fixedDeltaTime);
        }
      }
    }
  }

  private void SetValue(int newValue, float height) {
    Game.instance.Gameboard.Heightmap.SetHeight(digitIndex, newValue, height);
    // New heightmap code
    if (useNewHeightmap) {
      Debug.LogFormat("digitIndex {0}", digitIndex);
      foreach (Heightmap.Cell h in Game.instance.Gameboard.Heightmap.DigitCells(digitIndex)) {
        Platform obj = objects[h.DigitY, h.DigitX];
        Debug.LogFormat("Setting value platform height of {0},{1} (global: {4},{5}) to {2} (target: {3})", h.DigitX, h.DigitY, h.Height, h.Target, h.IndexX, h.IndexY);
        obj.SetHeight(h.Height);
      }
    } else {
      // Old code -- delete
      for (int x = 0; x < letterWidth; x++) {
        for (int y = 0; y < letterHeight; y++) {
          if (Masks.Digits[newValue, y, x] == 1) {
            Platform obj = objects[y, x];
            obj.Adjust(height);
          }
        }
      }
    }
  }

  private IEnumerator AnimateValue(int newValue, Gameboard gameboard) {
    newValue = newValue % 10;

    // New heightmap code
    Game.instance.Gameboard.Heightmap.Grow(digitIndex, newValue);

    // Old heightmap code
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        if (Masks.Digits[newValue, y, x] == 1) {
          Platform obj = objects[y, x];
          obj.SetPending();
        }
      }
    }
    yield return new WaitForSeconds(pendingSeconds);
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        if (Masks.Digits[newValue, y, x] == 1) {
          Platform obj = objects[y, x];
          obj.SetBoost(bumpIncrement);
        }
      }
    }
    value = newValue;
  }

#if UNITY_EDITOR
  private Color gizmoColor_ = new Color(1.0f, 0.0f, 1.0f, 0.5f);

  public void OnDrawGizmosSelected() {
    Gizmos.color = gizmoColor_;
    Vector3 dimensions = new Vector3(letterWidth, 1, letterHeight);
    Gizmos.DrawCube(transform.position + dimensions / 2 + new Vector3(0, -0.9f, 0), dimensions);
  }
#endif
}