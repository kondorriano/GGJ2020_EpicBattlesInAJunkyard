using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PlayerCount = 2;
    public Material[] PlayerMaterials;

    class PlayerHolder
    {
        public Camera camera;
        public PlayerController player;

        public void Destroy()
        {
            if (camera != null)
                GameObject.Destroy(camera);

            if (player != null)
                GameObject.Destroy(player);
        }
    }

    PlayerHolder[] players;

    void Start()
    {
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        while (true)
        {
            if (PlayerCount < 2 && PlayerCount > 4)
                Debug.Break();

            if (players != null)
                foreach (PlayerHolder p in players)
                    if (p != null)
                        p.Destroy();

            players = new PlayerHolder[PlayerCount];

            Camera CameraTemplate = transform.Find("CameraTemplate")?.GetComponent<Camera>();
            if (CameraTemplate == null)
                Debug.Break();

            PlayerController PlayerTemplate = transform.Find("PlayerTemplate")?.GetComponent<PlayerController>();
            if (PlayerTemplate == null)
                Debug.Break();

            for (int i = 0; i < PlayerCount; i++)
            {
                players[i] = new PlayerHolder();

                // Create camera
                players[i].camera = Instantiate(CameraTemplate);
                players[i].camera.gameObject.SetActive(true);
                players[i].camera.gameObject.name = string.Format("PlayerCamera{0}", i + 1);

                Rect rect = players[i].camera.rect;
                rect.x = (i % 2) * 0.5f;
                rect.width = 0.5f;

                if (PlayerCount > 2)
                {
                    rect.height = 0.5f;
                    if ((i / 2) < 1)
                        rect.y = 0.5f;
                }

                players[i].camera.rect = rect;

                // Create player
                players[i].player = Instantiate(PlayerTemplate);
                players[i].player.gameObject.SetActive(true);

                float side = ((i % 2) * 2) - 1.0f;
                float team = ((i / 2) * 2) - 1.0f;
                players[i].player.transform.position = new Vector3(50.0f * side + 3.0f * team, 0, 0);

                MeshRenderer plRender = players[i].player.GetComponent<MeshRenderer>();
                if (plRender && PlayerMaterials != null && i < PlayerMaterials.Length)
                    plRender.material = PlayerMaterials[i];

                CameraPlayer cp = players[i].camera.GetComponent<CameraPlayer>();
                cp.Follow = players[i].player.transform;
            }

            while(true)
                yield return null;
        }
    }

    void Update()
    {
        
    }
}
