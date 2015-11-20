using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class SkeletonRender : Photon.MonoBehaviour {
	//public GameObject BodySourceManager;
	public GameObject bone_prefab;
	
	public static GameObject body;
	private ulong body_id;
	public static GameObject head;
	public static GameObject[] bones;
	private SkeletonManager skeletonManager;

    private Vector3 leftCorrectDrawVec = Vector3.zero;
    private Vector3 leftDrawVec = Vector3.zero;
    private Vector3 rightCorrectDrawVec = Vector3.zero;
    private Vector3 rightDrawVec = Vector3.zero;
    private Color markerColor = Color.white;
    private int markerRadius = 3;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
	{
		{ Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
		{ Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
		{ Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
		{ Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
		
		{ Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
		{ Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
		{ Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
		{ Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
		
		{ Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
		{ Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
		{ Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
		{ Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.HandRight, Kinect.JointType.WristRight },
		{ Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
		{ Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
		{ Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
		{ Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
		{ Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
		{ Kinect.JointType.Neck, Kinect.JointType.Head },
	};

    void Start ()
    {
    }
	
	void Update () {
		skeletonManager = GetComponent<SkeletonManager>();
		if (skeletonManager == null) return;
		
		Kinect.Body[] data = skeletonManager.GetData();
        if (data == null)
        {
            return;
        }
		
		List<ulong> trackingIds = new List<ulong> ();
		foreach (var body in data) {
			if(body == null) continue;
			if(body.IsTracked) trackingIds.Add(body.TrackingId);
		}

		if (!trackingIds.Contains (body_id)) {
			Destroy(body); body = null;
			body_id = 0;
		}
		
		foreach (var _body in data) {
			if (body_id == 0) body_id = _body.TrackingId;
			
			if (body_id == _body.TrackingId) {
				if (_body == null) continue;
				
				if (_body.IsTracked) {
					if (body == null)
						body = CreateBodyObject (body_id);
					RefreshBodyObject (_body, body);
				}
			}
		}
	}
	
	private GameObject CreateBodyObject(ulong body_id) {
		GameObject body = new GameObject("Body:" + body_id);
		
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			GameObject jointObj = null;
            if (jt == Kinect.JointType.Head)
            {
                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                jointObj = Instantiate(head);
            }
            else
                jointObj = new GameObject();//Instantiate(new GameObject());
			jointObj.name = jt.ToString();
			jointObj.transform.parent = body.transform;
		}
		
		Transform bodyTransform = body.transform;
		bones = new GameObject[9];
		bones[0] = addBone("Body", 1.5f, bone_prefab, bodyTransform, 
		                   Kinect.JointType.SpineBase, 
		                   Kinect.JointType.SpineShoulder);
		bones[1] = addBone("LeftArmTop", 0.8f, bone_prefab, bodyTransform, Kinect.JointType.ElbowLeft);
		bones[2] = addBone("LeftArmBottom", 0.5f, bone_prefab, bodyTransform, Kinect.JointType.WristLeft);
		bones[3] = addBone("RightArmTop", 0.8f, bone_prefab, bodyTransform, Kinect.JointType.ElbowRight);
		bones[4] = addBone("RightArmBottom", 0.5f, bone_prefab, bodyTransform, Kinect.JointType.WristRight);
		bones[5] = addBone("LeftLegTop", 0.8f, bone_prefab, bodyTransform, Kinect.JointType.KneeLeft);
		bones[6] = addBone("LeftLegBottom", 0.5f, bone_prefab, bodyTransform, Kinect.JointType.AnkleLeft);
		bones[7] = addBone("RightLegTop", 0.8f, bone_prefab, bodyTransform, Kinect.JointType.KneeRight);
		bones[8] = addBone("RightLegBottom", 0.5f, bone_prefab, bodyTransform, Kinect.JointType.AnkleRight);
		
		return body;
	}
	
	private GameObject addBone(string name, float radius, GameObject prefab, Transform body, Kinect.JointType joint1, Kinect.JointType? joint2 = null) {
        GameObject bone = (GameObject) Instantiate(prefab, Vector3.zero, Quaternion.identity);
		bone.name = name; bone.transform.parent = body;
		BoneScript script = bone.GetComponent("BoneScript") as BoneScript;
		script.radius = radius;
		script.joint1 = body.FindChild (joint1.ToString ()).gameObject;
		if (joint2 != null)
			script.joint2 = body.FindChild (((Kinect.JointType) joint2).ToString ()).gameObject;
		else if (_BoneMap.ContainsKey (joint1))
			script.joint2 = body.FindChild (_BoneMap [joint1].ToString ()).gameObject;
		else
			Debug.LogError ("BAD JOINT: " + joint1);
		return bone;
	}
	
	private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject) {
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			Kinect.Joint sourceJoint = body.Joints[jt];
			Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
			jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            if (jt == Kinect.JointType.Head)
            {
                Camera.main.transform.localRotation = Quaternion.identity;
                Camera.main.transform.Rotate(new Vector3(0, 180, 0));
                Camera.main.transform.position = (Vector3)jointObj.transform.position;
            }
		}


        whiteboard_raytrace(bodyObject.transform.FindChild(Kinect.JointType.ElbowRight.ToString()),
            bodyObject.transform.FindChild(Kinect.JointType.HandRight.ToString()),
            body.HandRightState,
            GameObject.Find("RightHandDraw").GetComponent<LineRenderer>(),
            "right");


        whiteboard_raytrace(bodyObject.transform.FindChild(Kinect.JointType.ElbowLeft.ToString()),
            bodyObject.transform.FindChild(Kinect.JointType.HandLeft.ToString()),
            body.HandLeftState,
            GameObject.Find("LeftHandDraw").GetComponent<LineRenderer>(),
            "left");


    }
	
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint) {
		return new Vector3(joint.Position.X * -10, joint.Position.Y * 10, joint.Position.Z * 10);
	}



    private void whiteboard_raytrace(Transform elbowObj, Transform handObj, Kinect.HandState handState, LineRenderer lr, string side)
    {
        RaycastHit hit;
        Ray ray;

        if (side == "right")
        {
            rightCorrectDrawVec = handObj.position - elbowObj.position;
            rightDrawVec = Vector3.Lerp(rightDrawVec, rightCorrectDrawVec, Time.deltaTime * 5);
            ray = new Ray(elbowObj.position, rightDrawVec);
        }
        else
        {
            leftCorrectDrawVec = handObj.position - elbowObj.position;
            leftDrawVec = Vector3.Lerp(leftDrawVec, leftCorrectDrawVec, Time.deltaTime * 5);
            ray = new Ray(elbowObj.position, leftDrawVec);
        }
        
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "whiteboard")
            {
                //Debug.Log("hit whiteboard");
                drawRayLine(lr, ray.origin, hit.point);
                if (handState == Kinect.HandState.Closed)
                    drawWhiteboard(hit);
            }
            else if (hit.collider.tag == "markerRed")
            {
                //Debug.Log("hit marker");
                markerColor = Color.red;
                markerRadius = 3;
                drawRayLine(lr, ray.origin, hit.point);
            }
            else if (hit.collider.tag == "markerEraser")
            {
                //Debug.Log("hit eraser");
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
            MeshCollider meshCollider = hit.collider as MeshCollider;
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
        this.photonView.RPC("ChatMessage", PhotonTargets.All, hit.transform.name, ((int)pixelUV.x).ToString(), ((int)pixelUV.y).ToString(), markerRadius.ToString(), markerColor.ToString());
            //drawCircle(tex, (int)pixelUV.x, (int)pixelUV.y, markerRadius, markerColor);  
    }


    [PunRPC]
    void ChatMessage(string name, string x, string y, string markerRadius, string markerColor)
    {
        //Debug.Log("ChatMessage " + name + " " + x + " " + y + " " + markerRadius + " " + markerColor);
        Texture2D tex = GameObject.Find(name).transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
        drawCircle(tex, int.Parse(x), int.Parse(y), int.Parse(markerRadius), Color.red);
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
        tex.Apply();
    }


}
