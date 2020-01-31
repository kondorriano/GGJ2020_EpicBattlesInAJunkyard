using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PlayerCount = 2;
    Camera[] PlayerCameras;

    void Start()
    {
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        while(true)
        {
            //if (Camera.main && Camera.main.enabled)
            //    Camera.main.enabled = false;

            if (PlayerCount < 2 && PlayerCount > 4)
                Debug.Break();

            if (PlayerCameras != null)
                foreach (Camera camera in PlayerCameras)
                    if (camera != null)
                        Destroy(camera);

            PlayerCameras = new Camera[PlayerCount];
            Camera CameraTemplate = transform.Find("CameraTemplate")?.GetComponent<Camera>();

            if (!CameraTemplate)
                Debug.Break();

            for (int i = 0; i < PlayerCount; i++)
            {
                PlayerCameras[i] = Instantiate(CameraTemplate);
                PlayerCameras[i].gameObject.SetActive(true);
                PlayerCameras[i].gameObject.name = string.Format("PlayerCamera{0}", i + 1);

                Rect rect = PlayerCameras[i].rect;
                rect.x = (i % 2) * 0.5f;
                rect.width = 0.5f;

                if (PlayerCount > 2)
                {
                    rect.height = 0.5f;
                    if ((i/2) < 1)
                        rect.y = 0.5f;
                }

                PlayerCameras[i].rect = rect;
            }

            while(true)
                yield return null;

            yield return null;
        }
    }

    void Update()
    {
        
    }
}
