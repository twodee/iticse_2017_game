
public class Level {
  public string filePath;
  public string instructions;
  public EndLevelCondition endLevelCondition;
  public string collected = "";
  public int world;
  public int level;

  public Level(string filePath, string instructions, EndLevelCondition endLevelCondition, int world, int level) {
    this.filePath = filePath;
    this.instructions = instructions;
    this.endLevelCondition = endLevelCondition;
    this.world = world;
    this.level = level;
  }
}
