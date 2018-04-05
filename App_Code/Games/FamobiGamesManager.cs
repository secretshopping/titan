using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FamobiGamesManager
{
    public static FamobiGameFromApi GetGameInformation(string name)
    {
        using (WebClient client = new MyWebClient())
        {
            JObject famobiJsonApi = JObject.Parse(client.DownloadString("http://api.famobi.com/feed#_ga=1.71573971.1347937337.1454317072"));

            //throw new Exception(famobiJsonApi["games"][0][]);
            FamobiGameFromApi game = new FamobiGameFromApi();
            bool GameExist = false;
            for (int i = 0; i < famobiJsonApi["games"].LongCount(); i++)
            {

                if (famobiJsonApi["games"][i]["name"].ToString() == name)
                {
                    GameExist = true;
                    game.name = famobiJsonApi["games"][i]["name"].ToString();
                    game.description = famobiJsonApi["games"][i]["description"].ToString();
                    game.thumbnail = famobiJsonApi["games"][i]["thumb_120"].ToString();
                    game.url = famobiJsonApi["games"][i]["link"].ToString();
                    game.aspectRatio = famobiJsonApi["games"][i]["aspect_ratio"].ToString();
                }
            }

            if (GameExist == false)
            {
                throw new MsgException("We could not find this game in famobi Api");
            }

            return game;
        }
    }
}