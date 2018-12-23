using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnEnable()
	{
		StartCoroutine(ItemMover());
	}

	IEnumerator ItemMover(){
		Debug.Log("실행");
		transform.Rotate(new Vector3(0, 0, 10));
		while(transform.rotation.z % 180 != 0){
			transform.Rotate(new Vector3(0, 0, 10));
			yield return new WaitForSeconds(0.05f);
		}
		
	}
}
