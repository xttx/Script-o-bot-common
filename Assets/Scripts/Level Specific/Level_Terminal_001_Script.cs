using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class Level_Terminal_001_Script : MonoBehaviour
{
    // public VideoClip wall_main = null;
    // public VideoClip wall_correct = null;
    // public VideoClip wall_wrong = null;
    // public VideoClip floor_main = null;
    // public VideoClip floor_correct = null;
    // public VideoClip floor_wrong = null;

    public VideoPlayer[] Video_Players_To_Wait = null;

    GameObject Fade_Obj = null;

    // Update is called once per frame
    void Update()
    {
        if (Fade_Obj == null) return;
        if (DOTween.IsTweening( Fade_Obj.GetComponent<Image>()) ) return;

        foreach (var v in Video_Players_To_Wait) {
            if (!v.isPrepared) return;
        }
        Fade();
    }

    void OnEnable() {
        CreateFade();
    }

    void OnDisable() {
        if (Fade_Obj != null) {
            Destroy(Fade_Obj); Fade_Obj = null;
        }
    }

    void CreateFade()
    {
        var cnv = GameObject.Find("Canvas").GetComponent<Canvas>();
        Fade_Obj = new GameObject("Fade", typeof(RectTransform));
        Fade_Obj.transform.SetParent(cnv.transform);
        Fade_Obj.transform.SetSiblingIndex(1);
        var rt = Fade_Obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = new Vector2(0f, 0f);
        rt.offsetMax = new Vector2(0f, 0f);
        var img = Fade_Obj.AddComponent<Image>();
        img.color = Color.black;
    }

    void Fade()
    {
        if (Fade_Obj != null) {
            Fade_Obj.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(()=>{
                Destroy(Fade_Obj); Fade_Obj = null;
            });
        }
    }
}
