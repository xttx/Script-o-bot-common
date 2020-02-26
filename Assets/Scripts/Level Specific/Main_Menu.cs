using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Main_Menu : MonoBehaviour
{
    public CanvasGroup[] studios = new CanvasGroup[]{};
    public float studio_fadeIn = 0.25f;
    public float studio_fadeOut = 0.25f;
    public float studio_show = 0.5f;

    public Texture2D cursor_std = null;
    public bool new_3d_layout = false;

    RectTransform panelOptions = null;
    RectTransform panelLoadGame = null;
    CanvasGroup main_canvas = null;
    CanvasGroup panel_ui_cnv = null;
    CanvasGroup panelOptions_cv = null;
    CanvasGroup panelLoadGame_cv = null;
    GameObject scene_obj = null;

    bool skip_animation = false;

    // Start is called before the first frame update
    void Start()
    {
        if ( cursor_std != null ) Cursor.SetCursor(cursor_std, new Vector2(2, 3), CursorMode.Auto);

        panelOptions = GameObject.Find("Panel_Options").GetComponent<RectTransform>();
        panelOptions_cv = panelOptions.GetComponent<CanvasGroup>();
        panelOptions_cv.alpha = 0f;
        panelLoadGame = GameObject.Find("Panel_LoadLevel").GetComponent<RectTransform>();
        panelLoadGame_cv = panelLoadGame.GetComponent<CanvasGroup>();
        panelLoadGame_cv.alpha = 0f;

        scene_obj = GameObject.Find("Scene_Obj");
        if (scene_obj != null) scene_obj.SetActive(false);

        if (new_3d_layout) {
            GameObject.Find("Canvas_Options").GetComponent<CanvasGroup>().alpha = 0f;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        Options_Global_Static.Load_Level.AddListener((i) => { Start_Game(); } );

        StartCoroutine(Animations());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { 
            if (Options_Global_Static.SubPanelClose()) return;
            skip_animation = true; Close_Options(); 
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) { skip_animation = true; }
    }

    IEnumerator Animations() {
        main_canvas = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
        //var img_back = GameObject.Find("Background_Image").GetComponent<Image>();
        var img_back = GameObject.Find("Background_Image").GetComponent<CanvasGroup>();
        var img_title_sub = GameObject.Find("Image_Title_Sub").GetComponent<Image>();
        GameObject.Find("Panel_Arc").transform.localScale = Vector3.zero;
        var rt_title_sub = GameObject.Find("Image_Title_Sub").GetComponent<RectTransform>();
        var panel_ui = GameObject.Find("Panel_UI").GetComponent<RectTransform>();
        panel_ui_cnv = panel_ui.GetComponent<CanvasGroup>();
        //img_back.color = new Color(1f, 1f, 1f, 0f);
        img_back.alpha = 0f;
        img_title_sub.color = new Color(1f, 1f, 1f, 0f);
        //rt_title.sizeDelta = new Vector2(rt_title.sizeDelta.x, 0f);
        panel_ui_cnv.alpha = 0f;
        foreach (var cv in studios) {
            cv.alpha = 0f; cv.interactable = false; cv.blocksRaycasts = false; 
        }


        Tweener t;
        float end_time = 0f;
        foreach (var cv in studios) {
            skip_animation = false;
            t = cv.DOFade(1f, studio_fadeIn);
            while (t.IsActive()) { yield return null; if (skip_animation) goto Studio_fadeOut; }

            end_time = Time.unscaledTime + studio_show;
            while (Time.unscaledTime < end_time) { yield return null; if (skip_animation) goto Studio_fadeOut; }

            Studio_fadeOut:
            skip_animation = false;
            cv.DOFade(0f, studio_fadeOut);
            while (t.IsActive()) { yield return null; if (skip_animation) break; }
        }

        //Fade in background image
        t = img_back.DOFade(1f, 1f);

        end_time = Time.unscaledTime + 0.5f;
        while (Time.unscaledTime < end_time) { yield return null; }
        t = Animate_Title_Letters(0f);
        end_time = Time.unscaledTime + 1.2f;
        while (Time.unscaledTime < end_time) { yield return null; if (skip_animation) break; }

        skip_animation = false;
        img_title_sub.DOFade(1f, 0.3f);
        t = rt_title_sub.DOScale(new Vector3(5f, 5f, 5f), 0.75f ).From();
        while (t.IsActive()) { yield return null; if (skip_animation) break; }
        panel_ui_cnv.DOFade(1f, 0.3f);
        panel_ui.DOAnchorPosX(350f, 0.3f).From();

        panel_ui.transform.GetChild(1).GetComponent<Button>().onClick.AddListener( ()=>{
            //Start Game
            Start_Game();
        });
        panel_ui.transform.GetChild(2).GetComponent<Button>().onClick.AddListener( ()=>{
            //Load game
            var cc = GameObject.Find("Cube_Center").transform;
            if (panelLoadGame_cv.interactable || DOTween.IsTweening(cc)) return;

            if (scene_obj != null) scene_obj.SetActive(true);

            panelLoadGame_cv.alpha = 1f;
            Camera.main.clearFlags = CameraClearFlags.Skybox;
            GameObject.Find("Canvas_LoadLevel").GetComponent<CanvasGroup>().alpha = 1f;

            main_canvas.DOFade(0.75f, 0.5f);
            cc.DOScale(0.9f, 0.75f);
            cc.DORotate(new Vector3(0f, 90, 0f), 0.75f).SetDelay(0.5f);
            cc.DOScale(1.035f, 0.5f).SetDelay(1f).OnComplete(() => {
                panelLoadGame_cv.interactable = true;
                panelLoadGame_cv.blocksRaycasts = true;
            });
        });
        panel_ui.transform.GetChild(3).GetComponent<Button>().onClick.AddListener( ()=>{
            //Show Options
            if (!new_3d_layout) {
                if (panelOptions_cv.interactable || DOTween.IsTweening(panelOptions_cv)) return;
                panelOptions.DOAnchorPosY(200f, 0.5f).From();
                panelOptions_cv.DOFade(1f, 0.5f).OnComplete(() => {
                    panelOptions_cv.interactable = true;
                    panelOptions_cv.blocksRaycasts = true;
                });
            } else {
                var cc = GameObject.Find("Cube_Center").transform;
                if (panelOptions_cv.interactable || DOTween.IsTweening(cc)) return;

                if (scene_obj != null) scene_obj.SetActive(true);

                panelOptions_cv.alpha = 1f;
                Camera.main.clearFlags = CameraClearFlags.Skybox;
                GameObject.Find("Canvas_Options").GetComponent<CanvasGroup>().alpha = 1f;

                main_canvas.DOFade(0.75f, 0.5f);
                cc.DOScale(0.9f, 0.75f);
                cc.DORotate(new Vector3(0f, -90, 0f), 0.75f).SetDelay(0.5f);
                cc.DOScale(1.035f, 0.5f).SetDelay(1f).OnComplete(() => {
                    panelOptions_cv.interactable = true;
                    panelOptions_cv.blocksRaycasts = true;
                });
            }
        });
        panel_ui.transform.GetChild(4).GetComponent<Button>().onClick.AddListener( ()=>{
            //Exit game
            Application.Quit();
        });
    }

    Tweener Animate_Title_Letters(float delay = 0f) {
        var t = GameObject.Find("Panel_Arc").transform;
        t.localScale = new Vector3(1f, 1f, 1f);
        Tweener tw = null;
        for (int i = 0; i < t.childCount; i++) {
            var l_rt = t.GetChild(i).GetComponent<RectTransform>();
            tw = l_rt.DOScaleY(0f, 0.3f).From().SetEase(Ease.OutBounce).SetDelay(delay);
            delay += 0.1f;
        }
        return tw;
    }

    public void Close_Options()
    {
        if (!new_3d_layout) {
            if (!panelOptions_cv.interactable || DOTween.IsTweening(panelOptions_cv)) return;
            panelOptions_cv.interactable = false;
            panelOptions_cv.blocksRaycasts = false;
            panelOptions_cv.DOFade(0f, 0.5f);
            panelOptions.DOAnchorPosY(200f, 0.5f).OnComplete(() => {
                panelOptions.anchoredPosition = new Vector2(panelOptions.anchoredPosition.x, 0f);
            });
        } else {
            var cc = GameObject.Find("Cube_Center").transform;
            if ((!panelOptions_cv.interactable && !panelLoadGame_cv.interactable) || DOTween.IsTweening(cc)) return;

            panelOptions_cv.interactable = false;
            panelOptions_cv.blocksRaycasts = false;
            panelLoadGame_cv.interactable = false;
            panelLoadGame_cv.blocksRaycasts = false;

            cc.DOScale(0.9f, 0.75f);
            cc.DORotate(new Vector3(0f, 0, 0f), 0.75f).SetDelay(0.5f);
            cc.DOScale(1f, 0.5f).SetDelay(1f);
            main_canvas.DOFade(1f, 0.5f).SetDelay(1f);
        }
    }

    void Start_Game() {
        //Start game
        panel_ui_cnv.interactable = false;
        panel_ui_cnv.blocksRaycasts = false;
        panelLoadGame_cv.interactable = false;
        panelLoadGame_cv.blocksRaycasts = false;
        main_canvas.DOFade(0f, 3f);
        panelOptions_cv.DOFade(0f, 3f);
        panelLoadGame_cv.DOFade(0f, 3f);
        StartCoroutine(LoadScene());
        //SceneManager.LoadSceneAsync(1);
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            Debug.Log("Loading progress: " + asyncOperation.progress);
            //var p = Mathf.RoundToInt(asyncOperation.progress * 100);
            //sld.value = asyncOperation.progress;
            if (asyncOperation.progress >= 0.9f) {
                main_canvas.DOKill(); panelOptions_cv.DOKill(); panelLoadGame_cv.DOKill();
                asyncOperation.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
    }
}
