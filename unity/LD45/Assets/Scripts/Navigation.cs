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

    public override bool IsPossibleMove(Vector2Int location) {
      return heightmap.IsValidMove(location.x, location.y);
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
    Vector2Int start = new Vector2Int(a.HeightmapX, a.HeightmapY);
    Vector2Int end = new Vector2Int(b.HeightmapX, b.HeightmapY);
    return pathfinder.GetPath(start, end, out points);
  }
}
