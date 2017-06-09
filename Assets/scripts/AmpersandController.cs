using UnityEngine;
using System.Collections;

public class AmpersandController : PlayerController {
  public float barbLength;

  private LineRenderer lineRenderer;
  private LineRenderer leftBarbRenderer;
  private LineRenderer rightBarbRenderer;
  private CellController targetCell;
  private Vector2 targetPosition;
  private Coroutine caster;

  override public void Start() {
    base.Start();
    this.Type = "R";
    lineRenderer = GetComponent<LineRenderer>();
    leftBarbRenderer = transform.Find("leftbarb").GetComponent<LineRenderer>();
    rightBarbRenderer = transform.Find("rightbarb").GetComponent<LineRenderer>();
    targetCell = null;
    caster = null;
  }
  
  override public void Update() {
    base.Update();

    // Emit feeler pointer on left-click.
    if (Input.GetMouseButtonDown(0)) {
      if (caster != null) {
        StopCoroutine(caster);
      }
      caster = StartCoroutine(CastPointer()); 
    }

    // Cancel pointer on right-click.
    if (Input.GetMouseButtonDown(1)) {
      Depoint();  
    }

    // Only update pointer if we're not currently sending out a feeler ray.
    if (caster == null && lineRenderer.enabled) {
      Vector2 diff = targetPosition - (Vector2) transform.position;
      diff.Normalize();
      RaycastHit2D hit = Physics2D.Raycast(transform.position, diff, Mathf.Infinity, Utilities.GROUND_MASK);

      // If our ray hits a different object than it did before, that means some
      // other object got in the way.
      if (hit.collider.gameObject != targetCell.gameObject) {
        Depoint();  
      } else {
        Vector2 perp = new Vector3(-diff.y, diff.x);
        lineRenderer.SetPosition(0, (Vector2) transform.position + diff * 0.3f); 
        leftBarbRenderer.SetPosition(0, targetPosition + barbLength * (perp - 1.5f * diff)); 
        rightBarbRenderer.SetPosition(0, targetPosition - barbLength * (perp + 1.5f * diff)); 
      }
    }
  }

  void Depoint() {
    lineRenderer.enabled = false;
    leftBarbRenderer.enabled = false;
    rightBarbRenderer.enabled = false;
    targetCell = null;
  }

  void PointAt(Vector2 position) {
    Vector2 diff = position - (Vector2) transform.position;
    diff.Normalize();
    Vector2 perp = new Vector3(-diff.y, diff.x);
    lineRenderer.SetPosition(0, (Vector2) transform.position + diff * 0.3f); 
    leftBarbRenderer.SetPosition(0, position + barbLength * (perp - 1.5f * diff)); 
    rightBarbRenderer.SetPosition(0, position - barbLength * (perp + 1.5f * diff)); 
    lineRenderer.SetPosition(1, position); 
    leftBarbRenderer.SetPosition(1, position); 
    rightBarbRenderer.SetPosition(1, position); 
  }

  IEnumerator CastPointer() {
    lineRenderer.enabled = true;
    leftBarbRenderer.enabled = true;
    rightBarbRenderer.enabled = true;

    Vector3 mousePixels = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
    Vector2 to = Camera.main.ScreenToWorldPoint(mousePixels);
    Vector2 from = transform.position;
    float maximumLength = Vector2.Distance(from, to);
    Vector2 diff = to - from;
    diff.Normalize();

    /* Ray ray = new Ray(from, diff); */
    /* float maximumLength; */
    /* float cameraHeight = Camera.main.orthographicSize; */
    /* float cameraWidth = cameraHeight * Camera.main.aspect; */
    /* Bounds bounds = new Bounds((Vector2) Camera.main.transform.position, new Vector2(cameraWidth, cameraHeight)); */
    /* Debug.Log("ray: " + ray); */
    /* Debug.Log("bounds: " + bounds); */
    /* bounds.IntersectRay(ray, out maximumLength); */
    /* Debug.Log("maximumLength: " + maximumLength); */

    float startTime = Time.time;
    float elapsedTime = 0.0f;
    float targetTime = 0.5f;
    bool isHit = false;
    targetCell = null;

    RaycastHit2D hit;
    while (elapsedTime < targetTime && !isHit) {
      float proportion = elapsedTime / targetTime;
      float length = proportion * maximumLength;
      hit = Physics2D.Raycast(from, diff, length, Utilities.GROUND_MASK);
      if (hit.collider != null) {
        PointAt(hit.point); 
        targetCell = hit.collider.gameObject.GetComponent<CellController>();
        targetPosition = hit.point;
        isHit = true;
      } else {
        PointAt(from + diff * length); 
      }
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    lineRenderer.enabled = targetCell != null;
    leftBarbRenderer.enabled = targetCell != null;
    rightBarbRenderer.enabled = targetCell != null;
    caster = null;
  }

  public CellController Target {
    get {
      return targetCell;
    }
  }
}
