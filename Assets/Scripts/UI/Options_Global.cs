using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Options_Global : MonoBehaviour
{
    public Texture[] Level_Preview = new Texture[]{};

    // Start is called before the first frame update
    void Start()
    {
        Options_Global_Static.Volume_Global_sld = GameObject.Find("Audio_GLB").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.Volume_Music_sld = Options_Global_Static.Volume_Music_sld = GameObject.Find("Audio_MUS").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.Volume_FX_sld = Options_Global_Static.Volume_FX_sld = GameObject.Find("Audio_SFX").transform.GetChild(0).GetComponent<Slider>();
        Options_Global_Static.aud_mus_main = Camera.main.GetComponent<AudioSource>();
        Options_Global_Static.volume_hud_mask = GameObject.Find("Panel_Volume").transform.GetChild(0).GetComponent<RectTransform>();

        Transform Panel_ScriptEditor = GameObject.Find("Panel_ScriptEditor").transform;
        Options_Global_Static.ed_activate = Panel_ScriptEditor.Find("Dropdown_ed_act").GetComponent<Dropdown>();
        Options_Global_Static.ed_deactivate = Panel_ScriptEditor.Find("Dropdown_ed_deact").GetComponent<Dropdown>();
        Options_Global_Static.ed_camera_controls = Panel_ScriptEditor.Find("Dropdown_ed_cam_act").GetComponent<Dropdown>();
        Options_Global_Static.Editor_Opt_FontSize = Panel_ScriptEditor.Find("Font_Size").GetComponent<Slider>();
        Options_Global_Static.Editor_Opt_TabSpaces = Panel_ScriptEditor.Find("Tab_Spaces").GetComponent<Slider>();
        Options_Global_Static.Editor_Opt_CursorWidth = Panel_ScriptEditor.Find("Cursor_Width").GetComponent<Slider>();
        Options_Global_Static.Intellisense_toggle = Panel_ScriptEditor.Find("Intellisense_toggle").GetComponent<Toggle>();
        Options_Global_Static.Intellisense_max_sug = Panel_ScriptEditor.Find("Intellisense_max_sug").GetComponent<Slider>();
        Options_Global_Static.Editor_Autoclose = Panel_ScriptEditor.Find("Autoclose_toggle").GetComponent<Toggle>();

        Options_Global_Static.BOT_FastRebase = GameObject.Find("FastRebase_toggle").GetComponent<Toggle>();

        Options_Global_Static.button_hotkeys = GameObject.Find("Button_Hotkeys").GetComponent<Button>();
        Options_Global_Static.button_colors = GameObject.Find("Button_EditColors").GetComponent<Button>();
        Options_Global_Static.SubPanel_Hotkeys = GameObject.Find("Panel_Edit_Hotkeys").transform;
        Options_Global_Static.SubPanel_EditorColors = GameObject.Find("Panel_Editor_Colors").transform;
        Options_Global_Static.SubPanel_Blokator = GameObject.Find("Panel_Blokator").GetComponent<Image>();

        Options_Global_Static.load_game_list_content = GameObject.Find("SaveGame_Template");
        //Options_Global_Static.Level_Preview = Level_Preview;

        Scenario.Init();
        Options_Global_Static.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Options_Global_Static.Update();
    }
}

public static class Options_Global_Static
{
    #region "Declarations"

    //Config
    static float volume_change_speed = 0.01f;
    static float volume_last_updated = -100f;

    //Tabs to desactivate
    //public static Transform tab_continer = null;

    //Volume
    public static float Volume_Global = 1f;
    public static float Volume_Music = 1f;
    public static float Volume_FX = 1f;
    public static Slider Volume_Global_sld = null;
    public static Slider Volume_Music_sld = null;
    public static Slider Volume_FX_sld = null;
    public static Text Volume_Global_txt = null;
    public static Text Volume_Music_txt = null;
    public static Text Volume_FX_txt = null;
    public static RectTransform volume_hud_mask = null;
    public static AudioSource aud_mus_main = null;
    public static AudioSource aud_mus_levelComplete = null;
    public static AudioSource aud_fx = null;
    public static AudioSource aud_fx2 = null;
    public static AudioSource aud_fx_bot = null;
    public static AudioSource aud_fx_type = null;

    //Editor options
    public static Dropdown ed_activate = null;
    public static Dropdown ed_deactivate = null;
    public static Dropdown ed_camera_controls = null;
    public static Slider Editor_Opt_FontSize = null;
    public static Slider Editor_Opt_TabSpaces = null;
    public static Slider Editor_Opt_CursorWidth = null;
    public static Toggle Intellisense_toggle = null;
    public static Slider Intellisense_max_sug = null;
    public static Text_Editor ScriptEditor_My = null;
    public static Toggle Editor_Autoclose = null;

    //Options BOT
    public static Toggle BOT_FastRebase = null;

    //Sub_Panels
    public static Button button_colors = null;
    public static Button button_colors_ui = null;
    public static Button button_hotkeys = null;
    public static Image SubPanel_Blokator = null;
    public static Image SubPanel_BlokatorUI = null;
    public static Transform SubPanel_EditorColors = null;
    public static Transform SubPanel_Hotkeys = null;
    public static Transform SubPanel_UIColors = null;

    //Option SubPanels -> Hotkeys
    static float Hotkey_Timer = -1f;
    static Engine.Key Hotkey_to_set;
    static Transform Hotkey_waiting_panel = null;

    //Option SubPanels -> Colors
    static ColorPicker Clr_picker = null;
    static ColorPicker Clr_picker_UI = null;
    static Text_Editor Script_editor_clr = null;
    static Image Current_color_panel = null;
    static System.Reflection.FieldInfo Current_color_field = null;
    static string color_test_str = "using System;\npublic class test_colors {\n\n   public a;\n   private b;\n   void Start() {\n\n      //This is a comment text\n\n      var xxx = yyy;\n      string str = \"This is a quoted string\";\n\n      while (int i=0; i<5; i++) //Keyword\n      bool a = false    //Constant\n      typeof()          //Reflection\n   }\n}";

    //Option SubPanels -> Colors UI
    static Engine.higlight_colors_info current_ui_color_info;
    static System.Reflection.FieldInfo Current_color_field_UI = null;

    //Load Game Panel
    public static int current_progress = 0;
    //public static Texture[] Level_Preview = new Texture[]{};
    public static GameObject load_game_list_content;

    //Events
    public static ASTT.UnityInt1Event Load_Level = new ASTT.UnityInt1Event();

    static bool inited = false;
    static string color_ui_preview_text = @"using <n>Namespace</n>;
<c>Classes</c>.<m>Method()</m>;

<t>simple types:</t>
<t>int</t> x; <t>float</t> y; <t>bool</t> b;

<str>string literal:</str>
<t>string</t> s = <str>""This is a string literal""</str>;

<instr>instructions / commands / keywords:</instr>
<instr>while</instr> (true) { <instr>if</instr> (a) { ... } <instr>else</instr> { ... } }

<cmt>//This is a comment</cmt>";

    #endregion

    public static void Init()
    {
        if (inited) { Init_SetObjects(); return; }

        //Init volume
        #region "Init volume"
        if (PlayerPrefs.HasKey("Audio_Volume_Global"))
            Volume_Global = PlayerPrefs.GetFloat ("Audio_Volume_Global");
        else
            Volume_Global = 1;
        if (PlayerPrefs.HasKey("Audio_Volume_Music"))
            Volume_Music = PlayerPrefs.GetFloat ("Audio_Volume_Music");
        else
            Volume_Music = 1;
        if (PlayerPrefs.HasKey("Audio_Volume_FX"))
            Volume_FX = PlayerPrefs.GetFloat ("Audio_Volume_FX");
        else
            Volume_FX = 1;
        #endregion

        //Init default hotkeys
        #region "Init hotkeys"
		Engine.Hotkeys.Add(Engine.Key.Main_Menu,       	new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.L} });
		Engine.Hotkeys.Add(Engine.Key.Camera_Left,       	new KeyCode[][]{ new KeyCode[]{KeyCode.LeftArrow} });
		Engine.Hotkeys.Add(Engine.Key.Camera_Right,      	new KeyCode[][]{ new KeyCode[]{KeyCode.RightArrow} });
		Engine.Hotkeys.Add(Engine.Key.Camera_Reset,      	new KeyCode[][]{ new KeyCode[]{KeyCode.Keypad5} });
		Engine.Hotkeys.Add(Engine.Key.Maze_Solver,       	new KeyCode[][]{ new KeyCode[]{KeyCode.S} });
		Engine.Hotkeys.Add(Engine.Key.Maze_Regenerate,   	new KeyCode[][]{ new KeyCode[]{KeyCode.C} });
		Engine.Hotkeys.Add(Engine.Key.WindowMode_Toggle, 	new KeyCode[][]{ new KeyCode[]{KeyCode.BackQuote} });
		Engine.Hotkeys.Add(Engine.Key.WindowMode_SwitchWindow, 		new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.Tab} });
		Engine.Hotkeys.Add(Engine.Key.WindowMode_WinFullscreenToggle, new KeyCode[][]{ new KeyCode[]{KeyCode.F11} });
		Engine.Hotkeys.Add(Engine.Key.Volume_Up,      new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.Equals}, new KeyCode[]{KeyCode.LeftAlt, KeyCode.KeypadPlus} });
		Engine.Hotkeys.Add(Engine.Key.Volume_Down,    new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.Minus}, new KeyCode[]{KeyCode.LeftAlt, KeyCode.KeypadMinus} });
        Engine.Hotkeys.Add(Engine.Key.Set_Breakpoint, new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.B} });
		Engine.Hotkeys.Add(Engine.Key.Intellisense_Up,   		new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.UpArrow} });
		Engine.Hotkeys.Add(Engine.Key.Intellisense_Down,		new KeyCode[][]{ new KeyCode[]{KeyCode.LeftAlt, KeyCode.DownArrow} });
		Engine.Hotkeys.Add(Engine.Key.Intellisense_Submit,	new KeyCode[][]{ new KeyCode[]{KeyCode.Tab}, new KeyCode[]{KeyCode.LeftAlt, KeyCode.Return}, new KeyCode[]{KeyCode.LeftAlt, KeyCode.KeypadEnter} });
		Engine.Hotkeys.Add(Engine.Key.Intellisense_Enable,	new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.I} });
		Engine.Hotkeys.Add(Engine.Key.Script_Control_Pause_Immediate,	new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.P} });
        Engine.Hotkeys.Add(Engine.Key.Debug_Show_Console,               new KeyCode[][]{ new KeyCode[]{KeyCode.F2} });
		Engine.Hotkeys.Add(Engine.Key.Debug_Generate_Script,			new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.G} });
        Engine.Hotkeys.Add(Engine.Key.Script_Time_x1,                   new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.Keypad1 } });
		Engine.Hotkeys.Add(Engine.Key.Script_Time_x2,                   new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.Keypad2 } });
        Engine.Hotkeys.Add(Engine.Key.Script_Time_x3,                   new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.Keypad3 } });
        Engine.Hotkeys.Add(Engine.Key.Script_Time_x5,                   new KeyCode[][]{ new KeyCode[]{KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.Keypad5 } });

		Engine.Hotkeys_descriptions.Add (Engine.Key.Main_Menu, 		     "Main Menu");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Camera_Left, 		 "Camera - Left");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Camera_Right, 	     "Camera - Right");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Camera_Reset, 	     "Camera - Reset");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Maze_Solver, 		 "Maze - Solve");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Maze_Regenerate,     "Maze - Regenerate");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Volume_Up, 	 	     "Volume - Up");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Volume_Down, 	 	 "Volume - Down");
		Engine.Hotkeys_descriptions.Add (Engine.Key.WindowMode_Toggle, 				"Window Mode - Toggle");
		Engine.Hotkeys_descriptions.Add (Engine.Key.WindowMode_SwitchWindow, 		"Window Mode - Switch Windows");
		Engine.Hotkeys_descriptions.Add (Engine.Key.WindowMode_WinFullscreenToggle, "Window Mode - Fullscreen Toggle");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Set_Breakpoint,         "Set Breakpoint");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Intellisense_Up,	 	"Suggestion - Up");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Intellisense_Down,	    "Suggestion - Down");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Intellisense_Submit,	"Suggestion - Submit");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Intellisense_Enable,	"Suggestion - Enable Toggle");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Script_Control_Pause_Immediate,	"Script - Pause Execution");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Debug_Show_Console,	            "Debug - Show Console");
		Engine.Hotkeys_descriptions.Add (Engine.Key.Debug_Generate_Script,			"Debug - Generate Levels Script");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Script_Time_x1,			"Time - x1 (Reset)");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Script_Time_x2,			"Time - x2");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Script_Time_x3,			"Time - x3");
        Engine.Hotkeys_descriptions.Add (Engine.Key.Script_Time_x5,			"Time - x5");

		foreach (var kv in Engine.Hotkeys) Engine.Hotkeys_Defaults.Add(kv.Key, kv.Value);
        #endregion

        //Get Hotkeys from saved preferences
        #region "Get Hotkeys from saved preferences"
        if (PlayerPrefs.HasKey("KeyConfig")) {
            string data = PlayerPrefs.GetString("KeyConfig");
            var opt = System.StringSplitOptions.RemoveEmptyEntries;
            foreach (string action in data.Split(new char[]{';'}, opt)) {
                var arr = action.Split(new char[]{':'}, opt);
                Engine.Key k_action = (Engine.Key)System.Enum.Parse(typeof(Engine.Key), arr[0]);
                if (!Engine.Hotkeys.ContainsKey(k_action)) continue;
                List<KeyCode[]> k_action_keys = new List<KeyCode[]>();
                foreach (string key_combination in arr[1].Split(new char[]{'|'}, opt)) {
                    List<KeyCode> kk_list = new List<KeyCode>();
                    foreach (string hk in key_combination.Split(new char[]{'+'}, opt)) {
                        kk_list.Add( (KeyCode)System.Enum.Parse(typeof(KeyCode), hk) );
                    }
                    k_action_keys.Add( kk_list.ToArray() );
                }
                Engine.Hotkeys[k_action] = k_action_keys.ToArray();
            }
        }
        #endregion

        if (PlayerPrefs.HasKey("Game_Progress")) current_progress = PlayerPrefs.GetInt("Game_Progress");

        Init_SetObjects();
        inited = true;
    }

    public static void Init_SetObjects()
    {
        //Volume
        #region "Volume"
        Volume_Global_sld.onValueChanged.RemoveAllListeners();
        Volume_Music_sld.onValueChanged.RemoveAllListeners();
        Volume_FX_sld.onValueChanged.RemoveAllListeners();

        Volume_Global_txt = Volume_Global_sld.transform.parent.GetChild(1).GetComponent<Text>();
        Volume_Music_txt = Volume_Music_sld.transform.parent.GetChild(1).GetComponent<Text>();
        Volume_FX_txt = Volume_FX_sld.transform.parent.GetChild(1).GetComponent<Text>();

        Volume_Global_sld.onValueChanged.AddListener((float v)=> Volume_changed(0) );
        Volume_Music_sld.onValueChanged.AddListener((float v)=> Volume_changed(1) );
        Volume_FX_sld.onValueChanged.AddListener((float v)=> Volume_changed(2) );

        Volume_Global_sld.value = Volume_Global;
        Volume_Music_sld.value = Volume_Music;
        Volume_FX_sld.value = Volume_FX;
        #endregion

        //Init ScriptEditor Activation Settings
        #region "Init ScriptEditor Activation Settings"
        if (PlayerPrefs.HasKey("Editor_Opt_Activate")) ed_activate.value = PlayerPrefs.GetInt("Editor_Opt_Activate");
        if (PlayerPrefs.HasKey("Editor_Opt_DeActivate")) ed_deactivate.value = PlayerPrefs.GetInt("Editor_Opt_DeActivate");
        if (PlayerPrefs.HasKey("Editor_Opt_Cam_Activate")) ed_camera_controls.value = PlayerPrefs.GetInt("Editor_Opt_Cam_Activate");
        ed_activate.onValueChanged.AddListener          ( (int v)=> { 
            PlayerPrefs.SetInt("Editor_Opt_Activate", v); 
            if (ScriptEditor_My != null && !Engine.desktop_manager.is_active) { 
                switch (v) {
                    case 0 : ScriptEditor_My.Activate_Settings.Activate = Text_Editor.Activate_Scheme_Info.Activate_Enum.OnClick; break;
                    case 1 : ScriptEditor_My.Activate_Settings.Activate = Text_Editor.Activate_Scheme_Info.Activate_Enum.OnMouseOver; break;
                }
            }
        });
        ed_deactivate.onValueChanged.AddListener        ( (int v)=> { 
            PlayerPrefs.SetInt("Editor_Opt_DeActivate", v);
            if (ScriptEditor_My != null && !Engine.desktop_manager.is_active) { 
                switch (v) {
                    case 0 : ScriptEditor_My.Activate_Settings.DeActivate = Text_Editor.Activate_Scheme_Info.DeActivate_Enum.OnClickOutside; break;
                    case 1 : ScriptEditor_My.Activate_Settings.DeActivate = Text_Editor.Activate_Scheme_Info.DeActivate_Enum.OnMouseOut; break;
                    case 2 : ScriptEditor_My.Activate_Settings.DeActivate = Text_Editor.Activate_Scheme_Info.DeActivate_Enum.Never; break;
                }
            }
        });
        ed_camera_controls.onValueChanged.AddListener   ( (int v)=> { 
            PlayerPrefs.SetInt("Editor_Opt_Cam_Activate", v); 
            if (Engine.desktop_manager != null && !Engine.desktop_manager.is_active) { 
                switch (v) {
                    case 0 : Engine.camera_Controller.cam_control_activation = Camera_Controller.cam_control_activation_enum.OnMouseOutIfEditor; break;
                    case 1 : Engine.camera_Controller.cam_control_activation = Camera_Controller.cam_control_activation_enum.OnEditorDeActivation; break;
                    case 2 : Engine.camera_Controller.cam_control_activation = Camera_Controller.cam_control_activation_enum.OnBoth; break;
                    case 3 : Engine.camera_Controller.cam_control_activation = Camera_Controller.cam_control_activation_enum.Always; break;
                }
            }
        });
        ed_activate.onValueChanged.Invoke(ed_activate.value);
        ed_deactivate.onValueChanged.Invoke(ed_deactivate.value);
        ed_camera_controls.onValueChanged.Invoke(ed_camera_controls.value);
        #endregion

        #region "Options / Editor -> UI Controls EventHandlers"
        Editor_Opt_FontSize.onValueChanged.AddListener((i)=> {
            int v = Mathf.RoundToInt(Editor_Opt_FontSize.value);
            Transform tr = Editor_Opt_FontSize.transform;
            tr.GetChild(tr.childCount-1).GetComponent<Text>().text = v.ToString();
            if (ScriptEditor_My!= null) { ScriptEditor_My.font_size = v; ScriptEditor_My.Update_Text_Size(); }
            PlayerPrefs.SetInt("Editor_Opt_FontSize", v);
        });
        Editor_Opt_TabSpaces.onValueChanged.AddListener((i)=> {
            int v = Mathf.RoundToInt(Editor_Opt_TabSpaces.value);
            Transform tr = Editor_Opt_TabSpaces.transform;
            tr.GetChild(tr.childCount-1).GetComponent<Text>().text = v.ToString();
            if (ScriptEditor_My!= null) { ScriptEditor_My.tab_spaces = v; }
            PlayerPrefs.SetInt("Editor_Opt_TabSpaces", v);
        });
        Editor_Opt_CursorWidth.onValueChanged.AddListener((i)=> {
            int v = Mathf.RoundToInt(Editor_Opt_CursorWidth.value);
            Transform tr = Editor_Opt_CursorWidth.transform;
            tr.GetChild(tr.childCount-1).GetComponent<Text>().text = v.ToString();
            if (ScriptEditor_My!= null) { ScriptEditor_My.cursor_width = (float)v + 0.5f; ScriptEditor_My.UpdateCursor(); }
            PlayerPrefs.SetInt("Editor_Opt_CursorWidth", v);
        });
        Editor_Autoclose.onValueChanged.AddListener((b)=> {
            if (ScriptEditor_My!= null) { 
                if (b) { ScriptEditor_My.Auto_Close_Pairs = "[];();{};'';\"\""; } else { ScriptEditor_My.Auto_Close_Pairs = ""; }
                ScriptEditor_My.Update_AutoclosePairs();
            }
            PlayerPrefs.SetInt("Editor_Opt_AutoclosePairs", b ? 1 : 0);
        });
        if (PlayerPrefs.HasKey("Editor_Opt_FontSize")) Editor_Opt_FontSize.value = (float)PlayerPrefs.GetInt("Editor_Opt_FontSize");
        if (PlayerPrefs.HasKey("Editor_Opt_TabSpaces")) Editor_Opt_TabSpaces.value = (float)PlayerPrefs.GetInt("Editor_Opt_TabSpaces");
        if (PlayerPrefs.HasKey("Editor_Opt_CursorWidth")) Editor_Opt_CursorWidth.value = (float)PlayerPrefs.GetInt("Editor_Opt_CursorWidth");
        if (PlayerPrefs.HasKey("Editor_Opt_AutoclosePairs")) Editor_Autoclose.isOn = PlayerPrefs.GetInt("Editor_Opt_AutoclosePairs") > 0 ? true : false;

        Intellisense_toggle.onValueChanged.AddListener((b)=> {
            if (ScriptEditor_My!= null) { ScriptEditor_My.enable_intellisense = b; }
            PlayerPrefs.SetInt("Editor_Opt_Intellisense_Enabled", b ? 1 : 0);
        });
        Intellisense_max_sug.onValueChanged.AddListener((i)=> {
            int v = Mathf.RoundToInt(Intellisense_max_sug.value);
            Transform tr = Intellisense_max_sug.transform;
            tr.GetChild(tr.childCount-1).GetComponent<Text>().text = v.ToString();
            if (ScriptEditor_My!= null) { ScriptEditor_My.Intellisense_Settings.max_suggestion_count = v; }
            PlayerPrefs.SetInt("Editor_Opt_Intellisense_Max_Sug", v);
        });
        if (PlayerPrefs.HasKey("Editor_Opt_Intellisense_Enabled")) Intellisense_toggle.isOn = PlayerPrefs.GetInt("Editor_Opt_Intellisense_Enabled") > 0 ? true : false;
        if (PlayerPrefs.HasKey("Editor_Opt_Intellisense_Max_Sug")) Intellisense_max_sug.value = (float)PlayerPrefs.GetInt("Editor_Opt_Intellisense_Max_Sug");
        Intellisense_toggle.onValueChanged.Invoke(Intellisense_toggle.isOn);
        Intellisense_max_sug.onValueChanged.Invoke(Intellisense_max_sug.value);
        #endregion

        //BOT Options
        #region "BOT Options"
        BOT_FastRebase.onValueChanged.AddListener((b)=> PlayerPrefs.SetInt("BOT_Opt_FastRebase", b ? 1 : 0) );
        if (PlayerPrefs.HasKey("BOT_Opt_FastRebase")) BOT_FastRebase.isOn = PlayerPrefs.GetInt("BOT_Opt_FastRebase") > 0 ? true : false;
        #endregion

        //Sub Panels
        button_hotkeys.onClick.AddListener(()=> SubPanel_Hotkeys_ShowHide() );
        button_colors.onClick.AddListener(()=> SubPanel_EditorColors_ShowHide() );
        SubPanel_Blokator.color = Color.clear; SubPanel_Blokator.raycastTarget = false;
        if (button_colors_ui != null) button_colors_ui.onClick.AddListener(()=> SubPanel_UIColors_ShowHide() );
        if (SubPanel_BlokatorUI != null) { SubPanel_BlokatorUI.color = Color.clear; SubPanel_BlokatorUI.raycastTarget = false; }

        //Options / Sub -> Hotkeys
        #region "Options / Sub -> Hotkeys"
        Transform template = null;
        if (SubPanel_Hotkeys.childCount == 1) template = SubPanel_Hotkeys.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        else                                  template = SubPanel_Hotkeys.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        foreach (var kv in Engine.Hotkeys) {
            Transform HK = GameObject.Instantiate(template.gameObject, template.parent).transform;
            HK.GetChild(0).GetComponent<Text>().text = Engine.Hotkeys_descriptions.ContainsKey(kv.Key) ? Engine.Hotkeys_descriptions[kv.Key] : "UnKnown Key";
            HK.GetChild(1).GetChild(0).GetComponent<Text>().text = SubPanel_Hotkeys_ToString(kv.Key);
            HK.GetChild(2).GetComponent<Button>().onClick.AddListener(()=> SubPanel_Hotkeys_Set_Hotkey(HK.GetChild(1), kv.Key) ); //Set
            HK.GetChild(3).GetComponent<Button>().onClick.AddListener(()=> { //Erase
                Engine.Hotkeys[kv.Key] = new KeyCode[][]{ new KeyCode[] {KeyCode.None} };
                HK.GetChild(1).GetChild(0).GetComponent<Text>().text = SubPanel_Hotkeys_ToString(kv.Key);
            });
            HK.GetChild(4).GetComponent<Button>().onClick.AddListener(()=> { //Delete
                Engine.Hotkeys[kv.Key] = Engine.Hotkeys_Defaults[kv.Key];
                HK.GetChild(1).GetChild(0).GetComponent<Text>().text = SubPanel_Hotkeys_ToString(kv.Key);
            });
        }
        template.gameObject.SetActive(false);
        SubPanel_Hotkeys.gameObject.SetActive(false);
        SubPanel_Hotkeys.GetComponent<CanvasGroup>().alpha = 0f;
        #endregion

        //Options / Sub -> Script Editor color changer
        #region "Options / Sub -> Script Editor color changer"
        Dropdown preset_dropdown = SubPanel_EditorColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
        Button[] preset_buttons = SubPanel_EditorColors.Find("Panel_Presets").GetComponentsInChildren<Button>();
        Clr_picker = SubPanel_EditorColors.GetChild(2).GetComponent<ColorPicker>();
        Script_editor_clr = SubPanel_EditorColors.Find("Script_Editor_Clr").Find("Text_Editor_Template").GetComponent<Text_Editor>();
        Clr_picker.onValueChanged.AddListener((Color c) => {
            var cs = Script_editor_clr.Color_Scheme;
            Current_color_field.SetValue(cs, Clr_picker.CurrentColor);
            Current_color_panel.color = Clr_picker.CurrentColor;
            Script_editor_clr.Color_Scheme = cs;
            //Script_editor_clr.Color_Scheme_Apply();
            //if (Current_color_field.Name.ToLower().StartsWith("t_")) Script_editor_clr.text = color_test_str;
            bool update_highlight = Current_color_field.Name.ToLower().StartsWith("t_") ? true : false;
            Script_editor_clr.Color_Scheme_Apply(update_highlight);
            //if (preset_dropdown.value != 0) preset_dropdown.value = 0;
        });
        preset_buttons[0].onClick.AddListener(()=> { //Load color preset
            string preset_name = preset_dropdown.options[preset_dropdown.value].text;
            SubPanel_EditorColors_LoadColors(preset_name);
        });
        preset_buttons[1].onClick.AddListener(()=> { //Update color preset
            string scheme = SubPanel_EditorColors_SerializeCurrentColors();
            string preset_name = preset_dropdown.options[preset_dropdown.value].text;
            PlayerPrefs.SetString("ColorScheme_" + preset_name, scheme);

            string preset_names = PlayerPrefs.GetString("ColorScheme_!Names");
            if (!preset_names.Contains(preset_name + ";")) PlayerPrefs.SetString("ColorScheme_!Names", preset_names + preset_name + ";");
            Debug.Log("Color scheme for copy: " + scheme);
        });
        preset_buttons[2].onClick.AddListener(()=> { //Add New color preset
            Script_editor_clr.DeActivate();
            Dialog_Panel_Txt.OnButtonPressed_U.AddListener(SubPanel_EditorColors_PresetAdd);
            Dialog_Panel_Txt.ShowDialog("Please, enter new preset name: ", "OK", "New Preset", new string[]{"[:;|]@@@Characters : ; | are not alowed.", "^(Default \\(Built-In\\)|Alternative \\(Built-In\\)|Vyrviglaznozt' \\(Built-In\\)|\\(Current\\))+$@@@Using names of built-in presets is not allowed."});
        });
        preset_buttons[3].onClick.AddListener(()=> { //Delete color preset
            Script_editor_clr.DeActivate();
            string preset_name = preset_dropdown.options[preset_dropdown.value].text;
            string[] undeletable = new string[] {"(Current)", "Default (Built-In)", "Alternative (Built-In)", "Vyrviglaznozt' (Built-In)"};
            if ( undeletable.Select(x=>x.ToLower()).Contains(preset_name.ToLower()) ) {
                Dialog_Panel.ShowDialog("You can not delete built-in preset \"" + preset_name + "\".", "OK", "");
            } else {
                Dialog_Panel.OnButtonPressed += SubPanel_EditorColors_PresetDelete;
                Dialog_Panel.ShowDialog("Deleting preset \"" + preset_name + "\"?", "Yes", "No");
            }
        });
        preset_dropdown.onValueChanged.AddListener((int i)=> { 
            bool active = i <= 3 ? false : true;
            foreach (int b in new int[]{1, 3}) preset_buttons[b].interactable = active;
        });
        //Ok / Cancel Buttons
        SubPanel_EditorColors.Find("Button_Ok").GetComponentInChildren<Button>().onClick.AddListener(()=>{
            SubPanel_EditorColors_SaveAndHide();
        });
        SubPanel_EditorColors.Find("Button_Cancel").GetComponentInChildren<Button>().onClick.AddListener(()=>{
            SubPanel_EditorColors_ShowHide(true);
        });
        //Default color presets
        if (!PlayerPrefs.HasKey("ColorScheme_Default (Built-In)")) PlayerPrefs.SetString("ColorScheme_Default (Built-In)", "background:30|30|30|255;cursor:255|255|255|255;scrollbar_back:30|30|30|255;scrollbar_front:66|66|66|255;scrollbar_front_highlight:88|88|88|255;scrollbar_front_pressed:109|109|109|255;selection_color:58|61|65|255;highlight_default_color:200|32|32|180;t_std:156|220|254|255;t_sym:255|255|255|255;t_quote:255|165|0|255;t_comment:0|128|0|255;t_keyword:197|134|192|255;t_modif:86|156|214|255;t_types:78|201|176|255;t_const:220|220|170|255;t_other:209|105|105|255;t_refl:206|145|120|255;Inactive_Tint:255|255|255|255;");
        if (!PlayerPrefs.HasKey("ColorScheme_Alternative (Built-In)")) PlayerPrefs.SetString("ColorScheme_Alternative (Built-In)", "background:245|245|245|255;cursor:0|0|0|255;scrollbar_back:245|245|245|255;scrollbar_front:187|187|187|255;scrollbar_front_highlight:143|143|143|255;scrollbar_front_pressed:100|100|100|255;selection_color:223|226|231|255;highlight_default_color:200|32|32|180;t_std:46|46|46|255;t_sym:219|8|115|255;t_quote:139|90|0|255;t_comment:0|128|0|255;t_keyword:160|100|155|255;t_modif:70|124|168|255;t_types:67|154|136|255;t_const:220|220|170|255;t_other:209|105|105|255;t_refl:206|145|120|255;Inactive_Tint:255|255|255|255;");
        if (!PlayerPrefs.HasKey("ColorScheme_Vyrviglaznozt' (Built-In)")) PlayerPrefs.SetString("ColorScheme_Vyrviglaznozt' (Built-In)", "background:255|253|0|255;cursor:225|0|255|255;scrollbar_back:255|0|0|255;scrollbar_front:0|255|226|255;scrollbar_front_highlight:0|255|10|255;scrollbar_front_pressed:109|109|109|255;selection_color:0|255|255|255;highlight_default_color:200|32|32|180;t_std:156|220|254|255;t_sym:255|255|255|255;t_quote:255|165|0|255;t_comment:0|128|0|255;t_keyword:197|134|192|255;t_modif:86|156|214|255;t_types:78|201|176|255;t_const:220|220|170|255;t_other:209|105|105|255;t_refl:206|145|120|255;Inactive_Tint:255|255|255|255;");
        if (!PlayerPrefs.HasKey("ColorScheme_(Current)")) PlayerPrefs.SetString("ColorScheme_(Current)", PlayerPrefs.GetString("ColorScheme_Default (Built-In)"));
        foreach (string s in PlayerPrefs.GetString("ColorScheme_!Names").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries)) preset_dropdown.options.Add( new Dropdown.OptionData(s) );
        SubPanel_EditorColors_LoadColors("(Current)", true);
        SubPanel_EditorColors.gameObject.SetActive(false);
        SubPanel_EditorColors.GetComponent<CanvasGroup>().alpha = 0f;
        #endregion

        //Options / Sub -> UI color changer
        #region "Options / Sub -> UI color changer"
        if (SubPanel_UIColors != null) {
            Text color_ui_preview_txt = SubPanel_UIColors.GetChild(7).GetChild(0).GetChild(1).GetComponent<Text>();
            Dropdown presetUI_dropdown = SubPanel_UIColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
            Button[] presetUI_buttons = SubPanel_UIColors.Find("Panel_Presets").GetComponentsInChildren<Button>();
            Clr_picker_UI = SubPanel_UIColors.GetChild(2).GetComponent<ColorPicker>();
            Clr_picker_UI.onValueChanged.AddListener((Color c) => {
                Color clr = Clr_picker_UI.CurrentColor;
                Current_color_panel.color = clr;

                if (Current_color_field_UI != null) {
                    if (Current_color_field_UI.Name == "background") { clr.a = current_ui_color_info.background.a; }
                    //Current_color_field.SetValue(current_ui_color_info, Clr_picker_UI.CurrentColor);
                    var reference = __makeref(current_ui_color_info);
                    Current_color_field_UI.SetValueDirect(reference, clr);
                    color_ui_preview_txt.text = Scenario.Code_HighLight(color_ui_preview_text, current_ui_color_info);
                } else {
                    var cb = current_ui_color_info.background;
                    float a = clr.r > clr.g ? clr.r : clr.g; a = a > clr.b ? a : clr.b;
                    current_ui_color_info.background = new Color(cb.r, cb.g, cb.b, a);
                }

                var img = SubPanel_UIColors.GetChild(7).GetChild(0).GetComponent<Image>();
                img.color = current_ui_color_info.background;
            });
            presetUI_buttons[0].onClick.AddListener(()=> { //Load color preset
                string preset_name = presetUI_dropdown.options[presetUI_dropdown.value].text;
                SubPanel_UIColors_LoadColors(preset_name);
            });
            presetUI_buttons[1].onClick.AddListener(()=> { //Update color preset
                string scheme = SubPanel_UIColors_SerializeCurrentColors();
                string preset_name = presetUI_dropdown.options[presetUI_dropdown.value].text;
                PlayerPrefs.SetString("ColorSchemeUI_" + preset_name, scheme);

                string preset_names = PlayerPrefs.GetString("ColorSchemeUI_!Names");
                if (!preset_names.Contains(preset_name + ";")) PlayerPrefs.SetString("ColorSchemeUI_!Names", preset_names + preset_name + ";");
                Debug.Log("ColorUI scheme for copy: " + scheme);
            });
            presetUI_buttons[2].onClick.AddListener(()=> { //Add New color preset
                Dialog_Panel_Txt.OnButtonPressed_U.AddListener(SubPanel_UIColors_PresetAdd);
                Dialog_Panel_Txt.ShowDialog("Please, enter new preset name: ", "OK", "New Preset", new string[]{"[:;|]@@@Characters : ; | are not alowed.", "^(Default \\(Built-In\\)|Alternative \\(Built-In\\)|Vyrviglaznozt' \\(Built-In\\)|\\(Current\\))+$@@@Using names of built-in presets is not allowed."});
            });
            presetUI_buttons[3].onClick.AddListener(()=> { //Delete color preset
                string preset_name = presetUI_dropdown.options[presetUI_dropdown.value].text;
                string[] undeletable = new string[] {"(Current)", "Default (Built-In)", "Alternative (Built-In)", "Vyrviglaznozt' (Built-In)"};
                if ( undeletable.Select(x=>x.ToLower()).Contains(preset_name.ToLower()) ) {
                    Dialog_Panel.ShowDialog("You can not delete built-in preset \"" + preset_name + "\".", "OK", "");
                } else {
                    Dialog_Panel.OnButtonPressed += SubPanel_UIColors_PresetDelete;
                    Dialog_Panel.ShowDialog("Deleting preset \"" + preset_name + "\"?", "Yes", "No");
                }
            });
            presetUI_dropdown.onValueChanged.AddListener((int i)=> { 
                bool active = i <= 3 ? false : true;
                foreach (int b in new int[]{1, 3}) presetUI_buttons[b].interactable = active;
            });

            //Ok / Cancel Buttons
            SubPanel_UIColors.Find("Button_Ok").GetComponentInChildren<Button>().onClick.AddListener(()=>{
                SubPanel_UIColors_SaveAndHide();
            });
            SubPanel_UIColors.Find("Button_Cancel").GetComponentInChildren<Button>().onClick.AddListener(()=>{
                SubPanel_UIColors_ShowHide(true);
            });
            //Default color presets
            if (!PlayerPrefs.HasKey("ColorSchemeUI_Default (Built-In)")) PlayerPrefs.SetString("ColorSchemeUI_Default (Built-In)", "nmsp:41|202|134|255;cls:160|160|255|255;method:255|0|0|255;str:255|185|0|255;built_in_types:160|160|255|255;comments:115|186|133|255;instruction:160|160|255|255;background:0|0|0|160;");
            if (!PlayerPrefs.HasKey("ColorSchemeUI_Alternative (Built-In)")) PlayerPrefs.SetString("ColorSchemeUI_Alternative (Built-In)", "nmsp:41|202|134|255;cls:160|160|255|255;method:255|0|0|255;str:255|185|0|255;built_in_types:160|160|255|255;comments:115|186|133|255;instruction:160|160|255|255;background:0|0|0|160;");
            if (!PlayerPrefs.HasKey("ColorSchemeUI_Vyrviglaznozt' (Built-In)")) PlayerPrefs.SetString("ColorSchemeUI_Vyrviglaznozt' (Built-In)", "nmsp:41|202|134|255;cls:160|160|255|255;method:255|0|0|255;str:255|185|0|255;built_in_types:160|160|255|255;comments:115|186|133|255;instruction:160|160|255|255;background:0|0|0|160;");
            if (!PlayerPrefs.HasKey("ColorSchemeUI_(Current)")) PlayerPrefs.SetString("ColorSchemeUI_(Current)", PlayerPrefs.GetString("ColorSchemeUI_Vyrviglaznozt' (Built-In)"));
            foreach (string s in PlayerPrefs.GetString("ColorSchemeUI_!Names").Split(new char[]{';'}, System.StringSplitOptions.RemoveEmptyEntries)) presetUI_dropdown.options.Add( new Dropdown.OptionData(s) );
            SubPanel_UIColors_LoadColors("(Current)", true);
            SubPanel_UIColors.gameObject.SetActive(false);
            SubPanel_UIColors.GetComponent<CanvasGroup>().alpha = 0f;
        }
        #endregion

        #region "Load Level"
        //Old button highlight color = 26AEE7
        if (load_game_list_content != null) { 
            load_game_list_content.SetActive(false);

            //Fill load level
            for (int i = 0; i < load_game_list_content.transform.parent.childCount; i++) {
                GameObject g = load_game_list_content.transform.parent.GetChild(i).gameObject;
                if (g != load_game_list_content) GameObject.Destroy(g);
            }

            for (int i = 0; i < Engine.Level_names.Count; i++) {
                if (Engine.Level_names[i] == "Level Chapter Complete") continue;
                
                GameObject g_new = GameObject.Instantiate(load_game_list_content);
                g_new.transform.SetParent(load_game_list_content.transform.parent);
                g_new.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                g_new.GetComponent<RectTransform>().localRotation = Quaternion.identity;

                string preview_name = "Preview_" + Engine.Level_obj_names[i].Substring( Engine.Level_obj_names[i].IndexOf(" ") + 1 );
                var texture = Resources.Load<Texture2D>("Level_Preview/" + preview_name);
                if (texture == null) texture = Resources.Load<Texture2D>("Level_Preview/no-preview-big1");
                g_new.transform.GetChild(2).GetComponent<RawImage>().texture = texture;
                //if (Level_Preview.Length > i) g_new.transform.GetChild(2).GetComponent<RawImage>().texture = Level_Preview[i];
                
                g_new.transform.GetChild(3).GetComponent<Text>().text = Engine.Level_names[i];
                g_new.transform.localScale = new Vector3(1f, 1f, 1f);
                g_new.SetActive(true);
                int ind = i; //using iteration variable in lambda make it always call when i=3
                g_new.GetComponent<Event_DblClick>().on_dbl_click.AddListener(()=>{
                    Engine.current_scene = ind + 1; if (Load_Level != null) { Load_Level.Invoke( ind ); }
                });
            }
            Hide_Levels_Above_Current_Progress();

            //Load button
            var load_buttton = load_game_list_content.transform.parent.parent.parent.parent.Find("Button_Load");
            load_buttton.GetComponent<Button>().onClick.AddListener(()=>{
                var active = load_game_list_content.transform.parent.GetComponentsInChildren<Toggle>().Where(tgl => tgl.isOn).LastOrDefault();
                if (active != null) { active.GetComponent<Event_DblClick>().on_dbl_click.Invoke(); }
            });
        }
        #endregion
    }

    public static void Update()
    {
        //Check Volume_hotkey - show volume bar
        #region "Check Volume_hotkey - show volume bar"
        //gauge height range: 2 - 396
        if (Engine.check_key(Engine.Key.Volume_Up, false)) {
            Volume_Global += volume_change_speed;
            if (Volume_Global > 1f) Volume_Global = 1f;
            volume_last_updated = Time.unscaledTime;
            Volume_Global_sld.value = Volume_Global;
        }
        if (Engine.check_key(Engine.Key.Volume_Down, false)) {
            Volume_Global -= volume_change_speed;
            if (Volume_Global < 0f) Volume_Global = 0f;
            volume_last_updated = Time.unscaledTime;
            Volume_Global_sld.value = Volume_Global;
        }

        //if (volume_hud_mask == null) return;
        if (Time.unscaledTime < volume_last_updated + 3) {
            volume_hud_mask.gameObject.SetActive(true);
            float h = Mathf.Lerp(2f, 396f, Volume_Global);
            volume_hud_mask.sizeDelta = new Vector2(volume_hud_mask.sizeDelta.x, h);
            //AudioListener.volume = Options.Volume_Global;
        } else {
            volume_hud_mask.gameObject.SetActive(false);
        }
        #endregion

        //Wait for hotkey for hotkey configuration
        #region "Wait for hotkey for hotkey_configuration"
        if (Hotkey_Timer > 0f) {
            Hotkey_Timer -= Time.unscaledDeltaTime;
            Text txt = Hotkey_waiting_panel.GetChild(0).GetComponent<Text>();
            if (Hotkey_Timer > 0f) {
                //Waiting
                txt.text = Mathf.Ceil(Hotkey_Timer).ToString();

                bool mod_alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                bool mod_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                bool mod_ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
                    if(Input.GetKey(vKey)){
                        bool is_mod_key = false;
                        is_mod_key = is_mod_key || vKey == KeyCode.LeftAlt || vKey == KeyCode.RightAlt;
                        is_mod_key = is_mod_key || vKey == KeyCode.LeftShift || vKey == KeyCode.RightShift;
                        is_mod_key = is_mod_key || vKey == KeyCode.LeftControl || vKey == KeyCode.RightControl;
                        if (!is_mod_key) {
                            Hotkey_Timer = -1f;

                            List<KeyCode> h = new List<KeyCode>();
                            if (mod_ctrl) h.Add(KeyCode.LeftControl);
                            if (mod_alt) h.Add(KeyCode.LeftAlt);
                            if (mod_shift) h.Add(KeyCode.LeftShift);
                            h.Add(vKey);

                            if (Engine.Hotkeys[Hotkey_to_set].Count() == 1 && Engine.Hotkeys[Hotkey_to_set][0][0] == KeyCode.None)
                                Engine.Hotkeys[Hotkey_to_set] = new KeyCode[][] { new KeyCode[]{} };

                            //Check if we already have this hotkey
                            bool already_exist = false;
                            foreach (KeyCode[] old_hotkey in Engine.Hotkeys[Hotkey_to_set]) {
                                already_exist = (old_hotkey.Count() == h.Count()) && (old_hotkey.Intersect(h).Count() == h.Count());
                                if (already_exist) break;
                            }

                            if (!already_exist) {
                                List<KeyCode[]> l = Engine.Hotkeys[Hotkey_to_set].ToList();
                                l.Add(h.ToArray());
                                Engine.Hotkeys[Hotkey_to_set] = l.ToArray();
                            }

                            txt.text = SubPanel_Hotkeys_ToString(Hotkey_to_set);

                            //Restore buttons
                            var cnt = Hotkey_waiting_panel.parent.parent;
                            for (int i = 0; i < cnt.childCount; i++) cnt.GetChild(i).GetComponent<Button>().interactable = true;
                        }
                    }
                }
            } else {
                //Restore previous hotkey
                txt.text = SubPanel_Hotkeys_ToString(Hotkey_to_set);
                //Restore buttons
                var cnt = Hotkey_waiting_panel.parent.parent;
                for (int i = 0; i < cnt.childCount; i++) cnt.GetChild(i).GetComponent<Button>().interactable = true;
            }
            return;
        }
        #endregion
    }

    public static void Volume_changed(int ind = -1) {
        if (ind <= 0 ) {
            Volume_Global = Volume_Global_sld.value;
            Volume_Global_txt.text = Mathf.Round(Volume_Global * 100).ToString();
            PlayerPrefs.SetFloat("Audio_Volume_Global", Volume_Global);
            AudioListener.volume = Volume_Global;
            //Debug.Log("Set volume global = " + Volume_Global);
        }

        if (ind < 0 || ind == 1 ) {
            Volume_Music = Volume_Music_sld.value;
            Volume_Music_txt.text = Mathf.Round(Volume_Music * 100).ToString();
            PlayerPrefs.SetFloat("Audio_Volume_Music", Volume_Music);
            if (aud_mus_main != null) aud_mus_main.volume = Volume_Music;
            if (aud_mus_levelComplete != null) aud_mus_levelComplete.volume = Volume_Music;
            //Debug.Log("Set volume music = " + Volume_Music);
        }

        if (ind < 0 || ind == 2 ) {
            Volume_FX = Volume_FX_sld.value;
            Volume_FX_txt.text = Mathf.Round(Volume_FX * 100).ToString();
            PlayerPrefs.SetFloat("Audio_Volume_FX", Volume_FX);
            if (aud_fx != null) aud_fx.volume = Volume_FX;
            if (aud_fx2 != null) aud_fx2.volume = Volume_FX;
            if (aud_fx_bot != null) aud_fx_bot.volume = Volume_FX;
            if (aud_fx_type != null) aud_fx_type.volume = Volume_FX;
            //Debug.Log("Set volume sound = " + Volume_FX);
        }
    }

    public static void Hide_Levels_Above_Current_Progress() {
        Transform cnt = load_game_list_content.transform.parent;
        for (int i = 1; i < cnt.childCount; i++) {
            if (i-1 <= current_progress) cnt.GetChild(i).gameObject.SetActive(true); 
            else cnt.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void SubPanel_Hotkeys_ShowHide(bool hide = false) {
        SubPanel_Blokator.DOKill();
        if (SubPanel_EditorColors!= null) SubPanel_EditorColors.GetComponent<CanvasGroup>().DOKill();

        if (!hide) {
            //Show

            //Deactivate tabs
            //TODO: Remove additional check when new interface is done
            // if (tab_continer != null && tab_continer.GetChild(0).GetComponent<Toggle>() != null) {
            //     for (int i = 0; i < tab_continer.childCount; i++){
            //         tab_continer.GetChild(i).GetComponent<Toggle>().interactable = false;
            //     }
            // }

            SubPanel_Hotkeys.gameObject.SetActive(true);
            SubPanel_Blokator.raycastTarget = true;
            SubPanel_Blokator.DOFade(0.7f, 0.3f).SetUpdate(true);
            SubPanel_Hotkeys.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetUpdate(true);
            SubPanel_Hotkeys.GetComponent<CanvasGroup>().blocksRaycasts = true;
        } else {
            //Hide

            //Reactivate tabs
            //TODO: Remove additional check when new interface is done
            // if (tab_continer != null && tab_continer.GetChild(0).GetComponent<Toggle>() != null) {
            //     for (int i = 0; i < tab_continer.childCount; i++){
            //         tab_continer.GetChild(i).GetComponent<Toggle>().interactable = true;
            //     }
            // }

            SubPanel_Blokator.raycastTarget = false;
            SubPanel_Blokator.DOFade(0f, 0.3f).SetUpdate(true);
            SubPanel_Hotkeys.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetUpdate(true).OnComplete(()=> SubPanel_Hotkeys.gameObject.SetActive(false) );
            SubPanel_Hotkeys.GetComponent<CanvasGroup>().blocksRaycasts = false;

            //Save hotkeys
            string save_data = "";
            foreach (var kv in Engine.Hotkeys) {
                save_data += kv.Key.ToString() + ":";
                foreach (KeyCode[] k_arr in kv.Value) {
                    foreach (KeyCode k in k_arr) save_data += k.ToString() + "+";
                    if (save_data.EndsWith("+")) save_data = save_data.Substring(0, save_data.Length-1);
                    save_data += "|";
                }
                if (save_data.EndsWith("|")) save_data = save_data.Substring(0, save_data.Length-1);
                save_data += ";";
            }
            PlayerPrefs.SetString("KeyConfig", save_data);
        }
    }
    public static void SubPanel_Hotkeys_Set_Hotkey(Transform panel, Engine.Key k)
    {
        Hotkey_Timer = 3f;
        Hotkey_to_set = k;
        Hotkey_waiting_panel = panel;

        var cnt = panel.parent.parent;
        for (int i = 0; i < cnt.childCount; i++) {
            cnt.GetChild(i).GetComponent<Button>().interactable = false;
        }
        panel.parent.GetComponent<Button>().interactable = true;
        panel.parent.GetComponent<Button>().Select();
    }
    public static string SubPanel_Hotkeys_ToString(Engine.Key k)
    {
        string str = "";
        foreach (KeyCode[] kc_arr in Engine.Hotkeys[k]) {
            if (str != "") str += ", ";
            foreach (KeyCode kc in kc_arr) {
                if (str != "" && !str.EndsWith(", ")) str += " + ";

                if      (kc == KeyCode.LeftControl || kc == KeyCode.RightControl) str += "Ctrl";
                else if (kc == KeyCode.LeftAlt     || kc == KeyCode.RightAlt)     str += "Alt";
                else if (kc == KeyCode.LeftShift   || kc == KeyCode.RightShift)   str += "Shift";
                else str += kc.ToString();
            }
        }
        return str;
    }
    public static bool SubPanelClose() {
        if (SubPanel_EditorColors.GetComponent<CanvasGroup>().blocksRaycasts) { SubPanel_EditorColors_ShowHide(true); return true; }
        if (SubPanel_Hotkeys.GetComponent<CanvasGroup>().blocksRaycasts) { SubPanel_Hotkeys_ShowHide(true); return true; }
        if (SubPanel_UIColors != null && SubPanel_UIColors.GetComponent<CanvasGroup>().blocksRaycasts) { SubPanel_UIColors_ShowHide(true); return true; }
        return false;
    }

    public static void SubPanel_EditorColors_ShowHide(bool hide = false)
    {
        SubPanel_Blokator.DOKill();
        SubPanel_EditorColors.GetComponent<CanvasGroup>().DOKill();

        if (!hide) {
            //Show
            // //TODO: Remove additional check when new interface is done
            // if (tab_continer != null && tab_continer.GetChild(0).GetComponent<Toggle>() != null) {
            //     for (int i = 0; i < tab_continer.childCount; i++){
            //         tab_continer.GetChild(i).GetComponent<Toggle>().interactable = false;
            //     }
            // }

            SubPanel_Blokator.raycastTarget = true;
            SubPanel_Blokator.DOFade(0.7f, 0.3f);
            SubPanel_EditorColors.gameObject.SetActive(true);
            SubPanel_EditorColors.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
            SubPanel_EditorColors.GetComponent<CanvasGroup>().blocksRaycasts = true;

            var color_fields = typeof(Text_Editor.Color_Scheme_Info).GetFields();
            if (ScriptEditor_My != null) {
                foreach (var field in color_fields) {
                    field.SetValue(Script_editor_clr.Color_Scheme, (Color)field.GetValue(ScriptEditor_My.Color_Scheme));
                }
                Script_editor_clr.Color_Scheme_Apply();
            }
            Script_editor_clr.text = color_test_str;

            var drp = SubPanel_EditorColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
            drp.value = 0; drp.onValueChanged.Invoke(0);

            SubPanel_EditorColors_ReCreateButtons();
        } else {
            //Hide
            //TODO: Remove additional check when new interface is done
            // if (tab_continer != null && tab_continer.GetChild(0).GetComponent<Toggle>() != null) {
            //     for (int i = 0; i < tab_continer.childCount; i++){
            //         tab_continer.GetChild(i).GetComponent<Toggle>().interactable = true;
            //     }
            // }

            SubPanel_Blokator.raycastTarget = false;
            SubPanel_Blokator.DOFade(0f, 0.3f).SetUpdate(true);
            SubPanel_EditorColors.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetUpdate(true).OnComplete(()=> SubPanel_EditorColors.gameObject.SetActive(false) );
            SubPanel_EditorColors.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
    public static void SubPanel_EditorColors_ReCreateButtons() {
        //Reset
        Transform cnt = SubPanel_EditorColors.GetChild(1).GetChild(0).GetChild(0);
        for (int i = cnt.childCount-1; i >= 1; i--) GameObject.DestroyImmediate(cnt.GetChild(i).gameObject);

        GameObject template = cnt.GetChild(0).gameObject;
        template.SetActive(true);

        var color_fields = typeof(Text_Editor.Color_Scheme_Info).GetFields();
        foreach (var field in color_fields) {
            Color clr = (Color)field.GetValue(Script_editor_clr.Color_Scheme);
            GameObject inst = GameObject.Instantiate(template, cnt);
            inst.transform.GetChild(0).GetComponent<Text>().text = field.Name.Replace("_", " ");

            Image color_panel = inst.transform.GetChild(1).GetComponent<Image>(); color_panel.color = clr;

            inst.GetComponent<Button>().onClick.AddListener( ()=> { Current_color_field = field; Current_color_panel = color_panel; Clr_picker.CurrentColor = color_panel.color; });
        }

        cnt.GetChild(1).GetComponent<Button>().Select();
        cnt.GetChild(1).GetComponent<Button>().onClick.Invoke();
        template.SetActive(false);
    }
    public static void SubPanel_EditorColors_SaveAndHide()
    {
        SubPanel_EditorColors_ShowHide(true);

        //Apply new color scheme
        var color_fields = typeof(Text_Editor.Color_Scheme_Info).GetFields();
        if (ScriptEditor_My != null) {
            foreach (var field in color_fields) {
                field.SetValue(ScriptEditor_My.Color_Scheme, (Color)field.GetValue(Script_editor_clr.Color_Scheme));
            }
            ScriptEditor_My.Color_Scheme_Apply(true);
        }
        //ScriptEditor_My.text = ScriptEditor_My.text;
        PlayerPrefs.SetString("ColorScheme_(Current)", SubPanel_EditorColors_SerializeCurrentColors() );
    }
    public static void SubPanel_EditorColors_LoadColors(string scheme_name, bool load_to_main_editor = false)
    {
        //SubPanel_EditorColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>().value = 0;
        if (!PlayerPrefs.HasKey("ColorScheme_" + scheme_name)) return;

        string scheme = PlayerPrefs.GetString("ColorScheme_" + scheme_name);
        var color_fields = typeof(Text_Editor.Color_Scheme_Info).GetFields();
        Text_Editor te = (load_to_main_editor && ScriptEditor_My != null) ? ScriptEditor_My : Script_editor_clr;

        var opt = System.StringSplitOptions.RemoveEmptyEntries;
        foreach(string data_field in scheme.Split(new char[]{';'}, opt)) {
            string[] data = data_field.Split(new char[]{':'}, opt);
            var field = color_fields.Where(x=> x.Name.ToLower() == data[0].ToLower()).FirstOrDefault();
            if (field == null) continue;

            string[] data_clr = data[1].Split(new char[]{'|'}, opt);
            Color clr = new Color32( byte.Parse(data_clr[0]), byte.Parse(data_clr[1]), byte.Parse(data_clr[2]), byte.Parse(data_clr[3]) );

            var cs = te.Color_Scheme;
            field.SetValue(cs, clr);
            //Debug.Log("Set " + field.Name + " to " + clr.ToString());
        }

        te.Color_Scheme_Apply(true);
        //te.text = te.text.Substring(0, te.text.Length-1);
        SubPanel_EditorColors_ReCreateButtons();
    }
    public static void SubPanel_EditorColors_PresetAdd(string s) {
        s=s.Trim();
        Dialog_Panel_Txt.OnButtonPressed_U.RemoveAllListeners();
        
        if (s == "") return;

        string preset_names = PlayerPrefs.GetString("ColorScheme_!Names");
        if (preset_names.ToLower().Contains(s.ToLower() + ";")) {  
            Dialog_Panel.ShowDialog("This preset already exist.", "OK", ""); return;
        }

        Dropdown preset_dropdown = SubPanel_EditorColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
        preset_dropdown.options.Add(new Dropdown.OptionData(s));
        preset_dropdown.value = preset_dropdown.options.Count() - 1;

        string scheme = SubPanel_EditorColors_SerializeCurrentColors();
        PlayerPrefs.SetString("ColorScheme_" + s, scheme);
        PlayerPrefs.SetString("ColorScheme_!Names", preset_names + s + ";");
    }
    public static void SubPanel_EditorColors_PresetDelete(int b) {
        Dialog_Panel.OnButtonPressed -= SubPanel_EditorColors_PresetDelete;
        if (b == 2) return;

        Dropdown preset_dropdown = SubPanel_EditorColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
        string current_preset = preset_dropdown.options[preset_dropdown.value].text;
        PlayerPrefs.DeleteKey("ColorScheme_" + current_preset);

        string preset_names = PlayerPrefs.GetString("ColorScheme_!Names");
        var r = new System.Text.RegularExpressions.Regex(current_preset + ";", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        preset_names = r.Replace(preset_names, "");
        PlayerPrefs.SetString("ColorScheme_!Names", preset_names);

        preset_dropdown.options.RemoveAt(preset_dropdown.value);
        preset_dropdown.value = 0;
    }
    public static string SubPanel_EditorColors_SerializeCurrentColors()
    {
        string scheme = "";
        var color_fields = typeof(Text_Editor.Color_Scheme_Info).GetFields();
        foreach (var field in color_fields) {
            Color32 clr = (Color)field.GetValue(Script_editor_clr.Color_Scheme);
            scheme += field.Name + ":" + clr.r.ToString() + "|" + clr.g.ToString() + "|" + clr.b.ToString() + "|" + clr.a.ToString() + ";";
        }
        return scheme;
    }

    public static void SubPanel_UIColors_ShowHide(bool hide = false) {
        SubPanel_BlokatorUI.DOKill();
        SubPanel_UIColors.GetComponent<CanvasGroup>().DOKill();

        if (!hide) {
            //Show
            SubPanel_BlokatorUI.raycastTarget = true;
            SubPanel_BlokatorUI.DOFade(0.7f, 0.3f);
            SubPanel_UIColors.gameObject.SetActive(true);
            SubPanel_UIColors.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
            SubPanel_UIColors.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
            current_ui_color_info = Engine.higlight_colors;

            var drp = SubPanel_UIColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
            drp.value = 0; drp.onValueChanged.Invoke(0);

            SubPanel_UIColors_ReCreateButtons();
        } else {
            //Hide
            SubPanel_BlokatorUI.raycastTarget = false;
            SubPanel_BlokatorUI.DOFade(0f, 0.3f).SetUpdate(true);
            SubPanel_UIColors.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetUpdate(true).OnComplete(()=> SubPanel_EditorColors.gameObject.SetActive(false) );
            SubPanel_UIColors.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
    public static void SubPanel_UIColors_ReCreateButtons() {
        //Reset
        Transform cnt = SubPanel_UIColors.GetChild(1).GetChild(0).GetChild(0);
        for (int i = cnt.childCount-1; i >= 1; i--) GameObject.DestroyImmediate(cnt.GetChild(i).gameObject);

        GameObject template = cnt.GetChild(0).gameObject;
        template.SetActive(true);

        var color_fields = typeof(Engine.higlight_colors_info).GetFields();
        foreach (var field in color_fields) {
            Color clr = (Color)field.GetValue(current_ui_color_info);
            GameObject inst = GameObject.Instantiate(template, cnt);
            inst.transform.GetChild(0).GetComponent<Text>().text = field.Name.Replace("_", " ");

            Image color_panel = inst.transform.GetChild(1).GetComponent<Image>(); color_panel.color = clr;

            inst.GetComponent<Button>().onClick.AddListener( ()=> { Current_color_field_UI = field; Current_color_panel = color_panel; Clr_picker_UI.CurrentColor = color_panel.color; });
        }

        //Additional entry for transparency
        float a = current_ui_color_info.background.a;
        GameObject inst_tr = GameObject.Instantiate(template, cnt);
        inst_tr.transform.GetChild(0).GetComponent<Text>().text = "background opacity";
        Image color_panel_tr = inst_tr.transform.GetChild(1).GetComponent<Image>(); color_panel_tr.color = new Color(a, a, a, 1f);
        inst_tr.GetComponent<Button>().onClick.AddListener( ()=> { Current_color_field_UI = null; Current_color_panel = color_panel_tr; Clr_picker_UI.CurrentColor = color_panel_tr.color; });

        cnt.GetChild(1).GetComponent<Button>().Select();
        cnt.GetChild(1).GetComponent<Button>().onClick.Invoke();
        template.SetActive(false);
    }
    public static void SubPanel_UIColors_SaveAndHide() {
        SubPanel_UIColors_ShowHide(true);

        Engine.higlight_colors = current_ui_color_info;

        //Apply new colors to currently displayed text in dialog panels
        var dialog = GameObject.Find("Dialog_Text");
        var dialog_imp = GameObject.Find("Dialog_Important_Text");
        Image d_img = dialog.transform.parent.GetComponent<Image>();
        Image d_img_imp = dialog_imp.transform.parent.GetChild(0).GetComponent<Image>();
        Text d_text = dialog.GetComponent<Text>();
        Text d_text_imp = dialog_imp.GetComponent<Text>();
        d_text.text = Scenario.Code_HighLight( Engine.engine_inst.cur_text );
        d_text_imp.text = Scenario.Code_HighLight( Engine.engine_inst.cur_text_imp );
        d_img.color = Engine.higlight_colors.background;
        d_img_imp.color = Engine.higlight_colors.background;

        PlayerPrefs.SetString("ColorSchemeUI_(Current)", SubPanel_UIColors_SerializeCurrentColors() );
    }
    public static void SubPanel_UIColors_LoadColors(string scheme_name, bool load_to_main_UI = false){
        if (!PlayerPrefs.HasKey("ColorSchemeUI_" + scheme_name)) return;

        string scheme = PlayerPrefs.GetString("ColorSchemeUI_" + scheme_name);
        var color_fields = typeof(Engine.higlight_colors_info).GetFields();
        var reference = __makeref(current_ui_color_info);
        //var referenceM = __makeref(Engine.higlight_colors);

        var opt = System.StringSplitOptions.RemoveEmptyEntries;
        foreach(string data_field in scheme.Split(new char[]{';'}, opt)) {
            string[] data = data_field.Split(new char[]{':'}, opt);
            var field = color_fields.Where(x=> x.Name.ToLower() == data[0].ToLower()).FirstOrDefault();
            if (field == null) continue;

            string[] data_clr = data[1].Split(new char[]{'|'}, opt);
            Color clr = new Color32( byte.Parse(data_clr[0]), byte.Parse(data_clr[1]), byte.Parse(data_clr[2]), byte.Parse(data_clr[3]) );

            field.SetValueDirect(reference, clr);
            //if (load_to_main_UI) field.SetValueDirect(referenceM, clr);
        }
        if (load_to_main_UI) Engine.higlight_colors = current_ui_color_info;

        SubPanel_UIColors_ReCreateButtons();
    }
    public static void SubPanel_UIColors_PresetAdd(string s) {
        s=s.Trim();
        Dialog_Panel_Txt.OnButtonPressed_U.RemoveAllListeners();
        
        if (s == "") return;

        string preset_names = PlayerPrefs.GetString("ColorSchemeUI_!Names");
        if (preset_names.ToLower().Contains(s.ToLower() + ";")) {  
            Dialog_Panel.ShowDialog("This preset already exist.", "OK", ""); return;
        }

        Dropdown preset_dropdown = SubPanel_UIColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
        preset_dropdown.options.Add(new Dropdown.OptionData(s));
        preset_dropdown.value = preset_dropdown.options.Count() - 1;

        string scheme = SubPanel_UIColors_SerializeCurrentColors();
        PlayerPrefs.SetString("ColorSchemeUI_" + s, scheme);
        PlayerPrefs.SetString("ColorSchemeUI_!Names", preset_names + s + ";");
    }
    public static void SubPanel_UIColors_PresetDelete(int b) {
        Dialog_Panel.OnButtonPressed -= SubPanel_UIColors_PresetDelete;
        if (b == 2) return;

        Dropdown preset_dropdown = SubPanel_UIColors.Find("Panel_Presets").GetComponentInChildren<Dropdown>();
        string current_preset = preset_dropdown.options[preset_dropdown.value].text;
        PlayerPrefs.DeleteKey("ColorSchemeUI_" + current_preset);

        string preset_names = PlayerPrefs.GetString("ColorSchemeUI_!Names");
        var r = new System.Text.RegularExpressions.Regex(current_preset + ";", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        preset_names = r.Replace(preset_names, "");
        PlayerPrefs.SetString("ColorSchemeUI_!Names", preset_names);

        preset_dropdown.options.RemoveAt(preset_dropdown.value);
        preset_dropdown.value = 0;
    }
    public static string SubPanel_UIColors_SerializeCurrentColors()
    {
        string scheme = "";
        var color_fields = typeof(Engine.higlight_colors_info).GetFields();
        foreach (var field in color_fields) {
            Color32 clr = (Color)field.GetValue(current_ui_color_info);
            scheme += field.Name + ":" + clr.r.ToString() + "|" + clr.g.ToString() + "|" + clr.b.ToString() + "|" + clr.a.ToString() + ";";
        }
        return scheme;
    }
}