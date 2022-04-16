﻿using Buxfer.Client;
using BuxferToYNAB.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuxferToYNAB.Services
{
    public class SyncTransactionService
    {
        private readonly IConfiguration _config;
        private readonly string _buxferUser, _buxferPassword, _ynabToken, _ynabAPIURL, _ynabBudgetId,
            _ynabCheckAcctId, _ynabCreditCardAcctId;
        private readonly DateTime _sinceDate;

        public SyncTransactionService(IConfiguration config)
        {
            _config = config;
            _buxferUser = _config["BuxferUser"];
            _buxferPassword = _config["BuxferPwd"];
            _ynabToken = _config["YNABToken"];
            _ynabBudgetId = _config["YNABBudgetId"];
            _ynabCheckAcctId = _config["YNABCheckAcctId"];
            _ynabCreditCardAcctId = _config["YNABCreditCardAcctId"];
            _ynabAPIURL = "https://api.youneedabudget.com/v1/budgets";

            _sinceDate = DateTime.Today.AddDays(-8);
        }

        public async void SyncTransactions()
        {
            // buxfer
            var transactionsBuxfer = GetTransactionsFromBaxferAsync();
            var lastSevenDaysTransactions = transactionsBuxfer.Result.Entities.Where(x => x.Date >= _sinceDate).ToList().OrderBy(x => x.Date);
            var lastSevenDaysTransactionsGroupByAccounts = lastSevenDaysTransactions.GroupBy(x => x.AccountName);

            //// ynab
            //var budgetList = GetBudgetListYNAB();
            //var defaultBudget = budgetList.FirstOrDefault(x=>x.name == "Personal 2022");

            foreach (var transactionsGroupByAccount in lastSevenDaysTransactionsGroupByAccounts)
            {
                var ynabTrasactionsByAccount = GetTransactionsListByAccountYNAB(transactionsGroupByAccount.Key);

                foreach (var transactionBuxfer in transactionsGroupByAccount)
                {
                    if (!CheckIfTransactionExistInYNAB(transactionBuxfer, ynabTrasactionsByAccount))
                    {
                        PostTransactionToYNAB(transactionBuxfer);
                    }
                }
            }
        }

        private void PostTransactionToYNAB(Buxfer.Client.Transaction transactionBuxfer)
        {
            throw new NotImplementedException();
        }

        private bool CheckIfTransactionExistInYNAB(Buxfer.Client.Transaction transactionBuxfer, List<Models.Transaction> ynabTrasactionsByAccount)
        {
            var transactionExist = ynabTrasactionsByAccount
                 .Where(x => x.memo.ToUpper() == transactionBuxfer.Description.ToUpper()
                 && x.date == transactionBuxfer.Date
                 && Math.Abs(x.amount) / 1000 == Math.Abs(transactionBuxfer.Amount)
                 ).Any();

            return transactionExist;
        }

        private List<Models.Transaction> GetTransactionsListByAccountYNAB(string key)
        {
            var accountId = "";
            switch (key)
            {
                case "764393658":
                    accountId = _ynabCheckAcctId;
                    break;
                case "************9095":
                    accountId = _ynabCreditCardAcctId;
                    break;
            }

            var client = new RestClient($"https://api.youneedabudget.com/v1/budgets/{_ynabBudgetId}/accounts/{accountId}/transactions");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {_ynabToken}");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("since_date", _sinceDate.Date.ToString("yyyy-MM-dd"));
            IRestResponse response = client.Execute(request);
            var transactionList = JsonSerializer.Deserialize<TransactionWrapper>(response.Content);

            return transactionList.data.transactions;
        }

        public async Task<FilterResult<Buxfer.Client.Transaction>> GetTransactionsFromBaxferAsync()
        {
            var client = new BuxferClient(_buxferUser, _buxferPassword);

            var lastTransactions = await client.GetTransactions();
            return lastTransactions;
        }
    }
}