using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetactFastCat : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter2D(Collider2D other)
	{
		//Check Trigger

		if (other.transform.tag=="Player" && !other.gameObject.GetComponent<PhotonView> ().isMine && other.gameObject.GetComponent<CharacterController> ().CharacterNumber == 1) {
			//if Collider object tag is player and photonview component is not mine and charater number==2 activate this function
			Debug.Log("test");
			if (other.gameObject.GetComponent<SpriteRenderer> ().enabled == false) {
				//if Sprite renderer is false
				other.gameObject.GetComponent<PhotonView> ().RPC ("DeactiveTransparentSkill", PhotonTargets.AllBuffered, null); //call deactvie skill rpc
			}
		}
	}
}
