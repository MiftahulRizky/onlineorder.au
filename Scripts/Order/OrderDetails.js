document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Detail.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("CUSTOMERID: " + CUSTOMERID);
    console.log("USERNAME: " + USERNAME);
    console.log("HEADERID: " + HEADERID);
    console.log("PRICEACCESS: " + PRICEACCESS);
    console.log("CUSTOMERCONTACTID: " + CUSTOMERCONTACTID);
    console.log("PREVIEWACCESS: " + PREVIEWACCESS);
    console.log("REPRINT: " + REPRINT);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  orderDetailPageLoaded();
});

// ==============================================|| INITIALIZATION ||========================================
let DataTableDetails;
const spanEl = {
  retailerName: document.getElementById("spanRetailerName"),
  retailerId: document.getElementById("spanRetailerId"),
  orderId: document.getElementById("spanOrderId"),
  joNumber: document.getElementById("spanJoNumber"),
  orderType: document.getElementById("spanOrderProductType"),
  orderNo: document.getElementById("spanOrderNo"),
  orderCust: document.getElementById("spanOrderCust"),
  createdDate: document.getElementById("spanCreatedDate"),
  createdBy: document.getElementById("spanCreatedBy"),
  note: document.getElementById("spanNote"),
  statusNote: document.getElementById("spanStatusNote"),
  statusOrder: document.getElementById("spanStatusOrder"),
  delivery: document.getElementById("spanDelivery"),
  submittedDate: document.getElementById("spanSubmittedDate"),
  productionDate: document.getElementById("spanProductionDate"),
  completedDate: document.getElementById("spanCompletedDate"),
  canceledDate: document.getElementById("spanCanceledDate"),
  total: document.getElementById("spanTotal"),
  gst: document.getElementById("spanGST"),
  final: document.getElementById("spanFinalTotal"),
};
const btnEl = {
  btnFinish: document.getElementById("btnFinish"),
  btnPreviewPrint: document.getElementById("btnPreviewPrint"),
  btnPreviewPDF: document.getElementById("btnPreviewPDF"),
  btnJobSheet: document.getElementById("btnJobSheet"),
  btnConvertToJob: document.getElementById("btnConvertToJob"),
  btnReprintJobSheet: document.getElementById("btnReprintJobSheet"),
  btnChangeJobStatus: document.getElementById("btnChangeJobStatus"),
  btnSubmit: document.getElementById("btnSubmit"),
  btnEditHeader: document.getElementById("btnEditHeader"),
  btnDeleteHeader: document.getElementById("btnDeleteHeader"),
  btnQuote: document.getElementById("btnQuote"),
  btnQuoteDetail: document.getElementById("btnQuoteDetail"),
  btnDownloadQuote: document.getElementById("btnDownloadQuote"),
  btnMoreAction: document.getElementById("btnMoreAction"),
  btnReloadPricing: document.getElementById("btnReloadPricing"),
  btnChangeStatus: document.getElementById("btnChangeStatus"),
  btnSendOrderMail: document.getElementById("btnSendOrderMail"),
  btnDownloadBarcode: document.getElementById("btnDownloadBarcode"),
  btnQuoteDisc: document.getElementById("btnQuoteDisc"),
  dividerPrintQuote: document.getElementById("dividerPrintQuote"),
  btnPrintQuote: document.getElementById("btnPrintQuote"),
  btnEmailQuote: document.getElementById("btnEmailQuote"),
  dividerEmailDeposit: document.getElementById("dividerEmailDeposit"),
  btnEmailDeposit: document.getElementById("btnEmailDeposit"),
  dividerLogs: document.getElementById("dividerLogs"),
  btnLogs: document.getElementById("btnLogs"),
  btnAddItem: document.getElementById("btnAddItem"),
  btnAddService: document.getElementById("btnAddService"),
  divPrice: document.getElementById("divPrice"),
  msgThanks: document.getElementById("msgThanks"),
  thMarkUp: document.querySelector(".thMarkUp"),
  thPrice: document.querySelector(".thPrice"),
};
const liEl = {
  liDetailItem: document.getElementsByClassName("liDetailItem"),
  liEditItem: document.getElementsByClassName("liEditItem"),
  liCopyItem: document.getElementsByClassName("liCopyItem"),
  liDeleteItem: document.getElementsByClassName("liDeleteItem"),
  liEditPricingItem: document.getElementsByClassName("liEditPricingItem"),
  liPricingItem: document.getElementsByClassName("liPricingItem"),
  liDivider: document.getElementsByClassName("liDivider"),
  liDividerBarcode: document.getElementsByClassName("liDividerBarcode"),
  liDownloadBarcodeItem: document.getElementsByClassName(
    "liDownloadBarcodeItem",
  ),
};
const elChangeStatus = {
  divSubmittedDate: document.getElementById("divSubmittedDate"),
  divCompletedDate: document.getElementById("divCompletedDate"),
  divCanceledDate: document.getElementById("divCanceledDate"),
  divDescription: document.getElementById("divDescription"),
};
const elModal = {
  modalChangeStatus: document.getElementById("modalChangeStatus"),
  modalQuoteDisc: document.getElementById("modalQuoteDisc"),
  modalAddItem: document.getElementById("modalAddItem"),
  modalAddService: document.getElementById("modalAddService"),
  modalSendMailQuote: document.getElementById("modalSendMailQuote"),
  modalEditPricingItem: document.getElementById("modalEditPricingItem"),
  modalEditPricingAllItem: document.getElementById("modalEditPricingAllItem"),
  modalLogs: document.getElementById("modalLogs"),
};

// ==============================================|| EVENTS ||================================================
// Button Event
Object.values(btnEl).forEach((el) => {
  if (!el) return;
  el.addEventListener("click", async (e) => {
    try {
      const id = e.currentTarget.id;

      if (id === "btnFinish") {
        window.location.href = "/order";
      }

      if (id === "btnPreviewPrint") {
        handlerCreatePDFOrder(HEADERID, "preview");
      }

      if (id === "btnPreviewPDF") {
        handlerCreatePDFOrder(HEADERID, "download");
      }

      if (id === "btnConvertToJob") {
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
          if (!result.isConfirmed) return;
          const StatusOrder =
            document.getElementById("spanStatusOrder").innerHTML;
          if (!StatusOrder) throw new Error("Status Order not found.");

          if (["Completed", "Canceled"].includes(StatusOrder.trim())) {
            throw new Error(
              `Cannot convert this order as the status is <b>${statusOrder}</b>`,
            );
          }

          const loadMsg = "Please wait while we convert";
          handlerCreateJOBOrder(HEADERID, "convert", loadMsg);
        });
      }

      if (id === "btnReprintJobSheet") {
        const loadMsg = "Please wait while we reprint";
        handlerCreateJOBOrder(HEADERID, "reprint", loadMsg);
      }

      if (id === "btnSubmit") {
        const loadMsg = "Please wait while we submit the order.";
        handlerSubmitOrder(HEADERID, "submit", loadMsg);
      }

      if (id === "btnEditHeader") {
        window.location.href = `/order/header?action=edit&param=${HEADERID}&ordertype=${ORDERTYPE}`;
      }

      if (id === "btnDeleteHeader") {
        handlerDeleteHeader(HEADERID);
      }

      if (id === "btnQuoteDetail") {
        const loadMsg = "Please wait while we generate the document.";
        handlerCreatePDFCustomerQuote(HEADERID, USERNAME, "preview", loadMsg);
      }

      if (id === "btnDownloadQuote") {
        const loadMsg = "Please wait while we generate the document.";
        handlerCreatePDFCustomerQuote(HEADERID, USERNAME, "download", loadMsg);
      }

      if (id === "btnReloadPricing") {
        const status = document.getElementById("spanStatusOrder").innerHTML;
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
          if (!result.isConfirmed) return;

          handlerReloadPricingOnReadyPage(HEADERID, status, "click");
        });
      }

      if (id === "btnChangeStatus") {
        handlerShowBSModal("modalChangeStatus");
      }

      if (id === "btnSendOrderMail") {
        handlerCreatePDFOrder(HEADERID, "mail");
      }

      if (id === "btnDownloadBarcode") {
        handlerDownloadBarcode(HEADERID, "");
      }

      if (id === "btnQuoteDisc") {
        handlerShowBSModal("modalQuoteDisc");
      }

      if (id === "btnPrintQuote") {
        handlerPrintQuote(HEADERID, "preview");
      }

      if (id === "btnEmailQuote") {
        document
          .querySelectorAll(
            "#modalSendMailQuote .form-control, #modalSendMailQuote .form-select",
          )
          .forEach((e) => {
            e.classList.remove("is-invalid");
          });
        handlerShowBSModal("modalSendMailQuote");
      }

      if (id === "btnLogs") {
        handlerShowBSModal("modalLogs");
      }

      if (id === "btnCopyJoNumber") {
        alert("Copy to clipboard");
      }
    } catch (error) {
      const msg = `Event btnEl: ${error.message}`;
      catchMessages(msg);
    }
  });
});

// Event on modal
const modalHandlers = {
  modalChangeStatus: (modal) => {
    modal.addEventListener("change", (e) => {
      e.target.classList.remove("is-invalid");
      const id = e.target.id;

      if (id === "status") {
        const status = e.target.value;
        hanlderDisplayElementModalChangeStatus(status);
      }
    });

    modal.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });

    modal.addEventListener("click", (e) => {
      const id = e.target.id;
      if (id === "tooltipDescription") {
        const status = document.querySelector(
          "#modalChangeStatus #status",
        ).value;
        let title = `${status} Description`;
        let msg = `explain and write why you changed it to <b>${status}</b> status`;

        Swal.fire({
          title: title,
          html: msg,
          customClass: {
            popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
          },
          icon: "question",
        });
      }

      if (id === "submitChangeStatus") {
        submitChangeStatus();
      }
    });
  },

  modalSendMailQuote: (modal) => {
    modal.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });

    modal.addEventListener("click", (e) => {
      const id = e.target.id;
      if (id === "btnSendMailQuote") {
        submitSendMailQuote();
      }
    });
  },

  modalQuoteDisc: (modal) => {
    modal.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });

    modal.addEventListener("click", (e) => {
      const id = e.target.id;
      if (id === "btnSubmitOverrideDisc") {
        document
          .querySelectorAll(
            "#modalQuoteDisc .form-control, #modalQuoteDisc .form-select",
          )
          .forEach((el) => {
            el.classList.remove("is-invalid");
          });
        submitOverrideDisc(id);
      }
    });
  },
};
Object.entries(elModal).forEach(([key, modal]) => {
  if (!modal) return;

  if (modalHandlers[key]) {
    modalHandlers[key](modal);
  }
});

// ============================================|| FUNCTION ||================================================
// ------------------------------------------||Binding Function ||-------------------------------------------
const bindOrderAggregate = async (headerid, ordertype) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindOrderAggregate`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          headerid,
          ordertype,
          loginid: LOGINID,
          rolename: ROLENAME,
          customercontactid: CUSTOMERCONTACTID,
          applicationid: APPLICATIONID,
        },
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const { d: data } = await response.json();

    if (!data) {
      throw new Error("No data");
    }

    if (data.error) {
      throw new Error(data.message);
    }

    console.log(data);

    handlerHeaderInfo(data.header);
    bindDetails(data.detail);
    handlerDisplayElement(data.header, data.detail);
    handlerCheckOrder(data.header.ResCheckOrder);
    handlerChangeStatus(data.header);
    handlerSetRandomElementValues(data.header, data.detail, data.other);
  } catch (error) {
    let msg = `bindOrderAggregate: ${error.message}`;
    catchMessages(msg);
  }
};

const bindDetails = (details) => {
  try {
    if (!details || details.length === 0) return;

    // if ($.fn.DataTable.isDataTable("#tableAjax")) {
    //   $("#tableAjax").DataTable().clear().destroy();
    // }

    const columnDefs = [
      {
        width: "5%",
        data: null,
        className: "text-center",
        render: (data, type, row, meta) => meta.row + 1,
      },
      { width: "5%", data: "Id", className: "text-center" },
      { width: "5%", data: "Qty", className: "text-center" },
      { width: "20%", data: "Location" },
      { width: "60%", data: "Product" },
      { width: "5%", data: "Cost", className: "thPrice" },
      { width: "5%", data: "Markup", className: "thMarkUp" },
      {
        width: "5%",
        data: null,
        className: "text-center",
        orderable: false,
        render: (data, type, row) => {
          return `
            <div class="dropdown text-center">
              <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
              </button>
              <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
                <li class="liDetailItem">
                  <a class="dropdown-item" href="javascript:void(0);" id="btnDetailItem" data-id="${row.Id}"" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}" data-designname="${row.DesignName}" data-production='${row.Production}'>
                    <i class="ti ti-info-square-rounded me-1 opacity-50 fs-2"></i>Detail
                  </a>
                </li>
                <li class="liEditItem">
                  <a class="dropdown-item" href="javascript:void(0);" id="btnEditItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}" data-designname="${row.DesignName}" data-production='${row.Production}'>
                  <i class="ti ti-edit me-1 opacity-50 fs-2"></i>Edit
                  </a>
                </li>
                <li class="liCopyItem">
                  <a class="dropdown-item" href="javascript:void(0);" id="btnCopyItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-product="${row.Product}" >
                    <i class="ti ti-copy-plus me-1 opacity-50 fs-2"></i>Copy
                  </a>
                </li>
                <li class="liDeleteItem">
                  <a class="dropdown-item text-danger" href="javascript:void(0);" id="btnDeleteItem" data-id="${row.Id}" data-product="${row.Product}">
                    <i class="ti ti-trash-x me-1 opacity-50 fs-2"></i>Delete
                  </a>
                </li>
                <div class="dropdown-divider liDivider"></div>
                <li class="liEditPricingItem">
                  <a class="dropdown-item " href="javascript:void(0);" id="btnEditPricingItem" data-id="${row.Id}" data-cost="${row.RealCost}" data-designid="${row.DesignId}" data-blindid="${row.BlindId}" data-qty="${row.Qty}">
                    <i class="ti ti-pencil-dollar text-success fs-1 me-1 opacity-50"></i>Edit Pricing
                  </a>
                </li>
                <li class="liPricingItem">
                  <a class="dropdown-item " href="javascript:void(0);" id="btnPricingItem" data-id="${row.Id}">
                    <i class="ti ti-tags me-1 opacity-50 fs-2"></i>Pricing
                  </a>
                </li>
                <div class="dropdown-divider liDividerBarcode"></div>
                <li class="liDownloadBarcodeItem">
                  <a class="dropdown-item " href="javascript:void(0);" id="btnDownloadBarcodeItem" data-id="${row.Id}" >
                    <i class="ti ti-file-barcode fs-2 me-1 opacity-50"></i>Download Barcode
                  </a>
                </li>
              </ul>
            </div>
          `;
        },
      },
    ];

    DataTableDetails = $("#tableAjax").DataTable({
      data: details,
      pageLength: 100,
      responsive: true,
      bPaginate: true,
      bInfo: true,
      bFilter: true,
      bDestroy: true,
      autoWidth: false,
      columns: columnDefs,
      language: {
        search: "",
        lengthMenu: "_MENU_",
      },
      initComplete: () => {
        stylingColumnSearchAndPaging("#tableAjax");
      },
    });
  } catch (error) {
    const msg = `bindDetails: ${error.message}`;
    catchMessage(msg);
  }
};

// ----------------------------------------------|| Handler Functions ||-------------------------------------
const handlerHeaderInfo = (item) => {
  try {
    if (!item) return;

    setText(spanEl.retailerName, item.CustomerName);
    setText(spanEl.retailerId, item.CustomerId);
    setText(spanEl.orderId, item.OrderId);
    setText(spanEl.orderNo, item.OrderNumber);
    setText(spanEl.orderCust, item.OrderName);
    setText(spanEl.note, item.OrderNote);
    setText(spanEl.statusNote, item.StatusAdditional);
    setText(spanEl.statusOrder, item.Status);
    setText(spanEl.delivery, item.Delivery);

    setText(spanEl.createdBy, item.CreatedByName);
    spanEl.createdDate.textContent = formatDate(item.CreatedDate);

    spanEl.submittedDate.textContent = formatDate(item.SubmittedDate);
    spanEl.productionDate.textContent = formatDate(item.JobDate);
    spanEl.completedDate.textContent = formatDate(item.CompletedDate);
    spanEl.canceledDate.textContent = formatDate(item.CanceledDate);

    spanEl.joNumber.innerHTML = item.JoNumberId
      ? `<span class="badge badge-outline text-red">${item.JoNumberId}</span> <a href="javascript:void(0);" id="btnCopyJoNumber" class="btn btn-sm  border-0 bg-transparent" data-jonumber="${item.JoNumberId}"><i class="ti ti-copy fs-2 opacity-50"></i></a>`
      : "-";

    setText(spanEl.orderType, item.OrderType);

    spanEl.total.innerHTML = formatCurrency(item.SumPrice);
    spanEl.gst.innerHTML = formatCurrency(item.Gst);
    spanEl.final.innerHTML = formatCurrency(item.FinalTotal);
  } catch (error) {
    const msg = `handlerHeaderInfo : ${error.message}`;
    catchMessages(msg);
  }
};

const handlerDisplayElement = (header, detail) => {
  try {
    Object.values(btnEl).forEach((el) => {
      if (el) el.classList.add("d-none");
    });
    if (!header || !detail) return;

    DataTableDetails?.columns(5)?.visible(false);
    DataTableDetails?.columns(6)?.visible(false);

    btnEl.btnFinish.classList.remove("d-none");
    btnEl.btnPreviewPrint.classList.remove("d-none");
    btnEl.btnPreviewPDF.classList.remove("d-none");
    btnEl.btnConvertToJob.classList.remove("d-none");

    btnEl.dividerPrintQuote.classList.remove("d-none");
    btnEl.btnPrintQuote.classList.remove("d-none");
    btnEl.btnEmailQuote.classList.remove("d-none");

    if (header.JoNumberId) {
      btnEl.btnReprintJobSheet.classList.remove("d-none");
    }

    if (["Administrator"].includes(ROLENAME)) {
      btnEl.btnJobSheet.classList.remove("d-none");

      if (["Draft", "Pending Price Approval"].includes(header.Status)) {
        btnEl.btnSubmit.classList.remove("d-none");
        btnEl.btnEditHeader.classList.remove("d-none");
        btnEl.btnDeleteHeader.classList.remove("d-none");
        btnEl.btnAddItem.classList.remove("d-none");
      }

      if (!["Completed"].includes(header.Status)) {
        btnEl.btnAddService.classList.remove("d-none");
        btnEl.btnQuoteDisc.classList.remove("d-none");
      }
      btnEl.btnDownloadBarcode.classList.remove("d-none");

      btnEl.btnQuote.classList.remove("d-none");
      btnEl.btnQuoteDetail.classList.remove("d-none");
      btnEl.btnDownloadQuote.classList.remove("d-none");

      if (
        ["New Order", "In Production", "Completed", "On Hold"].includes(
          header.Status,
        )
      ) {
        btnEl.btnChangeStatus.classList.remove("d-none");
        btnEl.btnSendOrderMail.classList.remove("d-none");
      }

      btnEl.btnMoreAction.classList.remove("d-none");

      btnEl.btnEmailDeposit.classList.remove("d-none");
      btnEl.dividerEmailDeposit.classList.remove("d-none");

      if (!["Canceled"].includes(header.Status)) {
        btnEl.btnReloadPricing.classList.remove("d-none");
      }

      btnEl.dividerLogs.classList.remove("d-none");
      btnEl.btnLogs.classList.remove("d-none");
    }

    if (["PPIC & DE", "Customer Service"].includes(ROLENAME)) {
      btnEl.btnJobSheet.classList.remove("d-none");

      if (["Draft", "Pending Price Approval"].includes(header.Status)) {
        btnEl.btnSubmit.classList.remove("d-none");
        btnEl.btnEditHeader.classList.remove("d-none");
        btnEl.btnDeleteHeader.classList.remove("d-none");
        btnEl.btnAddItem.classList.remove("d-none");
      }

      if (!["Completed"].includes(header.Status)) {
        btnEl.btnAddService.classList.remove("d-none");
        btnEl.btnQuoteDisc.classList.remove("d-none");
      }
      btnEl.btnDownloadBarcode.classList.remove("d-none");

      if (
        ["New Order", "In Production", "Completed", "On Hold"].includes(
          header.Status,
        )
      ) {
        btnEl.btnChangeStatus.classList.remove("d-none");
      }

      btnEl.btnMoreAction.classList.remove("d-none");

      if (!["Canceled"].includes(header.Status)) {
        btnEl.btnReloadPricing.classList.remove("d-none");
      }
    }

    if (["Administrator"].includes(ROLENAME)) {
      if (["Draft", "Pending Price Approval"].includes(header.Status)) {
        btnEl.btnSubmit.classList.remove("d-none");
        btnEl.btnEditHeader.classList.remove("d-none");
        btnEl.btnDeleteHeader.classList.remove("d-none");
        btnEl.btnAddItem.classList.remove("d-none");
      }

      btnEl.btnQuote.classList.remove("d-none");
      btnEl.btnQuoteDetail.classList.remove("d-none");
      btnEl.btnDownloadQuote.classList.remove("d-none");
    }

    // ----------------------------------------------|| Hide Button Datatable ||---------------------------------------
    Object.values(liEl).forEach((el) => {
      Array.from(el).forEach((li) => {
        li.classList.add("d-none");
      });
    });

    if (["Draft", "Pending Price Approval"].includes(header.Status)) {
      ["liEditItem", "liCopyItem", "liDeleteItem"].forEach((key) => {
        Array.from(liEl[key]).forEach((li) => {
          li.classList.remove("d-none");
        });
      });

      if (
        ["PPIC & DE", "Customer Service", "Manager", "Account"].includes(
          ROLENAME,
        ) &&
        header.CreatedBy.toUpperCase() !== LOGINID.toUpperCase()
      ) {
        ["liEditItem", "liCopyItem", "liDeleteItem"].forEach((key) => {
          Array.from(liEl[key]).forEach((li) => {
            li.classList.add("d-none");
          });
        });

        ["liDetailItem"].forEach((key) => {
          Array.from(liEl[key]).forEach((li) => {
            li.classList.add("d-none");
          });
        });
      }
    }

    if (["Additional", "Surcharge"].includes(detail.DesignName)) {
      ["liDeleteItem"].forEach((key) => {
        Array.from(liEl[key]).forEach((li) => {
          li.classList.remove("d-none");
        });
      });
    }

    let hideEditPricing = "True";
    let hidePricing = "True";
    if (["True", "1"].includes(PRICEACCESS)) {
      DataTableDetails?.columns(5).visible(true);
      btnEl.thPrice.classList.remove("d-none");
      btnEl.divPrice.classList.remove("d-none");

      if (
        ["Administrator", "PPIC & DE", "Customer Service"].includes(ROLENAME)
      ) {
        [
          "liEditPricingItem",
          "liDividerBarcode",
          "liDownloadBarcodeItem",
        ].forEach((key) => {
          Array.from(liEl[key]).forEach((li) => {
            li.classList.remove("d-none");
          });
        });
        hideEditPricing = "False";
      }

      ["liPricingItem"].forEach((key) => {
        Array.from(liEl[key]).forEach((li) => {
          li.classList.remove("d-none");
        });
      });
      hidePricing = "False";
    }

    if (["True", "1"].includes(MARKUPACCESS)) {
      DataTableDetails?.columns(6).visible(true);
      btnEl.thMarkUp.classList.remove("d-none");
    }

    if (hideEditPricing == "False" && hidePricing == "False") {
      ["liDivider"].forEach((key) => {
        Array.from(liEl[key]).forEach((li) => {
          li.classList.remove("d-none");
        });
      });
    }
  } catch (error) {
    const msg = `handlerDisplayElement: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerCheckOrder = (res) => {
  try {
    if (!res) return;
    if (!["Yes"].includes(res.Action)) return;

    Swal.fire({
      title: "Order Information",
      html: res.Message,
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
  } catch (error) {
    const msg = `handlerCheckOrder: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerCreatePDFOrder = async (headerid, action) => {
  swalLoadingShow("Please wait while we generate the document.");
  try {
    const response = await fetch(`${PDFORDERMETHOD}/CreatePDFOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid,
        action,
      }),
    });
    Swal.close();
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);

      if (action === "download") {
        window.location.href = res.url;
      } else if (action === "preview") {
        window.open(res.url, "_blank");
      } else if (action === "submit" || action === "mail") {
        location.reload();
      }
    }
  } catch (error) {
    const msg = `handlerCreatePDFOrder: ${error.message}`;
    cathcMessages(msg);
  }
};

const handlerCreateJOBOrder = async (headerid, action, msgloading) => {
  swalLoadingShow(msgloading);
  try {
    const response = await fetch(`${JOBSHEETMETHOD}/CreateJOBOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: headerid,
        action: action,
      }),
    });
    swal.close();

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);

      if (action === "download") {
        window.location.href = res.url;
      } else if (action === "reprint" || action === "preview") {
        window.open(res.url, "_blank");
      } else if (action === "convert") {
        location.reload();
      }
    }
  } catch (error) {
    const msg = `handlerCreateJOBOrder: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerSubmitOrder = async (headerid, action, msgloading) => {
  Swal.fire({
    title: "Are you sure?",
    html: `Sure to ${action} this order? <br/>You won't be able to revert this!`,
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
        const response = await fetch(`${URIMETHOD}/SubmitOrder`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json; charset=utf-8",
          },
          body: JSON.stringify({
            headerid: headerid,
            loginid: LOGINID,
            rolename: ROLENAME,
          }),
        });

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();
        const res = data.d || data;

        if (res.warning) {
          await isWarning(res.message.toUpperCase());
        } else if (res.error) {
          throw new Error(res.message);
        } else if (res.success) {
          handlerCreatePDFOrder(headerid, action, msgloading);
        }
      } catch (error) {
        const msg = `handlerSubmitOrder: ${error.message}`;
        catchMessages(msg);
      }
    }
  });
};

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
    const response = await fetch(`${URIMETHOD}/DeleteOrderHeader`, {
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
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message);
    } else if (res.success) {
      await isSuccess(res.message);
      window.location.href = "/order";
    }
  } catch (error) {
    const msg = `handlerDeleteHeader: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerCreatePDFCustomerQuote = async (
  headerid,
  username,
  action,
  msgloading,
) => {
  try {
    swalLoadingShow(msgloading);

    const response = await fetch(`${PDFORDERMETHOD}/CreatePDFCustomerQuote`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid, username, action }),
    });

    Swal.close();

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);

      if (action === "download") {
        window.location.href = res.url;
      } else if (action === "preview") {
        window.open(res.url, "_blank");
      }
    }
  } catch (error) {
    const msg = `handlerCreatePDFCustomerQuote: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerReloadPricingOnReadyPage = async (headerid, status, action) => {
  if (!headerid) return;

  if (
    ["binding"].includes(action) &&
    !["Draft", "Pending Price Approval"].includes(status)
  ) {
    return;
  }

  if (["click"].includes(action)) {
    swalLoadingShow("Please wait while we reload the pricing.");
  }

  try {
    const response = await fetch(`${URIMETHOD}/ReloadPricing`, {
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
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      if (action === "binding") {
        if (["Administrator"].includes(ROLENAME)) {
          console.log(res.message);
        }
      } else if (["click"].includes(action)) {
        await isSuccess(res.message);
        location.reload();
      }
    }
  } catch (error) {
    const msg = `handlerReloadPricingOnReadyPage: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerChangeStatus = (header) => {
  try {
    document
      .querySelectorAll(
        "#modalChangeStatus .form-control, #modalChangeStatus .form-select",
      )
      .forEach((e) => {
        e.classList.remove("is-invalid");
      });
    bindStatus("#modalChangeStatus #status", header.Status);
    setValueModalChangeStatus(header);
    hanlderDisplayElementModalChangeStatus(header.Status);
    // await handlerShowBSModal("modalChangeStatus");
  } catch (error) {
    const msg = `handlerChangeStatus: ${error.message}`;
    catchMessages(msg);
  }
};

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

const hanlderDisplayElementModalChangeStatus = (status) => {
  Object.values(elChangeStatus).forEach((el) => {
    if (el) el.classList.add("d-none");
  });

  if (status) {
    elChangeStatus.divDescription.classList.remove("d-none");
    switch (status) {
      case "New Order":
        elChangeStatus.divSubmittedDate.classList.remove("d-none");
        break;
      case "Completed":
        elChangeStatus.divCompletedDate.classList.remove("d-none");
        break;
      case "Canceled":
        divCanceledDate.removeAttribute("hidden");
        elChangeStatus.divCanceledDate.classList.remove("d-none");
        break;
    }
  }
};

const handlerSetRandomElementValues = (header, detail, other) => {
  try {
    // Ovveride Customer Discount
    document.querySelector("#modalQuoteDisc #discount").value =
      header.QuoteDisc || 0;

    // Send Mail Quote
    document.querySelector("#modalSendMailQuote #id").value =
      other.SendMailQuoteId;
    document.querySelector("#modalSendMailQuote #from").value =
      other.SendMailQuoteFrom;
    document.querySelector("#modalSendMailQuote #mailto").value =
      other.SendMailQuoteTo;

    // Logs
    const table = document.querySelector("#modalLogs #table-logs tbody");
    table.innerHTML = "";

    const logs = other?.Logs || [];
    if (logs.length === 0) {
      const tr = document.createElement("tr");
      tr.innerHTML = `
      <td class="text-center">
        No logs found
      </td>
    `;
      table.appendChild(tr);
    }

    if (logs.length > 0) {
      logs.forEach((log) => {
        const formattedDate = formatDotNetDate(log.ActionDate);

        const tr = document.createElement("tr");
        tr.innerHTML = `
          <td>
            <b>${log.FullName}</b> on ${formattedDate}. Action: ${log.Description}
          </td>
        `;
        table.appendChild(tr);
      });
    }
  } catch (error) {
    const msg = `handlerSetRandomElementValues: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerDownloadBarcode = async (headerid, itemid) => {
  swalLoadingShow("Please wait while we download the barcode.");
  try {
    const response = await fetch(`${URIMETHOD}/DownloadBarcode`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, itemid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;
    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);
      window.open(res.url, "_blank");
    }
  } catch (error) {
    const msg = `handlerDownloadBarcode: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerPrintQuote = async (headerid, action) => {
  swalLoadingShow("Please wait while we print the quote.");
  try {
    const response = await fetch(`${PDFORDERMETHOD}/CreatePDFQuote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, action, username: USERNAME }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;
    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);
      window.open(res.url, "_blank");
    }
  } catch (error) {
    const msg = `handlerPrintQuote: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| Binding function ||-------------------------------------
const bindStatus = (params, statusNow) => {
  if (!params || !statusNow) return;
  let list = ["Draft"];

  // if(["Pending Price Approval"].includes(statusNow)) {
  //   list.push("Draft")
  // }

  if (["Draft"].includes(statusNow)) {
    list.push("New Order", "Canceled");
  }

  if (["On Hold"].includes(statusNow)) {
    list.push("In Production", "On Hold", "Canceled");
  }

  if (["New Order"].includes(statusNow)) {
    list.push("New Order", "In Production", "On Hold", "Canceled");
  }

  if (["In Production"].includes(statusNow)) {
    list.push("In Production", "On Hold", "Completed", "Canceled");
  }

  generateOption(params, list);
};

// ----------------------------------------------|| Submit Functions ||--------------------------------------
const submitChangeStatus = async () => {
  document
    .querySelectorAll("#modalChangeStatus .form-control")
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalChangeStatus #submitChangeStatus",
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

  const paramsChangeStatus = { username: USERNAME, loginid: LOGINID };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalChangeStatus #${field}`);
    paramsChangeStatus[field] = el ? el.value : "";
  });

  try {
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait while we update the status.");

    const response = await fetch(`${URIMETHOD}/UpdateStatusOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: paramsChangeStatus }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
      const fieldElement = document.querySelector(res.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else if (res.success) {
      await isSuccess(res.message);
      handlerHideBSModal("modalChangeStatus");
      location.reload();
    }
  } catch (error) {
    const msg = `submitChangeStatus: ${error.message}`;
    catchMessages(msg);
  } finally {
    // === Setelah request selesai (sukses atau error) ===
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit`;
  }

  return false;
};

const submitOverrideDisc = async (button) => {
  try {
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = ["discount"];

    const formData = {
      headerid: HEADERID,
      loginid: LOGINID,
      rolename: ROLENAME,
    };

    fields.forEach((field) => {
      formData[field] = document.querySelector(
        `#modalQuoteDisc #${field}`,
      ).value;
    });

    // swal.close();
    // return console.table(formData);

    const response = await fetch(URIMETHOD + "/SubmitOverrideDisc", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: formData }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`${response.status}\n${errorText}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message?.toUpperCase());
    } else if (res.success) {
      let msg = res.message;
      msg += `<br/> <br/> Do you want to reload the pricing?`;
      const statusOrder = document.getElementById("spanStatusOrder").innerHTML;

      Swal.fire({
        title: "Success!",
        html: msg,
        icon: "success",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        customClass: {
          popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
        },
      }).then((result) => {
        if (result.isConfirmed) {
          handlerReloadPricingOnReadyPage(HEADERID, statusOrder, "click");
        } else {
          location.reload();
        }
        handlerHideBSModal("modalQuoteDisc");
      });
    }
  } catch (error) {
    const msg = `submitOverrideDisc: ${error.message}`;
  } finally {
    document.getElementById(button).innerHTML = "Save Changes";
  }
};

const submitSendMailQuote = async () => {
  // Hapus semua tanda invalid di awal
  document
    .querySelectorAll(
      "#modalSendMailQuote .form-control, #modalSendMailQuote .form-select",
    )
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalSendMailQuote #btnSendMailQuote",
  );

  const fields = ["id", "from", "mailto", "cc"];

  const additionalParams = {
    username: USERNAME,
    headerid: HEADERID,
    loginid: LOGINID,
  };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalSendMailQuote #${field}`);
    additionalParams[field] = el ? el.value : "";
  });

  try {
    // === Sebelum request ===
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait a moment");

    // === Kirim request ===
    const response = await fetch(`${URIMETHOD}/SubmitSendMailQuote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: additionalParams }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
      const fieldElement = document.querySelector(res.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else if (res.success) {
      handlerHideBSModal("modalSendMailQuote");
      await isSuccess(res.message);
    }
  } catch (error) {
    const msg = `submitSendMailQuote: ${error.message}`;
    catchMessages(msg);
  } finally {
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `Send`;
  }

  return false;
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const orderDetailPageLoaded = async () => {
  try {
    if (!ULTRON || !ORDERTYPE) window.location.href = "/order";

    if (CUSTOMERID == "LS-A224") window.location.href = "/order"; // JPM Direct

    if (CUSTOMERID == "DEFAULT" && USERNAME == "galih") {
      window.location.href = "/order";
    }

    if (!["Administrator"].includes(ROLENAME)) {
      window.location.href = "/order";
    }

    await bindOrderAggregate(HEADERID, ORDERTYPE);
    await loaderFadeOut();
  } catch (error) {
    const msg = `orderDetailPageLoaded: ${error.message}`;
    catchMessages(msg);
  }
};

const getItemData = async (query) => {
  try {
    const response = await fetch(`${URIMETHOD}/GetItemData`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: query }), // ✅ FIX
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};

const bindSelect = async ({
  elementId,
  field,
  params = {},
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  const select = document.getElementById(elementId);
  select.innerHTML = "";

  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field,
          ...params,
        },
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`${response.status}\n${text}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!Array.isArray(data)) {
      throw new Error(`No data returned from server : ${field}`);
    }

    select.innerHTML = "";

    // default option
    if (withDefaultOption && data.length > lengthDefaultOption) {
      const opt = document.createElement("option");
      opt.value = "";
      opt.text = "";
      select.add(opt);
    }

    // render options
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      option.setAttribute("data-name", item.text);
      select.add(option);
    });

    select.classList.add("fw-bold");

    // callback setelah render
    if (afterRender) {
      await afterRender(data, select);
    }

    // kalau cuma 1 data
    if (data.length === 1 && onSingle) {
      select.selectedIndex = 0;
      await onSingle(data[0], select);
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  // const sel = document.getElementById(elementId);
  const sel = document.querySelector(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  if (!["#modalChangeStatus #status"].includes(elementId)) {
    list.sort();
  }

  // default option kalau lebih dari 1 data
  if (list.length > lengthDefaultOption) {
    const defaultOption = new Option("", "");
    sel.add(defaultOption);
  }

  list.forEach((item) => {
    const option = new Option(item.toUpperCase(), item);
    option.setAttribute("data-name", item);
    sel.add(option);
  });
};

const setText = (el, val) => {
  if (!el) return;
  el.innerHTML = val || "-";
};

const formatDate = (dateStr) => {
  if (!dateStr) return "-";

  const d = parseCustomDate(dateStr);
  if (!d || isNaN(d.getTime())) return "-";

  return ROLENAME === "Administrator"
    ? d
        .toLocaleDateString("id-ID", {
          weekday: "long",
          year: "numeric",
          month: "long",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
          hour12: false,
        })
        .replace(/\./g, ":")
    : d.toLocaleDateString("en-US", {
        year: "numeric",
        month: "long",
        day: "2-digit",
      });
};

const formatCurrency = (num) => {
  if (!num) return "-";
  let result = `$${Number(num).toLocaleString("en-US", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`;
  return `<span class="badge badge-outline text-green" style="font-size:larger;">${result}</span>`;
};

const formatCurrencyDetail = (value) => {
  if (!value) return "$0.00";

  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(parseFloat(value));
};

const parseCustomDate = (value) => {
  if (!value || typeof value !== "string") return null;

  // Format ISO: 2025-07-10 08:42:01.653
  if (/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/.test(value)) {
    return new Date(value.replace(" ", "T"));
  }

  // Format: 10/07/2025 08:42:01 (24 jam)
  const match24 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{2}):(\d{2}):(\d{2})$/,
  );
  if (match24) {
    const [_, day, month, year, hour, minute, second] = match24;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(
        2,
        "0",
      )}T${hour}:${minute}:${second}`,
    );
  }

  // Format: 13/07/2025 1:06:07 PM (12 jam)
  const match12 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{1,2}):(\d{2}):(\d{2}) (\w{2})$/,
  );
  if (match12) {
    let [_, day, month, year, hour, minute, second, period] = match12;
    hour = parseInt(hour, 10);
    if (period === "PM" && hour < 12) hour += 12;
    if (period === "AM" && hour === 12) hour = 0;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}T${hour
        .toString()
        .padStart(2, "0")}:${minute}:${second}`,
    );
  }

  return null;
};

const formatDotNetDate = (value) => {
  if (!value) return "";

  // Ambil angka di dalam /Date(XXXXX)/
  const timestamp = parseInt(value.replace("/Date(", "").replace(")/", ""));

  const date = new Date(timestamp);

  const day = String(date.getDate()).padStart(2, "0");
  const monthNames = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];
  const month = monthNames[date.getMonth()];
  const year = date.getFullYear();

  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${day} ${month} ${year} ${hours}:${minutes}`;
};

const stylingColumnSearchAndPaging = (params) => {
  const input = $(params + "_filter input");
  input
    .addClass("form-control form-control-sm")
    .attr("placeholder", "🔍 Type here to search...")
    .css({
      width: "250px",
      height: "40px",
      fontSize: "15px",
      display: "inline-block",
    });

  const lengthSelect = $(params + "_length select");
  lengthSelect.addClass("form-select form-select-sm").css({
    width: "65px",
    fontSize: "15px",
    height: "40px",
  });
};

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

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
};
