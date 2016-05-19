using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	Rigidbody2D playerBody;
	public float speedForce = 5f;
	public float increaseBoost = 2f;
	public float defaultBoost = 1f;
	
	// Use this for initialization
	void Start () {
		playerBody = this.GetComponent<Rigidbody2D>();
	}
	
	//FixedUpdate por usar vectores, fuerzas
	void FixedUpdate () {
		Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical")) * speedForce;

		bool isBoost = CrossPlatformInputManager.GetButton("Boost");
		Debug.Log(isBoost);
		playerBody.AddForce(moveVector * (isBoost ? increaseBoost : defaultBoost));
	}
}
