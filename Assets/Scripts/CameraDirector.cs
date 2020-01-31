using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    PlayerController[] follow;
    Camera[] cameras;
    float orthographicSize;

    public void Setup(Camera CameraTemplate, params PlayerController[] ToFollow)
    {
        follow = ToFollow;
        if (cameras != null)
            foreach(Camera c in cameras)
                Destroy(c);

        cameras = new Camera[follow.Length];
        for(int i = 0; i < cameras.Length; i++)
        {
            cameras[i] = Instantiate(CameraTemplate);
            cameras[i].gameObject.SetActive(true);
            cameras[i].gameObject.name = string.Format("Camera{0}", i + 1);
            cameras[i].transform.SetParent(transform, false);
        }

        orthographicSize = CameraTemplate.orthographicSize;
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
            var currPos = follow[i].ToFollow.position;
            var position = currCamera.transform.position;
            lastX = position.x = currPos.x;

            for (int j = i + 1; j < cameras.Length; j++)
            {
                var nextPos = follow[j].ToFollow.transform.position;
                if (lastX + radius > nextPos.x)
                {
                    posAccum++;
                    rect.width = step * posAccum;
                    position.x += nextPos.x;
                    cameras[j].enabled = false;
                    lastX = nextPos.x;
                    i = j;
                }
            }

            position.x = position.x / posAccum;
            currCamera.transform.position = position;
            currCamera.rect = rect;
            currCamera.enabled = true;
            currCamera.orthographicSize = orthographicSize;
        }
    }
}
