using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	Rigidbody2D playerBody;
	public float speedForce = 5f;
	public float rotationvelocity = 5f;
	public float increaseBoost = 10f;
	public float defaultBoost = 1f;
	public GameObject bulletprefab;
	public Transform bulletspawn;

	//states
	private bool isShoting=false;
	private float shipBounderyRadious = 0.5f;
	// Use this for initialization
	void Start () {
		playerBody = this.GetComponent<Rigidbody2D>();
	}
	
	//FixedUpdate por usar vectores, fuerzas
	void FixedUpdate () {
		/*Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical")) * speedForce;
		//Debug.Log("Z:"+this.rot);
		//cont++;
		//Debug.Log(moveVector.x);
		
		//Debug.Log(isBoost);
		playerBody.AddForce(moveVector * (isBoost? increaseBoost: defaultBoost));
		playerBody.MoveRotation(playerBody.rotation+CrossPlatformInputManager.GetAxis("VerticalRot")*rotationvelocity);*/
		bool isBoost = CrossPlatformInputManager.GetButton("Boton");
		if(Input.GetKeyDown(KeyCode.Space) || isShoting){
			CmdFire();
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

		Quaternion rot = transform.rotation;
		float z = rot.eulerAngles.z;

		z-= CrossPlatformInputManager.GetAxis("VerticalRot") *rotationvelocity;
		//Debug.Log(CrossPlatformInputManager.GetAxis("VerticalRot")+" rota: "+ rotationvelocity);

		rot = Quaternion.Euler(0,0,z);

		transform.rotation = rot;

		Vector3 pos2 = transform.position;

		Vector3 velocity = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal") * speedForce * Time.deltaTime, CrossPlatformInputManager.GetAxis("Vertical") * speedForce * Time.deltaTime,0) * (isBoost? increaseBoost: defaultBoost);

		pos2 += rot * velocity;

		transform.position = pos2;

	}
	void Update() {
		if(!isLocalPlayer)
			return;
		 if (Input.GetKeyDown(KeyCode.Escape)) 
    		Application.Quit(); 
		
		/*if (Input.GetKeyUp(KeyCode.Escape)) { 
			if (Application.platform == RuntimePlatform.Android) { 
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic("currentActivity"); 
				activity.Call("moveTaskToBack", true); 
			} else { Application.Quit(); } 
			}*/

		isShoting = CrossPlatformInputManager.GetButtonDown("Shot");
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
