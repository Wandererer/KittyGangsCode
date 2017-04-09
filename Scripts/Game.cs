using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game  {

	private static Game instance;

	public static Game Instance
	{
		get 
		{ 
			if (instance == null)
				instance = new Game ();

			return instance;
		}

	}


	public GameScene gameScene { get; private set; }
	public RoomManager roomManager { get; private set;}

	public Game()
	{
		roomManager = new RoomManager ();
		gameScene = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameScene> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
