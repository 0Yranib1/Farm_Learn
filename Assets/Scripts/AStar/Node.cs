using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition; //网格坐标
        public int gCost = 0;//到起始点的代价
        public int hCost = 0;//到目标点的代价
        public int FCost=> gCost + hCost;//f=g+h 当前格子值
        public bool isObstacle = false; //当前节点是否是障碍物
        public Node parentNode; //父节点

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }
        public int CompareTo(Node other)
        {
            int resulet = FCost.CompareTo(other.FCost);
            if (resulet == 0)
            {
                return hCost.CompareTo(other.hCost);
            }
            return resulet;
        }
    }
}

