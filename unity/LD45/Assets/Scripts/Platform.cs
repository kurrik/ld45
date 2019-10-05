using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifier))]
public class Platform : MonoBehaviour {
  private NavMeshModifier navMeshModifier;
  private Renderer renderer_;
  private float unpaidBoost = 0;

  public float boostRate = 0.3f; // Units per second.
  public float stopThreshold = -0.05f; // Floor at which the piece stops moving.
  public float startThreshold = 0.1f; // Altitude at which a reactivated block appears.
  public Color defaultColor = new Color(0.53f, 0.53f, 0.53f);
  public Color pendingColor = new Color(1.0f, 0.93f, .41f);
  public Color movingColor = new Color(1.0f, .38f, 0.0f);

  // Boost is paid out over time.
  public void SetBoost(float amount) {
    SetColor(movingColor);
    unpaidBoost += amount;
  }

  // Adjust is immediate since it's integrated over time by the caller.
  public void Adjust(float amount) {
    ImmediatelyAdjustY(amount);
  }

  public void SetHeight(float height) {
    ImmediatelyAdjustY(height - transform.position.y);
  }

  public void SetPending() {
    SetColor(pendingColor);
    if (transform.position.y < startThreshold) {
      SetHeight(startThreshold);
    }
  }

  public void SpawnPickup(GameObject prefab) {
    GameObject obj = Instantiate<GameObject>(
      prefab,
      transform.position + prefab.transform.position,
      Quaternion.identity,
      transform
    );
  }

  private void SetColor(Color color) {
    if (renderer_) {
      renderer_.material.SetColor("_Color", color);
    }
  }

  private void SetNavigationEnabled(bool value) {
    if (navMeshModifier) {
      navMeshModifier.ignoreFromBuild = !value;
    }
  }

  private void Start() {
    navMeshModifier = GetComponent<NavMeshModifier>();
    renderer_ = GetComponentInChildren<Renderer>();
    SetColor(defaultColor);
  }

  private void ImmediatelyAdjustY(float amount) {
    Vector3 pos = transform.position;
    pos.y += amount;
    if (pos.y < stopThreshold) {
      pos.y = stopThreshold;
      SetNavigationEnabled(false);
    } else {
      SetNavigationEnabled(true);
    }
    transform.position = pos;
  }

  private void FixedUpdate() {
    if (System.Math.Abs(unpaidBoost) < 0.0001f) {
      return;
    }
    float magnitude = Mathf.Abs(unpaidBoost);
    float maxUpdate = boostRate * Time.fixedDeltaTime;
    if (magnitude < maxUpdate) {
      ImmediatelyAdjustY(unpaidBoost);
      unpaidBoost = 0;
      SetColor(defaultColor);
    } else {
      float sign = unpaidBoost > 0 ? 1 : -1;
      float adjustment = sign * maxUpdate;
      ImmediatelyAdjustY(adjustment);
      unpaidBoost -= adjustment;
    }
  }
}
