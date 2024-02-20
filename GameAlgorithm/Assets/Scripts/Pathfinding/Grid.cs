using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    Vector3 inputPos;
    Vector2 numberOfGrids;
    public float nodeSize;
    Node[,] grid; //����� 2���� �迭
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
                // �� ����Ʈ�� �ش� ��ǥ���� �ִ��� Ȯ�� 
                Pair<float,float> pos = getWall.getVal.Find(a=> a.First == nodePosition.x && a.Second == nodePosition.y);

                if (pos != null) //�ش� ��ǥ�� ���� ��� 
                {
                    grid[x, y] = new Node(false, nodePosition, x, y);
                }
                else //�̵��� �� �ִ� �Ÿ��� ��� 
                    grid[x, y] = new Node(true, nodePosition, x, y);
            }
        }    
    }

    public Vector3 SetPlayer(int x,int y)
    {
        inputPos = new Vector3(x, y,0);
        return inputPos;
    }

    //�̿��� ������ 8���� �̿���带 �������� �Լ� 
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        //f_cost�� h_cost�� ���� ��츦 ����ؼ� �Ʒ� ��尡 ���� �켱������ �����Բ� ����
        for (int x = -1; x <= 1; ++x) //�Ʒ��ʿ��� �������� ���� 
        {
            for (int y = -1; y <= 1; ++y) //���ʿ��� �Ʒ������� ����
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

    //vector3 ��ǥ���� grid�� row,col�� ��ȯ 
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

    //����Ƽ���� ���� �׸������ �׷��ְ�
    //�� ������ ���� ã���� ��쿡 �ִ� ��θ� ������� �׷��ִ� ������ ���� 
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

