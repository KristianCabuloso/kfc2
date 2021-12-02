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
    Transform playerLookTransform;

    public void Setup(WeaponController playerWeaponController)
    {
        this.playerWeaponController = playerWeaponController;
        playerLookTransform = playerWeaponController.GetComponentInChildren<CameraLook>().transform;

        print("==================================");
        print(playerWeaponController);
        print(playerWeaponController.CurrentWeapon);
        print(playerWeaponController.CurrentWeapon.targetSprite);
        targetImage.sprite = playerWeaponController.CurrentWeapon.targetSprite;
    }

    void Update()
    {
        if (!playerWeaponController)
            return;
            
        if (playerWeaponController.GetForwardRaycastHitHealth(playerLookTransform.forward))
        {
            targetImage.color = hasTargetColor;
        }
        else
        {
            targetImage.color = noTargetColor;
        }
    }
}
