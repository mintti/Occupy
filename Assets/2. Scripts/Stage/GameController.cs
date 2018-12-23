using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class Picture
{
    public Material m;
    public Sprite s;
    public int w, h; //가로, 높이 비율
}
[Serializable]
public class Array
{
    public Picture[] picture;
}

public class GameController : MonoBehaviour {
    public OtherStageArea DestroyWayPoint;
    private UIController UI;
    public PlayerCtrl Player;
    public GameObject Enemy;
    public int scene = 0;

    //#0  MakeStage
    public Array[] pictures = new Array[4]; //사진
    public GameObject basicCube;
    public GameObject img;
    public GameObject cubeList;
    
    public GameObject[,] cube;

    public int w, h;
    public GameObject Boundary;
    public int imgNum;

    //#1  MakeCube
    int i = 0, j = 0;
    float timeRate = 0;
    float nextTime;
    public int maxCube; //전체 큐브갯수
    int p, q;


    //#5 Cube;
    public List<GameObject> Occu = new List<GameObject>();
    
    //GamePlay
    public int chance;
    public int PlayerCubeCount; //플레이어가 먹은 큐브 갯수 카운트. 이용하여 Game.Pr을 구함.
    public int time;
    public int replay;  //다시시작횟수

    //아이템 관련
    public GameObject[] item;
    public GameObject itemTextPos; //아이템 생성시 텍스트 pos
    public GameObject itemText; //지정 후 프리팹 생성후 생성된 오브젝트 내 스크립트 함수 호출

    //Camera
    public Transform Camera;
    public GameObject Camera2;
    float x, y, z;
    public void Start()
    {
        scene = 0;
        UI = this.GetComponent<UIController>();
        replay =0;
    }


    private void Update()
    {
        switch (scene)
        {
            //앞서 테마선택 후 Makestage() 함수를 호출한 뒤 이다.
            //일정시간 시간을 주어 큐브를 만든다.
            case 1:
                {//스테이지 구성 큐브들 생성
                    MakeCube();
                    break;
                }
            case 2:
                {//큐브들 알맞게 정렬 후 다음 단계
                    cubeList.GetComponent<GridLayoutGroup>().enabled = false;
                    PlayerCubeCount = 0;
                    chance = 3;
                    scene = 3;
                    break;
                }
            //초반 스타트 큐브 및 Player 위치 지정
            case 3:
                {
                    RandomBase();
                    Player.transform.position = cube[Player.minY, Player.minX].transform.position;
                    Player.gameObject.SetActive(true);
                    Enemy.gameObject.SetActive(true);

                    //카메라 확대 계산 용 x,z
                    x = -Player.transform.position.x / ((Camera.position.y - 8) / 0.1f);
                    z = -Player.transform.position.z/ ((Camera.position.y - 8) / 0.1f);

                    scene = 4;
                    break;
                }
            case 4: //카메라를 y축으로 확대합니다.
                {
                    Camera.position -= new Vector3( x, 0.1f, z);
                    if (Camera.position.y <= 8.1f)
                    {
                        Player.gameObject.SetActive(true);
                        //Camera.GetComponent<SmoothFollow>().enabled = true;
                        Camera2.SetActive(true);
                        Camera.gameObject.SetActive(false);

                        nextTime = 1;
                        timeRate = 0;
                        time = 130;
                        scene = 5;
                    }
                    break;
                }

            case 5: //게임 진행에 관련된 메소드들 관리
                {
                    if(timeRate > nextTime)
                    {
                        time--;
                        timeRate = 0;


                        //20초마다 아이템 생성
                        if(time % 20 == 0){
                            //아이템 뭐 할지 ..  
                            int i = Random.Range(0, item.Length);
                            int p, q;
                            if(i == 2 && Player.moveSpeed >= 0.08f) //일정스피드 이상 일 때, 더이상 스피드 아이템 생성안함.
                                i = Random.Range(0,  item.Length-1);
                            
                            //아이템이 생성될 수 있는 Cube들을 List에 넣어줌.
                            List<GameObject> TempCubeList = new List<GameObject>();

                            for(p = 0; p< h ;p++){
                                for(q = 0; q < w; q++)
                                    if(cube[p,q].tag == "Cube")
                                        TempCubeList.Add(cube[p, q]);
                            }
                            
                            int n = Random.Range(0,TempCubeList.Count);
                            
                            Vector3 pos = TempCubeList[n].transform.position;
                            TempCubeList.Clear();
                            pos.y = 0.5f;
                            GameObject  obj = Instantiate(item[i], pos ,Quaternion.identity);
                                    
                            Debug.Log("아이템이 생성되었습니다.");
                            String arrow;//아이템 위치 적는..

                            if(Player.transform.position.x < obj.transform.position.x)
                                arrow = "오른쪽";
                            else
                                arrow = "왼쪽";

                            if(Player.transform.position.z < obj.transform.position.z)
                                arrow += "위";
                            else
                                arrow +="아래";

                            //아이템 위치 알려주는 텍스트 생성
                            GameObject text = Instantiate(itemText, Vector3.zero, Quaternion.identity);
                            text.GetComponent<ItemTextMover>().TextSetting(arrow);
                            text.transform.SetParent(itemTextPos.transform, false);

                            //30초 뒤 파괴 선언   
                            Destroy(obj,30.0f);
                        }
                            
                    }
                        
                    timeRate += Time.deltaTime;

                    //시간 0아래로가면 목숨깎임
                    if (time <= 0)
                    {
                        ChanceDown();
                    }
                    //80프로 이상시 클리어
                    if(UI.pr >= 80){
                        UI.scene = 6;
                        scene = 6;
                        Enemy.SetActive(false);
                    }
                    break;
                }
                case 6 : {
                    break;
                }
            default :
                break;
        }
    }

    //#S0 _ 1회 호출
    public void MakeStage(int themeNum)
    {
        imgNum = Random.Range(0, pictures[themeNum].picture.Length);
        //cube 갯수 : 쿼드 10x10 기준 400개 , 쿼드 0.5당 큐브 1개
        w = pictures[themeNum].picture[imgNum].w*2;
        h = pictures[themeNum].picture[imgNum].h*2;
        maxCube =  w * h;
        cube = new GameObject[h, w];

    
        //이미지 불러오기 & 스테이지 크기 조정
        img.GetComponent<MeshRenderer>().material = pictures[themeNum].picture[imgNum].m;
        img. GetComponent<Transform>().localScale = new Vector3(w/2, h/2, 1);
        cubeList.GetComponent<GridLayoutGroup>().constraintCount =  w;
        Boundary.GetComponent<Transform>().localScale =  new Vector3(w/2*1.01f, h/2*1.01f, 5);

        //이미지 스포 방지 MakeCube()에서 Scene바뀔 때 true로 변경
        img.GetComponent<MeshRenderer>().enabled = false;

        //크기 이외의 WayPoint 제거_컴포넌드 OtherStageArea;
        DestroyWayPoint.ActiveOffWayPoint();

        //초기 세팅
        p = q = 0;
        Player.gameObject.SetActive(false);
        //Camera.GetComponent<SmoothFollow>().enabled = false;
        Camera.gameObject.SetActive(true);
        Camera2.SetActive(false);

        scene = 1;
    }

    //스테이지 구성큐브 생성
    public void MakeCube()
    {
        //한개씩 생성하지말고,,,j개씩 생성
        if( i == 0)
        {
            for (j = 3; maxCube % j != 0; j--) ;
        }
        if (i != maxCube)
        {
            //cube 생성
            for (int i = 0; i < j; i++)
            {
                GameObject obj = Instantiate(basicCube, cubeList.transform);

                if (q != 0)
                    p = Convert.ToInt32(q / w);
                //생성 후 관리를 위해 cube배열에 넣어줌.
                cube[p, (q++) % w] = obj;
                cube[p, (q-1) % w].GetComponent<Cube>().SetCoor(p, (q-1) % w);
                cube[p, (q - 1) % w].GetComponent<Cube>().Game = this;
                cube[p, (q - 1) % w].GetComponent<Cube>().Player = Player;
            }
            i += j;
            // 카메라 y축
            y += 0.1f; // 0.1
        }
        else //종료한다.
        {
            //카메라 고정
            Camera.position = new Vector3(Camera.position.x, 8, Camera.position.z);

            img.GetComponent<MeshRenderer>().enabled = true;
            scene = 2;
            Debug.Log("MakeCube Complete");
        }
        Camera.position = new Vector3(Camera.position.x, y, Camera.position.z);
    }

    //#S3 _ 초기 큐브 생성
    public void RandomBase()
    {
        int n;

        n = Random.Range(3, 6);
        p = Random.Range(1, h-n);
        q = Random.Range(1, w-n);
  
        //안쪽큐브 none큐브
        for(int i = 0; i< n; i++)
        {
            for(int j = 0; j< n; j++)
            {
                cube[p + i, q + j].GetComponent<Cube>().UpdateType('n');
            }
        }

        //바깥쪽 Player큐브 지정
        for(int i = 0; i<n+2; i++)
        {
            cube[p - 1, q - 1 + i].GetComponent<Cube>().UpdateType('p');
            cube[p + n, q - 1 + i].GetComponent<Cube>().UpdateType('p');
        }
        
        for(int i = 0; i <n; i++)
        {
            cube[p + i, q - 1].GetComponent<Cube>().UpdateType('p');
            cube[p + i, q + n].GetComponent<Cube>().UpdateType('p');
        }

        //PlayerCube 영역저장 (PlayerCtrl 에서 사용됨)
        Player.minX = q - 1;
        Player.maxX = q + n;
        Player.minY = p - 1;
        Player.maxY = p + n;


        PlayerCubeCount = (n + 2) * (n + 2);
    }


    //목숨 올리는 함수
    public void ChanceUp()
    {
        chance++;
    }

    //목숨감소하는 함수
    public void ChanceDown()
    {
        chance--;
        //목숨 0되면 게임오버 씬으로..
        if(chance < 0)
        {
            UI.sw = true;
            UI.scene = 5;

        }
        else
        {
            Player.SetPlayerPostion();
            time += 30;
        }
    }
   
}

