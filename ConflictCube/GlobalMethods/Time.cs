namespace ConflictCube.Time
{
    public class Time
    {
        private static float _CurrentTime = 0;
        public static float CurrentTime {
            get {
                return _CurrentTime;
            }

            set {
                DifTime = value - _CurrentTime;
                _CurrentTime = value;
            }
        }

        public static float DifTime { get; set; }


        public static bool CooldownIsOver(float LastTime, float Cooldown)
        {
            return CurrentTime - LastTime > Cooldown;
        }
    }
}
