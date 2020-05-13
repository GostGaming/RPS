using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed = 10f;
    public NavMeshAgent nav;
    public GameObject enemy;
    private Transform thisTransform = null;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        thisTransform = GetComponent<Transform>();
        body = GetComponent<Rigidbody>();
      //  nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy == null) {
            Destroy(gameObject);
        }
        else {
            FollowEnemy();
        }
    }

    public void FollowEnemy() {
        //nav.destination = enemy.transform.position;
        Vector3 direction = transform.position - enemy.transform.position;
        body.angularVelocity = Vector3.Cross(direction, transform.forward) * speed;
        body.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "EnemyUnit" || other.tag == "EnemyDepositNode") {
            Destroy(gameObject);
        }
    }
}
