using UnityEngine;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour {
  public GameObject panel;
  public Text text;
  
  private void Start() {
    panel.SetActive(false);
  }

  public void ToggleMenu() {
    bool nextActiveState = !panel.activeSelf;
    if (nextActiveState) {
      text.text = string.Format("You currently have {0} points", Game.instance.gameboard.Value);
      Time.timeScale = 0.0f;
    } else {
      Time.timeScale = 1.0f;
    }
    panel.SetActive(nextActiveState);
  }
}
  