using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
	private TurnManager turnManager;
	private List<int> CatchRouteID = new List<int>();
	private Vector3 pos;// for calc.

	public void Awake()
	{
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
	}
	public void Initialize()
	{
		transform.position = Vector3.zero;
		CatchRouteID.Clear();
	}

	public float[] getNextMoveTime()
	{
		float[] ab = {2, 0};
		return ab;
	}

	public void Calculate ()
	{
		CatchRouteID.Clear();

	}

	private void FullSearch()
	{
		Initialize();
		int catchcount = 0;
		float time = 0.0f;
		pos = transform.position;
		int catchresult = 0;

		catchresult = CanCatch(pos.x, pos.y, 0, time);

		if (catchresult == 1)
		{
			catchcount++;
			float nexttime = (Mathf.Abs(pos.x - TurnManager.BallList[0].nextPosition.x) / 2);
			time = time + nexttime;
			pos = TurnManager.BallList[0].nextPosition + (TurnManager.BallList[0].velocity * nexttime);
		}

		catchresult = CanCatch(pos.x, pos.y, 1, time);


		if (catchresult >= 2)
		{
		}

		if (catchresult >= 3)
		{
		}

		if (catchresult >= 4)
		{
		}

	}

	/// <summary>
	/// Determines whether this instance can catch the specified rx ry ballnumber.
	/// 0: false, 1: only opposite, 2: only follow diagonal, 3: can virtical, 4: can against diagonal.
	/// </summary>
	/// <returns><c>true</c> if this instance can catch the specified rx ry ballnumber; otherwise, <c>false</c>.</returns>
	/// <param name="rx">Rx.</param>
	/// <param name="ry">Ry.</param>
	/// <param name="ballnumber">Ballnumber.</param>
	private int CanCatch(float rx, float ry, int ballnumber, float time)
	{
		Ball b = TurnManager.BallList[ballnumber];
		Vector3 bpos = b.nextPosition + (time * b.velocity);
		if (ry != b.nextPosition.y)
		{
			if (rx - bpos.x < b.velocity.x * TurnManager.YRange * (Mathf.Sqrt(2) - 1))
			{
				return 0;
			}
			else if (rx - bpos.x < b.velocity.x * TurnManager.YRange * 1)
			{
				return 2;
			}
			else if (rx - bpos.x < b.velocity.x * TurnManager.YRange * (Mathf.Sqrt(2) + 1))
			{
				return 3;
			}
			else
			{
				return 4;
			}
		}
		else
		{
			if (b.velocity.x >= 1)
			{
				if (rx - bpos.x < 0)
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}
			else if (b.velocity.x <= -1)
			{
				if (rx - bpos.x > 0)
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}
		}
		return 0;
	}
}
