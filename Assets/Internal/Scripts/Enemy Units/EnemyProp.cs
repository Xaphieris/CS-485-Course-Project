using UnityEngine;

public class EnemyProp : MonoBehaviour
{
    public int health;
    public int armor;
    public int moveRange;
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
            Destroy(this.transform.gameObject);
            Debug.Log("Enemy " + this.transform.name + " destroyed");
        }
    }
}
