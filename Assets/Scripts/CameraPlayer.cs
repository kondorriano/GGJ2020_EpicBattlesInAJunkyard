using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform Follow;

    void Start()
    {
        if (Follow != null)
        {
            Vector3 p = transform.position;
            p.x = Follow.position.x;
            p.y = Follow.position.y;
            transform.position = p;
        }
    }

    const float FollowFactor = 0.9f;
    void FixedUpdate()
    {
        if (Follow != null)
        {
            Vector3 p = transform.position;
            p.x = p.x * FollowFactor + Follow.position.x * (1.0f - FollowFactor);
            p.y = p.y * FollowFactor + Follow.position.y * (1.0f - FollowFactor);
            transform.position = p;
        }
    }
}
