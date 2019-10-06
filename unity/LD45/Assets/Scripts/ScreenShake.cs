using UnityEngine;

public class ScreenShake : MonoBehaviour {
  public float amount = 0.1f;
  public float dampingSpeed = 1.0f;
  public float shakeMagnitude = 0.05f;

  private float shakeDuration = 0f;
  private Vector3 initialPosition;

  void OnEnable() {
    initialPosition = transform.localPosition;
  }

  void Update() {
    if (shakeDuration > 0) {
      transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
      shakeDuration -= Time.deltaTime * dampingSpeed;
    } else {
      shakeDuration = 0f;
      transform.localPosition = initialPosition;
    }
  }

  public void Shake() {
    shakeDuration = amount;
  }
}
