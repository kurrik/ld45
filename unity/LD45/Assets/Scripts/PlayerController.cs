using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  public Camera cam;
  public NavMeshAgent agent;

  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;

      // Old navigation - DELETE
      if (Physics.Raycast(ray, out hit)) {
        agent.SetDestination(hit.point);
      }

      // New navigation - KEEP
      if (Physics.Raycast(ray, out hit, 1000.0f, Gameboard.PlatformLayer)) {
        Platform platform = hit.collider.gameObject.GetComponentInParent<Platform>();
        if (platform) {
          Debug.LogFormat("Raycast hit on platform: {0}", platform);
        }
      }
    }
  }
}