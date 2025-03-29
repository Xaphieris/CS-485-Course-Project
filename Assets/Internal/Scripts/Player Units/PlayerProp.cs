using UnityEngine;

public class PlayerProp : MonoBehaviour
{
    public int health;
    public int baseHealth;
    public int armor;
    public int baseArmor;
    public int moveRange;
    public GameObject tile;

    public int attack_1dmg;
    public int attack_1range;

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
            Destroy(this.gameObject);
        }
    }
}
