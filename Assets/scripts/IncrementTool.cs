using System;
using UnityEngine;
using UnityEngine.UI;



public class IncrementTool : Tool {
  int incrementValue = 1;
  Text text;

  void Awake() {
    id = "I";
    text = gameObject.transform.Find("mod/canvas/text").GetComponent<Text>();
  }

  int IncrementValue {
    set {
      incrementValue = value;
      text.text = incrementValue == 1 ? "++" : "--";
    }
  }

  override public void Interact() {
    player.targetCell = null;

    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
      PointerController controller = pointer.GetComponent<PointerController>();
      CellBehavior target = controller.Target;
      // find the next object in the controller's array
      if (target != null && target.owningArray != null) {
        int index = target.arrayIndex + incrementValue;
        if (index < 0) {
          index = 0;
        }
        else if (index == target.owningArray.Count) {
          index = target.owningArray.Count - 1;
        }
        CellBehavior newTarget = target.owningArray.Get(index).GetComponent<CellBehavior>();
        if (newTarget != controller.Target) {
          controller.Target = newTarget;
          player.levelController.OnIncrement(controller, player, incrementValue);
        }
      }
    }
    else {
      Bump();
    }

    player.UnLock();
  }

  override public void Bump() {
    IncrementValue = -1 * incrementValue;
  }
}

