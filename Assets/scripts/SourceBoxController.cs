using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

// To use this class, add an InputField to your scene, and attach this script
// to it. It will automatically add a listener to process commands typed in the
// box. Blurring focus and hitting enter both trigger the processing.
//
// Some class that can respond to the source code entry should implement the
// SourceListener interface and register itself with this script. Only one
// listener can be registered.

public class SourceBoxController : MonoBehaviour {
  private static readonly string lvalue = @"(\*\s*)?([a-z]+)(?:\s*\[\s*(-?\d+|[a-z]+)\s*\])?";
  private static readonly string integer = @"(-?\d+)";

  private static readonly Regex integerX = new Regex(@"^" + integer + @"$");
  private static readonly Regex assignmentX = new Regex(@"^\s*" + lvalue + @"\s*=\s*(?:" + lvalue + @"|" + integer + @")\s*$");
  private static readonly Regex unaryIncrementX = new Regex(@"^\s*(?:(--|\+\+)\s*" + lvalue + @"|" + lvalue + @"\s*(--|\+\+))\s*$");
  private static readonly Regex binaryIncrementX = new Regex(@"^\s*" + lvalue + @"\s*([-+])=\s*(?:" + lvalue + @"|" + integer + @")\s*$");
 
  // TODO: Have client wire up a real listener.
  public SourceListener listener = new SourceListenerImplementation();

  private InputField input;

  void Start() {
    input = GetComponent<InputField>(); 
    input.onEndEdit.AddListener(OnInput);
  }

  // Handle arbitrary command.
  void OnInput(string command) {
    bool isValid = true;

    if (assignmentX.IsMatch(command)) {
      processAssignment(command);
    } else if (binaryIncrementX.IsMatch(command)) {
      processBinaryIncrement(command);
    } else if (unaryIncrementX.IsMatch(command)) {
      processUnaryIncrement(command);
    } else {
      listener.OnFail(command);
      isValid = false;
    }

    // If the input was okay, let's clear the field to get ready for
    // the next command. If it wasn't, we leave the text for them to
    // edit and resubmit.
    if (isValid) {
      input.text = "";
    }

    // Unity triggers this event on two occasions: hitting enter and losing
    // focus. If they hit enter, we expect the user to want to type in some
    // next command -- like in a REPL. We must actively restore focus in such a
    // case.
    if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
      input.ActivateInputField();
    }
  }

  // Handle a = b.
  private void processAssignment(string command) {
    Match match = assignmentX.Match(command);

    bool isStarred = match.Groups[1].Value.Length > 0;
    string identifier = match.Groups[2].Value;
    string subscript = match.Groups[3].Value;
    int right = ResolveRight(match, 4);

    // Now we want lhs = right.
    if (subscript.Length == 0) {
      listener.OnAssign(command, isStarred, identifier, right);
    } else {
      int index = ResolveIndex(subscript);
      listener.OnAssign(command, isStarred, identifier, index, right); 
    }
  }

  // Handle a++, --b.
  private void processUnaryIncrement(string command) {
    Match match = unaryIncrementX.Match(command);

    string identifier;
    bool isIncrement;
    bool isStarred = false;
    bool isPre = match.Groups[1].Value.Length > 0;
    string subscript = null;

    if (isPre) { // is ++p
      isIncrement = match.Groups[1].Value == "++";
      isStarred = match.Groups[2].Value.Length > 0;
      identifier = match.Groups[3].Value;   
      subscript = match.Groups[4].Value;
    } else { // is p++
      isIncrement = match.Groups[8].Value == "++";
      isStarred = match.Groups[5].Value.Length > 0;
      identifier = match.Groups[6].Value;   
      subscript = match.Groups[7].Value;
    }

    if (subscript.Length == 0) { // no subscript
      listener.OnIncrement(command, isStarred, identifier, isIncrement ? 1 : -1); 
    } else {
      int index = ResolveIndex(subscript);
      listener.OnIncrement(command, isStarred, identifier, index, isIncrement ? 1 : -1); 
    }
  }

  // Handle a += b, b -= a.
  private void processBinaryIncrement(string command) {
    Match match = binaryIncrementX.Match(command);

    bool isStarred = match.Groups[1].Value.Length > 0;
    string identifier = match.Groups[2].Value;
    string subscript = match.Groups[3].Value;
    int right = ResolveRight(match, 5);

    // a -= b is treated as a += -b.
    bool isMinus = match.Groups[4].Value == "-";
    if (isMinus) {
      right = -right;
    }

    // Now we want lhs += right.
    if (subscript.Length == 0) {
      listener.OnIncrement(command, isStarred, identifier, right);
    } else {
      int index = ResolveIndex(subscript);
      listener.OnIncrement(command, isStarred, identifier, index, right); 
    }
  }

  // Determine the value of a subscript, which is either an integer or
  // an identifier.
  private int ResolveIndex(string subscript) {
    if (integerX.IsMatch(subscript)) {
      return int.Parse(subscript);
    } else {
      return listener.Evaluate(false, subscript);
    }
  }

  // Determine the value of the right-hand side, which can be one of two
  // things: an integer or an expression involving an identifier. The latter
  // can actually be three things: a raw identifier, an identifier with an
  // integer subscript, or an identifier with another identifier as its
  // subscript. So really, four things.
  private int ResolveRight(Match match, int iStar) {
    int right;

    if (match.Groups[iStar + 3].Value.Length > 0) { // integer
      right = int.Parse(match.Groups[iStar + 3].Value);
    } else { // expression involving identifier
      bool rightIsStarred = match.Groups[iStar].Value == "*";
      string rightIdentifier = match.Groups[iStar + 1].Value;
      string rightSubscript = match.Groups[iStar + 2].Value;

      if (rightSubscript.Length == 0) { // p
        right = listener.Evaluate(rightIsStarred, rightIdentifier);
      } else if (integerX.IsMatch(rightSubscript)) { // p[17]
        int index = int.Parse(rightSubscript);
        right = listener.Evaluate(rightIsStarred, rightIdentifier, index);
      } else { // p[q]
        int index = listener.Evaluate(false, rightSubscript);
        right = listener.Evaluate(rightIsStarred, rightIdentifier, index);
      }
    }

    return right;
  }

  // Determines if the input box is focused. If your scripts are listening for
  // key events, you probably don't want to receive them when the player is
  // typing in the box. In those scripts, use this property to assert that the
  // box isn't focused:
  //
  //   if (!inputController.IsFocused && Input.GetKeyDown(KeyCode.A))
  //     ...
  public bool IsFocused {
    get {
      return input.isFocused;
    }
  }
}
