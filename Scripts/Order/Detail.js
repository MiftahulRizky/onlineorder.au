window.addEventListener("DOMContentLoaded", function () {
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
  checkSessionDetail();
});
// ==================================================EVENTS==================================================
// ------------------------------------------||Card 1 Event ||-------------------------------------------
// BUTTON FINISH
document.querySelector("#btnFinish").addEventListener("click", () => {
  window.location.href = "/order";
});

// BUTTON PREVIEW PRINT
document.querySelector("#btnPreviewPrint").addEventListener("click", () => {
  handlerCreatePDFOrder(
    headerId,
    "preview",
    "Please wait while we generate the document."
  );
});

// BUTTON PREVIEW PDF
document.querySelector("#btnPreviewPDF").addEventListener("click", () => {
  handlerCreatePDFOrder(
    headerId,
    "download",
    "Please wait while we generate the document."
  );
});

// BUTTON CONVERT TO JOB
document.querySelector("#btnConvertToJob").addEventListener("click", () => {
  handlerConvertToJob(headerId, "convert", "Please wait while we convert...");
});

// BUTTON RE PRINT JOB SHEET
document.querySelector("#btnReprintJobSheet").addEventListener("click", () => {
  handlerCreateJOBOrder(headerId, "reprint", "Please wait while we reprint...");
});

// BUTTON SUBMIT ORDER
document.querySelector("#btnSubmit").addEventListener("click", () => {
  handlerSubmitOrder(
    headerId,
    "submit",
    "Please wait while we submit the order."
  );
});

// BUTTON EDIT HEADER
document.querySelector("#btnEditHeader").addEventListener("click", () => {
  // handlerEditHeader(headerId);
  window.location.href = "/order/create?arterix=edit&obelix=" + headerId;
});

// BUTTON DELETE HEADER
document.querySelector("#btnDeleteHeader").addEventListener("click", () => {
  handlerDeleteHeader(headerId);
});

// BUTTON QUOTE DETAIL
document.querySelector("#btnQuoteDetail").addEventListener("click", () => {
  handlerCreatePDFQuote(
    headerId,
    userName,
    "preview",
    "Please wait while we generate the document."
  );
});

// BUTTON DOWNLOAD QUOTE
document.querySelector("#btnDownloadQuote").addEventListener("click", () => {
  handlerCreatePDFQuote(
    headerId,
    userName,
    "download",
    "Please wait while we generate the document."
  );
});

// BUTTON CHANGE STSTUS
document.querySelector("#btnChangeStatus").addEventListener("click", () => {
  document
    .querySelectorAll(
      "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
    )
    .forEach((e) => {
      e.classList.remove("is-invalid");
    });
  handlerChangeStatus(headerId);
});

// BUTTON SEND MANUAL ORDER
document.querySelector("#btnSendOrderMail").addEventListener("click", () => {
  handlerCreatePDFOrder(
    headerId,
    "mail",
    "Please wait while we generate the document."
  );
});

// BTN RELOAD PRICING
document.querySelector("#btnReloadPricing").addEventListener("click", () => {
  const statusOrder = document.getElementById("spanStatusOrder").innerHTML;
  handlerReloadPricing(headerId, statusOrder, "click");
});

// BUTTON ADD ITEMS
document.querySelector("#btnAddItem").addEventListener("click", () => {
  document
    .querySelectorAll("#modalAddItem .form-control, #modalAddItem .form-select")
    .forEach((e) => {
      e.classList.remove("is-invalid");
    });

  handlerSelDesignType("#modalAddItem #designid");
  handlerShowBSModal("modalAddItem");
});

// ------------------------------------------||modalAddItem Event ||------------------------------------
// CHANGE DESIGN TYPE
document.querySelectorAll("#modalAddItem .form-select").forEach((e) => {
  e.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
  });
});

// BUTTON SUBMIT ADD ITEM
document
  .querySelector("#modalAddItem #submitAddItem")
  .addEventListener("click", () => {
    const designId = document.querySelector("#modalAddItem #designid").value;
    const action = "AddItem";
    submitSelectProduct(headerId, action, designId);
  });

// ------------------------------------------||modalChangeStatus Event ||------------------------------------
// CHANGE STATUS
document
  .querySelector("#modalChangeStatus #status")
  .addEventListener("change", (e) => {
    const status = e.target.value;
    hanlderDisplayElementModalChangeStatus(status);
  });

document
  .querySelectorAll(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  )
  .forEach((e) => {
    e.addEventListener("change", (e) => {
      e.target.classList.remove("is-invalid");
    });
    e.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

// TOOLTIP DESCRIPTION CLICK
document
  .querySelector("#modalChangeStatus #tooltipDescription")
  .addEventListener("click", () => {
    const status = document.querySelector("#modalChangeStatus #status").value;
    handlerTooltip("modalChangeStatus", status);
  });

// BUTTON SUBMIT CHANGE STATUS
document
  .querySelector("#modalChangeStatus #submitChangeStatus")
  .addEventListener("click", () => {
    submitChangeStatus();
  });

// ------------------------------------------||tableAjax Event ||------------------------------------
// BUTTON DETAIL ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDetailItem") {
    const id = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const headerid = e.target.dataset.headerid;
    handlerEditItem(id, headerid, "ViewItem", designid);
  }
});

document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnEditItem") {
    const id = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const headerid = e.target.dataset.headerid;
    handlerEditItem(id, headerid, "EditItem", designid);
  }
});

// BUTTON COPY ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnCopyItem") {
    const id = e.target.dataset.id;
    const product = e.target.dataset.product;
    handlerCopyItem(id, product, "Please wait while we copy the item...");
  }
});

// BUTTON DELETE ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDeleteItem") {
    const id = e.target.dataset.id;
    const product = e.target.dataset.product;
    handlerDeleteItem(id, product, "Please wait while we delete the item...");
  }
});

// BUTTON PRICING ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnPricingItem") {
    const id = e.target.dataset.id;
    handlerPricingItem(id);
    handlerShowBSModal("modalPricingItem");
  }
});

// HANDLER NEXT ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnNextItem") {
    const id = e.target.dataset.id;
    const designId = e.target.dataset.designid;
    const msgBody = e.target.dataset.next;
    handlerNextItem(id, headerId, "NextItem", designId, msgBody);
  }
});

// ==================================================FUNCTION================================================
// --------------------------------------------||Submit Function ||-------------------------------------------
// SUBMIT CHANGE STATUS
const submitChangeStatus = async () => {
  // Hapus semua tanda invalid di awal
  document
    .querySelectorAll("#modalChangeStatus .form-control")
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalChangeStatus #submitChangeStatus"
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
    const el = document.querySelector(`#modalChangeStatus #${field}`);
    paramsChangeStatus[field] = el ? el.value : "";
  });

  try {
    // === Sebelum request ===
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait while we update the status.");

    // === Kirim request ===
    const response = await fetch(`${uriMethod}/UpdateStatusOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: paramsChangeStatus }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    // === Setelah sukses ===
    if (data.error) {
      await isError(data.error.message.toUpperCase());
      const fieldElement = document.querySelector(data.error.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      await isSuccess(data.success.message);
      handlerHideBSModal("modalChangeStatus");
      location.reload();
    }
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  } finally {
    // === Setelah request selesai (sukses atau error) ===
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit`;
  }

  return false;
};

// SUBMIT SELECT PRODUCT
const submitSelectProduct = (headerid, action, designid) => {
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
};

// ------------------------------------------||Handler Function ||-------------------------------------------
// HANDLER HIDE BOOTSTRAP MODAL
const handlerHideBSModal = (id) => {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    // Jika modal belum pernah di-show dan belum punya instance, buat dan langsung hide
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
};

// HANDLER SHOW BOOTSTRAP MODAL
const handlerShowBSModal = (params) => {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
};

// HANDLER DISPLAY ELEMENT
const handlerDisplayElement = (item) => {
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
  const divPrice = document.getElementById("divPrice");
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
  divPrice.setAttribute("hidden", true);
  msgThanks.setAttribute("hidden", true);

  if (!item) return;

  // btnJobSheet
  btnJobSheet.removeAttribute("hidden");
  if (roleName !== "Administrator" && roleName !== "PPIC & DE") {
    btnJobSheet.setAttribute("hidden", true);
  }

  // btnReprintJobSheet & btnChangeJobStatus
  if (item.JoNumberId) {
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
};

// HANDLER HEADER INFO
const handlerHeaderInfo = (item) => {
  // INITIALIZE ELEMENTS
  // CARD 1
  const spanJoNumber = document.getElementById("spanJoNumber");
  const spanOrderNo = document.getElementById("spanOrderNo");
  const spanOrderCust = document.getElementById("spanOrderCust");
  const spanCreatedDate = document.getElementById("spanCreatedDate");
  const spanCreatedBy = document.getElementById("spanCreatedBy");
  const spanNote = document.getElementById("spanNote");
  const spanStatusNote = document.getElementById("spanStatusNote");
  const spanStatusOrder = document.getElementById("spanStatusOrder");
  const spanDelivery = document.getElementById("spanDelivery");

  // CARD 2
  const spanSubmittedDate = document.getElementById("spanSubmittedDate");
  const spanCompletedDate = document.getElementById("spanCompletedDate");
  const spanCanceledDate = document.getElementById("spanCanceledDate");
  const spanTotal = document.getElementById("spanTotal");
  const spanGST = document.getElementById("spanGST");
  const spanFinalTotal = document.getElementById("spanFinalTotal");

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
    spanJoNumber.innerHTML = item.JoNumberId
      ? `<span class="badge badge-outline text-red">${item.JoNumberId}</span>`
      : "-";
    spanOrderNo.innerHTML = item.OrderNumber;
    spanOrderCust.innerHTML = item.OrderName;

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

    // GET CREATED BY
    fetch(`${uriMethod}/GetCreatedBy`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        id: item.CreatedBy,
      }),
    })
      .then((response) => {
        if (!response.ok) {
          // Error dari server (misal 404, 500)
          throw new Error(`${response.status} ${response.statusText}`);
        }
        return response.json();
      })
      .then((response) => {
        const data = response.d;
        if (!data) {
          const msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerDisplayElement"
              : "Please contact our IT team at support@onlineorder.au";
          isError(msg);
          return;
        }

        spanCreatedBy.innerHTML = data.createdby ? data.createdby : "??????";
      })
      .catch((error) => {
        const msg =
          roleName === "Administrator"
            ? error.message
            : "Please contact our IT team at support@onlineorder.au";
        isError(msg);
      });

    // CARD INFORMATION HEADER 2 | PRICES INFORMATION
    fetch(`${uriMethod}/GetAmountPriceHeader`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: item.Id,
        pricesaccess: pricesAccess,
      }),
    })
      .then((response) => {
        if (!response.ok) {
          // Error dari server (misal 404, 500)
          throw new Error(`${response.status} ${response.statusText}`);
        }
        return response.json();
      })
      .then((response) => {
        const data = response.d;
        if (!data) {
          const msg =
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
      })
      .catch((error) => {
        const msg =
          roleName === "Administrator"
            ? error.message
            : "Please contact our IT team at support@onlineorder.au";
        isError(msg);
      });
  }
};

// HANDLER PREVIEW PRINT ORDER
const handlerCreatePDFOrder = async (headerid, action, msgloading) => {
  // Tampilkan loading SweetAlert
  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${uriMethod}/CreatePDFOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: headerid,
        action: action,
      }),
    });

    Swal.close(); // Tutup loading Swal setelah respons diterima

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      await isError(result.error.message.toUpperCase());
      location.reload();
    } else {
      await isSuccess(result.success.message);

      if (action === "download") {
        window.location.href = result.success.url;
      } else if (action === "preview") {
        window.open(result.success.url, "_blank");
      } else if (action === "submit" || action === "mail") {
        location.reload();
      }
    }
  } catch (error) {
    Swal.close(); // Tutup loading Swal saat error
    const msg = error.message || "Something went wrong while creating the PDF.";
    isError(msg);
  }
};

// HANDLER SUBMIT ORDER HEADER
const handlerSubmitOrder = async (headerid, action, msgloading) => {
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
  }).then(async (result) => {
    if (result.isConfirmed) {
      try {
        const response = await fetch(`${uriMethod}/SubmitOrder`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json; charset=utf-8",
          },
          body: JSON.stringify({
            headerid: headerid,
          }),
        });

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();
        const resultData = data.d || data;

        if (resultData.error) {
          isError(
            resultData.error.message.toUpperCase(),
            resultData.error.field
          );
        } else {
          handlerCreatePDFOrder(headerid, action, msgloading);
        }
      } catch (error) {
        const msg =
          error.message || "Something went wrong while submitting the order.";
        isError(msg);
      }
    }
  });
};

// HANDLER CONVERT TO JOB
const handlerConvertToJob = async (headerid, action, msgloading) => {
  const result = await Swal.fire({
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
  });

  if (!result.isConfirmed) return;

  const statusOrder = document
    .getElementById("spanStatusOrder")
    ?.innerHTML?.trim();

  if (!statusOrder) {
    return isError("Order status not found.");
  }

  if (["Draft", "Completed", "Canceled"].includes(statusOrder)) {
    await isError(
      `Cannot convert this order as the status is <b>${statusOrder}</b>`
    );
    return;
  }

  // Jika semua valid → lanjut buat job
  await handlerCreateJOBOrder(headerid, action, msgloading);
};

// HANDLER CREATE JOB
const handlerCreateJOBOrder = async (headerid, action, msgloading) => {
  // Tampilkan loading SweetAlert sebelum request
  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${uriMethod}/CreateJOBOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: headerid,
        action: action,
      }),
    });

    // Tutup loading setelah menerima response
    Swal.close();

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      isError(result.error.message.toUpperCase(), result.error.field);
    } else {
      await isSuccess(result.success.message);

      if (action === "download") {
        window.location.href = result.success.url;
      } else if (action === "reprint" || action === "preview") {
        window.open(result.success.url, "_blank");
      } else if (action === "convert") {
        location.reload();
      }
    }
  } catch (error) {
    Swal.close();
    const msg =
      error.message || "Something went wrong while creating JOB Order.";
    isError(msg);
  }
};

// HANDLER EDIT HEADER
const handlerEditHeader = async (headerid) => {
  try {
    const response = await fetch(`${uriMethod}/SetSessionOpenEditOrderHeader`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    // Jika sukses, arahkan ke halaman edit order
    window.location.href = "/order/header";
  } catch (error) {
    isError("Gagal menyetel session: " + error.message);
  }
};

// HANDLER DELETE HEADER
const handlerDeleteHeader = async (headerid) => {
  const result = await Swal.fire({
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
  });

  if (!result.isConfirmed) return;

  swalLoadingShow("Please wait while we delete the order.");

  try {
    const response = await fetch(`${uriMethod}/DeleteOrderHeader`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id: headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(
        resultData.error.message.toUpperCase(),
        resultData.error.field
      );
    } else {
      await isSuccess(resultData.success.message);
      window.location.href = "/order";
    }
  } catch (error) {
    const msg =
      error.message || "Something went wrong while deleting the order.";
    isError(msg);
  }
};

// HANDLER CREATE PDF QUOTE
const handlerCreatePDFQuote = async (
  headerid,
  username,
  action,
  msgloading
) => {
  try {
    // Tampilkan loading SweetAlert
    swalLoadingShow(msgloading);

    const response = await fetch(`${uriMethod}/CreatePDFQuote`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid, username, action }),
    });

    Swal.close(); // Tutup loading Swal setelah respons diterima

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    if (data.error) {
      isError(data.error.message.toUpperCase(), data.error.field);
    } else {
      await isSuccess(data.success.message);

      if (action === "download") {
        window.location.href = data.success.url;
      } else if (action === "preview") {
        window.open(data.success.url, "_blank");
      }
    }
  } catch (error) {
    Swal.close(); // Tutup loading Swal jika error
    isError(`Gagal membuat PDF Quote: ${error.message}`);
  }
};

// HANDLER CHANGE STATUS
const handlerChangeStatus = async (headerid) => {
  try {
    if (!headerid) return;

    const response = await fetch(`${uriMethod}/BindOrderHeaderByID`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerChangeStatus"
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    for (const item of data) {
      await handlerSelStatus("#modalChangeStatus #status", item.Status);
      await setValueModalChangeStatus(item);
      await hanlderDisplayElementModalChangeStatus(item.Status);
      await handlerShowBSModal("modalChangeStatus");
    }
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// HANDLER CELECT STSTUS
const handlerSelStatus = async (params, statusNow) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!params) return;

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
          data.unshift({ value: "Draft", text: "Draft" });
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
      data.unshift({ value: "Draft", text: "Draft" });
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

    await bindOrders(status, active, storeType);
  }
};

// SET VALUE MODAL CHANGE STATUS
const setValueModalChangeStatus = (itemData) => {
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
};

// HANDLER DISPLAY ELEMENT MODAL CHANGE STATUS
const hanlderDisplayElementModalChangeStatus = (status) => {
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
};

// HANDLER TOOLTIP
const handlerTooltip = (modalName, params) => {
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
};

// HANDLER RELOAD PRICING
const handlerReloadPricing = async (headerid, status, action) => {
  try {
    const result = await Swal.fire({
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
    });

    if (result.isConfirmed) {
      await handlerReloadPricingOnReadyPage(headerid, status, action);
    }
  } catch (error) {
    console.error("Error reloading pricing:", error);
    isError("Failed to reload pricing: " + error.message);
  }
};

// HANDLER RELOAD PRICING ON READY PAGE
const handlerReloadPricingOnReadyPage = async (headerid, status, action) => {
  if (!headerid) return;

  if (action === "binding" && status !== "Draft") {
    return;
  }

  if (action === "click") {
    swalLoadingShow("Please wait while we reload the pricing.");
  }

  try {
    const response = await fetch(`${uriMethod}/ReloadPricing`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      return result.error.message; // sama seperti resolve(result.error.message)
    } else {
      if (action === "binding") {
        if (roleName === "Administrator") {
          console.log(result.success.message);
        }
      } else if (action === "click") {
        await isSuccess(result.success.message);
        location.reload();
      }
    }
  } catch (error) {
    console.error("Reload pricing failed:", error);
    isError(error.message);
  }
};

// HANDLER SELECT DESIGN TYPE
const handlerSelDesignType = async (params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  try {
    const response = await fetch(`${uriMethod}/BindDesignType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d;

    if (!result || result.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerSelDesignType"
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
      return Promise.reject(msg);
    }

    if (Array.isArray(result)) {
      sel.innerHTML = ""; // reset ulang

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      result.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        sel.add(option);
      });
    }

    return Promise.resolve();
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
    return Promise.reject(msg);
  }
};

// HANDLER EDIT ITEM
const handlerEditItem = async (id, headerid, action, designid) => {
  if (!id || !headerid || !action || !designid) {
    if (roleName === "Administrator") {
      if (!id) await isError("ID NOT FOUND!");
      if (!headerid) await isError("HEADER ID NOT FOUND!");
      if (!action) await isError("ACTION NOT FOUND!");
      if (!designid) await isError("DESIGN ID NOT FOUND!");
      return;
    }

    await isError("Please contact our IT team at support@onlineorder.au");
    return;
  }

  try {
    const response = await fetch(`${uriMethod}/SetSessionOpenPageInputItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        id,
        headerid,
        action,
        designid,
      }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    const finePage = result.success.message.replace("~", "");
    window.location.href = finePage;
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? "Gagal menyetel session: " + error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

// HANDLER COPY ITEM
const handlerCopyItem = async (id, product, msgloading) => {
  const result = await Swal.fire({
    title: "Copy this item?",
    html: product,
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, copy it!",
  });

  if (!result.isConfirmed) return;

  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${uriMethod}/CopyItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    Swal.close();

    if (resultData.error) {
      await isError(
        resultData.error.message.toUpperCase(),
        resultData.error.field
      );
    } else {
      await isSuccess(resultData.success.message);
      location.reload();
    }
  } catch (error) {
    Swal.close();
    const msg =
      roleName === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  }
};

// HANDLER DELETE ITEM
async function handlerDeleteItem(id, product, msgloading) {
  const result = await Swal.fire({
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
  });

  if (!result.isConfirmed) return;

  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${uriMethod}/DeleteItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id }),
    });

    if (!response.ok) {
      const msg = `${response.status}\n${response.statusText}`;
      throw new Error(msg);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      isError(resultData.error.message.toUpperCase(), resultData.error.field);
    } else {
      await isSuccess(resultData.success.message);
      location.reload();
    }
  } catch (error) {
    isError(error.message || "An unexpected error occurred");
  }
}

// HANDLER PRICING ITEM
const handlerPricingItem = (id) => {
  const paramData = { id: id };

  const columnDefs = [
    {
      width: "5%",
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { width: "60%", data: "Description" },
    { width: "15%", data: "Cost" },
    { width: "15%", data: "FinalCost" },
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
};

// HANDLER NEXT ITEM
const handlerNextItem = async (id, headerid, action, designid, msgbody) => {
  const result = await Swal.fire({
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
  });

  if (!result.isConfirmed) return;

  // VALIDATE FORM
  if (!id || !headerid || !action || !designid) {
    if (roleName === "Administrator") {
      if (!id) isError("ID NOT FOUND !");
      if (!headerid) isError("HEADER ID NOT FOUND !");
      if (!action) isError("ACTION NOT FOUND !");
      if (!designid) isError("DESIGN ID NOT FOUND !");
      return;
    }
    isError("Please contact our IT team at support@onlineorder.au");
    return;
  }

  try {
    const response = await fetch(`${uriMethod}/SetSessionOpenPageInputItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        id,
        headerid,
        action,
        designid,
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    const finePage = resultData.success.message.replace("~", "");
    window.location.href = finePage;
  } catch (error) {
    if (roleName === "Administrator") {
      isError("Gagal menyetel session: " + error.message);
    } else {
      isError("Please contact our IT team at support@onlineorder.au");
    }
  }
};

// HANDLER CHEKC ORDER
const handlerCheckOrder = async (headerid, status, userid) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${uriMethod}/CheckOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid,
        status,
        userid,
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      console.log(result.error.message);
      return;
    }

    if (result.success.url === "Yes") {
      await Swal.fire({
        title: "Order Information",
        html: result.success.message,
        icon: "info",
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
      });
    }
  } catch (error) {
    isError(`${error.message}`);
  }
};

// ------------------------------------------||Binding Function ||-------------------------------------------
// BIND ORDER HEADER
const bindOrderHeaderByID = async (headerid, ordertype) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${uriMethod}/BindOrderHeaderByID`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, ordertype }),
    });

    if (!response.ok) {
      const msg =
        roleName === "Administrator"
          ? `${response.status} - ${response.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    const dataResponse = await response.json();
    const data = dataResponse.d;

    if (!data || data.length === 0) {
      window.location.href = "/order";
    }

    for (const item of data) {
      // await handlerReloadPricingOnReadyPage(item.Id, item.Status, "binding");
      await handlerDisplayElement(item);
      await handlerHeaderInfo(item);
      await bindDetails(item.Id, item.Status, item.CreatedBy);

      await handlerCheckOrder(item.Id, item.Status, item.CreatedBy);
    }
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// BIND ORDER DETAILS
let tableData;
const bindDetails = (headerid, status, userid) => {
  const paramData = {
    headerid: headerid,
    status: status,
    userid: userid,
  };

  // render: function (data, type, row, meta)
  const columnDefs = [
    {
      width: "5%",
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: true,
      render: (row) => `<div class="text-center">${row.Id}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { width: "20%", data: "Location" },
    {
      width: "60%",
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
  ];

  const thPricing = document.querySelectorAll(".thPrice");
  thPricing.forEach((el) => el.setAttribute("hidden", true));
  if (pricesAccess === "True" || pricesAccess === "1") {
    columnDefs.push({ width: "5%", data: "Cost" });
    thPricing.forEach((el) => el.removeAttribute("hidden"));
  }

  const thMarkUp = document.querySelectorAll(".thMarkUp");
  thMarkUp.forEach((el) => el.setAttribute("hidden", true));
  if (markupAccess === "True" || markupAccess === "1") {
    columnDefs.push({ width: "5%", data: "MarkUp" });
    thMarkUp.forEach((el) => el.removeAttribute("hidden"));
  }

  columnDefs.push({
    width: "5%",
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
        <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
          <li ${hideDetail}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnDetailItem" data-id="${row.Id}"" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}">
              <i class="ti ti-info-square-rounded fs-2 me-1 opacity-50"></i>Detail
            </a>
          </li>
          <li ${hideEdit}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnEditItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}">
              <i class="ti ti-edit me-1 fs-2 opacity-50"></i>Edit
            </a>
          </li>
          <li ${hideCopy}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnCopyItem" data-id="${row.Id}" data-product="${row.Product}">
              <i class="ti ti-copy-plus fs-2 me-1 opacity-50"></i>Copy
            </a>
          </li>
          <li ${hideDelete}>
            <a class="dropdown-item text-danger" href="javascript:void(0);" id="btnDeleteItem" data-id="${row.Id}" data-product="${row.Product}">
              <i class="ti ti-trash-x me-1 fs-2 opacity-50"></i>Delete
            </a>
          </li>
          <div class="dropdown-divider"></div>
          <li>
            <a class="dropdown-item " href="javascript:void(0);" id="btnPricingItem" data-id="${row.Id}">
              <i class="ti ti-tags fs-1 me-1 opacity-50"></i>Pricing
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
};

// --------------------------------------------||Other Function ||-------------------------------------------
// CHECK SESSION
const checkSessionDetail = () => {
  bindOrderHeaderByID(headerId, INFYNITY);
};

// FORMAT DATE TIME
const parseCustomDate = (value) => {
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
};
