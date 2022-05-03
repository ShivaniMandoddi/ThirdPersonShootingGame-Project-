using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using MyObjectPool;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnemyController instance;

    NavMeshAgent agent;
    Animator animator;
    public Transform player;
    public State currentState;
    public GameObject bloodEffect;
    public GameObject enemyRagdoll;
    
    Player players = new Player();
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
        players.health = players.maxhealth;
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        player.GetComponent<PlayerMovement>().PlayerHealth(players.health);
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
    public void playerHealth()
    {
        players.health = Mathf.Clamp(players.health - 1, 0, players.maxhealth);
        player.GetComponent<PlayerMovement>().PlayerHealth(players.health);
        Debug.Log("Player health: " + players.health);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Bullet")
        {

            collision.gameObject.SetActive(false);
             if (Random.Range(0, 5) < 3)
             {
                 Instantiate(enemyRagdoll, this.transform.position, Quaternion.identity);
                 animator.SetBool("IsAttacking", false);
                 
                currentState = new Dead1(this.gameObject, agent, animator, player);
            }
             else
             {
                animator.SetBool("IsAttacking", false);
                currentState = new Dead(this.gameObject, agent, animator, player);
            } 
        }
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
    public bool CanSeePlayer()        //Checking Distanse between player and enemy
    {
        if (Vector3.Distance(nPC.transform.position, playerPosition.position) < 15f)
            return true;
        return false;
    }
}
public class Idle : State               //Idle State
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
        if (CanSeePlayer())                   //If enemy can see player, enemy goes to chase state
        {
            nextState = new Chase(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        else                        // Otherwise, goes to Wonder State
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
        float randValueX = nPC.transform.position.x + Random.Range(-8f, 8f);           // Taking random x and z position values
        float randValueZ = nPC.transform.position.z + Random.Range(-8f, 8f);
        float ValueY = Terrain.activeTerrain.SampleHeight(new Vector3(randValueX, 0f, randValueZ));  //Setting y position related to terrain height
        Vector3 destination = new Vector3(randValueX, ValueY, randValueZ);
        agent.SetDestination(destination);              // Transforming enemy position into ranodm position
        if (CanSeePlayer())           // If  enemy can see player, then enemy goes to chase state 
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
        agent.stoppingDistance = 4f;

    }
    public override void Enter()
    {
        animator.SetBool("IsRunning",true);
        base.Enter();

    }
    public override void Update()
    {
        agent.SetDestination(playerPosition.position);    // Enemy set to follow player
        nPC.transform.LookAt(playerPosition.position); // Enemy looking at player 

        if (!CanSeePlayer())       // If enemy can't see player, enemy goes to wondering state
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

        if (Vector3.Distance(nPC.transform.position, playerPosition.position) > agent.stoppingDistance+0.3f)  // If enemy is far than stopping distance, again to idle state 
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
public class Dead : State// Dead State
{
    float time;
    public Dead(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.DEATH;
        
    }
    public override void Enter()
    {
        animator.SetBool("IsDead", true);
        base.Enter();

    }
    public override void Update()
    {
        time = time + Time.deltaTime;
        if(time>4f)
        {
            
            time = 0f;
            nextState = new Idle(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
            
        }
        
    }
    public override void Exit()
    {
        animator.SetBool("IsDead", false);
        nPC.SetActive(false);
        base.Exit();
    }

}
public class Dead1 : State// Dead State
{
    float time;
    public Dead1(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.DEATH;

    }
    public override void Enter()
    {

        nextState = new Idle(nPC, agent, animator, playerPosition);
        eventStage = EVENTS.EXIT;
    }
    public override void Update()
    {
        

    }
    public override void Exit()
    {
        nPC.SetActive(false);
        base.Exit();
    }

}

