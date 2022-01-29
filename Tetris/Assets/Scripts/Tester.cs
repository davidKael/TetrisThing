using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    float timer = 0;
    float fallWait = 0.5f;
    float rushSpeed = 6;
    int yOffset = 20;
    BoxFormation bf;

    bool isRushing = false;

   

    private void Update()
    {


        isRushing = Input.GetKey(KeyCode.DownArrow);
    

        if(bf != null)
        {
            if (timer <= 0)
            {

                bf.Fall();
                if (bf.IsPlaced)
                {
                    bf = null;
                    return;
                }
                else
                {
                    timer = fallWait;
                }

               
            }

            else
            {
                timer -= Time.deltaTime * (isRushing ? rushSpeed : 1);
            }

        }
        else
        {
            bf = new BoxFormation();
            timer = fallWait;
        }

    }


}
