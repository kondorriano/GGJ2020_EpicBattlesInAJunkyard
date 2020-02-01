using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : Piece
{
    public float baseRotation = 75f;

    public override void ApplyAction(ActionKey key, float actionValue)
    {
        if (key == ActionKey.AxisH1)
        {
            if (isAttached) rb.AddTorque(-actionValue * baseRotation);
        }
    }

    /*
    void LateUpdate()
    {
        float actionValue = Input.GetAxis("Horizontal");
        if (isAttached) rb.AddTorque(-actionValue * baseRotation);
    }*/
}
