using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data.DTOs;
using BackendService.Data.DTOs.Group.Request;
using BackendService.Data.DTOs.Group.Response;
using BackendService.Data.DTOs.Transaction.Response;
using BackendService.Data.DTOs.User.Response;
using BackendService.Data.Entities;
using BackendService.Data.Enums;
using BackendService.Data.Repository;
using BackendService.Helpers;
using Microsoft.EntityFrameworkCore.Internal;

namespace BackendService.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IGroupJoinRequestRepository _groupJoinRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly ITransactionRepository _transactionRepository;

        public GroupService(IHashService hashService, IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository, IGroupJoinRequestRepository groupJoinRequestRepository,
            IUserRepository userRepository, ITransactionRepository transactionRepository)
        {
            _hashService = hashService;
            _groupRepository = groupRepository;
            _groupUserRepository = groupUserRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        #region Services

        public async Task<BaseResponse<bool>> AddGroupAsync(AddGroupRequest model)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};

            if (model.UserId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }

            var existGroup = await _groupRepository.GetGroupWithSameNameAsync(model.GroupName, model.UserId);

            if (existGroup != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupNameExist;
                return response;
            }

            var group = await _groupRepository.AddAsync(new Group
            {
                CreatedBy = model.UserId,
                ShareCode = _hashService.EncryptString(model.UserId + _hashService.GenerateCode()),
                GroupName = model.GroupName,
                Description = model.Description,
                MoneyType = model.MoneyShortCut,
                Budget = model.Budget
            });

            if (group != null)
            {
                var groupUser = await _groupUserRepository.AddAsync(new GroupUsers
                {
                    UserId = model.UserId,
                    GroupId = group.Id,
                });
                response.Data = true;
                return response;
            }

            response.HasError = true;
            response.Error = ErrorCodes.AddGroupFailed;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<GetUserGroupsDto>>> GetUserGroupsAsync(int userId)
        {
            var response = new BaseResponse<IEnumerable<GetUserGroupsDto>> {HasError = false, Data = null};
            if (userId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }

            var groups = _groupRepository.GetGroupsByUserId(userId);

            response.Data = groups;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<GetGroupJoinRequestsDto>>> GetGroupJoinRequests(string shareCode)
        {
            var response = new BaseResponse<IEnumerable<GetGroupJoinRequestsDto>> {HasError = false, Data = null};
            if (string.IsNullOrEmpty(shareCode))
            {
                response.HasError = true;
                response.Error = ErrorCodes.ShareCodeRequired;
                return response;
            }

            var groupJoinRequests = _groupJoinRequestRepository.GetRequestsByShareCode(shareCode);

            if (groupJoinRequests == null)
            {
                response.Data = null;
                response.HasError = false;
                return response;
            }

            var users = _userRepository.GetUsersById(groupJoinRequests.Select(x => x.FromUserId)).Select(x =>
                new GetGroupJoinRequestsDto
                {
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    RequestId = groupJoinRequests.FirstOrDefault(y => y.FromUserId == x.Id).Id
                }).ToList();

            response.Data = users;
            return response;
        }

        public async Task<BaseResponse<bool>> SendGroupJoinRequest(int userId, string shareCode)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            if (userId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserIdRequired;
                return response;
            }

            var group = await _groupRepository.GetGroupByShareCode(shareCode);

            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIsNotExist;
                return response;
            }

            var existRequest = await _groupJoinRequestRepository.GetByShareCodeAndUserId(userId, shareCode);

            if (existRequest != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.ExistGroupJoinRequest;
                return response;
            }

            var groupUsers = await _groupUserRepository.GetByGroupId(@group.Id);
            var groupExistUser = groupUsers.FirstOrDefault(x => x.Id == userId);

            if (groupExistUser != null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserAlreadyExist;
                return response;
            }

            var joinRequest = await _groupJoinRequestRepository.AddAsync(new GroupJoinRequest
            {
                FromUserId = userId,
                GroupShareCode = shareCode,
                IsActive = true,
            });
            response.Data = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<GetGroupUsersInfoDto>>> GetGroupUsers(int groupId)
        {
            var response = new BaseResponse<IEnumerable<GetGroupUsersInfoDto>> {HasError = false, Data = null};

            if (groupId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIdRequired;
                return response;
            }

            var groupUsers = await _groupUserRepository.GetByGroupId(groupId);
            var users = _userRepository.GetUsersById(groupUsers.Select(x => x.Id)).Select(x => new GetGroupUsersInfoDto
            {
                UserId = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            response.Data = users;
            return response;
        }

        public async Task<BaseResponse<bool>> ReplyGroupJoinRequestAsync(int requestId, int groupId, int adminId,
            bool isApproved)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};

            var joinRequest = await _groupJoinRequestRepository.GetByIdAsync(requestId);

            if (joinRequest == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            if (!joinRequest.IsActive)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            var group = await _groupRepository.GetGroupByShareCode(joinRequest.GroupShareCode);

            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.JoinRequestFail;
                return response;
            }

            if (group.CreatedBy != adminId)
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotAuthForOperation;
                return response;
            }

            if (isApproved)
            {
                await _groupUserRepository.AddAsync(new GroupUsers
                {
                    UserId = joinRequest.FromUserId,
                    GroupId = groupId,
                });
            }

            joinRequest.IsActive = false;
            await _groupJoinRequestRepository.UpdateAsync(joinRequest);

            response.Data = true;
            return response;
        }

        public async Task<BaseResponse<List<GetGroupDetailDto>>> GetGroupDetailsAsync(string token)
        {
            var response = new BaseResponse<List<GetGroupDetailDto>>
                {HasError = false, Data = new List<GetGroupDetailDto>()};
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            var userGroups = _groupRepository.GetGroupsByUserId(currentUser.Id);

            if (userGroups == null)
            {
                return response;
            }

            foreach (var userGroup in userGroups)
            {
                var groupDetail = new GetGroupDetailDto
                {
                    GroupId = userGroup.GroupId, AdminId = userGroup.AdminId, ShareCode = userGroup.GroupShareCode,
                    Name = userGroup.GroupName, Description = userGroup.Description, Currency = userGroup.Currency, Budget = userGroup.Budget
                };

                var groupUsers = await _groupUserRepository.GetByGroupId(userGroup.GroupId);

                groupDetail.UserInfos = groupUsers.ToList();

                var groupTransactions = await _transactionRepository.GetGroupTransactions(userGroup.GroupId);

                groupDetail.TransactionInfos = SetGroupTransactions(groupTransactions, groupUsers);
                groupDetail.ExpenseGroup = CalculateGroupedExpenses(groupDetail.TransactionInfos);
                response.Data.Add(groupDetail);

                //static group category add
                groupDetail.ExpenseCategories = new List<GroupCategoryDto>();
                groupDetail.ExpenseCategories.AddRange(ConstantCategory.GroupExpenseCategory.Select(x => new GroupCategoryDto{Id = 0, Name = x, Type = (int)GroupCategoryType.Expense}));
                
                //dynamic group category add
                var groupCategories = _groupRepository.GetGroupCategories(userGroup.GroupId, (int) GroupCategoryType.Expense);
                groupDetail.ExpenseCategories.AddRange(groupCategories.Select(x => new GroupCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = (int)GroupCategoryType.Expense
                }));

                groupDetail.ExpenseCategories = groupDetail.ExpenseCategories.OrderBy(x => x.Name).ToList();
            }

            return response;
        }

        public async Task<BaseResponse<GetGroupDetailDto>> GetGroupDetailAsync(string token, string shareCode)
        {
            var response = new BaseResponse<GetGroupDetailDto>
                {HasError = false, Data = new GetGroupDetailDto()};
            var currentUser = await _userRepository.GetUserByToken(token);

            if (currentUser == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }

            var userGroup = await _groupRepository.GetGroupByShareCode(shareCode);

            if (userGroup == null)
            {
                return response;
            }
            
            var groupDetail = new GetGroupDetailDto
            {
                GroupId = userGroup.Id, AdminId = userGroup.CreatedBy, ShareCode = userGroup.ShareCode,
                Name = userGroup.GroupName, Description = userGroup.Description, Currency = userGroup.MoneyType, Budget = userGroup.Budget
            };

            var groupUsers = await _groupUserRepository.GetByGroupId(userGroup.Id);

            groupDetail.UserInfos = groupUsers.ToList();

            var groupTransactions = await _transactionRepository.GetGroupTransactions(userGroup.Id);

            groupDetail.TransactionInfos = SetGroupTransactions(groupTransactions, groupUsers);
            groupDetail.ExpenseGroup = CalculateGroupedExpenses(groupDetail.TransactionInfos);

            //static group category add
            groupDetail.ExpenseCategories = new List<GroupCategoryDto>();
            groupDetail.ExpenseCategories.AddRange(ConstantCategory.GroupExpenseCategory.Select(x => new GroupCategoryDto{Id = 0, Name = x, Type = (int)GroupCategoryType.Expense}));
                
            //dynamic group category add
            var groupCategories = _groupRepository.GetGroupCategories(userGroup.Id, (int) GroupCategoryType.Expense);
            groupDetail.ExpenseCategories.AddRange(groupCategories.Select(x => new GroupCategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = (int)GroupCategoryType.Expense
            }));

            groupDetail.ExpenseCategories = groupDetail.ExpenseCategories.OrderBy(x => x.Name).ToList();
            
            response.Data = groupDetail;

            return response;
        }

        public async Task<BaseResponse<bool>> AddGroupCategory(AddGroupCategoryDto groupCategory, string token)
        {
            var response = new BaseResponse<bool> {HasError = false, Data = false};
            var user = await _userRepository.GetUserByToken(token);
            var group = await _groupRepository.GetGroupByShareCode(groupCategory.GroupShareCode);

            if (user == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.UserNotExist;
                return response;
            }
            
            if (group == null)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIsNotExist;
                return response;
            }

            if (string.IsNullOrEmpty(groupCategory.Name))
            {
                response.HasError = true;
                response.Error = ErrorCodes.EmptyCategory;
                return response;
            }
            
            if (!Enum.IsDefined(typeof(GroupCategoryType), groupCategory.Type))
            {
                response.HasError = true;
                response.Error = ErrorCodes.NotValidCategoryType;
                return response;
            }
            
            var categories = _groupRepository.GetGroupCategories(group.Id, groupCategory.Type);
            var isCategoryExist = categories.Any(x => x.Name == groupCategory.Name) ||
                                  ConstantCategory.GroupExpenseCategory.Any(x => x == groupCategory.Name);
            if (isCategoryExist)
            {
                response.HasError = true;
                response.Error = ErrorCodes.CategoryExist;
                return response;
            }
            
            try
            {
                await _groupRepository.InsertGroupCategory(new GroupCategory
                {
                    Type = groupCategory.Type,
                    Name = groupCategory.Name,
                    GroupId = group.Id
                }, user.Id);
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Error = ErrorCodes.Error;
                response.Error.Message = e.Message;
                return response;
            }

            response.Data = true;
            return response;
        }
        
        #endregion

        #region privateMethods

        private List<TransactionDto> SetGroupTransactions(IEnumerable<GroupTransactionDto> groupTransactions, IEnumerable<UserDto> users)
        {
            if (groupTransactions == null)
            {
                return null;
            }

            var response = new List<TransactionDto>();
            var transactions = groupTransactions.GroupBy(x => x.TransactionId).ToList();

            foreach (var transaction in transactions)
            {
                var transactionValue = transaction.First();
                var transactionDto = new TransactionDto
                {
                    TransactionId = transaction.Key, Amount = transactionValue.Amount,CreateTime = transactionValue.CreateTime,
                    Description = transactionValue.Description, Type = transactionValue.Type,
                    AdderId = transactionValue.AddedBy,
                    AdderName = users?.First(x => x.Id == transactionValue.AddedBy).FirstName,
                    AdderSurname = users?.First(x => x.Id == transactionValue.AddedBy).LastName,
                    Category = transactionValue.Category,
                    RelatedUsers = new List<RelatedUserDto>()
                };
                foreach (var item in transaction)
                {
                    var user = users.FirstOrDefault(x => x.Id == item.RelatedUserId);
                    if (user != null)
                    {
                        var relatedUser = new RelatedUserDto
                        {
                            UserId = item.RelatedUserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        };
                        
                        transactionDto.RelatedUsers.Add(relatedUser);
                    }
                }

                response.Add(transactionDto);
            }

            return response;
        }

        private static GroupExpenseDto CalculateGroupedExpenses(List<TransactionDto> groupDetailTransactions)
        {
            var response = new GroupExpenseDto();
            
            if (groupDetailTransactions ==null)
            {
                return null;
            }
            
            var expenses = groupDetailTransactions.Where(x => x.Type == TransactionType.Expense.ToString()).ToList();

            if (expenses.Count == 0)
            {
                return null;
            }
            
            response.Total = expenses.Sum(x => x.Amount);
            response.GroupedExpenses = new List<ExpenseGroupInfoDto>();
            
            foreach (var expenseGroup in expenses.GroupBy(x => x.Category))
            {
                var categoryTotal = expenseGroup.Sum(y => y.Amount);
                response.GroupedExpenses.Add(new ExpenseGroupInfoDto
                {
                    Category = expenseGroup.Key,
                    CategoryTotal = categoryTotal,
                    Percentage = response.Total == 0 ? 0 : (categoryTotal/response.Total) * 100
                });
            }
            
            return response;
        }

        #endregion
    }
}