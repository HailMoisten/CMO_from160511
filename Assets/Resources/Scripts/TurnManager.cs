using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {
    public static List<Ball> BallList = new List<Ball>();
    public static Robot RobotA;
    public static Transform BallContainer;
    public static int XRange;
    public void SetXRange(string str) { XRange = int.Parse(str); }
    public static int YRange;
    public void SetYRange(string str) { YRange = int.Parse(str); }
    public static int Number;
    public void SetNumber(string str) { Number = int.Parse(str); }

    public static Vector3[] velPattern = {Vector3.right, Vector3.left};
    public static bool GetStop = true;
    public void GetStopToggle() { GetStop = !GetStop; }

    private int turn = 0;

    public void Awake()
    {
        Time.timeScale = 4.0f;
        XRange = 64;
        YRange = 4;
        Number = 4;
        RobotA = GameObject.Find("Robot").GetComponent<Robot>();
        BallContainer = GameObject.Find("BallContainer").transform;

    }

    public void Initialize()
    {
        // get
        // get params

        // clear
        turn = 0;
        int id = 0;
        foreach (Transform balls in BallContainer) {
            GameObject.Destroy(balls.gameObject);
        }
        BallList.Clear();
        RobotA.Initialize();

        // set
        for (int i = 0; i < Number; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                GameObject g = (GameObject)Instantiate(Resources.Load("Prefabs/Ball Blue"), Vector3.forward, Quaternion.identity);
                g.transform.SetParent(BallContainer);
                g.GetComponent<Ball>().Initialize(id, -1 * Random.Range(0, XRange),
                    Random.Range(0, 2) * YRange, velPattern[0]);
                BallList.Add(g.GetComponent<Ball>());
            }
            else
            {
                GameObject g = (GameObject)Instantiate(Resources.Load("Prefabs/Ball Red"),  Vector3.forward, Quaternion.identity);
                g.transform.SetParent(BallContainer);
                g.GetComponent<Ball>().Initialize(id,  1 * Random.Range(0, XRange),
                    Random.Range(0, 2) * YRange, velPattern[1]);
                BallList.Add(g.GetComponent<Ball>());
            }
            id++;
        }
    }

    public void Calculate()
    {
        RobotA.Calculate();
    }

    public void NextTurn()
    {
        float[] ab = RobotA.getNextMoveTime(turn);
        RobotA.Move(RobotA.getNextRPos(turn), ab[0], ab[1]);

        for (int i = 0; i < BallList.Count; i++)
        {
            BallList[i].Move(ab[0], ab[1]);
        }
        turn++;
    }

}
