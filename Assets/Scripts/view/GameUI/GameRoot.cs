using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

/**************************************
*FileName: GameRoot.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 基类主要用于设置和初始化Canvas等
*		   和不同尺寸的适配
**************************************/

public class GameRoot : MonoBehaviour
{
	private static GameRoot m_Instance = null;
	public static GameRoot Instance
	{
		get
		{
			if(null == m_Instance)
			{
				InitRoot();
			}

			return m_Instance;
		}
	}


	public Transform root;
	public Camera uiCamera;

	
	static void InitRoot()
	{
		GameObject obj = new GameObject("GameRoot");
		obj.layer = LayerMask.NameToLayer("UI");
		Canvas canvas = obj.AddComponent<Canvas>();
		m_Instance = obj.AddComponent<GameRoot>();
		obj.AddComponent<RectTransform>();
		obj.AddComponent<GraphicRaycaster>();

		m_Instance.root = obj.transform;
		
		GameObject camObj = new GameObject("UICamera");
		camObj.layer = LayerMask.NameToLayer("UI");
		camObj.transform.parent = obj.transform;
		camObj.transform.localPosition = new Vector3(0, 0, -100f);
		
		Camera camera = camObj.AddComponent<Camera>();
		camera.clearFlags = CameraClearFlags.Depth;
		camera.orthographic =true;
		camera.farClipPlane = 200f;
		//camera.cullingMask = 1<< 5;
		camera.cullingMask = -1;
	
		camera.nearClipPlane = -50f;
		camera.farClipPlane = 50f;
		camObj.AddComponent<GUILayer>();
		m_Instance.uiCamera = camera;

		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.pixelPerfect = true;
		canvas.worldCamera = camera;
		
		CanvasScaler canvasScaler = obj.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1280f, 720f);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		
		GameObject esObj = GameObject.Find("EventSystem");
		if(esObj != null)
		{
			GameObject.DestroyImmediate(esObj);
		}

		GameObject eventObj = new GameObject("EventSystem");
		eventObj.layer = LayerMask.NameToLayer("UI");
		eventObj.transform.SetParent(obj.transform);
		eventObj.AddComponent<EventSystem>();
		eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

		
	}

	void OnDestroy()
	{
		m_Instance = null;
	}
		

} 
