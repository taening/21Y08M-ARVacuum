using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ARVaccum.Development
{
    public class SettingManager : MonoBehaviour
    {
        // UI Component Variables
        private Button m_CaptureButton;
        private Button m_CalibrationButton;
        private Canvas m_MessageCanvas;
        private Button m_MessageOKButton;
        private Button m_LogButton;
        private Canvas m_LogCanvas;

        // Reference Scripts Variables
        private MyCalibration m_MyCalibration;

        // Capture Button Pressed Event
        private void CaptureButtonPressed()
        {
            if (m_MyCalibration.savedFileCount < 20)
            {
                StartCoroutine(m_MyCalibration.ScreenShot(m_MyCalibration.savedFileCount.ToString()));
                m_MyCalibration.savedFileCount += 1;

                if (m_MyCalibration.savedFileCount == 20)
                {
                    m_CaptureButton.gameObject.SetActive(false);
                    m_CalibrationButton.gameObject.SetActive(true);
                }
            }
        }

        // Calibration Button Pressed Event
        private void CalibrationButtonPressed()
        {
            if (NativeLibAdapter.CameraCalibration(m_MyCalibration.absolutePath) == true)
            {
                Debug.Log("Calibration Success...");
                m_MessageCanvas.GetComponentInChildren<Text>().text = "설정이 완료됐습니다.";
            }
            else
            {
                Debug.Log("Calibration Failed...");
                m_MessageCanvas.GetComponentInChildren<Text>().text = "설정에 실패했습니다.\n다시 시도해주세요.";
                m_MyCalibration.InitWorkingDirectory();
            }
            m_CalibrationButton.gameObject.SetActive(false);
            m_MessageCanvas.gameObject.SetActive(true);
            Debug.Log("Message Canvas Set Visible True...");
        }

        // Message OK Button Pressed Event
        private void MessageOKButtonPressed()
        {
            if (m_MessageCanvas.gameObject.activeSelf == true)
                m_MessageCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene("01.Scenes/Menu");
        }

        // Log Button Pressed Event
        private void LogButtonPressed()
        {
            if (m_LogCanvas.gameObject.activeSelf == false)
                m_LogCanvas.gameObject.SetActive(true);
            else
                m_LogCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            // Capture Button UI Setting
            m_CaptureButton = GameObject.Find("CaptureButton").GetComponent<Button>();
            m_CaptureButton.onClick.AddListener(CaptureButtonPressed);
            Debug.Log("Capture Button Setting(Visible==True) Success...");

            // Calibration Button UI Setting
            m_CalibrationButton = GameObject.Find("CalibrationButton").GetComponent<Button>();
            m_CalibrationButton.onClick.AddListener(CalibrationButtonPressed);
            m_CalibrationButton.gameObject.SetActive(false);
            Debug.Log("Calibration Button Setting(Visible==False) Success...");

            // Message Canvas UI Setting
            m_MessageCanvas = GameObject.Find("MessageCanvas").GetComponent<Canvas>();
            m_MessageCanvas.gameObject.SetActive(false);
            Debug.Log("Message Canvas Setting(Visible==False) Success...");

            // Message OK Button UI Setting
            m_MessageOKButton = m_MessageCanvas.GetComponentInChildren<Button>();
            m_MessageOKButton.onClick.AddListener(MessageOKButtonPressed);
            Debug.Log("Message OK Button Setting(Visible==False) Success...");

            // Log Button UI Setting
            m_LogButton = GameObject.Find("LogButton").GetComponent<Button>();
            m_LogButton.onClick.AddListener(LogButtonPressed);
            Debug.Log("Log Button Setting(Visible==True) Success...");

            // Log Canvas UI Setting
            m_LogCanvas = GameObject.Find("LogCanvas").GetComponent<Canvas>();
            m_LogCanvas.gameObject.SetActive(false);
            Debug.Log("Log Canvas Setting(Visible==False) Success...");

            // MyCalibration Script Setting
            m_MyCalibration = GetComponent<MyCalibration>();
            m_MyCalibration.absolutePath = Application.persistentDataPath;
            m_MyCalibration.InitWorkingDirectory();
            Debug.Log("Working Directory Setting Success...");
        }
    }
}
