using UnityEngine;

public class Outliner : MonoBehaviour
{
	[SerializeField]
	public Shader DrawAsSolidColor;
	[SerializeField]
	public Shader Outline;
	Material _outlineMaterial;
	Camera TempCam;
	float[] kernel;

	void Start()
	{		
		_outlineMaterial = new Material(Outline);
		TempCam = new GameObject().AddComponent<Camera>();
		TempCam.name = "Outliner Camera";							
		TempCam.transform.position = gameObject.transform.position;                     //Setting outliner camera in same coordinates as the main camera before setting it... 
		TempCam.transform.rotation = gameObject.transform.rotation;						//.. as parent so when unity asigns it as parent it aligns where we want it
		TempCam.transform.SetParent(gameObject.transform);								//We set it as child so it follows the main camera and takes advantage of the already implemented code
		TempCam.depth = 2;																//The highest render order is the least number(e.g. maincamera is 0 depth)
		kernel = GaussianKernel.Calculate(5, 21);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (Manager.instance.isSelected == true)
		{
			TempCam.CopyFrom(Camera.current);
			TempCam.backgroundColor = Color.black;
			TempCam.clearFlags = CameraClearFlags.Color;
			TempCam.cullingMask = 1 << LayerMask.NameToLayer("Building");
			var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.R8);
			TempCam.targetTexture = rt;
			TempCam.RenderWithShader(DrawAsSolidColor, "");
			_outlineMaterial.SetFloatArray("kernel", kernel);
			_outlineMaterial.SetInt("_kernelWidth", kernel.Length);
			_outlineMaterial.SetTexture("_SceneTex", src);
			//No need for more than 1 sample, which also makes the mask a little bigger than it should be.
			rt.filterMode = FilterMode.Point;
			Graphics.Blit(rt, dst, _outlineMaterial);
			TempCam.targetTexture = src;
			RenderTexture.ReleaseTemporary(rt);
		}
	}
}