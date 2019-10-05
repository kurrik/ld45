using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifier))]
public class Platform : MonoBehaviour {
  private NavMeshModifier navMeshModifier;
  private Renderer renderer_;
  private float unpaidBoost = 0;

  public float boostRate = 0.3f; // Units per second.
  public float stopThreshold = -0.05f; // Floor at which the piece stops moving.
  public Color defaultColor = new Color(135, 135, 135);
  public Color pendingColor = new Color(255, 236, 105);
  public Color movingColor = new Color(255, 96, 0);

  // Boost is paid out over time.
  public void SetBoost(float amount) {
    SetColor(movingColor);
    unpaidBoost += amount;
  }

  // Adjust is immediate since it's integrated over time by the caller.
  public void Adjust(float amount) {
    ImmediatelyAdjustY(amount);
  }

  public void SetPending() {
    SetColor(pendingColor);
  }

  private void SetColor(Color color) {
    if (renderer_) {
      renderer_.material.SetColor("_Color", color);
    }
  }

  private void SetNavigationEnabled(bool value) {
    navMeshModifier.ignoreFromBuild = !value;
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
