using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Driver : MonoBehaviour
{
    public int[] grid;
    private Grid[] objGrid;
    private int owner = 0;
    private int[] horSum;
    private int[] verSum;
    private int obl01;
    private int obl02;

    private Toggle firstCom;
    private bool isFirst;
    private GameObject resultCom;
    private Text resultTxt;
    private void Init()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = 0;
            ShowObj(i);
        }

        for (int i = 0;i < 3; i++)
        {
            horSum[i] = 0;
            verSum[i] = 0;
        }
        obl01 = 0;
        obl02 = 0;

        resultCom.SetActive(false);
        isFirst = firstCom.isOn;
        owner = isFirst ? 1 : -1;
        if (!isFirst) AIRun();
    }
    void Start()
    {
        grid = new int[9];
        objGrid = new Grid[9];
        for (int i = 0; i < grid.Length; i++)
        {
            int index = i;
            grid[index] = 0;
            objGrid[index] = this.transform.Find("Grid" + (index + 1)).GetComponent<Grid>();
            objGrid[index].transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (owner == 0) return;

                Move(index);
            });
        }
        horSum = new int[3];
        verSum = new int[3];

        transform.parent.Find("BeginBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            Init();
        });

        transform.parent.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.Quit();
        });

        firstCom = transform.parent.Find("FirstBtn").GetComponent<Toggle>();
        resultCom = transform.parent.Find("ResultShow").gameObject;
        resultTxt = resultCom.transform.GetChild(0).GetComponent<Text>();

        Init();
    }

    private void Move(int index)
    {
        if (grid[index] != 0) return;

        grid[index] = owner;
        int horIndex = index / 3;
        int verIndex = index % 3;

        horSum[horIndex] += owner;
        verSum[verIndex] += owner;

        if(horIndex == verIndex)
        {
            obl01 += owner;
        }

        if(horIndex + verIndex == 2)
        {
            obl02 += owner;
        }

        ShowObj(index);
        JudgeWin();

    }
    private void ShowObj(int index)
    {
        objGrid[index].SetFlag(grid[index]);
    }

    private void JudgeWin()
    {
        bool winFlag = false;
        for (int i = 0; i < 3; i++)
        {
            if (math.abs(horSum[i]) == 3 || math.abs(verSum[i]) == 3)
            {
                winFlag = true;

            }
        }

        if (math.abs(obl01) == 3 || math.abs(obl02) == 3) winFlag = true;

        if(winFlag)
        {
            resultCom.SetActive(true);
            string name = owner == 1 ? "你" : "对方";
            resultTxt.text = name + "获得了游戏胜利";
            owner = 0;
        }
        else
        {
            if(IsFull())
            {
                resultCom.SetActive(true);
                resultTxt.text = "平局";
                return;
            }

            owner = -owner;
            if (owner == -1) AIRun(); 
        }
    }

    private bool IsFull()
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[i] == 0) return false;
        }

        return true;
    }

    private int GetScore()
    {
        for (int i = 0; i < 3; i++) 
        {
            if (math.abs(grid[3 * i] + grid[3 * i + 1] + grid[3 * i + 2]) == 3)
            {
                return grid[3 * i];
            }

            if (math.abs(grid[i] + grid[i + 3] + grid[i + 6]) == 3)
            {
                return grid[i];
            }
        }

        if (math.abs(grid[0] + grid[4] + grid[8]) == 3)
        {
            return grid[4];
        }

        if (math.abs(grid[2] + grid[4] + grid[6]) == 3)
        {
            return grid[4];
        }

        return 0;
    }

    private int MaxMin(bool isRobot,ref int depth)
    {
        depth++;
        int score = GetScore();
        if (score < 0) return -1;
        else if (score > 0) return 1;
        else if (IsFull()) return 0;

        int bestValue = isRobot ? int.MaxValue : int.MinValue;
        if(isRobot)
        {     
            for (int i = 0; i < 9; i++)
            {
                if (grid[i] == 0)
                {
                    grid[i] = -1;
                    bestValue = math.min(bestValue, MaxMin(!isRobot,ref depth));
                    grid[i] = 0;
                }
            }
            return bestValue;
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (grid[i] == 0)
                {
                    grid[i] = 1;
                    bestValue = math.max(bestValue, MaxMin(!isRobot,ref depth));
                    grid[i] = 0;
                }
            }
            return bestValue;
        }
    }

    private void AIRun()
    {
        int best = int.MaxValue;
        int move = -1;
        int minDepth = int.MaxValue;
        for (int i = 0; i < 9; i++)
        {
            if (grid[i] == 0)
            {
                grid[i] = -1;
                int depth = 0;
                int value = MaxMin(false,ref depth);
                grid[i] = 0;
                if(value < best)
                {
                    minDepth = depth;
                    best = value;
                    move = i;
                }
                else if(value == best)
                {
                    if (depth < minDepth)
                    {
                        minDepth = depth;
                        best = value;
                        move = i;
                    }
                }
            }
        }

        if(move >= 0)
        {
            Move(move);
        }
    }

    void Update()
    {
        
    }
}
