using UnityEngine;
using System.Collections;

public class DrawingScript : MonoBehaviour {
	private int markerRadius = 5;
	private Color markerColor;
	private Vector3 rightCorrectDrawVec;
	private Vector3 rightDrawVec;
	private Vector3 leftCorrectDrawVec;
	private Vector3 leftDrawVec;
	private Vector2 lastMark;

	void Start () {}
	
	// Update is called once per frame
	void Update () {
		whiteboard_raytrace(BodyScript.joints[(int)BodyScript.JointType.ElbowRight].transform,
		                    BodyScript.joints[(int)BodyScript.JointType.WristRight].transform, 
		                    BodyScript.handRightState, 
		                    GameObject.Find("RightHandDraw").GetComponent<LineRenderer>(), "right");

		whiteboard_raytrace(BodyScript.joints[(int)BodyScript.JointType.ElbowLeft].transform,
		                    BodyScript.joints[(int)BodyScript.JointType.WristLeft].transform, 
		                    BodyScript.handLeftState, 
		                    GameObject.Find("LeftHandDraw").GetComponent<LineRenderer>(), "left");
	}

	private void whiteboard_raytrace(Transform elbowObj, Transform handObj, string handState, LineRenderer lr, string side)
	{
		RaycastHit hit;
		Ray ray;
		
		if (side == "right")
		{
			rightCorrectDrawVec = handObj.position - elbowObj.position;
			rightDrawVec = rightCorrectDrawVec;//Vector3.Lerp(rightDrawVec, rightCorrectDrawVec, Time.deltaTime * 2);
			ray = new Ray(elbowObj.position, rightDrawVec);
		}
		else
		{
			leftCorrectDrawVec = handObj.position - elbowObj.position;
			leftDrawVec = leftCorrectDrawVec;//Vector3.Lerp(leftDrawVec, leftCorrectDrawVec, Time.deltaTime * 2);
			ray = new Ray(elbowObj.position, leftDrawVec);
		}
		
		
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.tag == "whiteboard")
			{
				//Debug.Log("hit whiteboard");
				markerColor = Color.red;
				drawRayLine(lr, ray.origin, hit.point);
				if (handState == "Closed")
					drawWhiteboard(hit);
			}
			else if (hit.collider.tag == "markerRed")
			{
				Debug.Log("hit marker");
				markerColor = Color.red;
				markerRadius = 3;
				drawRayLine(lr, ray.origin, hit.point);
			}
			else if (hit.collider.tag == "markerEraser")
			{
				Debug.Log("hit eraser");
				markerColor = Color.white;
				markerRadius = 6;
				drawRayLine(lr, ray.origin, hit.point);
			}
			
		}
		else
		{
			//lr.enabled = false;
		}
	}
	
	private void drawRayLine(LineRenderer lr, Vector3 pt1, Vector3 pt2)
	{
		lr.enabled = true;
		lr.SetColors(markerColor, markerColor);
		lr.SetPosition(0, pt1);
		lr.SetPosition(1, pt2);
		
	}
	
	
	private void drawWhiteboard(RaycastHit hit)
	{
		//Debug.DrawLine(ray.origin, hit.point);
		Renderer rend = hit.transform.GetComponent<Renderer>();
		Texture2D tex = rend.material.mainTexture as Texture2D;
		Vector2 pixelUV = hit.textureCoord;
		Debug.Log (pixelUV.x + ", " + pixelUV.y);
		pixelUV.x *= tex.width;
		pixelUV.y *= tex.height;

		//for (float i = 0; i <= 1f; i += 0.5f) {
			//Vector2 l = Vector2.Lerp (lastMark, new Vector2(pixelUV.x, pixelUV.y), i);
			drawCircle(tex, (int) pixelUV.x, (int) pixelUV.y, markerRadius, markerColor);
		//}

		lastMark = new Vector2 (pixelUV.x, pixelUV.y);
		tex.Apply();
	}
	
	private void drawCircle(Texture2D tex, int cx, int cy, int r, Color col)
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
