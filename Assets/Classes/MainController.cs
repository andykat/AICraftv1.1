using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class MainController : MonoBehaviour {
	public GameObject[] unitObjectPreFabs;
	public GameObject[] wins;
	public GameObject bulletGO;
	//50 fps
	private float framePeriod = 0.08f;
	private float timeCurrent = -100000.0f;

	private float PI = (float)Math.PI;

	private int startingResource = 500;
	//private int resourceIncrease = 3;

	private float charRadius = 0.5f;

	//Board Data
	private float boardHeight = 8.0f;
	private float boardWidth = 5.0f;
	private float halfBoardHeight = 4.0f;
	private float halfBoardWidth = 2.5f;

	//All data for units
	private Unit[] unitData = new Unit[6]; 
	private List<Unit>[] units = new List<Unit>[2];
	private AI[] players = new AI[2];
	private int[] resources = new int[2];
	private int[] ids = new int[2];

	private List<GameObject>[] unitGOs = new List<GameObject>[2];

	private List<GameObject> bulletGOs = new List<GameObject>();
	private List<Bullet> bullets = new List<Bullet> ();
	private float bulletSpeed = 0.4f;

	//Loads the AI that you choose
	private void loadAI()
	{
		players [0] = new DumbMarinesAI ();
		players [0].initData (unitData);
		players [1] = new GreedyAndyAI ();
		players [1].initData (unitData);
	}


	void Start () {
		initUnitData ();

		//Load AI
		loadAI ();

		unitGOs [0] = new List<GameObject> ();
		unitGOs [1] = new List<GameObject> ();

		//initalize units
		units [0] = new List<Unit> ();
		units [1] = new List<Unit> ();

		//add nexus
		units [0].Add (createUnit(1, 0, 0.0f, -halfBoardHeight + charRadius, PI/2.0f, 1));
		units [1].Add (createUnit (1, 1, 0.0f, halfBoardHeight - charRadius, 1.5f*PI,1001));

		unitGOs [0].Add (Instantiate(unitObjectPreFabs[0]) as GameObject);
		unitGOs [1].Add (Instantiate(unitObjectPreFabs[1]) as GameObject);

		unitGOs [0] [0].transform.position = new Vector2 (units [0] [0].getX (), units [0] [0].getY ());
		unitGOs [1] [0].transform.position = new Vector2 (units [1] [0].getX (), units [1] [0].getY ());

		unitGOs [0] [0].transform.eulerAngles = new Vector3 (0.0f, 0.0f, units [0] [0].getRotato () * 180.0f / PI);
		unitGOs [1] [0].transform.eulerAngles = new Vector3 (0.0f, 0.0f, units [1] [0].getRotato () * 180.0f / PI);

		//set up resources
		resources [0] = startingResource;
		resources [1] = startingResource;

		//set up ids
		ids [0] = 2;
		ids [1] = 1002;

		//Spawn units
		spawnUnits ();
		//start the game
		timeCurrent = -0.5f;
	}
	
	//Loop
	void Update () {
		//update time
		timeCurrent += Time.deltaTime;

		//check if next frame can be run
		if (timeCurrent < framePeriod) {
			return;
		}

		timeCurrent -= framePeriod;

		//run AI
		List<Command> p0Commands = players [0].loop (units [0], units [1]);
		List<Command> p1Commands = players [1].loop (units [1], units [0]);

		//run commands received from AI.
		runCommands (p0Commands, p1Commands);


		//check and delete dead units
		for(int i=0;i<2;i++)
		{
			for(int j=0;j<units[i].Count;j++)
			{
				if(units[i][j].getHealth() < 1)
				{
					//unit has died
					if(j==0)
					{
						endGame(i);
					}
					else
					{
						units[i].RemoveAt(j);
						Destroy(unitGOs[i][j]);
						unitGOs[i].RemoveAt(j);
						j--;
					}
				}
			}
		}

		//update bullets
		for(int i=0;i<bullets.Count;i++)
		{
			bullets[i].iterate();
			bulletGOs[i].transform.position = new Vector2(bullets[i].getX(), bullets[i].getY ());
			if(bullets[i].getX() < -10000.0f)
			{
				Destroy(bulletGOs[i]);
				bulletGOs.RemoveAt(i);
				bullets.RemoveAt(i);
				i--;
			}
		}
	}

	private void endGame(int loser)
	{
		timeCurrent = -1000000.0f;
		framePeriod = 1000000.0f;
		int winner = getEnemy (loser);
		wins [winner].transform.position = new Vector3 (-2.27f, 2.35f, -0.01f);

	}

	//Runs commands received from AI Class
	private void runCommands(List<Command> lc0, List<Command> lc1)
	{
		bool[] finishedCommand = new bool[units [0].Count];
		for (int i=0; i<units[0].Count; i++) {
			finishedCommand[i] = false;
		}

		bool[] finishedCommand1 = new bool[units [1].Count];
		for (int i=0; i<units[1].Count; i++) {
			finishedCommand1[i] = false;
		}

		//only runs first 100 commands
		int tN = minimum (100, lc0.Count);
		int tN1 = minimum (100, lc1.Count);

		//run attack commands for player 0
		for (int i=0; i<tN; i++) {
			if(lc0[i].getType() == 3)
			{
				int si = idFind(0, lc0[i].getSelfID());
				if(!finishedCommand[si])
				{
					//attack
					attack(0, lc0[i].getSelfID(), lc0[i].getEnemyID());
					finishedCommand[si] = true;
				}
			}
		}

		//run attack commands for player 1
		for (int i=0; i<tN1; i++) {
			if(lc1[i].getType() == 3)
			{
				int si = idFind(1, lc1[i].getSelfID());
				if(!finishedCommand1[si])
				{
					//attack
					attack(1, lc1[i].getSelfID(), lc1[i].getEnemyID());
					finishedCommand1[si] = true;
				}
			}
		}

		//run non attack commands for player 0
		for (int i=0; i<tN; i++) {
			if(lc0[i].getType() == 2)
			{
				int si = idFind(0, lc0[i].getSelfID());
				if(!finishedCommand[si])
				{
					//move
					move(0, lc0[i].getSelfID(), lc0[i].getFloat(0));
					finishedCommand[si] = true;
				}
			}
		}

		//run non attack commands for player 1
		for (int i=0; i<tN1; i++) {
			if(lc1[i].getType() == 2)
			{
				int si = idFind(1, lc1[i].getSelfID());
				if(!finishedCommand1[si])
				{
					//move
					move(1, lc1[i].getSelfID(), lc1[i].getFloat(0));
					finishedCommand1[si] = true;
				}
			}
		}
	}

	private void attack(int player, int selfID, int enemyID)
	{
		//search for indexes of ids'
		int selfIndex = idFind (player, selfID);
		int enemyPlayer = getEnemy (player);
		int enemyIndex = idFind (enemyPlayer, enemyID);
		if (selfIndex == -1 || enemyIndex == -1) {
			return;
		}

		//check if enemy is in attack Range
		if (units [player] [selfIndex].getAttackRange () > dist (units [player] [selfIndex].getX (), units [player] [selfIndex].getY ()
		                                         , units [enemyPlayer] [enemyIndex].getX (), units [enemyPlayer] [enemyIndex].getY ())) 
		{
			if(units[enemyPlayer][enemyIndex].getIsGround())
			{
				if(!units[player][selfIndex].getCanAttackGround())
				{
					return;
				}
			}
			else
			{
				if(!units[player][selfIndex].getCanAttackAir())
				{
					return;
				}
			}

			//if unit is aoe
			if(units[player][selfIndex].getIsAOE())
			{
				float eX = units[enemyPlayer][enemyIndex].getX ();
				float eY = units[enemyPlayer][enemyIndex].getY ();
				for(int i=1;i<units[enemyPlayer].Count;i++)
				{
					if(units[enemyPlayer][i].getIsGround())
					{
						if(dist(eX, eY, units[enemyPlayer][i].getX(), units[enemyPlayer][i].getY ()) < units[player][selfIndex].getAoeRadius())
						{
							units[enemyPlayer][i].setHealth(units[enemyPlayer][i].getHealth() - 
							                                         units[player][selfIndex].getAttackDamage());

							//create bullet
							int tI = bulletGOs.Count;
							bulletGOs.Add(Instantiate(bulletGO as GameObject));
							bullets.Add(new Bullet(units[player][selfIndex].getX(),units[player][selfIndex].getY(),units[enemyPlayer][i].getX(),units[enemyPlayer][i].getY(), bulletSpeed));
							
							bulletGOs[tI].transform.position = new Vector2(bullets[tI].getX(), bullets[tI].getY ());
							bulletGOs[tI].transform.eulerAngles = new Vector3(0.0f, 0.0f, bullets[tI].getDir()*180.0f/PI);
						}
					}
				}
			}
			else
			{
				//unit is single target

				//take away health
				units[enemyPlayer][enemyIndex].setHealth(units[enemyPlayer][enemyIndex].getHealth() - 
				                                         units[player][selfIndex].getAttackDamage());

				//create bullet
				int tI = bulletGOs.Count;
				bulletGOs.Add(Instantiate(bulletGO as GameObject));
				bullets.Add(new Bullet(units[player][selfIndex].getX(),units[player][selfIndex].getY(),units[enemyPlayer][enemyIndex].getX(),units[enemyPlayer][enemyIndex].getY(), bulletSpeed));

				bulletGOs[tI].transform.position = new Vector2(bullets[tI].getX(), bullets[tI].getY ());
				bulletGOs[tI].transform.eulerAngles = new Vector3(0.0f, 0.0f, bullets[tI].getDir()*180.0f/PI);
			}


		}
		
	}

	private void move(int player, int id, float rotato)
	{
		//search for index of ID
		int cIndex = idFind(player, id);

		if (cIndex == -1) {
			//id not found
			return;
		}

		//tried to move unit on the other team!
		if (player != units [player] [cIndex].getTeam ()) {
			return;
		}

		units [player] [cIndex].setRotato (rotato);
		unitGOs [player] [cIndex].transform.eulerAngles = new Vector3 (0.0f, 0.0f, rotato * 180.0f / PI);
		float newX = units [player] [cIndex].getX () + units [player] [cIndex].getMoveSpeed () * ((float)Math.Cos (rotato));
		float newY = units [player] [cIndex].getY () + units [player] [cIndex].getMoveSpeed () * ((float)Math.Sin (rotato));

		if (newX > halfBoardWidth || newX < -halfBoardWidth || newY > halfBoardHeight || newY < -halfBoardHeight) {
			//out of bounds
			return;
		}

		units [player] [cIndex].setX (newX);
		units [player] [cIndex].setY (newY);

		unitGOs [player] [cIndex].transform.position = new Vector2 (newX, newY);
	}
	private void spawnUnits(){
		List<Command> p0Spawns = players [0].spawn (resources [0]);
		List<Command> p1Spawns = players [1].spawn (resources [1]);
		for (int i=0; i<p0Spawns.Count; i++) {
			spawn(0, p0Spawns[i].getInt(0),i+1);
		}
		for (int i=0; i<p1Spawns.Count; i++) {
			spawn(1, p1Spawns[i].getInt(0),i+1);
		}

	}
	private void spawn(int player, int type, int position)
	{
		//calculate spawn location
		float spawnX = -halfBoardWidth + 0.6f * ((float)position);
		float spawnY = units [player] [0].getY() + ((float)Math.Sin (units [player] [0].getRotato())) * 0.5f;
		Unit newUnit;
		//add unit to list
		if (type < 2 || type > 5) {
			//wrong type
			return;
		}
		else
		{
			//not enough resources
			if(resources[player] < unitData[type].getCost())
			{
				return;
			}
			newUnit = createUnit (type, player, spawnX, spawnY, units [player] [0].getRotato (), ids [player]);
		}

		int cIndex = units [player].Count;
		if (cIndex != unitGOs [player].Count) {
			// wtf different number of objects
			return;
		}
		units [player].Add (newUnit);

		//update resources, increment unit id
		resources [player] -= unitData [type].getCost ();
		ids [player] ++;



		//add unit gameObject
		unitGOs [player].Add (Instantiate (unitObjectPreFabs [type * 2 - 2 + player]) as GameObject);

		unitGOs [player] [cIndex].transform.position = new Vector2 (units [player] [cIndex].getX (), units [player] [cIndex].getY ());
		
		unitGOs [player] [cIndex].transform.eulerAngles = new Vector3 (0.0f, 0.0f, units [player] [cIndex].getRotato () * 180.0f / PI);

	}

	//sets up Unit
	private Unit createUnit(int u, int team, float x, float y, float rotato, int id)
	{
		Unit tUnit = new Unit (unitData [u].getType(), unitData[u].getCost(), unitData[u].getMaxHealth(), unitData [u].getMaxHealth(), unitData [u].getMoveSpeed(), 
		               		   unitData [u].getAttackRange() , unitData [u].getAttackDamage(), team, x, y, rotato, id, 
		                       unitData[u].getIsGround(), unitData[u].getCanAttackGround(), unitData[u].getCanAttackAir(), 
		                       unitData[u].getIsAOE(), unitData[u].getAoeRadius());
		return tUnit;

	}



	//initialize data for units
	private void initUnitData()
	{
		// base
		unitData [1] = new Unit (1, 100000, 150, 150, 0.0f, 0.0f, 0, 0, 0.0f, 0.0f, 0.0f, 0, true, false, false, false, 0.0f);

		//zergling
		unitData [2] = new Unit (2, 40, 40, 40, 0.3f, 0.6f, 10, 0, 0.0f, 0.0f, 0.0f, 0, true, true, false, false, 0.0f);

		// marine
		unitData [3] = new Unit (3, 80, 97, 97, 0.10f, 1.4f, 5, 0, 0.0f, 0.0f, 0.0f, 0, true, true, true, false, 0.0f);

		//the flying triangle
		unitData [4] = new Unit (4, 80, 35, 35, 0.2f, 1.0f, 12, 0, 0.0f, 0.0f, 0.0f, 0, false, true, true, false, 0.0f);

		// firebat
		unitData [5] = new Unit (5, 100, 60, 60, 0.15f, 1.4f, 5, 0, 0.0f, 0.0f, 0.0f, 0, true, true, false, true, 0.7f);

	}

	//finds index of unit with ID in units List
	private int idFind(int player, int id)
	{
		int cIndex = -1;
		for (int i=0; i<units[player].Count; i++) {
			if(units[player][i].getID() == id)
			{
				cIndex = i;
				break;
			}
		}
		return cIndex;
	}

	private int getEnemy(int player)
	{
		if (player == 0) {
			return 1;
		}
		return 0;
	}

	private int minimum(int a, int b)
	{
		if(a<b)
		{
			return a;
		}
		return b;
	}
	private float dist(float x1, float y1, float x2, float y2)
	{
		return ((float)Math.Sqrt(((double) (  (x2-x1)*(x2-x1) + (y2-y1)*(y2-y1)  )   )));
	}
}

