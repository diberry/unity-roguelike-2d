using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{

   [Serializable]
   public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // game board is columns x rows
    public int columns = 8;
    public int rows = 8;

    // random range - min and max items
    public Count wallCount = new Count(5, 9); 
    public Count foodCount = new Count(1,5);

    //spawn prefabs
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // no Z axis because it is a 2D game
    float NO_Z_AZIS_FOR_2D = 0f;
    UnityEngine.Quaternion NO_ROTATION = Quaternion.identity;

    // transform to keep hierarchy clean - child them to boardHolder
    private Transform boardHolder;

    // track all positions on game board
    private List<Vector3> gridPositions = new List<Vector3>();

    void IntitialiseList()
    {
        gridPositions.Clear();

        // starting at 1 to leave outer layer around grid
        // so that level is solvable
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y=1;y<rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, NO_Z_AZIS_FOR_2D));
            }
        }
    }

    // outerwall and floor
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // starting with -1 to build edge around gameboard
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // if in outer wall, choose outerwall
                if(x== -1 || x==columns || y==-1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, NO_Z_AZIS_FOR_2D), NO_ROTATION) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }


    Vector3 RandomPosition()
    {
        // get random location
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];

        // once used, remove from list - no duplicates
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0;i<objectCount;i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, NO_ROTATION);
        }
    }


    /// <summary>
    /// Called by Game Manager
    /// Only public method
    /// </summary>
    /// <param name="level"></param>
    public void SetupScene(int level)
    {
        BoardSetup();
        IntitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // enemies add logarithmically so difficulty with increase by log for each level
        int enemyCount = (int)Mathf.Log(level, 2f);

        // put in random location but only certain amount - not range of amounts 
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // exit always in top right corner
        // if resize gameboard, exit still in correct place
        Instantiate(exit, new Vector3(columns-1,rows-1, NO_Z_AZIS_FOR_2D), NO_ROTATION);
    }

}
