using UnityEngine;

public abstract class CameraManager : MonoBehaviour
{
    public PlayerUIHandler[] GameUIs = null;

    public virtual void Destroy()
    {
        if (GameUIs != null)
        {
            for (int i = 0; i < GameUIs.Length; i++)
            {
                Destroy(GameUIs[i].gameObject);
            }

            GameUIs = null;
        }
    }

    public virtual void Setup(Camera CameraTemplate, PlayerController[] ToFollow, CanvasGroup UI, PlayerUIHandler PlayerUITemplate)
    {
        GameUIs = new PlayerUIHandler[ToFollow.Length];
        for (int i = 0; i < GameUIs.Length; i++)
        {
            GameUIs[i] = Instantiate(PlayerUITemplate);
            GameUIs[i].gameObject.SetActive(true);
            GameUIs[i].gameObject.name = string.Format("PlayerUI{0}", i + 1);
            GameUIs[i].transform.SetParent(UI.transform, false);
        }
    }
}

public class CameraSplit : CameraManager
{
    PlayerController[] follow;
    Camera[] cameras;
    float orthographicSize;

    public override void Destroy()
    {
        if (cameras != null)
            foreach (Camera c in cameras)
                Destroy(c.gameObject);

        cameras = null;
        follow = null;

        base.Destroy();
    }

    public override void Setup(Camera CameraTemplate, PlayerController[] ToFollow, CanvasGroup UI, PlayerUIHandler PlayerUITemplate)
    {
        Destroy();
        base.Setup(CameraTemplate, ToFollow, UI, PlayerUITemplate);

        follow = ToFollow;
        cameras = new Camera[follow.Length];
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i] = Instantiate(CameraTemplate);
            cameras[i].gameObject.SetActive(true);
            cameras[i].gameObject.name = string.Format("Camera{0}", i + 1);
            cameras[i].transform.SetParent(transform, false);

            Rect rect = cameras[i].rect;
            rect.x = (i % 2) * 0.5f;
            rect.width = 0.5f;

            if (ToFollow.Length > 2)
            {
                rect.height = 0.5f;
                if ((i / 2) < 1)
                    rect.y = 0.5f;
            }

            cameras[i].rect = rect;

            CameraPlayer cp = cameras[i].GetComponent<CameraPlayer>();
            if (cp == null)
                cp = cameras[i].gameObject.AddComponent<CameraPlayer>();

            cp.Follow = follow[i];
        }

        orthographicSize = CameraTemplate.orthographicSize;
    }
}
