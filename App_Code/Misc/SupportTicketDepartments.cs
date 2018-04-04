using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Memberships;
using System;
using System.Data;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for SupportTicketDepartment
/// </summary>
[Serializable]
public class SupportTicketDepartment : BaseTableObject
{
    private string name;
    private int id, status;

    //Default department
    public const int DefaultDepartament = 0;

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "SupportTicketDepartments"; } }

    protected override string dbTable { get { return TableName; } }

    //public static int DepartmentId { get { return GetDepartmentId();}}

    public static class Columns
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Status = "Status";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Name)]
    public string Name { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Status)]
    protected int _Status { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

    public UniversalStatus Status
    {
        get { return (UniversalStatus)_Status; }
        set { _Status = (int)value; }
    }

    public SupportTicketDepartment()
        :base() {}

    public SupportTicketDepartment(int id) : base(id) { }

    public SupportTicketDepartment(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate){ }

    public static List<SupportTicketDepartment> GetAllDepartments()
    {
        return TableHelper.SelectAllRows<SupportTicketDepartment>();
    }

    public static List<SupportTicketDepartment> GetAllAvailableDepartments()
    {
        return TableHelper.SelectRows<SupportTicketDepartment>(TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }

    public static ListItem[] AvailableDepartments
    {
        get
        {
            var query = from SupportTicketDepartment departament in GetAllAvailableDepartments()
                        select new ListItem(departament.Name, departament.Id.ToString());

            return query.ToArray();
        }
    }

    public static ListItem[] AllDepartments
    {
        get
        {
            var query = from SupportTicketDepartment departament in GetAllDepartments()
                        select new ListItem(departament.Name, departament.Id.ToString());

            return query.ToArray();
        }
    }

}