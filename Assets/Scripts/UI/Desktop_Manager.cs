using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Rect_Extension;

public class Desktop_Manager : MonoBehaviour
{
    public Camera BigEdit_Cam = null;
    public GameObject desktop_wallpaper = null;
    public RectTransform Window_Log = null;
    public RectTransform Window_Script = null;
    public RectTransform Window_RobotCam = null;
    public RectTransform Window_RobotCam_Title = null;
    //public Image Window_Script_Border_Std = null;
    public Canvas Window_Script_Canvas = null;
    public CanvasGroup Window_Script_Border_Std = null;
    public CanvasGroup Window_Script_Border_Win = null;
    Text_Editor txt_editor = null;
    Tutorial_Arrow TutorialArrow = null; //To disable activation while it is shown
	Tutorial_RedPanel TutorialRedPanel = null; //To disable activation while it is shown

    //public KeyCode activation_key = KeyCode.BackQuote; 

    public float anim_in_duration = 1.7f;
    public float anim_out_duration = 0.7f;
    public float anim_max_duration = 0.4f;

    public Texture2D cursor_std = null;
    public Texture2D cursor_resize_w = null;
    public Texture2D cursor_resize_h = null;
    public Texture2D cursor_resize_lt_rb = null;
    public Texture2D cursor_resize_lb_rt = null;
    
    public Quaternion Default_RobotCam_Geom = new Quaternion(-200, 75f, -500f, -250f);
    public Quaternion Default_Script_Geom = new Quaternion(-7f, -7f, 550f, 380f);
    public Quaternion Max_RobotCam_Geom = new Quaternion(0f, -7f, 0f, -14f);
    public Quaternion Max_Script_Geom = new Quaternion(8f, -2f, 29f, 3f);
    public Quaternion Full_RobotCam_Geom = new Quaternion(0f, 0f, 0f, 0f);
    public Quaternion Full_Script_Geom = new Quaternion(14f, 12f, 36f, 15f);
    //public Vector2 Min_Size_Robot = new Vector2(-500f, -240); //-1350, -700
    //public Vector2 Min_Size_Script = new Vector2(360f, 247);  //590, 390

    public Win_Param_Info[] Win_Params = new Win_Param_Info[]{};
    Dictionary<string, Win_Param_Info> Win_Params_Dict = new Dictionary<string, Win_Param_Info>();
    [System.Serializable]
    public struct Win_Param_Info {
        public string name;
        public RectTransform rt;
        public Vector2 Min_Size;
    }

    #if UNITY_EDITOR
    [ShowOnly] public string cur_wRobot_Pos = "";
    [ShowOnly] public string cur_wRobot_Size = "";
    [ShowOnly] public string cur_wScript_Pos = "";
    [ShowOnly] public string cur_wScript_Size = "";
    [ShowOnly] public string cur_wLog_Pos = "";
    [ShowOnly] public string cur_wLog_Size = "";
    #endif

    float min_script_width = 0f;

    Vector2 drag_editor = new Vector2(float.MinValue, float.MinValue);
    Vector2 drag_robot_cam = new Vector2(float.MinValue, float.MinValue);

    Vector2 drag_resize_point = new Vector2(float.MinValue, float.MinValue);
    Vector2 drag_resize_orig_pos = new Vector2(float.MinValue, float.MinValue);
    Vector2 drag_resize_orig_size = new Vector2(float.MinValue, float.MinValue);

    Quaternion win_robot_orig_geom = new Quaternion();
    Quaternion win_script_orig_geom = new Quaternion();
    Dictionary<string, Quaternion> win_desktop_geometries = new Dictionary<string, Quaternion>();

    public enum event_type { click, p_enter, p_exit, drag_begin, drag_end, drag };
    public enum border_sides { left, right, top, bottom, left_bottom, left_top, right_top, right_bottom, middle };

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.pixelDragThreshold = 0;

        txt_editor = Window_Script.GetComponentInChildren<Text_Editor>();
        min_script_width = Window_Script.sizeDelta.x;

        //Stretch script window for different aspects
        float size_y = Window_Script_Canvas.GetComponent<RectTransform>().sizeDelta.y + Window_Script.anchoredPosition.y - 174.3f;
        Window_Script.sizeDelta = new Vector2(Window_Script.sizeDelta.x, size_y);

        win_robot_orig_geom = new Quaternion(Window_RobotCam.anchoredPosition.x, Window_RobotCam.anchoredPosition.y, Window_RobotCam.sizeDelta.x, Window_RobotCam.sizeDelta.y );
        win_script_orig_geom = new Quaternion(Window_Script.anchoredPosition.x, Window_Script.anchoredPosition.y, Window_Script.sizeDelta.x, Window_Script.sizeDelta.y );

        // win_desktop_geometries.Add("0", new Quaternion(-200, 75f, -500f, -250f));
        // win_desktop_geometries.Add("1", new Quaternion(-7f, -7f, 550f, 380f));
        // win_desktop_geometries.Add("0-Full", new Quaternion(-200, 75f, -500f, -250f));
        // win_desktop_geometries.Add("1-Full", new Quaternion(-7f, -7f, 550f, 380f));
        win_desktop_geometries.Add("0", Default_RobotCam_Geom);       //Geometry for first time and restore after maximizr
        win_desktop_geometries.Add("1", Default_Script_Geom);      //Geometry for first time and restore after maximizr
        win_desktop_geometries.Add("0-Full", Default_RobotCam_Geom);  //Geometry to restore after fullscreen
        win_desktop_geometries.Add("1-Full", Default_Script_Geom); //Geometry to restore after fullscreen
        foreach (var wp in Win_Params) { Win_Params_Dict.Add(wp.name, wp); }

		TutorialArrow = GameObject.Find("Tutorial_Arrow").GetComponent<Tutorial_Arrow>();
		TutorialRedPanel = GameObject.Find("Tutorial_RedPanel").GetComponent<Tutorial_RedPanel>();

        if ( cursor_std != null ) Cursor.SetCursor(cursor_std, new Vector2(2, 3), CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera_Controller.level_complete_shown) return;

        //Activate win mode
        if (Engine.check_key(Engine.Key.WindowMode_Toggle)) { //Tilde
            if (!TutorialArrow.is_active && !TutorialRedPanel.is_active) {
                if (Camera.main.targetTexture == null)  Switch_Mode(1); //Activate desktop
                else                                    Switch_Mode(0); //DeActivate desktop
            }
        }

        if (is_active) {
            //If in win mode
            if (DOTween.IsTweening(Window_RobotCam) || DOTween.IsTweening(Window_Script_Border_Win)) return;

            if (Engine.check_key(Engine.Key.WindowMode_SwitchWindow)) {
                if (Window_RobotCam.GetSiblingIndex() == 2) Window_Script.SetSiblingIndex(2);
                else Window_RobotCam.SetSiblingIndex(2);
            }

            if (Engine.check_key(Engine.Key.WindowMode_WinFullscreenToggle)) {
                if (Window_RobotCam.GetSiblingIndex() == 2) Max_Restore("0-full");
                else Max_Restore("1-full");
            }
        }

        #if UNITY_EDITOR
            cur_wRobot_Pos = Window_RobotCam.anchoredPosition.ToString();
            cur_wRobot_Size = Window_RobotCam.sizeDelta.ToString();
            cur_wScript_Pos = Window_Script.anchoredPosition.ToString();
            cur_wScript_Size = Window_Script.sizeDelta.ToString();
            cur_wLog_Pos = Win_Params_Dict["w_log"].rt.anchoredPosition.ToString();
            cur_wLog_Size = Win_Params_Dict["w_log"].rt.sizeDelta.ToString();
        #endif
    }

    public void Switch_Mode(int mode, bool force = false, float anim_duration = -1f) {
        if (force) {
            Window_RobotCam.DOKill(); Window_Script.DOKill(); Window_Script_Border_Win.DOKill();
        } else {
            if (DOTween.IsTweening(Window_RobotCam) || DOTween.IsTweening(Window_Script) || DOTween.IsTweening(Window_Script_Border_Win)) return;
        }

        if (mode == 1) {
            //Activate win mode
            if (Camera.main.targetTexture != null) return; //already activated

            if (anim_duration < 0f) anim_duration = anim_in_duration;

            Camera MainCam = Camera.main;
            RenderTexture rt = (RenderTexture)Window_RobotCam.transform.GetChild(0).GetComponent<RawImage>().mainTexture;
            MainCam.targetTexture = rt;
            BigEdit_Cam.gameObject.SetActive(true);
            desktop_wallpaper.SetActive(true);
            Window_RobotCam.gameObject.SetActive(true);

            //Options.Show_Hide_Tutorial_Dialog_Button(true, anim_in_duration);

            //Force never activate/deactivate, because it's handled by win-manager
            //txt_editor.Activate_Settings.Activate = Text_Editor.Activate_Scheme_Info.Activate_Enum.Never;
            //txt_editor.Activate_Settings.DeActivate = Text_Editor.Activate_Scheme_Info.DeActivate_Enum.Never;
            txt_editor.Activate_Settings.Activate = Text_Editor.Activate_Scheme_Info.Activate_Enum.OnClick;
            txt_editor.Activate_Settings.DeActivate = Text_Editor.Activate_Scheme_Info.DeActivate_Enum.OnClickOutside;
            Engine.camera_Controller.cam_control_activation = Camera_Controller.cam_control_activation_enum.OnEditorDeActivation;

            Restore_Last_Win_Geometry(0, anim_duration);
            Restore_Last_Win_Geometry(1, anim_duration);

            //Script window borders cross fade
            Window_Script_Border_Win.DOKill(); Window_Script_Border_Std.DOKill();
            Window_Script_Border_Win.DOFade ( 1f, anim_duration ).SetUpdate(true);
            Window_Script_Border_Std.DOFade ( 0f, anim_duration ).SetUpdate(true);

            Window_Script.SetSiblingIndex(2);
        }
        else if (mode == 0) {
            //DeActivate win mode
            if (Camera.main.targetTexture == null) return; //already deactivated

            if (anim_duration < 0f) anim_duration = anim_out_duration;

            //Options.Show_Hide_Tutorial_Dialog_Button(false, anim_in_duration);

            Restore_Original_Win_Geometry(0, anim_duration);
            Restore_Original_Win_Geometry(1, anim_duration); //Sibling index will be set to this window (last called)

            //Script window borders cross fade
            Window_Script_Border_Win.DOKill(); Window_Script_Border_Std.DOKill();
            Window_Script_Border_Win.DOFade ( 0f, anim_duration ).SetUpdate(true);
            Window_Script_Border_Std.DOFade ( 1f, anim_duration ).SetUpdate(true).OnComplete(()=>{
                Camera MainCam = Camera.main;
                MainCam.targetTexture = null;
                BigEdit_Cam.gameObject.SetActive(false);
                desktop_wallpaper.SetActive(false);
                Window_RobotCam.gameObject.SetActive(false);
                Window_RobotCam.SetSiblingIndex(2);

                //Restore activation settings
                Options_Global_Static.ed_activate.onValueChanged.Invoke( Options_Global_Static.ed_activate.value );
                Options_Global_Static.ed_deactivate.onValueChanged.Invoke( Options_Global_Static.ed_activate.value );
                Options_Global_Static.ed_camera_controls.onValueChanged.Invoke( Options_Global_Static.ed_camera_controls.value );
            });
        }
    }

    public void Restore_Last_Win_Geometry (int win, float anim_duration = -1f) {
        if (!is_active) return;

        if (anim_duration < 0f) anim_duration = anim_in_duration;

        if (win == 0) {
            Window_RobotCam.DOKill();
            Quaternion g0 = win_desktop_geometries["0"];
            Window_RobotCam.DOAnchorPos( new Vector2(g0.x, g0.y), anim_duration ).SetUpdate(true);
            Window_RobotCam.DOSizeDelta( new Vector2(g0.z, g0.w), anim_duration ).SetUpdate(true).OnUpdate(()=> Resize_Robot_Camera_Render_Texture() );
        }
        else if (win == 1) {
            Window_Script.DOKill();
            Quaternion g1 = win_desktop_geometries["1"];
            Window_Script.DOAnchorPos( new Vector2(g1.x, g1.y), anim_duration ).SetUpdate(true);
            Window_Script.DOSizeDelta( new Vector2(g1.z, g1.w), anim_duration ).SetUpdate(true);
        }
    }

    public void Restore_Original_Win_Geometry (int win, float anim_duration = -1f) {
        if (!is_active) return;

        if (anim_duration < 0f) anim_duration = anim_out_duration;

        if (win == 0) {
            Window_RobotCam.DOKill();

            //Save last windows geometries
            win_desktop_geometries["0"] = new Quaternion(Window_RobotCam.anchoredPosition.x, Window_RobotCam.anchoredPosition.y, Window_RobotCam.sizeDelta.x, Window_RobotCam.sizeDelta.y);

            Window_RobotCam.DOAnchorPos( new Vector2(win_robot_orig_geom.x, win_robot_orig_geom.y), anim_duration ).SetUpdate(true);
            Window_RobotCam.DOSizeDelta( new Vector2(win_robot_orig_geom.z, win_robot_orig_geom.w), anim_duration ).SetUpdate(true).OnUpdate(()=> Resize_Robot_Camera_Render_Texture() );

            Window_RobotCam.SetSiblingIndex(2);
        }
        else if (win == 1) {
            Window_Script.DOKill();

            //Save last windows geometries
            win_desktop_geometries["1"] = new Quaternion(Window_Script.anchoredPosition.x, Window_Script.anchoredPosition.y, Window_Script.sizeDelta.x, Window_Script.sizeDelta.y);

            Window_Script.DOAnchorPos( new Vector2(win_script_orig_geom.x, win_script_orig_geom.y), anim_duration ).SetUpdate(true);
            Window_Script.DOSizeDelta( new Vector2(win_script_orig_geom.z, win_script_orig_geom.w), anim_duration ).SetUpdate(true);

            Window_Script.SetSiblingIndex(2);
        }
    }

    public void Border_Pointer_Handler (Desktop_Manager_DragArea da, event_type e, border_sides side) {
        //Handle script editor resize in no-window mode
        if (!is_active && da.w_tag == "w_script" && side == border_sides.left) {
            if (e == event_type.p_enter) {
                Border_Pointer_Handler_Set_Cursor(border_sides.left);
            } else if (e == event_type.p_exit) {
                if (drag_resize_point.x > float.MinValue) return; //If dragging don't change cursor
                Border_Pointer_Handler_Set_Cursor(border_sides.middle);
            } else if (e == event_type.drag_begin) {
                if (!Input.GetMouseButton(0)) return; //Don't trigger drag with right button

                Border_Pointer_Handler_Set_Cursor(side);
                drag_resize_point = new Vector2(Input.mousePosition.x, float.MinValue);
                drag_resize_orig_size = Window_Script.sizeDelta;
            } else if (e == event_type.drag_end) {
                drag_resize_point = new Vector2(float.MinValue, float.MinValue);
                if (ExtensionMethods.ObjectsUnderPointer().Where(go=> go == da.gameObject).Count() == 0) {
                    if (cursor_std == null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
                    else Cursor.SetCursor(cursor_std, new Vector2(2, 3), CursorMode.Auto);                    
                }
            } else if (e == event_type.drag) {
                if (!Input.GetMouseButton(0)) return;

                //Move/Resize (dragging)
                float scale = desktop_wallpaper.transform.parent.GetComponent<Canvas>().scaleFactor;
                float delta = drag_resize_point.x - Input.mousePosition.x;
                delta = delta / scale;

                float new_size = drag_resize_orig_size.x + delta;
                if (new_size < min_script_width) new_size = min_script_width;
                Window_Script.sizeDelta = new Vector2(new_size, Window_Script.sizeDelta.y);                
            }
        }

        if (!is_active && da.w_tag != "w_log") return;

        if (e == event_type.click) {
            if (da.w_tag == "w_robotcam") { Window_RobotCam.SetSiblingIndex(2); }
            if (da.w_tag == "w_script") { Window_Script.SetSiblingIndex(2); }
        }

        if (e == event_type.p_enter) {
            if (drag_resize_point.x > float.MinValue) return; //If dragging don't change cursor
            Border_Pointer_Handler_Set_Cursor(side);
        } else if (e == event_type.p_exit) {
            //Debug.Log("P_Exit");
            if (drag_resize_point.x > float.MinValue) return; //If dragging don't change cursor
            Border_Pointer_Handler_Set_Cursor(border_sides.middle);
        } else if (e == event_type.drag_begin) {
            //Debug.Log("Begin_Drag");
            if (!Input.GetMouseButton(0)) return; //Don't trigger drag with right button

            Border_Pointer_Handler_Set_Cursor(side);
            drag_resize_point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RectTransform rt = null;
            if      (da.w_tag == "w_robotcam") { rt = Window_RobotCam; }
            else if (da.w_tag == "w_script")   { rt = Window_Script; }
            else if (da.w_tag == "w_log")      { rt = Window_Log; }
            drag_resize_orig_size = rt.sizeDelta;
            drag_resize_orig_pos = rt.anchoredPosition;            
        } else if (e == event_type.drag_end) {
            drag_resize_point = new Vector2(float.MinValue, float.MinValue);

            //If we are off the dragging object - reset cursor
            if (ExtensionMethods.ObjectsUnderPointer().Where(go=> go == da.gameObject).Count() == 0) {
                if (cursor_std == null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
                else Cursor.SetCursor(cursor_std, new Vector2(2, 3), CursorMode.Auto);                    
            }
        } else if (e == event_type.drag) {
            if (!Input.GetMouseButton(0)) return; //Don't trigger drag with right button

            //Move/Resize (dragging)
            float scale = desktop_wallpaper.transform.parent.GetComponent<Canvas>().scaleFactor;
            Vector2 delta = drag_resize_point - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            delta = delta / scale;

            float dx_min = 0f; float dy_min = 0f; RectTransform rt = null;
            var param = Win_Params_Dict[da.w_tag];
            dx_min = param.Min_Size.x - drag_resize_orig_size.x;
            dy_min = param.Min_Size.y - drag_resize_orig_size.y;
            rt = param.rt;
            // if (da.w_tag == "w_robotcam") {
            //     //Robot camera window
            //     dx_min = Min_Size_Robot.x - drag_resize_orig_size.x;
            //     dy_min = Min_Size_Robot.y - drag_resize_orig_size.y;
            //     rt = Window_RobotCam;
            // } 
            // else if (da.w_tag == "w_script") {
            //     //Script window
            //     dx_min = Min_Size_Script.x - drag_resize_orig_size.x;
            //     dy_min = Min_Size_Script.y - drag_resize_orig_size.y;
            //     rt = Window_Script;
            // }
            // else if (da.w_tag == "w_log") {
            //     rt = Window_Log;
            // }

            Vector2 new_pos = drag_resize_orig_pos;
            string b = side.ToString();
            if (!b.Contains("top") && !b.Contains("bottom") && !b.Contains("middle")) delta.y = 0;
            if (!b.Contains("left") && !b.Contains("right") && !b.Contains("middle")) delta.x = 0;
            if (b.Contains("left"))   { if (delta.x < dx_min) { delta.x = dx_min; } new_pos.x = drag_resize_orig_pos.x - (delta.x * (1f - rt.pivot.x)); }
            if (b.Contains("right"))  { delta.x *= -1; if (delta.x < dx_min) { delta.x = dx_min; } new_pos.x = drag_resize_orig_pos.x + (delta.x * rt.pivot.x); }
            if (b.Contains("bottom")) { if (delta.y < dy_min) { delta.y = dy_min; } new_pos.y = drag_resize_orig_pos.y - (delta.y * (1f - rt.pivot.y)); }
            if (b.Contains("top"))    { delta.y *= -1; if (delta.y < dy_min) { delta.y = dy_min; } new_pos.y = drag_resize_orig_pos.y + (delta.y * rt.pivot.y); }
            if (b.Contains("middle")) { new_pos = drag_resize_orig_pos - delta; }

            if (!b.Contains("middle")) rt.sizeDelta = drag_resize_orig_size + delta;
            rt.anchoredPosition = new_pos;

            if (da.w_tag == "w_robotcam") { Resize_Robot_Camera_Render_Texture(); }
        }
    }

    void Border_Pointer_Handler_Set_Cursor(border_sides side) {
        switch (side) {
            case border_sides.left         : Cursor.SetCursor(cursor_resize_w, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.right        : Cursor.SetCursor(cursor_resize_w, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.bottom       : Cursor.SetCursor(cursor_resize_h, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.top          : Cursor.SetCursor(cursor_resize_h, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.left_bottom  : Cursor.SetCursor(cursor_resize_lb_rt, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.right_bottom : Cursor.SetCursor(cursor_resize_lt_rb, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.right_top    : Cursor.SetCursor(cursor_resize_lb_rt, new Vector2(16, 16), CursorMode.Auto); break;
            case border_sides.left_top     : Cursor.SetCursor(cursor_resize_lt_rb, new Vector2(16, 16), CursorMode.Auto); break;
            default                        : if (cursor_std == null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
                                             else Cursor.SetCursor(cursor_std, new Vector2(2, 3), CursorMode.Auto); 
                                             break;
        }
    }

    public void Max_Restore (string param) {
        if (!is_active) return;

        RectTransform win = null;
        Vector2 geom_p = Vector2.zero;
        Vector2 geom_s = Vector2.zero;

        if (param.ToLower().EndsWith("max")) {
            if (param.StartsWith("0")) {
                //Robot cam window
                if (DOTween.IsTweening(Window_RobotCam)) return;

                win = Window_RobotCam;
                Vector2 pos_max = new Vector2(Max_RobotCam_Geom.x, Max_RobotCam_Geom.y);
                Vector2 size_max = new Vector2(Max_RobotCam_Geom.z, Max_RobotCam_Geom.w);
                if (Window_RobotCam.anchoredPosition == pos_max && Window_RobotCam.sizeDelta == size_max) {
                    //Minimize
                    var g = win_desktop_geometries["0"];
                    geom_p = new Vector2(g.x, g.y); geom_s = new Vector2(g.z, g.w);
                } else {
                    //Maximize
                    win_desktop_geometries["0"] = new Quaternion(Window_RobotCam.anchoredPosition.x, Window_RobotCam.anchoredPosition.y, Window_RobotCam.sizeDelta.x, Window_RobotCam.sizeDelta.y);
                    geom_p = pos_max; geom_s = size_max;
                }
            }
            if (param.StartsWith("1")) {
                //Script window 
                if (DOTween.IsTweening(Window_Script) || DOTween.IsTweening(Window_Script_Border_Win)) return;

                //Default orig script size: -7, -7, 255, 380
                //Max script size: 8, -2, 850 (canvas.size.x + 29), 459 (canvas.size.y + 3)

                //Get canvas size
                Vector2 size = desktop_wallpaper.transform.parent.GetComponent<RectTransform>().rect.size;

                win = Window_Script;
                Vector2 pos_max = new Vector2(Max_Script_Geom.x, Max_Script_Geom.y);
                Vector2 size_max = new Vector2(size.x + Max_Script_Geom.z, size.y + Max_Script_Geom.w);
                //Debug.Log("pos_cur = " + Window_Script.anchoredPosition + ", size_cur = " + Window_Script.sizeDelta + ", pos_max = " + pos_max + ", size_max = " + size_max);
                if (Window_Script.anchoredPosition == pos_max && Window_Script.sizeDelta == size_max) {
                    //Minimize
                    var g = win_desktop_geometries["1"];
                    geom_p = new Vector2(g.x, g.y); geom_s = new Vector2(g.z, g.w);
                } else {
                    //Maximize
                    win_desktop_geometries["1"] = new Quaternion(Window_Script.anchoredPosition.x, Window_Script.anchoredPosition.y, Window_Script.sizeDelta.x, Window_Script.sizeDelta.y);
                    geom_p = pos_max; geom_s = size_max;
                }
            }
        } else if (param.ToLower().EndsWith("full")) {
            if (param.StartsWith("0")) {
                //Robot cam window
                if (DOTween.IsTweening(Window_RobotCam)) return;

                win = Window_RobotCam;
                Vector2 pos_full = new Vector2(Full_RobotCam_Geom.x, Full_RobotCam_Geom.y);
                Vector2 size_full = new Vector2(Full_RobotCam_Geom.z, Full_RobotCam_Geom.w);
                if (Window_RobotCam.anchoredPosition == pos_full && Window_RobotCam.sizeDelta == size_full) {
                    //Restore
                    var g = win_desktop_geometries["0-Full"];
                    geom_p = new Vector2(g.x, g.y); geom_s = new Vector2(g.z, g.w);
                } else {
                    //Go Full screen
                    win_desktop_geometries["0-Full"] = new Quaternion(Window_RobotCam.anchoredPosition.x, Window_RobotCam.anchoredPosition.y, Window_RobotCam.sizeDelta.x, Window_RobotCam.sizeDelta.y);
                    geom_p = pos_full; geom_s = size_full;
                }
            }
            if (param.StartsWith("1")) {
                //Script window 
                if (DOTween.IsTweening(Window_Script) || DOTween.IsTweening(Window_Script_Border_Win)) return;

                //Get canvas size
                Vector2 size = desktop_wallpaper.transform.parent.GetComponent<RectTransform>().rect.size;

                win = Window_Script;
                Vector2 pos_full = new Vector2(Full_Script_Geom.x, Full_Script_Geom.y);
                Vector2 size_full = new Vector2(size.x + Full_Script_Geom.z, size.y + Full_Script_Geom.w);
                //Debug.Log(size_full);
                if (Window_Script.anchoredPosition == pos_full && Window_Script.sizeDelta == size_full) {
                    //Restore
                    //Debug.Log("Restore");
                    var g = win_desktop_geometries["1-Full"];
                    geom_p = new Vector2(g.x, g.y); geom_s = new Vector2(g.z, g.w);
                } else {
                    //Go Full screen
                    //Debug.Log("FullScreen");
                    win_desktop_geometries["1-Full"] = new Quaternion(Window_Script.anchoredPosition.x, Window_Script.anchoredPosition.y, Window_Script.sizeDelta.x, Window_Script.sizeDelta.y);
                    geom_p = pos_full; geom_s = size_full;
                }
            }
        }

        // .OnComplete callback is needed to set final pos, because tween is often offset by half pixel 
        //and science I check it to get window state - the check fails
        win.DOAnchorPos( geom_p , anim_max_duration ).SetUpdate(true).OnComplete(()=> win.anchoredPosition = geom_p);
        var t = win.DOSizeDelta( geom_s , anim_max_duration ).SetUpdate(true).OnComplete(()=> win.sizeDelta = geom_s);
        if (win == Window_RobotCam) t.OnUpdate( ()=> Resize_Robot_Camera_Render_Texture() );
    }

    void Resize_Robot_Camera_Render_Texture() {
        //float scaling_texture = 2f;
        float scaling_texture = Window_Script_Canvas.scaleFactor;
        Camera.main.targetTexture.Release();
        Camera.main.targetTexture = new RenderTexture( Mathf.RoundToInt(Window_RobotCam.rect.width * scaling_texture), Mathf.RoundToInt(Window_RobotCam.rect.height * scaling_texture), 24, RenderTextureFormat.RGB111110Float );
        Window_RobotCam.transform.GetChild(0).GetComponent<RawImage>().texture = Camera.main.targetTexture;
    }

    public void Ensure_Script_Window_Original_Size() {
        if (Camera.main.targetTexture != null) return; //We are in window mode, nothing to do
        //TODO: if, at some point, we will deactivate win mode to no-original winScript size,
        //          then this should be called BEFORE Switch_Mode() to ensure it will return to its original size
        //          but Switch_Mode() will kill DOSizeDelta tween. Need to figure out how to do it better
        //          maybe making a parameter in Switch_Mode couldd do the trick
        if (DOTween.IsTweening(Window_Script)) return; //We are switching modes
        if (Mathf.Approximately(Window_Script.sizeDelta.x, min_script_width)) return; //Already in default size
        Window_Script.DOSizeDelta( new Vector2(min_script_width, Window_Script.sizeDelta.y), 0.7f).SetUpdate(true);
    }

    public bool is_active {
        get { 
            if (Camera.main == null) return false;
            return Camera.main.targetTexture != null; 
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: valueStr = prop.intValue.ToString(); break;
            case SerializedPropertyType.Boolean: valueStr = prop.boolValue.ToString(); break;
            case SerializedPropertyType.Float:   valueStr = prop.floatValue.ToString("0.00000"); break;
            case SerializedPropertyType.String:  valueStr = prop.stringValue; break;
            default:  valueStr = "(not supported)"; break;
        }
        EditorGUI.LabelField(position,label.text, valueStr);
    }
}
public class ShowOnlyAttribute : PropertyAttribute { }
#endif