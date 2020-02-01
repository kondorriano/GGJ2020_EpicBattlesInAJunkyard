using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPiece : Piece
{
    public GameObject CannonBall;
    public float baseImpulseShot = 1000f;
    public float baseCannonBallSpeed = 15f;
    // Start is called before the first frame update

    /*
    void LateUpdate()
    {
        if (isAttached && Input.GetKeyDown(KeyCode.Space) && CannonBall != null)
        {
            float actionValue = 1f;
            GameObject cb = Instantiate(CannonBall, transform.position + transform.up, transform.rotation);
            Rigidbody2D crb = cb.GetComponent<Rigidbody2D>();
            crb.velocity = transform.up * actionValue * baseCannonBallSpeed;
            rb.AddForce(-transform.up * baseImpulseShot * actionValue);
        }
    }*/

    public override void ApplyAction(ActionKey key, float actionValue)
    {
        if (isAttached && CannonBall != null && key == ActionKey.B)
        {
            GameObject cb = Instantiate(CannonBall, transform.position + transform.up, transform.rotation);
            Rigidbody2D crb = cb.GetComponent<Rigidbody2D>();
            crb.velocity = transform.up * baseCannonBallSpeed;
            rb.AddForce(-transform.up * baseImpulseShot * actionValue);
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
                isAttached = true;
            }
        }
    }
}
