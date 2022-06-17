using Epam.Library.Entities;

namespace Epam.Library.BLL.Interfaces;

public interface IValidatable<in T>
{
    bool IsValid(T obj, out List<Error> errors);
}