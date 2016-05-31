using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//overriding method
	void OnCollisionEnte2D(){
		Debug.Log("Collision");
		Destroy(gameObject);
	}

	void OnTriggerEnter2d(){
		Debug.Log("Trigger");
	}
}
