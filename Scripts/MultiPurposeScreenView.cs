using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShapeShooter
{
    public class MultiPurposeScreenView : MonoBehaviour
    {
        private const string INTRO_HEADER = "Shape Shooter";
        private const string INTRO_TEXT = "Press SPACE To Play<br>Press W/A/S/D or Arrow Keys to Move<br>Press SPACE to fire";
        private const string LEVEL_UP_HEADER = "Level Up!";
        private const string LEVEL_UP_TEXT = "Press Space to go to Next Level";
        private const string GAME_OVER_HEADER = "Game Over!";
        private const string GAME_OVER_TEXT = "Press Restart button to play Again!";
        private const string GAME_PAUSED_HEADER = "GAME PAUSED";
        private const string GAME_PAUSED_TEXT = "Press Esc to Continue";
        private const string GAME_CONGRATS = "CONGRATULATIONS";
        private const string GAME_CONGRATS_INFO = "Press Restart button to play Again!";
        private const string MISSILE_FIRED = "Missile Fired : {0}";
        
        [SerializeField] private TextMeshProUGUI screenHeaderTextField;
        [SerializeField] private TextMeshProUGUI screenInfoTextField;
        [SerializeField] private Button restartButton;
        [SerializeField] private TextMeshProUGUI missileFiredTextField;

        private Action buttonClickedCallback;
        
        private void Start()
        {
            SetTextByScreenType(GameManager.GameManagerInstance.ScreenType);
        }

        public void InitScreen(int missileFired, Action buttonClickedCallback)
        {
            this.buttonClickedCallback = buttonClickedCallback;
            if (GameManager.GameManagerInstance.ScreenType == ScreenType.Congrats)
            {
                missileFiredTextField.gameObject.SetActive(true);
                missileFiredTextField.SetText(string.Format(MISSILE_FIRED, missileFired));
            }
        }

        //Set Text based on screen type
        private void SetTextByScreenType(ScreenType type)
        {
            switch (type)
            {
               case ScreenType.Intro:
                   SetText(INTRO_HEADER, INTRO_TEXT);
                   break;
               case ScreenType.LevelUp:
                   SetText(LEVEL_UP_HEADER, LEVEL_UP_TEXT);
                   break;
               case ScreenType.GameOver:
                   SetText(GAME_OVER_HEADER, GAME_OVER_TEXT);
                   SetButton();
                   break;
               case ScreenType.GamePaused:
                   SetText(GAME_PAUSED_HEADER, GAME_PAUSED_TEXT);
                   break;
               case ScreenType.Congrats:
                   SetText(GAME_CONGRATS, GAME_CONGRATS_INFO);
                   SetButton();
                   break;
            }
        }

        private void SetText(string header, string info)
        {
            screenHeaderTextField.SetText(header);
            screenInfoTextField.SetText(info);
        }

        private void SetButton()
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            restartButton.onClick.RemoveListener(OnButtonClicked);
            buttonClickedCallback?.Invoke();
        }
        
    }
}