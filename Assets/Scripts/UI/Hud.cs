using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    public Damage_Info Damage_Settings = new Damage_Info();
    [System.Serializable]
    public class Damage_Info {
        public GameObject Damage_Text = null;
        public Color BOT_Negative = Color.red;
        public Color BOT_Positive = Color.green;
        public Color BOT_Zerro = Color.white;
        public Color Enemy_Negative = Color.yellow;
        public Color Enemy_Positive = Color.blue;
        public Color Enemy_Zerro = Color.white;
    }

    public GameObject help_overlay = null;

    public static Hud inst;

    bool hud_is_hidden = false;

    int last_checked_HP;
    int last_checked_MP;
    int last_checked_qEnergy;

    RectTransform volume_mask;
    RectTransform gauge_HP_rt;
    RectTransform gauge_qe_rt;
    Image gauge_Action_rt;
    Text txt;
    Transform txt_container;
    Transform txt_container_scrollable;
    RectTransform txt_containerRT;
    CanvasGroup txt_container_scrollable_CG;
    Transform txt_container_scrollable_txt_template;
    float txt_containerCurY = 180f;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;

        gauge_HP_rt = transform.Find("Gauge_HP").GetComponent<RectTransform>();
        gauge_qe_rt = transform.Find("GaugeSup1").GetComponent<RectTransform>();
        gauge_Action_rt = transform.Find("Gauge_Speed").GetComponent<Image>();

        Transform panel_log = GameObject.Find("Panel_Hud_Log").transform;
        txt = panel_log.Find("Text").GetComponent<Text>();
        txt.gameObject.SetActive(false);
        txt_container = panel_log.Find("TextContainer").transform;
        txt_containerRT = txt_container.GetComponent<RectTransform>();
        txt_container_scrollable = panel_log.Find("TextContainer_Scroll").transform;
        txt_container_scrollable_CG = txt_container_scrollable.GetComponent<CanvasGroup>();
        txt_container_scrollable_txt_template = txt_container_scrollable.GetChild(0).GetChild(0).GetChild(0).GetChild(0);

        volume_mask = GameObject.Find("Panel_Volume").transform.GetChild(0).GetComponent<RectTransform>();

        last_checked_HP = BOT.HP;
        last_checked_MP = BOT.MP;
        last_checked_qEnergy = BOT.qEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if (BOT.qEnergy != last_checked_qEnergy) {
            gauge_qe_rt.DOKill();
            //if (txt.gameObject.activeSelf) txt.DOKill();

            int got = BOT.qEnergy - last_checked_qEnergy;
            // if (got > 0) {txt.text = "Got " + got.ToString() + " quantic energy.";}
            // else if (got < 0) {txt.text = "Lost " + got.ToString() + " quantic energy.";}
            // txt.gameObject.SetActive(true); txt.color = Color.white;
            // txt.DOFade(0f, 1f).SetDelay(3f).OnComplete(()=> {txt.gameObject.SetActive(false); });
            if (got > 0)      { Log_sub ("Got " + got.ToString() + " quantic energy."); }
            else if (got < 0) { Log_sub ("Lost " + got.ToString() + " quantic energy."); }

            //Remap value;
            float n = Mathf.InverseLerp(0f, 18f, BOT.qEnergy); //0 - 18 is a min/max qEnergy range
            float width = Mathf.Lerp(0f, 90f, n); //0 - 90 is a gauge recttransform width range
            gauge_qe_rt.DOSizeDelta(new Vector2(width, gauge_qe_rt.sizeDelta.y), 1f);

            last_checked_qEnergy = BOT.qEnergy;
        }
        if (BOT.HP != last_checked_HP) {
            gauge_HP_rt.DOKill();

            int got = BOT.HP - last_checked_HP;
            if (got > 0)      { Log_sub ("BOT receive " + got.ToString() + " HP."); }
            else if (got < 0) { Log_sub ("BOT lost " + got.ToString() + " HP."); }

            //Remap value;
            float n = Mathf.InverseLerp(0f, 100f, BOT.HP); //0 - 100 is a min/max HP range
            float width = Mathf.Lerp(17f, 340f, n); //0 - 340 is a gauge recttransform width range
            gauge_HP_rt.DOSizeDelta(new Vector2(width, gauge_HP_rt.sizeDelta.y), 1f);

            last_checked_HP = BOT.HP;

            if (BOT.HP <= 0) {
                //Dead
                //dead = true;
                BOT.bot_animator.SetTrigger("Die");
            }
        }
        if (gauge_Action_rt.fillAmount < 1f) {
            float t = gauge_Action_rt.fillAmount + Time.deltaTime * ((float)BOT.speed / 100f);
            if (t > 1f) t = 1f;
            gauge_Action_rt.fillAmount = t;
        }

        //Show/Hide scrollable log
        if (Engine.check_key(Engine.Key.Debug_Show_Console)) { Show_Hide_Scrollable_Log(); }

        //Hide hud elements if desktop_mode is active and script is not running.
        if (!Engine.desktop_manager.is_active) {
            if (hud_is_hidden) ShowHide_HudElements();
        } else {
            if (!Control_UI.isPlaying()) { if (!hud_is_hidden) ShowHide_HudElements(true); }
            else                         { if (hud_is_hidden) ShowHide_HudElements(); }
        }

        //Help overlay
        if (Input.GetKeyDown(KeyCode.F1)) {
            Engine.Key[] important_keys = new Engine.Key[]{ Engine.Key.WindowMode_Toggle, Engine.Key.Debug_Show_Console, Engine.Key.Set_Breakpoint, Engine.Key.Volume_Up, Engine.Key.Volume_Down };
            string hotkey_info = "";
            string hotkey_descriptions = "";
            foreach (var k in important_keys) {
                hotkey_descriptions += Engine.Hotkeys_descriptions[k] + " : \n";
                hotkey_info += string.Join( " + ", Engine.Hotkeys[k][0].Select(x => x.ToString()) ) + "\n";
            }
            help_overlay.transform.GetChild(0).GetComponent<Text>().text = hotkey_descriptions;
            help_overlay.transform.GetChild(1).GetComponent<Text>().text = hotkey_info;
            help_overlay.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.F1)) {
            help_overlay.SetActive(false);
        }
    }

    List<string> log_queue = new List<string>();
    List<string> log_buffer = new List<string>();
    public static void Log(string str) { inst.Log_sub(str); }
    public void Log_sub(string str, bool add_to_queue = true) {
        //Debug.Log("Logging '" + str + "'");
        if (add_to_queue) { 
            log_queue.Add(str);
            log_buffer.Add(str);
            if (log_buffer.Count > 70) log_buffer.RemoveAt(0);
        }

        //Handle Scrollable log
        if (txt_container_scrollable_CG.interactable) {
            if (add_to_queue) {
                if (txt_container_scrollable_txt_template.parent.childCount > 71) 
                    Destroy(txt_container_scrollable_txt_template.parent.GetChild(1).gameObject);

                var t = Instantiate(txt_container_scrollable_txt_template.gameObject, txt_container_scrollable_txt_template.parent);
                t.GetComponent<Text>().text = str; t.SetActive(true);
            }
            return;
        }

        if (DOTween.IsTweening(txt_containerRT)) {
            StartCoroutine(Log_sub_wait_for_complete()); return;
        }

        str = log_queue[0];
        GameObject txt_new_obj = Instantiate (txt.gameObject, txt_container);
        Text txt_new = txt_new_obj.GetComponent<Text>();
        txt_new.text = str;
        txt_containerRT.DOKill();
        txt_containerCurY += 30f;
        txt_containerRT.DOAnchorPosY(txt_containerCurY, 0.1f).SetUpdate(true).OnComplete( ()=> {
            log_queue.RemoveAt(0);
            //txt_new.transform.SetParent(txt_container);
            txt_new.rectTransform.anchoredPosition = new Vector2 (0f, 0 - txt_containerCurY + 210f);
            txt_new.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            txt_new_obj.SetActive(true); txt_new.color = Color.white;
            txt_new.DOFade(0f, 2f).SetDelay(3f).SetUpdate(true).OnComplete(()=> { Destroy ( txt_new_obj ); });
        });
    }
    IEnumerator Log_sub_wait_for_complete () {
        while (DOTween.IsTweening(txt_containerRT)) yield return new WaitForEndOfFrame();
        Log_sub("", false);
    }
    public void Show_Hide_Scrollable_Log() {
        if (DOTween.IsTweening(txt_container_scrollable_CG)) return;
        if ( Mathf.Approximately(txt_container_scrollable_CG.alpha, 0f)) {
            //Show
            txt_container_scrollable_CG.DOFade(1f, 0.3f);
            txt_container_scrollable_CG.interactable = true;
            txt_container_scrollable_CG.blocksRaycasts = true;

            foreach (string s in log_buffer) {
                var t = Instantiate(txt_container_scrollable_txt_template.gameObject, txt_container_scrollable_txt_template.parent);
                t.GetComponent<Text>().text = s; t.SetActive(true);
            }

            txt_containerRT.DOKill(true);
            for (int i = txt_container.childCount - 1; i >= 0; i--) {
                txt_container.GetChild(i).GetComponent<Text>().DOKill(true);
            }
            log_queue.Clear();
        } else {
            //Hide
            log_queue.Clear();
            txt_container_scrollable_CG.DOFade(0f, 0.3f);
            txt_container_scrollable_CG.interactable = false;
            txt_container_scrollable_CG.blocksRaycasts = false;

            Clear_Scrollable_Log();
        }
    }
    public void Clear_Scrollable_Log() {
        for (int i = txt_container_scrollable_txt_template.parent.childCount - 1; i >= 1; i--)  {
            Destroy( txt_container_scrollable_txt_template.parent.GetChild(i).gameObject );
        }
    }

    public static void ShowDamage(Vector3 pos, int damage, bool is_bot_damage = true, string enemy_name = "") {
        GameObject d_text = Instantiate(inst.Damage_Settings.Damage_Text, inst.Damage_Settings.Damage_Text.transform.parent);
        d_text.transform.SetSiblingIndex(0);
        d_text.SetActive(true);

        string log = "";
        Text txt = d_text.GetComponent<Text>();
        txt.text = damage.ToString();
        if (is_bot_damage) {
            log += "Bot ";
            if (damage > 0)      { txt.color = inst.Damage_Settings.BOT_Negative; log += "received " + damage + " damage"; }
            else if (damage < 0) { txt.color = inst.Damage_Settings.BOT_Positive; log += "gained " + damage + " HP"; }
            else                 { txt.color = inst.Damage_Settings.BOT_Zerro; log += "received " + damage + " damage"; }
            //inst.Log_sub(log); - BOT HP Logging is handled by Hud.Update
        } else {
            log += "Enemy " + enemy_name + " ";
            if (damage > 0)      { txt.color = inst.Damage_Settings.Enemy_Positive; log += "received " + damage + " damage"; }
            else if (damage < 0) { txt.color = inst.Damage_Settings.Enemy_Negative; log += "gained " + damage + " HP"; }
            else                 { txt.color = inst.Damage_Settings.Enemy_Zerro; log += "received " + damage + " damage"; }
            inst.Log_sub(log);
        }

        //position
        RectTransform CanvasRect = inst.transform.parent.GetComponent<RectTransform>();
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(pos);
        Vector2 ScreenPosition = new Vector2( ViewportPosition.x * CanvasRect.sizeDelta.x, ViewportPosition.y * CanvasRect.sizeDelta.y);

        var rt = d_text.GetComponent<RectTransform>();
        rt.anchoredPosition = ScreenPosition;
        rt.DOAnchorPosY(ScreenPosition.y + 200f, 2f).OnComplete(()=> Destroy(d_text) );
    }

    public void ShowHide_HudElements(bool hide = false) {
        var rt = GetComponent<RectTransform>();
        rt.DOKill();
        if(!hide) {
            rt.DOAnchorPosX(-30f, 0.75f).SetUpdate(true); hud_is_hidden = false;
        } else {
            rt.DOAnchorPosX(-680f, 0.75f).SetUpdate(true); hud_is_hidden = true;
        }
    }
}
