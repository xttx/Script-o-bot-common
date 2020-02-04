using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Scaler_Advanced : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRectTransformDimensionsChange()
    {
        var rt = GetComponent<RectTransform>();
        Debug.Log("size changed: " + rt.rect.width + "x" + rt.rect.height + ", scale: " + rt.lossyScale.x );

        var cs = GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(Screen.width, Screen.height);

    }
}
