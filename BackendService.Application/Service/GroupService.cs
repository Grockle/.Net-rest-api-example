using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Application.Constants;
using BackendService.Application.Interface;
using BackendService.Application.Interface.Helper;
using BackendService.Application.Models.Requests.Group;
using BackendService.Application.Models.Responses.General;
using BackendService.Application.Models.Responses.Group;
using BackendService.Application.Models.Responses.Transaction;
using BackendService.Application.Models.Responses.User;
using BackendService.Domain.Entity;
using BackendService.Domain.IRepository;
using BackendService.Domain.Model;
using Mapster;

namespace BackendService.Application.Service
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

        public async Task<BaseResponse<IEnumerable<GetGroupJoinResponse>>> JoinRequestsAsync(string shareCode)
        {
            var response = new BaseResponse<IEnumerable<GetGroupJoinResponse>> { HasError = false, Data = null };
            
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
                new GetGroupJoinResponse
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

        public async Task<BaseResponse<IEnumerable<GetGroupUsersInfoResponse>>> UsersAsync(int groupId)
        {
            var response = new BaseResponse<IEnumerable<GetGroupUsersInfoResponse>> { HasError = false, Data = null };

            if (groupId == 0)
            {
                response.HasError = true;
                response.Error = ErrorCodes.GroupIdRequired;
                return response;
            }

            var groupUsers = await _groupUserRepository.GetByGroupId(groupId);
            var users = _userRepository.GetUsersById(groupUsers.Select(x => x.Id)).Select(x => new GetGroupUsersInfoResponse
            {
                UserId = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            response.Data = users;
            return response;
        }

        public async Task<BaseResponse<List<GetGroupDetailResponse>>> DetailsAsync(string token)
        {
            var response = new BaseResponse<List<GetGroupDetailResponse>> { HasError = false, Data = new List<GetGroupDetailResponse>() };
            
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
                var groupDetail = new GetGroupDetailResponse
                {
                    GroupId = userGroup.GroupId,
                    AdminId = userGroup.AdminId,
                    ShareCode = userGroup.GroupShareCode,
                    Name = userGroup.GroupName,
                    Description = userGroup.Description,
                    Currency = userGroup.Currency,
                    Budget = userGroup.Budget
                };

                var groupUsers = await _groupUserRepository.GetByGroupId(userGroup.GroupId);

                groupDetail.UserInfos = groupUsers.Adapt<List<GetGroupDetailResponseUserItem>>();

                var groupTransactions = await _transactionRepository.GetGroupTransactions(userGroup.GroupId);

                groupDetail.TransactionInfos = SetGroupTransactions(groupTransactions, groupUsers);
                groupDetail.ExpenseGroup = CalculateGroupedExpenses(groupDetail.TransactionInfos);
                response.Data.Add(groupDetail);

                //static group category add
                groupDetail.ExpenseCategories = new List<GetGroupDetailResponseGroupItem>();
                groupDetail.ExpenseCategories.AddRange(ConstantCategory.GroupExpenseCategory.Select(x => new GetGroupDetailResponseGroupItem { Id = 0, Name = x, Type = (int)GroupCategoryType.Expense }));

                //dynamic group category add
                var groupCategories = _groupRepository.GetGroupCategories(userGroup.GroupId, (int)GroupCategoryType.Expense);
                groupDetail.ExpenseCategories.AddRange(groupCategories.Select(x => new GetGroupDetailResponseGroupItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = (int)GroupCategoryType.Expense
                }));

                groupDetail.ExpenseCategories = groupDetail.ExpenseCategories.OrderBy(x => x.Name).ToList();
            }

            return response;
        }

        public async Task<BaseResponse<GetGroupDetailResponse>> DetailAsync(string token, string shareCode)
        {
            var response = new BaseResponse<GetGroupDetailResponse>
            { HasError = false, Data = new GetGroupDetailResponse() };
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

            var groupDetail = new GetGroupDetailResponse
            {
                GroupId = userGroup.Id,
                AdminId = userGroup.CreatedBy,
                ShareCode = userGroup.ShareCode,
                Name = userGroup.GroupName,
                Description = userGroup.Description,
                Currency = userGroup.MoneyType,
                Budget = userGroup.Budget
            };

            var groupUsers = await _groupUserRepository.GetByGroupId(userGroup.Id);

            groupDetail.UserInfos = groupUsers.Adapt<List<GetGroupDetailResponseUserItem>>();

            var groupTransactions = await _transactionRepository.GetGroupTransactions(userGroup.Id);

            groupDetail.TransactionInfos = SetGroupTransactions(groupTransactions, groupUsers);
            groupDetail.ExpenseGroup = CalculateGroupedExpenses(groupDetail.TransactionInfos);

            //static group category add
            groupDetail.ExpenseCategories = new List<GetGroupDetailResponseGroupItem>();
            groupDetail.ExpenseCategories.AddRange(ConstantCategory.GroupExpenseCategory.Select(x => new GetGroupDetailResponseGroupItem { Id = 0, Name = x, Type = (int)GroupCategoryType.Expense }));

            //dynamic group category add
            var groupCategories = _groupRepository.GetGroupCategories(userGroup.Id, (int)GroupCategoryType.Expense);
            groupDetail.ExpenseCategories.AddRange(groupCategories.Select(x => new GetGroupDetailResponseGroupItem
            {
                Id = x.Id,
                Name = x.Name,
                Type = (int)GroupCategoryType.Expense
            }));

            groupDetail.ExpenseCategories = groupDetail.ExpenseCategories.OrderBy(x => x.Name).ToList();

            response.Data = groupDetail;

            return response;
        }

        public async Task<BaseResponse<bool>> AddAsync(AddGroupRequest model)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };

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
        
        public async Task<BaseResponse<bool>> AddCategoryAsync(AddGroupCategoryRequest groupCategory, string token)
        {
            var response = new BaseResponse<bool> { HasError = false, Data = false };
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

        private static List<GetGroupDetailResponseTransactionItem> SetGroupTransactions(IEnumerable<GroupTransactionDto> groupTransactions, IEnumerable<UserDto> users)
        {
            if (groupTransactions == null)
            {
                return null;
            }

            var response = new List<GetGroupDetailResponseTransactionItem>();
            var transactions = groupTransactions.GroupBy(x => x.TransactionId).ToList();

            foreach (var transaction in transactions)
            {
                var transactionValue = transaction.First();
                var transactionDto = new GetGroupDetailResponseTransactionItem
                {
                    TransactionId = transaction.Key,
                    Amount = transactionValue.Amount,
                    CreateTime = transactionValue.CreateTime,
                    Description = transactionValue.Description,
                    Type = transactionValue.Type,
                    AdderId = transactionValue.AddedBy,
                    AdderName = users?.First(x => x.Id == transactionValue.AddedBy).FirstName,
                    AdderSurname = users?.First(x => x.Id == transactionValue.AddedBy).LastName,
                    Category = transactionValue.Category,
                    RelatedUsers = new List<TransactionResponseItem>()
                };
                foreach (var item in transaction)
                {
                    var user = users.FirstOrDefault(x => x.Id == item.RelatedUserId);
                    if (user != null)
                    {
                        var relatedUser = new TransactionResponseItem
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

        private static GroupExpenseResponse CalculateGroupedExpenses(IReadOnlyCollection<GetGroupDetailResponseTransactionItem> groupDetailTransactions)
        {
            var response = new GroupExpenseResponse();

            if (groupDetailTransactions == null)
            {
                return null;
            }

            var expenses = groupDetailTransactions.Where(x => x.Type == TransactionType.Expense.ToString()).ToList();

            if (expenses.Count == 0)
            {
                return null;
            }

            response.Total = expenses.Sum(x => x.Amount);
            response.GroupedExpenses = new List<GroupExpenseResponseItem>();

            foreach (var expenseGroup in expenses.GroupBy(x => x.Category))
            {
                var categoryTotal = expenseGroup.Sum(y => y.Amount);
                response.GroupedExpenses.Add(new GroupExpenseResponseItem
                {
                    Category = expenseGroup.Key,
                    CategoryTotal = categoryTotal,
                    Percentage = response.Total == 0 ? 0 : (categoryTotal / response.Total) * 100
                });
            }

            return response;
        }

        #endregion
    }
}