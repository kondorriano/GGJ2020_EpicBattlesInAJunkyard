using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;

        foreach (Transform t in go.transform)
            SetLayerRecursive(t.gameObject, layer);
    }

    public void Init(int vehicleLayer)
    {
        SetLayerRecursive(gameObject, vehicleLayer);
    }
}
