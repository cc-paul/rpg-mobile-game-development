using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWeilding : MonoBehaviour {
    [Header("Swordsman Weapon Holders")]
    [SerializeField] private GameObject swordAttackHolder;
    [SerializeField] private GameObject swordBackHolder;

    private PlayerStatsManager playerStatsManager;

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    private void Start() {
        ChangeWeaponVisibility(isAttackMode: false);
    }

    public void ChangeWeaponVisibility(bool isAttackMode) {
        if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
            swordAttackHolder.SetActive(isAttackMode);
            swordBackHolder.SetActive(!isAttackMode);
        }
    }
}
