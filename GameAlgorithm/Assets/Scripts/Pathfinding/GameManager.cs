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
    public TileBase _changeTile; //클릭하면 나타날 타일 
    public TileBase _defaultTile; //기본 타일 

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

            //전에 클릭한 마우스 위치 저장 
            premouse_x = mouse_x;
            premouse_y = mouse_y;

            //mouse 좌표값 전달
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

        //오픈셋에 리스트 자료구조를 지정한 이유는 노드를 추가하기 쉽고 순차적으로 탐색하기 쉽게 하려고...
        List<Node> openSet = new List<Node>();
        //클로즈드셋에 있는 노드는 순차 탐색을 사용할 필요 없이 지정한 노드만 빨리 찾기 위함 -> hashset 사용 
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            #region 가장 낮은 값을 가진 노드를 선택한다.
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            #endregion

            #region 가장 낮은 값을 가진 노드가 종착노드면 탐색을 종료한다.
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            #endregion

            #region 현재 노드를 오픈 셋에서 빼서 클로즈드 셋으로 이동한다.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            #endregion

            #region 이웃노드를 가져와서 값을 계산한 후 오픈 셋에 추가한다.
            foreach (Node n in grid.GetNeighbours(currentNode))
            {
                if (!n.isWalkable || closedSet.Contains(n))
                {
                    continue;
                }

                int g = currentNode.gCost + GetDistance(currentNode, n);
                int h = GetDistance(n, targetNode);
                int f = g + h;

                // 오픈 셋에 이미 중복 노드가 있는 경우 값이 작은 쪽으로 변경한다.
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

        //시작 노드에서 종착 노드까지
        //최단거리 길찾기에 필요한 모든 노드들의 배열을 만들어 낸다.
        path.Reverse();
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) //대각선 - 14의 가중치. 일반 - 10의 가중치 
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
