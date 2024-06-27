using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

//https://forum.unity.com/threads/the-recommended-way-to-seek-time.471547/

public enum Status {
    StoppedState,
    PlayingState,
    PausedState
}

public enum StereoMode
{
    noStereo,
    overUner,
    sideBySide,
    overUnderInverted,
    sideBySideInverted
}

public class UDPstruct {
	public UDPstruct() {
		id = null; yaw = -1; pitch = -1; roll = -1; fov = -1; url = null; state = Status.StoppedState; position = -1; exit = false;}
	public string id;
	public float yaw;
	public float pitch;
	public float roll;
	public float fov;
	public string url;
	public Status state;
	public int position;
	public bool exit;
}

public class MediaController : MonoBehaviour {

    //public MediaPlayer mediaPlayer;
	public VideoPlayer videoPlayer;


	// VR camera + Unity camera

	// unity cam
	public Camera cam;
	public GameObject cameraPrefab;

	// vr cam
	public GameObject cameraRigEyes;
	private bool VREnable;

	// state management
	private int stateMessageToHandle; // used to avoid multiple call of VideoPlayerAction() in update()
    private bool isPlaying;
    private bool isPaused;
    private bool isStopped;

	// camera rotation stuff
	private Vector3 rotation; 
    private rotateCamera cameraRotation;
    private bool cameraChanged;
    private float RadToDegree;
    private Vector3 oldRotation;

	//parsing
	private UDPstruct last_message;

	// synchronization
    private int VideoPositionMs; // video position from AVpro player
	private int SoundPositionMs; // sound position coming from plugin

	// quit
	private bool exit; // application shutdown

	// video switch
	private bool videoSwitch; // if user change video
	private string videoPath;

	// for initialization
	private bool _firstRun;
	private UDPCommunication network;



    // Use this for initialization
    void Start()
    {
        isPlaying = true;
        isPaused = false;
        isStopped = false;
        cameraChanged = false;
		RadToDegree = 180 / Mathf.PI;
        oldRotation = new Vector3(0, 0, 0);
		rotation = new Vector3(0, 0, 0);
        cameraRotation = GameObject.Find("Main Camera").GetComponent<rotateCamera>();
		network = GameObject.Find("Network").GetComponent<UDPCommunication>();
		VideoPositionMs = 0;
		SoundPositionMs = 0;
		exit = false;
		videoSwitch = false;
		videoPath = null;
		_firstRun = true;
		last_message = null;
		stateMessageToHandle = 0;

		// VR management
		//string vr = GetArgs ("-vr");
		if (UnityEngine.XR.XRDevice.isPresent) //resetScript.VRenable) //vr != null && vr.CompareTo("1") == 0 /*|| UnityEngine.XR.XRDevice.isPresent*/)
		{
			VREnable = true;
			cameraPrefab.SetActive(false);

			Debug.Log ("[NoiseMakers] VR device detected and ready to use");
			Debug.Log ("[NoiseMakers] VR activate");
		}
		else
		{
			DisableVR ();
			VREnable = false;

			/* All cameras are active per default we want either default unity camera or VR cameras */
			GameObject cameraRig = GameObject.Find ("[CameraRig]");
			cameraRig.SetActive (false);

			//Destroy (cameraRig);
			Debug.Log ("[NoiseMakers] VR disable");
		}
    }

	IEnumerator LoadDevice(string newDevice, bool enable)
	{
		XRSettings.LoadDeviceByName(newDevice);
		yield return null;
		XRSettings.enabled = enable;
	}

	// reset controller parameters
	void ResetController() {
		isPlaying = true;
		isPaused = false;
		isStopped = false;
		cameraChanged = false;
		oldRotation = new Vector3(0, 0, 0);
		rotation = new Vector3(0, 0, 0);
		VideoPositionMs = 0;
		SoundPositionMs = 0;
		videoSwitch = false;
		videoPath = null;
		last_message = null;
	}

	// disable VR and unload openVR 
	void DisableVR()
	{
		StartCoroutine(LoadDevice("none", false));
	}


	// get arguments passed through the executable
	private static string GetArgs(string name)
	{
		var args = System.Environment.GetCommandLineArgs ();
		for (int i_arg = 0; i_arg < args.Length; i_arg++) 
		{
			if (args[i_arg] == name && args.Length > i_arg + 1) 
			{
				return args [i_arg + 1];
			}
		}
		return null;
	}


	// handle video state (play pause and stop)
	private void HandleState(Status state, int position) {
		if (position == -1)
			return;
		this.SoundPositionMs = position;
		switch (state)
		{
		case Status.StoppedState:
			this.Stop ();
			break;
		case Status.PausedState:
			this.Pause ();
			break;
		case Status.PlayingState:
			this.Play();
			break;
		default:
			break;
		}
	}

	// handle camera rotation
	private void HandleRotation(float yaw, float pitch, float roll) {
		if (yaw == -1 || pitch == -1 || roll == -1)
			return;
		network.SetSendPermission (false);
		if (oldRotation.x != pitch || oldRotation.y != yaw || oldRotation.z != roll)
        {
            this.CameraRotate(pitch, yaw, roll);
            oldRotation.x = pitch;
            oldRotation.y = yaw;
            oldRotation.z = roll;
        }
		network.SetOldRotation (new Vector3(yaw, pitch, roll));
		network.SetSendPermission (true);
	}


	// handle video switch with the path of the new video
	private void HandleVideoSwitch(string url) {
		if (url == null)
			return;
		videoPath = url;
		videoSwitch = true;
	}

	// OSC message parsing
	public UDPstruct OscMessageHandle(string addr, string str) {
		UDPstruct mess = new UDPstruct();
		char[] delimiters = {' '};
		string url = null;

		if (addr.CompareTo ("/mediaController") != 0) {
			return null;
		}

		string[] words = str.Split (delimiters);

		for (int i_word = 0; i_word < words.Length; i_word++) {

			// get State
			if (words[i_word].CompareTo ("state") == 0 && i_word + 1 < words.Length) {
				try {
					mess.state = (Status)Enum.Parse(typeof(Status), words [i_word + 1]);
				}
				catch (Exception e) {
					Debug.Log ("[NoiseMakers][Parser] Impossible conversion:" + e.ToString());
				}
			}

			// get position in ms
			if (words[i_word].CompareTo ("position") == 0 && i_word + 1 < words.Length) {
				try {
					mess.position = int.Parse(words [i_word + 1]);
				}
				catch (Exception e) {
					Debug.Log ("[NoiseMakers][Parser] Impossible conversion:" + e.ToString());
				}
			}

			// get yaw pitch roll
			if (words[i_word].CompareTo ("yawpitchroll") == 0 && i_word + 3 < words.Length) {
				try {
					mess.yaw = int.Parse(words [i_word + 1]);
					mess.pitch = int.Parse(words [i_word + 2]);
					mess.roll = int.Parse(words [i_word + 3]);
				}
				catch (Exception e) {
					Debug.Log ("[NoiseMakers][Parser] Impossible conversion:" + e.ToString());
				}
			}

			// get exit
			if (words[i_word].CompareTo ("exit") == 0 && i_word + 1 < words.Length) {
				try {
					mess.exit = (int.Parse(words [i_word + 1]) == 1 ? true : false);
				}
				catch (Exception e) {
					Debug.Log ("[NoiseMakers][Parser] Impossible conversion:" + e.ToString());
				}
			}

			// get url
			string[] array = str.Split('\"');
			foreach (var arg in array) {
				foreach (var character in arg) {
					if (character == '/' || character == '\\') {
						url = arg;
						break;
					}
				}
			}
			if (words[i_word].CompareTo ("url") == 0 && i_word + 1 < words.Length && url != null) {
				mess.url = url;
			}

		}
		return mess;
	}


	// dispatch message 
	public void ParseMessage(string addr, string str) {
		UDPstruct message = OscMessageHandle(addr, str);

		if (message == null)
			return;
		if (message.exit == false && message.fov == -1 && message.id == null && message.url == null && message.yaw == -1 &&
			message.pitch == -1 && message.position == -1 && message.roll == -1 && message.state == Status.StoppedState) {
			return;
		}
		if (last_message == null) {
			if (message.exit == true)
				exit = true;
			HandleState (message.state, message.position);
			HandleRotation (message.pitch, message.yaw, message.roll);
			HandleVideoSwitch (message.url);
			last_message = message;
			return;
		}
		if (message.exit == true) {
			exit = true;
		}
		HandleState (message.state, message.position);
		HandleRotation (message.yaw, message.pitch, message.roll);
		if (last_message.url == null || last_message.url.CompareTo(message.url) != 0) {
			HandleVideoSwitch (message.url);
		}
		last_message = message;
	}

    // Update is called once per frame
    void Update()
    {
		if (exit) 
		{
			network.socketShutdown ();
			Application.Quit ();
		}
        if (cameraChanged)
        {
            rotation = cameraRotation.GetRotation();
            cam.transform.eulerAngles = rotation;
            cameraChanged = false;
        }
		if (VREnable) 
		{
			Camera c = cameraRigEyes.GetComponent<Camera> ();
			this.SetCameraRotation (c.transform.eulerAngles);
		}
		if (videoSwitch) 
		{
			Animator splashScreen = GameObject.Find("SplashScreen").GetComponent<Animator>();
			splashScreen.SetTrigger ("HideTrigger");

			//mediaPlayer.OpenVideoFromFile (0, videoPath);
			videoPlayer.url = videoPath;
			videoSwitch = false;
			ResetController ();
		}
		/* let UdpCommunication receives message only when contructor has been called and everything is initialized */
		if (_firstRun) {
			network.startReceive ();
			_firstRun = false;
		}
		if (stateMessageToHandle > 0) {
			videoPlayer.Prepare ();
			VideoPlayerAction ();
		}
    }

	private void VideoPlayerAction() {
		if (isPaused) {
			//videoPlayer.Pause();
			VideoPositionMs = System.Convert.ToInt32(videoPlayer.time);
			if (VideoPositionMs != SoundPositionMs)
			{
				VideoPositionMs = SoundPositionMs;

				// start playing (otherwise it starts from time/frame 0).
				videoPlayer.Play();
				videoPlayer.Pause();
				videoPlayer.time = System.Convert.ToDouble(VideoPositionMs / 1000.0f);
				Debug.Log(string.Format("VideoPlayer time set to: {0:F3}", VideoPositionMs));

			}
		}
		if (isPlaying) {
			videoPlayer.Play();
		}
		if (isStopped) {
			videoPlayer.Stop();
		}
		stateMessageToHandle--;
	}

	// stop the video clip
    private void Stop()
    {
        if (!isStopped)
        {
            isPaused = false;
            isPlaying = false;
            isStopped = true;
			stateMessageToHandle++;
        }
    }


	// pause the video clip and synchronize with the sound
    private void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            isPlaying = false;
            isStopped = false;
			stateMessageToHandle++;
        }
		if (VideoPositionMs != SoundPositionMs)
        {
			VideoPositionMs = SoundPositionMs;
			this.Seek(SoundPositionMs);
        }
    }

	// play the video clip
    private void Play()
    {
        if (!isPlaying)
        {
            isPaused = false;
            isPlaying = true;
            isStopped = false;
			stateMessageToHandle++;
        }
    }


	// seek in the video
    private void Seek(int ms)
    {
        //mediaPlayer.m_Control.Seek(ms);
    }


	// rotate camera when changed with UDP message
    private void CameraRotate(float rotX, float rotY, float rotZ)
    {
		rotation.x = rotX * RadToDegree;
		rotation.y = rotY * RadToDegree;
		rotation.z = rotZ * RadToDegree;
        cameraRotation.SetRotation(rotation);
        cameraChanged = true;
    }


	// set camera's roation when changed with mouse/vive
    public void SetCameraRotation(Vector3 rot)
    {
        this.rotation = rot;
    }

	// get camera rotation (used to send yaw pitch roll in UDPcommunication)
    public Vector3 GetCameraRotation()
    {
        return this.rotation;
    }
}
