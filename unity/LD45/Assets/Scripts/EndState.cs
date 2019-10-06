﻿using UnityEngine;
using UnityEngine.UI;

public class EndState : MonoBehaviour, IGameState {
  public Text pointsText;

  private void OnEnable() {
    Game.instance.states.PushState(this);
  }

  public void Start() {
  }

  public void StateUpdate(GameStateManager states) {
    if (!gameObject.activeSelf) {
      states.PopState();
    }
    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
      states.PopState();
      Game.instance.Reload();
    }
  }

  public void SetPoints(int points) {
    pointsText.text = string.Format("You scored {0} points!", points);
  }
}