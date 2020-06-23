using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMiner : ObjectInfo
{
      public ResourceManager resourceManager;

    public bool isGathering;

    public int heldTurns;
    public int MAX_HELD_TURNS = 10;

    public GameObject depositNode;
    GameObject[] gatherNodes;
    GameObject gatherNode;
    public NavMeshAgent navAgent;

    public GameObject tell;
    public Material[] tellMats;

    void Start()
    {
        // Unit information
        this.unitType = UnitTypes.Miner;
        objName = "Enemy Miner";
        objDescription = "Basic mining unit. Destroy this to hinder enemy resource gathering";

        this.maxHealth = 50f;
        this.unitHealth = maxHealth;

        // Selection circle initiation
        this.circle = GetComponent<LineRenderer>();
        this.isSelected = false;
        
        // Task initiation
        setTask(Tasks.Idle);
        isGathering = false;

        navAgent = GetComponent<NavMeshAgent>();
        
         // Set Unit "tell" color to enemy color
        tell.GetComponent<MeshRenderer>().material = tellMats[1];

        depositNode = GameObject.FindGameObjectWithTag("EnemyDepositNode");

        StartCoroutine(GatherTick());
        StartGather();
    }

    void Update()
    {
        // Healthbar
        healthBar.fillAmount = this.unitHealth / maxHealth;
        // Ensure healthbar "billboards" properly
        healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
    

         if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        if (gatherNode == null) {
            if (heldTurns != 0) {
                DeliverTurns();
            }
            else {
                setTask(Tasks.Idle);
            }
        }
        if(this.unitHealth <= 0) {
            Destroy(gameObject);
        }

        if (heldTurns >= MAX_HELD_TURNS) {
            DeliverTurns();
        }
        if (navAgent.speed <= 0 && getTask() != Tasks.Gathering) {
            setTask(Tasks.Idle);
        }
    }
    public void StartGather() {
        gatherNode = findGatherNode();
        if (gatherNode != null) {
            navAgent.destination = gatherNode.transform.position;
            setTask(Tasks.Gathering);
        }
        else DeliverTurns();
    }
    private void OnDestroy() {
        resourceManager.Hands--;
    }

    public void OnTriggerEnter(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode" && getTask() == Tasks.Gathering) {
            hitObject.GetComponent<GatherNode>().gatherers++;
            isGathering = true;
            gatherNode = hitObject;
            GatherTick();
            // stop moving while gathering
            
        }
        if (hitObject.tag == "EnemyDepositNode" && getTask() == Tasks.Delivering) {
            // deposit turns and set held to zero
            resourceManager.turns += heldTurns;
            heldTurns = 0;
            // go back to the mining node if possible
            StartGather();
        }
    }
    
    public void OnTriggerExit(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode") {
            hitObject.GetComponent<GatherNode>().gatherers--;
            isGathering = false;
        }
    }

    private void DeliverTurns() {
        navAgent.destination = depositNode.transform.position;
        setTask(Tasks.Delivering);
    }

    private GameObject findGatherNode() {
        GameObject closestNode = null;
        gatherNodes = GameObject.FindGameObjectsWithTag("GatherNode");
        // No nodes to gather from, don't bother with the rest
        if (gatherNodes.Length == 0) return null;

        float min = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject node in gatherNodes) {
            Vector3 direction = node.transform.position - position;
            float nodeDist = direction.sqrMagnitude;
            if (nodeDist < min) {
                min = nodeDist;
                closestNode = node;
            }
        }
        return closestNode;
    }

    private void setTask(Tasks setTask) {
        this.task = setTask;
    }
    private Tasks getTask() {
        return this.task;
    }

    IEnumerator GatherTick() {
        while (true) {
            yield return new WaitForSeconds(GatherNode.GATHER_TIME);
            if(isGathering) {
                heldTurns++;
            }
        }
    }
}
