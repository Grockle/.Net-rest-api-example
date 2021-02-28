using BackendService.Application.Models;
using BackendService.Application.Models.Responses.General;

namespace BackendService.Application.Constants
{
    public static class ErrorCodes
    {
        public static readonly BaseResponseErrorItem Error = new BaseResponseErrorItem {Code = 0, Message = ""};
        public static readonly BaseResponseErrorItem EmailExist = new BaseResponseErrorItem {Code = 1, Message = "Email is already exist"};
        public static readonly BaseResponseErrorItem WrongEmailPassword = new BaseResponseErrorItem {Code = 2, Message = "Wrong email or password"};
        public static readonly BaseResponseErrorItem NotVerifiedEmail = new BaseResponseErrorItem {Code = 3, Message = "Email is not confirmed"};
        public static readonly BaseResponseErrorItem CommonProcessError = new BaseResponseErrorItem {Code = 4, Message = "Error occurred during process"};
        public static readonly BaseResponseErrorItem EmailConfirmFail = new BaseResponseErrorItem {Code = 5, Message = "Email confirmation failed"};
        public static readonly BaseResponseErrorItem InvalidToken = new BaseResponseErrorItem {Code = 6, Message = "Invalid token"};
        public static readonly BaseResponseErrorItem ResetCodeNotValid = new BaseResponseErrorItem {Code = 7, Message = "Reset code not valid"};
        public static readonly BaseResponseErrorItem SendPasswordResetCodeNotSuccess = new BaseResponseErrorItem {Code = 8, Message = "Reset code request not success"};
        public static readonly BaseResponseErrorItem ChangePasswordFailed = new BaseResponseErrorItem {Code = 9, Message = "Change password operation failed"};
        public static readonly BaseResponseErrorItem UpdateEmailConfirmCodeFail = new BaseResponseErrorItem {Code = 10, Message = "Update email confirmation code failed"};
        public static readonly BaseResponseErrorItem UserIdRequired = new BaseResponseErrorItem {Code = 11, Message = "UserId is required"};
        public static readonly BaseResponseErrorItem GroupNameExist = new BaseResponseErrorItem {Code = 12, Message = "Group name already exist"};
        public static readonly BaseResponseErrorItem AddGroupFailed = new BaseResponseErrorItem {Code = 13, Message = "Add group failed"};
        public static readonly BaseResponseErrorItem ShareCodeRequired = new BaseResponseErrorItem {Code = 14, Message = "Share code is required"};
        public static readonly BaseResponseErrorItem GroupIsNotExist = new BaseResponseErrorItem {Code = 15, Message = "Group is not exist"};
        public static readonly BaseResponseErrorItem ExistGroupJoinRequest = new BaseResponseErrorItem {Code = 16, Message = "You send request already"};
        public static readonly BaseResponseErrorItem UserAlreadyExist = new BaseResponseErrorItem {Code = 17, Message = "User already exist"};
        public static readonly BaseResponseErrorItem GroupIdRequired = new BaseResponseErrorItem {Code = 18, Message = "GroupId is required"};
        public static readonly BaseResponseErrorItem JoinRequestFail = new BaseResponseErrorItem {Code = 19, Message = "Reply join request failed"};
        public static readonly BaseResponseErrorItem NotAuthForOperation = new BaseResponseErrorItem {Code = 20, Message = "You are not authorized for this operation"};
        public static readonly BaseResponseErrorItem EmptyRelatedUsers = new BaseResponseErrorItem {Code = 21, Message = "You need to choose least one user for this operation"};
        public static readonly BaseResponseErrorItem TransactionTypeError = new BaseResponseErrorItem {Code = 22, Message = "Invalid transaction type"};
        public static readonly BaseResponseErrorItem UserNotExist = new BaseResponseErrorItem {Code = 23, Message = "Invalid User"};
        public static readonly BaseResponseErrorItem NotValidUserForTransfer = new BaseResponseErrorItem {Code = 24, Message = "Person can not transfer yourself"};
        public static readonly BaseResponseErrorItem NotValidCategoryType = new BaseResponseErrorItem {Code = 25, Message = "Category type is not valid"};
        public static readonly BaseResponseErrorItem NotEditableCategory = new BaseResponseErrorItem {Code = 26, Message = "You can not edit this category"};
        public static readonly BaseResponseErrorItem CategoryNotExist = new BaseResponseErrorItem {Code = 27, Message = "Category not exist"};
        public static readonly BaseResponseErrorItem EmptyCategory = new BaseResponseErrorItem {Code = 28, Message = "Category can not be empty"};
        public static readonly BaseResponseErrorItem AccountNotExist = new BaseResponseErrorItem {Code = 29, Message = "Account not exist"};
        public static readonly BaseResponseErrorItem NotEditableAccount = new BaseResponseErrorItem {Code = 30, Message = "You can not edit this account"};
        public static readonly BaseResponseErrorItem NotPermission = new BaseResponseErrorItem {Code = 31, Message = "You have not permission for this operation"};
        public static readonly BaseResponseErrorItem CategoryExist = new BaseResponseErrorItem {Code = 32, Message = "Category is already exist"};
        public static readonly BaseResponseErrorItem NotValidUser = new BaseResponseErrorItem {Code = 33, Message = "Not valid user"};
        public static readonly BaseResponseErrorItem ZeroGroup = new BaseResponseErrorItem {Code = 34, Message = "GroupId can not empty"};
        
    }
}