using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
IMPORTANT:
  for this to work, you need to:

  set function:
    AssignPositioningIfNeeded
  to "public", in TMP_InpitField.cs

  OBSOLETE / NOT NEEDED
  add event:
        [Serializable]
        public class LateUpdateEvent : UnityEvent<string> { }

        [SerializeField]
        private LateUpdateEvent m_OnLateUpdate = new LateUpdateEvent();

        public LateUpdateEvent onLateUpdate { get { return m_OnLateUpdate; } set { SetPropertyUtility.SetClass(ref m_OnLateUpdate, value); } }

  put event invoke in LateUpdate() before UpdateScrollbar();
        if (onLateUpdate != null) onLateUpdate.Invoke(m_Text);

*/

public class TMPro_HScrollbar : MonoBehaviour
{
    public Scrollbar HScroll = null;

    static TMPro.TMP_InputField TMP_InputField = null;
    static RectTransform TMP_Text_Rect = null;

    float viewport_width = 0f;
    bool scrolled_by_script = false;

    // Start is called before the first frame update
    void Start()
    {
        TMP_InputField = GetComponent<TMPro.TMP_InputField>();
        TMP_Text_Rect = TMP_InputField.textComponent.rectTransform;
        viewport_width = TMP_Text_Rect.parent.GetComponent<RectTransform>().rect.width;

        HScroll.onValueChanged.AddListener(OnScroll);

        
        //TMP_InputField.onEndEdit.AddListener( (string str)=> { Debug.Log("onEndEdit"); } );
        //TMP_InputField.onSubmit.AddListener( (string str)=> { Debug.Log("onSubmit"); } );
        //TMP_InputField.onValueChanged.AddListener( (string str)=> { Debug.Log("onValueChanged"); } );
        //TMP_InputField.onLateUpdate.AddListener( (string str)=> { Debug.Log("onLateUpdate"); } );
    }

    void OnScroll(float i)
    {
        if (scrolled_by_script) return;

        float current_width = LayoutUtility.GetPreferredSize( TMP_Text_Rect, 0 );
        float maximum_possible_offset = current_width - viewport_width;
        float requested_offset = Mathf.Lerp(0f, maximum_possible_offset, HScroll.value);
        TMP_Text_Rect.anchoredPosition = new Vector2(requested_offset * -1, TMP_Text_Rect.anchoredPosition.y);

        //Debug.Log ("Width: " + current_width.ToString() + ", OffsetMax: " + maximum_possible_offset.ToString() + ", Offset_Calculated: " + requested_offset.ToString() );

        TMP_InputField.AssignPositioningIfNeeded();
    }

    public static void Scroll_H_ToZero ()
    {
        TMP_Text_Rect.anchoredPosition = new Vector2(0f, TMP_Text_Rect.anchoredPosition.y);
        TMP_InputField.AssignPositioningIfNeeded();
    }

    // Update is called once per frame
    void Update()
    {
        scrolled_by_script = true;

        //Set scrollbar size
        float current_width = LayoutUtility.GetPreferredSize( TMP_Text_Rect, 0 );
        if (current_width > viewport_width) {
            HScroll.size = viewport_width / current_width;
        } else
            HScroll.size = 1f;

        //Set scrollbar position
        if (HScroll.size < 1f) {
            float maximum_possible_offset = current_width - viewport_width;
            float current_offset = TMP_Text_Rect.anchoredPosition.x * -1;
            HScroll.value = Mathf.InverseLerp (0f, maximum_possible_offset, current_offset);
            //Debug.Log ("Offset: " + current_offset.ToString() + ", OffsetMax: " + maximum_possible_offset.ToString() );
        } else
            HScroll.value = 0f;

        scrolled_by_script = false;
    }
}
