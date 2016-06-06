using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
    private TurnManager turnManager;
    private List<List<float>> SolutionList = new List<List<float>>();
    private List<List<float>> FullSearchList = new List<List<float>>();

    public Vector3 nextPosition = Vector3.zero;

    public void Awake()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }
    public void Initialize()
    {
        transform.position = Vector3.zero;
        SolutionList = InitializeTwoDList(SolutionList);
        FullSearchList = InitializeTwoDList(FullSearchList);
    }

    private List<List<float>> InitializeTwoDList(List<List<float>> twodlist)
    {
        twodlist.Clear();
        twodlist.Add(new List<float>());
        twodlist.Add(new List<float>());
        twodlist.Add(new List<float>());
        twodlist.Add(new List<float>());
        twodlist.Add(new List<float>());
        twodlist[0].Add(-1);
        twodlist[1].Add(0.0f);
        twodlist[2].Add(0.0f);
        twodlist[3].Add(0.0f);
        twodlist[4].Add(0.0f);
        return twodlist;
    }
    private int[] ClearArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++) { array[i] = 0; }
        return array;
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
        float[] ab = {SolutionList[2][i], 0};
        return ab;
    }
    public Vector3 getNextRPos(int i)
    {
        return new Vector3(SolutionList[3][i], SolutionList[4][i], 0);
    }

    public void Calculate ()
    {
        Initialize();
        FullSearch();
        Debug.Log("Count: " + SolutionList[0].Count);
//        Debug.Log("order id");
//        for (int i = 0; i < CatchOrder[0].Count; i++)
//        {
//            Debug.Log(CatchOrder[0][i]);
//        }
        Debug.Log("each time");
        for (int i = 0; i < SolutionList[2].Count; i++)
        {
            Debug.Log(SolutionList[2][i]);
        }
    }

    private void FullSearch()
    {
        int n = TurnManager.BallList.Count;
        int[] dirArray = new int[n];
        int[] ballArray = new int[n];

        bool test = canCatch(new Vector3(FullSearchList[3][0], FullSearchList[4][0], 0), ballArray[0], FullSearchList[2][0], dirArray[0]);
        Debug.Log("testflag:" + test);

        bool endflag = false;

        // Calculate all order.
        for (int k = 0; k < n * n; k++)
        {
            // Calculate all direction.
            for (int j = 0; j < n * 4; j++)
            {
                // One search loop.
                for (int i = 0; i < n; i++)
                {
                    if (endflag) { }
                    else
                    {
                        if (FullSearchList[0].Contains(ballArray[i]))
                        {
                            endflag = true;
                        }
                        else
                        {
                            if (canCatch(new Vector3(FullSearchList[3][i], FullSearchList[4][i], 0), ballArray[i], FullSearchList[2][i], dirArray[i]))
                            {
                                List<float> catchedlist = catchCalc(
                                    new Vector3(FullSearchList[3][i], FullSearchList[4][i], 0),
                                    ballArray[i],
                                    FullSearchList[2][i],
                                    dirArray[i]);
                                Debug.Log(catchedlist[0]);
                                Debug.Log(catchedlist[1]);
                                Debug.Log(catchedlist[2]);
                                Debug.Log(catchedlist[3]);
                                Debug.Log(catchedlist[4]);

                                FullSearchList[0].Add(catchedlist[0]);
                                FullSearchList[1].Add(catchedlist[1]);
                                FullSearchList[2].Add(catchedlist[2]);
                                FullSearchList[3].Add(catchedlist[3]);
                                FullSearchList[4].Add(catchedlist[4]);
                            }
                            else { endflag = true; }
                        }
                    }
                }
                RouteEnd(FullSearchList);
                FullSearchList = InitializeTwoDList(FullSearchList);
                endflag = false;
                // One search loop end.

                dirArray[n - 1]++;
                dirArray = carryCheck(dirArray, n, 4, 0);
            }
            dirArray = ClearArray(dirArray);
            // Direction loop end.

            ballArray[n - 1]++;
            ballArray = carryCheck(ballArray, n, n, 0);
        }
        // Order loop end.

    }

    private int[] carryCheck(int[] array, int size, int upper, int pointa)
    {
        if (array[size - 1 - pointa] >= upper)
        {
            array[size - 1 - pointa] -= upper;
            pointa++;
            array[size - 1 - pointa]++;
            return carryCheck(array, size, upper, pointa);
        }
        else
        {
            return array;
        }
    }

    private bool canCatch(Vector3 rpos, int ballid, float time, int dir)
    {
        Ball b = TurnManager.BallList[ballid];
        Vector3 bpos = b.nextPosition + (time * b.velocity);
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

        return cancatch;

    }

    /// <summary>
    /// Fulls the search part.
    /// dir:
    /// 0: horizontal, 1: rightchange, 2: virtical, 3: leftchange.
    /// </summary>
    /// <returns>The search part.</returns>
    /// <param name="rpos">Rpos.</param>
    /// <param name="ballid">Ballid.</param>
    /// <param name="time">Time.</param>
    /// <param name="dir">Dir.</param>
    /// <param name="calclist">Calclist.</param>
    private List<float> catchCalc(Vector3 rpos, int ballid, float time, int dir)
    {
        Ball b = TurnManager.BallList[ballid];
        Vector3 bpos = b.nextPosition + (time * b.velocity);
        float usedtime = 0.0f;

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
            default:
                break;
        }
        usedtime = usedtime + (Mathf.Abs(rpos.x - bpos.x) / 2);
        time = time + usedtime;// need
        bpos = bpos + (b.velocity * (Mathf.Abs(rpos.x - bpos.x) / 2));
        rpos = bpos;// need

        List<float> calclist = new List<float>();
        calclist.Add(b.identification);
        calclist.Add(dir);
        calclist.Add(time);
        calclist.Add(rpos.x);
        calclist.Add(rpos.y);

        return calclist;
    }

    private void RouteEnd(List<List<float>> calclist)
    {
        if (SolutionList[0].Count < calclist[0].Count)
        {
            SolutionList.Clear();
            SolutionList.Add(calclist[0]);
            SolutionList.Add(calclist[1]);
            SolutionList.Add(calclist[2]);
            SolutionList.Add(calclist[3]);
            SolutionList.Add(calclist[4]);
            Debug.Log("time: ");
            for (int i = 0; i < SolutionList[2].Count; i++)
            {
                Debug.Log(SolutionList[2][i]);
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
    private void FullSearchByRecursion(Vector3 rpos, int ballid, float time, int dir, List<List<float>> delivlist)
    {
        if (delivlist.Count == 0)
        {
            delivlist.Add(new List<float>());
            delivlist.Add(new List<float>());
            delivlist.Add(new List<float>());
            delivlist.Add(new List<float>());
            delivlist.Add(new List<float>());
        }

        if (delivlist[0].Contains(ballid))
        {
            // end of recursion.
            FullSearchEndByRecursion(delivlist);
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


                List<List<float>> calclist = new List<List<float>>();
                calclist.Add(delivlist[0]);
                calclist.Add(delivlist[1]);
                calclist.Add(delivlist[2]);
                calclist.Add(delivlist[3]);
                calclist.Add(delivlist[4]);

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
                FullSearchEndByRecursion(delivlist);
            }
        }

    }

    private void FullSearchEndByRecursion(List<List<float>> calclist)
    {
        if (SolutionList[0].Count < calclist[0].Count)
        {
            SolutionList.Clear();
            SolutionList.Add(calclist[0]);
            SolutionList.Add(calclist[1]);
            SolutionList.Add(calclist[2]);
            SolutionList.Add(calclist[3]);
            SolutionList.Add(calclist[4]);
            Debug.Log("time: ");
            for (int i = 0; i < SolutionList[2].Count; i++)
            {
                Debug.Log(SolutionList[2][i]);
            }
        }
    }

}
