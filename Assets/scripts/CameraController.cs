using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
  public Transform player1;
  public Transform player2;
  public float minSizeY = 4.5f;
  private Camera myCamera;
  
  void Start() {
    player1 = GameObject.Find("/players/star").transform;
    player2 = GameObject.Find("/players/ampersand").transform;
    myCamera = GetComponent<Camera>();
    
  }


  void SetCameraPos() {
    Vector3 mid1 = player1.position - myCamera.transform.position;
    Vector3 mid2 = player2.position - myCamera.transform.position;

    float testX = myCamera.orthographicSize* Screen.width / Screen.height - 0.5f;

    float diffX = 0;
    if (Mathf.Abs(mid1.x) > testX && Mathf.Abs(mid1.x) > Mathf.Abs(mid2.x)) {
      diffX = mid1.x;
    }
    else if (Mathf.Abs(mid2.x) > testX) {
      diffX = mid2.x;
    }
    if (diffX != 0) { 
      myCamera.transform.position = new Vector3(
          myCamera.transform.position.x + diffX - Mathf.Sign(diffX)*testX,
          myCamera.transform.position.y,
          myCamera.transform.position.z
      );

    }
  }

  void SetCameraSize() {
    //horizontal size is based on actual screen ratio
    float minSizeX = minSizeY * Screen.width / Screen.height;


    //multiplying by 0.5, because the ortographicSize is actually half the height
    float width = Mathf.Abs(player1.position.x - player2.position.x) * 0.5f + 1.0f;
    float height = Mathf.Abs(player1.position.y - player2.position.y) * 0.5f;

    //computing the size
    float camSizeX = Mathf.Max(width, minSizeX);
    myCamera.orthographicSize = Mathf.Max(height,
        camSizeX * Screen.height / Screen.width, minSizeY);
  }

  void Update() {
	//SetCameraSize();
    //SetCameraPos();
  }
  
}
