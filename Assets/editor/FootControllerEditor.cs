using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FootController))]
[CanEditMultipleObjects]
public class FootControllerEditor : Editor {
  void OnSceneGUI() {
    FootController foot = (FootController) target; 
    Handles.color = Color.red;
    Handles.DrawWireDisc(foot.position, Vector3.forward, foot.radius);
  }
}
