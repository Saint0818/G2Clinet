using UnityEngine;
using System.Collections;
using System.Threading;

public class FpsLimiter : MonoBehaviour {
	float oldTime = 0.0F;
	float theDeltaTime= 0.0F;
	float curTime= 0.0F;
	float timeTaken = 0.0F;
	public int FrameRate = 30;
	public bool ShowFPS = false;

	private  float updateInterval = 0.5F;
	private string fpsText = "";
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = FrameRate;
		QualitySettings.vSyncCount = 1;
		theDeltaTime = (1.0F /FrameRate);
		oldTime = Time.realtimeSinceStartup;

		timeleft = updateInterval;  
	}

	void OnGUI() {
		if (ShowFPS) {
			GUI.Label(new Rect(100, 0, 100, 36), fpsText);
		}
	}

	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			fpsText = System.String.Format("{0:F2} FPS",fps);

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}

	// Update is called once per frame
	void LateUpdate () {
		curTime = Time.realtimeSinceStartup;
		timeTaken = (curTime - oldTime);
		if(timeTaken < theDeltaTime){
			Thread.Sleep((int)(1000*(theDeltaTime - timeTaken)));
		}
		
		oldTime = Time.realtimeSinceStartup;
	}
}