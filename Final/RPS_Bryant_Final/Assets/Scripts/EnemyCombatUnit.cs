using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyCombatUnit : ObjectInfo
{
    public NavMeshAgent navAgent;
    // In case of death, remove a hand
    public ResourceManager resourceManager;

    public float attackRange;
    public float attackDelay = 1f;
    public float attackDamage = 10f;
    public GameObject playerUnit;

    // Start is called before the first frame update
    void Start()
    {
        // circle not for selected but attack blink
        this.circle = GetComponent<LineRenderer>();
        this.isSelected = false;
        this.unitHealth = 100;
        resourceManager = Camera.main.GetComponent<ResourceManager>();

        navAgent = GetComponent<NavMeshAgent>();
        
        playerUnit = null;
        StartCoroutine(AttackTick());
        findPlayerUnit();
        setTask(Tasks.Attacking);
        AttackTick();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.circle != null) {
            this.circle.enabled = isSelected;
        }
        if(this.unitHealth <= 0) {
            Destroy(gameObject);
        }
        
        if (navAgent.speed <= 0 && getTask() != Tasks.Attacking) {
            setTask(Tasks.Idle);
        }
    }

    private void findPlayerUnit() {
        GameObject closestPlayerUnit = null;
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("Unit");
        if (playerUnits.Length == 0)
        { // No units found, target a building
            playerUnits = GameObject.FindGameObjectsWithTag("ProductionBuilding");
            // no production buildings found, find the deposit node
            if (playerUnits.Length == 0) {
                playerUnits = GameObject.FindGameObjectsWithTag("DepositNode");
            }
            else playerUnits = null;
            // nothing found, player has lost. We shouldn't get here.
        }
        if (playerUnits != null) {
            float min = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject unit in playerUnits) {
                Vector3 direction = unit.transform.position - position;
                float unitDist = direction.sqrMagnitude;
                if (unitDist < min) {
                    min = unitDist;
                    closestPlayerUnit = unit;
                }
            }
            playerUnit = closestPlayerUnit;
        }
    }
  

    private void attack() {
        if (playerUnit == null) {
            findPlayerUnit();
        }
        ObjectInfo playerUnitInfo = playerUnit.GetComponent<ObjectInfo>();
        Vector3 playerUnitPos = playerUnit.transform.position;
        if (isInRange(playerUnitPos)) {
            playerUnitInfo.unitHealth -= Mathf.Floor(attackDamage * typeCompare(playerUnitInfo.unitType));
        }
        else {
            navAgent.destination = playerUnitPos;
            navAgent.stoppingDistance = attackRange;
        }
        // run attack animation
        // intantiate arrows
        AttackTick();
    }

    public bool isInRange(Vector3 pos) {
        Vector3 direction = pos - transform.position;
            float dist = direction.sqrMagnitude;
            return (dist < attackRange);
    }

    public float typeCompare(UnitTypes targetUnitType) {
        UnitTypes rock = UnitTypes.RockUnit;
        UnitTypes paper = UnitTypes.PaperUnit;
        UnitTypes scissors = UnitTypes.ScissorsUnit;
        float modifier = 1f;
        if (this.unitType == rock && targetUnitType == paper ||
            this.unitType == paper && targetUnitType == scissors ||
            this.unitType == scissors && targetUnitType == rock) {
                modifier = 0.5f;
        }
        else if (this.unitType == rock && targetUnitType == scissors ||
                this.unitType == paper && targetUnitType == rock ||
                this.unitType == scissors && targetUnitType == paper) {
                    modifier = 2f;
            }
        return modifier;
    }

    void OnDrawGizmosSelected()
    {
        // Range indication
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
    private void setTask(Tasks setTask) {
        this.task = setTask;
    }
    private Tasks getTask() {
        return this.task;
    }
     void OnDestroy() {
        resourceManager.Hands--;
    }

    IEnumerator AttackTick() {
        while (true) {
            yield return new WaitForSeconds(attackDelay);
            if(getTask() == Tasks.Attacking) {
                attack();
            }
        }
    }
}
