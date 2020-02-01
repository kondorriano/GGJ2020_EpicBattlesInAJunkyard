using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    [SerializeField] Vector3 _exitCarOffset = Vector3.zero;
    public Vector3 ExitPosition
    {
        get { return transform.position + _exitCarOffset; }
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _exitCarOffset, 1);
    }
}
