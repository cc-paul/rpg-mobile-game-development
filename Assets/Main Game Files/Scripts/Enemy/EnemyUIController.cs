using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image hpFillAmount;

    public void UpdateHealthUI(
        float _currentHP,
        float _maxHP
    ) {
        hpFillAmount.fillAmount = _currentHP / _maxHP;
    }
}
