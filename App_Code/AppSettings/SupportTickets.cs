using Prem.PTC.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Windows.Forms;

namespace Prem.PTC
{
    public static partial class AppSettings
    {
        public class SupportTickets
        {
            public static void Save()
            {
                appSettings.SaveSupportTickets();
            }

            public static void Reload()
            {
                if (!createAppSettingsInstanceIfNeeded()) appSettings.SaveSupportTickets();
            }

            public static bool TicketsDisabled
            {
                get { return appSettings.TicketsDisabled; }
                set { appSettings.TicketsDisabled = value; }
            }

            public static bool InsertNameWhenCreatingTicketsEnabled
            {
                get { return appSettings.InsertNameWhenCreatingTicketsEnabled; }
                set { appSettings.InsertNameWhenCreatingTicketsEnabled = value; }
            }

            public static bool InsertPhoneWhenCreatingTicketsEnabled
            {
                get { return appSettings.InsertPhoneWhenCreatingTicketsEnabled; }
                set { appSettings.InsertPhoneWhenCreatingTicketsEnabled = value; }
            }

            public static int MaxSimultaneousOpenUserTickets
            {
                get { return appSettings.MaxSimultaneousOpenUserTickets; }
                set { appSettings.MaxSimultaneousOpenUserTickets = value; }
            }

            public static bool TicketDepartmentsEnabled
            {
                get { return appSettings.TicketDepartmentsEnabled; }
                set { appSettings.TicketDepartmentsEnabled = value; }
            }
        }

        internal partial class AppSettingsTable : BaseTableObject
        {
            [Column("TicketsDisabled")]
            internal bool TicketsDisabled { get { return _ticketsDisabled; } set { _ticketsDisabled = value; SetUpToDateAsFalse(); } }

            [Column("InsertNameWhenCreatingTicketsEnabled")]
            internal bool InsertNameWhenCreatingTicketsEnabled { get { return _insertNameWhenCreatingTicketsEnabled; } set { _insertNameWhenCreatingTicketsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("InsertPhoneWhenCreatingTicketsEnabled")]
            internal bool InsertPhoneWhenCreatingTicketsEnabled { get { return _insertPhoneWhenCreatingTicketsEnabled; } set { _insertPhoneWhenCreatingTicketsEnabled = value; SetUpToDateAsFalse(); } }

            [Column("MaxSimultaneousOpenUserTickets")]
            internal int MaxSimultaneousOpenUserTickets { get { return _MaxSimultaneousOpenUserTickets; } set { _MaxSimultaneousOpenUserTickets = value; SetUpToDateAsFalse(); } }

            [Column("TicketDepartmentsEnabled")]
            internal bool TicketDepartmentsEnabled { get { return _TicketDepartmentsEnabled; } set { _TicketDepartmentsEnabled = value; SetUpToDateAsFalse(); } }

            private bool _ticketsDisabled, _insertNameWhenCreatingTicketsEnabled, _insertPhoneWhenCreatingTicketsEnabled, _TicketDepartmentsEnabled;
            private int _MaxSimultaneousOpenUserTickets;

            internal void ReloadSupportTickets()
            {
                ReloadPartially(IsUpToDate, buildSupportTicketsProperties());
            }

            internal void SaveSupportTickets()
            {
                SavePartially(IsUpToDate, buildSupportTicketsProperties());
            }

            private PropertyInfo[] buildSupportTicketsProperties()
            {
                var supportTicketsValues = new PropertyBuilder<AppSettingsTable>();
                supportTicketsValues
                    .Append(x => x.TicketsDisabled)
                    .Append(x => x.InsertNameWhenCreatingTicketsEnabled)
                    .Append(x => x.InsertPhoneWhenCreatingTicketsEnabled)
                    .Append(x => x.MaxSimultaneousOpenUserTickets)
                    .Append(x => x.TicketDepartmentsEnabled);

                return supportTicketsValues.Build();
            }
        }
    }
}