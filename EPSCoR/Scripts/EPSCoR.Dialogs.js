/* Dependencies:
    jQuery
    jQueryUi
*/

EPSCoR.Dialogs = {

    init: function() {
        $('div[id^="dialog-"]').dialog({
            autoOpen: false,
            resizable: true,
            modal: true,
            height: "auto",
            width: "auto",
            show: {
                effect: "clip",
                duration: 500
            },
            hide: {
                effect: "clip",
                duration: 500
            }
        });
    },

    openDialog: function (id, options) {
        var $dialog = $('#' + id);
        if (options.fillScreen) {
            $dialog.dialog("option", "height", window.innerHeight * .8);
            $dialog.dialog("option", "width", window.innerWidth * .8);
        }

        $dialog.dialog('open');

        if (options.url) {
            $dialog.load(options.url);
        }
    },

    showDiv: function (divid) {
        $('#content div').hide(200);
        $('#' + divid).show(200);
    },

    yesNoDialog: function (yesBtn, noBtn, title, text, onClose) {
        var btns = {};
        btns[yesBtn] = function () {
            $(this).dialog("close");
            onClose(true);
        };
        btns[noBtn] = function () {
            $(this).dialog("close");
            onClose(false);
        };
        $("<div><span>" + text + "</div>").dialog({
            autoOpen: true,
            title: title,
            modal: true,
            buttons: btns
        });
    }
};