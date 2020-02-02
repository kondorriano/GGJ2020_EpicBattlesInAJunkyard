using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    [SerializeField] Vector3 _exitCarOffset = Vector3.zero;
    [SerializeField] BasePiece _vehicleCore = null;

    Vector2 _leftAxisInput = Vector2.zero;

    PlayerController _playerController;

    public void SetLeftAxis(Vector2 leftInput)
    {
        _leftAxisInput = leftInput;
    }
    public void FixedTick(float fixedDeltaTime)
    {
        ApplyActions();
    }

    void ApplyActions()
    {
        if(_leftAxisInput.x != 0) ApplyInputAction(ActionKey.AxisH1, _leftAxisInput.x);
        if (_leftAxisInput.y != 0) ApplyInputAction(ActionKey.AxisV1, _leftAxisInput.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 7.5f) _playerController.HealthLoss(collision.relativeVelocity.magnitude*0.5f);
    }

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

    public void Init(PlayerController playerController, int vehicleLayer)
    {
        SetLayerRecursive(gameObject, vehicleLayer);
        _playerController = playerController;
    }

    public void ApplyInputAction(ActionKey action, float poweredInput = 1)
    {
        ApplyActionsRecursively(_vehicleCore, action, poweredInput);
    }

    void ApplyActionsRecursively(Piece piece, ActionKey action, float poweredInput)
    {
        if (piece == null) return;

        piece.ApplyAction(action, poweredInput);
        foreach (Piece newPiece in piece.piecesAttachedToMe)
        {
            ApplyActionsRecursively(newPiece, action, poweredInput);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _exitCarOffset, 1);
    }

    internal void LowerVehicle()
    {
        if (_vehicleCore != null) _vehicleCore.Unrise();
    }

    internal void RiseVehicle()
    {
        if (_vehicleCore != null) _vehicleCore.StartRise();
    }
}
