using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReviveHUD : MonoBehaviour
{
    [SerializeField] RectTransform reviveBarRectTransform;
    [SerializeField] RectTransform dieBarRectTransform;

    /*RectTransform reviveBarTransform;
    RectTransform dieBarTransform;*/

    PlayerReviveController reviveController;

    void Start()
    {
        Clear();
    }

    public void Setup(PlayerReviveController reviveController)
    {
        this.reviveController = reviveController;
    }

    void Update()
    {
        if (!reviveController || reviveController.State == ReviveState.None)
            return;

        Vector3 _scale;

        switch (reviveController.State)
        {
            case ReviveState.Dying:
                _scale = dieBarRectTransform.localScale;
                _scale.x = reviveController.Count / reviveController.timeToDie;
                dieBarRectTransform.localScale = _scale;
                break;

            case ReviveState.Reviving:
                _scale = reviveBarRectTransform.localScale;
                _scale.x = 1f - (reviveController.Count / reviveController.timeToRevive);
                reviveBarRectTransform.localScale = _scale;
                break;
        }
    }

    public void Clear()
    {
        Vector3 dieScale = dieBarRectTransform.localScale;
        dieScale.x = 0f;
        dieBarRectTransform.localScale = dieScale;

        Vector3 reviveScale = reviveBarRectTransform.localScale;
        reviveScale.x = 0f;
        reviveBarRectTransform.localScale = reviveScale;
    }
}
