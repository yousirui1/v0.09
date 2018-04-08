using System.Collections;
//using UnityEngine.EventSystem;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIRoot : MonoBehaviour
{
    private static UIRoot m_Instance = null;
    public static UIRoot Instance
    {
        get
        {
            if (null == m_Instance)
            {
                InitRoot();
            }

            return m_Instance;
        }
    }


    public Transform root;
    public Transform fixedRoot;
    public Transform normalRoot;
    public Transform popupRoot;
    public Camera uiCamera;



    static void InitRoot()
    {
        GameObject go = new GameObject("UIRoot");
        go.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = go.AddComponent<Canvas>();
        m_Instance = go.AddComponent<UIRoot>();
        go.AddComponent<RectTransform>();
        go.AddComponent<GraphicRaycaster>();
        

        m_Instance.root = go.transform;

        GameObject camObj = new GameObject("UICamera");
        camObj.layer = LayerMask.NameToLayer("UI");
        camObj.transform.parent = go.transform;
        camObj.transform.localPosition = new Vector3(0, 0, -100f);
       


        Camera camera = camObj.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Depth;
        camera.orthographic = true;
        camera.farClipPlane = 200f;
        camera.cullingMask = 1 << 5;
        camera.nearClipPlane = -50f;
        camera.farClipPlane = 50f;
        //add audio listener
        //camObj.AddComponent<AudioListener>();
        camObj.AddComponent<GUILayer>();
        m_Instance.uiCamera = camera;


     
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = true;
        canvas.worldCamera = camera;





        CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1280f, 720f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;



        GameObject subRoot;


        subRoot = CreateSubCanvasForRoot(go.transform, 0);
        subRoot.name = "NormalRoot";
        m_Instance.normalRoot = subRoot.transform;
        m_Instance.normalRoot.transform.localScale = Vector3.one;

        subRoot = CreateSubCanvasForRoot(go.transform, 250);
        subRoot.name = "FixedRoot";
        m_Instance.fixedRoot = subRoot.transform;
        m_Instance.fixedRoot.transform.localScale = Vector3.one;


        subRoot = CreateSubCanvasForRoot(go.transform, 500);
        subRoot.name = "PopupRoot";
        m_Instance.popupRoot = subRoot.transform;
        m_Instance.popupRoot.transform.localScale = Vector3.one;

        
        GameObject esObj = GameObject.Find("EventSystem");
        if (esObj != null)
        {
            GameObject.DestroyImmediate(esObj);

        }

        GameObject eventObj = new GameObject("EventSystem");
        eventObj.layer = LayerMask.NameToLayer("UI");
        eventObj.transform.SetParent(go.transform);
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

    }


    static GameObject CreateSubCanvasForRoot(Transform root, int sort)
    {
        GameObject go = new GameObject("canvas");
        go.transform.parent = root;
        go.layer = LayerMask.NameToLayer("UI");

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        return go;

    }

    void OnDestroy()
    {
        m_Instance = null;

    }




}
