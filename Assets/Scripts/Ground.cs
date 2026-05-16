using UnityEngine;

public class Ground : MonoBehaviour
{
    private int w, h;
    private float orthographicSize;
    private Sprite s;
    private SpriteRenderer rend;
    // Use this for initialization
    void Start()
    {
        orthographicSize = Camera.main.orthographicSize;
        h = (int)(orthographicSize * 200f);
        w = (int)(orthographicSize * Screen.width / Screen.height * 200f);
        rend = GetComponent<SpriteRenderer>();
        Color32[] c2 = rend.sprite.texture.GetPixels32();
        Texture2D t2 = new Texture2D(w, h);
        s = Sprite.Create(t2, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
        rend.sprite = s;
        int nx = w / 1024, ny = h / 1024;
        for (int i = 0; i < nx; i++)
            for (int j = 0; j < ny; j++)
                t2.SetPixels32(i * 1024, j * 1024, 1024, 1024, c2);
        for (int i = 0; i < nx; i++)
            for (int j = 0; j < 1024; j++)
                for (int k = 0; k < h % 1024; k++)
                    t2.SetPixel(i * 1024 + j, ny * 1024 + k, c2[j + k * 1024]);
        for (int i = 0; i < ny; i++)
            for (int j = 0; j < w % 1024; j++)
                for (int k = 0; k < 1024; k++)
                    t2.SetPixel(nx * 1024 + j, i * 1024 + k, c2[j + k * 1024]);
        for (int j = 0; j < w % 1024; j++)
            for (int k = 0; k < h % 1024; k++)
                t2.SetPixel(nx * 1024 + j, ny * 1024 + k, c2[j + k * 1024]);
        t2.Apply();
        rend.material.SetVector("scrSize", new Vector2(Screen.width, Screen.height));
        rend.material.SetVector("size2", new Vector2(orthographicSize * Screen.width / Screen.height, orthographicSize));
        c2 = null;
        Destroy(this);
    }
}