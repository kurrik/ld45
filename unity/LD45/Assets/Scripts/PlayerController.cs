using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {
  public Camera cam;
  public Navigation navigation;
  public GameObject pointsCollectedPrefab;
  public float walkingRate = 3.0f;

  private UnityEvent<Platform> destinationSetEvent = new DestinationSetEvent();
  private UnityEvent<Platform> playerOnPlatformEvent = new DestinationSetEvent();

  private Platform playerPlatform;

  private int pathIndex;
  private Vector3 pathStart;
  private Vector3 pathNext;
  private Vector2Int[] path;

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

  public void SpawnPoints(int points) {
    GameObject obj = Instantiate<GameObject>(
      pointsCollectedPrefab,
      transform.position,
      Quaternion.identity,
      transform.parent
    );
    PointsCollected pc = obj.GetComponent<PointsCollected>();
    if (pc) {
      pc.Activate(transform.position, points);
    }
  }

  public void SetPath(Vector2Int[] p) {
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
        pathNext = navigation.gameboard.HeightmapCoordToWorldCoord(path[pathIndex]);
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
    } else {
      if (playerPlatform) {
        transform.position = navigation.gameboard.HeightmapCoordToWorldCoord(playerPlatform.Coordinates);
      }
    }
  }

  public void OnPlatformSelected(Platform platform) {
    destinationSetEvent.Invoke(platform);
  }

  private void OnTriggerEnter(Collider other) {
    Platform platform = other.gameObject.GetComponentInParent<Platform>();
    if (platform) {
      // Debug.LogFormat("Player on platform: {0}", platform);
      playerOnPlatformEvent.Invoke(platform);
      playerPlatform = platform;
    } else {
      if (other.CompareTag(Game.GroundTag)) {
        Game.instance.End(); 
      }
    }
  }
}