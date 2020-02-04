using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.ExpressionParser_Logical;

public class Terminal_Universal : MonoBehaviour, Engine.ISetable
{
    public terminal_type_info terminal_type = terminal_type_info.expresiions;
    public enum terminal_type_info { expresiions, mail, potatoes };

    public zadanie_info[] zadaniya = new zadanie_info[]{};

    [System.Serializable]
    public class zadanie_info {
        public int iterations_count = 1;
        public Quaternion ABCD = Quaternion.identity;
        public bool A_Randomize = false;
        public bool B_Randomize = false;
        public bool C_Randomize = false;
        public bool D_Randomize = false;
        public Quaternion ABCD_Rand_Max = Quaternion.identity;
        public string condition_to_test = "A > B && B > C";
    }
    public class terminal_data {
        public int A; public int B; public int C; public int D;
        public string str = "";
    }
    public class responce {
        public bool resp_bool = false;
        public string[] resp_str_arr = null;
        public string ToString(terminal_type_info t) {
            if (t == terminal_type_info.expresiions) return resp_bool.ToString();
            else if (t == terminal_type_info.mail) return "xxx...";
            return "";
        }
    }

    public bool write_only = false;
    public string load_resource = "";
    public bool dont_autoset_ready_flag = false;

    public Tablo tablo = null;
    public tablo_anim_info tablo_anim = new tablo_anim_info();
    [System.Serializable]
    public class tablo_anim_info {
        public float delay_after_read = 0.1f; //Was 0.25f
        public float delay_after_responce = 0.25f; //Was 0.4f, Was 0.75f
        public float show_result_randomization_time = 1f; //Was 1.5f
        public float show_result_next_row_delay = 0.25f; //Was 0.25f
        public AudioClip Answer_Done = null;
        public AudioClip Answer_Correct = null;
        public AudioClip Answer_Wrong = null;
        public AudioClip Data_Stream = null;
    }

    public UnityEvent OnRead = null;
    public UnityEvent OnResponceCorrect = null;
    public UnityEvent OnResponceInCorrect = null;
    public UnityEvent OnTaskChange = null;
    public UnityEvent OnAllTasksFinished = null;

    public bool mail_convert_dollars = false;

    List<bool> task_responce_is_correct = new List<bool>();
    List<responce> task_responces_bot = new List<responce>();
    List<responce> task_responces_correct = new List<responce>();
    List<terminal_data> task_zadaniya_list = new List<terminal_data>();
    [HideInInspector] public responce current_bot_responce = new responce();
    [HideInInspector] public responce current_correct_responce = new responce();
    [HideInInspector] public bool terminal_is_ready = false;
    [HideInInspector] public string terminal_error = "";

    [HideInInspector] public int cur_zad = 0;
    [HideInInspector] public int current_iteration = 0;
    [HideInInspector] public string current_string_orig = "";

    string init_tablo_expression = @"       Task:1
  Condition:A<B<C
N   Answer   BOT   
1         
2
3
4
5
6
7
8
9
10";
    string init_tablo_mail = @"    Task: Mail
  Condition: MAil
N   Answer   BOT   
1         
2
3
4
5
6
7
8
9
10";

    Parser parser = new Parser();
    string[] string_resource = new string[]{};
    bool ready_for_read = true;
    bool ready_for_accept_responce = false;

    void Start() {
        if (!string.IsNullOrEmpty(load_resource)) {
            var txt = Resources.Load<TextAsset>(load_resource);
            string_resource = txt.text.Split(new string[]{"******************************"}, System.StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public terminal_data Read() {
        if (write_only) { terminal_error = "This terminal is write_only and does not accept reading data."; return null; }
        if (!ready_for_read) { terminal_error = "Terminal does not accept reading data before responding to previous problem."; return null; }
        terminal_is_ready = false;
        ready_for_read = false;
        ready_for_accept_responce = true;
        zadanie_info z = zadaniya[cur_zad];
        terminal_data td = new terminal_data();

        if (terminal_type == terminal_type_info.expresiions) {
            td.A = Mathf.RoundToInt(z.ABCD.x); td.B = Mathf.RoundToInt(z.ABCD.y); 
            td.C = Mathf.RoundToInt(z.ABCD.z); td.D = Mathf.RoundToInt(z.ABCD.w);
            if (z.A_Randomize) td.A = Random.Range(td.A, Mathf.RoundToInt(z.ABCD_Rand_Max.x) + 1);
            if (z.B_Randomize) td.B = Random.Range(td.B, Mathf.RoundToInt(z.ABCD_Rand_Max.y) + 1);
            if (z.C_Randomize) td.C = Random.Range(td.C, Mathf.RoundToInt(z.ABCD_Rand_Max.z) + 1);
            if (z.D_Randomize) td.D = Random.Range(td.D, Mathf.RoundToInt(z.ABCD_Rand_Max.w) + 1);
            
            LogicExpression exp = parser.Parse(zadaniya[cur_zad].condition_to_test);
            exp["A"].Set(td.A); exp["B"].Set(td.B); exp["C"].Set(td.C); exp["D"].Set(td.D);
            current_correct_responce = new responce(){ resp_bool = exp.GetResult() };
        }
        else if (terminal_type == terminal_type_info.mail) {
            if (z.A_Randomize) td.str = string_resource[ Random.Range(0, string_resource.Length) ];
            else td.str = string_resource[ current_iteration ];

            current_string_orig = td.str;
            current_correct_responce = new responce(){ resp_str_arr = parse_mail(td.str) };
        }

        if (tablo != null) { 
            if (current_iteration == 0) Tablo_NewTask();
            else StartCoroutine( Tablo_SetRowAfterTime(0f, "", tablo_anim.delay_after_read ) );
        }

        task_zadaniya_list.Add(td);

        if (OnRead != null) OnRead.Invoke();
        StartCoroutine(ReadWait());
        return td;
    }

    public void Responce<T>(T r) {
        if (!ready_for_accept_responce && !write_only) { terminal_error = "Terminal does not accept responce before 'read'."; return; }
        terminal_is_ready = false;
        ready_for_read = true;
        ready_for_accept_responce = false;
        bool ansewer_is_correct = false;
        if (terminal_type == terminal_type_info.expresiions && typeof(T) == typeof(bool))
        { 
            current_bot_responce = new responce(){ resp_bool = (bool)(object)r };
            ansewer_is_correct = current_bot_responce.resp_bool == current_correct_responce.resp_bool; 
        }
        else if (terminal_type == terminal_type_info.mail && typeof(T).IsArray && typeof(string).IsAssignableFrom(typeof(T).GetElementType()) )
        {
            var resp = (string[])(object)r;
            //Debug.Log("Received responce is an array, length: " + resp.Length);
            current_bot_responce = new responce(){ resp_str_arr = resp };
            if (resp.Length == 2) {
                var resp_correct = current_correct_responce.resp_str_arr;
                ansewer_is_correct = resp[0] == resp_correct[0] && resp[1] == resp_correct[1];
            }
        }
        else if (terminal_type == terminal_type_info.potatoes) 
        {
            var o = (object)r;
            if (o == null) { terminal_error = "Terminal error: null reference exception."; return; }
            else {
                var arr = o as int[][];
                if (arr == null) { terminal_error = "Terminal error: wrong data received. Correct type is int[][]."; return; }
                else { 
                    if (check_potatoes(arr)) { Engine.SetRequirement("TERMINAL"); } else { Hud.Log("Terminal did not accept the responce."); }
                }
            }
        }

        task_responces_bot.Add(current_bot_responce);
        task_responces_correct.Add(current_correct_responce);
        task_responce_is_correct.Add(ansewer_is_correct);
        if (tablo != null) {
            //bool last = (cur_zad == 9 && current_iteration == 9) ? true : false;
            //StartCoroutine( SetTabloRowAfterTime(0f, "DONE", delay_after_responce, true, last ) );
            bool last_resp = (current_iteration == zadaniya[cur_zad].iterations_count - 1) ? true : false;
            bool last_task = (cur_zad == zadaniya.Length - 1) ? true : false;
            StartCoroutine( Tablo_SetRowAfterTime(0f, "DONE", tablo_anim.delay_after_responce, true, last_resp, last_task ) );
        }

        if (ansewer_is_correct) {
            if (OnResponceCorrect != null) OnResponceCorrect.Invoke();
        } else {
            if (OnResponceInCorrect != null) OnResponceInCorrect.Invoke();
        }

        current_iteration++;
        if (current_iteration >= zadaniya[cur_zad].iterations_count) {
            cur_zad++;
            current_iteration = 0;
            if (cur_zad >= zadaniya.Length) {
                cur_zad = 0;
                if (OnAllTasksFinished != null) OnAllTasksFinished.Invoke();
            } else {
                if (OnTaskChange != null) OnTaskChange.Invoke();
            }
        }

        StartCoroutine(ResponceWait());
    }

    IEnumerator ReadWait() {
        //Debug.Log("Reading terminal...");
        yield return new WaitForSeconds(1f);
        //Debug.Log("Reading terminal done.");
        if (!dont_autoset_ready_flag) terminal_is_ready = true;
    }
    IEnumerator ResponceWait() {
        //Debug.Log("Responding to terminal...");
        yield return new WaitForSeconds(1f);
        //Debug.Log("Responding to terminal done. Correct ansewer was " + current_responce);
        if (!dont_autoset_ready_flag) terminal_is_ready = true;
    }

    //On script play
    public void Set() {
        cur_zad = 0;
        current_iteration = 0;
        terminal_is_ready = true;
        ready_for_read = true;
        ready_for_accept_responce = false;
        terminal_error = "";

        StopAllCoroutines();
        if (tablo != null) {
            tablo.AbortAnimations(); tablo.SetText("");
        }
    }
    //On script stop
    public void Reset() {
        StopAllCoroutines(); 
        if (tablo != null) { tablo.AbortAnimations(); }
        Engine.engine_inst.Play_Sound_SFX(null, 1, false, false);
    }

    string[] parse_mail(string mail) {
        string title = mail.Length > 140 ? mail.Substring(0, 140) + "..." : mail;
        //mail = mail.Replace("высокорогов", "Высокорогов").Replace("дмитрий", "Дмитрий").Replace("бахилович", "Бахилович");
        mail = mail.Replace("лежебок", "Лежебок").Replace("лодыревн", "Лодыревн");
        
        string mail_l = mail.ToLower();
        if (mail.Contains("вы выиграли") || mail.Contains("одобрен кредит")) title = "(СПАМ) " + title;

        mail = mail.Replace("а", "#$***$#").Replace("о", "а").Replace("#$***$#", "о");

        //Changing $ to pingv
        if (mail_convert_dollars) {
            int ind = mail.IndexOf("$");
            while ( ind >= 0 ) {
                string n = "";
                for (int i = ind + 1; i < mail.Length - 1; i++) {
                    if (char.IsDigit(mail[i])) n = n + mail[i]; else break;
                }
                if (n != "") {
                    int dollars = int.Parse(n);
                    string pingvins = (dollars * 17).ToString() + "пнгв.";
                    mail = mail.Remove(ind, n.Length + 1);
                    mail = mail.Insert(ind, pingvins);
                }

                ind = mail.IndexOf("$", ind + 1);
            }
        }

        return new string[]{title, mail};
    }
    bool check_potatoes(int[][] r) {
        var potatoes = transform.parent.GetComponentsInChildren<Floor_RandomPotatoes>();
        if (r.Length != potatoes.Length) { 
            Hud.Log("Terminal: first dimension of your array has incorrect length of " + r.Length.ToString() + ", expected length " + potatoes.Length.ToString());
            return false; 
        }

        //var correct_responce = new int[potatoes.Length][];
        for (int i = 0; i < potatoes.Length; i ++) {
            var correct = potatoes[i].potatoes;

            if (r[i].Length != correct.GetLength(0)) {
                Hud.Log("Terminal: element " + i.ToString() + " of your array has incorrect length of " + r[i].Length.ToString() + ", expected length " + correct.GetLength(0).ToString());
                return false; 
            }

            int incorrect = -1;
            for (int c = 0; c < r[i].Length; c++) {
                if (r[i][c] != correct[c,0]) { incorrect = c; break; }
            }
            if (incorrect >= 0) {
                Hud.Log("Terminal: element [" + i.ToString() + "][" + incorrect.ToString() + "] has incorrect data " + r[i][incorrect].ToString() + ", expected " + correct[incorrect,0].ToString());
                return false;
            }
        }

        Hud.Log("Terminal: Responce correct - accepted.");
        return true;
    }

    void Tablo_NewTask() {
        task_responces_bot = new List<responce>();
        task_responces_correct = new List<responce>();
        task_responce_is_correct = new List<bool>();
        task_zadaniya_list = new List<terminal_data>();
        if (terminal_type == terminal_type_info.expresiions)  tablo.SetText(init_tablo_expression);
        else if (terminal_type == terminal_type_info.mail)    tablo.SetText(init_tablo_mail);

        string r = "       Task:" + (cur_zad + 1).ToString();
        tablo.SetTextRow(r, 0); tablo.AnimateRow(0);

        //Show first read info
        tablo.OnAnimationEnd.AddListener(()=> {
            tablo.OnAnimationEnd.RemoveAllListeners();
            StartCoroutine( Tablo_SetRowAfterTime(0f, "", tablo_anim.delay_after_read ) );
        });
    }
    IEnumerator Tablo_SetRowAfterTime(float wait, string BOT_Responce, float terminal_ready_delay, bool done_sound = false, bool last_read = false, bool last_task = false) {
        if (wait > 0) yield return new WaitForSeconds(wait);

        int N = current_iteration + 1;
        string r = N.ToString();
        if (N < 10) r += "   "; else r += "  ";

        r += current_correct_responce.ToString(terminal_type);

        r += new string(' ', 13 - r.Length) + BOT_Responce;
        tablo.SetTextRow(r, 3 + current_iteration);
        if (done_sound) Engine.engine_inst.Play_Sound_SFX(tablo_anim.Answer_Done);

        yield return new WaitForSeconds(terminal_ready_delay);
        if (!last_read) {
            terminal_is_ready = true;
        } else {
            StartCoroutine( Tablo_DisplayResults(last_task) );
        }
    }
    IEnumerator Tablo_DisplayResults(bool last_task) {
        Engine.engine_inst.Play_Sound_SFX(tablo_anim.Data_Stream, 1, true);
        for (int i = 0; i < 10; i++) {
            string r = Tablo_Create_Result_Row(i + 1, task_responces_correct[i], task_responces_bot[i], null);

            tablo.SetTextRow(r, i+3);
            tablo.AnimateRowFromRandom(i+3, 0f, 1, tablo_anim.show_result_randomization_time, 13);

            StartCoroutine ( Tablo_ProcessResultAfterTime(i, task_responces_correct[i], task_responces_bot[i]) );

            //Wait before next row
            yield return new WaitForSeconds(tablo_anim.show_result_next_row_delay);
        }
        //Wait for last row to finish random animation
        yield return new WaitForSeconds(tablo_anim.show_result_randomization_time - tablo_anim.show_result_next_row_delay);
        Engine.engine_inst.Play_Sound_SFX(tablo_anim.Data_Stream, 1, true, true);
        //Wait another 1 sec, before processing to the next task, for user to read the tablo
        yield return new WaitForSeconds(1f);

        if (last_task) {
            //Last task
            bool no_error = true;
            for (int i = 0; i < 10; i++) { if (!task_responce_is_correct[i]) {no_error = false; break;} }
            if (no_error) Engine.SetRequirement("TERMINAL");
        } 
        terminal_is_ready = true;
    }
    IEnumerator Tablo_ProcessResultAfterTime(int ind, responce resp_terminal, responce resp_bot) {
        int tablo_row_ind = ind + 3;
        while (tablo.IsRowAnimating(tablo_row_ind)) yield return null;
        string r = Tablo_Create_Result_Row(ind + 1, resp_terminal, resp_bot, task_responce_is_correct[ind]);
        tablo.SetTextRow(r, tablo_row_ind);

        if (terminal_type == terminal_type_info.expresiions) {
            string s1 = task_zadaniya_list[ind].A.ToString() + ",";
            if (s1.Length < 5) s1 = new string(' ', 5 - s1.Length) + s1;
            string s2 = task_zadaniya_list[ind].B.ToString() + ",";
            if (s2.Length < 5) s2 = new string(' ', 5 - s2.Length) + s2;
            string s3 = task_zadaniya_list[ind].C.ToString() + ",";
            if (s3.Length < 5) s3 = new string(' ', 5 - s3.Length) + s3;
            string s4 = task_zadaniya_list[ind].D.ToString();
            if (s4.Length < 20) s4 = new string(' ', 5 - s4.Length) + s4;
            tablo.SetTextRow(s1 + s2 + s3 + s4, 1);
        }
        
        if (task_responce_is_correct[ind]) {
            Engine.engine_inst.Play_Sound_SFX ( tablo_anim.Answer_Correct );
        } else {
            Engine.engine_inst.Play_Sound_SFX ( null, 1, false, true);
            Engine.engine_inst.Play_Sound_SFX ( tablo_anim.Answer_Wrong );
            StopAllCoroutines(); tablo.AbortAnimations(Tablo.clear_mode.clear_below_first_animated_row);
            Engine.engine_inst.GetComponent<Compiler>().Stop();
        }
    }

    string Tablo_Create_Result_Row(int N, responce resp_terminal, responce resp_bot, bool? colorize) {
        string r = N.ToString();
        if (N < 10) r += "   "; else r += "  ";
        r += resp_terminal.ToString(terminal_type);
        r += new string(' ', 13 - r.Length);

        var b = resp_bot.ToString(terminal_type);
        if (colorize == null) { r += b; }
        else if (colorize == true) { r+= "<color=\"green\">" + b + "</color>"; }
        else if (colorize == false) { r+= "<color=\"red\">" + b + "</color>"; }

        return r;
    }
}
