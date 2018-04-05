using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;


public class ResourceHelper
{

    public static List<DictionaryEntry> ReadResourceFile(string path)
    {
        List<DictionaryEntry> resourceEntries = new List<DictionaryEntry>();

        //Get existing resources
        ResXResourceReader reader = new ResXResourceReader(path);
        reader.UseResXDataNodes = true;
        System.ComponentModel.Design.ITypeResolutionService typeres = null;
        if (reader != null)
        {
            IDictionaryEnumerator id = reader.GetEnumerator();
            DictionaryEntry emptyValue;
            emptyValue.Value = "";

            foreach (DictionaryEntry d in reader)
            {
                //Read from file:
                if (d.Value == null)
                {
                    emptyValue.Key = d.Key;
                    resourceEntries.Add(emptyValue);
                }
                else
                    resourceEntries.Add(d);
            }
            reader.Close();
        }

        return resourceEntries;
    }
    
    public static void AddOrUpdateResource(DictionaryEntry newElement, string resourceFilepath)
    {
        var resx = new List<DictionaryEntry>();
        using (var reader = new ResXResourceReader(resourceFilepath))
        {
            resx = reader.Cast<DictionaryEntry>().ToList();
            var existingResource = resx.Where(r => r.Key == newElement.Key).FirstOrDefault();
            if (existingResource.Key == null && existingResource.Value == null) // NEW!
            {
                resx.Add(newElement);
            }
            else // MODIFIED RESOURCE!
            {
                var modifiedResx = new DictionaryEntry()
                { Key = existingResource.Key, Value = newElement.Value };
                resx.Remove(existingResource);  // REMOVING RESOURCE!
                resx.Add(modifiedResx);  // AND THEN ADDING RESOURCE!
            }
        }
        using (var writer = new ResXResourceWriter(resourceFilepath))
        {
            resx.ForEach(r =>
            {
                // Again Adding all resource to generate with final items
                writer.AddResource(r.Key.ToString(), r.Value.ToString());
            });
            writer.Generate();
            writer.Close();
        }
    }

}