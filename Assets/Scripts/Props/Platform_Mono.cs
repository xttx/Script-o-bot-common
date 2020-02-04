using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Platform_Mono : MonoBehaviour, Engine.IResetable
{
    public Vector2Int[] override_X = new Vector2Int[]{};
    public Vector2Int[] override_Y = new Vector2Int[]{};
    public Vector2Int[] override_Z = new Vector2Int[]{};
    public Vector2Int limit_X = new Vector2Int(-999, 999);
    public Vector2Int limit_Y = new Vector2Int(-999, 999);
    public Vector2Int limit_Z = new Vector2Int(-999, 999);
    public UnityEvent OnMoveBegin = null;
    public UnityEvent OnMoveEnd = null;
    public UnityEvent OnReset = null;

    public ParticleSystem particles = null;
    public GameObject fx = null;

    public anim_info[] anim_chain = new anim_info[]{};
    public enum animation_info {delay, spawn_particles, translate_bot, scale_bot, rotate_bot, move_platform};
    [System.Serializable]
    public class anim_info {
        public animation_info animation;
        public float delay = 0f;
        public float duration = 1f;
        public Vector3 param = Vector3.zero;
        public bool use_offset_of_bot_pos = false;
        public bool wait = false;
        public Vector3 rotation = Vector3.zero;
    }

    // public test_info[] test_arr = new test_info[]{};
    // [System.Serializable]
    // public class test_info {
    //     public float a;
    //     public animation_info animation;
    //     public float delay = 0f;
    //     public float duration = 1f;
    //     public Vector3 param = Vector3.zero;
    //     public bool use_offset_of_bot_pos = false;
    //     public bool wait = false;
    //     public Vector3 rotation = Vector3.zero;
    // }
    // public test_info test_info_no_arr = new test_info();

    Vector3 pos_orig = Vector3.zero;

    void Start() { pos_orig = transform.position; }

    public void Reset() {
        BOT_Helpers.Platform.wait = false;
        BOT_Helpers.Platform.inst.StopAllCoroutines();
        foreach (var p in BOT_Helpers.Platform.spawn_particles) { Destroy(p.gameObject); }
        foreach (var g in BOT_Helpers.Platform.spawn_gameobjects) { Destroy(g); }
        BOT_Helpers.Platform.spawn_particles.Clear();
        BOT_Helpers.Platform.spawn_gameobjects.Clear();
        transform.DOKill(); BOT.bot_obj.transform.DOKill();
        BOT.bot_obj.transform.SetParent(null, true);
        BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = true;
        DontDestroyOnLoad(BOT.bot_obj);

        transform.position = pos_orig;
        if (OnReset != null) OnReset.Invoke();
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Platform_Mono.anim_info))]
public class MyScriptEditor : PropertyDrawer
{
    SerializedProperty p_anim;
    SerializedProperty p_delay;
    SerializedProperty p_duration;
    SerializedProperty p_V3_param;
    SerializedProperty p_use_bot_offset;
    SerializedProperty p_wait;
    SerializedProperty p_V3_rotation;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // First get the attribute since it contains the range for the slider
        RangeAttribute range = attribute as RangeAttribute;

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        position.height = EditorGUIUtility.singleLineHeight;

        property.isExpanded = EditorGUI.Foldout (position, property.isExpanded, label);

        if (property.isExpanded) {
            var indentedRect = EditorGUI.IndentedRect(position);
            float step_height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            p_anim = property.FindPropertyRelative("animation");
            p_delay = property.FindPropertyRelative("delay");
            p_duration = property.FindPropertyRelative("duration");
            p_V3_param = property.FindPropertyRelative("param");
            p_use_bot_offset = property.FindPropertyRelative("use_offset_of_bot_pos");
            p_wait = property.FindPropertyRelative("wait");
            p_V3_rotation = property.FindPropertyRelative("rotation");

            position.y += step_height;
            position.x = indentedRect.x;
            label.text = "Anim";
            EditorGUI.PropertyField(position, p_anim, label, true);

            //position.y += step_height;
            //Rect r = new Rect(position.x, position.y, 100f, step_height * 2.5f );
            //EditorGUI.DrawRect(r, Color.gray);
            // EditorGUI.LabelField(position, "asdfasdfasdfasd\nfasdfasdfasdf\nasdfasdfsa", GUIStyle.none);
            // position.y += step_height * 1.5f;


            bool[] enable_arr = new bool[6];

            if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.delay)
            { 
                enable_arr = new bool[]{true, false, false, false, false, false};
            }
            else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.move_platform)
            { 
                enable_arr = new bool[]{false, false, true, true, false, true};
            }
            else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.rotate_bot)
            { 
                enable_arr = new bool[]{true, true, false, false, true, true};
            }
            else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.scale_bot)
            { 
                enable_arr = new bool[]{true, true, true, true, true, false};
            }
            else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.spawn_particles)
            { 
                enable_arr = new bool[]{false, false, true, true, false, true};
            }
            else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.translate_bot)
            { 
                enable_arr = new bool[]{false, false, false, false, false, false};
            }

            SerializedProperty[] p_arr = new SerializedProperty[]{p_delay, p_duration, p_V3_param, p_use_bot_offset, p_wait, p_V3_rotation};

            position.y += step_height;
            for (int i = 0; i < p_arr.Length; i++) {
                if (!enable_arr[i]) continue;

                //position.y += step_height;
                label.text = p_arr[i].name;
                EditorGUI.PropertyField(position, p_arr[i], label, true);
                position.y += EditorGUI.GetPropertyHeight(p_arr[i], label);

            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = 1;
        p_anim = property.FindPropertyRelative("animation");
        p_V3_param = property.FindPropertyRelative("param");
        float v3_param_height = EditorGUI.GetPropertyHeight(p_V3_param, label);
        float v_space = EditorGUIUtility.standardVerticalSpacing;

        float add_height = 0f; //(EditorGUIUtility.singleLineHeight + v_space) * 2.5f;

        if (!property.isExpanded || p_anim == null) { lineCount = 1; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.delay)           
            { lineCount = 3; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.move_platform)   
            { lineCount = 3; add_height += v3_param_height * 2f + v_space; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.rotate_bot)
            { lineCount = 5; add_height += v3_param_height + v_space; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.scale_bot)
            { lineCount = 6; add_height += v3_param_height + v_space; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.spawn_particles)
            { lineCount = 3; add_height += v3_param_height * 2f + v_space; }
        else if (p_anim.enumValueIndex == (int)Platform_Mono.animation_info.translate_bot)   
            { lineCount = 2; }

        return ( EditorGUIUtility.singleLineHeight * lineCount ) + ( v_space * (lineCount-1) ) + add_height;
    }
}
#endif