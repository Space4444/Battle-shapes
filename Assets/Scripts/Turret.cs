using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    public Game game;
    public Player player;
    public GameObject shot, shotExp;
    public float fireRate = 22;
    public float length, damage = 2f, shotSpd = .25f, shotSpd1 = 4f;
    public AudioSource strikeSource;
    private int fireTimer = 20;
    private Vector2 deltaMin;
    private Enemy eMin;
    private List<GameObject> bullets = new List<GameObject>();

    private void Start()
    {
        length = StaticVars.maxShotAge * shotSpd;
    }

    private void Update()
    {
        bool found = false;
        float cMin1 = float.MaxValue;
        Enemy eMin1 = null;
        for (int j = 0, count = game.enemies.Count; j < count; j++)
        {
            Enemy e = game.enemies[j];
            Vector2 delta = e.transform.position - transform.position;
            float c = delta.magnitude;
            if (cMin1 > c && c < length)
            {
                cMin1 = c;
                eMin1 = e;
                deltaMin = delta;
                found = true;
            }
        }
        eMin = found ? eMin1 : null;
        for (int i = 0, count = game.enemies.Count; i < count; i++)
        {
            Enemy enemy = game.enemies[i];
            for (int j = 0, count1 = bullets.Count; j < count1; j++)
            {
                GameObject bullet = bullets[j];
                if (bullet)
                {
                    Enemy enemyScript = enemy;
                    if ((enemy.transform.position - bullet.transform.position).sqrMagnitude < enemyScript.r2 * enemyScript.r2)
                    {
                        if (strikeSource.enabled)
                            strikeSource.Play();
                        enemyScript.Hit(damage);
                        Instantiate(shotExp, bullet.transform.position, Quaternion.identity);
                        Destroy(bullet);
                    }
                }
                else
                {
                    bullets.RemoveAt(j);
                    count1--;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (eMin)
        {
            Vector2 plSpd = player.moving ? player.maxSpd * new Vector2(Mathf.Cos(player.angle), Mathf.Sin(player.angle)) : Vector2.zero;
            Vector2 spd = eMin.speed - plSpd;
            float ang1 = Mathf.Atan2(deltaMin.y, deltaMin.x);
            float sin = spd.magnitude * shotSpd1 * Mathf.Sin(Mathf.Atan2(spd.y, spd.x) - ang1);
            if (sin >= -1f && sin <= 1f)
            {
                ang1 += Mathf.Asin(sin);
                Quaternion rot = Quaternion.Euler(0f, 0f, ang1 * Mathf.Rad2Deg);
                transform.rotation = rot;
                if (++fireTimer > fireRate)
                {
                    fireTimer = 0;
                    bullets.Add(Instantiate(shot, transform.position, rot));
                    bullets[bullets.Count - 1].GetComponent<Shot>().Init(shotSpd * new Vector2(Mathf.Cos(ang1), Mathf.Sin(ang1)) + plSpd);
                }
            }
        }
        else
            transform.localRotation = Quaternion.identity;
    }
}