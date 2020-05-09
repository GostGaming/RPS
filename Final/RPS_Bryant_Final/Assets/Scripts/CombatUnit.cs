using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CombatUnit : ObjectInfo
{

    public NavMeshAgent navAgent;
    // In case of death, remove a hand
    public ResourceManager resourceManager;
    public float attackRange;
    public float attackDelay = 1f;
    public float attackDamage = 10f;
    public GameObject targetEnemy;
    public SelectOnClick selectOnClick;

    // Start is called before the first frame update
    void Start()
    {
         // Selection circle initiation
        this.circle = GetComponent<LineRenderer>();
        this.isSelected = false;
        this.unitHealth = 100;

        resourceManager = Camera.main.GetComponent<ResourceManager>();
        resourceManager.Hands++;
        selectOnClick = Camera.main.GetComponent<SelectOnClick>();
        navAgent = GetComponent<NavMeshAgent>();
        
        targetEnemy = null;
        StartCoroutine(AttackTick());
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
        if (this.isSelected && Input.GetMouseButtonDown(1)) {
            RightClick();
        }
        if (navAgent.speed <= 0 && getTask() != Tasks.Attacking) {
            setTask(Tasks.Idle);
        }
    }

    public void RightClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if(hit.collider.tag == "EnemyUnit" ||
                hit.collider.tag == "EnemyDepositNode") {
                targetEnemy = hit.collider.gameObject;
                setTask(Tasks.Attacking);
                AttackTick();
            }
            else {
                // if (hit.collider.tag == "Ground") {
                // Not an enemy, just move to the point
                // Game currently does  not allow friendly fire
                navAgent.stoppingDistance = 1f;
                navAgent.destination = hit.point;
                setTask(Tasks.Moving);
            }
        }
    }

    private void attack() {
        if(targetEnemy != null) {
            ObjectInfo enemyInfo = targetEnemy.GetComponent<ObjectInfo>();
            Vector3 enemyPosition = targetEnemy.transform.position;
            if (isInRange(enemyPosition)) {
                enemyInfo.unitHealth -= Mathf.Floor(attackDamage * typeCompare(enemyInfo.unitType));
            }
            else {
                navAgent.destination = enemyPosition;
                navAgent.stoppingDistance = attackRange;
            }
        }
        else {
            findNearEnemy();
            if (targetEnemy == null) setTask(Tasks.Idle);
        }
        // run attack animation
        // intantiate arrows
       
    }

    public bool isInRange(Vector3 pos) {
        Vector3 direction = pos - transform.position;
            float dist = direction.sqrMagnitude;
            return (dist < attackRange);
    }

    
    private void findNearEnemy() {
        GameObject closestEnemyUnit = null;
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        if (enemyUnits.Length == 0)
        { // No units found, target a building
            enemyUnits = GameObject.FindGameObjectsWithTag("ProductionBuilding");
            // no production buildings found, find the deposit node
            if (enemyUnits.Length == 0) {
                enemyUnits = GameObject.FindGameObjectsWithTag("DepositNode");
            }
            else enemyUnits = null;
            // nothing found, player has lost. We shouldn't get here.
        }
        if (enemyUnits != null) {
            float min = Mathf.Infinity;
            Vector3 position = transform.position;

            foreach (GameObject unit in enemyUnits) {
                Vector3 direction = unit.transform.position - position;
                float unitDist = direction.sqrMagnitude;
                if (unitDist < min) {
                    min = unitDist;
                    closestEnemyUnit = unit;
                }
            }
            targetEnemy = closestEnemyUnit;
        }
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
                    modifier = 1.5f;
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
        selectOnClick.selectedUnits.Remove(gameObject);
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
