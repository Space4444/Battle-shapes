using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Game game;
    public Player player;
    public WormHole hole;
    public Shop shop;
    public float scale = 2f;
    private float ang, absX, absY;
    public Vector2 holePos, shopPos;

    static Material Material;
    static void CreateLineMaterial()
    {
        if (!Material)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material = new Material(shader);
            Material.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            Material.SetInt("_ZWrite", 0);
        }
    }

    private void Start()
    {
        CreateLineMaterial();
        holePos = hole.transform.position;
        shopPos = shop.transform.position;
        // Apply the line material
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        Material.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        #region background
        GL.Begin(GL.QUADS);
        GL.Color(new Color(0f, 0f, 1f, .33f));
        GL.Vertex3(-85, -85, 0);
        GL.Vertex3(-85, 85, 0);
        GL.Vertex3(85, 85, 0);
        GL.Vertex3(85, -85, 0);
        GL.End();
        #endregion
        
        Vector2 plPos = player.transform.position, iconPos;

        if (!game.bossLvl || game.bossDefeated)
        {
            #region wormhole icon
            iconPos = (holePos - plPos) * scale;
            absX = System.Math.Abs(iconPos.x);
            absY = System.Math.Abs(iconPos.y);
            if (absX < 85d && absY < 85d)
            {
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(new Color(0f, 1f, 0f, 1f));
                GL.Vertex(new Vector2(5f, 0f) + iconPos);
                for (ang = 0.314159265358979f; ang < Mathf.PI; ang += .314159265358979f)
                {
                    Vector2 vec = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f;
                    GL.Vertex(vec + iconPos);
                    GL.Vertex(new Vector2(vec.x, -vec.y) + iconPos);
                }
                GL.Vertex(new Vector2(-5f, 0f) + iconPos);
                GL.End();
            }
            else
            {
                ang = (float)System.Math.Atan2(iconPos.y, iconPos.x);
                if (absX > absY)
                {
                    iconPos.x = 85f * System.Math.Sign(iconPos.x);
                    iconPos.y = 85f * Mathf.Tan(ang) * System.Math.Sign(iconPos.x);
                }
                else
                {
                    iconPos.x = 85f / Mathf.Tan(ang) * System.Math.Sign(iconPos.y);
                    iconPos.y = 85f * System.Math.Sign(iconPos.y);
                }
                GL.Begin(GL.TRIANGLES);
                GL.Color(new Color(0f, 1f, 0f, 1f));
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                ang += Mathf.PI * .66666666666666f;
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                ang += Mathf.PI * .66666666666666f;
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                GL.End();
            }
            #endregion
        }
        if (game.level % 3f == 0f)
        {
            #region shop icon
            iconPos = (shopPos - plPos) * scale;
            absX = System.Math.Abs(iconPos.x);
            absY = System.Math.Abs(iconPos.y);
            if (absX < 85d && absY < 85d)
            {
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(new Color(.9f, .9f, 0f, 1f));
                GL.Vertex(new Vector2(5f, 0f) + iconPos);
                for (ang = .314159265358979f; ang < Mathf.PI; ang += .314159265358979f)
                {
                    Vector2 vec = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f;
                    GL.Vertex(vec + iconPos);
                    GL.Vertex(new Vector2(vec.x, -vec.y) + iconPos);
                }
                GL.Vertex(new Vector2(-5f, 0f) + iconPos);
                GL.End();
            }
            else
            {
                ang = (float)System.Math.Atan2(iconPos.y, iconPos.x);
                if (absX > absY)
                {
                    iconPos.x = 85f * System.Math.Sign(iconPos.x);
                    iconPos.y = 85f * Mathf.Tan(ang) * System.Math.Sign(iconPos.x);
                }
                else
                {
                    iconPos.x = 85f / Mathf.Tan(ang) * System.Math.Sign(iconPos.y);
                    iconPos.y = 85f * System.Math.Sign(iconPos.y);
                }
                GL.Begin(GL.TRIANGLES);
                GL.Color(new Color(.9f, .9f, 0f, 1f));
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                ang += Mathf.PI * .66666666666666f;
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                ang += Mathf.PI * .66666666666666f;
                GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 9f + iconPos);
                GL.End();
            }
            #endregion
        }

        #region plyer icon
        GL.Begin(GL.TRIANGLES);
        GL.Color(new Color(1f, 1f, 1f, 1f));
        ang = player.angle1;
        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f);
        ang += Mathf.PI * .66666666666666f;
        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f);
        ang += Mathf.PI * .66666666666666f;
        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f);
        GL.End();
        #endregion

        if (game.bossLvl)
        {
            if (!game.bossDefeated)
            {
                #region boss icon
                iconPos = ((Vector2)game.boss.transform.position - plPos) * scale;
                absX = System.Math.Abs(iconPos.x);
                absY = System.Math.Abs(iconPos.y);
                if (absX < 85d && absY < 85d)
                {
                    GL.Begin(GL.TRIANGLE_STRIP);
                    GL.Color(Color.red);
                    GL.Vertex(new Vector2(5f, 0f) + iconPos);
                    for (ang = .314159265358979f; ang < Mathf.PI; ang += .78539816339f)
                    {
                        Vector2 vec = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f;
                        GL.Vertex(vec + iconPos);
                        GL.Vertex(new Vector2(vec.x, -vec.y) + iconPos);
                    }
                    GL.Vertex(new Vector2(-5f, 0f) + iconPos);
                    GL.End();
                }
                else
                {
                    ang = (float)System.Math.Atan2(iconPos.y, iconPos.x);
                    if (absX > absY)
                    {
                        iconPos.x = 85f * System.Math.Sign(iconPos.x);
                        iconPos.y = 85f * Mathf.Tan(ang) * System.Math.Sign(iconPos.x);
                    }
                    else
                    {
                        iconPos.x = 85f / Mathf.Tan(ang) * System.Math.Sign(iconPos.y);
                        iconPos.y = 85f * System.Math.Sign(iconPos.y);
                    }
                    GL.Begin(GL.TRIANGLES);
                    GL.Color(new Color(1f, 0f, 0f, 0.75f));
                    GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f + iconPos);
                    ang += Mathf.PI * .66666666666666f;
                    GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f + iconPos);
                    ang += Mathf.PI * .66666666666666f;
                    GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 8.5f + iconPos);
                    GL.End();
                }
                #endregion
            }
        }
        #region enemies icons
            for (int i = 0, count = game.enemies.Count; i < count; i++)
            {
                Enemy enemy = game.enemies[i];
                if (enemy)
                {
                    iconPos = ((Vector2)enemy.transform.position - plPos) * scale;
                    absX = System.Math.Abs(iconPos.x);
                    absY = System.Math.Abs(iconPos.y);
                    if (absX < 85d && absY < 85d)
                    {
                        GL.Begin(GL.TRIANGLE_STRIP);
                        GL.Color(Color.red);
                        GL.Vertex(new Vector2(2f, 0f) + iconPos);
                        for (ang = .314159265358979f; ang < Mathf.PI; ang += .78539816339f)
                        {
                            Vector2 vec = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 2f;
                            GL.Vertex(vec + iconPos);
                            GL.Vertex(new Vector2(vec.x, -vec.y) + iconPos);
                        }
                        GL.Vertex(new Vector2(-2f, 0f) + iconPos);
                        GL.End();
                    }
                    else
                    {
                        ang = (float)System.Math.Atan2(iconPos.y, iconPos.x);
                        if (absX > absY)
                        {
                            iconPos.x = 85f * System.Math.Sign(iconPos.x);
                            iconPos.y = 85f * Mathf.Tan(ang) * System.Math.Sign(iconPos.x);
                        }
                        else
                        {
                            iconPos.x = 85f / Mathf.Tan(ang) * System.Math.Sign(iconPos.y);
                            iconPos.y = 85f * System.Math.Sign(iconPos.y);
                        }
                        GL.Begin(GL.TRIANGLES);
                        GL.Color(new Color(1f, 0f, 0f, 0.75f));
                        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f + iconPos);
                        ang += Mathf.PI * .66666666666666f;
                        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f + iconPos);
                        ang += Mathf.PI * .66666666666666f;
                        GL.Vertex(new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * 5f + iconPos);
                        GL.End();
                    }
                }
            }
        #endregion

        GL.PopMatrix();
    }

    public void OnDragTouchpad(Vector2 pos)
    {
        pos = new Vector2(pos.x, pos.y + 180 - Screen.height);//for android
    }

    public void OnScaleChanged(float value)
    {
        scale = 0.1f + value * 1.9f;
    }

}