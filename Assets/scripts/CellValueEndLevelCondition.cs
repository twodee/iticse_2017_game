using UnityEngine;
using UnityEngine.UI;

public class CellValueEndLevelCondition : EndLevelCondition {
  private Text text;
  private string expected;
  private LevelController levelController;
  private int x;
  private int y;
  public CellValueEndLevelCondition(LevelController levelController, int x, int y, string expected) {
    this.levelController = levelController;
    this.expected = expected;
    this.x = x;
    this.y = y;
    SetText();
  }
  public bool Check() {
    if (this.text == null) {
      SetText();
    }
    return this.text != null && this.text.text == expected;
  }

  private void SetText() {
    if (levelController.HasAt(x, y)) {
      GameObject go = levelController.FindAt(x, y);
      this.text = go.GetComponentInChildren<Text>();
      go.GetComponent<CellController>().SetExpected(expected);

    }
  }
}
