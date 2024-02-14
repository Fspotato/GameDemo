using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    [SerializeField] GameObject hpBar;
    [SerializeField] Text hpText;

    public void ShowHpBar(float hp, float maxHp)
    {
        if (hp == 0 || maxHp == 0) return;

        hpText.text = "Hp: " + ((int)hp).ToString() + "/" + ((int)maxHp).ToString();
        hpBar.GetComponent<Slider>().value = hp / maxHp;
    }
}
