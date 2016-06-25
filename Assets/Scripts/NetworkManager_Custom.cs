using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager_Custom : NetworkManager {

	public void StarupHost(){
		SetPort();
		NetworkManager.singleton.StartHost();
	}

	public void JoinGame(){
		SetIPAddress();
		SetPort();
		NetworkManager.singleton.StartClient();
	}

	void SetIPAddress(){
		string IpAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
		NetworkManager.singleton.networkAddress = IpAddress;
	}



	void SetPort(){
		NetworkManager.singleton.networkPort = 7777;
	}

	void OnLevelWasLoaded (int level){
		if(level==0){
			//SetUpMenuSceneButtons();
			StartCoroutine(SetUpMenuSceneButtons());
		}

		else{
			SetupOtherSceneButtons();
		}
	}

	IEnumerator SetUpMenuSceneButtons(){
		yield return new WaitForSeconds(0.3f);
		GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StarupHost);

		GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(JoinGame);
	}

	void SetupOtherSceneButtons(){
		GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
	}

}
