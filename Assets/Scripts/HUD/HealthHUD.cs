using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Color lowLifeColor = Color.red;
    public Color mediumLifeColor = Color.yellow;
    public Color highLifeColor = Color.green;
    [SerializeField] Image healthImage;
    [SerializeField] TMPro.TextMeshProUGUI healthAmountText;

    RectTransform healthImageTransform;

    Health playerHealth;

    void Start()
    {
        healthImageTransform = healthImage.GetComponent<RectTransform>();
    }

    public void Setup(Health playerHealth)
    {
        print(">>>>> ATRIBUIU NA HEALTHHUD:" + playerHealth.name);
        this.playerHealth = playerHealth;

        playerHealth.state.AddCallback("PlayerHealth", OnHealthChange);
    }

    void OnHealthChange()
    {
        int healthAmount = playerHealth.state.PlayerHealth;
        float healthPercentage = (float)healthAmount / playerHealth.MaxHealth;
        print(">>>HEALTH_PERCENTAGE => " + healthPercentage.ToString());

        Vector3 _scale = healthImageTransform.localScale;
        _scale.x = healthPercentage;
        healthImageTransform.localScale = _scale;

        Color _color = highLifeColor;
        if (healthPercentage <= 0.5f)
        {
            _color = lowLifeColor;
        }
        else if (healthPercentage < 0.9f)
        {
            _color = mediumLifeColor;
        }
        healthImage.color = _color;

        healthAmountText.text = healthAmount.ToString();
    }
}
