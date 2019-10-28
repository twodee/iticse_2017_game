
using System;
using UnityEngine;
using UnityEngine.UI;



public class FreeTool : Tool {
  void Awake() {
    id = "F";

  }
  override public void Interact() {
    player.targetCell = null;

    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
      PointerController controller = pointer.GetComponent<PointerController>();
      CellBehavior target = controller.Target;
      // delete the array (but not really)
      if (target != null && target.owningArray != null) {
        CellArray array = target.owningArray;
        for (int i = 0; i < array.Count; i++) {
          GameObject go = array.Get(i);
          go.SetActive(false);

        }
        player.levelController.OnFree(controller, player, true);
        controller.Target = null;
      }
      else if (target != null) {
        // delete just the single entity
        target.gameObject.SetActive(false);
        player.levelController.OnFree(controller, player, false);

        controller.Target = null;
      }
    }
    player.UnLock();
  }
}