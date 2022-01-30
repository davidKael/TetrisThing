using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour
{
  

 
    float fallTimer = 0;
    float sideTimer = 0;
    float fallWait = 0.5f;
    float sideWait = 0.5f;
    float rushSpeed = 6;
    BoxFormation bf;

    bool isRushing = false;
    bool isMovingHorizontal = false;

    [SerializeField]
    List<FormTemplate> formTemplates = new List<FormTemplate>();
    System.Random random = new System.Random();




    private void Update()
    {
        if (!GameState.IsGameOver)
        {
            int horizontalInput = 0;
            int verticalInput = 0;

            isRushing = Input.GetKey(KeyCode.DownArrow);

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                sideTimer = 0;
            }
            else
            {
                isMovingHorizontal = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
            }

            if (bf != null)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    bf.InstaDrop();
                }

                else
                {

                    if (fallTimer <= 0)
                    {
                        verticalInput = -1;
                        fallTimer = fallWait;
                    }
                    else
                    {
                        fallTimer -= Time.deltaTime * (isRushing ? rushSpeed : 1);
                    }


                    if (isMovingHorizontal)
                    {
                        if (sideTimer <= 0)
                        {
                            horizontalInput += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                            horizontalInput += Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
                            sideTimer = sideWait;
                        }
                        else
                        {
                            sideTimer -= Time.deltaTime * rushSpeed;
                        }
                    }

                    Vector2Int velocity = new Vector2Int(horizontalInput, verticalInput);

                    bf.Move(velocity);
                }



                if (bf.IsPlaced)
                {
                    bf = null;
                    return;
                }

            }
            else
            {
                bf = new BoxFormation(GetRandomForm());
            }
        }
       
    }

    FormTemplate GetRandomForm()
    {
        return formTemplates[random.Next(0, formTemplates.Count)];
    }


}
