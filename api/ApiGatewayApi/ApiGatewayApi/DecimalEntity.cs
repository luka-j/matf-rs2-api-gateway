namespace ApiGatewayApi;

public partial class DecimalEntity
{
    private const decimal NanoFactor = 1_000_000_000;

    public DecimalEntity(long unit, int nanos)
    {
        Unit = unit;
        Nanos = nanos;
    }

    public static implicit operator decimal(DecimalEntity decimalValue) => decimalValue.ToDecimal();

    public static implicit operator DecimalEntity(decimal value) => FromDecimal(value);

    public decimal ToDecimal()
    {
        return Unit + Nanos / NanoFactor;
    }

    public static DecimalEntity FromDecimal(decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32((value - units) * NanoFactor);
        return new DecimalEntity(units, nanos);
    }
}