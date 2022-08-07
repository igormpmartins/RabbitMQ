using Matching.Common.Enums;

namespace Matching.Common.Models
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public OperationType Operation { get; set; }

        public CalculationRequest() { }

        public CalculationRequest(int number1, int number2, OperationType operation) : base()
        {
            Number1 = number1;
            Number2 = number2;
            Operation = operation;
        }

        public override string ToString() => $"{Number1} {Operation.ToStringSymbol()} {Number2}";
    }
}
