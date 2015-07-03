using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bullet {
	private float cX;
	private float cY;
	private float endX;
	private float endY;
	private float speed;
	private float lastDist = 10000.0f;
	private float vX;
	private float vY;
	private float dir;

	public Bullet(float tStartX, float tStartY, float tEndX, float tEndY, float tSpeed)
	{
		cX = tStartX;
		cY = tStartY;
		endX = tEndX;
		endY = tEndY;
		speed = tSpeed;
		calcVelocity ();
	}
	private void calcVelocity()
	{
		dir = getDirection (cX, cY, endX, endY);
		vX = ((float)Math.Cos (dir)) * speed;
		vY = ((float)Math.Sin (dir)) * speed;
	}

	public void iterate()
	{
		cX += vX;
		cY += vY;
		float newDist = dist (cX, cY, endX, endY);
		if (newDist > lastDist) {
			cX = -10001.0f;
			cY = -10001.0f;
		} else {

			lastDist = newDist;
		}
	}
	public float getX()
	{
		return cX;
	}
	public float getY()
	{
		return cY;
	}
	public float getDir()
	{
		return dir;
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
