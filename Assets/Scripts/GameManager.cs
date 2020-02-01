using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PlayerCount = 2;
    public Material[] PlayerMaterials;

    [Serializable]
    public struct Junk
    {
        public GameObject prefab;
        public int freq;
    }

    public Junk[] JunkPrefabs;
    public int InitialJunkCount = 100;
    List<GameObject> junkList = new List<GameObject>();

    PlayerController[] players;
    CameraManager cameraManager;
    bool reset = false;

    void Start()
    {
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        while (true)
        {
            reset = false;

            if (PlayerCount < 2 && PlayerCount > 4)
                Debug.Break();

            if (players != null)
                foreach (PlayerController p in players)
                    if (p != null)
                        Destroy(p.gameObject);

            foreach (GameObject j in junkList)
                    Destroy(j);

            junkList.Clear();

            if (cameraManager != null)
            {
                cameraManager.Destroy();
                Destroy(cameraManager.gameObject);
            }

            // CREATE INITIAL JUNK

            int totalJunk = 0;
            int[] freqAccum = new int[JunkPrefabs.Length];
            for (int i = 0; i < JunkPrefabs.Length; i++)
            {
                totalJunk += JunkPrefabs[i].freq;
                freqAccum[i] = totalJunk;
            }

            UnityEngine.Random rand = new UnityEngine.Random();
            for (int i = 0; i < InitialJunkCount; i++)
            {
                int sel = 0;
                int val = UnityEngine.Random.Range(0, totalJunk);
                for (; sel < freqAccum.Length && val > freqAccum[sel]; sel++);

                GameObject junk = Instantiate(JunkPrefabs[sel].prefab);
                junk.name = string.Format("Junk{0}({1})", i, JunkPrefabs[sel].prefab.name);
                junkList.Add(junk);
                float pos = (float) (Math.Round(UnityEngine.Random.Range(0.0f, 1.0f)) * 2.0f - 1.0f) * 75.0f;
                junk.transform.position = new Vector3(pos, 1.0f, 0.0f);
                junk.transform.rotation *= Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 2 * Mathf.PI));
                yield return null;
            }

            //// WAIT 5 SECS
            float time = Time.time;
            while ((Time.time - time) < 5)
                yield return null;

            // CREATE PLAYERS
            players = new PlayerController[PlayerCount];

            Camera CameraTemplate = transform.Find("CameraTemplate")?.GetComponent<Camera>();
            if (CameraTemplate == null)
                Debug.Break();

            PlayerController PlayerTemplate = transform.Find("PlayerTemplate")?.GetComponent<PlayerController>();
            if (PlayerTemplate == null)
                Debug.Break();

            for (int i = 0; i < PlayerCount; i++)
            {
                // Create player
                players[i] = Instantiate(PlayerTemplate);
                players[i].gameObject.SetActive(true);
                players[i].gameObject.name = string.Format("Player{0}", i + 1);
                players[i].Init(i);

                float side = ((i % 2) * 2) - 1.0f;
                float team = ((i / 2) * 2) - 1.0f;
                players[i].transform.position = new Vector3(50.0f * side + 3.0f * team, 0, 0);

                foreach(MeshRenderer plRender in players[i].ColorRenderers)
                    if (plRender && PlayerMaterials != null && i < PlayerMaterials.Length)
                        plRender.material = PlayerMaterials[i];
            }

            if (PlayerCount == 2)
            {
                var camDirectorObj = new GameObject();
                camDirectorObj.name = "CameraDirector";
                cameraManager = camDirectorObj.AddComponent<CameraDirector>();
                cameraManager.Setup(CameraTemplate, players);
            }
            else
            {
                var camDirectorObj = new GameObject();
                camDirectorObj.name = "CameraSplit";
                cameraManager = camDirectorObj.AddComponent<CameraSplit>();
                cameraManager.Setup(CameraTemplate, players);
            }

            while(!reset)
                yield return null;
        }
    }

    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.R))
            reset = true;
    }
}
