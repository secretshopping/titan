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

function stopWatch(timerSeconds) {

    if (is_visible_focus == true) {
        timerElapsedTime = timerElapsedTime + 1500;
    }

    var seconds = (timerFullTime - timerElapsedTime) / 1000;

    if (seconds <= 0) {
        clearInterval(timer);
        if (formSent == false) {
            console.log('before-click');
            document.getElementById('CreditPostback').click(); return false;
            formSent = true;
        }

    }
}

function startCounter(timerSeconds) {
    timerElapsedTime = 0;
    timerFullTime = timerSeconds * 1000;
    timer = setInterval('stopWatch(' + timerSeconds + ')', 1500);
    formSent = false;
}
