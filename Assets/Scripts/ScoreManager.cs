using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int p1Score, p2Score;
    public TextMeshProUGUI p1ScoreText, p2ScoreText;

    public GameTimer timer;
    public PlayerHealth p1Health, p2Health;
    public PlayerMovement p1Movement;
    public Player2Movement p2Movement;

    private bool gameEnded;

    public void PlayerDied(string playerTag)
    {

        Debug.Log($"ScoreManager.PlayerDied({playerTag})");
        // …

        if (gameEnded) return;

        // 1. Increment the *winner’s* score
        if (playerTag == "Player1") p2Score++;
        else if (playerTag == "Player2") p1Score++;

        UpdateScoreUI();

        // 2. Stop timer & schedule round reset
        timer.StopRound();
        Invoke(nameof(ResetRound), 2f);
    }

    private void UpdateScoreUI()
    {
        p1ScoreText.text = p1Score.ToString();
        p2ScoreText.text = p2Score.ToString();
    }

    private void ResetRound()
    {
        // 1. Refill health
        p1Health.ResetHealth();
        p2Health.ResetHealth();

        // 2. Reset visual state
        p1Movement.ResetState();
        p2Movement.ResetState();

        // 3. Check for overall game end
        if (p1Score >= 3 || p2Score >= 3)
        {
            gameEnded = true;
            Debug.Log("Game Over!");
            // TODO: show final winner UI
            return;
        }

        // 4. Start next round
        timer.ResumeRound();
        Debug.Log("Next Round!");
    }

    private void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        timer.StopRound();

        p1Health.DisableControls();
        p2Health.DisableControls();

        Debug.Log("Game Over! Final Scores: P1: " + p1Score + ", P2: " + p2Score);
    }
}