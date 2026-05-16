using UnityEngine;

public class EnemyShot : MonoBehaviour {

    private Player player;
    public SpriteRenderer rend;
    private float r2, maxSpd, accel, damage;
    private int id, age = 0, maxAge;
    private Vector2 speed;
    private GameObject exp, dying;
    private Color col1, col2;
    private System.Action Accelerate;
    private void Start()
    {
        exp = StaticVars.enemyShotExp;
        dying = StaticVars.enemyShotDie;
        r2 = 0.25f;
    }

    public void Create(int ID, Vector2 spd, float dmg, Color32 c1, Color32 c2)
    {
        player = StaticVars.player;
        speed = spd;
        maxSpd = speed.magnitude;
        accel = maxSpd * 0.05f;
        id = ID;
        damage = dmg;
        rend.material.SetColor("col1", col1 = c1);
        rend.material.SetColor("col2", col2 = c2);
        switch (id)
        {
            case 0:
                maxAge = 50;
                Accelerate = () => { };
                break;
            case 1:
                rend.sprite = StaticVars.shotSprites[1];
                maxAge = 120;
                Accelerate = () =>
                {
                    speed += accel * ((Vector2)(player.transform.position - transform.position)).normalized;
                    speed = maxSpd * speed.normalized;
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(speed.y, speed.x) * Mathf.Rad2Deg);
                };
                break;
        }
    }

    void FixedUpdate()
    {
        if (++age > maxAge)
        {
            Explosion e;
            (e = Instantiate(dying, transform.position, Quaternion.identity).GetComponent<Explosion>()).speed = speed;
            e.SetCol(col1,col2);
            Destroy(gameObject);
        }
        if ((transform.position - player.transform.position).sqrMagnitude < r2 + player.r2 && player.hitTimer > 25f)
        {
            player.Hit(damage);
            Explosion e;
            (e = Instantiate(exp, transform.position, Quaternion.identity).GetComponent<Explosion>()).speed = speed;
            e.SetCol(col1, col2);
            Destroy(gameObject);
        }
        Accelerate();
        transform.position = (Vector2)transform.position + speed;
    }
}
