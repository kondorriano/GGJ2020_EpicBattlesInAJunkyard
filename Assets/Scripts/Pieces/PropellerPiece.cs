using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerPiece : Piece
{
    public float basePropellerPower = 50;
    public Transform Rotator;
    public float RotatorSpeed = 45;

    /*
    void LateUpdate()
    {
        if (isAttached)
        {
            float actionValue = Input.GetAxis("Vertical");
            rb.AddForce(actionValue * transform.up * basePropellerPower);
            if (Rotator != null)
            {
                Rotator.Rotate(Vector3.up, actionValue * RotatorSpeed);
            }
        }
    }*/

    public override void ApplyAction(ActionKey key, float actionValue)
    {
        if (isAttached && key == ActionKey.AxisV1)
        {
            rb.AddForce(actionValue * transform.up * basePropellerPower);
            if (Rotator != null)
            {
                Rotator.Rotate(Vector3.up, actionValue * RotatorSpeed);
            }
        }
    }

    public override void AttachToRB(Rigidbody2D attachedTo, PlayerController pc = null)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p == null) p = attachedTo.GetComponentInParent<Piece>();
            if (p != null)
            {
                FixedJoint2D fj2d = gameObject.AddComponent<FixedJoint2D>();
                fj2d.connectedBody = attachedTo;
                fj2d.breakForce = breakForce;
                //rj2d.correctionScale = 0f;
                activeJoints.Add(fj2d);
                p.piecesAttachedToMe.Add(this);
                parentPiece = p;
                isAttached = true;
            }
        }
    }
}
