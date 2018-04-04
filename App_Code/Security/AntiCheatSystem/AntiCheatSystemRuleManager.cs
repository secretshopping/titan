using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AntiCheatSystemRuleManager
{
    List<AntiCheatSystemRule> list;

    public AntiCheatSystemRuleManager()
    {
        list = TableHelper.SelectAllRows<AntiCheatSystemRule>();
    }

    public void CheckRuleEnabled(AntiCheatRuleType type)
    {
        if (CheckIfRuleEnabled(type) == false)
            return;
    }

    public bool CheckIfRuleEnabled(AntiCheatRuleType type)
    {
        bool Enabled = false;

        foreach (var rule in list)
            if (rule.Type == type)
                Enabled = rule.Enabled;

        return Enabled;
    }

    public string GetRuleText(AntiCheatRuleType type)
    {
        string text = String.Empty;

        foreach (var rule in list)
            if (rule.Type == type)
                text = rule.RuleText;

        return text;
    }

}