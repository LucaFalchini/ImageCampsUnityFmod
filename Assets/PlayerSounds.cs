using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private FMODUnity.EventReference footstepsEvent;
    [SerializeField] private FMODUnity.EventReference jumpEvent;
    [SerializeField] private FMODUnity.EventReference landEvent;

    private FMOD.Studio.EventInstance footsteps;

    [Header("Ground Detection")]
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private Vector3 rayOffset = new Vector3(0f, 0.3f, 0f);
    [SerializeField] private LayerMask groundLayers;

    private float currentSurface = -1f;

    private void Awake()
    {
        if (!footstepsEvent.IsNull)
        {
            footsteps = FMODUnity.RuntimeManager.CreateInstance(footstepsEvent);
        }
    }

    /* =========================
       FOOTSTEPS
       ========================= */

    public void PlayFootsteps()
    {
        if (!footsteps.isValid())
            return;

        footsteps.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
        );

        UpdateGround();
        footsteps.start();
    }

    private void Update()
    {
        if (footsteps.isValid())
        {
            UpdateGround();
        }
    }

    private void UpdateGround()
    {
        Vector3 origin = transform.position + rayOffset;

        Debug.DrawRay(origin, Vector3.down * rayDistance, Color.green);

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            rayDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore))
        {
            float targetSurface = currentSurface;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Lava"))
                targetSurface = 1f;
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Rock"))
                targetSurface = 0f;

            if (targetSurface != currentSurface)
            {
                FMODUnity.RuntimeManager.StudioSystem
                    .setParameterByName("Footsteps_System", targetSurface);

                currentSurface = targetSurface;

                Debug.Log($"Footsteps → {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            }
        }
    }

    /* =========================
       JUMP & LAND
       ========================= */

    public void PlayJump()
    {
        if (jumpEvent.IsNull)
            return;

        FMODUnity.RuntimeManager.PlayOneShot(
            jumpEvent,
            transform.position
        );

        Debug.Log("FMOD Jump");
    }

    public void PlayLand()
    {
        if (landEvent.IsNull)
            return;

        FMODUnity.RuntimeManager.PlayOneShot(
            landEvent,
            transform.position
        );

        Debug.Log("FMOD Land");
    }

    private void OnDestroy()
    {
        if (footsteps.isValid())
        {
            footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footsteps.release();
        }
    }
}







//using UnityEngine;
//using FMODUnity;

//public class PlayerSounds : MonoBehaviour
//{
//    [Header("FMOD Events")]
//    [SerializeField] private EventReference footstepsEvent;
//    [SerializeField] private EventReference jumpEvent;
//    [SerializeField] private EventReference landEvent;

//    private FMOD.Studio.EventInstance footstepsInstance;

//    [Header("Ground Detection")]
//    [SerializeField] private Transform groundRayOrigin; // Player root
//    [SerializeField] private LayerMask lavaLayer;

//    private void Awake()
//    {
//        if (!footstepsEvent.IsNull)
//        {
//            footstepsInstance = RuntimeManager.CreateInstance(footstepsEvent);
//        }
//    }

//    // =========================
//    // FOOTSTEPS (Animation Event)
//    // =========================
//    public void PlayFootsteps()
//    {
//        footstepsInstance = RuntimeManager.CreateInstance(footstepsEvent);

//        UpdateGroundParameter();

//        footstepsInstance.set3DAttributes(
//            RuntimeUtils.To3DAttributes(gameObject)
//        );

//        footstepsInstance.start();
//        footstepsInstance.release();
//    }

//    // =========================
//    // JUMP (Animation Event)
//    // =========================
//    public void PlayJump()
//    {
//        if (jumpEvent.IsNull) return;

//        RuntimeManager.PlayOneShot(
//            jumpEvent,
//            groundRayOrigin.position
//        );
//    }

//    // =========================
//    // LAND (Animation Event)
//    // =========================
//    public void PlayLand()
//    {
//        RuntimeManager.PlayOneShot(landEvent, transform.position);
//    }

//    // =========================
//    // GROUND SWITCH (Layer-based)
//    // =========================
//    private void UpdateGroundParameter()
//    {
//        if (groundRayOrigin == null) return;

//        RaycastHit hit;
//        Ray ray = new Ray(groundRayOrigin.position, Vector3.down);

//        Debug.DrawRay(groundRayOrigin.position, Vector3.down * 1.5f, Color.red);

//        if (Physics.Raycast(ray, out hit, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
//        {
//            bool isLava =
//                ((1 << hit.collider.gameObject.layer) & lavaLayer) != 0;

//            RuntimeManager.StudioSystem
//                .setParameterByName("Footsteps_System", isLava ? 1f : 0f);
//        }
//    }
//}






//using UnityEngine;
//using FMODUnity;

//public class PlayerSounds : MonoBehaviour
//{
//    [SerializeField] private EventReference _footsteps;
//    private FMOD.Studio.EventInstance footsteps;

//    [Header("Ground Detection")]
//    [SerializeField] private Transform groundRayOrigin;   // <- Player root
//    [SerializeField] private LayerMask lavaLayer;         // <- Layer Lava

//    private void Awake()
//    {
//        if (!_footsteps.IsNull)
//        {
//            footsteps = RuntimeManager.CreateInstance(_footsteps);
//        }
//    }

//    public void PlayFootsteps()
//    {
//        if (!footsteps.isValid()) return;

//        footsteps.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
//        GroundSwitch();
//        footsteps.start();
//    }

//    private void Update()
//    {
//        if (footsteps.isValid())
//        {
//            GroundSwitch();
//        }
//    }

//    private void GroundSwitch()
//    {
//        if (groundRayOrigin == null) return;

//        RaycastHit hit;
//        Ray ray = new Ray(groundRayOrigin.position, Vector3.down);

//        Debug.DrawRay(groundRayOrigin.position, Vector3.down * 1.5f, Color.red);

//        if (Physics.Raycast(ray, out hit, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
//        {
//            // 🔥 ACA VA EL CÓDIGO QUE PREGUNTASTE
//            if (((1 << hit.collider.gameObject.layer) & lavaLayer) != 0)
//            {
//                RuntimeManager.StudioSystem.setParameterByName("Footsteps_System", 1f);
//            }
//            else
//            {
//                RuntimeManager.StudioSystem.setParameterByName("Footsteps_System", 0f);
//            }
//        }
//    }
//}



//ESTE ES EL CÓDIGO FINAL CON JUMP Y LAND

//using FMODUnity;
//using System;
//using UnityEngine;

//public class PlayerSounds : MonoBehaviour
//{
//    [Header("FMOD Events")]
//    [SerializeField] private FMODUnity.EventReference _footsteps;
//    private FMOD.Studio.EventInstance footsteps;
//    [SerializeField] private FMODUnity.EventReference _jump;
//    [SerializeField] private FMODUnity.EventReference _land;
//    [SerializeField] private Transform rayOrigin;
//    [SerializeField] private float rayDistance = 2f;
//    [SerializeField] private Vector3 rayOffset = new Vector3(0f, 0.3f, 0f);
//    [SerializeField] private LayerMask groundLayers;



//    private float currentSurface = -1f;

//    private void Awake()
//    {
//        if (!_footsteps.IsNull)
//        {
//            footsteps = FMODUnity.RuntimeManager.CreateInstance(_footsteps);
//        }
//    }

//    public void PlayFootsteps()
//    {
//        if (!footsteps.isValid())
//            return;

//        footsteps.set3DAttributes(
//            FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
//        );

//        UpdateGround();
//        footsteps.start();
//    }

//    private void Update()
//    {
//        if (footsteps.isValid())
//        {
//            UpdateGround();
//        }
//    }

//    private void UpdateGround()
//    {
//        Vector3 origin = transform.position + rayOffset;

//        Debug.DrawRay(origin, Vector3.down * rayDistance, Color.green);

//        if (Physics.Raycast(
//            origin,
//            Vector3.down,
//            out RaycastHit hit,
//            rayDistance,
//            groundLayers,
//            QueryTriggerInteraction.Ignore))
//        {
//            float targetSurface = currentSurface;

//            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Lava"))
//                targetSurface = 1f;
//            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Rock"))
//                targetSurface = 0f;

//            if (targetSurface != currentSurface)
//            {
//                FMODUnity.RuntimeManager.StudioSystem
//                    .setParameterByName("Footsteps_System", targetSurface);

//                currentSurface = targetSurface;
//                Debug.Log($"Footsteps → {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        if (footsteps.isValid())
//        {
//            footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
//            footsteps.release();
//        }
//    }

//    // 🎧 Called from Animation Event (jump start)
//    public void PlayJump()
//    {
//        if (_jump.IsNull)
//            return;

//        FMODUnity.RuntimeManager.PlayOneShotAttached(
//            _jump,
//            gameObject
//        );
//    }

//    private void PlayLand()
//    {
//        if (_land.IsNull)
//            return;

//        FMODUnity.RuntimeManager.PlayOneShotAttached(
//            _land,
//            gameObject
//        );
//    }

// ---------- GROUND CHECK ----------

//private bool IsGrounded()
//{
//    return Physics.Raycast(
//        transform.position + Vector3.up * 0.1f,
//        Vector3.down,
//        0.3f
//    );
//}

// ---------- FOOTSTEP SWITCH ----------

//private void GroundSwitch()
//{
//    RaycastHit hit;
//    Vector3 origin = rayOrigin.position; // Solución CS1503: Transform -> Vector3


//    if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance, groundLayers.value))
//    {
//        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Lava"))
//        {
//            footsteps.setParameterByName("Footsteps_System", 1f);
//        }
//        else
//        {
//            footsteps.setParameterByName("Footsteps_System", 0f);
//        }
//    }

//}
//}




//using UnityEngine;

//public class PlayerSounds : MonoBehaviour
//{
//    [SerializeField] private FMODUnity.EventReference _footsteps;
//    private FMOD.Studio.EventInstance footsteps;

//    [Header("Ground Detection")]
//    [SerializeField] private float rayDistance = 2f;
//    [SerializeField] private Vector3 rayOffset = new Vector3(0f, 0.3f, 0f);
//    [SerializeField] private LayerMask groundLayers;
//    [SerializeField] private Transform rayOrigin;

//    private float currentSurface = -1f;

//    private void Awake()
//    {
//        if (!_footsteps.IsNull)
//        {
//            footsteps = FMODUnity.RuntimeManager.CreateInstance(_footsteps);
//        }
//    }

//    public void PlayFootsteps()
//    {
//        if (!footsteps.isValid())
//            return;

//        footsteps.set3DAttributes(
//            FMODUnity.RuntimeUtils.To3DAttributes(transform.position)
//        );

//        UpdateGround();
//        footsteps.start();
//    }

//    private void Update()
//    {
//        if (footsteps.isValid())
//        {
//            UpdateGround();
//        }
//    }

//    private void UpdateGround()
//    {
//        Vector3 origin = rayOrigin.position + rayOffset;


//        Debug.DrawRay(origin, Vector3.down * rayDistance, Color.green);

//        if (Physics.Raycast(
//            origin,
//            Vector3.down,
//            out RaycastHit hit,
//            rayDistance,
//            groundLayers,
//            QueryTriggerInteraction.Ignore))
//        {
//            float targetSurface = currentSurface;

//            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Lava"))
//                targetSurface = 1f;
//            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Rock"))
//                targetSurface = 0f;

//            if (targetSurface != currentSurface)
//            {
//                FMODUnity.RuntimeManager.StudioSystem
//                    .setParameterByName("Footsteps_System", targetSurface);

//                currentSurface = targetSurface;
//                Debug.Log($"Footsteps → {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        if (footsteps.isValid())
//        {
//            footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
//            footsteps.release();
//        }
//    }
//}

