using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherStageArea : MonoBehaviour {
	public GameController Game;
	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			Game = gameControllerObject.GetComponent<GameController> ();
		}
		if (Game == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	//적 이동 스팟들이 범위 외 일경우 상태 false해줌. WayPointGroup에 존재함.
	public void ActiveOffWayPoint(){
		for(int i = 0; i< transform.childCount ; i++){
			Vector3 WayPointPos = transform.GetChild(i).transform.position;
			if(WayPointPos.x < -Game.w/4 || WayPointPos.x > Game.w/4 ||
				WayPointPos.z < -Game.h/4 || WayPointPos.z > Game.h/4 ){
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}
}
