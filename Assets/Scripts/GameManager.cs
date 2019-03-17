using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // singleton - only 1 allowed
    public static GameManager instance = null;

    public BoardManager boardScript;

    // for testing
    // 3 = where enemies appear
    private int level = 3;


    // Start is called before the first frame update
    void Awake()
    {
        // make sure only have 1 game manager
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);

        // don't destry between scense
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
