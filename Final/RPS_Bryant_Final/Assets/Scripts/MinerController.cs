using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinerController : ObjectInfo
{
    public ResourceManager resourceManager;

    public bool isGathering;

    public int heldTurns;
    public int MAX_HELD_TURNS = 10;

    public GameObject[] depositNodes;
    GameObject miningNode;
    public NavMeshAgent navAgent;

    void Start()
    {
        GameObject player =  GameObject.FindGameObjectWithTag("MainCamera");
        resourceManager = player.GetComponent<ResourceManager>();
        setTask(Tasks.Idle);
        navAgent = GetComponent<NavMeshAgent>();

        objName = "Miner";
        objDescription = "Basic mining unit. Right click on mining node " +
        "to begin harvesting turns.";

        this.circle = GetComponent<LineRenderer>();

        isGathering = false;
        this.isSelected = false;
        StartCoroutine(GatherTick());
    }
    
    void Update()
    {
         if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        if (miningNode == null) {
            if (heldTurns != 0) {
                DeliverTurns();
            }
            else {
                setTask(Tasks.Idle);
            }
        }
        if (this.isSelected && Input.GetMouseButtonDown(1)) {
            RightClick();
        }
        if (heldTurns >= MAX_HELD_TURNS) {
            DeliverTurns();
        }
        if (navAgent.speed <= 0 && getTask() != Tasks.Gathering) {
            setTask(Tasks.Idle);
        }
    }

    public void RightClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.tag == "Ground") {
                navAgent.destination = hit.point;
                setTask(Tasks.Moving);
            }
            else {
                if (hit.collider.tag == "GatherNode") {
                    setTask(Tasks.Gathering);
                    miningNode = hit.collider.gameObject;
                }
                else if (hit.collider.tag =="DepositNode"){
                    setTask(Tasks.Delivering);
                }
                navAgent.destination = hit.collider.gameObject.transform.position;
            }
        }
    }
    public void OnTriggerEnter(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode" && getTask() == Tasks.Gathering) {
            hitObject.GetComponent<GatherManager>().gatherers++;
            isGathering = true;
            
             GatherTick();
        }
        if (hitObject.tag == "DepositNode" && getTask() == Tasks.Delivering) {
            // deposit turns and set held to zero
            resourceManager.turns += heldTurns;
            heldTurns = 0;
            // go back to the mining node if possible
            if (miningNode != null) {
                navAgent.destination = miningNode.transform.position;
                setTask(Tasks.Gathering);
            }
            else {
                setTask(Tasks.Idle);
            }
        }

    }
    
    public void OnTriggerExit(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode") {
            hitObject.GetComponent<GatherManager>().gatherers--;
            isGathering = false;
        }
    }

    private void DeliverTurns() {
        depositNodes = GameObject.FindGameObjectsWithTag("DepositNode");
        navAgent.destination = GetClosestDepositNode(depositNodes).transform.position;
        depositNodes = null;
        setTask(Tasks.Delivering);
    }

    private GameObject GetClosestDepositNode(GameObject[] depositNodes) {
        GameObject closestNode = null;
        float minDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject obj in depositNodes) {
            Vector3 direction = obj.transform.position - position;
            float nodeDistance = direction.sqrMagnitude;
            if (nodeDistance < minDistance) {
                minDistance = nodeDistance;
                closestNode = obj;
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
            yield return new WaitForSeconds(GatherManager.GATHER_TIME);
            if(isGathering) {
                heldTurns++;
            }
        }
    }
}
