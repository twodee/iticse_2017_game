using System;
using UnityEngine;

public class MallocTool : PointerTool {
  override protected void Awake() {
    base.Awake();
    id = "M";
  }

  override protected void InteractNonCell() {
    if (!IsPointerAttached()) {
      // create a new box of memory, start searching heap for contiguous space
      GameObject cell = player.levelController.Malloc(1, false);
      player.targetCell = cell.GetComponent<CellBehavior>();
      Point();
      player.levelController.OnMalloc(this, player, true, false, 1);
    }
  }
}
