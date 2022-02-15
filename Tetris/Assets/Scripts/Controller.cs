using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    #region FieldsInEditor
    [Header("Parts")]
    [SerializeField] List<FormTemplate> _formTemplates = new List<FormTemplate>();
    [SerializeField] PlayerGrid _grid;
    [SerializeField] DisplayGrid _nextGrid;
    [SerializeField] DisplayGrid _holdGrid;
    [Header("Settings")]
    [SerializeField] float _moveTime = 0.5f;
    [SerializeField] float _verticalRushSpeed = 5;
    [SerializeField] float _horizontallRushSpeed = 4;
    #endregion

    #region Fields
    float _fallTimer = 0;
    float _sideTimer = 0;
    BoxFormation _currForm;
    FormTemplate _nextFromTemp;
    FormTemplate _holdFormTemp;
    bool _isHoldAvailable = true;
    System.Random random = new System.Random();
    #endregion

    #region UnityMethods
    private void Update()
    {
        if (!GameState.IsGameOver)
        {
            RunFormControl();
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns a random FormTemplate
    /// </summary>
    /// <returns></returns>
    FormTemplate GetRandomForm()
    {
        return _formTemplates[random.Next(0, _formTemplates.Count)];
    }

    /// <summary>
    /// Called to save current form and get next one or the last holded form
    /// </summary>
    void Hold()
    {
        FormTemplate temp = _currForm.Form;
        _grid.DeleteFormFromGrid(_currForm);

        if (_holdFormTemp == null)
        {
            _currForm = new BoxFormation(_nextFromTemp, _grid);
            _nextGrid.DisplayForm(_nextFromTemp = GetRandomForm());
        }
        else
        {
            _currForm = new BoxFormation(_holdFormTemp, _grid);
        }

        _holdGrid.DisplayForm(_holdFormTemp = temp);
        _isHoldAvailable = false;
        _sideTimer = _moveTime;
        _fallTimer = _moveTime;
    }

    /// <summary>
    /// Returns input from player on the x-axis
    /// </summary>
    /// <returns></returns>
    int GettingHorizontalInput()
    {
        int input = (Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0);

        if (input != 0)
        {
            _sideTimer = _moveTime;
        }
        else
        {
            if (_sideTimer > 0)
            {
                _sideTimer -= Time.deltaTime * _horizontallRushSpeed;
                input = 0;
            }
            else
            {
                input = (Input.GetKey(KeyCode.RightArrow) && !Input.GetKeyUp(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKeyUp(KeyCode.RightArrow) ? -1 : 0);
                _sideTimer = _moveTime;
            }
        }
        return input;
    }

    /// <summary>
    /// Returns input from player on the Y-axis
    /// </summary>
    /// <returns></returns>
    int GetVerticalInput()
    {
        int output;

        if (_fallTimer <= 0)
        {
            output = -1;

            _fallTimer = _moveTime;
        }
        else
        {
            _fallTimer -= Time.deltaTime * (Input.GetKey(KeyCode.DownArrow) && !Input.GetKeyUp(KeyCode.DownArrow) ? _verticalRushSpeed : 1);
            output = 0;
        }

        return output;
    }

    /// <summary>
    /// Checks if player pressed the "InstaDrop"-button this frame
    /// </summary>
    /// <returns></returns>
    bool GetInstaDropInput()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }

    /// <summary>
    /// Checks if player is pressed the "Rotation"-button this frame
    /// </summary>
    /// <returns></returns>
    bool GetRotationInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    /// <summary>
    /// Checks if player pressed the "hold"-button this frame
    /// </summary>
    /// <returns></returns>
    bool GetHoldInput()
    {
        return Input.GetKeyDown(KeyCode.H);
    }

    /// <summary>
    /// Checks if there is an active form in player-control
    /// </summary>
    /// <returns></returns>
    bool IsFormInPlay()
    {
        return _currForm != null;
    }

    /// <summary>
    /// Flow of a player run
    /// </summary>
    void RunFormControl()
    {
        if (IsFormInPlay())
        {
            if (_isHoldAvailable && GetHoldInput())
            {
                Hold();
            }
            else if (GetInstaDropInput())
            {
                _currForm.InstaDrop();

            }
            else if (GetRotationInput())
            {
                _currForm.Rotate();
            }
            else
            {
                Vector2Int velocity = new Vector2Int(GettingHorizontalInput(), GetVerticalInput());
                _currForm.Move(velocity);
            }

            if (_currForm.IsPlaced)
            {
                ScoreHandler.PlaceForm(_currForm, _grid);
                _currForm = null;
            }
        }
        else
        {
            if (_nextFromTemp != null)
            {
                _sideTimer = _moveTime;
                _fallTimer = _moveTime;
                _currForm = new BoxFormation(_nextFromTemp, _grid);
                _isHoldAvailable = true;
            }

            _nextGrid.DisplayForm(_nextFromTemp = GetRandomForm());
        }
    }
    #endregion
}