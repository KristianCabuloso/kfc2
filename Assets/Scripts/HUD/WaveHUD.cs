using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class WaveHUD : MonoBehaviour
{
    [SerializeField, Tooltip("{current_wave} = Número da onda atual")]
    string textString = "Onda: {current_wave}";

    TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void RefreshTextString(int wave)
    {
        text.text = textString.Replace("{current_wave}", wave.ToString());
    }
}
