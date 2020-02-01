using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    protected bool isAttached = false;
    protected Rigidbody2D rb;
    //public Rigidbody2D attachOnSpace;
    public Rigidbody2D attachOnAwake;
    protected List<Joint2D> activeJoints;
    [HideInInspector]
    public List<Piece> piecesAttachedToMe;
    public float breakForce = 750;

    void Awake()
    {
        activeJoints = new List<Joint2D>();
        piecesAttachedToMe = new List<Piece>();
        rb = GetComponent<Rigidbody2D>();

        //Para testear cosas de pieces
        if (attachOnAwake != null) AttachToRB(attachOnAwake);
    }

    void OnJointBreak2D(Joint2D brokenJoint)
    {
        Unattach();
    }

    public void Unattach()
    {
        foreach (Joint2D j in activeJoints)
        {
            Destroy(j);
        }
        foreach (Piece p in piecesAttachedToMe)
        {
            p.Unattach();
        }
        piecesAttachedToMe = new List<Piece>();
        activeJoints = new List<Joint2D>();
        isAttached = false;
    }

    public virtual void ApplyAction(float actionValue)
    {
        
    }

    public virtual void ApplyAction()
    {

    }

    public virtual void AttachToRB(Rigidbody2D attachedTo, PlayerController pc = null)
    {
        
    }
}
