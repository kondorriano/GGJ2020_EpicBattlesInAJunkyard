﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandler : MonoBehaviour
{
    #region Human Data
    [Header("Human Data")]
    [SerializeField] float _constantVelocity = 10;

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
    Piece _attachedPiece = null;
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

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
        _humanRigidbody = GetComponent<Rigidbody2D>();
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
    }

    #region Human Actions
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
            velocity.x = 0;//Mathf.Lerp(velocity.x, 0, .7f);
            _humanRigidbody.velocity = velocity;
        }
        else
        {
            Vector2 velocity = _humanRigidbody.velocity;
            velocity.x += InputDirection.x * _constantVelocity;//_movementAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -_constantVelocity, _constantVelocity);
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

    public void StartJump()
    {
        if (OnAir) return;
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
            return (_attachedJoint == attachedJoint);
        }

        return false;
    }

    public void AttachPiece()
    {
        if (_attachedPiece != null)
        {
            RelativeJoint2D attachedJoint =_attachedPiece.GetComponent<RelativeJoint2D>();
            if (_attachedJoint == attachedJoint)
                Destroy(_attachedJoint);

            _attachedPiece = null;
            _attachedJoint = null;
        }

        if (_pieceSelector.CurrentPiece != null)
        {
            _attachedPiece = _pieceSelector.CurrentPiece;
            _pieceSelector.transform.position = _attachedPiece.transform.position;
            _attachedJoint = _attachedPiece.gameObject.AddComponent<RelativeJoint2D>();
            _attachedJoint.connectedBody = _pieceSelector.GetComponent<Rigidbody2D>();
            _attachedJoint.autoConfigureOffset = false;
            _attachedJoint.correctionScale = 0.1f;
            _attachedJoint.linearOffset = Vector2.zero;
            _attachedJoint.breakForce = 100.0f * _attachedPiece.GetComponent<Rigidbody2D>()?.mass ?? 1.0f;
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
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _pieceSelectorRadius);
    }
}