using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gameplay;

public class Player : MonoBehaviour {

    // Animation
    private Animator animator;

    // Player
    [SerializeField] private PlayerTag playerTag;
    public enum PlayerTag
    {
        Player1,
        Player2,
    }

    private UIManager uiManager;

    void Start() {
        animator = GetComponentInChildren<Animator>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void Defeat() {
        if (playerTag == PlayerTag.Player1)
            PlayerPrefs.SetInt("player_2_score", PlayerPrefs.GetInt("player_2_score", 0) + 1);
        else if (playerTag == PlayerTag.Player2)
            PlayerPrefs.SetInt("player_1_score", PlayerPrefs.GetInt("player_1_score", 0) + 1);

        PlayerPrefs.SetInt("current_round", PlayerPrefs.GetInt("current_round", 1) + 1);

        uiManager.UpdateUI();

        // WE PLAYED FOR 3 ROUNDS
        if (PlayerPrefs.GetInt("current_round", 1) == 4)
        {
            uiManager.GameOver();
        }
        else
        {
            Invoke("ResetScene", 2);
        }

        animator.SetTrigger("die");
        GetComponent<CapsuleCollider>().enabled = false;

    }

    private void ResetScene()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void GetHit()
    {
       // animator.SetTrigger("getHit");
    }

}
