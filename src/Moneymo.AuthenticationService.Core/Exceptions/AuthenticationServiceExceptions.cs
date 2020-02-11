using System.Net;
using moneymo.core.apiresponsewrapper.Wrappers;

namespace Moneymo.AuthenticationService.Core.Exceptions
{
    public class AuthenticationServiceExceptions
    {
        public class InvalidTokenException : ApiException
        {
            public InvalidTokenException(string sessionKey) :
                base(errorType: AuthenticationServiceErrorTypes.InvalidToken.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.InvalidToken.ErrorCode,
                    message: $"Invalid token :{sessionKey}",
                    statusCode: (int)HttpStatusCode.NotFound)
            {
            }
        }
        public class InvalidPasswordException : ApiException
        {
            public InvalidPasswordException() :
                base(errorType: AuthenticationServiceErrorTypes.InvalidPassword.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.InvalidPassword.ErrorCode,
                    message: $"Invalid password",
                    statusCode: (int)HttpStatusCode.Unauthorized)
            {
            }
        }
        public class UserNotActiveException : ApiException
        {
            public UserNotActiveException(string username) :
                base(errorType: AuthenticationServiceErrorTypes.UserNotActive.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.UserNotActive.ErrorCode,
                    message: $"User is not active for given username : {username}",
                    statusCode: (int)HttpStatusCode.Unauthorized)
            {
            }
        }
        public class LoginFailedException : ApiException
        {
            public LoginFailedException(string username) :
                base(errorType: AuthenticationServiceErrorTypes.LoginFailed.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.LoginFailed.ErrorCode,
                    message: $"Login failed due to the wrong password for given username: {username}",
                    statusCode: (int)HttpStatusCode.Unauthorized)
            {
            }
        }
        public class WrongParameterValueException : ApiException
        {
            public WrongParameterValueException(string parameterName) :
                base(errorType: AuthenticationServiceErrorTypes.WrongParameterValue.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.WrongParameterValue.ErrorCode,
                    message: $"Parameter with name '{parameterName}' cannot be null or empty",
                    statusCode: (int)HttpStatusCode.BadRequest)
            {
            }
        }
        public class InternalException : ApiException
        {
            public InternalException() :
                base(errorType: AuthenticationServiceErrorTypes.InternalException.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.InternalException.ErrorCode,
                    message: $"Internal server error",
                    statusCode: (int)HttpStatusCode.InternalServerError)
            {
            }
        }
        public class TokenExpiredException : ApiException
        {
            public TokenExpiredException(string token) :
                base(errorType: AuthenticationServiceErrorTypes.TokenExpired.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.TokenExpired.ErrorCode,
                    message: $"Token expired",
                    statusCode: (int)HttpStatusCode.Unauthorized)
            {
            }
        }

        public class RecordNotFoundException : ApiException
        {
            public RecordNotFoundException(string recordName) :
                base(errorType: AuthenticationServiceErrorTypes.RecordNotFound.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.RecordNotFound.ErrorCode,
                    message: $"{recordName} not found",
                    statusCode: (int)HttpStatusCode.NotFound)
            {
            }
        }

        public class UsernameAlreadyExistsException : ApiException
        {
            public UsernameAlreadyExistsException(string username) :
                base(errorType: AuthenticationServiceErrorTypes.UsernameAlreadyExists.ErrorName,
                    errorCode: AuthenticationServiceErrorTypes.UsernameAlreadyExists.ErrorCode,
                    message: $"User with Username='{username}' already exists.",
                    statusCode: (int)HttpStatusCode.BadRequest)
            {
            }
        }
    }
}