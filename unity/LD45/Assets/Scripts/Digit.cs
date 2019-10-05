using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Digit : MonoBehaviour {
  private const int letterWidth = 5;
  private const int letterHeight = 9;

  public GameObject platformPrefab = null;
  public float pendingSeconds = 1.0f;
  public float transitionSeconds = 0.5f;
  public float bumpIncrement = 0.1f;
  public Color defaultColor;
  public Color pendingColor;
  public Color movingColor;

  private int value = 0;
  private GameObject[,] objects = new GameObject[letterHeight, letterWidth];
  private float[,] heights = new float[letterHeight, letterWidth];

  public int Value { 
    get => value;
    set => StartCoroutine(ChangeValue(value));
  }

  void Start() {
    for (int x = 0; x < letterWidth; x++) {
      for (int y = 0; y < letterHeight; y++) {
        float height = 0.0f;
        heights[y, x] = height;
        Vector3 offset = new Vector3(-x, height, y);
        objects[y, x] = Instantiate<GameObject>(
          platformPrefab,
          offset,
          Quaternion.identity,
          transform
        );
        objects[y, x].GetComponentInChildren<Renderer>().material.SetColor("_Color", defaultColor);
      }
    }
  }

  private struct Address {
    public int X;
    public int Y;

    public Address(int x, int y) {
      X = x;
      Y = y;
    }
  }

  private class MaskedAddresses : IEnumerable<Address> {
    private int v;

    public MaskedAddresses(int newValue) {
      v = newValue;
    }

    private IEnumerator<Address> maskedAddresses() {
      for (int x = 0; x < letterWidth; x++) {
        for (int y = 0; y < letterHeight; y++) {
          if (masks[v, y, x] == 1) {
            yield return new Address(x, y);
          }
        }
      }
    }

    public IEnumerator<Address> GetEnumerator() {
      return maskedAddresses();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }

  private IEnumerator ChangeValue(int newValue) {
    newValue = newValue % 10;
    if (newValue == value) {
      yield break;
    }
    MaskedAddresses maskedAddresses = new MaskedAddresses(newValue);
    foreach (Address a in maskedAddresses) {
      objects[a.Y, a.X].GetComponentInChildren<Renderer>().material.SetColor("_Color", pendingColor);
    }
    yield return new WaitForSeconds(pendingSeconds);
    foreach (Address a in maskedAddresses) {
      objects[a.Y, a.X].GetComponentInChildren<Renderer>().material.SetColor("_Color", movingColor);
    }
    float elapsed = 0.0f;
    yield return new WaitForEndOfFrame();
    while (elapsed < transitionSeconds) {
      elapsed += Time.deltaTime;
      float pct = elapsed / transitionSeconds;
      foreach (Address a in maskedAddresses) {
        float start = heights[a.Y, a.X];
        Vector3 pos = objects[a.Y, a.X].transform.position;
        pos.y = start + (bumpIncrement * pct);
        objects[a.Y, a.X].transform.position = pos;
      }
      yield return new WaitForEndOfFrame();
    }
    value = newValue;
    foreach (Address a in maskedAddresses) {
      heights[a.Y, a.X] += bumpIncrement;
      objects[a.Y, a.X].GetComponentInChildren<Renderer>().material.SetColor("_Color", defaultColor);
    }
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
}
