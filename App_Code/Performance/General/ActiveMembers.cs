using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ActiveMembers
{
    public static List<Member> List
    {
        get
        {
            List<Member> result = new List<Member>();

            //Process in 8k batches
            int batchSize = 3000;
            int start = 0;
            int end = 3000;

            //TEMP
            int listOfRows = (int)TableHelper.SelectScalar(@"
                SELECT COUNT(*) FROM Users WHERE AccountStatusInt = 1");

            while (start < listOfRows)
            {
                var batch = TableHelper.GetListFromRawQuery<Member>(String.Format(@"
                            SELECT * FROM (
                                SELECT u.*, ROW_NUMBER() OVER (ORDER BY UserId) rownum
                                FROM  Users as u WHERE AccountStatusInt = 1
                            ) seq
                            WHERE seq.rownum BETWEEN {0} AND {1}", start, end));

                start = end + 1;
                end = end + batchSize;

                result.AddRange(batch);
            }

            return result;
        }
    }
}