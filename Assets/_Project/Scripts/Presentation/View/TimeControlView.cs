using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Presentation.Views
{
    public class TimeControlView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _dayText;
        [SerializeField] private TextMeshProUGUI _hourText;
        [SerializeField] private TextMeshProUGUI _phaseText;

        [SerializeField] private Button _pass1HourButton;
        [SerializeField] private Button _pass3HourButton;
        [SerializeField] private Button _pass6HourButton;

        [SerializeField] private Button _leaveWorkButton;
        [SerializeField] private Button _nextDayButton;

        public Observable<Unit> OnPass1HourClicked => _pass1HourButton.OnClickAsObservable();
        public Observable<Unit> OnPass3HourClicked => _pass3HourButton.OnClickAsObservable();
        public Observable<Unit> OnPass6HourClicked => _pass6HourButton.OnClickAsObservable();
        public Observable<Unit> OnLeaveWorkClicked => _leaveWorkButton.OnClickAsObservable();
        public Observable<Unit> OnNextDayClicked => _nextDayButton.OnClickAsObservable();

        public void SetDay(int day)
        {
            // 추후 맵핑이 되면 수정 필요.. 글면 Config SO는 Presenter에서 받고 조립 한 뒤 text만 넘기기
            _dayText.text = $"Day {day}";
        }

        public void SetDay(string text)
        {
            _dayText.text = text;
        }

        public void SetHour(int hour)
        {
            _hourText.text = $"Hour {hour}";
        }

        public void SetHour(string text)
        {
            _hourText.text = text;
        }

        public void SetPhase(int phase)
        {
            _phaseText.text = $"Phase {phase}";
        }

        public void SetPhase(string text)
        {
            _phaseText.text = text;
        }

        public void SetLeaveWorkEnabled(bool isEnabled)
        {
            _leaveWorkButton.interactable = isEnabled;
        }

        public void SetNextDayEnabled(bool isEnabled)
        {
            _nextDayButton.interactable = isEnabled;
        }
    }
}