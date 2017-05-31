using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate{
	private float X;
	private float Y;

	public Coordinate(float tX, float tY)
	{
		X = tX;
		Y = tY;
	}
	public float getX()
	{
		return X;
	}
	public float getY()
	{
		return Y;
	}
}
