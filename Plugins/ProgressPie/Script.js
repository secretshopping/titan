var timer;

var timerElapsedTime;
var timerFullTime;

var is_visible_focus = true;
var formSent = true;

// main visibility API function 
// use visibility API to check if current tab is active or not
var vis = (function () {
    var stateKey,
        eventKey,
        keys = {
            hidden: "visibilitychange",
            webkitHidden: "webkitvisibilitychange",
            mozHidden: "mozvisibilitychange",
            msHidden: "msvisibilitychange"
        };
    for (stateKey in keys) {
        if (stateKey in document) {
            eventKey = keys[stateKey];
            break;
        }
    }
    return function (c) {
        if (c) document.addEventListener(eventKey, c);
        return !document[stateKey];
    }
})();

window.focus();

window.onfocus = function () {
    is_visible_focus = true;
};

window.onblur = function () {
    is_visible_focus = false;
};

// check if current tab is active or not
vis(function () {

    if (vis()) {
        is_visible_focus = true;
        window.focus();
    } else {
        // tween pause() code goes here
        is_visible_focus = false;
    }

});


//FOCUS END

function drawTimer(percent) {
    window.focus();
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

function stopWatch(timerSeconds, timeLeftString, requierFocus) {

    if (is_visible_focus == true || !requierFocus) {
        timerElapsedTime = timerElapsedTime + 300;
    }

    var seconds = (timerFullTime - timerElapsedTime) / 1000;

    if (seconds <= 0) {
        drawTimer(100);
        clearInterval(timer);
        //FINISH, send postback

        if (formSent == false) {
            __doPostBack('CaptchaPostbackHackButton', 'eValue');
            formSent = true;
        }

    } else {
        var percent = 100 - ((seconds / timerSeconds) * 100);
        drawTimer(percent);
        document.title = timeLeftString + ': ' + parseInt(seconds) + 's';
    }
}

function startBar(timerSeconds, timeLeftString, requierFocus) {
    timerElapsedTime = 0;
    timerFullTime = timerSeconds * 1000;
    timer = setInterval('stopWatch(' + timerSeconds + ',"' + timeLeftString + '",' + requierFocus + ')', 300);
    formSent = false;
}
