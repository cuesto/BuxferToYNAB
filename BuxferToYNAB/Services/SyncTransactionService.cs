using Buxfer.Client;
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
        private readonly string _buxferUser, _buxferPassword, _ynabToken, _ynabAPIURL;

        public SyncTransactionService(IConfiguration config)
        {
            _config = config;
            _buxferUser = _config["BuxferUser"];
            _buxferPassword = _config["BuxferPwd"];
            _ynabToken = _config["YNABToken"];
            _ynabAPIURL = "https://api.youneedabudget.com/v1/budgets";
            var accessToken = _ynabToken;

        }

        public async void SyncTransactions()
        {
            // buxfer
            var transactionsBuxfer = GetTransactionsFromBaxferAsync();
            var lastSevenDaysTransactions = transactionsBuxfer.Result.Entities.Where(x => x.Date >= DateTime.Today.AddDays(-8)).ToList().OrderBy(x=>x.Date);

            // ynab
            var budgetList = GetBudgetListYNAB();
            var defaultBudget = budgetList.FirstOrDefault(x=>x.name == "Personal 2022");

            foreach(var transBuxfer in lastSevenDaysTransactions)
            {
                if(!CheckIfTransactionExistInYNAB(transBuxfer, defaultBudget))
                {

                }
            }

        }

        private bool CheckIfTransactionExistInYNAB(Transaction transBuxfer, Models.Budget defaultBudget)
        {
            throw new NotImplementedException();
        }

        private List<Models.Budget> GetBudgetListYNAB()
        {
            var client = new RestClient(_ynabAPIURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + _ynabToken);
            IRestResponse response = client.Execute(request);

            var budgetList = JsonSerializer.Deserialize<BudgetWrapper>(response.Content);

            return budgetList.data.budgets;
        }

        //public async Task<YNAB.SDK.Model.BudgetSummaryResponse> ListBudgets()
        //{
        //    try
        //    {
        //        var budgetsResponse = ynabApi.Budgets.GetBudgets();

        //        var a = budgetsResponse.Data.Budgets.ToList();

        //        return budgetsResponse;
        //        // Won't get here because an error will be thrown
        //    }
        //    catch (YNAB.SDK.Client.ApiException ex)
        //    {
        //        Console.WriteLine(ex.ErrorCode); // 401
        //    }

        //    return null;
        //}

        public async Task<FilterResult<Transaction>> GetTransactionsFromBaxferAsync()
        {
            var client = new BuxferClient(_buxferUser, _buxferPassword);

            var lastTransactions = await client.GetTransactions();
            return lastTransactions;
        }
    }
}
