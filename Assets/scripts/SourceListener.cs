public interface SourceListener {
  // These methods are used to resolve the right-hand side of += and = operations.
  int Evaluate(bool isStarred, string identifier);
  int Evaluate(bool isStarred, string identifier, int subscriptInteger);

  // These methods handle += operations.
  void OnIncrement(string command, bool isStarred, string identifier, int delta);
  void OnIncrement(string command, bool isStarred, string identifier, int subscriptInteger, int delta);

  // These methods handle = operations.
  void OnAssign(string command, bool isStarred, string identifier, int delta);
  void OnAssign(string command, bool isStarred, string identifier, int subscriptInteger, int delta);

  void OnFail(string command);
}
