using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    private int _playerId;

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        var piece = go.GetComponent<Piece>();
        if (piece != null)
            go.layer = layer;

        foreach (Transform t in go.transform)
            SetLayerRecursive(t.gameObject, layer);
    }

    public void Init(int PlayerId)
    {
        _playerId = PlayerId;
        int layer = LayerMask.NameToLayer(string.Format("Vehicle0{0}", _playerId + 1));
        SetLayerRecursive(gameObject, layer);
    }
}
