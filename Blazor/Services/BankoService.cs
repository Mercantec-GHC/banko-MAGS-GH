using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Models;
using Newtonsoft.Json;

namespace Blazor.Services
{
    public class BankoService
    {
        private List<Plade> plates = new List<Plade>();
        private string gameState = "1Row";
        private List<int> drawnNumbers = new List<int>();

        public List<int> GetDrawnNumbers() => drawnNumbers;

        public int GetTotalPlateCount() => plates.Count;

        public async Task InitializeGameAsync(string filePath)
        {
            // Clear existing plates before initialization
            plates.Clear();

            string json = await File.ReadAllTextAsync(filePath);
            var platesData = JsonConvert.DeserializeObject<List<Plade>>(json);

            // Adding predefined plates
            plates.Add(
                new Plade(
                    "Boris",
                    new List<int> { 15, 41, 51, 71, 81 },
                    new List<int> { 6, 18, 25, 34, 57 },
                    new List<int> { 8, 19, 47, 59, 68 }
                )
            );
            plates.Add(
                new Plade(
                    "Rasmus",
                    new List<int> { 10, 31, 60, 70, 81 },
                    new List<int> { 5, 27, 42, 52, 75 },
                    new List<int> { 8, 19, 29, 55, 78 }
                )
            );
            plates.Add(
                new Plade(
                    "Gustav",
                    new List<int> { 1, 24, 31, 40, 70 },
                    new List<int> { 7, 12, 25, 33, 87 },
                    new List<int> { 19, 38, 43, 56, 67 }
                )
            );
            plates.Add(
                new Plade(
                    "Johannes",
                    new List<int> { 1, 14, 22, 70, 81 },
                    new List<int> { 15, 26, 34, 72, 84 },
                    new List<int> { 35, 49, 57, 68, 85 }
                )
            );

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

        public List<(string Id, int RemainingNumbers, string Type)> GetClosestToWinning(
            int count = 3
        )
        {
            var result = new List<(string Id, int RemainingNumbers, string Type)>();

            if (gameState == "1Row")
            {
                // Find plates closest to getting one row
                var oneRowPlates = plates
                    .Select(p =>
                    {
                        int minRowCount = Math.Min(
                            Math.Min(p.Row1.Count, p.Row2.Count),
                            p.Row3.Count
                        );
                        return (p.Id, minRowCount, "1 Række");
                    })
                    .OrderBy(x => x.minRowCount)
                    .Take(count)
                    .ToList();

                result.AddRange(oneRowPlates);
            }
            else if (gameState == "2Rows")
            {
                // Find plates closest to getting two rows
                var twoRowsPlates = plates
                    .Where(p => p.CheckForOneRow() && !p.CheckForTwoRows())
                    .Select(p =>
                    {
                        int remainingNumbers = 0;
                        if (p.Row1.Count == 0)
                            remainingNumbers = Math.Min(p.Row2.Count, p.Row3.Count);
                        else if (p.Row2.Count == 0)
                            remainingNumbers = Math.Min(p.Row1.Count, p.Row3.Count);
                        else // p.Row3.Count == 0
                            remainingNumbers = Math.Min(p.Row1.Count, p.Row2.Count);

                        return (p.Id, remainingNumbers, "2 Rækker");
                    })
                    .OrderBy(x => x.remainingNumbers)
                    .Take(count)
                    .ToList();

                result.AddRange(twoRowsPlates);
            }
            else if (gameState == "Full")
            {
                // Find plates closest to getting full plate
                var fullPlates = plates
                    .Where(p => p.CheckForTwoRows() && !p.CheckForFullPlate())
                    .Select(p =>
                    {
                        int remainingNumbers = 0;
                        if (p.Row1.Count > 0)
                            remainingNumbers = p.Row1.Count;
                        else if (p.Row2.Count > 0)
                            remainingNumbers = p.Row2.Count;
                        else // p.Row3.Count > 0
                            remainingNumbers = p.Row3.Count;

                        return (p.Id, remainingNumbers, "Fuld Plade");
                    })
                    .OrderBy(x => x.remainingNumbers)
                    .Take(count)
                    .ToList();

                result.AddRange(fullPlates);
            }

            return result;
        }

        public string GetCurrentGameState()
        {
            return gameState switch
            {
                "1Row" => "1 Række",
                "2Rows" => "2 Rækker",
                "Full" => "Fuld Plade",
                "Done" => "Spil afsluttet",
                _ => gameState
            };
        }

        public void RestartGame()
        {
            // Clear drawn numbers and reset game state
            drawnNumbers.Clear();
            gameState = "1Row";

            // Reset all plates to their original state
            foreach (var plate in plates)
            {
                plate.ResetPlate();
            }
        }
    }
}
