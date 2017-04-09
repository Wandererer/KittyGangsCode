using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	[SerializeField]
	private Sprite IdleSprite; //idle sprite for my character

	[SerializeField]
	private Sprite ThrowReadySprite; //throw ready character ready for my character

	[SerializeField]
	private Sprite ThrowSprite; // throw sprite for my character

	[SerializeField]
	private Sprite SternSprite;

	private CharacterMotionState motionState; //what my motion is

	private UISlider hpSlider;

	[SerializeField]
	private bool isSkillActive = false;

	private float time = 0.2f; //change sprite by time
	private float sternTime=2.0f;
	private float SkillTime = 2.5f;

	private int hp = 1;

	[SerializeField]
	private float currentHp=1;

	[SerializeField]
	private SideType type;

	public SideType Type{
		get { return type;}
		set { type = value;}
	}

	private SpriteRenderer spriteRenderer; //my sprite renderer

	public CharacterMotionState MotionState{
		get { return motionState;}
	}

	public bool SkillActive{
		get { return isSkillActive;}
		set { isSkillActive = value;}
	}

	int characterNumber; //what my character number is

	// Use this for initialization
	void Start () {
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		motionState = CharacterMotionState.Idle; //current state is idle
		characterNumber = GetComponent<CharacterController> ().CharacterNumber; //get my character number
		hpSlider=this.GetComponentInChildren<UISlider>();
	}
	
	// Update is called once per frame
	void Update () {
		ChangeToIdle (); //if my sprite is throw
		//SetHpBar();
		CheckHp ();
		ChangeToIdleIfStern ();
	}

	[PunRPC]
	public void IdleMotion(int number)
	{
		//change sprite to idle by photonview
		if (Game.Instance.gameScene.SelectedCharacter == number  && this.GetComponent<PhotonView>().isMine) {
			motionState = CharacterMotionState.Idle;
			spriteRenderer.sprite = IdleSprite;
		}
		else if (Game.Instance.gameScene.OppSelectedCharacter  == number && !this.GetComponent<PhotonView>().isMine) {
			this.GetComponent<SpriteRenderer>().sprite =  IdleSprite;;
		}
	}

	[PunRPC]
	public void ThrowReadyMotion(int number)
	{
		//change sprite to throwready by photonview
		if (Game.Instance.gameScene.SelectedCharacter == number && this.GetComponent<PhotonView>().isMine) {
			motionState = CharacterMotionState.Ready;
			spriteRenderer.sprite = ThrowReadySprite;
		}
		else if (!this.GetComponent<PhotonView>().isMine && Game.Instance.gameScene.OppSelectedCharacter  == number) {
		//	Debug.Log (number + " safsadf");
			this.GetComponent<SpriteRenderer>().sprite = ThrowReadySprite;
		}
		/*motionState = CharacterMotionState.Ready;
		spriteRenderer.sprite = ThrowReadySprite;*/
	}

	[PunRPC]
	public void ThrowMotion(int number)
	{
		//change sprite to throwmotion by photonview
		if (Game.Instance.gameScene.SelectedCharacter == characterNumber && this.GetComponent<PhotonView>().isMine) {
			motionState = CharacterMotionState.Throw;
			spriteRenderer.sprite = ThrowSprite;
		}
		else if (Game.Instance.gameScene.OppSelectedCharacter == characterNumber && !this.GetComponent<PhotonView>().isMine) {
			this.GetComponent<SpriteRenderer>().sprite = ThrowSprite;
		}
		/*motionState = CharacterMotionState.Throw;
		spriteRenderer.sprite = ThrowSprite;*/
	}

	[PunRPC]
	public void SternMotion(int number)
	{
		if(Game.Instance.gameScene.SelectedCharacter==characterNumber && this.GetComponent<PhotonView>().isMine)
		{
			Debug.Log("mine");
			motionState = CharacterMotionState.Stern;
			spriteRenderer.sprite = SternSprite;
		}

		else if(Game.Instance.gameScene.OppSelectedCharacter == characterNumber && !this.GetComponent<PhotonView>().isMine)
		{
			Debug.Log ("testasdfsadf");
			this.motionState = CharacterMotionState.Stern;
			this.GetComponent<SpriteRenderer> ().sprite = SternSprite;
		}
	}

	private void ChangeToIdleIfStern()
	{
		if(spriteRenderer.sprite.name==SternSprite.name && motionState==CharacterMotionState.Stern)
		{
			sternTime -= Time.deltaTime;
			if(sternTime<0)
			{
				GetComponent<PhotonView> ().RPC ("IdleMotion", PhotonTargets.All, characterNumber);
				sternTime = 2.0f;
			}
		}
	}

	private void ChangeToIdle()
	{
		//change to idle by time
		if(this.GetComponent<SpriteRenderer>().sprite.name==ThrowSprite.name && motionState==CharacterMotionState.Throw)
		//if(motionState==CharacterMotionState.Throw)
		{
			time -= Time.deltaTime;
			if(time<0)
			{
				GetComponent<PhotonView> ().RPC ("IdleMotion", PhotonTargets.All, characterNumber);
				time = 0.2f;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{

		if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType==SideType.Opp && Type==SideType.Opp)
		{
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), other.transform.GetComponent<Collider2D> ());
		}
		else if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType==SideType.Opp)
		{
			/*if (other.transform.GetComponent<Rigidbody2D> ().velocity.x < 0.1f) {
				Destroy (other.gameObject); //if bullet is too slow destroy bullet
				return;
			}*/
			Debug.Log("hit by opp");
			if(other.transform.GetComponent<Bullet>().Skill==CatSkill.Stern)
			{
				//GetComponent<PhotonView> ().RPC ("SternMotion", PhotonTargets.All, this.characterNumber);
				this.motionState = CharacterMotionState.Stern;
				this.spriteRenderer.sprite = SternSprite;
			}
			float damage= other.transform.GetComponent<Bullet> ().Damage;
			Destroy (other.gameObject);
			//GetComponent<PhotonView>().RPC("SetHpBar",PhotonTargets.All, damage);
			SetHpBar(damage);
		}
		else if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType==SideType.Mine)
		{
			if(this.SternSprite.name.ToString().StartsWith("Opp"))
			{
				//Debug.Log ("hit by me");
				if(other.transform.GetComponent<Bullet>().Skill==CatSkill.Stern)
				{
					//GetComponent<PhotonView> ().RPC ("SternMotion", PhotonTargets.All, this.characterNumber);
					this.motionState = CharacterMotionState.Stern;
					GetComponent<SpriteRenderer>().sprite = SternSprite;
				}
				float damage= other.transform.GetComponent<Bullet> ().Damage;
				Destroy (other.gameObject);
				//GetComponent<PhotonView>().RPC("SetHpBar",PhotonTargets.All, damage);
				SetHpBar(damage);
			}
		}
		else if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType==SideType.Mine)
		{
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), other.transform.GetComponent<Collider2D> ());
		}
	}

	[PunRPC]
	private void SetHpBar(float damage)
	{
		currentHp -= damage;
		GetComponentInChildren<UISlider> ().value = currentHp;
	}

	private void CheckHp()
	{
		if(currentHp<=0)
		{
			PhotonNetwork.Destroy (this.gameObject);
		}
	}


	public void SetCharacterSprites(Sprite[] sprites)
	{
		IdleSprite = sprites [0];
		ThrowReadySprite = sprites [1];
		ThrowSprite = sprites [2];
		SternSprite = sprites [3];

		GetComponent<SpriteRenderer>().sprite = IdleSprite;
	}

	public void ActiveSkill()
	{
		SkillActive = true;
	}

	public void DeactiveSkill()
	{
		SkillActive = false;
	}
}

