using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OffsetTool : Tool {
  int offset;
  CellBehavior originalTarget;
  PointerController originalPointer;
  Text text;

  void Awake() {
    id = "O";
    text = gameObject.transform.Find("mod/canvas/text").GetComponent<Text>();
    offset = 0;
    originalTarget = null;
  }

  public int Offset {
    get {
      return offset;
    }
    private set
    {
      offset = value;
      text.text = offset.ToString();
    }
  }

  override public void Interact() {
    player.targetCell = null;
    Offset = offset + 1;
    if (originalPointer != null) {
      if (ReTarget()) {
        Offset = 0;
      }
    }
    else {
      Bump();
    }
    player.levelController.OnOffsetChange(player, this, offset, false);

    player.UnLock();
  }

  override public void Bump() {
    Offset = 0;
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
        originalPointer.CurrentOffset = this;

        ReTarget();
      }
    }
  }

  override public void InActive() {
    if (originalPointer != null) {
      originalPointer.Target = originalTarget;
      originalPointer.CurrentOffset = null;

      originalTarget = null;
      originalPointer = null;
    }
  }

  bool ReTarget() {
//    Debug.Log("retargetted " + originalTarget.gameObject.tag + " at " + (originalTarget.arrayIndex+offset));
    int index = originalTarget.arrayIndex+offset;
    if (index >= originalTarget.owningArray.Count) {
      index = 0;
    }
    originalPointer.Target = originalTarget.owningArray.Get(index).GetComponent<CellBehavior>();
    return index == 0;
  }

  override public void Enter(CellBehavior cell) {
    if (cell.GetType() == typeof(PointerController)) {
      GameObject pointer = player.GetOnPointer();
      if (pointer != null) {
        //      Debug.Log("entered while on Pointer");
        PointerController controller = pointer.GetComponent<PointerController>();
        CellBehavior target = controller.Target;
        // find the next object in the controller's array
        if (target != null && target.owningArray != null) {
          originalTarget = target;
          originalPointer = controller;

          originalPointer.CurrentOffset = this;


          ReTarget();
        }
      }
    }
  }

  override public void Exit(CellBehavior cell) {
    if (cell.GetType() == typeof(PointerController)) {

      if (originalPointer != null && originalTarget != null) {
//      Debug.Log("exited with original Pointer");
        originalPointer.Target = originalTarget;
        originalPointer.CurrentOffset = null;
        originalPointer = null;
        originalTarget = null;
      }
    }
  }
}