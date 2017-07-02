
public class Level {
  public string filePath;
  public string instructions;
  public EndLevelCondition endLevelCondition;
  public string collected;

  public Level(string filePath, string instructions, EndLevelCondition endLevelCondition) {
    this.filePath = filePath;
    this.instructions = instructions;
    this.endLevelCondition = endLevelCondition;
  }
}
