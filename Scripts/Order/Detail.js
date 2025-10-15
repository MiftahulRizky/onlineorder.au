$(document).ready(function () {
  if (roleName == "Administrator") {
    console.log("Detail.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("userId: " + userId);
    console.log("userName: " + userName);
    console.log("headerId: " + headerId);
    console.log("itemId: " + itemId);
    console.log("pricesAccess: " + pricesAccess);
    console.log("printPreview: " + printPreview);
    console.log("Reprint: " + Reprint);
    console.log("uriMethod: " + uriMethod);
  }
  checkSession();
});
// ==================================================EVENTS==================================================
// ------------------------------------------||Card 1 Event ||-------------------------------------------
// BUTTON FINISH
$("#btnFinish").on("click", () => (window.location.href = "/order"));

// BUTTON PREVIEW PRINT
$("#btnPreviewPrint").on("click", function () {
  handlerCreatePDFOrder(
    headerId,
    "preview",
    "Please wait while we generate the document."
  );
});

// BUTTON PREVIEW PDF
$("#btnPreviewPDF").on("click", function () {
  handlerCreatePDFOrder(
    headerId,
    "download",
    "Please wait while we generate the document."
  );
});

// BUTTON CONVERT TO JOB
$("#btnConvertToJob").on("click", function () {
  handlerConvertToJob(headerId, "convert", "Please wait while we convert...");
});

// BUTTON RE PRINT JOB SHEET
$("#btnReprintJobSheet").on("click", function () {
  handlerCreateJOBOrder(headerId, "reprint", "Please wait while we reprint...");
});

// BUTTON SUBMIT ORDER
$("#btnSubmit").on("click", function () {
  handlerSubmitOrder(
    headerId,
    "submit",
    "Please wait while we submit the order."
  );
});

// BUTTON EDIT HEADER
$("#btnEditHeader").on("click", function () {
  handlerEditHeader(headerId);
});

// BUTTON DELETE HEADER
$("#btnDeleteHeader").on("click", function () {
  handlerDeleteHeader(headerId);
});

// BUTTON QUOTE DETAIL
$("#btnQuoteDetail").on("click", function () {
  handlerCreatePDFQuote(
    headerId,
    userName,
    "preview",
    "Please wait while we generate the document."
  );
});

// BUTTON DOWNLOAD QUOTE
$("#btnDownloadQuote").on("click", function () {
  handlerCreatePDFQuote(
    headerId,
    userName,
    "download",
    "Please wait while we generate the document."
  );
});

// BUTTON CHANGE STSTUS
$("#btnChangeStatus").on("click", function () {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  );
  handlerChangeStatus(headerId);
});

// BUTTON SEND MANUAL ORDER
$("#btnSendOrderMail").on("click", function () {
  handlerCreatePDFOrder(
    headerId,
    "mail",
    "Please wait while we generate the document."
  );
});

// BTN RELOAD PRICING
$("#btnReloadPricing").on("click", function () {
  const statusOrder = document.getElementById("spanStatusOrder").innerHTML;
  handlerReloadPricing(headerId, statusOrder, "click");
});

// BUTTON ADD ITEMS
$("#btnAddItem").on("click", function () {
  handlerResetFormError(
    "#modalAddItem .form-control, #modalAddItem .form-select"
  );
  handlerSelDesignType("#modalAddItem #designid");
  handlerShowBSModal("modalAddItem");
});

// ------------------------------------------||modalAddItem Event ||------------------------------------
// CHANGE DESIGN TYPE
$("#modalAddItem #designid").on("change", function () {
  $(this).removeClass("is-invalid");
});

// BUTTON SUBMIT ADD ITEM
$("#modalAddItem #submitAddItem").on("click", function () {
  const designId = $("#modalAddItem #designid").val();
  const action = "AddItem";
  submitSelectProduct(headerId, action, designId);
});

// ------------------------------------------||modalChangeStatus Event ||------------------------------------
// CHANGE STATUS
$("#modalChangeStatus #status").on("change", function () {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  );
  const status = $(this).val();
  hanlderDisplayElementModalChangeStatus(status);
});

// CHANGE SUBMITTED DATE
$("#modalChangeStatus #submitteddate").on("change", function () {
  $(this).removeClass("is-invalid");
});

// CHANGE COMPLETED DATE
$("#modalChangeStatus #completeddate").on("change", function () {
  $(this).removeClass("is-invalid");
});

// CHANGE CANCELED DATE
$("#modalChangeStatus #canceleddate").on("change", function () {
  $(this).removeClass("is-invalid");
});

// INPUT DESCRIPTION
$("#modalChangeStatus #description").on("input", function () {
  $(this).removeClass("is-invalid");
});

// TOOLTIP DESCRIPTION CLICK
$("#modalChangeStatus #tooltipDescription").on("click", function () {
  const status = $("#modalChangeStatus #status").val();
  handlerTooltip("modalChangeStatus", status);
});

// BUTTON SUBMIT CHANGE STATUS
$("#modalChangeStatus #submitChangeStatus").on("click", submitChangeStatus);

// ------------------------------------------||tableAjax Event ||------------------------------------
// BUTTON DETAIL ITEM
$("#tableAjax").on("click", "#btnDetailItem", function () {
  const id = $(this).data("id");
  const designid = $(this).data("designid");
  const headerid = $(this).data("headerid");
  handlerEditItem(id, headerid, "ViewItem", designid);
});

// BUTTON EDIT ITEM
$("#tableAjax").on("click", "#btnEditItem", function () {
  const id = $(this).data("id");
  const designid = $(this).data("designid");
  const headerid = $(this).data("headerid");
  handlerEditItem(id, headerid, "EditItem", designid);
});

// BUTTON COPY ITEM
$("#tableAjax").on("click", "#btnCopyItem", function () {
  const id = $(this).data("id");
  const product = $(this).data("product");
  handlerCopyItem(id, product, "Please wait while we copy the item...");
});

// BUTTON DELETE ITEM
$("#tableAjax").on("click", "#btnDeleteItem", function () {
  const id = $(this).data("id");
  const product = $(this).data("product");
  handlerDeleteItem(id, product, "Please wait while we delete the item...");
});

// BUTTON PRICING ITEM
$("#tableAjax").on("click", "#btnPricingItem", function () {
  const id = $(this).data("id");
  // console.log(id);
  // return;
  handlerPricingItem(id);
  handlerShowBSModal("modalPricingItem");
});

// HANDLER NEXT ITEM
$("#tableAjax").on("click", "#btnNextItem", function () {
  const id = $(this).data("id");
  const designId = $(this).data("designid");
  const msgBody = $(this).data("next");
  handlerNextItem(id, headerId, "NextItem", designId, msgBody);
});

// ==================================================FUNCTION================================================
// --------------------------------------------||Submit Function ||-------------------------------------------
// SUBMIT CHANGE STATUS
function submitChangeStatus() {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  );

  const fields = [
    "id",
    "status",
    "statusOld",
    "submitteddate",
    "completeddate",
    "canceleddate",
    "description",
  ];
  const paramsChangeStatus = { username: userName };

  fields.forEach((field) => {
    paramsChangeStatus[field] = document.querySelector(
      "#modalChangeStatus #" + field
    ).value;
  });

  $.ajax({
    type: "post",
    url: uriMethod + "/UpdateStatusOrder",
    data: JSON.stringify({ data: paramsChangeStatus }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    beforeSend: function () {
      $("#modalChangeStatus #submitChangeStatus").attr("disable", "disable");
      $("#modalChangeStatus #submitChangeStatus").html(
        '<i class="fa fa-spin fa-spinner"</i>'
      );
      swalLoadingShow("Please wait while we update the status.");
    },
    complete: function () {
      $("#modalChangeStatus #submitChangeStatus").removeAttr("disable");
      $("#modalChangeStatus #submitChangeStatus").html(
        `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit `
      );
    },
    success: function (response) {
      const result = response.d || response;
      // Swal.close();
      if (result.error) {
        isError(result.error.message.toUpperCase()).then(() => {
          const fieldElement = document.querySelector(result.error.field);
          if (fieldElement) {
            fieldElement.focus();
            fieldElement.classList.add("is-invalid");
          }
        });
      } else {
        isSuccess(result.success.message).then(() => {
          handlerHideBSModal("modalChangeStatus");
          location.reload();
        });
      }
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var msg =
        roleName === "Administrator"
          ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
          : "Something went wrong, please try again!";
      isError(msg);
    },
  });
  return false;
}

// SUBMIT SELECT PRODUCT
function submitSelectProduct(headerid, action, designid) {
  // VALIDATE FORM
  if (!headerid || !action || !designid) {
    if (roleName === "Administrator") {
      if (!headerid) {
        isError("HEADER ID NOT FOUND !").then(() => {
          const fieldElement = document.querySelector(
            "#modalAddItem #designid"
          );
          if (fieldElement) {
            fieldElement.focus();
            fieldElement.classList.add("is-invalid");
          }
        });
      }
      if (!action) {
        isError("ACTION NOT FOUND !").then(() => {
          const fieldElement = document.querySelector(
            "#modalAddItem #designid"
          );
          if (fieldElement) {
            fieldElement.focus();
            fieldElement.classList.add("is-invalid");
          }
        });
      }
      if (!designid) {
        isError("DESIGN ID NOT FOUND !").then(() => {
          const fieldElement = document.querySelector(
            "#modalAddItem #designid"
          );
          if (fieldElement) {
            fieldElement.focus();
            fieldElement.classList.add("is-invalid");
          }
        });
      }
      return;
    }
    if (!headerid) {
      isError("THIS ORDER IS MISSING !").then(() => {
        const fieldElement = document.querySelector("#modalAddItem #designid");
        if (fieldElement) {
          fieldElement.focus();
          fieldElement.classList.add("is-invalid");
        }
      });
    }
    if (!action) {
      isError("THIS ORDER IS MISSING !").then(() => {
        const fieldElement = document.querySelector("#modalAddItem #designid");
        if (fieldElement) {
          fieldElement.focus();
          fieldElement.classList.add("is-invalid");
        }
      });
    }
    if (!designid) {
      isError("PLEASE SELECT A PRODUCT !").then(() => {
        const fieldElement = document.querySelector("#modalAddItem #designid");
        if (fieldElement) {
          fieldElement.focus();
          fieldElement.classList.add("is-invalid");
        }
      });
    }
    return;
  }

  $.ajax({
    type: "POST",
    url: uriMethod + "/SetSessionOpenPageInputItem",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({
      id: "",
      headerid: headerid,
      action: action,
      designid: designid,
    }),
    success: function (response) {
      const result = response.d || response;
      var finePage = result.success.message.replace("~", "");
      window.location.href = finePage;
    },
    error: function (xhr, status, error) {
      var msg = xhr.status + "\n" + xhr.responseText + "\n" + error;
      // isError(msg);
      // return;
      if (roleName === "Administrator") {
        isError("Gagal menyetel session: " + error);
        return;
      }
      isError("Please contact our IT team at support@onlineorder.au");
    },
  });
}

// ------------------------------------------||Handler Function ||-------------------------------------------
// HANDLER HIDE BOOTSTRAP MODAL
function handlerHideBSModal(id) {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    // Jika modal belum pernah di-show dan belum punya instance, buat dan langsung hide
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
}

// HANDLER SHOW BOOTSTRAP MODAL
function handlerShowBSModal(params) {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
}

function handlerResetFormError(params) {
  // params : "#modalSaveData .form-control, #modalSaveData .form-select"
  document.querySelectorAll(params).forEach((element) => {
    element.classList.remove("is-invalid");
  });
}

// HANDLER DISPLAY ELEMENT
function handlerDisplayElement(item) {
  // INITIALIZE ELEMENTS
  const btnJobSheet = document.getElementById("btnJobSheet");
  const btnReprintJobSheet = document.getElementById("btnReprintJobSheet");
  const btnChangeJobStatus = document.getElementById("btnChangeJobStatus");
  const btnSubmit = document.getElementById("btnSubmit");
  const btnEditHeader = document.getElementById("btnEditHeader");
  const btnDeleteHeader = document.getElementById("btnDeleteHeader");
  const btnQuote = document.getElementById("btnQuote");
  const btnQuoteDetail = document.getElementById("btnQuoteDetail");
  const btnDownloadQuote = document.getElementById("btnDownloadQuote");
  const btnAdministrator = document.getElementById("btnAdministrator");
  const btnChangeStatus = document.getElementById("btnChangeStatus");
  const btnSendOrderMail = document.getElementById("btnSendOrderMail");
  const btnReloadPricing = document.getElementById("btnReloadPricing");
  const btnAddItem = document.getElementById("btnAddItem");
  const msgThanks = document.getElementById("msgThanks");

  //SET DEFAULT HIDE ELEMENT
  btnJobSheet.setAttribute("hidden", true);
  btnReprintJobSheet.setAttribute("hidden", true);
  btnChangeJobStatus.setAttribute("hidden", true);
  btnSubmit.setAttribute("hidden", true);
  btnEditHeader.setAttribute("hidden", true);
  btnDeleteHeader.setAttribute("hidden", true);
  btnQuote.setAttribute("hidden", true);
  btnQuoteDetail.setAttribute("hidden", true);
  btnDownloadQuote.setAttribute("hidden", true);
  btnAdministrator.setAttribute("hidden", true);
  btnChangeStatus.setAttribute("hidden", true);
  btnSendOrderMail.setAttribute("hidden", true);
  btnReloadPricing.setAttribute("hidden", true);
  btnAddItem.setAttribute("hidden", true);
  msgThanks.setAttribute("hidden", true);

  if (roleName === "Administrator") {
    // console.log(item);
  }

  if (item) {
    // btnJobSheet
    // if (item.Status !== "Draft" && item.Status !== "Canceled") {
    // }
    btnJobSheet.removeAttribute("hidden");
    if (roleName !== "Administrator" && roleName !== "PPIC & DE") {
      btnJobSheet.setAttribute("hidden", true);
    }

    // btnReprintJobSheet & btnChangeJobStatus
    if (item.JoNumber) {
      btnReprintJobSheet.removeAttribute("hidden");
      // btnChangeJobStatus.removeAttribute("hidden");
    }

    // btnSubmit, btnEditHeader, btnDeleteHeader, & btnAddItem
    if (item.Status === "Draft") {
      switch (roleName) {
        case "Customer":
          btnSubmit.removeAttribute("hidden");
          btnEditHeader.removeAttribute("hidden");
          btnDeleteHeader.removeAttribute("hidden");
          btnAddItem.removeAttribute("hidden");
          break;
        case "PPIC & DE":
          if (item.UserId.toUpperCase() === userId) {
            btnEditHeader.removeAttribute("hidden");
            btnDeleteHeader.removeAttribute("hidden");
            btnAddItem.removeAttribute("hidden");
          }
          break;
        case "Administrator":
          btnSubmit.removeAttribute("hidden");
          btnEditHeader.removeAttribute("hidden");
          btnDeleteHeader.removeAttribute("hidden");
          btnAddItem.removeAttribute("hidden");
          break;
      }
    }

    // btnQuote, btnQuoteDetail, & btnDownloadQuote
    if (roleName === "Administrator" || roleName === "Customer") {
      btnQuote.removeAttribute("hidden");
      btnQuoteDetail.removeAttribute("hidden");
      btnDownloadQuote.removeAttribute("hidden");
    }

    // btnAdministrator, btnChangeStatus, btnAddItem, & btnSendOrderMail
    switch (item.Status) {
      case "New Order":
      case "In Production":
      case "Completed":
      case "On Hold":
        msgThanks.removeAttribute("hidden");
        if (roleName === "Administrator" || roleName === "PPIC & DE") {
          btnAdministrator.removeAttribute("hidden");
          btnChangeStatus.removeAttribute("hidden");
          if (roleName === "Administrator") {
            btnSendOrderMail.removeAttribute("hidden");
            btnAddItem.removeAttribute("hidden");
          }
        }
        break;
    }

    // btnReloadPricing
    if (item.Status !== "Canceled") {
      btnReloadPricing.removeAttribute("hidden");
      if (roleName !== "Administrator") {
        btnReloadPricing.setAttribute("hidden", true);
      }
    }
  }
}

// HANDLER HEADER INFO
function handlerHeaderInfo(item) {
  // INITIALIZE ELEMENTS
  // CARD 1
  spanJoNumber = document.getElementById("spanJoNumber");
  spanOrderNo = document.getElementById("spanOrderNo");
  spanOrderCust = document.getElementById("spanOrderCust");
  spanCreatedDate = document.getElementById("spanCreatedDate");
  spanCreatedBy = document.getElementById("spanCreatedBy");
  spanNote = document.getElementById("spanNote");
  spanStatusNote = document.getElementById("spanStatusNote");
  spanStatusOrder = document.getElementById("spanStatusOrder");
  spanDelivery = document.getElementById("spanDelivery");

  // CARD 2
  spanSubmittedDate = document.getElementById("spanSubmittedDate");
  spanCompletedDate = document.getElementById("spanCompletedDate");
  spanCanceledDate = document.getElementById("spanCanceledDate");
  spanTotal = document.getElementById("spanTotal");
  spanGST = document.getElementById("spanGST");
  spanFinalTotal = document.getElementById("spanFinalTotal");

  // SET INFORMATION OR VALUES
  if (item) {
    // INITIALIZE DATE FORMATTER
    const us = {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    };
    const indo = {
      year: "numeric",
      month: "long",
      day: "2-digit",
    };
    // CARD 1
    spanJoNumber.innerHTML = item.JoNumber
      ? `<span class="badge badge-outline text-red">${item.JoNumber}</span>`
      : "-";
    spanOrderNo.innerHTML = item.OrderNo;
    spanOrderCust.innerHTML = item.OrderCust;

    // CreatedDate
    const customDate = parseCustomDate(item.CreatedDate);
    if (!customDate || isNaN(customDate.getTime())) {
      console.warn("Tanggal tidak valid:", item.CreatedDate);
      spanCreatedDate.innerHTML = "-";
      return;
    }
    if (roleName === "Administrator") {
      spanCreatedDate.innerHTML = customDate
        .toLocaleDateString("id-ID", us)
        .replace(/\./g, ":");
    } else {
      spanCreatedDate.innerHTML = customDate.toLocaleDateString("en-US", indo);
    }

    spanCreatedBy.innerHTML = item.UserName;
    spanNote.innerHTML = item.Note ? item.Note : "-";
    spanStatusNote.innerHTML = item.StatusDescription
      ? item.StatusDescription
      : "-";
    spanStatusOrder.innerHTML = item.Status;
    spanDelivery.innerHTML = item.Delivery;

    // CARD 2
    // SubmittedDate
    if (!item.SubmittedDate) spanSubmittedDate.innerHTML = "-";
    if (item.SubmittedDate) {
      const cardPrice = document.getElementById("cardPrice");
      cardPrice.classList.add("mb-3", "mt-1");

      const customDate = parseCustomDate(item.SubmittedDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.SubmittedDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (roleName === "Administrator") {
        spanSubmittedDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanSubmittedDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo
        );
      }
    }

    // CompletedDate
    if (!item.CompletedDate) spanCompletedDate.innerHTML = "-";
    if (item.CompletedDate) {
      const customDate = parseCustomDate(item.CompletedDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.CompletedDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (roleName === "Administrator") {
        spanCompletedDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanCompletedDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo
        );
      }
    }

    // CanceledDate
    if (!item.CanceledDate) spanCanceledDate.innerHTML = "-";
    if (item.CanceledDate) {
      const customDate = parseCustomDate(item.CanceledDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.CanceledDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (roleName === "Administrator") {
        spanCanceledDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanCanceledDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo
        );
      }
    }

    // CARD INFORMATION HEADER 2 | PRICES INFORMATION
    $.ajax({
      type: "POST",
      url: uriMethod + "/GetAmountPriceHeader",
      data: JSON.stringify({
        headerid: item.Id,
        pricesaccess: pricesAccess,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (!data) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerDisplayElement"
              : "Please contact our IT team at support@onlineorder.au";
          isError(msg);
          return;
        }

        spanTotal.innerHTML = data.amount
          ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${data.amount}</span>`
          : `<span style="font-size:larger;">0</span>`;

        spanGST.innerHTML = data.gst
          ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${data.gst}</span>`
          : `<span style="font-size:larger;">0</span>`;

        spanFinalTotal.innerHTML = data.finaltotal
          ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${data.finaltotal}</span>`
          : `<span style="font-size:larger;">0</span>`;
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        isError(msg);
      },
    });
  }
}

// HANDLER PREVIEW PRINT ORDER
function handlerCreatePDFOrder(headerid, action, msgloading) {
  // Tampilkan loading SweetAlert sebelum AJAX
  swalLoadingShow(msgloading);

  $.ajax({
    type: "post",
    url: uriMethod + "/CreatePDFOrder",
    data: JSON.stringify({
      headerid: headerid,
      action: action,
    }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",

    success: function (response) {
      Swal.close(); // Tutup loading Swal saat sukses

      const result = response.d || response;
      if (result.error) {
        isError(result.error.message.toUpperCase()).then(() => {
          location.reload();
        });
      } else {
        isSuccess(result.success.message).then(() => {
          if (action === "download") {
            window.location.href = result.success.url;
          } else if (action === "preview") {
            window.open(result.success.url, "_blank");
          } else if (action === "submit" || action === "mail") {
            location.reload();
          }
        });
      }
    },

    error: function (xhr, ajaxOptions, thrownError) {
      Swal.close(); // Tutup loading Swal saat error

      var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
      isError(msg);
    },
  });
}

// HANDLER SUBMIT ORDER HEADER
function handlerSubmitOrder(headerid, action, msgloading) {
  Swal.fire({
    title: "Are you sure?",
    html:
      "Sure to " +
      action +
      " this order? <br/>You won't be able to revert this!",
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, " + action + " it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        type: "post",
        url: uriMethod + "/SubmitOrder",
        data: JSON.stringify({
          headerid: headerid,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase(), result.error.field);
          } else {
            handlerCreatePDFOrder(headerid, action, msgloading);
          }
        },
        error: function (xhr, ajaxOptions, thrownError) {
          var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
          isError(msg);
        },
      });
    }
  });
}

// HANDLER CONVERT TO JOB
function handlerConvertToJob(headerid, action, msgloading) {
  Swal.fire({
    title: "Are you sure?",
    html: "Sure to convert this order to a job? <br /> This action cannot be undone.",
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, convert it!",
  }).then((result) => {
    if (result.isConfirmed) {
      const statusOrder = document.getElementById("spanStatusOrder").innerHTML;
      if (
        statusOrder === "Draft" ||
        statusOrder === "Completed" ||
        statusOrder === "Canceled"
      ) {
        isError(
          "Cannot convert this order as the status is <b>" +
            statusOrder +
            "</b>"
        );
      } else {
        handlerCreateJOBOrder(headerid, action, msgloading);
      }
    }
  });
}

// HANDLER CREATE JOB
function handlerCreateJOBOrder(headerid, action, msgloading) {
  // Tampilkan loading SweetAlert sebelum AJAX
  swalLoadingShow(msgloading);

  $.ajax({
    type: "post",
    url: uriMethod + "/CreateJOBOrder",
    data: JSON.stringify({
      headerid: headerid,
      action: action,
    }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",

    success: function (response) {
      Swal.close(); // Tutup loading Swal saat sukses

      const result = response.d || response;
      if (result.error) {
        isError(result.error.message.toUpperCase(), result.error.field);
      } else {
        isSuccess(result.success.message).then(() => {
          if (action === "download") {
            window.location.href = result.success.url;
          } else if (action === "reprint" || action === "preview") {
            window.open(result.success.url, "_blank");
          } else if (action === "convert") {
            location.reload();
          }
        });
      }
    },

    error: function (xhr, ajaxOptions, thrownError) {
      Swal.close(); // Tutup loading Swal saat error

      var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
      isError(msg);
    },
  });
}

// HANDLER EDIT HEADER
function handlerEditHeader(headerid) {
  $.ajax({
    type: "POST",
    url: uriMethod + "/SetSessionOpenEditOrderHeader",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({ headerid: headerid }),
    success: function () {
      window.location.href = "/order/header";
    },
    error: function (xhr, status, error) {
      isError("Gagal menyetel session: " + error);
    },
  });
}

// HANDLER DELETE HEADER
function handlerDeleteHeader(headerid) {
  Swal.fire({
    title: "Are you sure?",
    html: "Sure to delete this order?",
    icon: "warning",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      swalLoadingShow("Please wait while we delete the order.");
      $.ajax({
        type: "post",
        url: uriMethod + "/DeleteOrderHeader",
        data: JSON.stringify({
          id: headerid,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase(), result.error.field);
          } else {
            isSuccess(result.success.message).the(() => {
              window.location.href = "/order";
            });
          }
        },
        error: function (xhr, ajaxOptions, thrownError) {
          var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
          isError(msg);
        },
      });
    }
  });
}

// HANDLER CREATE PDF QUOTE
function handlerCreatePDFQuote(headerid, username, action, msgloading) {
  // Tampilkan loading SweetAlert sebelum AJAX
  swalLoadingShow(msgloading);

  $.ajax({
    type: "post",
    url: uriMethod + "/CreatePDFQuote",
    data: JSON.stringify({
      headerid: headerid,
      username: username,
      action: action,
    }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",

    success: function (response) {
      Swal.close(); // Tutup loading Swal saat sukses

      const result = response.d || response;
      if (result.error) {
        isError(result.error.message.toUpperCase(), result.error.field);
      } else {
        isSuccess(result.success.message).then(() => {
          if (action === "download") {
            window.location.href = result.success.url;
          } else if (action === "preview") {
            window.open(result.success.url, "_blank");
          }
        });
      }
    },

    error: function (xhr, ajaxOptions, thrownError) {
      Swal.close(); // Tutup loading Swal saat error

      var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
      isError(msg);
    },
  });
}

// HANDLER CHANGE STATUS
function handlerChangeStatus(headerid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();
    // console.log("bindItemOrder", headerid);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindOrderHeaderByID",
      data: JSON.stringify({
        headerid: headerid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerChangeStatus"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return Promise.resolve()
            .then(() =>
              handlerSelStatus("#modalChangeStatus #status", item.Status)
            )
            .then(() => setValueModalChangeStatus(item))
            .then(() => hanlderDisplayElementModalChangeStatus(item.Status))
            .then(() => {
              return Promise.all([handlerShowBSModal("modalChangeStatus")])
                .then(resolve)
                .catch(reject);
            });
        });

        Promise.all(promises)
          .then(() => resolve())
          .catch((error) => reject(error));
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// HANDLER CELECT STSTUS
function handlerSelStatus(params, statusNow) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    if (!params) return resolve();

    let data = [];

    // for cardChangeStatus => status
    if (params === "#modalChangeStatus #status" && statusNow) {
      switch (statusNow) {
        case "Draft":
          data = [
            { value: "New Order", text: "New Order" },
            { value: "Canceled", text: "Canceled" },
          ];

          if (roleName !== "Administrator") {
            // Tambahkan Draft di awal (unshift) atau akhir (push)
            data.unshift({ value: "Draft", text: "Draft" }); // Menambahkan di awal
          }
          break;
        case "New Order":
          data = [
            { value: "New Order", text: "New Order" },
            { value: "In Production", text: "In Production" },
            { value: "On Hold", text: "On Hold" },
            { value: "Canceled", text: "Canceled" },
          ];
          break;
        case "In Production":
          data = [
            { value: "In Production", text: "In Production" },
            { value: "Completed", text: "Completed" },
            { value: "Canceled", text: "Canceled" },
          ];
          break;
      }
      if (roleName === "Administrator") {
        // Tambahkan Draft di awal (unshift) atau akhir (push)
        data.unshift({ value: "Draft", text: "Draft" }); // Menambahkan di awal
      }
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      sel.appendChild(option);
    });

    // for cardOrder => status
    if (params === "#cardOrder #status") {
      const status = sel.options[sel.selectedIndex].value;
      const active = document.querySelector("#cardOrder #active").value;
      const storeType = document.querySelector("#cardOrder #storetype").value;
      // console.log("status: " + status);
      // console.log("active: " + active);
      // console.log("storeType: " + storeType);
      bindOrders(status, active, storeType);
    }

    resolve();
  });
}

// SET VALUE MODAL CHANGE STATUS
function setValueModalChangeStatus(itemData) {
  const mapping = {
    id: "Id",
    status: "Status",
    statusOld: "Status",
    submitteddate: "SubmittedDate",
    completeddate: "CompletedDate",
    canceleddate: "CanceledDate",
    description: "StatusDescription",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.querySelector("#modalChangeStatus #" + id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]];

    // Jika kosong/null/undefined, langsung set sebagai empty string
    if (!value) {
      el.value = "";
      return;
    }

    // Jika input type date dan format tanggal dd/mm/yyyy hh:mm:ss
    if (el.type === "date" && typeof value === "string") {
      const datePart = value.split(" ")[0]; // contoh: "16/07/2025"
      const parts = datePart.split("/"); // hasil: ["16", "07", "2025"]
      if (parts.length === 3) {
        value = `${parts[2]}-${parts[1]}-${parts[0]}`;
      } else {
        console.warn(`Format tanggal tidak sesuai: ${value}`);
        value = ""; // fallback kosong agar tidak error
      }
    }

    // Khusus untuk description, potong setelah "Notes from the office:<br />"
    if (id === "description" && typeof value === "string") {
      const marker = "Notes from the office:<br />";
      if (value.includes(marker)) {
        value = value.split(marker)[1] || "";
        // Hapus tag HTML jika masih ada
        value = value.replace(/<[^>]*>/g, "");
      }
    }

    el.value = value;
  });
}

// HANDLER DISPLAY ELEMENT MODAL CHANGE STATUS
function hanlderDisplayElementModalChangeStatus(status) {
  // INITIALIZE ELEMENT
  const divSubmittedDate = document.getElementById("divSubmittedDate");
  const divCompletedDate = document.getElementById("divCompletedDate");
  const divCanceledDate = document.getElementById("divCanceledDate");
  const divDescription = document.getElementById("divDescription");

  // SET DEFAULT HIDE ELEMENT
  divSubmittedDate.setAttribute("hidden", true);
  divCompletedDate.setAttribute("hidden", true);
  divCanceledDate.setAttribute("hidden", true);
  divDescription.setAttribute("hidden", true);

  if (status) {
    switch (status) {
      case "New Order":
        divSubmittedDate.removeAttribute("hidden");
        divDescription.removeAttribute("hidden");
        break;
      case "Completed":
        divCompletedDate.removeAttribute("hidden");
        divDescription.removeAttribute("hidden");
        break;
      case "Canceled":
        divCanceledDate.removeAttribute("hidden");
        divDescription.removeAttribute("hidden");
        break;
    }
  }
}

// HANDLER TOOLTIP
function handlerTooltip(modalName, params) {
  // INITIALIZE MESSAGE
  let title = "Tooltip";
  let msg = "This message is a tooltip";
  if (modalName === "modalChangeStatus") {
    switch (params) {
      case "New Order":
      case "Canceled":
        title = params + " Description";
        msg =
          "explain and write why you changed it to <b>" +
          params +
          "</b> status";
        break;
    }
  }

  Swal.fire({
    title: title,
    html: msg,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    icon: "question",
  });
}

// HANDLER RELOAD PRICING
function handlerReloadPricing(headerid, status, action) {
  Swal.fire({
    title: "Are you sure?",
    text: "Sure to reload the pricing?",
    icon: "warning",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, reload it!",
  }).then((result) => {
    if (result.isConfirmed) {
      handlerReloadPricingOnReadyPage(headerid, status, action);
    }
  });
}

// HANDLER RELOAD PRICING ON READY PAGE
function handlerReloadPricingOnReadyPage(headerid, status, action) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();

    if (action === "binding" && status !== "Draft") {
      return resolve();
    }

    if (action === "click") {
      swalLoadingShow("Please wait while we reload the pricing.");
    }

    $.ajax({
      type: "post",
      url: uriMethod + "/ReloadPricing",
      data: JSON.stringify({
        headerid: headerid,
      }),
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      success: function (response) {
        const result = response.d || response;
        if (result.error) {
          resolve(result.error.message); //console.log(result.error.message);
        } else {
          if (action === "binding") {
            if (roleName === "Administrator")
              console.log("result.success.message");
          } else if (action === "click") {
            isSuccess(result.success.message).then(() => {
              location.reload();
            });
          }
          resolve();
        }
      },

      error: function (xhr, ajaxOptions, thrownError) {
        reject(xhr.status + "\n" + xhr.responseText + "\n" + thrownError);
      },
    });
  });
}

// HANDLER SELECT DESIGN TYPE
function handlerSelDesignType(params) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindDesignType",
      //   data: JSON.stringify({
      //     designId: designId,
      //   }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerSelDesignType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          const defaultOption = document.createElement("option");
          defaultOption.text = "";
          defaultOption.value = "";
          sel.add(defaultOption);

          data.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            // option.setAttribute("data-page", item.page);
            sel.add(option);
          });
        }

        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// HANDLER EDIT ITEM
function handlerEditItem(id, headerid, action, designid) {
  // VALIDATE FORM
  if (!id || !headerid || !action || !designid) {
    if (roleName === "Administrator") {
      if (!id) {
        isError("ID NOT FOUND !");
      }
      if (!headerid) {
        isError("HEADER ID NOT FOUND !");
      }
      if (!action) {
        isError("ACTION NOT FOUND !");
      }
      if (!designid) {
        isError("DESIGN ID NOT FOUND !");
      }
      return;
    }
    isError("Please contact our IT team at support@onlineorder.au");
    return;
  }

  $.ajax({
    type: "POST",
    url: uriMethod + "/SetSessionOpenPageInputItem",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({
      id: id,
      headerid: headerid,
      action: action,
      designid: designid,
    }),
    success: function (response) {
      const result = response.d || response;
      var finePage = result.success.message.replace("~", "");
      window.location.href = finePage;
    },
    error: function (xhr, status, error) {
      if (roleName === "Administrator") {
        isError("Gagal menyetel session: " + error);
        return;
      }
      isError("Please contact our IT team at support@onlineorder.au");
    },
  });
}

// HANDLER COPY ITEM
function handlerCopyItem(id, product, msgloading) {
  Swal.fire({
    title: "Copy this item ?",
    html: product,
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, copy it!",
  }).then((result) => {
    if (result.isConfirmed) {
      swalLoadingShow(msgloading);
      $.ajax({
        type: "post",
        url: uriMethod + "/CopyItem",
        data: JSON.stringify({
          id: id,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase(), result.error.field);
          } else {
            isSuccess(result.success.message).then(() => {
              location.reload();
            });
          }
        },
        error: function (xhr, ajaxOptions, thrownError) {
          var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
          isError(msg);
        },
      });
    }
  });
}

// HANDLER DELETE ITEM
function handlerDeleteItem(id, product, msgloading) {
  Swal.fire({
    title: "Sure delete this item ?",
    html: product,
    icon: "warning",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      swalLoadingShow(msgloading);
      $.ajax({
        type: "post",
        url: uriMethod + "/DeleteItem",
        data: JSON.stringify({
          id: id,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase(), result.error.field);
          } else {
            isSuccess(result.success.message).then(() => {
              location.reload();
            });
          }
        },
        error: function (xhr, ajaxOptions, thrownError) {
          var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
          isError(msg);
        },
      });
    }
  });
}

// HANDLER PRICING ITEM
function handlerPricingItem(id) {
  if ($.fn.DataTable.isDataTable("#tablePricingDetail")) {
    $("#tablePricingDetail").DataTable().destroy(); // Hancurkan instance DataTables yang ada
  }

  const paramData = { id: id };

  const columnDefs = [
    {
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { data: "Description" },
    { data: "Cost" },
    { data: "FinalCost" },
  ];

  tablePricingData = $("#tablePricingDetail").DataTable({
    processing: true,
    serverSide: true, // <<< INI KUNCI PENTINGNYA
    order: [], // Tetap bisa set default order di sini
    pageLength: 25,
    initComplete: function () {
      $("#tablePricingDetail_filter").hide();
      $("#tablePricingDetail_length").hide();
      $("#tablePricingDetail_info").hide();
      $("#tablePricingDetail_paginate").hide();
    },
    ajax: {
      url: uriMethod + "/BindOrderPricingDetails",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: function (d) {
        return JSON.stringify({
          params: {
            ...paramData,
            draw: d.draw,
            start: d.start,
            length: d.length,
            order: d.order,
            columns: d.columns,
            search: d.search,
          },
        });
      },
      dataSrc: function (json) {
        json.recordsTotal = json.d.recordsTotal;
        json.recordsFiltered = json.d.recordsFiltered;
        // console.log(json);
        return json.d.data;
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    columns: columnDefs,
  });
}

// HANDLER NEXT ITEM
function handlerNextItem(id, headerid, action, designid, msgbody) {
  Swal.fire({
    title: "Information",
    html: msgbody,
    icon: "info",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, do it!",
  }).then((result) => {
    if (result.isConfirmed) {
      // VALIDATE FORM
      if (!id || !headerid || !action || !designid) {
        if (roleName === "Administrator") {
          if (!id) {
            isError("ID NOT FOUND !");
          }
          if (!headerid) {
            isError("HEADER ID NOT FOUND !");
          }
          if (!action) {
            isError("ACTION NOT FOUND !");
          }
          if (!designid) {
            isError("DESIGN ID NOT FOUND !");
          }
          return;
        }
        isError("Please contact our IT team at support@onlineorder.au");
        return;
      }

      $.ajax({
        type: "POST",
        url: uriMethod + "/SetSessionOpenPageInputItem",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
          id: id,
          headerid: headerid,
          action: action,
          designid: designid,
        }),
        success: function (response) {
          const result = response.d || response;
          var finePage = result.success.message.replace("~", "");
          window.location.href = finePage;
        },
        error: function (xhr, status, error) {
          if (roleName === "Administrator") {
            isError("Gagal menyetel session: " + error);
            return;
          }
          isError("Please contact our IT team at support@onlineorder.au");
        },
      });
    }
  });
}

// HANDLER CHEKC ORDER
function handlerCheckOrder(headerid, status, userid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();

    $.ajax({
      type: "post",
      url: uriMethod + "/CheckOrder",
      data: JSON.stringify({
        headerid: headerid,
        status: status,
        userid: userid,
      }),
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      success: function (response) {
        const result = response.d || response;
        if (result.error) {
          resolve(console.log(result.error.message));
        } else {
          if (result.success.url === "Yes") {
            Swal.fire({
              title: "Order Information",
              showClass: {
                popup: `
                animate__animated
                animate__fadeInUp
                animate__faster
              `,
              },
              hideClass: {
                popup: `
                animate__animated
                animate__fadeOutDown
                animate__faster
              `,
              },
              customClass: {
                popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
              },
              html: result.success.message,
              icon: "info",
            });
            return resolve();
          }

          resolve();
        }
      },

      error: function (xhr, ajaxOptions, thrownError) {
        reject(xhr.status + "\n" + xhr.responseText + "\n" + thrownError);
      },
    });
  });
}

// ------------------------------------------||Binding Function ||-------------------------------------------
// BIND ORDER HEADER
function bindOrderHeaderByID(headerid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();
    // console.log("bindItemOrder", headerid);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindOrderHeaderByID",
      data: JSON.stringify({
        headerid: headerid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : bindOrderHeaderByID"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return (
            Promise.resolve()
              // .then(() =>
              //   handlerReloadPricingOnReadyPage(item.Id, item.Status, "binding")
              // )
              .then(() => handlerDisplayElement(item))
              .then(() => handlerHeaderInfo(item))
              .then(() => bindDetails(item.Id, item.Status, item.UserId))
              .then(() => {
                return Promise.all([
                  handlerCheckOrder(item.Id, item.Status, item.UserId),
                ])
                  .then(resolve)
                  .catch(reject);
              })
          );
        });

        Promise.all(promises)
          .then(() => resolve())
          .catch((error) => reject(error));
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND ORDER DETAILS
function bindDetails(headerid, status, userid) {
  if ($.fn.DataTable.isDataTable("#tableAjax")) {
    $("#tableAjax").DataTable().destroy(); // Hancurkan instance DataTables yang ada
  }

  const paramData = {
    headerid: headerid,
    status: status,
    userid: userid,
  };

  // render: function (data, type, row, meta)
  const columnDefs = [
    {
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      data: null,
      orderable: true,
      render: (row) => `<div class="text-center">${row.Id}</div>`,
    },
    {
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { data: "Location" },
    {
      data: null,
      orderable: false,
      render: (row) => {
        // HIDE BUTTON NEXT
        let brNext = row.HideNext !== "hidden" ? "</br>" : "";
        return `
          ${row.Product}
          ${brNext}
          <button type="button" class="btn btn-sm btn-outline-success mt-1" id="btnNextItem" data-id="${row.Id}" data-designid="${row.DesignId}" data-next="${row.TextNext}" ${row.HideNext}>
            <i class="bi bi-node-plus me-1"></i>
            Next Item
          </button>
          `;
      },
    },
    { data: "Cost" },
  ];

  const thMarkUp = document.querySelectorAll(".thMarkUp");
  thMarkUp.forEach((el) => el.setAttribute("hidden", true));
  if (markupAccess === "True") {
    columnDefs.push({ data: "MarkUp" });
    thMarkUp.forEach((el) => el.removeAttribute("hidden"));
  }

  columnDefs.push({
    data: null,
    orderable: false,
    render: (row) => {
      // HIDE BUTTON DETAIL
      let hideDetail = "";
      if (row.StatusHeader === "Draft") {
        hideDetail = "hidden";
        if (roleName === "PPIC & DE" && userid !== row.UserId) {
          hideDetail = "";
        }
      }

      // HIDE BUTTON EDIT
      let hideEdit = "hidden";
      if (row.StatusHeader === "Draft") {
        hideEdit = "";
        if (roleName === "PPIC & DE" && userid !== row.UserId) {
          hideEdit = "hidden";
        }
      }

      // HIDE BUTTON COPY
      let hideCopy = "hidden";
      if (row.StatusHeader === "Draft") {
        hideCopy = "";
        if (roleName === "PPIC & DE" && userid !== row.UserId) {
          hideCopy = "hidden";
        } else if (roleName === "Manager" || roleName === "Account") {
          hideCopy = "hidden";
        }
      }

      // HIDE BUTTON DELETE
      let hideDelete = "hidden";
      if (row.StatusHeader === "Draft") {
        hideDelete = "";
        if (roleName === "PPIC & DE" && userid !== row.UserId) {
          hideDelete = "hidden";
        } else if (roleName === "Manager" || roleName === "Account") {
          hideDelete = "hidden";
        }
      }

      return `
      <div class="dropdown text-center">
        <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
          <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
        </button>
        <ul class="dropdown-menu dropdown-menu-end">
          <li ${hideDetail}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnDetailItem" data-id="${row.Id}"" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}">
              <i class="bi bi-info-circle me-2 opacity-50"></i>Detail
            </a>
          </li>
          <li ${hideEdit}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnEditItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}">
              <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
            </a>
          </li>
          <li ${hideCopy}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnCopyItem" data-id="${row.Id}" data-product="${row.Product}">
              <i class="bi bi-copy me-2 opacity-50"></i>Copy
            </a>
          </li>
          <li ${hideDelete}>
            <a class="dropdown-item text-danger" href="javascript:void(0);" id="btnDeleteItem" data-id="${row.Id}" data-product="${row.Product}">
              <i class="bi bi-trash3 me-2"></i>Delete
            </a>
          </li>
          <li>
            <a class="dropdown-item " href="javascript:void(0);" id="btnPricingItem" data-id="${row.Id}">
              <i class="bi bi-tags me-2 opacity-50"></i>Pricing
            </a>
          </li>
        </ul>
      </div>
    `;
    },
  });

  tableData = $("#tableAjax").DataTable({
    processing: true,
    serverSide: true, // <<< INI KUNCI PENTINGNYA
    order: [], // Tetap bisa set default order di sini
    pageLength: 100,
    initComplete: function () {
      $("#tableAjax_filter").hide();
      $("#tableAjax_length").hide();
      $("#tableAjax_info").hide();
      $("#tableAjax_paginate").hide();
    },
    ajax: {
      url: uriMethod + "/BindOrderDetails",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: function (d) {
        return JSON.stringify({
          params: {
            ...paramData,
            draw: d.draw,
            start: d.start,
            length: d.length,
            order: d.order,
            columns: d.columns,
            search: d.search,
          },
        });
      },
      dataSrc: function (json) {
        json.recordsTotal = json.d.recordsTotal;
        json.recordsFiltered = json.d.recordsFiltered;
        // console.log(json);
        return json.d.data;
      },
      complete: function () {
        loaderFadeOut(); // Loader disembunyikan setelah data Ajax berhasil
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    columns: columnDefs,
  });
}

// --------------------------------------------||Other Function ||-------------------------------------------
// CHECK SESSION
function checkSession() {
  setSessionAlive();

  bindOrderHeaderByID(headerId);
}

// FORMAT DATE TIME
function parseCustomDate(value) {
  if (!value || typeof value !== "string") return null;

  // Format ISO: 2025-07-10 08:42:01.653
  if (/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/.test(value)) {
    return new Date(value.replace(" ", "T"));
  }

  // Format: 10/07/2025 08:42:01 (24 jam)
  const match24 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{2}):(\d{2}):(\d{2})$/
  );
  if (match24) {
    const [_, day, month, year, hour, minute, second] = match24;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(
        2,
        "0"
      )}T${hour}:${minute}:${second}`
    );
  }

  // Format: 13/07/2025 1:06:07 PM (12 jam)
  const match12 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{1,2}):(\d{2}):(\d{2}) (\w{2})$/
  );
  if (match12) {
    let [_, day, month, year, hour, minute, second, period] = match12;
    hour = parseInt(hour, 10);
    if (period === "PM" && hour < 12) hour += 12;
    if (period === "AM" && hour === 12) hour = 0;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}T${hour
        .toString()
        .padStart(2, "0")}:${minute}:${second}`
    );
  }

  return null;
}
