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


        public void BuildPath(string sceneName, Vector2Int starPos, Vector2Int endPos)
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, starPos, endPos))
            {
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC移动路径
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
                
            }

            return pathFound;

        }
        
    }
}

