using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class manage : MonoBehaviour {
	
	public Text i;
	private string save;
	public GameObject manager;

	// Use this for initialization
	void Start () {
		save = "";
	}
	
	// Update is called once per frame
	void Update () {
		string val = i.text;
		if (save != val) {
			GameObject.Destroy (GameObject.Find ("Manager(Clone)"));
			GameObject.Instantiate (manager);
			save = val;
		}
	}
}
