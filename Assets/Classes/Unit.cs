using UnityEngine;
using System.Collections;

public class Unit {
	private int type = 0;
	private int cost = 0;
	private int maxHealth = 0;
	private int health = 0;
	private float moveSpeed = 0.0f;
	private float attackRange = 0.0f;
	private int attackDamage = 0;
	private int team = 0;
	private float x = 0.0f;
	private float y = 0.0f;
	private float rotato = 0.0f;
	private int ID = 0;
	private bool isGround = true;
	private bool canAttackGround = true;
	private bool canAttackAir = false;
	private bool isAOE = false;
	private float aoeRadius = 0.0f;

	public Unit(int tType, int tCost, int tMaxHealth, int tHealth, float tMoveSpeed, float tAttackRange, int tAttackDamage, 
	            int tTeam, float tX, float tY, float tRotato, int tid, bool tIsGround, bool tCanAttackGround, 
	            bool tCanAttackAir, bool tIsAOE, float tAoeRadius)
	{
		type = tType;
		cost = tCost;
		maxHealth = tMaxHealth;
		health = tHealth;
		moveSpeed = tMoveSpeed;
		attackRange = tAttackRange;
		attackDamage = tAttackDamage;
		team = tTeam;
		x = tX;
		y = tY;
		rotato = tRotato;
		ID = tid;
		isGround = tIsGround;
		canAttackGround = tCanAttackGround;
		canAttackAir = tCanAttackAir;
		isAOE = tIsAOE;
		aoeRadius = tAoeRadius;
	}

	public int getType() {return type;}

	public int getCost() {
		return cost;
	}
	public void setCost(int tCost){
		cost = tCost;
	}
	public int getMaxHealth(){
		return maxHealth;
	}
	public int getHealth() {return health;}
	
	public void setHealth(int newHealth) {health = newHealth;}
	
	public float getMoveSpeed() {return moveSpeed;}
	
	public float getAttackRange() {return attackRange;}
	
	public int getAttackDamage() {return attackDamage;}
	
	public int getTeam() {return team;}
	
	public float getX() {return x;}
	
	public void setX(float newX) {x = newX;}
	
	public float getY(){return y;}
	
	public void setY(float newY){y = newY;}
	
	public float getRotato(){return rotato;}
	
	public void setRotato(float newRotato){rotato = newRotato;}

	public int getID(){
		return ID;
	}

	public bool getIsGround(){
		return isGround;
	}
	public bool getCanAttackGround(){
		return canAttackGround;
	}
	public bool getCanAttackAir(){
		return canAttackAir;
	}

	public bool getIsAOE()
	{
		return isAOE;
	}
	public float getAoeRadius()
	{
		return aoeRadius;
	}
}
