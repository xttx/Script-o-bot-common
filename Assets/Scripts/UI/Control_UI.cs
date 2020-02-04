using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Control_UI : MonoBehaviour
{
    CanvasGroup cv;
    Button b_play;
    Button b_stop;
    Button b_pause;
    Button b_frame;
    RectTransform rt;
    static Control_UI inst;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        cv = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
        b_play = transform.GetChild(0).GetComponent<Button>();
        b_stop = transform.GetChild(1).GetComponent<Button>();
        b_pause = transform.GetChild(2).GetComponent<Button>();
        b_frame = transform.GetChild(3).GetComponent<Button>();
        set_stop_state();
    }



    public static void hide() {
        disable_canvas();
        inst.rt.DOKill();
        inst.rt.anchoredPosition = new Vector2(0f, 0f);
        inst.rt.DOAnchorPosY(-110f, 0.5f).SetEase(Ease.InOutSine);
    }
    public static void show(bool enable_canvas_after_show = true) {
        inst.rt.DOKill();
        inst.rt.anchoredPosition = new Vector2(0f, -110f);
        inst.rt.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InOutSine).OnComplete(()=>{
            if (enable_canvas_after_show) enable_canvas();
        });
    }

    public static void set_stop_state() {
        inst.b_frame.gameObject.SetActive(false);
        inst.b_pause.gameObject.SetActive(true);
        disable_stop(); disable_pause(); enable_play(); disable_frame();
    }
    public static void set_play_state() {
        inst.b_frame.gameObject.SetActive(false);
        inst.b_pause.gameObject.SetActive(true);
        enable_stop(); enable_pause(); disable_play(); disable_frame();
    }
    public static void set_pause_state() {
        inst.b_frame.gameObject.SetActive(true);
        inst.b_pause.gameObject.SetActive(false);
        enable_stop(); disable_pause(); enable_play(); disable_frame(); //enable_frame();
    }

    public static bool isPlaying() {
        return inst.b_stop.interactable;
    }

    public static bool isInPauseState() {
        return inst.b_frame.gameObject.activeSelf;
    }

    public static void enable_all() {
        inst.b_play.interactable  = true;
        inst.b_stop.interactable  = true;
        inst.b_pause.interactable = true;
        inst.b_frame.interactable = true;
    }
    public static void disable_all() {
        inst.b_play.interactable  = false;
        inst.b_stop.interactable  = false;
        inst.b_pause.interactable = false;
        inst.b_frame.interactable = false;
    }
    public static void enable_play()    { inst.b_play.interactable  = true; }
    public static void enable_stop()    { inst.b_stop.interactable  = true; }
    public static void enable_pause()   { inst.b_pause.interactable = true; }
    public static void enable_frame()   { inst.b_frame.interactable = true; }
    public static void enable_canvas()  { inst.cv.interactable      = true; }
    public static void disable_play()   { inst.b_play.interactable  = false; }
    public static void disable_stop()   { inst.b_stop.interactable  = false; }
    public static void disable_pause()  { inst.b_pause.interactable = false; }
    public static void disable_frame()  { inst.b_frame.interactable = false; }
    public static void disable_canvas() { inst.cv.interactable      = false; }
}
