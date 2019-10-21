using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerClasses : MonoBehaviour
{
    public Dropdown playerBlue;
    public Dropdown playerRed;

    private void Start()
    {
        var class_blue = SetTeamParameters(playerBlue.value);
        var class_red = SetTeamParameters(playerRed.value);
        foreach (var item in class_blue)
        {
            //Debug.Log(item);
        }
    }

    public float[] SetTeamParameters(int type)
    {
        if(type == 0)
        {
            float[] class_params = new float[8];
            class_params[0] = Random.Range(20, 50); // HP
            class_params[1] = Random.Range(80, 100); // movement speed
            class_params[2] = Random.Range(80, 100); // damage
            class_params[3] = Random.Range(80, 100); // accuracy
            class_params[4] = Random.Range(20, 50); // clipsize
            class_params[5] = Random.Range(80, 100); // rate of fire
            class_params[6] = Random.Range(80, 100); // number of bullets per shot
            class_params[7] = 2.0f; // range
            class_params = NormalizeArray(class_params);
            return class_params;
        }
        else if (type == 1)
        {
            float[] class_params = new float[8];
            class_params[0] = Random.Range(50, 80); // HP
            class_params[1] = Random.Range(50, 80); // movement speed
            class_params[2] = Random.Range(50, 80); // damage
            class_params[3] = Random.Range(50, 80); // accuracy
            class_params[4] = Random.Range(50, 80); // clipsize
            class_params[5] = Random.Range(50, 80); // rate of fire
            class_params[6] = Random.Range(50, 80); // number of bullets per shot
            class_params[7] = 1.0f; // range
            class_params = NormalizeArray(class_params);
            return class_params;

        }
        else
        {
            float[] class_params = new float[8];
            class_params[0] = Random.Range(80, 100); // HP
            class_params[1] = Random.Range(20, 50); // movement speed
            class_params[2] = Random.Range(20, 50); // damage
            class_params[3] = Random.Range(20, 50); // accuracy
            class_params[4] = Random.Range(80, 100); // clipsize
            class_params[5] = Random.Range(20, 50); // rate of fire
            class_params[6] = Random.Range(80, 100); // number of bullets per shot
            class_params[7] = 0.0f; // range
            class_params = NormalizeArray(class_params);
            return class_params;

        }
    }

    public float[] NormalizeArray(float[] arr)
    {
        for (int i = 0; i < arr.Length-1; i++)
        {
            arr[i] = (arr[i] - 20.0f) / (100.0f - 20.0f); 
        }
        return arr;
    }
}
