using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MeetupMeetingManagement.Models
{
    public class Membership
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum State { None, Current, Due, Expired };

        public Membership()
        {
            this.Status = State.None;
        }

        public State Status { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public DateTime TransactionDate { get; private set; }

        internal static Membership Create(IDictionary<string, string> membershipDues)
        {
            var today = DateTime.UtcNow.Date;
            var membership = new Membership();
            var transactionTime = "0";
            if(membershipDues.TryGetValue("transaction_time", out transactionTime))
            {
                double time;
                if(double.TryParse(transactionTime, out time))
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var transaction = epoch.AddMilliseconds(time);

                    membership.TransactionDate = transaction.Date;
                    membership.ExpirationDate = transaction.AddYears(1).Date;
                    if (membership.ExpirationDate.Date < today)
                    {
                        membership.Status = State.Expired;
                    }
                    else if (membership.ExpirationDate.Date.AddDays(-30) <= today)
                    {
                        membership.Status = State.Due;
                    }
                    else
                    {
                        membership.Status = State.Current;
                    }
                }
            }

            return membership;
        }
    }
}