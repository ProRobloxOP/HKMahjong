using Unity.VisualScripting;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.OnScreen;

public class WelcomeScreen : MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

     // Update is called once per frame
     void Update()
    {
       
    }

     public void OnPointerClick(PointerEventData eventData)
     {
           gameObject.SetActive(false);
     }
}
