using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTypeInfo
{
    public int generalCount, maxCount, currentCount;
}

public class Game : MonoBehaviour
{
    public int enemyCount = 0, maxEnemyCount = 30;
    public float level = 1;
    public Shop shop;
    public SpriteRenderer backRend;
    public Text lvlText;
    public Camera rendTexCam;
    public RawImage rawImage;
    public AudioClip commonLvlMusic, bossLvlMusic, deathMusic;
    public AudioSource musicSource, selectSource;
    public AudioSource[] soundsSources = new AudioSource[2];
    public EnemyTypeInfo[] enemiesInfo = new EnemyTypeInfo[3];
    public Player player;
    public GameObject enemy, pauseButton, pauseMenu, minimap, item, pickingItemsInfo, wormHole, bossPref, settings;
    public Boss boss;
    public LevelLoader levelLoader;
    public Toggle musicToggle, soundsToggle;
    public Slider musicSlider, soundsSlider;
    public List<Enemy> enemies = new List<Enemy>();
    public List<GameObject> infos = new List<GameObject>();
    public List<Item> items = new List<Item>();
    public bool bossLvl = false, bossDefeated = false;
    private float w, h, starChance;
    private int tap1Id, tap2Id;
    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("lvl"))
        {
            level = PlayerPrefs.GetFloat("lvl");
            starChance = Mathf.Min(.5f, level * .005f);
            if (bossLvl = level % 1f == .5f)
                backRend.material.SetFloat("boss", 1f);
        }
        musicSource.clip = bossLvl ? bossLvlMusic : commonLvlMusic;
        if (musicSource.enabled)
            musicSource.Play();
        if (PlayerPrefs.HasKey("music"))
            musicToggle.isOn = musicSource.enabled = PlayerPrefs.GetString("music") == "1";
        if (PlayerPrefs.HasKey("sounds"))
        {
            bool value = soundsToggle.isOn = PlayerPrefs.GetString("sounds") == "1";
            for (int i = 0, count = soundsSources.Length; i < count; i++)
                soundsSources[i].enabled = value;
        }
        if (PlayerPrefs.HasKey("musicVol"))
            musicSlider.value = musicSource.volume = PlayerPrefs.GetFloat("musicVol");
        if (PlayerPrefs.HasKey("soundsVol")) {
            float value = soundsSlider.value = PlayerPrefs.GetFloat("soundsVol");
            for (int i = 0, count = soundsSources.Length; i < count; i++)
                soundsSources[i].volume = value;
        }
        h = Camera.main.orthographicSize + 1;
        w = (h - 1) * Screen.width / Screen.height + 1;
        if (bossLvl)
        {
            float x, y;
            do
            {
                x = Random.Range(-60f, 60f);
                y = Random.Range(-60f, 60f);
            }
            while (Mathf.Abs(x - player.transform.position.x) < w*3f && Mathf.Abs(y - player.transform.position.y) < h*3f);
            int id = (int)level / 3 - 1;
            bool star = level > 54 && Random.value < starChance;
            if (star)
                maxEnemyCount = 10;
            enemies.Add(boss = Instantiate(bossPref, new Vector3(x, y), Quaternion.identity).GetComponent<Boss>());
            enemies[enemies.Count-1].SetId(star ? -id : id);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                enemiesInfo[i] = new EnemyTypeInfo();
                enemiesInfo[i].currentCount = 0;
                enemiesInfo[i].maxCount = (i + 1) * 3;
                enemiesInfo[i].generalCount = (i + 1) * 6;
            }
            EnemyTypeInfo info = enemiesInfo[((int)level - 1) % 3];
            int id = (int)(level - 1) / 3 * 3;
            for (float a = 0f; a < 6f; a += 2.0943951024f)
            {
                enemies.Add(Instantiate(enemy, new Vector3(2f * Mathf.Cos(a) + wormHole.transform.position.x, 2f * Mathf.Sin(a) + wormHole.transform.position.y), Quaternion.identity).GetComponent<Enemy>());
                enemies[enemies.Count - 1].SetId(id);
                info.currentCount++;
                info.maxCount++;
                info.generalCount++;
            }
            id += 9;
            if (level % 3f == 0f)
                for (float a = 0f; a < 6f; a += 1.0471975512f)
                {
                    enemies.Add(Instantiate(enemy, new Vector3(2f * Mathf.Cos(a) + shop.transform.position.x, 2f * Mathf.Sin(a) + shop.transform.position.y), Quaternion.identity).GetComponent<Enemy>());
                    enemies[enemies.Count - 1].SetId(id);
                    info.currentCount++;
                    info.maxCount++;
                    info.generalCount++;
                }
        }
        rawImage.texture = rendTexCam.targetTexture = new RenderTexture(Screen.width,Screen.height,0);
        lvlText.text = string.Concat("Level ", ((int)level + (int)(Mathf.CeilToInt(level - 1f) * 0.33333333f)).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossLvl)
        {
            for (int i = 0, id = (int)level - 1; i < 3 && id >= 0; i++, id--)
            {
                EnemyTypeInfo info = enemiesInfo[i];
                if (info.currentCount < info.maxCount && info.currentCount < info.generalCount && Random.value > .9f)
                {
                    float x, y;
                    do
                    {
                        x = player.transform.position.x + Random.Range(-20f, 20f);
                        y = player.transform.position.y + Random.Range(-20f, 20f);
                    }
                    while (Mathf.Abs(x - player.transform.position.x) < w && Mathf.Abs(y - player.transform.position.y) < h);
                    enemies.Add(Instantiate(enemy, new Vector3(x, y), Quaternion.identity).GetComponent<Enemy>());
                    enemies[enemies.Count - 1].SetId(Random.value<starChance ? -id : id);
                    info.currentCount++;
                }
            }
        }
        for (int i = 0, count = items.Count; i < count; i++)
        {
            float m;
            Item it = items[i];
            if ((m = ((Vector2)(it.transform.position - player.transform.position)).magnitude) < 3f)
            {
                if (m < .2f)
                {
                    player.Upgrade(it.id);
                    infos.Add(Instantiate(pickingItemsInfo, StaticVars.canvas.transform.GetChild(0)));
                    GameObject info = infos[infos.Count - 1];
                    info.GetComponent<RectTransform>().anchoredPosition = new Vector2(16.2f, -28.4f - 26f * (infos.Count - 1));
                    info.GetComponent<Text>().text = string.Concat("You found ", StaticVars.itemNames[it.id], "!");
                    string s = "";
                    if(player.itemCounts[it.id]<=player.maxItemCounts[it.id])
                        switch (it.id)
                        {
                            case 7:
                                s = "Repair upgrades now available!";
                                break;
                            case 9:
                                s = "Life steal upgrades now available!";
                                break;
                            case 10:
                                s = "Stunning time upgrades now available!";
                                break;
                            case 11:
                                s = "Critical chance upgrades now available!";
                                break;
                            case 12:
                                s = "Damage reflection upgrades now available!";
                                break;
                            case 13:
                                s = "Slowing time upgrades now available!";
                                break;
                            case 14:
                                s = "Critical damage upgrades now available!";
                                break;
                        }
                    if (s != "")
                    {
                        infos.Add(Instantiate(pickingItemsInfo, StaticVars.canvas.transform.GetChild(0)));
                        GameObject info1 = infos[infos.Count - 1];
                        info1.GetComponent<RectTransform>().anchoredPosition = new Vector2(16.2f, -28.4f - 26f * (infos.Count - 1));
                        info1.GetComponent<Text>().text = s;
                    }
                    Destroy(it.gameObject);
                    items.RemoveAt(i);
                    break;
                }
                float k = .2f / m;
                it.transform.position = new Vector3(it.transform.position.x + (player.transform.position.x - it.transform.position.x) * k, it.transform.position.y + (player.transform.position.y - it.transform.position.y) * k);
            }
        }
    }

    public void NextLevel()
    {
        if (level % 3f == 0f || level % 1f == .5f)
            level += .5f;
        else
            level++;
        PlayerPrefs.SetFloat("lvl", level);
        player.Save();
        minimap.SetActive(false);
        levelLoader.LoadLevel(1);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        levelLoader.LoadLevel(1);
    }

    public void Over()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("music", musicSource.enabled ? "1" : "0");
        PlayerPrefs.SetString("sounds", selectSource.enabled ? "1" : "0");
        PlayerPrefs.SetFloat("musicVol", musicSource.volume);
        PlayerPrefs.SetFloat("soundsVol", selectSource.volume);
    }

    public void Pause()
    {
        if (selectSource.enabled)
            selectSource.Play();
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        minimap.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (selectSource.enabled)
            selectSource.Play();
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        minimap.SetActive(true);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        if (selectSource.enabled)
            selectSource.Play();
        Time.timeScale = 1f;
        levelLoader.LoadLevel(0);
    }

    public void OnOffMusic(bool value)
    {
        if (selectSource.enabled)
            selectSource.Play();
        musicSource.enabled = value;
        PlayerPrefs.SetString("music", value ? "1" : "0");
    }

    public void OnOffSounds(bool value)
    {
        for (int i = 0, count = soundsSources.Length; i < count; i++)
            soundsSources[i].enabled = value;
        if(selectSource.enabled)
            selectSource.Play();
        PlayerPrefs.SetString("sounds", value ? "1" : "0");
    }

    public void SetMusicVol(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("musicVol", value);
    }

    public void SetSoundsVol(float value)
    {
        for (int i = 0, count = soundsSources.Length; i < count; i++)
            soundsSources[i].volume = value;
        PlayerPrefs.SetFloat("soundsVol", value);
    }

    public void OpenSettings()
    {
        if (selectSource.enabled)
            selectSource.Play();
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        if (selectSource.enabled)
            selectSource.Play();
        settings.SetActive(false);
    }
}