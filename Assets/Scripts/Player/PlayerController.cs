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
    private float power = 3;
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
            yield return _vehicleHandler?.GetComponent<MeshRenderer>();
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
        int vehicleLayer = LayerMask.NameToLayer(string.Format("Vehicle{0}", _playerID + 1));
        int pieceLayer = LayerMask.NameToLayer(string.Format("Grabbed{0}", _playerID + 1));

        _humanHandler.Init(this, pieceLayer, vehicleLayer);
        _vehicleHandler.Init(vehicleLayer);
    }

    private void FixedUpdate()
    {
        if(OutsideVehicle) _humanHandler.FixedTick(Time.fixedDeltaTime);
    }  
    
    public void addPower(float p)
    {
        power += p;
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
            else
                _humanHandler.GrabPiece();
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
