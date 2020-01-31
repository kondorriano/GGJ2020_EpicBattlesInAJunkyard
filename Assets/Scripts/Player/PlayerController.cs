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

    Vector2 _inputDirection;
    Vector2 _inputPieceDirection;
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
        FixedTick(Time.fixedDeltaTime);
    }

    void FixedTick(float fixedDeltaTime)
    {
        ApplyHumanMovement(fixedDeltaTime);
    }

    #region Human Actions
    void ApplyHumanMovement(float fixedDeltaTime)
    {
        if (_inputDirection.x == 0)
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
    void ApplyHumanJump()
    {
        _humanRigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
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
            Debug.Log("JAMPU");
            ApplyHumanJump();
        }        
    }

    public void RotatePieceEvent(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger " + context.ReadValue<float>());
    }
    #endregion
}
