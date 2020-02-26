using System.Collections;
using System.Collections.Generic;
//using System.Collections.Concurrent;
using Microsoft.CSharp;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.Reflection;
using DG.Tweening;

public class Compiler : MonoBehaviour {
	//InputField ScriptInputField = null;
	TMPro.TMP_InputField ScriptInputField = null;
	Text_Editor ScriptEditor_My = null;
	//TMPro.TextMeshProUGUI ScriptTextField = null;


	public BOT_Param_struct BOT_Param = new BOT_Param_struct();
	[Serializable]
	public struct BOT_Param_struct {
		public GameObject Shield_Prefab;
	}

	classes_dict classes_dictionary = new classes_dict();
	class classes_dict
	{
		public Dictionary<string, classes_dict> dict = new Dictionary<string, classes_dict>();
	}

	//BOT bot_inst;
	Action deleg = null;
	IAsyncResult res = null;

	public static bool dont_show_rebase_anim_next_time = false;

	Suggestion suggestion = null;

	public static classes_info cur_active_class = null;
	public static List<classes_info> classes = new List<classes_info>();
	public static List<classes_info> functions = new List<classes_info>();

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<BOT>();
		gameObject.AddComponent<BOT_Helpers.Platform>();
		BOT.bot_shield = BOT_Param.Shield_Prefab;

		GameObject tmp = GameObject.Find("Script_InputField_TMPRO");
		if (tmp != null) {
			ScriptInputField = tmp.GetComponent<TMPro.TMP_InputField>();
			ScriptInputField.onValueChanged.AddListener(Suggestion);
		} else {
			ScriptEditor_My = GameObject.Find("Text_Editor").GetComponent<Text_Editor>();
			BOT.script_editor = ScriptEditor_My;
		}


		cur_active_class = new classes_info("");
		cur_active_class.id = 0; 
		cur_active_class.changed = false;
		cur_active_class.active = true; cur_active_class.enabled = true; classes.Add(cur_active_class);

		GameObject suggestion_obj = GameObject.Find("Suggestion");
		if (suggestion_obj != null) suggestion = suggestion_obj.GetComponent<Suggestion>();

		bool WRITE_ASSEMBLIES_LIST_TO_FILE = false;
		bool READ_ASSEMBLIES_LIST_FROM_FILE = false;
		bool READ_ASSEMBLIES_LIST_FROM_REFLECTION = false;
		string assemblies_txt_file = "D:\\Unity 2018.1.0f2\\Projects\\Script-o-bot\\assemblies list.txt";

		//test get all classes
		if (!READ_ASSEMBLIES_LIST_FROM_FILE && READ_ASSEMBLIES_LIST_FROM_REFLECTION) {
			//Get classes from reflection
			System.IO.StreamWriter sw = null;
			if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw = System.IO.File.CreateText(assemblies_txt_file);

			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				//Debug.Log(asm.FullName);
				if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("Assembly full name: " + asm.FullName);
				if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("----------------------------------------------------------------------------------");

				string t = asm.FullName.ToUpper();
				if (true || t.Contains("CSHARP") || t.Contains("MSCORLIB") || t.Contains("SYSTEM") || t.Contains("UNITYENGINE"))
				{
					foreach (Type type in asm.GetTypes())
					{
						//Debug.Log(type.FullName);
						//if (type.FullName.ToUpper().Contains("BOT")) Debug.Log(asm.FullName + " " + type.FullName);
						string ttt = (type.IsEnum ? " - ENUM" : "");
						if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("Type: " + type.FullName + ttt);

						if (type.IsEnum) {
							foreach (string s in type.GetEnumNames()) {
								if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("Enum: " + s);
							}
						}

						string t_name = type.FullName;
						t_name = t_name.Replace("+", ".").Replace(">", "").Replace("<", "");
						if (t_name.Contains("`")) t_name = t_name.Substring(0, t_name.IndexOf("`"));
						if (t_name.Contains("c__")) t_name = t_name.Substring(0, t_name.IndexOf("c__"));

						classes_dict current_level_dict = classes_dictionary;
						foreach (string n in t_name.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries))
						{
							if (!current_level_dict.dict.ContainsKey(n)) current_level_dict.dict.Add(n, new classes_dict());
							current_level_dict = current_level_dict.dict[n];
						}

						MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
						foreach (var mi in methodInfos) {
							if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("Method: " + mi.Name);
							//if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.WriteLine("Method: " + mi.Name + " (" + mi.MemberType.ToString() + ")");
							if (!current_level_dict.dict.ContainsKey(mi.Name)) current_level_dict.dict.Add(mi.Name, new classes_dict());
						}
					}
				}
				if (WRITE_ASSEMBLIES_LIST_TO_FILE) { sw.WriteLine(""); sw.WriteLine(""); sw.WriteLine(""); }
			}
			if (WRITE_ASSEMBLIES_LIST_TO_FILE) sw.Close();
		} else if (READ_ASSEMBLIES_LIST_FROM_FILE) {
			//Get classes from file
			var sr = System.IO.File.OpenText(assemblies_txt_file);
			classes_dict current_level_dict = classes_dictionary;

			while (!sr.EndOfStream) {
				string l = sr.ReadLine().Trim();

				if (l == "") continue;
				if (l.ToUpper().StartsWith("-")) continue;
				
				if (l.ToUpper().StartsWith("TYPE:")) {
					current_level_dict = classes_dictionary;

					string t_name = l.Substring(5).Trim();
					t_name = t_name.Replace("+", ".").Replace(">", "").Replace("<", "");
					if (t_name.Contains("`")) t_name = t_name.Substring(0, t_name.IndexOf("`"));
					if (t_name.Contains("c__")) t_name = t_name.Substring(0, t_name.IndexOf("c__"));

					foreach (string n in t_name.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries))
					{
						if (!current_level_dict.dict.ContainsKey(n)) current_level_dict.dict.Add(n, new classes_dict());
						current_level_dict = current_level_dict.dict[n];
					}
				}

				if (l.ToUpper().StartsWith("METHOD:")) {
					string m_name = l.Substring(7).Trim();
					if (!current_level_dict.dict.ContainsKey(m_name)) current_level_dict.dict.Add(m_name, new classes_dict());
				}
			}
		}

		// Type thisType = this.GetType();
		// MethodInfo theMethod = thisType.GetMethod("gameObject.transform.position.x");
		// theMethod.Invoke(this, null);
	}
	
	// Update is called once per frame
	void Update () {
		BOT.Update();

		if (res != null && res.IsCompleted) {
			bool there_is_suspended_loot = false;
			foreach (var g in Engine.Level_Spawnable) {
				Loot lt = g.GetComponent<Loot>();
				if (lt != null && lt.isDeactivating() ) { there_is_suspended_loot = true; break; }
			}

			//If script error, ignore suspended_enemie. 
			//Otherwise fighting bomb with 0HP will freeze script (it will throw error, but the bomb will suspend stopping)
			bool there_is_suspended_enemie = false;
			if (BOT.last_error == "") { 
				foreach (var e in Engine.Level_Enemies) if (e.suspend) { there_is_suspended_enemie = true; break; }
			}

			if (!there_is_suspended_enemie && !there_is_suspended_loot && !BOT.dying_in_progress) {
				if (deleg != null){
					try {
						deleg.EndInvoke(res);
					} catch (Exception e) {
						//need to skip "Thread was being aborted" because it's by design. 
						//BUT need to show other errors if there are more then one
						//operation is not valid due to current state of object
						Error_Panel.ShowError(e.Message.Replace("`", "'"));
					}
					deleg = null;
				}
				res = null;
				BOT.script_thread = null;
				ResetBotState();
			}
		}

		//Textmesh pro automatically reenable wrapping each time :(
		//Debug.Log(ScriptInputField.textComponent.enableWordWrapping.ToString());
		if (ScriptInputField != null) {
			if (ScriptInputField.textComponent.enableWordWrapping) ScriptInputField.textComponent.enableWordWrapping = false;
		}

		TransformMy.Update();
	}

	public void Play()
	{
		if (Control_UI.isInPauseState()){
			Resume(); return;
		}

		if (res != null) {
			Error_Panel.ShowError("Script already running."); return;
		}

		res = null;
		deleg = null;
		string str = "";
		if (ScriptInputField != null) str = ScriptInputField.text; else str = ScriptEditor_My.text;
		cur_active_class.content = str;
		//str = Regex.Replace(str, "<.*?>", string.Empty); //Remove HTML tags TODO: is it dangerous? Check "if (a < 1 && a > 5)". Don't need it with my script editor
		str = Regex.Replace(str, "(for)(.*?)(;)(.*?)(;)", "$1$2я$4я");

		//Fill debug_info - each instruction start/end
		//Also assure if/else/for/while keywords has a {} after them
		//TODO: wrap if / for / foreach / while and lambda ()=> in {} block if there is no one present. do it before entering loop
		//TODO: если не парные фигурные скобки - ничего не делать, просто дать компилятору ругнутся
		//TODO: nested ()  i.e. if ((a < 10) && (b > 5))
		//TODO: if with bracket if(){command;} - p() not added, need to put if/while/for detection outside of ';'
		//TODO: empty {} after if/while/for/else
		//TODO: "if (mail.Contains("1") || mail.Contains("2")) title = "3 " + title;" --> translates to "BOT.P(478, 510);if (mail.Contains("1") {BOT.P(511, 572);|| mail.Contains("2")) title = "3 " + title;}"
		//TODO: "if (char.IsDigit(mail[i])) n = n + mail[i]; else break;" translates to "BOT.P(1021, 1046);if (char.IsDigit(mail[i]){BOT.P(1046, 1063);) n = n + mail[i];} else {BOT.P(1070, 1075);break;}"
		//DONE: nested if/for
		//DONE: need to skip comments
		//DONE: need to skip text in "" and ''
		List<char> empties = new char[]{'\n', '\r', '\t', ' ', '\0'}.ToList();
		List<int> insert_brackets = new List<int>();
		BOT.DebugInfo_Instructions = new List<BOT.DebugInfoStruct>();
		
		int first_bracket = str.IndexOf("{");
		if (first_bracket < 0) {Error_Panel.ShowError("Script does not contain any functions."); return;}
		if (Regex.IsMatch(str.Substring(0, first_bracket + 1), "class\\s+\\w+\\s*{$")) first_bracket = str.IndexOf("{", first_bracket + 1);
		if (first_bracket < 0) {Error_Panel.ShowError("Script does not contain any functions."); return;}

		bool in_quotes_single = false;
		bool in_quotes_double = false;
		bool in_comment_singleLine = false;
		bool in_comment_multiLine = false;

		//local function to add entry to debugInfo
		void add_to_debugInfo(int start, int end) {
			BOT.DebugInfoStruct t = new BOT.DebugInfoStruct();
			t.CommandCharStart = start; t.CommandCharEnd = end;
			t.Editor_Line_Index = ScriptEditor_My.Get_Line_By_Char_Index(start);
			BOT.DebugInfo_Instructions.Add(t);
		}

		int cmdStart = first_bracket + 1;
		for (int i = first_bracket; i < str.Length; i++) {
			//Handle quotes
			if (str[i] == '"' && !in_quotes_single) {
				if (in_quotes_double && i > 0 && str[i-1] != '\\')	in_quotes_double = false;
				else												in_quotes_double = true;
			}
			if (str[i] == '\'' && !in_quotes_double) { 
				if (in_quotes_single && i > 0 && str[i-1] != '\\')	in_quotes_single = false;
				else 												in_quotes_single = true;
			}
			if (in_quotes_single || in_quotes_double) { continue; }

			//Handle comments
			if (str.Length > i+1 && str[i] == '/' && str[i+1] == '/') in_comment_singleLine = true;
			if (str.Length > i+1 && str[i] == '/' && str[i+1] == '*') in_comment_multiLine = true;
			if (in_comment_singleLine) {
				if (str[i] == '\n') { in_comment_singleLine = false; }
				cmdStart = i + 1; continue;
			}
			if (in_comment_multiLine) {
				if (str.Length > i+1 && str[i] == '*' && str[i+1] == '/') in_comment_multiLine = false;
				cmdStart = i + 2; continue;
			}

			if (str[i] == '{') {
				//if there is no ';' inside {} code block, then skip it, because it's lambda or array init - new int[]{3,5,7};
				int ind_close = str.IndexOf("}", i);
				int ind_semicolon = str.IndexOf(";", i);
				if (ind_close < ind_semicolon) {
					//Check if we have full enclosed block here, with the same number of '{' and '}'
					string block = str.Substring(i, ind_semicolon - i);
					//Debug.Log("block: " + block);
					if ( block.Count(c => c == '{') == block.Count(c => c == '}') )
						i = ind_semicolon;
					else
						cmdStart = i + 1; 

					//Debug.Log("cmdStart: " + cmdStart);
				}
				//If we'll want to count conditional and loop operators as command - comment this 'else' block
				//TODO: Uncommenting this will break function declarations as BOT.P will be injected before bracket of new func declaration
				else
					cmdStart = i + 1;
			}
			if (str[i] == '}') cmdStart = i + 1;

			if (str[i] == ';') {
				//Debug.Log("found ; - '" + str.Substring(i-5, 6) + "', cmdstart = " + cmdStart );
				while (empties.Contains(str[cmdStart])) cmdStart++;

				string current_command_substr = "";
				while (true) {
					int new_cmd_start = -1;
					current_command_substr = str.Substring(cmdStart).Trim();
					if      (current_command_substr.StartsWith("if"))    { new_cmd_start = str.IndexOfBalancedEnd('(', ')', cmdStart) + 1; }
					else if (current_command_substr.StartsWith("while")) { new_cmd_start = str.IndexOfBalancedEnd('(', ')',  cmdStart) + 1; }
					else if (current_command_substr.StartsWith("for"))   { new_cmd_start = str.IndexOfBalancedEnd('(', ')',  cmdStart) + 1; }
					else if (current_command_substr.StartsWith("else"))  { new_cmd_start = str.IndexOf("else", cmdStart) + 5; }

					if(new_cmd_start > 0) {
						if (!current_command_substr.StartsWith("else")) {
							//If we'll want to count conditional and loop operators as command - uncomment this
							add_to_debugInfo(cmdStart, new_cmd_start);
						}

						cmdStart = new_cmd_start;
						while (empties.Contains(str[cmdStart])) cmdStart++;

						string new_substr = str.Substring(cmdStart).Trim();
						if(!new_substr.StartsWith("{")) insert_brackets.Add(cmdStart);
					}

					//Debug.Log(current_command_substr.Substring(0, 20).Replace("\n", "") + ", OK = " + f.ToString() + ", new_substr = " + new_substr.Trim().Substring(0, 20).Replace("\n", ""));

					if (str.Substring(cmdStart).Trim().StartsWith("{")) { cmdStart = str.IndexOf("{", cmdStart) + 1; }
					else if (new_cmd_start <= 0) break;
				}

				add_to_debugInfo(cmdStart, i);

				cmdStart = i + 1;
			}
		}

		for (int i = BOT.DebugInfo_Instructions.Count - 1; i >=0; i--)
		{
			int st = BOT.DebugInfo_Instructions[i].CommandCharStart;
			str = str.Insert(st, "BOT.P(" + i.ToString() + ")я");

			if (insert_brackets.Contains(st)) {
				int end = str.IndexOf(";", st) + 1;
				int end_block = str.IndexOf("}", st) + 1;
				if (end > 5 && end < end_block) { 
					str = str.Insert(end, "}").Insert(st, "{");
				}
			}
		}

		/*
		//Inject stuff to start method - BOT.P(n) call after each instruction
		str = str.Replace(";", ";BOT.P(1);");
		str = str.Replace("{", "{BOT.P(1);");
		//Replace all BOT.P(1) by BOT.P(n)
		int n = 0;
		str = System.Text.RegularExpressions.Regex.Replace(str, "BOT.P\\(1\\)", m => "BOT.P(" + n++ + ")");
		//Remove last BOT.P call, because it is after last instruction
		str = str.Replace("BOT.P("+ (n-1).ToString() +");", "");
		*/

		//Inject stuff to start method - using and class envelop
		// str = "using UnityEngine; \npublic class Compiled {" + str.Replace("void","public static void") + "}";
		// int ind = str.IndexOf("Start");
		// if (ind < 0) {Error_Panel.ShowError("Missing 'Start' function."); return;}
		// ind = str.IndexOf("{", ind);
		// if (ind < 0) {Error_Panel.ShowError("Missing 'Start' function."); return;}
		// str = str.Insert(ind + 1, "\nBOT.script_thread = System.Threading.Thread.CurrentThread;\n");
		str = str.Replace("я", ";");

		// str += "\n\nclass test_class {";
		// str += "  BOT_Shield g;";
		// str += "  public void ttt(float x, float y, float z) {";
		// str += "    Debug.Log(" + UnityEngine.Random.Range(0,9).ToString() + ");";
		// str += "    Vector3 pos = new Vector3(0f, 0f, 0f);";
		// str += "    g = BOT.CreateShield(pos);";
		// str += "    g.test_event += aaa;";
		// str += "    g.MoveTo(x, y, z);";
		// str += "  }";
		// str += "  public void aaa() {";
		// str += "  	g.Release();";
		// str += "  }";
		// str += "}";

		//Compiling secondary classes
		//TODO: it should not be done everey time 'play' is pressed
		string str_to_compile = "";
		foreach (classes_info cl in classes) {
			// if (cl.id != 0 && cl.enabled) {
			// 	str += "\n\n" + cl.content + "\n\n";
			// }

			string class_content = cl.id == cur_active_class.id ? str : cl.content;
			if (cl.id == 0) {
				//class_content = "using UnityEngine; using BOT_Helpers; \npublic class Compiled {" + class_content.Replace("void","public static void") + "}";
				class_content = "using UnityEngine; using BOT_Helpers; \npublic class Compiled {" + class_content.Replace("void","public void") + "}";
				int ind = class_content.IndexOf("Start");
				if (ind < 0) {Error_Panel.ShowError("Missing 'Start' function."); return;}
				ind = class_content.IndexOf("{", ind);
				if (ind < 0) {Error_Panel.ShowError("Missing 'Start' function."); return;}
				class_content = class_content.Insert(ind + 1, "\nBOT.script_thread = System.Threading.Thread.CurrentThread;\n");
			}
			if (cl.enabled) {
				str_to_compile += "\n\n" + class_content + "\n\n";
			}
		}

		//Debug - show final script
		BOT.DebugInfo_Instructions.Add(new BOT.DebugInfoStruct());
		BOT.DebugInfo_Instructions.Add(new BOT.DebugInfoStruct());
		string str1 = Regex.Replace(str_to_compile, "BOT.P\\((\\d+)\\)", m => "BOT.P(" + BOT.DebugInfo_Instructions[ int.Parse(m.Groups[1].Value) ].CommandCharStart.ToString() + ", " + BOT.DebugInfo_Instructions[ int.Parse(m.Groups[1].Value) ].CommandCharEnd.ToString() + ")" );
		var o = GameObject.Find("Panel_Options").transform.Find("Tab6_Content");
		o.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = str1;

		str_to_compile += "public class launcher { public static void launch() { var l = new Compiled(); l.Start(); } }";

		var assembly = Compile(str_to_compile);
		if (assembly != null)
		{
			foreach (GameObject g in Engine.Level_Spawnable){
				Loot lt = g.GetComponent<Loot>();
				if (lt != null) lt.Set();
			}
			foreach (var ISetable in Engine.Level_Setable) {
				ISetable.Set();
			}
			var rr = Engine.Level_Readable.Where(x=> x.randomize_on_play).ToArray();
			if (rr.Count() == 2) {
				string[] t = Phrase_Generator.Generate_Random_Phrase().Split(new char[]{' '});
				rr[0].text = t[0]; rr[1].text = t[1];
			}

			Engine.Loot_list.Clear();
			Engine.commands_count = BOT.DebugInfo_Instructions.Count - 2; //Substract 2, because we added 2 empty entry above, to show debug script
			BOT.last_error = "";
			BOT.suspend_script = 0;
			BOT.Set_Pause(BOT.Pause_States.Reset);
			//var methodStart = assembly.GetType("Compiled").GetMethod("Start");
			var methodStart = assembly.GetType("launcher").GetMethod("launch");

			deleg = (Action)Delegate.CreateDelegate(typeof(Action), methodStart);
			res = deleg.BeginInvoke(null, null);
			Control_UI.set_play_state();
		}
	}
	public void Stop(bool noDissolveAnim = false)
	{
		if (BOT.script_thread != null && BOT.script_thread.IsAlive)
		{
			res = null;	deleg = null;
			BOT.script_thread.Abort();
			BOT.script_thread = null;
			BOT.events.Clear();
			BOT.bot_instance.StopAllCoroutines();

			if (BOT.dead || BOT.dying_in_progress) {
				BOT.bot_instance.StopAllCoroutines();
				BOT.dead = false; BOT.dying_in_progress = false;
				//BOT.bot_animator.Play("Anim_Idle");
			}
		}
		ResetBotState(noDissolveAnim);
	}
	public void Pause()
	{
		BOT.Set_Pause(BOT.Pause_States.Pause_Command);
		Control_UI.set_pause_state();
	}
	public void Pause_Immediate()
	{
		BOT.Set_Pause(BOT.Pause_States.Pause_Immediate);
		Control_UI.set_pause_state();
		Control_UI.enable_frame();
	}
	public void Resume()
	{
		BOT.Set_Pause(BOT.Pause_States.Reset);
		Control_UI.set_play_state();
	}
	public void AdvanceFrame()
	{
		Control_UI.disable_frame();
		BOT.Set_Pause(BOT.Pause_States.Advance_Command);
	}
	public void ResetBotState(bool noAnim = false)
	{
		//Debug.Log("Reset bot state");

		//In case we have highlighted text in script window, used by debugger
		GameObject ed_my = GameObject.Find("Text_Editor");
		if (ed_my != null) ed_my.GetComponent<Text_Editor>().HighlightText(-1, -1);

		BOT.HP = 100;
		BOT.act = null; BOT.act_in_process = false; BOT.is_paused = false;
		BOT.Set_Pause(BOT.Pause_States.Reset);
		Robot.PlaySfx(-1);
		Control_UI.set_stop_state(); Engine.CheckRequirement(); Control_UI.disable_canvas();
		Engine.commands_count = -1;

		//TODO: check if it's not another camera
		Camera.main.DOKill();
		Camera.main.DOFieldOfView(30f, 2f);

		//Bot does not need rebase anim if he is at right place, and his props (like say-plane) are hidden
		bool bot_does_not_need_rebase_anim = isBotAlreadyInPlace();
		if (bot_does_not_need_rebase_anim) Control_UI.enable_canvas();

		//noanim - is used when stop is called from LoadNextScene
		//dont_show_rebase_anim_next_time - is set by Engine Step - i.e. after requirement met for level complete, but there are some additional step
		if (!bot_does_not_need_rebase_anim && !noAnim && !Camera_Controller.level_complete_shown && !dont_show_rebase_anim_next_time) {
			BOT.bot_obj.GetComponent<Robot>().Dissolve();
			StartCoroutine("ResetBotState_Coroutine");
		}
		dont_show_rebase_anim_next_time = false;

		//Reset talk plane & scanner
		BOT.tw_talk = null;
		BOT.talk_plane.transform.DOKill();
		BOT.talk_plane.transform.localScale = new Vector3(0f, 0f, 0f);
		BOT.bot_obj.transform.GetChild(0).GetChild(8).GetChild(0).DOKill();
		BOT.bot_obj.transform.GetChild(0).GetChild(8).GetChild(0).gameObject.SetActive(false);

		//Reset loot and collectors
		//Debug.Log("Spawnable count: " + Engine.Level_Spawnable.Count.ToString());
		if (!Camera_Controller.level_complete_shown) {
			foreach (GameObject g in Engine.Level_Spawnable){
				Loot lt = g.GetComponent<Loot>();
				if (lt != null) lt.Reset();
			}
			foreach (Collector cl in Engine.Level_Collectors){
				cl.status = 0;
			}
			foreach (Enemy en in Engine.Level_Enemies) {
				en.Reset(false);
			}
			foreach (Projectile pr in Engine.Level_Projectiles) {
				Destroy(pr.gameObject);
			}
			foreach (Engine.IDestructible idstr in Engine.Level_Destructibles) {
				idstr.Destroy();
			}
			foreach (Engine.IResetable r in Engine.Level_Resetable) {
				r.Reset();
			}
			foreach (GameObject dstr in Engine.Objects_To_Destroy) {
				Destroy(dstr);
			}
			Engine.Objects_To_Destroy.Clear();
			Engine.Level_Projectiles.Clear();
			Engine.Level_Destructibles.Clear();

			Hud.Log("Скрипт завершился, задание не выполнено.");
			if (Engine.requirement_additional_info != "") Hud.Log(Engine.requirement_additional_info);
		}

		//Need to reset death AFTER enemy reset, otherwise requirement is true even if bot is dead
		BOT.dead = false;
	}
	bool isBotAlreadyInPlace() {
		bool bot_does_not_need_rebase_anim = true;

		Vector2 pos_bot_2d = new Vector2(BOT.bot_obj.transform.position.x, BOT.bot_obj.transform.position.z);
		Vector2 pos_spawner_2d = new Vector2(Engine.spawner.transform.position.x, Engine.spawner.transform.position.z);
		// Debug.Log("float min: " + float.MinValue);
		// Debug.Log("pos dif: " + Vector3.Distance(BOT.bot_obj.transform.position, Engine.spawner.transform.position));
		// Debug.Log("pos dif 2d: " + Vector2.Distance(pos_bot_2d, pos_spawner_2d));
		// Debug.Log("pos dif height: " + Mathf.Abs(BOT.bot_obj.transform.position.y - Engine.spawner.transform.position.y));
		// Debug.Log("ang dif: " + Quaternion.Angle(BOT.bot_obj.transform.rotation, Engine.spawner.transform.rotation));
		// Debug.Log("size dif: " + Vector3.Distance(BOT.bot_obj.transform.lossyScale, new Vector3(3f, 3f, 3f)));

		bot_does_not_need_rebase_anim &= Vector2.Distance(pos_bot_2d, pos_spawner_2d) <= 0.001f;
		bot_does_not_need_rebase_anim &= Mathf.Abs(BOT.bot_obj.transform.position.y - Engine.spawner.transform.position.y) < 0.02f;
		bot_does_not_need_rebase_anim &= Quaternion.Angle(BOT.bot_obj.transform.rotation, Engine.spawner.transform.rotation) <= 0.001f;
		bot_does_not_need_rebase_anim &= Vector3.Distance(BOT.bot_obj.transform.lossyScale, new Vector3(3f, 3f, 3f)) <= 0.001;

		//Debug.Log("bot_does_not_need_rebase_anim = " + bot_does_not_need_rebase_anim.ToString());

		if (bot_does_not_need_rebase_anim) {
			//Reset bot parts and arms
			Engine.bot_base.GetComponent<Robot_Base>().Appear();
		}

		return bot_does_not_need_rebase_anim;
	}
	IEnumerator ResetBotState_Coroutine() {
		BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = true;
		BOT.bot_obj.GetComponent<BoxCollider>().enabled = false;
		yield return new WaitForSeconds(0.75f); //Wait for bot to dissolve

		BOT.bot_animator.Play("Anim_Idle"); //If stopped while dead or picking up anim - reset animator
		if (Options_Global_Static.BOT_FastRebase.isOn) {
			BOT.bot_obj.transform.position = Engine.spawner.transform.position;
			BOT.bot_obj.transform.rotation = Engine.spawner.transform.rotation;
			BOT.bot_obj.transform.localScale = new Vector3(3f, 3f, 3f);
			Engine.bot_base.GetComponent<Robot_Base>().Appear(); //Reset arms and other bor parts
			BOT.bot_obj.GetComponent<Robot>().Appear();
        	BOT.bot_obj.GetComponent<BoxCollider>().enabled = true;
        	BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
			yield return new WaitForSeconds(0.75f); //Wait for bot to appear
			Control_UI.enable_canvas();
			yield break;
		}

		//Move base to spawner
		Vector3 spawn_pos = Engine.spawner.transform.position;
		Engine.bot_base.transform.position = new Vector3(spawn_pos.x, 0.1f, spawn_pos.z);
		Engine.bot_base.transform.rotation = Engine.spawner.transform.rotation;

		//Move bot to base and activate animation
		BOT.bot_obj.transform.parent = Engine.bot_base.transform.GetChild(2);
		BOT.bot_obj.transform.localPosition = Vector3.zero - Vector3.up * 0.6f; //0.4f;
		BOT.bot_obj.transform.localRotation = Quaternion.identity;
		BOT.bot_obj.transform.localScale = new Vector3(2f, 3f, 2f);
		BOT.bot_obj.GetComponent<Robot>().Appear_Immediate();
		BOT.bot_obj.SetActive(false);
		Engine.bot_base.SetActive(true);
		Engine.bot_base.GetComponent<Animator>().SetTrigger("Trig");
		//Reparanting and reactivating robot is done in Robot_Base script from animation events
	}

	public void Compile_Secondary_Classes() {
		string str = "";
		foreach (classes_info cl in classes) {
			if (cl.id != 0 && cl.enabled) {
				str += cl.content + "\n\n";
			}
		}
		var assembly = Compile(str);
		if (assembly != null)
		{
			Debug.Log("Compiled.");
			//Debug.Log("Compiled. Assembly location: " + assembly.Location);
		}
	}


	public static Assembly Compile(string source) {
		//var provider = new CSharpCodeProvider();
		var provider2 = new CSharpCompiler.CodeCompiler();
		var param = new CompilerParameters();

		// Add ALL of the assembly references
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (!(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder))
				param.ReferencedAssemblies.Add(assembly.Location);
		}

		// Add specific assembly references
		//param.ReferencedAssemblies.Add("System.dll");
		//param.ReferencedAssemblies.Add("CSharp.dll");
		//param.ReferencedAssemblies.Add("UnityEngines.dll");

		// Generate a dll in memory
		param.GenerateExecutable = false;
		param.GenerateInMemory = true;
		param.WarningLevel = 3;

		// Compile the source
		//var result = provider.CompileAssemblyFromSource(param, source);
		var result = provider2.CompileAssemblyFromSource(param, source);

		if (result.Errors.Count > 0) {
			var msg = new StringBuilder();
			foreach (CompilerError error in result.Errors) {
				if (error.IsWarning) { Hud.Log("Compile warning: " + error.ErrorText.Replace("`", "'") ); continue; }
				msg.AppendFormat("Error ({0}): {1}\n", error.ErrorNumber, error.ErrorText.Replace("`", "'") );
			}
			if (msg.Length > 0) Error_Panel.ShowError(msg.ToString());
		}

		// Return the assembly
		return result.CompiledAssembly;
	}

	public void Suggestion (string str)
	{
		if (cur_active_class != null) {
			cur_active_class.changed = true;
			cur_active_class.content = str;
		}

		str = " " + str;
		//int pos = ScriptInputField.caretPosition + 1;
		int pos = ScriptInputField.stringPosition;
		if (pos >= str.Length) return;
		int pos2 = str.LastIndexOfAny(new char[]{';', ' ', '\n', '{', '}'}, pos);
		str = str.Substring(pos2 + 1, pos - pos2);
		//Debug.Log ("Str = '" + str + "', corrPos = "+ pos.ToString() + ", word bedin pos = " + pos2.ToString());
		if (str == "") return;

		var current_namespace = classes_dictionary.dict; //Root
		var arr = str.Split(new char[]{'.'}, StringSplitOptions.None);
		for (int n = 0; n < arr.Length - 1; n++) {
			if (current_namespace.ContainsKey(arr[n])) {
				//Debug.Log("Switch namespace to: " + arr[n]);
				current_namespace = current_namespace[arr[n]].dict;
			}
			else {
				//Debug.Log("Namespace not found: " + arr[n]);
				return;
			}
		}

		int sugCount = 0;
		string sugStr = "";
		List<string> sugList = new List<string>();
		foreach (KeyValuePair<string, classes_dict> kv in current_namespace)
		{
			//Debug.Log(kv.Key);
			if (kv.Key.ToUpper().StartsWith(arr[arr.Length-1].ToUpper()))
			{
				sugList.Add(kv.Key);
				sugStr += kv.Key + "\n";
				sugCount += 1;
				if (sugCount > 15) break;
			}
		}
		if (sugStr != "" && suggestion != null) suggestion.Suggest(sugStr);
		//Debug.Log(sugStr);
	}

}

public class classes_info {
	public int id = -1;
	public string content = "";
	public string original_name = "";
	public bool active = false;
	public bool changed = false;
	public bool saved = false;
	public string saved_fileName = "";
	public bool enabled = false;

	const string EMPTY_CLASS_NAME = "Unnamed class";
	const string MAIN_CLASS_NAME = "Main";

	Regex func_regexp_name = new Regex(@"(((abstract|async|extern|override|partial|readonly|sealed|static|virtual|volatile|public|private|protected|internal)\s+)*[\w]+\s+[\w]+\s*\(.*?\))\s*{", RegexOptions.Compiled);
	//Regex func_regexp_content = new Regex(@"({(?>[^{}]+|(?1))*})", RegexOptions.Compiled);

	public classes_info(string class_content, string function_name = "") {
		content = class_content;
		if (function_name == "") 
			original_name = GetClassName(); 
		else 
			original_name = function_name;
	}

	public string GetClassName() {
		Match m = Regex.Match(content, "class\\s+(\\w+)", RegexOptions.Compiled);
		if (m.Success && m.Groups.Count > 1) return m.Groups[1].Value;
		if (id == 0) return "Main";
		return EMPTY_CLASS_NAME;
	}
	public static bool IsClass(string text = "") {
		return Regex.Match(text, "class\\s+(\\w+)", RegexOptions.Compiled).Success;
	}

	public string[] GetFuncSignatures() {
		int in_brackets = 0;
		string functions = "";

		string name = GetClassName();
		bool has_class_def = name != EMPTY_CLASS_NAME && name != MAIN_CLASS_NAME;

		foreach (char c in content) {
			if (c == '{') { in_brackets++; functions += "{"; continue; }
			if (c == '}') { in_brackets--; functions += "}"; continue; }

			if      (has_class_def  && in_brackets != 1) continue;
			else if (!has_class_def && in_brackets != 0) continue;

			if (c != '\r' && c != '\n' && c != '\t') functions += c;
		}
		functions = Regex.Replace(functions, @"\s+", " ", RegexOptions.Compiled); //Replace multiple spaces by single
		functions = functions.Replace(" (", "(");
		//Debug.Log(GetClassName() + "-> found " + func_regexp_name.Matches(functions).Count + " functions in: " + functions);
		return func_regexp_name.Matches(functions).Cast<Match>().Select(x=>x.Groups[1].Value).ToArray();
		//functions = functions.Replace(")", ")|");
		//return functions.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries).Select((x)=>x.Trim()).Where((x)=>x != "").ToArray();
	}

	public static string[] GetFuncNameAndParamFromSignature(string signature) {
        string name = signature.Substring(0, signature.LastIndexOf("(")).Trim(); 
        name = name.Substring(name.LastIndexOf(" ") + 1).Trim();

		return new string[]{ name, signature.Substring(signature.LastIndexOf("(")).Trim() };
	}

	public string GetFuncBySignature(string signature) {
		int ind = content.IndexOf(signature);
		if (ind < 0) return "";

		int ind_br = content.IndexOf("{", ind) + 1;

		int end = 0;
		int level = 0;
		bool in_str1 = false;
		bool in_str2 = false;
		for (int i = ind_br; i < content.Length; i++) {
			if (content[i] == '"') in_str1 = !in_str1;
			if (content[i] == '\'') in_str2 = !in_str2;
			if (in_str1 || in_str2) continue;

			if (content[i] == '{') level++;
			if (content[i] == '}') level--;
			if (level < 0) { end = i; break; }
		}

		//Debug.Log("start: " + ind + ", end: " + end + ", count: " + (end - ind) + ", content: " + content.Substring(ind, end - ind));

		if (level >= 0) return ""; //not closed brackets
		//else return signature + " {\r\n" + content.Substring(ind, end - ind).Trim();
		else return content.Substring(ind, end - ind + 1).Trim();

		// Match m = func_regexp_content.Match(content, ind);
		// if (m.Success) return signature + m.Value.Trim(); else return "";
	}
}


public struct EnemyInfo {
	public int HP;
	public int MP;
	public float pos_x;
	public float pos_y;
}
public class BOT : MonoBehaviour
{
	//Stats
	public static int HP = 100;
	public static int MP = 7;
	public static int speed = 5;
	public static int qEnergy = 0;

	//Config
	public static float moving_speed = 0.025f;
	static float rotating_speed = 1f;

	//Obj reference
	public static GameObject bot_obj;
	public static GameObject talk_plane;
	public static GameObject bot_shield;
	static Text talk_text;
	public static BOT bot_instance;
	public static Text_Editor script_editor;

	//Controls
	public static Action act = null;
	public static List<Action> events = new List<Action>();
	public static bool act_in_process = false;
	public static bool pause = false;
	public static bool pause_command = false;
	public static bool frame_advance = false;
	public static bool frame_advance_command = false;
	public static bool is_paused = false;
	public static System.Threading.Thread script_thread = null;
	public static int suspend_script = 0;
	public enum Pause_States { Reset, Pause_Command, Pause_Immediate, Advance_Command };

	//Internals
	static int val_to_return_int = 0;
	static float val_to_return_float = 0f;
	static bool val_to_return_bool = false;
	static string val_to_return_string = "";
	static Vector2 val_to_return_vector2;
	static Enemy val_to_return_enemy = null;
	static EnemyInfo val_to_return_enemy_info;
	static UnityEngine.Object val_to_return_object;
	static System.Object val_to_return_object_sys;
	public static List<KeyCode> key_downs = new List<KeyCode>();
	Action bot_anim_pause = null;
	Action bot_anim_unpause = null;

	//public static
	public static bool dead = false;
	public static bool dying_in_progress = false;
	public static Animator bot_animator = null;
	public static List<Collider> cur_colliders = new List<Collider>();
	public static Tweener tw_talk = null;
	public static string last_error = "";
	public static string blocked = "";

	//Debugger
	public static List<DebugInfoStruct> DebugInfo_Instructions = new List<DebugInfoStruct>();
	public struct DebugInfoStruct {
		public int CommandCharStart;
		public int CommandCharEnd;
		public int Editor_Line_Index;
	}

	public enum Directions { left, right, forward, backward }

	void Start()
	{
		bot_instance = this;
		bot_obj = GameObject.Find("Robot");
		bot_animator = bot_obj.transform.GetChild(0).GetComponent<Animator>();
		talk_plane = bot_obj.transform.Find("TalkPlane").gameObject;
		talk_text = GameObject.Find("Talk_Text").GetComponent<Text>();
		talk_plane.SetActive(false);

		bot_anim_pause = new Action(()=> bot_animator.speed = 0f );
		bot_anim_unpause = new Action(()=> bot_animator.speed = 1f );
	}

	//This functions are called async, from another thread
	public static void Say(string str, float duration = 1.2f)
	{
		if (blocked.ToLower().Contains("say(x)")) throw new Exception("I don't know about 'Say(x)' method yet.");
		act = () => bot_instance.SayShow(str, duration); Thread_Wait();
	}
	public static void Say(float f, float duration = 1.2f)
	{
		if (blocked.ToLower().Contains("say(f)")) throw new Exception("I don't know about 'Say(f)' method yet.");
		act = () => bot_instance.SayShow(f.ToString("N8"), duration); Thread_Wait();
	}
	public static void Move(int d = 999999)
	{
		if (d == 999999 && blocked.ToLower().Contains("move()")) throw new Exception("I don't know about 'Move()' method yet.");
		if (d != 999999 && blocked.ToLower().Contains("move(x)")) throw new Exception("I don't know about 'Move(x)' method yet.");
		act = () => bot_instance.StartCoroutine("MoveCoroutine", d); Thread_Wait();
	}
	public static void Move(float f)
	{
		if (blocked.ToLower().Contains("move(x)")) throw new Exception("I don't know about 'Move(x)' method yet.");
		act = () => bot_instance.StartCoroutine("MoveCoroutineFloat", f); Thread_Wait();
	}
	public static void Rotate(int d)
	{
		if (blocked.ToLower().Contains("rotate(x)")) throw new Exception("I don't know about 'Rotate(x)' method yet.");
		act = () => bot_instance.StartCoroutine("RotateCoroutine", d); Thread_Wait();
	}
	public static void RotateTo(float x, float y, float step = 0f)
	{
		bool step_Is_0 = Mathf.Approximately(step, 0f);
		if (step_Is_0 && blocked.ToLower().Contains("rotateto(x,y)")) throw new Exception("I don't know about 'RotateTo(x, y)' method yet.");
		if (!step_Is_0 && blocked.ToLower().Contains("rotateto(x,y,f)")) throw new Exception("I don't know about 'RotateTo(x, y, s)' method yet.");
		act = () => bot_instance.StartCoroutine(bot_instance.RotateToCoroutine(x, y, step)); Thread_Wait();
	}
	public static void Scale(float value)
	{
		if (blocked.ToLower().Contains("scale(v)")) throw new Exception("I don't know about 'Scale(v)' method yet.");
		act = () => bot_instance.ScaleSub(value); Thread_Wait();
	}
	public static int PickUp()
	{
		if (blocked.ToLower().Contains("pickup()")) throw new Exception("I don't know about 'PickUp()' function yet.");
		act = () => bot_instance.StartCoroutine(bot_instance.PickupCoroutine("", false)); Thread_Wait();
		return val_to_return_int;
	}
	public static int CheckContainerCapacity()
	{
		if (blocked.ToLower().Contains("checkcontainercapacity()")) throw new Exception("I don't know about 'CheckContainerCapacity()' function yet.");
		act = () => bot_instance.StartCoroutine(bot_instance.PickupCoroutine("check", false)); Thread_Wait();
		return val_to_return_int;
	}
	public static void DestroyContainer()
	{
		if (blocked.ToLower().Contains("destroycontainer()")) throw new Exception("I don't know about 'DestroyContainer()' method yet.");
		act = () => bot_instance.StartCoroutine(bot_instance.PickupCoroutine("destroy", false)); Thread_Wait();
	}
	public static void DestroyContainerWithAdditionalBrutality()
	{
		if (blocked.ToLower().Contains("destroycontainerwithadditionalbrutality()")) throw new Exception("I don't know about 'DestroyContainerWithAdditionalBrutality()' method yet.");
		act = () => bot_instance.StartCoroutine(bot_instance.PickupCoroutine("destroy", true)); Thread_Wait();
	}
	public static void PutEnergy(int q)
	{
		//Debug.Log("PutEnergy - async");
		if (blocked.ToLower().Contains("putenergy()")) throw new Exception("I don't know about 'PutEnergy()' method yet.");
		KeyValuePair<Loot.loot_type_enum, int> t;
		t = new KeyValuePair<Loot.loot_type_enum, int>(Loot.loot_type_enum.QuanticEnergy, q);
		act = () => bot_instance.StartCoroutine("PutCoroutine", t); Thread_Wait();
	}
	public static void Fight()
	{
		if (blocked.ToLower().Contains("fight()")) throw new Exception("I don't know about 'Fight()' method yet.");
		act = () => bot_instance.StartCoroutine("Fight", ""); Thread_Wait();
	}
	public static int CheckPotatoes() {
		if (blocked.ToLower().Contains("checkpotatoes()")) throw new Exception("I don't know about 'CheckPotatoes()' method yet.");
		act = () => bot_instance.CheckPotatesSub(); Thread_Wait();
		return val_to_return_int;
	}
	public static Enemy GetClosestEnemy()
	{
		if (blocked.ToLower().Contains("getclosestenemy()")) throw new Exception("I don't know about 'GetClosestEnemy()' function yet.");
		act = () => bot_instance.GetClosestEnemySub(); Thread_Wait();
		return val_to_return_enemy;
	}
	public static EnemyInfo GetClosestEnemyInfo()
	{
		if (blocked.ToLower().Contains("getclosestenemyinfo()")) throw new Exception("I don't know about 'GetClosestEnemyInfo()' function yet.");
		act = () => bot_instance.GetClosestEnemyInfoSub(); Thread_Wait();
		return val_to_return_enemy_info;
	}
	public static Projectile GetClosestProjectile()
	{
		if (blocked.ToLower().Contains("getclosestprojectile()")) throw new Exception("I don't know about 'GetClosestProjectile()' function yet.");
		act = () => bot_instance.GetClosestProjectileSub(); Thread_Wait();
		return ((Projectile)val_to_return_object);
	}
	public static string GetText() {
		if (blocked.ToLower().Contains("gettext()")) throw new Exception("I don't know about 'GetText()' function yet.");
		act = () => bot_instance.GetTextSub(); Thread_Wait();
		return val_to_return_string;
	}
	public static bool GetIndicatorState() {
		if (blocked.ToLower().Contains("getsndicatorstate()")) throw new Exception("I don't know about 'GetIndicatorState()' function yet.");
		act = () => bot_instance.GetIndicatorStateSub(); Thread_Wait();
		return val_to_return_bool;
	}
	public static void SetSwitchState(bool b) {
		if (blocked.ToLower().Contains("setswitchstate()")) throw new Exception("I don't know about 'SetSwitchState(bool b)' method yet.");
		act = () => bot_instance.SetSwitchStateSub(b); suspend_script++; Thread_Wait();
	}
	public static Vector2 Position()
	{
		if (blocked.ToLower().Contains("position()")) throw new Exception("I don't know about 'Position()' function yet.");
		act = () => bot_instance.GetBotPosition(); Thread_Wait();
		return val_to_return_vector2;
	}
	public static bool GetKey(KeyCode k)
	{
		if (blocked.ToLower().Contains("getkey()")) throw new Exception("I don't know about 'getkey()' function yet.");
		act = () => bot_instance.GetKeySub(k); Thread_Wait();
		return val_to_return_bool;
	}
	public static bool GetKeyDown(KeyCode k)
	{
		if (blocked.ToLower().Contains("getkeydown()")) throw new Exception("I don't know about 'getkeydown()' function yet.");
		act = () => bot_instance.GetKeySub(k, true); Thread_Wait();
		return val_to_return_bool;
	}
	public static bool CheckObstacle(Directions dir = Directions.forward)
	{
		if (blocked.ToLower().Contains("checkobstacle()")) throw new Exception("I don't know about 'CheckObstacle()' function yet.");
		act = () => bot_instance.CheckObstacleSub(dir); Thread_Wait();
		return val_to_return_bool;
	}
	public static BOT_Shield CreateShield(Vector3 pos)
	{
		if (blocked.ToLower().Contains("createshield()")) throw new Exception("I don't know about 'CreateShield()' function yet.");
		act = () => bot_instance.CreateShieldSub(pos); Thread_Wait();
		return ((BOT_Shield)val_to_return_object);
	}
	public static float GetFloorSquare()
	{
		if (blocked.ToLower().Contains("getfloorsquare()")) throw new Exception("I don't know about 'GetFloorSquare()' function yet.");
		act = () => bot_instance.GetFloorSquareSub(); Thread_Wait();
		return val_to_return_float;
	}
	// public static Terminal.terminal_data Terminal_Read() {
	// 	if (blocked.ToLower().Contains("terminal_read()")) throw new Exception("I don't know about 'Terminal_Read()' function yet.");
	// 	act = () => bot_instance.StartCoroutine( bot_instance.Terminal_ReadSub() ); Thread_Wait();
	// 	return (Terminal.terminal_data)val_to_return_object_sys;
	// }
	// public static void Terminal_Answer(bool responce) {
	// 	if (blocked.ToLower().Contains("terminal_answer()")) throw new Exception("I don't know about 'Terminal_Answer()' method yet.");
	// 	act = () => bot_instance.StartCoroutine ( bot_instance.Terminal_AnsewerSub(responce) ); Thread_Wait();
	// }
	// public static void Terminal_Answer(string[] responce) {
	// 	if (blocked.ToLower().Contains("terminal_answer()")) throw new Exception("I don't know about 'Terminal_Answer()' method yet.");
	// 	act = () => bot_instance.StartCoroutine ( bot_instance.Terminal_AnsewerSub(false, true, responce) ); Thread_Wait();
	// }
	public static Terminal_Universal.terminal_data Terminal_Read() {
		if (blocked.ToLower().Contains("terminal_read()")) throw new Exception("I don't know about 'Terminal_Read()' function yet.");
		act = () => bot_instance.StartCoroutine( bot_instance.TerminalU_ReadSub() ); Thread_Wait();
		return (Terminal_Universal.terminal_data)val_to_return_object_sys;
	}
	public static void Terminal_Answer(bool responce) {
		if (blocked.ToLower().Contains("terminal_answer()")) throw new Exception("I don't know about 'Terminal_Answer()' method yet.");
		act = () => bot_instance.StartCoroutine ( bot_instance.TerminalU_AnsewerSub(responce) ); Thread_Wait();
	}
	public static void Terminal_Answer(string[] responce) {
		if (blocked.ToLower().Contains("terminal_answer()")) throw new Exception("I don't know about 'Terminal_Answer()' method yet.");
		act = () => bot_instance.StartCoroutine ( bot_instance.TerminalU_AnsewerSub(false, true, responce) ); Thread_Wait();
	}
	public static void Terminal_Answer(object responce) {
		if (blocked.ToLower().Contains("terminal_answer()")) throw new Exception("I don't know about 'Terminal_Answer()' method yet.");
		act = () => bot_instance.StartCoroutine ( bot_instance.TerminalU2_AnsewerSub(responce) ); Thread_Wait();
	}
	public static void Die()
	{
		if (blocked.ToLower().Contains("die()")) throw new Exception("I don't know about 'die()' method yet.");
		HP = 0; Thread_Wait();
	}
	public static void ProcessEvents()
	{
		lock (events) {
			foreach (var a in events) {
				a.Invoke();
				if (last_error != "") { throw new Exception(last_error); } 
			}
			events.Clear();
			key_downs.Clear();
		}
	}
	public static void WaitForEvents()
	{
		while (true) {
			System.Threading.Thread.Sleep(10);
			ProcessEvents();
		}
	}
	public static void P(int n)
	{
		//This is called from user script by autogenerated calls between each user command
		if (script_editor.Have_breakpoint ( DebugInfo_Instructions[n].Editor_Line_Index )) {
			BOT.Set_Pause(BOT.Pause_States.Pause_Command);
			act = () => { Control_UI.set_pause_state(); act = null; act_in_process = false; }; Thread_Wait();
		}
		if (pause_command) {
			act = () => bot_instance.StartCoroutine("Pause", n); Thread_Wait();
		}
	}
	public static void Thread_Wait()
	{
		while (act != null || suspend_script > 0) {
			System.Threading.Thread.Sleep(10);
			if (last_error != "") { throw new Exception(last_error); }
		}
		if (last_error != "") { throw new Exception(last_error); }
	}
	//End async functions

	public void SayShow(string str, float duration = 1.2f)
	{
		//Debug.Log("Saying: " + str);
		tw_talk = null;
		talk_text.text = str;
		talk_plane.transform.DOKill();
		talk_plane.transform.localScale = new Vector3(0f, 0f, 0f);
		talk_plane.SetActive(true);

		//Face to camera
		Vector3 lookPos = Camera.main.transform.position - talk_plane.transform.position; lookPos.y = 0f;
		talk_plane.transform.rotation = Quaternion.LookRotation(lookPos);
		talk_plane.transform.Rotate(90f, 0f, 0f);

		tw_talk = talk_plane.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1f).SetEase(Ease.OutBounce).OnComplete(()=>{
			foreach (var sl in Engine.Level_StringListiners) sl.OnSay(str);
			tw_talk = talk_plane.transform.DOScale(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.OutBounce).SetDelay(duration).OnComplete(()=>{
				act_in_process = false; act = null; tw_talk = null; Engine.SetRequirement("SAY");
			});
		});
	}

	IEnumerator MoveCoroutine(int d)
	{
		//Debug.Log(d);
		float speed = moving_speed;
		if (d < 0) speed *= -1;
		Vector3 cur_pos = bot_obj.transform.localPosition;
		Vector3 target_pos = bot_obj.transform.localPosition + (bot_obj.transform.forward * d);

		//bot_obj.GetComponent<AudioSource>
		Robot.PlaySfx(0);
		float bot_fall_threashold = (bot_obj.transform.lossyScale.y / 3f) * 0.5f;
		while (Vector3.Distance(cur_pos, target_pos) > (moving_speed * Time.timeScale))
		{
			if (script_thread == null) break;
			if (!pause && bot_obj.transform.position.y > bot_fall_threashold) //This is needed to leave control when bot is falling
			{
				cur_pos = bot_obj.transform.localPosition;
				cur_pos += bot_obj.transform.forward * speed * Time.timeScale;
				//bot_obj.transform.localPosition = cur_pos;
				bot_obj.GetComponent<Rigidbody>().MovePosition(cur_pos);
				yield return new WaitForFixedUpdate();
			}
			if (frame_advance) pause = true;
			yield return null;
		}

		Robot.PlaySfx(-1);
		if (!pause && bot_obj.transform.position.y > bot_fall_threashold) bot_obj.GetComponent<Rigidbody>().MovePosition(target_pos);

		//Debug.Log("Finish moving.");
		if (script_thread != null) bot_obj.transform.localPosition = target_pos;
		act_in_process = false; act = null;
	}

	IEnumerator MoveCoroutineFloat(float d)
	{
		float speed = moving_speed;
		if (d < 0) speed *= -1;
		Vector3 cur_pos = bot_obj.transform.localPosition;
		Vector3 target_pos = bot_obj.transform.localPosition + (bot_obj.transform.forward * d);
		while (Vector3.Distance(cur_pos, target_pos) > (moving_speed * Time.timeScale))
		{
			if (script_thread == null) break;
			if (!pause && bot_obj.transform.position.y > 0.5f) //This is needed to leave control when bot is falling
			{
				cur_pos += bot_obj.transform.forward * speed * Time.timeScale;
				bot_obj.transform.localPosition = cur_pos;
			}
			if (frame_advance) pause = true;
			yield return null;
		}

		if (!pause && bot_obj.transform.position.y > 0.5f) bot_obj.GetComponent<Rigidbody>().MovePosition(target_pos);

		//Debug.Log("Finish moving.");
		if (script_thread != null) bot_obj.transform.localPosition = target_pos;
		act_in_process = false; act = null;
	}

	IEnumerator RotateCoroutine(int d)
	{
		float speed = rotating_speed;
		if (d < 0) speed *= -1;

		Vector3 old_pos = bot_obj.transform.position;
		Vector3 cur_rot = bot_obj.transform.rotation.eulerAngles;
		float cur_rot_y = cur_rot.y;
		float target_rot_y = cur_rot_y + d;
		Robot.PlaySfx(1);
		while (Mathf.Abs(Mathf.DeltaAngle(cur_rot_y, target_rot_y)) > (rotating_speed * Time.timeScale))
		{
			if (script_thread == null) break;
			if (!pause)
			{
				cur_rot_y += speed * Time.timeScale;
				bot_obj.transform.rotation = Quaternion.Euler ( cur_rot.x, cur_rot_y, cur_rot.z );
			}
			if (frame_advance) pause = true;
			yield return null;
		}
		Robot.PlaySfx(-1);

		old_pos.y = bot_obj.transform.position.y;
		bot_obj.transform.position = old_pos;
		
		if (script_thread != null) bot_obj.transform.rotation = Quaternion.Euler ( cur_rot.x, target_rot_y, cur_rot.z );
		act_in_process = false; act = null;
	}

	IEnumerator RotateToCoroutine(float x, float y, float step = 0f)
	{
		Vector3 targetPos = new Vector3(x, 0f, y);
		Vector3 direction = (targetPos - bot_obj.transform.position).normalized;
		Quaternion target_rot = Quaternion.LookRotation( new Vector3(direction.x, 0f, direction.z));

		Vector3 cur_rot = bot_obj.transform.rotation.eulerAngles;
		float cur_rot_y = cur_rot.y;

		float speed = rotating_speed;
		if (Mathf.DeltaAngle(cur_rot_y, target_rot.eulerAngles.y) < 0) speed *= -1;

		float total = 0f;
		while (Mathf.Abs(Mathf.DeltaAngle(cur_rot_y, target_rot.eulerAngles.y)) > (rotating_speed * Time.timeScale))
		{
			if (script_thread == null) break;
			if (!pause)
			{
				cur_rot_y += speed * Time.timeScale;
				total += Mathf.Abs(speed * Time.timeScale);
				bot_obj.transform.rotation = Quaternion.Euler ( cur_rot.x, cur_rot_y, cur_rot.z );
				if (step > 0f && total >= step) {
					act_in_process = false; act = null; yield break;
				}
			}
			if (frame_advance) pause = true;
			yield return null;
		}
		if (script_thread != null) bot_obj.transform.rotation = Quaternion.Euler ( cur_rot.x, target_rot.eulerAngles.y, cur_rot.z );
		act_in_process = false; act = null;
	}

	IEnumerator PickupCoroutine(string str = "", bool brutality = false) {
		val_to_return_int = 0;
		List<Collider> colliders_to_remove = new List<Collider>();

		//Version of pickup in front
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no loot in front of me."; yield break;
		}

		Collider col = hit.collider;

		Loot loot = col.GetComponent<Loot>();
		if (loot) {
			while (bot_animator.GetCurrentAnimatorStateInfo(0).IsName("Anim_PickUp_Front")) yield return null;

			//BOT Animation
			float l = 0f;
			if (str != "check") {
				if (str == "destroy" && loot.gameObject.name.StartsWith("toilet")) {
					//Custom destroy-toilet animation
					if (brutality) {
						//Sledgehammer
						bot_animator.SetTrigger("SledgeHammer");
						yield return new WaitForSeconds_BOTPauseAware((1f / 60f) * 50f, bot_anim_pause, bot_anim_unpause);
						var broken_toilet = Instantiate(loot.additional_resources[0], loot.transform.position, loot.transform.rotation);
						Engine.Objects_To_Destroy.Add(broken_toilet); loot.gameObject.SetActive(false);
						yield return new WaitForSeconds_BOTPauseAware(1.5f, bot_anim_pause, bot_anim_unpause);
						Engine.Objects_To_Destroy.Remove(broken_toilet); Destroy(broken_toilet); loot.gameObject.SetActive(true);
					} else {
						//Petard
						//Test blink while throwing petard
						// BOT.bot_animator.SetTrigger("Blink");
						// yield return new WaitForSeconds(0.2f);

						BOT.bot_animator.Play("New State", 1); //Reset eyes before playing timeline
						BOT.bot_animator.Play("New State", 2); //Reset tail before playing timeline
						yield return new WaitForEndOfFrame();
						var pd = bot_obj.GetComponent<UnityEngine.Playables.PlayableDirector>();
						pd.SetGenericBinding(pd.playableAsset.outputs.ElementAt(1).sourceObject, loot.gameObject);
						pd.Play();
						yield return new WaitForSeconds_BOTPauseAware(Convert.ToSingle(pd.duration), bot_anim_pause, bot_anim_unpause);
					}
				} else {
					//Pickup/Destroy - bot arm animation
					foreach(AnimationClip a in bot_animator.runtimeAnimatorController.animationClips)
						if (a.name.ToUpper() == "Anim_PickUp_Front".ToUpper())
					l = a.length - 0.5f;

					bot_animator.SetTrigger("Pickup");
					yield return new WaitForSeconds_BOTPauseAware(l / 2, bot_anim_pause, bot_anim_unpause);
				}
				
				val_to_return_int = loot.quantity; //Because random containers requier to get quantity at the moment we pick it

				//Add loot to bot, after half animation, because it will be instantly logged
				if (str == "") { //Only if pickup
					if (loot.loot_type == Loot.loot_type_enum.HP)                 { HP += loot.quantity; }
					else if (loot.loot_type == Loot.loot_type_enum.MP)            { MP += loot.quantity; }
					else if (loot.loot_type == Loot.loot_type_enum.QuanticEnergy) { qEnergy += loot.quantity; }
					Engine.Loot_list.Add(new KeyValuePair<Loot.loot_type_enum, int>(loot.loot_type, loot.quantity));
					//Debug.Log("Loot list count: " + Engine.Loot_list.Count.ToString());
				}
			} else {
				//Check - scanner animation
				Transform scanner = bot_obj.transform.GetChild(0).GetChild(8).GetChild(0);
				scanner.localRotation = Quaternion.Euler(-25f, 0f, 0f);
				scanner.gameObject.SetActive(true);
				Tweener scan_tween = scanner.DOLocalRotate(new Vector3(25f, 0f, 0f), 2f);
				yield return new WaitForSeconds_BOTPauseAware(2f, new Action(()=>scan_tween.Pause()), new Action(()=>scan_tween.Play()));
				scanner.gameObject.SetActive(false);
				val_to_return_int = loot.quantity; //Because random containers requier to get quantity at the moment we pick it
				Hud.Log("Scan result: container contains " + loot.quantity.ToString() + " " + loot.loot_type.ToString());
			}

			//If it's pickup or destroy operation - hide continaer
			if (str != "check") {
				if (str != "destroy") 	{ loot.PickUpAndDestroy(0); }
				else 					{ loot.PickUpAndDestroy(1); }
				colliders_to_remove.Add(col);
			}

			yield return new WaitForSeconds_BOTPauseAware(l / 2, bot_anim_pause, bot_anim_unpause);
			if (str != "check") while (loot.gameObject.activeSelf) yield return null;
			if (str == "destroy") Hud.Log("Container destroyed");
		} else {
			last_error = "There is no loot in front of me.";
		}

		for (int i = 0; i < colliders_to_remove.Count; i++){
			cur_colliders.Remove(colliders_to_remove[i]);
		}

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	IEnumerator PutCoroutine(KeyValuePair<Loot.loot_type_enum, int> s) {
		//Debug.Log("PutEnergy - main thread");

		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no collector in front of me."; yield break; 
		}

		Collider col = hit.collider;
		Collector clctr = col.GetComponent<Collector>();
		if (clctr) {
			clctr.Put(s.Key, s.Value);
			clctr.GetComponent<Animator>().SetTrigger("Open");

			//TODO: BOT Put Animation
			yield return new WaitForSeconds_BOTPauseAware(1f);
		} else {
			last_error = "There is no collector in front of me.";
		}

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	IEnumerator Fight(string str)
	{
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no enemy in front of me."; yield break;
		}

		Collider col = hit.collider;
		Enemy e = col.transform.parent.GetComponent<Enemy>();
		if (e) {
			if (e.HP <= 0) {last_error = "The enemy is already dead."; yield break;}

			bot_animator.SetTrigger("Pickup");

			float l = 0f;
			foreach(AnimationClip a in bot_animator.runtimeAnimatorController.animationClips)
				if (a.name.ToUpper() == "Anim_PickUp_Front".ToUpper())
					l = a.length - 0.5f;

			yield return new WaitForSeconds_BOTPauseAware(l, bot_anim_pause, bot_anim_unpause);
			e.TakeDamage(1);
			yield return new WaitForSeconds_BOTPauseAware(0.5f, bot_anim_pause, bot_anim_unpause);
		} else {
			last_error = "There is no enemy in front of me.";
		}

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	void GetKeySub(KeyCode k, bool down = false)
	{
		if (down) val_to_return_bool = key_downs.Contains(k);
		else	  val_to_return_bool = Input.GetKey(k);
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetClosestEnemySub()
	{
		Enemy enemy = null;
		float min_dist = 1000000000;
		foreach (Enemy e in Engine.Level_Enemies) {
			float cur_dist = Vector3.Distance(e.transform.GetChild(0).position, bot_obj.transform.position);
			if (cur_dist < min_dist) {
				enemy = e;
				min_dist = cur_dist;
			}
		}

		if (enemy == null) Hud.Log("There is no living enemies.");
		val_to_return_enemy = enemy;

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetClosestEnemyInfoSub()
	{
		Enemy enemy = null;
		float min_dist = 1000000000;
		foreach (Enemy e in Engine.Level_Enemies) {
			float cur_dist = Vector3.Distance(e.transform.GetChild(0).position, bot_obj.transform.position);
			if (cur_dist < min_dist) {
				enemy = e;
				min_dist = cur_dist;
			}
		}

		if (enemy == null) Hud.Log("There is no living enemies.");
		EnemyInfo ei = new EnemyInfo();
		ei.HP = enemy.HP;
		ei.pos_x = enemy.transform.GetChild(0).position.x;
		ei.pos_y = enemy.transform.GetChild(0).position.z;
		val_to_return_enemy_info = ei;

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetClosestProjectileSub()
	{
		Projectile prj = null;
		float min_dist = 3;
		foreach (Projectile p in Engine.Level_Projectiles) {
			if (p.captured) continue;
			float cur_dist = Vector3.Distance(p.transform.position, bot_obj.transform.position);
			if (cur_dist < min_dist) {
				prj = p;
				min_dist = cur_dist;
			}
		}

		//if (prj == null) Hud.Log("There is no projectiles near me."); //This just spam to log if doing it in loop
		val_to_return_object = prj;

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetFloorSquareSub()
	{
		RaycastHit hit;
		if (Physics.Raycast(bot_obj.transform.position, -bot_obj.transform.up, out hit)) {
			Vector3 sz = hit.collider.bounds.size;
			val_to_return_float = sz.x * sz.z;
		} else {
			val_to_return_float = -1f;
		}

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetTextSub() {
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no text in front of me."; return;
		}

		Collider col = hit.collider;
		Readable r = col.GetComponent<Readable>();
		if (r) {
			val_to_return_string = r.text;
		} else {
			last_error = "There is no text in front of me.";
		}

		Hud.Log("Bot has read: \""+ val_to_return_string + "\"");
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void GetIndicatorStateSub() {
		RaycastHit[] hits = Physics.RaycastAll(bot_obj.transform.position, bot_obj.transform.forward, 1f);
		if (hits.Count() > 0) {
			var led = hits.Where(h=> h.transform.GetComponent<Indicator_Led>() != null).Select(h=> h.transform.GetComponent<Indicator_Led>() ).FirstOrDefault();
			if (led != null) {
				val_to_return_bool = led.GetState();
				if (val_to_return_bool) Hud.Log("Бот узнал что индикатор включён"); else Hud.Log("Бот узнал что индикатор выключен");
			} else {
				last_error = "There is no indicators in front of me."; return;
			}
		} else {
			last_error = "There is no indicators in front of me."; return;
		}
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void SetSwitchStateSub(bool b) {
		RaycastHit[] hits = Physics.RaycastAll(bot_obj.transform.position, bot_obj.transform.forward, 1f);
		if (hits.Count() > 0) {
			var sw = hits.Where(h=> h.transform.GetComponent<Switch>() != null).Select(h=> h.transform.GetComponent<Switch>() ).FirstOrDefault();
			if (sw != null) {
				sw.SetState(b, true);
				if (b) Hud.Log("Бот повернул переключатель в позицию вкл."); else Hud.Log("Бот повернул переключатель в позицию выкл.");
			} else {
				last_error = "There is no switches in front of me."; return;
			}
		} else {
			last_error = "There is no switches in front of me."; return;
		}
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	void CheckPotatesSub() {
		val_to_return_int = 0;

		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, -bot_obj.transform.up);
		if (Physics.Raycast(ray, out hit, 1f)) {
			Collider col = hit.collider;
			Floor_RandomPotatoes p = col.GetComponent<Floor_RandomPotatoes>();
			if (p) { 
				val_to_return_int = p.GetCellPotatoCount();
			}
		}
	
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	//Old Terminal --- obsolete
	// IEnumerator Terminal_ReadSub() {
	// 	RaycastHit hit;
	// 	Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
	// 	if (!Physics.Raycast(ray, out hit, 1f)) {
	// 		last_error = "There is no terminal in front of me."; 
	// 		if (frame_advance) pause = true;
	// 		act_in_process = false; act = null;
	// 		yield break;
	// 	}

	// 	Collider col = hit.collider;
	// 	Terminal t = col.GetComponent<Terminal>();
	// 	if (t) {
	// 		Terminal.terminal_data td = t.Read();
	// 		while (!t.terminal_is_ready) { yield return null; }
	// 		val_to_return_object_sys = td;
	// 	} else {
	// 		//Try Terminal_Mail
	// 		Terminal_Mail tm = col.GetComponent<Terminal_Mail>();
	// 		if (tm) {
	// 			Terminal.terminal_data td = tm.Read();
	// 			while (!tm.terminal_is_ready) { yield return null; }
	// 			val_to_return_object_sys = td;
	// 		} else {
	// 			last_error = "There is no terminal in front of me.";
	// 		}
	// 	}

	// 	if (frame_advance) pause = true;
	// 	act_in_process = false; act = null;
	// }
	// IEnumerator Terminal_AnsewerSub(bool responce, bool is_mail_terminal = false, string[] responce_str = null) {
	// 	RaycastHit hit;
	// 	Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
	// 	if (!Physics.Raycast(ray, out hit, 1f)) {
	// 		last_error = "There is no terminal in front of me."; 
	// 		if (frame_advance) pause = true;
	// 		act_in_process = false; act = null;
	// 		yield break;
	// 	}

	// 	Collider col = hit.collider;
	// 	if (!is_mail_terminal) {
	// 		Terminal t = col.GetComponent<Terminal>();
	// 		if (t) {
	// 			t.Responce(responce);
	// 			while (!t.terminal_is_ready) { yield return null; }
	// 		} else {
	// 			last_error = "There is no terminal in front of me.";
	// 		}
	// 	} else {
	// 		Terminal_Mail t = col.GetComponent<Terminal_Mail>();
	// 		if (t) {
	// 			t.Responce(responce_str);
	// 			while (!t.terminal_is_ready) { yield return null; }
	// 		} else {
	// 			last_error = "There is no terminal in front of me.";
	// 		}
	// 	}


	// 	if (frame_advance) pause = true;
	// 	act_in_process = false; act = null;
	// }
	IEnumerator TerminalU_ReadSub() {
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no terminal in front of me."; 
			if (frame_advance) pause = true;
			act_in_process = false; act = null;
			yield break;
		}

		Collider col = hit.collider;
		Terminal_Universal t = col.GetComponent<Terminal_Universal>();
		if (t) {
			Terminal_Universal.terminal_data td = t.Read();
			if (string.IsNullOrEmpty(t.terminal_error)) { 
				while (!t.terminal_is_ready) { yield return null; }
				val_to_return_object_sys = td;
			} else {
				last_error = t.terminal_error;
				val_to_return_object_sys = null;
			}
		} else {
			last_error = "There is no terminal in front of me.";
		}

		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	IEnumerator TerminalU_AnsewerSub(bool responce, bool is_mail_terminal = false, string[] responce_str = null) {
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no terminal in front of me."; 
			if (frame_advance) pause = true;
			act_in_process = false; act = null;
			yield break;
		}

		Collider col = hit.collider;
		Terminal_Universal t = col.GetComponent<Terminal_Universal>();
		if (t) {
			if (!is_mail_terminal)	t.Responce(responce);
			else 					t.Responce(responce_str);

			if (string.IsNullOrEmpty(t.terminal_error)) {
				while (!t.terminal_is_ready) { yield return null; }
			} else {
				last_error = t.terminal_error;
			}
		} else {
			last_error = "There is no terminal in front of me.";
		}
		
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}
	IEnumerator TerminalU2_AnsewerSub(object responce) {
		RaycastHit hit;
		Ray ray = new Ray(bot_obj.transform.position, bot_obj.transform.forward);
		if (!Physics.Raycast(ray, out hit, 1f)) {
			last_error = "There is no terminal in front of me."; 
			if (frame_advance) pause = true;
			act_in_process = false; act = null;
			yield break;
		}

		Collider col = hit.collider;
		Terminal_Universal t = col.GetComponent<Terminal_Universal>();
		if (t) {
			t.Responce(responce);

			if (string.IsNullOrEmpty(t.terminal_error)) {
				while (!t.terminal_is_ready) { yield return null; }
			} else {
				last_error = t.terminal_error;
			}
		} else {
			last_error = "There is no terminal in front of me.";
		}
		
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	void GetBotPosition()
	{
		val_to_return_vector2 = new Vector2(bot_obj.transform.position.x, bot_obj.transform.position.z);
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	void CheckObstacleSub(Directions dir)
	{
		Vector3 real_dir = Vector3.zero;
		if (dir == Directions.left) real_dir = -bot_obj.transform.right;
		if (dir == Directions.right) real_dir = bot_obj.transform.right;
		if (dir == Directions.forward) real_dir = bot_obj.transform.forward;
		if (dir == Directions.backward) real_dir = -bot_obj.transform.forward;

		val_to_return_bool = Physics.Raycast(bot_obj.transform.position, real_dir, 1f);
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	void CreateShieldSub(Vector3 pos)
	{
		//GameObject shld = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject shld_outer = Instantiate(bot_shield);
		shld_outer.transform.position = pos;
		shld_outer.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		shld_outer.transform.GetChild(0).GetComponent<PSMeshRendererUpdater>().Color = new Color32(181, 80, 75, 255);

		GameObject shld_inner = Instantiate(bot_shield);
		shld_inner.transform.SetParent( shld_outer.transform );
		shld_inner.transform.localPosition = Vector3.zero;
		shld_inner.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
		shld_inner.transform.localScale = new Vector3(0.875f, 0.875f, 0.875f);
		shld_inner.transform.GetChild(0).GetComponent<PSMeshRendererUpdater>().Color = new Color32(134, 232, 255, 255);

		var shld_cmp = shld_outer.AddComponent<BOT_Shield>();

		val_to_return_object = shld_cmp;
		if (frame_advance) pause = true;
		act_in_process = false; act = null;
	}

	void ScaleSub (float value)
	{
		Vector3 bot_default_scale = new Vector3(3f, 3f, 3f);
		Camera.main.DOFieldOfView(30f * value, 3f);
		bot_obj.transform.DOScale( bot_default_scale * value, 1f ).SetEase(Ease.InBounce).OnComplete(()=>{
			if (frame_advance) pause = true;
			act_in_process = false; act = null;
		});
	}

	IEnumerator Pause(int n)
	{
		//Debug.Log("Start pause coroutine");
		Control_UI.enable_frame();

		//Highlight current instruction in script
		is_paused = true;
		int r1 = DebugInfo_Instructions[n].CommandCharStart;
		int r2 = DebugInfo_Instructions[n].CommandCharEnd;
		Text_Editor t = GameObject.Find("Text_Editor").GetComponent<Text_Editor>();
		t.HighlightText(r1, r2);
		
		while (pause_command) {
			yield return null;
		}
		//Debug.Log("Start pause coroutine - unpause");
		Control_UI.disable_frame();

		//Reset highlight current instruction
		t.HighlightText(-1, -1);

		is_paused = false;
		
		if (frame_advance_command) pause_command = true;
		act_in_process = false; act = null;
	}

	public static void Set_Pause(Pause_States p) {
		switch (p) {
			case Pause_States.Reset:
				BOT.key_downs.Clear();
				BOT.frame_advance = false; BOT.pause = false; 
				BOT.frame_advance_command = false; BOT.pause_command = false;
				break;
			case Pause_States.Pause_Command:
				BOT.frame_advance = false; BOT.pause = false;
				BOT.frame_advance_command = false; BOT.pause_command = true;
				break;
			case Pause_States.Pause_Immediate:
				BOT.frame_advance = false; BOT.pause = true;
				BOT.frame_advance_command = false; BOT.pause_command = false;
				break;
			case Pause_States.Advance_Command:
				BOT.frame_advance_command = true; BOT.pause = false;
				if (is_paused) BOT.pause_command = false; else BOT.pause_command = true;
				break;
		}
		//Debug.Log("Set state: " + p.ToString() + ", variables: " + BOT.pause + "," + BOT.pause_command + "," + BOT.frame_advance + "," + BOT.frame_advance_command);
	}

	IEnumerator wait_and_die(float t)
	{
		yield return new WaitForSeconds(t);
		dying_in_progress = false;
		last_error = "BOT is dead. Long live the new BOT!";
	}

	public static void Update()
	{
		//This is executed in MAIN thread
		if (HP <= 0) { 
			if (!dead) {
				dead = true;
				dying_in_progress = true;
				bot_instance.StopAllCoroutines();
				bot_instance.StartCoroutine("wait_and_die", 4f);
			} else
				return;
		}

		if (!act_in_process && act != null)
		{
			act_in_process = true; act.Invoke();
		}

		if (pause && tw_talk != null && tw_talk.IsPlaying())
			tw_talk.Pause();
		if (!pause && tw_talk != null && !tw_talk.IsPlaying())
		{
			tw_talk.Play();
			if (frame_advance) pause = true;
		}
	}
}
public class BOT_Shield : MonoBehaviour, Engine.IDestructible {
	public Projectile captured = null;
	bool release_request = false;

	public delegate void test_deleg();
	public event test_deleg test_event;
	System.Threading.Thread event_req_thread = null;

	Vector3 bounds_size;

	Vector3 target_dest = new Vector3(float.MinValue, float.MinValue, float.MinValue);

	TransformMy tm;
	//GameObject thread_safe_gameobject;

	void Start() {
		//thread_safe_gameobject = gameObject;
		tm = gameObject.transform2();

		float radius = transform.GetChild(1).lossyScale.x;
		float bounds_offset = Mathf.Sin( Mathf.Deg2Rad * 45f ) * radius;
		bounds_size = new Vector3(bounds_offset, bounds_offset, bounds_offset);

		Engine.Level_Destructibles.Add(this);
	}

	void Update() {
		float radius = transform.lossyScale.y / 2;
		RaycastHit hit;
		Ray ray = new Ray(transform.position - new Vector3(0f, radius, 0f), transform.up);
		if (Physics.Raycast(ray, out hit, radius)) {
			Bounds my_bounds = new Bounds(transform.position, bounds_size);
			if (my_bounds.Contains(hit.collider.bounds.min) && my_bounds.Contains(hit.collider.bounds.max)) {
				//Debug.Log("Check inside shield INSIDE, found: " + hit.transform.name);
				var rb = hit.transform.GetComponent<Rigidbody>();
				var pr = hit.transform.GetComponent<Projectile>();
				if (rb != null && pr != null && captured == null) {
					if (rb.velocity.magnitude > 0.1f) {
						rb.velocity = rb.velocity / 4;
					} else {
						captured = pr;
						pr.captured = true;
						pr.speed = Vector3.zero;
						rb.isKinematic = true;
						hit.transform.SetParent(transform);
						hit.transform.DOLocalMove(Vector3.zero, 0.1f).OnComplete(()=>
							hit.transform.DOLocalMoveY(0.12f, 1f).SetLoops(-1, LoopType.Yoyo)
						);
					}
				}

				if (rb == null && pr != null && captured == null) {
					captured = pr;
					pr.captured = true;
					pr.speed = Vector3.zero;
					hit.transform.SetParent(transform);
					hit.transform.DOLocalMove(Vector3.zero, 0.1f).OnComplete(()=>
						hit.transform.DOLocalMoveY(0.12f, 1f).SetLoops(-1, LoopType.Yoyo)
					);
				}
			}
		}

		if (release_request) {
			if (captured != null) {
				captured.transform.SetParent(null, true);
				var rb = captured.GetComponent<Rigidbody>();
				if (rb != null) {
					rb.DOKill();
				} else {
					captured.transform.DOKill();
					rb = captured.gameObject.AddComponent<Rigidbody>();
				}
				var random_force = new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, UnityEngine.Random.Range(0.2f, 0.2f));
				rb.AddForce(random_force, ForceMode.Impulse);
			}
			Engine.Level_Destructibles.Remove(this);
			Destroy(gameObject);
		}
	}

	public void MoveTo(float x, float y, float z) {
		while (tm == null) System.Threading.Thread.Sleep(10);

		tm.MoveTo(x, y, z);
		//target_dest = new Vector3(x, y, z);
		event_req_thread = BOT.script_thread;
		tm.on_move_dest_reached += () => { 
			//Debug.Log("Event is working. Event thread id = " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", BOT thread id = " + BOT.script_thread.ManagedThreadId); 
			//Debug.Log("3");
			BOT.events.Add( new Action(()=> { this.test_event(); }) );
			//BOT.events.Add( new Action(()=> { Debug.Log("aaa"); }) );
			//if (BOT.script_thread != null && BOT.script_thread == event_req_thread && test_event != null) test_event();
		};
	}

	public void Release(){
		//This is called from user-script
		release_request = true;
	}

	public void Destroy(){
		if (captured != null) {
			captured.transform.SetParent(null, true);
			captured.transform.DOKill();
			var rb = captured.GetComponent<Rigidbody>();
			if (rb != null) rb.DOKill();
		}

		if (tm != null) tm.Reset_Event();
		Destroy(gameObject);
	}

	// public TransformMy t{
	// 	get { return thread_safe_gameobject.transform2(); }
	// }
}

namespace BOT_Helpers {
	public class Platform : MonoBehaviour {
		float platform_speed = 1f;
		public static bool wait = false;
		public static Vector3 target_pos = Vector3.zero;

		public static List<GameObject> spawn_gameobjects = new List<GameObject>();
		public static List<ParticleSystem> spawn_particles = new List<ParticleSystem>();

		public static Platform inst = null;
		void Start() { inst = this; }

		public static void MoveTo(int x, int y, int z) {
			BOT.act = new Action(()=> inst.StartCoroutine( inst.MoveTo_Coroutine(x,y,z) ) );
			BOT.Thread_Wait();
		}

		IEnumerator MoveTo_Coroutine(int x, int y, int z) {
			//BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = true;
			//var cl1 = BOT.bot_obj.GetComponent<BoxCollider>(); cl1.enabled = false;
			//var cl2 = BOT.bot_obj.transform.GetChild(3).GetComponent<SphereCollider>(); cl2.enabled = false;
			//yield return new WaitForFixedUpdate();

			RaycastHit hit;
			Ray ray = new Ray(BOT.bot_obj.transform.position, Vector3.down);
			if (!Physics.Raycast(ray, out hit, 1f)) {
				Debug.Log("Raycast - false. Pos-from: " + BOT.bot_obj.transform.position);
				//BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false; cl1.enabled = true; cl2.enabled = true;

				//Ray ray2 = new Ray(BOT.bot_obj.transform.position + (Vector3.down * 2f), Vector3.up);
				//bool test = Physics.Raycast(ray2, out hit, 3f);
				//Debug.Log("Raycast - from bottom = " + test);

				BOT.last_error = "Platform class can not be used if BOT is not standing on the platform"; yield break;
			}
			
			var pm = hit.transform.GetComponent<Platform_Mono>();
			if (pm == null) {
				Debug.Log("Raycast - " + hit.transform.gameObject.name);
				//BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false; cl1.enabled = true; cl2.enabled = true;
				BOT.last_error = "Platform class can not be used if BOT is not standing on the platform"; yield break;
			}

			//cl1.enabled = true; cl2.enabled = true;

			if (x < pm.limit_X.x || x > pm.limit_X.y) { BOT.last_error = "Platform move range X is outside of the limits"; yield break; }
			if (y < pm.limit_Y.x || y > pm.limit_Y.y) { BOT.last_error = "Platform move range Y is outside of the limits"; yield break; }
			if (z < pm.limit_Z.x || z > pm.limit_Z.y) { BOT.last_error = "Platform move range Z is outside of the limits"; yield break; }

			var override_x = pm.override_X.Where(v=> v.x == x); if ( override_x.Count() > 0 ) x = override_x.ElementAt(0).y;
			var override_y = pm.override_Y.Where(v=> v.x == y); if ( override_y.Count() > 0 ) y = override_y.ElementAt(0).y;
			var override_z = pm.override_Z.Where(v=> v.x == z); if ( override_z.Count() > 0 ) z = override_z.ElementAt(0).y;
			target_pos = new Vector3(x,y,z);

			if (pm.OnMoveBegin != null) pm.OnMoveBegin.Invoke();
			while (wait) yield return null;

			var pmt = pm.transform;
			if (pm.particles == null && pm.fx == null) {
				//No particle fx
				BOT.bot_obj.transform.SetParent(pmt, true);
				BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = true;

				pmt.DOMoveX((float)x, platform_speed).SetSpeedBased();
				while ( DOTween.IsTweening(pmt) ) { yield return null; }
				pmt.DOMoveY((float)y, platform_speed).SetSpeedBased();
				while ( DOTween.IsTweening(pmt) ) { yield return null; }
				pmt.DOMoveZ((float)z, platform_speed).SetSpeedBased();
				while ( DOTween.IsTweening(pmt) ) { yield return null; }

				Engine.spawner.transform.position = pmt.position + (Vector3.up * 0.6f);

				BOT.bot_obj.transform.SetParent(null, true);
				DontDestroyOnLoad(BOT.bot_obj);
				//BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
			} else {
				//Particle fx
				BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = true;

				if (pm.anim_chain.Count() > 0) {
					List<GameObject> gameobjects_list = new List<GameObject>();
					List<ParticleSystem> particles_list = new List<ParticleSystem>();
					foreach (var anim in pm.anim_chain) {
						var param_pos = anim.param;
						if (anim.use_offset_of_bot_pos) param_pos += BOT.bot_obj.transform.position;
						switch (anim.animation) {
							case Platform_Mono.animation_info.spawn_particles :
								if (pm.particles != null) {
									var p_go = Instantiate(pm.particles.gameObject, param_pos, Quaternion.Euler(anim.rotation) );
									p_go.SetActive(true);
									var p_ps = p_go.GetComponent<ParticleSystem>(); p_ps.Play();
									spawn_particles.Add(p_ps); particles_list.Add(p_ps);
								} else {
									var go = Instantiate(pm.fx, param_pos, Quaternion.Euler(anim.rotation) );
									go.SetActive(true);
									spawn_gameobjects.Add(go); gameobjects_list.Add(go);
								}
								break;
							case Platform_Mono.animation_info.translate_bot :

								break;
							case Platform_Mono.animation_info.scale_bot :
								if (anim.duration < 0.01f)
									BOT.bot_obj.transform.localScale = param_pos;
								else {
									BOT.bot_obj.transform.DOScale(param_pos, anim.duration).SetDelay(anim.delay);
									if (anim.wait) while ( DOTween.IsTweening(BOT.bot_obj.transform) ) { yield return null; }
								}
								break;
							case Platform_Mono.animation_info.rotate_bot :
								if (anim.duration < 0.01f)
									BOT.bot_obj.transform.rotation = Quaternion.Euler(anim.rotation);
								else {
									BOT.bot_obj.transform.DORotate(anim.rotation, anim.duration).SetDelay(anim.delay);
									if (anim.wait) while ( DOTween.IsTweening(BOT.bot_obj.transform) ) { yield return null; }
								}
								break;
							case Platform_Mono.animation_info.move_platform :
								pmt.transform.position = target_pos;
								BOT.bot_obj.transform.position = target_pos + anim.param;
								BOT.bot_obj.transform.rotation = Quaternion.Euler(anim.rotation);
								break;
							case Platform_Mono.animation_info.delay :
								if (anim.delay <= 0f) yield return null;
								else yield return new WaitForSeconds(anim.delay);
								break;
						}
					}

					foreach (var ps in particles_list) { spawn_particles.Remove(ps); Destroy(ps); }
					foreach (var go in gameobjects_list) { spawn_gameobjects.Remove(go); Destroy(go); }

					BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
					goto Finish;
				}

				//Spawn particles - disappear
				var particle_pos = BOT.bot_obj.transform.position + new Vector3(0f, 0.1f, 0f); // - new Vector3(0f, 0.6f, 0f);
				var particle_rot = Quaternion.Euler(-90f, 0f, 0f);
				var ps_new = Instantiate(pm.particles.gameObject, particle_pos, particle_rot );
				ps_new.SetActive(true); 
				var p = ps_new.GetComponent<ParticleSystem>(); spawn_particles.Add(p);
				//var m = p.main; m.simulationSpeed = 2f; 
				p.Play();
				
				//Scale bot - disappear
				BOT.bot_obj.transform.DOScale(new Vector3(3f, 0f, 3f), 0.5f).SetDelay(0.5f);
				while ( DOTween.IsTweening( BOT.bot_obj.transform ) ) { yield return null; }
				
				////Scale bot - appear
				pmt.transform.position = target_pos;
				BOT.bot_obj.transform.position = new Vector3(x, y + 0.6f, z);
				BOT.bot_obj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
				//BOT.bot_obj.transform.DOScale(new Vector3(3f, 3f, 3f), 0.5f);
				BOT.bot_obj.transform.localScale = new Vector3(0f, 0f, 0f);
				BOT.bot_obj.transform.DOScale(new Vector3(3f, 0.01f, 3f), 0.5f).OnComplete(()=> {
					BOT.bot_obj.transform.DOScale(new Vector3(3f, 3f, 3f), 0.5f);
				});

 				////Spawn particles - appear
				var particle_pos2 = BOT.bot_obj.transform.position + new Vector3(0f, 0.1f, 0f); // - new Vector3(0f, 0.6f, 0f);
				var ps_new2 = Instantiate(pm.particles.gameObject, particle_pos2, particle_rot );
				ps_new2.SetActive(true); 
				var p2 = ps_new2.GetComponent<ParticleSystem>(); spawn_particles.Add(p2);
				//var m2 = p2.main; m2.simulationSpeed = 2f; 
				p2.Play();
				
				//When scaling from 0, bot rotate to random direction
				yield return null;
				BOT.bot_obj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

				//while ( DOTween.IsTweening( BOT.bot_obj.transform ) ) { yield return null; }
				yield return new WaitForSeconds(1.5f);
				spawn_particles.Remove(p); spawn_particles.Remove(p2);
				Destroy(ps_new); Destroy(ps_new2);

				BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
			}

			Finish:
			if (pm.OnMoveEnd != null) pm.OnMoveEnd.Invoke();
			while (wait) yield return null;

			if (BOT.frame_advance) BOT.pause = true;
			BOT.act_in_process = false; BOT.act = null;
		}
	}
	public class Console {
		public static void Log(string s) {
			BOT.act = new Action(()=> { 
				Hud.Log(s);
				if (BOT.frame_advance) BOT.pause = true;
				BOT.act_in_process = false; BOT.act = null;
			});
			BOT.Thread_Wait();
		}
	}
}

public static class ExtensionMethods
{
	// static Dictionary<GameObject, Vector3> moving = new Dictionary<GameObject, Vector3>();

	// public static void Move(this Transform trans, Vector3 speed)
	// {
	// 	moving.Add(trans.gameObject, speed);
	// }

	// public static void Update()
	// {
	// 	foreach (KeyValuePair<GameObject, Vector3> kv in moving) {
	// 		//Debug.Log(kv.Key.name + " " +kv.Value.ToString());
	// 		kv.Key.transform.Translate(kv.Value);
	// 	}
	// }

	public static TransformMy transform2(this GameObject g) {
		return new TransformMy(g);
	}

	public static void TextMeshProHighlight(TMPro.TMP_Text TMP, int first_char, int last_char, Color c)
	{
		//Debug.Log("Highlight r1 = " + first_char.ToString() + ", r2 = " + last_char.ToString());

		TMPro.TMP_TextInfo ti = TMP.textInfo;
		if (last_char < 0) last_char = ti.characterCount - 1;

		for (int i = first_char; i <= last_char; i++){
			if (!ti.characterInfo[i].isVisible) continue;

			int materialIndex = ti.characterInfo[i].materialReferenceIndex;
			var newVertexColors = ti.meshInfo[materialIndex].colors32;
			int vertexIndex = ti.characterInfo[i].vertexIndex;
			newVertexColors[vertexIndex + 0] = c;
            newVertexColors[vertexIndex + 1] = c;
            newVertexColors[vertexIndex + 2] = c;
            newVertexColors[vertexIndex + 3] = c;
		}

		// New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
		TMP.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Colors32);
	}

	public static GameObject[] ObjectsUnderPointer() {
		PointerEventData pointerData = new PointerEventData (EventSystem.current) { pointerId = -1, };
		pointerData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
		return results.Select(rr=> rr.gameObject).ToArray();
	}

	public static int IndexOfBalancedEnd (this string s, char open, char close, int start = 0) {
		int ind = s.IndexOf(open, start);
		if (ind < 0) return ind;

		int cur_level = 1;
		for (int i = ind + 1; i < s.Length; i++) {
			if (s[i] == open) { cur_level++; }
			if (s[i] == close) { cur_level--; }
			if (cur_level == 0) { ind = i; break; }
		}

		// var sw = System.IO.File.AppendText("D:/Unity 2018.1.0f2/Projects/Script-o-bot/IndexOfBalancedEnd.log");
		// sw.WriteLine("cur level = " + cur_level.ToString() + ", ind = " + ind.ToString());
		// sw.Close();

		if (cur_level > 0) return -1;
		return ind;
	}
}

public class TransformMy
{
	//Test:
	//Debug.Log(BOT.bot_obj.transform2().position.x.ToString());
	//BOT.bot_obj.transform2().position = new Vector3(1f, 1f, 1f);

	public delegate void move_dest_reached_deleg();
	public event move_dest_reached_deleg on_move_dest_reached;

	public GameObject obj = null;
	static List<TransformMy> inst = new List<TransformMy>();
	static Dictionary<GameObject, Vector3> moving = new Dictionary<GameObject, Vector3>();

	static bool in_pause_mode = false;

	List<Action> actions = new List<Action>();
	static List<Tweener> tweeners = new List<Tweener>();

	public TransformMy(GameObject g) {
		obj = g; inst.Add(this);
	}

	public Vector3 position {
		get { 
			Vector3 v = Vector3.left;
			actions.Add ( new Action( ()=> v = obj.transform.position) );
			while (actions.Count > 0) System.Threading.Thread.Sleep(10);
			return v;
		}
		set {
			actions.Add ( new Action( ()=> obj.transform.position = value) );
			while (actions.Count > 0) System.Threading.Thread.Sleep(10);
		}
	}

	public void Move( Vector3 speed ) {
		moving.Add(obj, speed);
	}

	public Tweener MoveTo( float x, float y, float z ) {
		//Running in user script thread
		Vector3 dest = new Vector3(x, y, z);
		float distance = Vector3.Distance(position, dest);

		Tweener tw = null;
		actions.Add ( new Action( ()=> {
			tw = obj.transform.DOMove(dest, distance).SetEase(Ease.Linear).OnComplete(()=> {
				lock (tweeners) { tweeners.Remove(tw); }
				if (this.on_move_dest_reached != null) { this.on_move_dest_reached(); }
			});
			tweeners.Add(tw);
		}));

		while (actions.Count > 0) System.Threading.Thread.Sleep(10);
		return tw;
	}

	public static void Update() {
		//Running in unity main thread
		lock (inst) {
			foreach (var i in inst.ToArray()) {
				lock (i.actions) {
					foreach (var a in i.actions) { a.Invoke(); }
					i.actions.Clear();
				}
			}
		}

		//Move
		lock (moving) {
			foreach (KeyValuePair<GameObject, Vector3> kv in moving) {
				kv.Key.transform.Translate(kv.Value);
			}
		}

		//Handle pause
		if ((BOT.pause || BOT.is_paused) && !in_pause_mode) {
			in_pause_mode = true;
			lock (tweeners) { foreach (Tweener t in tweeners) t.Pause(); }
		}
		if (!(BOT.pause || BOT.is_paused) && in_pause_mode) {
			in_pause_mode = false;
			lock (tweeners) { foreach (Tweener t in tweeners) t.Play(); }
		}
	}

	public void Reset_Event() {
		obj.transform.DOKill();
		this.on_move_dest_reached = null;
	}
}
