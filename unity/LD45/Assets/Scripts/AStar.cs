using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AStar {
  public abstract bool IsPossibleMove(Vector2Int location);

  private class Step {
    public readonly Vector2Int Location;
    public readonly int Cost;
    public readonly Step Parent;

    public Step(Vector2Int location, int cost, Step parent) {
      Location = location;
      Cost = cost;
      Parent = parent;
    }
  }

  private static Vector2Int[] movements = {
    new Vector2Int(-1, 0),
    new Vector2Int(0, -1),
    new Vector2Int(1, 0),
    new Vector2Int(0, 1)
  };

  public bool GetPath(Vector2Int start, Vector2Int destination, out Vector2Int[] path) {
    Priority_Queue.SimplePriorityQueue<Vector2Int> open = new Priority_Queue.SimplePriorityQueue<Vector2Int>();
    Dictionary<Vector2Int, bool> closed = new Dictionary<Vector2Int, bool>();
    Dictionary<Vector2Int, Step> allSteps = new Dictionary<Vector2Int, Step>();

    Step startStep = new Step(start, 0, null);
    allSteps.Add(startStep.Location, startStep);
    open.Enqueue(startStep.Location, 0);

    Step currentStep;
    Vector2Int currentLocation;
    Step neighborStep;

    // While lowest rank in OPEN is not the GOAL:
    while (open.Count > 0) {
      // Set current = remove lowest rank item from OPEN
      currentLocation = open.Dequeue();
      if (!allSteps.TryGetValue(currentLocation, out currentStep)) {
        throw new Exception("Could not find expected step");
      }
      if (currentLocation == destination) {
        // Reconstruct reverse path from goal to start by following parent pointers
        path = GetPoints(currentStep);
        return true;
      }
      // Add current to CLOSED
      closed.Add(currentLocation, true);

      // For neighbors of current:
      foreach (Vector2Int m in movements) {
        bool inOpen;
        bool inClosed;
        int nextCost = currentStep.Cost + 1;
        Vector2Int nextLocation = currentLocation + m;
        Step nextStep = new Step(nextLocation, nextCost, currentStep);

        if (!IsPossibleMove(nextLocation)) {
          // Spot is bad.
          continue;
        }

        // If neighbor in OPEN and cost less than g(neighbor):
        // remove neighbor from OPEN, because new path is better
        if (open.Contains(nextLocation)) {
          if (!allSteps.TryGetValue(nextLocation, out neighborStep)) {
            throw new Exception("Could not find expected step");
          }
          if (nextCost < neighborStep.Cost) {
            open.Remove(nextLocation);
            allSteps.Remove(nextLocation);
            inOpen = false;
          } else {
            inOpen = true;
          }
        } else {
          inOpen = false;
        }

        // If neighbor in CLOSED and cost less than g(neighbor):
        // remove neighbor from CLOSED
        if (closed.ContainsKey(nextLocation)) {
          if (!allSteps.TryGetValue(nextLocation, out neighborStep)) {
            throw new Exception("Could not find expected step");
          }
          if (nextCost < neighborStep.Cost) {
            closed.Remove(nextLocation);
            inClosed = false;
          } else {
            inClosed = true;
          }
        } else {
          inClosed = false;
        }

        // If neighbor not in OPEN and neighbor not in CLOSED:
        if (!inOpen && !inClosed) {
          int h = Heuristic(nextLocation, destination);
          neighborStep = new Step(nextLocation, nextCost, currentStep);
          allSteps.Add(nextLocation, neighborStep);
          open.Enqueue(nextLocation, nextCost + h);
        }
      }
    }
    path = null;
    return false;
  }

  private int Heuristic(Vector2Int from, Vector2Int to) {
    Vector2Int diff = to - from;
    return diff.sqrMagnitude;
  }

  private Vector2Int[] GetPoints(Step destination) {
    int count = 1;
    Step marker = destination;
    while (marker.Parent != null) {
      count += 1;
      marker = marker.Parent;
    }
    Vector2Int[] points = new Vector2Int[count];
    marker = destination;
    for (int i = count - 1; i > -1; i--) {
      points[i] = marker.Location;
      marker = marker.Parent; 
    }
    return points;
  }
}