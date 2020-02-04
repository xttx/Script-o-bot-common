using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ASTT {
public class Panel_Scaler_Areas : MonoBehaviour
{
    public float scale = 100f;

    public float L = 0f;
    public float R = 0f;
    public float T = 0f;
    public float B = 0f;
    public float W = 0f;

    public float Drag_L = 0f;
    public float Drag_R = 0f;
    public float Drag_T = 0f;
    public float Drag_H = 0f;

    public Rect Expand_TL = new Rect(0f, 0f, 0f, 0f);
    public Rect Expand_TR = new Rect(0f, 0f, 0f, 0f);
    public Rect Expand_BL = new Rect(0f, 0f, 0f, 0f);
    public Rect Expand_BR = new Rect(0f, 0f, 0f, 0f);

    //public bool show = false;
    public float alpha = 1f;

    public references_info references = new references_info();

    [System.Serializable]
    public class references_info {
        public RectTransform L = null;
        public RectTransform R = null;
        public RectTransform T = null;
        public RectTransform B = null;
        public RectTransform TL = null;
        public RectTransform TR = null;
        public RectTransform BL = null;
        public RectTransform BR = null;
        public RectTransform DRAG = null;
    }

    void OnValidate()
    {
        this.Invoke("Update_Size", 0f);
    }

    // Update is called once per frame
    void Update_Size()
    {
        if (alpha > 1f) alpha = 1f;
        if (alpha < 0f) alpha = 0f;

        // references.L.GetComponent<Image>().enabled = show;
        // references.R.GetComponent<Image>().enabled = show;
        // references.T.GetComponent<Image>().enabled = show;
        // references.B.GetComponent<Image>().enabled = show;
        // references.TL.GetComponent<Image>().enabled = show;
        // references.TR.GetComponent<Image>().enabled = show;
        // references.BL.GetComponent<Image>().enabled = show;
        // references.BR.GetComponent<Image>().enabled = show;
        // references.DRAG.GetComponent<Image>().enabled = show;

        references.L.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.R.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.T.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.B.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.TL.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.TR.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.BL.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.BR.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        references.DRAG.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f, alpha);

        references.L.offsetMin = new Vector2(L, B + W);
        references.L.offsetMax = new Vector2(L + W, -T - W);
        references.R.offsetMin = new Vector2(R - W , B + W);
        references.R.offsetMax = new Vector2(R, -T - W);
        references.T.offsetMin = new Vector2(L + W, -W - T);
        references.T.offsetMax = new Vector2(R - W, -T);
        references.B.offsetMin = new Vector2(L + W, B);
        references.B.offsetMax = new Vector2(R - W, B + W);

        references.TL.offsetMin = new Vector2(L + Expand_TL.x, -W - T - Expand_TL.height);
        references.TL.offsetMax = new Vector2(L + W + Expand_TL.width, -T - Expand_TL.y);
        references.TR.offsetMin = new Vector2(R - W + Expand_TR.x, -W - T - Expand_TR.height);
        references.TR.offsetMax = new Vector2(R + Expand_TR.width, -T - Expand_TR.y);
        references.BL.offsetMin = new Vector2(L + Expand_BL.x, B - Expand_BL.height);
        references.BL.offsetMax = new Vector2(L + W + Expand_BL.width, B + W - Expand_BL.y);
        references.BR.offsetMin = new Vector2(R - W + Expand_BR.x, B - Expand_BR.height);
        references.BR.offsetMax = new Vector2(R + Expand_BR.width, B + W - Expand_BR.y);

        references.DRAG.offsetMin = new Vector2(Drag_L, -Drag_H - Drag_T);
        references.DRAG.offsetMax = new Vector2(-Drag_R, -Drag_T);
    }
}
}