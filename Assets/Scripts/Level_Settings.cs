using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Settings : MonoBehaviour
{
    public AudioClip LevelMusic = null;
    public AudioClip[] LevelSounds = null;

    public camera_overrides_info camera_controls_overrides = new camera_overrides_info();
    public camera_cinematic_info camera_cinematic = new camera_cinematic_info();

    [System.Serializable]
    public class camera_overrides_info {
        public bool camera_handled_by_level = false;

        public bool disable_controls_zoom = false;
        public bool disable_controls_rotate_around = false;
        public bool disable_controls_rotate_rightMouse = false;

        public bool override_camera_position = false;
        public bool disable_camera_reposition = false;
        public Vector3 target_offset = Vector3.zero;
        public Vector2 rotation_center_offset = Vector2.zero;
        public float camera_height = 5.63f;
        public float camera_angle = 307.5f;
        public float target_distance = 12.34f;
    }
    [System.Serializable]
    public class camera_cinematic_info {
        public bool enabled = false;
        public cinematic_info[] level_start_cinematics = new cinematic_info[]{};
        public cinematic_loop level_start_cinematics_loop = cinematic_loop.none;
        public cinematic_order level_start_cinematics_order = cinematic_order.forward;
        public cinematic_info[] level_cinematics = new cinematic_info[]{};
        public cinematic_loop level_cinematics_loop = cinematic_loop.none;
        public cinematic_order level_cinematics_order = cinematic_order.forward;

        public enum cinematic_loop {none, loop, yoyo};
        public enum cinematic_order {forward, random};

        [System.Serializable]
        public class cinematic_info {
            public AnimationClip anim = null;
            public cinematic_points[] way_points = new cinematic_points[]{};
            public cinematic_points[] look_points = new cinematic_points[]{};
            public CameraPath camera_path = null;
            public BezierSolution.BezierSpline bezier_spline = null;
            public BezierSolution.BezierSpline bezier_spline_look = null;
            public float speed = 0.5f;

            [System.Serializable]
            public class cinematic_points {
                public Transform point_t = null;
                public Vector3 point_v = Vector3.zero;
                public bool look_to_bot;
            }
        }
    }

    public GameObject[] Level_Disableable = new GameObject[]{};

    public void Disable_On_Animation_Event() {
        foreach (var g in Level_Disableable) g.SetActive(false);
    }
}
