using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    Vector3 inputPos;
    Vector2 numberOfGrids;
    public float nodeSize;
    Node[,] grid; //노드의 2차원 배열
    public Tilemap _WallTilemap;

    float nodeHalfSize;
    int gridSizeX, gridSizeY;


    void Start()
    {
        nodeHalfSize = nodeSize * 0.5f;

        gridSizeY = Mathf.RoundToInt(Camera.main.orthographicSize);
        gridSizeX = Mathf.RoundToInt(gridSizeY * Camera.main.aspect);

        gridSizeX *= 2;
        gridSizeY *= 2;

        numberOfGrids.y = Mathf.RoundToInt(Mathf.RoundToInt(gridSizeY) / nodeSize);
        numberOfGrids.x = Mathf.RoundToInt(Mathf.RoundToInt(gridSizeX) / nodeSize);


        CreateGrid();
    }

    void CreateGrid()
    {
        GetWall getWall = _WallTilemap.gameObject.GetComponent<GetWall>();
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * numberOfGrids.x / 2 - Vector3.up * numberOfGrids.y / 2;
        
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = bottomLeft + Vector3.right * (x * nodeSize + nodeHalfSize ) + Vector3.up * (y * nodeSize + nodeHalfSize);
                // 벽 리스트에 해당 좌표값이 있는지 확인 
                Pair<float,float> pos = getWall.getVal.Find(a=> a.First == nodePosition.x && a.Second == nodePosition.y);

                if (pos != null) //해당 좌표가 벽인 경우 
                {
                    grid[x, y] = new Node(false, nodePosition, x, y);
                }
                else //이동할 수 있는 거리인 경우 
                    grid[x, y] = new Node(true, nodePosition, x, y);
            }
        }    
    }

    public Vector3 SetPlayer(int x,int y)
    {
        inputPos = new Vector3(x, y,0);
        return inputPos;
    }

    //이웃한 인접한 8개의 이웃노드를 가져오는 함수 
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        //f_cost와 h_cost가 같은 경우를 대비해서 아래 노드가 먼저 우선순위를 가지게끔 구현
        for (int x = -1; x <= 1; ++x) //아래쪽에서 위쪽으로 선택 
        {
            for (int y = -1; y <= 1; ++y) //왼쪽에서 아래쪽으로 선택
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //vector3 좌표값을 grid의 row,col로 변환 
    public Node GetNodeFromPosition(Vector3 position)
    {
        float percentX = (position.x + 0.5f + numberOfGrids.x / 2) / numberOfGrids.x;
        float percentY = (position.y + 0.5f + numberOfGrids.y / 2) / numberOfGrids.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1 ) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1 ) * percentY);
        return grid[x, y];
    }

    public List<Node> path;

    //유니티에서 현재 그리드들을 그려주고
    //그 다음에 길을 찾았을 경우에 최단 경로를 까만색으로 그려주는 역할을 수행 
    void OnDrawGizmos()
    {
       // Gizmos.DrawWireCube(transform.position, new Vector3(numberOfGrids.x  , numberOfGrids.y , 1  ));

        if (grid != null)
        {
            Node playernode = GetNodeFromPosition(inputPos);

            foreach (Node n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.white : Color.red;

                if (playernode == n)
                {
                    Gizmos.color = Color.black;
                }
                else
                {
                    if (path != null && path.Contains(n))
                        Gizmos.color = Color.black;
                }

                Gizmos.DrawCube(n.position, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }

}

