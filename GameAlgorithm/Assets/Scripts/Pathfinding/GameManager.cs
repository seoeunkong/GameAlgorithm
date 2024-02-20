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

    public int mouse_x, mouse_y;
    int premouse_x, premouse_y;

    // Start is called before the first frame update
    void Start()
    {
        _Tilemap.SetTile(new Vector3Int(0, 0, 0), _changeTile);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
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
            
        }
    }
}
