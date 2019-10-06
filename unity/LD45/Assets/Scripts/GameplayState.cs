using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class GameplayState : MonoBehaviour, IGameState {
  private PlayerController playerController;

  private void Start() {
    playerController = GetComponent<PlayerController>();
  }

  public void StateUpdate(GameStateManager states) {
    if (Input.touchCount > 0) {
      for (int i = 0; i < Input.touchCount; i++) {
        if (Input.GetTouch(i).phase == TouchPhase.Began) {
          CheckTouch(Input.GetTouch(i).position);
        }
      }
    } else if (Input.GetMouseButtonDown(0)) {
      CheckTouch(Input.mousePosition);
    } 
  }

  private bool CheckTouch(Vector3 point) {
    bool touched = false;
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(point);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 1000.0f, Gameboard.PlatformLayer)) {
        Platform platform = hit.collider.gameObject.GetComponentInParent<Platform>();
        if (platform) {
          playerController.OnPlatformSelected(platform);
          touched = true;
        }
      }
    }
    return touched;
  }
}