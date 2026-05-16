using UnityEngine;

public class Explosion : MonoBehaviour {

    public Vector2 speed;
    public ParticleSystem system;
    public float lifeTime = 10f;
    private float timer = 0f;
    public void SetCol(Color c1, Color c2)
    {
        var col = system.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, .0f), new GradientColorKey(c2, .33f), new GradientColorKey(c1, .66f), new GradientColorKey(Color.black, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .0f), new GradientAlphaKey(.0f, 1.0f) });
        col.color = grad;
    }
    private void FixedUpdate()
    {
        transform.position = (Vector2)transform.position + speed;
        timer++;
        if (timer > lifeTime)
            Destroy(gameObject);
    }
}
