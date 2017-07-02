public class LinkTargetEndLevelCondition : EndLevelCondition {
  private PointerController pointer;
  private CellController target;
  public LinkTargetEndLevelCondition(PointerController pc, CellController cc) {
    pointer = pc;
    target = cc;
  }

  public bool Check() {
    return pointer.Target == target;
  }
}
