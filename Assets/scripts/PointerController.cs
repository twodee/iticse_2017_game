using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerController : CellBehavior
{
  private CellBehavior targetCell;
  private LineRenderer lineRenderer;

  override protected void Awake() {
    base.Awake();
    lineRenderer = GetComponent<LineRenderer>();
    targetCell = null;
  }
  
  public CellBehavior Target {
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
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -0.1f));
        lineRenderer.SetPosition(1, new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, -0.1f));
      }
    }
  }

  override public void SetLoot(string text) {
    if (targetCell != null) {
      targetCell.SetLoot(text);
    }
  }

  override public string GetLoot() {
    if (targetCell != null) {
      return targetCell.GetLoot();
    }
    else {
      return null;
    }
  }

}