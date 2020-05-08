using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Spawner : ObjectInfo
{
    public NavMeshAgent navAgent;
    // Delay between units
    public float spawnDelay = 15f;
    public float countDown;
    // Queue of units to spawn
    public Queue<GameObject> unitQueue;
    // only allow 5 units in queue at a time
    public int maxQueueSize = 5;
    public bool isSpawning;

    public ResourceManager resourceManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player =  GameObject.FindGameObjectWithTag("MainCamera");
        resourceManager = player.GetComponent<ResourceManager>();
        
        unitQueue = new Queue<GameObject>();
        this.objName = "Spawner";
        this.objDescription = "Used to spawn one of three units: Rock, Paper, or Scissors.";
        this.isSelected = false;
        this.circle = GetComponent<LineRenderer>();
        this.task = Tasks.Idle;
        isSpawning = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        if (isSpawning) {
            countDown--;
            Debug.Log(countDown);
        }
    }

    public void addToQueue(GameObject unit) {
        if (unitQueue.Count < maxQueueSize) {
            unitQueue.Enqueue(unit);

            Debug.Log("Adding obj to queue");
        }
        else {
            Debug.Log("Queue full");
        }
    }

    IEnumerator SpawnTick() {
        while (true) {
            yield return new WaitForSeconds(spawnDelay);
            if(isSpawning) {
                if (unitQueue.Peek() != null) {
                    Instantiate(unitQueue.Dequeue());
                    countDown = spawnDelay;
                    resourceManager.turns--;
                }
                else {
                    isSpawning = false;
                }
            }
        }
    }
}
