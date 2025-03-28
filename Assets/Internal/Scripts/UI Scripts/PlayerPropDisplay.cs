using TMPro;
using UnityEngine;


public class PlayerPropDisplay : MonoBehaviour
{
    public GameObject playerContainer;
    private GameObject playerUnit;
    private PlayerProp playerProp;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ArmorText;
    public TextMeshProUGUI PlayerUnitName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerContainer = GameObject.Find("Player Units");
        // playerUnit = playerContainer.transform.GetChild(1).gameObject;
        // playerProp = playerUnit.GetComponent<PlayerProp>();

        // PlayerUnitName.text = playerUnit.name;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerUnit == null)
        {
            playerUnit = playerContainer.transform.GetChild(1).gameObject;
            playerProp = playerUnit.GetComponent<PlayerProp>();

            PlayerUnitName.text = playerUnit.name;
        }

        HealthText.text = "Health: " + playerProp.health + "/" + playerProp.baseHealth;
        ArmorText.text = "Armor: " + playerProp.armor + "/" + playerProp.baseArmor;
    }
}
