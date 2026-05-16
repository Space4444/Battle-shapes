using UnityEngine;

public class Boss : Enemy
{
    
    //private Color col1;
    private bool shooting;
    private GameObject healthBar, wormHole;
    private Minimap miniMap;
    private int phase = 0/*, hitTimer = 0*/, shotId;

    private void Start()
    {
        player = StaticVars.player;
        game = StaticVars.game;
        wormHole = StaticVars.wormHole;
        healthBar = StaticVars.bossHealthBar;
        miniMap = StaticVars.miniMap;
        healthBar.transform.parent.gameObject.SetActive(true);
    }

    public override void SetId(int ID)
    {
        star = ID < 0;
        id = Mathf.Abs(ID);
        int id1 = id % 9;
        r2 = 2.45f;
        switch (id1)
        {
            case 0:
                rotSpd = Random.Range(0, 2) * 10f - 5f;
                HP = maxHP = 200f;
                damage = 10;
                maxSpd = .2f;
                accel = .0075f;
                break;
            case 1:
                rotSpd = Random.Range(0, 2) * 5f - 2.5f;
                HP = maxHP = 300f;
                damage = 15;
                maxSpd = .25f;
                accel = .01f;
                fireRate = 10f;
                shotSpd = .2f;
                break;
            case 2:
                rotSpd = Random.Range(0, 2) * 6f - 3f;
                HP = maxHP = 500f;
                damage = 20;
                maxSpd = .3f;
                accel = .015f;
                fireRate = 20f;
                shotSpd = .15f;
                break;
            case 3:
                rotSpd = Random.Range(0, 2) * 20f - 10f;
                HP = maxHP = 1000f;
                damage = 30;
                maxSpd = .35f;
                accel = .0175f;
                if (!star)
                    r2 = 3.46f;
                break;
            case 4:
                rotSpd = Random.Range(0, 2) * 6f - 3f;
                HP = maxHP = 1500f;
                damage = 40;
                maxSpd = .35f;
                accel = .0175f;
                fireRate = 10f;
                shotSpd = .2f;
                if (!star)
                    r2 = 3.46f;
                break;
            case 5:
                rotSpd = Random.Range(0, 2) * 6f - 3f;
                HP = maxHP = 2000f;
                damage = 50;
                maxSpd = .375f;
                accel = .018f;
                fireRate = 20f;
                shotSpd = .15f;
                if (!star)
                    r2 = 3.46f;
                break;
            case 6:
                rotSpd = Random.Range(0, 2) * 22f - 11f;
                HP = maxHP = 3000f;
                damage = 70;
                maxSpd = .4f;
                accel = .02f;
                break;
            case 7:
                rotSpd = Random.Range(0, 2) * 8f - 4f;
                HP = maxHP = 4000f;
                damage = 100;
                maxSpd = .4f;
                accel = .02f;
                fireRate = 10f;
                shotSpd = .3f;
                break;
            case 8:
                rotSpd = Random.Range(0, 2) * 8f - 4f;
                HP = maxHP = 6000f;
                damage = 120;
                maxSpd = .41f;
                accel = .022f;
                fireRate = 20f;
                shotSpd = .2f;
                break;
        }
        if (shooting = star || id1 % 3 != 0)
        {
            int Id = id / 3 * 9 + id % 3;
            shotId = star ? -Id : Id;
        }
        int id2 = id / 9;
        int r = id2 * 81, g = id2 * 94, b = id2 * 180;
        if(id2!=0)
            shapeRend.material.SetColor("col1", col1 = new Color32((byte)((255 + r) % 256), (byte)(g % 256), (byte)(b % 256), 255));
        int i = 9;
        switch (id1 / 3)
        {
            case 0:
                angles = 3;
                break;
            case 1:
                angles = 4;
                i = 10;
                break;
            case 2:
                angles = 6;
                i = 11;
                break;
        }
        shapeRend.sprite = StaticVars.bossSprites[star ? i : id1];
        gunAngle = 360f / angles;
        HP += 10000 * id2;
        maxHP = HP;
        damage += 150 * id2;
        maxSpd = Mathf.Min(1f, maxSpd += .25f * id2);
        accel += .02f * id2;
        if ((fireRate -= 2f * id2) < 5f)
            fireRate = 5f;
        shotSpd = Mathf.Min(1f, shotSpd += .2f * id2);
        money = (id+1) * 400;
        if (star)
        {
            fireRate *= .5f;
            maxHP *= 2f;
            HP = maxHP;
            accel *= 1.5f;
            damage *= 2f;
            money *= 2;
        }
        SetActions();
    }

    protected override void SetActions()
    {
        if (star)
            Rotate = () =>
            {
                angle = Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);
            };
        else
            Rotate = () =>
            {
                angle += rotSpd;
            };
    }

    public override void Hit(float dmg)
    {
        if (HP > 0)
        {
            //rend.material.SetFloat("hit", .5f);
            //hitTimer = 0;
            HP -= dmg;
            healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(HP / maxHP, 1f);
            if (HP <= 0)
            {
                if(StaticVars.bossExpSnd.enabled)
                    StaticVars.bossExpSnd.Play();
                wormHole.transform.position = transform.position;
                miniMap.holePos = transform.position;
                wormHole.SetActive(true);
                healthBar.transform.parent.gameObject.SetActive(false);
                game.bossDefeated = true;
                game.musicSource.clip = game.commonLvlMusic;
                if (game.musicSource.enabled)
                    game.musicSource.Play();
                for (int i = 0, count = star ? 2 : 1; i < count; i++)
                {
                    int item = Random.Range(0, player.itemCount);
                    int item0 = item;
                    for (; player.itemCounts[item] >= player.maxItemCounts[item] || (player.requiredItems[item] != -1 && player.itemCounts[player.requiredItems[item]] == 0); item = item + 1 < player.itemCount ? ++item : 0)
                        if ((item + 1) % player.itemCount == item0)
                        {
                            item = -1;
                            break;
                        }
                    if (item >= 0)
                    {
                        game.items.Add(Instantiate(game.item, transform.position, Quaternion.identity).GetComponent<Item>());
                        game.items[game.items.Count - 1].SetId(item);
                    }
                }
                player.Earn(money);
                Instantiate(StaticVars.bossExp, transform.position, Quaternion.identity).GetComponent<Explosion>().SetCol(col1, Color.yellow);
                Destroy(gameObject);
            }
        }
    }
    
    private void Update()
    {
        if (stunTime <= 0f)
        {
            float r = r2 + player.r2;
            if ((transform.position - player.transform.position).sqrMagnitude < r * r && player.hitTimer > 25f)
            {
                player.Hit(damage);
                if (player.reflection != 0f)
                    Hit(damage * player.reflection);
            }
        }
    }

    private void ChangePhase(params int[] p)
    {
        phase = p[Random.Range(0, p.Length)];
    }

    private void FixedUpdate()
    {
        if (--slowTime == 0f)
        {
            maxSpd *= 2f;
            rotSpd *= 2f;
            speed *= 2f;
            accel *= 2f;
            shapeRend.material.SetFloat("slow", 0f);
        }
        if (--stunTime <= 0f)
        {
            if (stunTime == 0f)
                shapeRend.material.SetFloat("stun", 0f);
            //rend.material.SetFloat("hit", hitTimer++ < 2 ? 1f : 0f);
            Rotate();
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            switch (phase)
            {
                case 0:
                    if (Mathf.Abs(transform.position.x - player.transform.position.x) > 20 || Mathf.Abs(transform.position.y - player.transform.position.y) > 20)
                    {
                        ChangePhase(1);
                        speed = maxSpd * (player.transform.position - transform.position).normalized;
                    }
                    else if (Mathf.Abs(transform.position.x - player.transform.position.x) > 4 || Mathf.Abs(transform.position.y - player.transform.position.y) > 4)
                        if (Random.value < .01f)
                        {
                            if (shooting)
                                ChangePhase(1, 2);
                            else
                                ChangePhase(1);
                            speed = maxSpd * (player.transform.position - transform.position).normalized;
                        }
                    speed += accel * ((Vector2)(player.transform.position - transform.position)).normalized;
                    speed = maxSpd * speed.normalized;
                    transform.position = (Vector2)transform.position + speed;
                    break;
                case 1:
                    if (Random.value < .01f)
                        if (shooting)
                            ChangePhase(0, 2);
                        else
                            ChangePhase(0);
                    if (Mathf.Abs(transform.position.x - player.transform.position.x) > 10 || Mathf.Abs(transform.position.y - player.transform.position.y) > 10)
                        speed = maxSpd * (player.transform.position - transform.position).normalized;
                    transform.position = (Vector2)transform.position + speed;
                    break;
                case 2:
                    if (game.enemyCount>=game.maxEnemyCount || Random.value < .01f)
                        ChangePhase(0, 1);
                    if (++fireTimer > fireRate)
                    {
                        fireTimer = 0f;
                        for (float i = 0; i < 360; i += gunAngle)
                        {
                            float ang = angle + i;
                            float angRad = ang * Mathf.Deg2Rad;
                            Enemy enemy;
                            Vector2 vector = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                            game.enemies.Add(enemy = Instantiate(shot, (Vector2)transform.position + (r2 - 1) * vector, Quaternion.identity).GetComponent<Enemy>());
                            enemy.SetId(shotId);
                            enemy.speed = shotSpd * vector;
                        }
                    }
                    break;
            }
        }
    }

}