using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour, Engine.ISetable, Engine.IResetable
{
    public Enemy_Types Enemy_Type;
    public enum Enemy_Types {
        Bomb,
        Slime,
        Troll
    }
    public int HP = -1;
    public Vector2Int HP_Range = new Vector2Int(-1, -1);
    public bool reset_position_on_script_stop = true;
    public bool deactivate_self_on_hard_reset = false;
    public bool suspend_while_kill_animation = false;

    public float AI_Detect_dist = 4;
    public float AI_Move_Speed = 0.01f;
    public float AI_Move_while_rotate_threshold = 10f;
    public Attack_Info[] AI_Attacks = null;

    [System.Serializable]
    public struct Attack_Info {
        public int   Probability;
        public float Attack_dist;
        public float Attack_cooldown;
        public float Attack_global_cooldown;
        public float Attack_rot_threshold;
    }
    public float AI_Attack1_dist = 1f;            //To delete
    public float AI_Attack1_rot_threshold = 2f;   //to delete


    bool killed = false;
    public bool suspend = false;

    bool init_done = false;
    int initial_HP;
    float[] cooldowns = null;
    float   cooldown_global = 0f;
    Vector3 initial_position;
    Quaternion initial_rotation;
    List<Material> mat_to_flash_on_hit = new List<Material>();

    //per enemy
    bool slime_is_teleporting = false;

    // Start is called before the first frame update
    void Start()
    {
        if (HP_Range.x >= 0f && HP_Range.y >= HP_Range.x) HP = Random.Range(HP_Range.x, HP_Range.y + 1);
        initial_HP = HP;
        if (AI_Attacks != null && AI_Attacks.Length > 0) cooldowns = new float[AI_Attacks.Length];

        Transform sub_obj = transform.GetChild(0);
        initial_position = sub_obj.position;
        initial_rotation = sub_obj.rotation;

        if (Enemy_Type == Enemy_Types.Bomb) {
            mat_to_flash_on_hit.Add( sub_obj.GetComponent<MeshRenderer>().materials[1] );
        }
        foreach (var mat in mat_to_flash_on_hit)
            mat.EnableKeyword("_EMISSION");

        init_done = true;
        //Debug.Log("Enemy set.");
    }

    // Update is called once per frame
    void Update()
    {
        if (BOT.is_paused || BOT.pause) {
            if (slime_is_teleporting) { transform.GetChild(0).GetComponent<Animator>().speed = 0f; }
            return;
        }

        //Manage cooldowns
        if (cooldowns != null) {
            for (int i = 0; i < cooldowns.Length; i++) {
                if (cooldowns[i] > 0) cooldowns[i] -= Time.deltaTime;
            }
        }
        if (cooldown_global > 0) cooldown_global -= Time.deltaTime; //to delete

        //If bot script is running and enemy is not killed - Call AI
        if (BOT.script_thread != null && BOT.script_thread.IsAlive && !killed) AI();

        //per enemy handle
        if (slime_is_teleporting) {
            Transform slime = transform.GetChild(0);
            Animator a = slime.GetComponent<Animator>();
            if (a.speed == 0f) a.speed = 1f;

            var asi = a.GetCurrentAnimatorStateInfo(0);
            if (asi.IsName("Anim_Teleport_Out")) {
                slime_is_teleporting = false;
                var sl_pos = new Vector2(slime.position.x, slime.position.z);
                var sl_pos_y = slime.position.y;
                var bt_pos = new Vector2(BOT.bot_obj.transform.position.x, BOT.bot_obj.transform.position.z);
                float distance_to_bot = Vector2.Distance(sl_pos, bt_pos);
                float random_rad = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float x = (Mathf.Sin(random_rad) * distance_to_bot) + bt_pos.x;
                float y = (Mathf.Cos(random_rad) * distance_to_bot) + bt_pos.y;
                slime.position = new Vector3(x, sl_pos_y, y);
            }
        }
    }

    //On script play
    public void Set() {
        if (Enemy_Type == Enemy_Types.Bomb) {
            //Bomb should have rigidbody kinematic true, to allow fall from top in 'structures' level,
            //But it should not be moved by bot. Set kinematic to true on script play, and false when script stops
            transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    //On script stop
    public void Reset() {
        if (Enemy_Type == Enemy_Types.Bomb) {
            //Bomb should have rigidbody kinematic true, to allow fall from top in 'structures' level,
            //But it should not be moved by bot. Set kinematic to true on script play, and false when script stops
            transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
        }
    }


    public void TakeDamage(int damage)
    {
        if (killed) return;
        if (damage == 0) return;

        //Debug.Log("Damage taken: " + damage.ToString() + ", HP = " + HP.ToString());

        //Flash on hit
        foreach (var mat in mat_to_flash_on_hit)
            mat.DOColor(new Color(0.8f, 0f, 0f, 1f), "_EmissionColor", 0.25f).SetLoops(2, LoopType.Yoyo);

        HP -= damage;
        Hud.ShowDamage(transform.GetChild(0).position, damage, false, Enemy_Type.ToString());

        Test_if_Killed();
    }

    bool Test_if_Killed()
    {
        if (HP < 0) HP = 0;
        if (HP > 0) return false;

        killed = true;
        if (suspend_while_kill_animation) suspend = true;

        if (Enemy_Type == Enemy_Types.Bomb) {
            StartCoroutine("ExplodeBomb");
        } else if (Enemy_Type == Enemy_Types.Slime) {
            Animator a = transform.GetChild(0).GetComponent<Animator>();
            a.ResetTrigger("Idle");
            a.SetTrigger("Die");
            StartCoroutine(Reset_Suspend_After_Time(0.95f));
        }
        return true;
    }

    public void Inflict_Damage(int d)
    {
        if (BOT.script_thread == null) return;

        BOT.HP -= d;
        if (BOT.HP < 0) BOT.HP = 0;
        Hud.ShowDamage(BOT.bot_obj.transform.position, d, true, Enemy_Type.ToString());
    }

    IEnumerator Reset_Suspend_After_Time(float t)
    {
        yield return new WaitForSeconds(t);
        suspend = false;
    }

    IEnumerator ExplodeBomb()
    {
        float damage_distance = 3f;
        float explosion_delay = 4f;

        //Show Timer
        float e = explosion_delay;
        var tmp_tr = transform.GetChild(0).GetChild(3);
        tmp_tr.localPosition = new Vector3(0f, 0f, 0.37f);
        var tmp = tmp_tr.GetComponent<TMPro.TextMeshPro>();
        tmp.text = explosion_delay.ToString("N2"); 
        tmp_tr.gameObject.SetActive(true);
        DOTween.To(()=>e, x=>e = x, 0f, explosion_delay).OnUpdate(()=> tmp.text = e.ToString("N2"));
        tmp_tr.DOLocalMoveZ(0.6f, explosion_delay);

        //Burn
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        var ps = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        yield return new WaitForSeconds_BOTPauseAware(explosion_delay, new System.Action(()=> ps.Pause(true)), new System.Action(()=> ps.Play(true)));
        if (BOT.script_thread == null || !BOT.script_thread.IsAlive) yield break;

        //Explode
        tmp_tr.gameObject.SetActive(false);
        tmp_tr.localPosition = new Vector3(0f, 0f, 0.37f);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        //Setup damage plane
        float damage_plane_scale = ((damage_distance * 2f) + 1f) / 10 / transform.GetChild(0).localScale.y;
        Transform damage_plane = transform.GetChild(0).GetChild(2);
        damage_plane.DOLocalRotate( new Vector3 (0f, 360f, 0f), 0.4f, RotateMode.LocalAxisAdd).OnComplete(()=> {
            damage_plane.localRotation = Quaternion.Euler(90f, 0f, 0f);
        });
        damage_plane.DOScale(new Vector3(damage_plane_scale, damage_plane_scale, damage_plane_scale), 0.3f).OnComplete(()=>{
            damage_plane.DOScale(new Vector3(0f, 0f, 0f), 0.1f);
        });

        var ps2 = transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();
        var a_pause = new System.Action(()=> { ps2.Pause(); DOTween.Pause(damage_plane); } );
        var a_unpause = new System.Action(()=> { ps2.Play(); DOTween.Play(damage_plane); } );
        yield return new WaitForSeconds_BOTPauseAware(0.4f, a_pause, a_unpause);
        if (Vector3.Distance (transform.GetChild(0).position, BOT.bot_obj.transform.position) < damage_distance) Inflict_Damage(10000);

        if (BOT.script_thread == null || !BOT.script_thread.IsAlive) yield break;

        //Turn off in the middle of exploding
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        suspend = false;
    }

    public void Reset(bool hard_reset) {
        //Debug.Log("Enemy Reset");

        killed = false;
        suspend = false;
        if (cooldowns != null) {
            for (int i = 0; i < cooldowns.Length; i++) {
                cooldowns[i] = 0f;
            }
        }

        //Hard reset is called when level loads or reloads. Soft reset - after script stop
        if (reset_position_on_script_stop || hard_reset) ResetPosition();
        if (hard_reset) Reset();

        if (init_done) { 
            HP = initial_HP; 
            if (HP_Range.x >= 0f && HP_Range.y >= HP_Range.x) HP = Random.Range(HP_Range.x, HP_Range.y + 1); 
        }

        if (Enemy_Type == Enemy_Types.Bomb) {
            transform.GetChild(0).GetChild(2).DOKill();                                 //Damage plane
            transform.GetChild(0).GetChild(2).localScale = new Vector3(0f, 0f, 0f);     //Damage plane
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);              //Particles fitil
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);              //Particles explosion
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

            var tmp_tr = transform.GetChild(0).GetChild(3);
            tmp_tr.gameObject.SetActive(false); tmp_tr.localPosition = new Vector3(0f, 0f, 0.37f);
        }

        if (hard_reset && deactivate_self_on_hard_reset) gameObject.SetActive(false);
    }

    public void ResetPosition() {
        if (init_done) {
            Transform sub_obj = transform.GetChild(0);
            sub_obj.position = initial_position;
            sub_obj.rotation = initial_rotation;
        }
    }

    public void AI() {
        if (!init_done) return;

        if (Enemy_Type == Enemy_Types.Slime) {
            Vector3 target_pos = Vector3.zero;
            Vector3 direction = Vector3.zero;
            Transform slime = transform.GetChild(0);
            Vector3 sl_pos_y0 = new Vector3(slime.position.x, 0f, slime.position.z);
            Vector3 bot_pos_y0 = new Vector3(BOT.bot_obj.transform.position.x, 0f, BOT.bot_obj.transform.position.z);

            float distance_to_bot = Vector3.Distance (sl_pos_y0, bot_pos_y0);
            //Debug.Log("Slime: distance to bot = " + distance_to_bot.ToString());
            if (distance_to_bot > AI_Detect_dist) {
                //return to base
                Vector3 in_pos_y0 = new Vector3(initial_position.x, 0f, initial_position.z);
                float dist_to_base = Vector3.Distance(sl_pos_y0, in_pos_y0);
                if (dist_to_base > 0.001) {
                    //Debug.Log("Slime: return to base. Cur pos = " + sl_pos_y0.ToString() + ", initial pos = " + in_pos_y0.ToString() + ", distance to base = " + dist_to_base.ToString());
                    target_pos = in_pos_y0;
                } else {
                    slime.GetComponent<Animator>().SetTrigger("Idle");
                    if (Mathf.DeltaAngle(slime.rotation.eulerAngles.y, initial_rotation.eulerAngles.y) > 1f) {
                        //Debug.Log("Slime: return to base - already on base, rotating to origin");
                        slime.rotation = Quaternion.Slerp(slime.rotation, initial_rotation, Time.deltaTime * 2f);
                    } else {
                        //Debug.Log("Slime: return to base - already on base, do nothing");
                    }
                }
            } else {
                //bot detected
                if (distance_to_bot <= AI_Attack1_dist) {
                    //can attack
                    float angle_delta = Rotate_To_Target(slime, bot_pos_y0, 5f);
                    if (angle_delta > AI_Attack1_rot_threshold) { slime.GetComponent<Animator>().SetTrigger("Move"); return; }

                    //attack
                    //Debug.Log("Slime: can attack, Attack");
                    int attack = Get_Random_Possible_Attack_Index();
                    if (attack == -1) return;

                    cooldowns[attack] = AI_Attacks[attack].Attack_cooldown;
                    cooldown_global = AI_Attacks[attack].Attack_global_cooldown;
                    if (attack == 0) {
                        Inflict_Damage( Random.Range(3, 10) );
                        slime.GetComponent<Animator>().SetTrigger("Attack");                        
                    }
                    if (attack == 1) {
                        slime_is_teleporting = true;
                        slime.GetComponent<Animator>().SetTrigger("Teleport");
                    }
                } else {
                    //Move to bot
                    //Debug.Log("Slime: Move to bot");
                    target_pos = bot_pos_y0;
                }
            }

            if (target_pos != Vector3.zero) {
                //Move mode
                float distance_to_target = Vector3.Distance(sl_pos_y0, new Vector3(target_pos.x, 0f, target_pos.z));

                float angle_delta = Rotate_To_Target(slime, target_pos, 5f);
                if (angle_delta > AI_Move_while_rotate_threshold) return;

                if (distance_to_target - 0.01f > AI_Move_Speed) {
                    //Debug.Log("Slime: Move mode - Moving");
                    slime.position += slime.forward * AI_Move_Speed;
                    slime.GetComponent<Animator>().SetTrigger("Move");
                } else {
                    //Debug.Log("Slime: Move mode - Near distance, set direct position.");
                    slime.position = new Vector3 (target_pos.x, slime.position.y, target_pos.z);
                }
            }
        }
    }

    float Rotate_To_Target(Transform transform_to_rotate, Vector3 target, float rot_speed) {
        Vector2 cur_pos = new Vector2 (transform_to_rotate.position.x, transform_to_rotate.position.z);
        float distance_to_target = Vector2.Distance(cur_pos, new Vector2(target.x, target.z));
        if (distance_to_target <= 0.01) return 0f; //Can't calculate rotation if too neear

        Vector3 direction = (target - new Vector3(cur_pos.x, 0f, cur_pos.y) ).normalized;
        Quaternion target_rot = Quaternion.LookRotation( new Vector3(direction.x, 0f, direction.z));
        float cur_rot_y = transform_to_rotate.eulerAngles.y;
        float angle_delta = Mathf.Abs(Mathf.DeltaAngle(cur_rot_y, target_rot.eulerAngles.y));
        if (angle_delta > 0.1f) {
            //Need rotate
            //Debug.Log("Slime: rotate. Angle delta = " + angle_delta.ToString());
            transform_to_rotate.rotation = Quaternion.Slerp(transform_to_rotate.rotation, target_rot, Time.deltaTime * rot_speed);
        } else {
            //Debug.Log("Slime: rotate. Angle delta too low. Set rotation directly.");
            transform_to_rotate.rotation = target_rot;
        }
        return angle_delta;
    }

    int Get_Random_Possible_Attack_Index() {
        if (AI_Attacks == null || AI_Attacks.Length == 0) return -1;
        if (cooldown_global > 0) return -1;

        List<int> probability_indices = new List<int>();
        List<Attack_Info> possible_attacks = new List<Attack_Info>();
        for (int i = 0; i < AI_Attacks.Length; i++) {
            //Debug.Log("Attack " + i.ToString() + ": cooldown = " + cooldowns[i].ToString());
            if (cooldowns[i] <= 0) { 
                possible_attacks.Add(AI_Attacks[i]);
                probability_indices.AddRange ( Enumerable.Repeat(i, AI_Attacks[i].Probability) );
            }
        }
        if (possible_attacks.Count == 0) return -1;

        int a = probability_indices [ Random.Range(0, probability_indices.Count) ];
        //Debug.Log("Selected attack = " + a.ToString());
        return a;
    }
}
