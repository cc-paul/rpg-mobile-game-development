using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelInfoDisplay : MonoBehaviour {
    [Header("Player UI Elements")]
    [SerializeField] private TextMeshProUGUI clanNameText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterNameTextDuplicate;
    [SerializeField] private TextMeshProUGUI hpTextMain;
    [SerializeField] private TextMeshProUGUI mpTextMain;
    [SerializeField] private Image clanImage;
    [SerializeField] private Image schoolImage;
    [SerializeField] private Image schoolImageDuplicate;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImageMain;
    [SerializeField] private Image mpImageMain;
    [SerializeField] private GameObject nameAndClanHolder;
    [SerializeField] private GameObject nameHolder;
    [SerializeField] private bool updateMainScreenUI;

    public void DisplayCharacterDetails(
       string _clanName,
       string _characterName,
       bool _hasClan
    ) {
        //TODO : School Image and Clan Image is not working yet

        clanNameText.text = _clanName;
        characterNameText.text = _characterName;
        characterNameTextDuplicate.text = characterNameText.text;
        schoolImageDuplicate.sprite = schoolImage.sprite;

        nameAndClanHolder.gameObject.SetActive(_hasClan);
        nameHolder.gameObject.SetActive(!_hasClan);
    }

    public void DisplayLifeStats(
        float _currentHP,
        float _maxHP,
        float _currentMP,
        float _maxMP
    ) {
        int convertedHP = Mathf.Max(0, (int)_currentHP);
        int convertedMP = Mathf.Max(0, (int)_currentMP);
        string expectedHPString = $"{convertedHP}/{(int)_maxHP}";
        string expectedMPString = $"{convertedMP}/{(int)_maxMP}";

        hpImage.fillAmount = _currentHP / _maxHP; 

        if (updateMainScreenUI) {
            hpImageMain.fillAmount = _currentHP / _maxHP;
            hpTextMain.text = expectedHPString;
            mpImageMain.fillAmount = _currentMP / _maxMP;
            mpTextMain.text = expectedMPString;
        }
    }
}
