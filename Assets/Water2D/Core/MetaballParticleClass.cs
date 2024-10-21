using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MetaballParticleClass : MonoBehaviour
{
    public GameObject MObject;
    public float LifeTime;
    public Water2D.Water2D_Spawner SpawnerParent;

    public bool Active
    {
        get { return _active; }
        set
        {
            _active = value;
            if (MObject)
            {
                MObject.SetActive(value);
                if (tr)
                    tr.Clear();
            }
            else
            {
                Debug.LogWarning("MObject is not assigned.");
            }

            if (value)
            {
                delta *= 0;
                wakeUpTime = Time.time;

                if (glowSP)
                    glowSP.enabled = true;
            }

            if (!value)
            {
                // Subtract 1 unit of particle usage.
                if (SpawnerParent)
                    SpawnerParent.DropsUsed--;
                else
                    Debug.LogWarning("SpawnerParent is not assigned.");

                if (rb != null) // reset speed simulation in Editor
                    rb.velocity *= 0f;

                if (_isFreeze)
                {
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.sharedMaterial = null;
                    _isFreeze = false;
                }

                delta *= 0;
            }
            ScaleDownIsPerforming = false;
        }
    }

    public bool witinTarget;
    public Vector2 Editor_Velocity; // velocity used within editor simulation
    public Vector2 Velocity_Limiter_X; // Limiter of speed in X in simulation
    public Vector2 Velocity_Limiter_Y; // Limiter of speed in Y in simulation

    public bool ScaleDown = false;
    public float endSize = 0f;
    public bool BlendingColor;
    public SpriteRenderer glowSP;

    private bool _active;
    private bool _isFreeze;
    private float delta;
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private TrailRenderer tr;
    private SpriteRenderer sr;
    private Collider2D[] Contacts;
    private float deltaSimul;
    private float fixedDeltaSimul = 0f;
    private float wakeUpTime;

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }
#endif

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        cc = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        Contacts = new Collider2D[4];

        if (rb == null) Debug.LogError("Rigidbody2D is missing.");
        if (cc == null) Debug.LogError("CircleCollider2D is missing.");
        if (sr == null) Debug.LogError("SpriteRenderer is missing.");
    }

    void Update()
    {
        if (Active == true)
        {
            // Cache the sprite renderer component
            if (SpawnerParent?.GlowEffect == true && glowSP == null)
            {
                Transform t = MObject?.transform.Find("_glow");
                if (t != null)
                    glowSP = t.GetComponent<SpriteRenderer>();
                else
                    Debug.LogWarning("Glow object not found.");
            }

            if (sr != null && sr.isVisible)
                ResizeQuadEffectController.setMinMaxParticlePosition(transform.position, transform.localScale.x * 0.12f);

            if (ScaleDown)
                ScaleItDown();

            VelocityLimiter();

            if (BlendingColor)
                Blend();

            if (SpawnerParent?.Water2DEmissionType == Water2D.Water2D_Spawner.EmissionType.FillerCollider)
                return;

            if (LifeTime < 0)
                return;

            if (delta > LifeTime)
            {
                delta *= 0;
                Active = false;
            }
            else
            {
                delta += Time.deltaTime;
            }

            // Handle collisions in fixed time only in the editor
            if (deltaSimul > fixedDeltaSimul)
            {
                deltaSimul *= 0;
                OnCollisionEnter2DEditor();
            }
            else
            {
                deltaSimul += Time.deltaTime;
            }
        }
    }

    void VelocityLimiter()
    {
        if (rb == null)
            return;

        Vector2 _vel = rb.velocity;

        if (_vel.x < Velocity_Limiter_X.x)
            _vel.x = Velocity_Limiter_X.x;
        if (_vel.x > Velocity_Limiter_X.y)
            _vel.x = Velocity_Limiter_X.y;
        if (_vel.y < Velocity_Limiter_Y.x)
            _vel.y = Velocity_Limiter_Y.x;
        if (_vel.y > Velocity_Limiter_Y.y)
            _vel.y = Velocity_Limiter_Y.y;

        rb.velocity = _vel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SpawnerParent != null)
            SpawnerParent.InvokeOnCollisionEnter2D(gameObject, collision.contacts[0].collider.gameObject);
        else
            Debug.LogWarning("SpawnerParent is not assigned.");
    }

    private void OnCollisionEnter2DEditor()
    {
        if (Application.isPlaying || cc == null || Contacts == null)
            return;

        int i = Physics2D.OverlapCircleNonAlloc(rb.position, cc.radius * 0.9f, Contacts);
        if (i > 0)
        {
            for (int j = 0; j < Contacts.Length; j++)
            {
                if (Contacts[j] == null || Contacts[j].GetInstanceID() == gameObject.GetInstanceID())
                    continue;

                SpawnerParent?.InvokeOnCollisionEnter2D(gameObject, Contacts[j].gameObject);
            }
        }
    }

    bool ScaleDownIsPerforming = false;
    void ScaleItDown()
    {
        // Initializes
        if (!ScaleDownIsPerforming)
        {
            ScaleDownIsPerforming = true;
        }

        if (ScaleDownIsPerforming)
            ScaleDownPerform();
    }

    void ScaleDownPerform()
    {
        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(endSize, endSize), (delta / LifeTime) * Time.deltaTime * 0.8f);
    }

    void Blend()
    {
        if (cc == null || Contacts == null)
            return;

        int i = Physics2D.OverlapCircleNonAlloc(rb.position, cc.radius * 8f, Contacts, 1 << gameObject.layer);
        if (i > 0)
        {
            for (int j = 0; j < Contacts.Length; j++)
            {
                if (Contacts[j] == null || Contacts[j].tag != "Metaball_liquid")
                    continue;

                Color c2 = Contacts[j].GetComponent<SpriteRenderer>().color;
                if (c2 == sr.color)
                    return;

                sr.color = Color.Lerp(sr.color, c2, 0.045f);
                if (Contacts[j].GetComponent<MetaballParticleClass>().SpawnerParent.Blending)
                {
                    Contacts[j].GetComponent<SpriteRenderer>().color = Color.Lerp(c2, sr.color, 0.045f);
                }
            }
        }
    }

    // Re-adding missing methods
    public void SetColor(Color _color)
    {
        if (sr != null)
        {
            sr.color = _color;
        }
    }

    public void SetFreeze(bool _val = true)
    {
        _isFreeze = _val;
        rb.constraints = _val ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
    }

    public bool GetFreeze()
    {
        return _isFreeze;
    }

    public void SetHighDensity(bool _val = true)
    {
        _isFreeze = _val;
        rb.angularDrag = _val ? 25f : 0f;
        rb.constraints = _val ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.None;
    }

    public void removeGlow()
    {
        if (glowSP != null)
        {
            glowSP.enabled = false;
        }
    }
}
