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
        public async Task<List<ExpenseOutput>> GetAllExpensesAsync(Guid ColocationId)
        {
            try
            {
                var expenses = await _expenseRepository.GetAllExpensesDTOAsync(ColocationId);

                _logger.LogInformation("Succes : All expenses were retrived from db");

                return expenses.Select(x => x.ToOutput()).OrderBy(x => x.DateOfPayment).ToList();
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting all expenses from the db", ex);
            }
        }

        /// <summary>
        /// Get an expense
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <returns>The expense</returns>
        /// <exception cref="ContextException">An error occured during the retrival of the expense</exception>
        public async Task<ExpenseOutput> GetExpenseAsync(Guid id)
        {
            try
            {
                var expense = await _expenseRepository.GetExpenseDTOAsync(id);
                
                if (expense == null)
                {
                    throw new NotFoundException($"The expense with id {id} was not found");
                }
                
                _logger.LogInformation($"Succes : Expense with id {id} was retrived from db");
                
                return expense.ToOutput();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting the expense from the db", ex);
            }
        }

        /// <summary>
        /// Split the amount of the expense between the people
        /// </summary>
        /// <param name="totalAmount">The total of the ammount to share</param>
        /// <param name="numPeople">The number of people</param>
        /// <returns>A list of *numPeople* decimal splited</returns>
        private static List<decimal> SplitAmountEvenly(decimal totalAmount, int numPeople)
        {
            decimal baseAmount = Math.Floor(totalAmount / numPeople * 100) / 100;
            decimal remainingCents = totalAmount - (baseAmount * numPeople);

            List<decimal> result = Enumerable.Repeat(baseAmount, numPeople).ToList();

            for (int i = 0; i < remainingCents * 100; i++)
            {
                result[i] += 0.01m;
            }

            return result;
        }

        /// <summary>
        /// Add the entries to the database and update the balance with the amount splited evenly
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntryWithEvenlyType(ExpenseInput input, Guid expenseId)
        {
            var splitAmounts = SplitAmountEvenly(input.Amount, input.SplitBetween!.Count);
            var entryList = new List<Entry>();
            var balances = await _expenseRepository.GetBalanceFromUserIdListAsync(input.SplitBetween);

            for (int i = 0; i < input.SplitBetween.Count; i++)
            {
                var newEntry = new Entry
                {
                    UserId = input.SplitBetween[i],
                    ExpenseId = expenseId,
                    Amount = -splitAmounts[i]
                };
                entryList.Add(newEntry);
                var balance = balances.First(x => x.UserId == newEntry.UserId);
                balance.PersonalBalance += newEntry.Amount;
                balance.LastUpdate = DateTime.UtcNow;
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

            await _expenseRepository.UpdateRangeBalanceAsync(balances);
            await _expenseRepository.AddRangeEntryAsync(entryList);
        }

        /// <summary>
        /// Add the entries to the database and update the balance with the amount splited with the value
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntryWithValueType(ExpenseInput input, Guid expenseId)
        {
            var entryList = new List<Entry>();
            var users = input.SplitValues!.Keys.ToList();
            var balances = await _expenseRepository.GetBalanceFromUserIdListAsync(users);

            foreach (var entry in input.SplitBetween!)
            {
                var newEntry = new Entry
                {
                    UserId = entry,
                    ExpenseId = expenseId,
                    Amount = input.SplitValues![entry]
                };
                entryList.Add(newEntry);
                var balance = balances.First(x => x.UserId == entry);
                balance.PersonalBalance += newEntry.Amount;
                balance.LastUpdate = DateTime.UtcNow;
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

            await _expenseRepository.UpdateRangeBalanceAsync(balances);
            await _expenseRepository.AddRangeEntryAsync(entryList);
        }

        /// <summary>
        /// Add the entries to the database and update the balance with the amount splited with the percentage
        /// </summary>
        /// <param name="input">The input of AddExpense</param>
        private async Task AddEntryWithPercentageType(ExpenseInput input, Guid expenseId)
        {
            var entryList = new List<Entry>();
            var users = input.SplitPercentages!.Keys.ToList();
            var balances = await _expenseRepository.GetBalanceFromUserIdListAsync(users);

            foreach (var entry in input.SplitBetween!)
            {
                var newEntry = new Entry
                {
                    UserId = entry,
                    ExpenseId = expenseId,
                    Amount = input.Amount * input.SplitPercentages![entry] / 100
                };
                entryList.Add(newEntry);
                var balance = balances.First(x => x.UserId == entry);
                balance.PersonalBalance += newEntry.Amount;
                balance.LastUpdate = DateTime.UtcNow;
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

            await _expenseRepository.UpdateRangeBalanceAsync(balances);
            await _expenseRepository.AddRangeEntryAsync(entryList);
        }

        private static bool CheckPercent(Dictionary<Guid, int> splitPercentages)
        {
            int sum = 0;
            foreach (var percentage in splitPercentages)
            {
                sum += percentage.Value;
            }
            return sum == 100;
        }

        public async Task<Guid> AddExpenseAsync(ExpenseInput input)
        {
            try
            {
                if (input is null || input.SplitBetween is null || input.SplitBetween!.Count == 0)
                {
                    throw new InvalidEntityException("The expense must be split between at least one person");
                }

                using (var transaction = await _expenseRepository.BeginTransactionAsync())
                {
                    try
                    {
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

                        if (input.SplitType == SplitTypeEnum.ByValue)
                        {
                            if (input.SplitValues is null)
                            {
                                throw new InvalidEntityException("The expense must have a value for each person");
                            }
                            await AddEntryWithValueType(input, expense.Id);
                        }
                        else if (input.SplitType == SplitTypeEnum.ByPercentage)
                        {
                            if (input.SplitPercentages is null || !CheckPercent(input.SplitPercentages))
                            {
                                throw new InvalidEntityException("The expense must have a percentage for each person and it must be equal to 100%");
                            }
                            await AddEntryWithPercentageType(input, expense.Id);
                        }
                        else
                        {
                            if (input.SplitBetween.Count == 0)
                            {
                                throw new InvalidEntityException("The expense must be not null and split between number must at least 1 people");
                            }
                            await AddEntryWithEvenlyType(input, expense.Id);
                        }

                        var splitBetween = input.SplitBetween.Select(x => new SplitBetween
                        {
                            ExpenseId = expense.Id,
                            UserId = x
                        }).ToList();

                        await _expenseRepository.AddRangeSplitBetweenAsync(splitBetween);

                        await _expenseRepository.AddExpenseAsync(expense);

                        await _expenseRepository.SaveChangesAsync();
                        
                        transaction.Commit();
                        
                        return expense.Id;
                    }
                    catch (InvalidEntityException)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new ContextException("NO DATA WAS MODIFIED : An error occurred while adding the expense to the db", ex);
                    }
                }
            }
            catch (InvalidEntityException)
            {
                throw;
            }
            catch (ContextException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ContextException("NO DATA WAS MODIFIED : An error happened during the creation of the transaction");
            }
        }
    }
}
