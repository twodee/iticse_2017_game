using System;
using UnityEngine;

public class PointerTool : Tool {
  public float barbLength;

  private LineRenderer lineRenderer;
  private LineRenderer leftBarbRenderer;
  private LineRenderer rightBarbRenderer;
  private Coroutine caster;
  private Vector2 targetPosition;

  void Awake() {
    lineRenderer = GetComponent<LineRenderer>();
    leftBarbRenderer = transform.Find("leftbarb").GetComponent<LineRenderer>();
    rightBarbRenderer = transform.Find("rightbarb").GetComponent<LineRenderer>();
  }

  bool IsPointerAttached() {
    return caster == null && lineRenderer.enabled;
  }

  public void Update() {
    // Only update pointer if we're not currently sending out a feeler ray.
    if (IsPointerAttached()) {
      Vector2 diff = targetPosition - (Vector2)transform.position;
      diff.Normalize();
//      RaycastHit2D hit = Physics2D.Raycast(transform.position, diff, Mathf.Infinity, Utilities.GROUND_MASK);

      // If our ray hits a different object than it did before, that means some
      // other object got in the way.
//      if (hit.collider.gameObject != targetCell.gameObject &&
//          (targetCell.pointer == null || hit.collider.gameObject != targetCell.pointer.gameObject)) {
//        Depoint();
//      }
//      else {
        Vector2 perp = new Vector3(-diff.y, diff.x);
        lineRenderer.SetPosition(0, (Vector2)transform.position);// + diff * 0.3f);
        leftBarbRenderer.SetPosition(0, targetPosition + barbLength * (perp - 1.5f * diff));
        rightBarbRenderer.SetPosition(0, targetPosition - barbLength * (perp + 1.5f * diff));


//      }
    }
  }

  override public void Interact() {
    GameObject p = player.GetOnPointer();
    if (p == null && player.IsConnectedToOther()) {
//      p = star.GetOnPointer();
    }
    if (p != null) {
      PointerController sourcePointer = p.GetComponent<PointerController>();
      if (player.targetCell == null) {
        // pick up the targetCell from pointer if it exists
        player.targetCell = sourcePointer.Target;
        player.levelController.OnAttach(sourcePointer, player.avatar, true);

        if (player.targetCell != null) {
          // need to draw the line
          lineRenderer.enabled = true;
          leftBarbRenderer.enabled = true;
          rightBarbRenderer.enabled = true;
          caster = null;
          Vector3 pos = player.targetCell.gameObject.transform.position;
          targetPosition = pos;
          PointAt(pos);//new Vector2(pos.x, pos.y - 0.5f));
        }
      }
      else {
        sourcePointer.Target = player.targetCell;
        player.levelController.OnAttach(sourcePointer, player.avatar, false);

        if (IsPointerAttached()) {
          Depoint();
        }
      }
    }
    player.UnLock();
  }

  void Depoint() {
    lineRenderer.enabled = false;
    leftBarbRenderer.enabled = false;
    rightBarbRenderer.enabled = false;
    player.targetCell = null;
  }

  void PointAt(Vector2 position) {
    Vector2 diff = position - (Vector2) transform.position;
    diff.Normalize();
    Vector2 perp = new Vector3(-diff.y, diff.x);
    lineRenderer.SetPosition(0, (Vector2) transform.position);// + diff * 0.3f);
    leftBarbRenderer.SetPosition(0, position + barbLength * (perp - 1.5f * diff));
    rightBarbRenderer.SetPosition(0, position - barbLength * (perp + 1.5f * diff));
    lineRenderer.SetPosition(1, position);
    leftBarbRenderer.SetPosition(1, position);
    rightBarbRenderer.SetPosition(1, position);
  }

  override public void InActive() {
    Depoint();
  }
}

