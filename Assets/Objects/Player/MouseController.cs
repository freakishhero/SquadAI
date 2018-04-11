using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

    [SerializeField]
    Image selection_box;

    Vector2 start_position;
    Rect selection_rect;

    public void OnBeginDrag(PointerEventData eventData)
    {        
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            selection_box.gameObject.SetActive(true);
            start_position = eventData.position;
            selection_rect = new Rect();
        }  
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.position.x < start_position.x)
        {
            selection_rect.xMin = eventData.position.x;
            selection_rect.xMax = start_position.x;
        }
        else
        {
            selection_rect.xMin = start_position.x;
            selection_rect.xMax = eventData.position.x;
        }

        if (eventData.position.y < start_position.y)
        {
            selection_rect.yMin = eventData.position.y;
            selection_rect.yMax = start_position.y;
        }
        else
        {
            selection_rect.yMin = start_position.y;
            selection_rect.yMax = eventData.position.y;
        }

        selection_box.rectTransform.offsetMin = selection_rect.min;
        selection_box.rectTransform.offsetMax = selection_rect.max;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        selection_box.gameObject.SetActive(false);

        foreach (Selection selection in Selection.all_units)
        {
            if(selection_rect.Contains(Camera.main.WorldToScreenPoint(selection.transform.position)))
            {
                selection.OnSelect(eventData);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        //When the left mouse button is pushed
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Selection.DeselectAll(new BaseEventData(EventSystem.current));
            //Add a mouse press
            if (GameManager.MouseClicks == 0)
            {
                GameManager.MouseClicks = 1;
                Debug.Log("Single click: " + GameManager.MouseClicks);
                Debug.Log(Selection.currently_selected.Count);
                GameManager.First_Click_Time = Time.time;
            }
            //If the mouse has already been pressed
            else if (GameManager.MouseClicks == 1)
            {
                Selection.SelectAll(new BaseEventData(EventSystem.current));
                Debug.Log(Selection.currently_selected.Count);
                Debug.Log("Double click.");
                GameManager.MouseClicks = 0;
            }

            

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            float distance = 0;

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == gameObject)
                {
                    distance = result.distance;
                    break;
                }
            }

            GameObject next_object = null;
            float max_distance = Mathf.Infinity;

            foreach (RaycastResult result in results)
            {
                if (result.distance > distance && result.distance < max_distance)
                {
                    next_object = result.gameObject;
                    max_distance = result.distance;
                }
            }

            if (next_object)
            {
                ExecuteEvents.Execute<IPointerClickHandler>(next_object, eventData, (x, y) =>
                {
                    x.OnPointerClick((PointerEventData)y);
                });
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Selection selection in Selection.currently_selected)
            {
                selection.GetComponent<Unit>().Target.transform.position = pos;
            }
        }
    }
}
