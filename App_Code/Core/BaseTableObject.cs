using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Prem.PTC
{
    public delegate void PreDeleteEventHandler(object sender, EventArgs e);
    public delegate void PreReloadEventHandler(object sender, EventArgs e);
    public delegate void PreSaveEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Base class for all things that are kept in database table
    /// </summary>
    [Serializable]
    public abstract class BaseTableObject : ITableObject
    {
        public abstract Database Database { get; }

        /// <summary>
        /// Returns table name associated with things stored in this class
        /// </summary>
        public static string TableName { get { return string.Empty; } }

        /// <summary>
        /// Every class must provide non-static getter to TableName, 
        /// because static properties don't work with inheritance
        /// (which is used every time when base table object
        /// connects with database: in constructor, Save(), Reload(), ...).
        /// Therefore every subclass must implement instance property dbTable
        /// to make inheritance work
        /// i know this sucks...
        /// </summary>
        protected abstract string dbTable { get; }

        [Column("IdName", IsPrimaryKey = true)]
        public abstract int Id { get; protected set; }

        string _idName;
        protected string IdName
        {
            get
            {
                if (String.IsNullOrEmpty(_idName)) _idName = Column.Get(getProperty(GetType(), "Id")).Name;
                return _idName;
            }
        }

        private PropertyInfo[] Properties
        {
            get
            {
                return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        /// <summary>
        /// Id number which indicates that object is not in database (-1 by default)
        /// </summary>
        protected const int NotInDatabaseId = -1;

        /// <summary>
        /// True if things stored in this object are in database
        /// but don't have to be up to date
        /// </summary>
        public bool IsInDatabase { get { return Id != NotInDatabaseId; } }

        /// <summary>
        /// True if things stored in this object are in database and are saved
        /// False if at least one variable differs from table collumn
        /// </summary>
        public virtual bool IsUpToDate { get; private set; }

        protected void SetUpToDateAsFalse() { IsUpToDate = false; }

        // dictionary containing all columns except primary
        private Dictionary<string, object> _cachedQueryDictionary = null;

        /// <summary>
        /// Get dictionary with all columns and values set from this 
        /// instance ready to use in queries.
        /// Creates dictionary with all columns except primary key.
        /// </summary>
        private Dictionary<string, object> getQueryDictionary(bool forceSave)
        {

            if (!this.IsUpToDate || _cachedQueryDictionary == null || forceSave)
            {
                _cachedQueryDictionary = new Dictionary<string, object>();

                foreach (var property in this.Properties)
                    foreach (var attr in property.GetCustomAttributes(typeof(Column), true))
                        if (attr is Column)
                        {
                            var column = attr as Column;
                            if (!(IsInDatabase && column.IgnoreWhileUpdating))
                                AddColumnPropertyToDictionary(property, column);
                        }
            }

            return _cachedQueryDictionary;

        }

        private void AddColumnPropertyToDictionary(PropertyInfo property, Column column)
        {
            if (!column.IsPrimaryKey)
            {
                object value = property.GetValue(this, null);
                
                if (value != null) //Comment this line to save null to database
                    _cachedQueryDictionary[column.Name] = value;

                WriteColumnPropertyDebugInfo(column, value);
            }
        }

        private void WriteColumnPropertyDebugInfo(Column column, object value)
        {
            //string message = "Name: " + column.Name + ", Type: ";
            //message += (value != null) ? value.GetType().ToString() : "null";
            //System.Diagnostics.Debug.WriteLine(message);
        }

        // dictionary containing Id name and value
        private Dictionary<string, object> _cachedIdDictionary = null;

        /// <summary>
        /// Get dictionary with primary key
        /// </summary>
        protected Dictionary<string, object> IdDictionary
        {
            get
            {
                if (_cachedIdDictionary == null)
                {
                    _cachedIdDictionary = new Dictionary<string, object>(1);
                    _cachedIdDictionary[IdName] = Id;
                }
                return _cachedIdDictionary;
            }
        }

        public event PreDeleteEventHandler PreDelete;
        public event PreReloadEventHandler PreReload;
        public event PreSaveEventHandler PreSave;

        protected virtual void OnPreDelete(EventArgs e)
        {
            if (PreDelete != null) PreDelete(this, e);
        }

        protected virtual void OnPreReload(EventArgs e)
        {
            if (PreReload != null) PreReload(this, e);
        }

        protected virtual void OnPreSave(EventArgs e)
        {
            if (PreSave != null) PreSave(this, e);
        }


        /// <summary>
        /// Selects values from database with apropriate rowId
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DbException" />
        protected BaseTableObject(int id)
        {
            using (var bridge = ParserPool.Acquire(Database))
            {
                Id = id;
                DataRow row;
                row = bridge.Instance.Select(dbTable, IdDictionary).Rows[0];

                SetColumnsFromDataRow(row, true);
            }
        }

        /// <summary>
        /// Creates new instance basing on delivered DataRow
        /// </summary>
        /// <param name="row">Row of db table :)</param>
        /// <remarks>Maybe this constructor should be deleted</remarks>
        public BaseTableObject(DataRow row, bool isUpToDate = true)
        {
            if (row.Table.Columns.Contains(IdName))
                Id = row.Field<int>(IdName);
            else
                Id = NotInDatabaseId;

            SetColumnsFromDataRow(row, isUpToDate);
        }

        /// <summary>
        /// Creates blank instance of class
        /// </summary>
        protected BaseTableObject()
        {
            Id = NotInDatabaseId;
            IsUpToDate = false;
        }


        /// <summary>
        /// Saves current instance in database. <br />
        /// Performs: Insert (when object is not already in database),
        /// Update (when is already in database and !IsUpToDate),
        /// or nothing (when IsUpToDate);
        /// </summary>
        /// <param name="forceSave">if true, saves even if object IsUpToDate</param>
        /// <exception cref="DbException" />
        public virtual void Save(bool forceSave = false)
        {
            OnPreSave(EventArgs.Empty);

            if (IsInDatabase && IsUpToDate && !forceSave) return;

            using (var bridge = ParserPool.Acquire(Database))
            {
                if (!IsInDatabase)
                {
                    bridge.Instance.Insert(dbTable, getQueryDictionary(forceSave));
                    getIdJustAfterInserting(bridge.Instance);
                }
                else if (IsInDatabase && (!IsUpToDate || forceSave))
                    bridge.Instance.Update(dbTable, getQueryDictionary(forceSave), IdDictionary);

                IsUpToDate = true;
            }
        }

        private void getIdJustAfterInserting(Parser parser)
        {
            Id = (int)parser.Select(dbTable, "MAX(" + IdName + ")", null);
        }

        /// <summary>
        /// Saves part of object in database specified by columnPropertiesToSave. 
        /// Can be used only when object IsInDatabase (otherwise throws an exception).
        /// If !IsInDatabase Use Save() instead.
        /// </summary>
        /// <param name="isUpToDate">You must explicitly set whether after 
        /// partial save all columns are up to date, if you are not positive set false</param>
        /// <param name="columnPropertiesToSave"></param>
        protected void SavePartially(bool isUpToDate, params string[] columnPropertiesToSave)
        {
            if (!IsInDatabase) throw new InvalidOperationException("Object must be saved in datbase. Use Save()");
            if (IsInDatabase && !IsUpToDate)
            {
                using (var bridge = ParserPool.Acquire(Database))
                {
                    bridge.Instance.Update(dbTable, GetColumnsDictionary(columnPropertiesToSave), IdDictionary);
                    IsUpToDate = isUpToDate;
                }
            }
        }

        /// <summary>
        /// Saves part of object in database specified by columnPropertiesToSave. 
        /// Can be used only when object IsInDatabase (otherwise throws an exception).
        /// If !IsInDatabase Use Save() instead.
        /// </summary>
        /// <param name="isUpToDate">You must explicitly set whether after 
        /// partial save all columns are up to date, if you are not positive set false</param>
        /// <param name="columnPropertiesToSave"></param>
        protected void SavePartially(bool isUpToDate, params PropertyInfo[] columnPropertiesToSave)
        {
            if (!IsInDatabase) throw new InvalidOperationException("Object must be saved in datbase. Use Save()");
            if (IsInDatabase && !IsUpToDate)
            {
                using (var bridge = ParserPool.Acquire(Database))
                {
                    bridge.Instance.Update(dbTable, GetColumnsDictionary(columnPropertiesToSave), IdDictionary);
                    IsUpToDate = isUpToDate;
                }
            }
        }

        /// <summary>
        /// Removes current instance of class from database
        /// but doesn't affect this object
        /// </summary>
        /// <exception cref="DbException"/>
        public virtual void Delete()
        {
            OnPreDelete(EventArgs.Empty);

            if (IsInDatabase)
            {
                using (var bridge = ParserPool.Acquire(Database))
                {
                    bridge.Instance.Delete(dbTable, IdDictionary);
                    Id = NotInDatabaseId;
                }
            }

            IsUpToDate = false;
        }

        /// <summary>
        /// Downloads object from database and sets its contents with downloaded values.
        /// </summary>
        /// <exception cref="DbException" />
        public virtual void Reload()
        {
            OnPreReload(EventArgs.Empty);

            if (IsInDatabase)
            {
                using (var bridge = ParserPool.Acquire(Database))
                {
                    DataRow objectRow = bridge.Instance.Select(dbTable, IdDictionary).Rows[0];
                    SetColumnsFromDataRow(objectRow, IsUpToDate);

                    IsUpToDate = true;
                }
            }
            else throw new InvalidOperationException("Object not in database");
        }

        /// <exception cref="DbException" />
        protected void ReloadPartially(bool isUpToDate, params string[] columnPropertiesToReload)
        {
            reloadPartially(isUpToDate, Parser.Columns(columnPropertiesToReload));
        }

        /// <exception cref="DbException" />
        protected void ReloadPartially(bool isUpToDate, params PropertyInfo[] columnPropertiesToReload)
        {
            StringCollection columns = new StringCollection();
            foreach (var property in columnPropertiesToReload)
            {
                columns.Add(Column.Get(property).Name);
            }

            reloadPartially(isUpToDate, columns);
        }

        private void reloadPartially(bool isUpToDate, StringCollection columnPropertiesToReload)
        {
            if (IsInDatabase)
            {
                using (var bridge = ParserPool.Acquire(Database))
                {
                    DataRow objectRow = bridge.Instance.Select(dbTable, columnPropertiesToReload, IdDictionary).Rows[0];
                    SetColumnsFromDataRow(objectRow, isUpToDate);
                }
            }
            else throw new InvalidOperationException("Object not in database");
        }

        /// <summary>
        /// Fills class properties and variables basing on provided DataRow
        /// </summary>
        /// <param name="row"></param>
        protected void SetColumnsFromDataRow(DataRow row, bool isUpToDate)
        {
            MethodInfo dataRowFieldMethod = typeof(DataRowExtensions).GetMethod("Field", new Type[] { typeof(DataRow), typeof(String) });

            foreach (PropertyInfo property in this.Properties)
                foreach (var attr in property.GetCustomAttributes(typeof(Column), true))
                {
                    if (attr is Column)
                        SetColumnValue(row, dataRowFieldMethod, property, attr as Column);
                }

            IsUpToDate = isUpToDate;
        }

        private void SetColumnValue(DataRow row, MethodInfo method, PropertyInfo property, Column column)
        {
            if (row.Table.Columns.Contains(column.Name))
            {
                MethodInfo field = getConcreteFieldMethod(method, property);

                object columnValue = field.Invoke(null, new object[] { row, column.Name });

                // Special cast is needed when decimal (Money) is retrieved because we store money in class as Prem.PTC.Money, not as decimal
                if (columnValue is decimal)
                {
                    try
                    {
                        property.SetValue(this, (Money)((decimal)columnValue), null);
                    }
                    catch (ArgumentException)
                    {
                        try
                        {
                            //We default to BTC, to skip FormatException being thrown
                            //It's OK. We later update to proper CryptocurrencyType
                            property.SetValue(this, new CryptocurrencyMoney(Titan.Cryptocurrencies.CryptocurrencyType.BTC, ((decimal)columnValue)), null);
                        }
                        catch (ArgumentException)
                        {
                            property.SetValue(this, (Decimal)columnValue, null);
                        }
                    }
                }
                else
                    property.SetValue(this, columnValue, null);
            }
        }

        /// <summary>
        /// Constructs ready-to-invoke DataRow.Field<>() reflecton method
        /// </summary>
        private MethodInfo getConcreteFieldMethod(MethodInfo generalMethod, PropertyInfo property)
        {
            // money is stored in db as Decimal therefore we must provide DataRow.Field<Decimal> method instead of DataRow.Field<Money>
            if (property.PropertyType == typeof(CryptocurrencyMoney))
                return generalMethod.MakeGenericMethod(typeof(Decimal?));
            else if (property.PropertyType == typeof(Money))
                return generalMethod.MakeGenericMethod(typeof(Decimal?));
            else if (property.PropertyType == typeof(Points))
                return generalMethod.MakeGenericMethod(typeof(Decimal?));
            else
                return generalMethod.MakeGenericMethod(property.PropertyType);
        }


        /// <summary>
        /// Get dictionary with derived columns and values set ready to use in queries.
        /// </summary>
        /// <param name="columnProperties"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetColumnsDictionary(params string[] columnProperties)
        {
            var dictionary = new Dictionary<string, object>(columnProperties.Length);
            Type classType = this.GetType();

            foreach (string propertyName in columnProperties)
            {
                var property = getProperty(classType, propertyName);
                Column column = Column.Get(property);
                dictionary[column.Name] = property.GetValue(this, null);
            }

            return dictionary;
        }

        /// <summary>
        /// Get dictionary with derived columns and values set ready to use in queries.
        /// </summary>
        /// <param name="columnProperties"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetColumnsDictionary(params PropertyInfo[] columnProperties)
        {
            var dictionary = new Dictionary<string, object>(columnProperties.Length);

            foreach (var property in columnProperties)
            {
                Column column = Column.Get(property);
                dictionary[column.Name] = property.GetValue(this, null);
            }

            return dictionary;
        }

        /// <summary>
        /// Call it whenever you want to get name of your class member.
        /// For example: Name(x => x.Id) returns "Id".
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected static string ElName(Expression<Func<object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body == null)
                throw new ArgumentException("Invalid property expression", "expr");

            return body.Member.Name;
        }

        private PropertyInfo getProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }

    #region ColumnAttributeDefinition
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = true)]
    class Column : System.Attribute
    {
        public string Name { get; private set; }
        public bool IsPrimaryKey { get; set; }
        public bool IgnoreWhileUpdating { get; set; }

        public Column(string name, bool isPrimaryKey = false, bool ignoreWhileUpdating = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            IgnoreWhileUpdating = ignoreWhileUpdating;
        }

        public static Column Get(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes(true);
            foreach (object attr in attrs)
                if (attr is Column) return (attr as Column);

            throw new ArgumentException("Property's not a column.");
        }

        private static bool typeIsObject(Type type) { return type.BaseType == null; }
    }
    #endregion
}