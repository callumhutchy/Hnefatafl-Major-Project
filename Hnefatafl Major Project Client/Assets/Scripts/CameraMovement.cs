using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraMovement : MonoBehaviour
{

    public GameObject throne;

    public float rotationSpeed = 15f;

    bool spinRight = false;
    bool spinLeft = false;

    void Start()
    {
        throne = GameObject.FindGameObjectWithTag("throne");
        zoomSlider.minValue = minFov;
        zoomSlider.maxValue = maxFov;
        zoomSlider.value = Camera.main.fieldOfView;
    }

    float minFov = 15f;
    float maxFov = 90f;
    float scrollSensitivty = 10f;

    public Slider zoomSlider;


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

        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivty;
        fov = Mathf.Clamp(fov, minFov, maxFov);

        fov = zoomSlider.value;

        Camera.main.fieldOfView = fov;


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
