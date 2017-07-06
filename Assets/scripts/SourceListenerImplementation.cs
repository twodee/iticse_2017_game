using UnityEngine;

public class SourceListenerImplementation : SourceListener {
  public void OnIncrement(string command, bool isStarred, string identifier, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + " += " + right);
  }

  public void OnIncrement(string command, bool isStarred, string identifier, int subscriptInteger, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + "[" + subscriptInteger + "] += " + right);
  }

  public void OnIncrement(string command, bool isStarred, string identifier, string subscriptIdentifier, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + "[" + subscriptIdentifier + "] += " + right);
  }

  public int Evaluate(bool isStarred, string identifier) {
    return isStarred ? 10 : 20;
  }

  public int Evaluate(bool isStarred, string identifier, int index) {
    return isStarred ? 11 : 21;
  }

  public void OnAssign(string command, bool isStarred, string identifier, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + " = " + right);
  }

  public void OnAssign(string command, bool isStarred, string identifier, int subscriptInteger, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + "[" + subscriptInteger + "] = " + right);
  }

  public void OnAssign(string command, bool isStarred, string identifier, string subscriptIdentifier, int right) {
    MonoBehaviour.print(command + " | " + (isStarred ? "*" : "") + identifier + "[" + subscriptIdentifier + "] = " + right);
  }

  public void OnFail(string command) {
    MonoBehaviour.print("Bad command: " + command);
  }
}
