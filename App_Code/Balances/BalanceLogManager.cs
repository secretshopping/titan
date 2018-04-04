using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ExtensionMethods;
using Prem.PTC.Members;
using System.Data;
using MarchewkaOne.Titan.Balances;

public class BalanceLogManager
{
    public static void FastAddLogs(List<KeyValuePair<int, Money>> listIdsDifference, BalanceType type, string note, BalanceLogType balanceLogType)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //Executing in 800 batches for good performance
            DateTime now = AppSettings.ServerTime;
            bool Active = true;
            int j = 0;

            while (Active)
            {
                bool AreRecordsInQuery = false;

                //Batch start
                var Query = ConstructInsertQueryStart();
                int i = 0;

                while (j < listIdsDifference.Count && i < 800)
                {
                    AreRecordsInQuery = true;

                    ConstructInsertQuery(ref Query, listIdsDifference[j].Key, now, note, type, listIdsDifference[j].Value, balanceLogType);

                    i++;
                    j++;
                }

                if (AreRecordsInQuery)
                {
                    if (Query[Query.Length - 1] == ',')
                        Query[Query.Length - 1] = ';';

                    bridge.Instance.ExecuteRawCommandNonQuery(Query.ToString());
                }
                else
                    Active = false;
            }
        }
    }
    protected static void ConstructInsertQuery(ref StringBuilder sb, int userId, DateTime date, string note, BalanceType type, Money amount, BalanceLogType balanceLogType)
    {
        sb.Append(" (")
          .Append(userId)
          .Append(", '")
          .Append(date.ToDBString())
          .Append("', '")
          .Append(note)
          .Append("', ")
          .Append(amount.ToClearString())
          .Append(", ")
          .Append((int)type)
          .Append(", ")
          .Append((int)balanceLogType)
          .Append(")");

        sb.Append(",");
    }

    #region Merged from TRAFFICUNITY
    //MERGED FROM TRAFFICUNITY
    public static void FastAddLogs(List<KeyValuePair<int, Money>> listIdsDifference, BalanceType type, string note, List<KeyValuePair<int, Money>> ActualBalances, BalanceLogType balanceLogType)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //Executing in 800 batches for good performance
            DateTime now = AppSettings.ServerTime;
            bool Active = true;
            int j = 0;

            while (Active)
            {
                bool AreRecordsInQuery = false;

                //Batch start
                var Query = ConstructInsertQueryStart();
                int i = 0;

                while (j < listIdsDifference.Count && i < 800)
                {
                    AreRecordsInQuery = true;

                    ConstructInsertQuery(ref Query, listIdsDifference[j].Key, now, note, type, listIdsDifference[j].Value, ActualBalances[j].Value, balanceLogType);

                    i++;
                    j++;
                }

                if (AreRecordsInQuery)
                {
                    if (Query[Query.Length - 1] == ',')
                        Query[Query.Length - 1] = ';';

                    bridge.Instance.ExecuteRawCommandNonQuery(Query.ToString());
                }
                else
                    Active = false;
            }
        }
    }

    protected static void ConstructInsertQuery(ref StringBuilder sb, int userId, DateTime date, string note, BalanceType type, Money amount, Money accountState, BalanceLogType balanceLogType)
    {
        sb.Append(" (")
          .Append(userId)
          .Append(", '")
          .Append(date.ToDBString())
          .Append("', '")
          .Append(note)
          .Append("', ")
          .Append(amount.ToClearString())
          .Append(", ")
          .Append((int)type)
          .Append(", ")
          .Append(accountState.ToClearString())
          .Append(", ")
          .Append((int)balanceLogType)
          .Append(")");

        sb.Append(",");
    }
    public static void GlobalMemberAdjustHelper(Parser parser, string selectCommand, string updateCommand, string whereCommand, string note, Money amount, BalanceLogType balanceLogType)
    {
        //Add logs FAST
        var InactivityUsersDataTable = parser.ExecuteRawCommandToDataTable(selectCommand + whereCommand);
        var InactivityUsers = TableHelper.GetListFromDataTable<Member>(InactivityUsersDataTable, 100, true);
        var List = new List<KeyValuePair<int, Money>>();
        var ListActuaiBalance = new List<KeyValuePair<int, Money>>();

        foreach (var member in InactivityUsers)
        {
            List.Add(new KeyValuePair<int, Money>(member.Id, amount * -1));
            ListActuaiBalance.Add(new KeyValuePair<int, Money>(member.Id, member.MainBalance - amount));
        }

        BalanceLogManager.FastAddLogs(List, BalanceType.MainBalance, note, ListActuaiBalance, balanceLogType);

        //Update all
        parser.ExecuteRawCommandNonQuery(updateCommand + whereCommand);
    }
    protected static StringBuilder ConstructInsertQueryStart()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"INSERT INTO BalanceLogs (UserId, DateOccured, Note, Amount, Balance, AccountState, BalanceLogType) VALUES");
        return sb;
    }
    #endregion

    public static void AddRange(string dataTableCommand, string note, BalanceType target, BalanceLogType balanceLogType)
    {
        using (var parser = ParserPool.Acquire(Database.Client))
        {
            AddRange(parser.Instance, dataTableCommand, note, target, balanceLogType);
        }
    }

    public static void AddRange(Parser parser, string dataTableCommand, string note, BalanceType target, BalanceLogType balanceLogType)
    {
        //You need to provide DataTable with the following variables (User Ids must be unique):
        //userId => UserId
        //amount => money that will be added to the balance
        //state => current account state (AFTER the operation)

        var List = new List<KeyValuePair<int, Money>>();
        var ListActualBalance = new List<KeyValuePair<int, Money>>();

        var dt = parser.ExecuteRawCommandToDataTable(dataTableCommand);

        foreach (DataRow row in dt.Rows)
        {
            if (new Money((Decimal)row["amount"]) != Money.Zero)
            {
                List.Add(new KeyValuePair<int, Money>((int)row["userId"], new Money((Decimal)row["amount"])));
                ListActualBalance.Add(new KeyValuePair<int, Money>((int)row["userId"], new Money((Decimal)row["state"])));
            }
        }

        BalanceLogManager.FastAddLogs(List, target, note, ListActualBalance, balanceLogType);
    }




}