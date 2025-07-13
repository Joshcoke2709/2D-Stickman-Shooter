using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int p1Score = 0;
    public int p2Score = 0;

    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;

    public GameTimer timer;
    public PlayerHealth p1Health;
    public PlayerHealth p2Health;
    public PlayerMovement playerMovement;
    public Player2Movement player2Movement;

    private bool gameEnded = false;

    public void PlayerDied(string playerTag)
    {
        if (gameEnded) return;

        if (playerTag == "Player1")
            p2Score++;
        else if (playerTag == "Player2")
            p1Score++;

        UpdateScoreUI();

        if (p1Score >= 3 || p2Score >= 3)
        {
            EndGame();
        }
        else
        {
            ResetRound();
        }
    }

    void UpdateScoreUI()
    {
        p1ScoreText.text = p1Score.ToString();
        p2ScoreText.text = p2Score.ToString();
    }

    void ResetRound()
    {
        timer.StopRound();
        Invoke(nameof(ResumeGame), 2f);
        Debug.Log("Round Reset!");
    }

    void ResumeGame()
    {
        p1Health.ResetHealth();
        p2Health.ResetHealth();
        p1Health.EnableControls();
        p2Health.EnableControls();

        timer.Resume();
    }

    void EndGame()
    {
        gameEnded = true;
        timer.StopRound();
        Debug.Log("Game Over!");
    }
}