using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoodState : MonoBehaviour {
	private float cant;
	// Use this for initialization
	Scrollbar bar;
	void Start () {
		bar = gameObject.GetComponent<Scrollbar>();
	}
	public void setChange(float amount){
		cant = amount;
		//Debug.Log(amount+" "+cant);
	}
	// Update is called once per frame
	void Update () {
		if(cant>0.5)
			bar.value = 1;
		else
			bar.value =0;
	}
}
