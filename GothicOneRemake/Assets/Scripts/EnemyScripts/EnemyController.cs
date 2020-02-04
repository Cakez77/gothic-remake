using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyController : MonoBehaviour {

    private EnemyAnimator enemy_Anim;
    private NavMeshAgent navMeshAgent;

    private EnemyState enemy_State;

    public float walk_Speed = 0.5f;
    public float run_Speed = 4f;

    public float chase_Distance = 7f;
    private float current_Chase_Distance;
    public float attack_Distance = 1.8f;
    public float chase_After_Attack_Distance = 2f;

    public float patrol_Radius_Min = 20f, patrol_Radius_Max = 60f;
    public float patrol_For_This_Time = 15f;
    private float patrol_Timer;

    public float wait_Before_Attack = 2f;
    private float attack_Timer;

    private Transform target;

    private void Awake() {
        enemy_Anim = GetComponent<EnemyAnimator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;
    }

    private void Start() {

        enemy_State = EnemyState.PATROL;

        patrol_Timer = patrol_For_This_Time;

        // when the enemy first gets to the player
        // attack right away
        attack_Timer = wait_Before_Attack;

        // momorize the value of chase distance
        // so that we can put it back 
        current_Chase_Distance = chase_Distance;
    }

    private void Update() {

        if (enemy_State == EnemyState.PATROL) {
            // patrol
            Patrol();
        }

        if (enemy_State == EnemyState.CHASE) {
            // chase
            Chase();
        }

        if (enemy_State == EnemyState.ATTACK) {
            // attack
            Attack();
        }
    }

    void Patrol() {

        // tell nav agent that he can move
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = walk_Speed;

        // add to the patrol timer
        patrol_Timer += Time.deltaTime;

        if (patrol_Timer > patrol_For_This_Time) {
            SetNewRandomDestination();
            patrol_Timer = 0f;
        }

        if (navMeshAgent.velocity.sqrMagnitude > 0) {
            enemy_Anim.Walk(true);
        } else {
            enemy_Anim.Walk(false);
        }

        // test the distance beween the player and the enemy
        if (Vector3.Distance(transform.position, target.position) <= chase_Distance) {
            // chase

            enemy_Anim.Walk(false);
            enemy_Anim.Run(true);

            enemy_State = EnemyState.CHASE;
            Chase();
        }

    }

    void Chase() {
        // enable the agent to move again
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = run_Speed;

        // set the players destination ad the destination
        // because we are chasing (running towards) the player
        navMeshAgent.SetDestination(target.position);

        if (navMeshAgent.velocity.sqrMagnitude > 0) {

            enemy_Anim.Run(true);

        } else {

            enemy_Anim.Run(false);
        }

        // id the distance between the enemy and the player is less than attack distance
        if(Vector3.Distance(transform.position, target.position) <= attack_Distance) {
            
            // stop the animations
            enemy_Anim.Run(false);
            enemy_Anim.Walk(false);
            enemy_State = EnemyState.ATTACK;

            //reset the chase distance to previous
            if(chase_Distance != current_Chase_Distance) {
                chase_Distance = current_Chase_Distance;
            }
        } else if(Vector3.Distance(transform.position, target.position) > chase_Distance) {
            // player run away from enemy

            // stop running
            enemy_Anim.Run(false);
            enemy_State = EnemyState.PATROL;

            // reset the patrol timer so that the function
            // can calculate the new patrol detination right away
            patrol_Timer = patrol_For_This_Time;

            if(chase_Distance != current_Chase_Distance) {
                chase_Distance = current_Chase_Distance;
            }
        }
    } // chase

    void Attack() { }

    void SetNewRandomDestination() {

        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

        Vector3 randomDirection = Random.insideUnitSphere * rand_Radius;
        print("Random point: " + randomDirection);
        // move the random position inside the patrol range
        randomDirection += transform.position;

        print("Random local point: " + randomDirection);
        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(randomDirection, out navMeshHit, rand_Radius, -1);

        print("navMeshHit: " + navMeshHit.position);

        navMeshAgent.SetDestination(navMeshHit.position);

    }
} // class