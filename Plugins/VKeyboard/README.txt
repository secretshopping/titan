Originally posted by Jeremy Satterfield in his [blog](http://jsatt.blogspot.com/2010/01/on-screen-keyboard-widget-using-jquery.html), [jQuery plugins](http://plugins.jquery.com/project/virtual_keyboard) and on [Snipplr](http://snipplr.com/view/21577/virtual-keyboard-widget/). Currently maintained by [Mottie](https://github.com/Mottie/Keyboard).

## Features ([Demo](http://mottie.github.com/Keyboard/))

* An on-screen virtual keyboard embedded within the browser window which will popup when a specified entry field is focused.
* The user can then type and preview their input before Accepting or Canceling.
* Add custom keyboard layouts easily.
* Add up to four standard key sets to each layout that use the shift and alt keys (default, shift, alt and alt-shift).
* Add any number of optional modifier keys (meta keys) to add more key sets.
* Each meta key set also includes the shift, alt and alt-shift keysets - New in version 1.8.9.
* Position the keyboard in any location around the element, or target another element on the page.
* Easily modify the key text to any language or symbol.
* Allow direct input or lock the preview window.
* Set a maximum length to the inputted content.
* Scroll through the other key sets using the mouse wheel while hovering over a key to bypass the need to use alt, shift or meta keys.
* Easily type in characters with diacritics. Here are some default combination examples - it is possible to add more.

    * ' + vowel ( vowel with acute accent, e.g. ' + e = é )
    * \` + vowel ( vowel with grave accent, e.g., \` + e = è )
    * " + vowel ( vowel with diaeresis, e.g., " + e = ë )
    * ^ + vowel ( vowel with circumflex accent, e.g., ^ + e = ê )
    * ~ + certain letters ( letter with tilde, e.g. ~ + n = ñ, ~ + o = õ )

* Enable, disable or add more diacritic functionality as desired.
* Use callbacks and event triggers that occur when the keyboard is open or closed and when the content has changed, been accepted or canceled.
* ARIA support (may not be fully implemented)
* As jQuery UI is a dependancy, this plugin's styling will automatically match the selected jQuery UI theme with the exception of the required CSS found in the keyboard.css file.
* Built in watermarking. It emulates HTML5's placeholder, if the browser doesn't support it.
* Typing extension allows you to simulate typing into the keyboard for demo purposes or to assist user input.
* Autocomplete extension will integrate this keyboard plugin with jQuery UI's autocomplete widget.
* Multiple region specific keyboard layouts included in a separate directory. This is a work in progress and slowly growing.

## Documentation

Moved to the Wiki Pages: [Home](https://github.com/Mottie/Keyboard/wiki/Home) | [FAQ](https://github.com/Mottie/Keyboard/wiki/FAQ) | [Setup](https://github.com/Mottie/Keyboard/wiki/Setup) | [Usage](https://github.com/Mottie/Keyboard/wiki/Usage) | [Options](https://github.com/Mottie/Keyboard/wiki/Options) ( [Layout](https://github.com/Mottie/Keyboard/wiki/Layout), [Language](https://github.com/Mottie/Keyboard/wiki/Language), [Useability](https://github.com/Mottie/Keyboard/wiki/Useability), [Actions](https://github.com/Mottie/Keyboard/wiki/Actions) ) | [Methods](https://github.com/Mottie/Keyboard/wiki/Methods) | [Theme](https://github.com/Mottie/Keyboard/wiki/Theme) | [Log](https://github.com/Mottie/Keyboard/wiki/Log)

## To Do

* Waiting for requests :)
* Add more regional keyboard layouts.
* Add an input mask extension. I think I'll try to make it compatible with [this plugin](https://github.com/RobinHerbots/jquery.inputmask).

## Known Problems 

* *IE* and *Opera*:
    * In a text area with multiple carriage returns, the caret positioning will be off when repositioning it with the mouse.
    * Using the right and left arrow keys to navigate through a text area with multiple carriage returns is problematic. The caret doesn't behave like in other browsers when moving from one line to the next. You can always reposition the caret using the mouse.
* *Opera*: When pressing the tab key while inside a textarea, all browsers but Opera add the tab to the virtual keyboard textarea.
* *Safari*: See the QWERTY Text Area demo with a locked input. While using the virtual keyboard to type, it enters the text in backwards! This is because textareas with a "readonly" attribute always returns zero for the caret postion.
* *Typing Extension*:
    * When pressing "Alt", the key set will change to the alt key set, but the focus will be moved to the browser menu. Pressing it quickly a second time will return the focus. This is build into the browser and it isn't possible (as far as I know) to automatically restore the window focus the first time alt is pressed.
    * Holding down the Alt key and trying to type is also not possible since the Windows OS is assuming you are trying to type a shortcut key to access the browser menu. You can still click the keys in the alt key set with the mouse.
    * Simulated typing on the keyboard breaks when the CapLock is on. Still looking for a cross-browser solution.

## Dependencies
* Required
    * jQuery 1.4.3+
    * jQuery UI Positioning Utility
    * jQuery UI CSS (can be customized)
    * jQuery caret (included with source)
* Optional
    * jQuery mousewheel plugin - allows using mousewheel to scroll through other key sets
    * jQuery keyboard typing extension - simulate typing on the virtual keyboard
    * jQuery keyboard autocomplete extension - for use with jQuery UI autocomplete

## Licensing

* Keyboard code: [MIT License](http://www.opensource.org/licenses/mit-license.php) for all versions.
* Caret code by C. F., Wong (Cloudgen): [MIT License](http://www.opensource.org/licenses/mit-license.php)
* Layouts files: Most are under [WTFPL](http://sam.zoy.org/wtfpl/), unless the file itself specifies otherwise.

## Change Log

Only the latest changes will be shown below, see the wiki log to view older versions.

### Version 1.14.1 (10/8/2012)

* Disabled jQuery UI Themeswitcher from the main and layouts demo pages, as the script is no longer available.
* Updated demos to use jQuery 1.8 and jQuery UI 1.9.

### Version 1.14 (10/2/2012)

* Added iPad &amp; iPad email demos by [gitaarik](https://github.com/gitaarik).

### Version 1.13 (9/9/2012)

* Fixed error caused by closing a keyboard in OSX using ctrl-esc or alt-esc. Fixes [issue #102](https://github.com/Mottie/Keyboard/issues/102).
* Added Japanese and Spanish layouts thanks to [pacoalcantara](https://github.com/pacoalcantara)!
* Added Polish layout thanks to Piotr (via email)!
* Wide keys now use a `min-width` instead of `width`. This allows the key to properly expand to fit the text within it.
* Updated autocomplete extension to save the caret position in IE9. Thanks to [banku](https://github.com/banku) for the fix in [issue #95](https://github.com/Mottie/Keyboard/issues/95).
* Updated navigation extension:
  * Removed the `toggleKey` option.
  * Custom key codes can be assigned to any of the navigation keys within the new `$.keyboard.navigationKeys` object. Extend it as follows:

    ```javascript
    // change default navigation keys
    $.extend($.keyboard.navigationKeys, {
      toggle     : 112, // toggle key; F1 = 112 (event.which value for function 1 key)
      enter      : 13,  // Enter
      pageup     : 33,  // PageUp key
      pagedown   : 34,  // PageDown key
      end        : 35,  // End key
      home       : 36,  // Home key
      left       : 37,  // Left arrow key
      up         : 38,  // Up arrow key
      right      : 39,  // Right arrow key
      down       : 40   // Down arrow key
    });
    ```

   Enhancement request from [issue #90](https://github.com/Mottie/Keyboard/issues/90). Thanks [faboudib](https://github.com/faboudib)!

  * Movement of the highlighted navigation key can now be triggered using `navigate` for predefined movement; see the [updated demo](http://mottie.github.com/Keyboard/navigate.html)

    ```javascript
    // navkey contains the name of the key: e.g. "home", "right", "pageup", etc
    var navkey = "pageup";
    $('#keyboard').trigger('navigate', navkey);
    ```

    Or, highlight a specific navigation key using the `navigateTo` trigger:

    ```javascript
    // navigate to the third row and fourth key (zero-based indexes) - [ row, index ]
    $('#keyboard').trigger('navigateTo', [2,3]);
    ```

### Version 1.12 (7/24/2012)

* Made api functions `accept()` and `close()` return a boolean showing if the content was accepted or not.
  * See [this demo](http://jsfiddle.net/Mottie/MK947/77/) for an example of how to use this returns when replacing the Accept action key function.
  * See [issue #88](https://github.com/Mottie/Keyboard/issues/88) for details.

### Version 1.11 (7/24/2012)

* Switching inputs should now work properly
  * Extra thanks to [david-hollifield](https://github.com/david-hollifield) for the help in fixing this bug!
  * Fixes [issues #86](https://github.com/Mottie/Keyboard/issues/86).
* Modified the validate procressing to no longer disable the accept button.
  * The accept button now gets a class applied indicating if the input is valid (`ui-keyboard-valid-input`) or invalid (`ui-keyboard-invalid-input`).
  * Very basic css added to colorize the accept button for these states.
  * Fixes [issue #88](https://github.com/Mottie/Keyboard/issues/88).
* Added `cancelClose` option
  * This option only works with `acceptValid` is `true` and the `validate` function returns `false`.
  * If `true`, this option will cancel the keyboard close initiated by the accept button. The keyboard can still be closed by pressing escape or the cancel button.
  * If `false`, the validate function will ignore the user input, restore the input's previous value, and close the keyboard.

### Version 1.10 (7/9/2012)

* Added `{next}` and `{prev}` action keys which makes switching between input/textareas easier.
* Added the ability to make some action keys get the button action class applied
  * The action class (options.css.buttonAction) makes the button stand out like the `{accept}`, `{cancel}` & `{enter}` keys.
  * Add double exclamation point to the custom key name `{custom!!}` or to any built-in action key except: `{accept}`, `{alt}`, `{bksp}`, `{cancel}`, `{combo}`, `{dec}`, `{enter}`, `{meta#}`, `{shift}`, `{sign}`, `{sp:#}`, `{space}` or `{tab}`.
  * See a demo named "Custom Action Key" in the *More Demos* section of the [home page wiki documentation](https://github.com/Mottie/Keyboard/wiki).
* Added `stopAtEnd` option which when `true` prevents the default switch input function from wrapping to the first or last element. Useful when used in combination with the new `{next}` and `{prev}` action keys.
* Modified diacritic key modification code:
  * As before, diacritic (dead) key combinations will be ignored when the `{combo}` key is inactive.
  * But now, in modern browsers, when the `{combo}` key is reactivated, only the two characters immediately to the left of the caret will be evaluated for key combinations instead of the entire input string.
  * Older IE (IE8 and older) will continue to check and update the entire input string.
  * Change made to make dead keys more useful as described in [issue #79](https://github.com/Mottie/Keyboard/issues/79).
* Fixed `stayOpen` function not allowing keyboards to open/close with multiple keyboards. Hopefully this new method will squash all the problems with `stayOpen` and `alwaysOpen` options. Fixes [issue #82](https://github.com/Mottie/Keyboard/issues/82).

### Version 1.9.21 (6/18/2012)

* IE should now behave like other browsers when switching inputs; clicking on another input with a keyboard open will now switch immediately instead of requiring a second click.

### Version 1.9.20 (6/17/2012)

* Added Latvian layout. Thanks to Ivars via email.

### Version 1.9.19 (6/17/2012)

* Modified script to add "ui-keyboard-autoaccepted" class name to the original input if the content was autoaccepted. Discussed in [issue #66](https://github.com/Mottie/Keyboard/issues/66).
* Added `resetDefault` option which when `true` will force the keyset to reset to the default when the keyboard becomes visible.
* Mulitple keyboards that are always open will not keep focus properly. Fixes issues [#69](https://github.com/Mottie/Keyboard/issues/69), [#73](https://github.com/Mottie/Keyboard/issues/73) and [#75](https://github.com/Mottie/Keyboard/issues/75).
* Fixed carriage return issue in a textarea in IE8 (hopefully). Thanks to [blookie](https://github.com/blookie) for reporting it and providing a fix in [issue #71](https://github.com/Mottie/Keyboard/issues/71).
* IE should now close the keyboard after clicking accept. Base element will no longer maintain focus. Fix for [issue #72](https://github.com/Mottie/Keyboard/issues/72).
* Reveal will no longer unbind all events when `openOn` is empty. Fix for [issue #74](https://github.com/Mottie/Keyboard/issues/74).
* Fixed locked keyboard input not allowing opening the keyboard a second time. Fix for [issue #77](https://github.com/Mottie/Keyboard/issues/77).
* Fixed `stayOpen` option not working at all.
* Added Hebrew layout. Thanks to Ofir Klinger for contributing the work!
* Added a keyboard object as a variable in the typing callback function. Probably not necessary, but added anyway :P

### Version 1.9.18 (5/13/2012)

* Fixed an issue of the input clearing when `usePreview` is `false` and `alwaysOpen` is `true`. Brought up in [issue #37](https://github.com/Mottie/Keyboard/issues/37#issuecomment-5298677).

### Version 1.9.17 (5/8/2012)

* Added Turkish layouts. Thanks to [barisaydinoglu](https://github.com/barisaydinoglu)!

### Version 1.9.16 (4/30/2012)

* Caret position is now better retained in older IE. Fix for [issue #61](https://github.com/Mottie/Keyboard/issues/61).
* Invalid input should now revert back to the last valid input instead of breaking the keyboard. Fix for [issue #62](https://github.com/Mottie/Keyboard/issues/62).
* The repeating key obtained by holding down the mouse on a virtual key can now be disabled by setting the `repeatRate` to `0` (zero). Fix for [issue #63](https://github.com/Mottie/Keyboard/issues/63).
* Clicking on a virtual keyboard key will no longer submit a form - fix for [issue #64](https://github.com/Mottie/Keyboard/issues/64).

### Version 1.9.15

* Updated Mobile demo
  * Updated to [jQuery Mobile version 1.1.0 RC1](http://jquerymobile.com/blog/2012/02/28/announcing-jquery-mobile-1-1-0-rc1/)
  * Extra demo css added because the theme selector radio buttons were not displaying properly due to some issues with the data-attributes showing "[Object object]" instead of true or false. I'm not sure why, and don't have the time to investigate.
* Fixed a problem where keyboards with `alwaysOpen` and `autoAccept` set to `true` would keep focus on the input when clicking outside the input. Fix for [issue #59](https://github.com/Mottie/Keyboard/issues/59).
* Fixed an issue with `tabNavigation` not working properly. Also, discovered that `tabindex="0"` should not be used. Fix for [issue #60](https://github.com/Mottie/Keyboard/issues/60).

### Version 1.9.14

* Multiple synchronized keyboards with `alwaysOpen` and `autoAccept` set to `true` should now switch properly. Fix for [issue #58](https://github.com/Mottie/Keyboard/issues/58).

### Version 1.9.13

* Multiple synchronized keyboards with `alwaysOpen` set to `true` should now switch properly. Fix for [issue #58](https://github.com/Mottie/Keyboard/issues/58).

### Version 1.9.12.1

* Updated jquery.mousewheel.js, as the it was only scrolling in one direction.
