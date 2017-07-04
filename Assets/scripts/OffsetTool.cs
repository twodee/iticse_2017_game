using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OffsetTool : Tool {
  int offset;
  CellBehavior originalTarget;
  PointerController originalPointer;

  void Awake() {
    offset = 0;
    originalTarget = null;
  }
  override public void Interact() {
    player.targetCell = null;
    offset++;
    if (originalPointer != null) {
      if (ReTarget()) {
        offset = 0;
      }
    }
    player.UnLock();
  }


  override public void Active() {
    originalTarget = null;
    originalPointer = null;
    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
      PointerController controller = pointer.GetComponent<PointerController>();
      CellBehavior target = controller.Target;
      // find the next object in the controller's array
      if (target != null && target.owningArray != null) {
        originalTarget = target;
        originalPointer = controller;
        ReTarget();
      }
    }
  }

  override public void InActive() {
    originalTarget = null;
    originalPointer = null;
  }

  bool ReTarget() {
    int index = originalTarget.arrayIndex+offset;
    if (index >= originalTarget.owningArray.Count) {
      index = 0;
    }
    originalPointer.Target = originalTarget.owningArray.Get(index).GetComponent<CellBehavior>();
    return index == 0;
  }

  override public void Enter(CellBehavior cell) {
    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
//      Debug.Log("entered while on Pointer");
      PointerController controller = pointer.GetComponent<PointerController>();
      CellBehavior target = controller.Target;
      // find the next object in the controller's array
      if (target != null && target.owningArray != null) {
        originalTarget = target;
        originalPointer = controller;

        ReTarget();
      }
    }
  }

  override public void Exit(CellBehavior cell) {
    if (originalPointer != null && originalTarget != null) {
//      Debug.Log("exited with original Pointer");
      originalPointer.Target = originalTarget;
      originalPointer = null;
      originalTarget = null;
    }
  }
}