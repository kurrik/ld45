using UnityEngine;

public class Pickup : MonoBehaviour {
  new private Renderer renderer;
  private bool collected = false;
  private float rotationAngle = 0.0f;
  public float rotationRate = 100f;
  public int points = 1;

  public Vector2Int Coordinates = new Vector2Int(-1, -1);

  [ColorUsage(true, true)]
  public Color defaultColor = new Color(0.62f, 0.19f, 0.15f);
  [ColorUsage(true, true)]
  public Color twoColor;
  [ColorUsage(true, true)]
  public Color fiveColor;
  [ColorUsage(true, true)]
  public Color tenColor;
  [ColorUsage(true, true)]
  public Color twentyColor;

  public void SetPoints(int p) {
    points = p;
    if (points >= 20) {
      SetColor(twentyColor);
    } else if (points >= 10) {
      SetColor(tenColor);
    } else if (points >= 5) {
      SetColor(fiveColor);
    } else if (points >= 2) {
      SetColor(twoColor);
    } else {
      SetColor(defaultColor);
    }
  }

  private void SetColor(Color color) {
    if (renderer) {
      // renderer.material.SetColor("_Color", color);
      renderer.material.SetColor("_EmissionColor", color);
      renderer.material.EnableKeyword("_EMISSION");
    }
  }

  private void Start() {
    renderer = GetComponentInChildren<Renderer>();
    SetPoints(points);
  }

  private void Update() {
    rotationAngle += rotationRate * Time.deltaTime;
    transform.localRotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
  }

  private void OnDisable() {
    if (collected) {
      return;
    }
    collected = true;
    Gameboard board = GetComponentInParent<Gameboard>();
    if (board) {
      board.OnPickupDisabled(this);
    }
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
