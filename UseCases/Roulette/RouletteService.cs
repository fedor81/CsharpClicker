using System;

namespace CSharpClicker.Web.UseCases.Roulette
{
    public class RouletteService
    {
        private readonly static Dictionary<int, int> RouletteDict = new Dictionary<int, int>()
        {
            // Сколько элементов находится в сете
            // Key - элемент, value - количество
            { 0, 50 },
            { 1, 15 },
            { 2, 10 },
            { 3, 7 },
            { 4, 5 },
            { 5, 3 },
            { 6, 2 },
            { 7, 1 }
        };

        private readonly int[] RouletteSet;
        public readonly int MaxItem;
        public readonly int MinItem;
        public int RouletteSetLength => RouletteSet.Length;
        private readonly Random random = new();

        public RouletteService()
        {
            var set = new List<int>();

            foreach (var item in RouletteDict)
            {
                for (int i = 0; i < item.Value; i++)
                    set.Add(item.Key);
            }

            RouletteSet = set.ToArray();
            MinItem = RouletteSet.Min();
            MaxItem = RouletteSet.Max();
        }

        public IEnumerable<int> GetRouletteSet()
        {
            return RouletteSet;
        }

        /// <summary>
        /// Случайным образом выбирает какой элемент победит
        /// </summary>
        /// <returns>Победный элемент сета</returns>
        public async Task<int> GetWinnerItem()
        {
            random.Shuffle(RouletteSet);
            return RouletteSet[random.Next(0, RouletteSetLength)];
        }
    }
}
