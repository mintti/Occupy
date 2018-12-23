using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour {
public PlayerCtrl Player;
public ParticleSystem Bullet;
public ParticleSystem ProtectBullet;

//아이템 이펙트
public ParticleSystem item_chance;
public ParticleSystem item_time;
public ParticleSystem item_speed;


//오디오 관리(쉴드)
public AudioSource ColAudio;
public AudioClip shild;
public AudioClip item;
	// Use this for initialization
	void Start () {
		ColAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = Player.transform.position;
	}


	//캐릭터 Col 관리
	void OnTriggerEnter(Collider other)
    {
		switch(other.tag){
			case "Bullet" :{
				if(Player.ValidAttack){
					Destroy(other.gameObject);
					Player.HitBullet();
					Bullet.Play();
					}
				else{
					ColAudio.PlayOneShot(shild);
					ProtectBullet.Play();
				}
				break;
			}
			case "Enemy" : {
				if(Player.ValidAttack){	
					Player.HitBullet();
				}
				else{
					ColAudio.PlayOneShot(shild);
					ProtectBullet.Play();
				}
				break;
			}
			case "Chance" : {
				Destroy(other.gameObject);
				takeItem(0);
				break;
			}
			case "Time" : {
				Destroy(other.gameObject);
				takeItem(1);
				break;
			}
			case "Speed" : {
				Destroy(other.gameObject);
				takeItem(2);
				break;
			}
		}
	}

	//아이템 효과 지정함수
	public void takeItem(int num){
		
		ColAudio.PlayOneShot(item);
		if(num == 0) {
			item_chance.Play();
			Player.Game.ChanceUp();
		}
		else if( num == 1){
			item_time.Play();
			Player.Game.time +=30;
		}
		else if(num ==2){
			item_speed.Play();
			Player.moveSpeed +=0.02f;
		}
	}
}
