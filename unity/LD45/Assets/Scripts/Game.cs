using UnityEngine;

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

  public void Awake() {
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

  void Update() {
    states.Current.StateUpdate(states);
  }
}