using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Eric : AI {

	private Unit[] unitData;
	public Eric()
	{
	}
	void AI.initData(Unit[] tD){
		unitData = tD;
	}

	List<Command> AI.loop(List<Unit> myUnits, List<Unit> enemyUnits, List<Coordinate> foods, int resources)
	{
		List<Command> commands = new List<Command>();

		//spawn marines whenever possible
		Command spawnc = new Command();
		spawnc.addSpawn (UnityEngine.Random.Range(2,5));
		commands.Add (spawnc);


		float dir = -1.5708f;
		if(myUnits[0].getY () - enemyUnits[0].getY () < 0)
		{
			dir = 1.5708f;
		}

		for (int i = 1; i < myUnits.Count; i++)
		{
			int goal = 0;
			float min = 2341234;
			for (int j = 0; j < enemyUnits.Count; j++) {
				float d = dist (enemyUnits [j].getX (), enemyUnits [j].getY (), myUnits [i].getX (), myUnits [i].getY ());
				if (d < min) {
					min = d;
					goal = j;
				}
			}
			bool didAttack = true;
			if(dist(myUnits[i].getX(),myUnits[i].getY(), enemyUnits[goal].getX (), enemyUnits[goal].getY ()) < myUnits[i].getAttackRange())
			{
				if(enemyUnits[goal].getIsGround())
				{
					if(!myUnits[i].getCanAttackGround())
					{
						didAttack = false;
					}
				}
				else
				{
					if(!myUnits[goal].getCanAttackAir())
					{
						didAttack = false;
					}
				}
				if(didAttack)
				{
					//Sends attack command
					Command tAttack = new Command();
					tAttack.addAttack(myUnits[i].getID(), enemyUnits[goal].getID());
					commands.Add (tAttack);
				}
			}
			else{
				didAttack = false;
			}
			if (!didAttack) {
				if (myUnits [i].getType () == 2 && foods.Count > 0) {
					float tmin = 2341234;
					int tgoal = 0;
					for (int j = 0; j < foods.Count; j++) {
						float d = dist (foods [j].getX (), foods [j].getY (), myUnits [i].getX (), myUnits [i].getY ());
						if (d < min) {
							tmin = d;
							tgoal = j;
						}
					}

					Command moveC = new Command ();
					moveC.addMove (myUnits [i].getID (), getDirection (myUnits [i].getX (), myUnits [i].getY (), foods [tgoal].getX (), foods [tgoal].getY ()));
					commands.Add (moveC);
				} else {
					Command moveC = new Command ();
					moveC.addMove (myUnits [i].getID (), getDirection (myUnits [i].getX (), myUnits [i].getY (), enemyUnits [goal].getX (), enemyUnits [goal].getY ()));
					commands.Add (moveC);
				}
		
			}
		}

		//handle all units
		/*for (int i=1; i<myUnits.Count; i++) {
			//gets coordinates and attackRange of unit
			float attackRange = myUnits[i].getAttackRange();
			float tx = myUnits[i].getX ();
			float ty = myUnits[i].getY ();
			bool attacked = false;
			for(int j=0; j<enemyUnits.Count;j++)
			{
				//check if enemy is within attack Rnage
				if(dist(tx,ty, enemyUnits[j].getX (), enemyUnits[j].getY ()) < attackRange)
				{
					if(enemyUnits[j].getIsGround())
					{
						if(!myUnits[i].getCanAttackGround())
						{
							continue;
						}
					}
					else
					{
						if(!myUnits[i].getCanAttackAir())
						{
							continue;
						}
					}
					//Sends attack command
					Command tAttack = new Command();
					tAttack.addAttack(myUnits[i].getID(), enemyUnits[j].getID());
					commands.Add (tAttack);
					attacked = true;
					break;
				}
			}

			if(!attacked)
			{
				//did not find someone to attack

				//move forward
				Command moveC = new Command();
				moveC.addMove(myUnits[i].getID(), getDirection(myUnits[i].getX (), myUnits[i].getY (), enemyUnits[0].getX(), enemyUnits[0].getY()));
				commands.Add (moveC);
			}

		}*/

		return commands;
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
