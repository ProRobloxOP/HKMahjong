

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class WelcomeScreen : MonoBehaviour, IPointerClickHandler
{
      public GameObject buttonsObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
      {;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector3(10, 6, 8);
      }

     // Update is called once per frame
     void Update()
    {
       
    }

      public void OnPointerClick(PointerEventData eventData)
      {
            gameObject.SetActive(false);
            buttonsObject.SetActive(true);
      }

}
