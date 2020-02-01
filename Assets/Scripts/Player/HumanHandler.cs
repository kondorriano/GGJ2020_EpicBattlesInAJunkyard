using System.Collections;
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
    [SerializeField] Transform _pieceSelector = null;
    [SerializeField] float _pieceConstantVelocity = 10;
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
        ApplyPieceSelectorMovement(fixedDeltaTime);

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
        float deficit = Vector2.Dot(velocity.normalized, _jumpDirection.normalized);
        velocity -= _jumpDirection.normalized * deficit * _jumpForce;
        velocity += _jumpDirection * _jumpForce;
        _humanRigidbody.velocity = velocity;
    }

    public void StartJump()
    {
        Debug.Log(OnAir);
        if (OnAir) return;
        _jumpDirection = normalizedCollision;
        ApplyJump();
        _jumping = true;
        _jumpTime = _jumpMaxTime;
        _jumpCounter = 0;

        normalizedCollision = accumCollision = Vector3.zero;
        accumCollisionCount = 0;
        _cannotJump = true;
    }

    public void JumpUp()
    {
        if (_jumping) _jumpTime = _jumpMinTime;
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

        //Debug.Log("Stay");
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
