using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioClip IntroClip;
    public AudioClip IngameClip;

    public CanvasGroup TitleScreen;
    public CanvasGroup GameUI;
    public CanvasGroup Announcements;

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
    public bool FastSpawn = false;

    List<GameObject> junkList = new List<GameObject>();

    PlayerController[] players;
    CameraManager cameraManager;

    bool BackToTitle = true;
    bool ResetGame = false;

    int TeamLeftScore = 0;
    int TeamRightScore = 0;

    void Start()
    {
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        TitleScreen.alpha = 0.0f;
        Announcements.alpha = 0.0f;

        while (true)
        {
            GameUI.alpha = 0.0f;

            MusicSource.clip = IntroClip;
            MusicSource.Play();

            // Title screen fade
            if (!FastSpawn) 
            {
                float time = Time.time;
                while ((Time.time - time) < 3)
                    yield return null;

                while ((Time.time - time) < 4)
                {
                    TitleScreen.alpha = (Time.time - time) - 3.0f;
                    yield return null;
                }
            }

            // Show title screen
            TitleScreen.alpha = 1.0f;

            while(BackToTitle)
                yield return null;

            MusicSource.Stop();
            MusicSource.clip = IngameClip;
            MusicSource.Play();

            if (!FastSpawn)
            {
                float time = Time.time;
                while ((Time.time - time) < 1)
                {
                    TitleScreen.alpha = 1.0f - (Time.time - time);
                    yield return null;
                }
            }

            // Hide title screen
            TitleScreen.alpha = 0.0f;
            GameUI.alpha = 1.0f;
            TeamLeftScore = 0;
            TeamRightScore = 0;

            while (true)
            {
                ResetGame = false;

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

                if (BackToTitle)
                    break;

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
                    for (; sel < freqAccum.Length && val > freqAccum[sel]; sel++) ;

                    GameObject junk = Instantiate(JunkPrefabs[sel].prefab);
                    junk.name = string.Format("Junk{0}({1})", i, JunkPrefabs[sel].prefab.name);
                    junkList.Add(junk);
                    float pos = (float)(Math.Round(UnityEngine.Random.Range(0.0f, 1.0f)) * 2.0f - 1.0f) * 75.0f;
                    junk.transform.position = new Vector3(pos, 1.0f, 0.0f);
                    junk.transform.rotation *= Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 2 * Mathf.PI));

                    if (!FastSpawn || i % 10 == 0)
                        yield return null;
                }

                // WAIT 5 SECS
                if (!FastSpawn)
                {
                    float time = Time.time;
                    while ((Time.time - time) < 2.5f)
                        yield return null;
                }

                // CREATE PLAYERS
                players = new PlayerController[PlayerCount];

                Camera CameraTemplate = transform.Find("CameraTemplate")?.GetComponent<Camera>();
                if (CameraTemplate == null)
                    Debug.Break();

                PlayerController PlayerTemplate = transform.Find("PlayerTemplate")?.GetComponent<PlayerController>();
                if (PlayerTemplate == null)
                    Debug.Break();

                if (GameUI == null)
                    Debug.Break();

                PlayerUIHandler PlayerUITemplate = GameUI.transform.Find("PlayerUITemplate")?.GetComponent<PlayerUIHandler>();
                if (PlayerUITemplate == null)
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

                    foreach (MeshRenderer plRender in players[i].ColorRenderers)
                        if (plRender && PlayerMaterials != null && i < PlayerMaterials.Length)
                            plRender.material = PlayerMaterials[i];
                }

                if (PlayerCount == 2)
                {
                    var camDirectorObj = new GameObject();
                    camDirectorObj.name = "CameraDirector";
                    cameraManager = camDirectorObj.AddComponent<CameraDirector>();
                    cameraManager.Setup(CameraTemplate, players, GameUI, PlayerUITemplate);
                }
                else
                {
                    var camDirectorObj = new GameObject();
                    camDirectorObj.name = "CameraSplit";
                    cameraManager = camDirectorObj.AddComponent<CameraSplit>();
                    cameraManager.Setup(CameraTemplate, players, GameUI, PlayerUITemplate);
                }

                for (int i = 0; i < PlayerCount; i++)
                {
                    var healthMeter = cameraManager.GameUIs[i].transform.Find("HealthMeter")?.GetComponent<UnityEngine.UI.Text>();
                    if (healthMeter != null)
                        healthMeter.color = Color.Lerp(Color.black, PlayerMaterials[i].color, 0.8f);
                }

                // Check if someone won
                bool leftWon = false;
                while (true)
                {
                    // Check if 
                    int teamLeftAlive = 0;
                    int teamRightAlive = 0;

                    for (int i = 0; i < players.Length; i++)
                    {
                        bool left = (i % 2) == 0;
                        bool dead = players[i].health <= 0;

                        cameraManager.GameUIs[i].HealthMeter.text = string.Format("Health: {0}%", (int) Math.Round(players[i].health));

                        if (left)
                            teamLeftAlive += dead ? 0 : 1;
                        else
                            teamRightAlive += dead ? 0 : 1;
                    }

                    if (ResetGame || BackToTitle)
                        continue;

                    if (teamLeftAlive == 0)
                    {
                        leftWon = false;
                        break;
                    }

                    if (teamRightAlive == 0)
                    {
                        leftWon = true;
                        break;
                    }

                    yield return null;
                }

                UnityEngine.UI.Text annText = Announcements.GetComponent<UnityEngine.UI.Text>();
                if(annText != null)
                {
                    if (leftWon)
                    {
                        annText.text = "Team left\n WINS";
                    }
                    else
                    {
                        annText.text = "Team right\n WINS";
                    }
                }

                // FADE IN
                if (!FastSpawn)
                {
                    float time = Time.time;
                    while ((Time.time - time) < 1)
                    {
                        Announcements.alpha = 1.0f - (Time.time - time);
                        yield return null;
                    }
                }

                Announcements.alpha = 1.0f;

                {
                    float time = Time.time;
                    while ((Time.time - time) < 5)
                    {
                        yield return null;
                    }
                }

                // FADE OUT
                if (!FastSpawn)
                {
                    float time = Time.time;
                    while ((Time.time - time) < 1)
                    {
                        Announcements.alpha = Time.time - time;
                        yield return null;
                    }
                }

                Announcements.alpha = 0.0f;
            }
        }
    }

    void OnGUI()
    {
        if (BackToTitle)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                BackToTitle = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
                ResetGame = true;

            if (Input.GetKeyDown(KeyCode.Escape))
                BackToTitle = true;
        }
    }
}
