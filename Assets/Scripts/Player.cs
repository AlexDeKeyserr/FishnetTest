using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
public class Player : NetworkBehaviour
{
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float speed;
    [SerializeField] private float mouseSens;
    [SerializeField] private GameObject hitParticle;

    private float xRot;
    private float yRot;

    private Score score;
    private CharacterController controller;
    private Transform cam;

    #region //ONSTART - UPDATE//
    public override void OnStartClient()
    {
        base.OnStartClient();

        controller = GetComponent<CharacterController>();
        score = FindObjectOfType<Score>();

        if (!IsOwner) return;

        Debug.Log(score);
        score.CmdAddNewPlayer();

        cam = Camera.main.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (!IsOwner) return;

        Movement();
        CameraMov();
        Fire();
    }
    #endregion

    #region //PLAYER MOVEMENT//
    private void Movement()
    {
        int up = Input.GetKey(KeyCode.Z) ? 1 : 0;
        int down = Input.GetKey(KeyCode.S) ? -1 : 0;
        int left = Input.GetKey(KeyCode.Q) ? -1 : 0;
        int right = Input.GetKey(KeyCode.D) ? 1 : 0;

        Vector3 mov = transform.right * (left + right) + transform.forward * (up + down);
        controller.Move(speed * Time.deltaTime * mov.normalized);
    }
    private void CameraMov()
    {
        yRot = Input.GetAxis("Mouse X") * mouseSens;
        xRot += Input.GetAxis("Mouse Y") * mouseSens;
        xRot = Mathf.Clamp(xRot, -90, 90);

        transform.eulerAngles = new Vector3(0 , transform.eulerAngles.y + yRot, 0);
        cam.eulerAngles = new Vector3(-xRot, transform.eulerAngles.y, 0);
        cam.position = cameraPos.position;
    }
    #endregion

    #region //FIRE BULLET//
    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit))
            {
                CmdSpawnHitParticles(hit.point, hit.normal);

                if (hit.collider.transform.CompareTag("Target"))
                {
                    Target target = hit.collider.GetComponent<Target>();
                    target.TargetHit();
                    target.CmdTargetHit();
                }
                if (hit.collider.transform.CompareTag("Player"))
                {
                    Player player = hit.collider.GetComponent<Player>();
                    player.CmdPlayerHit();
                }
            }
        }
    }
    [ServerRpc]
    private void CmdSpawnHitParticles(Vector3 spawnPos, Vector3 spawnDir)
    {
        GameObject hitInstance = Instantiate(hitParticle, spawnPos, Quaternion.identity);
        hitInstance.transform.forward = spawnDir;
        Spawn(hitInstance);
    }
    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void CmdPlayerHit(NetworkConnection sender = null) => score.ChangePoint(sender, -1);
}