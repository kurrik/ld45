using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class PointsCollected : MonoBehaviour {
  public float travelDistance = 1.0f;
  public float travelDuration = 0.3f;

  private TextMesh textMesh;

  public void Activate(Vector3 start, int points) {

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

  private IEnumerator Animate(Vector3 start, int points) {
    yield return new WaitForEndOfFrame();
    textMesh.text = string.Format("+{0}", points);
    float startPosition = start.y + Random.Range(1.5f, 2.5f);
    float endPosition = startPosition + travelDistance + Random.Range(-0.2f, 0.2f);
    float duration = travelDuration + Random.Range(-0.1f, 0.1f);
    float elapsed = 0.0f;
    while (elapsed < duration) {
      elapsed += Time.deltaTime;
      float pct = elapsed / duration;
      // Ease out
      pct = Mathf.Sin(pct * Mathf.PI * 0.5f);
      float position = Mathf.Lerp(startPosition, endPosition, pct);
      Vector3 pos = transform.position;
      pos.y = position;
      transform.position = pos;
      yield return new WaitForEndOfFrame();
    }
    Destroy(gameObject);
  }
}