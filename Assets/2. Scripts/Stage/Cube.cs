using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    public GameController Game;
    public PlayerCtrl Player;

    public int p, q;

    public ParticleSystem occuCubeEff;

    public Material[] cubeMaterial = new Material[3]; //큐브 Prefab 0.기본 1.addCube 2.  Player땅 (noneCube는 렌더끔)

    public void SetCoor(int p, int q)
    {
        this.p = p;
        this.q = q;
    }

    public void UpdateType(char type)
    {
        switch (type)
        {
            case 'n':
                {
                    gameObject.tag = "NoneCube";
                    GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<BoxCollider>().size = new Vector3(1.1f, 1.1f, 5);
                    occuCubeEff.Play();
                    Destroy(occuCubeEff, 2);
                    break; 
                }
            case 'c':
                {
                    gameObject.tag = "Cube";
                    gameObject.GetComponent<MeshRenderer>().material = cubeMaterial[0];
                    GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
                    break;
                }
            case 'a':
                {
                    gameObject.tag = "AddCube";
                    gameObject.GetComponent<MeshRenderer>().material = cubeMaterial[1];
                     GetComponent<BoxCollider>().size = new Vector3(1.1f, 1.1f, 5);
                    break;
                }
            case 's':
                {
                    gameObject.tag = "StandCube";
                    gameObject.GetComponent<MeshRenderer>().material = cubeMaterial[1];
                     GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
                    break;
                }
            case 'p':
                {
                    gameObject.tag = "PlayerCube";
                    gameObject.GetComponent<MeshRenderer>().material = cubeMaterial[2];
                     GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
                    Game.PlayerCubeCount++;
                    break;
                    
                }
                
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(this.tag == "AddCube" || this.tag == "StandCube"){
            if(other.tag =="Enemy"){
                if(Player.ValidAttack)
                    Player.HitBullet();
            }
        }

        if(this.tag == "NoneCube"){
            if(other.tag =="Chance"){
                Destroy(other.gameObject);
                Player.PlayerCol.takeItem( 0);
            }
            else if(other.tag =="Time"){
                Destroy(other.gameObject);
                Player.PlayerCol.takeItem(1);
            }
            else if(other.tag =="Speed"){
                Destroy(other.gameObject);
                Player.PlayerCol.takeItem(2);
            }
        }
	}

}
