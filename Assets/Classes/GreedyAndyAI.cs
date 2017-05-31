using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GreedyAndyAI : AI {
	private int resources;
	private List<Unit> myUnits;
	private List<Unit> enemyUnits;
	private static int unitN = 6;
	private float maxDistance = 9.45f;
	private float distancePower = 0.5f;
	private float percentHealthPower = 2.0f;

	private float[][] counterValue = new float[unitN][];
	private Unit[] unitData;
	public GreedyAndyAI()
	{
		initCounterValue ();
	}
	void AI.initData(Unit[] tD){
		unitData = tD;
	}
	/*List<Command> AI.spawn(int resources){
		List<Command> commands = new List<Command> ();
		int unitType = 2;
		while (resources > 0) {
			Command spawnc = new Command();
			spawnc.addSpawn (unitType);
			commands.Add (spawnc);
			resources -= unitData[unitType].getCost();
			unitType++;
			if(unitType >= 5){
				unitType = 2;
			}
		}
		return commands;
	}*/

	List<Command> AI.loop(List<Unit> tMyUnits, List<Unit> tEnemyUnits, List<Coordinate> foods)
	{
		//resources = tResources;
		myUnits = tMyUnits;
		enemyUnits = tEnemyUnits;
		List<Command> commands = new List<Command> ();
		
		//spawn the counter whenever possible
		Command spawnc = new Command ();
		int spawnType = smartSpawn ();
		if (spawnType == 0) {
			if(resources > 119)
			{
				spawnc.addSpawn (2);
				commands.Add (spawnc);
			}
		} else {
			spawnc.addSpawn (spawnType);
			commands.Add (spawnc);
		}



		//run attack/move commands
		for (int i=1; i<myUnits.Count; i++) {
			//calculate the ideal enemy to attack


			//gets coordinates and attackRange of unit
			float attackRange = myUnits[i].getAttackRange();
			float tx = myUnits[i].getX ();
			float ty = myUnits[i].getY ();

			int maxScoreIndex = 0;
			float maxScore = -10000.0f;
			for(int j=0; j<enemyUnits.Count;j++)
			{
				float tScore = probabilityOfAttack(tx, ty, enemyUnits[j].getX (), enemyUnits[j].getY (), attackRange) 
							   *
							   rewardOfAttack(myUnits[i].getType(), enemyUnits[j].getType(), canAttack(i, j), enemyUnits[j].getHealth(), enemyUnits[j].getMaxHealth());

				if(tScore > maxScore)
				{
					maxScore = tScore;
					maxScoreIndex = j;
				}
			}
			Command tC = new Command();
			//check if ideal enemy is within range
			if(dist(tx, ty, enemyUnits[maxScoreIndex].getX (), enemyUnits[maxScoreIndex].getY ()) < attackRange)
			{
				//attack enemy
				tC.addAttack(myUnits[i].getID(), enemyUnits[maxScoreIndex].getID());

			}
			else
			{
				//move toward enemy
				tC.addMove(myUnits[i].getID(), getDirection(tx, ty, enemyUnits[maxScoreIndex].getX (), enemyUnits[maxScoreIndex].getY ())); 
			}

			commands.Add(tC);

		}


		return commands;
	}
	
	private float probabilityOfAttack(float selfX, float selfY, float enemyX, float enemyY, float selfAttackRange)
	{
		float tDist = dist (selfX, selfY, enemyX, enemyY);
		//if enemy is within range
		if (tDist < selfAttackRange) {
			return 1.0f;
		}

		//if enemy is out of range
		return (0.6f - (0.6f * ((float)Math.Pow( tDist / maxDistance, distancePower))));
	}
	private float rewardOfAttack(int selfType, int enemyType, float ifAttack, float enemyCurrentHealth, float enemyMaxHealth)
	{
		return counterValue [selfType] [enemyType] * ifAttack * ((float)Math.Pow((2.0f - (enemyCurrentHealth / enemyMaxHealth)),percentHealthPower));
	}


	private int smartSpawn()
	{
		int[] unitCounter = new int[unitN];
		for (int i=0; i<unitN; i++) {
			unitCounter [i] = 0;
		}
		for (int i=1; i<enemyUnits.Count; i++) {
			unitCounter[enemyUnits[i].getType()]++;	
		}
		int max = -1;
		int maxIndex = -1;
		for (int i=0; i<unitN; i++) {
			if (unitCounter [i] > max) {
				max = unitCounter [i];
				maxIndex = i;
			}
		}
		if (max == 0) {
			return 0;
		}
		return counter (maxIndex);

	}

	//sets up unit matchup
	private void initCounterValue ()
	{
		for (int i=0; i<unitN; i++) {
			counterValue [i] = new float[unitN];
			for(int j=0;j<unitN;j++)
			{
				counterValue[i][j] = 0.0f;
			}
		}



		//zergling
		counterValue[2][1] = 1.0f;
		counterValue[2][2] = 0.8f;
		counterValue[2][3] = 0.9f;
		counterValue[2][4] = 0.7f;
		counterValue [2] [5] = 0.3f;

		//marine
		counterValue[3][1] = 1.0f;
		counterValue[3][2] = 0.7f;
		counterValue[3][3] = 0.8f;
		counterValue[3][4] = 0.9f;
		counterValue [3] [5] = 0.9f;

		//flying thingy
		counterValue[4][1] = 1.0f;
		counterValue[4][2] = 0.9f;
		counterValue[4][3] = 0.7f;
		counterValue[4][4] = 0.8f;
		counterValue [4] [5] = 0.9f;

		//firebat
		counterValue[5][1] = 0.9f;
		counterValue[5][2] = 0.9f;
		counterValue[5][3] = 0.5f;
		counterValue[5][4] = 0.0f;
		counterValue[5][5] = 0.8f;
		
	}

	//returns 1 if unit cna attack enemy unit
	private float canAttack(int selfIndex, int enemyIndex)
	{
		if (enemyUnits [enemyIndex].getIsGround()) {
			if(myUnits[selfIndex].getCanAttackGround())
			{
				return 1.0f;
			}
			else
			{
				return -10.0f;
			}
		} else {
			if(myUnits[selfIndex].getCanAttackAir())
			{
				return 1.0f;
			}
			else
			{
				return -10.0f;
			}
		}
	}

	//returns direct Counter unit
	private int counter(int a)
	{
		if (a == 2) {
			return 4;
		}
		if (a == 3) {
			return 2;
		}
		if (a == 4) {
			return 3;
		}
		if (a == 5) {
			return 4;
		}
		else {
			return 3;
		}
	}
	private float getDirection(float x1, float y1, float x2, float y2)
	{
		return ((float) Math.Atan2 (y2 - y1, x2 - x1 ));
	}
	private float dist(float x1, float y1, float x2, float y2)
	{
		return ((float)Math.Sqrt(((double) (  (x2-x1)*(x2-x1) + (y2-y1)*(y2-y1)  )   )));
	}
}
