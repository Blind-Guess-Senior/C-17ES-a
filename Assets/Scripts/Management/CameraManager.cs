using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private GameManager gameManager;

    [Header("Main Camera")] 
    [SerializeField] private Camera mainCamera;

    [SerializeField] private List<Vector3> rooms = new List<Vector3>(); // RoomID to camera pos
    [SerializeField] private int cameraMoveSpeed = 20;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.RegisterHandler("SwitchRoom", CameraMove);
    }

    public object CameraMove(params object[] args)
    {
        if (args.Length >= 1 && args[0] is int newRoomID)
        {
            StartCoroutine(CameraMoveProcess(newRoomID));
        }
        return null;
    }

    private IEnumerator CameraMoveProcess(int newRoomID)
    {
        Vector3 cameraTargetPosition = rooms[newRoomID];
        while (Vector3.Distance(mainCamera.transform.position, cameraTargetPosition) > 0.01f)
        {
            // 使用 MoveTowards 以固定速度移动
            float step = cameraMoveSpeed * Time.deltaTime;

            mainCamera.transform.position =
                Vector3.MoveTowards(mainCamera.transform.position, cameraTargetPosition, step);

            yield return null; // 等待下一帧
        }
        mainCamera.transform.position = cameraTargetPosition;
        gameManager.EventOccur("SwitchRoomFinish", newRoomID);
    }
}