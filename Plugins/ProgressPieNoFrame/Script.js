var timer;

var timerElapsedTime;
var timerFullTime;
var openedWindow;

function drawTimer(percent) {
    $('div.timer').html('<div class="percent"></div><div id="slice"' + (percent > 50 ? ' class="gt50"' : '') + '><div class="pie"></div>' + (percent > 50 ? '<div class="pie fill"></div>' : '') + '</div>');
    var deg = 360 / 100 * percent;
    $('#slice .pie').css({
        '-moz-transform': 'rotate(' + deg + 'deg)',
        '-webkit-transform': 'rotate(' + deg + 'deg)',
        '-o-transform': 'rotate(' + deg + 'deg)',
        'transform': 'rotate(' + deg + 'deg)'
    });
    $('.percent').html(Math.round(percent) + '%');
}

function stopWatch(timerSeconds, timeLeftString) {
    if (!openedWindow || openedWindow.closed) {
        console.log('closed :(');
    }
    else {
        timerElapsedTime = timerElapsedTime + 1000;
    }

    var seconds = (timerFullTime - timerElapsedTime) / 1000;

    if (seconds <= 0) {
        drawTimer(100);
        clearInterval(timer);
        //FINISH, send postback
        $('#form1').submit();

    } else {
        var percent = 100 - ((seconds / timerSeconds) * 100);
        drawTimer(percent);
        document.title = timeLeftString + ': ' + parseInt(seconds) + 's';
    }
}

function startBar(timerSeconds, timeLeftString) {
    timerElapsedTime = 0;
    timerFullTime = timerSeconds * 1000;
    timer = setInterval('stopWatch(' + timerSeconds + ',"' + timeLeftString + '")', 1000);
}
