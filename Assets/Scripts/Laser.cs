using UnityEngine;

public class Laser : MonoBehaviour {

    private int timer = 0;
    public SpriteRenderer rend;
	void FixedUpdate () {
        Color c = rend.color;
        rend.color = new Color(c.r, c.g, c.b, 1f - timer * 0.04f);
        if (++timer > 25)
            Destroy(gameObject);
	}
}
