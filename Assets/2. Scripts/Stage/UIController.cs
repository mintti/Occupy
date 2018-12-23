using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {
    private GameController Game;
    
    public PlayerCtrl Player;
    public SoundController Sound;

    public GameObject BG;
    public GameObject Canvas;
    public int scene;
    public bool sw;
    float nextTime;
    int i,j;

    //#1 메인 화면
    public GameObject FlashObject;
    float timeRate;
    public float showTime;
    public float nonShowTime;

    //#2 게임 플레이 방법
    //#3 맵 선택
    int themeNum;
    public GameObject SelectText;

    //#4 게임 진행
    public Text chance;
    public Text percent;
    public Text time;
    public AudioClip rePlayAudio;

    //#5 게임 오버

    public Text OverTime;

    //#6 게임 클리어
    public Text GameRank;
    public Text Porcess;
    public Text TotalScore;
    public GameObject image;
    private void Start()
    {
        Game = this.GetComponent<GameController>();
        //Start Scene
        scene = 1;

        //Flash 
        timeRate = 0;
        sw = true;
        

        //Select;
        themeNum = 0;

        BG.SetActive(true);
        Canvas.transform.GetChild(1).gameObject.SetActive(true);
        Canvas.transform.GetChild(2).gameObject.SetActive(false);
        Canvas.transform.GetChild(3).gameObject.SetActive(false);
        Canvas.transform.GetChild(4).gameObject.SetActive(false);
        Canvas.transform.GetChild(5).gameObject.SetActive(false);
        Canvas.transform.GetChild(6).gameObject.SetActive(false);

        Time.timeScale = 1;
    }
    private void Update()
    {

        switch (scene)
        {
            //#1 _ MainLogo
            case 1:
                {
                    Flash();
                    break;
                }
            //#2 _ HowPlay
            case 2:
                {
                    break;
                }
            //#3 _ SelectMap
            case 3:
                {
                    Select();
                    break;
                }
            //#4 _ GamePlay
            case 4:
                {
                    if(Game.scene == 5)
                        Play();
                    break;
                }
            //#5 _ GameOver
            case 5:
                {
                    GameOver();
                    break;
                }
            //#6 _ ClearGame
            case 6:
                {
                    GameClear();
                    break;
                }
        }
    }

    //showTime, nonShowTime, sw, timeRate, j, scene
    public void Flash()
    {   
        //title 깜빡거리는 효과들 제어
        if (nonShowTime == 0.5f)
        {
            timeRate += Time.deltaTime;
            if (sw)
            {
                if (timeRate >= showTime)
                {
                    FlashObject.SetActive(false);
                    timeRate = 0;
                    sw = false;
                }
            }
            else
            {
                if (timeRate >= nonShowTime)
                {
                    FlashObject.SetActive(true);
                    timeRate = 0;
                    sw = true;
                }
            }
            if (Input.anyKeyDown)
            {
                Sound.PlayAudio_Select();
                timeRate = 0;
                nextTime = 1.6f;
                showTime = 0;
                nonShowTime = 0.2f;
                sw = false;
            }
        }
        else
        {//위에서 아무키나 눌렀을 경우 효과제어
            Canvas.transform.GetChild(scene).GetComponent<Transform>().position += new Vector3(0, 0.5f, 0);
            if (timeRate > nextTime) // Next Scene
            {
                timeRate = 0;
                Canvas.transform.GetChild(scene).gameObject.SetActive(false);
                scene = 3;
                Canvas.transform.GetChild(scene).gameObject.SetActive(true);
                
                showTime = 0;
                nonShowTime = 2f;
                j = 1;
            }
            if (showTime > nonShowTime)
            {
                Canvas.transform.GetChild(scene).gameObject.SetActive(!Canvas.transform.GetChild(scene).gameObject.active);
                showTime = 0;
            }
            timeRate += Time.deltaTime;
            showTime += Time.deltaTime;
        }

    }

    //Var = showTime, nonShowTime, i, j, sw, themeNum, timeRate, nextTime, scene
    public void Select() {

        if (showTime > nonShowTime)
        {
            showTime = 0;
            j *= -1;
        }
        SelectText.GetComponent<Transform>().position += new Vector3(j, 0, 0);
        showTime += Time.deltaTime;

        Transform list = Canvas.transform.GetChild(3).transform.GetChild(0).transform;
        if (!sw)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                list.GetChild(themeNum).GetChild(1).gameObject.SetActive(false);
                themeNum = (themeNum + 1) % 4;
                list.GetChild(themeNum).GetChild(1).gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                list.GetChild(themeNum).GetChild(1).gameObject.SetActive(false);
                themeNum--;
                if (themeNum < 0) themeNum = 3;
                list.GetChild(themeNum).GetChild(1).gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Sound.PlayAudio_Select();
                sw = true;
                nextTime = 0.4f;
                timeRate = 0;
                i = 0;
            }
        }
        else
        {
            if (i < 4)
            {
                if (timeRate > nextTime)
                {
                    list.GetChild(themeNum).gameObject.SetActive(!list.GetChild(themeNum).gameObject.active);
                    i++;
                    timeRate = 0;
                }
                timeRate += Time.deltaTime;
            }
            else
            {
                
                Canvas.transform.GetChild(scene).gameObject.SetActive(false);
                scene = 4;
                Canvas.transform.GetChild(scene).gameObject.SetActive(true);
                BG.SetActive(false);
                
                //테마에 맞는 스테이지 제작 함수 호출
                Game.MakeStage(themeNum);
            }
        }
    }

    //게임 진행도를 저장하는 변수.
    public float pr;
    public void Play()
    {
        //Time.timeScale = 1;
        pr = Game.PlayerCubeCount / (float)Game.maxCube * 100;
        // Debug.Log("PCCOunt : " + Game.PlayerCubeCount);
        percent.text = (int)pr + " %";
        chance.text = "CHANCE : " + Game.chance;
        time.text = Convert.ToString(Game.time);
        
        
    }
    //#5 GameOver
    public void GameOver(){
        //Time.timeScale = 0;
        Canvas.transform.GetChild(4).gameObject.SetActive(false);
        Canvas.transform.GetChild(scene).gameObject.SetActive(true);
        Game.Enemy.SetActive(false);

        if(sw){//게임오버 첫 호출 시 기본 세팅
            timeRate = 0;
            nextTime = 1;
            i = 10; //카운트 다운 10초
            OverTime.text = Convert.ToString(i);
            sw = false;
        }
        else{
            if(i>0){ 
                if(timeRate > nextTime){
                    timeRate = 0;
                    i--;
                    OverTime.text = Convert.ToString(i);
                }

                timeRate += Time.deltaTime;

                //R눌러서 목숨얻고 진행
                if(Input.GetKey(KeyCode.R)){
                    Canvas.transform.GetChild(4).gameObject.GetComponent<AudioSource>().clip = rePlayAudio;
                    Canvas.transform.GetChild(4).gameObject.SetActive(true);
                    Canvas.transform.GetChild(scene).gameObject.SetActive(false);
                    Game.Enemy.SetActive(true);
                    Player.SetPlayerPostion();
                    //리세팅
                    Game.chance = 3;
                    Game.time = 130;
                    scene = 4;
                    Game.replay++;
                }
                else if (Input.anyKeyDown){ //이 외의 키 다운 입력시 초 빠르게 내림
                    i--;
                    OverTime.text = Convert.ToString(i);
                }
            }
            else{ //카운트 다운 완료시 씬로드로 게임 다시시작
                SceneManager.LoadScene("asdf");
                }

        }
    }

    //#6GameClear
    public void GameClear(){
        float score;
        float scope = 1;

        //전체이미지를 보여주기
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(Game.w *15 , Game.h *15);
        image.GetComponent<Image>().sprite = Game.pictures[themeNum].picture[Game.imgNum].s;

        
        Canvas.transform.GetChild(scene).gameObject.SetActive(true);
        score = Game.chance * 200 + Game.time * 5;
        if(pr <=83)
            scope = 1;
        else if(pr <=85)
            scope = 1.2f;
        else if( pr <= 90)
            scope =1.5f;
        else if( pr <= 95)
            scope =1.75f;
        else if(pr <= 100)
            scope =2;

        score *= scope;
        score -= 600*Game.replay;
        if(score < 0)
            score = 0;

        //랭크 집계 /C : 1 B : 1000  A: 1500 S : 1750 SS:2000
        if(score >= 2000)
            GameRank.text = "★SS★";
        else if(score >=1750)
            GameRank.text = "★S★";
        else if(score >=1500)
            GameRank.text = "★A★";
        else if(score >=1000)
            GameRank.text = "★B★";
        else if( score > 0)
            GameRank.text = "★C★";
        else
            GameRank.text = "★F★";
        
        Porcess.text = "("+Game.time + "(T)*5+" + Game.chance + "(C)*200)*" + scope + " - 600 * " + Game.replay + "(R) =";
        TotalScore.text = Convert.ToString(Convert.ToInt32(score));
        if(Input.GetKey(KeyCode.R))
            SceneManager.LoadScene("asdf");
        
    }
}
