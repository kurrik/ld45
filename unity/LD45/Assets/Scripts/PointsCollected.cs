using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class PointsCollected : MonoBehaviour {
  public float travelDistance = 1.0f;
  public float travelDuration = 0.3f;
  public float startOffset = 2.0f;
  private Vector3 startPosition;

  private TextMesh textMesh;

  public void Activate(Transform start, int points) {
    startPosition = start.TransformPoint(Vector3.down * startOffset);
    transform.position = startPosition;
    StartCoroutine(Animate(start, points));
  }

  private void Start() {
    textMesh = GetComponent<TextMesh>();
    // transform.localEulerAngles = new Vector3(180, 0, 0);
  }

  private void Update() {
    Transform cameraTransform = Camera.main.transform;
    transform.LookAt(
      transform.position + cameraTransform.rotation * Vector3.forward,
      cameraTransform.rotation * Vector3.up
    );
  }

  private IEnumerator Animate(Transform start, int points) {
    yield return new WaitForEndOfFrame();
    textMesh.text = string.Format("+{0}", points);
    Vector3 endPosition = start.TransformPoint(Vector3.down * (travelDistance + startOffset + Random.Range(-0.2f, 0.2f)));
    float duration = travelDuration + Random.Range(-0.1f, 0.1f);
    float elapsed = 0.0f;
    while (elapsed < duration) {
      elapsed += Time.deltaTime;
      float pct = elapsed / duration;
      // Ease out
      pct = Mathf.Sin(pct * Mathf.PI * 0.5f);
      transform.position = Vector3.Lerp(startPosition, endPosition, pct);
      yield return new WaitForEndOfFrame();
    }
    Destroy(gameObject);
  }
}