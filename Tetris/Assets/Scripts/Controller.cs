using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    [SerializeField] private List<FormTemplate> formTemplates = new List<FormTemplate>();
    [SerializeField] private float _moveDelay = 0.5f;
    [SerializeField] private float _verticalRushSpeed = 5;
    [SerializeField] private float _horizontallRushSpeed = 4;

    private float _fallTimer = 0;
    private float _sideTimer = 0;
    private BoxFormation _currForm;
    private FormTemplate _nexFormTemp;
    private FormTemplate _holdFormTemp;
    private System.Random random = new System.Random();

    private bool _isHoldAvailable = true;

    
    [SerializeField] PlayerGrid _grid;

    private void Update()
    {
        if (!GameState.IsGameOver)
        {
            RunFormControl();
        }
    }

    private FormTemplate GetRandomForm()
    {
        return formTemplates[random.Next(0, formTemplates.Count)];
    }

    private void Hold()
    {
        FormTemplate temp = _currForm.Form;
        _grid.DeleteFormFromGrid(_currForm);

        if (_holdFormTemp == null)
        {
            _currForm = new BoxFormation(_nexFormTemp, _grid);
            _nexFormTemp = GetRandomForm();
        }
        else
        {
            _currForm = new BoxFormation(_holdFormTemp, _grid);
        }

        _holdFormTemp = temp;
        _isHoldAvailable = false;
        _sideTimer = _moveDelay;
        _fallTimer = _moveDelay;
    }

    private int GettingHorizontalInput()
    {
        int input = (Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0);

        if (input != 0)
        {
            _sideTimer = _moveDelay;
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
                _sideTimer = _moveDelay;
            }
        }
        return input;
    }

    private int GetVerticalInput()
    {
        int output;

        if (_fallTimer <= 0)
        {
            output = -1;

            _fallTimer = _moveDelay;
        }
        else
        {
            _fallTimer -= Time.deltaTime * (Input.GetKey(KeyCode.DownArrow) && !Input.GetKeyUp(KeyCode.DownArrow) ? _verticalRushSpeed : 1);
            output = 0;
        }

        return output;
    }

    private bool GetInstaDropInput()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }

    private bool GetRotationInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool GetHoldInput()
    {
        return Input.GetKeyDown(KeyCode.H);
    }

    private bool IsFormInPlay()
    {
        return _currForm != null;
    }

    private void RunFormControl()
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
            if (_nexFormTemp != null)
            {
                _sideTimer = _moveDelay;
                _fallTimer = _moveDelay;
                _currForm = new BoxFormation(_nexFormTemp, _grid);
                _isHoldAvailable = true;
            }
            _nexFormTemp = GetRandomForm();
        }
    }
}