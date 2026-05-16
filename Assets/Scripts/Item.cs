using UnityEngine;

public class Item : MonoBehaviour {

    public int id;
    private float rotSpd;
    private Vector2 spd;
    private void Start()
    {
        spd = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        rotSpd = Random.Range(0, 2) * 2f - 1f;
    }
    private void FixedUpdate()
    {
        spd *= 0.9f;
        transform.position = (Vector2)transform.position + spd;
        transform.Rotate(0f,0f,rotSpd);
    }
    public void SetId(int ID)
    {
        id = ID;
        GetComponent<SpriteRenderer>().sprite = StaticVars.itemSprites[id];
    }
}
