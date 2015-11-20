﻿using UnityEngine;
using System.Collections;

public class DrawingScript : Photon.MonoBehaviour {
	private int markerRadius = 5;
	private Color markerColor;

	private Vector3 rightCorrectDrawVec;
	private Vector3 rightDrawVec;
	private string lastRightState;

	private Vector3 leftCorrectDrawVec;
	private Vector3 leftDrawVec;
	private string lastLeftState;

	private Vector2? lastMarkLeft;
	private Vector2? lastMarkRight;

	void Start () {}
	
	// Update is called once per frame
	void Update () {
		Vector3 rightCorrectDrawVec = BodyScript.joints [(int)BodyScript.JointType.WristRight].transform.position - BodyScript.joints [(int)BodyScript.JointType.ElbowRight].transform.position;
		Vector3 leftCorrectDrawVec  = BodyScript.joints [(int)BodyScript.JointType.WristLeft].transform.position  - BodyScript.joints [(int)BodyScript.JointType.ElbowLeft].transform.position;


		if (BodyScript.handLeftState == "Closed" || BodyScript.handLeftState == "Open")
			lastLeftState = BodyScript.handLeftState;
		if (BodyScript.handRightState == "Closed" || BodyScript.handRightState == "Open")
			lastRightState = BodyScript.handRightState;

		if (lastLeftState == "Closed") {
			leftDrawVec = Vector3.Lerp (leftDrawVec, leftCorrectDrawVec, Time.deltaTime * 2);
			whiteboard_raytrace (leftDrawVec, BodyScript.joints [(int)BodyScript.JointType.WristLeft].transform, GameObject.Find ("LeftHandDraw").GetComponent<LineRenderer> (), "left");
		} else {
			leftDrawVec = leftCorrectDrawVec;
			lastMarkLeft = null;
			GameObject.Find ("LeftHandDraw").GetComponent<LineRenderer> ().enabled = false;
		}

		if (lastRightState == "Closed") {
			rightDrawVec = Vector3.Lerp (rightDrawVec, rightCorrectDrawVec, Time.deltaTime * 2);
			whiteboard_raytrace (rightDrawVec, BodyScript.joints [(int)BodyScript.JointType.WristRight].transform, GameObject.Find ("RightHandDraw").GetComponent<LineRenderer> (), "right");
		} else {
			rightDrawVec = rightCorrectDrawVec;
			lastMarkRight = null;
			GameObject.Find ("RightHandDraw").GetComponent<LineRenderer> ().enabled = false;
		}

	}

	private void whiteboard_raytrace(Vector3 drawVec, Transform elbow, LineRenderer lr, string side) {
		RaycastHit hit;
		Ray ray = new Ray(elbow.position, drawVec);
		
		if (Physics.Raycast(ray, out hit)) {

			if (hit.collider.tag == "whiteboard") {
				markerColor = Color.red;
				drawRayLine(lr, ray.origin, hit.point);
				drawWhiteboard(hit, side);
			} else if (hit.collider.tag == "markerRed") {
				Debug.Log("hit marker");
				markerColor = Color.red;
				markerRadius = 3;
				drawRayLine(lr, ray.origin, hit.point);
			} else if (hit.collider.tag == "markerEraser") {
				Debug.Log("hit eraser");
				markerColor = Color.white;
				markerRadius = 6;
				drawRayLine(lr, ray.origin, hit.point);
			}
		} else {
			lr.enabled = false;
		}
	}
	
	private void drawRayLine(LineRenderer lr, Vector3 pt1, Vector3 pt2)
	{
		lr.enabled = true;
		lr.SetColors(markerColor, markerColor);
		lr.SetPosition(0, pt1);
		lr.SetPosition(1, pt2);
	}
	
	
	private void drawWhiteboard(RaycastHit hit, string side)
	{
		//Debug.DrawLine(ray.origin, hit.point);
		Renderer rend = hit.transform.GetComponent<Renderer>();
		Texture2D tex = rend.material.mainTexture as Texture2D;
		Vector2 pixelUV = hit.textureCoord;
		pixelUV.x *= tex.width;
		pixelUV.y *= tex.height;
		Vector2 thisMark = new Vector2 (pixelUV.x, pixelUV.y);
		if (thisMark.x == 0 && thisMark.y == 0)
			return;
		if (side == "left") {
			if(lastMarkLeft == null)
				lastMarkLeft = thisMark;
		} else if (side == "right") {
			if(lastMarkRight == null)
				lastMarkRight = thisMark;
		}

		Vector2 lastMark = (Vector2)(side == "left" ? lastMarkLeft : lastMarkRight);
		//drawCircle(tex, (Vector2) (side == "left" ? lastMarkLeft : lastMarkRight), thisMark, markerRadius, markerColor);
		this.photonView.RPC("ChatMessage", PhotonTargets.All, hit.transform.name, ((int)lastMark.x).ToString (), ((int)lastMark.y).ToString (), ((int)pixelUV.x).ToString(), ((int)pixelUV.y).ToString(), markerRadius.ToString(), markerColor.ToString());

		if (side == "left") {
			lastMarkLeft = new Vector2 (pixelUV.x, pixelUV.y);
		} else if (side == "right") {
			lastMarkRight = new Vector2 (pixelUV.x, pixelUV.y);
		}
		tex.Apply();
	}


	[PunRPC]
	void ChatMessage(string name, string x1, string y1, string x2, string y2, string markerRadius, string markerColor)
	{
		//Debug.Log("ChatMessage " + name + " " + x + " " + y + " " + markerRadius + " " + markerColor);
		Texture2D tex = GameObject.Find(name).transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
		drawCircle(tex, new Vector2(int.Parse(x1), int.Parse(y1)), new Vector2(int.Parse(x2), int.Parse(y2)), int.Parse(markerRadius), Color.red);
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
