using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //Control do grid
    public List<Sprite> sprites = new List<Sprite>();
    public GameObject tilePrefab;
    public int gridDimension = 8;
    public float distance = 1.0f;
    private GameObject[,] grid;

    //GameManeger
    public GameObject GameOverMenu;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreEndText;

    public int startingMoves = 5;
    private int numMove;

    public int NumMove
    {
        get
        {
            return numMove;
        }
        set
        {
            numMove = value;
            movesText.text = "Moevs: " + numMove;
        }
    }
    public int Score
    {
        get
        {
            return Score;
        }
        set 
        {
            Score = value;
            scoreText.text = "Score: " + Score;
        }
    }

    public static GridManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
        Score = 0;
        numMove = startingMoves;
        GameOverMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    { 
        grid = new GameObject[gridDimension, gridDimension];
        InitGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitGrid()
    {
        Vector3 posOffset = transform.position - new Vector3(gridDimension * distance / 2.0f, gridDimension * distance / 2.0f, 0);

        for (int row = 0; row < gridDimension; row++)
        {
            for (int column = 0; column < gridDimension; column++)
            {
                GameObject newTile = Instantiate(tilePrefab);

                //escolher a peça que vai nesse objeto
                List<Sprite> possibleSprites = new List<Sprite>(sprites);

                //verificação horizontal
                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if(left1 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                //verificação vertical
                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down1 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

                Tile tile = newTile.AddComponent<Tile>();
                tile.position = new Vector2Int(column, row);
                newTile.transform.parent = transform;
                newTile.transform.position = new Vector3(column * distance, row * distance, 0) + posOffset;
                grid[column, row] = newTile;
            }
        }
        
    }

    bool CheckMatches()
    {
        //Semelhante a lista. Calculo mais rápido. Porém, não aceita valores iguais.
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>();
        for (int row = 0; row < gridDimension; row++)
        {
            for (int column = 0; column < gridDimension; column++)
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }
            }

            for (int colum = 0; colum < gridDimension; colum++)
            { 
                SpriteRenderer current = GetSpriteRendererAt(colum, row);
                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(colum, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }

        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }

        Score += matchedTiles.Count;
        return matchedTiles.Count > 0;
    }

    List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for(int i = col + 1; i < gridDimension; i++)
        {
            SpriteRenderer nextCol = GetSpriteRendererAt(i, row);
            if(nextCol.sprite != sprite)
            {
                break;
            }
            result.Add(nextCol);
        }
        return result;
    }

    List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for(int i = row + 1; i < gridDimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if(nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    Sprite GetSpriteAt(int column, int row)
    {
        if(column < 0 || column >= gridDimension || row < 0 || row >= gridDimension)
        {
            return null;
        }

        GameObject tile = grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= gridDimension || row < 0 || row >= gridDimension)
        {
            return null;
        }

        GameObject tile = grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }

    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        GameObject tile1 = grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        bool matches = CheckMatches();

        if(!matches)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
            SoundManeger.Instance.PlaySound(SoundManeger.SoundType.TypeMove);
            
        }
        else
        {
            SoundManeger.Instance.PlaySound(SoundManeger.SoundType.TypePop);
            NumMove--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMove <= 0)
            {
                NumMove = 0;
                GameOver();
            }
        }
    }

    void FillHoles()
    {
        for(int column = 0; column < gridDimension; column++)
        {
            for(int row = 0; row < gridDimension; row++)
            {
                while(GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for(int filler = row; filler < gridDimension - 1; filler++)
                    {
                        next = GetSpriteRendererAt(column, filler + 1);
                        current.sprite = next.sprite;
                        current = next;
                    }
                    next.sprite = sprites[Random.Range(0, sprites.Count)];
                }
            }
        }
    }
    void GameOver()
    {
        scoreEndText.text = "Score: " + scoreEndText.ToString();
        GameOverMenu.SetActive(true);
        SoundManeger.Instance.PlaySound(SoundManeger.SoundType.TypeGameOver);   
    }
}
