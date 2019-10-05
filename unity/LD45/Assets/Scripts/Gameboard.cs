using UnityEngine;
using UnityEngine.AI;

public class Gameboard : MonoBehaviour {
  private int value = 0;
  private int maxValue = 0;
  private int bakeTicks = 0;
  private float descendDelayElapsed = 0.0f;

  public float descendDelay = 5.0f;
  public float descendRate = 0.05f; 

  public Heightmap Heightmap = new Heightmap();
  public NavMeshSurface surface;
  public Digit[] digits;
  public GameObject pickupPrefab;

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
  }

  private void Start() {
    for (int i = 0; i < digits.Length; i++) {
      digits[i].SetDigitIndex(i);
    }
    maxValue = ((int)Mathf.Pow(10.0f, digits.Length)) - 1;
    SpawnPickup();
  }

  private void Update() {
    BakeNavigation();
  }

  private void FixedUpdate() {
    Heightmap.Tick(Time.fixedDeltaTime);
  }

  private void SpawnPickup() {
    if (digits.Length > 0) {
      Platform platform = digits[0].FindPickupPlatform();
      if (platform != null) {
        platform.SpawnPickup(pickupPrefab);
      }
    }
  }

  private void SetValue(int newValue) {
    if (newValue > maxValue) {
      Debug.LogErrorFormat("Trying to set gameboard value to {0} which is higher than max of {1}", value, maxValue);
      return;
    }
    int position = 0;
    int v = newValue;
    while (v > 0) {
      int digit = v % 10;
      digits[position].StartAnimateValue(digit, this);
      v = v / 10;
      position += 1;
    }
    value = newValue;
    SpawnPickup();
  }
}