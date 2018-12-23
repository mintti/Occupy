using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
	public PlayerCtrl Player;
	public enum State
	{
		PATROL,
		TRACE,
		ATTACK,
		DIE
	}

	public State state = State.PATROL;
	private Transform playerTr;
	private Transform enemyTr;

	private Animator animator;
	public float attackDist = 5.0f;
	public float traceDist = 10.0f;
	public bool isDie = false;

	private WaitForSeconds ws;

	private MoveAgent moveAgent;

	private readonly int hashMove = Animator.StringToHash("IsMove");
	private readonly int hashSpeed = Animator.StringToHash("Speed");
	void Awake()
	{
		var player = GameObject.FindGameObjectWithTag("Player");

		if(player!=null)
			playerTr = player.GetComponent<Transform>();

		enemyTr = GetComponent<Transform>();
		animator = GetComponent<Animator>();
		moveAgent = GetComponent<MoveAgent>();

		ws = new WaitForSeconds(0.3f);
	}
	void Update()
	{
		//Speed 파라미터에 이동속도를 전달
		animator.SetFloat(hashSpeed, moveAgent.speed);
	}
	void OnEnable()
	{
		StartCoroutine(CheckState());
		StartCoroutine(Action());
	}

	IEnumerator CheckState(){
		while(!isDie){
			if(state == State.DIE) yield break;
			float dist = (playerTr.position - enemyTr.position).sqrMagnitude;

			if(Player.ValidAttack){
				if(dist <= attackDist)
					state = State.ATTACK;
				state = State.TRACE;
			}
			else{
				state = State.PATROL;
			}
			yield return new WaitForSeconds(0.3f);
		}
	}

	IEnumerator Action()
	{
		while(!isDie)
		{
			yield return ws;
			switch(state)
			{
				case State.PATROL :
					moveAgent.patrolling = true;
					animator.SetBool(hashMove, true);
					break;
				case State.TRACE :
					moveAgent.traceTarget = playerTr.position;
					animator.SetBool(hashMove, true);
					break;
				case State.ATTACK:
					moveAgent.Stop();
					animator.SetBool(hashMove, false);
					break;
				case State.DIE:
					moveAgent.Stop();
					break;
				
			}
		}
	}

	public int GetEnemyDie(){
		if(state == State.TRACE || state == State.ATTACK)
			return 1;
		return 0;
	}
}
