using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using CameraTransitions;

using UnityEngine.EventSystems;

public class Camera_Controller : MonoBehaviour {
	Engine engine;

	Transform bot_obj;
	Transform bot_tr;

	TMP_Text Level_Complete_txt;

	public cam_control_activation_enum cam_control_activation = cam_control_activation_enum.OnEditorDeActivation;
	public enum cam_control_activation_enum { OnMouseOutIfEditor, OnEditorDeActivation, OnBoth, Always }

	public float camera_distance = 12.34f;
	public float camera_circular_offset_angle = 307.5f;
	public float camera_height = 5.63f;
	public Vector2 camera_rotation_center_offset = Vector2.zero;
	public Vector3 camera_target_offset = Vector3.zero;
	Quaternion camera_rotation; 	//Right click rotation
	Vector2 camera_rotation_offset; //Right click rotation
	Vector3 initial_camera_pos;
	public float speed_damping_rot = 1f;
	public float speed_damping_pos = 1f;
	public float speed_skybox = 0.2f;
	CanvasGroup OptionCanvas = null; //Need to check if options panel is active to desactivate camera controls
	CanvasGroup LogCanvas = null; //Need to check if log panel is active to desactivate camera controls
	//TMPro.TMP_InputField ScriptInput = null; //Need to check if script input is active to desactivate camera controls
	Text_Editor ScriptEditor_My = null;
	//float ScriptInput_ScrollValue = 0f;
	//bool ScriptInput_MouseOnIt = false;

	public Camera camera_main;
	public Camera camera_orbit;
	public float speed_level_complete_gradient = 0.05f;
	public float speed_orbit = 0.05f;
	float orbit_distance = 5f;
	CameraTransition cameraTransition;

	bool need_load_scene = false;
	public static bool level_complete_shown = false;
	float orbit_current = 0f;
	float level_complete_gradient_current_hue = 0;
	float manual_rotate_timer = 0f;
	bool manual_rotate_pos_ok = false;
	bool manual_rotate_rot_ok = false;
	//bool manual_rotate_tar_ok = false;
	float _camera_height = 0f;
	float _camera_distance = 0f;
	
	[HideInInspector] public bool camera_handled_by_level_script = false;
	[HideInInspector] public bool disable_camera_reposition = false;
	[HideInInspector] public bool disable_user_arrowRotation = false;
	[HideInInspector] public bool disable_user_rigtMouseRotation = false;
	[HideInInspector] public bool disable_user_zoom = false;
	[HideInInspector] public Level_Settings.camera_cinematic_info cinematic = null;
	[HideInInspector] public Vector3Int cinematic_index = new Vector3Int(-1, 0, 0);
	bool cinematic_is_playing = false;
	Level_Settings.camera_cinematic_info.cinematic_info current_cinematic = null;

	bool disable_user_zoom_by_hud = false;

	// Use this for initialization
	void Start () {
		engine = GameObject.Find("Compiler").GetComponent<Engine>();

		bot_obj = GameObject.Find("Robot").GetComponent<Transform>();
		bot_tr = bot_obj.transform;
		Level_Complete_txt = GameObject.Find("Level_Complete_txt").GetComponent<TMP_Text>();

		camera_main = GameObject.Find("Main Camera").GetComponent<Camera>();
		initial_camera_pos = camera_main.transform.position;

		cameraTransition = GetComponent<CameraTransition>();

		OptionCanvas = GameObject.Find("Panel_Options").GetComponent<CanvasGroup>();
		LogCanvas = GameObject.Find("Panel_Log").GetComponent<CanvasGroup>();

		// GameObject tmpro = GameObject.Find("Script_InputField_TMPRO");
		// if (tmpro != null)
		// 	ScriptInput = tmpro.GetComponent<TMPro.TMP_InputField>();
		// else
			ScriptEditor_My = GameObject.Find("Text_Editor").GetComponent<Text_Editor>();
		

		//LevelCompleteShow();
	}
	
	// Update is called once per frame
	void Update () {
		//Rotate skybox
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed_skybox);

		SetLevelCompleteColors();

		//Load next scene in the middle of transition, after level complete animation
		if (need_load_scene && cameraTransition.Progress >= 0.5f)
		{
			//If BOT is rebasing, need to cancel animation
			//BOT.bot_obj.GetComponent<Robot>().Appear_Immediate();
			//Engine.bot_base.GetComponent<Robot_Base>().CancelAnimation();

			need_load_scene = false;
			engine.LoadNextScene();
		}

		if (cinematic_is_playing) return;
		if (cinematic != null && cinematic.enabled) {
			if (cinematic_index.x < 0) {
				//Startup cinematic
				if (cinematic.level_start_cinematics.Length > cinematic_index.y) {
					cinematic_is_playing = true;
					var current = cinematic.level_start_cinematics[cinematic_index.y];
					StartCoroutine( Play_Camera_Cinematic(current) );
					cinematic_index.y++; cinematic_index.z = 0; return;
				}
				cinematic_index.x = 1; cinematic_index.y = 0;
			}

			if (cinematic.level_cinematics_loop == Level_Settings.camera_cinematic_info.cinematic_loop.loop) {
				if (cinematic_index.y >= cinematic.level_cinematics.Length) { cinematic_index.y = 0; }
			}
			if (cinematic.level_cinematics.Length > cinematic_index.y) {
				//Level cinematic
				cinematic_is_playing = true;
				var current = cinematic.level_cinematics[cinematic_index.y];
				StartCoroutine( Play_Camera_Cinematic(current) );
				cinematic_index.y++; cinematic_index.z = 0; return;
			}
		}
		//camera_main.transform.position -= camera_offset;

		if (camera_handled_by_level_script) return;

		//Position on circle
		//distance_to_bot = Vector3.Distance(bot_obj.position, camera_main.transform.position);
		//float rad = camera_main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		
		//Need to check if script input field is under pointer, to disable scroll
		bool ScriptInput_MouseOnIt = false;
        PointerEventData pointerData = new PointerEventData (EventSystem.current) { pointerId = -1, };
		pointerData.position = Input.mousePosition;

		bool can_use_control = false;
		//Disable controls if options or log is opened
		can_use_control = Mathf.Approximately(OptionCanvas.alpha, 0f) && Mathf.Approximately(LogCanvas.alpha, 0f) && !Engine.Ref_objs.options.Is_ClassFunc_Panel_Shown;

		if (cam_control_activation == cam_control_activation_enum.OnMouseOutIfEditor || cam_control_activation == cam_control_activation_enum.OnBoth) {
			//Camera control enabled on 'Mouse out of Editor' or 'both'
			List<RaycastResult> RaycastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, RaycastResults);
			foreach (var r in RaycastResults)
				if (r.gameObject == ScriptEditor_My.gameObject) { ScriptInput_MouseOnIt = true; break; }
			can_use_control = can_use_control && !ScriptInput_MouseOnIt;
		}
		if (cam_control_activation == cam_control_activation_enum.OnEditorDeActivation || cam_control_activation == cam_control_activation_enum.OnBoth) {
			//Camera control enabled on 'Editor Deactivation' or 'both'
			can_use_control = can_use_control && !ScriptEditor_My.Activate_Settings.IsActive;
		}

		//Debug.Log("height = " + camera_height + ", _height = " + _camera_height);

		//Move and rotate main camera
		float damp_coeff = 1f;
		if (can_use_control) {
			//Scroll zoom
			if (!disable_user_zoom && !disable_user_zoom_by_hud && Input.mouseScrollDelta.y != 0f) {
				_camera_distance -= Input.mouseScrollDelta.y;
				if (_camera_distance < 7f) _camera_distance = 7f;
				if (_camera_distance > 20f) _camera_distance = 20f;

				_camera_height = Mathf.Lerp( 0.6f + camera_target_offset.y, camera_height, _camera_distance / camera_distance );
			}

			//Orbit
			if (!disable_user_arrowRotation) {
				if (Engine.check_key(Engine.Key.Camera_Left, false) || Engine.check_key(Engine.Key.Camera_Right, false)) { 
					damp_coeff = 10f; manual_rotate_timer += Time.unscaledDeltaTime * 2f;
					//Reset right-mouse button rotation
					camera_rotation = camera_main.transform.rotation; camera_rotation_offset = new Vector2(0f, 0f);
				} 
				else { manual_rotate_timer = 0f; manual_rotate_pos_ok = false; manual_rotate_rot_ok = false; } //manual_rotate_tar_ok = false; }

				if (manual_rotate_timer == 0f || (manual_rotate_pos_ok && manual_rotate_rot_ok)) { // && manual_rotate_tar_ok)) {
					if (Engine.check_key(Engine.Key.Camera_Left, false)) camera_circular_offset_angle -= 0.5f;
					if (Engine.check_key(Engine.Key.Camera_Right, false)) camera_circular_offset_angle += 0.5f;
				}

				if (Engine.check_key(Engine.Key.Camera_Reset)) { _camera_distance = camera_distance; camera_circular_offset_angle = 307.5f; }
			}
		} else { 
			manual_rotate_timer = 0f; 
		}
		
		//Camera position (orbit)
		float rad = camera_circular_offset_angle * Mathf.Deg2Rad;
		float x = Mathf.Sin(rad) * _camera_distance;
		float y = Mathf.Cos(rad) * _camera_distance;
		Vector3 target_pos  = new Vector3(-x + camera_rotation_center_offset.x, _camera_height, -y + camera_rotation_center_offset.y);
		if (!disable_camera_reposition) {
			if (manual_rotate_timer > 0f) {
				if (!manual_rotate_pos_ok) {
					camera_main.transform.position = Vector3.Lerp (camera_main.transform.position, target_pos, manual_rotate_timer);
					if (Vector3.Distance(camera_main.transform.position, target_pos) < 1f) manual_rotate_pos_ok = true;
				} else {
					camera_main.transform.position = target_pos;
				}
			} else {
				camera_main.transform.position = Vector3.Lerp (camera_main.transform.position, target_pos, Time.unscaledDeltaTime * speed_damping_pos * damp_coeff);
			}
		}

		if (Input.GetKey(KeyCode.Mouse1) && damp_coeff < 2f && can_use_control && !disable_user_rigtMouseRotation) {
			//Right-click Rotation
			if (Input.GetKeyDown(KeyCode.Mouse1)) {
				camera_rotation = camera_main.transform.rotation;
				camera_rotation_offset = new Vector2(0f, 0f);
			}
			camera_rotation_offset += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			var rotation = Quaternion.Euler(camera_rotation.eulerAngles + new Vector3(-camera_rotation_offset.y, camera_rotation_offset.x, 0f));
			camera_main.transform.rotation = rotation;
		} else {
			//Look At BOT rotation
			var target_offset = camera_target_offset;
			// if (manual_rotate_timer > 0f) { 
			// 	target_offset = Vector3.Lerp(camera_target_offset, Vector3.zero, manual_rotate_timer); 
			// 	if (manual_rotate_timer >= 1f) manual_rotate_tar_ok = true;
			// }
			//target_offset = Vector3.Lerp(camera_target_offset, Vector3.zero, manual_rotate_timer);
			var rotation = Quaternion.LookRotation ((bot_obj.position + target_offset) - camera_main.transform.position);
			if (manual_rotate_timer > 0f) {
				if (!manual_rotate_rot_ok) {
					camera_main.transform.rotation = Quaternion.RotateTowards(camera_main.transform.rotation, rotation, 1f);
					if (Quaternion.Angle(camera_main.transform.rotation, rotation) < 1f) manual_rotate_rot_ok = true;
				} else {
					camera_main.transform.rotation = rotation;
				}
			} else {
	  			camera_main.transform.rotation = Quaternion.Slerp (camera_main.transform.rotation, rotation, Time.unscaledDeltaTime * speed_damping_rot * damp_coeff);
			}
		}
	}

	public void LevelCompleteShow()
	{
		//Disable window mode
		Engine.desktop_manager.Switch_Mode(0, true);
		Engine.engine_inst.Reference_objects.options.Hide_Tutorial_Dialog(true);

		//Set timescale to x1
		Engine.Ref_objs.options.Time_Control_Panel.GetChild(0).GetComponent<Toggle>().isOn = true;

		//Close all opened panels
		Options_Global_Static.SubPanelClose();
		var opt = Engine.engine_inst.Reference_objects.options;
		if (opt.Is_Log_Panel_Shown) opt.Panel_Log_Hide();
		if (opt.Is_ClassFunc_Panel_Shown) opt.Panel_ClassFunc_Hide();
		if (opt.Is_Options_Panel_Shown) opt.Hide();

		//Get level name to show
		string l = Engine.Level_obj_names[Engine.current_scene-1].Replace("00", "0").Replace(" 0", " ").Replace("-0", "-");
		l = System.Text.RegularExpressions.Regex.Replace(l, "level", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

		Engine.Ref_objs.options.ScriptSave("Autosave - Level " + l);

		orbit_current = 0f;
		level_complete_gradient_current_hue = 0;
		Level_Complete_txt.alpha = 0f;
		Level_Complete_txt.text = "LEVEL " + l.ToString() + " COMPLETE";
		Level_Complete_txt.transform.localScale = new Vector3(3f, 4.5f, 3f);
		Level_Complete_txt.transform.localRotation = Quaternion.Euler(0f, 0f, -20f);
		Level_Complete_txt.transform.parent.GetComponent<CanvasGroup>().alpha = 1f;
		Level_Complete_txt.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;

		level_complete_shown = true;

		//Animate text
		Level_Complete_txt.transform.DOScale(new Vector3(1f, 1.5f, 1f), 1f).SetEase(Ease.InOutSine).SetUpdate(true);
		Level_Complete_txt.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InOutSine).SetUpdate(true);
		Level_Complete_txt.DOFade(1f, 1.5f).SetUpdate(true);

		//Reset and Animate 'NEXT' button
		RectTransform button_rt = Level_Complete_txt.transform.parent.GetChild(1).GetComponent<RectTransform>();
		button_rt.localScale = new Vector3(0f, 1f, 1f);
		button_rt.localRotation = Quaternion.Euler(0f, 0f, 0f);
		button_rt.DOScaleX(-1f, 0.5f).SetDelay(1f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => {
			button_rt.DOScaleX(1f, 0.5f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => {
				button_rt.transform.GetChild(0).GetComponent<Button>().interactable = true;
			});
		});

		cameraTransition.DoTransition(CameraTransitionEffects.Flash, camera_main, camera_orbit, 3.0f);

		AudioSource aud = camera_main.GetComponent<AudioSource>();
		AudioSource aud2 = engine.GetComponent<AudioSource>();
		aud.DOFade(0f, 0.7f).SetUpdate(true).OnComplete(() => {
			aud.Stop();
			aud.volume = Options_Global_Static.Volume_Music;
			aud2.volume = Options_Global_Static.Volume_Music;
			aud2.clip = engine.LevelComplete_Music;
			aud2.Play();
		});
	}

	public void LevelCompleteHide()
	{
		//Fade out fanfare music
		AudioSource aud2 = engine.GetComponent<AudioSource>();
		aud2.DOFade(0f, 1f).SetUpdate(true);

		CanvasGroup cv = Level_Complete_txt.transform.parent.GetComponent<CanvasGroup>();
		RectTransform button_rt = Level_Complete_txt.transform.parent.GetChild(1).GetComponent<RectTransform>();

		float fade_time = 1.2f;
		button_rt.localScale = new Vector3(1f, 1f, 1f);
		button_rt.localRotation = Quaternion.Euler(0f, 0f, 0f);
		button_rt.DOLocalRotate(new Vector3(0f, 0f, 720), fade_time, RotateMode.FastBeyond360).SetEase(Ease.OutQuad).SetUpdate(true);
		button_rt.DOScale(5f, fade_time).SetEase(Ease.OutQuad).SetUpdate(true);
		button_rt.transform.GetChild(0).GetComponent<Button>().interactable = false;
		Level_Complete_txt.transform.DOLocalRotate(new Vector3(0f, 0f, 50f), fade_time).SetEase(Ease.OutSine).SetUpdate(true);

		cv.DOFade(0f, fade_time).SetUpdate(true).OnComplete(() => {
			StartCoroutine( Disable_level_complete_shown_after_time(3.0f) );
			need_load_scene = true;
			cameraTransition.DoTransition(CameraTransitionEffects.Flash, camera_orbit, camera_main, 3.0f);
		});
	}

	IEnumerator Disable_level_complete_shown_after_time(float t) {
		yield return new WaitForSecondsRealtime(t);
		level_complete_shown = false;
		Level_Complete_txt.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	void SetLevelCompleteColors()
	{
		if (!level_complete_shown) return;

		float h1 = level_complete_gradient_current_hue;
		float h2 = level_complete_gradient_current_hue + 0.25f;
		float h3 = level_complete_gradient_current_hue + 0.5f;
		float h4 = level_complete_gradient_current_hue + 0.75f;
		if (h1 > 1f) h1 -= 1f;
		if (h2 > 1f) h2 -= 1f;
		if (h3 > 1f) h3 -= 1f;
		if (h4 > 1f) h4 -= 1f;

		float sat = 1f;
		float val = 1f;
		Color h1c = Color.HSVToRGB(h1, sat, val);
		Color h2c = Color.HSVToRGB(h2, sat, val);
		Color h3c = Color.HSVToRGB(h3, sat, val);
		Color h4c = Color.HSVToRGB(h4, sat, val);
		
		Level_Complete_txt.colorGradient = new VertexGradient(h1c, h2c, h3c, h4c);

		level_complete_gradient_current_hue += speed_level_complete_gradient;
		if (level_complete_gradient_current_hue > 1f) level_complete_gradient_current_hue -= 1f;

		float x = Mathf.Sin(orbit_current) * orbit_distance;
		float z = Mathf.Cos(orbit_current) * orbit_distance;
		x += bot_tr.position.x;
		z += bot_tr.position.z;
		//Transform tr = GameObject.Find("Cube").transform;
		//tr.position = new Vector3(x, bot_tr.position.y, z);
		camera_orbit.transform.position = new Vector3(x, bot_tr.position.y + 1.5f, z);
		camera_orbit.transform.LookAt(bot_tr, Vector3.up);

		orbit_current += speed_orbit;
		if (orbit_current > 6.28319f) orbit_current = 0f;
	}

	IEnumerator Play_Camera_Cinematic(Level_Settings.camera_cinematic_info.cinematic_info c) {
		current_cinematic = c;
		bool update_now = false;
		if (c.anim != null) {
			Animator a = camera_main.GetComponent<Animator>();
			if (a == null) { a = camera_main.gameObject.AddComponent<Animator>(); a.updateMode = AnimatorUpdateMode.UnscaledTime; }
			PlayableGraph playableGraph = PlayableGraph.Create();
        	playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        	var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", a);
        	// Wrap the clip in a playable
        	var clipPlayable = AnimationClipPlayable.Create(playableGraph, c.anim);
        	// Connect the Playable to an output
        	playableOutput.SetSourcePlayable(clipPlayable);
        	// Plays the Graph.
        	playableGraph.Play();

			yield return new WaitForSecondsRealtime(c.anim.length);
			playableGraph.Destroy();
		} else if (c.way_points != null && c.way_points.Length >= 1) {
			camera_main.transform.position = c.way_points[0].point_t.position;
			camera_main.transform.LookAt (c.way_points[0].point_v);
			//TODO - waypoint animation
		} else if (c.camera_path != null) {
			float FOV_orig = camera_main.fieldOfView;
			var cpa = c.camera_path.GetComponent<CameraPathAnimator>();
			cpa.animationObject = camera_main.transform;
			cpa.animateFOV = true;
			cpa.Play(true);
			while (cpa.isPlaying) {
				camera_main.fieldOfView = c.camera_path.GetPathFOV(cpa.currentTime);
				yield return null;
			}
			cpa.Stop();
			cpa.animationObject = null;
			camera_main.fieldOfView = FOV_orig;
			//If we reset fov, and the next cinematic will set it again in next frame, we'll have camera flashing
			//So we call update right now, to update cinematic (if any) this frame
			update_now = true;
		} else if (c.bezier_spline != null) {
			var bw = camera_main.GetComponent<BezierSolution.BezierWalkerManual>();
			if (bw == null) bw = camera_main.gameObject.AddComponent<BezierSolution.BezierWalkerManual>();
			bw.spline = c.bezier_spline;
			if (c.look_points != null && c.look_points.Length > 0) bw.lookAt = BezierSolution.LookAtMode.None; else bw.lookAt = BezierSolution.LookAtMode.Forward;

			for (float t = 0f; t <= 1f; t += Time.deltaTime * c.speed) {
				bw.NormalizedT = t;

				if (c.look_points != null && c.look_points.Length > 0) {
					float p = Mathf.Lerp(0, c.look_points.Length - 1, t);
					var p1 = c.look_points[ Mathf.FloorToInt(p) ];
					var p2 = c.look_points[ Mathf.CeilToInt(p) ];
					Quaternion q1 = Quaternion.Euler(p1.point_v);
					Quaternion q2 = Quaternion.Euler(p2.point_v);
					if (p1.look_to_bot) q1 = Quaternion.LookRotation(BOT.bot_obj.transform.position - camera_main.transform.position);
					if (p2.look_to_bot) q2 = Quaternion.LookRotation(BOT.bot_obj.transform.position - camera_main.transform.position);
					camera_main.transform.rotation = Quaternion.Lerp(q1, q2, p - Mathf.Floor(p));
				}

				yield return null;
			}
			bw.spline = null;
		}
		current_cinematic = null;
		cinematic_is_playing = false;
		if (update_now) Update();
	}

	public void Reset_Camera_Cinematic() {
		_camera_height = camera_height;
		_camera_distance = camera_distance;

		if (!cinematic_is_playing) return;
		if (current_cinematic == null) return;

		//TODO - Animation clip and waypoint cinematic reset
		if (current_cinematic.camera_path != null) {
			var cpa = current_cinematic.camera_path.GetComponent<CameraPathAnimator>();
			cpa.Stop();
		} else if (current_cinematic.bezier_spline != null) {
			StopCoroutine( Play_Camera_Cinematic(current_cinematic) );
			var bw = camera_main.GetComponent<BezierSolution.BezierWalkerManual>();
			bw.spline = null;
			current_cinematic = null; cinematic_is_playing = false;
		}

		float rad = camera_circular_offset_angle * Mathf.Deg2Rad;
		float x = Mathf.Sin(rad) * camera_distance;
		float y = Mathf.Cos(rad) * camera_distance;
		Vector3 target_pos  = new Vector3(-x + camera_rotation_center_offset.x, camera_height, -y + camera_rotation_center_offset.y);
		camera_main.transform.position = target_pos;
	}

	public void Enable_Mouse_Wheel() { disable_user_zoom_by_hud = false; }
	public void Disable_Mouse_Wheel() { disable_user_zoom_by_hud = true; }
}
