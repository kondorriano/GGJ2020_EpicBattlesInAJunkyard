using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    int _playerID = 0;
    [SerializeField] float _movementAcceleration = 1;
    [SerializeField] float _maximumVelocity = 10;
    Vector2 _inputDirection;
    Vector2 _inputPieceDirection;

    Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        Init(_playerID);
    }

    public void Init(int playerID)
    {
        _playerID = playerID;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        FixedTick(Time.fixedDeltaTime);
    }

    void FixedTick(float fixedDeltaTime)
    {
        ApplyMovement(fixedDeltaTime);
    }

    void ApplyMovement(float fixedDeltaTime)
    {
        if(_inputDirection.x == 0)
        {
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = 0;//Mathf.Lerp(velocity.x, 0, .7f);
            _rigidbody.velocity = velocity;
        }
        else
        {
            Vector2 velocity = _rigidbody.velocity;
            velocity.x += _inputDirection.x * _maximumVelocity;//_movementAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x , - _maximumVelocity, _maximumVelocity);
            _rigidbody.velocity = velocity;
        }
    }

    void ApplyJump()
    {
        _rigidbody.AddForce(Vector2.up*3, ForceMode2D.Impulse);
    }

    #region Input Events
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
            ApplyJump();
        }        
    }

    public void RotatePieceEvent(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger " + context.ReadValue<float>());
    }
    #endregion
}
