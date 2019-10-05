using UnityEngine;

public class Game : MonoBehaviour {
  public static Game instance = null;
  public Gameboard Gameboard;

  public void Awake() {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      Destroy(gameObject);
      return;
    }
    Application.targetFrameRate = 60;
  }
}