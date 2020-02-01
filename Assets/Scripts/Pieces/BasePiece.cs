using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : Piece
{
    public float baseRotation = 75f;

    public override void ApplyAction(float actionValue)
    {

    }

    void LateUpdate()
    {
        //Testing solo
        if (isAttached)
        {
            float actionValue = Input.GetAxis("Horizontal");
            if (isAttached) rb.AddTorque(-actionValue * baseRotation);
        }
    }
}
