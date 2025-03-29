using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    public bool playerTurn;
    public bool enemyTurn;

    public int turnNumber = 1;
    public GameObject pointAndClickController;
    public GameObject enemyController;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI enemiesText;
    public GameObject enemyUnitContainer;
    public GameObject playerUnitContainer;
    public int numberOfEnemies;
    public int numberOfPlayerUnits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(pointAndClickController == null)
        {
            pointAndClickController = GameObject.Find("PointAndClickController");
        }
        
        if(enemyController == null)
        {
            enemyController = GameObject.Find("EnemyUnitController");
        }
        

        playerTurn = true;
        enemyTurn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTurn)
        {
            pointAndClickController.SetActive(true);
        }

        if(enemyTurn)
        {
            pointAndClickController.SetActive(false);
        }
        UpdateTurn();

        GetNumberOfEnemies();
        GetNumberOfPlayerUnits();
    }

    public void EndPlayerTurn()
    {

        Debug.Log("Player Turn Ended");
        enemyController.SetActive(true);
        enemyController.GetComponent<EnemyUnitController>().StartTurn();

        playerTurn = false;
        enemyTurn = true;
    }

    public void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn Ended");
        enemyController.SetActive(false);
        playerTurn = true;
        enemyTurn = false;

        turnNumber++;
        pointAndClickController.GetComponent<PointAndClickController>().playerAttacks = 1;
        pointAndClickController.GetComponent<PointAndClickController>().playerMoves = 1;
    }

    public void UpdateTurn()
    {
        turnText.text = ("Turn: " + turnNumber);
    }

    public void GetNumberOfEnemies()
    {
        numberOfEnemies = 0;
        int children;
        
        children = enemyUnitContainer.transform.childCount;
        //Debug.Log("Number of children: " + children);

        for(int i = 0; i < children; i++)
        {
            if(enemyUnitContainer.transform.GetChild(i).gameObject.activeSelf)
            {
                numberOfEnemies++;
            }
        }

        enemiesText.text = "Enemies: " + numberOfEnemies;

        if(numberOfEnemies == 0 && turnNumber > 1)
        {
            endGameWin();
        }
    }

    public void GetNumberOfPlayerUnits()
    {
        numberOfPlayerUnits = 0;
        int children;
        
        children = playerUnitContainer.transform.childCount;
        //Debug.Log("Number of children: " + children);

        for(int i = 0; i < children; i++)
        {
            if(playerUnitContainer.transform.GetChild(i).gameObject.activeSelf)
            {
                numberOfPlayerUnits++;
            }
        }

        if(numberOfPlayerUnits == 0 && turnNumber > 1)
        {
            endGameLoss();
        }
    }

    private void endGameWin()
    {
        SceneManager.LoadScene("GameWin");
    }

    private void endGameLoss()
    {
        SceneManager.LoadScene("GameOver");
    }
}
