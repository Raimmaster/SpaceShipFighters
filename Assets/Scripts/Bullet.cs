using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//overriding method
	void OnCollisionEnter2D(Collision2D collision){
		GameObject hit = collision.gameObject;
		Debug.Log("Collision");
		Health health = hit.GetComponent<Health>();
		if(health != null){
			health.takeDamage(10);
			Debug.Log("ATACANDO");
		}
		Debug.Log("Salio");
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(){
		Debug.Log("Trigger");
	}
}
