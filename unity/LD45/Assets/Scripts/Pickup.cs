using UnityEngine;

public class Pickup : MonoBehaviour {
  private bool collected = false;
  private float rotationAngle = 0.0f;
  public float rotationRate = 100f;


  private void Update() {
    rotationAngle += rotationRate * Time.deltaTime;
    transform.localRotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
  }

  private void OnTriggerEnter(Collider other) {
    if (collected) {
      return;
    }
    collected = true;
    Gameboard board = GetComponentInParent<Gameboard>();
    if (board) {
      board.OnPickupTouched(this);
    }
  }
}
