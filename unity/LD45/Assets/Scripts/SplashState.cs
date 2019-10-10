using UnityEngine;

public class SplashState : MonoBehaviour, IGameState {
  public GameObject quitButton;
  private GameStateManager stateManager;

  public void Register(GameStateManager states) {
    stateManager = states;
  }

  public void Unregister(GameStateManager states) {
    stateManager = null;
  }

  public void Start() {
    if (Game.instance.FastDebugStartup) {
      gameObject.SetActive(false);
    } else {
      Game.instance.states.PushState(this);
      Time.timeScale = 0.0f;
    }
  }

  public void Advance() {
    stateManager.PopState();
    gameObject.SetActive(false);
    Time.timeScale = 1.0f;
  }

  public void StateUpdate(GameStateManager states) {
    if (Input.anyKeyDown) {
      if (!Input.GetMouseButtonDown(0) && Input.touchCount == 0) {
        Advance();
      }
    }
  }
}