using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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

            //Escolher a peça que vai nesse objeto
            List<Sprite> possibleSprites = new List<Sprite>(sprites);

            //Verificação horizontal
            Sprite left1 = GetSpriteAt(column -1, row);
            Sprite left2 = GetSpriteAt(column - 2, row);
            if(left1 != null && left1 == left2)
            {
                possibleSprites.Remove(left1);
            }

            //Verificação vertical
            Sprite down1 = GetSpriteAt(column, row -1);
            Sprite down2 = GetSpriteAt(column, row -2);
            if (down1 != null && down1 == down2)
            {
                possibleSprites.Remove(down1);
            }

            SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
            renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

            newTile.transform.parent = transform;
            newTile.transform.position = new Vector3(column * distance, row * distance, 0) + posOffset;
            grid[column, row] = newTile;


        }
    }
}

Sprite GetSpriteAt(int column, int row)
{   
    //Se o espaço for menor que 0 ele não ira puxar sprite, então irá ficar vazio
    if(column < 0 || column >= gridDimension || row < 0 || row >= gridDimension)
    {
        return null;
    }

    GameObject tile = grid[column, row];
    SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
    return renderer.sprite;

}
}
