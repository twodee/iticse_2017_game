using System.Collections;

public class AndAllEndLevelCondition : EndLevelCondition {
  private ArrayList all;
  public AndAllEndLevelCondition() {
    all = new ArrayList();
  }

  public void Add(EndLevelCondition condition) {
      all.Add(condition);
  }

  public bool Check() {
    foreach (EndLevelCondition condition in all) {
      if (!condition.Check()) {
        return false;
      }
    }
    return true;
  }
}
