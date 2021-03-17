using System;

namespace RyBot.Helpers
{
    public static class RandomHelper
    {
        private static Random _rng;

        public static int RandomNumber(int min, int max)
        {
            // seed
            if (_rng == null)
                _rng = new Random(DateTime.Now.Millisecond);

            return _rng.Next(min, max);
        }

        public static void OneInNChance(int chanceOfSuccess, int upperBound, Action onSuccess, Action onFailure)
        {
            // seed
            if (_rng == null)
                _rng = new Random(DateTime.Now.Millisecond);

            var roll = _rng.Next(upperBound);

            // run rng
            if (roll < chanceOfSuccess) {
                onSuccess?.Invoke();
            }
            else {
                onFailure?.Invoke();
            }
        }
    }
}
