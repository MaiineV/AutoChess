using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerManager
{
    private static int L_FLOOR = 8;
    private static int L_WALL = 9;
    private static int L_NODE = 10;

    public static LayerMask LM_FLOOR = 1 << L_FLOOR;
    public static LayerMask LM_WALL = 1 << L_WALL;
    public static LayerMask LM_NODE = 1 << L_NODE;
    
}
