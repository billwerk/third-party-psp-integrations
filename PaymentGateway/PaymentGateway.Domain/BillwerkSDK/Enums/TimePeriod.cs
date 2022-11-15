namespace Billwerk.Payment.SDK.Enums
{
    //N.C. from IT-8259: This is same enum as Pactas.SDK.DTO.PeriodUnit
    //we can not use Pactas.SDK.DTO since it will produce circular dependencies with Billwerk.Payment.SDK.
    //after discussion Christian was decided duplicate it for now, to restrict set of possible values instead of using smth like string.
    public enum TimePeriod
    {
        Day = 1,
        Week = 2,
        Month = 3,
        Year = 4,
        Hour = 5,
        Minute = 6,
        Second = 7
    }
}
