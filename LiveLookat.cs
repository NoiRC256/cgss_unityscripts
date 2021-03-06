﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
☆☆☆ CGSS人物模型视线追踪脚本 By NoiR_CCC ☆☆☆
用 CamLookAt 物体 + LookAtHandler 脚本把相机世界变换转换成相对于 Head 的本地变换，再计算 Eye 与 CamLookAt 之间的 Vector3，Quaternion 差，输出相应的x，w补偿值

*默认数值适合10倍大小的 Kobayakawa Sae 0132，0132_1011, 0132_1311 HQ 模型
*需要将左/右 Eye 设为左/右 Eye Locator 的子骨骼，分别调整 Eye Locator 旋转值使其与面部平行
*/

public class LiveLookat : MonoBehaviour
{
    [Header("俺寻思这代码能跑.exe")]
    [Header("相机替身 'LookAtHandler' ")]
    public GameObject LookAtHandler; 
    //↓官模自带双眼定位骨骼↓可通过脚本同步左右眼骨骼↓（过时，现在直接使Eye骨骼沿EyeLocator左右移动）
    [Header("左眼定位（过时）")]
    public GameObject eyeLocatorL;
    [Header("左眼")]
    public GameObject eyeLeft;
    [Header("右眼定位（过时）")]
    public GameObject eyeLocatorR;
    [Header("右眼")]
    public GameObject eyeRight;

    [Header("平滑速度")]
    public float speed = 8.75f; // Lerp speed;

    // 数值
    [Header("变量未实装，只能在行内修改")]
    public float eRelativeModDivisor = 175; // 追踪灵敏度（越小越灵敏）
    public float QuaternionDifferenceWeight = 1.5; // 角度补偿权重（越大越灵敏）
    public float ReyeMaxOut = 0.00625f; // 右眼外侧限制
    public float ReyeMaxIn = -0.00785f; //右眼内侧限制
    public float LeyeMaxOut = -0.00625f; // 左眼外侧限制
    public float LeyeMaxInd = 0.00785f; //左眼内侧限制

    //计算相机位置
    public void calcCamPos()
    {
        //==========变量==========//
        Vector3 camPos = LookAtHandler.transform.localPosition; // Camera LookAt Handler Position | 相机 世界位置→本地位置
	
	// Eye Locator calculations are unnecessary
	// 过时功能，已移除
        /*
        Vector3 eLOrigin = eyeLocatorL.transform.position; // Left Eye World Position | 左眼定位 世界位置
        Vector3 eROrigin = eyeLocatorR.transform.position; // Right Eye World Position |右眼定位 世界位置

        Vector3 eLLocalOrigin = eyeLocatorL.transform.localPosition; // Left Eye Local Position | 左眼定位 本地位置
        Vector3 eRLocalOrigin = eyeLocatorR.transform.localPosition; // Right Eye Local Position | 右眼定位 本地位置
        */

        Vector3 eyeLOrigin = eyeLeft.transform.position; // Left Eye World Position | 左眼 世界位置
        Vector3 eyeROrigin = eyeRight.transform.position; // Right Eye World Position |右眼 世界位置

        Vector3 eyeLLocalOrigin = eyeLeft.transform.localPosition; // Left Eye Local Position | 左眼 本地位置
        Vector3 eyeRLocalOrigin = eyeRight.transform.localPosition; // Right Eye Local Position | 右眼 本地位置

        Quaternion camRotation = LookAtHandler.transform.rotation; // Camera LookAt Handler Rotation | 相机旋转


        //==========功能==========//
   
        // Calculate Rotation difference between Right Eye and LookAt Handler | 计算右眼与相机之间的角度差
        Quaternion ReyeRotation = eyeLocatorR.transform.rotation; // 旧版
        Quaternion RighteyeRotation = eyeRight.transform.rotation;
        Quaternion RighteyeRotationDiff = new Quaternion(RighteyeRotation.x, RighteyeRotation.y, RighteyeRotation.z, RighteyeRotation.w - camRotation.w); // 虽然写了一长串但真正会用到的只是第四个w值_(:з」∠)_
        
        Vector3 eRRelative = new Vector3( camPos.x - eyeRLocalOrigin.x - 1.5f * (1.015f + RighteyeRotationDiff.w), eyeRLocalOrigin.x - camPos.x, eyeRLocalOrigin.x - camPos.x); // Local Position difference between Right Eye and Camera LookAt Handler | 计算右眼与相机之间的x轴差
        Vector3 eRTarget = eRRelative / 175; //+ new Vector3(0.04776929f, 0.07901000f, 0.08240000f); // Calculate new target position | 右眼原位置 + x轴差距
        eRTarget.x = Mathf.Clamp(eRTarget.x, -0.00785f, 0.00625f); // Clamp new target position | 限制视线偏移范围 [内,外]
        Vector3 desR = new Vector3(eRTarget.x, eyeRight.transform.localPosition.y, eyeRight.transform.localPosition.z);

        eyeRight.transform.localPosition = desR; // Set new target position | 应用新位置
        eyeRight.transform.localPosition = Vector3.Lerp(eyeRight.transform.localPosition, desR, Time.deltaTime * speed);

        // Calculate Rotation difference between Left Eye and LookAt Handler | 计算左眼与相机之间的角度差
        Quaternion LeyeRotation = eyeLocatorL.transform.rotation; // 旧版
        Quaternion LefteyeRotation = eyeLeft.transform.rotation;
        Quaternion LefteyeRotationDiff = new Quaternion(LefteyeRotation.x, LefteyeRotation.y, LefteyeRotation.z, LefteyeRotation.w - camRotation.w);

        Vector3 eLRelative = new Vector3( camPos.x - eyeLLocalOrigin.x - 1.5f * (0.985f + LefteyeRotationDiff.w), eyeLLocalOrigin.x - camPos.x, eyeLLocalOrigin.x - camPos.x); // Local Position difference between Left eye and Camera LookAt Handler | 计算左眼与相机之间的x轴差
        Vector3 eLTarget = eLRelative / 175; //+ new Vector3(-0.04776929f, 0.07901000f, 0.082400000f); // Calculate new target position | 左眼原位置 + x轴差距
        eLTarget.x = Mathf.Clamp(eLTarget.x, -0.00625f, 0.00785f); // Clamp new target position | 限制视线偏移范围 [外,内]
        Vector3 desL = new Vector3(eLTarget.x, eyeLeft.transform.localPosition.y, eyeLeft.transform.localPosition.z);
        
        eyeLeft.transform.localPosition = desL; // Set new target position | 应用新位置
        eyeLeft.transform.localPosition = Vector3.Lerp(eyeLeft.transform.localPosition, desL, Time.deltaTime * speed);

        //==========测试Log==========//

        //Debug.Log("Right Eye X Modification: " + (eRTarget.x));
        Debug.Log("Left Eye X Modification: " + (eLTarget.x));
        //Debug.Log("Right eye Rotation Diff: "+ReyeRotationDiff);
        //Debug.Log("Left eye Rotation Diff: "+LeyeRotationDiff);

        //以下无效

     /*
         if (eLRelative.x > 0)
         {
            eLTarget.x = Mathf.Clamp(eLTarget.x, -0.0530f, -0.0470f);
             eRTarget.x = Mathf.Clamp(eRTarget.x, 0.0420f, 0.0480f);
         }
             if(eLRelative.x < 0)
         {
            eLTarget.x = Mathf.Clamp(eLTarget.x, -0.0480f, -0.0420f);
            eRTarget.x = Mathf.Clamp(eRTarget.x, 0.0470f, 0.0530f);
         }
     */
    }

    //==========应用==========//
    void Start ()
    {
        //calcCamPos();
    }	
	void Update ()
    {
        calcCamPos();
    }

    private void FixedUpdate()
    {
        calcCamPos();
    }

    /*
      void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

   
    string myLog;
    Queue myLogQueue = new Queue();
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(myLog);
    }
    */
}
