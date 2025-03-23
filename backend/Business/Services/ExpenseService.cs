using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;

namespace Business.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ILogger<ColocationService> _logger;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;

        public ExpenseService(ILogger<ColocationService> logger,
            IExpenseRepository expenseRepository,
            IUserRepository userRepository)
        {
            _logger = logger;
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get all expenses
        /// </summary>
        /// <param name="ColocationId">The colocation of the returned expenses</param>
        /// <returns>All the expense of the colocation</returns>
        /// <exception cref="ContextException">An error happened during the retriving of expenses</exception>
        public async Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid colocationId)
        {
            var expenses = await _expenseRepository.GetAllExpensesDTOAsync(colocationId);

            _logger.LogInformation("Succes : All expenses were retrived from db");

            return expenses.Select(x => x.ToOutput()).OrderBy(x => x.DateOfPayment).ToList();
        }

        /// <summary>
        /// Get an expense
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <returns>The expense</returns>
        /// <exception cref="ContextException">An error occured during the retrival of the expense</exception>
        public async Task<ExpenseOutput> GetExpenseAsync(Guid id)
        {
            var expense = await _expenseRepository.GetExpenseDTOAsync(id);
                
            if (expense == null)
            {
                throw new NotFoundException($"The expense with id {id} was not found");
            }
                
            _logger.LogInformation($"Succes : Expense with id {id} was retrived from db");
                
            return expense.ToOutput();
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
        /// Add the entries to the database and update the balance with the amount splited evenly
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntriesAndUpdateBalance(ExpenseInput input, Guid expenseId, Dictionary<Guid, decimal> splitedAmounts)
        {
            var entryList = new List<Entry>();
            var balances = await _expenseRepository.GetBalanceFromUserIdListAsync(input.SplitBetween!);

            if (balances.Count != input.SplitBetween!.Count)
            {
                throw new InvalidEntityException("Some users in the split between list are not in the colocation");
            }

            _logger.LogInformation($"Succes : All balances from the users found");

            for (int i = 0; i < input.SplitBetween!.Count; i++)
            {
                var newEntry = new Entry
                {
                    UserId = input.SplitBetween[i],
                    ExpenseId = expenseId,
                    Amount = -splitedAmounts[input.SplitBetween[i]]
                };
                entryList.Add(newEntry);
                var balance = balances.First(x => x.UserId == newEntry.UserId);
                balance.PersonalBalance += newEntry.Amount;
                balance.LastUpdate = DateTime.UtcNow;
                _logger.LogInformation($"Succes : Entry for user {newEntry.UserId} added");
            }

            var paiment = new Entry
            {
                UserId = input.PaidBy,
                ExpenseId = expenseId,
                Amount = input.Amount
            };
            entryList.Add(paiment);
            var paimentBalance = balances.First(x => x.UserId == input.PaidBy);
            paimentBalance.PersonalBalance += input.Amount;
            paimentBalance.LastUpdate = DateTime.UtcNow;

            _logger.LogInformation($"Succes : Entry for user {paiment.UserId} added");

            try
            {
                await _expenseRepository.UpdateRangeBalanceAsync(balances);
                await _expenseRepository.AddRangeEntryAsync(entryList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding/updating the entries and balances to the db");
                throw new ContextException("An error occurred while adding/updating the entries and balances to the db", ex);
            }

            _logger.LogInformation($"Succes : All entries added to the db");
            _logger.LogInformation($"Succes : All balances updated");
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
        /// Add an expense to the database
        /// </summary>
        /// <param name="input">The Expense input</param>
        /// <returns>The GUID of the created expense</returns>
        /// <exception cref="InvalidEntityException">The input is invalid</exception>
        /// <exception cref="ContextException">An error when the expense is created in the DB</exception>
        public async Task<Guid> AddExpenseAsync(ExpenseInput input)
        {
            using (var transaction = await _expenseRepository.BeginTransactionAsync())
            {
                try
                {
                    _logger.LogInformation("Succes : Transaction started");

                    var expense = new Expense
                    {
                        Id = Guid.NewGuid(),
                        ColocationId = input.ColocationId,
                        CreatedBy = input.CreatedBy,
                        Name = input.Name,
                        Description = input.Description,
                        Amount = input.Amount,
                        PaidBy = input.PaidBy,
                        SplitType = input.SplitType.ToString(),
                        DateOfPayment = input.DateOfPayment
                    };

                    List<SplitBetween> splitBetween = null!;

                    switch (input.SplitType)
                    {
                        case SplitTypeEnum.ByValue:
                            if (input.SplitValues is null || !CheckTotalAmount(input.SplitValues, input.Amount))
                            {
                                throw new InvalidEntityException("The expense must have a value for each person and the total must be equal to the sum of the expenses");
                            }

                            _logger.LogInformation("Succes : Expense is split by value");

                            input.SplitBetween = input.SplitValues.Keys.ToList();

                            await AddEntriesAndUpdateBalance(input, expense.Id, input.SplitValues);

                            splitBetween = input.SplitValues.Select(x => new SplitBetween
                            {
                                ExpenseId = expense.Id,
                                UserId = x.Key
                            }).ToList();
                            break;

                        case SplitTypeEnum.ByPercentage:
                            if (input.SplitPercentages is null || !CheckPercent(input.SplitPercentages))
                            {
                                throw new InvalidEntityException("The expense must have a percentage for each person and the percentage must be equal to 100%");
                            }

                            _logger.LogInformation("Succes : Expense is split by percentage");

                            input.SplitBetween = input.SplitPercentages.Keys.ToList();

                            await AddEntriesAndUpdateBalance(input, expense.Id, SplitAmountByPercentage(input.Amount, input.SplitPercentages));

                            splitBetween = input.SplitPercentages.Select(x => new SplitBetween
                            {
                                ExpenseId = expense.Id,
                                UserId = x.Key
                            }).ToList();
                            break;

                        default:
                            if (input.SplitBetween is null || input.SplitBetween.Count == 0)
                            {
                                throw new InvalidEntityException("The expense must be not null and split between number must at least be 1 people");
                            }

                            _logger.LogInformation("Succes : Expense is split evenly");

                            await AddEntriesAndUpdateBalance(input, expense.Id, SplitAmountEvenly(input.Amount, input.SplitBetween));

                            splitBetween = input.SplitBetween.Select(x => new SplitBetween
                            {
                                ExpenseId = expense.Id,
                                UserId = x
                            }).ToList();
                            break;
                    }

                    try
                    {
                        await _expenseRepository.AddRangeSplitBetweenAsync(splitBetween);
                        await _expenseRepository.AddExpenseAsync(expense);
                        await _expenseRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while adding the expense to the db");
                        throw new ContextException("An error occurred while adding the expense to the db", ex);
                    }

                    _logger.LogInformation("Succes : All data added to the db");

                    transaction.Commit();
                        
                    _logger.LogInformation("Succes : Transaction commited");

                    return expense.Id;
                }
                catch (InvalidEntityException)
                {
                    _logger.LogError("The input is invalid, transaction rollbacked");
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
        /// Get all balances from a colocation
        /// </summary>
        /// <param name="ColocationId">The colocation id of the balance you want</param>
        /// <returns>The list of all balances</returns>
        /// <exception cref="ContextException">Error in db</exception>
        public async Task<List<BalanceOutput>> GetAllBalanceAsync(Guid ColocationId)
        {
            var balances = await _expenseRepository.GetAllBalancesOutputFromColocationIdListAsync(ColocationId);
                
            _logger.LogInformation($"Succes : All balances from the colocation {ColocationId} found");
                
            return balances;
        }

        public async Task<List<BalanceOutput>> RecalculateBalanceAsync(Guid colocationId)
        {
            var entries = await _expenseRepository.GetAllEntriesFromColocationIdAsync(colocationId);
            var balances = await _expenseRepository.GetAllBalancesFromColocationIdListAsync(colocationId);

            if (entries.Count == 0 || balances.Count == 0)
            {
                _logger.LogInformation($"Succes : No entries or balance found for the colocation {colocationId}");
                return balances.Select(x => x.ToOutput()).ToList();
            }

            _logger.LogInformation($"Succes : All entries and balances from the colocation {colocationId} found");

            balances.Select(b => b.PersonalBalance = entries.Where(e => e.UserId == b.UserId).Select(x => x.Amount).Sum()).ToList();

            await _expenseRepository.UpdateRangeBalanceAsync(balances);
            await _expenseRepository.SaveChangesAsync();
                
            _logger.LogInformation($"Succes : All balances updated");

            return balances.Select(x => x.ToOutput()).ToList();
        }
    }
}
