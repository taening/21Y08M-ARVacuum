using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct MatrixInfo
{
    public float m00; public float m01; public float m02; public float m03;
    public float m10; public float m11; public float m12; public float m13;
    public float m20; public float m21; public float m22; public float m23;
    public float m30; public float m31; public float m32; public float m33;
}

public class NativeLibAdapter : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool cameraCalibration(string absPath);
    public static bool CameraCalibration(string absPath)
    {
        return cameraCalibration(absPath);
    }

    [DllImport("__Internal")]
    private static extern void createArucoMarker(int markerID, string absPath);
    public static void CreateArucoMarker(int markerID, string absPath)
    {
        createArucoMarker(markerID, absPath);
    }

    [DllImport("__Internal")]
    private static extern MatrixInfo detectArucoMarkerCenter(byte[] bytes, int width, int height);
    public static MatrixInfo DetectArucoMarkerCenter(byte[] bytes, int width, int height)
    {
        return detectArucoMarkerCenter(bytes, width, height);
    }

    [DllImport("__Internal")]
    private static extern MatrixInfo setAbsolutePath2(string absPath);
    public static MatrixInfo SetAbsolutePath2(string absPath)
    {
        return setAbsolutePath2(absPath);
    }
#endif
}
