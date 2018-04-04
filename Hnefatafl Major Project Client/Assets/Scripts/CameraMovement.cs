using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public GameObject throne;

    public float rotationSpeed = 15f;

    bool spinRight = false;
    bool spinLeft = false;

    void Start()
    {
        throne = GameObject.FindGameObjectWithTag("throne");
    }

   
    void Update()
    {
        if (spinRight && !spinLeft)
        {
            transform.RotateAround(throne.transform.position, throne.transform.up, -rotationSpeed * Time.deltaTime);

        }
        else if (spinLeft && !spinRight)
        {
            transform.RotateAround(throne.transform.position, throne.transform.up, rotationSpeed * Time.deltaTime);

        }
    }


    public void OnLeftDown()
    {   
        Debug.Log("Down");
        spinLeft = true;
    }

    public void OnLeftUp()
    {   
        Debug.Log("Up");
        spinLeft = false;
    }

    public void OnRightDown()
    {
        Debug.Log("Down");
        spinRight = true;
    }

    public void OnRightUp()
    {
        Debug.Log("Up");
        spinRight = false;
    }

}
