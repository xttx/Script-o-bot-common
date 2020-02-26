using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class Options : MonoBehaviour //, IPointerClickHandler
{
    public Texture[] Level_Preview = null;    

    CanvasGroup cv_opt;
    CanvasGroup cv_log;
    CanvasGroup cv_funClass;
    RectTransform rt_opt;
    RectTransform rt_funClass;
    GameObject user_script_list_content;
    //GameObject load_game_list_content;
    Toggle[] save_dir_location_toggles = new Toggle[3];
    [HideInInspector] public Transform Time_Control_Panel = null;
    string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Script-o-Bot\\UserScripts";

    RectTransform options_Button;
    [HideInInspector] static RectTransform Bottom_Buttons_Panel;
    [HideInInspector] static GameObject Bottom_Buttons_Panel_Expand_Button;
    [HideInInspector] static RectTransform hideTutorialDialog_Button;

    GameObject log_container;

    InputField scriptSaveName;
    TMP_InputField script_text = null;
    Text_Editor ScriptEditor_My = null;

    bool need_show = false;

    // Start is called before the first frame update
    void Start()
    {
        //Check if/where we can save files
        #region "Check if/where we can save files"
        if (!PlayerPrefs.HasKey("UserScript_Dir")) PlayerPrefs.SetString("UserScript_Dir", Engine.user_script_dir);
        bool userScriptDir_changed = PlayerPrefs.GetString("UserScript_Dir").Trim().ToLower() != Engine.user_script_dir.Trim().ToLower();
        if (userScriptDir_changed) PlayerPrefs.SetString("UserScript_Dir", Engine.user_script_dir);

        if (!PlayerPrefs.HasKey("Can_Write_To_UserScript_Dir") || userScriptDir_changed) {
            Debug.Log("Check if userscript dir is writable.");
            string p = Path.GetFullPath(Engine.user_script_dir + "/tmp.test");
            File.WriteAllText(p, "test");
            if (File.Exists(p)) {
                File.Delete(p);
                PlayerPrefs.SetInt("Can_Write_To_UserScript_Dir", 1);
            } else {
                PlayerPrefs.SetInt("Can_Write_To_UserScript_Dir", 0);
            }
        }

        if (!PlayerPrefs.HasKey("Can_Write_To_AppData_Dir")) {
            Debug.Log("Check if appdata dir is writable.");
            bool dir1_was_created = false;
            bool dir2_was_created = false;
            string d1 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Script-o-Bot";
            if (!Directory.Exists(d1)) { Directory.CreateDirectory(d1); dir1_was_created = true; }

            string d2 = d1 + "\\UserScripts";
            if (!Directory.Exists(d2)) { Directory.CreateDirectory(d2); dir2_was_created = true; }

            string p = d2 + "\\tmp.test";
            File.WriteAllText(p, "test");
            if (File.Exists(p)) {
                File.Delete(p);
                PlayerPrefs.SetInt("Can_Write_To_AppData_Dir", 1);
            } else {
                PlayerPrefs.SetInt("Can_Write_To_AppData_Dir", 0);
            }

            if (dir2_was_created) Directory.Delete(d2);
            if (dir1_was_created) Directory.Delete(d1);
        }
        #endregion

        //Reference to script editor, and handler OnScriptChanged
        #region "Reference to script editor, and handler OnScriptChanged"
        GameObject tmp = GameObject.Find("Script_InputField_TMPRO");
		if (tmp != null) {
			script_text = tmp.GetComponent<TMPro.TMP_InputField>();
		} else {
			ScriptEditor_My = GameObject.Find("Text_Editor").GetComponent<Text_Editor>();
            ScriptEditor_My.OnTextChanged += (o, e) => { 
                var cur_class = Compiler.cur_active_class;
                if (cur_class != null && !cur_class.changed && cur_class.saved) {
                    //Mark current class as changed
                    cur_class.changed = true;
                    var cnt = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0).parent;
                    var bdh = cnt.GetComponentsInChildren<Button_Data_Holder>().Where(x => x.int1 == Compiler.cur_active_class.id).FirstOrDefault();
                    bdh.transform.GetChild(1).GetComponent<Text>().color = Color.yellow;
                }
            };
		}
        #endregion

        #region "References and reset of some values"
        cv_opt = GetComponent<CanvasGroup>();
        cv_funClass = GameObject.Find("Panel_ClassFuncMan").GetComponent<CanvasGroup>();
        log_container = GameObject.Find("Panel_Log");
        cv_log = log_container.GetComponent<CanvasGroup>();
        rt_opt = GetComponent<RectTransform>();
        rt_funClass = GameObject.Find("Panel_ClassFuncMan").GetComponent<RectTransform>();
        rt_funClass.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        rt_funClass.anchoredPosition = new Vector2(490f, -460f);
        options_Button = GameObject.Find("Panel_Options_Button").transform.GetChild(0).GetComponent<RectTransform>();
        Bottom_Buttons_Panel = GameObject.Find("Bottom_Buttons").GetComponent<RectTransform>();
        //Bottom_Buttons_Panel.offsetMin = new Vector2(1370f, Bottom_Buttons_Panel.offsetMin.y);
        Bottom_Buttons_Panel_Expand_Button = Bottom_Buttons_Panel.transform.Find("Borders").Find("Button").gameObject;
        //Bottom_Buttons_Panel_Expand_Button.GetComponent<Image>().color = new Color32(255,255,255,0);
        //Bottom_Buttons_Panel_Expand_Button.GetComponent<Button>().interactable = false;
        hideTutorialDialog_Button = GameObject.Find("Panel_HideDialog").GetComponent<RectTransform>();
        //hideTutorialDialog_Button.anchoredPosition -= new Vector2(0f, 150f);
        
        Time_Control_Panel = GameObject.Find("Panel_Control_Time").transform;
        Time_Control_Panel.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((b)=> { if(b) Time.timeScale = 1f; });
        Time_Control_Panel.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((b)=> { if(b) Time.timeScale = 2f; });
        Time_Control_Panel.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((b)=> { if(b) Time.timeScale = 3f; });
        Time_Control_Panel.GetChild(3).GetComponent<Toggle>().onValueChanged.AddListener((b)=> { if(b) Time.timeScale = 5f; });
        #endregion

        #region "Init references in GlobalOptions"
        //Enable all tabs to allow GameObject.Find all needed objects
        for (int i = 3; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }

        //Global Options
        Transform Panel_ScriptEditor = GameObject.Find("Panel_ScriptEditor").transform;
        Options_Global_Static.ScriptEditor_My = ScriptEditor_My;

        //Options - volume
        Options_Global_Static.Volume_Global_sld = GameObject.Find("Audio_GLB").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.Volume_Music_sld = GameObject.Find("Audio_MUS").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.Volume_FX_sld = GameObject.Find("Audio_SFX").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.aud_mus_main = Camera.main.GetComponent<AudioSource>();
        Options_Global_Static.aud_mus_levelComplete = GameObject.Find("Compiler").GetComponent<AudioSource>();
        Options_Global_Static.aud_fx = Camera.main.GetComponents<AudioSource>()[1];
        Options_Global_Static.aud_fx2 = Camera.main.GetComponents<AudioSource>()[2];
        Options_Global_Static.aud_fx_bot = GameObject.Find("Robot").GetComponent<AudioSource>();
        Options_Global_Static.aud_fx_type = GameObject.Find("Dialog_Text").GetComponent<AudioSource>();
        Options_Global_Static.volume_hud_mask = GameObject.Find("Panel_Volume").transform.GetChild(0).GetComponent<RectTransform>();

        //Options - script-editor
        Options_Global_Static.ed_activate = Panel_ScriptEditor.Find("Dropdown_ed_act").GetComponent<Dropdown>();
        Options_Global_Static.ed_deactivate = Panel_ScriptEditor.Find("Dropdown_ed_deact").GetComponent<Dropdown>();
        Options_Global_Static.ed_camera_controls = Panel_ScriptEditor.Find("Dropdown_ed_cam_act").GetComponent<Dropdown>();
        Options_Global_Static.Editor_Opt_FontSize = Panel_ScriptEditor.Find("Font_Size").GetComponent<Slider>();
        Options_Global_Static.Editor_Opt_TabSpaces = Panel_ScriptEditor.Find("Tab_Spaces").GetComponent<Slider>();
        Options_Global_Static.Editor_Opt_CursorWidth = Panel_ScriptEditor.Find("Cursor_Width").GetComponent<Slider>();
        Options_Global_Static.Intellisense_toggle = Panel_ScriptEditor.Find("Intellisense_toggle").GetComponent<Toggle>();
        Options_Global_Static.Intellisense_max_sug = Panel_ScriptEditor.Find("Intellisense_max_sug").GetComponent<Slider>();
        Options_Global_Static.Editor_Autoclose = Panel_ScriptEditor.Find("Autoclose_toggle").GetComponent<Toggle>();

        //Options - bot
        Options_Global_Static.BOT_FastRebase = GameObject.Find("FastRebase_toggle").GetComponent<Toggle>();

        //Options - sub-panels
        //Options_Global_Static.tab_continer = transform.GetChild(0);
        Options_Global_Static.button_hotkeys = GameObject.Find("Button_Hotkeys").GetComponent<Button>();
        Options_Global_Static.button_colors = GameObject.Find("Button_EditColors").GetComponent<Button>();
        Options_Global_Static.button_colors_ui = GameObject.Find("Button_UI_Colors").GetComponent<Button>();
        Options_Global_Static.SubPanel_Hotkeys = GameObject.Find("Panel_Edit_Hotkeys").transform;
        Options_Global_Static.SubPanel_EditorColors = GameObject.Find("Panel_Editor_Colors").transform;
        Options_Global_Static.SubPanel_UIColors = GameObject.Find("Panel_UI_Colors").transform;
        Options_Global_Static.SubPanel_Blokator = GameObject.Find("Panel_Blokator").GetComponent<Image>();
        Options_Global_Static.SubPanel_BlokatorUI = GameObject.Find("Panel_Blokator_UI").GetComponent<Image>();


        //Load / save game
        Options_Global_Static.load_game_list_content = GameObject.Find("SaveGame_Template");
        //Options_Global_Static.Level_Preview = Level_Preview;
        Options_Global_Static.Load_Level.RemoveAllListeners(); 
        Options_Global_Static.Load_Level.AddListener((i) => { LoadLevel(i); } );

        Options_Global_Static.Init();
        #endregion

        #region "Load/Save script"
        scriptSaveName = GameObject.Find("ScriptSave_InputField").GetComponent<InputField>();
        user_script_list_content = GameObject.Find("Script_List_Text"); user_script_list_content.SetActive(false);

        Transform panel_save_dir = GameObject.Find("Panel_SaveDir").transform;
        save_dir_location_toggles[0] = panel_save_dir.GetChild(0).GetComponent<Toggle>();
        save_dir_location_toggles[1] = panel_save_dir.GetChild(1).GetComponent<Toggle>();
        save_dir_location_toggles[2] = panel_save_dir.GetChild(2).GetComponent<Toggle>();
        if (PlayerPrefs.GetInt("Can_Write_To_UserScript_Dir") == 0) {
            save_dir_location_toggles[1].isOn = true;
            save_dir_location_toggles[0].interactable = false;
        }
        if (PlayerPrefs.GetInt("Can_Write_To_AppData_Dir") == 0) {
            if (save_dir_location_toggles[1].isOn) save_dir_location_toggles[2].isOn = true;
            save_dir_location_toggles[1].interactable = false;
        }
        if (PlayerPrefs.HasKey("UserSave_Location")) {
            int i = PlayerPrefs.GetInt("UserSave_Location");
            if (save_dir_location_toggles[i].interactable) save_dir_location_toggles[i].isOn = true;
        }
        for (int i = 0; i < 3; i++) {
            save_dir_location_toggles[i].onValueChanged.AddListener((b) => {
                if (!b) return;

                int save_location = 0;
                if      (save_dir_location_toggles[1].isOn) save_location = 1;
                else if (save_dir_location_toggles[2].isOn) save_location = 2;
                PlayerPrefs.SetInt("UserSave_Location", save_location);
                Fill_user_scripts_from_files();
                Fill_classes_from_files();
            });
        }
        #endregion

        //Colorize bot-o-pedia
        var bpd = transform.Find("Tab5_Content");
        for (int i = 1; i < bpd.childCount-2; i++) {
            var t = bpd.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            t.text = Scenario.Code_HighLight(t.text);
        }

        //Init tabs
        #region "Init Tabs"
        //Disable all but first page
        transform.GetChild(2).gameObject.SetActive(true);
        for (int i = 4; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }

        //toggle off all but first toggle
        transform.GetChild(1).GetComponent<ASTT.Tab_Panel>().tab_active = 0;
        #endregion

        //Init
        cv_opt.alpha = 0f;
        cv_opt.interactable = false;
        cv_opt.blocksRaycasts = false;
        rt_opt.anchoredPosition = new Vector2(0f, -200f);

        Fill_classes_from_files();
    }

    // Update is called once per frame
    void Update()
    {
        Options_Global_Static.Update();

        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            if (Dialog_Panel.IsShown) { Dialog_Panel.InvokeButtonPress(2); return; }
            if (Dialog_Panel_Txt.IsShown) { Dialog_Panel_Txt.InvokeCancel(); return; }
            if (Error_Panel.IsShown) { Error_Panel.HideError_st(); return; }
            if (Options_Global_Static.SubPanelClose()) { return; }

            if (Is_Options_Panel_Shown) { Hide(); return; }

            if (Is_Log_Panel_Shown)     { Panel_Log_Hide(); }
            else if (!ScriptEditor_My.Close_Intellisense()) {
                if      (Is_ClassFunc_Panel_Shown)  { Panel_ClassFunc_Hide(); }
                else if (!Is_Options_Panel_Shown && !DOTween.IsTweening(cv_opt) && !DOTween.IsTweening(cv_log)) { need_show = true; }
            }
        }

        //Time scale
        if (Engine.check_key(Engine.Key.Script_Time_x1)) { Time_Control_Panel.GetChild(0).GetComponent<Toggle>().isOn = true; }
		if (Engine.check_key(Engine.Key.Script_Time_x2)) { Time_Control_Panel.GetChild(1).GetComponent<Toggle>().isOn = true; }
		if (Engine.check_key(Engine.Key.Script_Time_x3)) { Time_Control_Panel.GetChild(2).GetComponent<Toggle>().isOn = true; }
		if (Engine.check_key(Engine.Key.Script_Time_x5)) { Time_Control_Panel.GetChild(3).GetComponent<Toggle>().isOn = true; }

        need_show = need_show || Engine.check_key(Engine.Key.Main_Menu);
        if (need_show && !Camera_Controller.level_complete_shown) {
            //Show menu
            cv_opt.DOKill();
            rt_opt.DOKill();
            cv_opt.DOFade(1f, 0.7f).SetUpdate(true);
            //container_rt.anchoredPosition = new Vector2(0f, -200f);
            rt_opt.DOAnchorPosY(0f, 0.7f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(()=>{
                cv_opt.blocksRaycasts = true; cv_opt.interactable = true;
            });


            //Fill user scripts from files
            Fill_user_scripts_from_files();
        }

        need_show = false;
    }

    public void Show()
    {
        if (!cv_opt.blocksRaycasts && !DOTween.IsTweening(cv_opt)) { 
            need_show = true; 
            options_Button.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(()=>{
                options_Button.rotation = Quaternion.identity;
            });
        }
    }

    public void Show_Dialog_Log()
    {
        CanvasGroup cg_log = log_container.GetComponent<CanvasGroup>();
        if (!cg_log.blocksRaycasts && !DOTween.IsTweening(cg_log) && !cv_opt.blocksRaycasts) { 
            RectTransform log_btn = GameObject.Find("Panel_Log_Button").transform.GetChild(0).GetComponent<RectTransform>();
            log_btn.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f).SetLoops(2,LoopType.Yoyo).SetUpdate(true);

            cg_log.DOKill();
            cg_log.DOFade(1f, 0.7f).SetUpdate(true).OnComplete( ()=> { cg_log.blocksRaycasts = true; });

            Transform category_content = log_container.transform.GetChild(0).GetChild(0).GetChild(0);
            GameObject text_template_cat = category_content.GetChild(0).gameObject;
            Text content_text = log_container.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();

            //Fill dialog texts until current step index
            int scene_index = 0;
            string last_important_str = "";
            List<string> contents = new List<string>();
            for (int i = 0; i <= Engine.current_step; i++) {
                if (scene_index >= contents.Count()) contents.Add("");

                var st = Scenario.steps[i].step_type;
                if (st == Scenario.step_type.dialog || st == Scenario.step_type.dialog_continue || st == Scenario.step_type.dialog_important || st == Scenario.step_type.dialog_important_continue) {
                    string str = Scenario.steps[i].str_param1;

                    if (st == Scenario.step_type.dialog_important) last_important_str = str;
                    if (st == Scenario.step_type.dialog_important_continue) last_important_str += str;

                    if (st != Scenario.step_type.dialog_important_continue)
                        contents[scene_index] += str + "\n\n";
                    else
                        contents[scene_index] += last_important_str + "\n\n";
                }
                if (st == Scenario.step_type.level_complete_show) scene_index++;
            }

            //Unfortunately can not just add new level to the list, because last shown level will not be updated
            //  if new level will be loaded at the middle of previous level

            //Remember last selected level
            int last_selected_index = -1;
            var last_selected = category_content.GetComponentsInChildren<Toggle>().Where(t => t.isOn).FirstOrDefault();
            if (last_selected != null) last_selected_index = last_selected.transform.GetSiblingIndex();

            //Destroy levels
            for (int i = category_content.childCount - 1; i >= 1; i--)
                DestroyImmediate(category_content.GetChild(i).gameObject);

            //Fill level list until current level
            for (int i = 0; i < contents.Count; i++) {
                var txt = Instantiate(text_template_cat, category_content);
                var txt_hidden = txt.transform.GetChild(2).GetComponent<Text>();
                txt_hidden.text = Scenario.Code_HighLight(contents[i], null);
                txt.GetComponent<Toggle>().onValueChanged.AddListener( (b)=> { 
                    if (b) content_text.text = txt_hidden.text;
                });
                txt.transform.GetChild(1).GetComponent<Text>().text = Engine.Level_names[i];
                txt.SetActive(true);
            }

            //Update current level info
            //var last_tgl = category_content.GetChild(category_content.childCount - 1).GetComponent<Toggle>();
            //last_tgl.transform.GetChild(2).GetComponent<Text>().text = contents[contents.Count-1];
            //if (last_tgl.isOn) content_text.text = contents[contents.Count-1];

            //If there is no active toggles - activate last toggle
            var last_tgl = category_content.GetChild(category_content.childCount - 1).GetComponent<Toggle>();
            if (last_selected_index >= 0 && category_content.childCount > last_selected_index) {
                category_content.GetChild(last_selected_index).GetComponent<Toggle>().isOn = true;
            } else {
                last_tgl.isOn = true;
            }
            //if (category_content.GetComponentsInChildren<Toggle>().Where(t => t.isOn).Count() == 0) last_tgl.isOn = true;

            //content_text.text = content_list[0].text;
        }
    }

    public void Hide() {
        EventSystem.current.SetSelectedGameObject(null);

        cv_opt.interactable = false;
        cv_opt.blocksRaycasts = false;
        cv_opt.DOKill();
        rt_opt.DOKill();
        cv_opt.DOFade(0f, 0.7f).SetUpdate(true);
        rt_opt.DOAnchorPosY(-200f, 0.7f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

#region ClassFunc Show/Load/Save/Delete
    public void Show_Dialog_ClassFunc()
    {
        Engine.desktop_manager.Restore_Original_Win_Geometry(1);

        rt_funClass.DOAnchorPos(new Vector2(-270f, 0f), 0.3f).SetUpdate(true);
        rt_funClass.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetUpdate(true);
        cv_funClass.DOFade(1f, 0.3f).SetUpdate(true).OnComplete(()=>{
            cv_funClass.interactable = true;
            cv_funClass.blocksRaycasts = true;
        });

        // //Update currently edited class from script_text_field
        //var cur_class = Compiler.classes.Where( (x)=>x.active ).FirstOrDefault();
        //cur_class.content = Get_Script_Text();
        // string[] cur_class_func_signatures = cur_class == null ? new string[]{} : cur_class.GetFuncSignatures();
        // string[] cur_class_func_names = cur_class_func_signatures.Count() <= 0 ? new string[]{} : cur_class_func_signatures.Select(x=> string.Join("", classes_info.GetFuncNameAndParamFromSignature(x))).ToArray();

        //This will also update currently edited class from script_text_field
        List_Func();

        //Transform f_template = GameObject.Find("Scroll View_Func").transform.GetChild(0).GetChild(0).GetChild(0);
        Transform c_template = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0);
        // Button func_save_btn = f_template.parent.parent.parent.parent.Find("Button_F_S").GetComponent<Button>();
        // Button func_delete_btn = f_template.parent.parent.parent.parent.Find("Button_F_D").GetComponent<Button>();

        //Destroy old buttons
        //for (int i = f_template.parent.childCount - 1; i >= 1; i--) { Destroy(f_template.parent.GetChild(i).gameObject); }
        for (int i = c_template.parent.childCount - 1; i >= 1; i--) { Destroy(c_template.parent.GetChild(i).gameObject); }

        // //Create new func buttons
        // f_template.GetComponent<Toggle>().enabled = true;
        // f_template.transform.GetChild(1).gameObject.SetActive(true);
        // f_template.transform.GetChild(3).gameObject.SetActive(true);
        // f_template.transform.GetChild(2).GetComponent<Text>().text = "--- Saved Functions ---";
        // //List saved functions
        // List<string> saved_functions_names = new List<string>();
        // foreach (var fnc in Compiler.functions) {
        //     GameObject new_obj = Instantiate(f_template.gameObject, f_template.parent);
        //     new_obj.transform.GetChild(2).GetComponent<Text>().text = fnc.original_name;

        //     Button btn = new_obj.transform.GetChild(1).GetComponent<Button>();
        //     btn.interactable = !cur_class_func_names.Contains(fnc.original_name);
        //     if (btn.interactable) btn.onClick.AddListener(()=> Add_Func_To_Class( btn.transform.parent.GetComponent<Button_Data_Holder>() ));
        //     //Debug.Log("Cur func = '" + fnc.original_name + "', Cur class funcs: '" + string.Join("|||", cur_class_funcs) + "'");

        //     Button_Data_Holder bdh = new_obj.GetComponent<Button_Data_Holder>();
        //     bdh.int1 = fnc.id;

        //     new_obj.GetComponent<Toggle>().onValueChanged.AddListener((b)=> {
        //         if (!b) return;
        //         func_save_btn.interactable = false; func_delete_btn.interactable = true;
        //     });
        // }
        // GameObject Func_Label2 = Instantiate(f_template.gameObject, f_template.parent);
        // Func_Label2.GetComponent<Toggle>().enabled = false;
        // Func_Label2.transform.GetChild(1).gameObject.SetActive(false); //Checkbox
        // Func_Label2.transform.GetChild(3).gameObject.SetActive(false); //Eye
        // Func_Label2.transform.GetChild(2).GetComponent<Text>().text = "--- Functions from classes ---";
        // //List unsaved functions (from all classes)
        // foreach (var cl in Compiler.classes) {
        //     string[] fs = cl.GetFuncSignatures();
        //     if (fs == null || fs.Count() == 0) continue;
        //     foreach (string f in fs) {
        //         if (cl.id == 0 && (f.StartsWith("void Start ") || f.StartsWith("void Start("))) continue;
        //         GameObject new_obj = Instantiate(f_template.gameObject, f_template.parent);
                
        //         new_obj.transform.GetChild(2).GetComponent<Text>().text = cl.GetClassName() + "." + string.Join("", classes_info.GetFuncNameAndParamFromSignature(f));
        //         //new_obj.transform.GetChild(1).gameObject.SetActive(false);

        //         Button btn = new_obj.transform.GetChild(1).GetComponent<Button>();
        //         btn.interactable = cl != cur_class; // && cur_class_funcs.Contains(f);
        //         btn.onClick.AddListener(()=> Add_Func_To_Class( btn.transform.parent.GetComponent<Button_Data_Holder>() ));
        //         //Debug.Log("Cur func = '" + f + "', Cur class funcs: '" + string.Join("|||", cur_class_funcs) + "'");

        //         //Transform text - remove class name
        //         string name = f;
        //         if (name.Contains(".")) name = name.Substring( name.LastIndexOf(".") + 1 );
                
        //         Button_Data_Holder bdh = new_obj.GetComponent<Button_Data_Holder>();
        //         bdh.int2 = cl.id; bdh.str1 = name;

        //         new_obj.GetComponent<Toggle>().onValueChanged.AddListener((b)=> {
        //             if (!b) return;
        //             func_save_btn.interactable = true; func_delete_btn.interactable = false;
        //         });
        //     }
        // }
        // f_template.GetComponent<Toggle>().enabled = false;
        // f_template.transform.GetChild(1).gameObject.SetActive(false); //Checkbox
        // f_template.transform.GetChild(3).gameObject.SetActive(false); //Eye

        //Create new class buttons
        var class_names = Compiler.classes.Where((x)=>x.id != 0);
        foreach (var class_info in class_names.OrderBy((c)=>c.GetClassName()) ) {
            GameObject new_obj = Instantiate(c_template.gameObject, c_template.parent);
            new_obj.GetComponent<Button_Data_Holder>().int1 = class_info.id;
            Text txt = new_obj.transform.GetChild(1).GetComponent<Text>();
            txt.text = class_info.GetClassName();
            if      (!class_info.saved)   txt.color = Color.red;
            else if (class_info.changed)  txt.color = Color.yellow;
            else                          txt.color = new Color(221, 221, 221, 255);

            if (class_info.active) new_obj.GetComponent<Toggle>().isOn = true; else new_obj.GetComponent<Toggle>().isOn = false;
            //class_info.associated_class_list_text = txt;

            Toggle enabled = new_obj.transform.GetChild(2).GetComponent<Toggle>();
            enabled.enabled = true;
            if (class_info.enabled) enabled.isOn = true; else enabled.isOn = false;
            enabled.onValueChanged.AddListener((bool b)=> class_info.enabled = enabled.isOn );
        }
    }

    public void Fill_classes_from_files() {
        //Get saved class list depending on where we store scripts
        //Debug.Log("Fill classes");
        string[] classes = new string[]{};
        string[] functions = new string[]{};
        string class_script_dir = Engine.user_script_dir + "/Classes";
        string class_appdata_dir = appDataDir + "/Classes";
        string dir = "";
        if (save_dir_location_toggles[0].isOn && Directory.Exists(class_script_dir))
            dir = class_script_dir;
        else if (save_dir_location_toggles[1].isOn && Directory.Exists(class_appdata_dir))
            dir = class_appdata_dir;
        // else if (PlayerPrefs.HasKey("ClassesNames"))
        //     classes = PlayerPrefs.GetString("ClassesNames").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);

        Compiler.functions.Clear();
        Compiler.classes.ForEach(x => { x.saved = false; x.changed = true; } ); //If we switch save location, we renew all existing classes

        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) {
            //Files
            if (dir == "") return;

            //Functions
            if (File.Exists(dir + "/!functions.txt")) {
                functions = File.ReadAllText(dir + "/!functions.txt").Split(new string[]{"\r\n******************************\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
            }

            //Classes Files
            classes = Directory.GetFiles(dir);
            foreach (var f in classes) {
                if (System.IO.Path.GetFileName(f).StartsWith("!")) continue;

                StreamReader r = File.OpenText(f);
                classes_info ci = new classes_info( r.ReadToEnd() );
                r.Close();

                ci.saved_fileName = Path.GetFileNameWithoutExtension(f);
                ci.saved = true;

                if (ci.GetClassName() != "Main") {
                    var c = Compiler.classes.Where(x => x.saved_fileName == ci.saved_fileName).FirstOrDefault();
                    if (c != null) { c.content = ci.content; c.saved = true; continue; } 

                    ci.id = Compiler.classes.Count + 1;
                    Compiler.classes.Add(ci);
                } else {
                    ci = Compiler.classes[0];
                }
            }
        } else {
            //Registry

            //Functions
            if (PlayerPrefs.HasKey("Saved_Functions")) {
                functions = PlayerPrefs.GetString("Saved_Functions").Split(new string[]{"\r\n******************************\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
            }

            //Classes
            if (PlayerPrefs.HasKey("Saved_Class_Names")) {
                string saved_names = PlayerPrefs.GetString("Saved_Class_Names");
                string[] saved_names_arr = saved_names.Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var n in saved_names_arr) {
                    classes_info ci = new classes_info( PlayerPrefs.GetString("Saved_Class_" + n) );

                    ci.saved_fileName = Path.GetFileNameWithoutExtension(n);
                    ci.saved = true;

                    if (ci.GetClassName() != "Main") {
                        var c = Compiler.classes.Where(x => x.saved_fileName == ci.saved_fileName).FirstOrDefault();
                        if (c != null) { c.content = ci.content; c.saved = true; continue; }

                        ci.id = Compiler.classes.Count + 1;
                        Compiler.classes.Add(ci);
                    } else {
                        ci = Compiler.classes[0];
                    }
                }
            }
        }

        //Functions
        foreach (var func in functions) {
            string first_line = func.Split(new char[]{'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (first_line == null) continue;

            int ind_brackets = first_line.IndexOf("{");
            string signature = ind_brackets > 0 ? first_line.Substring(0, first_line.IndexOf("{")).Trim() : first_line;
            string[] f = classes_info.GetFuncNameAndParamFromSignature(signature);

            classes_info func_info = new classes_info(func, f[0] + f[1]);
            func_info.id = Compiler.functions.Count;
            Compiler.functions.Add(func_info);
        }
    }

    public void Create_New_Class() {
        classes_info ci = new classes_info("");
        ci.changed = false; ci.id = Compiler.classes.Count + 1;
        Compiler.classes.Add(ci);
        ci.saved_fileName = new string(Enumerable.Repeat("ABCDEF1234567890", 32).Select(s => s[Random.Range(0, 16)]).ToArray());

        Transform c_template = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0);
        GameObject new_obj = Instantiate(c_template.gameObject, c_template.parent);
        new_obj.GetComponent<Toggle>().isOn = false;
        new_obj.GetComponent<Button_Data_Holder>().int1 = ci.id;
        Text txt = new_obj.transform.GetChild(1).GetComponent<Text>();
        txt.text = ci.GetClassName();
        txt.color = Color.red;

        //Sort - move to correct position
        int new_sibling_index = new_obj.transform.GetSiblingIndex();
        int p = new_sibling_index - 1;
        for (int i = p; i > 0; i--) {
            var t = c_template.parent.GetChild(i).GetChild(1).GetComponent<Text>().text;
            if (string.Compare(txt.text, t) >= 0) break;
            new_sibling_index = i;
        }
        new_obj.transform.SetSiblingIndex(new_sibling_index);
    }
    public void Change_Active_Class(Toggle tgl) {
        if (!tgl.isOn) return;

        int id = tgl.GetComponent<Button_Data_Holder>().int1;
        classes_info cl = Compiler.classes.Where((x) => x.id == id).FirstOrDefault();
        if(cl == null) return;

        var active_class = Compiler.cur_active_class;
        active_class.active = false;
        active_class.content = Get_Script_Text();

        //Change old active class name label, if it was changed
        if (active_class.id != 0) {
            var link_tgls = tgl.transform.parent.GetComponentsInChildren<Button_Data_Holder>().Where((d)=>d.int1 == active_class.id);
            if (link_tgls != null)
                foreach (Text link_txt in link_tgls.Select( (y)=>y.transform.GetChild(1).GetComponent<Text>() )) link_txt.text = active_class.GetClassName();
        }

        //We set the cur_class to null, to swallow ScriptEditor.OnTextChanged event
        Compiler.cur_active_class = null;
        Set_Script_Text ( cl.content, true ); 
        Compiler.cur_active_class = cl;
        Compiler.cur_active_class.active = true;
        Disable_Unavailable_Func();
    }
    public void Save_Class() {
        Transform c_cnt = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0);
        Toggle tgl = c_cnt.GetComponentsInChildren<Toggle>().Where((x) => x.isOn && x.transform.parent == c_cnt).FirstOrDefault();

        // var a = c_cnt.GetComponentsInChildren<Toggle>();
        // var b = a.Where((x)=>x.isOn).FirstOrDefault();
        // Debug.Log("Found " + a.Count().ToString() + " toggles");
        // Debug.Log("Active is null: " + (b == null).ToString());
        // Debug.Log("Data is null: " + (b.GetComponent<Button_Data_Holder>() == null).ToString());

        int id = tgl.GetComponent<Button_Data_Holder>().int1;
        classes_info cl = Compiler.classes.Where((x)=>x.id == id).FirstOrDefault();
        cl.content = Get_Script_Text();
        string class_name = cl.GetClassName();
        tgl.transform.GetChild(1).GetComponent<Text>().text = class_name;

        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) {
            string dir = "";
            if      (save_dir_location_toggles[0].isOn) dir = Engine.user_script_dir + "/Classes";
            else if (save_dir_location_toggles[1].isOn) dir = appDataDir + "/Classes";

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(dir + "/" + cl.saved_fileName + ".txt", cl.content);
        } else {
            string saved_names = "";
            if (PlayerPrefs.HasKey("Saved_Class_Names")) saved_names = PlayerPrefs.GetString("Saved_Class_Names");
            if (!saved_names.Contains(cl.saved_fileName + ";")) saved_names += cl.saved_fileName + ";";
            PlayerPrefs.SetString("Saved_Class_Names", saved_names);
            PlayerPrefs.SetString("Saved_Class_" + cl.saved_fileName, cl.content);
        }
        cl.changed = false; cl.saved = true;

        //Set color to 'saved'
        tgl.transform.GetChild(1).GetComponent<Text>().color = new Color(221, 221, 221, 255);

        //Sort - move to correct position
        int new_sibling_index = c_cnt.childCount - 1;
        tgl.transform.SetSiblingIndex(new_sibling_index);
        for (int i = (new_sibling_index - 1); i > 0; i--) {
            var t = c_cnt.GetChild(i).GetChild(1).GetComponent<Text>().text;
            if (string.Compare(class_name, t) >= 0) break;
            new_sibling_index = i;
        }
        tgl.transform.SetSiblingIndex(new_sibling_index);
    }
    public void Delete_Class() {
        Transform c_cnt = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0);
        Toggle tgl = c_cnt.GetComponentsInChildren<Toggle>().Where((x) => x.isOn && x.transform.parent == c_cnt).FirstOrDefault();

        int id = tgl.GetComponent<Button_Data_Holder>().int1;
        classes_info cl = Compiler.classes.Where((x)=>x.id == id).FirstOrDefault();
        string class_name = cl.GetClassName();

        Dialog_Panel.OnButtonPressed_U.AddListener((int b)=> {
            Dialog_Panel.OnButtonPressed_U.RemoveAllListeners();
            if (b != 1) return;

            if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) { 
                string dir = "";
                if      (save_dir_location_toggles[0].isOn) dir = Engine.user_script_dir + "/Classes";
                else if (save_dir_location_toggles[1].isOn) dir = appDataDir + "/Classes";
                if (!Directory.Exists(dir)) return;

                string f = dir + "/" + cl.saved_fileName + ".txt";
                if (!File.Exists(f)) return;
                File.Delete(f);
            } else {
                if (!PlayerPrefs.HasKey("Saved_Class_Names")) return;
                string saved_names = PlayerPrefs.GetString("Saved_Class_Names");
                saved_names = saved_names.Replace(cl.saved_fileName + ";", "");
                PlayerPrefs.SetString("Saved_Class_Names", saved_names);
                if (PlayerPrefs.HasKey("Saved_Class_" + cl.saved_fileName)) { PlayerPrefs.DeleteKey("Saved_Class_" + cl.saved_fileName); }
            }

            int sib_ind = tgl.transform.GetSiblingIndex();
            if (c_cnt.childCount <= (sib_ind + 1)) sib_ind--; else sib_ind++;
            c_cnt.transform.GetChild(sib_ind).GetComponent<Toggle>().isOn = true;
            Destroy( tgl.gameObject );
        });
        Dialog_Panel.ShowDialog("Delete class \"" + class_name + "\"?", "Yes", "No");

    }

    public void List_Func() {
        //Update currently edited class from script_text_field and get functions in it
        var cur_class = Compiler.classes.Where( (x)=>x.active ).FirstOrDefault();
        cur_class.content = Get_Script_Text();
        string[] cur_class_func_signatures = cur_class == null ? new string[]{} : cur_class.GetFuncSignatures();
        string[] cur_class_func_names = cur_class_func_signatures.Count() <= 0 ? new string[]{} : cur_class_func_signatures.Select(x=> string.Join("", classes_info.GetFuncNameAndParamFromSignature(x))).ToArray();

        Transform f_template = GameObject.Find("Scroll View_Func").transform.GetChild(0).GetChild(0).GetChild(0);
        Button func_save_btn = f_template.parent.parent.parent.parent.Find("Button_F_S").GetComponent<Button>();
        Button func_delete_btn = f_template.parent.parent.parent.parent.Find("Button_F_D").GetComponent<Button>();

        //Destroy old buttons
        for (int i = f_template.parent.childCount - 1; i >= 1; i--) { Destroy(f_template.parent.GetChild(i).gameObject); }

        //Create new func buttons
        f_template.GetComponent<Toggle>().enabled = true;
        f_template.transform.GetChild(1).gameObject.SetActive(true);
        f_template.transform.GetChild(3).gameObject.SetActive(true);
        f_template.transform.GetChild(2).GetComponent<Text>().text = "--- Saved Functions ---";
        //List saved functions
        List<string> saved_functions_names = new List<string>();
        foreach (var fnc in Compiler.functions.OrderBy(x => x.original_name)) {
            GameObject new_obj = Instantiate(f_template.gameObject, f_template.parent);
            new_obj.transform.GetChild(2).GetComponent<Text>().text = fnc.original_name;

            Button btn = new_obj.transform.GetChild(1).GetComponent<Button>();
            btn.interactable = !cur_class_func_names.Contains(fnc.original_name);
            if (btn.interactable) btn.onClick.AddListener(()=> Add_Func_To_Class( btn.transform.parent.GetComponent<Button_Data_Holder>() ));
            //Debug.Log("Cur func = '" + fnc.original_name + "', Cur class funcs: '" + string.Join("|||", cur_class_funcs) + "'");

            Button_Data_Holder bdh = new_obj.GetComponent<Button_Data_Holder>();
            bdh.int1 = fnc.id;

            new_obj.GetComponent<Toggle>().onValueChanged.AddListener((b)=> {
                if (!b) return;
                func_save_btn.interactable = false; func_delete_btn.interactable = true;
            });
        }
        GameObject Func_Label2 = Instantiate(f_template.gameObject, f_template.parent);
        Func_Label2.GetComponent<Toggle>().enabled = false;
        Func_Label2.transform.GetChild(1).gameObject.SetActive(false); //Checkbox
        Func_Label2.transform.GetChild(3).gameObject.SetActive(false); //Eye
        Func_Label2.transform.GetChild(2).GetComponent<Text>().text = "--- Functions from classes ---";
        //List unsaved functions (from all classes)
        foreach (var cl in Compiler.classes.OrderByDescending(x => x.id == 0).ThenBy((c)=>c.GetClassName()) ) {
            string[] fs = cl.GetFuncSignatures();
            if (fs == null || fs.Count() == 0) continue;
            foreach (string f in fs.OrderBy(x => x)) {
                if (cl.id == 0 && (f.StartsWith("void Start ") || f.StartsWith("void Start("))) continue;
                GameObject new_obj = Instantiate(f_template.gameObject, f_template.parent);
                
                new_obj.transform.GetChild(2).GetComponent<Text>().text = cl.GetClassName() + "." + string.Join("", classes_info.GetFuncNameAndParamFromSignature(f));
                //new_obj.transform.GetChild(1).gameObject.SetActive(false);

                Button btn = new_obj.transform.GetChild(1).GetComponent<Button>();
                btn.interactable = cl != cur_class; // && cur_class_funcs.Contains(f);
                btn.onClick.AddListener(()=> Add_Func_To_Class( btn.transform.parent.GetComponent<Button_Data_Holder>() ));
                //Debug.Log("Cur func = '" + f + "', Cur class funcs: '" + string.Join("|||", cur_class_funcs) + "'");

                //Transform text - remove class name
                string name = f;
                if (name.Contains(".")) name = name.Substring( name.LastIndexOf(".") + 1 );
                
                Button_Data_Holder bdh = new_obj.GetComponent<Button_Data_Holder>();
                bdh.int2 = cl.id; bdh.str1 = name;

                new_obj.GetComponent<Toggle>().onValueChanged.AddListener((b)=> {
                    if (!b) return;
                    func_save_btn.interactable = true; func_delete_btn.interactable = false;
                });
            }
        }
        f_template.GetComponent<Toggle>().enabled = false;
        f_template.transform.GetChild(1).gameObject.SetActive(false); //Checkbox
        f_template.transform.GetChild(3).gameObject.SetActive(false); //Eye
        func_save_btn.interactable = false; func_delete_btn.interactable = false;

    }
    public void Save_Func() {
        Transform c_cnt = GameObject.Find("Scroll View_Func").transform.GetChild(0).GetChild(0);
        Toggle tgl = c_cnt.GetComponentsInChildren<Toggle>().Where((x) => x.isOn && x.transform.parent == c_cnt).FirstOrDefault();
        if (tgl == null) return;

        var bdh = tgl.GetComponent<Button_Data_Holder>();
        if (bdh.int1 < 0) {
            //This is unsaved function
            if (bdh.int2 < 0) return;
            var cl = Compiler.classes.Where((x)=>x.id == bdh.int2).FirstOrDefault();
            if (cl == null) return;
            string func = cl.GetFuncBySignature(bdh.str1);
            if (func == "") return;

            string[] f_arr = classes_info.GetFuncNameAndParamFromSignature(bdh.str1);
            string funcNameAndParam = f_arr[0] + f_arr[1];

            classes_info func_info = new classes_info(func, funcNameAndParam);
            func_info.id = Compiler.functions.Count;
            bdh.int1 = Compiler.functions.Count;
            Compiler.functions.Add(func_info);

            Save_Func_All();

            //Transform text - remove return type
            tgl.transform.GetChild(2).GetComponent<Text>().text = funcNameAndParam;

            //Move function to 'saved' category in the list and sort alphabetically
            //Find '---UnSaved functions---' text siblling index
            Toggle t = c_cnt.GetComponentsInChildren<Toggle>(true).Where((x) => !x.enabled && x.transform.parent == c_cnt).LastOrDefault();
            int sibling_index = t.transform.GetSiblingIndex();
            for (int i = sibling_index - 1; i > 0; i--) {
                string txt = c_cnt.GetChild(i).GetChild(2).GetComponent<Text>().text;
                //Debug.Log("Compare '" + txt + "' & '" + f_arr[0] + "' res: " + string.Compare(txt, f_arr[0]) );
                if (string.Compare(txt, funcNameAndParam) <= 0) break;
                sibling_index = i;
            }
            tgl.transform.SetSiblingIndex(sibling_index);
            //tgl.transform.GetChild(1).gameObject.SetActive(true);

            //Recreate on click events
            Button func_save_btn = c_cnt.parent.parent.parent.Find("Button_F_S").GetComponent<Button>();
            Button func_delete_btn = c_cnt.parent.parent.parent.Find("Button_F_D").GetComponent<Button>();
            tgl.onValueChanged.RemoveAllListeners();
            tgl.onValueChanged.AddListener((b)=> {
                if (!b) return;
                func_save_btn.interactable = false; func_delete_btn.interactable = true;
            });
            tgl.onValueChanged.Invoke(true);
        } else {
            //This is saved function and can not be saved
        }
    }
    public void Save_Func_All() {
        string saving_str = "";
        foreach (var fi in Compiler.functions) {
            saving_str += fi.content + "\r\n******************************\r\n";
        }

        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) {
            string dir = "";
            if      (save_dir_location_toggles[0].isOn) dir = Engine.user_script_dir + "/Classes";
            else if (save_dir_location_toggles[1].isOn) dir = appDataDir + "/Classes";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(dir + "/!functions.txt", saving_str);
        } else {
            PlayerPrefs.SetString("Saved_Functions", saving_str);
        }
    }
    public void Delete_Func() {
        Transform c_cnt = GameObject.Find("Scroll View_Func").transform.GetChild(0).GetChild(0);
        Toggle tgl = c_cnt.GetComponentsInChildren<Toggle>().Where((x) => x.isOn && x.transform.parent == c_cnt).FirstOrDefault();
        if (tgl == null) return;

        var bdh = tgl.GetComponent<Button_Data_Holder>();
        if (bdh.int1 >= 0) {
            //This is saved function
            var func = Compiler.functions.Where(x=> x.id == bdh.int1).FirstOrDefault();
            if (func == null) return;
            Compiler.functions.Remove(func);
            Destroy(tgl.gameObject);
            Save_Func_All();
        } else {
            //This is unsaved function, that can not be deleted
        }

    }
    public void Add_Func_To_Class(Button_Data_Holder bdh) {
        if (bdh == null) return;
        //if (bdh.int1 < 0) return;

        string content = "";
        var func = Compiler.functions.Where(x=> x.id == bdh.int1).FirstOrDefault();
        if (func == null && bdh.int1 < 0) { 
            //This is unsaved function
            if (bdh.int2 < 0) return;
            var cl = Compiler.classes.Where((x)=>x.id == bdh.int2).FirstOrDefault();
            if (cl == null) return;
            content = cl.GetFuncBySignature(bdh.str1);
        } else {
            content = func.content;
        }
        if (content == "") return;

        string txt = ScriptEditor_My.text;
        if (classes_info.IsClass(txt)) {
            int ind = txt.LastIndexOf("}");
            if (ind > 0) {
                ScriptEditor_My.text = txt.Insert(ind, "\n" + content + "\n");
            }
        } else {
            ScriptEditor_My.text = txt + content;
        }
        
        var btn = bdh.transform.GetChild(1).GetComponent<Button>();
        btn.interactable = false;
    }
    public void Disable_Unavailable_Func() {
        Transform cnt = GameObject.Find("Scroll View_Func").transform.GetChild(0).GetChild(0);

        var cur_class = Compiler.classes.Where( (x)=>x.active ).FirstOrDefault();
        string[] fs = cur_class.GetFuncSignatures().Select(x => classes_info.GetFuncNameAndParamFromSignature(x)[0].Replace(" ", "") ).ToArray();
        //Debug.Log("Current class functions list: " + string.Join(", ", fs));

        if (fs == null || fs.Count() == 0) {
            //Enable all
            for (int i = 1; i < cnt.childCount; i++) { cnt.GetChild(i).GetChild(1).GetComponent<Button>().interactable = true; }
            return;
        }
        
        for (int i = 1; i < cnt.childCount; i++) {
            Button_Data_Holder bdh = cnt.GetChild(i).GetComponent<Button_Data_Holder>();
            string name = "";
            if (bdh.int1 < 0) {
                //This is unsaved function
                if (bdh.str1 != "") { name = classes_info.GetFuncNameAndParamFromSignature(bdh.str1)[0]; }
                //Debug.Log("Checking UNsaved func: '" + name + "'");
            } else {
                //This is a saved function
                var func = Compiler.functions.Where(x=> x.id == bdh.int1).FirstOrDefault();
                if (func != null) { name = classes_info.GetFuncNameAndParamFromSignature(func.original_name)[0]; }
                //Debug.Log("Checking saved func: '" + name + "'");
            }

            bool enable = false;
            //if (name != "") enable = fs.Where(x => x.StartsWith(name)).Count() == 0;
            if (name != "") enable = !fs.Contains(name);
            bdh.transform.GetChild(1).GetComponent<Button>().interactable = enable;
        }
    }

#endregion

#region Script Load/Save/Delete
    public void Fill_user_scripts_from_files() {
        //Remove all from script list
        for (int i = 0; i < user_script_list_content.transform.parent.childCount; i++) {
            GameObject g = user_script_list_content.transform.parent.GetChild(i).gameObject;
            if (g != user_script_list_content) Destroy(g);
        }

        //Get script list
        string[] files = new string[]{};
        //string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Script-o-Bot\\UserScripts";
        if (save_dir_location_toggles[0].isOn && Directory.Exists(Engine.user_script_dir))
            files = Directory.GetFiles(Engine.user_script_dir);
        else if (save_dir_location_toggles[1].isOn && Directory.Exists(appDataDir))
            files = Directory.GetFiles(appDataDir);
        else if (PlayerPrefs.HasKey("UserScriptNames"))
            files = PlayerPrefs.GetString("UserScriptNames").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);

        //Fill script list
        foreach (string f in files){
            string fNameNoExt = Path.GetFileNameWithoutExtension(f);
            GameObject f_new = Instantiate(user_script_list_content);
            f_new.transform.SetParent(user_script_list_content.transform.parent);
            f_new.transform.GetChild(0).GetComponent<Text>().text = fNameNoExt;
            f_new.transform.localScale = new Vector3(1f, 1f, 1f);
            f_new.SetActive(true);
            f_new.GetComponent<Button>().onClick.AddListener(()=>ScriptSave_NameSelect(f));
            

            //Adding EventTrigger prevent mouse scrolling of list
            // EventTrigger et = f_new.AddComponent<EventTrigger>();
            // EventTrigger.Entry ee = new EventTrigger.Entry();
            // EventTrigger.TriggerEvent ev = new EventTrigger.TriggerEvent();
            // ev.AddListener((BaseEventData d) => { if (((PointerEventData)d).clickCount == 2) { ScriptSave_NameSelect(f); ScriptLoad(); } });
            // ee.eventID = EventTriggerType.PointerClick;
            // ee.callback = ev;
            // et.triggers.Add( ee );
            Additional_Events ae = f_new.AddComponent<Additional_Events>();
            ae.OnDoubleClick += (GameObject g) => { ScriptSave_NameSelect(f); ScriptLoad(); };
        }
    }
    public void ScriptSave_NameSelect(string f) { //PointerEventData eventData) {
        //Debug.Log(eventData.clickCount.ToString());
        scriptSaveName.text = Path.GetFileNameWithoutExtension(f);
    }
    public void ScriptLoad() {
        string f = scriptSaveName.text.Trim();
        if (f == "") return;

        //Switch to main class
        GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;


        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) {
            //Load script from file
            if (!f.ToLower().EndsWith(".botScript")) f += ".botScript";

            string full_path = Path.GetFullPath(Engine.user_script_dir + "/" + f);
            if (save_dir_location_toggles[1].isOn) full_path = Path.GetFullPath(appDataDir + "\\" + f);

            if (!File.Exists(full_path))
            {
                Error_Panel.ShowError("File '" + full_path + "' does not exist."); return;
            }

            //Debug.Log("Load " + full_path);
            StreamReader r = File.OpenText(full_path);
            Set_Script_Text( r.ReadToEnd().Replace("\r", "") ); //Why the heck did I do this -- .Replace("\n\n","\n"); ???
            r.Close();
        } else {
            //Load script from registry
            string[] list_U = new string[]{};
            if (PlayerPrefs.HasKey("UserScriptNames")) {
                list_U = PlayerPrefs.GetString("UserScriptNames").ToUpper().Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
            }
            if (!list_U.Contains(f.ToUpper())) {
                Error_Panel.ShowError("The script '" + f + "' does not exist."); return;
            }

            if (PlayerPrefs.HasKey("UserScript_" + f.ToUpper())) {
                Set_Script_Text( PlayerPrefs.GetString("UserScript_" + f.ToUpper()) );
            } else {
                Set_Script_Text( "" );
            }
        }

        if (script_text != null) TMPro_HScrollbar.Scroll_H_ToZero();
        Hide();
    }
    public void ScriptSave(string filename = "") {
        string f = scriptSaveName.text.Trim();
        if (filename != "") f = filename;

        //Check if we are currently editing main class
        bool isEditingMainClass = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>().isOn;
        string script_str = isEditingMainClass ? Get_Script_Text() : Compiler.classes.Where((x)=>x.id == 0).First().content;
        script_str = Get_Enabled_Classes_Usings() + script_str;

        if (f.Trim() == "") {
            Error_Panel.ShowError("You need to enter file name to save."); return;
        }

        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn)
        {
            //Save script to file
            if (!f.ToLower().EndsWith(".botscript")) f += ".botScript";

            string full_path = Path.GetFullPath(Engine.user_script_dir + "/" + f);
            if (save_dir_location_toggles[1].isOn) full_path = Path.GetFullPath(appDataDir + "\\" + f);

            string dir = Path.GetDirectoryName(full_path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (filename == "" && File.Exists(full_path)) {
                full_path_persistent = full_path;
                Dialog_Panel.OnButtonPressed_U.AddListener((b)=> ScriptSave_Sub_EventHandler(b) );
                Dialog_Panel.ShowDialog("Overwrite this save file?", "Yes", "No");
            } else {
                File.WriteAllText(full_path, script_str);
                Hide();
            }
        } else {
            //Save script to registry
            List<string> list = new List<string>();
            string[] list_U = new string[]{};
            if (PlayerPrefs.HasKey("UserScriptNames")) {
                list = PlayerPrefs.GetString("UserScriptNames").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                list_U = PlayerPrefs.GetString("UserScriptNames").ToUpper().Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
            }

            if (list_U.Contains(f.ToUpper())) {
                full_path_persistent = "@PlayerPrefs@/" + f;
                Dialog_Panel.OnButtonPressed_U.AddListener((b)=> ScriptSave_Sub_EventHandler(b) );

                Dialog_Panel.ShowDialog("Overwrite this save file?", "Yes", "No");
            } else {
                list.Add(f);
                PlayerPrefs.SetString("UserScriptNames", string.Join(";", list));
                PlayerPrefs.SetString("UserScript_" + f.ToUpper(), script_str);
                Hide();
            }
        }
    }
    string full_path_persistent = "";
    public void ScriptSave_Sub_EventHandler(int b) {
        Dialog_Panel.OnButtonPressed_U.RemoveAllListeners();

        //Check if we are currently editing main class
        bool isEditingMainClass = GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>().isOn;
        string script_str = isEditingMainClass ? Get_Script_Text() : Compiler.classes.Where((x)=>x.id == 0).First().content;
        script_str = Get_Enabled_Classes_Usings() + script_str;

        if (b == 1) {
            if (full_path_persistent.ToUpper().StartsWith("@PlayerPrefs@/".ToUpper())) {
                //Save script to registry
                string reg_name = "UserScript_" + full_path_persistent.Substring(14).ToUpper();
                PlayerPrefs.SetString(reg_name, script_str);
            } else {
                //Save script to file
                File.WriteAllText(full_path_persistent, script_str);
            }
            Hide();
        }
    }
    public void ScriptDelete() {
        string f = scriptSaveName.text.Trim();
        if (f == "") return;

        if (save_dir_location_toggles[0].isOn || save_dir_location_toggles[1].isOn) {
            //Delete script file
            if (!f.ToLower().EndsWith(".botScript")) f += ".botScript";

            string full_path = Path.GetFullPath(Engine.user_script_dir + "/" + f);
            if (save_dir_location_toggles[1].isOn) full_path = Path.GetFullPath(appDataDir + "\\" + f);

            if (!File.Exists(full_path))
            {
                Error_Panel.ShowError("File '" + full_path + "' does not exist."); return;
            }

            full_path_persistent = full_path;
        } else {
            //Delete script from registry
            string[] list_U = new string[]{};
            if (PlayerPrefs.HasKey("UserScriptNames")) {
                list_U = PlayerPrefs.GetString("UserScriptNames").ToUpper().Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries);
            }
            if (!list_U.Contains(f.ToUpper())) {
                Error_Panel.ShowError("The script '" + f + "' does not exist."); return;
            }

            full_path_persistent = "@PlayerPrefs@/" + f;
        }

        Dialog_Panel.OnButtonPressed += ScriptDelete_Sub_EventHandler;
        Dialog_Panel.ShowDialog("Delete this script?", "Yes", "No");
    }
    public void ScriptDelete_Sub_EventHandler(int b) {
        Dialog_Panel.OnButtonPressed -= ScriptDelete_Sub_EventHandler;
        if (b == 1) {
            string entry_in_list = "";
            if (full_path_persistent.ToUpper().StartsWith("@PlayerPrefs@/".ToUpper())) {
                //Delete script from registry
                full_path_persistent = full_path_persistent.Substring(14).ToUpper().Trim();
                string reg_name = "UserScript_" + full_path_persistent;
                if (PlayerPrefs.HasKey(reg_name)) PlayerPrefs.DeleteKey(reg_name);
                if (PlayerPrefs.HasKey("UserScriptNames")) {
                    var list = PlayerPrefs.GetString("UserScriptNames").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    for (int i = list.Count() - 1; i >= 0; i--)
                        if (list[i].ToUpper().Trim() == full_path_persistent) list.RemoveAt(i);
                    PlayerPrefs.SetString("UserScriptNames", string.Join(";", list));
                }
                entry_in_list = full_path_persistent.ToUpper().Trim();
            } else {
                //Delete script file
                File.Delete(full_path_persistent);
                entry_in_list = Path.GetFileNameWithoutExtension(full_path_persistent).ToUpper().Trim().Replace(".BOTSCRIPT", "");
            }

            //Remove script from list
            for (int i = 0; i < user_script_list_content.transform.parent.childCount; i++) {
                GameObject g = user_script_list_content.transform.parent.GetChild(i).gameObject;
                string txt = g.transform.GetChild(0).GetComponent<Text>().text.Trim().ToUpper();
                if (g != user_script_list_content && txt == entry_in_list) Destroy(g);
            }
        }
    }

    string Get_Script_Text() { //bool dont_add_usings = false) {
        string txt = "";
        if (script_text != null ) txt = script_text.text;
        else txt = ScriptEditor_My.text;

        return txt;
    }
    string Get_Enabled_Classes_Usings() {
        string usings = "";
        for (int i = 1; i < Compiler.classes.Count; i++) {
            //if (Compiler.classes[i].enabled) usings += "//USING_LOC " + Compiler.classes[i].GetClassName() + "\n";
            if (Compiler.classes[i].enabled) usings += "//USING_LOC " + Compiler.classes[i].saved_fileName + "\n";
        }
        return usings;
    }
    void Set_Script_Text(string str, bool dont_parse_usings = false) {
        if (!dont_parse_usings) {
            var rows = str.Split(new char[]{'\n'});
            int rows_to_remove = 0;
            List<string> usings = new List<string>();
            for (int i = 0; i < rows.Length; i++ ) {
                if (!rows[i].StartsWith("//USING_LOC ")) break;
                
                rows_to_remove++;

                string class_name = rows[i].Substring( rows[i].IndexOf(" ") + 1 );
                usings.Add(class_name);
            }

            for (int c = 1; c < Compiler.classes.Count; c++) {
                //if (usings.Contains(Compiler.classes[c].GetClassName()))    Compiler.classes[c].enabled = true;
                if (usings.Contains(Compiler.classes[c].saved_fileName))    Compiler.classes[c].enabled = true;
                else                                                        Compiler.classes[c].enabled = false;
            }

            var rows_list = rows.ToList();
            rows_list.RemoveRange(0, rows_to_remove);
            str = string.Join("\n", rows_list);
        }

        if (script_text != null ) script_text.text = str;
        else ScriptEditor_My.text = str;
    }
    #endregion

    void LoadLevel(int index){
        Engine.engine_inst.LoadNextScene(index);
        Hide();
    }

    public void Hide_Tutorial_Dialog(bool force_show = false) {
        var rt = hideTutorialDialog_Button.transform.GetChild(0).GetComponent<RectTransform>();
        rt.DOKill(); rt.localScale = new Vector3(1f, 1f, 1f);
        rt.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.5f).SetEase(Ease.InBounce).SetLoops(2, LoopType.Yoyo).SetUpdate(true);

        if (Engine.DialogTutorial.deactivated_by_user)  { Engine.DialogTutorial.ActivateByUser(); Engine.TutorialDialogImportant.Activate_By_User(); }
        else if (!force_show)                           { Engine.DialogTutorial.DeActivateByUser(); Engine.TutorialDialogImportant.DeActivate_By_User(); }
    }
    public static void Show_Hide_Tutorial_Dialog_Button(bool show, float anim_duration) {
        float size_delta_y = Options.Bottom_Buttons_Panel.sizeDelta.y;
        if (show) {
            Options.Bottom_Buttons_Panel.DOKill();
            Options.hideTutorialDialog_Button.DOKill();
            Options.Bottom_Buttons_Panel.DOAnchorPosX(617.5f, anim_duration / 3f).SetUpdate(true);
            Options.Bottom_Buttons_Panel.DOSizeDelta(new Vector2(-1225f, size_delta_y), anim_duration / 3f).SetUpdate(true);
            Options.hideTutorialDialog_Button.DOAnchorPosY(20f, anim_duration).SetUpdate(true);
            
            //Show collapse button
            Bottom_Buttons_Panel_Expand_Button.GetComponent<Image>().DOKill();
            Bottom_Buttons_Panel_Expand_Button.GetComponent<Image>().DOFade(1f, anim_duration / 3f).SetUpdate(true).OnComplete(()=>
                Bottom_Buttons_Panel_Expand_Button.GetComponent<Button>().interactable = true
            );
            Bottom_Buttons_Panel_Expand_Button.GetComponent<RectTransform>().DOKill();
            Bottom_Buttons_Panel_Expand_Button.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetUpdate(true);
        } else {
            Options.Bottom_Buttons_Panel.DOKill();
            Options.hideTutorialDialog_Button.DOKill();
            Options.Bottom_Buttons_Panel.DOAnchorPosX(685f, anim_duration / 2f).SetUpdate(true);
            Options.Bottom_Buttons_Panel.DOSizeDelta(new Vector2(-1370f, size_delta_y), anim_duration / 2f).SetUpdate(true);
            Options.hideTutorialDialog_Button.DOAnchorPosY(-130f, anim_duration).SetUpdate(true);

            //Hide collapse button
            Bottom_Buttons_Panel_Expand_Button.GetComponent<Button>().interactable = false;
            Bottom_Buttons_Panel_Expand_Button.GetComponent<Image>().DOKill();
            Bottom_Buttons_Panel_Expand_Button.GetComponent<Image>().DOFade(0f, anim_duration / 2f).SetUpdate(true);
            Bottom_Buttons_Panel_Expand_Button.GetComponent<RectTransform>().DOKill();
            Bottom_Buttons_Panel_Expand_Button.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetUpdate(true);
        }
    }
    public void Collapse_Bottom_Buttons()
    {
        RectTransform rt = Bottom_Buttons_Panel_Expand_Button.GetComponent<RectTransform>();
        rt.DOKill();
        Options.Bottom_Buttons_Panel.DOKill();
        if (rt.localRotation.eulerAngles.z < 100f) {
            Options.Bottom_Buttons_Panel.DOAnchorPosX(1250f, 0.5f).SetUpdate(true);
            rt.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.25f).SetUpdate(true);
            //if (!Engine.DialogTutorial.deactivated_by_user) Engine.DialogTutorial.DeActivateByUser();
        } else {
            Options.Bottom_Buttons_Panel.DOAnchorPosX(617.5f, 0.5f).SetUpdate(true);
            rt.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.25f).SetUpdate(true);
        }
    }

    public void ExitGame() {
        Dialog_Panel.OnButtonPressed += ExitGameSub;
        Dialog_Panel.ShowDialog("Do you really want to exit?", "Yes", "No");
    }
    public void ExitGameSub(int b) {
        Dialog_Panel.OnButtonPressed -= ExitGameSub;
        if (b == 1) {
            Debug.Log("Exiting.");
            Application.Quit();
        }
    }

    public bool Is_Log_Panel_Shown { get { return cv_log.blocksRaycasts; } }
    public bool Is_ClassFunc_Panel_Shown { get { return cv_funClass.blocksRaycasts || DOTween.IsTweening(cv_funClass); } }
    public bool Is_Options_Panel_Shown { get { return cv_opt.blocksRaycasts; } }
    public void Panel_Log_Hide() {
        EventSystem.current.SetSelectedGameObject(null);

        cv_log.DOKill();
        cv_log.DOFade(0f, 0.7f).SetUpdate(true);
        cv_log.blocksRaycasts = false;
    }
    public void Panel_ClassFunc_Hide() {
        EventSystem.current.SetSelectedGameObject(null);

        cv_funClass.DOKill();
        cv_funClass.DOFade(0f, 0.7f).SetUpdate(true);
        cv_funClass.interactable = false;
        cv_funClass.blocksRaycasts = false;
        rt_funClass.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.7f).SetUpdate(true);
        rt_funClass.DOAnchorPos(new Vector2(490f, -460f), 0.7f).SetUpdate(true);
        Engine.desktop_manager.Restore_Last_Win_Geometry(1);
    }

    public void Debug_OpenAllLevels() {
        PlayerPrefs.SetInt("Game_Progress", 999);
        Options_Global_Static.current_progress = 999;
        Options_Global_Static.Hide_Levels_Above_Current_Progress();
    }
}

public class Additional_Events : MonoBehaviour, IPointerClickHandler {
    public delegate void DoubleClickDeleg(GameObject sender);
    public event DoubleClickDeleg OnDoubleClick;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) {
            if (OnDoubleClick != null) OnDoubleClick(gameObject);
        }
    }
}
