using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DumbMarinesAI : AI {
	private Unit[] unitData;
	public DumbMarinesAI()
	{
	}
	void AI.initData(Unit[] tD){
		unitData = tD;
	}
	List<Command> AI.spawn(int resources){
		List<Command> commands = new List<Command> ();
		while (resources > unitData[3].getCost()) {
			Command spawnc = new Command();
			spawnc.addSpawn (3);
			commands.Add (spawnc);
			resources -= unitData[3].getCost();
		}
		return commands;
	}
	List<Command> AI.loop(List<Unit> myUnits, List<Unit> enemyUnits)
	{
		List<Command> commands = new List<Command>();
		
		//spawn marines whenever possible
		/*Command spawnc = new Command();
		spawnc.addSpawn (3);
		commands.Add (spawnc);*/
		
		float dir = -1.5708f;
		if(myUnits[0].getY () - enemyUnits[0].getY () < 0)
		{
			dir = 1.5708f;
		}
		
		//handle all units
		for (int i=1; i<myUnits.Count; i++) {
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
			
		}
		
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
