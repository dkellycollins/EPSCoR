/*jslint unparam: true */
/*global window, $ */
$(function () {
    'use strict';
    // Change this to the location of your server-side upload handler:
    var url = 'http://localhost:6615/Files/UploadFiles',
        cancelButton = $('<button/>')
            .addClass('btn btn-warning')
            .text('Cancel')
            .on('click', function () {
                var $this = $(this),
                    data = $this.data(),
                    $context = data.context;
                data.abort();
                setErrorStatus('Cancelled.', $context);
            }),
        progressBar = $('<div/>')
            .addClass('progress progress-success')
            .append($('<div/>')
                .addClass('bar')
            ),
        filesToUpload = new Array(),
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
            setStatus(status, $context);
        },
        setSuccessStatus = function (status, $context) {
            $context
                .removeClass('alert-info')
                .addClass('alert-success');
            setStatus(status);
        };

    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        autoUpload: false,
        acceptFileTypes: /(\.|\/)(csv)$/i,
        //sequentialUploads: true,
        //multipart: false, //This is required for chunking to work in firefox.
        maxChunkSize: 5000000, // 5 MB
    })
    //This is called when files are added.
    .bind('fileuploadadd', function (e, data) {
        $.each(data.files, function (index, file) {
            var $context = $('<div/>').appendTo('#files');
            data.context = $context;

            $context.addClass('alert alert-info');
            $context.append($('<span/>').text(file.name));
            $context.append($('<br/>'));
            $context.append($('<span/>').text('Processing...').addClass('status'));
            $context.append(progressBar.clone(true));
            $context.append(cancelButton.clone(true).data(data));
        })
    })
    //This is called if an added file is successfully processed.
    .bind('fileuploadprocessdone', function(e, data) {
        filesToUpload.push(data);
        setStatus('Ready to upload', data.context);
    })
    //This is called if an added file is not successfully processed.
    .bind('fileuploadprocessfail', function (e, data) {
        var error = data.files[data.index].error;
        setErrorStatus(error, data.context);
    })
    //This is called eveytime an individual file's progress is updated.
    .bind('fileuploadprogress', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10),
            progressTxt = (data.loaded / 1000) + ' KB / ' + (data.total / 1000) + ' KB';
        $('.bar', data.context).css(
            'width',
            progress + '%'
        );
        if (data.loaded / data.total === 1)
            setStatus('Processing...', data.context);
        else
            setStatus(progressTxt, data.context) 
    })
    //This is called when a file is successfully uploaded.
    .bind('fileuploaddone', function (e, data) {
        if (data.error)
            setErrorStatus(data.error, data.context);
        else
            setSuccessStatus('File uploaded.', data.context);
        $('.btn-warning', data.context).remove();
    })
    //This is called if a fail fails to upload.
    .bind('fileuploadfail', function (e, data) {
        setErrorStatus('File upload failed.', data.context);
        $('.btn-warning', data.context).remove();
    });

    $('#btnUpload').on('click', function () {
        filesToUpload.forEach(function (value, index) {
            value.submit();
        });
        filesToUpload = new Array();
    });
});