using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject shape, shot;
    public SpriteRenderer HPrend, shapeRend;
    public float r2, maxSpd = 0.15f;
    public Vector2 speed;
    protected bool star;
    protected Color32 col1, col2;
    protected Player player;
    protected Game game;
    protected int id, angles, money;
    protected float slowTime = -100f, stunTime = -100f, rotSpd, HP = 10, maxHP = 10, fireTimer = 500f, fireRate, angle = 0f, shotSpd, gunAngle, damage = 5f, accel = 0.005f;
    protected System.Action Rotate;
    private float alpha, shotDmg = 3f;
    private int hitTimer = 10;
    private enum MoveType{staying, moving, homing};
    private enum ShootingType{none, simple, homing};
    private MoveType moveType;
    private ShootingType shootingType;
    private System.Action Move, Shoot; 
    // Use this for initialization
    void Start () {
        HPrend.material.SetFloat("HP", HP);
        HPrend.material.SetFloat("maxHP", maxHP);
        r2 = 0.7f;
    }

    public virtual void SetId(int ID)
    {
        player = StaticVars.player;
        game = StaticVars.game;
        game.enemyCount++;
        star = ID < 0;
        id = Mathf.Abs(ID);
        int id1 = id % 27, id2 = id / 27;
        int r = id2*81, g=id2*94, b=id2*180,
            r1 = id2 * 147, g1 = id2 * 114, b1= id2 * 96;
        col1 = new Color32((byte)((255 + r) % 256), (byte)(g % 256), (byte)(b % 256), 255);
        col2 = new Color32((byte)((255 + r1) % 256), (byte)((255 + g1) % 256), (byte)(b1 % 256), 255);
        shapeRend.material.SetColor("col1", col1);
        shapeRend.material.SetColor("col2", col2);
        moveType = star ? MoveType.homing : (MoveType)(id1 % 3);
        shootingType = star ? ShootingType.homing : (ShootingType)((id1 / 3) % 3);
        int sprite = 27;
        switch (id1 / 9)
        {
            case 0:
                angles = 3;
                break;
            case 1:
                angles = 4;
                sprite = 28;
                break;
            case 2:
                angles = 6;
                sprite = 29;
                break;
        }
        shapeRend.sprite = StaticVars.enemySprites[star ? sprite : id1];
        gunAngle = 360f / angles;
        switch (shootingType)
        {
            case ShootingType.none:
            case ShootingType.simple:
                fireRate = 50f;
                float v1 = r2 * rotSpd * Mathf.Deg2Rad;
                shotSpd = .15f;
                alpha = Mathf.Atan2(v1, shotSpd);
                shotSpd = Mathf.Sqrt(v1 * v1 + shotSpd * shotSpd);
                break;
            case ShootingType.homing:
                fireRate = 200f;
                v1 = r2 * rotSpd * Mathf.Deg2Rad;
                shotSpd = .15f;
                alpha = Mathf.Atan2(v1, shotSpd);
                shotSpd = Mathf.Sqrt(v1 * v1 + shotSpd * shotSpd);
                break;
        }
        rotSpd = Random.Range(0, 2) * 2f - 1f;
        int id3 = id / 9;
        maxHP += 60 * id3;
        HP = maxHP;
        shotSpd = Mathf.Min(1f, shotSpd += .04f * id3);
        maxSpd = Mathf.Min(1f, maxSpd += .04f * id3);
        accel += .0015f * id3;
        shotDmg += 10f * id3;
        damage += 20f * id3;
        money = (id+1)*10;
        if (star)
        {
            fireRate *= .5f;
            maxHP *= 2f;
            HP = maxHP;
            accel *= 1.5f;
            shotDmg *= 2f;
            damage *= 2f;
            money *= 2;
        }
        switch (moveType)
        {
            case MoveType.staying:
                maxSpd = 0f;
                break;
            case MoveType.moving:
                speed = maxSpd * (player.transform.position - transform.position).normalized;
                break;
        }
        SetActions();
    }

    protected virtual void SetActions()
    {
        switch (moveType)
        {
            case MoveType.staying:
                Move = () => { };
                break;
            case MoveType.moving:
                Move = () =>
                {
                    if (Mathf.Abs(transform.position.x - player.transform.position.x) > 50 || Mathf.Abs(transform.position.y - player.transform.position.y) > 50)
                        Die();
                    transform.position = (Vector2)transform.position + speed;
                };
                break;
            case MoveType.homing:
                Move = () =>
                {
                    speed += accel * ((Vector2)(player.transform.position - transform.position)).normalized;
                    float m = speed.magnitude;
                    if (m > maxSpd)
                        speed = maxSpd * speed / m;
                    transform.position = (Vector2)transform.position + speed;
                };
                break;
        }
        switch (shootingType)
        {
            case ShootingType.none:
                Shoot = () => { };
                break;
            case ShootingType.simple:
                Shoot = () =>
                {
                    if (++fireTimer > fireRate)
                    {
                        fireTimer = 0f;
                        for (float i = 0f; i < 360f; i += gunAngle)
                        {
                            float ang = angle + i;
                            float angRad = ang * Mathf.Deg2Rad + alpha;
                            Instantiate(shot, transform.position, Quaternion.Euler(0f, 0f, ang)).GetComponent<EnemyShot>().Create(0, speed + shotSpd * new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad)), shotDmg, col1, col2);
                        }
                    }
                };
                break;
            case ShootingType.homing:
                Shoot = () =>
                {
                    if (++fireTimer > fireRate)
                    {
                        fireTimer = 0f;
                        for (float i = 0f; i < 360f; i += gunAngle)
                        {
                            float ang = angle + i;
                            float angRad = ang * Mathf.Deg2Rad + alpha;
                            Instantiate(shot, transform.position, Quaternion.Euler(0f, 0f, ang)).GetComponent<EnemyShot>().Create(1, speed + shotSpd * new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad)), shotDmg, col1, col2);
                        }
                    }
                };
                break;
        }
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

    public virtual void Hit(float dmg)
    {
        if (HP > 0f)
        {
            hitTimer = 0;
            HP -= dmg;
            if (HP <= 0f)
            {
                if (StaticVars.enemyExpSnd.enabled)
                    StaticVars.enemyExpSnd.Play();
                for (float i = 0f; i < 360f; i += gunAngle)
                {
                    float ang = angle + i;
                    float angRad = ang * Mathf.Deg2Rad + alpha;
                    Instantiate(shot, transform.position, Quaternion.Euler(0f, 0f, ang)).GetComponent<EnemyShot>().Create(0, speed + shotSpd * new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad)), shotDmg, col1, col2);
                }
                if (!game.bossLvl)
                {
                    float chance = 0f;
                    switch (moveType)
                    {
                        case MoveType.staying:
                            chance += .01f;
                            break;
                        case MoveType.moving:
                            chance += .05f;
                            break;
                        case MoveType.homing:
                            chance += .025f;
                            break;
                    }
                    switch (shootingType)
                    {
                        case ShootingType.simple:
                            chance += .025f;
                            break;
                        case ShootingType.homing:
                            chance += .05f;
                            break;
                    }
                    if (star)
                        chance *= 2f;
                    //chance = 1f;
                    if (Random.value < chance)
                    {
                        int item = /*new int[] {4,8,15}[Random.Range(0, 3)];/*/ Random.Range(0, player.itemCount);
                        if (player.itemCounts[item]<player.maxItemCounts[item] && (player.requiredItems[item]==-1 || player.itemCounts[player.requiredItems[item]]!=0))
                        {
                            game.items.Add(Instantiate(game.item, transform.position, Quaternion.identity).GetComponent<Item>());
                            game.items[game.items.Count - 1].SetId(item);
                        }
                    }
                }
                if (!game.bossLvl)
                    player.Earn(money);
                Explosion exp;
                (exp = Instantiate(StaticVars.enemyExp, transform.position, Quaternion.identity).GetComponent<Explosion>()).SetCol(col1, col2);
                exp.speed = speed;
                Die();
            }
            HPrend.material.SetFloat("HP", HP);
            HPrend.enabled = true;
        }
    }

    public virtual void Slow(float time)
    {
        if(time!=0f && slowTime<-2f)
        {
            slowTime = time;
            maxSpd *= .5f;
            rotSpd *= .5f;
            speed *= .5f;
            accel *= .5f;
            shapeRend.material.SetFloat("slow", .5f);
        }
    }

    public virtual void Stun(float time)
    {
        if (time != 0f && stunTime<-2f)
        {
            stunTime = time;
            shapeRend.material.SetFloat("stun", 1f);
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
            if(stunTime == 0f)
                shapeRend.material.SetFloat("stun", 0f);
            shapeRend.material.SetFloat("hit", hitTimer++ < 2 ? 1f : 0f);
            Rotate();
            shape.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            Move();
            Shoot();
        }
	}

    private void Die()
    {
        game.enemyCount--;
        int Id = (int)game.level - id - 1;
        if (id<3 && !game.bossLvl)
        {
            EnemyTypeInfo info = game.enemiesInfo[Id];
            info.currentCount--;
            info.generalCount--;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        game.enemies.Remove(this);
    }

}
