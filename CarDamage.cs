///--------------------------------------------------------------------------------------------
/// Simple Car Damage system By Ciorbyn Studio https://www.youtube.com/c/CiorbynStudio
/// Tutorial link: https://youtu.be/l04cw7EChpI
/// -------------------------------------------------------------------------------------------
using System.Diagnostics;
using UnityEngine;

public class CarDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float hits;
    public float maxhits = 5f;

    [Tooltip("Assign a smoke GameObject (e.g., a ParticleSystem GameObject).")]
    public GameObject CarSmoke;

    [Range(0.01f, 5f)] public float maxMoveDelta = 1.0f;
    public float maxCollisionStrength = 50.0f;
    [Range(0f, 1f)] public float YforceDamp = 0.1f;
    public float demolutionRange = 0.5f;
    [Range(0f, 1f)] public float impactDirManipulator = 0.0f;

    [Tooltip("Optional explicit mesh list; if empty, will auto-grab from children.")]
    public MeshFilter[] MeshList;

    [Header("Audio")]
    public AudioSource Crash;

    private MeshFilter[] meshfilters;
    private float sqrDemRange;

    // Save Vertex Data
    private struct permaVertsColl
    {
        public Vector3[] permaVerts;
    }
    private permaVertsColl[] originalMeshData;
    int i;

    [Header("Wheel Colliders")]
    public WheelCollider WC1;
    public WheelCollider WC2;
    public WheelCollider WC3;
    public WheelCollider WC4;

    [Header("Wheel Mesh Holders (Optional)")]
    public GameObject FRW;
    public GameObject FLW;
    public GameObject RRW;
    public GameObject RLW;

    [Header("Wheel Damage Scripts (Optional)")]
    public GameObject WheelDamageScript1;
    public GameObject WheelDamageScript2;
    public GameObject WheelDamageScript3;
    public GameObject WheelDamageScript4;

    void Awake()
    {
        // Try to auto-find a smoke object if not assigned.
        if (CarSmoke == null)
        {
            // Try common child names
            Transform t = transform.Find("Smoke") ?? transform.Find("EngineSmoke") ?? transform.Find("CarSmoke");
            if (t != null) CarSmoke = t.gameObject;

            // Fallback: find any ParticleSystem under this object (even if inactive)
            if (CarSmoke == null)
            {
                var ps = GetComponentInChildren<ParticleSystem>(true);
                if (ps != null) CarSmoke = ps.gameObject;
            }

            if (CarSmoke == null)
            {
                //Debug.LogWarning("[CarDamage] CarSmoke is not assigned and could not be auto-found. Assign it in the Inspector.");
            }
        }
    }

    public void Start()
    {
        meshfilters = (MeshList != null && MeshList.Length > 0)
            ? MeshList
            : GetComponentsInChildren<MeshFilter>();

        sqrDemRange = demolutionRange * demolutionRange;
        LoadOriginalMeshData();

        // Ensure smoke starts OFF
        if (CarSmoke != null) CarSmoke.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Repair();
    }

    void LoadOriginalMeshData()
    {
        if (meshfilters == null) { meshfilters = new MeshFilter[0]; }
        originalMeshData = new permaVertsColl[meshfilters.Length];

        for (i = 0; i < meshfilters.Length; i++)
        {
            var mf = meshfilters[i];
            var mesh = (mf != null) ? (mf.sharedMesh ?? mf.mesh) : null;
            originalMeshData[i].permaVerts = (mesh != null && mesh.vertices != null)
                ? mesh.vertices
                : new Vector3[0];
        }
    }


    [ContextMenu("Repair Now")]
    void Repair()
    {
        for (int i = 0; i < meshfilters.Length; i++)
        {
            meshfilters[i].mesh.vertices = originalMeshData[i].permaVerts;
            meshfilters[i].mesh.RecalculateNormals();
            meshfilters[i].mesh.RecalculateBounds();
        }

        // Reset visuals and state
        if (CarSmoke != null) CarSmoke.SetActive(false); // turn OFF smoke on repair
        hits = 0;

        if (FRW) FRW.SetActive(true);
        if (FLW) FLW.SetActive(true);
        if (RRW) RRW.SetActive(true);
        if (RLW) RLW.SetActive(true);

        if (WC1) WC1.radius = .4f;
        if (WC2) WC2.radius = .4f;
        if (WC3) WC3.radius = .4f;
        if (WC4) WC4.radius = .4f;

        if (WheelDamageScript1) WheelDamageScript1.SetActive(true);
        if (WheelDamageScript2) WheelDamageScript2.SetActive(true);
        if (WheelDamageScript3) WheelDamageScript3.SetActive(true);
        if (WheelDamageScript4) WheelDamageScript4.SetActive(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision == null || collision.contactCount == 0) return;

        Vector3 colRelVel = collision.relativeVelocity;
        colRelVel.y *= YforceDamp;

        var contact = collision.GetContact(0); // safe because of contactCount check
        Vector3 colPointToMe = transform.position - contact.point;

        float colStrength = colRelVel.magnitude * Vector3.Dot(contact.normal, colPointToMe.normalized);
        OnMeshForce(contact.point, Mathf.Clamp01(colStrength / maxCollisionStrength));
    }


    // if called by SendMessage(), we only have 1 param

    public void OnMeshForce(Vector3 originPos, float force)
    {
        if (Crash != null) Crash.Play();

        hits += 1f;
        if (hits > maxhits && CarSmoke != null) CarSmoke.SetActive(true);

        force = Mathf.Clamp01(force);

        if (meshfilters == null || meshfilters.Length == 0) return;

        for (int j = 0; j < meshfilters.Length; ++j)
        {
            var mf = meshfilters[j];
            if (mf == null) continue;                         // MeshList slot empty
            var mesh = mf.sharedMesh ?? mf.mesh;
            if (mesh == null) continue;                       // MeshFilter without mesh

            Vector3[] verts = mesh.vertices;
            if (verts == null || verts.Length == 0) continue; // corrupted/empty mesh

            for (int i = 0; i < verts.Length; ++i)
            {
                Vector3 scaledVert = Vector3.Scale(verts[i], transform.localScale);
                Vector3 vertWorldPos = mf.transform.position + (mf.transform.rotation * scaledVert);
                Vector3 originToMeDir = vertWorldPos - originPos;
                Vector3 flatVertToCenterDir = transform.position - vertWorldPos; flatVertToCenterDir.y = 0.0f;

                if (originToMeDir.sqrMagnitude < sqrDemRange)
                {
                    float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude / sqrDemRange);
                    float moveDelta = force * (1.0f - dist) * maxMoveDelta;
                    Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;
                    verts[i] += Quaternion.Inverse(transform.rotation) * moveDir;
                }
            }

            mesh.vertices = verts;
            mesh.RecalculateBounds();
            // optional: mesh.RecalculateNormals();
        }
    }

}
