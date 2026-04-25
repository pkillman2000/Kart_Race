using UnityEngine;

/// <summary>
/// A self-contained spark particle effect for car collisions.
/// Create a GameObject with this script and ParticleSystem, then save as a prefab.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class CarCollisionSparks : MonoBehaviour
{
    [Header("Spark Settings")]
    [Tooltip("Number of spark particles to emit.")]
    [SerializeField] private int sparkCount = 30;

    [Tooltip("Minimum speed of sparks.")]
    [SerializeField] private float minSparkSpeed = 5f;

    [Tooltip("Maximum speed of sparks.")]
    [SerializeField] private float maxSparkSpeed = 15f;

    [Tooltip("How long each spark lasts.")]
    [SerializeField] private float sparkLifetime = 0.5f;

    [Tooltip("Size of spark particles.")]
    [SerializeField] private float sparkSize = 0.1f;

    [Header("Colors")]
    [Tooltip("Color of sparks at start.")]
    [SerializeField] private Color startColor = new Color(1f, 0.8f, 0.2f); // Orange-yellow

    [Tooltip("Color of sparks at end.")]
    [SerializeField] private Color endColor = new Color(1f, 0.2f, 0.1f); // Red-orange

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private ParticleSystem.EmissionModule _emissionModule;
    private ParticleSystem.ShapeModule _shapeModule;

    private void Awake()
    {
        SetupParticleSystem();
    }

    private void SetupParticleSystem()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        // Configure main module
        _mainModule = _particleSystem.main;
        _mainModule.startLifetime = sparkLifetime;
        _mainModule.startSpeed = new ParticleSystem.MinMaxCurve(minSparkSpeed, maxSparkSpeed);
        _mainModule.startSize = sparkSize;
        _mainModule.startColor = new ParticleSystem.MinMaxGradient(startColor, endColor);
        _mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        _mainModule.playOnAwake = false;
        _mainModule.stopAction = ParticleSystemStopAction.Destroy;

        // Configure emission
        _emissionModule = _particleSystem.emission;
        _emissionModule.enabled = true;
        _emissionModule.rateOverTime = 0;
        _emissionModule.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, sparkCount)
        });

        // Configure shape (cone for directional sparks)
        _shapeModule = _particleSystem.shape;
        _shapeModule.enabled = true;
        _shapeModule.shapeType = ParticleSystemShapeType.Cone;
        _shapeModule.angle = 25f;
        _shapeModule.radius = 0.1f;

        // Add velocity limit
        var limitVelocity = _particleSystem.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.drag = 2f;

        // Add size over lifetime (sparks shrink)
        var sizeOverLifetime = _particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        ));

        // Add color over lifetime (fade out)
        var colorOverLifetime = _particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // Add trail for sparks
        var trails = _particleSystem.trails;
        trails.enabled = true;
        trails.ratio = 0.5f;
        trails.lifetime = 0.2f;
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 0.3f),
            new Keyframe(1f, 0f)
        ));

        // Configure renderer
        var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 2f;
        renderer.velocityScale = 0.1f;
    }

    /// <summary>
    /// Plays the spark effect with a specific intensity (affects particle count).
    /// </summary>
    public void Play(float intensity = 1f)
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        // Adjust particle count based on intensity
        int adjustedCount = Mathf.RoundToInt(sparkCount * intensity);
        _emissionModule.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, adjustedCount)
        });

        _particleSystem.Play();
    }

    /// <summary>
    /// Creates a spark effect prefab at runtime (useful for testing).
    /// </summary>
    public static GameObject CreateSparkPrefab()
    {
        GameObject sparkObject = new GameObject("CarCollisionSparks");
        CarCollisionSparks sparks = sparkObject.AddComponent<CarCollisionSparks>();
        sparkObject.AddComponent<ParticleSystem>();
        sparks.SetupParticleSystem();
        return sparkObject;
    }
}