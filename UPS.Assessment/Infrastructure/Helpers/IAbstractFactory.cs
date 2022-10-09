namespace UPS.Assessment.Infrastructure.Helpers;

public interface IAbstractFactory<T>
{
    T Create();
}