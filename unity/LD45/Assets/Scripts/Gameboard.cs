using UnityEngine;
using UnityEngine.AI;

public class Gameboard : MonoBehaviour {
  private int value = 0;
  private int maxValue = 0;
  private int bakeTicks = 0;

  public NavMeshSurface surface;
  public Digit[] digits;

  public int Value {
    get => value;
    set => SetValue(value);
  }

  public void BakeNavigation() {
    bakeTicks += 1;
    if (bakeTicks > 3) {
      surface.BuildNavMesh();
      bakeTicks = 0;
    }
  }

  private void Start() {
    if (digits.Length > 0) {
      digits[0].SetupInitialPlatform();
    }
    maxValue = ((int)Mathf.Pow(10.0f, digits.Length)) - 1;
  }

  private void Update() {
    BakeNavigation();
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
  }
}