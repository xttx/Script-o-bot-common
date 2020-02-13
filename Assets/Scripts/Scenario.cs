using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Scenario
{
    public static List<step_info> steps = new List<step_info>();
    public static List<List<KeyValuePair<string, int>>> steps_for_fastForward = new List<List<KeyValuePair<string, int>>>();

	public struct step_info
	{
		public step_type step_type;
		public string str_param1;
		public int int_param1;
		public int int_param2;
		public float float_param1;
		public float float_param2;
		public float float_param3;
		public float float_param4;
	}
	public enum step_type
	{
		dialog,
		dialog_continue,
		dialog_show,
		dialog_hide,
		dialog_important,
		dialog_important_continue,
		dialog_important_hide,
		setScript,
		tutorialArrow_Show,
		tutorialArrow_Hide,
		tutorialRedPanel_Show,
		tutorialRedPanel_Hide,
		check_Requirement,
		level_complete_show,
		//level_complete_hide,
		script_controls_show,
		script_controls_hide,
		pause,
		resume,
		skip_bot_rebase_anim,
		set_blocked_instructions,
		activate_object,
		wait,
		play_sound,
		restore_music_volume
	}

	// static string[] build_in_types = {"int", "long", "float", "double", "decimal", "bool", "string", "char"};
	// static Regex class_and_method = new Regex(@"([A-Za-z\.]+)\s?\(.*\)", RegexOptions.Compiled); //Group 1 = all chain namespace.class.method
	// static Regex quotes = new Regex("\".*?\"", RegexOptions.Compiled);

	static string template_001_006b = @"
//01. Число A является положительным а число B является отрицательным
//02. A < B < C
//03. Число A находится между числами B и C
//04. Каждое из чисел A, B, C положительное
//05. Хотя бы одно из чисел A, B, C положительное
//06. Ровно два из чисел A, B, C являются положительными
//07. A является двузначным
//08. Среди трех данных целых чисел есть хотя бы одна пара совпадающих
//09. Точка с координатами (a, b) лежит в правой нижней координатной четверти
//10. Точка с координатами (a, b) лежит в левой верхней или правой нижней координатной четверти
";
	static string template_001_006b2 = @"
//01. Число A является положительным а число B является отрицательным
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//02. A < B < C
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//03. Число A находится между числами B и C
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//04. Каждое из чисел A, B, C положительное
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//05. Хотя бы одно из чисел A, B, C положительное
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//06. Ровно два из чисел A, B, C являются положительными
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//07. A является двузначным
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//08. Среди трех данных целых чисел есть хотя бы одна пара совпадающих
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B; int c = td.C;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//09. Точка с координатами (a, b) лежит в правой нижней координатной четверти
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);
}

//10. Точка с координатами (a, b) лежит в левой верхней или правой нижней координатной четверти
for (int i = 0; i < 10; i++) {
   var td = BOT.Terminal_Read();
   int a = td.A; int b = td.B;
   bool responce = ??? //Write your expression here
   BOT.Terminal_Answer(responce);  
}";
	static string template_001_010 = @"
void Start ()
{
  for (int i = 0; i < 10; i++) {
    var d = BOT.Terminal_Read();
    string mail = d.str;
    string[] parsed_mail = new string[2]{};

    //Insert you code here



    BOT.Terminal_Answer(parsed_mail);
  }
}";
	static string template_001_010_2 = @"
void Start ()
{
  for (int i = 0; i < 10; i++) {
    var d = BOT.Terminal_Read();
    string mail = d.str;
    string[] parsed_mail = new string[2]{};

    //1. Надо создать заголовок письма - это должны быть первые 140 символов и три точки (""..."") если письмо длинее 140 символов, или всё письмо если оно короче.
    //2. Если имя или отчество Лежебоки Лодыревны начинается с маленькой буквы - заменить её на заглавную (Лежебока Лодыревна отключается, если видит своё имя/фамилию с маленькой буквы).
    //3. Нужно отфильровать почту на спам - пометить спамом все письма, содержащие 'вы выиграли' или 'одобрен кредит'. Если содержит - добавить перед заголовком строку ""(СПАМ) "".
    //4. Заменяем все ""а"" на ""о"" а ""о"" на ""а"".

    //Insert you code here



    BOT.Terminal_Answer(parsed_mail);
  }
}";
    // Start is called before the first frame update
    public static void Init()
    {
        if (steps.Count > 0) return; //Already initialized

		Engine.Level_step_indices.Add(0);

		#region levels_text_data
		//Old string color = #C3915Bff

		//Level 1-1
		Step_Add_SetScript ("");
		Step_Add_Dialog ("Привет, меня зовут <b>ГИЗМО</b>. И, к сожалению, я - робот.");
		Step_Add_Dialog ("Тяжело быть роботом - я ничего не умею делать, пока мне не прикажут...");
		Step_Add_Dialog ("Разговаривать со мной надо определённым образом, но, всё по порядку.");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Tutorial_Arrow_Show (-560F, 1F);
		Step_Add_Dialog ("У меня есть метод Start, он вызывается сразу при моём запуске.");
		Step_Add_Tutorial_RedPanel_Show (1F, 3F, 99F, 6F);
		Step_Add_SetScript ("\nvoid Start () \n{\n\n//All code here will be executed\n//  when you press Play\n\n}");
		Step_Add_Dialog ("Метод - это блок кода, который выполняет некоторые действия.");
		Step_Add_Dialog ("Весь код, находящийся в теле метода Start, т.е. между\nфигурными скобками { и } - выполнится сразу после нажатия кнопки \"Play\".");
		Step_Add_Tutorial_Arrow_Hide ();
		Step_Add_Tutorial_RedPanel_Hide ();
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog_Important ("<c>BOT</c>");
		Step_Add_Dialog ("В коде ко мне надо обращаться, используя статический класс <c>BOT</c>. Что такое статический класс, сейчас знать не особо нужно. Просто знай, что все команды, обращённые ко мне, начинаются с \"<c>BOT</c>.\"\nИменно с точкой, после <c>BOT</c>, она там специально!");
		Step_Add_Dialog ("<c>ВОТ</c>, это в смысле <b>БОТ</b>, то есть я. А не <b>ВОТ</b> в смысле \"вот\". То есть \"БОТ\" английскими буквами, а не \"Вот, нате вам креньдельки под язык, да куличики в ноздри\".\nПонятно? Нет? Ну и ладно...");
		Step_Add_Dialog ("У меня много разных методов: я умею двигаться, подбирать лут, драться, летать, фигачить кувалдой по унитазам, крафтить туалетную бумагу и даже... э-эээ...\nДавай не будем пока углубляться.");
		Step_Add_Dialog ("Методы, которые начинаются с \"<c>BOT</c>.\" - это команды лично мне, заставляющие меня что-то сделать.");
		Step_Add_Dialog ("Попробуем что-нибудь несложное.\nУ меня есть метод <m>Say</m>, он заставляет меня говорить.\n");
		Step_Add_Dialog_Important_Continue (".<m>Say</m>");
		Step_Add_Dialog_Continue ("Чтобы вызвать (выполнить) мой метод, нужно написать название моего класса, затем точку и, наконец, имя метода.");
		Step_Add_Dialog ("У этого метода есть один обязательный параметр - собственно, текст, который я должен сказать.\n");
		Step_Add_Dialog_Important_Continue ("(<str>\"Привет мир, я Гизмо!\"</str>)");
		Step_Add_Dialog_Continue ("Параметры указываются в скобках после имени метода.\nНу, и, потому, что это текст, он должен быть в кавычках, иначе я не пойму, что я должен интерпретировать как часть кода, а что как текст.");
		Step_Add_Dialog_Important_Continue (";");
		Step_Add_Dialog ("Наконец, после каждой команды должна стоять точка с запятой. Получается как-то так.");
		Step_Add_Dialog ("К сожалению, я глуповат, и регистр букв имеет значение. Т.е., если, к примеру, написать say только маленькими буквами, или только большими (SAY) - я не пойму.\n");
		Step_Add_Dialog_Continue ("Всё должно быть написано именно так, как заложено изначально. В данном случае - с большой буквы: <m>Say</m>.");
		Step_Add_Dialog ("А теперь, заставьте меня сказать \"Привет\", или что-то, типа того...");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("SAY");
		Step_Add_LevelComplete ("Level 1-1 - Методы.", "Level 001-01");

		//Level 1-2
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Set_Blocked_Instructions ("move(x);rotate(x);");
		Step_Add_Dialog_Show ();
		Step_Add_Dialog_Important_Hide ();
		Step_Add_ScriptControls_Hide ();
		Step_Add_Dialog ("Хорошо, продолжим.");
		Step_Add_Dialog ("Итак, здесь мы видим маленький красненький квадратик.");
		Step_Add_Dialog ("Вообще, когда мы видим маленький красненький квадратик - это значит, что мне надо попасть туда.");
		Step_Add_Dialog ("Без понятия, зачем - просто надо. Возможно, в детстве, мне не хватало маленьких красненьких квадратиков... Не знаю. Потом разберёмся.");
		Step_Add_Dialog ("Для этого мне надо двигаться. (Привет, кэп).");
		Step_Add_Dialog ("Давай посмотрим, какие методы у меня, как у класса, есть для передвижения...\n<m>Dance</m>(), <m>DoCoffee</m>(), <m>DestroyAllHumans</m>()... не, не то... <m>DesintegrateWorld</m>(), <m>MakeSandwitch</m>()... какой же я класный, столько всего умею!");
		Step_Add_Dialog ("Вот, нашёл.\n");
		Step_Add_Dialog_Continue ("Метод <m>Move</m>().");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>Move</m>();");
		Step_Add_Dialog ("У этого метода нет параметров, которые надо указывать в скобках. Но сами скобки всё равно обязательны - просто для того, чтобы я понял, что это вызов метода.");
		Step_Add_Dialog ("Попробуй передвинуть меня.");
		Step_Add_Dialog_Hide ();
		Step_Add_ScriptControls_Show ();
		Step_Add_Check_Requirement ("BOT.Position.x > 1.5");
		Step_Add_ScriptControls_Hide ();
		Step_Add_Pause ();
		Step_Add_Dialog_Show ();
		Step_Add_Dialog ("Не туда-а-а-а....");
		Step_Add_Dialog_Hide ();
		Step_Add_Resume ();
		Step_Add_Check_Requirement ("BOT.Position.y < -5");
		Step_Add_ScriptControls_Show ();
		Step_Add_Dialog ("Ладно, вырубай уже. Всё, я упал - кина не будет.");
		Step_Add_Check_Requirement ("ScriptStopped");
		Step_Add_Dialog ("Нехорошо вышло :(\n");
		Step_Add_Dialog_Continue ("Очевидно, метод <m>Move</m>() двигает меня вперёд. Но сейчас я стою, направленый в другую сторону.");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>Rotate</m>(n);");
		Step_Add_Dialog ("Попробуй перед методом <m>Move</m>() вызвать метод <m>Rotate</m>(n), где n - это число, в градусах, на которое я должен повернуться.");
		Step_Add_Dialog ("Если число будет положительное - я повернусь по часовой стрелке, если отрицательное - то против часовой стрелки.\n");
		Step_Add_Dialog_Continue ("И, да, поскольку это число, а не текст, то его не нужно брать в кавычки - и так сойдёт.");
		Step_Add_Dialog_Hide ();
		Step_Add_Set_Blocked_Instructions ("move(x);");
		Step_Add_Check_Requirement ("BOT.Position.z > 0.5");
		Step_Add_Pause ();
		Step_Add_ScriptControls_Hide ();
		Step_Add_Dialog ("О, да!\nМы почти на месте!");
		Step_Add_Resume ();
		Step_Add_Check_Requirement ("BOT.Position.z > 1.5");
		Step_Add_Dialog ("Чёрт...\n");
		Step_Add_Check_Requirement ("BOT.Position.z > 4");
		Step_Add_ScriptControls_Show ();
		Step_Add_Dialog_Continue ("Всё, вырубай.");
		Step_Add_Check_Requirement ("ScriptStopped");
		Step_Add_Dialog ("Знаешь, похоже, метод <m>Move</m>() в этом виде бесполезен.\nЯ просто пру вперёд, как танк, и в какой-то момент сорвусь с площадки или воткнусь в стену.");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>Move</m>(d);");
		Step_Add_Dialog ("Я тут покапался в себе... В общем, у метода <m>Move</m>() всё таки есть параметр - количество метров, на которое я должен продвинутся.");
		Step_Add_Dialog ("Это то, что нужно. Я этот параметр не заметил, потому что он опционалаен, то есть, его можно указывать, а можно и не указывать, иногда так бывает.\n");
		Step_Add_Dialog_Continue ("Ну да ладно.");
		Step_Add_Dialog ("Доведи меня уже до этого треклятого квадрата и пойдём дальше.");
		Step_Add_Dialog_Hide ();
		Step_Add_Set_Blocked_Instructions ("");
		Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.z = 1.5");
		Step_Add_Skip_Bot_Rebase_Anim ();
		Step_Add_Dialog ("Yeah!");
		Step_Add_LevelComplete ("Level 1-2 - Больше методов.", "Level 001-02");

		//Level 1-3a - variables int
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Хорошо, с методами, вроде бы, разобрались. Давай попробуем переменные.");
		Step_Add_Dialog ("Переменные - это способ заставить меня чего-то запомнить. И потом вспомнить.\n");
		Step_Add_Dialog ("Переменные - это ячейки памяти в моей башке. В них можно хранить числа, текст, всякое... Всё это - типы данных.\n");
		Step_Add_Dialog ("Сначала, чтобы я знал, что ты хочешь использовать переменную, её нужно объявить.\n");
		Step_Add_Dialog_Important ("<t>int</t> ");
		Step_Add_Dialog ("Делается это просто - сначала пишем тип данных переменной. Начнём с <t>int</t> - это тип \"целое число\".\n");
		Step_Add_Dialog_Important_Continue ("x");
		Step_Add_Dialog_Continue ("И имя переменной.\n");
		Step_Add_Dialog_Continue ("В данном случае \"x\" - это имя переменной (прямо как в школе учили), но оно может быть любым, только без пробелов и символов. (знаки подчёркивания использовать можно). \n");
		Step_Add_Dialog_Continue ("Однако, не стоит давать переменным невменяемые имена типа qwerty или kljsdfgi - просто потому, что потом невозможно понять для чего они нужны, что вообще тут делают и в чём смысл бытия...\n");
		Step_Add_Dialog_Important_Continue (";");
		Step_Add_Dialog_Continue ("Не забываем точку с запятой в конце.");
		Step_Add_Dialog ("С этого момента, я знаю, что у меня есть переменная типа <t>int</t> (целое число), под названием \"x\".");
		Step_Add_Dialog ("Попробуем выполнить задачку:\n");
		Step_Add_Dialog_Continue ("Видишь вот эту крутящуюся штуку?\n");
		Step_Add_Dialog_Continue ("Это контейнер квантовой энергии.\n");
		Step_Add_Dialog_Continue ("Из-за квантовой природы, количество энергии в контейнере не предопределено, и зависит от того, в какой момент он будет собран.\n");
		Step_Add_Dialog_Continue ("Может быть 0, может 10000");
		Step_Add_Dialog_Continue (", а может, мёртвый кот - как повезёт.");
		Step_Add_Dialog ("У меня есть функция <m>PickUp</m>(), которая заставляет меня подобрать контейнер с клетки, перед которой я нахожусь.\n");
		Step_Add_Dialog_Continue ("Это не метод, а именно функция.\n");
		Step_Add_Dialog_Continue ("Разница между методом и функцией в том, что функция возвращает какое-то значение, которое можно, например, запихнуть в переменную.\n");
		Step_Add_Dialog_Important_Continue ("\nx = <c>BOT</c>.<m>PickUp</m>();");
		Step_Add_Dialog_Continue ("Примерно вот так.");
		Step_Add_Dialog ("Функция <m>PickUp</m>() не имеет параметров (да, да, в этот раз точно, я проверил).\n");
		Step_Add_Dialog_Continue ("И возвращает она количество подобранной энергии.\n");
		Step_Add_Dialog_Continue ("Т.е. после этого наша переменная \"х\" станет равна количеству собранной энергии.\n");
		Step_Add_Dialog_Important_Continue ("\n\n<cmt>//То же самое, но короче</cmt>\n<t>int</t> x = <c>BOT</c>.<m>PickUp</m>();");
		Step_Add_Dialog_Continue ("Можно использовать и краткую форму присвоения. Она делает всё то же самое, но писать чуть меньше.\n");
		Step_Add_Dialog_Continue ("Главное, после объявления переменной, не забывать чего-то в неё запхнуть. Пустые переменные никому не нужны и вызывают критические ошибки при попытке их использования.");
		Step_Add_Dialog ("Вон та штука, в конце пути - это весы. На них нужно положить ровно столько энергии, сколько было подобрано из контейнера.");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>PutEnergy</m>(x);");
		Step_Add_Dialog ("Положить энергию на весы можно с помощью метода <m>PutEnergy</m>(x), где \"x\" - это количество енергии, которое нужно положить.\n");
		Step_Add_Dialog_Continue ("Не забудь, что положить энергию я могу только на весы, стоящие на клетке, прямо передо мной.");
		Step_Add_Dialog ("Ну что ж, попробуем...");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("Collectors");
		Step_Add_LevelComplete ("Level 1-3a - Переменные 1.", "Level 001-03a");

		//Level 1-3b - variables int 2
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Усложним задачу.\n");
		Step_Add_Dialog_Continue ("ыыыыыы :)))\n");
		Step_Add_Dialog ("Теперь перед нами два контейнера. Работают они так же, как и в предыдущем задании.\n");
		Step_Add_Dialog_Continue ("С весами та же история.\n");
		Step_Add_Dialog_Continue ("Но, теперь, нам необходимо положить на весы сумму энергии, собранной с обоих контейнеров.\n");
		Step_Add_Dialog_Important ("<t>int</t> a = 3 + 7 * 5;");
		Step_Add_Dialog ("С переменными можно производить всякие арифметические действия,\n");
		Step_Add_Dialog_Important_Continue ("\n<t>int</t> b = 2 + a + 5;");
		Step_Add_Dialog_Continue ("в которых могут учавствовать и другие переменные.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>int</t> x = a + b;");
		Step_Add_Dialog_Continue ("Эти действия, можно производить как при присвоении,\n");
		Step_Add_Dialog_Important_Continue ("\n<c>BOT</c>.<m>PutEnergy</m>(a + b);");
		Step_Add_Dialog_Continue ("так и сразу внутри параметра функции или метода.");
		Step_Add_Dialog ("Со всеми этими возможностями, посчитать сумму взятой энергии не должно быть большой проблемой.\n");
		Step_Add_Dialog_Continue ("Я верю, что ты справишся :).");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("Collectors");
		Step_Add_LevelComplete("Level 1-3b - Переменные 2.", "Level 001-03b");

		//Level 1-3c - variables float
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Это - лава.\n");
		Step_Add_Dialog_Continue ("Для справки - роботам нельзя в лаву.\n");
		Step_Add_Dialog ("Это стало известно в 1991 году, когда Джеймс Кэмерон, в одном из своих фильмов, безжалостно утопил двух роботов в лаве.\n");
		Step_Add_Dialog_Continue ("Один из них довольно долго агонизировал. Мучительная смерть... ужас...\n");
		Step_Add_Dialog ("Этот случай тогда вызвал сильный резонанс, среди робонаселения.\n");
		Step_Add_Dialog_Continue ("Роботы объявляли голодовку, устраивали забастовки, ходили на митинги, в одиночные и массовые пикеты, байкотировали доставку угля...\n");
		Step_Add_Dialog_Continue ("Но никто тогда не придал этому значения.\n");
		Step_Add_Dialog_Continue ("То ли потому что всем плевать на роботов, то ли потому что роботов тогда не было.\n");
		Step_Add_Dialog ("У нас есть мигающий красный квадратик, и, очевидно, мне надо на него попасть.\n");
		Step_Add_Dialog_Continue ("Потому что... ");
		Step_Add_Dialog_Continue ("Ой, да ладно! Я уже говорил: одно наличие красного мигающего квадратика уже является причиной необходимости на него попасть.\n");
		Step_Add_Dialog ("Что тут можно сделать?\n");
		Step_Add_Dialog_Continue ("Длина пути впереди меня известна - 8.86 метров.\n");
		Step_Add_Dialog_Continue ("И у меня есть функция, которая возвращает <i>площадь</i> поверхности, на которой я стою.\n");
		Step_Add_Dialog_Continue ("(В данном случае, эта площадь включает в себя <b>внутренний бассейн</b> с лавой, но и так сойдёт.)\n");
		Step_Add_Dialog ("Чтобы узнать длинну второй части пути, достаточно площадь разделить на 8.86. Но это всё дробные числа и в тип int они не влезут.\n");
		Step_Add_Dialog ("Для дробных чисел есть типы float и decimal.\n");
		Step_Add_Dialog_Continue ("На самом деле есть ещё double, но он от float отличается только точностью - float может хранить число, с точностью примерно до 8-и знаков после запятой, а double примерно до 15-и.\n");
		Step_Add_Dialog_Continue ("Всё это можно посмотреть в бот-о-педии, в разделе \"Типы Данных\".\n");
		Step_Add_Dialog ("float от decimal отличается довольно значительно.\n");
		Step_Add_Dialog_Continue ("decimal может иметь аж 28 знаков после запятой, но это так, для справки. Самое главное не в этом.\n");
		Step_Add_Dialog ("float хранится в памяти компьютера в двоичной системе счисления, и в этой системе, некоторые, вполне обычные десятичные дроби типа 1.1 или 1.3 можно записать только в виде бесконечных дробей.\n");
		Step_Add_Dialog_Continue ("Ну, типа как 1/3 можно записать только как 0.3333333333...\n");
		Step_Add_Dialog ("Вы, люди, считаете что 1 - (0.1 * 10) - это 0?\n");
		Step_Add_Dialog_Continue ("Какие же вы всё таки наивные...\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n  BOT.Say(1 - 0.1f * 10);\n\n}");
		Step_Add_Dialog_Continue ("Запустите пример, что бы узнать правду. Но знайте - мир никогда больше не будет прежним.\n");
		Step_Add_Dialog ("Работая с float мы часто имеем такую ошибку округления. Довольно противная штука... \n");
		Step_Add_Dialog_Continue ("По этому сравнивать флоаты между собой - тоже плохая идея. Они могут оказаться <i>почти</i> равными, но этого не достаточно, что бы сравнени сработало.\n");
		Step_Add_Dialog ("Тип decimal лишён проблем округления, но занимает больше памяти, и вся математика с этим типом происходит гораздо медленнее.\n");
		Step_Add_Dialog_Continue ("Разумеется, если нужно произвести одно, или даже тысячу действий - то разница в скорости будет настолько мала, что её не измерить никаким секундомером.\n");
		Step_Add_Dialog_Continue ("Скорость float-а проявит себя только если счёт идёт на миллионы, и сотни миллионов действий.\n");
		Step_Add_Dialog_Continue ("Да и тогда, на хороших процессорах разница едва заметна.\n");
		Step_Add_Dialog ("С памятью примерно то же самое: переменная типа float занимает 4 байта, а типа decimal - 16 байт.\n");
		Step_Add_Dialog_Continue ("Формально - это аж в четыре раза больше.\n");
		Step_Add_Dialog_Continue ("На практике - это критично только если у вас массив из 1000+ элементов.\n");
		Step_Add_Dialog_Continue ("Если вам нужна ОДНА, или даже десять переменных - да всем плевать.\n");
		Step_Add_Dialog ("Возвращаемся к красненькому квадратику. <i>ммм... моя прелесть...</i>\n");
		Step_Add_Dialog ("Моя функция GetFloorSquare() возвращает площадь поверхности, на которой я нахожусь.\n");
		Step_Add_Dialog_Continue ("И это float.\n");
		Step_Add_Dialog_Continue ("За что мне ужасно стыдно, кстати.\n");
		Step_Add_Dialog_Important ("<t>float</t> f;");
		Step_Add_Dialog ("Переменные объявляются по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		Step_Add_Dialog_Important_Continue ("\nf = 3.7f;");
		Step_Add_Dialog_Continue ("Константы float должны иметь суффикс 'f', что быбыть распознаными как float.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>decimal</t> d = 3.7m;");
		Step_Add_Dialog_Continue ("Константы decimal - суффикс 'm'.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>double</t> d2 = 3.7;");
		Step_Add_Dialog_Continue ("Все дробные константы без суффикса будут распознаны как тип double. \n");
		Step_Add_Dialog ("Кстати в суффиксах, в зависимости от вашего мировоззрения и религиозных убеждений, можно использовать как строчные так и прописные буквы - компилятор достаточно толерантен, и политкорректен, и не будет против.\n");
		Step_Add_Dialog ("Результатом деления (а так же сложения, вычитания и умножения) float на float - конечно же будет float.\n");
		Step_Add_Dialog_Continue ("Вообще, если в какой то операции оба операнда одного типа, то результат будет того же типа что и операнды.\n");
		Step_Add_Dialog_Important ("<t>int</t> a = 10 / 3; <cmt>//WTF???</cmt>");
		Step_Add_Dialog_Continue ("Поэтому, осторожнее с делением целых чисел: 10 / 3 будет равно 3 а не 3.3333, как всем бы того хотелось.\nПотому, что оба числа распознаны как int, соответственно результат - тоже int, т.е. целое число.");
		Step_Add_Dialog_Important_Continue ("\n<t>float</t> a = 10f / 3f;\n<t>double</t> a = 10.0 / 3.0;");
		Step_Add_Dialog ("Чтобы это пофиксить - юзаем суффиксы: 10f / 3f будут распознаны как float-ы, и дадут верный результат. А 10.0 / 3.0 будут распознаны как double, и тоже дадут верный результат.\n");
		Step_Add_Dialog_Important ("<t>float</t> a = 10f / 3;\n<t>double</t> a = 10.0 / 3;\n<t>double</t> a = 10.0 / 3f;");
		Step_Add_Dialog ("Если операнды разных типов, и один из типов может безопасно хранить в себе второй - то операнд нижестоящего типа будет преобразован в вышестоящий, по цепочке int -> float -> double.\n");
		Step_Add_Dialog_Important ("10m / 3f; <cmt>//Compilation error</cmt>");
		Step_Add_Dialog_Continue ("float/double не могут хранить в себе decimal и наоборот, по этому использование float/double и decimal в одном выражении не допускается. Один из типов нужно преобразовывать вручную, но об этом как-нибудь потом.\n");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>Move</m>(1);\n<c>BOT</c>.<m>Move</m>(1.3f);\n<c>BOT</c>.<m>Move</m>();");
		Step_Add_Dialog ("Ну, и о главном - мой метод Move() перегружен, т.е. может принимать в качестве аргумента как int так и float. Или вообще не иметь аргументов, в результате чего, как мы помним, я уезжаю в бесконечность.\n");
		Step_Add_Dialog_Continue ("Если с int я езжу строго по квадратикам, то с float я могу остановится где захочу. Полная свобода!\n");
		Step_Add_Dialog_Continue ("Этой информации должно быть достаточно, чтобы довести меня уже до того замечательного квадратика.\n");
		Step_Add_Dialog ("Не забудьте:\n- Когда скрипт завершится, мне нужно стоять точно на квадратике. Погрешность больше 0.01 не принимается. А значит перебором решить проблему не удастся.\n- чтобы доехать до конца пути, мне нужно ехать <b>на метр меньше</b>, чем длинна пути, т.к. один метр занимаю я сам.");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = -3.93 && BOT.Position.z = 3.36");
		Step_Add_LevelComplete("Level 1-3c - Переменные 3 - float.", "Level 001-03c");

		//Level 1-3d - variables string
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Опять маленький квадратик. ");
		Step_Add_Dialog_Continue ("Ну хоть лавы нет.\n");
		Step_Add_Dialog ("Тут, я так понимаю, в эту штуку с ушами, рядом с дверью, надо сказать пароль.\n");
		Step_Add_Dialog_Continue ("Части пароля, кто-то предусмотрительно забыл, в виде блокнотиков, раскиданых по уровню.\n");
		Step_Add_Dialog_Continue ("Я умею их читать.\n");
		Step_Add_Dialog ("Для этого, мне надо встать перед блокнотиком, повернутся к нему зубами, и громко вызвать функцию <c>BOT</c>.<m>GetText</m>().\n");
		Step_Add_Dialog_Continue ("Функция, с шикарной актёрской игрой и интонацией, вернёт текст записки.\n");
		Step_Add_Dialog ("Текст - это новый тип данных: string (строка).\n");
		Step_Add_Dialog_Continue ("Он, строго говоря, уже не просто тип, а класс, но на это нам пока плевать.\n");
		Step_Add_Dialog_Important ("<t>string</t> t;");
		Step_Add_Dialog ("Как обычно, переменная объявляется по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		Step_Add_Dialog_Important_Continue ("\nt = <str>\"some text here\"</str>;");
		Step_Add_Dialog_Continue ("Константы типа string берутся в кавычки.");
		Step_Add_Dialog ("Помните метод <c>BOT</c>.<m>Say</m>(<str>\"Привет!, или что вы там писали...\"</str>); из самого первого уровня?\n");
		Step_Add_Dialog_Continue ("Ну вот он, как раз принимает аргументом тип string. Просто мы этого тогда не знали, и просто использовали как аргумент строковую константу.\n");
		Step_Add_Dialog_Important_Continue ("\n<c>BOT</c>.<m>Say</m>(t);");
		Step_Add_Dialog_Continue ("Точно так же можно было использовать и переменную, теперь это понятно.\n");
		Step_Add_Dialog ("У типа string есть одно единственное действие - сложение.\n");
		Step_Add_Dialog_Continue ("Которое, собственно, никакое и не сложение вовсе.\n");
		Step_Add_Dialog_Important ("<t>string</t> a = <str>\"Привет!\"</str>;\n<t>string</t> b = <str>\"Я бот.\"</str>;\n<c>BOT</c>.<m>Say</m>(a + <str>\" \"</str> + b);");
		Step_Add_Dialog_Continue ("Если к строке <str>\"Гусь двухъярусный\"</str> <i>прибавить</i> строку <str>\"белый замшевый\"</str>, то получим просто одну строку, состоящию из двух <i>слагаемых</i>: <str>\"Гусь двухярусныйбелый замшевый\"</str>.\n");
		Step_Add_Dialog_Continue ("Отметим, что во первых, тут не хватает пробела - ему не откуда взятся. Что бы он был, его надо было добавить либо в конец первой строки, либо в начало второй.\n");
		Step_Add_Dialog ("И во вторых - никогда больше не называйте эту операцию <i>сложением</i> - это делает Билла Гейтса грустным.\n");
		Step_Add_Dialog_Continue ("Соединение двух строк называется <b>конкатенацией</b>, а то что для этого служит символ \"+\" - просто досадная пичалька.\n");
		Step_Add_Dialog ("Давайте прочитаем блокнотики, соберём пароль в единое целое, и прочитаем той штуке.\n");
		Step_Add_Dialog_Continue ("Читать надо с помощью уже знакомого метода <c>BOT</c>.<m>Say</m>(string t);\n");
		Step_Add_Dialog_Continue ("После чего, с гордо подянтой головой заедем на заветный квадратик.\n");
		Step_Add_Dialog ("Мы не знаем, в какой последовательности должны стоять найденые слова в пароле, по этому забрутфорсим слухострастие - прочитаем сначала в одном порядке, а потом в другом.\n");
		Step_Add_Dialog_Continue ("Благо, записок всего две, и комбинаций, стало быть, тоже только две.\n");
		Step_Add_Dialog_Continue ("На какую-то из них оно должно открыться.\n");
		Step_Add_Dialog ("Ах, да! Не забудьте, что между найдеными словами должен стоять пробел. Иначеменянепоймут.\n");
		Step_Add_Dialog_Important ("<t>string</t> <c>BOT</c>.<m>GetText</m>();");
		Step_Add_Dialog_Continue ("Поехали!\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = 0 && BOT.Position.z = 1");
		Step_Add_LevelComplete("Level 1-3d - Переменные 4 - string.", "Level 001-03d");

		//Level 1-3e - variables keyword 'var'
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog_Important ("<t>int</t> i = 4;\n<t>var</t> i = 4; <cmt>//тоже самое что и выше</cmt>");
		Step_Add_Dialog ("Знаете, если вы инициализируете переменную при объявлении, то ключевое слово \"var\" может заменить собой любой тип!\n");
		Step_Add_Dialog_Important ("<t>var</t> i = 4; <cmt>//объявляет переменную int</cmt>\n<t>var</t> f = 4f; <cmt>//объявляет переменную float</cmt>\n<t>var</t> s = <str>\"qwa\"</str>; <cmt>//объявляет переменную string</cmt>");
		Step_Add_Dialog_Continue ("\"var\" не является типом, это просто команда, которая говорит компилятору \"угадать\" тип переменной из типа, в который вы эту переменную инициализируете.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>var</t> x; <cmt>//ошибка</cmt>");
		Step_Add_Dialog_Continue ("Если при объявлении инициализации не происходит, то и \"угадывать\" тип переменный не откуда, соответственно просто \"<t>var</t> x;\" писать нельзя. Только \"<t>var</t> x = что_нибудь;\".\n");
		Step_Add_Dialog ("Используя \"var\", можно ускорить процесс написания кода, т.к. вместо, к примеру \"<t>int</t>\", где целых три буквы, вы можете написать всего... э-ээ... другие три буквы.\n");
		Step_Add_Dialog_Continue ("Окей, тут разница не слишком большая.\n");
		Step_Add_Dialog_Important ("<c>long_named_class</c> t = <instr>new</instr> <c>long_named_class();</c>\n\n<cmt>//тоже самое что и выше</cmt>\n<t>var</t> t = <instr>new</instr> <c>long_named_class</c>();");
		Step_Add_Dialog_Continue ("Намного больше пользы эта команда приносит при работе с классами, которые - спойлер - тоже типы.\n");
		Step_Add_Dialog ("Ну а пока, пользуйтесь, если хотите. Главное сами не запутайтесь, где у вас что, когда все переменные будут \"<t>var</t>\".\n");
		Step_Add_Dialog ("Данный урок не предполагает задания. Поехали дальше.");
		Step_Add_LevelComplete("Level 1-3e - Переменные+ - Ключевое слово var.", "Level 001-03e");

		//Level 1-4 - if
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("В объектно ориентированном программировании, помимо использования методов классов (объектов) и операций с переменными, есть лишь два оператора на которых и строится вся логика программы.\n");
		Step_Add_Dialog_Continue ("Остальное от лукавого.\n");
		Step_Add_Dialog_Continue ("Ну, или разновидность чего-то из вышеперечисленного.");
		Step_Add_Dialog ("Первый из этих двух операторов - оператор ветвления: if (если).\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 3;\n   if ( a < 5 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(3);\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		Step_Add_Tutorial_Arrow_Show ( -520, 6f );
		Step_Add_Dialog_Important ("<instr>if</instr> ( expression ) { <cmt>//do something;</cmt> }");
		Step_Add_Dialog_Continue ("Выглядит это так:\n");
		Step_Add_Tutorial_RedPanel_Show (1f, 6.5f, 99f, 10.5f);
		Step_Add_Dialog_Continue ("- если выражение в скобках верно - код, в идущих следом фигурных скобках выполняется.\n");
		Step_Add_Tutorial_RedPanel_Show (1f, 10.5f, 99f, 14.5f);
		Step_Add_Dialog_Continue ("- если выражение не верно, то этот код пропускается, и управление передаётся коду, следующему сразу после фигурных скобок.");
		Step_Add_Dialog ("Код, идущий после фигурных скобок оператора if выполнится в любом случае, хоть условие верно, хоть нет.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 3;\n   if ( a < 5 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(3);\n   }\n   else\n   {\n      BOT.Move(8);\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		Step_Add_Tutorial_Arrow_Show ( -520, 11f );
		Step_Add_Tutorial_RedPanel_Show (1f, 11.5f, 99f, 14.5f);
		Step_Add_Dialog_Continue ("А если нам нужен код, который выполнится только если условие <b><i>НЕ</i></b> верно, то для этого, после фигурных скобок, можно добавить ключевое слово \"else\", с ещё одним блоком кода.");
		Step_Add_Dialog ("Точка с запятой после фигурных скобок никогда не нужна, т.к. это не команда, а просто разграничение блока кода.");
		Step_Add_Tutorial_Arrow_Hide();
		Step_Add_Tutorial_RedPanel_Hide();
		Step_Add_Dialog_Important ("( a == 2 ) ( a ≺ b ) ( a ≻= 100 ) ( <c>BOT</c>.<m>IsActive</m>() )");
		Step_Add_Dialog ("Что же касается выражения, то это условие, которое может быть либо операцией сравнения чисел (больше, меньше, равно, не равно и т.д.), либо какой-нибудь функцией класса, возвращающей булево значение (типа функции <c>BOT</c>.<m>IsActive</m>(), которая возвращает true если бот <i>что-то делает</i>).");
		Step_Add_Dialog ("Обратите внимание, что оператор сравнения \"равно\" - это два знака равно.\n");
		Step_Add_Dialog_Continue ("- Один знак равно - присвоить переменной 'a' значение 'b'.\n- Два знака равно - сравнить переменные 'a' и 'b'.");
		Step_Add_Dialog_Important_Hide();
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n   if (  )\n   {\n   \n   \n   }\n\n\n}");
		Step_Add_Dialog ("Перед вами пять контейнеров с квантовой энергией.\n");
		Step_Add_Dialog_Continue ("Эти контейнеры чуть более стабильны чем предыдущие, и количество энергии в них меняется только когда вы запускаете скрипт.");
		Step_Add_Dialog ("Надо уничтожить контейнеры, где количество энергии будет больше или равно 10и (x ≻= 10).\n");
		Step_Add_Dialog_Continue ("Они... не знаю... опасны, наверное...\n");
		Step_Add_Dialog_Continue ("А остальные, где энергии меньше 10и - нужно собрать.");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>CheckContainerCapacity</m>();");
		Step_Add_Dialog ("Чтобы узнать, сколько энергии в контейнере, не собирая её, у меня есть функция <c>BOT</c>.<m>CheckContainerCapacity</m>();\n");
		Step_Add_Dialog_Continue ("Так же как и PickUp(), она возвращает целое число - тип int.\n");
		Step_Add_Dialog_Important_Continue("\n<c>BOT</c>.<m>DestroyContainer</m>();");
		Step_Add_Dialog_Continue ("Чтобы уничтожить контейнер перед собой - есть метод <c>BOT</c>.<m>DestroyContainer</m>();\n");
		Step_Add_Dialog ("Вперёд!");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && bot.maxLootQ < 10 && NoLootLeft");
		Step_Add_LevelComplete("Level 1-4 - Оператор ветвления if.", "Level 001-04");

		//Level 1-5 - loop while
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("В прошлый раз я сказал что всё объектно ориентированное программирование сводится к вызову методов классов, операциям с переменными и ещё двум ключевым моментам.");
		Step_Add_Dialog ("Первый из них - это операции ветвления, типа if.");
		Step_Add_Dialog ("Ну а второй, это циклы.");
		Step_Add_Dialog ("Циклы нужны для повтора каких-то действий несколько раз.");
		Step_Add_Dialog_Important ("<instr>while</instr> ( expression ) { <cmt>//do something;</cmt> }");
		Step_Add_Dialog ("Самый простой цикл - while (англ. до тех пор, пока).\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a;\n   a = 0;\n   while ( a < 100 )\n   {\n      BOT.Rotate(90);\n      BOT.Move(1);\n      a = a + 1;\n   }\n\n   BOT.Rotate(-90);\n   BOT.Move(5);\n\n}");
		Step_Add_Tutorial_Arrow_Show ( -520, 6f );
		Step_Add_Tutorial_RedPanel_Show ( 1f, 7.5f, 99f, 10.5f );
		Step_Add_Dialog_Continue ("Пока условие в скобках верно, код в фигурных скобках, следующих сразу за оператором будет повторяться раз за разом, снова и снова, пока всё не обратится в прах...");
		Step_Add_Dialog ("Контролируют количество циклов обычно вот так:\n");
		Step_Add_Tutorial_RedPanel_Hide();
		Step_Add_Tutorial_Arrow_Show ( -520, 5f );
		Step_Add_Dialog_Continue ("- инициализируют переменную в ноль,\n");
		Step_Add_Tutorial_Arrow_Show ( -520, 6f );
		Step_Add_Dialog_Continue ("- ставят в условии \"делать, пока переменная меньше 100\" (или любое другое кол-во повторений),\n");
		Step_Add_Tutorial_Arrow_Show ( -520, 10f );
		Step_Add_Dialog_Continue ("- и внутри цикла прибавляют к переменной еденицу.\n");
		Step_Add_Dialog ("Думаю, не сложно понять, что конкретно в этом цикле, код выполнится сто раз, т.е. пока 'а' будет в диапазоне от изначального 0 до 99-и, потому как в условии написано \"пока 'а' меньше 100\", а сто - не меньше ста, сто равно сто. Но никак не меньше.\n");
		Step_Add_Dialog_Continue ("И это досадно :(");
		Step_Add_Tutorial_Arrow_Hide();
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n   while (  )\n   {\n   \n   \n   }\n\n\n}");
		Step_Add_Dialog ("А теперь, уничтожте эти пять унитазов, используя всего пять команд.\n");
		Step_Add_Dialog_Continue ("(<instr>while</instr> и <instr>if</instr> за команду не считаются - это операторы. В общем и целом, за команду будет считаться то, после чего должна идти точка с запятой).");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && NoLootLeft && bot.commands <= 5");
		Step_Add_LevelComplete("Level 1-5 - Оператор цикла while.", "Level 001-05");

		//Level 1-5b - loop for
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Ох уж эти унитазы. Всё никак не уймутся.\n");
		Step_Add_Dialog_Continue ("Ууу, гады, выглядывают из за угла, ждут когда отвлечёшься... А только отвернёшься, они сразу - ХВАТЬ!");
		Step_Add_Dialog ("В общем, этих надо тоже, того... ");
		Step_Add_Dialog_Continue ("Убрать.");
		Step_Add_Dialog ("Используем это чтобы попробовать ещё один оператор цикла - for.\n");
		Step_Add_Dialog_Continue ("<instr>for</instr>, в целом, работает так же как и <instr>while</instr> - цикл, он цикл и есть - только немного удобнее.");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   int a = 0;\n   while ( a < 100 )\n   {\n      //Делать что-то 100 раз\n      a = a + 1;\n   }\n\n}");
		Step_Add_Tutorial_Arrow_Show ( -520f, 4f );
		Step_Add_Dialog ("Если в <instr>while</instr> мы сначала объявляли переменную, инициализировали её в ноль, ");
		Step_Add_Tutorial_Arrow_Show ( -520f, 8f );
		Step_Add_Dialog_Continue ("а внутри цикла прибавляли к ней единицу, в каждой итерации, ");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   for ( int a=0; a < 100; a=a+1 )\n   {\n      //Делать что-то 100 раз\n   }\n\n}");
		Step_Add_Tutorial_Arrow_Show ( -520f, 4f );
		Step_Add_Dialog_Continue ("то <instr>for</instr> позволяет сделать всё это сразу.");
		Step_Add_Tutorial_Arrow_Hide();
		Step_Add_Dialog_Important ("<instr>for</instr> ( initialization; expression; iteration ) \n{\n   <cmt>//do something;</cmt> \n}");
		Step_Add_Dialog ("Синтаксис у оператора вот такой. ");
		Step_Add_Dialog_Continue ("Где\n- \"initialization\" - это команда, которая выполняется один раз при старте самого первого цикла,\n");
		Step_Add_Dialog_Continue ("- \"expression\" - условие остановки цикла,\n");
		Step_Add_Dialog_Continue ("- \"iteration\" - команда выполняющаяся в каждой итерации цикла.\n");
		Step_Add_Dialog ("С оператором for можно легко уложиться в 3 команды.");
		Step_Add_Dialog_Hide ();
		Step_Add_SetScript ("\nvoid Start () \n{\n\n   for ( ; ; )\n   {\n\n   }\n\n}");
		Step_Add_Check_Requirement ("ScriptStopped && NoLootLeft && bot.commands <= 3");
		Step_Add_LevelComplete("Level 1-5b - Оператор цикла for.", "Level 001-05b");

		//Level 1-5c - ++ & --
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Да, кстати, я забыл кое что.\n");
		Step_Add_Dialog_Continue ("В циклах нам часто нужно прибавлять к переменной единицу.\n");
		Step_Add_Dialog_Continue ("Да и не только в циклах, в принципе...\n");
		Step_Add_Dialog_Important ("a = a + 1;");
		Step_Add_Dialog ("Мы для этого писали следующую конструкцию: \"a = a + 1;\"\n");
		Step_Add_Dialog_Important_Continue ("\na++; <cmt>//то же самое</cmt>");
		Step_Add_Dialog_Continue ("В общем, в c# есть унарный оператор инкремента - \"++\", который собственно это и делает - прибавляет к переменной один.\n");
		Step_Add_Dialog_Continue ("Унарным он называется потому, что, в отличии от сложения или деления, ему нужно не два операнда, а достаточно одного.\n");
		Step_Add_Dialog ("Собственно, две строчки ниже приводят к одному и тому же результату - увеличивают \"а\" на один.\n");
		Step_Add_Dialog_Continue ("Просто нижний вариант тупо короче.\n");
		Step_Add_Dialog_Important_Continue ("\na--;");
		Step_Add_Dialog ("Ещё есть оператор декремента - \"--\".\n");
		Step_Add_Dialog_Continue ("Работает аналогичным образом, только не прибавляет, а вычитает из переменной единицу.\n");
		Step_Add_Dialog_Important ("2++; <cmt>//Error</cmt>");
		Step_Add_Dialog ("Работают эти операторы только с переменными. Использовать, например, оператор \"++\" с константой, означало бы \"увеличить константу на один\". А это не допускается - она ж константа!\n");
		Step_Add_Dialog_Important_Continue ("\n2 = 2 + 1; <cmt>//Error</cmt>");
		Step_Add_Dialog_Continue ("Это было бы эквивалентно вот такой записи.\n");
		Step_Add_Dialog_Continue ("Выглядит тупо, не правда ли?\n");
		Step_Add_Dialog ("На этом откланиваюсь. Задания в этот раз не будет, отдыхайте :).\n");
		Step_Add_LevelComplete("Level 1-5c - Унарные операторы ++ и --.", "Level 001-05c");

		//Задание на float - спихнуть бочку с платформы не упав
		//Задание создать свой метод - собрать 5 контейнеров идущих друг за другом за две команды в функции старт (повернутся; вызвать функцию;)

		//Level 1-6a - variables bool
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Попереключаем переключатели, повключаем включатели...\n");
		Step_Add_Dialog_Continue ("Чтобы открыть дверь, и добратся до квадратика, те переключатели, над которыми горит лампочка нужно включить, а те, над которыми не горит - выключить.\n");
		Step_Add_Dialog_Continue ("Всё просто.\n");
		Step_Add_Dialog ("И лампочки и переключатели включаются в случайном порядке, при старте уровня, по этому просто запрограммировать меня на включение конкретных выключателей не получится.\n");
		Step_Add_Dialog_Continue ("Придётся действовать по обстоятельствам.\n");
		Step_Add_Dialog ("Состояние лампочки перед собой я могу получить с помощью функции <c>BOT</c>.<m>GetIndicatorState</m>().\n");
		Step_Add_Dialog_Important ("<t>bool</t> <c>BOT</c>.<m>GetIndicatorState</m>();");
		Step_Add_Dialog_Continue ("Она возвращает тип bool.\n");
		Step_Add_Dialog ("Это очень простой тип - переменная bool может хранить лишь одно из двух значений: true или false. Оба этих слова, кстати, являются константами.\n");
		Step_Add_Dialog_Continue ("Единственными возможными в типе bool константами.\n");
		Step_Add_Dialog_Important ("<t>bool</t> b;");
		Step_Add_Dialog ("Переменная объявляется по стандартному шаблону: ТИП_ДАННЫХ ИМЯ_ПЕРЕМЕННОЙ;\n");
		Step_Add_Dialog_Important_Continue ("\nb = true;\n<t>bool</t> i = <c>BOT</c>.<m>GetIndicatorState</m>();");
		Step_Add_Dialog_Continue ("Присваивание и использование - всё как обычно.\n");
		Step_Add_Dialog_Continue ("Математические операторы к bool не применимы.\n");
		Step_Add_Dialog_Important ("<t>bool</t> a = 10 > 5;\n<t>bool</t> a = x == 17;");
		Step_Add_Dialog ("Но есть у этого типа одна забавная особенность - результат логических выражений (типа a > 5 или b == 3, мы их уже использовали в if и while) - это тоже тип bool. А значит результат таких выражений можно <i>присвоить</i> переменной этого типа.\n");
		Step_Add_Dialog_Continue ("Не то, что бы это было мега полезно на данном этапе, просто знайте, что так можно.\n");
		Step_Add_Dialog_Important ("<t>bool</t> alive = <c>BOT</c>.<m>IsAlive</m>();\n<instr>if</instr> (alive == true) { <cmt>//do something</cmt> }\n<instr>if</instr> (alive) { <cmt>//exactly the same, as above</cmt> }");
		Step_Add_Dialog ("Ещё одна прикольная особенность: поскольку результат сравнения, это тип bool, то, чтобы использовать в сравнении непосредственно сам тип bool, не обязательно расписывать полностью if (a == true). Достаточно просто - if (a).\n");
		Step_Add_Dialog_Continue ("Можно и так и так, но второй вариант тупо короче.\n");
		Step_Add_Dialog ("Вместо сравнений можно использовать и константы.\n");
		Step_Add_Dialog_Important ("if (true) { ... }\nif (false) { ... }");
		Step_Add_Dialog_Continue ("Вот два if-а, один из них будет выполнятся <i>ВСЕГДА</i>, а второй <i>НИКОГДА</i>.\n");
		Step_Add_Dialog_Continue ("Практической пользы в этом нет вообще.\n");
		Step_Add_Dialog_Continue ("Разве что, для каких-то отладочных целей, когда нужно дебагать код внутри if-а, условие которого срабатывает раз в сто лет...\n");
		Step_Add_Dialog_Important ("<instr>while</instr> (true) \n{\n     <cmt>//some code</cmt>\n}");
		Step_Add_Dialog ("А вот практическая польза от использовании константы в условии цикла есть.\n");
		Step_Add_Dialog_Continue ("Это, так называемый, вечный цикл. Он не остановится никогда, пока программа не будет закрыта.\n");
		Step_Add_Dialog_Continue ("Такие иногда нужны, когда не получается в одном выражении описать условие остановки...\n");
		Step_Add_Dialog_Continue ("Но это оставим на потом.\n");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>SetSwitchState</m>(<t>bool</t> state_to_set);");
		Step_Add_Dialog ("Чтобы включить или выключить переключатель перед собой, у меня есть метод <c>BOT</c>.<m>SetSwitchState</m>(<t>bool</t> state_to_set);.\n");
		Step_Add_Dialog_Continue ("Что означает, что у этого метода есть один аргумент, типа bool, под названием 'state_to_set'.\n");
		Step_Add_Dialog_Continue ("И, очевидно, что он отвечает за то, будет ли переключатель включён или выключен.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>bool</t> <c>BOT</c>.<m>GetIndicatorState</m>();");
		Step_Add_Dialog_Continue ("Кстати, слово 'bool' перед названием функции, означает, что она <i>возвращает</i> данный тип.");
		Step_Add_Dialog ("Ну что, попробуем доехать до квадратика?\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && BOT.Position.x = 0 && BOT.Position.z = 3");
		Step_Add_LevelComplete("Level 1-6a - Bool 1.", "Level 001-06a");

		//Level 1-6b - variables bool 2 - logical expressions
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Dialog ("Это юнит-тест.\n");
		Step_Add_Dialog_Continue ("Для сложных программ часто пишут тесты отдельных их компонентов, чтобы автоматически проверять, не сломалось ли что-нибудь во время починки чего-то другого.\n");
		Step_Add_Dialog_Continue ("Я - сложная программа, и это, как раз один из таких тестов.\n");
		Step_Add_Dialog_Continue ("Конкретно этот, тестирует мою логику.\n");
		Step_Add_Dialog ("Логические выражения - это те самые условия, которые мы пишем в операторах if и while, и которые можно засунуть в переменные типа bool.\n");
		Step_Add_Dialog_Continue ("Но тут вам понадобятся ещё и логические операторы.\n");
		Step_Add_Dialog ("Я уже говорил, что тип bool нельзя сложить или разделить друг на друга, поэтому всякие +, -, *, / тут не работают.\n");
		Step_Add_Dialog_Continue ("Однако, вместо этих <i>математических</i> операторов, у bool есть три <i>логических</i> - 'И', 'ИЛИ' и 'НЕ'.\n");
		Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">&&</color> - И\n<color=\"#A0A0ffff\">||</color> - ИЛИ\n<color=\"#A0A0ffff\">!</color> - НЕ");
		Step_Add_Dialog_Continue ("Выглядят они вот так.\n");
		Step_Add_Dialog ("Допустим, нам надо выполнить условие, если x лежит в промежутке между 5ю и 10ю.\n");
		Step_Add_Dialog_Continue ("Иными словами, если \"x ≻= 5и _И_ x ≺= 10\".\n");
		Step_Add_Dialog_Important ("if (x ≻= 5 && x ≺= 10) { <cmt>//some code</cmt> }");
		Step_Add_Dialog_Continue ("Вот так это выглядит в коде.\n");
		Step_Add_Dialog_Important_Continue ("\nif (x ≺ 5 || x ≻ 10) { <cmt>//some code</cmt> }");
		Step_Add_Dialog ("ИЛИ работает схожим образом - если нам нужно условие, где x не лежит в промежутке между 5ю и 10ю, то есть если \"x ≺ 5и _ИЛИ_ x >10и\", то получится вот это\n");
		Step_Add_Dialog ("С НЕ вообще всё просто - это унарный оператор, т.е. который работает не с двумя а только с одним операндом. \n");
		Step_Add_Dialog_Important ("!true = false\n!false = true\nbool b = false;\nif (!b) { <cmt>//some code</cmt> }");
		Step_Add_Dialog_Continue ("И он просто переворачивает операнд: если было true - станет false, если было false - станет true.\n");
		Step_Add_Dialog ("Как и в математических выражениях, в логических тоже можно использовать скобки, что бы отделить части большего выражения друг от друга, и указать какие части должны выполнится первыми.\n");
		Step_Add_Dialog_Important ("if ( (x ≻=5 && x ≺=10) || (x ≻= 105 && x ≺= 110 ) )");
		Step_Add_Dialog_Continue ("Вот это большое и страшное выражение, сработает если x лежит в промежутке между 5ю и 10ю или 105ю и 110ю.\n");
		Step_Add_Dialog_Continue ("То есть сначала проверяется первая часть, потом вторая, и по скольку обе этих части связаны оператором _ИЛИ_, то если <i>хоть одна из них истинна</i> - всё условие	тоже будет истинным.\n");
		//Step_Add_Dialog_Important_Continue ("\nif ( x ≻=5 && x ≺=10 || x ≻= 105 && x ≺= 110 )");
		//Step_Add_Dialog ("Что бы было, если бы мы не использовали скобки?\n");
		//Step_Add_Dialog_Continue ("Допустим возьмём х = 150, который не находится ни в одном из двух нужных промежутков и не должен сработать.\n");
		//Step_Add_Dialog ("Без скобок, все части условия выполняются поочерёдно: \n35 ≻= 5и - да\n35 ≺= 10и - нет\n35 ≻= 105и - да");
		//Step_Add_Dialog_Continue (" -- и на этом пункте всё условие станет истинным, потому что в _ИЛИ_, если хоть один операнд true - то и всё условие true.\n");
		Step_Add_Dialog ("Теперь немного о самом тестировании.\n");
		Step_Add_Dialog_Continue ("Терминал даст 10 задач, по 10 вопросов в каждой задаче. \n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n" + template_001_006b.Replace("\r", "") + "\n\n}");
		Step_Add_Dialog_Continue ("Вот описание задач.\n");
		Step_Add_Dialog ("На каждый вопрос терминал даст несколько цифр и нужно ответить ему, верно ли поставленное условие для этих цифр или нет.\n");
		Step_Add_Dialog_Important ("<t>var</t> data = <c>BOT</c>.<m>Terminal_Read</m>();");
		Step_Add_Dialog ("Чтобы считать с терминала цифры, используем функцию BOT.Terminal_Read(), которая возвращает структуру.\n");
		Step_Add_Dialog_Continue ("<i>Мы пока, конечно, не знаем, что такое структура, но, надеюсь, это нас не остановит.</i>\n");
		Step_Add_Dialog_Important_Continue("\n<t>int</t> a = data.A; <t>int</t> b = data.B;\n<t>int</t> c = data.C; <t>int</t> d = data.D;");
		Step_Add_Dialog ("Забрать из структуры нужные числа можно так.\n");
		Step_Add_Dialog ("После этого, надо ответить терминалу, верно ли условие текущей задачи для данных цифр.\n");
		Step_Add_Dialog_Important ("<c>BOT</c>.<m>Terminal_Answer</m>( <t>bool</t> responce );");
		Step_Add_Dialog_Continue ("Делаем это с помощью метода BOT.Terminal_Answer(bool responce).\n");
		Step_Add_Dialog ("Я могу выдать вам темплейт для ответов терминалу.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n" + template_001_006b2.Replace("\r", "") + "\n\n}");
		Step_Add_Dialog_Continue ("ХОБА!\n");
		Step_Add_Dialog_Continue ("Теперь вам остаётся только написать условие, для каждой задачи, что бы сформировать ответ терминалу.\n");
		Step_Add_Dialog ("Поехали.\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Dialog_Important ("<color=\"#A0A0ffff\">&&</color> - И ( a && b = true если и a и b = true)\n<color=\"#A0A0ffff\">||</color> - ИЛИ ( a || b = true если или a или b = true)\n<color=\"#A0A0ffff\">!</color> - НЕ ( если a = false --- !a = true\n            если a = true --- !a = false )");
		Step_Add_Check_Requirement ("TERMINAL");
		Step_Add_LevelComplete("Level 1-6b - Bool 2.", "Level 001-06b");

		//Level 1-7 - arrays
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		Step_Add_Dialog ("В этом уровне 8 индикаторов и один рубильник в конце.\n");
		Step_Add_Dialog_Continue ("Когда рубильник будет включён, уровень перевернётся, и станут доступны ещё восемь рубильников.\n");
		Step_Add_Dialog_Continue ("Их надо будет выставить в состояние, соответствующее индикатору - если он горит, то включить рубильник, если не горит - то выключить.\n");
		Step_Add_Dialog_Continue ("Состояние индикаторов рандомизируется при старте скрипта.\n");
		Step_Add_Dialog ("Что-то похожее мы уже делали, но тогда, индикатор висел прямо над рубильником - мы узнавали его состояние, сразу переключали рубильник и шли к следующему.\n");
		Step_Add_Dialog_Continue ("А теперь, нужно будет сначала запомнить состояние всех восьми рубильников, перевернуть уровень, включив главный рубильник, и только потом попереключать рубильники в соответствии с запомненными состояниями индикаторов.\n");
		Step_Add_Dialog ("В чём прикол? А в том, что вам понадобится в цикле запоминать восемь <b>разных</b> значений: в каждой итерации цикла - новое значение.\n");
		Step_Add_Dialog_Continue ("Я очень хотел бы посмотреть, как вы будете создавать в каждой новой итерации новую переменную. И я имею в виду именно <i>новую</i>, а не перезаписывать старую, ведь тогда, то, что в ней было, потеряется...\n");
		Step_Add_Dialog_Continue ("Не, ну, то есть, можно, конечно, написать ~20и килобайтный код, который будет делать это всё вообще без циклов, но это слишком казуально.\n");
		Step_Add_Dialog ("Тут поможет такая штука, как массивы (анг. Array - множество).\n");
		Step_Add_Dialog_Continue ("Это, собственно говоря, способ запихнуть в одну переменную сразу несколько значений и обращатся к ним по индексу.\n");
		Step_Add_Dialog_Continue ("Ну то есть у вас в итоге будет одна переменная, скажем \"x\", но в ней будет храниться несколько отдельных элементов: x[1], x[2], x[3]... и т.д., сколько захотите.\n");
		Step_Add_Dialog ("Как и переменные, массивы строготипизированы, то есть в массиве типа int все элементы будут int, и в него нельзя запихать элементы bool или float.\n");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array_of_int;\n<t>bool</t>[] b;\n<t>float</t>[] f_arr;");
		Step_Add_Dialog_Continue ("Объявляются они почти как и обычные переменные, по шаблону \"тип_данных имя_переменной;\", только после типа, идут квадратные скобки.\n");
		Step_Add_Dialog ("Как и в случае с переменными, сразу после объявления, они, как бы, уже существуют, но не инициализированы, т.е. их значение равно <i>null</i> - <i>ничто</i>.\n");
		Step_Add_Dialog_Continue ("Любое обращение к такому массиву, как и к обычной неинициализированной переменной, приведёт к ошибке <i>null reference exception</i>.\n");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array_of_int;\nmy_array_of_int = <instr>new</instr> <t>int</t>[8];");
		Step_Add_Dialog ("Инициализируются массивы вот так.\n");
		Step_Add_Dialog_Important_Continue("\n\n<t>bool</t>[] b_arr = <instr>new</instr> <t>bool</t>[8];");
		Step_Add_Dialog_Continue ("Или можно сразу, во время объявления, вот так.\n");
		Step_Add_Dialog_Continue ("В обоих случаях будет создан массив, размером в 8 элементов. В первом случае это будет массив int-ов, а во втором - массив bool-ов.\n");
		Step_Add_Dialog_Continue ("Весь массив, в этом случае будет заполнен <i>значениями типа по умолчанию</i>: для всех числовых типов (int, float, double, decimal) - это 0, а для bool-а - false.\n");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array = <instr>new</instr> <t>int</t>[8];");
		Step_Add_Dialog ("Короче говоря, в данном примере, после инициализации, мы получаем массив размером в 8 элементов, забитый нулями.\n");
		Step_Add_Dialog_Important_Continue("\n<t>int</t> a = my_array[3];");
		Step_Add_Dialog_Continue ("Чтобы получить n-ный элемент, просто указываем его индекс в скобках.\n");
		Step_Add_Dialog_Important_Continue ("\n<c>BOT</c>.<m>Move</m>( my_array[5] );");
		Step_Add_Dialog_Continue ("Как и переменную, элемент массива можно сразу использовать в аргументах функций и методов.\n");
		Step_Add_Dialog_Important_Continue ("\n<c>BOT</c>.<m>Rotate</m>( my_array[5] * 2 );");
		Step_Add_Dialog_Continue ("Также можно использовать элементы в арифметических выражениях (или, в случае с bool - в логических).\n");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array = <instr>new</instr> <t>int</t>[8];\n<t>int</t> a = 5;\n<t>int</t> b = my_array[a];");
		Step_Add_Dialog ("Для доступа к n-ному элементу не обязательно использовать константу - подойдёт и другая переменная типа int. Эта особенность просто неоценимо полезна в циклах.");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array = <instr>new</instr> <t>int</t>[8];");
		Step_Add_Dialog ("Однако - и это важно помнить - при инициализации, мы указываем <i>количество</i> элементов в массиве, но индексация начинается с нуля, по этому самый последний элемент этого массива из 8и элементов - это my_array[7].\n");
		Step_Add_Dialog_Continue ("Потому что у него есть элементы 0, 1, 2, 3, 4, 5, 6 и 7. А всего их получается 8, можете пересчитать, я подожду.\n");
		Step_Add_Dialog_Continue ("Магия!\n");
		Step_Add_Dialog ("При попытке обратиться к элементу с индексом 8, закономерно получим ошибку \"index was outside the bounds of the array\".\n");
		Step_Add_Dialog_Important_Continue ("\nmy_array[0] = 42;\nmy_array[3] = 771;");
		Step_Add_Dialog ("Несложно догадаться, что чтобы присвоить элементу какое-то значение, нужно не забыть указать его индекс.\n");
		Step_Add_Dialog_Important_Continue ("\nmy_array = 321 <cmt>//This will throw an error;</cmt>");
		Step_Add_Dialog_Continue ("Попытавшись присвоить что-нибудь не отдельному элементу массива, а самому массиву, мы получим ошибку типа \"can not implicitly convert int to int[]\", т.е. \"не могу преобразовать число в массив чисел, в самом деле, где это видано, совсем сдурели что ли?\" (вольный перевод - Р.О. Бот).\n");
		Step_Add_Dialog ("Ну и посленее, что можно сказать про массивы - если очень хочется, можно присвоить всем элементам значения прямо во время инициализации.\n");
		Step_Add_Dialog_Important ("<t>int</t>[] my_array_of_int = <instr>new</instr> <t>int</t>[4]{ 24, 72, 36, 52 };\n<t>bool</t>[] bool_arr = <instr>new</instr> <t>bool</t>[2]{ true, false };");
		Step_Add_Dialog_Continue ("Для этого, после инициализации, в фигурных скобках указываем все элементы через запятую.\n");
		Step_Add_Dialog_Continue ("Разумеется, их количество должно соответствовать указанному размеру массива. Иначе, что? Правильно - ошибка.\n");
		Step_Add_Dialog ("Размер массива нельзя изменить после того как он инициализирован. Соответственно добавить или удалить из него элементы не получится.\n");
		Step_Add_Dialog_Important_Continue ("\nmy_array_of_int = <instr>new</instr> <t>int</t>[30];");
		Step_Add_Dialog_Continue ("Его можно заново переинициализировать, с новым размером, но все старые данные из него пропадут, т.к. это будет уже <i>новый</i> массив, о чём прозрачно намекает ключевое слово <i><instr>new</instr></i> при инициализации.\n");
		Step_Add_Dialog ("Ну вроде информации достаточно. Поехали.\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("Switch");
		Step_Add_LevelComplete("Level 1-7 - Массивы", "Level 001-07");

		//Level 1-7b - arrays multidimensional
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		Step_Add_Dialog ("Хорошо. Значит тут у нас 27 индикаторов - 3х3х3.\n");
		Step_Add_Dialog_Continue ("Как обычно, сначала нужно снять с них со всех показания. Затем, дёрнуть за главный рубильник, который заменит все индикаторы на рубильники, и, наконец, выставить свежепоявившиеся рубильники в состояние, соответствующих им индикаторов.\n");
		Step_Add_Dialog ("Я стою на платформе, которая умеет меня телепортировать по нужным координатам с помощью метода <c>Platform</c>.<m>MoveTo</m>(<t>int</t> x, <t>int</t> y, <t>int</t> z);\n");
		Step_Add_Dialog_Continue ("Доступные координаты - от 0,0,0 (правый-нижний-передний индикатор) до 2,2,2 (левый-верхний-задний)\n");
		Step_Add_Dialog_Continue ("Плюс ещё надо будет портнуться на изначальную координату -1,1,1 что бы повернуть главный рубильник.\n");
		Step_Add_Dialog ("В принципе, всё это не очень сложно сделать и с обычным массивом, но тут у нас вложенные циклы вырисовываются (ну, что бы перемещатся по координатам), а в них у нас не получится организовать счётчик от нуля до 26и.\n");
		Step_Add_Dialog_Continue ("Вместо этого там получится три счётчика - от нуля до 2х каждый.\n");
		Step_Add_Dialog ("Перевести три счётчика 0-2 в один 0-26 дело не хитрое: \"<t>int</t> counter = x*9 + y*3 + z;\". Но давайте попробуем решить эту задачу более удобным способом.\n");
		Step_Add_Dialog ("Для таких случаем у нас есть многомерные массивы.\n");
		Step_Add_Dialog_Continue ("Даже песня такая есть: \"Мно-го-мер-ны-е мас-си-вы, тум-да-да-да...\"\n");
		Step_Add_Play_Sound(0, 0);
		Step_Add_Wait(5f);
		Step_Add_Restore_Music_Volume();
		Step_Add_Dialog_Continue ("Ну, почти попал...\n");
		Step_Add_Dialog ("Так, что это вообще такое?\n");
		Step_Add_Dialog_Continue ("Ну, если для обычного, одномерного массива, нам нужно указать одну циферку - индекс - что бы добраться до содержимого, то в двумерном нужно будет две циферки (два индекса).\n");
		Step_Add_Dialog_Continue ("В трёх-мерном - три, в четырёх-мерном - четыре, и так далее.\n");
		Step_Add_Dialog_Continue ("Нет предела количеству измерений у массива - есть лишь адекватность программиста.\n");
		Step_Add_Dialog ("Вообще-то предел измерений есть - 32. Но это настолько до хрена, что в жизни ни кому никогда не понадобится.\n");
		Step_Add_Dialog_Continue ("3х-мерные массивы уже встречаются довольно редко. 4х-мерные - <i>крайне</i> редко. А если у вас в программе используются 5и, и более -мерные, велика вероятность что вашей программе нужно пройти медосмотр и порефакториться...\n");
		Step_Add_Dialog ("Если одномерный массив <t>int</t>[] - это ряд чисел, то двумерный можно представить как таблицу. Трёхмерный - как кубик. А четырёхмерный - как хороший способ сожрать всю память одной командой, и не подавится.\n");
		Step_Add_Dialog_Continue ("На самом деле, четырёхмерный массив сложно визуализировать, но по сути это просто означает, что для доступа к его элементу, вам нужно указать четыре индекса.\n");
		Step_Add_Dialog ("Объявляется многомерный массив так-же, как и одномерный, только в квадратных скобках ставятся запятые, соответствющие количеству измерений минус один.\n");
		Step_Add_Dialog_Important ("<t>int</t>[,] array_2_dimensions;");
		Step_Add_Dialog_Continue ("Т.е. <i>ноль</i> запятых для одномерного массива, одна запятая для двухмерного - как в примере ниже, \n");
		Step_Add_Dialog_Important_Continue ("\n<t>int</t>[,,] array_3_dimensions;");
		Step_Add_Dialog_Continue ("две - для трёх-мерного, три - для четырёхмерного, и т.д...\n");
		Step_Add_Dialog_Important ("<t>int</t>[,] a = new int[2,3] { { 10, 0, 12 }, { 13, 45, 72 } };\n<cmt>//Получим следующее:</cmt>\n<cmt>//a[0,0] = 10, a[0,1] = 0, a[0,2] = 12</cmt>\n<cmt>//a[1,0] = 13, a[1,1] = 45, a[1,2] = 72</cmt>");
		Step_Add_Dialog ("А вот так, можно заполнить многомерный массив значениями... можно я не буду это комментировать? Просто посмотрите на пример.\n");
		Step_Add_Dialog_Continue ("Грубо говоря, заполняем сначала первый элемент певого измерения, затем второй его элемент.\n");
		Step_Add_Dialog_Continue ("По аналогии с таблицей - сначала первую строчку, затем вторую. Где каждая из строчек - это три столбца (одномерный массив из 3х элементов).\n");
		Step_Add_Dialog ("Схожим образом можно заполнить и трёх-мерный, и 10-и мерный, но уже на 3х мерном вы окончательно запутаетесь в этих фигурных скобках.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n  int[,,] arr = new int[5,9,14];\n  for(int a=0; a<5; a++) {\n    for(int b=0; b<9; b++) {\n      for(int c=0; c<14; c++) {\n        arr[a,b,c] = Random.Range(0, 500);\n      }\n    }\n  }\n\n}");
		Step_Add_Tutorial_Arrow_Show (-560f, 8f);
		Step_Add_Dialog_Continue ("Такие массивы проще заполнять во вложенных циклах.\n");
		Step_Add_Dialog ("Что-ж, пора рубить рубильники, а то они ждать устали.\n");
		Step_Add_Dialog_Important ("<t>int</t>[,] arr1 = new int[2,3];\n<t>int</t>[,,] arr2 = new int[3,7,2];");
		Step_Add_Tutorial_Arrow_Hide ();
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("Switch");
		Step_Add_LevelComplete("Level 1-7b - Массивы многомерные", "Level 001-07b");

		//Level 1-7c - jagged arrays
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		Step_Add_Dialog ("В этом уровне против нас играет хитрый пол с картошкой.\n");
		Step_Add_Dialog_Continue ("Каждый раз, когда скрипт запускается - конфигурация пола и количество картошки на нём изменяется.\n");
		Step_Add_Dialog_Continue ("Всё что нужно сделать - это прочесать весь пол, собрать данные о количестве картошки на каждой клетке, и сообщить эту информацию терминалу.\n");
		Step_Add_Dialog_Important ("<t>float</t> <c>BOT</c>.<m>GetFloorSquare</m>();");
		Step_Add_Dialog ("Прочесать такой нестандартный пол, нам поможет знакомая функция GetFloorSquare().\n");
		Step_Add_Dialog_Continue ("Напоминаю, что она возвращает площадь пола, на котором я стою. \n");
		Step_Add_Dialog_Continue ("В данном случае, поскольку каждый фрагмент пола одного цвета - это отдельный пол, шириной в 1 метр, то по сути, данная функция вернёт <i>длинну</i> текущего отрезка пола.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>int</t> <c>BOT</c>.<m>CheckPotatoes</m>();");
		Step_Add_Dialog_Continue ("А количество картошки на клетке подо мной, нам возвращает функция <c>BOT</c>.<m>CheckPotatoes</m>().\n");
		Step_Add_Dialog ("Осталось, стало быть решить, как мы будем хранить информацию о количестве картошки.\n");
		Step_Add_Dialog ("Всего отрезков пола 5 - это видно: зелёный, жёлтый, красный, синий... и ещё один зелёный - у дизайнера цвета кончились.\n");
		Step_Add_Dialog_Continue ("Длинна каждого отрезка будет каждый раз разной.\n");
		Step_Add_Dialog_Continue ("Можно было бы взять обычный двухмерный массив <t>int</t>[5,100] - что б с запасом. Не может же фрагмент пола быть длинной больше чем 100!\n");
		Step_Add_Dialog_Continue ("Или может?...\n");
		Step_Add_Dialog_Continue ("Да не, ерунда...\n");
		Step_Add_Dialog ("Но есть проблема: здешний терминал хочет получить от нас в качестве ответа зубчатый массив, и никакой другой. На простой двухмерный массив он будет ругаться.\n");
		Step_Add_Dialog_Continue ("Ну что, погнали?\n");
		Step_Add_Dialog_Continue ("А, ну да, я ж забыл объяснить...\n");
		Step_Add_Dialog ("Зубчатый массив (jagged array), это, по сути массив массивов.\n");
		Step_Add_Dialog_Continue ("Т.е. одномерный массив, каждый элемент которого - это другой одномерный массив.\n");
		Step_Add_Dialog_Continue ("Таким образом, мы получаем функционал такой же как и у двухмерного массива, но с той разницей, что в зубчатом массиве, каждая строка таблицы - если проводить аналогию с таблицей - может иметь разное количество столбцов.\n");
		Step_Add_Dialog_Continue ("При больших объёмах данных это может сильно сэкономить объём используемой памяти.\n");
		Step_Add_Dialog_Important ("<t>int</t>[][] arr;");
		Step_Add_Dialog ("Объявляется такая штука вот таким образом. Также, как и обычный массив, только вместо одной пары квадратных скобок ставим две.\n");
		Step_Add_Dialog_Continue ("Ну, или можно три - что бы получить 3х-мерный зубчатый массив. Можно пять, десять, двадцать - максимальный ранг ограничен реализацией .net framework, но до 32х можно вроде бы везде.\n");
		Step_Add_Dialog_Continue ("В любом случае, это уже далеко за пределами нашей текущей задачи.\n");
		Step_Add_Dialog ("А вот инициализировать такой массив уже хитрее, чем обычный.\n");
		Step_Add_Dialog_Important ("<t>int</t>[][] arr = new <t>int</t>[3][];");
		Step_Add_Dialog_Continue ("При объявлении, мы можем инициализировать только первое измерение - типа, количество строк в таблице.\n");
		Step_Add_Dialog_Important_Continue ("\narr[0] = new <t>int</t>[3];\narr[1] = new <t>int</t>[17];\narr[2] = new <t>int</t>[9];");
		Step_Add_Dialog_Continue ("Поскольку каждый элемент такого массива, это другой, независимый массив, и все элементы могут быть разного размера, то инициализировать каждый элемент нужно отдельно.\n");
		Step_Add_Dialog_Important ("<t>int</t>[][] arr = new <t>int</t>[2][];\narr[0] = new <t>int</t>[4]{47,16,38,19};\narr[1] = new <t>int</t>[2]{91,37};");
		Step_Add_Dialog ("Вот так можно сразу запихнуть в массив данные.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>int</t> x = arr[0][3];\narr[1][0] = 76;");
		Step_Add_Dialog_Continue ("И вот так обратиться к отдельным элементам - в первых квадратных скобках указываем строку, во вторых - столбец.\n");
		Step_Add_Dialog ("Вроде всё. Поехали!\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("TERMINAL");
		Step_Add_LevelComplete("Level 1-7c - Массивы зубчатые", "Level 001-07c");

		//Level 1-8 - structures
		Step_Add_Dialog ("Ну вот и настало время твоего первого боя.\n");
		Step_Add_Dialog_Continue ("Твой враг будет агрессивен и очень силён. От его шагов меркнет небо и армии разбегаются в страхе.\n");
		Step_Add_Dialog_Continue ("Узри же, великого...");
		Step_Add_Play_Sound(0);
		Step_Add_Wait(2f);
		Step_Add_Play_Sound(1);
		Step_Add_Activate_Level_Object (0);
		Step_Add_Dialog ("Окей, это просто бомба.");
		Step_Add_Dialog ("К сожалению, чтобы пройти дальше, её, таки, придётся уничтожить.");
		Step_Add_Dialog ("Итак, особенности бомбы:\n");
		Step_Add_Dialog ("- Фигачить бомбу, как и любого врага, можно методом <c>BOT</c>.<m>Fight</m>(). Для этого, нужно стоять на клетке перед врагом и быть направленным в его сторону.\n");
		Step_Add_Dialog_Continue ("- Количество единиц здоровья бомбы заранее не известно (каждый раз, когда скрипт запускается - оно меняется).\n");
		Step_Add_Dialog_Continue ("- Когда у бомбы заканчивается здоровье, она немного ждёт и делает БУМ.\n");
		Step_Add_Dialog_Continue ("- В момент бума, лучше не стоять рядом с ней. Все роботы, в радиусе трёх клеток от БУМкающей бомбы, трагически погибают.\n");
		Step_Add_Dialog_Important ("<instr>while</instr> ( <color=\"#A0A0ffff\">true</color> )");
		Step_Add_Dialog ("Предлагаю сделать беcконечный цикл, в теле которого - фигачить бомбу и проверять её здоровье.");
		Step_Add_Dialog_Important_Continue ("\n{\n  <cmt>//фигачить бомбу и проверять здоровье;</cmt>\n  <instr>if</instr> (здоровье ≺= 0) { <instr>break</instr>; }\n}");
		Step_Add_Dialog ("В момент, когда её здоровье станет меньше или равно нулю - выйти из цикла, и свалить.\n");
		Step_Add_Dialog_Continue ("Для выхода из цикла есть команда <instr>break</instr>.\n");
		Step_Add_Dialog_Continue ("Проблема, стало быть, лишь в том, чтобы узнать здоровье бомбы.");
		Step_Add_Dialog ("Тут поможет функция <c>BOT</c>.<m>GetClosestEnemyInfo</m>() - она возвращает информацию о ближайшем враге.\n");
		Step_Add_Dialog_Continue ("Тип возвращаемого этой функцией значения - это структура EnemyInfo.\n");
		Step_Add_Dialog_Important ("<t>struct</t> <c>EnemyInfo</c> {\n  <t>int</t> HP; <cmt>//Здоровье</cmt>\n  <t>int</t> EP; <cmt>//Заряд</cmt>\n  <t>float</t> pos_x <cmt>//координата врага x на поле</cmt>\n  <t>float</t> pos_y <cmt>//координата врага y на поле</cmt>\n}");
		Step_Add_Dialog_Continue ("Структура - это всего лишь несколько переменных объединённых в одну.\n");
		Step_Add_Dialog ("Как видно из объявления, в структуре есть здоровье врага. И хранится оно в переменной HP типа int.");
		Step_Add_Dialog_Important ("<c>EnemyInfo</c> en = <c>BOT</c>.<m>GetClosestEnemyInfo</m>();\n<t>int</t> health = en.HP;");
		Step_Add_Dialog ("Пользоваться этим можно так: \n- объявляем переменную типа EnemyInfo, присваиваем ей информацию о текущем враге, а чтобы узнать здоровье, обращаемся к нужной переменной структуры через точку.\n");
		Step_Add_Dialog_Important_Continue ("\n\n<cmt>//или так</cmt>\n<t>int</t> health = <c>BOT</c>.<m>GetClosestEnemyInfo</m>().HP;");
		Step_Add_Dialog_Continue ("Ну, или, если вся структура не нужна, можно сразу запросить здоровье из функции, без промежуточной переменной. Также, через точку.");
		Step_Add_Dialog ("Если использовать первый способ, то можно подумать, что достаточно получить структуру EnemyInfo вне цикла, а в теле цикла лишь проверять en.HP. Но это не сработает.\n");
		Step_Add_Dialog_Continue ("Цифры в полученной структуре сами по себе менятся не будут. Соответственно условие en.HP ≺= 0 не сработает никогда, и я буду вечно дубасить бомбу, пока она меня не взорвёт.");
		Step_Add_Dialog ("Чтобы обновить структуру, нужно её переполучить с помощью GetClosestEnemyInfo(), которая вернёт актуальную структуру на момент выполнения команды.\n");
		Step_Add_Dialog_Continue ("Ну, или просто воспользоватся вторым способом.");
		Step_Add_Dialog ("Вперёд!");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		Step_Add_LevelComplete("Level 1-8 - Structures - First Battle.", "Level 001-08");

		//Level 1-9 - functions
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		Step_Add_Dialog ("Ой, сколько бомб...\n");
		Step_Add_Dialog_Continue ("Ну, в целом, ничего нового.\n");
		Step_Add_Dialog_Continue ("Ездим, разминируем, любуемся пейзажем...\n");
		Step_Add_Dialog_Continue ("В цикле это сделать не получится, потому что ехать нужно каждый раз в разное место, и этот код не получится зациклить.\n");
		Step_Add_Dialog_Continue ("В итоге, тут получится довольно много кода. Причём та часть, в которой мы дубасим бомбу, пока она не загорится, будет повторена 4 раза, в абсолютно одинаковом виде.\n");
		Step_Add_Dialog_Continue ("Нехорошо.");
		Step_Add_Dialog ("Ты знаешь, ты можешь писать свои методы и функции. Они помогают избавится от повторений одинакового кода.\n");
		Step_Add_Dialog_Continue ("До сих пор, весь код ты тоже писал внутри функции. Это функция Start().\n");
		Step_Add_Tutorial_Arrow_Show ( -560f, 1f ); //Top - Padding - LineHeight - LineSpace - HalfLineHeight
		Step_Add_Dialog_Continue ("Вот это - её определение.");
		Step_Add_Dialog ("Ключевое слово \"void\" означает, что функция ничего не возвращает, т.е. является методом.\n");
		Step_Add_Dialog_Continue ("Пустые скобки означают, что у функции нет параметров.\n");
		Step_Add_Tutorial_RedPanel_Show ( 0f, 3f, 100f, 10f );
		Step_Add_Dialog_Continue ("Ну, а дальше, между фигурных скобок, собственно, идёт тело функции, т.е. код, который выполняется при её вызове.");
		Step_Add_Tutorial_Arrow_Hide();
		Step_Add_Dialog ("Функция Start() вызывается автоматически, при запуске скрипта. Это, так называемый, EntryPoint - место, с которого начинает выполнятся программа, при её запуске.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n\n\n\n\n\n\n}");
		Step_Add_Tutorial_RedPanel_Hide();
		Step_Add_Tutorial_Arrow_Show ( -560f, 13f );
		Step_Add_Dialog_Continue ("Но, никто не мешает, по образу данной функции, создать свою. Скажем KillBomb().");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		Step_Add_Dialog ("Внутрь мы запихнём код, который будет взрывать бомбу.\n");
		Step_Add_Tutorial_Arrow_Show ( -560f, 4f ); //Top - Padding - LineHeight - LineSpace - HalfLineHeight		
		Step_Add_SetScript ("\nvoid Start () \n{\n  //Go to bomb\n  KillBomb(); //This call our function and kill the bomb\n\n\n\n\n\n\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		Step_Add_Dialog_Continue ("В основном же коде, нам останется только подойти к бомбе, и вызвать нашу новую функцию.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n  //Go to bomb 1\n  KillBomb(); //This call our function and kill the bomb\n  //Go to bomb 2\n  KillBomb();\n  //Go to bomb 3\n  KillBomb();\n  //Go to bomb 4\n  KillBomb();\n}\n\nvoid KillBomb() \n{\n  while (true)\n  {\n    BOT.Fight();\n    //Get bomb HP somehow\n    if (HP <= 0) { break; }\n  }\n}");
		Step_Add_Dialog_Continue ("И повторить четыре раза.\n");
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n}");
		Step_Add_Tutorial_Arrow_Hide();
		Step_Add_Dialog_Continue ("Попробуй.");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		Step_Add_LevelComplete("Level 1-9 - Functions.", "Level 001-09");

		//Level 1-10 - class string
		Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		Step_Add_Dialog ("Лежебоке Лодыревне Спать, менеджеру отдела по развитию коррупции, активистке, атеистке, феминистке, арфистке, химчистке и просто, хорошему человеку, приходит на почту много спама.\n");
		Step_Add_Dialog_Continue ("Она попросила помочь обработать почту в целом, и отфильтровать спам в частности.\n");
		Step_Add_Dialog ("Почта будет выдаваться с помощью уже знакомого терминала.\n");
		Step_Add_Dialog_Continue ("Терминал выдаст 10 рандомных е-мейлов, которые надо будет обработать.\n");
		Step_Add_Dialog ("Забираем из терминала структуру с помощью <t>var</t> data = <c>BOT</c>.<m>Terminal_Read</m>();\n");
		Step_Add_Dialog_Continue ("Вытаскиваем из неё письмо: <t>string</t> mail = data.str;\n");
		Step_Add_Dialog_Continue ("Обрабатываем это письмо, и отвечаем терминалу с помощью <c>BOT</c>.<m>Terminal_Answer</m>(<t>string</t>[] parsed_mail);\n");
		Step_Add_Dialog_Continue ("(Да, в этот раз нужно будет отвечать терминалу массивом <t>string</t>[] из двух элементов, что в них должно быть - я объясню).\n");
		Step_Add_Dialog_Continue ("И всё вот это вот, нужно сделать в цикле 10 раз. Но не заморачивайтесь - вот темплейт.\n");
		Step_Add_SetScript (template_001_010.Replace("\r", "") + "\n");
		Step_Add_Dialog ("В темплейт уже вписаны обращения к терминалу, и получение е-мейла в переменную <t>string</t> mail;\n");
		Step_Add_Dialog_Continue ("А так-же ответ, массивом <t>string</t>[] parsed_mail.\n");
		Step_Add_Dialog_Continue ("Вам нужно только обработать переменную \"mail\" и поместить в \"parsed_mail\".\n");
		Step_Add_Dialog ("Краткое содержание того, что, собственно, нужно сделать с письмом:\n");
		Step_Add_Dialog_Continue ("- 1. Надо создать заголовок письма - это должны быть первые 140 символов и три точки если письмо длинее 140 символов, или всё письмо если оно короче.\n");
		Step_Add_Dialog_Continue ("- 2. Если имя или отчество Лежебоки Лодыревны начинается с маленькой буквы - заменить её на заглавную (Лежебока Лодыревна отключается, если видит своё имя/фамилию с маленькой буквы).\n");
		Step_Add_Dialog ("- 3. Нужно отфильровать почту на спам - пометить спамом все письма, содержащие 'вы выиграли' или 'одобрен кредит'. Если содержит - добавить перед заголовком строку <str>\"(СПАМ) \"</str>.\n");
		Step_Add_Dialog_Continue ("- 4. Заменяем все \"а\" на \"о\" а \"о\" на \"а\" (Лежебока Лодыревна человек своеобразный, и ей так легче читать).\n");
		Step_Add_SetScript (template_001_010_2.Replace("\r", "") + "\n");
		Step_Add_Dialog ("Вот вам все задания комментариями в темплейт.\n");
		Step_Add_Dialog_Continue ("А теперь - главное: как это всё вообще делать.\n");
		Step_Add_Dialog ("Я уже упоминал, что тип string - это вообще-то класс. А это значит, что у него есть свои методы и функции.\n");
		Step_Add_Dialog_Continue ("(Для справки: остальные простые типы - всякие int, decimal и т.д. - это структуры, которые, в принципе, не так уж сильно от классов отличаются, и тоже имеют свои методы и функции, но они не прикольные).\n");
		Step_Add_Dialog_Important ("<t>string</t> greeting = <str>\"Привет, я - БОТ.\"</str>;");
		Step_Add_Dialog ("Объявляя переменную типа <t>string</t>, и присваивая ей значение, вы создаёте <i>экземпляр</i> (instance) класса <t>string</t>, и можете обращаться к его методам через эту переменную.\n");
		Step_Add_Dialog_Important_Continue ("\ngreeting.<m>Replace</m>(<str>\"я\"</str>, <str>\"ы\"</str>);");
		Step_Add_Dialog_Continue ("Например, у класса <t>string</t> есть функция <m>Replace</m>, которая заменяет в строке одну букву (или слово), на другую букву (или слово).\n");
		Step_Add_Dialog ("Осторожно - подстава!\n");
		Step_Add_Dialog_Continue ("Все вот эти функции (как и вообще любые функции), <i>возвращают</i> результат своей работы, но не меняют исходную переменную. То есть, после выполнения кода ниже - переменная \"greeting\" не изменится!\n");
		Step_Add_Dialog_Important_Continue (" <cmt>//безполезный вызов</cmt>");
		Step_Add_Dialog_Continue ("По сути, после выполнения такого кода, вообще ничего не изменится - функция вернёт результат, но он уйдёт в никуда.\n");
		Step_Add_Dialog_Important_Continue ("\ngreeting = greeting.<m>Replace</m>(<str>\"я\"</str>, <str>\"ы\"</str>);\n<cmt>//в greeting теперь лежит - \"Привет, <i>ы</i> - БОТ.\"</cmt>");
		Step_Add_Dialog_Continue ("Что бы реально заменить все <str>\"я\"</str> на <str>\"ы\"</str> в исходной строке, присваиваем результат функции обратно нашей переменной.\n");
		Step_Add_Dialog ("Думаю тут понятно, что и первым (что меняем) и вторым (на что меняем) аргументом, может быть и буква, и символ, и набор букв или символов, и всё в перемешку... короче, любая валидная строка или другая переменная типа <t>string</t>.\n");
		Step_Add_Dialog_Important ("<t>string</t> greeting = <str>\"Привет, я - БОТ.\"</str>;\ngreeting = greeting.<m>Replace</m>(<str>\"Привет\"</str>, <str>\"\"</str>);\n<cmt>//в greeting теперт лежит - \", я - БОТ.\"</cmt>");
		Step_Add_Dialog_Continue ("Вторым аргументом может даже быть пустая строка, тогда результатом будет полное уничтожение найденных подстрок в исходной строке.\n");
		Step_Add_Dialog_Important ("<t>string</t> t = <str>\"Привет, я - БОТ.\"</str>;");
		Step_Add_Dialog ("Коротко о других функциях класса <t>string</t> (готовтесь, их не мало, но они все простые).\n");
		Step_Add_Dialog_Important_Continue ("\n<t>string</t> t1 = t.<m>ToLower()</m>; <cmt>//\"привет, я - бот.\"</cmt>");
		Step_Add_Dialog_Continue ("- <t>string</t> <m>ToLower</m>() - возвращает исходную строку в нижнем регистре.\n");
		Step_Add_Dialog_Important_Continue ("\n<t>string</t> t2 = t.<m>ToUpper()</m>; <cmt>//\"ПРИВЕТ, Я - БОТ.\"</cmt>");
		Step_Add_Dialog_Continue ("- <t>string</t> <m>ToUpper</m>() - возвращает исходную строку в верхнем регистре.\n");
		Step_Add_Dialog_Important ("<t>string</t> t = <str>\"Привет, я - БОТ.\"</str>;\n<t>bool</t> b1 = t.<m>Contains</m>(<str>\",\"</str>); <cmt>//true</cmt>\n<t>bool</t> b2 = t.<m>Contains</m>(<str>\"Привет\"</str>); <cmt>//true</cmt>\n<t>bool</t> b3 = t.<m>Contains</m>(<str>\"к\"</str>); <cmt>//false</cmt>");
		Step_Add_Dialog ("- <t>bool</t> <m>Contains</m>(<t>string</t> substring) - возвращает <t>bool</t>, содержится-ли указанная первым аргументом подстрока в исходной строке.\n");
		Step_Add_Dialog_Important ("<t>string</t> t = <str>\"Привет, я - БОТ.\"</str>;\n<t>string</t> t1 = t.<m>Substring</m>(3); <cmt>//\"вет, я - БОТ.\"</cmt>\n<t>string</t> t2 = t.<m>Substring</m>(10); <cmt>//\"- БОТ.\"</cmt>");
		Step_Add_Dialog ("- <t>string</t> <m>Substring</m>(<t>int</t> index) - возвращает подстроку, начиная с позиции <i>index</i> (индексация начинается с нуля, т.е. первый символ, на самом деле нулевой).\n");
		Step_Add_Dialog_Important_Continue ("\n<t>string</t> t3 = t.<m>Substring</m>(2, 5); <cmt>//\"ивет,\"</cmt>");
		Step_Add_Dialog_Continue ("- <t>string</t> <m>Substring</m>(<t>int</t> index, <t>int</t> count) - перегрузка функции <m>Substring</m> с двумя аргументами типа <t>int</t>. Возвращает некоторое количество (<i>count</i>) символов, начиная с позиции <i>index</i>. В примере, мы получили 5 символов, начиная с похиции 2.\n");
		Step_Add_Dialog_Important ("<t>string</t> t = <str>\"Привет, я - БОТ.\"</str>;\n<t>int</t> i = t.<m>Length</m>; <cmt>//16</cmt>");
		Step_Add_Dialog ("Ну, и на конец, свойство Length. Возвращает просто количество символов в строке.\n");
		Step_Add_Dialog_Continue ("Его фишка в том, что оно не функция. Оно - свойство. С такими зверями мы ещё не сталкивались.\n");
		Step_Add_Dialog_Continue ("Ну, с точки зрения использования, свойство отличается от функции только тем, что у свойства нет никаких аргументов, стало быть и никакие скобки указывать не надо.\n");
		Step_Add_Dialog ("Как запомнить, где свойство, а где функция?\n");
		Step_Add_Dialog_Continue ("А вот никак. Бугагагага.\n");
		Step_Add_Dialog_Continue ("Нет, серьёзно. К сожалению, это можно только либо запомнить, либо каждый раз лезть в бот-о-педию.\n");
		Step_Add_Dialog_Continue ("В будущем, подсказка кода вам будет говорить, что свойство а что функция, но пока только так...\n");
		Step_Add_Dialog ("Возвращаемся к заданиям, и попробуем разобратся как их решать.\n");
		Step_Add_Dialog_Continue ("Отдавать терминалу надо массив <t>string</t> из двух элементов, где первый элемент будет заголовком письма, а второй - самим письмом.\n");
		Step_Add_Dialog ("1. Надо создать заголовок письма - это должны быть первые 140 символов и три точки если письмо длинее 140 символов, или всё письмо если оно короче.\n");
		Step_Add_Dialog_Continue ("С помощью свойства <m>Length</m> узнаём количество букв в письме, и если получилось меньше чем 141, суём в заголовок (первый элемент результирующего массива) всё письмо. А если больше - то с помощью функции <m>Substring</m> отдираем от письма первые 140 символов, добавляем к ним \"...\", и то что получилось так-же суём в заголовок.\n");
		Step_Add_Dialog ("2. Если имя или отчество Лежебоки Лодыревны начинается с маленькой буквы - заменить её на заглавную.\n");
		Step_Add_Dialog_Continue ("Тупо, с помощью <m>Replace</m> заменяем <str>\"<b>л</b>ежебока\"</str> на <str>\"<b>Л</b>ежебока\"</str>.\n");
		Step_Add_Dialog_Continue ("Однако, это не проконает, если имя будет указано в другом падеже (<str>лежебоке</str>, например).\n");
		Step_Add_Dialog_Continue ("Не проблема - заменим не <str>\"лежебок<b>а</b>\"</str>, а <str>\"лежебо<b>к</b>\"</str>. Такая замена должна сработать с любым падежом.\n");
		Step_Add_Dialog_Continue ("Ну, и так-же с отчеством.\n");
		Step_Add_Dialog ("3. Нужно отфильровать почту на спам - если письмо содержит 'вы выиграли' или 'одобрен кредит', то добавить перед заголовком строку <str>\"(СПАМ) \"</str>.\n");
		Step_Add_Dialog_Continue ("С помощью <m>Contains</m> узнаём, есть ли в письме указанные подстроки. И если есть - с помощью конкатенации изменяем заголовок указанным образом.\n");
		Step_Add_Dialog ("Но есть ньюанс: <m>Contains</m> (как и <m>Replace</m>, кстати) - регистрозависим, и <m>Contains</m>(<str>\"вы выиграли\"</str>) не обнаружит строку <str>\"Вы выиграли\"</str>, из-за заглавной буквы.\n");
		Step_Add_Dialog_Continue ("Тоже не проблема - достаточно перед проверкой, всё письмо перевести в нижний регистр с помощью функции <m>ToLower</m>().\n");
		Step_Add_Dialog_Continue ("Только не запихивайте результат обратно в письмо, т.к. само письмо нам нужно в правильном регистре. Используйте для этого временную переменную.\n");
		Step_Add_Dialog ("4. Заменяем все \"а\" на \"о\" а \"о\" на \"а\"\n");
		Step_Add_Dialog_Continue ("Если, при помощи <m>Replace</m>, сначала заменить <str>\"а\"</str> на <str>\"о\"</str>, а затем <str>\"о\"</str> на <str>\"а\"</str>, то во втором шаге будут заменены все буквы \"о\", включая те, которые вы получили на первом шаге.\n");
		Step_Add_Dialog_Continue ("А это не то, что имеется в виду в задании.\n");
		Step_Add_Dialog_Continue ("Простой workaround - заменить сначала все <str>\"а\"</str> на какую ни-будь кракозябру, которая точно не должна встретится в письме (что-то типа <str>\"ъыъ$@@#?#@@$щёщ\"</str>, или, не знаю... придумайте что ни будь).\n");
		Step_Add_Dialog_Continue ("Затем, меняем <str>\"о\"</str> на <str>\"а\"</str>, и, наконец, третим шагом, меняем обратно кракозябру на <str>\"о\"</str>.\n");
		Step_Add_Dialog ("Не то, что б это прям идеальный способ, т.к. никогда не знаешь, какие кракозябры могут попасться в письмах... Тут, главное, придумать кракозябру, которая там попадётся с наименьшей вероятностью.\n");
		Step_Add_Dialog ("Что ж, поехали. Удачи!\n");
		Step_Add_Dialog_Hide ();
		Step_Add_Check_Requirement ("TERMINAL");
		Step_Add_LevelComplete("Level 1-10 - class string.", "Level 001-10");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-11 - Float - Second Enemy.", "Level 001-11");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Dialog ("aaaaaa");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-12 - Algorithmes - The Maze.", "Level 001-12");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Dialog ("bbbbbb");
		// Step_Add_Check_Requirement ("ScriptStopped && noenemyleft");
		// Step_Add_LevelComplete("Level 1-13 - Classes - Ball sorting.", "Level 001-13");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_LevelComplete("Level 1-14 - Test Video Background.", "Level 001-14");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_LevelComplete("Level 1-15 - Test Falling Cubes.", "Level 001-15");

		// Step_Add_SetScript ("\nvoid Start () \n{\n\n\n\n\n\n\n\n\n}");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_Dialog ("tttttt");
		// Step_Add_LevelComplete("Level 1-16 - Reactor Room", "Level Reactor Room");

		Step_Add_Dialog_Hide ();
		Step_Add_Dialog_Important_Hide();
		Step_Add_ScriptControls_Hide ();
		Step_Add_Check_Requirement ("ETERNAL");
		Step_Add_LevelComplete("Level Chapter Complete", "Level Chapter Complete");
		#endregion

		//Set fast_forward info
		int level_index = 0;
		for (int ind = 0; ind < steps.Count; ind++) {
			if (steps[ind].step_type == step_type.dialog) {
				while (steps_for_fastForward.Count <= level_index) { steps_for_fastForward.Add(new List<KeyValuePair<string, int>>()); }
				var level_fast_forward = steps_for_fastForward[level_index];

				level_fast_forward.Add( new KeyValuePair<string, int>(steps[ind].str_param1, ind) );
			}
			else if (steps[ind].step_type == step_type.level_complete_show) { level_index++; }
		}

		Engine.Level_step_indices.RemoveAt(Engine.Level_step_indices.Count-1);
    }

    #region Add_Step_xxx functions
	public static void Step_Add_Pause () {
		step_info t = new step_info();
		t.step_type = step_type.pause; steps.Add(t);
	}
	public static void Step_Add_Resume () {
		step_info t = new step_info();
		t.step_type = step_type.resume; steps.Add(t);
	}
	public static void Step_Add_Dialog (string str, int breakAt = -1, int from = 0) {
		step_info t = new step_info();
		//t.step_type = step_type.dialog; t.str_param1 = Code_HighLight(str); t.int_param1 = breakAt; t.int_param2 = from; steps.Add(t);
		t.step_type = step_type.dialog; t.str_param1 = str; t.int_param1 = breakAt; t.int_param2 = from; steps.Add(t);
	}
	public static void Step_Add_Dialog_Continue (string str, int breakAt = -1) {
		step_info t = new step_info();
		//t.step_type = step_type.dialog_continue; t.str_param1 = Code_HighLight(str); t.int_param1 = breakAt; steps.Add(t);
		t.step_type = step_type.dialog_continue; t.str_param1 = str; t.int_param1 = breakAt; steps.Add(t);
	}
	public static void Step_Add_Dialog_Show () {
		step_info t = new step_info();
		t.step_type = step_type.dialog_show; steps.Add(t);
	}
	public static void Step_Add_Dialog_Hide () {
		step_info t = new step_info();
		t.step_type = step_type.dialog_hide; steps.Add(t);
	}
	public static void Step_Add_Dialog_Important (string str, int breakAt = -1, int from = 0) {
		step_info t = new step_info();
		//t.step_type = step_type.dialog_important; t.str_param1 = Code_HighLight(str); t.int_param1 = breakAt; t.int_param2 = from; steps.Add(t);
		t.step_type = step_type.dialog_important; t.str_param1 = str; t.int_param1 = breakAt; t.int_param2 = from; steps.Add(t);
	}
	public static void Step_Add_Dialog_Important_Continue (string str, int breakAt = -1) {
		step_info t = new step_info();
		//t.step_type = step_type.dialog_important_continue; t.str_param1 = Code_HighLight(str); t.int_param1 = breakAt; steps.Add(t);
		t.step_type = step_type.dialog_important_continue; t.str_param1 = str; t.int_param1 = breakAt; steps.Add(t);
	}
	public static void Step_Add_Dialog_Important_Hide () {
		step_info t = new step_info();
		t.step_type = step_type.dialog_important_hide; steps.Add(t);
	}
	public static void Step_Add_SetScript (string str) {
		step_info t = new step_info();
		t.step_type = step_type.setScript; t.str_param1 = str; steps.Add(t);
	}
	public static void Step_Add_Tutorial_Arrow_Show (float f1, float f2) {
		step_info t = new step_info();
		t.step_type = step_type.tutorialArrow_Show; t.float_param1 = f1; t.float_param2 = f2; steps.Add(t);
	}
	public static void Step_Add_Tutorial_Arrow_Hide () {
		step_info t = new step_info();
		t.step_type = step_type.tutorialArrow_Hide; steps.Add(t);
	}
	public static void Step_Add_Tutorial_RedPanel_Show (float f1, float f2, float f3, float f4) {
		step_info t = new step_info();
		t.step_type = step_type.tutorialRedPanel_Show;
		t.float_param1 = f1; t.float_param2 = f2; t.float_param3 = f3; t.float_param4 = f4; steps.Add(t);
	}
	public static void Step_Add_Tutorial_RedPanel_Hide () {
		step_info t = new step_info();
		t.step_type = step_type.tutorialRedPanel_Hide; steps.Add(t);
	}
	public static void Step_Add_Check_Requirement (string str) {
		step_info t = new step_info();
		t.step_type = step_type.check_Requirement; t.str_param1 = str; steps.Add(t);
	}
	public static void Step_Add_LevelComplete (string name, string obj_name) {
		Engine.Level_names.Add(name);
		Engine.Level_obj_names.Add(obj_name);
		step_info t = new step_info();
		t.step_type = step_type.level_complete_show; steps.Add(t);
		Engine.Level_step_indices.Add(steps.Count);
	}
	// public static void Step_Add_LevelComplete_Hide () {
	// 	step_info t = new step_info();
	// 	t.step_type = step_type.level_complete_hide; steps.Add(t);
	// }
	public static void Step_Add_ScriptControls_Show () {
		step_info t = new step_info();
		t.step_type = step_type.script_controls_show; steps.Add(t);
	}
	public static void Step_Add_ScriptControls_Hide () {
		step_info t = new step_info();
		t.step_type = step_type.script_controls_hide; steps.Add(t);
	}
	public static void Step_Add_Skip_Bot_Rebase_Anim () {
		step_info t = new step_info();
		t.step_type = step_type.skip_bot_rebase_anim; steps.Add(t);
	}
	public static void Step_Add_Set_Blocked_Instructions (string str) {
		step_info t = new step_info();
		t.str_param1 = str;
		t.step_type = step_type.set_blocked_instructions; steps.Add(t);
	}
	public static void Step_Add_Activate_Level_Object (int i) {
		step_info t = new step_info();
		t.int_param1 = i;
		t.step_type = step_type.activate_object; steps.Add(t);
	}
	public static void Step_Add_Play_Sound (int i, int fade_music = -1) {
		step_info t = new step_info();
		t.int_param1 = i; t.int_param2 = fade_music;
		t.step_type = step_type.play_sound; steps.Add(t);
	}
	public static void Step_Add_Restore_Music_Volume () {
		step_info t = new step_info();
		t.step_type = step_type.restore_music_volume; steps.Add(t);
	}
	public static void Step_Add_Wait (float f) {
		step_info t = new step_info();
		t.float_param1 = f;
		t.step_type = step_type.wait; steps.Add(t);
	}
    #endregion

	public static string Code_HighLight(string str, Engine.higlight_colors_info? colors = null) {
		if (colors == null) { colors = Engine.higlight_colors; }

		str = str.Replace("<n>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.nmsp) + "\">");
		str = str.Replace("<c>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.cls) + "\">");
		str = str.Replace("<m>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.method) + "\">");
		str = str.Replace("<t>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.built_in_types) + "\">");
		str = str.Replace("<str>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.str) + "\">");
		str = str.Replace("<cmt>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.comments) + "\">");
		str = str.Replace("<instr>", "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.Value.instruction) + "\">");

		str = str.Replace("</n>", "</color>"); str = str.Replace("</c>", "</color>"); str = str.Replace("</m>", "</color>");
		str = str.Replace("</t>", "</color>"); str = str.Replace("</str>", "</color>"); str = str.Replace("</cmt>", "</color>");
		str = str.Replace("</instr>", "</color>");

		return str;

		//if (str.Contains("<color")) return str;
		// var matches = quotes.Matches(str);
		// foreach (Match m in matches) {
			
		// }

		// matches = class_and_method.Matches(str);
		// for (int i = matches.Count - 1; i >=0; i-- ) {
		// 	Match m = matches[i];
		// 	if (!m.Groups[i].Success) continue;
			
		// 	string[] chain_arr = m.Groups[1].Value.Split(new char[]{'.'});
		// 	chain_arr[chain_arr.Length-1] = "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.method) + "\">" + chain_arr[chain_arr.Length-1] + "</color>";
		// 	if (chain_arr.Length > 1) {
		// 		chain_arr[chain_arr.Length-2] = "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.cls) + "\">" + chain_arr[0] + "</color>";
		// 	} 
		// 	if (chain_arr.Length > 2) {
		// 		for (int x = chain_arr.Length-1; x >= 2; x--){
		// 			chain_arr[x] = "<color=\"#" + ColorUtility.ToHtmlStringRGB(colors.nmsp) + "\">" + chain_arr[x] + "</color>";
		// 		}
		// 	}

		// 	str = str.Remove(m.Groups[1].Index, m.Groups[1].Length);
		// 	str = str.Insert(m.Groups[1].Index, string.Join(".", chain_arr));
		// }
	}

	public static void Convert_Generated_Script() {
		System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo(); nfi.NumberDecimalSeparator = ".";
		var file_text = System.IO.File.ReadAllText(Application.dataPath + "/../Generated_Script.txt");
		var text_arr = file_text.Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);

		string str = "";
		foreach (string line in text_arr) {
			if (line.Length < 34) continue;

			string cmd = line.Substring(0, 34);
			string param = (line.Length == 34) ? "" : line.Substring(34);
			if (cmd.StartsWith("ПОКАЗАТЬ В ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) {Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\"", "\\\"");
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }

				str += "		Step_Add_Dialog (\"" + str_param + "\"";
				if (int_param_arr[0] == -1 && int_param_arr[1] == 0) str += ");\n";
				else str += ", " + int_param_arr[0].ToString() + ", " + int_param_arr[1].ToString() + ");\n";
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ В ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\"", "\\\"");;
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 1) { Debug.Log("Could not parse: " + cmd + param); continue; }

				str += "		Step_Add_Dialog_Continue (\"" + str_param + "\"";
				if (int_param_arr[0] == -1) str += ");\n";
				else str += ", " + int_param_arr[0].ToString() + ");\n";
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ В НИЖНЕЙ ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\"", "\\\"");;
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }

				str += "		Step_Add_Dialog_Important (\"" + str_param + "\"";
				if (int_param_arr[0] == -1 && int_param_arr[1] == 0) str += ");\n";
				else str += ", " + int_param_arr[0].ToString() + ", " + int_param_arr[1].ToString() + ");\n";
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ В НИЖНЕЙ ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\"", "\\\"");;
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 1) { Debug.Log("Could not parse: " + cmd + param); continue; }

				str += "		Step_Add_Dialog_Important_Continue (\"" + str_param + "\"";
				if (int_param_arr[0] == -1) str += ");\n";
				else str += ", " + int_param_arr[0].ToString() + ");\n";
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ ПАНЕЛЬ ДИАЛОГА:")) {
				str += "		Step_Add_Dialog_Show ();\n";
			}
			else if (cmd.StartsWith("СКРЫТЬ ПАНЕЛЬ ДИАЛОГА")) {
				str += "		Step_Add_Dialog_Hide ();\n";
			}
			else if (cmd.StartsWith("СКРЫТЬ НИЖНЮЮ ПАНЕЛЬ ДИАЛОГА")) {
				str += "		Step_Add_Dialog_Important_Hide ();\n";
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КРАСНУЮ СТРЕЛКУ:")) {
				param = param.Trim();
				if (!param.StartsWith("(")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string float_param = param.Substring(1, param.Length-2);
				var float_param_arr = float_param.Split(new char[]{','}).Select(x=> float.Parse(x.Trim(), System.Globalization.NumberStyles.Any, nfi)).ToArray();
				if (float_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				str += "		Step_Add_Tutorial_Arrow_Show (" + float_param_arr[0].ToString(nfi) + "F, " + float_param_arr[1].ToString(nfi) + "F);\n";
			}
			else if (cmd.StartsWith("СКРЫТЬ КРАСНУЮ СТРЕЛКУ:")) {
				str += "		Step_Add_Tutorial_Arrow_Hide ();\n";
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КРАСНУЮ ОБВОДКУ:")) {
				param = param.Trim();
				if (!param.StartsWith("(")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string float_param = param.Substring(1, param.Length-2);
				var float_param_arr = float_param.Split(new char[]{','}).Select(x=> float.Parse(x.Trim(), System.Globalization.NumberStyles.Any, nfi)).ToArray();
				if (float_param_arr.Length != 4) { Debug.Log("Could not parse: " + cmd + param); continue; }
				str += "		Step_Add_Tutorial_RedPanel_Show (" + float_param_arr[0].ToString(nfi) + "F, " + float_param_arr[1].ToString(nfi) + "F, " + float_param_arr[2].ToString(nfi) + "F, " + float_param_arr[3].ToString(nfi) + "F);\n";
			}
			else if (cmd.StartsWith("СКРЫТЬ КРАСНУЮ ОБВОДКУ:")) {
				str += "		Step_Add_Tutorial_RedPanel_Hide ();\n";
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КНОПКИ УПРАВЛЕНИЯ")) {
				str += "		Step_Add_ScriptControls_Show ();\n";
			}
			else if (cmd.StartsWith("СКРЫТЬ КНОПКИ УПРАВЛЕНИЯ")) {
				str += "		Step_Add_ScriptControls_Hide ();\n";
			}
			else if (cmd.StartsWith("ВСТАВИТЬ СКРИПТ В РЕДАКТОР:")) {
				str += "		Step_Add_SetScript (\"" + param + "\");\n";
			}
			else if (cmd.StartsWith("ЗАБЛОКИРОВАТЬ ИНСТРУКЦИИ:")) {
				str += "		Step_Add_Set_Blocked_Instructions (\"" + param + "\");\n";
			}
			else if (cmd.StartsWith("ПРОПУСТИТЬ АНИМАЦИЮ БОТА")) {
				str += "		Step_Add_Skip_Bot_Rebase_Anim ();\n";
			}
			else if (cmd.StartsWith("АКТИВИРОВАТЬ ОБЪЕКТ:")) {
				str += "		Step_Add_Activate_Level_Object (\"" + param.Trim() + "\");\n";
			}
			else if (cmd.StartsWith("ПРОИГРАТЬ ЗВУК:")) {
				var int_param = param.Split(new char[]{','}).Select(x=>int.Parse(x)).ToArray();
				if (int_param.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				str += "		Step_Add_Play_Sound (" + int_param[0].ToString() + ", " + int_param[1].ToString() + ");\n";
			}
			else if (cmd.StartsWith("ВЕРНУТЬ ГРОМКОСТЬ")) {
				str += "		Step_Add_Restore_Music_Volume ();\n";
			}
			else if (cmd.StartsWith("ЖДАТЬ:")) {
				float f = float.Parse(param.Trim(), System.Globalization.NumberStyles.Any, nfi);
				str += "		Step_Add_Wait (" + f.ToString(nfi) + "F);\n";
			}
			else if (cmd.StartsWith("ПРИОСТАНОВИТЬ ВЫПОЛНЕНИЕ СКРИПТА")) {
				str += "		Step_Add_Pause ();\n";
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ ВЫПОЛНЕНИЕ СКРИПТА")) {
				str += "		Step_Add_Resume ();\n";
			}
			else if (cmd.StartsWith("ЖДАТЬ СОСТОЯНИЯ:")) {
				str += "		Step_Add_Check_Requirement (\"" + param.Trim() + "\");\n";
			}
			else if (cmd.StartsWith("ЗАКОНЧИТЬ УРОВЕНЬ:")) {
				param = param.TrimEnd();
				var str_param_arr = param.Split(new string[]{"---"}, System.StringSplitOptions.None);
				if (str_param_arr.Length != 2) continue;
				str += "		Step_Add_LevelComplete (\"" + str_param_arr[0] + "\", \"" + str_param_arr[1] + "\");\n\n";
			}
		}

		System.IO.File.WriteAllText(Application.dataPath + "/../Generated_Script_CNV.txt", str);
	}
	public static void Import_All_Script() {
		steps.Clear();
		Engine.Level_names.Clear();
		Engine.Level_obj_names.Clear();
		Engine.Level_step_indices.Clear();
		Engine.Level_step_indices.Add(0);

		System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo(); nfi.NumberDecimalSeparator = ".";

		var file_text = System.IO.File.ReadAllText(Application.dataPath + "/../Generated_Script.txt");
		var text_arr = file_text.Split(new string[]{"\r\n"}, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string line in text_arr) {
			if (line.Length < 34) continue;

			string cmd = line.Substring(0, 34);
			string param = (line.Length == 34) ? "" : line.Substring(34);
			if (cmd.StartsWith("ПОКАЗАТЬ В ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) {Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\\n", "\n");;
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Dialog(str_param, int_param_arr[0], int_param_arr[1]);
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ В ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\\n", "\n");
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 1) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Dialog_Continue(str_param, int_param_arr[0]);
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ В НИЖНЕЙ ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\\n", "\n");;
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Dialog_Important(str_param, int_param_arr[0], int_param_arr[1]);
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ В НИЖНЕЙ ПАНЕЛИ:")) {
				param = param.TrimEnd();
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				int ind = param.LastIndexOf("(");
				if (ind <= 0) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string str_param = param.Substring(0, ind).Replace("\\n", "\n");
				string int_param = param.Substring(ind+1); int_param = int_param.Substring(0, int_param.Length-1);
				var int_param_arr = int_param.Split(new char[]{','}).Select(x=> int.Parse(x.Trim())).ToArray();
				if (int_param_arr.Length != 1) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Dialog_Important_Continue(str_param, int_param_arr[0]);
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ ПАНЕЛЬ ДИАЛОГА:")) {
				Step_Add_Dialog_Show();
			}
			else if (cmd.StartsWith("СКРЫТЬ ПАНЕЛЬ ДИАЛОГА")) {
				Step_Add_Dialog_Hide();
			}
			else if (cmd.StartsWith("СКРЫТЬ НИЖНЮЮ ПАНЕЛЬ ДИАЛОГА")) {
				Step_Add_Dialog_Important_Hide();
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КРАСНУЮ СТРЕЛКУ:")) {
				param = param.Trim();
				if (!param.StartsWith("(")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string float_param = param.Substring(1, param.Length-2);
				var float_param_arr = float_param.Split(new char[]{','}).Select(x=> float.Parse(x.Trim(), System.Globalization.NumberStyles.Any, nfi)).ToArray();
				if (float_param_arr.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Tutorial_Arrow_Show(float_param_arr[0], float_param_arr[1]);
			}
			else if (cmd.StartsWith("СКРЫТЬ КРАСНУЮ СТРЕЛКУ:")) {
				Step_Add_Tutorial_Arrow_Hide();
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КРАСНУЮ ОБВОДКУ:")) {
				param = param.Trim();
				if (!param.StartsWith("(")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				if (!param.EndsWith(")")) { Debug.Log("Could not parse: " + cmd + param); continue; }
				string float_param = param.Substring(1, param.Length-2);
				var float_param_arr = float_param.Split(new char[]{','}).Select(x=> float.Parse(x.Trim(), System.Globalization.NumberStyles.Any, nfi)).ToArray();
				if (float_param_arr.Length != 4) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Tutorial_RedPanel_Show(float_param_arr[0], float_param_arr[1], float_param_arr[2], float_param_arr[3]);
			}
			else if (cmd.StartsWith("СКРЫТЬ КРАСНУЮ ОБВОДКУ:")) {
				Step_Add_Tutorial_RedPanel_Hide();
			}
			else if (cmd.StartsWith("ПОКАЗАТЬ КНОПКИ УПРАВЛЕНИЯ")) {
				Step_Add_ScriptControls_Show();
			}
			else if (cmd.StartsWith("СКРЫТЬ КНОПКИ УПРАВЛЕНИЯ")) {
				Step_Add_ScriptControls_Hide();
			}
			else if (cmd.StartsWith("ВСТАВИТЬ СКРИПТ В РЕДАКТОР:")) {
				Step_Add_SetScript(param.Replace("\\n", "\n"));
			}
			else if (cmd.StartsWith("ЗАБЛОКИРОВАТЬ ИНСТРУКЦИИ:")) {
				Step_Add_Set_Blocked_Instructions(param);
			}
			else if (cmd.StartsWith("ПРОПУСТИТЬ АНИМАЦИЮ БОТА")) {
				Step_Add_Skip_Bot_Rebase_Anim();
			}
			else if (cmd.StartsWith("АКТИВИРОВАТЬ ОБЪЕКТ:")) {
				Step_Add_Activate_Level_Object(int.Parse(param.Trim()));
			}
			else if (cmd.StartsWith("ПРОИГРАТЬ ЗВУК:")) {
				var int_param = param.Split(new char[]{','}).Select(x=>int.Parse(x)).ToArray();
				if (int_param.Length != 2) { Debug.Log("Could not parse: " + cmd + param); continue; }
				Step_Add_Play_Sound(int_param[0], int_param[1]);
			}
			else if (cmd.StartsWith("ВЕРНУТЬ ГРОМКОСТЬ")) {
				Step_Add_Restore_Music_Volume();
			}
			else if (cmd.StartsWith("ЖДАТЬ:")) {
				float f = float.Parse(param.Trim(), System.Globalization.NumberStyles.Any, nfi);
				Step_Add_Wait(f);
			}
			else if (cmd.StartsWith("ПРИОСТАНОВИТЬ ВЫПОЛНЕНИЕ СКРИПТА")) {
				Step_Add_Pause();
			}
			else if (cmd.StartsWith("ПРОДОЛЖИТЬ ВЫПОЛНЕНИЕ СКРИПТА")) {
				Step_Add_Resume();
			}
			else if (cmd.StartsWith("ЖДАТЬ СОСТОЯНИЯ:")) {
				Step_Add_Check_Requirement(param.Trim());
			}
			else if (cmd.StartsWith("ЗАКОНЧИТЬ УРОВЕНЬ:")) {
				param = param.TrimEnd();
				var str_param_arr = param.Split(new string[]{"---"}, System.StringSplitOptions.None);
				if (str_param_arr.Length != 2) continue;
				Step_Add_LevelComplete(str_param_arr[0], str_param_arr[1]);
			}
		}
		Engine.Level_step_indices.RemoveAt(Engine.Level_step_indices.Count-1);
		Debug.Log("Scenario script loaded from file.");
	}
	public static void Export_All_Script() {
		string str = "";
		int level = 0;
		System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo(); nfi.NumberDecimalSeparator = ".";
		foreach (var step in steps) {
			switch (step.step_type) {
				case step_type.dialog :
					str += "ПОКАЗАТЬ В ПАНЕЛИ:                " + step.str_param1.Replace("\n", "\\n") + "(" + step.int_param1.ToString() + "," + step.int_param2.ToString() + ")\r\n"; break;
				case step_type.dialog_continue :
					str += "ПРОДОЛЖИТЬ В ПАНЕЛИ:              " + step.str_param1.Replace("\n", "\\n") + "(" + step.int_param1.ToString() + ")\r\n"; break;
				case step_type.dialog_important :
					str += "ПОКАЗАТЬ В НИЖНЕЙ ПАНЕЛИ:         " + step.str_param1.Replace("\n", "\\n") + "(" + step.int_param1.ToString() + "," + step.int_param2.ToString() + ")\r\n"; break;
				case step_type.dialog_important_continue :
					str += "ПРОДОЛЖИТЬ В НИЖНЕЙ ПАНЕЛИ:       " + step.str_param1.Replace("\n", "\\n") + "(" + step.int_param1.ToString() + ")\r\n"; break;
				case step_type.dialog_show :
					str += "ПОКАЗАТЬ ПАНЕЛЬ ДИАЛОГА:          " + "\r\n"; break;
				case step_type.dialog_hide :
					str += "СКРЫТЬ ПАНЕЛЬ ДИАЛОГА             " + "\r\n"; break;
				case step_type.dialog_important_hide :
					str += "СКРЫТЬ НИЖНЮЮ ПАНЕЛЬ ДИАЛОГА      " + "\r\n"; break;
				case step_type.tutorialArrow_Show : 
					str += "ПОКАЗАТЬ КРАСНУЮ СТРЕЛКУ:         (" + step.float_param1.ToString(nfi) + "," + step.float_param2.ToString(nfi) + ")\r\n"; break;
				case step_type.tutorialArrow_Hide : 
					str += "СКРЫТЬ КРАСНУЮ СТРЕЛКУ:           " + "\r\n"; break;
				case step_type.tutorialRedPanel_Show : 
					str += "ПОКАЗАТЬ КРАСНУЮ ОБВОДКУ:         (" + step.float_param1.ToString(nfi) + "," + step.float_param2.ToString(nfi) + "," + step.float_param3.ToString(nfi) + "," + step.float_param4.ToString(nfi) + ")\r\n"; break;
				case step_type.tutorialRedPanel_Hide :
					str += "СКРЫТЬ КРАСНУЮ ОБВОДКУ:           " + "\r\n"; break;
				case step_type.script_controls_show :
					str += "ПОКАЗАТЬ КНОПКИ УПРАВЛЕНИЯ        " + "\r\n"; break;
				case step_type.script_controls_hide : 
					str += "СКРЫТЬ КНОПКИ УПРАВЛЕНИЯ          " + "\r\n"; break;
				case step_type.setScript : 
					str += "ВСТАВИТЬ СКРИПТ В РЕДАКТОР:       " + step.str_param1.Replace("\n", "\\n") + "\r\n"; break;
				case step_type.set_blocked_instructions : 
					str += "ЗАБЛОКИРОВАТЬ ИНСТРУКЦИИ:         " + step.str_param1 + "\r\n"; break;
				case step_type.skip_bot_rebase_anim : 
					str += "ПРОПУСТИТЬ АНИМАЦИЮ БОТА          " + "\r\n"; break;
				case step_type.activate_object :
					str += "АКТИВИРОВАТЬ ОБЪЕКТ:              " + step.int_param1.ToString() + "\r\n"; break;
				case step_type.play_sound : 
					str += "ПРОИГРАТЬ ЗВУК:                   " + step.int_param1.ToString() + "," + step.int_param2.ToString() + "\r\n"; break;
				case step_type.restore_music_volume : 
					str += "ВЕРНУТЬ ГРОМКОСТЬ                 " + "\r\n"; break;
				case step_type.wait : 
					str += "ЖДАТЬ:                            " + step.float_param1.ToString(nfi) + "\r\n"; break;
				case step_type.pause :
					str += "ПРИОСТАНОВИТЬ ВЫПОЛНЕНИЕ СКРИПТА  " + "\r\n"; break;
				case step_type.resume :
					str += "ПРОДОЛЖИТЬ ВЫПОЛНЕНИЕ СКРИПТА     " + "\r\n"; break;
				case step_type.check_Requirement :
					str += "ЖДАТЬ СОСТОЯНИЯ:                  " + step.str_param1 + "\r\n"; break;
				case step_type.level_complete_show :
					str += "ЗАКОНЧИТЬ УРОВЕНЬ:                " + Engine.Level_names[level] + "---" + Engine.Level_obj_names[level] + "\r\n\r\n"; level++; break;
			}
		}

		System.IO.File.WriteAllText(Application.dataPath + "/../Generated_Script.txt", str);
	}
}
