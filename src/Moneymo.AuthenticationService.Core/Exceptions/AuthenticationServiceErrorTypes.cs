namespace Moneymo.AuthenticationService.Core.Exceptions
{
    public class AuthenticationServiceErrorTypes
    {
        public static AuthenticationServiceErrorTypes RecordNotFound { get; } = new AuthenticationServiceErrorTypes("4000", "AUTH_NOT_FOUND");
        public static AuthenticationServiceErrorTypes InvalidToken { get; } = new AuthenticationServiceErrorTypes("4001", "AUTH_INVALID_TOKEN");
        public static AuthenticationServiceErrorTypes InvalidPassword { get; } = new AuthenticationServiceErrorTypes("1001", "AUTH_PWD_NOT_VALID");
        public static AuthenticationServiceErrorTypes UserNotActive { get; } = new AuthenticationServiceErrorTypes("1002", "AUTH_USER_NOT_ACTIVE");
        public static AuthenticationServiceErrorTypes LoginFailed { get; } = new AuthenticationServiceErrorTypes("1003", "AUTH_LOGIN_FAILED");
        public static AuthenticationServiceErrorTypes WrongParameterValue { get; } = new AuthenticationServiceErrorTypes("1005", "AUTH_WRONG_PARAMETER_VALUE");
        public static AuthenticationServiceErrorTypes InternalException { get; } = new AuthenticationServiceErrorTypes("1007", "AUTH_INTERNAL_ERROR");
        public static AuthenticationServiceErrorTypes TokenExpired { get; } = new AuthenticationServiceErrorTypes("1008", "AUTH_TOKEN_EXPIRED");
        public static AuthenticationServiceErrorTypes UsernameAlreadyExists { get; } = new AuthenticationServiceErrorTypes("1009", "USERNAME_ALREADY_EXISTS");

        public string ErrorCode { get; private set; }
        public string ErrorName { get; private set; }
        private AuthenticationServiceErrorTypes(string errorCode, string errorName)
        {
            ErrorCode = errorCode;
            ErrorName = errorName;
        }
        private AuthenticationServiceErrorTypes() { }
    }
}