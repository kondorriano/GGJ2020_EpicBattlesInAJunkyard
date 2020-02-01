using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPiece : Piece
{
    public float baseTurnPower = 50;

    public override void ApplyAction(float actionValue)
    {
        
    }

    void LateUpdate()
    {
        //Testing solo
        if (isAttached)
        {
            float actionValue = Input.GetAxis("Horizontal");
            if (isAttached) rb.AddTorque(-actionValue * baseTurnPower);
        }
    }

    public override void AttachToRB(Rigidbody2D attachedTo, PlayerController pc = null)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p != null) p = attachedTo.GetComponentInParent<Piece>();
            if (p != null)
            {
                p.piecesAttachedToMe.Add(this);

                DistanceJoint2D dj2d1 = gameObject.AddComponent<DistanceJoint2D>();
                DistanceJoint2D dj2d2 = gameObject.AddComponent<DistanceJoint2D>();
                //DistanceJoint2D dj2d3 = gameObject.AddComponent<DistanceJoint2D>();
                dj2d1.connectedBody = attachedTo;
                dj2d2.connectedBody = attachedTo;
                //dj2d3.connectedBody = attachedTo;
                dj2d1.breakForce = breakForce;
                dj2d2.breakForce = breakForce;
                //dj2d3.breakForce = breakForce;

                Vector2 dist = transform.position - attachedTo.transform.position;
                Vector2 offset = new Vector2(dist.y, -dist.x);

                dj2d1.connectedAnchor += offset;
                dj2d2.connectedAnchor -= offset;
                //dj2d3.connectedAnchor += dist;
                activeJoints.Add(dj2d1);
                activeJoints.Add(dj2d2);
                //activeJoints.Add(dj2d3);
                isAttached = true;
            }
        }
    }
}
