using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ARVaccum.Development
{
    public class MenuSceneManager : MonoBehaviour
    {
        // UI Component Variables
        private Button m_CleanButton;
        private Button m_SettingButton;
        private Canvas m_MessageCanvas;
        private Button m_MessageYesButton;
        private Button m_MessageNoButton;
        private Button m_LogButton;
        private Canvas m_LogCanvas;

        // Clean Button Pressed Event
        private void CleanButtonPressed()
        {
            SceneManager.LoadScene("01.Scenes/Clean");
        }

        // Setting Button Pressed Event
        private void SettingButtonPressed()
        {
            if (m_MessageCanvas.gameObject.activeSelf == false)
                m_MessageCanvas.gameObject.SetActive(true);
        }

        // Message Yes Button Pressed Event
        private void MessageYesButtonPressed()
        {
            SceneManager.LoadScene("01.Scenes/Setting");
        }

        // Message No Button Pressed Event
        private void MessageNoButtonPressed()
        {
            if (m_MessageCanvas.gameObject.activeSelf == true)
                m_MessageCanvas.gameObject.SetActive(false);
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
            // Screen Orientation Setting. Fixed Screen Portrait.
            Screen.orientation = ScreenOrientation.Portrait;

            // Clean Button UI Setting
            m_CleanButton = GameObject.Find("CleanButton").GetComponent<Button>();
            m_CleanButton.onClick.AddListener(CleanButtonPressed);
            Debug.Log("Clean Button Setting(Visible==True) Success...");

            // Setting Button UI Setting
            m_SettingButton = GameObject.Find("SettingButton").GetComponent<Button>();
            m_SettingButton.onClick.AddListener(SettingButtonPressed);
            Debug.Log("Setting Button Setting(Visible==True) Success...");

            // Message Canvas UI Setting
            m_MessageCanvas = GameObject.Find("MessageCanvas").GetComponent<Canvas>();
            m_MessageCanvas.gameObject.SetActive(false);
            Debug.Log("Message Canvas Setting(Visible==False) Success...");

            // Message Yes Button UI Setting
            m_MessageYesButton = m_MessageCanvas.GetComponentsInChildren<Button>()[0];
            m_MessageYesButton.onClick.AddListener(MessageYesButtonPressed);
            Debug.Log("Message Yes Button Setting(Visible==False) Success...");

            // Message No Button UI Setting
            m_MessageNoButton = m_MessageCanvas.GetComponentsInChildren<Button>()[1];
            m_MessageNoButton.onClick.AddListener(MessageNoButtonPressed);
            Debug.Log("Message No Button Setting(Visible==False) Success...");

            // Log Button UI Setting
            m_LogButton = GameObject.Find("LogButton").GetComponent<Button>();
            m_LogButton.onClick.AddListener(LogButtonPressed);
            Debug.Log("Log Button Setting(Visible==True) Success...");

            // Log Canvas UI Setting
            m_LogCanvas = GameObject.Find("LogCanvas").GetComponent<Canvas>();
            m_LogCanvas.gameObject.SetActive(false);
            Debug.Log("Log Canvas Setting(Visible==False) Success...");
        }
    }
}