using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Human Data
    [Header("Human Data")]
    [SerializeField] HumanHandler _humanHandler = null;
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
        get { return _humanHandler?.transform; }
    }

    public IEnumerable<MeshRenderer> ColorRenderers
    {
        get
        {
            yield return _humanHandler?.GetComponent<MeshRenderer>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init(_playerID);
    }

    public void Init(int playerID)
    {
        _playerID = playerID;
        _humanHandler.Init(this);
    }

    private void FixedUpdate()
    {
        if(OutsideVehicle) _humanHandler.FixedTick(Time.fixedDeltaTime);
    }    

    #region Input Human Events
    public void MovePlayerEvent(InputAction.CallbackContext context)
    {
        _humanHandler.InputDirection = context.ReadValue<Vector2>();
    }

    public void MovePieceEvent(InputAction.CallbackContext context)
    {
        _humanHandler.InputPieceDirection = context.ReadValue<Vector2>();
    }

    public void InteractEvent(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            _humanHandler.AttachPiece();
        }
    }

    public void JumpEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _humanHandler.StartJump();
        }
        else if(context.canceled)
        {
            _humanHandler.JumpUp();
        }
    }


    public void RotatePieceEvent(InputAction.CallbackContext context)
    {
        _humanHandler.PieceRotationDirection = context.ReadValue<float>();
    }
    #endregion

    
}
