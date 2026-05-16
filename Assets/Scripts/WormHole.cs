using UnityEngine;

public class WormHole : MonoBehaviour {

    public GameObject shop;
    public Game game;
    public Player player;
    private bool active = false;
    private void Awake()
    {
        float level = PlayerPrefs.HasKey("lvl") ? PlayerPrefs.GetFloat("lvl") : 1f;
        if (level % 1f == 0f)
        {
            Random.InitState((int)level);
            float angle = Random.Range(-3.14159265358979f, 3.14159265358979f), distance = level * 2f + 20f;
            transform.position = new Vector3(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));
            angle += Random.Range(0, 2) * 2f - 1f;
            shop.transform.position = new Vector3(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));
            if (level % 3f == 0f)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
                gameObject.layer = 8;
            }
        }
        else
            gameObject.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.Rotate(0f, 0f, -1f);
        if (active)
        {
            float m;
            if ((m = ((Vector2)(player.transform.position - transform.position)).magnitude) < 2f)
            {
                float k = .04f / m;
                player.transform.position = new Vector3(player.transform.position.x + (transform.position.x - player.transform.position.x) * k, player.transform.position.y + (transform.position.y - player.transform.position.y) * k, 0f);
                player.transform.localScale = new Vector3(m * .5f, m * .5f);
                if (m < .4f)
                {
                    game.NextLevel();
                    Destroy(this);
                }
            }
            
        }
        else if (((Vector2)(player.transform.position - transform.position)).sqrMagnitude > 4f)
            active = true;
	}
}
