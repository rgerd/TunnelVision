using UnityEngine;
using System.Collections;

public class BoneScript : MonoBehaviour {
	public GameObject joint1;
	public GameObject joint2;
	public float radius = 1;
	void Update () {
		Vector3 midpoint = (joint1.transform.position + joint2.transform.position) / 2;
		transform.localPosition = midpoint;
		float dist = Vector3.Distance (joint1.transform.position, joint2.transform.position) / 2;
		if (dist == 0)
			return;
		transform.localScale = new Vector3(radius, dist, radius);
		transform.localRotation = Quaternion.LookRotation (joint2.transform.position - joint1.transform.position);
		transform.Rotate (new Vector3 (90, 0, 0));
	}
}
