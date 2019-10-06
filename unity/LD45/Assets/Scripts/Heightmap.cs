using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class Heightmap : MonoBehaviour {
  public const int digitWidth = 5;
  public const int digitHeight = 9;
  public const int digits = 5;
  public const int unitsWide = digitWidth * digits;
  public const int unitsHigh = digitHeight;

  private const float closeEnough = 0.00001f;

  public float minGlobalHeight = -2.0f;
  public float maxGlobalHeight = 3.0f;
  public float tickDelay = 5.0f;
  public float tickRate = -0.07f;
  public float growDelay = 1.0f;
  public float growAmount = 0.2f;
  public float growRate = 0.4f;

  private float tickDelayElapsed = 0.0f;
  private UnityEvent<Cell> stateEvent = new StateEvent();

  private Cell[,] cells = new Cell[unitsHigh, unitsWide];

  [System.Serializable]
  public class StateEvent : UnityEvent<Cell> { }

  public enum State {
    Normal,
    Pending,
    Growing,
    Stopped,
    Initializing,
  }

  public class Cell {
    public int DigitIndex;
    public int IndexX;
    public int IndexY;
    public int DigitX;
    public int DigitY;
    public State State;
    public float Height;
    public float Target;

    public Cell(int digitIndex, int indexX, int indexY, int digitX, int digitY, float height) {
      DigitIndex = digitIndex;
      IndexX = indexX;
      IndexY = indexY;
      DigitX = digitX;
      DigitY = digitY;
      State = State.Initializing;
      Target = height;
      Height = height;
    }
  }

  public void Awake() {
    for (int d = 0; d < digits; d++) {
      for (int digitX = 0; digitX < digitWidth; digitX++) {
        for (int digitY = 0; digitY < digitHeight; digitY++) {
          int x = d * digitWidth + digitX;
          int y = digitY;
          int digitIndex = InternalIndexToDigitIndex(d);
          cells[y, x] = new Cell(digitIndex, x, y, digitX, digitY, minGlobalHeight);
        }
      }
    }
  }

  public IEnumerable<Cell> DigitCells(int digitIndex) {
    if (digitIndex >= digits || digitIndex < 0) {
      yield break;
    }
    int reverseIndex = DigitIndexToInternalIndex(digitIndex);
    int startX = reverseIndex * digitWidth;
    int endX = startX + digitWidth;
    int startY = 0;
    int endY = digitHeight;
    for (int x = startX; x < endX; x++) {
      for (int y = startY; y < endY; y++) {
        yield return cells[y, x];
      }
    }
  }

  public IEnumerable<Cell> SpawnableCells() {
    for (int x = 0; x < unitsWide; x++) {
      for (int y = 0; y < unitsHigh; y++) {
        Cell cell = cells[y, x];
        if (cell.State != State.Stopped && cell.Height > 0.05f) {
          yield return cell;
        }
      }
    }
  }

  public void SetHeight(int digitIndex, int maskIndex, float height) {
    foreach (Cell c in DigitCells(digitIndex)) {
      int maskValue = Masks.Digits[maskIndex, c.DigitY, c.DigitX];
      if (maskValue > 0) {
        // Debug.LogFormat("Setting height of {0},{1} to {2}", c.DigitX, c.DigitY, height);
        Cell mutableCell = GetMutableCell(c);
        mutableCell.Height = height;
        mutableCell.Target = height;
        if (height >= 0.0f && (c.State == State.Initializing || c.State == State.Stopped)) {
          SetState(mutableCell, State.Normal);
        }
      }
    }
    // Log();
  }

  public void Grow(int digitIndex, int maskIndex) {
    foreach (Cell c in DigitCells(digitIndex)) {
      int maskValue = Masks.Digits[maskIndex, c.DigitY, c.DigitX];
      if (maskValue > 0) {
        Cell mutableCell = GetMutableCell(c);
        mutableCell.Target += growAmount;
        SetState(mutableCell, State.Growing);
      }
    }
  }

  public void SetPending(int digitIndex, int maskIndex) {
    foreach (Cell c in DigitCells(digitIndex)) {
      int maskValue = Masks.Digits[maskIndex, c.DigitY, c.DigitX];
      if (maskValue > 0) {
        Cell mutableCell = GetMutableCell(c);
        SetState(mutableCell, State.Pending);
      }
    }
  }

  public void Log() {
    StringBuilder str = new StringBuilder();
    for (int y = 0; y < unitsHigh; y++) {
      for (int x = 0; x < unitsWide; x++) {
        Cell c = cells[y, x];
        str.AppendFormat("|{0:n1},{1:n1}", c.Target, c.Height);
      }
      str.Append("|\n");
    }
    Debug.Log(str.ToString());
  }

  public void Tick(float elapsed) {
    bool updateTick = true;
    if (tickDelayElapsed < tickDelay) {
      tickDelayElapsed += elapsed;
      updateTick = false;
    }
    float tickAmount = tickRate * elapsed;
    for (int x = 0; x < unitsWide; x++) {
      for (int y = 0; y < unitsHigh; y++) {
        Cell c = cells[y, x];
        MoveTowardTargetHeights(c, elapsed);
        if (updateTick) {
          MoveWithTick(c, tickAmount);
        }
        Clamp(c);
      }
    }
  }

  private Cell GetCellFromCoords(Vector2Int coords) {
    if (coords.y < 0 || coords.y >= unitsHigh || coords.x < 0 || coords.x >= unitsWide) {
      return null;
    }
    return cells[coords.y, coords.x];
  }

  public bool IsValidMove(Vector2Int current, Vector2Int next) {
    // No invalid data.
    Cell from = GetCellFromCoords(current);
    Cell to = GetCellFromCoords(next);
    if (from == null || to == null) {
      return false;
    }
    // Nothing stopped.
    if (to.State == State.Stopped) {
      return false;
    }
    // Nothing super close to the water.
    if (to.Height < 0.00f) {
      return false;
    }
    // No large jumps up.
    if ((to.Height - from.Height) > 0.5f) {
      return false;
    }
    return true;
  }

  public void AddStateListener(UnityAction<Cell> listener) {
    stateEvent.AddListener(listener);
  }

  public void GetHeightmapCoordinates(int digitIndex, int x, int y, out int heightmapX, out int heightmapY) {
    int reverseIndex = DigitIndexToInternalIndex(digitIndex);
    heightmapX = reverseIndex * digitWidth + x;
    heightmapY = y;
  }

  private int DigitIndexToInternalIndex(int digitIndex) {
    return digits - digitIndex - 1; // 0th digit is rightmost
  }

  private int InternalIndexToDigitIndex(int internalIndex) {
    return digits - internalIndex - 1;
  }

  private void SetState(Cell cell, State state) {
    if (cell.State != state) {
      cell.State = state;
      stateEvent.Invoke(cell);
    }
  }

  private ref Cell GetMutableCell(Cell cell) {
    return ref cells[cell.IndexY, cell.IndexX];
  }

  private void MoveTowardTargetHeights(Cell c, float elapsed) {
    float delta = c.Target - c.Height;
    float magnitude = Mathf.Abs(delta);
    if (magnitude < closeEnough) {
      c.Height = c.Target;
      if (c.State != State.Pending) {
        SetState(c, State.Normal);
      }
      return;
    }
    float maxUpdate = growRate * elapsed;
    if (magnitude < maxUpdate) {
      c.Height = c.Target;
      SetState(c, State.Normal);
    } else {
      float sign = delta > 0 ? 1 : -1;
      float adjustment = sign * maxUpdate;
      c.Height += adjustment;
      SetState(c, State.Growing);
    }
  }

  private void MoveWithTick(Cell c, float amount) {
    c.Height += amount;
    c.Target += amount;
  }

  private void Clamp(Cell c) {
    if (c.State == State.Pending && c.Height < 0.01f) {
      // Float underwater pending items above sea level, but don't enable navigation.
      c.Height = 0.01f;
      c.Target = 0.01f;
    } else if (c.Height < minGlobalHeight) {
      c.Height = minGlobalHeight;
      c.Target = minGlobalHeight;
      SetState(c, State.Stopped);
    } else if (c.Height > maxGlobalHeight) {
      c.Height = maxGlobalHeight;
      c.Target = maxGlobalHeight;
      SetState(c, State.Normal);
    }
  }
}
