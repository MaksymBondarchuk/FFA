namespace FFA
{
    internal static class Program
    {
        private static void Main()
        {
            var fireflyOptimisationAlgorithm = new FireflyOptimisationAlgorithm(50, 2);

            fireflyOptimisationAlgorithm.Algorithm();
        }
    }
}
