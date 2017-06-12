using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FootController))]
[CanEditMultipleObjects]
public class FootControllerEditor : Editor {
  void OnSceneGUI() {
    FootController foot = (FootController) target; 
    Vector3[] vertices = {
      new Vector3(foot.position.x - foot.width * 0.5f, foot.position.y - foot.height * 0.5f, 0.0f),
      new Vector3(foot.position.x - foot.width * 0.5f, foot.position.y + foot.height * 0.5f, 0.0f),
      new Vector3(foot.position.x + foot.width * 0.5f, foot.position.y + foot.height * 0.5f, 0.0f),
      new Vector3(foot.position.x + foot.width * 0.5f, foot.position.y - foot.height * 0.5f, 0.0f),
    };
    Handles.DrawSolidRectangleWithOutline(vertices, new Color(1, 1, 1, 0.2f), new Color(0, 0, 0, 1));
  }
}
