using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu_Sorting_Order_Hack : MonoBehaviour
{
    public RectTransform[] obj_to_adjust = null;

    RectTransform rt = null;

    void Start() {
        rt = GetComponent<RectTransform>();
    }

    void Update() {
        if (obj_to_adjust == null) return;
        if (rt.hasChanged) {
            rt.hasChanged = false;
            foreach (var obj_rt in obj_to_adjust) {
                obj_rt.anchoredPosition = new Vector2(1f, rt.anchoredPosition.y );
                obj_rt.sizeDelta = new Vector2(1f, rt.sizeDelta.y );
            }
        }
    }

    // void OnRectTransformDimensionsChange() {
    //     //Debug.Log("a");
    //     if (obj_to_adjust == null) return;
    //     foreach (var rt in obj_to_adjust) {
    //         rt.sizeDelta = new Vector2(rt.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y );
    //     }
    // }
}
