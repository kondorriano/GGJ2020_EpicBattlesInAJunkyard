using System;
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

        base.Destroy();
    }

    public override void Setup(Camera CameraTemplate, PlayerController[] ToFollow, CanvasGroup UI, PlayerUIHandler PlayerUITemplate)
    {
        Destroy();
        base.Setup(CameraTemplate, ToFollow, UI, PlayerUITemplate);

        follow = new PlayerController[ToFollow.Length];
        for (int i = 0; i < ToFollow.Length; i++)
            follow[i] = ToFollow[i];
        
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

        if (CameraTemplate.orthographic)
            orthographicSize = CameraTemplate.orthographicSize;
        else
        {
            double dst = Math.Abs(CameraTemplate.transform.position.z);
            double tan = Math.Tan(CameraTemplate.fieldOfView);
            double size = (dst * tan) / CameraTemplate.aspect;
            orthographicSize = (float) (size * 0.5);
        }
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
        int skipUntil = -1;
        for (int i = 0; i < cameras.Length; i++)
        {
            bool skip = i <= skipUntil; 
            Camera currCamera = cameras[i];
            var rect = currCamera.rect;
            rect.x = i * step;
            rect.width = step;
            currCamera.rect = rect;

            RectTransform currUIRect = GameUIs[follow[i]._playerID].GetComponent<RectTransform>();
            var min = currUIRect.anchorMin;
            var max = currUIRect.anchorMax;
            min.x = rect.x;
            max.x = rect.x + rect.width;
            currUIRect.anchorMin = min;
            currUIRect.anchorMax = max;

            float radius = (orthographicSize * currCamera.aspect) * 2;

            int posAccum = 1;
            var currPos = smoothPos[i];
            var position = currCamera.transform.position;

            if (!skip)
            {
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
                        skipUntil = j;
                    }
                }
            }

            position.x = position.x / posAccum;
            currCamera.transform.position = position;
            currCamera.rect = rect;
            currCamera.gameObject.SetActive(!skip);
            currCamera.orthographicSize = orthographicSize;
        }
    }
}
