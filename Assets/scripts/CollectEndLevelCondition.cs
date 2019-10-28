using System.Collections.Generic;

public class CollectEndLevelCondition : EndLevelCondition {
  private Level level;
  private Dictionary<char, int> counts;
  private Dictionary<char, int> test;
  public CollectEndLevelCondition(Level level, string contains) {
    this.level = level;
    this.counts = new Dictionary<char, int>();
    this.test = new Dictionary<char, int>();
    getCounts(contains, this.counts);
  }

  static void getCounts(string s, Dictionary<char, int> counts) {
    foreach (char c in s) {
      if (c != 'x') {
        int currentCount;
        counts.TryGetValue(c, out currentCount);
        counts[c] = currentCount + 1;
      }
    }
  }


  public bool Check() {
    string collected = level.collected;
    this.test.Clear();
    getCounts(collected, this.test);

    foreach(KeyValuePair<char, int> entry in counts) {
      int testCount;
      this.test.TryGetValue(entry.Key, out testCount);

      if (testCount < entry.Value) {
        return false;
      }
    }
    return true;
  }
}
