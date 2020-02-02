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
                FixedJoint2D fj2d = gameObject.AddComponent<FixedJoint2D>();
                fj2d.connectedBody = attachedTo;
                fj2d.breakForce = breakForce;
                activeJoints.Add(fj2d);
                isAttached = true;
                p.piecesAttachedToMe.Add(this);
                parentPiece = p;
            }
        }
    }
}
