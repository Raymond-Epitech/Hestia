using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories.Implementations;
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

        public ExpenseService(ILogger<ColocationService> logger, IExpenseRepository expenseRepository)
        {
            _logger = logger;
            _expenseRepository = expenseRepository;
        }

        public async Task<List<ExpenseOutput>> GetAllExpenses(Guid ColocationId)
        {
            try
            {
                var expenses = await _expenseRepository.GetAllExpensesDTOAsync(ColocationId);

                _logger.LogInformation("Succes : All expenses were retrived from db");

                return expenses.Select(x => x.ToOutput()).ToList();
            }
            catch (Exception ex)
            {
                throw new ContextException("An error occurred while getting all expenses from the db", ex);
            }
        }   

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

        private static async Task AddEntryWithEvenlyType(ExpenseInput input)
        {
            var splitAmounts = SplitAmountEvenly(input.Amount, input.SplitBetween.Count);

            for (int i = 0; i < input.SplitBetween.Count; i++)
            {
                var entry = new Entry
                {
                    UserId = input.SplitBetween[i],
                    Amount = splitAmounts[i]
                };

                await _expenseRepository.AddEntryAsync(entry);
            }
        }

        private static async Task AddEntryWithValueType(ExpenseInput input)
        {
            foreach (var entry in input.SplitBetween)
            {
                var newEntry = new Entry
                {
                    UserId = entry,
                    Amount = input.SplitValues![entry]
                };
                await _expenseRepository.AddEntryAsync(newEntry);
            }
        }

        private static async Task AddEntryWithPercentageType(ExpenseInput input)
        {
            foreach (var entry in input.SplitBetween)
            {
                var newEntry = new Entry
                {
                    UserId = entry,
                    Amount = input.Amount * input.SplitPercentages![entry] / 100
                };
                await _expenseRepository.AddEntryAsync(newEntry);
            }
        }

        public async Task<Guid> AddExpenseAsync(ExpenseInput input)
        {
            try
            {
                if (input.SplitBetween.Count == 0)
                {
                    throw new InvalidEntityException("The expense must be split between at least one person");
                }

                using (var transaction = await _expenseRepository.BeginTransactionAsync())
                {
                    try
                    {
                        Expense expense;

                        if (input.SplitType == SplitTypeEnum.ByValue)
                        {
                            if (input.SplitValues == null)
                            {
                                throw new InvalidEntityException("The expense must have a value for each person");
                            }

                            await AddEntryWithValueType(input);
                        }
                        else if (input.SplitType == SplitTypeEnum.ByPercentage)
                        {
                            if (input.SplitPercentages == null)
                            {
                                throw new InvalidEntityException("The expense must have a percentage for each person");
                            }

                            await AddEntryWithPercentageType(input);
                        }
                        else
                        {
                            if (input is null || input.SplitBetween.Count == 0)
                            {
                                throw new InvalidEntityException("The expense must be not null and split between number must at least 1 people");
                            }

                            await AddEntryWithEvenlyType(input);
                        }

                        await _expenseRepository.AddExpenseAsync(expense);
                        await _expenseRepository.SaveChangesAsync();
                        transaction.Commit();
                        return expense.Id;
                    }
                    catch (InvalidEntityException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new ContextException("An error occurred while adding the expense to the db", ex);
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
                throw new ContextException("An error happened during the creation of the transaction");
            }
        }
    }
}
