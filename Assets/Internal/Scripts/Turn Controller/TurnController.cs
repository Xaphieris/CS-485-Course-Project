using UnityEngine;

public class TurnController : MonoBehaviour
{
    public bool playerTurn;
    public bool enemyTurn;
    public GameObject pointAndClickController;
    public GameObject enemyController;

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


    }
}
