﻿/*jslint unparam: true, white: true, indent: 4 */
/* global: $ serverBase */
$(function () {
    'use strict';
    var baseUrl = serverBase + 'Files/',
        uploadUrl = baseUrl + 'UploadFiles',
        completeUrl = baseUrl + 'CompleteUpload',
        checkUrl = baseUrl + 'CheckFile',
        filesToUpload = [],
        setStatus = function (status, $context) {
            var $status = $('.status', $context);
            $status.text(status);
        },
        setErrorStatus = function (status, $context) {
            $('.progress', $context)
                .removeClass('progress-success')
                .addClass('progress-danger');
            $context
                .removeClass('alert-info')
                .addClass('alert-error');
            $('.close', $context).removeClass('hidden');
            setStatus(status, $context);
        },
        setSuccessStatus = function (status, $context) {
            $context
                .removeClass('alert-info')
                .addClass('alert-success');
            $('.close', $context).removeClass('hidden');
            setStatus(status, $context);
        },
        cancelButton = $('<button class="btn btn-warning">Cancel</button>')
            .on('click', function () {
                var $this = $(this),
                    data = $this.data(),
                    $context = data.context,
                    index = $context.queueIndex;
                if ($context.jqXHR) {
                    $context.jqXHR.abort();
                }
                if (index !== -1) {
                    filesToUpload.splice(index, 1);
                    setErrorStatus('Cancelled.', $context);
                    $(this).remove();
                }
            }),
        closeButton = $('<button type="button" class="close hidden" data-dismiss="alert">x</button>'),
        progressBar = $('<div class="progress progress-success"><div class="bar"></div></div>');

    $('#fileupload').fileupload({
        url: uploadUrl,
        dataType: 'json',
        autoUpload: false,
        acceptFileTypes: /(\.|\/)(csv)$/i,
        //sequentialUploads: true,
        //multipart: false, //This is required for chunking to work in firefox.
        maxChunkSize: 5000000 // 5 MB
    })
    //This is called when files are added.
    .bind('fileuploadadd', function (e, data) {
        $.each(data.files, function (index, file) {
            var $context = $('<div/>').appendTo('#files');
            data.context = $context;

            $context.addClass('alert alert-info');
            $context.append(closeButton.clone(true));
            $context.append($('<span/>').text(file.name));
            $context.append($('<br/>'));
            $context.append($('<span class="status">Processing...</span>'));
            $context.append(progressBar.clone(true));
            $context.append(cancelButton.clone(true).data(data));

            $.getJSON(checkUrl, { id: file.name }, function (result) {
                var file = result.file;
                data.uploadedBytes = file && file.size;
                $.blueimp.fileupload.prototype.options.add.call(that, e, data);
            });
        });
    })
    //This is called if an added file is successfully processed.
    .bind('fileuploadprocessdone', function(e, data) {
        data.context.queueIndex = filesToUpload.push(data) - 1;
        setStatus('Ready to upload', data.context);
    })
    //This is called if an added file is not successfully processed.
    .bind('fileuploadprocessfail', function (e, data) {
        var error = data.files[data.index].error;
        setErrorStatus(error, data.context);
        $('.btn-warning', data.context).remove();
    })
    //This is called eveytime an individual file's progress is updated.
    .bind('fileuploadprogress', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10),
            progressTxt = (data.loaded / 1000) + ' KB / ' + (data.total / 1000) + ' KB';
        $('.bar', data.context).css(
            'width',
            progress + '%'
        );
        if (data.loaded / data.total === 1) {
            setStatus('Processing...', data.context);
        } else {
            setStatus(progressTxt, data.context);
        }
    })
    //This is called when a file is successfully uploaded.
    .bind('fileuploaddone', function (e, data) {
        var fileName = data.files[data.index].name,
            $context = data.context;
        $.post(completeUrl + '/?id=' + fileName,
            function (requestData) {
                if (requestData.Error) {
                    setErrorStatus(requestData.Error, $context);
                } else {
                    setSuccessStatus('File uploaded.', $context);
                }
                $('.btn-warning', $context).remove();
            });
    })
    //This is called if a fail fails to upload.
    .bind('fileuploadfail', function (e, data) {
        setErrorStatus('File upload failed.', data.context);
        $('.btn-warning', data.context).remove();
    });

    $('#btnUpload').on('click', function () {
        filesToUpload.forEach(function (value, index) {
            value.context.jqXHR = value.submit();
            value.context.queueIndex = -1;
        });
        filesToUpload = [];
    });
});