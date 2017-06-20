using DarkTonic.MasterAudio;
using UnityEngine;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public class MA_PlayerControl : MonoBehaviour {
    public GameObject ProjectilePrefab;
    // ReSharper disable InconsistentNaming
    public bool canShoot = true;
    // ReSharper restore InconsistentNaming

    private const float MoveSpeed = 10f;
    private Transform _trans;
    private float _lastMoveAmt;

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
    void Awake() {
        useGUILayout = false;
        _trans = transform;
    }

    // ReSharper disable once UnusedMember.Local
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name.StartsWith("Enemy(")) {
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
				this.gameObject.SetActiveRecursively(false); 
#else
            gameObject.SetActive(false);
#endif
        }
    }

    // ReSharper disable UnusedMember.Local
    void OnDisable() {
    }

    void OnBecameInvisible() {
    }

    void OnBecameVisible() {
    }
    // ReSharper restore UnusedMember.Local

    // Update is called once per frame 
    // ReSharper disable once UnusedMember.Local
    void Update() {
        var moveAmt = Input.GetAxis("Horizontal") * MoveSpeed * AudioUtil.FrameTime;

        if (moveAmt != 0f) {
            if (_lastMoveAmt == 0f) {
                MasterAudio.FireCustomEvent("PlayerMoved", _trans.position);
            }
        } else {
            if (_lastMoveAmt != 0f) {
                MasterAudio.FireCustomEvent("PlayerStoppedMoving", _trans.position);
            }
        }

        _lastMoveAmt = moveAmt;

        var pos = _trans.position;
        pos.x += moveAmt;
        _trans.position = pos;

        if (!canShoot || !Input.GetMouseButtonDown(0)) {
            return;
        }

        var spawnPos = _trans.position;
        spawnPos.y += 1;

        Instantiate(ProjectilePrefab, spawnPos, ProjectilePrefab.transform.rotation);
    }
}
