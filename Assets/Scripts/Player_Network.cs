using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Network : NetworkBehaviour {
	
	//Player movements variables
	[SyncVar] private Vector2 syncPosition;

	public float movementLerpRate = 15;
	private Transform playerTransform;

	//player rotation variables
	[SyncVar] private Quaternion syncPlayerRotation;
	
	public float rotationLerpRate = 15;

	void Start () {
		playerTransform = GetComponent<Transform>();
		if(!isLocalPlayer){
			GetComponent<PlayerController>().enabled = false;
		}
	}

	private void FixedUpdate(){
		LerpPosition();
		Sendpostion();
		LerpRotation();
		SendRotation();
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

	private void LerpRotation(){
		if(!isLocalPlayer){
			playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * rotationLerpRate);
		}
	}
	
	[Command]
	private void CmdProvideRotationtoServer(Quaternion rotation){
		syncPlayerRotation = rotation;
	}

	[ClientCallback]
	private void SendRotation(){
		if(!isLocalPlayer){
			CmdProvideRotationtoServer(playerTransform.rotation);
		}
	}
}
