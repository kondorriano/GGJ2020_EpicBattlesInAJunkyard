using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePiece : Piece
{
    public float baseRotation = 75f;
    public float floatJointHeight = 10f;
    public float contractionSpeed = 0.5f;
    public float desiredFloatElevation = 0.33f;
    private float contraction;
    private bool rised = false;
    private List<Joint2D> joints;

    public override void Awake()
    {
        base.Awake();
        contraction = (floatJointHeight - desiredFloatElevation) / floatJointHeight;
        joints = new List<Joint2D>();
    }

    public override void ApplyAction(ActionKey key, float actionValue)
    {
        if (key == ActionKey.AxisH1)
        {
            rb.AddTorque(-actionValue * baseRotation);
        }
    }

    public override void Unattach()
    {
        //base.Unattach();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!rised)
            {
                StartRise();
            }
            else
            {
                Unrise();
            }
        }
    }

    public void Unrise()
    {
        if (rised)
        {
            foreach (Joint2D j in joints)
            {
                Destroy(j);
            }
            joints = new List<Joint2D>();
            rised = false;
        }
    }

    public void StartRise()
    {
        rised = true;
        DistanceJoint2D dj2d1 = gameObject.AddComponent<DistanceJoint2D>();
        DistanceJoint2D dj2d2 = gameObject.AddComponent<DistanceJoint2D>();
        DistanceJoint2D dj2d3 = gameObject.AddComponent<DistanceJoint2D>();
        DistanceJoint2D dj2d4 = gameObject.AddComponent<DistanceJoint2D>();

        joints.Add(dj2d1);
        joints.Add(dj2d2);
        joints.Add(dj2d3);
        joints.Add(dj2d4);

        dj2d1.anchor = new Vector2(-0.5f, 0);
        dj2d1.connectedAnchor = new Vector2(-3f + transform.position.x, floatJointHeight + transform.position.y);
        float d1 = dj2d1.distance;

        dj2d3.anchor = new Vector2(-0.5f, 0);
        dj2d3.connectedAnchor = new Vector2(3f + transform.position.x, floatJointHeight + transform.position.y);
        float d3 = dj2d3.distance;
        
        dj2d2.anchor = new Vector2(0.5f, 0);
        dj2d2.connectedAnchor = new Vector2(3f + transform.position.x, floatJointHeight + transform.position.y);
        float d2 = dj2d2.distance;

        dj2d4.anchor = new Vector2(0.5f, 0);
        dj2d4.connectedAnchor = new Vector2(-3f + transform.position.x, floatJointHeight + transform.position.y);
        float d4 = dj2d4.distance;

        dj2d1.autoConfigureDistance = false;
        dj2d1.distance = d1*contraction;
        dj2d2.autoConfigureDistance = false;
        dj2d2.distance = d2*contraction;
        dj2d3.autoConfigureDistance = false;
        dj2d3.distance = d3*contraction;
        dj2d4.autoConfigureDistance = false;
        dj2d4.distance = d4*contraction;
    }

}
