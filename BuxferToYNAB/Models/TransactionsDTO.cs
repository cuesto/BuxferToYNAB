using Buxfer.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BuxferToYNAB.Models
{
    [DebuggerDisplay("memo:{memo}; date:{date}; amount:{amount}; acct:{account_id}")]
    public class TransactionDTO
    {
        public string account_id { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
        public string payee_name { get; set; }
        public string memo { get; set; }
        public string flag_color { get; set; }
        public string import_id { get; set; }

        [JsonIgnore]
        public TransactionType Type { get; set; }
        [JsonIgnore]
        public decimal amountRaw { get; set; }
    }

    public class TransactionsDTO
    {
        public TransactionsDTO()
        {
            transactions = new List<TransactionDTO>();
        }

        public List<TransactionDTO> transactions { get; set; }
    }

}
