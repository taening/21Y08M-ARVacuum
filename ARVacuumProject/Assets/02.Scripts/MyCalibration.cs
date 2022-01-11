using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARVaccum.Development
{
    public class MyCalibration : MonoBehaviour
    {
        [SerializeField]
        private string m_AbsolutePath;
        public string absolutePath
        {
            get => m_AbsolutePath;
            set => m_AbsolutePath = value;
        }

        [SerializeField]
        private int m_SavedFileCount = 0;
        public int savedFileCount
        {
            get => m_SavedFileCount;
            set => m_SavedFileCount = value;
        }

        public void InitWorkingDirectory()
        {
            string workingDirectoryPath = m_AbsolutePath + "/CheckerBoardImages";
            if(Directory.Exists(workingDirectoryPath))
            {
                Directory.Delete(workingDirectoryPath, true);
                Debug.Log("Delete Previous Working Directory Success...");
            }

            Directory.CreateDirectory(workingDirectoryPath);
            Directory.CreateDirectory(workingDirectoryPath + "/Results");
            Debug.Log("Create New Working Directory Success...");

            NativeLibAdapter.CreateArucoMarker(23, m_AbsolutePath);
            Debug.Log("Create New Marker(ID:23) Success...");
            Debug.Log("New Camera Calibration Data is needed...");
        }

        public bool IsCalibrationDataExist()
        {
            string workingDirectoryPath = m_AbsolutePath + "/CheckerBoardImages";

            DirectoryInfo workingDirInfo = new DirectoryInfo(workingDirectoryPath);
            FileInfo[] workingDirFileInfo = workingDirInfo.GetFiles("*.png");

            DirectoryInfo resultsDirInfo = new DirectoryInfo(workingDirectoryPath + "/Results");
            FileInfo[] resultsDirFileInfo = resultsDirInfo.GetFiles("*.png");

            if (workingDirFileInfo.Length == 20 && resultsDirFileInfo.Length >= 15)
            {
                Debug.Log("Camera Calibration Data Already Exist...");
                return true;
            }
            else
            {
                Debug.Log("Camera Calibration Data is not Exist...");
                return false;
            }
        }

        public IEnumerator ScreenShot(string fileName)
        {
            // UI Active false for ScreenShot && ScreenShot will run next frame
            GameObject uiComponent = GameObject.Find("UI");
            uiComponent.SetActive(false);
            yield return null;

            // Screen Capture and Convert Texture2D to PNG file
            Texture2D m_Texture2D = ScreenCapture.CaptureScreenshotAsTexture();
            byte[] m_PNG = m_Texture2D.EncodeToPNG();

            // SaveImage to CheckerBoardImages Folder
            string filePath = m_AbsolutePath + "/CheckerBoardImages/" + fileName + ".png";
            File.WriteAllBytes(filePath, m_PNG);
            Debug.Log("Captured Image Save Success...");

            // UI Active true for next ScreenShoot
            uiComponent.SetActive(true);
        }
    }
}
