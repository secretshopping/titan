using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration;
using Prem.PTC;
using System.CodeDom;
using System.Web.UI;
using System.ComponentModel;
using System.Web.Compilation;

// Apply ExpressionEditorAttributes to allow the 
// expression to appear in the designer.
[ExpressionPrefix("ConnectionString")]
[ExpressionEditor("ConnectionStringEditor")]
public class ConnectionString : ExpressionBuilder
{
    private static readonly string IsCoAdministratorSessionKey = "CSCoAdministratorSessionKey";
    private static readonly string CoAdministratorUsernameSessionKey = "CSCoAdministratorUsernameSessionKey";

    // Create a method that will return the result 
    // set for the expression argument.
    public static object GetEvalData(string expression, Type target, string entry)
    {
        if (expression == "Client")
            return Client;

        return expression;
    }

    public static string PowerPacks
    {
        get
        {
            return "";
        }
    }

    public override object EvaluateExpression(object target, BoundPropertyEntry entry,
    object parsedData, ExpressionBuilderContext context)
    {
        return GetEvalData(entry.Expression, target.GetType(), entry.Name);
    }

    public override CodeExpression GetCodeExpression(BoundPropertyEntry entry,
    object parsedData, ExpressionBuilderContext context)
    {
        Type type1 = entry.DeclaringType;
        PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(type1)[entry.PropertyInfo.Name];
        CodeExpression[] expressionArray1 = new CodeExpression[3];
        expressionArray1[0] = new CodePrimitiveExpression(entry.Expression.Trim());
        expressionArray1[1] = new CodeTypeOfExpression(type1);
        expressionArray1[2] = new CodePrimitiveExpression(entry.Name);
        return new CodeCastExpression(descriptor1.PropertyType, new CodeMethodInvokeExpression(new
       CodeTypeReferenceExpression(base.GetType()), "GetEvalData", expressionArray1));
    }

    public override bool SupportsEvaluate
    {
        get { return true; }
    }

    public static string Client
    {
        get
        {
            return ConfigurationManager.ConnectionStrings[DatabaseHelper.GetConnectionString(Database.Client)].ConnectionString;
        }
    }

    public static string Forum
    {
        get
        {
            return ConfigurationManager.ConnectionStrings[DatabaseHelper.GetConnectionString(Database.Forum)].ConnectionString;
        }
    }    

    public static string GetConnectionString(Database database)
    {
        switch (database)
        {
            case Database.Client: return Client;
            case Database.Service: return Client;
            case Database.Forum: return Forum;

            default: throw new System.ArgumentException("No such database");
        }
    }
}

/// <summary>
/// Determines the database
/// </summary>
public enum Database
{
    Client, Service, Forum
}


/// <summary>
/// Static helper class for database enum
/// </summary>
internal static class DatabaseHelper
{
    /// <summary>
    /// Returns connection string key for ConfigurationManager.ConnectionStrings collection 
    /// depending on Database type
    /// </summary>
    /// <param name="database"></param>
    /// <returns>Connection string key for ConfigurationManager.ConnectionStrings collection</returns>
    internal static string GetConnectionString(Database database)
    {
        switch (database)
        {
            case Database.Client: return "ClientDbString";
            case Database.Service: return "ClientDbString";
            case Database.Forum: return "yafnet";
            default: throw new System.ArgumentException("No such database");
        }
    }
}