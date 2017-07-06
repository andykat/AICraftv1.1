using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AlanAppleBeesAI: AI {
	private int resources;
	private List<Unit> myUnits;
	private List<Unit> enemyUnits;
	private List<Coordinate> foods;
	//private int[] maxHealth = {0,200,40,97,35,60};
	private Unit[] unitData;
	private List<Command> commands;
    private Vector2 baseLocation;

	public AlanAppleBeesAI()
	{
	}
    void AI.initData(Unit[] tD) {
        unitData = tD;
        ////Debug.Log("Huehuehuehuehue!");
        //if (unitData[0].getTeam() == 0) {
        //    //units[0].Add(createUnit(1, 0, 0.0f, -halfBoardHeight + charRadius, PI / 2.0f, 1));
        //    //units[1].Add(createUnit(1, 1, 0.0f, halfBoardHeight - charRadius, 1.5f * PI, 1001));
        //    baseLocation = new Vector2(-MainController.boardHeight / 2f, 1);
        //} else {
        //    baseLocation = new Vector2(MainController.boardHeight / 2f, 1001);
        //}
    }

    //Copy and pastA is the best
    List<Command> AI.loop(List<Unit> myUnits, List<Unit> enemyUnits, List<Coordinate> foods, int resources) {
        List<Command> commands = new List<Command>();
        if (myUnits.Count<= 1) {
            baseLocation = getVec(myUnits[0]);
            Debug.Log("Base location: " + baseLocation);
        }

        trySpawnBest(commands, myUnits, enemyUnits, foods, resources);
        //handle all units
        for (int i = 1; i < myUnits.Count; i++) {
            tryDoBest(commands, myUnits[i], myUnits, enemyUnits, foods, resources);
        }
        return commands;
    }

    public void trySpawnBest(List<Command> commands, List<Unit> myUnits, List<Unit> enemyUnits, List<Coordinate> foods, int resources) {
        //spawn marines whenever possible
        Command spawnc = new Command();
        spawnc.addSpawn(3);
        commands.Add(spawnc);
    }

    public void tryDoBest(List<Command> commands, Unit myUnit, List<Unit> myUnits, List<Unit> enemyUnits, List<Coordinate> foods, int resources) {
        //Since you can only move OR attack, be able to discern which is better at the given moment
        //Attack if enemy army chance of death >= friendly army chacne of death

        //An enemy army's chance of death = (if any enemy can die) units that can get killed this turn

        tryAttackBest(commands, myUnit, enemyUnits);
        tryMoveBest(commands, myUnit, myUnits, enemyUnits, foods);

    }

    public int getFriendlyArmyCOD(List<Unit> myUnits, List<Unit> enemyUnits) {
        int answer = 0;
        if (myUnits != null && enemyUnits != null) {
            for (int i = 0; i < enemyUnits.Count; i++) {
                if (canKill(enemyUnits[i], myUnits)) {
                    answer += 1;
                }
            }
        }
        return answer;
    }

    public int getEnemyArmyCOD(List<Unit> myUnits, List<Unit> enemyUnits) {
        int answer = 0;
        if (myUnits != null && enemyUnits != null) {
            for (int i = 0; i < myUnits.Count; i++) {
                if (canKill(myUnits[i], enemyUnits)) {
                    answer += 1;
                }
            }
        }
        return answer;
    }

    public bool tryAttackBest(List<Command> commands, Unit myUnit, List<Unit> enemyUnits) {
        //Let's go stupid: unit to hit prioritized by quantity of health weighted to distance

        bool answer = false;
        //returns a boolean for whether or not it attacked
        List<Unit> attackableEnemies = getCanAttackEnemies(myUnit, enemyUnits);
        if (attackableEnemies!= null && attackableEnemies.Count > 0) {
            Command attackCommand = getAttackLowestHealthCommand(myUnit, attackableEnemies);
            if (attackCommand != null) {
                commands.Add(attackCommand);
                answer = true;
            }
        }
        return answer;
    }

    public bool tryMoveBest(List<Command> commands, Unit myUnit, List<Unit> myUnits, List<Unit> enemyUnits, List<Coordinate> foods) {
        bool moved= false; //the random integer that shows how much you should move
        Command moveC = new Command();
        Vector2 myVec = getVec(myUnit);
        //Vector move based on the health of the enemy

        //bool foundFriend = getAttractionVector(moveVec, myUnits, myUnit, 2);
        //bool foundEnemy = getAttractionVector(moveVec, enemyUnits, myUnit, -1);
        Vector2 enemyVec = getVec(myUnit);
        Vector2 friendVec = getAttractionVector(myUnits, myUnit, 3);
        Vector2 foodVec = getVec(myUnit);

        if (getFriendlyArmyCOD(myUnits, enemyUnits) > getEnemyArmyCOD(myUnits, enemyUnits)) {
            enemyVec = getAttractionVector(enemyUnits, myUnit, 10);
            Debug.Log("victory mode");
        } else {
            enemyVec = getAttractionVector(enemyUnits, myUnit, 1);
            friendVec = getVec(myUnit);
        }

        if (enemyUnits.Count <= 1) {
            Debug.Log("Kill mode");
            foodVec = moveForFood(myUnit, foods, 10f);
            enemyVec = getVec(myUnit);
            friendVec = getVec(myUnit);
            moveC.addMove(myUnit.getID(), getDirection(myUnit.getX(), myUnit.getY(), foodVec.x, foodVec.y));
        } else {

            List<Vector2> interests = new List<Vector2>() { enemyVec, friendVec, foodVec };
            interests.Sort(delegate (Vector2 a, Vector2 b) {
                return Mathf.Abs(Vector2.SqrMagnitude(a + myVec))
                .CompareTo(
                  Mathf.Abs(Vector2.SqrMagnitude(b + myVec)));
            });
            Vector2 moveVec = interests[0];
            //Debug.Log("Move vec:" + moveVec);
            moveC.addMove(myUnit.getID(), getDirection(myUnit.getX(), myUnit.getY(), moveVec.x, moveVec.y));
        }

        commands.Add(moveC);

        return moved;
    }

    private Vector2 moveForFood(Unit myUnit, List<Coordinate> foods, float scale) {
        Vector2 moveVec = new Vector2(0, 0);
        if (foods != null && foods.Count > 0) {
            Vector2 bestFood = getVec(foods[0]);
            for (int i = 0; i < foods.Count; i++) {
                //Vector2 pullVec = getVec(foods[i]);
                //Debug.Log("Food vec: " + pullVec);

                float distance = Mathf.Sqrt(Vector2.Distance(getVec(myUnit), getVec(foods[i])));
                if (distance < Vector2.Distance(getVec(myUnit), bestFood)) {
                    bestFood = getVec(foods[i]);
                }
            }

            //bestFood.Normalize();
            //bestFood.Scale(new Vector2(scale, scale));
            moveVec += bestFood;
        }
        return moveVec;
    }

    private Vector2 getAttractionVector(List<Unit> attractors, Unit thisUnit, float scale) {
        Vector2 bestCandidate = getVec(thisUnit);
        if (attractors != null && attractors.Count > 0) {
            for (int i = 0; i < attractors.Count; i++) {
                if (attractors[i] != thisUnit) {
                    bestCandidate +=  getVec(attractors[i]);
                }
            }
            bestCandidate.Normalize();
            bestCandidate.Scale(new Vector2(scale, scale));
            //Debug.Log("Best candidate scale: " + bestCandidate.SqrMagnitude());
        }

        bestCandidate += new Vector2(0, 0);
        return bestCandidate;
        //return new Vector2(0, 0);
        //return baseLocation;
    }

    private Vector2 getAttractionVector(List<Unit> attractors, Unit myUnit, float scale, float minDistanceToAffect) {
        Vector2 moveVec = getVec(myUnit);

        if (attractors != null && attractors.Count > 0) {
            Vector2 bestCandidate = getVec(attractors[0]);
            for (int i = 0; i < attractors.Count; i++) {
                //Vector2 pullVec = getVec(foods[i]);
                //Debug.Log("Food vec: " + pullVec);

                float distance = Mathf.Sqrt(Vector2.Distance(getVec(myUnit), getVec(attractors[i])));
                if (distance > minDistanceToAffect && distance < Vector2.Distance(getVec(myUnit), bestCandidate)) {
                    moveVec = getVec(attractors[i]);
                }
            }
        }
        moveVec.Normalize();
        moveVec.Scale(new Vector2(scale, scale));
        return getVec(myUnit) + moveVec;
    }

    ///UTILITIES FUNCTIONS UNDER HERE

    public Command getAttackLowestHealthCommand(Unit myUnit, List<Unit> enemyUnits) {
        Command attackC = new Command();
        Unit enemyToAttack = null;

        for (int i=  0; i< enemyUnits.Count; i++) {
            if (canAttack(myUnit, enemyUnits[i])){
                if (enemyToAttack== null || enemyUnits[i].getHealth() < enemyToAttack.getHealth()) { //if candidate has less health, attack the candidate
                    enemyToAttack = enemyUnits[i];
                } else if (enemyUnits[i].getHealth() == enemyToAttack.getHealth()) { //if the candidat ehas the same health, attack one with more damage
                    if (enemyUnits[i].getAttackDamage() > enemyToAttack.getAttackDamage()) {
                        enemyToAttack = enemyUnits[i];
                    }
                }
            }
        }
        //Debug.Log("Enemy to attack: " + enemyToAttack.getHealth());
        if (enemyToAttack != null) {
            attackC.addAttack(myUnit.getID(), enemyToAttack.getID()); //Testing: Comment out to not attack
            return attackC;
        }else {
            return null;
        }
    }

    public List<Unit> getCanAttackEnemies(Unit myUnit, List<Unit> enemyUnits) {
        List<Unit> answer = new List<Unit>();

        for (int j = 0; j < enemyUnits.Count; j++) {
            if (canAttack(myUnit, enemyUnits[j])){
                //Debug.Log("can attack!");
                answer.Add(enemyUnits[j]);
            }
        }
        return answer;
    }

    public bool canKill(Unit enemy, List<Unit> myUnits) {
        int totalDamageable = 0;
        if (myUnits != null) {
            for (int i = 0; i < myUnits.Count; i++) {
                if (canAttack(myUnits[i], enemy)) {
                    totalDamageable += myUnits[i].getAttackDamage();
                }
            }
        }
        return totalDamageable >= enemy.getHealth();
    }

    public bool canAttack(Unit attacker, Unit attackee) {
        bool answer = false;
        float attackRange = attacker.getAttackRange();
        if (dist(attacker, attackee) <= attackRange) {
            if (attackee.getIsGround()) {
                if (attacker.getCanAttackGround()) {
                    answer = true;
                }
            } else {
                if (attacker.getCanAttackAir()) {
                    answer = true;
                }
            }
        }

        return answer;
    }

    private Vector2 getVec(Unit a) {
        return new Vector2(a.getX(), a.getY());
    }

    private Vector2 getVec(Coordinate coor) {
        return new Vector2(coor.getX(), coor.getY());
    }

    private float getDirection(Unit a, Unit b) {
        float x1 = a.getX();
        float y1 = a.getY();
        float x2 = b.getX();
        float y2 = b.getY();
        return ((float)Math.Atan2(y2 - y1, x2 - x1));
    }
    private float dist(Unit a, Unit b) {
        float x1 = a.getX();
        float y1 = a.getY();
        float x2 = b.getX();
        float y2 = b.getY();
        return ((float)Math.Sqrt(((double)((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)))));
    }

    private float getDirection(float x1, float y1, float x2, float y2) {
        return ((float)Math.Atan2(y2 - y1, x2 - x1));
    }
    private float dist(float x1, float y1, float x2, float y2) {
        return ((float)Math.Sqrt(((double)((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)))));
    }
}
