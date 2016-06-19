using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class FollowPlayer : MonoBehaviour {
	private GameObject player;
	Quaternion rotation;
	// Use this for initialization
	void Awake(){
		rotation = transform.rotation;
	}

	void Start () {
		player = GameObject.FindWithTag("Player");
	}

	// Update is called once per frame
	void Update () {
//		if(!isLocalPlayer)
//			return;

		if(player != null)
		{
			this.transform.position = new Vector3(player.transform.position.x,
				player.transform.position.y, -550);
			//Debug.Log("Hey, hey, listen!");
			transform.rotation = rotation;
		}
		else
			player = GameObject.FindWithTag("Player");
	}
}
