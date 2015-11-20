using UnityEngine;
using System.Collections;

public class WhiteboardExample : MonoBehaviour
{
    void Start()
    {
        Texture2D texture = new Texture2D(800, 200);

        Color fillColor = Color.white;
        Color[] fillColorArray = texture.GetPixels();
        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = fillColor;
        }
        texture.SetPixels(fillColorArray);
        texture.Apply();

        GetComponent<Renderer>().material.mainTexture = texture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                draw(ray, hit, Color.red, 2);
            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                draw(ray, hit, Color.white, 5);
            }
        }
    }

    public void draw(Ray ray, RaycastHit hit, Color color, int radius)
    {
        Debug.DrawLine(ray.origin, hit.point);

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        drawCircle(tex, (int)pixelUV.x, (int)pixelUV.y, radius, color);
        tex.Apply();
    }

    void drawCircle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);
            }
        }
    }

}