using UnityEngine;
using UnityEngine.UI;

public class TurnCycle : MonoBehaviour
{
    Text turnLabel;
    int player = 1;
    static readonly Color Player1Color = new Color(0.2f, 0.5f, 1f, 1f);
    static readonly Color Player2Color = new Color(0.85f, 0.2f, 0.2f, 1f);

    public void SetLabel(Text label)
    {
        turnLabel = label;
        if (turnLabel != null) turnLabel.color = Player1Color;
    }

    public void EndTurn()
    {
        player = player == 1 ? 2 : 1;
        if (turnLabel != null)
        {
            turnLabel.text = "Player " + player + "'s Turn";
            turnLabel.color = player == 1 ? Player1Color : Player2Color;
        }
    }
}