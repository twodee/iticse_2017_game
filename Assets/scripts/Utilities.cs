using UnityEngine;
using System.Collections;

public class Utilities {
  public static int GROUND_LAYER = LayerMask.NameToLayer("ground");
  public static int GROUND_MASK = 1 << GROUND_LAYER;
}
