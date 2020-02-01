using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuralPiece : Piece
{
    // Start is called before the first frame update

    public override void AttachToRB(Rigidbody2D attachedTo, PlayerController pc = null)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p == null) p = attachedTo.GetComponentInParent<Piece>();
            if (p != null)
            {
                RelativeJoint2D rj2d = gameObject.AddComponent<RelativeJoint2D>();
                rj2d.connectedBody = attachedTo;
                rj2d.breakForce = breakForce;
                rj2d.correctionScale = 0.25f;
                activeJoints.Add(rj2d);
                isAttached = true;
            }
        }
    }
}
