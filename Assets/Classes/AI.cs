using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface AI
{
	void initData(Unit[] unitData);
	List<Command> spawn(int resources);
	List<Command> loop(List<Unit> myUnits, List<Unit> enemyUnits);
}
