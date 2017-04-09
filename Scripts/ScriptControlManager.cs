using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptControlManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find ("GameManager").GetComponent<GameScene>().enabled=true;

	}
	// Update is called once per frame
	void Update () {
		
	}
}
