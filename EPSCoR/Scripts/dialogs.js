function openUploadFilesDialog() {
    window.open("Home/UploadForm");
}

function openCalcDialog() {
    window.open("Home/CalcForm");
}

function openAboutDialog() {
    window.open("Home/AboutForm");
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