

using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class WelcomeScreen : MonoBehaviour, IPointerClickHandler
{

      public static event Action StartRound;
      private GameObject buttonsObject;
      private GameObject handGUIObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
      {;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector3(10, 6, 8);

            buttonsObject = gameObject.transform.parent.Find("Buttons").gameObject;
            handGUIObject = gameObject.transform.parent.Find("TileHand").gameObject;
      }

     // Update is called once per frame
     void Update()
    {
       
    }

      public void OnPointerClick(PointerEventData eventData)
      {
            print("Round Started!");
            gameObject.SetActive(false);
            buttonsObject.SetActive(true);
            handGUIObject.SetActive(true);

            StartRound?.Invoke();
      }

}
