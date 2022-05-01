using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnemyController instance;

    NavMeshAgent agent;
    Animator animator;
    public Transform player;
    public State currentState;
    public GameObject bloodEffect;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        agent.enabled = true;

        currentState = new Idle(this.gameObject, agent, animator, player);
    }

    private void Update()
    {
        
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }
            currentState = currentState.Process();

        

    }
}
public class State
{
    public enum STATE
    {
        ATTACK, CHASE, IDLE, DEATH, WONDER
    }
    public enum EVENTS
    {
        ENTER, UPDATE, EXIT
    }
    public STATE stateName;
    public EVENTS eventStage;

    public GameObject nPC;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform playerPosition;
    public State nextState;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition)
    {
        this.nPC = _npc;
        this.playerPosition = _playerPosition;
        this.agent = _agent;
        this.animator = _animator;
        eventStage = EVENTS.ENTER;
    }
    public virtual void Enter()
    {
        eventStage = EVENTS.UPDATE;
    }
    public virtual void Update() 
    {
        eventStage = EVENTS.UPDATE;
    }
    public virtual void Exit()
    {
        eventStage = EVENTS.EXIT;
    }
    public State Process()
    {
        if (eventStage == EVENTS.ENTER)
        {
            Enter();
        }
        if (eventStage == EVENTS.UPDATE)
        {
            Update();
        }
        if (eventStage == EVENTS.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
    public bool CanSeePlayer()
    {
        if (Vector3.Distance(nPC.transform.position, playerPosition.position) < 15f)
            return true;
        return false;
    }
}
public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.IDLE;
        agent.enabled = true;
    }
    public override void Enter()
    {
        animator.SetBool("IsIdle",true);
        base.Enter();

    }
    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Chase(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        else
        {
            nextState = new Wonder(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.SetBool("IsIdle", false);
        base.Exit();
    }


}
public class Wonder : State
{
    public Wonder(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.WONDER;

    }
    public override void Enter()
    {
        animator.SetBool("IsWalking",true);
        base.Enter();

    }
    public override void Update()
    {
        float randValueX = nPC.transform.position.x + Random.Range(-8f, 8f);
        float randValueZ = nPC.transform.position.z + Random.Range(-8f, 8f);
        float ValueY = Terrain.activeTerrain.SampleHeight(new Vector3(randValueX, 0f, randValueZ));
        Vector3 destination = new Vector3(randValueX, ValueY, randValueZ);
        agent.SetDestination(destination);
        if (CanSeePlayer())
        {
            nextState = new Chase(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.SetBool("IsWalking",false);
        base.Exit();
    }
}
public class Chase : State
{
    public Chase(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.CHASE;
        agent.stoppingDistance = 5f;

    }
    public override void Enter()
    {
        animator.SetBool("IsRunning",true);
        base.Enter();

    }
    public override void Update()
    {
        agent.SetDestination(playerPosition.position);
        nPC.transform.LookAt(playerPosition.position);

        if (!CanSeePlayer())
        {
            nextState = new Wonder(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        if (Vector3.Distance(nPC.transform.position, playerPosition.position) <= agent.stoppingDistance)
        {
            nextState = new Attack(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }

        // base.Update();
    }
    public override void Exit()
    {
        animator.SetBool("IsRunning",false);
        base.Exit();
    }


}
public class Attack : State
{
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.ATTACK;
        nPC.transform.LookAt(playerPosition.position);
    }
    public override void Enter()
    {
        animator.SetBool("IsAttacking",true);
        base.Enter();

    }
    public override void Update()
    {
        if (Vector3.Distance(nPC.transform.position, playerPosition.position) > agent.stoppingDistance + 1f)
        {
            nextState = new Idle(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.SetBool("IsAttacking",false);
        base.Exit();
    }


}
