using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services
{
    public class ExpenseService(ILogger<ColocationService> _logger,
        IRepository<Expense> _expenseRepository,
        IRepository<Entry> _entryRepository,
        IRepository<SplitBetween> _splitbetweenRepository,
        IRepository<Balance> _balanceRepository) : IExpenseService
    {
        /// <summary>
        /// Get all expenses
        /// </summary>
        /// <param name="ColocationId">The colocation of the returned expenses</param>
        /// <returns>All the expense of the colocation</returns>
        /// <exception cref="ContextException">An error happened during the retriving of expenses</exception>
        public async Task<List<OutputFormatForExpenses>> GetAllExpensesAsync(Guid colocationId)
        {
            var expensesRaw = await _expenseRepository.Query()
                .Where(e => e.ColocationId == colocationId)
                .Include(e => e.SplitBetweens)
                .ToListAsync();

            var expenses = expensesRaw.Select(e => new ExpenseOutput
            {
                Id = e.Id,
                ColocationId = e.ColocationId,
                CreatedBy = e.CreatedBy,
                Name = e.Name,
                Description = e.Description,
                Amount = e.Amount,
                PaidBy = e.PaidBy,
                SplitType = Enum.Parse<SplitTypeEnum>(e.SplitType),
                SplitBetween = e.SplitBetweens.AsEnumerable().ToDictionary(k => k.UserId, v => v.Amount),
                DateOfPayment = e.DateOfPayment,
                Category = e.Category
            }).ToList();

            _logger.LogInformation("Succes : All expenses were retrived from db");

            return expenses.ToOutputFormat();
        }

        /// <summary>
        /// Get an expense
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <returns>The expense</returns>
        /// <exception cref="ContextException">An error occured during the retrival of the expense</exception>
        public async Task<ExpenseOutput> GetExpenseAsync(Guid id)
        {
            var expenseRaw = await _expenseRepository.Query()
                .Where(e => e.Id == id)
                .Include(e => e.SplitBetweens)
                .FirstOrDefaultAsync();

            if (expenseRaw == null)
            {
                throw new NotFoundException($"The expense with id {id} was not found");
            }

            var expense = new ExpenseOutput
            {
                Id = expenseRaw.Id,
                ColocationId = expenseRaw.ColocationId,
                CreatedBy = expenseRaw.CreatedBy,
                Name = expenseRaw.Name,
                Description = expenseRaw.Description,
                Amount = expenseRaw.Amount,
                PaidBy = expenseRaw.PaidBy,
                SplitType = Enum.Parse<SplitTypeEnum>(expenseRaw.SplitType),
                SplitBetween = expenseRaw.SplitBetweens.AsEnumerable().ToDictionary(k => k.UserId, v => v.Amount),
                DateOfPayment = expenseRaw.DateOfPayment,
                Category = expenseRaw.Category
            };
                
            _logger.LogInformation($"Succes : Expense with id {id} was retrived from db");
                
            return expense;
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

        public async Task CreateSplitBetween(InputExpenseDTO input, Dictionary<Guid, decimal> splitedAmounts)
        {
            List<SplitBetween> splitBetween = null!;

            splitBetween = input.SplitBetween!.Select(x => new SplitBetween
            {
                ExpenseId = input.Id,
                UserId = x,
                Amount = splitedAmounts[x]
            }).ToList();

            await _splitbetweenRepository.AddRangeAsync(splitBetween);
        }

        /// <summary>
        /// Add the entries to the database and update the balance with the amount splited evenly
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntriesAndUpdateBalance(InputExpenseDTO input, Dictionary<Guid, decimal> splitedAmounts)
        {
            var entryList = new List<Entry>();
            var balances = await _balanceRepository.Query()
                .Where(b => input.SplitBetween!.Contains(b.UserId))
                .AsNoTracking()
                .ToListAsync();

            if (balances.Count() != input.SplitBetween!.Count())
            {
                throw new InvalidEntityException("Some users in the split between list are not in the colocation");
            }

            _logger.LogInformation($"Succes : All balances from the users found");

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
                var balance = balances.First(x => x.UserId == newEntry.UserId);
                balance.PersonalBalance += newEntry.Amount;
                balance.LastUpdate = DateTime.UtcNow;
                _logger.LogInformation($"Succes : Entry for user {newEntry.UserId} added");
            }

            var paiment = new Entry
            {
                UserId = input.PaidBy,
                ExpenseId = input.Id,
                Amount = input.Amount,
                ColocationId = input.ColocationId,
            };
            entryList.Add(paiment);
            var paimentBalance = balances.First(x => x.UserId == input.PaidBy);
            paimentBalance.PersonalBalance += input.Amount;
            paimentBalance.LastUpdate = DateTime.UtcNow;

            _logger.LogInformation($"Succes : Entry for user {paiment.UserId} added");

            await CreateSplitBetween(input, splitedAmounts);

            _balanceRepository.UpdateRange(balances);

            await _entryRepository.AddRangeAsync(entryList);
            
            _logger.LogInformation($"Succes : All entries added to the db");
            _logger.LogInformation($"Succes : All balances updated");
        }
        
        private async Task CreateEntriesAndUpdateBalance(InputExpenseDTO input)
        {
            switch (input.SplitType)
            {
                case SplitTypeEnum.ByValue:
                    if (input.SplitValues is null || !CheckTotalAmount(input.SplitValues, input.Amount))
                    {
                        throw new InvalidEntityException("The expense must have a value for each person and the total must be equal to the sum of the expenses");
                    }

                    _logger.LogInformation("Succes : Expense is split by value");

                    input.SplitBetween = input.SplitValues.Keys.ToList();

                    await AddEntriesAndUpdateBalance(input, input.SplitValues);

                    
                    break;

                case SplitTypeEnum.ByPercentage:
                    if (input.SplitPercentages is null || !CheckPercent(input.SplitPercentages))
                    {
                        throw new InvalidEntityException("The expense must have a percentage for each person and the percentage must be equal to 100%");
                    }

                    _logger.LogInformation("Succes : Expense is split by percentage");

                    input.SplitBetween = input.SplitPercentages.Keys.ToList();

                    await AddEntriesAndUpdateBalance(input, SplitAmountByPercentage(input.Amount, input.SplitPercentages));

                    break;

                default:
                    if (input.SplitBetween is null || input.SplitBetween.Count == 0)
                    {
                        throw new InvalidEntityException("The expense must be not null and split between number must at least be 1 people");
                    }

                    _logger.LogInformation("Succes : Expense is split evenly");

                    await AddEntriesAndUpdateBalance(input, SplitAmountEvenly(input.Amount, input.SplitBetween));

                    break;
            }

            _logger.LogInformation("Succes : All data added to the db");
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
            _logger.LogInformation("Succes : Transaction started");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                ColocationId = input.ColocationId,
                CreatedBy = input.CreatedBy,
                Name = input.Name,
                Description = input.Description,
                Amount = input.Amount,
                Category = input.Category,
                PaidBy = input.PaidBy,
                SplitType = input.SplitType.ToString(),
                DateOfPayment = input.DateOfPayment
            };

            using (var transaction = await _expenseRepository.BeginTransactionAsync())
            {
                try
                {
                    await CreateEntriesAndUpdateBalance(new InputExpenseDTO
                    {
                        Id = expense.Id,
                        Amount = input.Amount,
                        ColocationId = input.ColocationId,
                        PaidBy = input.PaidBy,
                        SplitBetween = input.SplitBetween,
                        SplitPercentages = input.SplitPercentages,
                        SplitValues = input.SplitValues,
                        SplitType = input.SplitType
                    });

                    await _expenseRepository.AddAsync(expense);
                    await _expenseRepository.SaveChangesAsync();

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
        /// Update an expense
        /// </summary>
        /// <param name="input">The model to update the existing expense</param>
        /// <returns>The Guid of the updated expense</returns>
        public async Task<Guid> UpdateExpenseAsync(ExpenseUpdate input)
        {
            var expense = await _expenseRepository.GetByIdAsync(input.Id);

            if (expense is null)
                throw new NotFoundException($"The expense with id {input.Id} was not found");

            using (var transaction = await _expenseRepository.BeginTransactionAsync())
            {
                try
                {
                    expense.Name = input.Name;
                    expense.Description = input.Description;
                    expense.DateOfPayment = input.DateOfPayment;
                    expense.Amount = input.Amount;
                    expense.PaidBy = input.PaidBy;
                    expense.SplitType = input.SplitType.ToString();
                    expense.Category = input.Category;
                    expense.DateOfPayment = input.DateOfPayment;

                    var entries = await _entryRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ToListAsync();

                    var splitbetween = await _splitbetweenRepository.Query()
                        .Where(e => e.ExpenseId == expense.Id)
                        .ToListAsync();

                    _ = _entryRepository.DeleteRangeAsync(entries);
                    _ = _splitbetweenRepository.DeleteRangeAsync(splitbetween);

                    await CreateEntriesAndUpdateBalance(new InputExpenseDTO
                    {
                        Id = input.Id,
                        Amount = input.Amount,
                        PaidBy = input.PaidBy,
                        SplitBetween = input.SplitBetween,
                        SplitPercentages = input.SplitPercentages,
                        SplitValues = input.SplitValues,
                        SplitType = input.SplitType,
                        ColocationId = input.ColocationId
                    });

                    _expenseRepository.Update(expense);

                    await _expenseRepository.SaveChangesAsync();

                    await RecalculateBalanceAsync(input.ColocationId);

                    transaction.Commit();

                    _logger.LogInformation("Succes : Transaction commited");

                    return expense.Id;
                }
                catch (InvalidEntityException)
                {
                    _logger.LogError("The input object is invalid, transaction rollbacked");
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
            await _expenseRepository.DeleteFromIdAsync(id);
            await _expenseRepository.SaveChangesAsync();
            _logger.LogInformation($"Succes : Expense with id {id} deleted");
            return id;
        }

        /// <summary>
        /// Get all balances from a colocation
        /// </summary>
        /// <param name="ColocationId">The colocation id of the balance you want</param>
        /// <returns>The list of all balances</returns>
        /// <exception cref="ContextException">Error in db</exception>
        public async Task<List<BalanceOutput>> GetAllBalanceAsync(Guid ColocationId)
        {
            var balances = await _balanceRepository.Query()
                .Where(e => e.User.ColocationId == ColocationId)
                .Select(e => new BalanceOutput
                {
                    UserId = e.UserId,
                    PersonalBalance = e.PersonalBalance,
                    LastUpdate = e.LastUpdate,
                })
                .ToListAsync();
                
            _logger.LogInformation($"Succes : All balances from the colocation {ColocationId} found");
                
            return balances;
        }

        public async Task<List<BalanceOutput>> RecalculateBalanceAsync(Guid colocationId)
        {
            var entries = await _entryRepository.Query()
                .Where(e => e.ColocationId == colocationId)
                .ToListAsync();
            var balances = await _balanceRepository.Query()
                .Where(b => b.User.ColocationId == colocationId)
                .ToListAsync();

            if (entries.Count == 0 || balances.Count == 0)
            {
                _logger.LogInformation($"Succes : No entries or balance found for the colocation {colocationId}");
                return balances.Select(x => x.ToOutput()).ToList();
            }

            _logger.LogInformation($"Succes : All entries and balances from the colocation {colocationId} found");

            balances.Select(b => b.PersonalBalance = entries.Where(e => e.UserId == b.UserId).Select(x => x.Amount).Sum()).ToList();

            _balanceRepository.UpdateRange(balances);
            await _expenseRepository.SaveChangesAsync();
                
            _logger.LogInformation($"Succes : All balances updated");

            return balances.Select(x => x.ToOutput()).OrderBy(x => x.PersonalBalance).ToList();
        }
    }
}
