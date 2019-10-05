using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  public Camera cam;
  public NavMeshAgent agent;

  private UnityEvent<Platform> destinationSetEvent = new DestinationSetEvent();
  private UnityEvent<Platform> playerOnPlatformEvent = new DestinationSetEvent();

  [System.Serializable]
  public class DestinationSetEvent : UnityEvent<Platform> { }

  [System.Serializable]
  public class PlayerOnPlatformEvent : UnityEvent<Platform> { }

  public void AddDestinationSetListener(UnityAction<Platform> listener) {
    destinationSetEvent.AddListener(listener);
  }

  public void AddPlayerOnPlatformListener(UnityAction<Platform> listener) {
    playerOnPlatformEvent.AddListener(listener);
  }

  private void Update() {
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
          destinationSetEvent.Invoke(platform);
        }
      }
    }
  }

  private void OnTriggerEnter(Collider other) {
    Platform platform = other.gameObject.GetComponentInParent<Platform>();
    if (platform) {
      Debug.LogFormat("Player on platform: {0}", platform);
      playerOnPlatformEvent.Invoke(platform);
    }
  }
}