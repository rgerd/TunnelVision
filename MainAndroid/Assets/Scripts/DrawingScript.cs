using UnityEngine;
using System.Collections;

public class DrawingScript : MonoBehaviour {
	private int markerRadius = 5;
	private Color markerColor;

	private Vector3 rightCorrectDrawVec;
	private Vector3 rightDrawVec;

	private Vector3 leftCorrectDrawVec;
	private Vector3 leftDrawVec;

	private Vector2? lastMark;

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
		pixelUV.x *= tex.width;
		pixelUV.y *= tex.height;
		Vector2 thisMark = new Vector2 (pixelUV.x, pixelUV.y);
		if (lastMark == null)
			lastMark = thisMark;
			
		drawCircle(tex, (Vector2) lastMark, thisMark, markerRadius, markerColor);

		lastMark = new Vector2 (pixelUV.x, pixelUV.y);
		tex.Apply();
	}
	
	private void drawCircle(Texture2D tex, Vector2 start, Vector2 end, int r, Color col) {
		int dx = (int)(end.x - start.x);
		int dy = (int)(end.y - start.y);
		int d  = (int) Mathf.Sqrt (dx * dx + dy * dy);

		for (int i = 0; i < d; i++) {
			float p = (float) i / (float) d;
			int _x = (int) (start.x + dx * p);
			int _y = (int) (start.y + dy * p);

			for(int j = -r; j < r; j++) {
				for(int k = -r; k < r; k++) {
					tex.SetPixel(_x + j, _y + k, col);
				}
			}
		}

	}
}
