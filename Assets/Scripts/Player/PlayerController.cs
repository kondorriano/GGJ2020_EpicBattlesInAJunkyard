using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Human Data
    [Header("Human Data")]
    [SerializeField] Rigidbody2D _humanRigidbody = null;

    [SerializeField] float _constantVelocity = 10;

    [SerializeField] float _jumpForce = 4;
    [SerializeField] float _jumpMaxTime = .15f;
    [SerializeField] float _jumpMinTime = .25f;

    [Header("Piece Selector Data")]
    [SerializeField] Transform _pieceSelector = null;
    [SerializeField] float _pieceConstantVelocity = 10;
    [SerializeField] float _pieceSelectorRadius = 4;


    private float _jumpTime;
    private float _jumpCounter;
    bool _jumping = false;

    Vector2 _inputDirection;
    Vector3 _inputPieceDirection;
    #endregion

    #region Vehicle Data
    [Header("Vehicle Data")]
    #endregion

    #region General
    int _playerID = 0;
    bool _outsideVehicle = true;
    bool OutsideVehicle
    {
        get { return _outsideVehicle; }
        set
        {
            _outsideVehicle = value;
            //SetInputSystem
            Debug.Log("Swap Input System");
        }
    }
    #endregion

    public Transform ToFollow
    {
        get { return _humanRigidbody?.transform; }
    }

    public MeshRenderer HumanMeshRenderer
    {
        get { return _humanRigidbody?.GetComponent<MeshRenderer>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init(_playerID);
    }

    public void Init(int playerID)
    {
        _playerID = playerID;
    }

    private void FixedUpdate()
    {
        if(OutsideVehicle) FixedHumanTick(Time.fixedDeltaTime);
    }

    void FixedHumanTick(float fixedDeltaTime)
    {
        ApplyPieceSelectorMovement(fixedDeltaTime);

        HandleJump(fixedDeltaTime);
        ApplyHumanMovement(fixedDeltaTime);
        //ON AIR
        if(_jumping)
        {
            ApplyHumanJump();
        }
    }

    #region Human Actions
    void ApplyPieceSelectorMovement(float fixedDeltaTime)
    {
        Vector3 localPosition = _pieceSelector.transform.localPosition;
        localPosition += _inputPieceDirection * _pieceConstantVelocity * fixedDeltaTime;
        _pieceSelector.transform.localPosition = Vector3.ClampMagnitude(localPosition, _pieceSelectorRadius);
    }

    void ApplyHumanMovement(float fixedDeltaTime)
    {
        if (Mathf.Abs(_inputDirection.x) <= .1f)
        {
            Vector2 velocity = _humanRigidbody.velocity;
            velocity.x = 0;//Mathf.Lerp(velocity.x, 0, .7f);
            _humanRigidbody.velocity = velocity;
        }
        else
        {
            Vector2 velocity = _humanRigidbody.velocity;
            velocity.x += _inputDirection.x * _constantVelocity;//_movementAcceleration * Time.fixedDeltaTime;
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
    void ApplyHumanJump()
    {
        Vector2 velocity = _humanRigidbody.velocity;
        velocity.y = _jumpForce ;
        _humanRigidbody.velocity = velocity;
    }

    void StartJump()
    {
        ApplyHumanJump();
        _jumping = true;
        _jumpTime = _jumpMaxTime;
        _jumpCounter = 0;
    }
    #endregion

    #region Input Human Events
    public void MovePlayerEvent(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    public void MovePieceEvent(InputAction.CallbackContext context)
    {
        _inputPieceDirection = context.ReadValue<Vector2>();
    }

    public void InteractEvent(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Debug.Log("Detect piece?");
        }
    }

    public void JumpEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartJump();
        }
        else if(context.canceled)
        {
            if (_jumping) _jumpTime = _jumpMinTime;
        }
    }


    public void RotatePieceEvent(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger " + context.ReadValue<float>());
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_humanRigidbody.transform.position, _pieceSelectorRadius);
    }
}
