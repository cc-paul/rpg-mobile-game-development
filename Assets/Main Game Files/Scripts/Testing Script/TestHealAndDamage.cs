using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHealAndDamage : MonoBehaviour {
    [Header("Variable Declarations and other assignments")]
    [SerializeField] private bool attackPlayer;

    private PlayerStatsController playerStatsController;

    private void OnTriggerEnter(Collider target) {
        if (attackPlayer) {
            DoDamageToPlayer(_target: target.gameObject, Global.Command.Attack);
        } else {
            HealThePlayer(_target: target.gameObject, Global.Command.Heal);
        }
    }

    private void OnTriggerExit(Collider target) {
        if (attackPlayer) {
            DoDamageToPlayer(_target: target.gameObject, Global.Command.Stop_Attack);
        } else {
            HealThePlayer(_target: target.gameObject, Global.Command.Stop_Heal);
        }
    }

    private void DoDamageToPlayer(GameObject _target,Global.Command _command) {
        if (_target.CompareTag(Global.GameTags.Player.ToString())) {
            playerStatsController = _target.transform.parent.Find(Global.GENERAL_SETTINGS).GetComponent<PlayerStatsController>();

            if (_command == Global.Command.Attack) {
                playerStatsController.InitializeContiniousDamage(_sourceComponent: this);
            } else if (_command == Global.Command.Stop_Attack) {
                playerStatsController.StopContiniousDamage();
            }
        }
    }

    private void HealThePlayer(GameObject _target, Global.Command _command) {
        if (_target.CompareTag(Global.GameTags.Player.ToString())) {
            playerStatsController = _target.transform.parent.Find(Global.GENERAL_SETTINGS).GetComponent<PlayerStatsController>();

            if (_command == Global.Command.Heal) {
                playerStatsController.InitializeContiniousHeal(_sourceComponent: this);
            } else if (_command == Global.Command.Stop_Heal) {
                playerStatsController.StopContiniousHeal();
            }
        }
    }
}