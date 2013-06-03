/*jslint unparam: true */
/*global window, $ */
$(function () {
    'use strict';
    // Change this to the location of your server-side upload handler:
    var url = 'http://localhost:6615/Upload/UploadFiles',
        cancelButton = $('<button/>')
            .addClass('btn btn-warning')
            .text('Cancel')
            .on('click', function () {
                var $this = $(this),
                    data = $this.data(),
                    $context = $(data.context);
                $context.remove();
                data.abort();
                addedFiles.splice(
                    addedFiles.indexOf(data),
                    1
                );
            }),
         progressBar = $('<div/>')
            .addClass('progress progress-success')
            .append($('<div/>')
                .addClass('bar')
            ),
        fileStatus = $('<span/>').addClass('status'),
        addedFiles = new Array();
    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        autoUpload: false,
        acceptFileTypes: /(\.|\/)(csv)$/i,
        //sequentialUploads: true,
        //multipart: false,
        //maxChunkSize: 5000000, // 5 MB
        add: function (e, data) {
            data.context = $('<div/>').appendTo('#files');
            $.each(data.files, function (index, file) {
                var node = $('<p/>')
                        .append($('<span/>')
                            .text(file.name)
                            .addClass('span1'));
                if (!index) {
                    node.append($('<span/>').text('Ready to upload').addClass('status span2'));
                    node.append(cancelButton.clone(true).data(data));
                    node.append(progressBar.clone(true).data(data));
                }
                node.appendTo(data.context);
                data.context.addClass('alert alert-info');
                addedFiles.push(data);
            })
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('.bar', data.context).css(
                'width',
                progress + '%'
            );
            $('.status', data.context).text(
                (data.loaded / 1000) + ' KB / ' + (data.total / 1000) + ' KB'
            );
        },
        always: function(e, data) {
            $('.btn-warning', data.context).remove();
        },
        done: function (e, data) {
            $('.status', data.context).text('File uploaded.');
            $(data.context)
                .removeClass('alert-info')
                .addClass('alert-success');
        },
        fail: function (e, data) {
            $('.status', data.context).text('File upload failed.');
            $('.progress', data.context)
                .removeClass('progress-success')
                .addClass('progress-danger');
            $(data.context)
                .removedClass('alert-info')
                .addClass('alert-error');
        }
    });

    $('#btnUpload').on('click', function () {
        addedFiles.forEach(function (value, index) {
            value.submit();
        });
        addedFiles = new Array();
    });
});