using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour {
  protected PlayerController player;
  public PlayerController Player {
    get {
      return player;
    }
    set
    {
      player = value;
    }
  }

  abstract public void Interact();
  virtual public void Bump() {
  }

  virtual public void Enter(CellBehavior cell) {
  }
  virtual public void Exit(CellBehavior cell) {
  }
  virtual public void Active() {
  }
  virtual public void InActive() {
  }
}

