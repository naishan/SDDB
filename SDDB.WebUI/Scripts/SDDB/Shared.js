﻿/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />



//---------------------------------------Global Settings-------------------------------------//

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

});

//---------------------------------------Modal Dialogs---------------------------------------//

$(document).ready(function () {

    //Get focus on ModalInfoBtnOk
    $("#ModalInfo").on("shown.bs.modal", function () { $("#ModalInfoBtnOk").focus(); });

    //Wire Up ModalInfoBtnOk
    $("#ModalInfoBtnOk").click(function () { $("#ModalInfo").modal("hide"); });

    //Get focus on ModalDeleteBtnCancel
    $("#ModalDelete").on("shown.bs.modal", function () { $("#ModalDeleteBtnCancel").focus(); });

    //Wire Up ModalDeleteBtnCancel 
    $("#ModalDeleteBtnCancel").click(function () { $("#ModalDelete").modal("hide"); });

});


//------------------------------------Common Main Methods----------------------------------//

//Show Modal Nothing Selected
function ShowModalNothingSelected() {
    $("#ModalInfoLabel").text("Nothing Selected");
    $("#ModalInfoBody").text("Please select one or more rows.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Selected other than one row
function ShowModalSelectOne() {
    $("#ModalInfoLabel").text("Selected One");
    $("#ModalInfoBody").text("Please select only one row.");
    $("#ModalInfoBodyPre").empty().hide();
    $("#ModalInfo").modal("show");
}

//Show Modal Delete
function ShowModalDelete(noOfRows) {
    $("#ModalDeleteBody").text("Confirm deleting " + noOfRows + " row(s).");
    $("#ModalDelete").modal("show");
}

//Wire Up ModalDeleteBtnOk
$("#ModalDeleteBtnOk").click(function () {
    $("#ModalDelete").modal("hide");
    DeleteRecords();
});

//ShowModalFail
function ShowModalFail(label, body, bodyPre) {
    label = (typeof label !== "undefined") ? label : "Undefined Error";
    body = (typeof body !== "undefined") ? body : "";
    bodyPre = (typeof bodyPre !== "undefined") ? bodyPre : "";
  
    $("#ModalInfoLabel").text(label);
    $("#ModalInfoBody").html(body);
    if (bodyPre != "") $("#ModalInfoBodyPre").text(bodyPre).show();
    $("#ModalInfo").modal("show");
}

//ShowModalAJAXFail
function ShowModalAJAXFail(xhr, status, error) {
    if (typeof xhr.responseJSON !== "undefined") {
        var errMessage = xhr.responseJSON.responseText.substr(0, 512)
    }
    else if (typeof xhr.responseText !== "undefined") {
        var errMessage = xhr.responseText.substr(0, 512)
    }
    if (typeof errMessage == "undefined" || errMessage == "") { errMessage = "No error details available." }
    $("#ModalInfoLabel").text("Server Error");
    $("#ModalInfoBody").html("Error type: <strong>" + error + "</strong> , Status: <strong>" + status + "</strong>");
    $("#ModalInfoBodyPre").text(errMessage).show();
    $("#ModalInfo").modal("show");
}

//Refresh main table from AJAX
function RefreshTable(table, url, getActive, httpType, projectIds, modelIds) {

    getActive = (typeof getActive !== "undefined" && getActive == false) ? false : true;
    httpType = (typeof httpType !== "undefined") ? httpType : "GET";
    projectIds = (typeof projectIds !== "undefined") ? projectIds : [];
    modelIds = (typeof modelIds !== "undefined") ? modelIds : [];

    $.ajax({
        type: httpType, url: url, timeout: 20000, data: { getActive: getActive, projectIds: projectIds, modelIds: modelIds }, dataType: "json",
        beforeSend: function () {
            table.clear().search("").draw();
            $("#ModalWait").modal({ show: true, backdrop: "static", keyboard: false });
        }
    })
        .always(function () { $("#ModalWait").modal("hide"); })
        .done(function (data) {
            table.rows.add(data.data).order([1, 'asc']).draw();
        })
        .fail(function (xhr, status, error) {
            ShowModalAJAXFail(xhr, status, error);
        });
}

//checking if form is valid
function FormIsValid(id, isCreate) {
    if ($("#" + id).valid()) return true;
    else if (isCreate) return false;
    else {
        var isValid = true;
        $("#" + id + " input").each(function (index) {
            if ($(this).data("ismodified") && $(this).hasClass("input-validation-error")) isValid = false;
        });
        return isValid;
    }
}

//Clear inputs from forms and reset .ismodified to false
function ClearFormInputs(formId, msArray) {
    $("#" + formId + " :input").prop("checked", false).prop("selected", false)
        .not(":button, :submit, :reset, :radio, :checkbox").val("");
    $("#" + formId + " [data-valmsg-for]").empty();
    $("#" + formId + " .input-validation-error").removeClass("input-validation-error");
    $("#" + formId + " .modifiable").data("ismodified", false);

    if (typeof msArray !== "undefined") {
        $.each(msArray, function (i, ms) { ms.clear(true); ms.isModified = false; });
    }
}

//initialize MagicSuggest and add to MagicSuggest array
function AddToMSArray(msArray, id, url, maxSelection, minChars) {

    var element = $("#" + id);
    var dataValRequired = $(element).attr("data-val-required");
    var dataValDbisunique = $(element).attr("data-val-dbisunique");

    var ms = element.magicSuggest({
        invalidCls: "input-validation-error",
        data: url,
        allowFreeEntries: false,
        minChars: minChars,
        maxSelection: maxSelection,
        required: (typeof dataValRequired !== "undefined") ? true : false,
        resultAsString: true,
        ajaxConfig: {
            error: function (xhr, status, error) { ShowModalAJAXFail(xhr, status, error); }
        }
    });

    ms.id = id;
    ms.dataValRequired = dataValRequired;
    ms.dataValDbisunique = dataValDbisunique;
    ms.isModified = false;

    $(ms).on('blur', function (e, m) {
        if (!this.isValid()) {
            $("#" + this.id).addClass("input-validation-error");
            $("[data-valmsg-for=" + this.id + "]").text("Input missing or invalid.").removeClass("field-validation-valid")
                .addClass("field-validation-error");
        }
        else {
            $("#" + this.id).removeClass("input-validation-error");
            $("[data-valmsg-for=" + this.id + "]").text("").addClass("field-validation-valid")
                .removeClass("field-validation-error");
        }
    });

    $(ms).on('selectionchange', function (e, m) { this.isModified = true });

    msArray.push(ms);
}

//check if MagicSuggests in MagicSugest Array are valid
function MsIsValid(msArray) {
    var msCheck = true;
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) msCheck = false;
    });
    return msCheck;
}

//change formatting of invalid MagicSugest's
function MsValidate(msArray) {
    $.each(msArray, function (i, ms) {
        if (!ms.isValid()) {
            $("#" + ms.id).addClass("input-validation-error");
            $("[data-valmsg-for=" + ms.id + "]").text("Input missing or invalid.").removeClass("field-validation-valid")
                .addClass("field-validation-error");
        }
    });
}

//enable or disable DbUnique MagicSuggests
function DisableUniqueMs(msArray, disable) {
    disable = (typeof disable !== "undefined" && disable == false) ? false : true;
    $.each(msArray, function (i, ms) {
        if (disable == true && typeof ms.dataValDbisunique !== "undefined" && ms.dataValDbisunique == "true") ms.disable();
        else ms.enable();
    });
}



