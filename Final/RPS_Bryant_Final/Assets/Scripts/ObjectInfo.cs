using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour
{
    
    public bool isSelected;
    public bool isGathering;

    public string objName;

    public int heldHands;
    public int maxHeldHands;
    
    private NavMeshAgent navAgent;

    void Start()
    {
        isSelected = false;
        isGathering = false;
        navAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(GatherTick());
    }
    
    void Update()
    {
        if (isSelected && Input.GetMouseButtonDown(1)) {
            RightClick();
        }
        if (heldHands >= maxHeldHands) {
            // go to dropoff
        }
    }

    public void RightClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.tag == "Ground") {
                navAgent.destination = hit.point;
                Debug.Log("moving...");
            }
            else if (hit.collider.tag == "GatherNode") {
                navAgent.destination = hit.collider.gameObject.transform.position;
                Debug.Log("Gathering...");

            }
        
        }

    }
    public void OnTriggerEnter(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode") {
            hitObject.GetComponent<GatherManager>().gatherers++;
            
        }

    }
    public void OnTriggerExit(Collider other) {
        GameObject hitObject = other.gameObject;
        if (hitObject.tag == "GatherNode") {
            hitObject.GetComponent<GatherManager>().gatherers--;
        }
    }

    IEnumerator GatherTick() {
        while (true) {
            yield return new WaitForSeconds(1);
            if(isGathering) {
                heldHands++;
            }
        }
    }

}
