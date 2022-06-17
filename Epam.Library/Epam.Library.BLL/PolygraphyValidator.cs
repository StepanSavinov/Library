using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public abstract class PolygraphyValidator : IValidatable<Polygraphy>
{
    public bool IsValid(Polygraphy poly, out List<Error> errors)
    {
        errors = new List<Error>();

        if (poly is null)
            throw new ArgumentNullException();

        ValidateName(poly.Name, ref errors);
        ValidateTotalPages(poly.TotalPages, ref errors);
        ValidateFootnote(poly.Footnote, ref errors);

        return !errors.Any();
    }

    private void ValidateName(string name, ref List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        else if (name.Length > 300)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyNameTooLong));
    }

    private void ValidateTotalPages(int pages, ref List<Error> errors)
    {
        if (pages < 0)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyPagesNegative));
    }

    private void ValidateFootnote(string footnote, ref List<Error> errors)
    {
        if (footnote.Length > 2000)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyFootnoteTooLong));
    }
}