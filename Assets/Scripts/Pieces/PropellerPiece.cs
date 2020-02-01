using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerPiece : Piece
{
    public float basePropellerPower = 50;
    public Transform Rotator;
    public float RotatorSpeed = 45;

    // Update is called once per frame
    void LateUpdate()
    {
        if (isAttached)
        {
            float actionValue = Input.GetAxis("Vertical");
            rb.AddForce(actionValue * transform.up * basePropellerPower);
            if (Rotator != null)
            {
                Rotator.Rotate(transform.up, actionValue * RotatorSpeed);
            }
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

    public override void AttachToRB(Rigidbody2D attachedTo, PlayerController pc = null)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p != null)
            {
                RelativeJoint2D rj2d = gameObject.AddComponent<RelativeJoint2D>();
                rj2d.connectedBody = attachedTo;
                rj2d.breakForce = breakForce;
                rj2d.correctionScale = 0.8f;
                activeJoints.Add(rj2d);
                isAttached = true;
            }
        }
    }
}
