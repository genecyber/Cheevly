namespace Cheevly.Skills {
    public class TimeSkills {

        private DateTime _time;

        public TimeSkills(DateTime time) {
            _time = time;
        }

        public TimeSkills()
            : this(DateTime.Now) {
        }

        public DateTime GetTime() {
            return _time;
        }

        public void SetTime(DateTime time) {
            _time = time;
        }

        public void AddTime(TimeSpan time) {
            _time = _time.Add(time);
        }
    }
}
