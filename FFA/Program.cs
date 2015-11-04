namespace FFA
{
    internal static class Program
    {
        private static void Main()
        {
            var fireflyOptimisationAlgorithm = new FireflyOptimisationAlgorithm(30, 10);

            fireflyOptimisationAlgorithm.Algorithm();
        }
    }
}
