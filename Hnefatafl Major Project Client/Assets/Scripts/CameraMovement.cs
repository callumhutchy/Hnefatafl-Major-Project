using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Makes the camera move
public class CameraMovement : MonoBehaviour
{
    //Get the throne tiles, this will be the rotation point
    public GameObject throne;
    //How quick the camera will rotate
    public float rotationSpeed = 15f;
    //Booleans to say which way to go
    bool spinRight = false;
    bool spinLeft = false;

    void Start()
    {//Get the throne
        throne = GameObject.FindGameObjectWithTag("throne");
        //Set the values of the zoom slider to the cameras min and max FOV
        zoomSlider.minValue = minFov;
        zoomSlider.maxValue = maxFov;
        zoomSlider.value = Camera.main.fieldOfView;
    }

    //The min and max fov
    float minFov = 15f;
    float maxFov = 90f;
    //How quick the zoom scrolls
    float scrollSensitivty = 10f;
    //A slider to control zooming
    public Slider zoomSlider;


    void Update()
    {   
        //Rotate the camera right
        if (spinRight && !spinLeft)
        {
            transform.RotateAround(throne.transform.position, throne.transform.up, -rotationSpeed * Time.deltaTime);

        }
        //Rotate the camera left
        else if (spinLeft && !spinRight)
        {
            transform.RotateAround(throne.transform.position, throne.transform.up, rotationSpeed * Time.deltaTime);

        }

        //Adjust the camera according to the slider 
        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivty;
        fov = Mathf.Clamp(fov, minFov, maxFov);

        fov = zoomSlider.value;

        Camera.main.fieldOfView = fov;


    }

    //Button functions to control holding the buttons down while rotating the camera
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
