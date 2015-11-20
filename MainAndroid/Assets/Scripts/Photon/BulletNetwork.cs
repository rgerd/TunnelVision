using UnityEngine;
using System.Collections;

public class BulletNetwork : Photon.MonoBehaviour {
	
	private bool appliedInitialUpdate;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(transform.localScale);
		}
		else
		{
			//Network player, receive data
			//controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			//GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
			
			if (!appliedInitialUpdate)
			{
				appliedInitialUpdate = true;
				transform.position = correctPlayerPos;
				transform.rotation = correctPlayerRot;
				//GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	private Vector3 correctPlayerScale = Vector3.zero;
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
			transform.localScale = Vector3.Lerp (transform.localScale, correctPlayerScale, Time.deltaTime * 5);
		}
	}
	
}
