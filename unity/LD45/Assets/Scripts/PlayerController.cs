using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  public Camera cam;
  public NavMeshAgent agent;
  public float walkingRate = 3.0f;

  private UnityEvent<Platform> destinationSetEvent = new DestinationSetEvent();
  private UnityEvent<Platform> playerOnPlatformEvent = new DestinationSetEvent();

  private int pathIndex;
  private Vector3 pathStart;
  private Vector3 pathNext;
  private Vector3[] path;

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

  public void SetPath(Vector3[] p) {
    if (p.Length > 0) {
      path = p;
      pathIndex = -1;
    } else {
      path = null;
    }
    SetNextPathDestination();
  }

  private void SetNextPathDestination() {
    pathStart = transform.position;
    if (path != null) {
      pathIndex++;
      if (pathIndex < path.Length) {
        pathNext = path[pathIndex];
      } else {
        path = null;
        pathNext = pathStart;
      }
    } else {
      pathNext = pathStart;
    }
  }

  private bool CloseToNextPathDestination() {
    return (pathNext - transform.position).sqrMagnitude < 0.01f;
  }

  private void Update() {
    if (CloseToNextPathDestination()) {
      SetNextPathDestination();
    }
    if (path != null) {
      transform.position = Vector3.MoveTowards(transform.position, pathNext, walkingRate * Time.deltaTime);
    }
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