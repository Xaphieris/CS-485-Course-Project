using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    public bool playerTurn;
    public bool enemyTurn;

    public int turnNumber = 1;
    public GameObject pointAndClickController;
    public GameObject enemyController;
    public TextMeshProUGUI turnText;

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
}
