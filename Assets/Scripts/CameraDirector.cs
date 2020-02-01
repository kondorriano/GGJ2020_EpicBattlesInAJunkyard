﻿using System;
using UnityEngine;

public class CameraDirector : CameraManager
{
    PlayerController[] follow;
    Vector2[] smoothPos;
    Camera[] cameras;
    float orthographicSize;

    public override void Destroy()
    {
        if (cameras != null)
            foreach (Camera c in cameras)
                Destroy(c.gameObject);

        cameras = null;
        follow = null;
        smoothPos = null;
    }

    public override void Setup(Camera CameraTemplate, params PlayerController[] ToFollow)
    {
        Destroy();

        follow = ToFollow;
        cameras = new Camera[follow.Length];
        smoothPos = new Vector2[follow.Length];
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i] = Instantiate(CameraTemplate);
            cameras[i].gameObject.SetActive(true);
            cameras[i].gameObject.name = string.Format("Camera{0}", i + 1);
            cameras[i].transform.SetParent(transform, false);
            smoothPos[i] = follow[i].ToFollow.position;
        }

        orthographicSize = CameraTemplate.orthographicSize;
    }

    const float FollowFactor = 0.8f;
    private void FixedUpdate()
    {
        if (follow == null) return;
        for (int i = 0; i < cameras.Length; i++)
        {
            Vector2 p = smoothPos[i];
            p.x = p.x * FollowFactor + follow[i].ToFollow.position.x * (1.0f - FollowFactor);
            //p.y = p.y * FollowFactor + follow[i].ToFollow.position.y * (1.0f - FollowFactor);
            smoothPos[i] = p;
        }
    }

    void Update()
    {
        Array.Sort(follow, delegate (PlayerController a, PlayerController b)
        {
            return a.ToFollow.position.x.CompareTo(b.ToFollow.position.x);
        });

        float step = (1.0f / follow.Length);
        float lastX = float.NegativeInfinity;
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera currCamera = cameras[i];
            var rect = currCamera.rect;
            rect.x = i * step;
            rect.width = step;
            currCamera.rect = rect;

            float radius = (orthographicSize * currCamera.aspect) * 2;

            int posAccum = 1;
            var currPos = smoothPos[i];
            var position = currCamera.transform.position;
            lastX = position.x = currPos.x;

            for (int j = i + 1; j < cameras.Length; j++)
            {
                var nextPos = smoothPos[j];
                if (lastX + radius > nextPos.x)
                {
                    posAccum++;
                    rect.width = step * posAccum;
                    position.x += nextPos.x;
                    cameras[j].gameObject.SetActive(false);
                    lastX = nextPos.x;
                    i = j;
                }
            }

            position.x = position.x / posAccum;
            currCamera.transform.position = position;
            currCamera.rect = rect;
            currCamera.gameObject.SetActive(true);
            currCamera.orthographicSize = orthographicSize;
        }
    }
}
