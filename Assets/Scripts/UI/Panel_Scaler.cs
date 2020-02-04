using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script editor border: T: Left/Right 60, Top 61.5
//                      TL: -44,37; TR: 46,37

namespace ASTT {
public class Panel_Scaler : MonoBehaviour
{
    public float scale = 100f;
    public offset_info offsets = new offset_info();

    [System.Serializable]
    public class offset_info {
        public float GLB_L = 0f;
        public float GLB_R = 0f;
        public float GLB_T = 0f;
        public float GLB_B = 0f;
        public float TL_l = 0;
        public float TL_t = 0;
        public float TL_r = 0;
        public float TL_b = 0;
        public float TR_l = 0;
        public float TR_t = 0;
        public float TR_r = 0;
        public float TR_b = 0;
        public float BL_l = 0;
        public float BL_t = 0;
        public float BL_r = 0;
        public float BL_b = 0;
        public float BR_l = 0;
        public float BR_t = 0;
        public float BR_r = 0;
        public float BR_b = 0;
        public bool L_ABS = false;
        public float L_l = 0;
        public float L_t = 0;
        public float L_r = 0;
        public float L_b = 0;
        public bool R_ABS = false;
        public float R_l = 0;
        public float R_t = 0;
        public float R_r = 0;
        public float R_b = 0;
        public bool T_ABS = false;
        public float T_l = 0;
        public float T_t = 0;
        public float T_r = 0;
        public float T_b = 0;
        public bool B_ABS = false;
        public float B_l = 0;
        public float B_t = 0;
        public float B_r = 0;
        public float B_b = 0;
    }

    void OnValidate()
    {
        this.Invoke("Update_Size", 0f);
    }

    // Update is called once per frame
    void Update_Size()
    {
        var cnt = GetComponent<RectTransform>();

        string offset_params = "FILL: NO;";

        offset_params += "GLB: " + string.Join("|", new float[]{offsets.GLB_L, offsets.GLB_T, offsets.GLB_R, offsets.GLB_B}) + ";" ;

        offset_params += "TL: " + string.Join("|", new float[]{offsets.TL_l, offsets.TL_t, offsets.TL_r, offsets.TL_b}) + ";" ;
        offset_params += "TR: " + string.Join("|", new float[]{offsets.TR_l, offsets.TR_t, offsets.TR_r, offsets.TR_b}) + ";" ;
        offset_params += "BL: " + string.Join("|", new float[]{offsets.BL_l, offsets.BL_t, offsets.BL_r, offsets.BL_b}) + ";" ;
        offset_params += "BR: " + string.Join("|", new float[]{offsets.BR_l, offsets.BR_t, offsets.BR_r, offsets.BR_b}) + ";" ;
        offset_params += "L: " + string.Join("|", new float[]{offsets.L_l, offsets.L_t, offsets.L_r, offsets.L_b}) + (offsets.L_ABS ? "|ABS" : "") + ";" ;
        offset_params += "R: " + string.Join("|", new float[]{offsets.R_l, offsets.R_t, offsets.R_r, offsets.R_b}) + (offsets.R_ABS ? "|ABS" : "") + ";" ;
        offset_params += "T: " + string.Join("|", new float[]{offsets.T_l, offsets.T_t, offsets.T_r, offsets.T_b}) + (offsets.T_ABS ? "|ABS" : "") + ";" ;
        offset_params += "B: " + string.Join("|", new float[]{offsets.B_l, offsets.B_t, offsets.B_r, offsets.B_b}) + (offsets.B_ABS ? "|ABS" : "") + ";" ;
        offset_params = offset_params.Replace(",", ".").Replace("|", ",");

        ASTT.Sprite_Scaler.ScaleSprites(cnt, scale / 100f, offset_params);
    }
}



}

