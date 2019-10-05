using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifier))]
public class Platform : MonoBehaviour {
  private NavMeshModifier navMeshModifier;
  private Renderer renderer_;

  public Color defaultColor = new Color(0.53f, 0.53f, 0.53f);
  public Color pendingColor = new Color(1.0f, 0.93f, .41f);
  public Color movingColor = new Color(1.0f, .38f, 0.0f);

  public int HeightmapX = -1;
  public int HeightmapY = -1;

  public void SetHeight(float height) {
    Vector3 pos = transform.position;
    pos.y = height;
    transform.position = pos;
  }

  public void OnCellStateChanged(Heightmap.Cell cell) {
    switch (cell.State) {
      case Heightmap.State.Growing:
        SetColor(movingColor);
        SetNavigationEnabled(true);
        break;
      case Heightmap.State.Pending:
        SetColor(pendingColor);
        SetNavigationEnabled(true);
        break;
      case Heightmap.State.Stopped:
        SetColor(defaultColor);
        SetNavigationEnabled(false);
        break;
      default:
        SetColor(defaultColor);
        SetNavigationEnabled(true);
        break;
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
}
