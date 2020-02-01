using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonPiece : Piece
{
    public float MaxEstend = 10f;

    void LateUpdate()
    {
        if (isAttached)
        {
            
        }
    }

    /*
    public override void ApplyAction(float actionValue)
    {
        if (isAttached)
        {
            rb.AddForce(actionValue * transform.up * basePropellerPower);
            if (Rotator != null)
            {
                Rotator.Rotate(transform.up, actionValue * RotatorSpeed);
            }
        }
    }
    */

}
