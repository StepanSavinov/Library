namespace Epam.Library.BLL;

public static class ErrorMessages
{
    public const string ErrorMessageAuthorFirstnameEmpty = "Firstname should not be empty";
    public const string ErrorMessageAuthorFirstnameIncorrect = "Incorrect firstname format";
    public const string ErrorMessageAuthorFirstnameTooLong = "Firstname length must not exceed 50 characters";
    public const string ErrorMessageAuthorAlreadyExist = "This author is already exist";
    public const string ErrorMessageUnexpected = "Unexpected error";

    public const string ErrorMessageAuthorLastnameEmpty = "Lastname should not be empty";
    public const string ErrorMessageAuthorLastnameIncorrect = "Incorrect lastname format";
    public const string ErrorMessageAuthorLastnameTooLong = "Lastname length must not exceed 200 characters";

    public const string ErrorMessagePolygraphyNameEmpty = "Polygraphy name should not be empty";
    public const string ErrorMessagePolygraphyNameTooLong = "Polygraphy name must not exceed 300 characters";
    public const string ErrorMessagePolygraphyDateEmpty = "Creation date should not be empty";
    public const string ErrorMessagePolygraphyDateIncorrect = "Incorrect date format";
    public const string ErrorMessagePolygraphyDateTooEarly = "Creation date should not be earlier than 1400";
    public const string ErrorMessagePolygraphyDateFuture = "Creation date should not be later than current year";
    public const string ErrorMessagePolygraphyPagesNegative = "Number of total pages cannot be negative";
    public const string ErrorMessagePolygraphyNotExist = "There is no polygraphy objects with this id";
    public const string ErrorMessagePolygraphyFootnoteTooLong = "Footnote length must not exceed 2000 characters";
    public const string ErrorMessagePolygraphyAlreadyExist = "This polygraphy object is alredy exist";
        
    public const string ErrorMessagePolygraphyAuthorsEmpty = "List of authors should not be empty";
    public const string ErrorMessagePolygraphyAuthorsNotExist = "You have entered a non-existent author";
    public const string ErrorMessagePolygraphyCityEmpty = "City name should not be empty";
    public const string ErrorMessagePolygraphyCityTooLong = "City length must not exceed 200 characters";
    public const string ErrorMessagePolygraphyCityIncorrect = "Incorrect city name format";
    public const string ErrorMessagePolygraphyPublisherTooLong = "Publisher length must not exceed 300 characters";
    public const string ErrorMessagePolygraphyPublisherEmpty = "Publisher name should not be empty";
    public const string ErrorMessagePolygraphyIsbnIncorrect = "Incorrect ISBN format";

    public const string ErrorMessagePolygraphyNumberNegative = "Number should not be 0 or negative";
    public const string ErrorMessagePolygraphyPublicationDateDontMatch = "Publication year must be same as creation year";
    public const string ErrorMessagePolygraphyPublicationDateTooEarly = "Publication date should not be earlier than 1474";
    public const string ErrorMessagePolygraphyPublicationDateFuture = "Publication date should not be later than current year";
    public const string ErrorMessagePolygraphyIssnIncorrect = "Incorrect ISSN format";

    public const string ErrorMessagePolygraphyCountryEmpty = "Country name should not be empty";
    public const string ErrorMessagePolygraphyCountryTooLong = "Country length must not exceed 200 characters";
    public const string ErrorMessagePolygraphyCountryIncorrect = "Incorrect country name format";
    public const string ErrorMessagePolygraphyNumberTooLong = "Number cannot contain more than 9 digits";
    public const string ErrorMessagePatentNewspaperDateTooEarly = "Creation date should not be earlier than 1474";
    public const string ErrorMessagePolygraphyPublicationLessThanCreation = "Publication date cannot be earlier than creation date";

    public const string ErrorMessageNewspaperNotExist = "You have entered a non-existent newspaper";
    public const string ErrorMessageNewspaperAlreadyExist = "This newspaper already exists";

    public const string ErrorMessageUserIncorrectUsername = "Incorrect username";
    public const string ErrorMessageUserEmptyUsername = "Username should not be empty";
       
    public const string ErrorMessageUserPasswordContainsUsername = "Password should not contain username";
    public const string ErrorMessageUserAlreadyExist = "This user already exists";
    public const string ErrorMessageUserNotExist = "There is no user with this username";

    public const string ErrorMessageUserPasswordShort = "Password must contain at least 3 characters";
    public const string ErrorMessageUserEmptyPassword = "Password should not be empty";
}