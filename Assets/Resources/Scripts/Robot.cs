﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour
{
    private TurnManager turnManager;
    private List<List<float>> AlgsSolutionList = new List<List<float>>();
    private List<List<float>> HumansSolutionList = new List<List<float>>();

    public Vector3 nextPosition = Vector3.zero;

    public void Awake()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }
    public void Initialize()
    {
        transform.position = Vector3.zero;
        AlgsSolutionList = InitializeTwoDList(AlgsSolutionList);
        HumansSolutionList = InitializeTwoDList(HumansSolutionList);
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

    public IEnumerator Move(Vector3 nextpos, float a, float b, float dir)
    {
        int d = Mathf.RoundToInt(dir);
        float t = 0.0f;
        switch (d)
        {
            case 0: break;
            case 1:
                t = Mathf.Sqrt(2) * TurnManager.YRange;
                if (nextPosition.y < nextpos.y) { nextPosition = nextPosition + ((Vector3.right + Vector3.up) * TurnManager.YRange); }
                else { nextPosition = nextPosition + ((Vector3.right + Vector3.down) * TurnManager.YRange); }
                break;
            case 2:
                t = 1 * TurnManager.YRange;
                if (nextPosition.y < nextpos.y) { nextPosition = nextPosition + (Vector3.up * TurnManager.YRange); }
                else { nextPosition = nextPosition + (Vector3.down * TurnManager.YRange); }
                break;
            case 3:
                t = Mathf.Sqrt(2) * TurnManager.YRange;
                if (nextPosition.y < nextpos.y) { nextPosition = nextPosition + ((Vector3.left + Vector3.up) * TurnManager.YRange); }
                else { nextPosition = nextPosition + ((Vector3.left + Vector3.down) * TurnManager.YRange); }
                break;
            default: break;
        }

        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", nextPosition,
            "time", t,
            "easetype", "linear"
        ));
        yield return new WaitForSeconds(t);

        t = a + (b * Mathf.Sqrt(2)) - t;
        nextPosition = nextpos;
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", nextPosition,
            "time", t,
            "easetype", "linear"
        ));

        yield return new WaitForSeconds(t);
        transform.position = nextPosition;

    }

    public float[] GetNextMoveTime(int i)
    {
        float[] ab = new float[2];
        if (i == 0) { ab[0] = AlgsSolutionList[2][i]; }
        else { ab[0] = AlgsSolutionList[2][i] - AlgsSolutionList[2][i - 1]; }
        return ab;
    }
    public float GetNextDir(int i)
    {
        return AlgsSolutionList[1][i];
    }
    public Vector3 GetNextRPos(int i)
    {
        return new Vector3(AlgsSolutionList[3][i], AlgsSolutionList[4][i], 0);
    }
    public int GetCount()
    {
        return AlgsSolutionList[0].Count;
    }

    public void Calculate()
    {
        Initialize();



        //FullSearch();
        //Debug.Log("Count: " + (AlgsSolutionList[0].Count - 1));
        //Debug.Log("order");
        //for (int i = 0; i < SolutionList[0].Count; i++)
        //{
        //    Debug.Log(SolutionList[0][i]);
        //}
        //Debug.Log("each time");
        //for (int i = 0; i < SolutionList[2].Count; i++)
        //{
        //    Debug.Log(SolutionList[2][i]);
        //}
    }
    private void SemiNeighborSearch()
    {


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
                    if (b.velocity.x > 0 && rpos.x - bpos.x >= TurnManager.YRange * 1)
                    {
                        cancatch = true;
                    }
                    else if (b.velocity.x < 0 && rpos.x - bpos.x <= -1 * TurnManager.YRange * 1)
                    {
                        cancatch = true;
                    }
                }
                break;
            case 3:
                if (rpos.y != bpos.y)
                {
                    if (b.velocity.x > 0 && rpos.x - bpos.x >= TurnManager.YRange * (Mathf.Sqrt(2) + 1))
                    {
                        cancatch = true;
                    }
                    else if (b.velocity.x < 0 && rpos.x - bpos.x <= -1 * TurnManager.YRange * (Mathf.Sqrt(2) - 1))
                    {
                        cancatch = true;
                    }
                }
                break;
            default:
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
    private List<List<float>> catchCalc(Vector3 rpos, int ballid, float time, int dir, List<List<float>> calclist)
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

        calclist[0].Add(b.identification);
        calclist[1].Add(dir);
        calclist[2].Add(time);
        calclist[3].Add(rpos.x);
        calclist[4].Add(rpos.y);

        return calclist;
    }

    private void RouteEnd(List<List<float>> calclist)
    {
        if (AlgsSolutionList[0].Count < calclist[0].Count)
        {
            AlgsSolutionList.Clear();
            AlgsSolutionList.Add(calclist[0]);
            AlgsSolutionList.Add(calclist[1]);
            AlgsSolutionList.Add(calclist[2]);
            AlgsSolutionList.Add(calclist[3]);
            AlgsSolutionList.Add(calclist[4]);
            //Debug.Log("time: ");
            //for (int i = 0; i < SolutionList[2].Count; i++)
            //{
            //    Debug.Log(SolutionList[2][i]);
            //}
        }
    }

    /*
    /// <summary>
    /// Need Initialize if you want to use.
    /// </summary>
    private List<List<float>> FullSearchList = new List<List<float>>();

    /// <summary>
    /// This is full search algorithm.
    /// Do not use this.
    /// </summary>
    private void FullSearch()
    {
        int n = TurnManager.BallList.Count;
        int[] dirArray = new int[n + 1];
        int[] ballArray = new int[n + 1];

        int orderb = (int)Mathf.Pow(n, n);
        int orderd = (int)Mathf.Pow(4, n);

        // Calculate all order.
        for (int k = 0; k < orderb; k++)
        {

            // Calculate all direction.
            for (int j = 0; j < orderd; j++)
            {
                singleSearch(n, ballArray, dirArray);

                FullSearchList = InitializeTwoDList(FullSearchList);
                dirArray[0]++;
                dirArray = carryCheck(dirArray, n, 4, 0);
            }
            dirArray = ClearArray(dirArray);
            // Direction loop end.
            //if (Mathf.RoundToInt((k / Mathf.Pow(n, n)) * 100)%10 == 0)
            //{
            //    Debug.Log(Mathf.RoundToInt((k / Mathf.Pow(n, n)) * 100) + "%...");
            //}
            ballArray[0]++;
            ballArray = carryCheck(ballArray, n, n, 0);
        }
        // Order loop end.

    }

    private void singleSearch(int n, int[] barray, int[] darray)
    {
        bool endflag = false;

        // One search loop.
        for (int i = 0; i < n; i++)
        {
            if (endflag) { continue; }
            else
            {
                if (FullSearchList[0].Contains(barray[i]))
                {
                    endflag = true; continue;
                }
                else
                {
                    if (canCatch(new Vector3(FullSearchList[3][i], FullSearchList[4][i], 0), barray[i], FullSearchList[2][i], darray[i]))
                    {
                        FullSearchList = catchCalc(
                            new Vector3(FullSearchList[3][i], FullSearchList[4][i], 0),
                            barray[i],
                            FullSearchList[2][i],
                            darray[i],
                            FullSearchList);
                    }
                    else { endflag = true; continue; }
                }
            }
        }
        RouteEnd(FullSearchList);
        // One search loop end.
    }

    private int[] carryCheck(int[] array, int size, int upper, int pointa)
    {
        if (array[pointa] >= upper)
        {
            array[pointa] = 0;
            pointa++;
            array[pointa]++;
            return carryCheck(array, size, upper, pointa);
        }
        else
        {
            return array;
        }
    }
    
     */

}
