using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//overriding method
	void OnCollisionEnter2D(Collision2D collision){
		GameObject hit = collision.gameObject;
		Debug.Log("Collision");
		Health health = hit.GetComponent<Health>();
		if(health != null){
			health.takeDamage(1);
			SpecialEffects.Instance.Damage(transform.position);
			SoundEffectsHelper.Instance.MakeShotReceivedSound();
			Debug.Log("ATACANDO");
		}
		Debug.Log("Salió");
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(){
		Debug.Log("Trigger");
	}
}
