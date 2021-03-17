using System;

namespace RyBot.Helpers
{
    public static class RandomHelper
    {
        private static Random rng = null;
        private static readonly object syncLock = new object();

        public static int RandomNumber(int min, int max)
        {
            // seed
            if (rng == null)
                rng = new Random(DateTime.Now.Millisecond);
            return rng.Next(min, max);
        }

        public static void OneInNChance(int chanceOfSuccess, int upperBound, Action onSuccess, Action onFailure)
        {
            // seed
            if (rng == null)
                rng = new Random(DateTime.Now.Millisecond);

            var roll = rng.Next(upperBound);

            //Console.WriteLine($"Rolled a {roll}:{upperBound}.");

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
