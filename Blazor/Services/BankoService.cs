using Blazor.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Blazor.Services
{
    public class BankoService
    {
        private List<Plade> plates = new List<Plade>();
        private string gameState = "1Row";
        private List<int> drawnNumbers = new List<int>();

        public List<int> GetDrawnNumbers() => drawnNumbers;

        public async Task InitializeGameAsync(string filePath)
        {
            string json = await File.ReadAllTextAsync(filePath);
            var platesData = JsonConvert.DeserializeObject<List<Plade>>(json);

            // Adding predefined plates
            plates.Add(new Plade("Boris", new List<int> { 15, 41, 51, 71, 81 }, new List<int> { 6, 18, 25, 34, 57 }, new List<int> { 8, 19, 47, 59, 68 }));
            plates.Add(new Plade("Rasmus", new List<int> { 10, 31, 60, 70, 81 }, new List<int> { 5, 27, 42, 52, 75 }, new List<int> { 8, 19, 29, 55, 78 }));
            plates.Add(new Plade("Gustav", new List<int> { 1, 24, 31, 40, 70 }, new List<int> { 7, 12, 25, 33, 87 }, new List<int> { 19, 38, 43, 56, 67 }));
            plates.Add(new Plade("Johannes", new List<int> { 1, 14, 22, 70, 81 }, new List<int> { 15, 26, 34, 72, 84 }, new List<int> { 35, 49, 57, 68, 85 }));

            foreach (var plateData in platesData)
            {
                plates.Add(new Plade(plateData.Id, plateData.Row1, plateData.Row2, plateData.Row3));
            }
        }

        public string DrawNumber(int drawnNumber)
        {
            if (drawnNumbers.Contains(drawnNumber) || drawnNumber <= 0 || drawnNumber > 90)
            {
                return $"The number {drawnNumber} is invalid or has already been drawn.";
            }

            drawnNumbers.Add(drawnNumber);

            foreach (var plade in plates)
            {
                plade.RemoveDrawnNumber(drawnNumber);
            }

            if (gameState == "1Row")
            {
                foreach (var plade in plates)
                {
                    if (plade.CheckForOneRow())
                    {
                        gameState = "2Rows";
                        return $"Banko med 1 række på plade: {plade.Id}";
                    }
                }
            }
            else if (gameState == "2Rows")
            {
                foreach (var plade in plates)
                {
                    if (plade.CheckForTwoRows())
                    {
                        gameState = "Full";
                        return $"Banko med 2 rækker på plade: {plade.Id}";
                    }
                }
            }
            else if (gameState == "Full")
            {
                foreach (var plade in plates)
                {
                    if (plade.CheckForFullPlate())
                    {
                        gameState = "Done";
                        return $"Banko! Fuld plade på: {plade.Id}";
                    }
                }
            }

            return null;
        }
    }
}
