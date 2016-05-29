using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//overriding method
	void OnCollisionEnter(){
		Destroy(gameObject);
	}
}
