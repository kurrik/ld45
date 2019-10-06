using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
  public static Game instance = null;
  public Gameboard gameboard;
  public GameStateManager states;
  public EndState endState;
  public SplashState splashState;

  public bool fastDebugStartup = false;
  private bool showDebug;

  public bool ShowDebug {
    get => showDebug;
    set => this.showDebug = value;
  }

  public bool FastDebugStartup {
    get { return ShowDebug && fastDebugStartup; }
  }

  public void End() {
    endState.gameObject.SetActive(true);
    endState.SetPoints(gameboard.Value);
    states.PushState(endState);
  }

  public void Reload() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private void Awake() {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      Destroy(gameObject);
      return;
    }

    Application.targetFrameRate = 60;

    states = new GameStateManager();
    states.PushState(gameboard.player.GetComponent<GameplayState>());
    splashState.gameObject.SetActive(true);
  }

  private void Update() {
    states.Current.StateUpdate(states);
  }
}