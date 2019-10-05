using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour {
  private const int letterWidth = 5;
  private const int letterHeight = 9;

  public GameObject platformPrefab = null;
  public float pendingSeconds = 1.0f;
  public float transitionSeconds = 0.5f;
  public float bumpIncrement = 0.2f;
  public float descendDelay = 5f;
  public float descendRate = 0.05f;
  public Color defaultColor;
  public Color pendingColor;
  public Color movingColor;

  private int value = 0;
  private float descendDelayElapsed = 0.0f;
  private Platform[,] objects = new Platform[letterHeight, letterWidth];

  public void SetupInitialPlatform() {
    SetValue(0, bumpIncrement * 3);
  }

  public void StartAnimateValue(int newValue, Gameboard gameboard) {
    StartCoroutine(AnimateValue(newValue, gameboard));
  }

  private void Awake() {
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        float height = -0.2f;
        Vector3 offset = new Vector3(letterWidth - x - 1, height, y);
        Platform obj = Instantiate<GameObject>(
          platformPrefab,
          transform.position + offset,
          Quaternion.identity,
          transform
        ).GetComponent<Platform>();
        obj.defaultColor = defaultColor;
        obj.pendingColor = pendingColor;
        obj.movingColor = movingColor;
        objects[y, x] = obj;
      }
    }
  }

  private void FixedUpdate() {
    if (descendDelayElapsed < descendDelay) {
      descendDelayElapsed += Time.fixedDeltaTime;
      return;
    }
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        Platform obj = objects[y, x];
        obj.Adjust(-descendRate * Time.fixedDeltaTime);
      }
    }
  }

  private void SetValue(int newValue, float height) {
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        if (masks[newValue, y, x] == 1) {
          Platform obj = objects[y, x];
          obj.Adjust(height);
        }
      }
    }
  }

  private IEnumerator AnimateValue(int newValue, Gameboard gameboard) {
    newValue = newValue % 10;
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        if (masks[newValue, y, x] == 1) {
          objects[y, x].SetPending();
        }
      }
    }
    yield return new WaitForSeconds(pendingSeconds);
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        if (masks[newValue, y, x] == 1) {
          Platform obj = objects[y, x];
          obj.SetBoost(bumpIncrement);
        }
      }
    }
    value = newValue;
  }

  public static int[,,] masks = {
    {
      {1,1,1,1,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
    },
    {
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
    },
    {
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {1,1,1,1,1},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,1,1,1,1},
    },
    {
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {1,1,1,1,1},
    },
    {
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
    },
    {
      {1,1,1,1,1},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {1,1,1,1,1},
    },
    {
      {1,1,1,1,1},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,0,0,0,0},
      {1,1,1,1,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
    },
    {
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
    },
    {
      {1,1,1,1,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
    },
    {
      {1,1,1,1,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,0,0,0,1},
      {1,1,1,1,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {0,0,0,0,1},
      {1,1,1,1,1},
    },
  };

#if UNITY_EDITOR
  private Color gizmoColor_ = new Color(1.0f, 0.0f, 1.0f, 0.5f);

  public void OnDrawGizmosSelected() {
    Gizmos.color = gizmoColor_;
    Vector3 dimensions = new Vector3(letterWidth, 1, letterHeight);
    Gizmos.DrawCube(transform.position + dimensions / 2 + new Vector3(0, -0.9f, 0), dimensions);
  }
#endif
}