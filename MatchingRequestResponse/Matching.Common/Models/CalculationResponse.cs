namespace Matching.Common.Models
{
    public class CalculationResponse
    {
        public int Result { get; set; }
        public override string ToString() => Result.ToString();
    }
}
