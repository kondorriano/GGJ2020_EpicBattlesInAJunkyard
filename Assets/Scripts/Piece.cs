using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWheel = false;
    public bool isAttached = false;
    protected Rigidbody2D rb;
    public Rigidbody2D attachOnSpace;
    public List<Joint2D> activeJoints;
    public List<Piece> piecesAttachedToMe;
    public float breakForce = 750;

    void Awake()
    {
        activeJoints = new List<Joint2D>();
        piecesAttachedToMe = new List<Piece>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isAttached)
            {
                Unattach();
            }
            else
            {
                if (attachOnSpace != null) AttachToRB(attachOnSpace);
            }
        }
        ApplyAction(Input.GetAxis("Horizontal"));
    }

    void OnJointBreak2D(Joint2D brokenJoint)
    {
        Unattach();
    }

    public virtual void ApplyAction(float actionValue)
    {
        
    }

    public virtual void ApplyAction()
    {

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

    public virtual void AttachToRB(Rigidbody2D attachedTo)
    {
        
    }
}
