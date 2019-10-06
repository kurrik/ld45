using UnityEngine;

public class Navigation : MonoBehaviour {
  private HeightmapAStar pathfinder;
  public Heightmap heightmap;
  public Gameboard gameboard;

  private class HeightmapAStar : AStar {
    private Heightmap heightmap;

    public HeightmapAStar(Heightmap h) {
      heightmap = h;
    }

    public override bool IsPossibleMove(Vector2Int current, Vector2Int next) {
      return heightmap.IsValidMove(current, next);
    }
  }

  public void Start() {
    pathfinder = new HeightmapAStar(heightmap);
  }

  public bool GetPoints(Platform a, Platform b, out Vector2Int[] points) {
    points = null;
    if (pathfinder == null) {
      return false;
    }
    Vector2Int start = a.Coordinates;
    Vector2Int end = b.Coordinates;
    return pathfinder.GetPath(start, end, out points);
  }
}
