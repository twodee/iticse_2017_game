using UnityEngine.UI;

public class CellFreedEndLevelCondition : EndLevelCondition {
  private CellBehavior cell;
  public CellFreedEndLevelCondition(CellBehavior cell) {
    this.cell = cell;
  }
  public bool Check() {
    return !this.cell.gameObject.activeSelf;
  }
}

