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
[ExpressionPrefix("ResourceLookup")]
[ExpressionEditor("ResourceLookupEditor")]
public class ResourceLookup : ExpressionBuilder
{
    // Create a method that will return the result 
    // set for the expression argument.
    public static object GetEvalData(string expression, Type target, string entry)
    {
        object obj = Resources.L1.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U4000.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U4200.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U5001.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U5004.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U5006.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U5007.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U5009.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6000.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6006.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6008.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6010.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6012.ResourceManager.GetObject(expression);
        if (obj == null)
            obj = Resources.U6013.ResourceManager.GetObject(expression);
        return obj;
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

}
