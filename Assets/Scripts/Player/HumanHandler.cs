﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandler : MonoBehaviour
{
    #region Human Data
    [Header("Human Data")]
    [SerializeField] float _acceleration = 20;
    [SerializeField] float _deceleration = 40;
    [SerializeField] float _maximumVelocity = 10;

    [SerializeField] float _jumpForce = 4;
    [SerializeField] float _jumpMaxTime = .35f;
    [SerializeField] float _jumpMinTime = .15f;

    [Header("Piece Selector Data")]
    [SerializeField] SelectorHandler _pieceSelector = null;
    [SerializeField] float _pieceConstantVelocity = 10;
    [SerializeField] float _selectorRotationForce = 135f;
    [SerializeField] float _pieceSelectorRadius = 4;

    //JUMP
    private float _jumpTime;
    private float _jumpCounter;
    Vector2 _jumpDirection;
    bool _jumping = false;
    bool _cannotJump = false;
    const float AIRTHRESHOLD = 0.2f;
    bool OnAir
    {
        get { return accumCollisionCount < AIRTHRESHOLD; }
    }

    //MOVEMENT
    public Vector2 InputDirection { get; set; }
    public Vector3 InputPieceDirection { get; set; }

    //SELECTOR
    int _pieceLayer;
    int _vehicleLayer;
    Piece _attachedPiece = null;
    Rigidbody2D _attachedOverlapping = null;
    public Rigidbody2D AttachedOverlapping
    {
        get { return _attachedOverlapping; }
    }
    RelativeJoint2D _attachedJoint = null;
    float _pieceRotationDirection = 0;
    public float PieceRotationDirection
    {
        get { return _pieceRotationDirection; }
        set { _pieceRotationDirection = value; }
    }
#endregion

PlayerController _playerController;
    Rigidbody2D _humanRigidbody;

    public void Init(PlayerController playerController, int pieceLayer, int vehicleLayer)
    {
        _playerController = playerController;
        _humanRigidbody = GetComponent<Rigidbody2D>();
        _pieceLayer = pieceLayer;
        _vehicleLayer = vehicleLayer;
    }

    public void FixedTick(float fixedDeltaTime)
    {
        HandleCollision();

        //SELECTOR
        ApplyPieceSelectorMovement(fixedDeltaTime);
        ApplySelectorRotation(fixedDeltaTime);

        HandleJump(fixedDeltaTime);
        ApplyMovement(fixedDeltaTime);
        
        //ON AIR
        if (_jumping)
        {
            ApplyJump();
        }

        HasPieceAttached();
        HandleGrabbedPiece();
    }

    #region Human Actions
    IEnumerable<RaycastHit2D> RaysFromBoundsAndDistance(Bounds bb, float dist)
    {
        Vector2 center = bb.center;
        yield return Physics2D.Raycast(center, new Vector2(bb.min.x, bb.min.y) - center, dist, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.min.x, bb.max.y) - center, dist, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.max.x, bb.max.y) - center, dist, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.max.x, bb.min.y) - center, dist, 1 << _vehicleLayer);

        yield return Physics2D.Raycast(center, new Vector2(bb.min.x, bb.center.y) - center, bb.extents.x, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.max.x, bb.center.y) - center, bb.extents.x, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.center.x, bb.min.y) - center, bb.extents.y, 1 << _vehicleLayer);
        yield return Physics2D.Raycast(center, new Vector2(bb.center.x, bb.max.y) - center, bb.extents.y, 1 << _vehicleLayer);
    }

    void HandleGrabbedPiece()
    {
        if (_attachedPiece == null)
        {
            _playerController.LowerVehicle();
            _attachedOverlapping = null;
            return;
        }

        if (_playerController.DistanceHumanVehicle() < 10) _playerController.RiseVehicle();

        Collider2D collider = _attachedPiece.GetComponent<Collider2D>();
        if (collider == null) return;

        var bb = collider.bounds;
        float dist = Vector3.Distance(bb.min, bb.center);

        float distMin = float.PositiveInfinity;
        Rigidbody2D rb = null;
        foreach(RaycastHit2D r in RaysFromBoundsAndDistance(bb, dist))
        {
            if (r && r.distance < distMin)
            {
                distMin = r.distance;
                rb = r.rigidbody;
            }
        }

        _attachedOverlapping = rb;
    }

    void ApplyPieceSelectorMovement(float fixedDeltaTime)
    {
        Vector3 localPosition = _pieceSelector.transform.localPosition;
        localPosition += InputPieceDirection * _pieceConstantVelocity * fixedDeltaTime;
        _pieceSelector.transform.localPosition = Vector3.ClampMagnitude(localPosition, _pieceSelectorRadius);
    }

    void ApplySelectorRotation(float fixedDeltaTime)
    {
        _pieceSelector.transform.Rotate(-Vector3.forward * PieceRotationDirection * _selectorRotationForce * fixedDeltaTime, Space.Self);
    }

    void HandleCollision()
    {
        const float accumFactor = 0.75f;
        accumCollision *= accumFactor;
        accumCollisionCount *= accumFactor;
    }

    void ApplyMovement(float fixedDeltaTime)
    {
        if (Mathf.Abs(InputDirection.x) <= .1f)
        {
            Vector2 velocity = _humanRigidbody.velocity;
            if(velocity.x > 0)
            {
                velocity.x -= _deceleration * fixedDeltaTime;
                if (velocity.x < 0) velocity.x = 0;
            }
            else if (velocity.x < 0)
            {
                velocity.x += _deceleration * fixedDeltaTime;
                if (velocity.x > 0) velocity.x = 0;
            }
            _humanRigidbody.velocity = velocity;
        }
        else
        {
            Vector2 velocity = _humanRigidbody.velocity;
            velocity.x += InputDirection.x * _acceleration * fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -_maximumVelocity, _maximumVelocity);
            _humanRigidbody.velocity = velocity;
        }
    }
    void HandleJump(float fixedDeltaTime)
    {
        if (!_jumping) return;
        _jumpCounter += fixedDeltaTime;
        _jumping = (_jumpCounter < _jumpTime);
    }
    void ApplyJump()
    {
        Vector2 velocity = _humanRigidbody.velocity;
        float deficit = Vector2.Dot(velocity.normalized, _jumpDirection);
        velocity -= _jumpDirection * deficit * velocity.magnitude;
        velocity += _jumpDirection * _jumpForce;
        _humanRigidbody.velocity = velocity;
    }

    public void StartJump(bool forced = false)
    {
        if (OnAir && !forced) return;
        _jumpDirection = normalizedCollision.normalized;
        ApplyJump();
        _jumping = true;
        _jumpTime = _jumpMaxTime;
        _jumpCounter = 0;

        normalizedCollision = accumCollision = Vector2.zero;
        accumCollisionCount = 0;
        _cannotJump = true;
    }

    public void JumpUp()
    {
        if (_jumping) _jumpTime = _jumpMinTime;
    }

    public bool HasPieceAttached()
    {
        if (_attachedPiece != null)
        {
            RelativeJoint2D attachedJoint = _attachedPiece.GetComponent<RelativeJoint2D>();
            if (_attachedJoint != attachedJoint)
            {
                _attachedJoint = null;
                _attachedPiece = null;
            }

            return (_attachedJoint != null);
        }

        return false;
    }
    public void AttachPiece()
    {
        if (_attachedPiece == null) return;

        RelativeJoint2D attachedJoint = _attachedPiece.GetComponent<RelativeJoint2D>();
        if (_attachedJoint == attachedJoint)
        {
            //_attachedPiece.Unattach();
            Destroy(_attachedJoint);

            _attachedPiece.gameObject.layer = _vehicleLayer;
            _attachedPiece.AttachToRB(_attachedOverlapping, _playerController);

            _attachedPiece = null;
            _attachedJoint = null;
        }
    }

    public bool IsSelectedPieceIsMyBase
    {
        get
        {
            return !HasPieceAttached()
                && _pieceSelector.CurrentPiece != null
                && _pieceSelector.CurrentPiece is BasePiece
                && _pieceSelector.CurrentPiece.gameObject.layer == _vehicleLayer;
        }
    }

    public void GrabPiece()
    {
        if (_attachedPiece != null)
        {
            RelativeJoint2D attachedJoint = _attachedPiece.GetComponent<RelativeJoint2D>();
            if (_attachedJoint == attachedJoint)
            {
                _attachedPiece.Unattach();
                Destroy(_attachedJoint);
            }

            _attachedPiece = null;
            _attachedJoint = null;
        }

        if (_pieceSelector.CurrentPiece != null && !(_pieceSelector.CurrentPiece is BasePiece))
        {
            _attachedPiece = _pieceSelector.CurrentPiece;
            _attachedPiece.Unattach();
            _attachedPiece.gameObject.layer = _pieceLayer;
            _pieceSelector.transform.position = _attachedPiece.transform.position;
            _attachedJoint = _attachedPiece.gameObject.AddComponent<RelativeJoint2D>();
            _attachedJoint.connectedBody = _pieceSelector.GetComponent<Rigidbody2D>();
            _attachedJoint.autoConfigureOffset = false;
            _attachedJoint.correctionScale = 0.1f;
            _attachedJoint.linearOffset = Vector2.zero;
            _attachedJoint.breakForce = 500.0f * _attachedPiece.GetComponent<Rigidbody2D>()?.mass ?? 1.0f;
        }
    }
    #endregion

    #region Collision Detection
    Vector2 accumCollision = Vector2.zero;
    float accumCollisionCount;
    Vector2 normalizedCollision;

    private ContactPoint2D[] contacts = new ContactPoint2D[16];
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_cannotJump) return;

        int length = collision.GetContacts(contacts);
        for (int i = 0; i < length; i++)
        {
            accumCollision += contacts[i].normal;
            accumCollisionCount += 1f;
        }

        normalizedCollision = accumCollision / accumCollisionCount;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _cannotJump = false;

        //Debug.Log("Enter");

        int length = collision.GetContacts(contacts);
        for (int i = 0; i < length; i++)
        {
            accumCollision += contacts[i].normal * 2;
            accumCollisionCount += 2f;
        }

        normalizedCollision = accumCollision / accumCollisionCount;


        if (collision.collider.gameObject.layer != _vehicleLayer && (collision.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle1") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle2") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle3") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle4")))
        {
            if (collision.relativeVelocity.magnitude > 15) _playerController.HealthLoss(collision.relativeVelocity.magnitude * 3f);
        }
    }
    #endregion

    private static void GizmoDrawRay(Vector2 origin, Vector2 direction, float distance, int layer)
    {
        Gizmos.DrawLine(origin, origin + (direction.normalized * distance));
    }

    private void OnDrawGizmos()
    {
        if (_attachedPiece != null)
        {
            Collider2D collider = _attachedPiece.GetComponent<Collider2D>();
            if (collider != null)
            {
                var bb = collider.bounds;
                float dist = Vector3.Distance(bb.min, bb.center);

                var restoreColor = Gizmos.color;
                Gizmos.color = Color.red;
                Vector2 center = bb.center;
                GizmoDrawRay(center, new Vector2(bb.min.x, bb.min.y) - center, dist, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.min.x, bb.max.y) - center, dist, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.max.x, bb.max.y) - center, dist, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.max.x, bb.min.y) - center, dist, 1 << _vehicleLayer);

                Gizmos.color = Color.green;
                GizmoDrawRay(center, new Vector2(bb.min.x, bb.center.y) - center, bb.extents.x, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.max.x, bb.center.y) - center, bb.extents.x, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.center.x, bb.min.y) - center, bb.extents.y, 1 << _vehicleLayer);
                GizmoDrawRay(center, new Vector2(bb.center.x, bb.max.y) - center, bb.extents.y, 1 << _vehicleLayer);

                Gizmos.color = restoreColor;
            }
        }

        Gizmos.DrawWireSphere(transform.position, _pieceSelectorRadius);
    }
}
