using UnityEngine.UI;

public class CellValueEndLevelCondition : EndLevelCondition {
  private Text text;
  private string expected;
  public CellValueEndLevelCondition(Text text, string expected) {
    this.text = text;
    this.expected = expected;
  }
  public bool Check() {
    return this.text.text == expected;
  }
}
