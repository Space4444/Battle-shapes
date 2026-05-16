using UnityEngine;

public class Shot : MonoBehaviour {
    
    private Vector2 speed;
    private int age = 0;
    private float maxSpd, accel;
    private bool homing = false;
    private Transform target;
    private Player player;

    public void Init(Vector2 spd)
    {
        speed = spd;
        player = StaticVars.player;
    }
    public void Init(Vector2 spd, Transform t)
    {
        speed = spd;
        maxSpd = speed.magnitude;
        accel = maxSpd * 0.05f;
        homing = true;
        target = t;
        player = StaticVars.player;
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (++age == StaticVars.maxShotAge)
            Destroy(gameObject);
        if (homing)
        {
            if (!target)
                homing = false;
            else
            {
                speed += accel * ((Vector2)(target.position - transform.position)).normalized;
                speed = maxSpd * speed.normalized;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(speed.y, speed.x) * Mathf.Rad2Deg);
            }
        }
        transform.position = (Vector2)transform.position + speed;
	}
    private void OnDestroy()
    {
        player.bullets.Remove(this);
    }
}
