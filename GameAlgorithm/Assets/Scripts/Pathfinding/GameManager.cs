using Minifantasy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//-9 ~ 8
//-5 ~ 4

public class GameManager : MonoBehaviour
{
    public Tilemap _Tilemap;
    public TileBase _changeTile; //Ŭ���ϸ� ��Ÿ�� Ÿ�� 
    public TileBase _defaultTile; //�⺻ Ÿ�� 

    public GameObject _orc;

    int mouse_x, mouse_y;
    int premouse_x, premouse_y;

    Transform start;
    public Transform _destination;
    bool activate = false;

    private Vector3 cacheStart, cacheDest;
    private Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        start = transform;
    }

    void Start()
    {
        _Tilemap.SetTile(new Vector3Int(0, 0, 0), _changeTile);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        if(start != null && activate)
        {
            Vector3 destPos =  new Vector3(Mathf.FloorToInt(_orc.transform.position.x), Mathf.FloorToInt(_orc.transform.position.y), 0);

            if (start.position != cacheStart || destPos != cacheDest)
            {
                FindPath(start.position, destPos);

                cacheStart = start.position;
                cacheDest = destPos;
            }
        }
       
    }

    void PlayerInput()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouse_x = Mathf.FloorToInt(mousePos.x);
        mouse_y = Mathf.FloorToInt(mousePos.y);


        if (Input.GetMouseButtonDown(0))
        {
            if (_Tilemap.GetTile(new Vector3Int(premouse_x, premouse_y, 0)).Equals(_changeTile))
            {
                _Tilemap.SetTile(new Vector3Int(premouse_x, premouse_y, 0), _defaultTile);
            }


            _Tilemap.SetTile(new Vector3Int(mouse_x, mouse_y, 0), _changeTile);

            //���� Ŭ���� ���콺 ��ġ ���� 
            premouse_x = mouse_x;
            premouse_y = mouse_y;

            //mouse ��ǥ�� ����
            this.gameObject.GetComponent<Grid>().SetPlayer(mouse_x, mouse_y);
            _orc.GetComponentInChildren<SetAnimatorParameter>().ToggleDirection(new Vector3Int(mouse_x, mouse_y, 0));
            
            start.position = new Vector3(mouse_x, mouse_y, 0);
            activate = true;
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromPosition(startPos);
        Node targetNode = grid.GetNodeFromPosition(targetPos);

        //���¼¿� ����Ʈ �ڷᱸ���� ������ ������ ��带 �߰��ϱ� ���� ���������� Ž���ϱ� ���� �Ϸ���...
        List<Node> openSet = new List<Node>();
        //Ŭ�����¿� �ִ� ���� ���� Ž���� ����� �ʿ� ���� ������ ��常 ���� ã�� ���� -> hashset ��� 
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            #region ���� ���� ���� ���� ��带 �����Ѵ�.
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            #endregion

            #region ���� ���� ���� ���� ��尡 �������� Ž���� �����Ѵ�.
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            #endregion

            #region ���� ��带 ���� �¿��� ���� Ŭ����� ������ �̵��Ѵ�.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            #endregion

            #region �̿���带 �����ͼ� ���� ����� �� ���� �¿� �߰��Ѵ�.
            foreach (Node n in grid.GetNeighbours(currentNode))
            {
                if (!n.isWalkable || closedSet.Contains(n))
                {
                    continue;
                }

                int g = currentNode.gCost + GetDistance(currentNode, n);
                int h = GetDistance(n, targetNode);
                int f = g + h;

                // ���� �¿� �̹� �ߺ� ��尡 �ִ� ��� ���� ���� ������ �����Ѵ�.
                if (!openSet.Contains(n))
                {
                    n.gCost = g;
                    n.hCost = h;
                    n.parent = currentNode;
                    openSet.Add(n);
                }
                else
                {
                    if (n.fCost > f)
                    {
                        n.gCost = g;
                        n.parent = currentNode;
                    }
                }
            }
            #endregion
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        //���� ��忡�� ���� ������
        //�ִܰŸ� ��ã�⿡ �ʿ��� ��� ������ �迭�� ����� ����.
        path.Reverse();
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) //�밢�� - 14�� ����ġ. �Ϲ� - 10�� ����ġ 
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
