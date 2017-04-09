using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterController : Photon.MonoBehaviour {

	private float speed=1f; //character speed
	private float tranparentSkillTime=2.5f; //fast cat skill time;
	private float detactSkillTime=2.5f; //eyecat skill time;
	private float sternSkillTime=2.5f; // strong cat skill time;

	Vector3 realPos;

	[SerializeField]
	private int characterNumber; //선택된 캐릭터 번호 - 해당 번호로 선택된 것만 움직이도록 만듬

	private GameObject myCharacter;

	private Color fastKatColor; //cat color

	private Renderer fastKatRenderer; //cat object Renderer

	private bool isDetactSkillActive=false; //eye cat skill is active?
	private bool isTranparentSkillActive=false; //fast cat skill is active?
	private bool isSternSkillAcrive=false; //strong cat skill is active?

	private SpriteRenderer mySpriteRenderer;

	private Vector3 prePosWhenActiveTransSkill;

	private Character character;

	private Vector3 LeftFireVector = new Vector3 (-1, 1, 0);
	private Vector3 RightFireVector = new Vector3(1,-1,0);

	string[] trashNameList = { "FastCatTrash", "EyeCatTrash", "StrongCatTrash" };

	public int CharacterNumber{
		get { return characterNumber;}
	}

	public Color FastCatColor{
		get { return fastKatColor; }
		set { fastKatColor = value;}
	}

	public bool DetactSkillActive{
		get { return isDetactSkillActive;}
		set { isDetactSkillActive = value;}
	}

	public bool TransparentSkillAcrive{
		get { return isTranparentSkillActive;}
		set { isTranparentSkillActive = value;}
	}

	public bool SternSkillActive{
		get{ return isSternSkillAcrive;}
		set { isSternSkillAcrive = value;}
	}

	void Awake()
	{
		
	}

	// Use this for initialization
	void Start () {
		ChangeMyObjectName (); //Change My Objects Names
		SetFastCatVariable (); //Set Fast Cat Variables
		character=GetComponent<Character>();
		mySpriteRenderer=GetComponent<SpriteRenderer>();
	}



	// Update is called once per frame
/*	void Update () {
		if (character.MotionState == CharacterMotionState.Stern)
			return;

		if(Game.Instance.gameScene.IsPlaying)
		{
		Move (); //Move My Character and Call Rpc For synchronize 
		ActiveSkill (); //Active my skill by Characternumber
		DeActiveSkill (); //Deactive my skill by Characternumber
		ThrowObject (); // Throw Object Motion
		//CheckSkillTimeByCharNumber (); //Check Skill Time By Character Number
		}
	}*/

	void Update()
	{
		if (character.MotionState == CharacterMotionState.Stern)
			return;

		if(Game.Instance.gameScene.IsPlaying)
		{
			Move (); //Move My Character and Call Rpc For synchronize 
			ActiveSkill (); //Active my skill by Characternumber
			DeActiveSkill (); //Deactive my skill by Characternumber
			ThrowObject (); // Throw Object Motion
			CheckSkillTimeByCharNumber (); //Check Skill Time By Character Number
		}
	}

	private void SetFastCatVariable()
	{
		//Set Fast Cat Color
		if (this.CharacterNumber == 2) {
			fastKatRenderer = this.GetComponent<Renderer> ();
			fastKatColor =fastKatRenderer.material.color;
		}
	}

	private void SetGageSlider()
	{
		
	}

	void ChangeMyObjectName() //내가 움직이는 캐릭터 이름 변경 
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject obj in objects)
		{
			if(obj.GetComponent<PhotonView>().isMine)
			{
				myCharacter = obj;
				obj.name = obj.GetComponent<PhotonView> ().viewID.ToString ();
			}
		}
	}


	void DissapearMySliders()
	{
		UISprite[] myUiSliders = this.GetComponentsInChildren<UISprite> ();

		foreach(UISprite sprite in myUiSliders)
		{
			sprite.enabled = false;
		}
	}

	void AppearMySliders()
	{
		UISprite[] myUiSliders = this.GetComponentsInChildren<UISprite> ();

		foreach(UISprite sprite in myUiSliders)
		{
			sprite.enabled = true;
		}
	}

	void Move()
	{
		if(Game.Instance.gameScene.SelectedCharacter!=characterNumber)
		{
			return; //자기가 선택한 번호가 다를 경우 움직이지 않도록 
		}

		float horizontal = Input.GetAxis ("Horizontal"); //평행 움직임 받아옴 
		float vertial = Input.GetAxis ("Vertical"); //수직 움직임 받아옴

		if(horizontal ==0 && vertial==0)
		{
			return;	
		}


		ChangeSortingOrderWhenMove ();


		Vector3 newPos = new Vector3 (horizontal, vertial, 0); //키보드 입력 대로 움직임 
		newPos = newPos * speed * Time.deltaTime; //각 기기별 속도 같게 만듬 
		//newPos = newPos * speed;
	 	
		realPos = (newPos * -1); //상대방 뷰에서 반대로 움직이도록 만듬 


		Vector3 tempPos = (transform.position + newPos);
		tempPos=Camera.main.WorldToViewportPoint (tempPos);  //경계선 영역을 넘겼는지 좌표를 계산


		if ((tempPos.x < 0 || tempPos.x > 1) || (tempPos.y < 0 || tempPos.y > 1)) //화면 경계선은 넘지 않도록 만듬
			return;
	

		transform.position += newPos; //움직임 적용 
			

		GetComponent<PhotonView> ().RPC ("ChangeRotationByHorizontal", PhotonTargets.All, horizontal);
		GetComponent<PhotonView> ().RPC ("MoveMe", PhotonTargets.All, realPos); //RPC 통신으로 내가 움직이면 상대방 뷰에서는 반대로 움직이게함 
	}

	[PunRPC]
	public void ChangeRotationByHorizontal(float h)
	{
		if(h>0){
			GetComponent<Transform> ().localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		}
		else if(h<0)
		{
			GetComponent<Transform> ().localRotation = Quaternion.Euler  (new Vector3 (0, 0, 0));
		}
	}


	private void ChangeSortingOrderWhenMove()
	{
		if(Game.Instance.gameScene.SortingOrderNumber!=mySpriteRenderer.sortingOrder)
		{
			mySpriteRenderer.sortingOrder = Game.Instance.gameScene.SortingOrderNumber;
		}
	}

	private void CheckSkillTimeByCharNumber() //Check Skill Time By CharacterNumber
	{
		
		switch(CharacterNumber)
		{
		case 1:
			CheckTransparentSkillTime ();
			break;

		case 2:
			CheckDetactSkillTime (); //Check Eye Cat Skill Time
			break;

		case 3:
			CheckSternSkillTime ();
			break;

		default:

			break;
		}
	}

	private void CheckDetactSkillTime()  //Check Eye Cat Skill Time
	{
		if(detactSkillTime<0f && DetactSkillActive ) //Detact Skill time is less than Zero and Detac Skill is Active 
			//execute for skill inactive
		{
			GetComponent<PhotonView>().RPC("DeActiveDetactTranparentCatSkill", PhotonTargets.All, null);
			detactSkillTime = 2.5f; //set detact skill time
			DetactSkillActive = false; //detact skill active false
		}

		if(DetactSkillActive) //if skill is active reduce skill time
		{
			detactSkillTime -= Time.deltaTime;
		}

	}

	private void CheckTransparentSkillTime()
	{
		if(tranparentSkillTime<0f && TransparentSkillAcrive)
		{
			GetComponent<PhotonView>().RPC ("DeactiveTransparentSkill", PhotonTargets.All, null);
			tranparentSkillTime = 2.5f;
			TransparentSkillAcrive = false;
		}	

		if(TransparentSkillAcrive)
		{
			tranparentSkillTime -= Time.deltaTime;
		}
	}

	private void CheckSternSkillTime()
	{
		if(sternSkillTime<0f && SternSkillActive)
		{
			GetComponent<PhotonView>().RPC("DeActiveSternSkill", PhotonTargets.All, null);
			sternSkillTime = 2.5f;
			SternSkillActive = false;
		}

		if(SternSkillActive)
		{
			sternSkillTime-=Time.deltaTime;
		}
	}

	[PunRPC]
	public void MoveMe(Vector3 realPosition)
	{
		if (photonView.isMine) //내 뷰일 경우 안움 직이게 해서  
			return;
		//realPosition = (transform.position + realPosition);

		transform.position += realPosition; //내 뷰와 상대방 뷰에서는 다르게 움직이는 것 처럼 보이게 함
		//this.transform.position=Vector3.Lerp(this.transform.position,realPosition,Time.deltaTime*5);
	}

	public void ActiveSkill()
	{
		if(Input.GetKeyDown(KeyCode.J) && GetComponent<PhotonView>().isMine) //j key pressed and photon view is mie
			
		{
			//Debug.Log ("Skilllsdfjlasjfd");
			//if j key pressed activate my skill
			switch(Game.Instance.gameScene.SelectedCharacter)
			{
			case 1:
				//selected character is 1 and pressed j key code Call RPC ActiveTransparentSkill

				GetComponent<PhotonView> ().RPC ("ActiveTransparentSkill", PhotonTargets.All, null);
				break;

			case 2:
				//selected character is 2 and pressed j key code Call RPC ActiveDetactTranparentCatSkill
				GetComponent<PhotonView>().RPC("ActiveDetactTranparentCatSkill", PhotonTargets.All, null);
	
				break;

			case 3:
				GetComponent<PhotonView> ().RPC ("ActiveSternSkill", PhotonTargets.All, null);
				break;

			default:

				break;
			}
		}
	}

	public void ThrowObject()
	{
		if(Input.GetKeyDown(KeyCode.Space)&& GetComponent<PhotonView>().isMine) //space is pressed and photon view is mine
		{
			Game.Instance.gameScene.SpacePressed = true;

			switch(CharacterNumber)
			{
			case 1:
				//selected character is 1 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowReadyMotion", PhotonTargets.All, CharacterNumber);
			
				break;

			case 2:
				//selected character is 2 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowReadyMotion", PhotonTargets.All, CharacterNumber);

				break;

			case 3:
				//selected character is 3 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowReadyMotion", PhotonTargets.All, CharacterNumber);
				break;

			default:
				
				break;
			}
		}

		else if(Input.GetKeyUp(KeyCode.Space) && GetComponent<Character>().MotionState==CharacterMotionState.Ready && GetComponent<PhotonView>().isMine)
		{
			//space is pressed than up and monster mostion state is ready and photon view is mine
			Game.Instance.gameScene.SpacePressed=false;
			switch(CharacterNumber)
			{
			case 1:
				//selected character is 1 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowMotion", PhotonTargets.All, CharacterNumber);
				GetComponent<PhotonView> ().RPC ("ThrowTrash", PhotonTargets.All, Game.Instance.gameScene.GageAmount);
				Game.Instance.gameScene.SetGageValueZero ();
				break;

			case 2:
				//selected character is 2 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowMotion", PhotonTargets.All, CharacterNumber);
				GetComponent<PhotonView> ().RPC ("ThrowTrash", PhotonTargets.All, Game.Instance.gameScene.GageAmount);
				Game.Instance.gameScene.SetGageValueZero ();
				break;

			case 3:
				//selected character is 3 and pressed space key code Call RPC ThrowReadyMotion
				GetComponent<PhotonView> ().RPC ("ThrowMotion", PhotonTargets.All, CharacterNumber);
				GetComponent<PhotonView> ().RPC ("ThrowTrash", PhotonTargets.All, Game.Instance.gameScene.GageAmount);
				Game.Instance.gameScene.SetGageValueZero ();
				break;

			default:

				break;
			}
		}
	}
		

	public void DeActiveSkill()
	{
		
		if(Input.GetKeyDown(KeyCode.L)  && GetComponent<PhotonView>().isMine )
		{
			//if L key pressed deactive skill
			//if j key pressed activate my skill
			switch(Game.Instance.gameScene.SelectedCharacter)
			{
			case 1:
				fastKatColor = Color.white;
				//selected character is 1 and pressed space key code Call RPC DeactiveTransparentSkill
				GetComponent<PhotonView> ().RPC ("DeactiveTransparentSkill", PhotonTargets.All, null);//call Deactive skill rpc
				break;

			case 2:
				//selected character is 2 and pressed space key code Call RPC DeActiveDetactTranparentCatSkill
				GetComponent<PhotonView>().RPC("DeActiveDetactTranparentCatSkill", PhotonTargets.All, null);
				break;

			case 3:
				GetComponent<PhotonView>().RPC("DeActiveSternSkill", PhotonTargets.All, null);
				break;

			default:

				break;
			}
		}
		else if(Input.GetKeyDown(KeyCode.L))
		{

		}
	}

	[PunRPC]
	public void ActiveTransparentSkill()
	{
		
		//different activate by photon view and selected character number
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber)
		{
			fastKatColor = Color.red; 
			TransparentSkillAcrive = true;
			//if is mine character color change to color for active
			Renderer myRenderer = this.GetComponent<SpriteRenderer> ();
			ActiveCatSkill ();
			myRenderer.material.color = fastKatColor; //change color
		

		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber)
		{
			Debug.Log (transform.name);
			//if is not mine dissapear my character for ghost
			this.GetComponent<SpriteRenderer> ().enabled = false;
			ActiveCatSkill ();
			DissapearMySliders ();
	
		}
	}

	[PunRPC]
	public void ActiveSternSkill()
	{
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber)
		{
			SternSkillActive = true;
			ActiveCatSkill ();

		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber)
		{
			ActiveCatSkill ();

		}
	}

	[PunRPC]
	public void DeActiveSternSkill()
	{
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber)
		{
			DeActiveCatSkill ();

		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber)
		{
			DeActiveCatSkill ();

		}
	}

	[PunRPC]
	public void ThrowTrash(float power)
	{
		GameObject bullet=GameObject.Instantiate(Resources.Load(trashNameList[CharacterNumber-1]), transform.position,Quaternion.identity) as GameObject;
		bullet.GetComponent<Bullet> ().enabled = true;

		if (GetComponent<PhotonView>().isMine && CharacterNumber==Game.Instance.gameScene.SelectedCharacter) {
			


			bullet.transform.position = this.transform.position;
			Vector3 moveVector = GetFirePosByYaw();
			bullet.GetComponent<Rigidbody2D> ().velocity = moveVector * power;
			bullet.GetComponent<Bullet> ().Damage = 0.1f;
			bullet.GetComponent<Bullet> ().ThrowType = GetComponent<Character> ().Type;
			if(CharacterNumber==3 && this.GetComponent<Character>().SkillActive)
			{
				bullet.GetComponent<Bullet> ().Skill = CatSkill.Stern;
			}
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());

		}
		else if(!GetComponent<PhotonView>().isMine && CharacterNumber==Game.Instance.gameScene.OppSelectedCharacter)
		{
			bullet.transform.position = this.transform.position;
			Vector3 moveVector = GetFirePosByYaw();
			moveVector *= -1;
			bullet.GetComponent<Rigidbody2D> ().velocity = moveVector * power;
			bullet.GetComponent<Bullet> ().Damage = 0.1f;
			bullet.GetComponent<Bullet> ().ThrowType = GetComponent<Character> ().Type;
			if(CharacterNumber==3 && this.GetComponent<Character>().SkillActive)
			{
				bullet.GetComponent<Bullet> ().Skill = CatSkill.Stern;
			}
			Physics2D.IgnoreCollision (bullet.GetComponent<Collider2D> (), GetComponent<Collider2D> ());
		}
	}



	private Vector3 GetFirePosByYaw()
	{
		//Debug.Log (GetComponent<Transform> ().localRotation.y);

		if(GetComponent<Transform>().localRotation.y==1)
		{
			//Debug.Log ("right");
			return RightFireVector;
		}
		else if(GetComponent<Transform>().localRotation.y==0)
		{
			//Debug.Log ("left");
			return LeftFireVector;
		}

		return LeftFireVector;
	}

	[PunRPC]
	public void DeactiveTransparentSkill()
	{
		//different Deactivate by photon view and selected character number
		//if is mine character color change to color for active
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber && CharacterNumber==1)
		{
			fastKatColor = Color.white;
			Renderer myRenderer = this.GetComponent<SpriteRenderer> ();
			myRenderer.enabled = true;  //renderer on for appear
			myRenderer.material.color = fastKatColor;//change color

		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber && CharacterNumber==1 )
		{
			//if is not mine reappear my character
			this.GetComponent<SpriteRenderer> ().enabled = true;// renderer on for appear
			prePosWhenActiveTransSkill=transform.position;
			AppearMySliders ();
			HierarchyChangeForBarReAppearWhenDeactivateTranparentSkill (prePosWhenActiveTransSkill);
		}
	}

	[PunRPC]
	public void ActiveDetactTranparentCatSkill()
	{
		//Active eye cat skill by photonview and selected character
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber && characterNumber==2)
		{
			DetactSkillActive = true;
			Renderer myRenderer = this.GetComponent<SpriteRenderer> ();
			ActiveCatSkill ();
			myRenderer.material.color = fastKatColor; //change color
			this.transform.GetChild (0).GetComponent<CircleCollider2D> ().enabled = true;//colider active for detact transparent cat
		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber && characterNumber==2)
		{
			//if is not mine reappear my character
			ActiveCatSkill();
			this.transform.GetChild (0).GetComponent<CircleCollider2D> ().enabled = true;//colider active for detact transparent cat
		}
	}

	[PunRPC]
	public void DeActiveDetactTranparentCatSkill()
	{
		//Deactive eye cat skill by photonview and selected character
		if(this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.SelectedCharacter==CharacterNumber && characterNumber==2)
		{
			fastKatColor = Color.white;
			Renderer myRenderer = this.GetComponent<SpriteRenderer> ();
			myRenderer.material.color = fastKatColor;//Change color
			this.transform.GetChild (0).GetComponent<CircleCollider2D> ().enabled = false; //colider inactive for detact transparent cat
		}
		else if(!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter==CharacterNumber && characterNumber==2)
		{
			//if is not mine reappear my character
			this.transform.GetChild (0).GetComponent<CircleCollider2D> ().enabled = false; //colider inactive for detact transparent cat
		}
	}


	private void HierarchyChangeForBarReAppearWhenDeactivateTranparentSkill(Vector3 prevPos)
	{
		this.GetComponent<Transform> ().parent = null;
		this.GetComponent<Transform> ().localPosition = prevPos;
		this.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
	}

	private void ActiveCatSkill()
	{
		this.GetComponent<Character> ().ActiveSkill ();
	}

	private void DeActiveCatSkill()
	{
		this.GetComponent<Character> ().DeactiveSkill ();
	}

}
