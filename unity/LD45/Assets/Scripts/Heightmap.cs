using System.Collections.Generic;

public class Heightmap {
  private const int digitWidth = 5;
  private const int digitHeight = 9;
  private const int digits = 5;
  public const int unitsWide = digitWidth * digits;
  public const int unitsHigh = digitHeight;

  public float minGlobalHeight = -0.1f;
  public float maxGlobalHeight = 5.0f;

  private float globalHeight = 0.0f;

  private float[,] localHeights = new float[unitsWide, unitsHigh];
  private float[,] targetHeights = new float[unitsWide, unitsHigh];
  private float[,] globalHeights = new float[unitsWide, unitsHigh];

  public float GlobalHeight {
    get => globalHeight;
    set => SetGlobalHeight(value);
  }

  public struct Height {
    int DigitX;
    int DigitY;
    float GlobalHeight;

    public Height(int digitX, int digitY, float globalHeight) {
      DigitX = digitX;
      DigitY = digitY;
      GlobalHeight = globalHeight;
    }
  }

  public IEnumerable<Height> DigitHeights(int digitIndex) {
    if (digitIndex >= digits || digitIndex < 0) {
      yield break;
    }
    int startX = digitIndex * digitWidth;
    int endX = startX + digitWidth;
    int startY = 0;
    int endY = digitHeight;
    for (int x = startX; x < endX; x++) {
      for (int y = startY; y < endY; y++) {
        yield return new Height(x - startX, y - startY, globalHeights[y, x]);
      }
    }
  }

  private void SetGlobalHeight(float height) {
    globalHeight = height;
    ComputeGlobalHeights();
  }

  private void ComputeGlobalHeights() {
    for (int x = 0; x < unitsWide; x++) {
      for (int y = 0; y < unitsWide; y++) {
        float h = localHeights[y, x] + globalHeight;
        if (h < minGlobalHeight) {
          h = minGlobalHeight;
          targetHeights[y, x] = h - globalHeight;
        } else if (h > maxGlobalHeight) {
          h = maxGlobalHeight;
          targetHeights[y, x] = h - globalHeight;
        }
      }
    }
  }
}
