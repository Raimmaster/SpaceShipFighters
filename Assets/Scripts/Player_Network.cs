using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Network : NetworkBehaviour {
	
	//Player movements variables
	[SyncVar] private Vector2 syncPosition;

	private float movementLerpRate = 15;
	private Transform playerTransform;
	private Vector2 lastposition;
	private float threshold = 0.5f;
	//player rotation variables
	[SyncVar] private Quaternion syncPlayerRotation;
	private Quaternion lastPlayerRot;
	private float rotThresHold = 5f;
	private float rotationLerpRate = 15;

	void Start () {
		playerTransform = GetComponent<Transform>();
		if(!isLocalPlayer){
			GetComponent<PlayerController>().enabled = false;
		}
	}

	void Update() {
		LerpPosition();
		Sendpostion();
		LerpRotation();
		SendRotation();
	}

	private void FixedUpdate(){
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
		if(isLocalPlayer && Vector2.Distance(playerTransform.position, lastposition)>threshold)
			CmdProvidePosToServer(playerTransform.position);
			lastposition = playerTransform.position;
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
		if(isLocalPlayer){
			if(Quaternion.Angle(playerTransform.rotation, lastPlayerRot) > rotThresHold){
				CmdProvideRotationtoServer(playerTransform.rotation);
				lastPlayerRot = playerTransform.rotation;
			}	
		}
	}
}
