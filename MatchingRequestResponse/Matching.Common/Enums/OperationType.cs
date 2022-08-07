namespace Matching.Common.Enums
{
    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public static class OperationTypeExtensions
    {
        public static string ToStringSymbol(this OperationType operation)
            => operation switch
            {
                OperationType.Add => "+",
                OperationType.Subtract => "-",
                OperationType.Multiply => "*",
                OperationType.Divide => "/",
                _ => operation.ToString(),
            };
    }
}
