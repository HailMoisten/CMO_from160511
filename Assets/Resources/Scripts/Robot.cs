using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
    private TurnManager turnManager;
    private List<List<float>> CatchOrder = new List<List<float>>();
    public Vector3 nextPosition = Vector3.zero;

    public void Awake()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }
    public void Initialize()
    {
        transform.position = Vector3.zero;
        CatchOrder.Clear();
        CatchOrder.Add(new List<float>());
        CatchOrder.Add(new List<float>());
        CatchOrder.Add(new List<float>());
        CatchOrder.Add(new List<float>());
        CatchOrder.Add(new List<float>());
    }

    public void Move(Vector3 nextpos, float a, float b)
    {
        float t = a + (b * Mathf.Sqrt (2));
        nextPosition = nextpos;
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", nextPosition,
            "time", t,
            "easetype", "linear"
        ));
        EndFix(a, b);
    }

    private IEnumerator EndFix(float a, float b){
        float time = a + (b * Mathf.Sqrt (2));
        yield return new WaitForSeconds(time);
        transform.position = nextPosition;
    }

    public float[] getNextMoveTime(int i)
    {
        float[] ab = {CatchOrder[2][i], 0};
        return ab;
    }
    public Vector3 getNextRPos(int i)
    {
        return new Vector3(CatchOrder[3][i], CatchOrder[4][i], 0);
    }

    public void Calculate ()
    {
        Initialize();
        FullSearch();
        Debug.Log("Count: " + CatchOrder[0].Count);
//        Debug.Log("order id");
//        for (int i = 0; i < CatchOrder[0].Count; i++)
//        {
//            Debug.Log(CatchOrder[0][i]);
//        }
        Debug.Log("each time");
        for (int i = 0; i < CatchOrder[2].Count; i++)
        {
            Debug.Log(CatchOrder[2][i]);
        }
    }

    private void FullSearch()
    {
        List<List<float>> firstlist = new List<List<float>>();
        firstlist.Add(new List<float>());
        firstlist.Add(new List<float>());
        firstlist.Add(new List<float>());
        firstlist.Add(new List<float>());
        firstlist.Add(new List<float>());

        for (int i = 0; i < TurnManager.BallList.Count; i++)
        {
            for (int d = 0; d < 4; d++)
            {
                FullSearchByRecursion(transform.position, i, 0.0f, d, new List<List<float>>());
            }
        }
    }
    /// <summary>
    /// Fulls the search by recursion.
    /// dir:
    /// 0: horizontal, 1: rightchange, 2: virtical, 3: leftchange.
    /// </summary>
    /// <param name="rpos">Rpos.</param>
    /// <param name="ballnumber">Ballnumber.</param>
    /// <param name="time">Time.</param>
    /// <param name="dir">Dir.</param>
    /// <param name="instlist">Instlist.</param>
    private void FullSearchByRecursion(Vector3 rpos, int ballid, float time, int dir, List<List<float>> calclist)
    {
        if (calclist.Count == 0)
        {
            calclist.Add(new List<float>());
            calclist.Add(new List<float>());
            calclist.Add(new List<float>());
            calclist.Add(new List<float>());
            calclist.Add(new List<float>());
        }

        if (calclist[0].Contains(ballid))
        {
            // end of recursion.
            FullSearchEnd(calclist);
            calclist.Clear();
        }
        else
        {
            Ball b = TurnManager.BallList[ballid];
            Vector3 bpos = b.nextPosition + (time * b.velocity);
            float usedtime = 0.0f;
            bool cancatch = false;

            switch (dir)
            {
                case 0:
                    if (rpos.y == bpos.y)
                    {
                        if (b.velocity.x > 0 && (rpos.x - bpos.x) >= 0)
                        {
                            cancatch = true;
                        }
                        else if (b.velocity.x < 0 && (rpos.x - bpos.x) <= 0)
                        {
                            cancatch = true;
                        }
                    }
                    break;
                case 1:
                    if (rpos.y != bpos.y)
                    {
                        if (b.velocity.x > 0 && rpos.x - bpos.x >= TurnManager.YRange * (Mathf.Sqrt(2) - 1))
                        {
                            cancatch = true;
                        }
                        else if (b.velocity.x < 0 && rpos.x - bpos.x <= -1 * TurnManager.YRange * (Mathf.Sqrt(2) + 1))
                        {
                            cancatch = true;
                        }
                    }
                    break;
                case 2:
                    if (rpos.y != bpos.y)
                    {
                        if (b.velocity.x > 0 && (rpos.x - bpos.x) >= TurnManager.YRange * 1)
                        {
                            cancatch = true;
                        }
                        else if (b.velocity.x < 0 && (rpos.x - bpos.x) <= TurnManager.YRange * 1)
                        {
                            cancatch = true;
                        }
                    }
                    break;
                case 3:
                    if (rpos.y != bpos.y)
                    {
                        if (b.velocity.x < 0 && bpos.x - rpos.x >= TurnManager.YRange * (Mathf.Sqrt(2) - 1))
                        {
                            cancatch = true;
                        }
                        else if (b.velocity.x > 0 && rpos.x - bpos.x >= TurnManager.YRange * (Mathf.Sqrt(2) + 1))
                        {
                            cancatch = true;
                        }
                    }
                    break;
                default :
                    break;
            }

            if (cancatch)
            {
                switch (dir)
                {
                    case 0: 
                        break;
                    case 1:
                        usedtime = TurnManager.YRange * (Mathf.Sqrt(2));
                        bpos = bpos + (usedtime * b.velocity);
                        if (bpos.y > rpos.y)
                        {
                            rpos = rpos + ((Vector3.right + Vector3.up) * TurnManager.YRange);
                        }
                        else
                        {
                            rpos = rpos + ((Vector3.right + Vector3.down) * TurnManager.YRange);
                        }
                        break;
                    case 2:
                        usedtime = TurnManager.YRange;
                        bpos = bpos + (usedtime * b.velocity);
                        if (bpos.y > rpos.y)
                        {
                            rpos = rpos + (Vector3.up * TurnManager.YRange);
                        }
                        else
                        {
                            rpos = rpos + (Vector3.down * TurnManager.YRange);
                        }
                        break;
                    case 3:
                        usedtime = TurnManager.YRange * (Mathf.Sqrt(2));
                        bpos = bpos + (usedtime * b.velocity);
                        if (bpos.y > rpos.y)
                        {
                            rpos = rpos + ((Vector3.left + Vector3.up) * TurnManager.YRange);
                        }
                        else
                        {
                            rpos = rpos + ((Vector3.left + Vector3.down) * TurnManager.YRange);
                        }
                        break;
                    default :
                        break;
                }
                usedtime = usedtime + (Mathf.Abs(rpos.x - bpos.x) / 2);
                time = time + usedtime;// need
                bpos = bpos + (b.velocity * (Mathf.Abs(rpos.x - bpos.x) / 2));
                rpos = bpos;// need

                calclist[0].Add(b.identification);
                calclist[1].Add(dir);
                calclist[2].Add(time);
                calclist[3].Add(rpos.x);
                calclist[4].Add(rpos.y);

                // Recursion
                for (int i = 0; i < TurnManager.BallList.Count; i++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        FullSearchByRecursion(rpos, i, time, d, calclist);
                    }
                }
                // Recursion
            }
            else
            {
                // end of recursion.
                FullSearchEnd(calclist);
                calclist.Clear();
            }
        }

    }

    private void FullSearchEnd(List<List<float>> calclist)
    {
        if (CatchOrder[0].Count < calclist[0].Count)
        {
            CatchOrder.Clear();
            CatchOrder.Add(calclist[0]);
            CatchOrder.Add(calclist[1]);
            CatchOrder.Add(calclist[2]);
            CatchOrder.Add(calclist[3]);
            CatchOrder.Add(calclist[4]);
            Debug.Log("time: ");
            for (int i = 0; i < CatchOrder[2].Count; i++)
            {
                Debug.Log(CatchOrder[2][i]);
            }
        }
    }

}
