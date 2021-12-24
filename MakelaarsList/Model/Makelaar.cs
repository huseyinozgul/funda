namespace MakelaarsList.Model
{
    public class Makelaar
    {
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }
        public int NumberOfItems { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", this.MakelaarId, this.MakelaarNaam, this.NumberOfItems);
        }
    }
}
