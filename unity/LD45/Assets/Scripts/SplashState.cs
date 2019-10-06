using UnityEngine;

public class SplashState : MonoBehaviour, IGameState {
  public void Start() {
    if (Game.instance.FastDebugStartup) {
      gameObject.SetActive(false);
    } else {
      Game.instance.states.PushState(this);
    }
  }

  public void StateUpdate(GameStateManager states) {
    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
      states.PopState();
      gameObject.SetActive(false);
    }
  }
}