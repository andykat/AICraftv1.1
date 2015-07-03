using UnityEngine;
using System.Collections;

public class Command {
	private int type = 0;
	private int selfID = 0;
	private int enemyID = 0;
	private int[] ints = new int[3];
	private float[] floats = new float[3];


	public Command()
	{
	}

	public void addSpawn(int unitType)
	{
		type = 1;
		ints [0] = unitType;
	}
	public void addMove(int unitID, float rotato)
	{
		type = 2;
		selfID = unitID;
		floats [0] = rotato;
	}
	public void addAttack(int unitID, int enemyUnitID)
	{
		type = 3;
		selfID = unitID;
		enemyID = enemyUnitID;
	}

	public int getType(){
		return type;
	}
	public int getSelfID(){
		return selfID;
	}
	public int getEnemyID(){
		return enemyID;
	}
	public int getInt(int index){
		return ints[index];
	}
	public float getFloat(int index)
	{
		return floats [index];
	}


}
