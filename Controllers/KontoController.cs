using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountSortingController : ControllerBase
    {
        private static List<BankAccount> _bankAccounts = GenerateRandomBankAccounts(10);  // Example list of accounts

        [HttpGet]
        public IActionResult GetBankAccountSortingData(int iterations = 10, int maxDatasetSize = 5000)
        {
            var selectionSortTimes = new List<double>();
            var mergeSortTimes = new List<double>();
            var datasetSizes = new List<int>();

            for (int size = 0; size <= maxDatasetSize; size += 1000)
            {
                var bankAccounts = GenerateRandomBankAccounts(size);

                // Measure SelectionSort execution time
                var stopwatch = Stopwatch.StartNew();
                SelectionSort(bankAccounts.ToList());
                stopwatch.Stop();
                selectionSortTimes.Add(stopwatch.ElapsedMilliseconds);

                // Measure MergeSort execution time
                stopwatch.Restart();
                MergeSort(bankAccounts.ToList());
                stopwatch.Stop();
                mergeSortTimes.Add(stopwatch.ElapsedMilliseconds);

                datasetSizes.Add(size);
            }

            return Ok(new { SelectionSortTimes = selectionSortTimes, MergeSortTimes = mergeSortTimes, DatasetSizes = datasetSizes });
        }

        [HttpPost("deposit")]
        public IActionResult Deposit(string accountName, decimal amount)
        {
            var account = _bankAccounts.FirstOrDefault(a => a.AccountName == accountName);
            if (account == null)
            {
                return NotFound(new { Message = "Account not found", AccountName = accountName });
            }

            account.Deposit(amount);
            return Ok(new { Message = "Deposit successful", AccountName = accountName, AmountDeposited = amount, NewBalance = account.Balance });
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw(string accountName, decimal amount)
        {
            var account = _bankAccounts.FirstOrDefault(a => a.AccountName == accountName);
            if (account == null)
            {
                return NotFound(new { Message = "Account not found", AccountName = accountName });
            }

            var success = account.Withdraw(amount);
            if (success)
            {
                return Ok(new { Message = "Withdrawal successful", AccountName = accountName, AmountWithdrawn = amount, NewBalance = account.Balance });
            }
            else
            {
                return BadRequest(new { Message = "Insufficient funds", AccountName = accountName, RequestedAmount = amount, CurrentBalance = account.Balance });
            }
        }

        // Method to generate random BankAccounts
        public static List<BankAccount> GenerateRandomBankAccounts(int count)
        {
            var random = new Random();
            return Enumerable.Range(0, count)
                .Select(i => new BankAccount($"Account{i}", random.Next(100, 10000)))
                .ToList();
        }

        // Selection Sort Implementation
        public static List<BankAccount> SelectionSort(List<BankAccount> accounts)
        {
            int n = accounts.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (accounts[j].CompareTo(accounts[minIndex]) < 0)
                    {
                        minIndex = j;
                    }
                }
                (accounts[i], accounts[minIndex]) = (accounts[minIndex], accounts[i]);
            }
            return accounts;
        }

        // Merge Sort Implementation
        public static List<BankAccount> MergeSort(List<BankAccount> accounts)
        {
            if (accounts.Count <= 1)
                return accounts;

            int middle = accounts.Count / 2;
            var left = accounts.Take(middle).ToList();
            var right = accounts.Skip(middle).ToList();

            return Merge(MergeSort(left), MergeSort(right));
        }

        private static List<BankAccount> Merge(List<BankAccount> left, List<BankAccount> right)
        {
            var result = new List<BankAccount>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (left[i].CompareTo(right[j]) <= 0)
                {
                    result.Add(left[i++]);
                }
                else
                {
                    result.Add(right[j++]);
                }
            }

            result.AddRange(left.Skip(i));
            result.AddRange(right.Skip(j));

            return result;
        }

        // BankAccount class implementing IComparable
        public class BankAccount : IComparable<BankAccount>
        {
            public string AccountName { get; set; }
            public decimal Balance { get; set; }

            public BankAccount(string accountName, decimal initialBalance)
            {
                AccountName = accountName;
                Balance = initialBalance;
            }

            // Method to deposit money into the account
            public void Deposit(decimal amount)
            {
                Balance += amount;
            }

            // Method to withdraw money from the account
            public bool Withdraw(decimal amount)
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    return true;
                }
                return false;
            }

            // IComparable implementation to compare BankAccount based on Balance
            public int CompareTo(BankAccount other)
            {
                if (other == null) return 1;
                return Balance.CompareTo(other.Balance);
            }
        }
    }
}
