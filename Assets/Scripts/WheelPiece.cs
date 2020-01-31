using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPiece : Piece
{
    public float turnPower = 30;

    public override void ApplyAction(float actionValue)
    {
        if (isAttached) rb.AddTorque(-actionValue * turnPower);
    }

    public override void AttachToRB(Rigidbody2D attachedTo)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p != null)
            {
                p.piecesAttachedToMe.Add(this);

                DistanceJoint2D dj2d1 = gameObject.AddComponent<DistanceJoint2D>();
                DistanceJoint2D dj2d2 = gameObject.AddComponent<DistanceJoint2D>();
                dj2d1.connectedBody = attachedTo;
                dj2d2.connectedBody = attachedTo;

                Vector2 dist = transform.position - attachedTo.transform.position;
                Vector2 offset = new Vector2(dist.y, -dist.x);

                dj2d1.connectedAnchor += offset;
                dj2d2.connectedAnchor -= offset;
                activeJoints.Add(dj2d1);
                activeJoints.Add(dj2d2);
            }

            isAttached = true;
        }
    }
}
