//Solution to count commands of c# code
//https://stackoverflow.com/questions/5956092/how-to-count-the-number-of-code-lines-in-a-c-sharp-solution-without-comments-an

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

public class Engine : MonoBehaviour {
	public static Engine engine_inst = null;

	static bool check_requirement = false;
	static string requirement_cur = "";
	static string requirement_needed = "";
	public static string requirement_additional_info = "";
	public static int commands_count = -1;

	public static string user_script_dir;

	public AudioClip LevelComplete_Music;

	public ReferenceObjectsStruct Reference_objects;
	public static ReferenceObjectsStruct Ref_objs;
	[System.Serializable]
	public struct ReferenceObjectsStruct {
		public GameObject Destroy_Container_Mesh_Effect;
		public Material Fully_Transparent_Mat;
		public Options options;
	}

	public static int current_scene = 1;

	public static Dictionary<string, GameObject> Level_obj_dict = new Dictionary<string, GameObject>();
	public static List<KeyValuePair<Loot.loot_type_enum, int>> Loot_list = new List<KeyValuePair<Loot.loot_type_enum, int>>();
	public static List<GameObject> Level_Spawnable   = new List<GameObject>();
	public static List<Collector> Level_Collectors   = new List<Collector>();
	public static List<Enemy> Level_Enemies        	 = new List<Enemy>();
	public static List<Readable> Level_Readable      = new List<Readable>();
	public static List<String_Listiner> Level_StringListiners = new List<String_Listiner>();
	public static List<Projectile> Level_Projectiles 		  = new List<Projectile>();
	public static List<ISetable> Level_Setable 	 		      = new List<ISetable>();
	public static List<IResetable> Level_Resetable 	 		  = new List<IResetable>();
	public static List<IDestructible> Level_Destructibles 	  = new List<IDestructible>();
	public static List<GameObject> Objects_To_Destroy 	  	  = new List<GameObject>();
	public interface ISetable { void Set(); }
	public interface IResetable { void Reset(); }
	public interface IDestructible { void Destroy(); }
	public interface IStatableBool { bool GetState(); }

	public static int current_step = -1;

	//Scene Objects
	[HideInInspector] public static Camera_Controller camera_Controller;
	[HideInInspector] public static GameObject bot_base;
	[HideInInspector] public static Desktop_Manager desktop_manager;

	//Script UI
	//InputField ScriptInputField = null;
	static TMPro.TMP_InputField ScriptInputField = null;
	static Text_Editor ScriptEditor_My = null;

	//Dialog UI
	Text DialogText = null;
	Button DialogOK = null;
	[HideInInspector] public static Tutorial_Dialog DialogTutorial = null;
	[HideInInspector] public static Tutorial_Dialog_Important TutorialDialogImportant = null;	
	TextTyper TextTyper = null;
	//Tutorial UI
	Tutorial_Arrow TutorialArrow = null;
	Tutorial_RedPanel TutorialRedPanel = null;
	AudioSource main_audio_source = null;
	AudioSource secondary_audio_source = null;
	AudioSource secondary_audio_source2 = null;
	public string cur_text = "";
	public string cur_text_imp = "";

	public static List<string> Level_names = new List<string>();
	public static List<string> Level_obj_names = new List<string>();
	public static List<int> Level_step_indices = new List<int>();
	public static Transform spawner = null;

	public static Dictionary<Key, KeyCode[][]> Hotkeys = new Dictionary<Key, KeyCode[][]>();
	public static Dictionary<Key, KeyCode[][]> Hotkeys_Defaults = new Dictionary<Key, KeyCode[][]>();
	public static Dictionary<Key, string> Hotkeys_descriptions = new Dictionary<Key, string>();
	public enum Key
	{
		Main_Menu,
		Camera_Left, Camera_Right, Camera_Reset,
		Script_Control_Pause_Immediate,
		Maze_Solver, Maze_Regenerate,
		WindowMode_Toggle, WindowMode_SwitchWindow, WindowMode_WinFullscreenToggle,
		Volume_Up, Volume_Down,
		Set_Breakpoint,
		Intellisense_Up, Intellisense_Down, Intellisense_Submit, Intellisense_Enable,
		Debug_Show_Console,
		Script_Time_x1, Script_Time_x2, Script_Time_x3, Script_Time_x5,
		Debug_Generate_Script
	}

	//Dialog code higlight colors
	public static higlight_colors_info higlight_colors = new higlight_colors_info(true);
	public struct higlight_colors_info {
		public Color nmsp; 				//29CA86
		public Color cls;				//A0A0FF
		public Color method;			//FF0000
		public Color str;				//FFB900
		public Color built_in_types;	//A0A0FF
		public Color comments;			//73BA85
		public Color instruction;		//A0A0FF
		public Color background;		//000000, alpha - 0,627451 (int 160)

		public higlight_colors_info(bool init) {
			nmsp = new Color32(41, 202, 134, 255);
			cls = new Color32(160, 160, 255, 255);
			method = new Color32(255, 0, 0, 255);
			str = new Color32(255, 185, 0, 255);
			built_in_types = new Color32(160, 160, 255, 255);
			comments = new Color32(115, 186, 133, 255);
			instruction = new Color32(160, 160, 255, 255);
			background = new Color32(0, 0, 0, 160);
		}
	}

	// Use this for initialization
	void Start () {
		//Time.timeScale = 5f;

		engine_inst = this;
		Ref_objs = Reference_objects;

		GameObject tmp = GameObject.Find("Script_InputField_TMPRO");
		if (tmp != null) ScriptInputField = tmp.GetComponent<TMPro.TMP_InputField>();
		else ScriptEditor_My = GameObject.Find("Text_Editor").GetComponent<Text_Editor>();
		//ScriptInputField.onValueChanged.AddListener(CodeHighLight);

		DialogText = GameObject.Find("Dialog_Text").GetComponent<Text>();
		DialogOK = GameObject.Find("Dialog_OK_Button").GetComponent<Button>();
		DialogTutorial = GameObject.Find("Panel_Dialog").GetComponent<Tutorial_Dialog>();
		
		//TypingMy = GameObject.Find("Panel_Dialog").GetComponent<TypingMy>();
		TextTyper = GameObject.Find("Dialog_Text").GetComponent<TextTyper>();
		DialogOK.gameObject.SetActive(false);
		DialogOK.onClick.AddListener(DoNextStep);
		TextTyper.PrintCompleted.AddListener(() => { DialogOK.gameObject.SetActive(true); });
		
		TutorialArrow = GameObject.Find("Tutorial_Arrow").GetComponent<Tutorial_Arrow>();
		TutorialRedPanel = GameObject.Find("Tutorial_RedPanel").GetComponent<Tutorial_RedPanel>();
		TutorialDialogImportant = GameObject.Find("Panel_Dialog_Important").GetComponent<Tutorial_Dialog_Important>();
		TutorialArrow.DeActivate(true);
		TutorialRedPanel.DeActivate(true);

		camera_Controller = GameObject.Find("CameraController").GetComponent<Camera_Controller>();
		desktop_manager = GetComponent<Desktop_Manager>();

		main_audio_source = Camera.main.GetComponent<AudioSource>();
		secondary_audio_source = Camera.main.GetComponents<AudioSource>()[1];
		secondary_audio_source2 = Camera.main.GetComponents<AudioSource>()[2];

		bot_base = GameObject.Find("Robot Base");
		bot_base.SetActive(false);
		
		DontDestroyOnLoad(bot_base);
		DontDestroyOnLoad(camera_Controller.camera_main.gameObject);
		DontDestroyOnLoad(camera_Controller.camera_orbit.gameObject);
		DontDestroyOnLoad(camera_Controller.gameObject);
		DontDestroyOnLoad(gameObject); //me, engine and compiler scripts
		DontDestroyOnLoad(TutorialArrow.transform.parent.gameObject); //Canvas
		DontDestroyOnLoad(GameObject.Find("Robot"));
		DontDestroyOnLoad(desktop_manager.BigEdit_Cam);
		DontDestroyOnLoad(UnityEngine.EventSystems.EventSystem.current.gameObject);
		if (ScriptEditor_My != null) DontDestroyOnLoad(ScriptEditor_My.transform.GetComponentInParent<Canvas>().gameObject);

		user_script_dir = System.IO.Path.GetFullPath(Application.dataPath + "/../UserScripts");
		if (!System.IO.Directory.Exists(user_script_dir)) System.IO.Directory.CreateDirectory(user_script_dir);

		//Level_step_indices.Add(0);
		list_scene_level_objects();

		//float line_offset_arr = 12f;
		//float line_offset_mul = 24f + 8;
		//float line_offset_half = 4f;
		//float red_square_x = -55f;
		//float red_square_w = 455f;
		if (ScriptEditor_My != null) {
			float cnv_scale_me = TutorialArrow.transform.GetComponentInParent<Canvas>().scaleFactor;
			float cnv_scale_scr = ScriptEditor_My.transform.GetComponentInParent<Canvas>().scaleFactor;
			//line_offset_arr = 0f;
			//line_offset_mul = (ScriptEditor_My.line_height * cnv_scale_scr) / cnv_scale_me;
			//line_offset_half = line_offset_mul;
			//red_square_x = -90f; red_square_w = 468f;
		}
		//Debug.Log("Line height = " + ScriptEditor_My.line_height + ", in big canvas space = " + line_offset_mul);

		Phrase_Generator.init();
		Scenario.Init();

		#region levels_text_data - OBSOLETE
		// //Level 1-1
		// Step_Add_SetScript ("");
		// Step_Add_Dialog ("Привет, меня зовут <b>ХУЙНЯ</b>. И я, сцуко, робот.");
		// Step_Add_Dialog ("Тяжело быть роботом - я ничего не умею делать, пока мне не прикажут...");
		// Step_Add_Dialog ("Разговаривать со мной надо определённым образом, но всё по порядку.");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-560, GetLinePositionTMPro(1) - line_offset_arr); //Top - Padding - LineHeight - LineSpace - HalfLineHeight
		// Step_Add_Dialog ("У меня есть функция Start, она вызывается сразу при моём запуске.");
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(3), red_square_w, line_offset_mul * 4 - line_offset_half);
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\\\\All code here will be executed\n\\\\  when you press Play\n\n}");
		// Step_Add_Dialog ("Это значит, что весь код находящийся в теле функции Start - т.е. между\nфигурными скобками { и } - выполнится сразу после нажатия кнопки \"Play\".");
		// Step_Add_Tutorial_Arrow_Hide ();
		// Step_Add_Tutorial_RedPanel_Hide ();
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>");
		// Step_Add_Dialog ("В коде, ко мне надо обращатся через переменную BOT. Это объектная переменная, т.е. она содержит ссылку на класс... Бла-бла-бла... забудь.\nПока достаточно знать что в переменной BOT сижу я сам.");
		// Step_Add_Dialog ("ВОТ, это в смысле <b>БОТ</b>, то есть я. А не <b>ВОТ</b> в смысле \"вот\". То есть \"БОТ\" английскими буквами, а не \"Вот, нате вам креньдельки под язык, да куличики в ноздри\".\nПонятно? Нет? Ну и пофиг вообще...");
		// Step_Add_Dialog ("У меня много разных методов и свойств: я умею двигаться, подбирать лут, драться, летать, хуярить кувалдой по стенам, крафтить туалетную бумагу и даже... э-эээ...\nДавай не будем пока углублятся.");
		// Step_Add_Dialog ("Методы - это команды мне, заставляющие меня что-то сделать.");
		// Step_Add_Dialog ("А свойства... Да хрен с ними со свойствами. Потом, как ни будь...");
		// Step_Add_Dialog ("Попробуем что ни будь не сложное.\nУ меня есть метод Say, он заставляет меня говорить.\n");
		// Step_Add_Dialog_Important_Continue (".<color=\"red\">Say</color>");
		// Step_Add_Dialog_Continue ("Что бы вызвать метод, нужно написать мою переменную, затем точку и наконец имя метода.");
		// Step_Add_Dialog ("У этого метода есть один обязательный параметр - собственно текст, который я должен сказать.\n");
		// Step_Add_Dialog_Important_Continue ("(<color=\"#C3915Bff\">\"Привет мир, я Хуйня!\"</color>)"); //color: 195 145 91; Hex: C3915B
		// Step_Add_Dialog_Continue ("Параметры указываются в скобках после имени метода.\nНу, и потому что это текст - он должен быть в кавычках, иначе я не пойму - что я должен интерпретировать как часть кода, а что как текст.");
		// Step_Add_Dialog_Important_Continue (";");
		// Step_Add_Dialog ("Наконец, после каждой команды, должна стоять точка с запятой. Получается как-то так.");
		// Step_Add_Dialog ("К сожалению, я тупой, и регистр букв имеет значение. Т.е. если написать say маленькими буквами, или большими (SAY) - я не пойму.\n");
		// Step_Add_Dialog_Continue ("Всё должно быть написано именно так, как задумано изначально. В данном случае - с большой буквы: Say.");
		// Step_Add_Dialog ("А теперь, заставьте меня сказать \"Привет\", или что-то типа того...");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("SAY");
		// Step_Add_LevelComplete("Level 1-1 - Методы.", "Level 001-01");

		// //Level 1-2
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Set_Blocked_Instructions ("move(x);rotate(x);");
		// Step_Add_Dialog_Show ();
		// Step_Add_Dialog_Important_Hide ();
		// Step_Add_ScriptControls_Hide ();
		// Step_Add_Dialog ("Хорошо, продолжим.");
		// Step_Add_Dialog ("Итак, здесь мы видим маленький красненький квадратик.");
		// Step_Add_Dialog ("Вообще, когда мы видим маленький красненький квадратик - это значит что мне надо попасть туда.");
		// Step_Add_Dialog ("Я хз зачем - просто надо. Возможно, в детстве, мне не хватало маленьких красненьких квадратиков... Не знаю. Потом разберёмся.");
		// Step_Add_Dialog ("Для этого мне надо двигаться. (привет, кэп).");
		// Step_Add_Dialog ("Давай посмотрим, какие методы у меня, как у класса, есть для передвижения...\nDance(), DoCoffee, DestroyAllHumans()... не то... DesintegrateWorld(), MakeSandwitch()... какой же я класный, столько всего умею!");
		// Step_Add_Dialog ("Вот, нашёл.\n");
		// Step_Add_Dialog_Continue ("Метод Move().");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Move</color>();");
		// Step_Add_Dialog ("У этого метода нет параметров, которые надо указывать в скобках. Но сами скобки всё равно обязательны - просто для того, что бы я понял что это вызов метода.");
		// Step_Add_Dialog ("Попробуй передвинуть меня.");
		// Step_Add_Dialog_Hide ();
		// Step_Add_ScriptControls_Show ();
		// Step_Add_Check_Requirement ("BOT.Position.x > 1.5");
		// Step_Add_ScriptControls_Hide ();
		// Step_Add_Pause ();
		// Step_Add_Dialog_Show ();
		// Step_Add_Dialog ("Сука, не туда-а-а-а....");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Resume ();
		// Step_Add_Check_Requirement ("BOT.Position.y < -5");
		// Step_Add_ScriptControls_Show ();
		// Step_Add_Dialog ("Ладно, вырубай уже. Всё, я упал - кина не будет."); //!!!Clicking this panel while in next requirement step casuse skip event.
		// Step_Add_Check_Requirement ("ScriptStopped");
		// Step_Add_Dialog ("Нехорошо вышло :(\n");
		// Step_Add_Dialog_Continue ("Очевидно метод Move() двигает меня вперёд. Но сейчас я стою, направленынй в другую сторону.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Rotate</color>(n);");
		// Step_Add_Dialog ("Попробуй перед методом Move() вызвать метод Rotate(n), где n - это число, в градусах, на которое я должен повернуться.");
		// Step_Add_Dialog ("Если число будет положительное - я повернусь по часовой стрелке, если отрицательное - то против часовой стрелки.\n");
		// Step_Add_Dialog_Continue ("И да, поскольку это число, а не текст, то его не нужно брать в кавычки - и так сойдёт.");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Set_Blocked_Instructions ("move(x);");
		// Step_Add_Check_Requirement ("BOT.Position.z > 0.5");
		// Step_Add_Pause ();
		// Step_Add_ScriptControls_Hide ();
		// Step_Add_Dialog ("О да!\nМы почти на месте!");
		// Step_Add_Resume ();
		// Step_Add_Check_Requirement ("BOT.Position.z > 1.5");
		// Step_Add_Dialog ("Бля...\n");
		// Step_Add_Check_Requirement ("BOT.Position.z > 4");
		// Step_Add_ScriptControls_Show ();		
		// Step_Add_Dialog_Continue ("Всё, вырубай.");
		// Step_Add_Check_Requirement ("ScriptStopped");
		// Step_Add_Dialog ("Знаешь, похоже метод Move() в этом виде бесполезен.\nЯ просто пру вперёд, как танк, и в какой то момент сорвусь с площадки или воткнусь в стену.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Move</color>(d);");
		// Step_Add_Dialog ("Я тут покапался в себе... вобщем, у метода Move() всё таки есть параметр - количество метров, на которое я должен продвинутся.");
		// Step_Add_Dialog ("Это то что нужно. Я этот параметр не заметил, потому что он опционалаен.\n");
		// Step_Add_Dialog_Continue ("Ну да ладно.");
		// Step_Add_Dialog ("Доведи меня уже до этого грёбанного квадрата, и пойдём дальше.");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Set_Blocked_Instructions ("");
		// Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.z = 1.5");
		// Step_Add_Skip_Bot_Rebase_Anim ();
		// Step_Add_Dialog ("Yeah!");
		// Step_Add_LevelComplete("Level 1-2 - Больше методов.", "Level 001-02");

		// //Level 1-3 - variables 1
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Хорошо, с методами вроде бы разобрались. Давай попробуем переменные.");
		// Step_Add_Dialog ("Переменные - это способ заставить меня чего-то запомнить. И потом вспомнить.\n");
		// Step_Add_Dialog ("Переменные - это ячейки памяти в моей башке. В них можно хранить числа, текст, всякое...\n");
		// Step_Add_Dialog ("Сначала, чтобы я знал, что ты хочешь использовать переменную, её нужно объявить.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> ");
		// Step_Add_Dialog ("Делается это просто - сначала пишем тип данных переменной. Начнём с int - это тип \"целое число\".\n");
		// Step_Add_Dialog_Important_Continue("x");
		// Step_Add_Dialog_Continue ("И имя переменной.\n");
		// Step_Add_Dialog_Continue ("В данном случае \"x\" - это имя переменной (прямо как в школе учили), но оно может быть любым, только без пробелов и символов. (знаки подчёркивания использовать можно). \n");
		// Step_Add_Dialog_Continue ("Однако, не стоит давать переменным невменяемые имена типа qwerty или kljsdfgi - просто потому, что потом невозможно понять для чего они нужны, что вообще тут делают и в чём смысл бытия...\n");
		// Step_Add_Dialog_Important_Continue(";");
		// Step_Add_Dialog_Continue ("Не забываем точку с запятой в конце.");
		// Step_Add_Dialog ("С этого момента, я знаю, что у меня есть переменная типа int (целое число), под названием \"x\".");
		// Step_Add_Dialog ("Попробуем выполнить задачку:\n");
		// Step_Add_Dialog_Continue ("Видишь, вот эту крутящуюся штуку?\n");
		// Step_Add_Dialog_Continue ("Это контейнер квантовой энергии.\n");
		// Step_Add_Dialog_Continue ("Из-за квантовой природы, количество энергии в контейнере не предопределено, и зависит от того, в какой момент он будет собран.\n");
		// Step_Add_Dialog_Continue ("Может быть 0, может 10000");
		// Step_Add_Dialog_Continue (", а может, мёртвый кот - как повезёт.");
		// Step_Add_Dialog ("У меня есть функция PickUp(), которая заставляет меня подобрать контейнер, с клетки, перед которой я нахожусь.\n");
		// Step_Add_Dialog_Continue ("Это не метод, а именно функция.\n");
		// Step_Add_Dialog_Continue ("Разница между методом и функцией в том, что функция возвращает какое-то значение, которое можно, например, запихнуть в переменную.\n");
		// //Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> x; x = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PickUp</color>();");
		// Step_Add_Dialog_Important_Continue ("\nx = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PickUp</color>();");
		// Step_Add_Dialog_Continue ("Примерно вот так.");
		// Step_Add_Dialog ("Функция PickUp() не имеет параметров (да, да, в этот раз точно, я проверил).\n");
		// Step_Add_Dialog_Continue ("И возвращает она количество подобранной энергии.\n");
		// Step_Add_Dialog_Continue ("Т.е. после этого наша переменная \"х\" станет равна количеству собранной энергии.\n");
		// Step_Add_Dialog_Important_Continue ("\n\n<color=\"#73BA85ff\">//Тоже самое, но короче</color>\n<color=\"#A0A0ffff\">int</color> x = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PickUp</color>();");
		// Step_Add_Dialog_Continue ("Можно использовать и краткую форму присвоения. Она делает всё то же самое, но писать чуть меньше.\n");
		// Step_Add_Dialog_Continue ("Главное, после объявления переменной, не забывать чего-то в неё запхнуть. Пустые переменные никому не нужны, и вызывают критические ошибки при попытке их использования.");
		// Step_Add_Dialog ("Вон та штука, в конце пути - это весы. На них нужно положить ровно столько энергии, сколько было подобрано из контейнера.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PutEnergy</color>(x);");
		// Step_Add_Dialog ("Положить энергию на весы можно с помощью метода PutEnergy(x), где \"x\" - это количество енергии, которое нужно положить.\n");
		// Step_Add_Dialog_Continue ("Не забудь, что положить энергию я могу только на весы, стоящие на клетке, прямо передо мной.");
		// Step_Add_Dialog ("Ну что ж, попробуем...");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("Collectors");
		// Step_Add_LevelComplete("Level 1-3a - Переменные 1.", "Level 001-03a");


		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Усложним задачу.\n");
		// Step_Add_Dialog_Continue ("ыыыыыы :)))\n");
		// Step_Add_Dialog ("Теперь перед нами два контейнера. Работают они так же, как и в предыдущем задании.\n");
		// Step_Add_Dialog_Continue ("С весами та же история.\n");
		// Step_Add_Dialog_Continue ("Но, теперь, нам необходимо положить на весы сумму энергии, собранной с обоих контейнеров.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> a = 3 + 7 * 5;");
		// Step_Add_Dialog ("С переменными можно производить всякие арифметические действия,\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">int</color> b = 2 + a + 5;");
		// Step_Add_Dialog_Continue ("в которых могут учавствовать и другие переменные.\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">int</color> x = a + b;");
		// Step_Add_Dialog_Continue ("Эти действия, можно производить как при присвоении,\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PutEnergy</color>(a + b);");
		// Step_Add_Dialog_Continue ("так и сразу внутри параметра функции или метода.");
		// Step_Add_Dialog ("Со всеми этими возможностями, посчитать сумму взятой энергии не должно быть большой проблемой.\n");
		// Step_Add_Dialog_Continue ("Я верю, что ты справишся :).");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("Collectors");
		// Step_Add_LevelComplete("Level 1-3b - Переменные 2.", "Level 001-03b");

		// /*
		// //Level 1-3 - old
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Хорошо, с методами вроде бы разобрались. Давай попробуем переменные.");
		// Step_Add_Dialog ("Переменные - это способ заставить меня чего-то запомнить. И потом вспомнить.\n");
		// Step_Add_Dialog_Continue ("Примерно как кнопка \"М\" на калькуляторе - она всегда есть, и никто толком не знает как ей пользоваться.\n");
		// Step_Add_Dialog_Continue ("Но у меня всё проще. ");
		// Step_Add_Dialog ("Переменные - это ячейки памяти в моей башке. В них можно хранить числа, текст, всякое...\n");
		// Step_Add_Dialog_Continue ("Переменные также могут хранить классы (т.е. объекты). К примеру \"BOT\", это объектная переменная, которая содержит ссылку на класс - меня. Но об этом позже...");
		// Step_Add_Dialog ("Зачем переменные нужны, будет понятно в процессе, но если в кратце, то есть три основных сценария.");
		// Step_Add_Dialog ("<b>Во первых</b> - хранение промежуточных результатов.\n");
		// Step_Add_Dialog_Continue ("Представь, что у тебя есть два супер кислотных булыжника, которые прожигают всё, к чему прикоснутся, и по этому стоят на специальных противокислотных подставках.\n");
		// Step_Add_Dialog_Continue ("И ещё у тебя есть \"Супер Клешня Манипулятор™ 3000 с Антикислотным Покрытием класса 7 для переноски супер кислотных булыжников\".\n");
		// Step_Add_Dialog_Continue ("Такая клешня у тебя одна, и может схапать только один булыжник.");
		// Step_Add_Dialog ("А тебе, нужно сука их на подставках ПОМЕНЯТЬ МЕСТАМИ.\n");
		// Step_Add_Dialog_Continue ("Проблему решится сама собой, если ты достанешь ещё одну пустую подставку.\n");
		// Step_Add_Dialog_Continue ("Эта подставка - это и будет переменная, для хранения промежуточных значений...");
		// Step_Add_Dialog ("<b>Во вторых</b> - кэширование данных.\n");
		// Step_Add_Dialog_Continue ("Если мне нужно будет, например, мяукать каждые пол-секунды, то будет глупо каждые пол-секунды перечитывать .ini файл, что бы узнать путь где лежит myaou.wav и с какой громкостью я должен его проиграть.\n");
		// Step_Add_Dialog ("Прочитать файл - это, кончено, довольно быстро, но я буду постоянно хрустеть хардом.\n");
		// Step_Add_Dialog_Continue ("И путь к файлу, и громкость, да и сам myaou.wav, если уж на то пошло, по хорошему нужно прочитать один раз и сохранить эти данные в переменных.\n");
		// Step_Add_Dialog_Continue ("Потом их можно будет доставать в нужный момент, без нагрузки диска, хоть каждую сотую секунды, хоть чаще.");
		// Step_Add_Dialog ("Ну и <b>в третьих</b> - упрощение понимания кода.\n");
		// Step_Add_Dialog_Continue ("TODO");
		// Step_Add_Dialog ("На практике всё это даже проще.");
		// Step_Add_Dialog ("Сначала, что бы я знал, что ты хочешь использовать переменную, её нужно сначала объявить.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> ");
		// Step_Add_Dialog_Continue ("Делается это просто - пишем тип данных переменной, ");
		// Step_Add_Dialog_Important_Continue("kakaya_to_peremennaya");
		// Step_Add_Dialog_Continue ("и имя переменной. ");
		// Step_Add_Dialog_Important_Continue(";");
		// Step_Add_Dialog_Continue ("Незабываем точку с запятой в конце.\n");
		// Step_Add_Dialog_Continue ("Собственно всё...\n");
		// Step_Add_Dialog_Continue ("С этого момента, я знаю, что у меня есть переменная типа int (целое число), под названием 'kakaya_to_peremennaya'.");
		// Step_Add_Dialog ("Имя переменной может быть любым. Можно даже (чур меня), использовать кириллицу!\n");
		// Step_Add_Dialog_Continue ("(Упаси Бог тебя так делать, но чисто технически это можно.)");
		// Step_Add_Dialog ("В именах переменных не должно быть прбелов и символов, кроме знака подчёркивания.\n");
		// Step_Add_Dialog_Continue ("Так же не стоит давать переменным имена типа qwerty или kljsdfgi - просто потому, что потом невозможно понять для чего они нужны, что вообще тут делают и в чём смысл бытия...");
		// Step_Add_Dialog ("А типы данных, их довольно много, но если обобщить, то все их можно условно разделить на 3 категории: циферки, буковки и прочая хрень.\n");
		// Step_Add_Dialog_Continue ("Я разблокирую для начала три типа, по одному из каждой категории:");
		// Step_Add_Dialog ("Тип 1: int - целые числа (это сокращение от integer, если что)\n");
		// Step_Add_Dialog_Continue ("Тип 2: string - текст. Не забываем, что весь текст в коде всегда берёться в кавычки, иначе я буду думать что это часть кода.\n");
		// Step_Add_Dialog_Continue ("и\nТип 3: LogarythmicQuaternionVectorARC-SinusHYPERSpeed_OVERDRIVE_MEGA _Pack_deluxe_collection-second_edition");
		// Step_Add_Dialog ("Ладно, шучу.\n");
		// Step_Add_Dialog_Continue ("Пусть будет:\nТип 3: bool - булева переменная. Она может быть только true или false. Иногда пригодждается...");
		// Step_Add_Dialog ("Пока тебе нужен будет только первый тип - int, целые числа. Остальные отложим на неопределённый срок.");
		// Step_Add_Dialog ("Так к чему я всё это, собственно, веду.\n");
		// Step_Add_Dialog_Continue ("Видишь вот эти две штуки? ");
		// Step_Add_Dialog_Continue ("Это контейнеры квантовой энергии.\n");
		// Step_Add_Dialog_Continue ("Из-за квантовой природы, количество энергии в контейнере не предопределено, и зависит от того, в какой момент они будут собраны.\n");
		// Step_Add_Dialog_Continue ("Может быть 0, может 10000");
		// Step_Add_Dialog_Continue (", а может мёртвый кот - как повезёт.");
		// Step_Add_Dialog ("У меня есть функция PickUp(), которая заставляет меня подобрать контейнер, с клетки, перед которой я нахожусь.\n");
		// Step_Add_Dialog_Continue ("Это не метод, а именно функция.\n");
		// Step_Add_Dialog_Continue ("Разница между методом и функцией в том, что функция возвращает какое-то значение, которое можно, например, запихнуть в переменную.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> i;\ni = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PickUp</color>();");
		// Step_Add_Dialog_Continue ("Примерно вот так.");
		// Step_Add_Dialog ("Функция PickUp() не имеет параметров (да, да, в этот раз точно, я проверил).\n");
		// Step_Add_Dialog_Continue ("И возвращает она, количество подобранного лута. В данном случае это будет количество подобранной энергии.");
		// Step_Add_Dialog ("Вон та штука, в конце пути - это весы. На них нужно положить ровно столько энергии, сколько в сумме было подобрано из двух контейнеров.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">PutEnergy</color>(n);");
		// Step_Add_Dialog ("Положить енергию на весы можно с помощью метода PutEnergy(n), где n - это количество енергии, которое нужно положить.");
		// Step_Add_Dialog ("Ну что ж, попробуй...");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("Collectors");
		// Step_Add_LevelComplete("Level 1-3 - Переменные.", "Level 001-03");
		// */

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Это - лава.\n");
		// Step_Add_Dialog_Continue ("Для справки - роботам нельзя в лаву.\n");
		// Step_Add_Dialog ("Это стало известно в 1991 году, когда Джеймс Кэмерон, в одном из своих фильмов, безжалостно утопил двух роботов в лаве.\n");
		// Step_Add_Dialog_Continue ("Один из них довольно долго агонизировал. Мучительная смерть... ужас...\n");
		// Step_Add_Dialog ("Этот случай тогда вызвал сильный резонанс, среди робонаселения.\n");
		// Step_Add_Dialog_Continue ("Роботы объявляли голодовку, устраивали забастовки, ходили на митинги, в одиночные и массовые пикеты, байкотировали доставку угля...\n");
		// Step_Add_Dialog_Continue ("Но никто тогда не придал этому значения.\n");
		// Step_Add_Dialog_Continue ("То ли потому что всем плевать на роботов, то ли потому что роботов тогда не было.\n");
		// Step_Add_Dialog ("У нас есть мигающий красный квадратик, и, очевидно, мне надо на него попасть.\n");
		// Step_Add_Dialog_Continue ("Потому что... ");
		// Step_Add_Dialog_Continue ("Ой, да ладно! Я уже говорил: одно наличие красного мигающего квадратика уже является причиной необходимости на него попасть.\n");
		// Step_Add_Dialog ("Что тут можно сделать?\n");
		// Step_Add_Dialog_Continue ("Длинна пути впереди меня известна - 8.86 метров.\n");
		// Step_Add_Dialog_Continue ("И у меня есть функция, которая возвращает <i>площадь</i> поверхности, на которой я стою.\n");
		// Step_Add_Dialog_Continue ("(В данном случае, эта площадь включает в себя <b>внутренний бассейн</b> с лавой, но и так сойдёт.)\n");
		// Step_Add_Dialog ("Что бы узнать длинну второй части пути, достаточно площадь разделить на 8.86. Но это всё дробные числа и в тип int они не влезут.\n");
		// Step_Add_Dialog ("Для дробных чисел есть типы float и decimal.\n");
		// Step_Add_Dialog_Continue ("На самом деле есть ещё double, но он от float отличается только точностью - float может хранить число, с точностью примерно до 8и знаков после запятой, а double примерно до 15и.\n");
		// Step_Add_Dialog_Continue ("Всё это можно посмотреть в бот-о-педии, в разделе \"Типы Данных\".\n");
		// Step_Add_Dialog ("float от decimal отличается довольно значительно.\n");
		// Step_Add_Dialog_Continue ("decimal может иметь аж 28 знаков после запятой, но это так, для справки. Самое главное не в этом.\n");
		// Step_Add_Dialog ("float хранится в памяти компьютера в двоичной системе счисления, и в этой системе, некоторые, вполне обычные десятичные дроби типа 1.1 или 1.3 можно записать только в виде безконечных дробей.\n");
		// Step_Add_Dialog_Continue ("Ну, типа как 1/3 можно записать только как 0.3333333333...\n");
		// Step_Add_Dialog ("Вы, люди, считаете что 1 - (0.1 * 10) - это 0?\n");
		// Step_Add_Dialog_Continue ("Какие же вы всё таки наивные...\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n  BOT.Say(1 - 0.1f * 10);\n\n}");
		// Step_Add_Dialog_Continue ("Запустите пример, что бы узнать правду. Но знайте - мир никогда больше не будет прежним.\n");
		// Step_Add_Dialog ("Работая с float мы часто имеем такую ошибку округления. Довольно противная штука... \n");
		// Step_Add_Dialog_Continue ("По этому сравнивать флоаты между собой тоже плохая идея - они могут оказатся <i>почти</i> равными, но этого не достаточно, что бы сравнени сработало.\n");
		// Step_Add_Dialog ("Тип decimal лишён проблем округления, но занимает больше памяти, и вся математика с этим типом происходит гораздо медленнее.\n");
		// Step_Add_Dialog_Continue ("Разумеется, если нужно произвести одно, или даже тысячу действий - то разница в скорости будет на столько мала, что её не измерить никаким секундомером.\n");
		// Step_Add_Dialog_Continue ("Скорость float-а проявит себя только если счёт идёт на миллионы, и сотни миллионов действий.\n");
		// Step_Add_Dialog_Continue ("Да и тогда, на хороших процессорах разница едва заметна.\n");
		// Step_Add_Dialog ("С памятью примерно то же самое: переменная типа float занимает 4 байта, а типа decimal - 16 байт.\n");
		// Step_Add_Dialog_Continue ("Формально - это аж в четыре раза больше.\n");
		// Step_Add_Dialog_Continue ("На практике - это критично только если у вас массив из 1000+ элементов.\n");
		// Step_Add_Dialog_Continue ("Если вам нужна ОДНА, или даже десять переменных - да всем плевать.\n");
		// Step_Add_Dialog ("Возвращаемся к красненькому квадратику. <i>ммм... моя прелесть...</i>\n");
		// Step_Add_Dialog ("Моя функция GetFloorSquare() возвращает площадь поверхности на которой я нахожусь.\n");
		// Step_Add_Dialog_Continue ("И это float.\n");
		// Step_Add_Dialog_Continue ("За что я мне ужасно стыдно, кстати.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">float</color> f;");
		// Step_Add_Dialog ("Переменные объявляются по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		// Step_Add_Dialog_Important_Continue ("\nf = 3.7f;");
		// Step_Add_Dialog_Continue ("Константы float должны иметь суффикс 'f', что бы быть распознаными как float.\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">decimal</color> d = 3.7m;");
		// Step_Add_Dialog_Continue ("Константы decimal - суффикс 'm'.\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">double</color> d2 = 3.7;");
		// Step_Add_Dialog_Continue ("Все дробные константы без суффикса будут распознаны как тип double. \n");
		// Step_Add_Dialog ("Кстати в суффиксах, в зависимости от вашего мировоззрения и религиозных убеждений, можно использовать как строчные так и прописные буквы - компилятор достаточно толерантен, и политкорректен, и не будет против.\n");
		// Step_Add_Dialog ("Результатом деления (а так же сложения, вычитания и умножения) float на float - конечно же будет float.\n");
		// Step_Add_Dialog_Continue ("Вообще, если в какой то операции оба операнда одного типа, то результат будет того же типа что и операнды.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">int</color> a = 10 / 3; <color=\"#73BA85ff\">//WTF???</color>");
		// Step_Add_Dialog_Continue ("По этому осторожнее с делением целых чисел: 10 / 3 будет равно 3 а не 3.3333, как всем бы того хотелось.\nПо тому, что оба числа распознаны как int, соответственно результат - тоже int, т.е. целое число.");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">float</color> a = 10f / 3f;\n<color=\"#A0A0ffff\">double</color> a = 10.0 / 3.0;");
		// Step_Add_Dialog ("Что бы это пофиксить - юзаем суффиксы: 10f / 3f будут распознаны как float-ы, и дадут верный результат. А 10.0 / 3.0 будут распознаны как double, и тоже дадут верный результат.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">float</color> a = 10f / 3;\n<color=\"#A0A0ffff\">double</color> a = 10.0 / 3;\n<color=\"#A0A0ffff\">double</color> a = 10.0 / 3f;");
		// Step_Add_Dialog ("Если операнды разных типов, и один из типов может безопасно хранить в себе второй - то операнд нижестоящего типа будет преобразован в вышестоящий, по цепочке int -> float -> double.\n");
		// Step_Add_Dialog_Important ("10m / 3f; <color=\"#73BA85ff\">//Compilation error</color>");
		// Step_Add_Dialog_Continue ("float/double не могут хранить в себе decimal и наоборот, по этому использование float/double и decimal в одном выражении не допускается. Один из типов нужно преобразовывать в ручную, но об этом как ни будь потом.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Move</color>(1);\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Move</color>(1.3f);\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Move</color>();");
		// Step_Add_Dialog ("Ну и о главном - мой метод Move() перегружен, т.е. может принимать в качестве аргумента как int так и float. Или вообще не иметь аргументов, в результате чего, как мы помним, я уезжаю в бесконечность.\n");
		// Step_Add_Dialog_Continue ("Если с int я езжу строго по квадратикам, то с float я могу остановится где захочу. Полная свобода!\n");
		// Step_Add_Dialog_Continue ("Этой информации должно быть достаточно, что бы довести меня уже до того замечательного квадратика.\n");
		// Step_Add_Dialog ("Не забудьте:\n- Когда скрипт завершится, мне нужно стоять точно на квадратике. Погрешность больше 0.01 не принимается. А значит перебором решить проблему не удастся.\n- что бы доехать до конца пути, мне нужно ехать <b>на метр меньше</b>, чем длинна пути, т.к. один метр занимаю я сам.");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = -3.93 && BOT.Position.z = 3.36");
		// Step_Add_LevelComplete("Level 1-3c - Переменные 3 - float.", "Level 001-03c");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Опять маленький квадратик. ");
		// Step_Add_Dialog_Continue ("Ну хоть лавы нет.\n");
		// Step_Add_Dialog ("Тут, я так понимаю, в эту штуку с ушами, рядом с дверью, надо сказать пароль.\n");
		// Step_Add_Dialog_Continue ("Части пароля, кто-то предусмотрительно забыл, в виде блокнотиков, раскиданых по уровню.\n");
		// Step_Add_Dialog_Continue ("Я умею их читать.\n");
		// Step_Add_Dialog ("Для этого, мне надо встать перед блокнотиком, повернутся к нему зубами, и громко вызвать функцию BOT.GetText().\n");
		// Step_Add_Dialog_Continue ("Функция, с шикарной актёрской игрой и интонацией, вернёт текст записки.\n");
		// Step_Add_Dialog ("Текст - это новый тип данных: string (строка).\n");
		// Step_Add_Dialog_Continue ("Он, строго говоря, уже не просто тип, а класс, но на это нам пока плевать.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">string</color> t;");
		// Step_Add_Dialog ("Как обычно, переменная объявляется по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		// Step_Add_Dialog_Important_Continue ("\nt = <color=\"#C3915Bff\">\"some text here\"</color>;");
		// Step_Add_Dialog_Continue ("Константы типа string берутся в кавычки.");
		// Step_Add_Dialog ("Помните метод <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Say</color>(<color=\"#C3915Bff\">\"Привет!, или что вы там писали...\"</color>); из самого первого уровня?\n");
		// Step_Add_Dialog_Continue ("Ну вот он, как раз принимает аргументом тип string. Просто мы этого тогда не знали, и просто использовали как аргумент строковую константу.\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Say</color>(t);");
		// Step_Add_Dialog_Continue ("Точно так же можно было использовать и переменную, теперь это понятно.\n");
		// Step_Add_Dialog ("У типа string есть одно единственное действие - сложение.\n");
		// Step_Add_Dialog_Continue ("Которое, собственно, никакое и не сложение вовсе.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">string</color> a = <color=\"#C3915Bff\">\"Привет!\"</color>;\n<color=\"#A0A0ffff\">string</color> b = <color=\"#C3915Bff\">\"Я бот.\"</color>;\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Say</color>(a + <color=\"#C3915Bff\">\" \"</color> + b);");
		// Step_Add_Dialog_Continue ("Если к строке <color=\"#C3915Bff\">\"Гусь двухярусный\"</color> <i>прибавить</i> строку <color=\"#C3915Bff\">\"белый замшевый\"</color>, то получим просто одну строку, состоящию из двух <i>слогаемых</i>: <color=\"#C3915Bff\">\"Гусь двухярусныйбелый замшевый\"</color>.\n");
		// Step_Add_Dialog_Continue ("Отметим, что во первых, тут не хватает пробела - ему не откуда взятся. Что бы он был, его надо было добавить либо в конец первой строки, либо в начало второй.\n");
		// Step_Add_Dialog ("И во вторых - никогда больше не называйте эту операцию <i>сложением</i> - это делает Билла Гейтса грустным.\n");
		// Step_Add_Dialog_Continue ("Соединение двух строк называется <b>конкатенацией</b>, а то что для этого служит символ \"+\" - просто досадная пичалька.\n");
		// Step_Add_Dialog ("Давайте прочитаем блокнотики, соберём пароль в единое целое, и прочитаем той штуке.\n");
		// Step_Add_Dialog_Continue ("Читать надо с помощью уже знакомого метода <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Say</color>(string t);\n");
		// Step_Add_Dialog_Continue ("После чего, с гордо подянтой головой заедем на заветный квадратик.\n");
		// Step_Add_Dialog ("Мы не знаем в какой последовательности должны стоять найденые слова в пароле, по этому забрутфорсим слухострастие - прочитаем сначала в одном порядке, а потом в другом.\n");
		// Step_Add_Dialog_Continue ("Благо записок всего две, и комбинаций, стало быть тоже только две.\n");
		// Step_Add_Dialog_Continue ("На какую то из них оно должно открытся.\n");
		// Step_Add_Dialog ("Ах, да! Не забудте, что между найдеными словами должен стоять пробел. Иначеменянепоймут.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">string</color> <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetText</color>();");
		// Step_Add_Dialog_Continue ("Поехали!\n");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = 0 && BOT.Position.z = 1");
		// Step_Add_LevelComplete("Level 1-3d - Переменные 4 - string.", "Level 001-03d");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("В объектно ориентированном программировании, помимо использования методов классов (объектов) и операций с переменными, есть лишь два оператора на которых и строится вся логика программы.\n");
		// Step_Add_Dialog_Continue ("Остальное от лукавого.\n");
		// Step_Add_Dialog_Continue ("Ну, или разновидность чего-то из вышеперечисленного.");
		// Step_Add_Dialog ("Первый из этих двух операторов - оператор ветвления: if (если).\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 3;\n   if ( a < 5 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(3);\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(6) - line_offset_arr);
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">if</color> ( expression ) { //do something; }");
		// Step_Add_Dialog_Continue ("Выглядит это так:\n");
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(8) + (line_offset_half / 2), red_square_w, line_offset_mul * 2);
		// Step_Add_Dialog_Continue ("- если выражение в скобках верно - код, в идущих следом фигурных скобках выполняется.\n");
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(12) + (line_offset_half / 2), red_square_w, line_offset_mul * 2);
		// Step_Add_Dialog_Continue ("- если выражение не верно, то этот код пропускается, и управление передаётся коду, следующему сразу после фигурных скобок.");
		// Step_Add_Dialog ("Код, идущий после фигурных скобок оператора if выполнится в любом случае, хоть условие верно, хоть нет.\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 3;\n   if ( a < 5 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(3);\n   }\n   else\n   {\n      BOT.Move(8);\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(11) - line_offset_arr);
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(13) + (line_offset_half / 2), red_square_w, line_offset_mul);
		// Step_Add_Dialog_Continue ("А если нам нужен код, который выполнится только если условие <b><i>НЕ</i></b> верно, то для этого, после фигурных скобок, можно добавить ключевое слово \"else\", с ещё одним блоком кода.");
		// Step_Add_Dialog ("Точка с запятой после фигурных скобок никогда не нужна, т.к. это не команда, а просто разграничение блока кода.");
		// Step_Add_Tutorial_Arrow_Hide();
		// Step_Add_Tutorial_RedPanel_Hide();
		// Step_Add_Dialog_Important ("( a == 2 ) ( a ≺ b ) ( a ≻= 100 ) ( BOT.IsActive() )");
		// Step_Add_Dialog ("Что же касается выражения, то это условие, которое может быть либо операцией сравнения чисел (больше, меньше, равно, не равно и т.д.), либо какой-нибудь функцией класса, возвращающей булево значение (типа функции BOT.IsActive(), которая возвращает true если бот <i>что-то делает</i>).");
		// Step_Add_Dialog ("Обратите внимание, что оператор сравнения \"равно\" - это два знака равно.\n");
		// Step_Add_Dialog_Continue ("- Один знак равно - присвоить переменной 'a' значение 'b'.\n- Два знака равно - сравнить переменные 'a' и 'b'.");
		// Step_Add_Dialog_Important_Hide();
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n   if (  )\n   {\n   \n   \n   }\n\n\n}");
		// Step_Add_Dialog ("Перед вами пять контейнеров с квантовой энергией.\n");
		// Step_Add_Dialog_Continue ("Эти контейнеры чуть более стабильны чем предыдущие, и количество энергии в них меняется только когда вы запускаете скрипт.");
		// Step_Add_Dialog ("Надо уничтожить контейнеры, где количество энергии будет больше или равно 10и (x ≻= 10).\n");
		// Step_Add_Dialog_Continue ("Они... не знаю... опасны, наверное...\n");
		// Step_Add_Dialog_Continue ("А остальные, где энергии меньше 10и - нужно собрать.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">CheckContainerCapacity</color>();");
		// Step_Add_Dialog ("Чтобы узнать, сколько энергии в контейнере, не собирая её, у меня есть функция BOT.CheckContainerCapacity();\n");
		// Step_Add_Dialog_Continue ("Так же как и PickUp(), она возвращает целое число - тип int.\n");
		// Step_Add_Dialog_Important_Continue("\n<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">DestroyContainer</color>();");
		// Step_Add_Dialog_Continue ("Чтобы уничтожить контейнер перед собой - есть метод BOT.DestroyContainer();\n");
		// Step_Add_Dialog ("Вперёд!");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && bot.maxLootQ < 10 && NoLootLeft");
		// Step_Add_LevelComplete("Level 1-4 - Оператор ветвления if.", "Level 001-04");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("В прошлый раз я сказал что всё объектно ориентированное программирование сводится к вызову методов классов, операциям с переменными и ещё двум ключевым моментам.");
		// Step_Add_Dialog ("Первый из них - это операции ветвления, типа if.");
		// Step_Add_Dialog ("Ну а второй, это циклы.");
		// Step_Add_Dialog ("Циклы нужны для повтора каких то действий несколько раз.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">while</color> ( expression ) { //do something; }");
		// Step_Add_Dialog ("Самый простой цикл - while (англ. до тех пор, пока).\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 0;\n   while ( a < 100 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(1);\n      a = a + 1;\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(6) - line_offset_arr);
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(8) + (line_offset_half / 2), red_square_w, line_offset_mul * 3);
		// Step_Add_Dialog_Continue ("Пока условие в скобках верно, код в фигурных скобках, следующих сразу за оператором будет повторяться раз за разом, снова и снова, пока всё не обратится в прах...");
		// Step_Add_Dialog ("Контролируют количество циклов обычно вот так:\n");
		// Step_Add_Tutorial_RedPanel_Hide();
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(5) - line_offset_arr);
		// Step_Add_Dialog_Continue ("- инициализируют переменную в ноль,\n");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(6) - line_offset_arr);
		// Step_Add_Dialog_Continue ("- ставят в условии \"делать, пока переменная меньше 100\" (или любое другое кол-во повторений),\n");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(10) - line_offset_arr);
		// Step_Add_Dialog_Continue ("- и внутри цикла прибавляют к переменной еденицу.\n");
		// Step_Add_Dialog ("Думаю, не сложно понять, что конкретно в этом цикле, код выполнится сто раз, т.е. пока 'а' будет в диапазоне от изначального 0 до 99-и, потому как в условии написано \"пока 'а' меньше 100\", а сто - не меньше ста, сто равно сто. Но никак не меньше.\n");
		// Step_Add_Dialog_Continue ("И это досадно :(");
		// Step_Add_Tutorial_Arrow_Hide();
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n   while (  )\n   {\n   \n   \n   }\n\n\n}");
		// Step_Add_Dialog ("А теперь, уничтожте эти пять унитазов, используя всего пять команд.\n");
		// Step_Add_Dialog_Continue ("(while и if за команду не считаются - это операторы. В общем и целом, за команду будет считаться то, после чего должна идти точка с запятой).");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && NoLootLeft && bot.commands <= 5");
		// Step_Add_LevelComplete("Level 1-5 - Оператор цикла while.", "Level 001-05");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Ох уж эти унитазы. Всё никак не уймутся.\n");
		// Step_Add_Dialog_Continue ("Ууу, гады, выглядывают из за угла, ждут когда отвлечёшься... А только отвернёшься, они сразу - ХВАТЬ!");
		// Step_Add_Dialog ("Вобщем этих надо тоже, того... ");
		// Step_Add_Dialog_Continue ("Убрать.");
		// Step_Add_Dialog ("Используем это что бы попробовать ещё один оператор цикла - for.\n");
		// Step_Add_Dialog_Continue ("for в целом работает так же как и while - цикл, он цикл и есть - только немного удобнее.");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a = 0;\n   while ( a < 100 )\n   {\n      //Делать что-то 100 раз\n      a = a + 1;\n   }\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(4) - line_offset_arr);
		// Step_Add_Dialog ("Если в while мы сначала объявляли переменную, инициализировали её в ноль, ");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(8) - line_offset_arr);
		// Step_Add_Dialog_Continue ("а внутри цикла прибавляли к ней единицу, в каждой итерации, ");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n   for ( int a=0; a < 100; a=a+1 )\n   {\n      //Делать что-то 100 раз\n   }\n\n}");
		// Step_Add_Tutorial_Arrow_Show (-520, GetLinePositionTMPro(4) - line_offset_arr);
		// Step_Add_Dialog_Continue ("то for позволяет сделать всё это сразу.");
		// Step_Add_Tutorial_Arrow_Hide();
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">for</color> ( initialization; expression; iteration ) \n{\n   //do something; \n}");
		// Step_Add_Dialog ("Синтаксис у оператора вот такой. ");
		// Step_Add_Dialog_Continue ("Где\n- \"initialization\" - это комманда, которая выполняется один раз при старте самого первого цикла,\n");
		// Step_Add_Dialog_Continue ("- \"iteration\" - комманда выполняющаяся в каждой итерации цикла,\n");
		// Step_Add_Dialog_Continue ("- \"expression\" - условие остановки цикла.");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && NoLootLeft && bot.commands <= 5");
		// Step_Add_LevelComplete("Level 1-5b - Оператор цикла for.", "Level 001-05b");

		// //Задание на float - спихнуть бочку с платформы не упав
		// //Задание создать свой метод - собрать 5 контейнеров идущих друг за другом за две команды в функции старт (повернутся; вызвать функцию;)

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Попереключаем переключатели, повключаем включатели...\n");
		// Step_Add_Dialog_Continue ("Что бы открыть дверь, и добратся до квадратика, те переключатели, над которыми горит лампочка нужно включить, а те, над которыми не горит - выключить.\n");
		// Step_Add_Dialog_Continue ("Всё просто.\n");
		// Step_Add_Dialog ("И лампочки и переключатели включаются в случайном порядке, при старте уровня, по этому просто запрограммировать меня на включение конкретных выключателей не получится.\n");
		// Step_Add_Dialog_Continue ("Придётся действовать по обстоятельствам.\n");
		// Step_Add_Dialog ("Состояние лампочки перед собой я могу получить с помощью функции <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetIndicatorState</color>().\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">bool</color> <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetIndicatorState</color>();");
		// Step_Add_Dialog_Continue ("Она возвращает тип bool.\n");
		// Step_Add_Dialog ("Это очень простой тип - переменная bool может хранить лишь одно из двух значений: true или false. Оба этих слова, кстати, являются константами.\n");
		// Step_Add_Dialog_Continue ("Единственными возможными в типе bool константами.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">bool</color> b;");
		// Step_Add_Dialog ("Переменная объявляется по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		// Step_Add_Dialog_Important_Continue ("\nb = true;\n<color=\"#A0A0ffff\">bool</color> i = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetIndicatorState</color>();");
		// Step_Add_Dialog_Continue ("Присваивание и использование - всё как обычно.\n");
		// Step_Add_Dialog_Continue ("Математические операторы к bool не применимы.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">bool</color> a = 10 > 5;\n<color=\"#A0A0ffff\">bool</color> a = x == 17;");
		// Step_Add_Dialog ("Но есть у этого типа одна забавная особенность - результат логических выражений (типа a > 5 или b == 3, мы их уже использовали в if и while) - это тоже тип bool. А значит результат таких выражений можно <i>присвоить</i> переменной этого типа.\n");
		// Step_Add_Dialog_Continue ("Не то, что бы это было мега полезно на данном этапе, просто знайте, что так можно.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">bool</color> alive = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">IsAlive</color>();\nif (alive == true) { <color=\"#73BA85ff\">//do something</color> }\nif (alive) { <color=\"#73BA85ff\">//exactly the same, as above</color> }");
		// Step_Add_Dialog ("Ещё одна прикольная особенность: поскольку результат сравнения, это тип bool, то что бы использовать в сравнении непосредственно сам тип bool, не обязательно расписывать полностью if (a == true). Достаточно просто - if (a).\n");
		// Step_Add_Dialog_Continue ("Можно и так и так, но второй вариант тупо короче.\n");
		// Step_Add_Dialog ("Вместо сравнений можно использовать и константы.\n");
		// Step_Add_Dialog_Important ("if (true) { ... }\nif (false) { ... }");
		// Step_Add_Dialog_Continue ("Вот два if-а, один из них будет выполнятся <i>ВСЕГДА</i>, а второй <i>НИКОГДА</i>.\n");
		// Step_Add_Dialog_Continue ("Практической пользы в этом нет вообще.\n");
		// Step_Add_Dialog_Continue ("Разве что для каких то отладочных целей, когда нужно дебагать код, внутри if-а, условие которого срабатывает раз в сто лет...\n");
		// Step_Add_Dialog_Important ("while (true) \n{\n     <color=\"#73BA85ff\">//some code</color>\n}");
		// Step_Add_Dialog ("А вот практическая польза от использовании константы в условии цикла есть.\n");
		// Step_Add_Dialog_Continue ("Это, так называемый вечный цикл. Он не остановится никогда, пока программа не будет закрыта.\n");
		// Step_Add_Dialog_Continue ("Такие иногда нужны, когда не получается в одном выражении описать условие остановки...\n");
		// Step_Add_Dialog_Continue ("Но это оставим на потом.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">SetSwitchState</color>(<color=\"#A0A0ffff\">bool</color> state_to_set);");
		// Step_Add_Dialog ("Что бы включить или выключить переключатель перед собой, у меня есть метод <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">SetSwitchState</color>(<color=\"#A0A0ffff\">bool</color> state_to_set);.\n");
		// Step_Add_Dialog_Continue ("Что означает, что у этого метода есть один аргумент, типа bool, под названием 'state_to_set'.\n");
		// Step_Add_Dialog_Continue ("И очевидно, что он отвечает за то, будет ли переключатель включён или выключен.\n");
		// Step_Add_Dialog_Important_Continue ("\n<color=\"#A0A0ffff\">bool</color> <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetIndicatorState</color>();");
		// Step_Add_Dialog_Continue ("Кстати, слово 'bool' перед названием функции, означает, что она <i>возвращает</i> данный тип.");
		// Step_Add_Dialog ("Ну что, попробуем доехать до квадратика?\n");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = 0 && BOT.Position.z = 3");
		// Step_Add_LevelComplete("Level 1-6a - Bool 1.", "Level 001-06a");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Dialog ("Это юнит-тест.\n");
		// Step_Add_Dialog_Continue ("Для сложных программ часто пишут тесты отдельных их компонентов, что бы автоматически проверять не сломалось ли что ни будь во время починки чего-то другого.\n");
		// Step_Add_Dialog_Continue ("Я - сложная программа, и это, как раз один из таких тестов.\n");
		// Step_Add_Dialog_Continue ("Конкретно этот, тестирует мою логику.\n");
		// Step_Add_Dialog ("Логические выражения, это те самые условия, которые мы пишем в операторах if и while, и которые можно засунуть в переменные типа bool.\n");
		// Step_Add_Dialog_Continue ("Но тут вам понадобятся ещё и логические операторы.\n");
		// Step_Add_Dialog ("Я уже говорил, что тип bool нельзя сложить или разделить друг на друга, по этому всякие +, -, *, / тут не работают.\n");
		// Step_Add_Dialog_Continue ("Однако, вместо этих <i>математических</i> операторов, у bool есть три <i>логических</i> - 'И', 'ИЛИ' и 'НЕ'.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">&&</color> - И\n<color=\"#A0A0ffff\">||</color> - ИЛИ\n<color=\"#A0A0ffff\">!</color> - НЕ");
		// Step_Add_Dialog_Continue ("Выглядят они вот так.\n");
		// Step_Add_Dialog ("Допустим, нам надо выполнить условие, если x лежит в промежутке между 5ю и 10ю.\n");
		// Step_Add_Dialog_Continue ("Иными словами, если \"x ≻= 5и _И_ x ≺= 10\".\n");
		// Step_Add_Dialog_Important ("if (x ≻= 5 && x ≺= 10) { //some code }");
		// Step_Add_Dialog_Continue ("Вот так это выглядит в коде.\n");
		// Step_Add_Dialog_Important_Continue ("\nif (x ≺ 5 || x ≻ 10) { //some code }");
		// Step_Add_Dialog ("ИЛИ работает схожим образом - если нам нужно условие, где x не лежит в промежутке между 5ю и 10ю, то есть если \"x ≺ 5и _ИЛИ_ x >10и\", то получится вот это\n");
		// Step_Add_Dialog ("С НЕ вообще всё просто - это унарный оператор, т.е. который работает не с двумя а только с одним операндом. \n");
		// Step_Add_Dialog_Important ("!true = false\n!false = true\nbool b = false;\nif (!b) { //some code }");
		// Step_Add_Dialog_Continue ("И он просто переворачивает операнд: если было true станет false, если было false станет true.\n");
		// Step_Add_Dialog ("Как и математические выражения, в логических тоже можно использовать скобки, что бы отделить части большего выражения друг от друга, и указать какие части должны выполнится первыми.\n");
		// Step_Add_Dialog_Important ("if ( (x ≻=5 && x ≺=10) || (x ≻= 105 && x ≺= 110 ) )");
		// Step_Add_Dialog_Continue ("Вот это большое и страшное выражение, сработает если x лежит в промежутке между 5ю и 10ю или 105ю и 110ю.\n");
		// Step_Add_Dialog_Continue ("То есть сначала проверяется первая часть, потом вторая, и по скольку обе этих части связаны оператором _ИЛИ_, то если <i>хоть одна из них истинна</i> - всё условие	тоже будет истинным.\n");
		// Step_Add_Dialog_Important_Continue ("\nif ( x ≻=5 && x ≺=10 || x ≻= 105 && x ≺= 110 )");
		// Step_Add_Dialog ("Что бы было если бы мы не использовали скобки?\n");
		// Step_Add_Dialog_Continue ("Допустип возьмём х = 150, который не находится ни в одном из двух нужных промежутков, и не должен сработать.\n");
		// Step_Add_Dialog ("Без скобок, все части условия выполняются поочерёдно: \n35 ≻= 5и - да\n35 ≺= 10и - нет\n35 ≻= 105и - да");
		// Step_Add_Dialog_Continue (" -- и на этом пункте всё условие станет истинным, потому что в _ИЛИ_, если хоть один операнд true - то и всё условие true.\n");
		// Step_Add_Dialog ("Теперь немного о самом тестировании.\n");
		// Step_Add_Dialog_Continue ("Терминал даст 10 задач, по 10 вопросов в каждой задаче. \n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n" + template_001_006b.Replace("\r", "") + "\n\n}");
		// Step_Add_Dialog_Continue ("Вот описание задач.\n");
		// Step_Add_Dialog ("На каждый вопрос, терминал даст несколько цифр, и нужно ответить ему - верно ли поставленное условие для этих цифр или нет.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">var</color> data = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Terminal_Read</color>();");
		// Step_Add_Dialog ("Что бы считать с терминала цифры, используем функцию BOT.Terminal_Read(), которая возвращает структуру.\n");
		// Step_Add_Dialog_Continue ("<i>Мы пока, конечно, не знаем что такое структура, но, надеюсь, это нас не остановит.</i>\n");
		// Step_Add_Dialog_Important_Continue("\n<color=\"#A0A0ffff\">int</color> a = data.A; <color=\"#A0A0ffff\">int</color> b = data.B;\n<color=\"#A0A0ffff\">int</color> c = data.C; <color=\"#A0A0ffff\">int</color> d = data.D;");
		// Step_Add_Dialog ("Забрать из структуры нужные числа можно так.\n");
		// Step_Add_Dialog ("После этого, надо ответить терминалу, верно ли условие текущей задачи для данных цифр.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">BOT</color>.<color=\"red\">Terminal_Answer</color>( <color=\"#A0A0ffff\">bool</color> responce );");
		// Step_Add_Dialog_Continue ("Делаем это с помощью метода BOT.Terminal_Answer(bool responce).\n");
		// Step_Add_Dialog ("Я могу выдать вам теплейт, для ответов терминалу.\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n" + template_001_006b2.Replace("\r", "") + "\n\n}");
		// Step_Add_Dialog_Continue ("ХОБА!\n");
		// Step_Add_Dialog_Continue ("Теперь вам остаётся только написать условие, для каждой задачи, что бы сформировать ответ терминалу.\n");
		// Step_Add_Dialog ("Поехали.\n");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">&&</color> - И ( a && b = true если и a и b = true)\n<color=\"#A0A0ffff\">||</color> - ИЛИ ( a || b = true если или a или b = true)\n<color=\"#A0A0ffff\">!</color> - НЕ ( если a = false --- !a = true\n            если a = true --- !a = false )");
		// Step_Add_Check_Requirement ("TERMINAL");
		// Step_Add_LevelComplete("Level 1-6b - Bool 2.", "Level 001-06b");

		// Step_Add_Dialog ("Ну вот и настало время твоего первого боя.\n");
		// Step_Add_Dialog_Continue ("Твой враг будет агрессивен и очень силён. От его шагов меркнет небо и армии расбегаются в страхе.\n");
		// Step_Add_Dialog_Continue ("Узри же, великого...");
		// Step_Add_Play_Sound(0);
		// Step_Add_Wait(2f);
		// Step_Add_Play_Sound(1);
		// Step_Add_Activate_Level_Object (0);
		// Step_Add_Dialog ("Окей, это просто бомба.");
		// Step_Add_Dialog ("К сожалению, что бы пройти дальше, её таки придётся уничтожить.");
		// Step_Add_Dialog ("И так, очобенности бобы:\n");
		// Step_Add_Dialog ("- Фигачить бомбу, как и любого врага, можно методом BOT.Fight(). Для этого, нужно стоять на клетке перед врагом и быть направленным в его сторону.\n");
		// Step_Add_Dialog_Continue ("- Количество единиц здоровья бомбы заранее не известно (каждый раз, когда скрипт запускается - оно меняется).\n");
		// Step_Add_Dialog_Continue ("- Когда у бомбы заканчивается здоровье, она немного ждёт, и делает БУМ.\n");
		// Step_Add_Dialog_Continue ("- В момент бума, лучше не стоять рядом с ней. Все роботы в радиусе трёх клеток от БУМкающей бомбы - трагически погибают.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">while</color> ( <color=\"#A0A0ffff\">true</color> )");
		// Step_Add_Dialog ("Предлагаю, сделать беcконечный цикл, в теле которого фигачить бомбу, и проверять её здоровье.");
		// Step_Add_Dialog_Important_Continue ("\n{\n  <color=\"#73BA85ff\">//фигачить бомбу и проверять здоровье;</color>\n  if (здоровье ≺= 0) { break; }\n}");
		// Step_Add_Dialog ("В момент, когда её здоровье станет меньше или равно нулю - выйти из цикла, и свалить.\n");
		// Step_Add_Dialog_Continue ("Проблема, стало быть, лишь в том, что бы узнать здоровье бомбы.");
		// Step_Add_Dialog ("Тут поможет функция BOT.GetClosestEnemyInfo() - она возвращает информацию о ближайшем враге.\n");
		// Step_Add_Dialog_Continue ("Тип возвращаемого этой функцией значения - это структура EnemyInfo.\n");
		// Step_Add_Dialog_Important ("<color=\"#A0ffA0ff\">struct</color> EnemyInfo {\n  <color=\"#A0A0ffff\">int</color> HP; <color=\"#73BA85ff\">//Здоровье</color>\n  <color=\"#A0A0ffff\">int</color> EP; <color=\"#73BA85ff\">//Заряд</color>\n  <color=\"#A0A0ffff\">float</color> pos_x <color=\"#73BA85ff\">//координата врага x на поле</color>\n  <color=\"#A0A0ffff\">float</color> pos_y <color=\"#73BA85ff\">//координата врага y на поле</color>\n}");
		// Step_Add_Dialog_Continue ("Структура - это всего лишь несколько переменных объединённых в одну.\n");
		// Step_Add_Dialog ("Как видно из объявления, в структуре есть здоровье врага. И хранится оно в переменной HP типа int.");
		// Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">EnemyInfo</color> en = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetClosestEnemyInfo</color>();\n<color=\"#A0A0ffff\">int</color> health = en.HP;");
		// Step_Add_Dialog ("Пользоваться этим можно так: \n- объявляем переменную типа EnemyInfo, присваиваем ей информацию о текущем враге, а что бы узнать здоровье, обращаемся к нужной переменной структуры через точку.\n");
		// Step_Add_Dialog_Important_Continue ("\n\n<color=\"#73BA85ff\">//или так</color>\n<color=\"#A0A0ffff\">int</color> health = <color=\"#A0A0ffff\">BOT</color>.<color=\"red\">GetClosestEnemyInfo</color>().HP;");
		// Step_Add_Dialog_Continue ("Ну, или, если вся структура не нужна, можно сразу запросить здоровье из функции, без промежуточной переменной. Так же через точку.");
		// Step_Add_Dialog ("Если использовать первый способ, то можно подумать, что достаточно получить структуру EnemyInfo вне цикла, а в теле цикла лишь проверять en.HP. Но это не сработает.\n");
		// Step_Add_Dialog_Continue ("Цифры в полученной структуре сами по себе менятся не будут. Соответственно условие en.HP ≺= 0 не сработает никогда, и я буду вечно дубасить бомбу, пока она меня не взорвёт");
		// Step_Add_Dialog ("Что бы обновить структуру, нужно её переполучить с помощью GetClosestEnemyInfo(), которая вернёт актуальную структуру на момент выполнения команды.\n");
		// Step_Add_Dialog_Continue ("Ну, или просто воспользоватся вторым способом.");
		// Step_Add_Dialog ("Вперёд!");
		// Step_Add_Dialog_Hide ();
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-7 - Structures - First Battle.", "Level 001-07");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("Ой, сколько бомб...\n");
		// Step_Add_Dialog_Continue ("Ну, в целом, ничего нового.\n");
		// Step_Add_Dialog_Continue ("Ездим, разминируем, любуемся пейзажем...\n");
		// Step_Add_Dialog_Continue ("Правда, тут получится довольно много кода. Причём та часть, в которой мы дубасим бомбу, пока она не загорится - будет повторена 4 раза, в абсолютно одинаковом виде.\n");
		// Step_Add_Dialog_Continue ("Не хорошо.");
		// Step_Add_Dialog ("Ты знаешь, ты можешь писать свои методы и функции. Они помогают избавится от повторений одинакового кода.\n");
		// Step_Add_Dialog_Continue ("До сих пор, весь код ты тоже писал внутри функции. Это функция Start().\n");
		// Step_Add_Tutorial_Arrow_Show (-560, GetLinePositionTMPro(1) - line_offset_arr); //Top - Padding - LineHeight - LineSpace - HalfLineHeight
		// Step_Add_Dialog_Continue ("Вот это - её определение.");
		// Step_Add_Dialog ("Ключевое слово \"void\" - означает что функция ничего не возвращает, т.е. является методом.\n");
		// Step_Add_Dialog_Continue ("Пустые скобки означают что у функции нет параметров.\n");
		// Step_Add_Tutorial_RedPanel_Show (red_square_x, GetLinePositionTMPro(3), red_square_w, line_offset_mul * 7);
		// Step_Add_Dialog_Continue ("Ну а дальше, между фигурных скобок, собственно идёт тело функции, т.е. код, который выполняется при её вызове.");
		// Step_Add_Dialog ("Функция Start() вызывается автоматически при запуске скрипта. Это так называемый EntryPoint - место, с которого начинает выполнятся программа, при её запуске.\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n\n\n\n\n\n\n}");
		// Step_Add_Tutorial_RedPanel_Hide();
		// Step_Add_Tutorial_Arrow_Show (-560, GetLinePositionTMPro(13) - line_offset_arr);
		// Step_Add_Dialog_Continue ("Но никто не мешает, по образу данной функции, создать свою. Скажем KillBomb().");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		// Step_Add_Dialog ("Внутрь мы запихнём код, который будет взрывать бомбу.\n");
		// Step_Add_Tutorial_Arrow_Show (-560, GetLinePositionTMPro(4) - line_offset_arr); //Top - Padding - LineHeight - LineSpace - HalfLineHeight		
		// Step_Add_SetScript ("\nvoid Start () \n{\n  //Go to bomb\n  KillBomb(); //This call our function and kill the bomb\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		// Step_Add_Dialog_Continue ("В основном же коде, нам останется только подойти к бомбе, и вызвать нашу новую функцию.\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n  //Go to bomb 1\n  KillBomb(); //This call our function and kill the bomb\n  //Go to bomb 2\n  KillBomb();\n  //Go to bomb 3\n  KillBomb();\n  //Go to bomb 4\n  KillBomb();\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		// Step_Add_Dialog_Continue ("И повторить четыре раза.\n");
		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		// Step_Add_Tutorial_Arrow_Hide();
		// Step_Add_Dialog_Continue ("Попробуй.");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-8 - Functions.", "Level 001-08");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-9 - Float - Second Enemy.", "Level 001-09");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-10 - Algorithmes - The Maze.", "Level 001-10");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-11 - Classes - Ball sorting.", "Level 001-11");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_LevelComplete("Level 1-12 - Test Video Background.", "Level 001-12");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_LevelComplete("Level 1-13 - Reactor Room", "Level Reactor Room");
		#endregion

		//Level_step_indices.RemoveAt(Level_step_indices.Count-1);
		LoadNextScene(current_scene - 1);

		// if (VideoPlayersToPrepare != null) {
		// 	foreach (var v in VideoPlayersToPrepare) v.Prepare();
		// }

		//Start from level 001-2
		//current_step = 31;
		//current_step = steps.Count - 10;
	}

	public void DoNextStep () {
		//Reset
		check_requirement = false;
		DialogOK.gameObject.SetActive(false);

		current_step += 1;
		if (current_step >= Scenario.steps.Count) return;
		Scenario.step_info cur_step = Scenario.steps[current_step];

		//Requirement step
		if (cur_step.step_type == Scenario.step_type.check_Requirement)
		{
			requirement_needed = cur_step.str_param1;
			requirement_cur = "";
			check_requirement = true;
		}

		//Script execution control
		if (cur_step.step_type == Scenario.step_type.pause)
		{
			GetComponent<Compiler>().Pause_Immediate(); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.resume)
		{
			GetComponent<Compiler>().Resume(); DoNextStep();
		}

		//SetScript step
		if (cur_step.step_type == Scenario.step_type.setScript)
		{
			if (ScriptInputField != null) { ScriptInputField.text = CodeHighLight(cur_step.str_param1); DoNextStep(); }
			else 						  { ScriptEditor_My.text = CodeHighLight(cur_step.str_param1); DoNextStep(); }
		}

		//Script Controls Show/Hide
		if (cur_step.step_type == Scenario.step_type.script_controls_show)
		{
			Control_UI.show(); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.script_controls_hide)
		{
			Control_UI.hide(); DoNextStep();
		}

		//Level Complete
		if (cur_step.step_type == Scenario.step_type.level_complete_show)
		{
			if (Options_Global_Static.current_progress < current_scene) {
				PlayerPrefs.SetInt("Game_Progress", current_scene);
				Options_Global_Static.current_progress = current_scene;
				Options_Global_Static.Hide_Levels_Above_Current_Progress();
			}
			camera_Controller.LevelCompleteShow();
		}

		if (cur_step.step_type == Scenario.step_type.skip_bot_rebase_anim)
		{
			Compiler.dont_show_rebase_anim_next_time = true; DoNextStep();
		}

		if (cur_step.step_type == Scenario.step_type.set_blocked_instructions)
		{
			BOT.blocked = cur_step.str_param1; DoNextStep();
		}

		if (cur_step.step_type == Scenario.step_type.activate_object)
		{
			string str = Level_obj_names[current_scene-1].ToUpper();
			Level_obj_dict[str].transform.GetChild(cur_step.int_param1).gameObject.SetActive(true);
			DoNextStep();
		}

		if (cur_step.step_type == Scenario.step_type.wait)
		{
			float t = cur_step.float_param1;
			StartCoroutine(wait_and_do_next_step(t));

		}

		if (cur_step.step_type == Scenario.step_type.play_sound)
		{
			int i = cur_step.int_param1;
			string str = Level_obj_names[current_scene-1].ToUpper();
			Level_Settings ls = Level_obj_dict[str].GetComponent<Level_Settings>();
			if (cur_step.int_param2 > -1) {
				//Fade music volume
				main_audio_source.DOKill();
				main_audio_source.DOFade(0f, 0.5f).OnComplete(()=> {
					Play_Sound_SFX (ls.LevelSounds[i]); DoNextStep();
				});
			} else {
				//Just play
				Play_Sound_SFX ( ls.LevelSounds[i] ); DoNextStep();
			}
		}

		if (cur_step.step_type == Scenario.step_type.restore_music_volume)
		{
			main_audio_source.DOKill();
			main_audio_source.DOFade(Options_Global_Static.Volume_Music, 0.5f);
			DoNextStep();
		}

		//Dialog steps
		#region Dialog Steps
		if (cur_step.step_type == Scenario.step_type.dialog)
		{
			cur_text = cur_step.str_param1;
			string str = Scenario.Code_HighLight(cur_step.str_param1);
			DialogTutorial.Activate();
			DialogText.GetComponent<TextTyper>().TypeText(str, 0.075f, cur_step.int_param1, cur_step.int_param2);
		}
		if (cur_step.step_type == Scenario.step_type.dialog_continue)
		{
			cur_text += cur_step.str_param1;
			string str = Scenario.Code_HighLight(cur_step.str_param1);
			DialogTutorial.Activate();
			DialogText.GetComponent<TextTyper>().TypeText_Continue(str, 0.075f, cur_step.int_param1);
		}
		if (cur_step.step_type == Scenario.step_type.dialog_show)
		{
			DialogTutorial.Activate(); DoNextStep ();
		}
		if (cur_step.step_type == Scenario.step_type.dialog_hide)
		{
			DialogTutorial.DeActivate(); DoNextStep ();
		}

		if (cur_step.step_type == Scenario.step_type.dialog_important)
		{
			cur_text_imp = cur_step.str_param1;
			string str = Scenario.Code_HighLight(cur_step.str_param1);
			TutorialDialogImportant.Type(str, 0.15f, cur_step.int_param1, cur_step.int_param2); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.dialog_important_continue)
		{
			cur_text_imp += cur_step.str_param1;
			string str = Scenario.Code_HighLight(cur_step.str_param1);
			TutorialDialogImportant.TypeContinue(str, 0.15f, cur_step.int_param1); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.dialog_important_hide)
		{
			TutorialDialogImportant.DeActivate(); DoNextStep();
		}
		#endregion
		//Show/Hide Tutorial Animations steps
		#region Show/Hide Tutorial Elements Animations steps
		if (cur_step.step_type == Scenario.step_type.tutorialArrow_Show)
		{
			desktop_manager.Switch_Mode(0, true);
			desktop_manager.Ensure_Script_Window_Original_Size();

			Canvas cv2 = TutorialArrow.GetComponentInParent<Canvas>();
			float l_posY = ScriptEditor_My.line_offset(cur_step.float_param2);
			TutorialArrow.SetPosition(cur_step.float_param1, -l_posY / cv2.scaleFactor);

			TutorialArrow.Activate(); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.tutorialArrow_Hide)
		{
			TutorialArrow.DeActivate(); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.tutorialRedPanel_Show)
		{
			desktop_manager.Switch_Mode(0, true);
			desktop_manager.Ensure_Script_Window_Original_Size();
			float cv2_scale = TutorialArrow.GetComponentInParent<Canvas>().scaleFactor;
			Vector3[] world_corners = new Vector3[4];
			ScriptEditor_My.GetComponent<RectTransform>().GetWorldCorners(world_corners);
			float x1 = world_corners[1].x / cv2_scale;
			float x2 = world_corners[2].x / cv2_scale;
			float x1_final = Mathf.Lerp(x1, x2, cur_step.float_param1 / 100f);
			float x2_final = Mathf.Lerp(x1, x2, cur_step.float_param3 / 100f);
			float width = x2_final - x1_final;
			x2_final = -1920 + x2_final;
			float l_posY_1 = ScriptEditor_My.line_offset(cur_step.float_param2) / cv2_scale;
			float l_posY_2 = ScriptEditor_My.line_offset(cur_step.float_param4) / cv2_scale;
			
			TutorialRedPanel.SetPositionAndSize(x2_final, -l_posY_1, width, l_posY_2 - l_posY_1);
			TutorialRedPanel.Activate(); DoNextStep();
		}
		if (cur_step.step_type == Scenario.step_type.tutorialRedPanel_Hide)
		{
			TutorialRedPanel.DeActivate(); DoNextStep();
		}
		#endregion
	}



	// Update is called once per frame
	void Update () {
		CheckRequirement();

		if (check_key(Key.Debug_Generate_Script)) { Scenario.Export_All_Script(); }
		if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) { Scenario.Import_All_Script(); }
		if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) { Scenario.Convert_Generated_Script(); }
		//if (Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)) { BOT.bot_animator.SetTrigger("Blink"); }

		//Pause immediate
		if (Control_UI.isPlaying() && check_key(Key.Script_Control_Pause_Immediate)) {
			// if (!BOT.pause) 
			// 	GetComponent<Compiler>().Pause_Immediate();
			// else
			// 	GetComponent<Compiler>().Resume();
			if (BOT.pause || Control_UI.isInPauseState()) 
				GetComponent<Compiler>().Resume();
			else
				GetComponent<Compiler>().Pause_Immediate();
		}
	}

	public void SkipText() {
		//This is called by Panel_Dialog->Panel gameobject, which is actually a button
		//this way onClick works, and after clicking on it once, enter or spacebar works too
		if (ScriptEditor_My != null && ScriptEditor_My.Activate_Settings.IsActive) return;

		if (DialogText.GetComponent<TextTyper>().IsTyping)
		{
			DialogText.GetComponent<TextTyper>().Skip();
		} else {
			if (DialogOK.IsActive()) DoNextStep();
		}
	}

	string CodeHighLight (string str) {
		//if (scriptRefresh) return null;

		//Debug.Log ("CodeHighLight called.");
		//scriptRefresh = true;
		string keywords = @"\b(public|private|partial|static|namespace|class|using|void|foreach|in)\b";
        MatchCollection keywordMatches = Regex.Matches(str, keywords);
		// foreach (Match m in keywordMatches) {
		// 	str = str.Remove(m.Index, m.Length);
		// 	str = str.Insert(m.Index, "<color=\"blue\">" + m.Value + "</color>");
		// }
		for (int i = keywordMatches.Count - 1; i >= 0; i--)
		{
			Match m = keywordMatches[i];
			str = str.Remove(m.Index, m.Length);
			str = str.Insert(m.Index, "<color=\"blue\">" + m.Value + "</color>");
		}
		//ScriptInputField.text = str;
		return str;
		//scriptRefresh = false;
	}

	public static void SetRequirement(string str)
	{
		requirement_cur = str;
		CheckRequirement();
	}

	public static bool CheckRequirement()
	{
		if (!check_requirement) return false;

		requirement_additional_info = "";
		if (Error_Panel.IsShown) return false;

		bool ok = false;
		string req = requirement_needed.ToLower().Trim();
		//Debug.Log("Checking requirement: " + requirement_needed);

		foreach (string req_sub in req.Split(new string[]{"&&"}, StringSplitOptions.RemoveEmptyEntries)) {
			ok = false;
			string req_loc = req_sub.Trim();

			if (requirement_cur.Trim() != "" && requirement_cur.ToLower().Trim() == req_loc) ok = true;

			if (req_loc.StartsWith("bot"))
			{
				string[] req_arr = req_loc.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				string op = req_arr[1].Trim();
				req_arr = new string[]{req_arr[0], req_arr[2]};

				float[] var_to_check = new float[]{0f, 0f};
				for (int i = 0; i <= 1; i++) {
					if      (req_arr[i].Trim().StartsWith("bot.position.x")) var_to_check[i] = BOT.bot_obj.transform.localPosition.x;
					else if (req_arr[i].Trim().StartsWith("bot.position.y")) var_to_check[i] = BOT.bot_obj.transform.localPosition.y;
					else if (req_arr[i].Trim().StartsWith("bot.position.z")) var_to_check[i] = BOT.bot_obj.transform.localPosition.z;
					else if (req_arr[i].Trim().StartsWith("bot.commands"))   { 
						// if (ScriptInputField != null)   var_to_check[i] = (float)ScriptInputField.text.Count(c => c == ';');
						// else 							var_to_check[i] = (float)ScriptEditor_My.text.Count(c => c == ';');
						if (commands_count == -1) op = "failed"; else var_to_check[i] = (float)commands_count;
					}
					else if (req_arr[i].Trim().StartsWith("bot.maxlootq")) {
					   //Debug.Log("Loot list count: " + Loot_list.Count.ToString());
					   if (Loot_list.Count > 0) var_to_check[i] = (float)Loot_list.Max(a => a.Value); else op = "failed";
					   //Debug.Log("Max loot = " + var_to_check[i].ToString());
					}
					else var_to_check[i] = (float)double.Parse(req_arr[i], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture); //(float)double.Parse(req_arr[i], System.Globalization.NumberStyles.AllowDecimalPoint); //float.Parse(req_arr[i]);
				}

				//if (op.Contains("=") && Mathf.Approximately(var_to_check[0], var_to_check[1])) ok = true;
				//if (op.Contains("=") && (int)Mathf.Round(var_to_check[0]) == (int)Mathf.Round(var_to_check[1])) ok = true;
				if (op.Contains("=") && Math.Round(var_to_check[0], 2) == Math.Round(var_to_check[1], 2)) ok = true;
				else if(op.Contains("<") && var_to_check[0] < var_to_check[1]) ok = true;
				else if(op.Contains(">") && var_to_check[0] > var_to_check[1]) ok = true;

				if (!ok && req_arr[0].Trim().StartsWith("bot.commands")) requirement_additional_info = "Количество комманд (" + commands_count.ToString() + ") не соответствует необходимому (" + req_arr[1].Trim() + ").";
				//Debug.Log("Testing expression: '" + var_to_check[0].ToString() + "', '" + op +"', '" + var_to_check[1].ToString() + ", result = " + ok.ToString());
			}

			if (req_loc == "nolootleft") {
				ok = true;
				foreach (var sp in Level_Spawnable) {
					Loot lt = sp.GetComponent<Loot>();
					if (lt != null && sp.activeSelf) { ok = false; break; }
				}
			}

			if (req_loc == "noenemyleft") {
				if (BOT.dead)
					ok = false;
				else {
					ok = true;
					foreach (Enemy en in Level_Enemies) {
						if (en.HP > 0 || en.suspend) { ok = false; break; }
					}
				}
			}

			if (req_loc == "collectors") {
				ok = true;
				foreach (Collector cl in Level_Collectors) {
					if (cl.status == 0) { ok = false; break; }
				}
			}

			if (req_loc == "ScriptStopped".ToLower()) {
				if (!Control_UI.isPlaying()) ok = true;
			}

			//Debug.Log("Check requirement: cur from compiler = '" + requirement_cur + "', needed = '" + req_sub + "', res = " + ok.ToString());

			if (!ok) break;
		}

		if (ok)
		{
			Debug.Log("Req passed! cur from compiler = '" + requirement_cur + "', needed = '" + requirement_needed + "'");
			requirement_cur = "";
			requirement_needed = "";
			check_requirement = false;
			requirement_additional_info = "";
			GameObject.Find("Compiler").GetComponent<Engine>().DoNextStep();
			return true;
		}
		return false;
	}

	public void LoadNextScene(int index = -1)
	{
		if (index == -1) current_scene += 1; else current_scene = index + 1;
		//Debug.Log("Loading: " + current_scene.ToString());
		string str = Level_obj_names[current_scene-1].ToUpper();
		current_step = Level_step_indices[current_scene-1] - 1;

		DialogTutorial.DeActivateImmediate();
		TutorialDialogImportant.DeActivateImmediate();
		TutorialArrow.DeActivate();
		TutorialRedPanel.DeActivate();

		if (!Level_obj_dict.ContainsKey(str))
		{
			//Load new scene
			string level_name = str.Split(new char[]{'-'}, System.StringSplitOptions.RemoveEmptyEntries)[0];
			UnityEngine.SceneManagement.SceneManager.LoadScene(level_name, LoadSceneMode.Single);
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
			return;
		}

		foreach (var kv in Level_obj_dict)
			kv.Value.SetActive(false);

		Level_obj_dict[str].SetActive(true);
		spawner = Level_obj_dict[str].transform.Find("Spawner");

		Level_Setable.Clear();
		Level_Resetable.Clear();
		Level_Spawnable.Clear();
		Level_Collectors.Clear();
		Level_Enemies.Clear();
		Level_Readable.Clear();
		Level_StringListiners.Clear();
		for (int i = 0; i < Level_obj_dict[str].transform.childCount; i++) {
			Loot lt = Level_obj_dict[str].transform.GetChild(i).GetComponent<Loot>();
			if (lt != null) { lt.Reset(true); }

			Enemy en = Level_obj_dict[str].transform.GetChild(i).GetComponent<Enemy>();
			if (en != null) { en.Reset(true); Level_Enemies.Add(en);}
		}
		foreach (Loot lt in Level_obj_dict[str].transform.GetComponentsInChildren<Loot>())
			Level_Spawnable.Add(lt.gameObject);
		foreach (Collector cl in Level_obj_dict[str].transform.GetComponentsInChildren<Collector>())
			Level_Collectors.Add(cl);
		foreach (Readable r in Level_obj_dict[str].transform.GetComponentsInChildren<Readable>())
			Level_Readable.Add(r);
		foreach (String_Listiner sl in Level_obj_dict[str].transform.GetComponentsInChildren<String_Listiner>())
			Level_StringListiners.Add(sl);
		foreach (ISetable s in Level_obj_dict[str].transform.GetComponentsInChildren<ISetable>(true))
			Level_Setable.Add(s);
		foreach (IResetable r in Level_obj_dict[str].transform.GetComponentsInChildren<IResetable>(true))
			Level_Resetable.Add(r);

		//Music
		Level_Settings ls = Level_obj_dict[str].GetComponent<Level_Settings>();
		AudioClip current_clip = main_audio_source.clip;
		if (ls) {
			AudioClip clp = ls.LevelMusic;
			if (clp != current_clip) {
				main_audio_source.Stop();
				main_audio_source.clip = clp;
			}

			if (main_audio_source.volume < 0.001f) {
				main_audio_source.volume = Options_Global_Static.Volume_Music;
			}

			if (!main_audio_source.isPlaying) {
				main_audio_source.Play();
				//Debug.Log("Playing: " + main_audio_source.clip.name);
			}

			camera_Controller.cinematic_index = new Vector3Int(-1, 0, 0);
			camera_Controller.camera_handled_by_level_script = ls.camera_controls_overrides.camera_handled_by_level;
			if (ls.camera_controls_overrides.override_camera_position) {
				var cam_param = ls.camera_controls_overrides;
				camera_Controller.disable_camera_reposition = cam_param.disable_camera_reposition;
				camera_Controller.camera_target_offset = cam_param.target_offset;
				camera_Controller.camera_rotation_center_offset = cam_param.rotation_center_offset;
				camera_Controller.camera_height = cam_param.camera_height;
				camera_Controller.camera_distance = cam_param.target_distance;
				camera_Controller.camera_circular_offset_angle = cam_param.camera_angle;
				camera_Controller.disable_user_arrowRotation = cam_param.disable_controls_rotate_around;
				camera_Controller.disable_user_rigtMouseRotation = cam_param.disable_controls_rotate_rightMouse;
				camera_Controller.disable_user_zoom = cam_param.disable_controls_zoom;
				if (ls.camera_cinematic.enabled) camera_Controller.cinematic = ls.camera_cinematic; else camera_Controller.cinematic = null;
			} else {
				//Defaults
				camera_Controller.camera_target_offset = new Vector3(1.2f, 0f, 1.2f);
				camera_Controller.camera_rotation_center_offset = Vector2.zero;
				camera_Controller.camera_height = 5.63f;
				camera_Controller.camera_distance = 12.34f;
				camera_Controller.camera_circular_offset_angle = 307.5f;
				camera_Controller.disable_camera_reposition = false;
				camera_Controller.disable_user_arrowRotation = false;
				camera_Controller.disable_user_rigtMouseRotation = false;
				camera_Controller.disable_user_zoom = false;
				camera_Controller.cinematic = null;
			}
			camera_Controller.Reset_Camera_Cinematic(); //must be called after camera settings are set
		}

		//Set current active class to main
		GameObject.Find("Scroll View_Class").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
		
		//Set timescale to x1
		if (Ref_objs.options.Time_Control_Panel != null) //It's null when this is called first time on init
			Ref_objs.options.Time_Control_Panel.GetChild(0).GetComponent<Toggle>().isOn = true;

		BOT.blocked = "";
		Control_UI.show(); if (Control_UI.isPlaying()) GetComponent<Compiler>().Stop(true);
		GameObject.Find("Robot").transform.position = spawner.transform.position;
		GameObject.Find("Robot").transform.rotation = spawner.transform.rotation;
		DoNextStep();
	}
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		list_scene_level_objects();

		string str = Level_obj_names[current_scene-1].ToUpper();
		if (!Level_obj_dict.ContainsKey(str))
		{
			Debug.Log("Needed level object not found in scene."); return;
		}

		LoadNextScene(current_scene - 1);
	}

	public void list_scene_level_objects()
	{
		Level_obj_dict.Clear();
		foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
		{
			if (g.name.ToUpper().StartsWith("LEVEL ")) { //Warning: without trailing space it also disables 'Level_Complete' obj
				Level_obj_dict.Add(g.name.ToUpper(), g);
			}
		}
	}

	IEnumerator wait_and_do_next_step(float t)
	{
		yield return new WaitForSeconds(t);
		DoNextStep();
	}

	public static bool check_key(Key k, bool down = true)
	{
		foreach (KeyCode[] key_arr in Hotkeys[k]) {
			bool ok = true;
			foreach (KeyCode kc in key_arr) {
				if (kc == KeyCode.LeftAlt || kc == KeyCode.RightAlt)
					if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) continue; else { ok = false; break; }
				if (kc == KeyCode.LeftShift || kc == KeyCode.RightShift)
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) continue; else { ok = false; break; }
				if (kc == KeyCode.LeftControl || kc == KeyCode.RightControl)
					if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) continue; else { ok = false; break; }

				if (!down && !Input.GetKey(kc)) { ok = false; break; }
				else if (down && !Input.GetKeyDown(kc)) { ok = false; break; }
			}

			if (ok) return true;
		}

		// string str = "";
		// foreach (KeyCode kc in Hotkeys[k]) str += kc.ToString() + " ";
		// Debug.Log("Keycode true: " + str);
		return false;
	}

	public void Play_Sound_SFX(AudioClip clip, int index = 0, bool loop = false, bool stop = false) {
		if (index == 0) {
			secondary_audio_source.Stop();
			if (stop) return;
			secondary_audio_source.clip = clip;
			secondary_audio_source.loop = loop;
			secondary_audio_source.Play();
		} else {
			secondary_audio_source2.Stop();
			if (stop) return;
			secondary_audio_source2.clip = clip;
			secondary_audio_source2.loop = loop;
			secondary_audio_source2.Play();
		}
	}

	void OnGUI() {
		Event e = Event.current;
		if (e.isKey) {
			if (Input.GetKeyDown(e.keyCode)) BOT.key_downs.Add(e.keyCode);
		}
	}
    void OnApplicationQuit()
    {
		RenderSettings.skybox.SetFloat("_RotationZ", 0f);
    }
}

public class WaitForSeconds_BOTPauseAware : CustomYieldInstruction
{
	public Action _action_on_pause = null;
	public Action _action_on_unpause = null;

	float t = 0f;
	bool pause_state_handled = false;

    public override bool keepWaiting
    {
        get
        {
			if (BOT.is_paused || BOT.pause) {
				if (!pause_state_handled) { 
					pause_state_handled = true; 
					if (_action_on_pause != null) _action_on_pause.Invoke();
				}
				return true;
			} else {
				if (pause_state_handled) { 
					pause_state_handled = false;
					if (_action_on_pause != null) _action_on_unpause.Invoke();
				}
			}

			t -= Time.deltaTime;
            return t > 0f;
        }
    }

    public WaitForSeconds_BOTPauseAware(float time, Action action_on_pause = null, Action action_on_unpause = null)
    {
        t = time;
		_action_on_pause = action_on_pause;
		_action_on_unpause = action_on_unpause;
    }
}
