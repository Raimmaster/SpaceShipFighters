using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class Bullet : NetworkBehaviour {

	void Start () {
		SoundEffectsHelper.Instance.MakePlayerShotSound();
	}
	//overriding method
	[ClientCallback]
	void OnCollisionEnter2D(Collision2D collision){
		GameObject hit = collision.gameObject;
		Debug.Log("Collision");
		Health health = hit.GetComponent<Health>();
		if(health != null){
			health.takeDamage(1);
			SpecialEffects.Instance.Damage(transform.position);
			SoundEffectsHelper.Instance.MakeShotReceivedSound();
			if((health.currentHealth-1)<=0){
				SpecialEffects.Instance.Explosion(transform.position);
			}
			Debug.Log("ATACANDO");
		}
		Debug.Log("Salió");
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(){
		Debug.Log("Trigger");
	}
}
