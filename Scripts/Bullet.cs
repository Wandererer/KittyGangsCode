using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	private float damage;

	private Rigidbody2D myRigidbody;

	private Vector2 test = new Vector2 (0, 0);

	public float Damage{
		get { return damage;}
		set { damage = value;}
	}
		
	private CatSkill catSkill=CatSkill.None;

	public CatSkill Skill{
		get { return catSkill;}
		set { catSkill = value;}
	}

	[SerializeField]
	private SideType throwType;

	public SideType ThrowType{
		get { return throwType; }
		set { throwType = value;}
	}

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		if(catSkill==CatSkill.Stern)
		{
			transform.localScale = new Vector3 (1.7f, 1.7f);
		}
	}
		


	// Update is called once per frame
	void FixedUpdate () {
		myRigidbody.velocity = myRigidbody.velocity * 0.99f;
		//myRigidbody.angularVelocity = myRigidbody.angularVelocity * 0.99f;
		if (myRigidbody.velocity.x == 0)
			Destroy (gameObject);
	}

	private void Destroy()
	{
		Vector3 tempPos = (transform.position);
		tempPos=Camera.main.WorldToViewportPoint (tempPos);  //경계선 영역을 넘겼는지 좌표를 계산


		if ((tempPos.x < 0 || tempPos.x > 1) || (tempPos.y < 0 || tempPos.y > 1)) //화면 경계선은 넘지 않도록 만듬
			Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{

		if (other.transform.tag=="Player" && other.transform.GetComponent<Character> ().Type != throwType)
			Destroy (gameObject);

		else if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType!=ThrowType)
		{
			Destroy (other.gameObject);
			Destroy (gameObject);
		}
		else if(other.transform.tag=="Bullet" && other.transform.GetComponent<Bullet>().ThrowType==ThrowType)
		{
			Physics2D.IgnoreCollision (GetComponent<Collider2D> (), other.transform.GetComponent<Collider2D> ());
		}


	}
}
