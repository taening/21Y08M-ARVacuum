using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARVacuum.Development
{
    public class Logger : MonoBehaviour
    {
        // SerializeField : private변수지만, Inspector에서 접근가능하게 해주는 기능
        // get set : private변수인 m_LogText에 접근하기 위한 기능
        [SerializeField]
        Text m_LogText;

        public Text logText
        {
            get { return m_LogText; }
            set { m_LogText = value; }
        }

        // SerializeField : private변수지만, Inspector에서 접근가능하게 해주는 기능
        // get set : private변수인 m_VisibleMessageCount에 접근하기 위한 기능
        [SerializeField]
        int m_VisibleMessageCount = 40;

        public int visibleMessageCount
        {
            get { return m_VisibleMessageCount; }
            set { m_VisibleMessageCount = value; }
        }

        int m_LastMessageCount;
        static List<string> s_Log = new List<string>();
        static StringBuilder m_StringBuilder = new StringBuilder();

        public void Log(string message)
        {
            lock (s_Log)
            {
                if (s_Log == null)
                    s_Log = new List<string>();
                s_Log.Add(message);
            }
        }

        public void LogClear()
        {
            lock (s_Log)
            {
                s_Log.Clear();
            }
        }

        void Awake()
        {
            if (m_LogText == null)
            {
                m_LogText = GetComponent<Text>();
            }

            lock (s_Log)
            {
                // ?. : C# Null 조건부 연산;
                s_Log?.Clear();
            }
        }

        // Update is called once per frame
        void Update()
        {
            lock (s_Log)
            {
                if (m_LastMessageCount != s_Log.Count)
                {
                    m_StringBuilder.Clear();
                    var startIndex = Mathf.Max(s_Log.Count - m_VisibleMessageCount, 0);
                    for (int i = startIndex; i < s_Log.Count; ++i)
                    {
                        m_StringBuilder.Append($"{i:000}> {s_Log[i]}\n");
                    }

                    var text = m_StringBuilder.ToString();

                    if (m_LogText)
                    {
                        m_LogText.text = text;
                    }
                    else
                    {
                        Debug.Log(text);
                    }
                }
                m_LastMessageCount = s_Log.Count;
            }
        }
    }
}