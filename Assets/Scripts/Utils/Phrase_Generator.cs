using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Phrase_Generator
{

    static List<string> txt_nouns = new List<string>();
    static List<string> txt_verbs = new List<string>();
    static List<string> txt_adjective = new List<string>();
    static Dictionary<int, List<KeyValuePair<string, int>>> adj_rules = new Dictionary<int, List<KeyValuePair<string, int>>>();

    public static void init() {
        //string path_n = "Assets/Resources/db_names_cut.txt";
        //string path_n = "Assets/Resources/db_names_cut_rod.txt";
        //string path_v = "Assets/Resources/db_glagols_cut.txt";
        //string path_a = "Assets/Resources/db_prilagatelnye.txt";
        //string path_a = "Assets/Resources/db_prilagatelnye_rule.txt";
        //string path_ar = "Assets/Resources/Spravochnik/adjective_rules.txt";
        //System.IO.StreamReader reader = null;

        //reader = new System.IO.StreamReader(path_n);
        //txt_nouns = reader.ReadToEnd().Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        //reader.Close();
        //reader = new System.IO.StreamReader(path_v);
        //txt_verbs = reader.ReadToEnd().Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        //reader.Close();
        //reader = new System.IO.StreamReader(path_a);
        //txt_adjective = reader.ReadToEnd().Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        //reader.Close();

        var textFile_n = Resources.Load<TextAsset>("db_names_cut_rod");
        txt_nouns = textFile_n.text.Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        var textFile_v = Resources.Load<TextAsset>("db_glagols_cut");
        txt_verbs = textFile_v.text.Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        var textFile_a = Resources.Load<TextAsset>("db_prilagatelnye_rule");
        txt_adjective = textFile_a.text.Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries).ToList();

        //Adj rules
        //reader = new System.IO.StreamReader(path_ar);
        //var arr = reader.ReadToEnd().Replace("ё", "е").Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
        var textFile_adj_rules = Resources.Load<TextAsset>("Spravochnik/adjective_rules");
        var arr = textFile_adj_rules.text.Replace("ё", "е").Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var row in arr) {
            //Debug.Log("parsing: " + row);
            var arr2 = row.Split(new string[]{" "}, System.StringSplitOptions.RemoveEmptyEntries);
            var rule_arr = arr2[1].Split(new string[]{","}, System.StringSplitOptions.None);
            
            int rule_index = int.Parse(arr2[0].Trim());
            List<KeyValuePair<string, int>> rule_list = new List<KeyValuePair<string, int>>();
            foreach (var rule in rule_arr) {
                if (rule == "") {
                    KeyValuePair<string, int> kv = new KeyValuePair<string, int>("", 0);
                    rule_list.Add(kv);
                } else {
                    string ending = rule.Substring(0, rule.Length-1);
                    int number = 0;
                    if (!int.TryParse(rule.Substring(rule.Length-1, 1), out number)) number = 0;
                    KeyValuePair<string, int> kv = new KeyValuePair<string, int>(ending, number);
                    rule_list.Add(kv);
                }
            }
            adj_rules.Add(rule_index, rule_list);
        }
        //reader.Close();
        //Debug.Log("adj rules count: " + adj_rules.Count() );

        #region ONE_TIME_DICTIONARY_CORRECTION
        //ONE TIME DICTIONARY CORRECTION
        //Nouns
        // string path_n_rod = "Assets/Resources/Spravochnik/nouns.txt";
        // reader = new System.IO.StreamReader(path_n_rod);
        // Dictionary<string, int> txt_nouns_rod = new Dictionary<string, int>();
        // var arr = reader.ReadToEnd().ToLower().Replace("ё", "е").Split(new string[]{"\n"}, System.StringSplitOptions.RemoveEmptyEntries);
        // foreach (var s in arr)
        // {
        //     var t = s.Split(new char[]{' '});
        //     if (!txt_nouns_rod.ContainsKey(t[0])) txt_nouns_rod.Add(t[0], int.Parse(t[1].Split(new char[]{','})[0]) );
        // }
        // reader.Close();
        // Debug.Log("txt_nouns_rod count: " + txt_nouns_rod.Count());
        // var w = System.IO.File.CreateText("D:/Unity 2018.1.0f2/Projects/Random Tycoon/Assets/Resources/db_names_cut_rod.txt");
        // foreach (var noun in txt_nouns) {
        //     if (txt_nouns_rod.ContainsKey(noun.ToLower())) {
        //         w.WriteLine(noun + " " + txt_nouns_rod[ noun.ToLower() ]);
        //     } else {
        //         w.WriteLine(noun + " 9");
        //     }
        // }
        // w.Close();
        //Adjectives
        // string path_p_rule = "Assets/Resources/Spravochnik/adjectives.txt";
        // reader = new System.IO.StreamReader(path_p_rule);
        // Dictionary<string, int> txt_adj_rule = new Dictionary<string, int>();
        // var arr = reader.ReadToEnd().ToLower().Replace("ё", "е").Split(new string[]{"\n"}, System.StringSplitOptions.RemoveEmptyEntries);
        // foreach (var s in arr) {
        //     var t = s.Split(new char[]{' '});
        //     if (!txt_adj_rule.ContainsKey(t[0])) txt_adj_rule.Add(t[0], int.Parse(t[1].Trim()) );
        // }
        // reader.Close();
        // Debug.Log("txt_adj_rule count: " + txt_adj_rule.Count());
        // var w = System.IO.File.CreateText("D:/Unity 2018.1.0f2/Projects/Random Tycoon/Assets/Resources/db_prilagatelnye_rule.txt");
        // foreach (var a in txt_adjective) {
        //     if (txt_adj_rule.ContainsKey(a.ToLower())) {
        //         w.WriteLine(a + " " + txt_adj_rule[ a.ToLower() ]);
        //     } else {
        //         w.WriteLine(a + " 999");
        //     }
        // }
        // w.Close();
        //END ONE TIME DICTIONARY CORRECTION
        #endregion
    }

    public static string Generate_Random_Phrase() {
        int r1 = Random.Range(0, txt_adjective.Count());
        int r2 = Random.Range(0, txt_nouns.Count());
        
        string n = txt_nouns[r2].Substring(0, txt_nouns[r2].Length - 2);
        string n_rod = txt_nouns[r2].Substring(txt_nouns[r2].Length - 1, 1);

        //var adj = System.Text.RegularExpressions.Regex.Split(txt_adjective[r1], @"\D+");
        var adj = txt_adjective[r1].Split(new char[]{' '});
        adj[0] = adj[0].Trim();

        //Debug.Log("split adj = " + txt_adjective[r1] + ", adj = " + adj[0] + ", num = " + adj[1] );

        if (n_rod != "1") {
            int rule_ind = int.Parse(adj[1]);
            int rule_sub_ind = -1;
            if (rule_ind > 100)     rule_sub_ind = -1;
            else if (n_rod == "2")  rule_sub_ind = 5;
            else if (n_rod == "3")  rule_sub_ind = 11;
            else if (n_rod == "0")  rule_sub_ind = 17;
            else                    rule_sub_ind = -1;

            if (rule_sub_ind >= 0) {
                var rule = adj_rules[rule_ind];
                string rule_ending = rule[rule_sub_ind].Key;
                int rule_letter_num = rule[rule_sub_ind].Value;
                //Debug.Log("adj = " + adj[0] + ", new_end = " + rule_ending + ", let_num = " + rule_letter_num);
                adj[0] = adj[0].Substring(0, adj[0].Length - rule_letter_num) + rule_ending;
            }
        }

        return adj[0] + " " + n;
    }
}
