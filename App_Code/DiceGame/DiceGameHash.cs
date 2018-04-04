using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using ExtensionMethods;


public class DiceGameHash : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "DiceGameHashes"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string UserId = "UserId";
        public const string Id = "Id";
        public const string ClientSeedCurrent = "ClientSeedCurrent";
        public const string ClientSeedPrevious = "ClientSeedPrevious";
        public const string ServerSeedCurrent = "ServerSeedCurrent";
        public const string ServerSeedPrevious = "ServerSeedPrevious";
        public const string ServerHashCurrent = "ServerHashCurrent";
        public const string ServerHashPrevious = "ServerHashPrevious";
        public const string CreatedDateCurrent = "CreatedDateCurrent";
        public const string CreatedDatePrevious = "CreatedDatePrevious";
    }

    

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; }  set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ClientSeedCurrent)]
    public String ClientSeedCurrent { get { return _ClientSeedCurrent; }  set { _ClientSeedCurrent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ClientSeedPrevious)]
    public String ClientSeedPrevious { get { return _ClientSeedPrevious; }  set { _ClientSeedPrevious = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ServerSeedCurrent)]
    public String ServerSeedCurrent { get { return _ServerSeedCurrent; }  set { _ServerSeedCurrent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ServerSeedPrevious)]
    public String ServerSeedPrevious { get { return _ServerSeedPrevious; }  set { _ServerSeedPrevious = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ServerHashCurrent)]
    public String ServerHashCurrent { get { return _ServerHashCurrent; }  set { _ServerHashCurrent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ServerHashPrevious)]
    public String ServerHashPrevious { get { return _ServerHashPrevious; }  set { _ServerHashPrevious = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatedDateCurrent)]
    public DateTime CreatedDateCurrent { get { return _CreatedDateCurrent; }  set { _CreatedDateCurrent = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatedDatePrevious)]
    public DateTime CreatedDatePrevious { get { return _CreatedDatePrevious; }  set { _CreatedDatePrevious = value; SetUpToDateAsFalse(); } }



    private int _id;
    private string _ClientSeedCurrent, _ClientSeedPrevious, _ServerSeedCurrent, _ServerSeedPrevious, _ServerHashCurrent, _ServerHashPrevious;
    private DateTime _CreatedDateCurrent, _CreatedDatePrevious;
    private int _UserId;
    private Member _user;
    public Member User
    {
        get
        {
            if (_user == null)
                _user = new Member(UserId);
            return _user;
        }
        set
        {
            _user = value;
            UserId = value.Id;
        }
    }

    public DiceGameHash()
            : base() { }

    public DiceGameHash(int id) : base(id) { }

    public DiceGameHash(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    /// <summary>
    /// Generates new Server Seed and Hash
    /// </summary>
    public void GenerateServerSeedAndHash()
    {
        ArchiveServerSeedAndHash();

        DateTime currentDate = DateTime.Now;
        ServerSeedCurrent = DiceGameHashLogic.GenerateServerSeed(currentDate, User.Name, AppSettings.Offerwalls.UniversalHandlerPassword);
        ServerHashCurrent = DiceGameHashLogic.GenerateServerHash(ServerSeedCurrent);
        
    }
    /// <summary>
    /// Generates first set of hashes
    /// </summary>
    private void GenerateFirstHashes()
    {
        CreatedDateCurrent = DateTime.Now;
        CreatedDatePrevious = DateTime.Now.Zero();
        DateTime currentDate = DateTime.Now;
        ServerSeedCurrent = DiceGameHashLogic.GenerateServerSeed(currentDate, User.Name, AppSettings.Offerwalls.UniversalHandlerPassword);
        ServerHashCurrent = DiceGameHashLogic.GenerateServerHash(ServerSeedCurrent);
        ClientSeedCurrent = DiceGameHashLogic.GenerateClientSeed();
    }
    /// <summary>
    /// Gets Seeds and Hashes from the DB or creates new if not present
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static DiceGameHash Get(Member user)
    {

        var diceGameHashes = TableHelper.SelectRows<DiceGameHash>(TableHelper.MakeDictionary("UserId", user.Id));
        if (diceGameHashes.Count == 0)
        {
            DiceGameHash hash = new DiceGameHash();
            hash.UserId = user.Id;
            hash.GenerateFirstHashes();
            hash.Save();
            return hash;
        }
        return diceGameHashes[0];
    }
    /// <summary>
    /// Moves Current Seed and Hash to Previous
    /// </summary>
    public void ArchiveServerSeedAndHash()
    {
        if (!String.IsNullOrEmpty(ServerSeedCurrent) && !String.IsNullOrEmpty(ServerHashCurrent))
        {
            //Archiving current data
            ServerSeedPrevious = ServerSeedCurrent;
            ServerHashPrevious = ServerHashCurrent;
        }
    }
    /// <summary>
    /// Updates Client Seed data in the DB
    /// </summary>
    /// <param name="numberOfBets"></param>
    public void UpdateClientSeed(string clientSeed)
    {
        ClientSeedPrevious = ClientSeedCurrent;
        ClientSeedCurrent = clientSeed;
        CreatedDatePrevious = CreatedDateCurrent;
        CreatedDateCurrent = DateTime.Now;
    }


}