using UnityEngine;
using System.Collections;

public class WhiteboardExample : MonoBehaviour
{
    void Start()
    {
        Texture2D texture = new Texture2D(128, 128);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed left click.");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point);
           
                Renderer rend = hit.transform.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;
                Texture2D tex = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;
                tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
                tex.Apply();

            }
                
        }
    }
}