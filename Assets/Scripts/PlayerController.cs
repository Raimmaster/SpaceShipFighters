using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	Rigidbody2D playerBody;
	public float speedForce = 5f;
	public float rotationForce = 5f;
	public float increaseBoost = 10f;
	public float defaultBoost = 1f;
	public GameObject bulletprefab;
	public Transform bulletspawn;

	//states
	private bool isShoting=false;
	private float rot=0;
	// Use this for initialization
	void Start () {
		playerBody = this.GetComponent<Rigidbody2D>();
	}
	
	//FixedUpdate por usar vectores, fuerzas
	void FixedUpdate () {
		Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical")) * speedForce;
		
		//Debug.Log(moveVector.x);
		bool isBoost = CrossPlatformInputManager.GetButton("Boton");
		//Debug.Log(isBoost);
		playerBody.AddForce(moveVector * (isBoost? increaseBoost: defaultBoost));
		playerBody.MoveRotation(transform.rotation+5*rotationForce);
		Debug.Log("Z 3:"+this.rot);
		if(Input.GetKeyDown(KeyCode.Space) || isShoting){
			CmdFire();
		}

	}

	public void setRotation(float rot){
		this.rot = rot;
		Debug.Log("Z 2:"+this.rot);
		//playerBody.MoveRotation(playerBody.rotation+5*rotationForce);
	}
	void Update() {
		if(!isLocalPlayer)
			return;
		
		isShoting = CrossPlatformInputManager.GetButtonDown("Shot");
		Debug.Log("Z "+rot);
		//playerBody.MoveRotation(playerBody.rotation+rot*rotationForce);
	}

	[Command]
	public void CmdFire(){
		//create de prefab 
		GameObject bullet = (GameObject)Instantiate(bulletprefab, bulletspawn.position, bulletspawn.rotation);

		//add velocity
		bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up*6.0f;

		//Spawn bullet on client
		//Note: important to add velocity to the bullet before spawn the bullet on the client
		NetworkServer.Spawn(bullet);

		//destroy bullet after 2 segs
		Destroy(bullet,2);
	}
}
