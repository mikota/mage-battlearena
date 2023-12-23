using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// UIManager USED IN GAMEPLAY
namespace Gameplay
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI player1Score;
        [SerializeField] private TMPro.TextMeshProUGUI player2Score;
        [SerializeField] private GameObject gameOverOverlay;
        [SerializeField] private TMPro.TextMeshProUGUI victoryText;
        [SerializeField] private PlayerController player1Controller;
        [SerializeField] private PlayerController player2Controller;

        void Start()
        {
            gameOverOverlay.SetActive(false);
            UpdateUI();
        }

        public void UpdateUI()
        {
            player1Score.text = PlayerPrefs.GetInt("player_1_score", 0).ToString();
            player2Score.text = PlayerPrefs.GetInt("player_2_score", 0).ToString();
        }

        public void GameOver()
        {
            gameOverOverlay.SetActive(true);
            if (PlayerPrefs.GetInt("player_1_score", 1) > PlayerPrefs.GetInt("player_2_score", 1))
                victoryText.text = "Player 1 Won";
            else
                victoryText.text = "Player 2 Won";

            player1Controller.GameOver();
            player2Controller.GameOver();
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
