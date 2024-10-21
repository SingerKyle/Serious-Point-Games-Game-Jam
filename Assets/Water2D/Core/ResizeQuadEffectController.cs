using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using System;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class ResizeQuadEffectController : MonoBehaviour
{
    public bool FlipTexture = false;
    public int sorting = 0;

    public Rect cullingRect;

    Texture effectTex2d;
    Camera effectCam;
    RenderTexture backTex2d;

    GameObject effectCamera;
    GameObject backgroundCamera;

    float minX, minY, maxX, maxY = 0f;
    float lminX, lminY, lmaxX, lmaxY = 0f;
    float radius;

    Vector3 screenInitPosInWorld;
    Vector3 screenFinalPosInWorld;

    [SerializeField] int sampleSize = 0;

    public static void setMinMaxParticlePosition(Vector2 _pos, float radius)
    {
        if (instance == null)
        {
            RebuildTextures();
        }

        if (_pos.x < instance.minX) instance.minX = _pos.x;
        if (_pos.y < instance.minY) instance.minY = _pos.y;
        if (_pos.x > instance.maxX) instance.maxX = _pos.x;
        if (_pos.y > instance.maxY) instance.maxY = _pos.y;

        if (instance.lminX != instance.minX || instance.lminY != instance.minY ||
            instance.lmaxX != instance.maxX || instance.lmaxY != instance.maxY)
        {
            instance.cullingRect = new Rect(instance.minX, instance.minY, instance.maxX, instance.maxY);
            instance.lminX = instance.minX;
            instance.lminY = instance.minY;
            instance.lmaxX = instance.maxX;
            instance.lmaxY = instance.maxY;

            instance.radius = radius;
        }
    }

    int getSampleSize(int sampleID)
    {
        int r = 32;
        if (sampleID == 0) return r;

        for (int i = 0; i < sampleID; i++)
        {
            r *= 2;
        }

        return r;
    }

    void LoadSampleSize()
    {
#if UNITY_EDITOR
        int sampleID = UnityEditor.EditorPrefs.GetInt("SampleID") >= 0 ? UnityEditor.EditorPrefs.GetInt("SampleID") : 2;
        sampleSize = getSampleSize(sampleID);
#endif
    }

    public static void RebuildTextures(int flipTex = -1)
    {
        if (instance == null)
        {
            ResizeQuadEffectController[] aux = FindObjectsOfType<ResizeQuadEffectController>();
            if (aux.Length > 1)
            {
                for (int i = 1; i < aux.Length; i++)
                {
                    DestroyImmediate(aux[i].gameObject);
                }
            }

            if (aux.Length > 0)
                instance = aux[0].GetComponent<ResizeQuadEffectController>();
        }

        if (flipTex == 0)
            instance.FlipTexture = false;
        if (flipTex == 1)
            instance.FlipTexture = true;

        instance.RebuildRenderTexturesAll();
    }

    public static ResizeQuadEffectController instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        //screenInitPosInWorld = Camera.main.ScreenToWorldPoint(Vector3.zero);
        //screenFinalPosInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    Camera cam;
    Vector3 pos;
    Vector3 size;

    private void Start()
    {
        RebuildRenderTexturesAll();

        gameObject.transform.hideFlags = HideFlags.HideInInspector;

        if (effectCamera == null)
            effectCamera = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : null;

        if (effectCamera == null)
        {
            Debug.LogError("effectCamera is not set or could not be found.");
            return;
        }

        cam = effectCamera.GetComponent<Camera>();

        pos = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        size = cam.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, cam.nearClipPlane));
    }

    void Update()
    {
        if (effectCamera == null)
        {
            effectCamera = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : null;

            if (effectCamera == null)
            {
                Debug.LogError("effectCamera is not set or could not be found.");
                return;
            }
        }

        if (effectCamera.GetComponent<Camera>().targetTexture == null)
        {
            RebuildRenderTexturesAll();
        }

        resetMinMax();

        if (instance != null)
        {
            instance.RebuildMesh();
        }
    }

    private void resetMinMax()
    {
        minY = minX = Mathf.Infinity;
        maxY = maxX = -Mathf.Infinity;
    }

    private Vector3[] _meshVertices;
    Mesh mesh;
    MeshFilter mf;
    Vector3 p1, p2, p3, p4;

    private void RebuildMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        mesh.Clear();

        var vertices = new Vector3[4];

        p1 = transform.InverseTransformPoint(new Vector3(cullingRect.x, cullingRect.y));
        p2 = transform.InverseTransformPoint(new Vector3(cullingRect.x, cullingRect.height));
        p3 = transform.InverseTransformPoint(new Vector3(cullingRect.width, cullingRect.height));
        p4 = transform.InverseTransformPoint(new Vector3(cullingRect.width, cullingRect.y));

        p1.x -= radius;
        p1.y -= radius;

        p2.x -= radius;
        p2.y += radius;

        p3.x += radius;
        p3.y += radius;

        p4.x += radius;
        p4.y -= radius;

        if (effectCamera == null)
        {
            effectCamera = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : null;
            cam = effectCamera.GetComponent<Camera>();
        }

        pos = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        size = cam.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, cam.nearClipPlane));

        if (p1.x < pos.x) p1.x = pos.x;
        if (p1.x > size.x) p1.x = size.x;
        if (p1.y < pos.y) p1.y = pos.y;

        if (p2.x < pos.x) p2.x = pos.x;
        if (p2.x > size.x) p2.x = size.x;
        if (p2.y > size.y) p2.y = size.y;

        if (p3.x > size.x) p3.x = size.x;
        if (p3.y > size.y) p3.y = size.y;

        if (p4.x > size.x) p4.x = size.x;
        if (p4.y < pos.y) p4.y = pos.y;

        vertices[0] = p1;
        vertices[1] = p2;
        vertices[2] = p3;
        vertices[3] = p4;

        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        mesh.RecalculateBounds();

        if (mf == null) mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;

        transform.position = new Vector3(0, 0, 10f);
        transform.localScale = Vector3.one;
    }

    public void AboutToRebuildAll()
    {
        effectCamera.GetComponent<Camera>().targetTexture = null;
    }

    public void RebuildRenderTexturesAll()
    {
#if UNITY_EDITOR
        LoadSampleSize(); // read from unity prefs
#endif

        if (sampleSize <= 0)
        {
            Debug.LogError("Sample tex is size 0");
            return;
        }

        if (effectCamera == null)
        {
            effectCamera = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : null;

            if (effectCamera == null)
            {
                Debug.LogError("effectCamera is not set or could not be found.");
                return;
            }
        }

        float size = sampleSize;
        float ratio;
        int width, height;

        ratio = (float)Camera.main.pixelHeight / (float)Camera.main.pixelWidth;

        width = (int)(size);
        height = (int)(size * ratio);

        if (gameObject.transform.parent.parent.Find("0-BGCamera"))
        {
            backgroundCamera = gameObject.transform.parent.parent.Find("0-BGCamera").gameObject;
        }

        if (backgroundCamera != null)
        {
            RenderTexture BackgroundRT = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_BackgroundTex", BackgroundRT);
            backgroundCamera.GetComponent<Camera>().forceIntoRenderTexture = true;
            backgroundCamera.GetComponent<Camera>().targetTexture = BackgroundRT;
        }

        RenderTexture EffectRT = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", EffectRT);
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_EffectTex", EffectRT);

        effectCamera.GetComponent<Camera>().forceIntoRenderTexture = true;
        effectCamera.GetComponent<Camera>().targetTexture = EffectRT;

        EffectRT.Release();

#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
        try
        {
            UnityEditor.SceneVisibilityManager.instance.Hide(gameObject, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
#endif

#if UNITY_EDITOR
        FlipTexture = UnityEditor.EditorPrefs.GetBool("_flipTexEditor");

        if (FlipTexture && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
        {
            GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_FlipTex", 1.0f);
        }
        else
        {
            GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_FlipTex", 0.0f);
        }
#endif

#if !UNITY_EDITOR
        if (FlipTexture && (Application.platform == RuntimePlatform.WindowsPlayer))
        {
            GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_FlipTex", 1.0f);
        }
        else
        {
            GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_FlipTex", 0.0f);
        }
#endif
    }

    public void SetSorting(int id = 0)
    {
        GetComponent<MeshRenderer>().sortingOrder = id;
    }
}
