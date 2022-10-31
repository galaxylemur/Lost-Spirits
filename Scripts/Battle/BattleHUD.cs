using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] HPBar hpBar;

    public void SetData(Spirit spirit)
    {
        hpBar.SetHP((float)spirit.Hp);
    }
}
