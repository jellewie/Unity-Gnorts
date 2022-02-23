using PublicCode;
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

		kernel = GaussianKernel.Calculate(5, 21);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (false)
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