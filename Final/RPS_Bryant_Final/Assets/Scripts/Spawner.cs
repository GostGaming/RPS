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
    public bool isSpawning;
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
        if(this.unitHealth <= 0) {
            Destroy(gameObject);
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

    public void spawnUnit(UnitTypes unit) {
        GameObject spawnUnit = null;
        switch (unit) {
            case UnitTypes.RockUnit:
                spawnUnit = rockUnit;
                break;
            case UnitTypes.PaperUnit:
                spawnUnit = paperUnit;
                break;
            case UnitTypes.ScissorsUnit:
                spawnUnit = scissorsUnit;
                break;
        }
        Instantiate(spawnUnit);
        resourceManager.turns--;
        if(unitQueue.Count > 0) {
            SpawnTick();
        }
    }

    IEnumerator SpawnTick() {
        while (true) {
            yield return new WaitForSeconds(spawnDelay);
            if(isSpawning) {
                if (unitQueue.Peek() != null) {
                    UnitTypes type = 
                            unitQueue.Dequeue().GetComponent<ObjectInfo>().unitType;
                    spawnUnit(type);
                    countDown = spawnDelay;
                }
                else {
                    isSpawning = false;
                }
            }
        }
    }
}
