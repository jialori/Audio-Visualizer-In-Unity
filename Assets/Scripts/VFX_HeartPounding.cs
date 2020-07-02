using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class VFX_HeartPounding : MonoBehaviour, IObserver
{
	public Renderer heartRenderer;

	public int highBeatThreshold;
	public int midBeatThreshold;
	[Tooltip("Using a third change will likely make the graphics flashing and might cause dizzy feelings.")]
	public bool useThirdChange;
	public int lowBeatThreshold;
	public Color defaultColor;
	public Color explodeColor;

    void Start()
    {
        AudioBeat.instance.Attach(this);
		heartRenderer.material.SetColor("_TintColor", defaultColor);
    }

    void OnDestroy()
    {
        AudioBeat.instance.Detach(this);
    }

    public void UpdateOnChange(ISubject subject)
	{
		int numBeat = (subject as AudioBeat).beat_count;
		if (numBeat >= highBeatThreshold) {
			heartRenderer.material.SetFloat("_Distance", 10f);
			heartRenderer.material.SetColor("_TintColor", explodeColor);
			// heartRenderer.material.SetFloat("_Speed", 45f);
		} 
		else if (numBeat >= midBeatThreshold) {
			heartRenderer.material.SetFloat("_Distance", 8f);
			heartRenderer.material.SetColor("_TintColor", defaultColor);
			// heartRenderer.material.SetFloat("_Speed", 100f);
		} 
		else if (useThirdChange && numBeat >= lowBeatThreshold) {
			heartRenderer.material.SetFloat("_Distance", 4.57f);
			heartRenderer.material.SetColor("_TintColor", defaultColor);
			// heartRenderer.material.SetFloat("_Speed", 100f);
		}
	}
}

	// public Shader Shader_heart_pounding;
	// public Shader Shader_heart_default;

	// public Renderer sphereRenderer;
	// public Shader Shader_sphere_glitch;
	// public Shader Shader_sphere_default;

	// public float poundChance = 0.1f;
	// public WaitForSeconds poundLoopWait = new WaitForSeconds(.1f);
	// public WaitForSeconds poundDuration = new WaitForSeconds(.1f);

	// IEnumerator Start()
	// {
	// 	while (true)
	// 	{
	// 		float poundTest = Random.Range(0f, 1f);

	// 		if (poundTest <= poundChance)
	// 		{
	// 			StartCoroutine(Pound());
	// 		}
	// 		yield return poundLoopWait;
	// 	}
	// }

	// IEnumerator Pound()
	// {
	// 	poundDuration = new WaitForSeconds(Random.Range(.05f, .25f));
	// 	// heart
	// 	heartRenderer.material.shader = Shader_heart_pounding;
	// 	sphereRenderer.material.shader = Shader_sphere_glitch;
	// 	yield return poundDuration;
	// 	heartRenderer.material.shader = Shader_heart_default;
	// 	sphereRenderer.material.shader = Shader_sphere_default;
	// 	// heartRenderer.material.SetFloat("_Amount", 0f);
	// }

// TODO: customize the editor presentation: only show `lowThreshold` if `useThird` is set to true
 // [CustomEditor(typeof(VFX_HeartPounding))]
 // public class MyScriptEditor : Editor
 // {
 //   void OnInspectorGUI()
 //   {
 //     var myScript = target as MyScript;
 
 //     myScript.flag = GUILayout.Toggle(myScript.flag, "Flag");
     
 //     if(myScript.flag)
 //       myScript.i = EditorGUILayout.IntSlider("I field:", myScript.i , 1 , 100);
 
 //   }
 // }