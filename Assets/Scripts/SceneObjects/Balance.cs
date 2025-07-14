using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public Transform leftPlate;  // 左托盘 Transform
    public Transform rightPlate; // 右托盘 Transform


    private float baseMoveSpeed = 0.5f;     // 基础移动速度
    private float maxMoveDistance = 2f;   // 最大位移距离

    private HashSet<Weighted> leftObjects = new HashSet<Weighted>();
    private HashSet<Weighted> rightObjects = new HashSet<Weighted>();

    private Vector3 leftInitialPos;     // 初始位置
    private Vector3 rightInitialPos;

    void Start()
    {
        leftInitialPos = leftPlate.localPosition;
        rightInitialPos = rightPlate.localPosition;
    }

    void Update()
    {
        float leftMass = GetTotalMass(leftPlate, leftObjects);
        float rightMass = GetTotalMass(rightPlate, rightObjects);

        float difference = rightMass - leftMass;
        // Debug.Log(difference);


        float moveDirection = difference; // 方向：+1 或 -1
        // Debug.Log(moveDirection);
        float speedMultiplier = difference;

        float currentMoveSpeed = baseMoveSpeed;

        // 计算新位置
        Vector3 newLeftPos = leftPlate.localPosition;
        Vector3 newRightPos = rightPlate.localPosition;

        // 向下/向上移动
        newLeftPos.y += moveDirection * currentMoveSpeed * Time.deltaTime;
        newRightPos.y -= moveDirection * currentMoveSpeed * Time.deltaTime;

        // 限制最大偏移
        newLeftPos.y = Mathf.Clamp(newLeftPos.y, leftInitialPos.y - maxMoveDistance, leftInitialPos.y + maxMoveDistance);
        newRightPos.y = Mathf.Clamp(newRightPos.y, rightInitialPos.y - maxMoveDistance, rightInitialPos.y + maxMoveDistance);

        // 更新位置
        leftPlate.localPosition = newLeftPos;
        rightPlate.localPosition = newRightPos;
    }

    // 获取托盘上的总质量
    private float GetTotalMass(Transform plate, HashSet<Weighted> objectsInPlate)
    {
        float totalMass = 0f;

        foreach (var weighted in objectsInPlate)
        {
            if (weighted != null)
            {
                totalMass += weighted.weight;
            }
        }

        return totalMass;
    }

    // 托盘触发器交互方法
    public void AddToLeft(Weighted rb) => leftObjects.Add(rb);
    public void RemoveFromLeft(Weighted rb) => leftObjects.Remove(rb);
    public void AddToRight(Weighted rb) => rightObjects.Add(rb);
    public void RemoveFromRight(Weighted rb) => rightObjects.Remove(rb);
}
