using UnityEngine;

public class nEmitter : MonoBehaviour
{
    public nParticle particle = null;
    public Transform renderTransform = null;
    public float pps = 10;
    public Vector2 lifeSpanRange = new Vector2(1, 3);
    public float startTime = 0f;
    public float stopTime = -1f;
    Vector3 spawnP;
    float spawnInterval;
    float nextSpawnTime = 0;
    float _time;
    Vector3 position;
    Vector3 tempSphere;
    nCreator creator;
    float emitterEnableTime;
    nInitSize initSize;
    float _startSize = 1f;
    nInitVelocity initVelocity;
    Vector3 _startVelocity = Vector3.zero;
    nInitRotation initRotation;
    float _startRotation = 0f;

    void Awake()
    {
        creator = GetComponent<nCreator>();
        initSize = GetComponent<nInitSize>();
        initVelocity = GetComponent<nInitVelocity>();
        initRotation = GetComponent<nInitRotation>();

    }

    void Start()
    {
        spawnInterval = 1 / pps;
        if (initSize != null)
        {
            _startSize = initSize.size;
        }

        if (renderTransform == null)
        {
            renderTransform = nParticleRoot.worldTransform;
        }
    }

    void OnEnable()
    {
        emitterEnableTime = Time.time;
        nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (creator == null || !creator.enabled)
        {
            return;
        }

        if (stopTime > 0)
        {
            if ((Time.time - emitterEnableTime) >= stopTime)
            {
                enabled = false;
            }
        }

        while (Time.time >= nextSpawnTime)
        {
            tempSphere = Random.insideUnitSphere;
            spawnP = creator.getSpawnPosition();
            position = transform.TransformPoint(spawnP);
            if (initVelocity != null)
            {
                _startVelocity = initVelocity.GetVelocity(ref spawnP);
            }

            if (initRotation != null)
            {
                _startRotation = initRotation.GetRotation();
            }

            SpawnParticle(
                particle,
                renderTransform,
                position,
                _startRotation,
                Random.Range(lifeSpanRange.x, lifeSpanRange.y),
                _startSize,
                _startVelocity);
            nextSpawnTime += spawnInterval;
        }
        _time = Time.time;
    }

    static void SpawnParticle(nParticle particle, Transform world, Vector3 position, float rotation, float lifeSpan, float startSize, Vector3 starVelocity)
    {
        GameObject newParticle = Instantiate(particle.gameObject, position, Quaternion.identity) as GameObject;

        RectTransform rect = newParticle.GetComponent<RectTransform>();

        rect.SetParent(world);
        rect.localScale = new Vector3(1f, 1f, 1f);
        rect.localEulerAngles = new Vector3(0f, 0f, rotation);

        nParticle nP = newParticle.GetComponent<nParticle>();
        nP.startSize = startSize;
        nP.startVelocity = starVelocity;
    }
}