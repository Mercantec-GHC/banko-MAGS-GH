namespace Blazor.Models
{
    public class Plade
    {
        public string Id { get; set; }
        public List<int> Row1 { get; set; }
        public List<int> Row2 { get; set; }
        public List<int> Row3 { get; set; }

        // Store the original row values for reset functionality
        private List<int> _originalRow1;
        private List<int> _originalRow2;
        private List<int> _originalRow3;

        public Plade(string id, List<int> row1, List<int> row2, List<int> row3)
        {
            Id = id;
            Row1 = new List<int>(row1);
            Row2 = new List<int>(row2);
            Row3 = new List<int>(row3);

            // Store original values
            _originalRow1 = new List<int>(row1);
            _originalRow2 = new List<int>(row2);
            _originalRow3 = new List<int>(row3);
        }

        public void RemoveDrawnNumber(int number)
        {
            Row1.Remove(number);
            Row2.Remove(number);
            Row3.Remove(number);
        }

        public void ResetPlate()
        {
            // Restore original values
            Row1 = new List<int>(_originalRow1);
            Row2 = new List<int>(_originalRow2);
            Row3 = new List<int>(_originalRow3);
        }

        public bool CheckForOneRow()
        {
            return Row1.Count == 0 || Row2.Count == 0 || Row3.Count == 0;
        }

        public bool CheckForTwoRows()
        {
            return (Row1.Count == 0 && Row2.Count == 0)
                || (Row1.Count == 0 && Row3.Count == 0)
                || (Row2.Count == 0 && Row3.Count == 0);
        }

        public bool CheckForFullPlate()
        {
            return Row1.Count == 0 && Row2.Count == 0 && Row3.Count == 0;
        }
    }
}
