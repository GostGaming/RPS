using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MinerController : ObjectInfo
{
    public ResourceManager resourceManager;

    public bool isGathering;

    public int heldTurns;
    public int MAX_HELD_TURNS = 10;

    public GameObject depositNode;
    GameObject[] gatherNodes;
    GameObject gatherNode;
    public NavMeshAgent navAgent;
    public SelectOnClick selectOnClick;

    public GameObject tell;
    public Material[] tellMats;
    

    void Start()
    {
        // Unit information
        this.unitType = UnitTypes.Miner;
        objName = "Miner";
        objDescription = "Basic mining unit. Right click on mining node " +
        "to begin harvesting turns.";

        this.maxHealth = 50f;
        this.unitHealth = maxHealth;

        // Selection circle initiation
        this.circle = GetComponent<LineRenderer>();
        this.isSelected = false;
        
        // Get player camera and attached resource manager
        resourceManager = Camera.main.GetComponent<ResourceManager>();
        selectOnClick = Camera.main.GetComponent<SelectOnClick>();
        // Task initiation
        setTask(Tasks.Idle);
        isGathering = false;

        navAgent = GetComponent<NavMeshAgent>();

        // Set Unit "tell" color to player color
        // TODO: Mostly for future use. Maybe set tag here and conditionals to NPC
        tell.GetComponent<MeshRenderer>().material = tellMats[0];


        StartCoroutine(GatherTick());
        refreshGatherNodes();
        
    }
    
    void Update()
    {
        // Healthbar
        healthBar.fillAmount = this.unitHealth / maxHealth;
        // Ensure healthbar "billboards" properly
        healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
    
        // Unit selection circle
        if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        // Deliver turns if the node dies
        if (gatherNode == null) {
            if (heldTurns != 0) {
                DeliverTurns();
            }
            else {
                setTask(Tasks.Idle);
            }
        }
        // Unit death
        if(this.unitHealth <= 0) {
            Destroy(gameObject);
        }

        // Only listen to right clicks if it's selected
        if (this.isSelected && Input.GetMouseButtonDown(1)) {
            RightClick();
        }
        // Reached max turns, turn them in
        if (heldTurns >= MAX_HELD_TURNS) {
            DeliverTurns();
        }
        // Not doing anything, set task to idle
        if (navAgent.speed <= 0 && getTask() != Tasks.Gathering) {
            setTask(Tasks.Idle);
        }
    }

    public void RightClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.tag == "Ground") {
                setTask(Tasks.Moving);
                navAgent.destination = hit.point;
            }
            else {
                // not ground, check object tag
                if (hit.collider.tag == "GatherNode") {
                    setTask(Tasks.Gathering);
                    gatherNode = hit.collider.gameObject;
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
            hitObject.GetComponent<GatherNode>().gatherers++;
            isGathering = true;
            gatherNode = hitObject;
            GatherTick();
            // stop moving while gathering
            
        }
        if (hitObject.tag == "DepositNode" && getTask() == Tasks.Delivering) {
            // deposit turns and set held to zero
            resourceManager.turns += heldTurns;
            heldTurns = 0;
            
            // go back to the mining node if possible
            if (gatherNode != null) {
                navAgent.destination = gatherNode.transform.position;
                setTask(Tasks.Gathering);
            }
            // Gather node is depleted, find another one
            else {
                gatherNode = findGatherNode();
                if (gatherNode != null) {
                    navAgent.destination = gatherNode.transform.position;
                    setTask(Tasks.Gathering);
                }
                else  {
                    setTask(Tasks.Idle);
                }
            }
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
    private void refreshGatherNodes() {
        gatherNodes = GameObject.FindGameObjectsWithTag("GatherNode");
    }

    private void setTask(Tasks setTask) {
        this.task = setTask;
    }
    private Tasks getTask() {
        return this.task;
    }
     void OnDestroy() {
        // Deselect object so we don't try to access it after it's dead
        this.isSelected = false;
        selectOnClick.selectedUnits.Remove(gameObject);
        resourceManager.Hands--;
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
