using BackendService.Data.DTOs;

namespace BackendService.Data.Enums
{
    public static class ErrorCodes
    {
        public static readonly ErrorMessageDto Error = new ErrorMessageDto {Code = 0, Message = ""};
        public static readonly ErrorMessageDto EmailExist = new ErrorMessageDto {Code = 1, Message = "Email is already exist"};
        public static readonly ErrorMessageDto WrongEmailPassword = new ErrorMessageDto {Code = 2, Message = "Wrong email or password"};
        public static readonly ErrorMessageDto NotVerifiedEmail = new ErrorMessageDto {Code = 3, Message = "Email is not confirmed"};
        public static readonly ErrorMessageDto CommonProcessError = new ErrorMessageDto {Code = 4, Message = "Error occurred during process"};
        public static readonly ErrorMessageDto EmailConfirmFail = new ErrorMessageDto {Code = 5, Message = "Email confirmation failed"};
        public static readonly ErrorMessageDto InvalidToken = new ErrorMessageDto {Code = 6, Message = "Invalid token"};
        public static readonly ErrorMessageDto ResetCodeNotValid = new ErrorMessageDto {Code = 7, Message = "Reset code not valid"};
        public static readonly ErrorMessageDto SendPasswordResetCodeNotSuccess = new ErrorMessageDto {Code = 8, Message = "Reset code request not success"};
        public static readonly ErrorMessageDto ChangePasswordFailed = new ErrorMessageDto {Code = 9, Message = "Change password operation failed"};
        public static readonly ErrorMessageDto UpdateEmailConfirmCodeFail = new ErrorMessageDto {Code = 10, Message = "Update email confirmation code failed"};
        public static readonly ErrorMessageDto UserIdRequired = new ErrorMessageDto {Code = 11, Message = "UserId is required"};
        public static readonly ErrorMessageDto GroupNameExist = new ErrorMessageDto {Code = 12, Message = "Group name already exist"};
        public static readonly ErrorMessageDto AddGroupFailed = new ErrorMessageDto {Code = 13, Message = "Add group failed"};
        public static readonly ErrorMessageDto ShareCodeRequired = new ErrorMessageDto {Code = 14, Message = "Share code is required"};
        public static readonly ErrorMessageDto GroupIsNotExist = new ErrorMessageDto {Code = 15, Message = "Group is not exist"};
        public static readonly ErrorMessageDto ExistGroupJoinRequest = new ErrorMessageDto {Code = 16, Message = "You send request already"};
        public static readonly ErrorMessageDto UserAlreadyExist = new ErrorMessageDto {Code = 17, Message = "User already exist"};
        public static readonly ErrorMessageDto GroupIdRequired = new ErrorMessageDto {Code = 18, Message = "GroupId is required"};
        public static readonly ErrorMessageDto JoinRequestFail = new ErrorMessageDto {Code = 19, Message = "Reply join request failed"};
        public static readonly ErrorMessageDto NotAuthForOperation = new ErrorMessageDto {Code = 20, Message = "You are not authorized for this operation"};
        public static readonly ErrorMessageDto EmptyRelatedUsers = new ErrorMessageDto {Code = 21, Message = "You need to choose least one user for this operation"};
        public static readonly ErrorMessageDto TransactionTypeError = new ErrorMessageDto {Code = 22, Message = "Invalid transaction type"};
        public static readonly ErrorMessageDto UserNotExist = new ErrorMessageDto {Code = 23, Message = "Invalid User"};
        public static readonly ErrorMessageDto NotValidUserForTransfer = new ErrorMessageDto {Code = 24, Message = "Person can not transfer yourself"};
        public static readonly ErrorMessageDto NotValidCategoryType = new ErrorMessageDto {Code = 25, Message = "Category type is not valid"};
        public static readonly ErrorMessageDto NotEditableCategory = new ErrorMessageDto {Code = 26, Message = "You can not edit this category"};
        public static readonly ErrorMessageDto CategoryNotExist = new ErrorMessageDto {Code = 27, Message = "Category not exist"};
        public static readonly ErrorMessageDto EmptyCategory = new ErrorMessageDto {Code = 28, Message = "Category can not be empty"};
        public static readonly ErrorMessageDto AccountNotExist = new ErrorMessageDto {Code = 29, Message = "Account not exist"};
        public static readonly ErrorMessageDto NotEditableAccount = new ErrorMessageDto {Code = 30, Message = "You can not edit this account"};
        public static readonly ErrorMessageDto NotPermission = new ErrorMessageDto {Code = 31, Message = "You have not permission for this operation"};
        public static readonly ErrorMessageDto CategoryExist = new ErrorMessageDto {Code = 32, Message = "Category is already exist"};
        public static readonly ErrorMessageDto NotValidUser = new ErrorMessageDto {Code = 33, Message = "Not valid user"};
        public static readonly ErrorMessageDto ZeroGroup = new ErrorMessageDto {Code = 34, Message = "GroupId can not empty"};
        
    }
}