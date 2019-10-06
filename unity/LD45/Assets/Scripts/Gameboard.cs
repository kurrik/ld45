using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gameboard : MonoBehaviour {
  public static  int PlatformLayer = 9;

  private int digitsCreated = 0;
  private int value = 0;
  private Vector3 playerOffset = new Vector3(0.5f, 0.5f, 0.5f);

  public Heightmap heightmap;
  public Navigation navigation;
  public PlayerController player;
  public GameObject pickupPrefab;
  public GameObject platformPrefab;
  public GameObject platformParent;
  public float pendingSeconds = 1.0f;
  public float spawnDelaySeconds = 0.8f;

  private Platform[,] platforms = new Platform[Heightmap.unitsHigh, Heightmap.unitsWide];
  private Platform destinationPlatform;
  private Platform playerPlatform;

  public int Value {
    get => value;
    set => SetValue(value);
  }

  public void OnPickupTouched(Pickup pickup) {
    Value += pickup.points;
    Destroy(pickup.gameObject);
    player.SpawnPoints(pickup.points);
    SpawnPickups(1);
  }

  public void OnPickupDisabled(Pickup pickup) {
    Debug.Log("Pickup was disabled!");
    Destroy(pickup.gameObject);
    SpawnPickups(1);
  }

  public void OnCellStateChanged(Heightmap.Cell cell) {
    Platform obj = platforms[cell.IndexY, cell.IndexX];
    if (obj) {
      obj.OnCellStateChanged(cell);
    }
  }

  public void OnDestinationSet(Platform platform) {
    if (destinationPlatform) {
      destinationPlatform.SetDestination(false);
    }
    destinationPlatform = platform;
    destinationPlatform.SetDestination(true);
    CheckPath();
  }

  public void OnPlayerOnPlatform(Platform platform) {
    if (playerPlatform) {
      playerPlatform.SetPlayerPlatform(false);
    }
    playerPlatform = platform;
    playerPlatform.SetPlayerPlatform(true);
    CheckPath();
  }

  public void CheckPath() {
    if (playerPlatform && destinationPlatform) {
      Vector2Int[] points;
      if (navigation.GetPoints(playerPlatform, destinationPlatform, out points)) {
        player.SetPath(points);
      }
    }
  }

  public Vector3 HeightmapCoordToWorldCoord(Vector2Int hc) {
    Platform platform = platforms[hc.y, hc.x];
    return platform.transform.position + playerOffset;
  }

  private void Awake() {
    player.AddDestinationSetListener(OnDestinationSet);
    player.AddPlayerOnPlatformListener(OnPlayerOnPlatform);
    heightmap.AddStateListener(OnCellStateChanged);
  }

  private void Start() {
    CreatePlatforms(0);
    digitsCreated = 1;
    heightmap.SetHeight(0, 0, 0.6f);
    SpawnPickups(2);
    player.transform.position = new Vector3(-0.5f, 1.0f, 0.5f);
  }

  private void Update() {
    for (int i = 0; i < digitsCreated; i++) {
      foreach (Heightmap.Cell h in heightmap.DigitCells(i)) {
        Platform platform = platforms[h.IndexY, h.IndexX];
        if (platform) {
          // Debug.LogFormat("Setting fixed platform height of {0},{1} (global: {4},{5}) to {2} (target: {3})", h.DigitX, h.DigitY, h.Height, h.Target, h.IndexX, h.IndexY);
          platform.SetHeight(h.Height);
        }
      }
    }
  }

  private void FixedUpdate() {
    heightmap.Tick(Time.fixedDeltaTime);
  }

  private void SpawnPickups(int count) {
    StartCoroutine(DelaySpawnPickups(count));
  }

  private IEnumerator DelaySpawnPickups(int count) {
    List<Heightmap.Cell> cells = heightmap.SpawnableCells().ToList<Heightmap.Cell>();
    int createdCount = 0;
    for (int i = 0; i < cells.Count; i++) {
      if (createdCount >= count) {
        yield break;
      }
      yield return new WaitForSeconds(spawnDelaySeconds);
      int randomIndex = Random.Range(i + 1, cells.Count);
      Heightmap.Cell randomCell = cells[randomIndex];
      cells[randomIndex] = cells[i];
      cells[i] = randomCell;
      if (!playerPlatform) {
        if (CreatePickupAtCell(randomCell, 1)) {
          createdCount += 1;
          continue;
        }
      } else {
        Vector2Int endCoordinates = new Vector2Int(randomCell.IndexX, randomCell.IndexY);
        int length = 0;
        int points = 1;
        if (navigation.HasPath(playerPlatform.Coordinates, endCoordinates, out length)) {
          // There's a path, lower value.
          points = Mathf.Max(8, Random.Range(1, length)) - 7;
        } else if (Random.Range(0.0f, 1.0f) > 0.9f) {
          // No path, go nuts.
          points = Random.Range(2, 20);
        }
        if (CreatePickupAtCell(randomCell, points)) {
          createdCount += 1;
          continue;
        }
      }
    }
  }

  private bool CreatePickupAtCell(Heightmap.Cell cell, int points) {
    Platform obj = platforms[cell.IndexY, cell.IndexX];
    if (obj) {
      return obj.SpawnPickup(pickupPrefab, points);
    }
    return false;
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
    // Debug.LogFormat("Value {0}", newValue);
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
        int xCoord;
        int yCoord;
        heightmap.GetHeightmapCoordinates(digit, x, y, out xCoord, out yCoord);
        obj.Coordinates.x = xCoord;
        obj.Coordinates.y = yCoord;
        obj.gameObject.layer = Gameboard.PlatformLayer;
        platforms[obj.Coordinates.y, obj.Coordinates.x] = obj;
      }
    }
  }

  private IEnumerator AnimateValue(int digit, int remainder) {
    remainder = remainder % 10;
    heightmap.SetPending(digit, remainder);
    yield return new WaitForSeconds(pendingSeconds);
    heightmap.Grow(digit, remainder);
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