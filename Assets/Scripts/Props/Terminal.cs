using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using B83.ExpressionParser_Logical;

public class Terminal : MonoBehaviour, Engine.ISetable
{
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

    public bool dont_autoset_ready_flag = false;

    public UnityEvent OnRead = null;
    public UnityEvent OnResponceCorrect = null;
    public UnityEvent OnResponceInCorrect = null;
    public UnityEvent OnTaskChange = null;
    public UnityEvent OnAllTasksFinished = null;

    [HideInInspector] public bool current_responce = false;
    [HideInInspector] public bool terminal_is_ready = false;

    [HideInInspector] public int cur_zad = 0;
    [HideInInspector] public int current_iteration = 0;
    //int A = 0; int B = 0; int C = 0; int D = 0;

    Parser parser = new Parser();

    public terminal_data Read() {
        terminal_is_ready = false;
        zadanie_info z = zadaniya[cur_zad];
        terminal_data td = new terminal_data();

        td.A = Mathf.RoundToInt(z.ABCD.x); td.B = Mathf.RoundToInt(z.ABCD.y); 
        td.C = Mathf.RoundToInt(z.ABCD.z); td.D = Mathf.RoundToInt(z.ABCD.w);
        if (z.A_Randomize) td.A = Random.Range(td.A, Mathf.RoundToInt(z.ABCD_Rand_Max.x) + 1);
        if (z.B_Randomize) td.B = Random.Range(td.B, Mathf.RoundToInt(z.ABCD_Rand_Max.y) + 1);
        if (z.C_Randomize) td.C = Random.Range(td.C, Mathf.RoundToInt(z.ABCD_Rand_Max.z) + 1);
        if (z.D_Randomize) td.D = Random.Range(td.D, Mathf.RoundToInt(z.ABCD_Rand_Max.w) + 1);
        
        LogicExpression exp = parser.Parse(zadaniya[cur_zad].condition_to_test);
        exp["A"].Set(td.A); exp["B"].Set(td.B); exp["C"].Set(td.C); exp["D"].Set(td.D);
        current_responce = exp.GetResult();

        if (OnRead != null) OnRead.Invoke();
        StartCoroutine(ReadWait());
        return td;
    }

    public void Responce(bool r) {
        terminal_is_ready = false;
        bool ansewer_is_correct = r == current_responce;

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

    public void Set() {
        cur_zad = 0;
        current_iteration = 0;
    }
    
    //public int GetIntA() { return td.A; }
    //public int GetIntB() { return td.B; }
    //public int GetIntC() { return td.C; }
    //public int GetIntD() { return td.D; }
}
