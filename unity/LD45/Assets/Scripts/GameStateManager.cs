using System.Collections.Generic;
using UnityEngine;

public interface IGameState {
  void StateUpdate(GameStateManager states);
  void Register(GameStateManager states);
  void Unregister(GameStateManager states);
}

public class GameStateManager {
  private LinkedList<IGameState> states_;

  public GameStateManager() {
    states_ = new LinkedList<IGameState>();
  }

  public void PushState(IGameState state) {
    states_.AddFirst(state);
    state.Register(this);
  }

  public bool PopState() {
    if (states_.Count > 1) {
      Current.Unregister(this);
      states_.RemoveFirst();
      return true;
    }
    Debug.LogWarningFormat("Attempted to pop state from stack of length {0}", states_.Count);
    return false;
  }

  public bool RemoveState(IGameState state) {
    if (states_.Count > 1) {
      state.Unregister(this);
      return states_.Remove(state);
    }
    return false;
  }

  public IGameState Current {
    get { return states_.First.Value; }
  }
}