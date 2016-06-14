using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {
	Rigidbody2D playerBody;
	public float speedForce = 5f;
	public float rotationvelocity = 5f;
	public float increaseBoost = 10f;
	public float defaultBoost = 1f;
	private float fireDelay = 0.5f;
	private float shootingTimer =0.0f;
	//states
	private bool isShoting=false;
	private float shipBounderyRadious = 0.5f;

	//past mood
	private float moodState;
	public GameObject bulletprefab;
	public Transform bulletspawn;
	public RectTransform healthBar;
	public RectTransform ShootingBar;
	public const float maxBurst = 75;
	private float currentBurst = maxBurst;
	private bool shootingBlocked = false;
	//[SyncVar (hook = "coolDown")]public bool cool = false;
	// Use this for initialization

	void coolDown(bool cool){
		if(cool){
			currentBurst -=15*Time.deltaTime;
			if(currentBurst<0){
				currentBurst=0;
				shootingBlocked = true;
			}
			ShootingBar.sizeDelta = new Vector2(currentBurst, ShootingBar.sizeDelta.y);
		}else{
			if(currentBurst<maxBurst){
				currentBurst +=10*Time.deltaTime;
			}
			if(currentBurst > maxBurst*0.25 )
				shootingBlocked = false;
			ShootingBar.sizeDelta = new Vector2(currentBurst, ShootingBar.sizeDelta.y);
		}
	}
	void Start () {
		playerBody = this.GetComponent<Rigidbody2D>();
		moodState = 0;
	}

	public void setShootingState(float amount){
		Debug.Log("amount: "+amount);
		if(amount>0.5){
			moodState = 1;
		}
		else{
			moodState=0;
		}
		Debug.Log("getState_mood: "+moodState);
	}

	//FixedUpdate por usar vectores, fuerzas
	void FixedUpdate () {
		//Updating the player move
		float shootingState = CrossPlatformInputManager.GetMood();
		
		if ((moodState == 0 && shootingState > 0.5) || ( moodState == 1 && shootingState < 0.5))
			setShootingState(shootingState);
		
		Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical")) * speedForce;
		bool isBoost = CrossPlatformInputManager.GetButton("Boton");
		playerBody.AddForce(moveVector * (isBoost? increaseBoost: defaultBoost));
		//Updating the player rotation
		//playerBody.MoveRotation(playerBody.rotation+CrossPlatformInputManager.GetAxis("VerticalRot")*rotationvelocity);
		float LO = CrossPlatformInputManager.GetAxis("VerticalRot");
		float LA = CrossPlatformInputManager.GetAxis("HorizontalRot");
		double B = Math.Atan(LO/LA) * 180 / Math.PI;
		if(LA <0){
			B +=180;
		}
		B += (-90);
		if(!B.Equals(Double.NaN))
			transform.rotation = Quaternion.Euler(0,0,(float)B);
		//Updating the Health bar rotation according to the player rotation
		Quaternion rot = healthBar.rotation;
		float z = playerBody.rotation+CrossPlatformInputManager.GetAxis("VerticalRot")*rotationvelocity;
		rot = Quaternion.Euler(0,0,z);
		healthBar.rotation = rot;
		
		//Fire Validation 
		//before Input.GetKeyDown(KeyCode.Space)
		
		if(moodState == 1 && (LA!=0 || LO!=0)){
			coolDown(true);
			//cool = true;
		}
		if(currentBurst<75){
			coolDown(false);
			//cool = false;
		}

		if(shootingTimer<0 && !shootingBlocked){
			if(LA!=0 || LO!=0){
				shootingTimer = fireDelay;
				CmdFire();
				isShoting = false;
			}
		}	
		
		//Restrict the ship to the camara's bounderies
		Vector3 pos = transform.position;
		if(pos.y+shipBounderyRadious >Camera.main.orthographicSize) {
			pos.y = Camera.main.orthographicSize- shipBounderyRadious;
			transform.position =pos;
		}
		if(pos.y-shipBounderyRadious < -Camera.main.orthographicSize) {
			pos.y = -Camera.main.orthographicSize +shipBounderyRadious;
			transform.position =pos;
		}

		float screenRatio  = (float)Screen.width / (float)Screen.height;
		float widthOrtho = Camera.main.orthographicSize * screenRatio;
		if(pos.x+shipBounderyRadious >widthOrtho) {
			pos.x = widthOrtho- shipBounderyRadious;
			transform.position =pos;
		}
		if(pos.x-shipBounderyRadious < -widthOrtho) {
			pos.x = -widthOrtho +shipBounderyRadious;
			transform.position =pos;
		}

		/*Quaternion rot = transform.rotation;
		float z = rot.eulerAngles.z;

		z-= CrossPlatformInputManager.GetAxis("VerticalRot") *rotationvelocity;
		//Debug.Log(CrossPlatformInputManager.GetAxis("VerticalRot")+" rota: "+ rotationvelocity);

		rot = Quaternion.Euler(0,0,z);

		transform.rotation = rot;

		Vector3 pos2 = transform.position;

		Vector3 velocity = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal") * speedForce * Time.deltaTime, CrossPlatformInputManager.GetAxis("Vertical") * speedForce * Time.deltaTime,0) * (isBoost? increaseBoost: defaultBoost);

		pos2 += rot * velocity;

		transform.position = pos2;*/


	}
	void Update() {
		if(!isLocalPlayer)
			return;
		 if (Input.GetKeyDown(KeyCode.Escape)) 
    		Application.Quit(); 
		
		//Variable to control shooting time
		shootingTimer -= Time.deltaTime;
		//Debug.Log("U_mood: "+Time.deltaTime);
		if(moodState==0){
			fireDelay = 0.5f;
			shootingBlocked = false;
		}else{
			fireDelay = 0.1f;
		}
		Debug.Log("Blocked: "+shootingBlocked);
		/*if (Input.GetKeyUp(KeyCode.Escape)) { 
			if (Application.platform == RuntimePlatform.Android) { 
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic("currentActivity"); 
				activity.Call("moveTaskToBack", true); 
			} else { Application.Quit(); } 
			}*/

		//isShoting = CrossPlatformInputManager.GetButtonDown("Shot");
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
