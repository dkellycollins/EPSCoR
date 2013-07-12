$(function() {
    $('#dialog-uploadFiles').dialog({
        autoOpen: false,
        resizable: true,
        modal: true,
        height: window.innerHeight * .8,
        width: window.innerWidth * .8,
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
        height: window.innerHeight * .8,
        width: window.innerWidth * .8,
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
        height: window.innerHeight * .8,
        width: window.innerWidth * .8,
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
    $('#dialog-uploadFiles').dialog("open");
}

function openCalcDialog() {
    window.open("Home/CalcForm");
}

function openAboutDialog() {
    $('#dialog-about').dialog("open");
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