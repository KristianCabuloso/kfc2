using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUD : MonoBehaviour
{
    public Color noTargetColor = Color.white;
    public Color hasTargetColor = Color.red;
    [SerializeField] Image targetImage;

    WeaponController playerWeaponController;
    //Transform playerLookTransform;

    public void Setup(WeaponController playerWeaponController/*, Transform playerLookTransform*/)
    {
        this.playerWeaponController = playerWeaponController;
        //this.playerLookTransform = playerLookTransform;
        //playerLookTransform = playerWeaponController.GetComponentInChildren<CameraLook>().transform;

        targetImage.sprite = playerWeaponController.CurrentWeapon.targetSprite;
    }

    void Update()
    {
        if (!playerWeaponController)
            return;
            
        if (playerWeaponController.CheckForwardTargetHealth())
        {
            targetImage.color = hasTargetColor;
        }
        else
        {
            targetImage.color = noTargetColor;
        }
    }
}
