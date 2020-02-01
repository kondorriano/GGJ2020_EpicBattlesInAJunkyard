using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public PlayerController Follow;
    void Start()
    {
        if (Follow != null)
        {
            Vector3 p = transform.position;
            p.x = Follow.ToFollow.position.x;
            p.y = Follow.ToFollow.position.y;
            transform.position = p;
        }
    }

    const float FollowFactor = 0.8f;
    void FixedUpdate()
    {
        if (Follow != null)
        {
            Vector3 p = transform.position;
            p.x = p.x * FollowFactor + Follow.ToFollow.position.x * (1.0f - FollowFactor);
            p.y = p.y * FollowFactor + Follow.ToFollow.position.y * (1.0f - FollowFactor);
            transform.position = p;
        }
    }
}
