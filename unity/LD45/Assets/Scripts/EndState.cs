using UnityEngine;
using UnityEngine.UI;

public class EndState : MonoBehaviour, IGameState {
  public Text pointsText;
  private float elapsed = 0.0f;
  public float minimumDelay = 1.0f;

  private void OnEnable() {
    elapsed = 0.0f;
    Game.instance.states.PushState(this);
  }

  public void Register(GameStateManager states) { }
  public void Unregister(GameStateManager states) { }

  public void Start() {
  }

  public void StateUpdate(GameStateManager states) {
    if (!gameObject.activeSelf) {
      states.PopState();
    }
    if (elapsed < minimumDelay) {
      elapsed += Time.deltaTime;
    } else {
      if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
        states.PopState();
        Game.instance.Reload();
      }
    }
  }

  public void SetPoints(int points) {
    pointsText.text = string.Format("You scored {0} points!", points);
  }
}