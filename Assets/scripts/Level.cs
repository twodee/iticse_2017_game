
[System.Serializable]
public class Level {
  public string filePath;
  public string instructions;
  public string consoleMatch;

  public Level(string filePath, string instructions, string consoleMatch) {
    this.filePath = filePath;
    this.instructions = instructions;
    this.consoleMatch = consoleMatch;
  }
}
