using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Floor_Random_Square_Glow2 : MonoBehaviour
{
    public anim_info[] animations = new anim_info[]{new anim_info()};
    [System.Serializable]
    public class anim_info {
        public Vector2 time_range = Vector2.zero;
        public Pattern_Info pattern_anim = Pattern_Info.RandomGlow;
        public float anim_delay_pattern = 0.2f;
        public float anim_random_glow_in = 0.3f;
        public float anim_random_glow_out = 0.7f;
        public float anim_random_delay = 0.2f;
        public Vector2Int anim_random_glow_count = new Vector2Int(2, 7);
        public Vector2 anim_random_glow2_in = new Vector2(0.2f, 0.5f);
        public Vector2 anim_random_glow2_out = new Vector2(0.4f, 0.8f);
        public Vector2Int anim_random_glow2_count = new Vector2Int(1, 4);
        public Vector2 pattern_glow_out = Vector2.zero;
        public float glow_intensity = 1.3f;
    }
    public enum Pattern_Info { RandomGlow, RandomGlowExt, Grid, Columns, Rows, Square, Spin, Checker, Checker2 }

    int anim_step = 0;
    float anim_switch_time = 0f;
    int pattern_step = 0;

    Material mat = null;

    GameObject[,] squares = new GameObject[9,9];
    Vector2[,] square_tex_offsets = new Vector2[9,9];
    float time = 0f;

    Dictionary<int, List<GameObject>[]> patterns = new Dictionary<int, List<GameObject>[]>();
    // class square_inf {
    //     int row;
    //     int col;
    //     public square_inf(int r, int c) { row = r; col = c; }
    // }

    List<Material> glowing_materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        //Global color cycling
        mat = GetComponent<MeshRenderer>().material;
        mat.DOColor(new Color(0.8f, 0.8f, 0.8f), 1.5f).SetLoops(-1, LoopType.Yoyo);

        //Creating additional squares on top of the main
        //float offset_x = 0.2f; //No cropped version of floor
        float offset_x = 0.0f;
        for (int x = 4; x >= -4; x--) {
            //float offset_z = 0.2f; //No cropped version of floor
            float offset_z = 0.0f;
            for (int z = 4; z >= -4; z--) {
                GameObject plane  = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //No cropped version of floor
                //plane.transform.parent = transform; //No cropped version of floor
                //plane.transform.localPosition = new Vector3(x, 0.001f, z);
                //plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //Cropped version of floor
                float scale_factor = transform.lossyScale.x / 9f;
                //plane.transform.position = new Vector3(x + transform.position.x, transform.position.y + 0.001f, z + transform.position.z);// * scale_factor * 10f;
                plane.transform.localScale = new Vector3(scale_factor, scale_factor, scale_factor);
                plane.transform.SetParent(transform);
                plane.transform.localPosition = new Vector3(x / 0.9f, 0.001f, z / 0.9f);

                Material mat_plane = new Material(mat);
                plane.GetComponent<MeshRenderer>().material = mat_plane;
                mat_plane.SetTextureScale("_MainTex", new Vector2(0.2f, 0.2f));
                mat_plane.SetTextureOffset("_MainTex", new Vector2(offset_x, offset_z));
                mat_plane.SetTexture ("_EmissionMap", mat_plane.mainTexture);
                mat_plane.SetColor ("_EmissionColor", Color.black);

                // Another thing to note is that Unity 5 uses the concept of shader keywords extensively.
                // So if your material is initially configured to be without emission, then in order to enable emission,
                //you need to enable the keyword.
                mat_plane.EnableKeyword("_EMISSION");

                squares[x + 4, z + 4] = plane;
                square_tex_offsets[x + 4, z + 4] = new Vector2(offset_x, offset_z);
                plane.SetActive(false);

                offset_z += 0.2f;
            }
            offset_x += 0.2f;
        }

        CreatePatterns();

        Glow_Random_Squares();

        if (animations[0].time_range.y > 0f) anim_switch_time = Random.Range(animations[0].time_range.x, animations[0].time_range.y);
    }

    void Glow_Random_Squares()
    {
        var param = animations[anim_step];

        int count = Random.Range(param.anim_random_glow_count.x, param.anim_random_glow_count.y);
        for (int c = 1; c <= count; c++)
        {
            int row = Random.Range(0, 8);
            int col = Random.Range(0, 8);

            squares[row, col].SetActive(true);
            MeshRenderer sq = squares[row, col].GetComponent<MeshRenderer>();

            glowing_materials.Add(sq.material);

            float _glow_intensity = param.glow_intensity;
            float _anim_random_glow_in = param.anim_random_glow_in;
            float _anim_random_glow_out = param.anim_random_glow_out;
            //Debug.Log("Glow square " + row + "x" + col + ", intensity = " + _glow_intensity + ", in = " + _anim_random_glow_in + ", out " + _anim_random_glow_out);
            sq.material.DOColor(new Color(_glow_intensity, _glow_intensity, _glow_intensity, 1f), "_EmissionColor", _anim_random_glow_in).OnComplete(()=>{
                sq.material.DOColor(new Color(0f, 0f, 0f, 1f), "_EmissionColor", _anim_random_glow_out).OnComplete(()=>{
                    squares[row, col].SetActive(false);
                    glowing_materials.Remove(sq.material);
                });
            });
        }
    }
    void Glow_Random_Squares2()
    {
        var param = animations[anim_step];

        int count = Random.Range(param.anim_random_glow2_count.x, param.anim_random_glow2_count.y);
        for (int c = 1; c <= count; c++) {
            int row = Random.Range(0, 8);
            int col = Random.Range(0, 8);
            if (squares[row, col].activeSelf) continue;

            float glow_in = Random.Range(param.anim_random_glow2_in.x, param.anim_random_glow2_in.y);
            float glow_out = Random.Range(param.anim_random_glow2_out.x, param.anim_random_glow2_out.y);

            squares[row, col].SetActive(true);
            MeshRenderer sq = squares[row, col].GetComponent<MeshRenderer>();
            glowing_materials.Add(sq.material);

            float _glow_intensity = param.glow_intensity;
            sq.material.DOColor(new Color(_glow_intensity, _glow_intensity, _glow_intensity, 1f), "_EmissionColor", glow_in).OnComplete(()=>{
                sq.material.DOColor(new Color(0f, 0f, 0f, 1f), "_EmissionColor", glow_out).OnComplete(()=>{
                    glowing_materials.Remove(sq.material);
                    squares[row, col].SetActive(false);
                });
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        var param = animations[anim_step];

        if (anim_switch_time > 0f && Time.time >= anim_switch_time) {
            anim_step++;
            if (anim_step >= animations.Length) anim_step = 0;

            if (animations[anim_step].time_range.y > 0f)
                anim_switch_time = Time.time + Random.Range(animations[anim_step].time_range.x, animations[anim_step].time_range.y);
            else
                anim_switch_time = 0f;

            //If we have more then one animation - reset squares
            pattern_step = 0;
            if (animations.Length > 1) {
                for (int x = 0; x <= 8; x++) {
                    for (int y = 0; y <= 8; y++) {
                        squares[x,y].SetActive(false);
                        squares[x,y].GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                    }
                }
            }
        }

        if (param.pattern_anim == Pattern_Info.RandomGlow) {
            if (Time.time > time + param.anim_random_glow_in + param.anim_random_glow_out + param.anim_random_delay) {
                time = Time.time;
                Glow_Random_Squares();
            }
        }
        if (param.pattern_anim == Pattern_Info.RandomGlowExt) {
            if (Time.time > time) {
                time = Time.time + Random.Range(0.1f, 0.3f);
                Glow_Random_Squares2();
            }
        }

        if (param.pattern_anim == Pattern_Info.RandomGlow || param.pattern_anim == Pattern_Info.RandomGlowExt) {
            foreach (Material m in glowing_materials) {
                m.color = mat.color;
            }
        } else {
            if (Time.time <= time + param.anim_delay_pattern) return;

            time = Time.time;

            int i = 0;
            switch (param.pattern_anim) {
                case Pattern_Info.Grid      : i = 0; break;
                case Pattern_Info.Columns   : i = 1; break;
                case Pattern_Info.Rows      : i = 2; break;
                case Pattern_Info.Square    : i = 3; break;
                case Pattern_Info.Spin      : i = 4; break;
                case Pattern_Info.Checker   : i = 5; break;
                case Pattern_Info.Checker2  : i = 6; break;

            }

            if (pattern_step >= patterns[i].Length) pattern_step = 0;
            List<GameObject> p = patterns[i][pattern_step];
            
            //Reset
            if (param.pattern_glow_out.y == 0f) {
                for (int x = 0; x <= 8; x++) {
                    for (int y = 0; y <= 8; y++) {
                        squares[x,y].SetActive(false);
                        squares[x,y].GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                    }
                }
            }

            foreach (GameObject g in p) {
                g.SetActive(true);
                g.GetComponent<MeshRenderer>().material.color = new Color(1.3f, 1.3f, 1.3f, 1f);

                //Fade out
                if (param.pattern_glow_out.y > 0f) {
                    MeshRenderer sq = g.GetComponent<MeshRenderer>();
                    glowing_materials.Add(sq.material);

                    float fade = Random.Range(param.pattern_glow_out.x, param.pattern_glow_out.y);
                    sq.material.DOColor(new Color(0f, 0f, 0f, 1f), "_EmissionColor", fade).OnComplete(()=>{
                        glowing_materials.Remove(sq.material);
                        g.SetActive(false);
                    });
                }
            }
            pattern_step += 1;
        }
    }

    void OnDestroy() {
        Debug.Log("Floor object destroyed, killing tweens");
        mat.DOKill();
        foreach (var m in glowing_materials) {
            m.DOKill();
        }
    }

    void CreatePatterns() {
        //Test
        // List<square_inf> p;
        // p = new List<square_inf>();
        // p.Add(new square_inf(0,0));
        List<GameObject>[] sqs1 = new List<GameObject>[5];
        List<GameObject>[] sqs2 = new List<GameObject>[9];
        List<GameObject>[] sqs3 = new List<GameObject>[9];
        List<GameObject>[] sqs4 = new List<GameObject>[5];
        List<GameObject>[] sqs5 = new List<GameObject>[12];
        List<GameObject>[] sqs6 = new List<GameObject>[2];
        List<GameObject>[] sqs7 = new List<GameObject>[2];

        for (int i = 0; i <= 4; i++) sqs1[i] = new List<GameObject>();
        for (int i = 0; i <= 8; i++) sqs2[i] = new List<GameObject>();
        for (int i = 0; i <= 8; i++) sqs3[i] = new List<GameObject>();
        for (int i = 0; i <= 4; i++) sqs4[i] = new List<GameObject>();
        for (int i = 0; i <= 11; i++) sqs5[i] = new List<GameObject>();
        for (int i = 0; i <= 1; i++) sqs6[i] = new List<GameObject>();
        for (int i = 0; i <= 1; i++) sqs7[i] = new List<GameObject>();

        //Spin pattern
        sqs5[0].Add(squares[0,4]); sqs5[0].Add(squares[1,4]); sqs5[0].Add(squares[2,4]); sqs5[0].Add(squares[3,4]); sqs5[0].Add(squares[4,4]); sqs5[0].Add(squares[5,4]); sqs5[0].Add(squares[6,4]); sqs5[0].Add(squares[7,4]); sqs5[0].Add(squares[8,4]);
        sqs5[1].Add(squares[0,3]); sqs5[1].Add(squares[1,3]); sqs5[1].Add(squares[2,4]); sqs5[1].Add(squares[3,4]); sqs5[1].Add(squares[4,4]); sqs5[1].Add(squares[5,4]); sqs5[1].Add(squares[6,4]); sqs5[1].Add(squares[7,5]); sqs5[1].Add(squares[8,5]);
        sqs5[2].Add(squares[0,2]); sqs5[2].Add(squares[1,2]); sqs5[2].Add(squares[2,3]); sqs5[2].Add(squares[3,3]); sqs5[2].Add(squares[4,4]); sqs5[2].Add(squares[5,5]); sqs5[2].Add(squares[6,5]); sqs5[2].Add(squares[7,6]); sqs5[2].Add(squares[8,6]);
        sqs5[3].Add(squares[0,0]); sqs5[3].Add(squares[1,1]); sqs5[3].Add(squares[2,2]); sqs5[3].Add(squares[3,3]); sqs5[3].Add(squares[4,4]); sqs5[3].Add(squares[5,5]); sqs5[3].Add(squares[6,6]); sqs5[3].Add(squares[7,7]); sqs5[3].Add(squares[8,8]);
        sqs5[4].Add(squares[2,0]); sqs5[4].Add(squares[2,1]); sqs5[4].Add(squares[3,2]); sqs5[4].Add(squares[3,3]); sqs5[4].Add(squares[4,4]); sqs5[4].Add(squares[5,5]); sqs5[4].Add(squares[5,6]); sqs5[4].Add(squares[6,7]); sqs5[4].Add(squares[6,8]);
        sqs5[5].Add(squares[3,0]); sqs5[5].Add(squares[3,1]); sqs5[5].Add(squares[4,2]); sqs5[5].Add(squares[4,3]); sqs5[5].Add(squares[4,4]); sqs5[5].Add(squares[4,5]); sqs5[5].Add(squares[4,6]); sqs5[5].Add(squares[5,7]); sqs5[5].Add(squares[5,8]);
        sqs5[6].Add(squares[4,0]); sqs5[6].Add(squares[4,1]); sqs5[6].Add(squares[4,2]); sqs5[6].Add(squares[4,3]); sqs5[6].Add(squares[4,4]); sqs5[6].Add(squares[4,5]); sqs5[6].Add(squares[4,6]); sqs5[6].Add(squares[4,7]); sqs5[6].Add(squares[4,8]);
        sqs5[7].Add(squares[5,0]); sqs5[7].Add(squares[5,1]); sqs5[7].Add(squares[4,2]); sqs5[7].Add(squares[4,3]); sqs5[7].Add(squares[4,4]); sqs5[7].Add(squares[4,5]); sqs5[7].Add(squares[4,6]); sqs5[7].Add(squares[3,7]); sqs5[7].Add(squares[3,8]);
        sqs5[8].Add(squares[6,0]); sqs5[8].Add(squares[6,1]); sqs5[8].Add(squares[5,2]); sqs5[8].Add(squares[5,3]); sqs5[8].Add(squares[4,4]); sqs5[8].Add(squares[3,5]); sqs5[8].Add(squares[3,6]); sqs5[8].Add(squares[2,7]); sqs5[8].Add(squares[2,8]);
        sqs5[9].Add(squares[8,0]); sqs5[9].Add(squares[7,1]); sqs5[9].Add(squares[6,2]); sqs5[9].Add(squares[5,3]); sqs5[9].Add(squares[4,4]); sqs5[9].Add(squares[3,5]); sqs5[9].Add(squares[2,6]); sqs5[9].Add(squares[1,7]); sqs5[9].Add(squares[0,8]);
        sqs5[10].Add(squares[0,6]); sqs5[10].Add(squares[1,6]); sqs5[10].Add(squares[2,5]); sqs5[10].Add(squares[3,5]); sqs5[10].Add(squares[4,4]); sqs5[10].Add(squares[5,3]); sqs5[10].Add(squares[6,3]); sqs5[10].Add(squares[7,2]); sqs5[10].Add(squares[8,2]);
        sqs5[11].Add(squares[0,5]); sqs5[11].Add(squares[1,5]); sqs5[11].Add(squares[2,4]); sqs5[11].Add(squares[3,4]); sqs5[11].Add(squares[4,4]); sqs5[11].Add(squares[5,4]); sqs5[11].Add(squares[6,4]); sqs5[11].Add(squares[7,3]); sqs5[11].Add(squares[8,3]);

        for (int x = 0; x <= 8; x++) {
            for (int y = 0; y <= 8; y++) {
                if (x == 0 || y == 0 || x == 8 || y == 8) sqs1[0].Add( squares[x,y] );
                if (x == 1 || y == 1 || x == 7 || y == 7) sqs1[1].Add( squares[x,y] );
                if (x == 2 || y == 2 || x == 6 || y == 6) sqs1[2].Add( squares[x,y] );
                if (x == 3 || y == 3 || x == 5 || y == 5) sqs1[3].Add( squares[x,y] );
                if (x == 4 || y == 4 || x == 4 || y == 4) sqs1[4].Add( squares[x,y] );

                sqs2[x].Add( squares[x,y] ); //Glow columns
                sqs3[y].Add( squares[x,y] ); //Glow rows

                if (x == 0 || y == 0 || x == 8 || y == 8)    sqs4[0].Add( squares[x,y] );
                if ((x == 1 || x == 7) && (y >= 1 && y <=7)) sqs4[1].Add( squares[x,y] );
                if ((y == 1 || y == 7) && (x >= 1 && x <=7)) sqs4[1].Add( squares[x,y] );
                if ((x == 2 || x == 6) && (y >= 2 && y <=6)) sqs4[2].Add( squares[x,y] );
                if ((y == 2 || y == 6) && (x >= 2 && x <=6)) sqs4[2].Add( squares[x,y] );
                if ((x == 3 || x == 5) && (y >= 3 && y <=5)) sqs4[3].Add( squares[x,y] );
                if ((y == 3 || y == 5) && (x >= 3 && x <=5)) sqs4[3].Add( squares[x,y] );
                if  (x == 4 && y == 4)                       sqs4[4].Add( squares[x,y] );

                if (x % 2 == 0 && y % 2 != 0) sqs6[0].Add( squares[x,y] );
                if (x % 2 != 0 && y % 2 == 0) sqs6[1].Add( squares[x,y] );
                
                if (x % 2 == 0) { if (y % 2 == 0) sqs7[0].Add(squares[x,y]); else sqs7[1].Add(squares[x,y]); }
                if (x % 2 != 0) { if (y % 2 == 0) sqs7[1].Add(squares[x,y]); else sqs7[0].Add(squares[x,y]); }
            }
        }
        patterns.Add(0, sqs1);
        patterns.Add(1, sqs2);
        patterns.Add(2, sqs3);
        patterns.Add(3, sqs4);
        patterns.Add(4, sqs5);
        patterns.Add(5, sqs6);
        patterns.Add(6, sqs7);
    }
}
