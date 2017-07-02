using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
  private CellController targetCell;
  private LineRenderer lineRenderer;

  void Awake() {
    lineRenderer = GetComponent<LineRenderer>();
    targetCell = null;
  }
  
  public CellController Target {
    get {
      return targetCell;
    }

    set {
      targetCell = value;
      if (targetCell == null) {
        lineRenderer.enabled = false;
      }
      else {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, (Vector2)transform.position);
        lineRenderer.SetPosition(1, (Vector2)targetCell.transform.position);
      }
    }
  }

}