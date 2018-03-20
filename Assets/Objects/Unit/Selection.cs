using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selection : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerClickHandler {

    public static HashSet<Selection> all_units = new HashSet<Selection>();
    public static HashSet<Selection> currently_selected = new HashSet<Selection>();

    Renderer m_renderer;

    [SerializeField]
    Material selected_material;
    [SerializeField]
    Material unselected_material;

    void Awake()
    {
        all_units.Add(this);
        m_renderer = GetComponent<Renderer>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        currently_selected.Add(this);
        m_renderer.material = selected_material;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        m_renderer.material = unselected_material;
    }

    public void  OnPointerClick(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
        {
            DeselectAll(eventData);
        }
        OnSelect(eventData);
    }

    public static void DeselectAll(BaseEventData eventData)
    {
        foreach(Selection selection in currently_selected)
        {
            selection.OnDeselect(eventData);
        }
        currently_selected.Clear();
    }
}
