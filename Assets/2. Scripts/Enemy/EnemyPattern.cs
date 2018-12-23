using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class EnemyPattern : MonoBehaviour {
	public UIController UI;
	public EnemyAI Enemy;
	float pr;
	public CircleBoltTime pattern1;
	public CircleBolt pattern2;
	public BossShot pattern3;

	public ParticleSystem Die;


	// Use this for initialization
	void Start () {
		Enemy = GetComponent<EnemyAI>();
		
		pr = UI.pr;
		
		pattern1 = this.GetComponent<CircleBoltTime>();
		pattern2 = this.GetComponent<CircleBolt>();
		pattern3 = this.GetComponent<BossShot>();

		pattern1.enabled= false;
		pattern2.enabled= false;
		pattern3.enabled= false;
		
	}

	
	// Update is called once per frame
	void Update () {
		pr = UI.pr;

		switch(Enemy.GetEnemyDie()){
			case 0 :{
				if(pr<20){
					pattern1.enabled = true;
				}
				else if(pr <80){
					pattern1.enabled = false;
					pattern2.enabled = true;
					}
				else if(pr < 80){
					pattern2.enabled = false;
					pattern3.enabled = true;
				}
				else{
					Die.Play();
				}
				
				break;
			}
			case 1 : {
				pattern1.enabled= false;
				pattern2.enabled= false;
				pattern3.enabled= false;
				break;
			}

		}
	}
}
