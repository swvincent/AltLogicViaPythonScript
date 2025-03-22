namespace AltLogicViaPythonScript.Models
{
    internal class LineItem
    {
        public int LineNo { get; set; }
        public required string Description { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityToRun { get; set; }

        public override string ToString() => $"{Description} Ordered: {QuantityOrdered} To Run: {QuantityToRun}";
    }
}
