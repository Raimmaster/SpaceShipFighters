using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Network : NetworkBehaviour {
	public float movementLerpRate = 15;
	[SyncVar] private Vector2 syncPosition;

	private Transform playerTransform;
	void Start () {
		playerTransform = GetComponent<Transform>();
		if(!isLocalPlayer){
			GetComponent<PlayerController>().enabled = false;
		}
	}

	private void FixedUpdate(){
		LerpPosition();
		Sendpostion();
	}

	private void LerpPosition()
	{	
		if(!isLocalPlayer){
			playerTransform.position = Vector2.Lerp(playerTransform.position, syncPosition, Time.deltaTime * movementLerpRate);
		}

	}

	[Command]
	private void CmdProvidePosToServer(Vector2 position){
		syncPosition = position;
	}

	[ClientCallback]
	private void Sendpostion(){
		if(isLocalPlayer)
			CmdProvidePosToServer(playerTransform.position);
	}
	
}
