using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AndrewBoxAI: AI {
	private int resources;
	private List<Unit> myUnits;
	private List<Unit> enemyUnits;
	private List<Coordinate> foods;
	private static int unitN = 6;
	private float maxDistance = 9.45f;
	private float distancePower = 0.5f;
	private float percentHealthPower = 2.0f;
	private int[] maxHealth = {0,200,40,97,35,60};
	private float[][] counterValue = new float[unitN][];
	private Unit[] unitData;
	private int zerglingcount = 0;
	private int marinecount = 0;
	private List<Command> commands;
	public AndrewBoxAI()
	{
		initCounterValue ();
	}
	void AI.initData(Unit[] tD){
		unitData = tD;
	}

	void zergling(Unit unit){
		
		//avoid air units
		Unit unitToAttack = unit;
		float maxScore = 0.0f;
		foreach (Unit enemyUnit in enemyUnits) 
		{
			
			if (enemyUnit.getType () == 3 || enemyUnit.getType () == 5 || enemyUnit.getType() == 2) 
			{
				float score = (1.2f - ((float)enemyUnit.getHealth()) / ((float)maxHealth[enemyUnit.getType()]))
					* probabilityOfAttack(unit.getX(), unit.getY(), enemyUnit.getX(), enemyUnit.getY(), unit.getAttackRange());
				if (score > maxScore) {
					maxScore = score;
					unitToAttack = enemyUnit;
				}
			}
		}
		Debug.Log ("maxScore:" + maxScore.ToString());

		if (maxScore > 0.06) {
			Command tC = new Command();
			if (dist (unit.getX(), unit.getY(), unitToAttack.getX (), unitToAttack.getY ()) < unit.getAttackRange()) {
				//attack enemy
				tC.addAttack (unit.getID (), unitToAttack.getID ());

			} else {
				//move toward enemy
				tC.addMove (unit.getID (), getDirection (unit.getX(), unit.getY(), unitToAttack.getX (), unitToAttack.getY ())); 
			}
			commands.Add(tC);
		}
		else {
			//collect food
			if (foods.Count > 0) {
				float tmin = 99999.0f;
				int tgoal = 0;
				for (int j = 0; j < foods.Count; j++) {
					float d = dist (foods [j].getX (), foods [j].getY (), unit.getX (), unit.getY ());
					if (d < tmin) {
						tmin = d;
						tgoal = j;
					}
				}

				Command moveC = new Command ();
				moveC.addMove (unit.getID (), getDirection (unit.getX (), unit.getY (), foods [tgoal].getX (), foods [tgoal].getY ()));
				commands.Add (moveC);
			}
		}
		
	}
	void marineAndFirebat(Unit unit)
	{
		int lowestHealthUnitIndex = -1;
		int lowestHealth = 999;
		for (int i = 0; i < enemyUnits.Count; i++) 
		{
			if (dist (unit.getX (), unit.getY (), enemyUnits[i].getX (), enemyUnits[i].getY ()) < unit.getAttackRange ()) 
			{
				if (enemyUnits [i].getHealth () < lowestHealth) 
				{
					lowestHealth = enemyUnits [i].getHealth ();
					lowestHealthUnitIndex = i;
				}
			}
		}
		if (lowestHealthUnitIndex > -1)
		{
			Command attackCommand = new Command ();
			attackCommand.addAttack (unit.getID (), enemyUnits [lowestHealthUnitIndex].getID());
			commands.Add (attackCommand);
			return;
		}

		//no zerglings, collect food

		if (zerglingcount < 1) {
			if (foods.Count > 0) {
				float tmin = 99999.0f;
				int tgoal = 0;
				for (int j = 0; j < foods.Count; j++) {
					float d = dist (foods [j].getX (), foods [j].getY (), unit.getX (), unit.getY ());
					if (d < tmin) {
						tmin = d;
						tgoal = j;
					}
				}

				Command moveC = new Command ();
				moveC.addMove (unit.getID (), getDirection (unit.getX (), unit.getY (), foods [tgoal].getX (), foods [tgoal].getY ()));
				commands.Add (moveC);
			}
		}

		float dir = -1.5708f;
		if(myUnits[0].getY () - enemyUnits[0].getY () < 0)
		{
			dir = 1.5708f;
		}
		if ((dir < 0 && unit.getY()  > 0.0f) ||(dir > 0 && unit.getY() < 0.0f)) {
			Command moveC = new Command ();
			moveC.addMove (unit.getID (), getDirection (unit.getX (), unit.getY (), 0.0f, 0.0f));
			commands.Add (moveC);
		} else {
			//idk
			if (marinecount > enemyUnits.Count - 2) {
				Command moveC = new Command ();
				moveC.addMove (unit.getID (), getDirection (unit.getX (), unit.getY (), enemyUnits[0].getX(), enemyUnits[0].getY()));
				commands.Add (moveC);
			}
		}
	}
	List<Command> AI.loop(List<Unit> tMyUnits, List<Unit> tEnemyUnits, List<Coordinate> tfoods, int tResources)
	{
		
		resources = tResources;
		myUnits = tMyUnits;
		enemyUnits = tEnemyUnits;
		foods = tfoods;
		commands = new List<Command> ();

		//spawn the counter whenever possible

		if (resources == 300) {
			Command spawn1 = new Command ();
			Command spawn2 = new Command ();
			Command spawn3 = new Command ();
			Command spawn4 = new Command ();
			spawn1.addSpawn (2);
			spawn2.addSpawn (3);
			spawn3.addSpawn (3);
			spawn4.addSpawn (3);
			commands.Add (spawn1);
			commands.Add (spawn2);
			commands.Add (spawn3);
			commands.Add (spawn4);

		} else {
			//count number of zerglings
			zerglingcount = 0;
			marinecount = 0;
			for (int i = 1; i < myUnits.Count; i++)
			{
				if (myUnits [i].getType () == 2) {
					zerglingcount += 1;
				} else if (myUnits [i].getType () == 3) {
					marinecount += 1;
				}
			}
			if (zerglingcount < 2) {
				if (resources > 39) {
					Command spawn1 = new Command ();
					spawn1.addSpawn (2);
					commands.Add (spawn1);
				}
			}
			else 
			{
				if (resources > 79) {
					Command spawn1 = new Command ();
					spawn1.addSpawn (3);
					commands.Add (spawn1);
				}
			}
		}

		//run attack/move commands
		for (int i=1; i<myUnits.Count; i++) {
			//calculate the ideal enemy to attack
			if (myUnits [i].getType () == 2)
			{
				zergling (myUnits [i]);
			}
			else if (myUnits [i].getType () == 3 || myUnits [i].getType () == 5)
			{
				marineAndFirebat (myUnits [i]);
			}
		
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
