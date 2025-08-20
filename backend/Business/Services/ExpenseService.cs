using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ExpenseService(ILogger<ColocationService> logger,
        IRepository<ExpenseCategory> expenseCategoryRepository,
        IRepository<Expense> expenseRepository,
        IRepository<Entry> entryRepository,
        IRepository<SplitBetween> splitbetweenRepository,
        IRepository<User> userRepository,
        IRealTimeService realTimeService,
        IAppCache cache) : IExpenseService
    {
        /// <summary>
        /// Get all expense categories
        /// </summary>
        /// <param name="colocationId">The colocation you want the expense categories from</param>
        /// <returns>The list of the expense categories</returns>
        public async Task<List<ExpenseCategoryOutput>> GetAllExpenseCategoriesAsync(Guid colocationId)
        {
            var cacheKey = $"expenseCategories:{colocationId}";

            return await cache.GetOrAddAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var expenseCategories = await expenseCategoryRepository.Query()
                    .Where(e => e.ColocationId == colocationId)
                    .Include(e => e.Expenses)
                    .Select(e => new ExpenseCategoryOutput
                    {
                        Id = e.Id,
                        Name = e.Name,
                        TotalAmount = e.Expenses.Sum(e => e.Amount)
                    })
                    .ToListAsync();

                logger.LogInformation("Succes : All expense categories were retrived from db");

                return expenseCategories;
            });
        }

        /// <summary>
        /// Get all expenses
        /// </summary>
        /// <param name="ColocationId">The colocation of the returned expenses</param>
        /// <returns>All the expense of the colocation</returns>
        /// <exception cref="ContextException">An error happened during the retriving of expenses</exception>
        public async Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid expenseCategoryId)
        {
            var expensesRaw = await expenseRepository.Query()
                .Where(e => e.ExpenseCategoryId == expenseCategoryId)
                .Include(e => e.SplitBetweens)
                .Include(e => e.ExpenseCategory)
                .ToListAsync();

            var expenses = expensesRaw.Select(e => new ExpenseOutput
            {
                Id = e.Id,
                CreatedBy = e.CreatedBy,
                Name = e.Name,
                Description = e.Description,
                Amount = e.Amount,
                PaidBy = e.PaidBy,
                SplitType = Enum.Parse<SplitTypeEnum>(e.SplitType),
                SplitBetween = e.SplitBetweens.AsEnumerable().ToDictionary(k => k.UserId, v => v.Amount),
                DateOfPayment = e.DateOfPayment,
                ExpenseCategoryId = e.ExpenseCategoryId,
                ExpenseCategoryName = e.ExpenseCategory.Name
            }).ToList();

            logger.LogInformation("Succes : All expenses were retrived from db");

            return expenses;
        }

        /// <summary>
        /// Get an expense
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <returns>The expense</returns>
        /// <exception cref="ContextException">An error occured during the retrival of the expense</exception>
        public async Task<ExpenseOutput> GetExpenseAsync(Guid id)
        {
            var expenseRaw = await expenseRepository.Query()
                .Where(e => e.Id == id)
                .Include(e => e.SplitBetweens)
                .Include(e => e.ExpenseCategory)
                .FirstOrDefaultAsync();

            if (expenseRaw == null)
            {
                throw new NotFoundException($"The expense with id {id} was not found");
            }

            var expense = new ExpenseOutput
            {
                Id = expenseRaw.Id,
                CreatedBy = expenseRaw.CreatedBy,
                Name = expenseRaw.Name,
                Description = expenseRaw.Description,
                Amount = expenseRaw.Amount,
                PaidBy = expenseRaw.PaidBy,
                SplitType = Enum.Parse<SplitTypeEnum>(expenseRaw.SplitType),
                SplitBetween = expenseRaw.SplitBetweens.AsEnumerable().ToDictionary(k => k.UserId, v => v.Amount),
                DateOfPayment = expenseRaw.DateOfPayment,
                ExpenseCategoryId = expenseRaw.ExpenseCategoryId,
                ExpenseCategoryName = expenseRaw.ExpenseCategory.Name
            };
                
            logger.LogInformation($"Succes : Expense with id {id} was retrived from db");
                
            return expense;
        }

        /// <summary>
        /// Add an expense category to the database
        /// </summary>
        /// <param name="input">The data to add into the DB</param>
        /// <returns>The Guid of the added category</returns>
        public async Task<Guid> AddExpenseCategoryAsync(ExpenseCategoryInput input)
        {
            var expenseCategory = new ExpenseCategory
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                ColocationId = input.ColocationId
            };
            await expenseCategoryRepository.AddAsync(expenseCategory);
            await expenseCategoryRepository.SaveChangesAsync();

            cache.Remove($"expenseCategories:{input.ColocationId}");

            var expenseCategoryOutput = new ExpenseCategoryOutput
            {
                Id = expenseCategory.Id,
                Name = expenseCategory.Name,
                TotalAmount = 0
            };
            await realTimeService.SendToGroupAsync(expenseCategory.ColocationId, "NewExpenseCategoryAdded", expenseCategoryOutput);

            logger.LogInformation($"Succes : Expense category with id {expenseCategory.Id} was added to db");
            return expenseCategory.Id;
        }

        /// <summary>
        /// Update an expense category
        /// </summary>
        /// <param name="input">The data to update</param>
        /// <returns>the Guid of the updated data</returns>
        /// <exception cref="NotFoundException">The data was not found</exception>
        public async Task<Guid> UpdateExpenseCategoryAsync(ExpenseCategoryUpdate input)
        {
            var expenseCategory = await expenseCategoryRepository.GetByIdAsync(input.Id);
            
            if (expenseCategory is null)
                throw new NotFoundException($"The expense category with id {input.Id} was not found");

            expenseCategory.Name = input.Name;

            expenseCategoryRepository.Update(expenseCategory);
            await expenseCategoryRepository.SaveChangesAsync();

            cache.Remove($"expenseCategories:{expenseCategory.ColocationId}");

            var expenseCategoryOutput = new ExpenseCategoryOutput
            {
                Id = expenseCategory.Id,
                Name = expenseCategory.Name,
                TotalAmount = expenseCategory.Expenses.Sum(e => e.Amount)
            };
            await realTimeService.SendToGroupAsync(expenseCategory.ColocationId, "ExpenseCategoryUpdated", expenseCategoryOutput);

            logger.LogInformation($"Succes : Expense category with id {expenseCategory.Id} was updated in db");

            return expenseCategory.Id;
        }

        /// <summary>
        /// Delete an expense category
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <returns>The id of the deleted data </returns>
        /// <exception cref="NotFoundException">The data was not found</exception>
        public async Task<Guid> DeleteExpenseCategoryAsync(Guid id)
        {
            var expenseCategory = await expenseCategoryRepository.GetByIdAsync(id);
            if (expenseCategory is null)
                throw new NotFoundException($"The expense category with id {id} was not found");
            expenseCategoryRepository.Delete(expenseCategory);
            await expenseCategoryRepository.SaveChangesAsync();

            cache.Remove($"expenseCategories:{expenseCategory.ColocationId}");
            cache.Remove($"balances:{expenseCategory.ColocationId}");
            cache.Remove($"refundMethods:{expenseCategory.ColocationId}");

            await realTimeService.SendToGroupAsync(expenseCategory.ColocationId, "ExpenseCategoryDeleted", expenseCategory.Id);

            logger.LogInformation($"Succes : Expense category with id {id} was deleted from db");
            return id;
        }

        /// <summary>
        /// Check if the percentage of the split is equal to 100%
        /// </summary>
        /// <param name="splitPercentages">The percentage</param>
        /// <returns>true if its equal to 100%, false if not</returns>
        private static bool CheckPercent(Dictionary<Guid, int> splitPercentages)
        {
            int sum = 0;
            foreach (var percentage in splitPercentages)
            {
                sum += percentage.Value;
            }
            return sum == 100;
        }

        /// <summary>
        /// Check if the total match with all the amount splited
        /// </summary>
        /// <param name="splitValue"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private static bool CheckTotalAmount(Dictionary<Guid, decimal> splitValue, decimal total)
        {
            decimal sum = 0;
            foreach (var value in splitValue)
            {
                sum += value.Value;
            }
            return sum == total;
        }

        /// <summary>
        /// Split the amount of the expense between the people
        /// </summary>
        /// <param name="totalAmount">The total of the ammount to share</param>
        /// <param name="numPeople">The number of people</param>
        /// <returns>A list of *numPeople* decimal splited</returns>
        private static Dictionary<Guid, decimal> SplitAmountEvenly(decimal totalAmount, List<Guid> peopleList)
        {
            decimal baseAmount = Math.Floor(totalAmount / peopleList.Count * 100) / 100;
            decimal remainingCents = totalAmount - (baseAmount * peopleList.Count);

            Dictionary<Guid, decimal> result = new Dictionary<Guid, decimal>();

            foreach (var person in peopleList)
            {
                result[person] = baseAmount;
            }

            for (int i = 0; i < remainingCents * 100; i++)
            {
                result[peopleList[i % peopleList.Count]] += 0.01m;
            }

            return result;
        }

        /// <summary>
        /// Distribute the total amount based on a percentage split among individuals.
        /// </summary>
        /// <param name="totalAmount">The total amount to distribute.</param>
        /// <param name="percentageSplit">A dictionary where the key is a Guid representing a person, and the value is their percentage of the total.</param>
        /// <returns>A dictionary with the total amount allocated to each Guid.</returns>
        private static Dictionary<Guid, decimal> SplitAmountByPercentage(decimal totalAmount, Dictionary<Guid, int> percentageSplit)
        {
            var result = new Dictionary<Guid, decimal>();

            decimal remainingAmount = totalAmount;
            var sortedEntries = percentageSplit.OrderByDescending(entry => entry.Value).ToList();

            foreach (var entry in sortedEntries)
            {
                decimal allocatedAmount = Math.Floor((totalAmount * entry.Value / 100) * 100) / 100;
                result[entry.Key] = allocatedAmount;
                remainingAmount -= allocatedAmount;
            }

            var keys = sortedEntries.Select(e => e.Key).ToList();

            for (int i = 0; i < remainingAmount * 100; i++)
            {
                result[keys[i % keys.Count]] += 0.01m;
            }

            return result;
        }

        /// <summary>
        /// Create the split between the users
        /// </summary>
        /// <param name="input">The input user from the DTO in main service function</param>
        /// <param name="splitedAmounts">The expense splited between the users</param>
        public async Task CreateSplitBetween(InputExpenseDTO input, Dictionary<Guid, decimal> splitedAmounts)
        {
            List<SplitBetween> splitBetween = null!;

            splitBetween = input.SplitBetween!.Select(x => new SplitBetween
            {
                ExpenseId = input.Id,
                UserId = x,
                Amount = splitedAmounts[x]
            }).ToList();

            await splitbetweenRepository.AddRangeAsync(splitBetween);
        }

        /// <summary>
        /// Add the entries to the database and update the balance with the amount splited evenly
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntries(InputExpenseDTO input, Dictionary<Guid, decimal> splitedAmounts)
        {
            var entryList = new List<Entry>();

            for (int i = 0; i < input.SplitBetween!.Count; i++)
            {
                var newEntry = new Entry
                {
                    UserId = input.SplitBetween[i],
                    ColocationId = input.ColocationId,
                    ExpenseId = input.Id,
                    Amount = -splitedAmounts[input.SplitBetween[i]]
                };
                entryList.Add(newEntry);
                logger.LogInformation($"Succes : Entry for user {newEntry.UserId} added");
            }

            var paiment = new Entry
            {
                UserId = input.PaidBy,
                ExpenseId = input.Id,
                Amount = input.Amount,
                ColocationId = input.ColocationId,
            };
            entryList.Add(paiment);
            logger.LogInformation($"Succes : Entry for user {paiment.UserId} added");

            await CreateSplitBetween(input, splitedAmounts);

            await entryRepository.AddRangeAsync(entryList);
            
            logger.LogInformation($"Succes : All entries added to the db");
        }

        /// <summary>
        /// Create the entries based on the split type
        /// </summary>
        /// <param name="input">The input user from the DTO in main service function</param>
        /// <exception cref="InvalidEntityException">The expense input in invalid</exception>
        private async Task CreateEntriesBySplitType(InputExpenseDTO input)
        {
            switch (input.SplitType)
            {
                case SplitTypeEnum.ByValue:
                    if (input.SplitValues is null || !CheckTotalAmount(input.SplitValues, input.Amount))
                    {
                        throw new InvalidEntityException("The expense must have a value for each person and the total must be equal to the sum of the expenses");
                    }

                    logger.LogInformation("Succes : Expense is split by value");

                    input.SplitBetween = input.SplitValues.Keys.ToList();

                    await AddEntries(input, input.SplitValues);

                    
                    break;

                case SplitTypeEnum.ByPercentage:
                    if (input.SplitPercentages is null || !CheckPercent(input.SplitPercentages))
                    {
                        throw new InvalidEntityException("The expense must have a percentage for each person and the percentage must be equal to 100%");
                    }

                    logger.LogInformation("Succes : Expense is split by percentage");

                    input.SplitBetween = input.SplitPercentages.Keys.ToList();

                    await AddEntries(input, SplitAmountByPercentage(input.Amount, input.SplitPercentages));

                    break;

                default:
                    if (input.SplitBetween is null || input.SplitBetween.Count == 0)
                    {
                        throw new InvalidEntityException("The expense must be not null and split between number must at least be 1 people");
                    }

                    logger.LogInformation("Succes : Expense is split evenly");

                    await AddEntries(input, SplitAmountEvenly(input.Amount, input.SplitBetween));

                    break;
            }

            logger.LogInformation("Succes : All data added to the db");
        }

        /// <summary>
        /// Add an expense to the database
        /// </summary>
        /// <param name="input">The Expense input</param>
        /// <returns>The GUID of the created expense</returns>
        /// <exception cref="InvalidEntityException">The input is invalid</exception>
        /// <exception cref="ContextException">An error when the expense is created in the DB</exception>
        public async Task<Guid> AddExpenseAsync(ExpenseInput input)
        {
            logger.LogInformation("Succes : Transaction started");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                CreatedBy = input.CreatedBy,
                Name = input.Name,
                Description = input.Description,
                Amount = input.Amount,
                ExpenseCategoryId = input.ExpenseCategoryId,
                PaidBy = input.PaidBy,
                SplitType = input.SplitType.ToString(),
                DateOfPayment = input.DateOfPayment
            };

            using (var transaction = await expenseRepository.BeginTransactionAsync())
            {
                try
                {
                    await CreateEntriesBySplitType(new InputExpenseDTO
                    {
                        Id = expense.Id,
                        Amount = input.Amount,
                        ExpenseCategoryId = input.ExpenseCategoryId,
                        PaidBy = input.PaidBy,
                        SplitBetween = input.SplitBetween,
                        SplitPercentages = input.SplitPercentages,
                        SplitValues = input.SplitValues,
                        SplitType = input.SplitType,
                        ColocationId = input.ColocationId
                    });

                    await expenseRepository.AddAsync(expense);
                    await expenseRepository.SaveChangesAsync();

                    // Invalidate outated cache
                    cache.Remove($"balances:{input.ColocationId}");
                    cache.Remove($"expenseCategories:{input.ColocationId}");
                    cache.Remove($"refundMethods:{input.ColocationId}");

                    transaction.Commit();

                    var splitBetweens = await splitbetweenRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ToListAsync();

                    var expenseOutput = new ExpenseOutput
                    {
                        Id = expense.Id,
                        CreatedBy = expense.CreatedBy,
                        Name = expense.Name,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        PaidBy = expense.PaidBy,
                        SplitType = Enum.Parse<SplitTypeEnum>(expense.SplitType),
                        SplitBetween = splitBetweens.ToDictionary(k => k.UserId, v => v.Amount),
                        DateOfPayment = expense.DateOfPayment,
                        ExpenseCategoryId = expense.ExpenseCategoryId,
                        ExpenseCategoryName = expense.ExpenseCategory.Name
                    };
                    await realTimeService.SendToGroupAsync(input.ColocationId, "NewExpenseAdded", expenseOutput);

                    logger.LogInformation("Succes : Transaction commited");

                    return expense.Id;
                }
                catch (InvalidEntityException)
                {
                    logger.LogError("The input is invalid, transaction rollbacked");
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (ContextException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Update an expense
        /// </summary>
        /// <param name="input">The model to update the existing expense</param>
        /// <returns>The Guid of the updated expense</returns>
        public async Task<Guid> UpdateExpenseAsync(ExpenseUpdate input)
        {
            var expense = await expenseRepository.GetByIdAsync(input.Id);

            if (expense is null)
                throw new NotFoundException($"The expense with id {input.Id} was not found");

            using (var transaction = await expenseRepository.BeginTransactionAsync())
            {
                try
                {
                    expense.Name = input.Name;
                    expense.Description = input.Description;
                    expense.DateOfPayment = input.DateOfPayment;
                    expense.Amount = input.Amount;
                    expense.PaidBy = input.PaidBy;
                    expense.SplitType = input.SplitType.ToString();
                    expense.ExpenseCategoryId = input.ExpenseCategoryId;
                    expense.DateOfPayment = input.DateOfPayment;

                    var entries = await entryRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ExecuteDeleteAsync();

                    var splitbetween = await splitbetweenRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ExecuteDeleteAsync();

                    await CreateEntriesBySplitType(new InputExpenseDTO
                    {
                        Id = input.Id,
                        Amount = input.Amount,
                        PaidBy = input.PaidBy,
                        SplitBetween = input.SplitBetween,
                        SplitPercentages = input.SplitPercentages,
                        SplitValues = input.SplitValues,
                        SplitType = input.SplitType,
                        ColocationId = input.ColocationId,
                        ExpenseCategoryId = input.ExpenseCategoryId
                    });

                    expenseRepository.Update(expense);

                    await expenseRepository.SaveChangesAsync();

                    // Invalidate outated cache
                    cache.Remove($"balances:{input.ColocationId}");
                    cache.Remove($"expenseCategories:{input.ColocationId}");
                    cache.Remove($"refundMethods:{input.ColocationId}");

                    transaction.Commit();

                    var splitbetweens = await splitbetweenRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ToListAsync();

                    var expenseOutput = new ExpenseOutput
                    {
                        Id = expense.Id,
                        CreatedBy = expense.CreatedBy,
                        Name = expense.Name,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        PaidBy = expense.PaidBy,
                        SplitType = Enum.Parse<SplitTypeEnum>(expense.SplitType),
                        SplitBetween = splitbetweens.ToDictionary(k => k.UserId, v => v.Amount),
                        DateOfPayment = expense.DateOfPayment,
                        ExpenseCategoryId = expense.ExpenseCategoryId,
                        ExpenseCategoryName = expense.ExpenseCategory.Name
                    };
                    await realTimeService.SendToGroupAsync(input.ColocationId, "ExpenseUpdated", expenseOutput);

                    logger.LogInformation("Succes : Transaction commited");

                    return expense.Id;
                }
                catch (InvalidEntityException)
                {
                    logger.LogError("The input object is invalid, transaction rollbacked");
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (ContextException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete an expense
        /// </summary>
        /// <param name="id">The id of the expense to delete</param>
        /// <returns>The Guid of the deleted expense</returns>
        public async Task<Guid> DeleteExpenseAsync(Guid id)
        {
            var expense = await expenseRepository.Query()
                .Include(e => e.ExpenseCategory)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (expense is null)
                throw new InvalidDataException($"No expense found with this id : {id}");

            expenseRepository.Delete(expense);
            await expenseRepository.SaveChangesAsync();

            cache.Remove($"balances:{expense.ExpenseCategory.ColocationId}");
            cache.Remove($"expenseCategories:{expense.ExpenseCategory.ColocationId}");
            cache.Remove($"refundMethods:{expense.ExpenseCategory.ColocationId}");

            await realTimeService.SendToGroupAsync(expense.ExpenseCategory.ColocationId, "ExpenseDeleted", id);

            logger.LogInformation($"Succes : Expense with id {id} deleted");
            return id;
        }

        /// <summary>
        /// Get all balances from a colocation
        /// </summary>
        /// <param name="ColocationId">The colocation id of the balance you want</param>
        /// <returns>The list of all balances</returns>
        /// <exception cref="ContextException">Error in db</exception>
        public async Task<Dictionary<Guid, decimal>> GetAllBalanceAsync(Guid colocationId)
        {
            string cacheKey = $"balances:{colocationId}";

            return await cache.GetOrAddAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var entries = await entryRepository.Query()
                .Where(e => e.ColocationId == colocationId)
                .ToListAsync();

                var balances = entries.GroupBy(k => k.UserId)
                    .ToDictionary(k => k.Key,
                    v => v.Sum(e => e.Amount));

                logger.LogInformation($"Succes : All balances from the colocation {colocationId} calculated");

                return balances;
            });
        }

        /// <summary>
        /// Calculates the optimal refund strategy for a list of debts using a greedy algorithm
        /// to minimize the number of transactions.
        /// </summary>
        /// <param name="debts">List of debts, each represented by {UserId, Balance}.</param>
        /// <returns>A list of refund transactions required to settle all debts.</returns>
        private async Task<List<RefundOutput>> CalculateBestRefundMethod(Dictionary<Guid, decimal> debts)
        {
            var debtorList = debts
                .Where(x => x.Value < 0)
                .Select(x => new { UserId = x.Key, Amount = -x.Value })
                .OrderByDescending(x => x.Amount)
                .ToList();

            var creditorList = debts
                .Where(x => x.Value > 0)
                .Select(x => new { UserId = x.Key, Amount = x.Value })
                .OrderByDescending(x => x.Amount)
                .ToList();

            var refunds = new List<RefundOutput>();

            int i = 0, j = 0;

            while (i < debtorList.Count && j < creditorList.Count)
            {
                var debtor = debtorList[i];
                var creditor = creditorList[j];

                var transferAmount = Math.Min(debtor.Amount, creditor.Amount);

                refunds.Add(new RefundOutput
                {
                    From = debtor.UserId,
                    To = creditor.UserId,
                    Amount = Math.Round(transferAmount, 2)
                });

                debtorList[i] = new { debtor.UserId, Amount = debtor.Amount - transferAmount };
                creditorList[j] = new { creditor.UserId, Amount = creditor.Amount - transferAmount };

                if (debtorList[i].Amount == 0)
                    i++;

                if (creditorList[j].Amount == 0)
                    j++;
            }

            var userIds = refunds
                .SelectMany(r => new[] { r.From, r.To })
                .Distinct()
                .ToList();

            var userPPs = await userRepository
                .Query()
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.PathToProfilePicture);

            foreach (var refund in refunds)
            {
                refund.FromUrlPP = userPPs.GetValueOrDefault(refund.From, "default.jpg");
                refund.ToUrlPP = userPPs.GetValueOrDefault(refund.To, "default.jpg");
            }

            return refunds;
        }

        /// <summary>
        /// Get the best refund method for a colocation
        /// </summary>
        /// <param name="colocationId">The colocation you want the refund method from</param>
        /// <returns>The best ways to cancel the debts</returns>
        public async Task<List<RefundOutput>> GetRefundMethodsAsync(Guid colocationId)
        {
            var cacheKey = $"refundMethods:{colocationId}";
            return await cache.GetOrAddAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var balances = await GetAllBalanceAsync(colocationId);
                return await CalculateBestRefundMethod(balances);
            });
        }
    }
}
