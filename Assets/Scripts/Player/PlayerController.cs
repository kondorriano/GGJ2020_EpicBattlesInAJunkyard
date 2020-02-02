using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionKey
{
    AxisH1,
    AxisH2,
    AxisV1,
    AxisV2,
    A,
    B,
    X,
    Y,
    LT,
    RT,
    LB,
    RB
}

public class PlayerController : MonoBehaviour
{
    #region Human Data
    [Header("Human Data")]
    [SerializeField] HumanHandler _humanHandler = null;
    #endregion

    #region Vehicle Data
    [Header("Vehicle Data")]
    [SerializeField] VehicleHandler _vehicleHandler = null;
    private float _power = 3;
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
        }
    }
    #endregion

    public Transform ToFollow
    {
        get 
        {
            if (OutsideVehicle)
                return _humanHandler?.transform;
            else
                return _vehicleHandler?.transform;
        }
    }

    public IEnumerable<MeshRenderer> ColorRenderers
    {
        get
        {
            yield return _humanHandler?.GetComponent<MeshRenderer>();
            yield return _vehicleHandler?.GetComponent<MeshRenderer>();
        }
    }

    PlayerInput _playerInput;

    // Start is called before the first frame update
    public void Init(int playerID)
    {
        _playerID = playerID;
        int vehicleLayer = LayerMask.NameToLayer(string.Format("Vehicle{0}", _playerID + 1));
        int pieceLayer = LayerMask.NameToLayer(string.Format("Grabbed{0}", _playerID + 1));

        _humanHandler.Init(this, pieceLayer, vehicleLayer);
        _vehicleHandler.Init(vehicleLayer);

        _playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        if (OutsideVehicle) _humanHandler.FixedTick(Time.fixedDeltaTime);
        else _vehicleHandler.FixedTick(Time.fixedDeltaTime);
    }  
    
    public void AddPower(float p)
    {
        _power += p;
    }

    void EnterVehicle()
    {
        OutsideVehicle = false;
        _playerInput.SwitchCurrentActionMap("Car");
        _humanHandler.gameObject.SetActive(false);
    }

    void ExitVehicle()
    {
        OutsideVehicle = true;
        _playerInput.SwitchCurrentActionMap("Player");
        _humanHandler.transform.position = _vehicleHandler.ExitPosition;
        _humanHandler.gameObject.SetActive(true);
        _humanHandler.StartJump(true);
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
            if(_humanHandler.AttachedOverlapping != null)
                _humanHandler.AttachPiece();
            else if (_humanHandler.IsSelectedPieceIsMyBase)
            {
                EnterVehicle();
            }
            else _humanHandler.GrabPiece();
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

    #region Input Vehicle Events
    public void ExitVehicleEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ExitVehicle();
        }
    }

    public void LeftJoystickEvent(InputAction.CallbackContext context)
    {
        _vehicleHandler.SetLeftAxis(context.ReadValue<Vector2>());
    }


    public void EastButtonEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("BTICAL");

            _vehicleHandler.ApplyInputAction(ActionKey.B, _power);
        }
    }

    public float DistanceHumanVehicle()
    {
        Vector2 vpos = _vehicleHandler.transform.position;
        Vector2 hpos = _humanHandler.transform.position;
        return (vpos - hpos).magnitude;
    }

    public void LowerVehicle()
    {
        _vehicleHandler.LowerVehicle();
    }

    public void RiseVehicle()
    {
        _vehicleHandler.RiseVehicle();
    }
    #endregion


}
