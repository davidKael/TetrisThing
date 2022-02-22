using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerGrid : GridBase
{


    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] PlayerState _playerState;

    #region Methods
    /// <summary>
    /// called to move down all active boxes on grid
    /// </summary>
    /// <param name="lowestRow">lowest row to move down</param>
    /// <param name="amount">how many rows to move</param>
    internal void MoveDownAll(int lowestRow)
    {
        for (int row = lowestRow; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                Boxes[new Vector2Int(col, row)].MoveDown(1);
            }
        }
    }
    
    /// <summary>
    /// Clear grid from given Formation
    /// </summary>
    /// <param name="boxFormation"></param>
    internal void DeleteFormFromGrid(BoxFormation boxFormation)
    {
        boxFormation.Positions.ForEach(pos => Boxes[pos].ResetBox());
        boxFormation.ghosts.ForEach(ghostPos => Boxes[ghostPos].ResetBox());
    }
    



    internal void PlaceForm(BoxFormation form, out bool isPlaced)
    {

        if (IsPlacedOverLimit(form))
        {
            _playerState.PlayerLost();
        }
        else
        {
            List<int> rows = new List<int>();

            foreach (Vector2Int placedPos in form.Positions)
            {
                if (!Boxes.Values.Any(box => !box.IsActive && box.Position.y == placedPos.y))
                {
                    if (!rows.Contains(placedPos.y))
                    {
                        rows.Add(placedPos.y);

                    }
                }
            }

            rows.Sort();

            for (int row = 0; row < rows.Count; row++)
            {
                for (int col = 0; col < 10; col++)
                {

                    Boxes[new Vector2Int(col, rows[row])].ResetBox(true);
                }

            }

            int rowsMoved = 0;
            for (int row = 0; row < rows.Count; row++)
            {
                MoveDownAll(rows[row] - rowsMoved);
                rowsMoved++;
            }

            if (rows.Count > 0)
            {
                _playerState.AddToScore(ScoreHandler.CalculateScore(rows.Count));
                UpdateText();
            }


        }



        isPlaced = true;
    }

    bool IsPlacedOverLimit(BoxFormation form)
    {
        return form.Positions.Any(pos => pos.y >= 20);
    }
    void UpdateText()
    {
        _scoreText.text = _playerState.Score.ToString();
    }
    #endregion

}
