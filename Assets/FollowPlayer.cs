using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
	}

	// Update is called once per frame
	void LateUpdate () {
		if(player != null)
		{
			this.transform.position = new Vector3(player.transform.position.x, 
				player.transform.position.y, -550);
			//Debug.Log("Hey, hey, listen!");
		}
		else
			player = GameObject.FindWithTag("Player");
	}
}
