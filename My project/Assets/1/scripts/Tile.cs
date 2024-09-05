using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Tile selected;
    private SpriteRenderer renderer;

    public Vector2Int position;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        renderer.color = Color.gray;
    }

    public void Unselect()
    {
        renderer.color = Color.white;
    }

    public void OnMouseDown()
    {
        if(selected != null)
        {
            if(selected == this)
            {
                return;
            }
            selected.Unselect();
            if(Vector2Int.Distance(selected.position, position) == 1)
            {
                GridManager.instance.SwapTiles(selected.position, position);
                selected = null;
            }
            else
            {
                selected = this;
                Select();
            }
        }
        else
        {
            selected = this;
            Select();
        }

    }

}
