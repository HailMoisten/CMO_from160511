using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
    private TurnManager turnManager;
    private List<int> CatchOrderID = new List<int>();
    private Vector3 pos;// for calc.

    public void Awake()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }
    public void Initialize()
    {
        transform.position = Vector3.zero;
        CatchOrderID.Clear();
    }

    public float[] getNextMoveTime()
    {
        float[] ab = {2, 0};
        return ab;
    }

    public void Calculate ()
    {
        CatchOrderID.Clear();

    }

    private void FullSearch()
    {
        Initialize();
        int catchcount = 0;
        float time = 0.0f;
        pos = transform.position;
        bool catchresult = true;

        for (int i = 0; i < TurnManager.BallList.Count; i++)
        {
            for (int d = 0; d < 5; d++)
            {
                if (CanCatch(pos, i, time, d, new List<int>()))
                {
                }
            }
        }

    }

    private void SingleSearch()
    {
        for (int i = 0; i < TurnManager.BallList.Count; i++)
        {
            for (int d = 0; d < 5; d++)
            {
//                if (CanCatch(pos, i, time, d))
            }
        }

    }

    /// <summary>
    /// Determines whether this instance can catch the specified pos ballnumber time dir.
    /// dir:
    /// 0: right, 1: rightchange, 2: change, 3: leftchange, 4: left.
    /// </summary>
    /// <returns><c>true</c> if this instance can catch the specified pos ballnumber time dir; otherwise, <c>false</c>.</returns>
    /// <param name="pos">Position.</param>
    /// <param name="ballnumber">Ballnumber.</param>
    /// <param name="time">Time.</param>
    /// <param name="dir">Dir.</param>
    private bool CanCatch(Vector3 rpos, int ballnumber, float time, int dir, List<int> instlist)
    {
        Ball b = TurnManager.BallList[ballnumber];
        Vector3 bpos = b.nextPosition + (time * b.velocity);
        float usedtime = 0.0f;

        switch (dir)
        {
            case 0:
                if (rpos.y == bpos.y)
                {
                    if (rpos.x - bpos.x > 0 && b.velocity.x < 1)
                    {
                        usedtime = (Mathf.Abs(rpos.x - bpos.x) / 2);
                        time = time + usedtime;// need
                        bpos = bpos + (b.velocity * usedtime);
                        rpos = bpos;// need
                        return true;
                    }
                }
                break;
            case 1:
                if (rpos.x - bpos.x < b.velocity.x * TurnManager.YRange * (Mathf.Sqrt(2) - 1))
                {
                }
                break;
            case 2:
                if (rpos.x - bpos.x < b.velocity.x * TurnManager.YRange * 1)
                {
                }
                break;
            case 3:
                if (rpos.x - bpos.x < b.velocity.x * TurnManager.YRange * (Mathf.Sqrt(2) + 1))
                {
                }
                break;
            case 4:
                if (rpos.y == bpos.y)
                {
                    if (rpos.x - bpos.x < 0 && b.velocity.x > -1)
                    {
                        return true;
                    }
                }
                break;
            default: break;
        }

        return false;
    }

}
