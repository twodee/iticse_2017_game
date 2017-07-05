public class LinkTargetEndLevelCondition : EndLevelCondition {
  private PointerController pointer;
  private CellBehavior target;
  public LinkTargetEndLevelCondition(PointerController pc, CellBehavior cc) {
    pointer = pc;
    target = cc;
  }

  public bool Check() {
    return pointer.Target == target;
  }
}
