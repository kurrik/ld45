﻿using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Gameboard : MonoBehaviour {
  public static  int PlatformLayer = 9;

  private int digitsCreated = 0;
  private int value = 0;
  private int bakeTicks = 0;

  public Heightmap Heightmap;
  public NavMeshSurface surface;
  public GameObject pickupPrefab;
  public GameObject platformPrefab;
  public GameObject platformParent;
  public float pendingSeconds = 1.0f;

  public Color defaultColor = new Color(0.53f, 0.53f, 0.53f);
  public Color pendingColor = new Color(1.0f, 0.93f, .41f);
  public Color movingColor = new Color(1.0f, .38f, 0.0f);

  private Platform[,] platforms = new Platform[Heightmap.unitsHigh, Heightmap.unitsWide];

  public int Value {
    get => value;
    set => SetValue(value);
  }

  // Public for editor UI.
  public void BakeNavigation() {
    bakeTicks += 1;
    if (bakeTicks > 2) {
      surface.BuildNavMesh();
      bakeTicks = 0;
    }
  }

  public void OnPickupTouched(Pickup pickup) {
    Value += 1;
    Destroy(pickup.gameObject);
    SpawnPickup();
  }

  public void OnCellStateChanged(Heightmap.Cell cell) {
    Platform obj = platforms[cell.IndexY, cell.IndexX];
    if (obj) {
      obj.OnCellStateChanged(cell);
    }
  }

  private void Start() {
    Heightmap.AddStateListener(OnCellStateChanged);
    CreatePlatforms(0);
    digitsCreated = 1;
    Heightmap.SetHeight(0, 0, 0.6f);
    SpawnPickup();
  }

  private void Update() {
    BakeNavigation();
    for (int i = 0; i < digitsCreated; i++) {
      foreach (Heightmap.Cell h in Heightmap.DigitCells(i)) {
        Platform platform = platforms[h.IndexY, h.IndexX];
        if (platform) {
          // Debug.LogFormat("Setting fixed platform height of {0},{1} (global: {4},{5}) to {2} (target: {3})", h.DigitX, h.DigitY, h.Height, h.Target, h.IndexX, h.IndexY);
          platform.SetHeight(h.Height);
        }
      }
    }
  }

  private void FixedUpdate() {
    Heightmap.Tick(Time.fixedDeltaTime);
  }

  private void SpawnPickup() {
    foreach (Heightmap.Cell h in Game.instance.Gameboard.Heightmap.DigitCells(0)) {
      if (Random.Range(0.0f, 1.0f) > 0.7f) {
        Platform obj = platforms[h.IndexY, h.IndexX];
        if (obj) {
          obj.SpawnPickup(pickupPrefab);
          return;
        }
      }
    }
  }

  private void SetValue(int newValue) {
    int digit = 0;
    int v = newValue;
    while (v > 0) {
      int remainder = v % 10;
      if (digit >= digitsCreated) {
        CreatePlatforms(digit);
        digitsCreated++;
      }
      StartCoroutine(AnimateValue(digit, remainder));
      v = v / 10;
      digit += 1;
    }
    value = newValue;
    Debug.LogFormat("Value {0}", newValue);
  }

  private void CreatePlatforms(int digit) {
    // 0,0 is bottom right hand corner of platforms so we can expand left.
    for (int x = 0; x < Heightmap.digitWidth; x++) {
      for (int y = 0; y < Heightmap.digitHeight; y++) {
        Vector3 offset = new Vector3(
          -Heightmap.digitWidth * (digitsCreated + 1) + x, 
          -0.2f,
          Heightmap.digitHeight - y - 1
        );
        Platform obj = Instantiate<GameObject>(
          platformPrefab,
          platformParent.transform.position + offset,
          Quaternion.identity,
          platformParent.transform
        ).GetComponent<Platform>();
        Heightmap.GetHeightmapCoordinates(digit, x, y, out obj.HeightmapX, out obj.HeightmapY);
        obj.gameObject.layer = Gameboard.PlatformLayer;
        obj.defaultColor = defaultColor;
        obj.pendingColor = pendingColor;
        obj.movingColor = movingColor;
        platforms[obj.HeightmapY, obj.HeightmapX] = obj;
      }
    }
  }

  private IEnumerator AnimateValue(int digit, int remainder) {
    remainder = remainder % 10;
    Heightmap.SetPending(digit, remainder);
    yield return new WaitForSeconds(pendingSeconds);
    Heightmap.Grow(digit, remainder);
  }

#if UNITY_EDITOR
  private Color gizmoColor_ = new Color(1.0f, 0.0f, 1.0f, 0.5f);

  public void OnDrawGizmosSelected() {
    Gizmos.color = gizmoColor_;
    Vector3 dimensions = new Vector3(Heightmap.digitWidth, 1, Heightmap.digitHeight);
    Gizmos.DrawCube(transform.position + dimensions / 2 + new Vector3(-Heightmap.digitWidth, -0.9f, 0), dimensions);
  }
#endif
}