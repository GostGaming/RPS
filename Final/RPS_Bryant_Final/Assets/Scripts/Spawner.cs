using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/**
* Controller for player spawn building
*/
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
    // how many turns each hand "costs"
    public int unitCost = 10;
    public bool isSpawning;
    // Get a decent spawn location
    public GameObject spawnLoc;
    // Get unit prefabs
    public GameObject rockUnit;
    public GameObject paperUnit;
    public GameObject scissorsUnit;

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
        countDown = spawnDelay;
        StartCoroutine(SpawnTick());
    }

    // Update is called once per frame
    void Update()
    {
        if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        if (isSpawning) {
            countDown -= Time.deltaTime;
            countDown = Mathf.Clamp(countDown, 0, spawnDelay);
            if (unitQueue.Count <= 0) isSpawning = false;
        }
        if(this.unitHealth <= 0) {
            Destroy(gameObject);
        }
        if (!isSpawning && unitQueue.Count > 0) {
            isSpawning = true;
            SpawnTick();
        }
        if (isSelected) selectionOptions();
    }


    void selectionOptions() {
        if (resourceManager.turns >= unitCost) {
            if(Input.GetKeyDown(KeyCode.R)) {
                addToQueue(rockUnit);
            }
            else if(Input.GetKeyDown(KeyCode.T)) {
                addToQueue(paperUnit);
            }
            else if(Input.GetKeyDown(KeyCode.Y)) {
                addToQueue(scissorsUnit);
            }
        }
    }

    public void addToQueue(GameObject unit) {
        
        if (unitQueue.Count < maxQueueSize) {
            resourceManager.turns -= unitCost;
            unitQueue.Enqueue(unit);
            isSpawning = true;
            Invoke("SpawnTick", spawnDelay);
            Debug.Log("Adding " + unit.name + " to queue");
        }
        else {
            Debug.Log("Queue full");
        }
    }

    IEnumerator SpawnTick() {
        while (true) {
            yield return new WaitForSeconds(spawnDelay);
            if(isSpawning) {
                Instantiate(unitQueue.Dequeue(), spawnLoc.transform);
                countDown = spawnDelay;
                resourceManager.Hands++;
            }
        }
    }
}
