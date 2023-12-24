using UnityEngine;

public class GameVariables : MonoBehaviour
    {
        private static bool _inHittingPhase = false;

        public static bool getHittingPhase()
        {
            return _inHittingPhase;
        }

        public static void setHittingPhase(bool inHittingPhase)
        {
            _inHittingPhase = inHittingPhase;
        }
    }