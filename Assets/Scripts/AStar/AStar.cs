using System.Collections;
using System.Collections.Generic;
using MFarm.Map;
using Unity.VisualScripting;
using UnityEngine;

namespace MFarm.AStar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes gridNodes;
        private Node starNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        private List<Node> openNodeList;//当前选中node周围8个节点
        private HashSet<Node> closedNodeList;//所有被选中的点
        private bool pathFound;

        /// <summary>
        /// 构建路径更新stack每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="starPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStep"></param>
        public void BuildPath(string sceneName, Vector2Int starPos, Vector2Int endPos,Stack<MovementStep> npcMovementStep)
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, starPos, endPos))
            {
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC移动路径
                    UpdatePathOnMovementStepStack(sceneName,npcMovementStep);
                }

            }
        }
        
        
        /// <summary>
        /// 构建网格节点信息,初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="starPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName,Vector2Int starPos,Vector2Int endPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions,
                    out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;
                
                openNodeList= new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }
            //获取起点和终点 gridNodes 坐标为gridNodes数组坐标减去gridOrigin坐标
            starNode = gridNodes.GetGridNode(starPos.x - gridOrigin.x, starPos.y - gridOrigin.y);
            targetNode=gridNodes.GetGridNode(endPos.x - gridOrigin.x, endPos.y - gridOrigin.y);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var key = (x + originX) + "x" + (y + originY) + "y" + sceneName;
                    Vector3Int tilePos = new Vector3Int(x+originX, y+originY, 0);
                    //下一行代码需要修改
                    TileDetails tile=GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);
                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);
                        if (tile.isNPCObstacle == true)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 找到最短路径所有node 添加到closeNodeList中
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起点
            openNodeList.Add(starNode);

            while (openNodeList.Count>0)
            {
                //节点排序，node内包括比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0];

                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                //计算周围八个点补充到openNodeList中
                EvaluateNeighbourNodes(closeNode);
                
            }

            return pathFound;

        }

        /// <summary>
        /// 评估周围八个点并生成对应消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos=currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x+x, currentNodePos.y+y);

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //链接父节点
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 获得有效NODE 非障碍 非已选择
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
            {
                return null;
            }

            Node neighbourNode = gridNodes.GetGridNode(x, y);
            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
            {
                return null;
            }
            else
            {
                return neighbourNode;
            }
        }
        
        /// <summary>
        /// 返回两点距离值
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            else
            {
                return 14 * xDistance + 10 * (yDistance - xDistance);
            }
        }

        /// <summary>
        /// 更新路径每一步坐标和场景名
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName,Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;
            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate =
                    new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
        
    }
}

