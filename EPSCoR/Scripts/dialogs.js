function openUploadFilesDialog() {
    alert('Upload files dialog');
}

function openCalcDialog() {
    alert('Calc dialog');
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