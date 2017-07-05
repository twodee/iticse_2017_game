using System;
using System.Collections;
using UnityEngine;

public class CellArray {
  public ArrayList objects;
  public CellArray() {
    objects = new ArrayList();
  }

  public void Add(GameObject go) {
    objects.Add(go);
  }

  public int Count {
    get {
      return objects.Count;
    }
  }

  public GameObject Get(int index) {
    return (GameObject)objects[index];
  }
}

