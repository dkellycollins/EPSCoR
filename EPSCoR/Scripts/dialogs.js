$(function() {
    $('#dialog-uploadFiles').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        show: {
            effect: "clip",
            duration: 500
        },
        hide: {
            effect: "clip",
            duration: 500
        }
    });

    $('#dialog-calc').dialog({
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

    $('#dialog-about').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        show: {
            effect: "clip",
            duration: 500
        },
        hide: {
            effect: "clip",
            duration: 500
        }
    });
});

function openUploadFilesDialog() {
    var $dialog = $('#dialog-uploadFiles');
    $dialog.dialog("option", "height", window.innerHeight * .8);
    $dialog.dialog("option", "width", window.innerWidth * .8);
    $dialog.dialog("open");
    //$dialog.load('Home/UploadForm');
}

function openCalcDialog() {
    var $dialog = $('#dialog-calc');
    $dialog.dialog("open");
    $dialog.load('Home/CalcForm');
}

function openAboutDialog() {
    var $dialog = $('#dialog-about');
    $dialog.dialog("option", "height", window.innerHeight * .8);
    $dialog.dialog("option", "width", window.innerWidth * .8);
    $dialog.dialog("open");
}

function showDiv(divid) {
    $('#content div').hide(200);
    $('#' + divid).show(200);
}

function yesnodialog(yesBtn, noBtn, title, text, onClose) {
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