using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10, tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio, scoreAudio, dieAudio;

    Rigidbody2D rigidbody;
    Quaternion downRotation, forwardRotation;

    GameManager game;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        // converts a 3d vec to 4d aka Quaternion
        downRotation = Quaternion.Euler(0,0,-90);
        forwardRotation = Quaternion.Euler(0,0,35);
        game = GameManager.Instance;
        rigidbody.simulated = false;
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted() {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed() {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update() {
        if(game.GameOver) return;

        // equiv. to Left Click or a Tap on Android
        if(Input.GetMouseButtonDown(0)) {
            tapAudio.Play();
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            // can also later try ForceMode2D.Impulse
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        // gravity, downfall
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    // are we hitting a scorezone or a deadzone
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "ScoreZone") {
            // register a score, play a sound
            OnPlayerScored(); // event sent to GAmeManager
            scoreAudio.Play();
        }

        if(col.gameObject.tag == "DeadZone") {
            Debug.Log("DeadZone entered");
            // freeze bird when he hits something
            rigidbody.simulated = false;
            // register a dead event, and play a sound
            OnPlayerDied(); // same as above;
            dieAudio.Play();
        }
    }
}
