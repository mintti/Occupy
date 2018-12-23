using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCtrl : MonoBehaviour
{
    public GameController Game;
    public PlayerCol PlayerCol;
    //접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    [SerializeField] private Transform tr;

    //Player 관련
    private Animator anim;
    public ParticleSystem PlayerOn;
    public bool ValidAttack; //true이면 추적당하고, false이면 추적당하지않음.
    public float moveSpeed;

    //Player위치를 기록 (죽을 경우 해당위치로 재포지셔닝 된다.)
    public Vector3 playerPos;
    List<GameObject> Occu ;
    //Collider 관련
    public int minX, minY, maxX, maxY; // x = q,  y = p
    //사운드
    public SoundController Sound;
    public AudioSource Audio;
    public AudioClip attacked;
    public AudioClip occupie;
    public void Start()
    {
        //스크립트가 실행된 후 처음 수행되는 Start 함수에서 Transform 컴포넌트를 할당
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();

        minX = minY = 1000;
        maxX = maxY = -1;

        Occu = Game.Occu;
        PlayerInitial();
    }
    public void PlayerInitial(){
        moveSpeed = 0.05f;
    } 
    void Update()
    {
        //이동
        if (Input.GetKey(KeyCode.UpArrow))
        {
            tr.position += Vector3.forward * moveSpeed;
            tr.eulerAngles = new Vector3(0, 0, 0);
            anim.SetBool("IsMove", true);
            Sound.PlayAudio_Walk();

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            tr.position += Vector3.right * moveSpeed;
            tr.eulerAngles = new Vector3(0, 90, 0);
            anim.SetBool("IsMove", true);
            Sound.PlayAudio_Walk();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            tr.position += Vector3.back * moveSpeed;
            tr.eulerAngles = new Vector3(0, 180, 0);
            anim.SetBool("IsMove", true);
            Sound.PlayAudio_Walk();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            tr.eulerAngles = new Vector3(0, 270, 0);
            tr.position += Vector3.left * moveSpeed;
            Sound.PlayAudio_Walk();
            anim.SetBool("IsMove", true);
        }
        else
        {
            anim.SetBool("IsMove", false);
        }
    }

    //PlayerChance--일 경우 재포지셔닝
    public void SetPlayerPostion()
    {
        for(int i = 0 ;i< Occu.Count;i++)
            Occu[i].GetComponent<Cube>().UpdateType('c');
        Occu.Clear();
        GetComponent<Transform>().position = playerPos;
        PlayerOn.Play();//Eff
         //피격 사운드
         Audio.volume = 1;
        Audio.PlayOneShot(attacked);

        //Speed초기화
        PlayerInitial();
    }

    //PlayerCube적합도 탐색함수
    private int PlayerCubeTest(int i, int j, List<GameObject> L){

        if(i<minY + (minY == 1 ? -1 : 0) || i >maxY ||j <minX + (minX == 1 ? -1 : 0)|| j>maxX)
            return 0;

        if(Game.cube[i,j].tag == "PlayerCube" || L.Contains(Game.cube[i,j]))
            return 1;

        L.Add(Game.cube[i,j]);

        return PlayerCubeTest(i-1, j, L) * PlayerCubeTest(i, j-1, L) * PlayerCubeTest(i+1, j, L) * PlayerCubeTest(i, j+1, L);
    }

    //큐브와의 충돌들 관리.
    public void OnCollisionEnter(Collision col)
    {
        
        switch (col.collider.tag)
        {
            case "Cube":
                {
                    if (!Occu.Contains(col.gameObject))
                    {
                        Occu.Add(col.gameObject);
                        col.gameObject.GetComponent<Cube>().UpdateType('s');
                    }
                    ValidAttack = true;
                    break;
                }
            //이전 큐브가 아닌경우 지나가지 못하도록함. 이전 큐브이면 기본 Cube상태로 변경.
            case "AddCube":
                {
                    if (Occu.Count >= 2 && Occu[Occu.Count - 2] == col.gameObject && Occu.Contains(col.gameObject))
                    {
                        
                        Occu[Occu.Count - 1].GetComponent<Cube>().UpdateType('c');
                        Occu.Remove(Occu[Occu.Count - 1]);

                        Occu[Occu.Count - 1].GetComponent<Cube>().UpdateType('s');
                        
                    }
                    ValidAttack = true;
                    break;
                }
            //땅먹기 구현.
            case "PlayerCube":
                {
                    playerPos = col.transform.position;

                    if (Occu.Count > 0)
                    {
                        Occu.Add(col.gameObject);

                        if (Occu.Count < 3)
                            Occu[0].GetComponent<Cube>().UpdateType('c');
                        else
                        {
                            //Add 큐브 -> PlayerCube변경
                            for (int i = 0; i < Occu.Count; i++)
                            {
                                if (Occu[i].tag == "PlayerCube")
                                    continue;

                                Cube c = Occu[i].GetComponent<Cube>();
                                c.UpdateType('p');

                                //검사 범위 변경.
                                if (c.p <= minY)
                                    minY = c.p;
                                else if (c.p >= maxY)
                                    maxY = c.p;

                                if (c.q <= minX)
                                    minX = c.q;
                                else if (c.q >= maxX)
                                    maxX = c.q;
                            }

                            if (minY == 0) minY = 1;
                            if (minX == 0) minX = 1;

                            //위에서 변경된 범위 내의 큐브 PlayerCube적합도 검사
                            List<GameObject> List = new List<GameObject>();
                            List<GameObject> CubeList= new List<GameObject>();;
                            List<GameObject> PlayerCubeList = new List<GameObject>();;
                            int result;

                            for (int i = minY; i <= maxY; i++)
                            {
                                for (int j = minX; j <= maxX; j++)
                                {
                                    if(Game.cube[i,j].tag != "Cube" ||
                                       CubeList.Contains(Game.cube[i,j]) || 
                                       PlayerCubeList.Contains(Game.cube[i,j]))
                                        continue;

                                    result = PlayerCubeTest(i, j, List);
                                    
                                    
                                    if(result == 0)
                                        CubeList.AddRange(List);
                                    else if(result == 1)
                                        PlayerCubeList.AddRange(List);
                                    
                                    List.Clear();
                                }
                            }

                            //적합한 큐브들 Player큐브로 변경
                            for(int i = 0; i< PlayerCubeList.Count;i++)
                                PlayerCubeList[i].GetComponent<Cube>().UpdateType('p');
                            //리스트 리셋
                            PlayerCubeList.Clear();
                            CubeList.Clear();
                            
                            bool oc = true;
                            //NoneCube의 자격
                            for (int i = minY; i < maxY; i++)
                            {
                                for (int j = minX; j < maxX ; j++)
                                {
                                    if (Game.cube[i, j].tag != "PlayerCube")
                                        continue;
                                    bool test = true;
                                    for (int p = -1; p <= 1 && test; p++)
                                    {
                                        for (int q = -1; q <= 1 && test; q++)
                                        {

                                            try
                                            {
                                                if (Game.cube[i + p, j + q].tag != "PlayerCube") ;
                                            }
                                            catch (Exception e)
                                            {
                                                continue;
                                            }

                                            if (Game.cube[i + p, j + q].gameObject == null || (Game.cube[i + p, j + q].tag != "PlayerCube" && Game.cube[i + p, j + q].tag != "NoneCube"))
                                            {
                                                test = false;
                                            }
                                        }
                                    }
                                    //None큐브의 자격이 있으므로 변경.
                                    if (test){
                                        if(oc){
                                        Audio.volume = 0.6f;
                                        Audio.PlayOneShot(occupie);
                                        oc = false;
                                        }
                                        Game.cube[i, j].GetComponent<Cube>().UpdateType('n');
                                    }
                                }

                            }
                        }
                        Occu.Clear(); //List 초기화
                    }
                    //적에게 추적당하지 않는상태로 변경.
                    ValidAttack = false;

                    break;
                }
            //스테이지에서 떨어졌을 경우
            case "Ground":
                {
                    Game.ChanceDown();
                    break;
                }

        }
    }

    public void OnCollisionExit(Collision col)
    {
        //List<GameObject> Occu = Game.Occu;
        switch (col.collider.tag)
        {
            //지나간 스탠드 큐브는 add큐브로
            case "StandCube":
                {
                    if (Occu.Contains(col.gameObject))
                        col.gameObject.GetComponent<Cube>().UpdateType('a');

                    else
                        col.gameObject.GetComponent<Cube>().UpdateType('c');

                    break;
                }
        }
    }

    public void Position(Vector3 vt) {
        tr.position = vt;
    }
    
    public void HitBullet(){
        Game.ChanceDown();
    }

}
