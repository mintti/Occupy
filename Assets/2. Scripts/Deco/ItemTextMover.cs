using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemTextMover : MonoBehaviour {

	public Text arrow;
	float nextTime;
	float timeRate;
	void Start(){
		timeRate = 0;
		nextTime = 3;
	}
	void Update(){
		if(timeRate > nextTime){
			Destroy(this.gameObject);
		}
		timeRate += Time.deltaTime;
		transform.position += new Vector3(0, 0.5f, 0);
	}

	public void TextSetting(string text){
		arrow.text = text;
	}
}
