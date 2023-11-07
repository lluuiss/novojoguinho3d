using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Random = UnityEngine.Random;

public class CombateEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth = 100;
    public float attackDamage;
    public float movementSpeed;
    public float lookRadius;
    public float colliderRadius = 2;
    public float rotationSpeed;

    [Header("Components")]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;
    private bool Walking;
    private bool attacking;
    private bool hiting;
    
    private bool waitFor;
    
    public bool playerIsDead;

    [Header("Others")]
    private Transform player;

    [Header("WayPoints")]
    public List<Transform> WayPoints = new List<Transform>();

    public int currentPathIndex;
    public float pathDistence;

    public AudioSource Somaranha;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalHealth > 0)
        {


            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= lookRadius)
            {

                //o personagem esta no raio de visao
                agent.isStopped = false;
                if (!attacking)
                {
                    agent.SetDestination(player.position);
                    anim.SetBool("Walk Forward", true);
                    Walking = true;
                }

                if (distance <= agent.stoppingDistance)
                {
                    StartCoroutine("Attack");
                    LookTarget();
                }
                else
                {
                    attacking = false;
                }
            }
            else
            {
                //o personagem esta fora do raio de visao
                anim.SetBool("Walk Forward", false);
                //agent.isStopped = true;
                Walking = false;
                attacking = false;
                MoveTowayPoint();
            }
        }
    }


    void MoveTowayPoint()
    {
        if (WayPoints.Count > 0)
        {
            float distance = Vector3.Distance(WayPoints[currentPathIndex].position, transform.position);
            agent.destination = WayPoints[currentPathIndex].position;


            if (distance <= pathDistence)
            {
                currentPathIndex = UnityEngine.Random.Range(0, WayPoints.Count);
            }
            anim.SetBool("Walk Forward", true);
            Walking = true;
        }  
    }
    
    IEnumerator Attack()
    {
        if (!waitFor && !hiting && !playerIsDead)
        {
            waitFor = true;
            attacking = true;
            Somaranha.Play();
            Walking = false;
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Pounce Attack", true);
            yield return new WaitForSeconds(1.5f);
            GetPlayer();
            //yield return new WaitForSeconds(1f);
            waitFor = false;
        }

        if (playerIsDead)
        {
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Pounce Attack", false);
            waitFor = false;
            attacking = false;
            agent.isStopped = true;
        }


    }

    void GetPlayer()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Player"))
            {
               //vai causar dano no player
               c.gameObject.GetComponent<Player>().GetHit(attackDamage);
               playerIsDead = c.gameObject.GetComponent<Player>().isDead;
            }
        }
    }

    public void GetHit(float damage)
    {
        totalHealth -= damage;
        //totalHealth = totalHealth - damage;
        if (totalHealth > 0)
        {
            //esta vivo
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damege");
            hiting = true;
            StartCoroutine("RecoveryFromHit");
        }
        else
        {
            //esta morto
            anim.SetTrigger("Die");
        }
    }
    
    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Pounce Attack", true);
        hiting = false;
        waitFor = false;
    }

    void LookTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}


