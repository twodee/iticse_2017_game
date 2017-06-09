using UnityEngine;
using System.Collections;

public class Utilities {
  public static int GROUND_LAYER = LayerMask.NameToLayer("ground");
  public static int GROUND_MASK = 1 << GROUND_LAYER;
  public static int PLAYER_R_LAYER = LayerMask.NameToLayer("playerR");
  public static int PLAYER_R_MASK = 1 << PLAYER_R_LAYER;
  public static int PLAYER_D_LAYER = LayerMask.NameToLayer("playerD");
  public static int PLAYER_D_MASK = 1 << PLAYER_D_LAYER;
}
