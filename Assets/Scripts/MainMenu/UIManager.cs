using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// UIManager USED IN MAIN MENU
namespace MainMenu
{
    public class UIManager : MonoBehaviour
    {
        public void StartRound()
        {
            PlayerPrefs.SetInt("current_round", 1);
            PlayerPrefs.SetInt("player_1_score", 0);
            PlayerPrefs.SetInt("player_2_score", 0);
            SceneManager.LoadScene("Gameplay");
        }
    }
}
