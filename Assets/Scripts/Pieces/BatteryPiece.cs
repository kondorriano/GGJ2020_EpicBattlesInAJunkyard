using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPiece : Piece
{
    // Start is called before the first frame update
    public float power = 1;
    private PlayerController current_pc;

    public override void AttachToRB(Rigidbody2D attachedTo, PlayerController pc)
    {
        if (!isAttached)
        {
            Piece p = attachedTo.GetComponent<Piece>();
            if (p == null) p = attachedTo.GetComponentInParent<Piece>();
            if (p != null)
            {
                RelativeJoint2D fj2d = gameObject.AddComponent<RelativeJoint2D>();
                fj2d.connectedBody = attachedTo;
                fj2d.breakForce = breakForce;
                fj2d.correctionScale = 0.25f;
                activeJoints.Add(fj2d);
                current_pc = pc;
                isAttached = true;

                if (current_pc != null) current_pc.AddPower(power);
            }
        }
    }

    public override void Unattach()
    {
        base.Unattach();
        if (current_pc != null) current_pc.AddPower(-power);
        current_pc = null;
    }
}
