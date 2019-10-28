using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLinkScript : MonoBehaviour {
  public Texture2D on;
  public Texture2D off;

  private RawImage toggleImage;
  private PointerController pointer;

  // Use this for initialization
  void Start () {
    toggleImage = GetComponent<RawImage>();
    pointer = GetComponentInParent<PointerController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (pointer.Target == null) {
      toggleImage.texture = off;
    }
    else {
      toggleImage.texture = on;
    }
	}
}
