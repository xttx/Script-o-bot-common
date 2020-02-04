using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arc_Placer : MonoBehaviour
{
    public float start_angle = 0f;
    public float end_angle = 90f;
    public float arc_radius = 500f;
    public float x_stretch = 1f;
    public float y_stretch = 1f;

    public float rot_start = 0f;
    public float rot_end = 0f;
    public bool look_at_rotation = false;
    
    public Vector2 center_offset = Vector2.zero;
    public Vector2 scale = new Vector2(1f, 1f);

    public float[] angle_offsets = new float[]{};
    public Vector2[] scale_offsets = new Vector2[]{};

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnValidate()
    {
        //GetComponent<RectTransform>().anchoredPosition = center_offset;

        int count = transform.childCount;
        float cur_angle = start_angle;
        float cur_rot_offset = rot_start;
        float angle_step = (end_angle - start_angle) / (float)(count - 1);
        float rot_step = (rot_end - rot_start) / (float)(count - 1);
        for (int i = 0; i < count; i++) {
            var rt = transform.GetChild(i).GetComponent<RectTransform>();

            float cur_angle_offset = 0f;
            if (angle_offsets.Length > i) cur_angle_offset = angle_offsets[i];
            var x = Mathf.Sin((cur_angle + cur_angle_offset) * Mathf.Deg2Rad) * arc_radius * x_stretch;
            var y = Mathf.Cos((cur_angle + cur_angle_offset) * Mathf.Deg2Rad) * arc_radius * y_stretch;

            rt.anchoredPosition = new Vector2(x, y); // - center_offset;
            if (look_at_rotation)
                rt.up = new Vector2(x + center_offset.x, y + center_offset.y);
            else
                rt.localRotation = Quaternion.Euler(0f, 0f, -cur_angle);

            rt.localRotation = Quaternion.Euler(0f, 0f, rt.localRotation.eulerAngles.z - cur_rot_offset);

            Vector2 cur_scale_offset = new Vector2(0f, 0f);
            if (scale_offsets.Length > i) cur_scale_offset = scale_offsets[i];
            rt.localScale = new Vector3(scale.x + cur_scale_offset.x, scale.y + cur_scale_offset.y, 1f);

            cur_angle += angle_step;
            cur_rot_offset += rot_step;
        }
    }
}
