using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool piercing = false, laser = false;
    public bool moving = false;
    public int fireRate = 11;
    public int itemCount = 29, money = 0;
    public char[] itemCounts, maxItemCounts;
    public int[] requiredItems;
    public float damage = 2f, maxHP = 30f, shotSpd = .2f, repair = 0f, lifeSteal = 0f, stun = 0f, slow = 0f, critChance = 0f, critDmg = 2f;
    public float angle = 0, angle1, r2 = 0.7f, hitTimer, HP = 50f, reflection = 0f, maxSpd = .12f;
    public Text moneyText, livesText;
    public Turret turret;
    public AudioSource gunSource, strikeSource, expSource, pickUpSource;
    public ParticleSystem trail, healing;
    public GameObject shot, shape, gameOverMenu, minimap, joystick1, joystick2, shotExp, spikes, retryButt, gamOverTxt, armor, critExp;
    public SpriteRenderer backgroundRend, rend;
    public SpriteRenderer HPrend;
    public Game game;
    public List<Shot> bullets = new List<Shot>();
    private Vector2 spd = new Vector2();
    private Quaternion rotation;
    private bool firing = false, anglesMatch = false, homing = false;
    private int gunAmount = 1, fireTimer = 10, expTimer = 0, lives = 3;
    private float length;
    private float[] shotCoords = new float[1], shotAngles = new float[1];
    private Color whileInvis;
    private List<Color> shotColor = new List<Color>();
    private Dictionary<int, System.Action> Actions = new Dictionary<int, System.Action> { };
    private System.Action BulletsHitTest, InstantiateShots;

    void Awake()
    {
        itemCounts = new char[itemCount];
        requiredItems = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, 7, -1, -1, 9, 12, 11, 14, 10, 13, };
        maxItemCounts = new char[] {(char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)1,
                                    (char)255,
                                    (char)8,
                                    (char)255,
                                    (char)255,
                                    (char)16,
                                    (char)10,
                                    (char)12,
                                    (char)15,
                                    (char)16,
                                    (char)20,
                                    (char)16,
                                    (char)16,};
        if (PlayerPrefs.HasKey("shotSpd"))
        {
            shotSpd = PlayerPrefs.GetFloat("shotSpd");
            turret.shotSpd = shotSpd;
            turret.shotSpd1 = 1f / shotSpd;
        }
        if (PlayerPrefs.HasKey("maxShotAge"))
            StaticVars.maxShotAge = PlayerPrefs.GetInt("maxShotAge");
        if (PlayerPrefs.HasKey("damage"))
        {
            damage = PlayerPrefs.GetFloat("damage", damage);
            turret.damage = damage;
        }
        if (PlayerPrefs.HasKey("maxHP"))
            HP = maxHP = PlayerPrefs.GetFloat("maxHP", maxHP);
        if (PlayerPrefs.HasKey("speed"))
            maxSpd = PlayerPrefs.GetFloat("maxSpd", maxSpd);
        if (PlayerPrefs.HasKey("repair"))
            repair = PlayerPrefs.GetFloat("repair", repair);
        if (PlayerPrefs.HasKey("fireRate"))
            fireRate = fireTimer = PlayerPrefs.GetInt("fireRate");
        if (PlayerPrefs.HasKey("steal"))
            lifeSteal = PlayerPrefs.GetFloat("steal");
        if (PlayerPrefs.HasKey("stun"))
            stun = PlayerPrefs.GetFloat("stun");
        if (PlayerPrefs.HasKey("critChance"))
            critChance = PlayerPrefs.GetFloat("critChance");
        if (PlayerPrefs.HasKey("critDmg"))
            critDmg = PlayerPrefs.GetFloat("critDmg");
        if (PlayerPrefs.HasKey("reflection"))
            reflection = PlayerPrefs.GetFloat("reflection");
        if (PlayerPrefs.HasKey("slow"))
            slow = PlayerPrefs.GetFloat("slow");
        if (PlayerPrefs.HasKey("money"))
            moneyText.text = (money = PlayerPrefs.GetInt("money")).ToString();
        if (PlayerPrefs.HasKey("lives"))
        {
            livesText.text = (lives = PlayerPrefs.GetInt("lives")).ToString();
            if (lives == 0)
            {
                retryButt.SetActive(false);
                gamOverTxt.SetActive(true);
                float lvl = PlayerPrefs.HasKey("lvl") ? PlayerPrefs.GetFloat("lvl") : 1f;
                gamOverTxt.transform.GetChild(0).GetComponent<Text>().text = string.Concat("You reached level ", (int)lvl + (int)(Mathf.CeilToInt(lvl - 1f) * 0.33333333f));
            }
        }
        if (PlayerPrefs.HasKey("itemCounts0"))
        {
            for (int i = 0; i < itemCount; i++)
            {
                char[] c = PlayerPrefs.GetString(string.Concat("itemCounts", i)).ToCharArray();
                itemCounts[i] = c.Length == 0 ? (char)0 : c[0];
            }
            piercing = itemCounts[4] != 0;
            homing = itemCounts[15] != 0;
            if (itemCounts[6] != 0)
            {
                var col = trail.colorOverLifetime;
                Gradient grad = new Gradient();
                grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, .0f), new GradientColorKey(Color.blue, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .0f), new GradientAlphaKey(.0f, 1.0f) });
                col.color = grad;
            }
            if (itemCounts[8] != 0)
            {
                laser = true;
                shot = StaticVars.laser;
                gunSource.clip = StaticVars.shotSounds[1];
            }
            if (itemCounts[12] != 0)
                spikes.SetActive(true);
            if (itemCounts[5] != 0)
                armor.SetActive(true);
            if (itemCounts[16] != 0)
                turret.gameObject.SetActive(true);
        }
        shot.GetComponent<SpriteRenderer>().color = Color.red;
        if (!laser)
            shot.GetComponent<SpriteRenderer>().sprite = itemCounts[4] != 0 ? StaticVars.shotSprites[3] : StaticVars.shotSprites[2];
        if (itemCounts[3] != 0)
            SetShotColor(Color.yellow, true);
        if (itemCounts[10] != 0)
            SetShotColor(Color.cyan, true);
        if (itemCounts[13] != 0)
            SetShotColor(new Color(1f, .5f, 0f), true);
        if (itemCounts[15] != 0)
            SetShotColor(new Color(1f, .5f, 1f), true);
        if (PlayerPrefs.HasKey("gunAmount"))
        {
            gunAmount = PlayerPrefs.GetInt("gunAmount");
            turret.fireRate = Mathf.FloorToInt(2f * fireRate / gunAmount);
        }
        if (gunAmount == 2)
        {
            shotCoords = new float[2];
            shotCoords[0] = -.1f;
            shotCoords[1] = .1f;
            shotAngles = new float[2];
            shotAngles[0] = 0f;
            shotAngles[1] = 0f;
        }
        else if (gunAmount > 2)
        {
            shotCoords = new float[gunAmount];
            shotAngles = new float[gunAmount];
            float angle0 = -gunAmount * 0.025f;
            for (int i = 0; i < gunAmount; i++)
            {
                shotCoords[i] = 0f;
                shotAngles[i] = i == 0 ? angle0 : shotAngles[i - 1] + .05f;
            }
        }
        HPrend.material.SetFloat("HP", HP);
        HPrend.material.SetFloat("maxHP", maxHP);
        whileInvis = new Color(.5f, .5f, 1f);

        length = StaticVars.maxShotAge * shotSpd;
        Actions.Add(0, () =>
        {
            for (int i = 0; i < game.enemies.Count; i++)
            {
                Enemy enemy = game.enemies[i];
                for (int j = 0; j < bullets.Count; j++)
                {
                    Shot bullet = bullets[j];
                    Enemy enemyScript = enemy;
                    if ((enemy.transform.position - bullet.transform.position).sqrMagnitude < enemyScript.r2 * enemyScript.r2)
                    {
                        if (strikeSource.enabled)
                            strikeSource.Play();
                        float dmg;
                        bool crit = Random.value < critChance;
                        if (crit)
                            Instantiate(critExp, enemyScript.transform.position, Quaternion.identity);
                        enemyScript.Hit(dmg = crit ? damage * critDmg : damage);
                        enemyScript.Stun(stun);
                        enemyScript.Slow(slow);
                        Heal(dmg * lifeSteal);
                        Instantiate(shotExp, bullet.transform.position, Quaternion.identity);
                        Destroy(bullet.gameObject);
                    }
                }
            }
        });
        Actions.Add(1, () =>
        {
            for (int i = 0; i < game.enemies.Count; i++)
            {
                Enemy enemy = game.enemies[i];
                for (int j = 0; j < bullets.Count; j++)
                {
                    Shot bullet = bullets[j];
                    Enemy enemyScript = enemy;
                    if ((enemy.transform.position - bullet.transform.position).sqrMagnitude < enemyScript.r2 * enemyScript.r2)
                    {
                        float dmg = damage * Time.deltaTime * 50f;
                        bool crit = Random.value < critChance;
                        if (crit)
                            Instantiate(critExp, enemyScript.transform.position, Quaternion.identity);
                        enemyScript.Hit(dmg = crit ? dmg * critDmg : dmg);
                        enemyScript.Stun(stun);
                        enemyScript.Slow(slow);
                        Heal(dmg * lifeSteal);
                    }
                }
            }
        });
        Actions.Add(2, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                bullets.Add(Instantiate(shot, new Vector3(transform.position.x + coords * sin, transform.position.y - coords * cos), rot).GetComponent<Shot>());
                bullets[bullets.Count - 1].Init(new Vector2(shotSpd * cos, shotSpd * sin) + spd);
            }
        });
        Actions.Add(3, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                bullets.Add(Instantiate(shot, new Vector3(transform.position.x + coords * sin, transform.position.y - coords * cos), rot).GetComponent<Shot>());
                Enemy eMin = null;
                float dAngMin = float.MaxValue;
                for (int j = 0, count = game.enemies.Count; j < count; j++)
                {
                    Enemy e = game.enemies[j];
                    Vector2 delta = e.transform.position - transform.position;
                    float c = delta.magnitude, dAng = Mathf.Abs(ang1 - Mathf.Atan2(delta.y, delta.x));
                    if (dAngMin > dAng && c < length)
                    {
                        dAngMin = dAng;
                        eMin = e;
                    }
                }
                if (eMin)
                    bullets[bullets.Count - 1].Init(new Vector2(shotSpd * cos, shotSpd * sin) + spd, eMin.transform);
                else
                    bullets[bullets.Count - 1].Init(new Vector2(shotSpd * cos, shotSpd * sin) + spd);
            }
        });
        Actions.Add(4, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                float cMin = float.MaxValue;
                Enemy eMin = null;
                for (int j = 0, count = game.enemies.Count; j < count; j++)
                {
                    Enemy e = game.enemies[j];
                    Vector2 delta = e.transform.position - transform.position;
                    float c = delta.magnitude;
                    if (cMin > c && c < length)
                    {
                        float ang2 = Mathf.Atan2(delta.y, delta.x);
                        if (Mathf.Abs(Ang(ang1 - ang2)) * c < e.r2)
                        {
                            cMin = c;
                            eMin = e;
                        }
                    }
                }
                if (cMin != float.MaxValue)
                {
                    float dmg;
                    bool crit = Random.value < critChance;
                    if (crit)
                        Instantiate(critExp, eMin.transform.position, Quaternion.identity);
                    eMin.Hit(dmg = crit ? damage * critDmg : damage);
                    eMin.Stun(stun);
                    eMin.Slow(slow);
                    Heal(dmg * lifeSteal);
                }
                else
                    cMin = length;
                Instantiate(shot, (Vector2)transform.position + new Vector2(cos, sin) * cMin * 0.5f + new Vector2(coords * sin, -coords * cos), rot).transform.localScale = new Vector3(cMin, 1f);
            }
        });
        Actions.Add(5, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                bool hit = false, home = false;
            repeat:
                float ang0 = 0;
                float cMin = float.MaxValue;
                Enemy eMin = null;
                for (int j = 0, count = game.enemies.Count; j < count; j++)
                {
                    Enemy e = game.enemies[j];
                    Vector2 delta = e.transform.position - transform.position;
                    float c = delta.magnitude;
                    if (c < length)
                    {
                        float ang2 = Mathf.Atan2(delta.y, delta.x);
                        if (cMin > c)
                        {
                            if (Mathf.Abs(Ang(ang1 - ang2)) * c < e.r2)
                            {
                                home = false;
                                hit = true;
                                cMin = c;
                                eMin = e;
                            }
                        }
                        if (!(hit || home))
                            if (Mathf.Abs(Ang(ang1 - ang2)) < .5f)
                            {
                                ang0 = ang2;
                                home = true;
                                hit = true;
                            }
                    }
                }
                if (cMin != float.MaxValue)
                {
                    float dmg;
                    bool crit = Random.value < critChance;
                    if (crit)
                        Instantiate(critExp, eMin.transform.position, Quaternion.identity);
                    eMin.Hit(dmg = crit ? damage * critDmg : damage);
                    eMin.Stun(stun);
                    eMin.Slow(slow);
                    Heal(dmg * lifeSteal);
                }
                else if (home)
                {
                    ang1 = ang0;
                    sin = Mathf.Sin(ang1);
                    cos = Mathf.Cos(ang1);
                    rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                    goto repeat;
                }
                else
                    cMin = length;
                Instantiate(shot, (Vector2)transform.position + new Vector2(cos, sin) * cMin * 0.5f + new Vector2(coords * sin, -coords * cos), rot).transform.localScale = new Vector3(cMin, 1f);
            }
        });
        Actions.Add(6, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                for (int j = 0; j < game.enemies.Count; j++)
                {
                    Enemy e = game.enemies[j];
                    Vector2 delta = e.transform.position - transform.position;
                    float c = delta.magnitude;
                    if (c < length)
                    {
                        float ang2 = Mathf.Atan2(delta.y, delta.x);
                        if (Mathf.Abs(Ang(ang1 - ang2)) * c < e.r2)
                        {
                            float dmg;
                            bool crit = Random.value < critChance;
                            if (crit)
                                Instantiate(critExp, e.transform.position, Quaternion.identity);
                            e.Hit(dmg = crit ? damage * critDmg : damage);
                            e.Stun(stun);
                            e.Slow(slow);
                            Heal(dmg * lifeSteal);
                        }
                    }
                }
                Instantiate(shot, (Vector2)transform.position + new Vector2(cos, sin) * length * 0.5f + new Vector2(coords * sin, -coords * cos), rot).transform.localScale = new Vector3(length, 1f);
            }
        });
        Actions.Add(7, () =>
        {
            for (int i = 0, len = shotCoords.Length; i < len; i++)
            {
                float ang1 = angle1 + shotAngles[i], coords = shotCoords[i],
                      sin = Mathf.Sin(ang1), cos = Mathf.Cos(ang1);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                bool hit = false, home = false;
            repeat:
                float ang0 = 0;
                for (int j = 0; j < game.enemies.Count; j++)
                {
                    Enemy e = game.enemies[j];
                    Vector2 delta = e.transform.position - transform.position;
                    float c = delta.magnitude;
                    if (c < length)
                    {
                        float ang2 = Mathf.Atan2(delta.y, delta.x);
                        if (Mathf.Abs(Ang(ang1 - ang2)) * c < e.r2)
                        {
                            home = false;
                            hit = true;
                            float dmg;
                            bool crit = Random.value < critChance;
                            if (crit)
                                Instantiate(critExp, e.transform.position, Quaternion.identity);
                            e.Hit(dmg = crit ? damage * critDmg : damage);
                            e.Stun(stun);
                            e.Slow(slow);
                            Heal(dmg * lifeSteal);
                        }
                        if (!(hit || home))
                            if (Mathf.Abs(Ang(ang1 - ang2)) < .5f)
                            {
                                ang0 = ang2;
                                home = true;
                                hit = true;
                            }
                    }
                }
                if (home)
                {
                    ang1 = ang0;
                    sin = Mathf.Sin(ang1);
                    cos = Mathf.Cos(ang1);
                    rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                    goto repeat;
                }
                Instantiate(shot, (Vector2)transform.position + new Vector2(cos, sin) * length * 0.5f + new Vector2(coords * sin, -coords * cos), rot).transform.localScale = new Vector3(length, 1f);
            }
        });
        BulletsHitTest = laser ? () => { } : Actions[piercing ? 1 : 0];
        InstantiateShots = Actions[laser ? (piercing ? (homing ? 7 : 6) : (homing ? 5 : 4)) : (homing ? 3 : 2)];
        //HP = maxHP = 30000f;damage = 50f;speed = .5f;
    }

    private float Ang(float value)
    {
        if (value > Mathf.PI)
            value -= 6.28318530718f;
        if (value < -Mathf.PI)
            value += 6.28318530718f;
        return value;
    }

    public void AddLife()
    {
        PlayerPrefs.SetInt("lives", ++lives);
        livesText.text = lives.ToString();
    }

    public void Hit(float dmg)
    {
        if (HP > 0)
        {
            HP -= dmg;
            if (HP <= 0)
            {
                if (lives == 0)
                    game.Over();
                else
                    PlayerPrefs.SetInt("lives", --lives);
                if (expSource.enabled)
                    expSource.Play();
                game.musicSource.clip = game.deathMusic;
                if (game.musicSource.enabled)
                    game.musicSource.Play();
                Camera.main.GetComponent<CameraFollowing>().enabled = false;
                HPrend.gameObject.SetActive(false);
                transform.Find("Image").gameObject.SetActive(false);
                joystick1.SetActive(false);
                joystick2.SetActive(false);
                StaticVars.canvas.transform.GetChild(1).gameObject.SetActive(false);
                minimap.SetActive(false);
                trail.Stop();
                Instantiate(StaticVars.playerExp, transform.position, Quaternion.identity);
            }
        }
        HPrend.material.SetFloat("HP", HP);
        hitTimer = 0f;
    }

    public void Heal(float hp)
    {
        if (hp != 0 && HP < maxHP)
        {
            HP += hp;
            if (!healing.isPlaying)
                healing.Play();
        }
    }

    public void Save()
    {
        for (int i = 0; i < itemCount; i++)
            PlayerPrefs.SetString(string.Concat("itemCounts" + i.ToString()), itemCounts[i].ToString());
        PlayerPrefs.SetInt("gunAmount", gunAmount);
        PlayerPrefs.SetInt("fireRate", fireRate);
        PlayerPrefs.SetFloat("shotSpd", shotSpd);
        PlayerPrefs.SetInt("maxShotAge", StaticVars.maxShotAge);
        PlayerPrefs.SetFloat("damage", damage);
        PlayerPrefs.SetFloat("maxHP", maxHP);
        PlayerPrefs.SetFloat("maxSpd", maxSpd);
        PlayerPrefs.SetFloat("repair", repair);
        PlayerPrefs.SetFloat("steal", lifeSteal);
        PlayerPrefs.SetFloat("stun", stun);
        PlayerPrefs.SetFloat("critChance", critChance);
        PlayerPrefs.SetFloat("critDmg", critDmg);
        PlayerPrefs.SetFloat("reflection", reflection);
        PlayerPrefs.SetFloat("slow", slow);
        PlayerPrefs.SetInt("money", money);
    }

    public void Earn(int amount)
    {
        moneyText.text = (money += amount).ToString();
    }

    private void SetShotColor(Color c1, bool add)
    {
        if (add)
            shotColor.Add(c1);
        else
            shotColor.Remove(c1);
        Vector4 c = Vector4.zero;
        int count = shotColor.Count;
        for (int i = 0; i < count; i++)
            c += (Vector4)shotColor[i];
        shot.GetComponent<SpriteRenderer>().color = count == 4 ? new Color(1f, .9f, .9f) : count == 0 ? Color.red : (Color)(c /= count);
    }

    public void Upgrade(int id)
    {
        if (pickUpSource.enabled)
            pickUpSource.Play();
        if (itemCounts[id] < maxItemCounts[id])
            switch (id)
            {
                case 0:
                    fireRate = Mathf.FloorToInt(fireRate * (gunAmount + .75f) / gunAmount);
                    gunAmount++;
                    if (gunAmount == 2)
                    {
                        shotCoords = new float[2];
                        shotCoords[0] = -.1f;
                        shotCoords[1] = .1f;
                        shotAngles = new float[2];
                        shotAngles[0] = 0f;
                        shotAngles[1] = 0f;
                    }
                    break;
                case 1:
                    fireRate = Mathf.FloorToInt(fireRate * (gunAmount + 1.5f) / gunAmount);
                    gunAmount += 2;
                    break;
                case 2:
                    fireRate = Mathf.FloorToInt(fireRate * (gunAmount + 3.75f) / gunAmount);
                    gunAmount += 5;
                    break;
                case 3:
                    shotSpd += .1f;
                    turret.shotSpd += .1f;
                    StaticVars.maxShotAge += 50;
                    turret.length = length = StaticVars.maxShotAge * shotSpd;
                    damage *= 1.5f;
                    turret.damage *= 1.5f;
                    fireRate = Mathf.FloorToInt(fireRate * 1.5f);
                    turret.fireRate = Mathf.FloorToInt(turret.fireRate * 1.5f);
                    SetShotColor(Color.yellow, true);
                    break;
                case 4:
                    piercing = true;
                    if (!laser)
                    {
                        damage *= .2f;
                        shot.GetComponent<SpriteRenderer>().sprite = StaticVars.shotSprites[3];
                    }
                    BulletsHitTest = laser ? () => { } : Actions[1];
                    InstantiateShots = Actions[laser ? (homing ? 7 : 6) : (homing ? 3 : 2)];
                    break;
                case 5:
                    HPrend.material.SetFloat("HP", HP += 50f);
                    HPrend.material.SetFloat("maxHP", maxHP += 50f);
                    armor.SetActive(true);
                    break;
                case 6:
                    maxSpd += .025f;
                    var col = trail.colorOverLifetime;
                    Gradient grad = new Gradient();
                    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, .0f), new GradientColorKey(Color.blue, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .0f), new GradientAlphaKey(.0f, 1.0f) });
                    col.color = grad;
                    break;
                case 7:
                    repair += .02f;
                    break;
                case 8:
                    laser = true;
                    gunSource.clip = StaticVars.shotSounds[1];
                    Color c = shot.GetComponent<SpriteRenderer>().color;
                    shot = StaticVars.laser;
                    shot.GetComponent<SpriteRenderer>().color = c;
                    if (piercing)
                        damage *= 5f;
                    BulletsHitTest = () => { };
                    InstantiateShots = Actions[piercing ? (homing ? 7 : 6) : (homing ? 5 : 4)];
                    break;
                case 9:
                    lifeSteal += .07f;
                    break;
                case 10:
                    stun += 10;
                    SetShotColor(Color.cyan, true);
                    break;
                case 11:
                    critChance += .2f;
                    break;
                case 12:
                    reflection += .5f;
                    spikes.SetActive(true);
                    break;
                case 13:
                    slow += 20;
                    SetShotColor(new Color(1f, .5f, 0f), true);
                    break;
                case 14:
                    critDmg += .25f;
                    break;
                case 15:
                    homing = true;
                    SetShotColor(new Color(1f, .5f, 1f), true);
                    InstantiateShots = Actions[laser ? (piercing ? 7 : 5) : 3];
                    break;
                case 16:
                    turret.gameObject.SetActive(true);
                    break;
                case 17:
                    float mult = itemCounts[3] == 0 ? 1f : 1.5f;
                    damage += (piercing && !laser ? .1f : .5f) * mult;
                    turret.damage += .5f * mult;
                    break;
                case 18:
                    fireRate = Mathf.FloorToInt(fireRate * .9f);
                    turret.fireRate = Mathf.FloorToInt(turret.fireRate * .9f);
                    break;
                case 19:
                    HPrend.material.SetFloat("maxHP", maxHP += 25f);
                    HPrend.material.SetFloat("HP", HP += 25f);
                    break;
                case 20:
                    repair += .01f;
                    break;
                case 21:
                    maxSpd += .01f;
                    break;
                case 22:
                    shotSpd += .01f;
                    turret.length = length = StaticVars.maxShotAge * shotSpd;
                    turret.shotSpd = shotSpd;
                    turret.shotSpd1 = 1f / shotSpd;
                    break;
                case 23:
                    lifeSteal += .02f;
                    break;
                case 24:
                    reflection += .1f;
                    break;
                case 25:
                    critChance += .05f;
                    break;
                case 26:
                    critDmg += .1f;
                    break;
                case 27:
                    stun += 2f;
                    break;
                case 28:
                    slow += 4f;
                    break;
            }
        if (gunAmount > 2)
        {
            shotCoords = new float[gunAmount];
            shotAngles = new float[gunAmount];
            float angle0 = -gunAmount * 0.025f;
            for (int i = 0; i < gunAmount; i++)
            {
                shotCoords[i] = 0f;
                shotAngles[i] = i == 0 ? angle0 : shotAngles[i - 1] + 0.05f;
            }
        }
        itemCounts[id]++;
    }

    public void Downgrade(int id)
    {
        if (itemCounts[id] <= maxItemCounts[id])
            switch (id)
            {
                case 0:
                    fireRate = Mathf.CeilToInt(fireRate * (gunAmount - .75f) / gunAmount);
                    gunAmount--;
                    break;
                case 1:
                    fireRate = Mathf.CeilToInt(fireRate * (gunAmount - 1.5f) / gunAmount);
                    gunAmount -= 2;
                    if (gunAmount == 2)
                    {
                        shotCoords = new float[2];
                        shotCoords[0] = -.1f;
                        shotCoords[1] = .1f;
                        shotAngles = new float[2];
                        shotAngles[0] = 0f;
                        shotAngles[1] = 0f;
                    }
                    break;
                case 2:
                    fireRate = Mathf.CeilToInt(fireRate * (gunAmount - 3.75f) / gunAmount);
                    gunAmount -= 5;
                    if (gunAmount == 2)
                    {
                        shotCoords = new float[2];
                        shotCoords[0] = -.1f;
                        shotCoords[1] = .1f;
                        shotAngles = new float[2];
                        shotAngles[0] = 0f;
                        shotAngles[1] = 0f;
                    }
                    break;
                case 3:
                    shotSpd -= .1f;
                    turret.shotSpd -= .1f;
                    StaticVars.maxShotAge -= 50;
                    length = StaticVars.maxShotAge * shotSpd;
                    turret.length = length;
                    damage *= .66666666f;
                    turret.damage *= .66666666f;
                    fireRate = Mathf.CeilToInt(fireRate * .66666666f);
                    turret.fireRate = Mathf.FloorToInt(turret.fireRate * .66666666f) + 1;
                    SetShotColor(Color.yellow, false);
                    break;
                case 4:
                    piercing = false;
                    if (!laser)
                    {
                        damage *= 5f;
                        shot.GetComponent<SpriteRenderer>().sprite = StaticVars.shotSprites[2];
                    }
                    BulletsHitTest = laser ? () => { } : Actions[0];
                    InstantiateShots = Actions[laser ? (homing ? 5 : 4) : (homing ? 3 : 2)];
                    break;
                case 5:
                    HPrend.material.SetFloat("maxHP", maxHP -= 50f);
                    Hit(50f);
                    armor.SetActive(false);
                    break;
                case 6:
                    maxSpd -= .025f;
                    var col = trail.colorOverLifetime;
                    Gradient grad = new Gradient();
                    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, .0f), new GradientColorKey(new Color(1f, .5f, 0f), 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .0f), new GradientAlphaKey(.0f, 1.0f) });
                    col.color = grad;
                    break;
                case 7:
                    repair -= .02f;
                    break;
                case 8:
                    laser = false;
                    gunSource.clip = StaticVars.shotSounds[0];
                    Color c = shot.GetComponent<SpriteRenderer>().color;
                    shot = StaticVars.shot;
                    shot.GetComponent<SpriteRenderer>().color = c;
                    if (piercing)
                        damage *= .2f;
                    BulletsHitTest = Actions[piercing ? 1 : 0];
                    InstantiateShots = Actions[homing ? 3 : 2];
                    break;
                case 9:
                    lifeSteal -= .07f;
                    break;
                case 10:
                    stun -= 10;
                    SetShotColor(Color.cyan, false);
                    break;
                case 11:
                    critChance -= .2f;
                    break;
                case 12:
                    reflection -= .5f;
                    spikes.SetActive(false);
                    break;
                case 13:
                    slow -= 20;
                    SetShotColor(new Color(1f, .5f, 0f), false);
                    break;
                case 14:
                    critDmg -= .25f;
                    break;
                case 15:
                    homing = false;
                    SetShotColor(new Color(1f, .5f, 1f), false);
                    InstantiateShots = Actions[laser ? (piercing ? 6 : 4) : 2];
                    break;
                case 16:
                    turret.gameObject.SetActive(false);
                    break;
                case 17:
                    float mult = itemCounts[3] == 0 ? 1f : 1.5f;
                    damage -= (piercing && !laser ? .1f : .5f) * mult;
                    turret.damage -= .5f * mult;
                    break;
                case 18:
                    fireRate = Mathf.CeilToInt(fireRate * 1.111111111f);
                    turret.fireRate = Mathf.CeilToInt(turret.fireRate * 1.111111111f);
                    break;
                case 19:
                    HPrend.material.SetFloat("maxHP", maxHP -= 25f);
                    Hit(25f);
                    break;
                case 20:
                    repair -= .01f;
                    break;
                case 21:
                    maxSpd -= .01f;
                    break;
                case 22:
                    shotSpd -= .01f;
                    turret.length = length = StaticVars.maxShotAge * shotSpd;
                    turret.shotSpd = shotSpd;
                    turret.shotSpd1 = 1f / shotSpd;
                    break;
                case 23:
                    lifeSteal -= .02f;
                    break;
                case 24:
                    reflection -= .1f;
                    break;
                case 25:
                    critChance -= .05f;
                    break;
                case 26:
                    critDmg -= .1f;
                    break;
                case 27:
                    stun -= 2f;
                    break;
                case 28:
                    slow -= 4f;
                    break;
            }
        if (gunAmount > 2)
        {
            shotCoords = new float[gunAmount];
            shotAngles = new float[gunAmount];
            float angle0 = -gunAmount * 0.025f;
            for (int i = 0; i < gunAmount; i++)
            {
                shotCoords[i] = 0f;
                shotAngles[i] = i == 0 ? angle0 : shotAngles[i - 1] + 0.05f;
            }
        }
        else if (gunAmount == 1)
        {
            shotCoords = new float[1];
            shotAngles = new float[1];
            shotCoords[0] = 0;
            shotAngles[0] = 0;
        }
        itemCounts[id]--;
    }
    // Update is called once per frame
    void Update()
    {
        shape.transform.rotation = rotation;
        if (HP > 0)
            backgroundRend.material.SetVector("pos", new Vector2(transform.position.x, transform.position.y));
        BulletsHitTest();
        if (firing && fireTimer >= fireRate)
        {
            if (gunSource.enabled)
                gunSource.Play();
            InstantiateShots();
            fireTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (HP > 0)
        {
            if (HP < maxHP)
            {
                HP += repair;
                HPrend.material.SetFloat("HP", HP);
                HPrend.enabled = true;
            }
            else
            {
                HP = maxHP;
                HPrend.enabled = false;
            }
            if (hitTimer++ < 25)
                rend.color = hitTimer % 2 == 0 ? whileInvis : Color.white;
            if (moving)
                transform.position = (Vector2)transform.position + spd;
            fireTimer++;
        }
        else if (++expTimer > 200)
        {
            minimap.SetActive(false);
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StartMove()
    {
        moving = true;
        trail.Play();
    }

    public void StopMove()
    {
        moving = false;
        trail.Stop();
        spd = Vector2.zero;
    }

    public void StartFire()
    {
        firing = true;
    }

    public void StopFire()
    {
        firing = false;
    }

    public void Moving(float ang, float value)
    {
        if (value > .5f)
        {
            angle = ang;
            spd.Set(maxSpd * Mathf.Cos(angle), maxSpd * Mathf.Sin(angle));
            if (!firing || anglesMatch)
            {
                rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                angle1 = ang;
            }
        }
        else
            angle = angle1;
    }

    public void Firing(float ang, float value)
    {
        if (value > .5f)
        {
            anglesMatch = false;
            angle1 = ang;
            rotation = Quaternion.Euler(0, 0, ang * Mathf.Rad2Deg);
        }
        else
            anglesMatch = true;
    }

}