using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {
	Rigidbody2D playerBody;
	//[SerializeField] Camera charCam;
	public float speedForce = 75f;
	public float rotationvelocity = 5f;
	public float increaseBoost = 10f;
	public float defaultBoost = 1f;
	private float fireDelay = 0.5f;
	private float shootingTimer =0.0f;
	//states
	private bool isShooting=false;
	private float shipBounderyRadious = 0.5f;

	//past mood
	private float moodState;
	public GameObject bulletprefab;
	public Transform bulletspawn;
	public RectTransform healthBar;
	private Transform trPlayer;
	public RectTransform ShootingBar;
	public const float maxBurst = 75;
	private float currentBurst = maxBurst;
	private bool shootingBlocked = false;
	private float camViewSize;
	//[SyncVar (hook = "coolDown")]public bool cool = false;
	// Use this for initialization

	//limits
	const float WIDTH = 45;
	const float HEIGHT = 30;

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
		//if(isLocalPlayer){
			playerBody = this.GetComponent<Rigidbody2D>();
			camViewSize = Camera.main.orthographicSize;
		//	GameObject.Find("Main Camera").SetActive(false);

			moodState = 0;
		//	charCam.enabled = true;
		//}
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
		Debug.Log("My x: " + transform.position.x + " My y: " + transform.position.y);
		if ((moodState == 0 && shootingState > 0.5) || ( moodState == 1 && shootingState < 0.5))
			setShootingState(shootingState);

		Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical")) * speedForce;
		bool isBoost = CrossPlatformInputManager.GetButton("Boton");
		playerBody.AddForce(moveVector * (isBoost? increaseBoost: defaultBoost));

		if(isBoost)
			SoundEffectsHelper.Instance.MakeBoostSound();

		//Updating the player rotation
		//playerBody.MoveRotation(playerBody.rotation+CrossPlatformInputManager.GetAxis("VerticalRot")*rotationvelocity);
		float LO = CrossPlatformInputManager.GetAxis("VerticalRot");
		float LA = CrossPlatformInputManager.GetAxis("HorizontalRot");
		double B = Math.Atan(LO/LA) * 180 / Math.PI;
		if(LA < 	0){
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
				isShooting = false;
			}
		}

		//Restrict the ship to the camara's bounderies

		Vector3 pos = transform.position;
		if(pos.y + shipBounderyRadious > camViewSize) {
			pos.y = camViewSize- shipBounderyRadious;
			transform.position =pos;
		}
		if(pos.y-shipBounderyRadious < -camViewSize) {
			pos.y = -camViewSize + shipBounderyRadious;
			transform.position = pos;
		}

		float screenRatio  = (float)Screen.width / (float)Screen.height;
		float widthOrtho = camViewSize * screenRatio;
		if(pos.x+shipBounderyRadious >widthOrtho) {
			pos.x = widthOrtho- shipBounderyRadious;
			transform.position =pos;
		}
		if(pos.x-shipBounderyRadious < -widthOrtho) {
			pos.x = -widthOrtho +shipBounderyRadious;
			transform.position =pos;
		}
		Debug.Log("Cam size: " + camViewSize);
		Debug.Log("S Width: " + Screen.width);
		Debug.Log("S Height: " + Screen.height);

		//Code for limiting movement by boundaries
		/*Vector3 pos = transform.position;
		if(pos.y > HEIGHT) {
			pos.y = HEIGHT;
			//pos.y = camViewSize - shipBounderyRadious;
			transform.position = pos;
		}

		if(pos.y < -HEIGHT) {
			pos.y = -HEIGHT;
			//pos.y = -camViewSize + shipBounderyRadious;
			transform.position = pos;
		}*/

		//float screenRatio  = (float)Screen.width / (float)Screen.height;
		//float widthOrtho = camViewSize * screenRatio;
		if(pos.x > WIDTH) {
			pos.x = WIDTH;
			//pos.x = camViewSize - shipBounderyRadious;
			transform.position = pos;
		}

		if(pos.x  < -WIDTH) {
			pos.x = -WIDTH;
			//pos.x = -camViewSize + shipBounderyRadious;
			transform.position = pos;
		}

		//end limit movement section
		/*Quaternion rot = transform.rotation;
		float z = rot.eulerAngles.z;

		z-= CrossPlatformInputManager.GetAxis("VerticalRot") *rotationvelocity;
		//Debug.Log(CrossPlatformInputManager.GetAxis("VerticalRot")+" rota: "+ rotationvelocity);

		rot = Quaternion.Euler(0,0,z);
		transform.rotation = rot;
		Vector3 pos2 = transform.position;
		Vector3 velocity = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal") * speedForce * Time.deltaTime, 
			CrossPlatformInputManager.GetAxis("Vertical") * speedForce * Time.deltaTime,0) * (isBoost? increaseBoost: defaultBoost);
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
		Debug.Log("Blocked: " + shootingBlocked);
		/*if (Input.GetKeyUp(KeyCode.Escape)) { 
			if (Application.platform == RuntimePlatform.Android) { 
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic("currentActivity"); 
				activity.Call("moveTaskToBack", true); 
			} else { Application.Quit(); } 
			}*/

		//isShooting = CrossPlatformInputManager.GetButtonDown("Shot");
		//playerBody.MoveRotation(playerBody.rotation+rot*rotationForce);
	}

	[Command]
	public void CmdFire(){
		//create de prefab 
		GameObject bullet = (GameObject)Instantiate(bulletprefab, bulletspawn.position, bulletspawn.rotation);

		//add velocity
		bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 12.0f;

		//Spawn bullet on client
		//Note: important to add velocity to the bullet before spawn the bullet on the client
		NetworkServer.Spawn(bullet);
		SoundEffectsHelper.Instance.MakePlayerShotSound();
		//destroy bullet after 7 segs
		Destroy(bullet, 7);
	}
}
