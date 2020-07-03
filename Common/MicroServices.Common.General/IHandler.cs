namespace MicroServices.Common.General
{
    public interface IHandler<in T> where T: Event
    {
    }
}
