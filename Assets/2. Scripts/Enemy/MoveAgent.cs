using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour {

	public List<Transform> wayPoints;
	public int nextIdx;

	private readonly float patrolSpeed = 2.0f;
	private readonly float traceSpeed = 4.0f;

	//회전할 때의 속도를 조절
	private float damping = 1.0f;
	private NavMeshAgent agent;
	
	private Transform enemyTr;
	public float speed
	{
		get{ return agent.velocity.magnitude;}
	}
	void Start () {
		enemyTr = GetComponent<Transform>();
		agent = GetComponent<NavMeshAgent>();

		agent.autoBraking= false;
		agent.updateRotation = false;

		agent.speed = patrolSpeed;

		var group = GameObject.Find("WayPointGroup");
		if(group != null)
		{
			group.GetComponentsInChildren<Transform>(wayPoints);
			wayPoints.RemoveAt(0);
		}
		this.patrolling = true;
	}
	void Update(){
		//적 캐릭터가 이동중일때만 회전
		if(agent.isStopped == false)
		{
			Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
			enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
		}
		if(!_patrolling) return;

		if(agent.velocity.sqrMagnitude>=0.2f * 0.2f
		&& agent.remainingDistance <= 0.5f)
		{
			nextIdx = ++nextIdx % wayPoints.Count;

			MoveWayPoint();
		}
	}
	// Update is called once per frame
	void MoveWayPoint()
	{
		if(agent.isPathStale) return;


		agent.destination = wayPoints[nextIdx].position;
		agent.isStopped = false;
	}

	private bool _patrolling;
	public bool patrolling
	{
		get{ return _patrolling;}
		set
		{
			_patrolling  = value;
			if(_patrolling)
			{
				agent.speed = patrolSpeed;
				//순찰 상태의 회전 계수
				damping = 1.0f;
				MoveWayPoint();
			}
		}
	}

	private Vector3 _traceTarget;
	public Vector3 traceTarget
	{
		get{return _traceTarget;}
		set{
			_traceTarget = value;
			agent.speed = traceSpeed;
			//추적 상태의 회전 계수
			damping = 7.0f;
			TraceTarget(_traceTarget);
		}
	}

	//주인공을 주적할 때 이동시키는 함수
	void TraceTarget(Vector3 pos)
	{
		if(agent.isPathStale) return;
		agent.destination = pos;
		agent.isStopped = false;
	}

	//순찰 및 추적을 중지시키는 함수
	public void Stop()
	{
		agent.isStopped = true;
		agent.velocity = Vector3.zero;
		_patrolling = false;
	}
}
