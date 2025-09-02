using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Client {
    
    public class TestInGame : MonoBehaviour {
        public static event Action<string> LoginEvent;
        public string userName = "Player1";
        
        // 定义GUI样式和尺寸
        private GUIStyle textFieldStyle;
        private GUIStyle buttonStyle;
        private int fontSize = 24; // 增大字体大小
        private int buttonWidth = 200;
        private int buttonHeight = 50;
        private int textFieldWidth = 300;
        private int textFieldHeight = 40;
        private int margin = 20; // 边距
        
        private void OnGUI() {
            // 初始化样式
            if (textFieldStyle == null) {
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                textFieldStyle.fontSize = fontSize;
                textFieldStyle.alignment = TextAnchor.MiddleLeft;
            }
            
            if (buttonStyle == null) {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = fontSize;
            }
            
            // 计算右上角位置
            float xPos = Screen.width - textFieldWidth - margin;
            float yPos = margin;
            
            // 创建右上角布局区域
            GUILayout.BeginArea(new Rect(
                xPos, 
                yPos,
                textFieldWidth, 
                textFieldHeight + buttonHeight + 20
            ));
            
            // 垂直布局
            GUILayout.BeginVertical();
            
            // 用户名输入框
            userName = GUILayout.TextField(userName, textFieldStyle, 
                GUILayout.Width(textFieldWidth), 
                GUILayout.Height(textFieldHeight));
            
            GUILayout.Space(10); // 添加间距
            
            // 开始按钮 - 右对齐
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // 将按钮推到右侧
            
            if (GUILayout.Button("Start", buttonStyle, 
                GUILayout.Width(buttonWidth), 
                GUILayout.Height(buttonHeight))) {
                LoginEvent?.Invoke(userName);
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}