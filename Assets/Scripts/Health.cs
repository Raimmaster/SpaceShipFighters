﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	[SyncVar (hook = "OnChangeHealth")]public int currentHealth = maxHealth;
	public RectTransform healthBar;

	public void takeDamage(int amount){
		if(!isServer){
			return;
		}
		currentHealth -= amount;
		if(currentHealth <=0){
			currentHealth =0;
			Debug.Log("Dead");
			RpcExplosion();
			//SpecialEffects.Instance.Explosion(transform.position);
			Destroy(gameObject);
		}
		Debug.Log(currentHealth);

	}

	[ClientRpc]
    void RpcExplosion()
    {
        SpecialEffects.Instance.Explosion(transform.position);
    }
	void OnChangeHealth(int health){		
		healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
	}
}
