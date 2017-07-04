using System;
using UnityEngine;


public class IncrementTool : Tool {
  void Awake() {
  }
  override public void Interact() {
    player.targetCell = null;

    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
      PointerController controller = pointer.GetComponent<PointerController>();
      CellBehavior target = controller.Target;
      // find the next object in the controller's array
      if (target != null && target.owningArray != null) {
        int index = target.arrayIndex+1;
        if (index == target.owningArray.Count) {
          index = 0;
        }
        controller.Target = target.owningArray.Get(index).GetComponent<CellBehavior>();
      }

    }

    player.UnLock();
  }
}

