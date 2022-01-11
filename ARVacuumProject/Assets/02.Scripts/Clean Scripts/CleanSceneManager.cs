using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARVaccum.Development
{
    [RequireComponent(typeof(ARPlaneManager), typeof(ARRaycastManager))]
    public class CleanSceneManager : MonoBehaviour
    {
        // Prefab Variables
        public GameObject m_Prefab;
        public GameObject m_PrefabPool;

        // UI Component Variables
        private Canvas m_InitFailedMessageCanvas;
        private Button m_InitFailedMessageOKButton;
        private Canvas m_StopCleanMessageCanvas;
        private Button m_StopCleanMessageYesButton;
        private Button m_StopCleanMessageNoButton;
        private Button m_CleanButton;
        private Button m_LogButton;
        private Canvas m_LogCanvas;

        // Reference Scripts Variables
        private MyCalibration m_MyCalibration;
        private MyARRaycast m_MyARRaycast;
        private ARCameraManager m_ARCameraManager;
        private ARPlaneManager m_ARPlaneManager;
        private ARRaycastManager m_ARRaycastManager;

        // etc. Variables
        private Texture2D frame;

        // Init Failed Message OK Button Pressed Event
        private void InitFailedMessageOKButtonPressed()
        {
            SceneManager.LoadScene("01.Scenes/Setting");
        }

        // Stop Clean Message Yes Button Pressed Event
        private void StopCleanMessageYesButtonPressed()
        {
            m_ARCameraManager.frameReceived -= CameraFrameRecieved;
            if (m_ARPlaneManager.isActiveAndEnabled == true && m_ARRaycastManager.isActiveAndEnabled == true)
            {
                m_ARPlaneManager.enabled = false;
                m_ARRaycastManager.enabled = false;
            }
            SceneManager.LoadScene("01.Scenes/Menu");
        }

        // Stop Clean Message No Button Pressed Event
        private void StopCleanMessageNoButtonPressed()
        {
            if(m_StopCleanMessageCanvas.gameObject.activeSelf == true)
                m_StopCleanMessageCanvas.gameObject.SetActive(false);
        }

        // Clean Button Pressed Event
        private void CleanButtonPressed()
        {
            if (m_CleanButton.GetComponentInChildren<Text>().text == "CLEAN START")
            {
                m_ARCameraManager.frameReceived += CameraFrameRecieved;
                if (m_ARPlaneManager.isActiveAndEnabled == false && m_ARRaycastManager.isActiveAndEnabled == false)
                {
                    m_ARPlaneManager.enabled = true;
                    m_ARRaycastManager.enabled = true;
                }
                m_CleanButton.gameObject.GetComponent<Image>().color = new Color(255, 185, 174);
                m_CleanButton.GetComponentInChildren<Text>().text = "CLEAN STOP";   
            }
            else
            {
                if (m_StopCleanMessageCanvas.gameObject.activeSelf == false)
                    m_StopCleanMessageCanvas.gameObject.SetActive(true);
            }
        }

        // Log Button Pressed Event
        private void LogButtonPressed()
        {
            if (m_LogCanvas.gameObject.activeSelf == false)
                m_LogCanvas.gameObject.SetActive(true);
            else
                m_LogCanvas.gameObject.SetActive(false);
        }

        // Get Current Image From CPU using ARCameraManager
        private unsafe void UpdateCameraImage()
        {
            if (!m_ARCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                return;

            // var format = TextureFormat.R8;
            XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.None;

            int w = image.width;
            int h = image.height;
            int startX = 0;
            int startY = (int)(h / 2.0f - (h * 0.61576f) / 2.0f);

            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(startX, startY, w, (int)(h * 0.61576f)),
                outputDimensions = new Vector2Int((int)(w / 2.0f), (int)(h * 0.61576f / 2.0f)),
                outputFormat = TextureFormat.R8,
                transformation = m_Transformation
            };

            int size = image.GetConvertedDataSize(conversionParams);

            using (var buffer = new NativeArray<byte>(size, Allocator.Temp))
            {
                image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
                image.Dispose();

                frame = new Texture2D(
                    conversionParams.outputDimensions.x,
                    conversionParams.outputDimensions.y,
                    conversionParams.outputFormat,
                    false);
                frame.LoadRawTextureData(buffer);
                frame.Apply();
            }
        }

        // Camera Frame Recieved Event
        private void CameraFrameRecieved(ARCameraFrameEventArgs args)
        {
            // Used Camera
            Camera aRCamera = m_ARCameraManager.gameObject.GetComponent<Camera>();

            // Update Current Image to frame Variable
            UpdateCameraImage();

            // Convert Transform Matrix -> Right Coordinate to Left Coordinate -> Local Coordinate to World Coordinate
            MatrixInfo rst = NativeLibAdapter.DetectArucoMarkerCenter(frame.GetRawTextureData(), frame.width, frame.height);
            DestroyImmediate(frame);

            Matrix4x4 m_Matrix;
            m_Matrix.m00 = rst.m00; m_Matrix.m01 = rst.m01; m_Matrix.m02 = rst.m02; m_Matrix.m03 = rst.m03;
            m_Matrix.m10 = rst.m10; m_Matrix.m11 = rst.m11; m_Matrix.m12 = rst.m12; m_Matrix.m13 = rst.m13;
            m_Matrix.m20 = rst.m20; m_Matrix.m21 = rst.m21; m_Matrix.m22 = rst.m22; m_Matrix.m23 = rst.m23;
            m_Matrix.m30 = rst.m30; m_Matrix.m31 = rst.m31; m_Matrix.m32 = rst.m32; m_Matrix.m33 = rst.m33;

            if (m_Matrix == Matrix4x4.zero)
                return; 

            m_Matrix = aRCamera.transform.localToWorldMatrix * m_Matrix;
            Vector3 position = new Vector3(m_Matrix.m03, m_Matrix.m13, m_Matrix.m23);
            Vector3 forward = new Vector3(m_Matrix.m02, m_Matrix.m12, m_Matrix.m22);
            Vector3 upwards = new Vector3(m_Matrix.m01, m_Matrix.m11, m_Matrix.m21);

            // 마커 방향으로 Raycast를 쏴서 ARPlane이 있는지 확인하기
            Pose? pose = m_MyARRaycast.RaycastToSetPoint(aRCamera.WorldToScreenPoint(position), PlaneClassification.Floor);
            if (pose.HasValue == true)
            {
                // ARPlane이 탐색되면 마커의 Position, Rotation 위치에 Prefab 생성하기
                Instantiate(m_Prefab, pose.GetValueOrDefault().position, Quaternion.LookRotation(forward, upwards)).transform.SetParent(m_PrefabPool.transform);

                /*
                if (m_Plane.activeSelf == false)
                    m_Plane.SetActive(true);

                m_Plane.transform.position = pose.GetValueOrDefault().position;
                m_Plane.transform.rotation = Quaternion.LookRotation(forward, upwards);
                */
            }
        }

        private void Start()
        {
            // Init Failed Message Canvas UI Setting
            m_InitFailedMessageCanvas = GameObject.Find("InitFailedMessageCanvas").GetComponent<Canvas>();
            m_InitFailedMessageCanvas.gameObject.SetActive(false);
            Debug.Log("Init Failed Message Canvas Setting(Visible==False) Success...");

            // Init Failed Message OK Button UI Setting
            m_InitFailedMessageOKButton = m_InitFailedMessageCanvas.GetComponentInChildren<Button>();
            m_InitFailedMessageOKButton.onClick.AddListener(InitFailedMessageOKButtonPressed);
            Debug.Log("Init Failed Message OK Button Setting(Visible==False) Success...");

            // Stop Clean Message Canvas UI Setting
            m_StopCleanMessageCanvas = GameObject.Find("StopCleanMessageCanvas").GetComponent<Canvas>();
            m_StopCleanMessageCanvas.gameObject.SetActive(false);
            Debug.Log("Stop Clean Message Canvas Setting(Visible==False) Success...");

            // Stop Clean Message Yes Button UI Setting
            m_StopCleanMessageYesButton = m_StopCleanMessageCanvas.GetComponentsInChildren<Button>()[0];
            m_StopCleanMessageYesButton.onClick.AddListener(StopCleanMessageYesButtonPressed);
            Debug.Log("Stop Clean Message Yes Button Setting(Visible==False) Success...");

            // Stop Clean Message No Button UI Setting
            m_StopCleanMessageNoButton = m_StopCleanMessageCanvas.GetComponentsInChildren<Button>()[1];
            m_StopCleanMessageNoButton.onClick.AddListener(StopCleanMessageNoButtonPressed);
            Debug.Log("Stop Clean Message No Button Setting(Visible==False) Success...");

            // Clean Button UI Setting
            m_CleanButton = GameObject.Find("CleanButton").GetComponent<Button>();
            m_CleanButton.onClick.AddListener(CleanButtonPressed);
            Debug.Log("Clean Button Setting(Visible==True) Success...");

            // Log Button UI Setting
            m_LogButton = GameObject.Find("LogButton").GetComponent<Button>();
            m_LogButton.onClick.AddListener(LogButtonPressed);
            Debug.Log("Log Button Setting(Visible==True) Success...");

            // Log Canvas UI Setting
            m_LogCanvas = GameObject.Find("LogCanvas").GetComponent<Canvas>();
            m_LogCanvas.gameObject.SetActive(false);
            Debug.Log("Log Canvas Setting(Visible==False) Success...");

            // ARCameraManager Script Setting
            m_ARCameraManager = GameObject.Find("AR Camera").GetComponent<ARCameraManager>();
            Debug.Log("ARCameraManager Script Setting(Enable==True) Success...");

            // ARPlaneManager Script Setting
            m_ARPlaneManager = GetComponent<ARPlaneManager>();
            m_ARPlaneManager.enabled = false;
            Debug.Log("ARPlaneManager Script Setting(Enable==False) Success...");

            // ARRaycastManager Script Setting
            m_ARRaycastManager = GetComponent<ARRaycastManager>();
            m_ARRaycastManager.enabled = false;
            Debug.Log("ARRaycastManager Script Setting(Enable==False) Success...");

            // MyCalibration Script Setting && Calibration
            m_MyCalibration = GetComponent<MyCalibration>();
            m_MyCalibration.absolutePath = Application.persistentDataPath;
            NativeLibAdapter.SetAbsolutePath2(Application.persistentDataPath);
            if (m_MyCalibration.IsCalibrationDataExist() == true)
            {
                if (NativeLibAdapter.CameraCalibration(m_MyCalibration.absolutePath) == false)
                {
                    Debug.Log("Calibration Failed...");
                    m_CleanButton.gameObject.SetActive(false);
                    m_InitFailedMessageCanvas.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Calibration Data not exist...");
                m_CleanButton.gameObject.SetActive(false);
                m_InitFailedMessageCanvas.gameObject.SetActive(true);
            }
            Debug.Log("Calibration Success...");

            // MyARRaycast Script Setting
            m_MyARRaycast = GetComponent<MyARRaycast>();
            m_MyARRaycast.aRRaycastManager = m_ARRaycastManager;
            Debug.Log("MyARRaycast Script Setting(Enalbe=True) Success...");
        }
    }
}