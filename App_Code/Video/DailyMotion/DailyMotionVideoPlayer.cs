using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DailyMotionVideoPlayer : VideoPlayer
{
    public DailyMotionVideoPlayer(string id, string title, string description, string views)
        : base(id, title, description, views)
	{
	}

    public override string ToHTML(int width, int height)
    {
        return @"
           <script src=""https://api.dmcdn.net/all.js""></script>
           <div id=""player""></div>
         
            <script>
                var isSent = false;

                var player = DM.player(document.getElementById('player'), {
                    video: '" + base.id + @"',
                    width: '" + width + @"',
                    height: '" + height + @"',
                    params: {
                        wmode: 'opaque',
                        autoplay: 1
                    }
                });

                player.addEventListener(""ended"", function (e) {
                    //If playlist, then we go to the next video
                    //Send AJAX request
                    document.getElementById('ajaxPostbackVideoTriggerEnded').click();                    
                });

                player.addEventListener(""timeupdate"", function (e) {
                    if (player.currentTime > '" + Prem.PTC.AppSettings.SearchAndVideo.CreditAfterSetTime + @"' && isSent == false)
                    {
                        isSent = true;

                        //Send AJAX request ONCE
                        document.getElementById('ajaxPostbackVideoTrigger').click();
                    }
                });

                //player.addEventListener(""progress"", function (e) {
                //     //Not used
                //});

                //player.addEventListener(""seeking"", function (e) {
                //     //Not used
                //});

                //player.addEventListener(""playing"", function (e) {
                //     //Not used
                //});

                //player.addEventListener(""seeked"", function (e) {
                //    //Not used
                //});

            </script>";
    }
}