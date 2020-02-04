using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Terminal_Mail : MonoBehaviour, Engine.ISetable
{
    public bool randomize_content = true;
    public int iterations = 10;

    public UnityEvent OnRead = null;
    public UnityEvent OnResponceCorrect = null;
    public UnityEvent OnResponceInCorrect = null;
    public UnityEvent OnTaskChange = null;
    public UnityEvent OnAllTasksFinished = null;

    public bool convert_dollars = false;
    public bool dont_autoset_ready_flag = false;

    [HideInInspector] public string[] current_responce = new string[2]{"",""};
    [HideInInspector] public string[] current_bot_responce = new string[2]{"",""};
    [HideInInspector] public bool terminal_is_ready = false;

    //[HideInInspector] public int cur_zad = 0;
    [HideInInspector] public int current_iteration = 0;
    [HideInInspector] public string current_mail = "";

    string[] mails = new string[]{};

    void Start() {
        var txt = Resources.Load<TextAsset>("Spam");
        mails = txt.text.Split(new string[]{"******************************"}, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public Terminal.terminal_data Read() {
        terminal_is_ready = false;
        Terminal.terminal_data td = new Terminal.terminal_data();

        if (randomize_content) td.str = mails[ Random.Range(0, mails.Length) ];
        else td.str = mails[ current_iteration ];

        current_mail = td.str;
        current_responce = parse_mail(td.str);

        if (OnRead != null) OnRead.Invoke();
        StartCoroutine(ReadWait());
        return td;
    }
    public void Responce(string[] r) {
        terminal_is_ready = false;
        bool ansewer_is_correct = (r.Length == 2 && r[0] == current_responce[0] && r[1] == current_responce[1]);
        //Debug.Log(current_bot_responce.Length + ", " + current_bot_responce + ", " + r.Length + ", " + r);
        if (r.Length == 2) {
            current_bot_responce = new string[2]{ r[0], r[1] };
        } else {
            current_bot_responce = new string[2]{"",""};
        }

        if (ansewer_is_correct) {
            if (OnResponceCorrect != null) OnResponceCorrect.Invoke();
        } else {
            if (OnResponceInCorrect != null) OnResponceInCorrect.Invoke();
        }

        current_iteration++;
        if (current_iteration >= iterations) {
            current_iteration = 0;
            if (OnAllTasksFinished != null) OnAllTasksFinished.Invoke();
        }

        StartCoroutine(ResponceWait());
    }

    IEnumerator ReadWait() {
        yield return new WaitForSeconds(1f);
        //Debug.Log("Reading terminal done.");
        if (!dont_autoset_ready_flag) terminal_is_ready = true;
    }
    IEnumerator ResponceWait() {
        yield return new WaitForSeconds(1f);
        //Debug.Log("Responding to terminal done. Correct ansewer was " + current_responce);
        if (!dont_autoset_ready_flag) terminal_is_ready = true;
    }

    public void Set() {
        current_iteration = 0;
    }

    string[] parse_mail(string mail) {
        string title = mail.Length > 140 ? mail.Substring(0, 140) + "..." : mail;
        mail = mail.Replace("высокорогов", "Высокорогов").Replace("дмитрий", "Дмитрий").Replace("бахилович", "Бахилович");
        
        string mail_l = mail.ToLower();
        if (mail.Contains("вы выиграли") || mail.Contains("одобрен кредит")) title = "(СПАМ) " + title;

        mail = mail.Replace("а", "#$***$#").Replace("о", "а").Replace("#$***$#", "о");

        //Changing $ to pingv
        if (convert_dollars) {
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


}
