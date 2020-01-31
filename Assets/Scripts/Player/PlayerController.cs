using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    int _playerID = 0;
    [SerializeField] float _movementAcceleration = 1;
    [SerializeField] float _maximumVelocity = 10;
    Vector2 _movementDirection;
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
        CalculateMovementDirection();
        ApplyMovement(fixedDeltaTime);
    }

    void CalculateMovementDirection()
    {
        _movementDirection.x = Input.GetAxis("Horizontal");
    }

    void ApplyMovement(float fixedDeltaTime)
    {
        if(_movementDirection.x == 0)
        {
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = Mathf.Lerp(velocity.x, 0, .7f);
            _rigidbody.velocity = velocity;
        }
        else
        {
            Vector2 velocity = _rigidbody.velocity;
            velocity.x += _movementDirection.x * _movementAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x , - _maximumVelocity, _maximumVelocity);
            _rigidbody.velocity = velocity;
        }
    }
}
