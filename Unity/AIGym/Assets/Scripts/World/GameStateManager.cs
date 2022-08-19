/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    public AgentManager agentManager;
    public bool gameOver = true;

    public void StartGame()
    {
        if (agentManager.Count() != 0)
        {
            UserErrorInfo.ErrorWriter?.FlushErrorLog();
            agentManager?.Reset();
        }
        gameOver = false;
    }

    private void Update()
    {
        if (gameOver || agentManager.Count() == 0) return;

        if (isGameWon || isGameLost)
        {
            GameOver();
            Debug.Log("Game Over!");
        }
    }

    private bool isGameWon => agentManager.Any(agent => agent._visitedPointWorthObjects.Contains("Finish"));

    private bool isGameLost => agentManager.All(agent => !agent.IsAlive);

    public void GameOver()
    {
        if (gameOver) return;
        UserErrorInfo.ErrorWriter.FlushErrorLog();
        agentManager.Reset();
        gameOver = true;
    }
}