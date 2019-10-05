using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Heightmap {
  public const int digitWidth = 5;
  public const int digitHeight = 9;
  public const int digits = 5;
  public const int unitsWide = digitWidth * digits;
  public const int unitsHigh = digitHeight;

  public float minGlobalHeight = -0.1f;
  public float maxGlobalHeight = 5.0f;
  public float tickDelay = 5.0f;
  public float tickRate = -0.05f;
  public float growDelay = 1.0f;
  public float growAmount = 0.2f;
  public float growRate = 0.4f;

  private float globalHeight = 0.0f;
  private float tickDelayElapsed = 0.0f;
  private float closeEnough = 0.00001f;

  private Cell[,] cells = new Cell[unitsHigh, unitsWide];

  public enum State {
    Normal,
    Pending,
    Growing,
    Stopped,
  }

  public class Cell {
    public int IndexX;
    public int IndexY;
    public int DigitX;
    public int DigitY;
    public State State;
    public float Height;
    public float Target;

    public Cell(int indexX, int indexY, int digitX, int digitY) {
      IndexX = indexX;
      IndexY = indexY;
      DigitX = digitX;
      DigitY = digitY;
      State = State.Normal;
      Target = 0.0f;
      Height = 0.0f;
    }
  }

  public Heightmap() {
    for (int d = 0; d < digits; d++) {
      for (int digitX = 0; digitX < digitWidth; digitX++) {
        for (int digitY = 0; digitY < digitHeight; digitY++) {
          int x = d * digitWidth + digitX;
          int y = digitY;
          cells[y, x] = new Cell(x, y, digitX, digitY);
        }
      }
    }
  }

  public IEnumerable<Cell> DigitCells(int digitIndex) {
    if (digitIndex >= digits || digitIndex < 0) {
      yield break;
    }
    int reverseIndex = digits - digitIndex - 1; // 0th digit is rightmost
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

  public void SetHeight(int digitIndex, int maskIndex, float height) {
    foreach (Cell c in DigitCells(digitIndex)) {
      int maskValue = Masks.Digits[maskIndex, c.DigitY, c.DigitX];
      if (maskValue > 0) {
        Debug.LogFormat("Setting height of {0},{1} to {2}", c.DigitX, c.DigitY, height);
        Cell mutableCell = GetMutableCell(c);
        mutableCell.Height = height;
        mutableCell.Target = height;
      }
    }
    Log();
  }

  public void Grow(int digitIndex, int maskIndex) {
    foreach (Cell c in DigitCells(digitIndex)) {
      int maskValue = Masks.Digits[maskIndex, c.DigitY, c.DigitX];
      if (maskValue > 0) {
        Cell mutableCell = GetMutableCell(c);
        mutableCell.Target += growAmount;
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
  
  private ref Cell GetMutableCell(Cell cell) {
    return ref cells[cell.IndexY, cell.IndexX];
  }

  private void MoveTowardTargetHeights(Cell c, float elapsed) {
    float delta = c.Target - c.Height;
    float magnitude = Mathf.Abs(delta);
    if (magnitude < closeEnough) {
      c.Height = c.Target;
      return;
    }
    float maxUpdate = growRate * elapsed;
    if (magnitude < maxUpdate) {
      c.Height = c.Target;
      c.State = State.Normal;
    } else {
      float sign = delta > 0 ? 1 : -1;
      float adjustment = sign * maxUpdate;
      c.Height += adjustment;
      c.State = State.Growing;
    }
  }

  private void MoveWithTick(Cell c, float amount) {
    c.Height += amount;
    c.Target += amount;
  }

  private void Clamp(Cell c) {
    if (c.Height < minGlobalHeight) {
      c.Height = minGlobalHeight;
      c.Target = minGlobalHeight;
      c.State = State.Stopped;
    } else if (c.Height > maxGlobalHeight) {
      c.Height = maxGlobalHeight;
      c.Target = maxGlobalHeight;
      c.State = State.Normal;
    }
  }
}
