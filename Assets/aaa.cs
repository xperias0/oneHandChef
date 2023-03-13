using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aaa : MonoBehaviour
{
    public enum State { none, idle, walk, attack, chase, escape, die }
    public State CurrentState = State.none;
    public List<Transform> Waypoints = new List<Transform>();
    public int WaypointIndex = -1;
    NavMeshAgent navMeshAgent;
    float FSMTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        ToWalk();

       

    }

    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    void FSM()
    {
        switch (CurrentState)
        {
            case State.idle:
                FSMTimer += Time.deltaTime;
                if (FSMTimer >= 1)
                {
                    FSMTimer = 0;
                    ToWalk();
                }
                break;

            case State.walk:
                // state update
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                    ToIdle();
                break;

            case State.attack:
                break;
        }
    }

    void ToIdle()
    {
        CurrentState = State.idle;
        // state start
    }

    void ToWalk()
    {
        CurrentState = State.walk;

        WaypointIndex = (WaypointIndex + 1) % Waypoints.Count;
        navMeshAgent.SetDestination(Waypoints[WaypointIndex].position);
    }
}
