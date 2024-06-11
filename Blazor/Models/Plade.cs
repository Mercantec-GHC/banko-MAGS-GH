namespace Blazor.Models
{
    public class Plade
    {
        public string Id { get; set; }
        public List<int> Row1 { get; set; }
        public List<int> Row2 { get; set; }
        public List<int> Row3 { get; set; }

        public Plade(string id, List<int> row1, List<int> row2, List<int> row3)
        {
            Id = id;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public void RemoveDrawnNumber(int number)
        {
            Row1.Remove(number);
            Row2.Remove(number);
            Row3.Remove(number);
        }

        public bool CheckForOneRow()
        {
            return Row1.Count == 0 || Row2.Count == 0 || Row3.Count == 0;
        }

        public bool CheckForTwoRows()
        {
            return (Row1.Count == 0 && Row2.Count == 0) ||
                   (Row1.Count == 0 && Row3.Count == 0) ||
                   (Row2.Count == 0 && Row3.Count == 0);
        }

        public bool CheckForFullPlate()
        {
            return Row1.Count == 0 && Row2.Count == 0 && Row3.Count == 0;
        }
    }
}

