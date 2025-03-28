using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyProp : MonoBehaviour
{
    public int health;
    public int armor;
    public int moveRange;
    public int attack_1range;
    public int attack_1dmg;
    public GameObject tile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //health = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(health < 1)
        {
            tile.GetComponent<TileProp>().hasEnemyUnit = false;
            tile.GetComponent<TileProp>().unit = null;
            Destroy(this.transform.gameObject);
            Debug.Log("Enemy " + this.transform.name + " destroyed");
        }
    }
}
