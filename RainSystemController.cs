
using UnityEngine;

public class RainSystemController : MonoBehaviour
{
    private static RainSystemController _instance;
    private ParticleSystem _rainPS;
    private AudioSource _rainAudio;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Setup();
    }

    private void Setup()
    {
        _rainPS = GetComponentInChildren<ParticleSystem>();
        if (_rainPS == null)
        {
            var child = new GameObject("RainParticles");
            child.transform.SetParent(transform);
            child.transform.localPosition = new Vector3(0, 20f, 0);
            child.transform.localRotation = Quaternion.identity;

            _rainPS = child.AddComponent<ParticleSystem>();
            var main = _rainPS.main;
            main.startSpeed = 30f;
            main.startSize = 0.05f;
            main.startLifetime = 2.0f;
            main.maxParticles = 5000;

            var emission = _rainPS.emission;
            emission.rateOverTime = 2000f;

            var shape = _rainPS.shape;
            shape.shapeType = ParticleSystemShapeType.ConeVolume;
            shape.angle = 0f;
            shape.radius = 20f;
            shape.length = 0f;

            var vel = _rainPS.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.World;
            vel.y = new ParticleSystem.MinMaxCurve(-40f);

            var renderer = _rainPS.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.lengthScale = 5f;
        }

        _rainAudio = GetComponent<AudioSource>();
        if (_rainAudio == null)
        {
            _rainAudio = gameObject.AddComponent<AudioSource>();
            _rainAudio.loop = true;
            _rainAudio.playOnAwake = false;
        }
    }

    public static void SetRainActive(bool active)
    {
        if (_instance == null)
        {
            var go = new GameObject("[RainSystemController]");
            _instance = go.AddComponent<RainSystemController>();
        }

        if (_instance._rainPS == null)
            _instance.Setup();

        var emission = _instance._rainPS.emission;
        emission.enabled = active;

        if (active)
        {
            if (!_instance._rainPS.isPlaying) _instance._rainPS.Play();
            if (_instance._rainAudio != null && !_instance._rainAudio.isPlaying)
            {
                _instance._rainAudio.volume = 0.2f;
            }
        }
        else
        {
            if (_instance._rainPS.isPlaying) _instance._rainPS.Stop();
            if (_instance._rainAudio != null && _instance._rainAudio.isPlaying)
                _instance._rainAudio.Stop();
        }
    }
}
