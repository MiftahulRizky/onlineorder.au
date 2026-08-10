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
let DataTablePricingItem;
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const btnEl = {
  btnFinish: getById("btnFinish"),
  btnPreviewPrint: getById("btnPreviewPrint"),
  btnPreviewPDF: getById("btnPreviewPDF"),
  btnJobSheet: getById("btnJobSheet"),
  btnConvertToJob: getById("btnConvertToJob"),
  btnReprintJobSheet: getById("btnReprintJobSheet"),
  btnChangeJobStatus: getById("btnChangeJobStatus"),
  btnSubmit: getById("btnSubmit"),
  btnEditHeader: getById("btnEditHeader"),
  btnDeleteHeader: getById("btnDeleteHeader"),
  btnQuote: getById("btnQuote"),
  btnQuoteDetail: getById("btnQuoteDetail"),
  btnDownloadQuote: getById("btnDownloadQuote"),
  btnMoreAction: getById("btnMoreAction"),
  btnReloadPricing: getById("btnReloadPricing"),
  btnChangeStatus: getById("btnChangeStatus"),
  btnSendOrderMail: getById("btnSendOrderMail"),
  btnDownloadBarcode: getById("btnDownloadBarcode"),
  btnExactSlip: getById("btnExactSlip"),
  btnQuoteDisc: getById("btnQuoteDisc"),
  dividerPrintQuote: getById("dividerPrintQuote"),
  btnPrintQuote: getById("btnPrintQuote"),
  btnEmailQuote: getById("btnEmailQuote"),
  dividerEmailDeposit: getById("dividerEmailDeposit"),
  btnEmailDeposit: getById("btnEmailDeposit"),
  dividerLogs: getById("dividerLogs"),
  btnLogs: getById("btnLogs"),
  btnAddItem: getById("btnAddItem"),
  btnAddSurcharge: getById("btnAddSurcharge"),
  divPrice: getById("divPrice"),
  msgThanks: getById("msgThanks"),
  thMarkUp: document.querySelector(".thMarkUp"),
  thPrice: document.querySelector(".thPrice"),
};
const spanEl = {
  retailerName: getById("spanRetailerName"),
  retailerId: getById("spanRetailerId"),
  orderId: getById("spanOrderId"),
  joNumber: getById("spanJoNumber"),
  joNumberMsg: getById("spanJoNumberMsg"),
  orderType: getById("spanOrderProductType"),
  orderNo: getById("spanOrderNo"),
  orderCust: getById("spanOrderCust"),
  createdDate: getById("spanCreatedDate"),
  createdBy: getById("spanCreatedBy"),
  note: getById("spanNote"),
  statusNote: getById("spanStatusNote"),
  statusOrder: getById("spanStatusOrder"),
  delivery: getById("spanDelivery"),
  submittedDate: getById("spanSubmittedDate"),
  productionDate: getById("spanProductionDate"),
  completedDate: getById("spanCompletedDate"),
  canceledDate: getById("spanCanceledDate"),
  total: getById("spanTotal"),
  gst: getById("spanGST"),
  final: getById("spanFinalTotal"),
};
const liEl = {
  liDetailItem: getByClass("liDetailItem"),
  liEditItem: getByClass("liEditItem"),
  liCopyItem: getByClass("liCopyItem"),
  liDeleteItem: getByClass("liDeleteItem"),
  liEditPricingItem: getByClass("liEditPricingItem"),
  liPricingItem: getByClass("liPricingItem"),
  liDivider: getByClass("liDivider"),
  liDividerBarcode: getByClass("liDividerBarcode"),
  liDownloadBarcodeItem: getByClass("liDownloadBarcodeItem"),
};
const elModal = {
  modalChangeStatus: getById("modalChangeStatus"),
  modalQuoteDisc: getById("modalQuoteDisc"),
  modalProductionDate: getById("modalProductionDate"),
  modalAddItem: getById("modalAddItem"),
  modalAddService: getById("modalAddService"),
  modalSendMailQuote: getById("modalSendMailQuote"),
  modalEditPricing: getById("modalEditPricing"),
  modalPricingItem: getById("modalPricingItem"),
  modalLogs: getById("modalLogs"),
};

const tableBtn = {};
// ==============================================|| EVENTS ||================================================

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
        if (!result.isConfirmed) return;

        await handlerReloadPricingOnReadyPage(HEADERID, status, "click");
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

      if (id === "btnExactSlip") {
        const result = await Swal.fire({
          title: "Are you sure?",
          text: "Sure to send the exact slip?",
          icon: "warning",
          showCancelButton: true,
          customClass: {
            popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
          },
          confirmButtonColor: "#3085d6",
          cancelButtonColor: "#d33",
          confirmButtonText: "Yes, send it!",
        });
        if (!result.isConfirmed) return;

        await handlerExactSlip();
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

      if (id === "btnAddItem") {
        document.querySelectorAll("#modalAddItem .form-select").forEach((e) => {
          e.classList.remove("is-invalid");
          e.value = "";
        });

        handlerShowBSModal("modalAddItem");
      }

      if (id === "btnAddSurcharge") {
        const designid = "415D0633-0648-42D8-B041-FE419E01BB3C";
        const production = "";
        const action = "AddItem";

        await submitSelectProduct(designid, production, action, id);
      }
    } catch (error) {
      const msg = `Event btnEl: ${error.message}`;
      catchMessages(msg);
    }
  });
});

Object.values(spanEl).forEach((el) => {
  if (!el) return;
  el.addEventListener("click", async (e) => {
    try {
      const id = e.currentTarget.id;

      if (id === "spanJoNumber") {
        const jonumber = e.currentTarget.dataset.number;
        const success = await copyToClipboard(jonumber);

        if (success) {
          showCopyMessage();
        }
      }

      if (id === "spanProductionDate") {
        const raw = e.currentTarget.dataset.date;
        console.log(raw);

        if (["Customer"].includes(ROLENAME)) return;

        const datePart = raw.split(" ")[0];
        const parts = datePart.split("/");
        let day, month, year;

        if (parts[0].length === 4) {
          // format yyyy/MM/dd
          [year, month, day] = parts;
        } else {
          // format dd/MM/yyyy atau d/MM/yyyy
          [day, month, year] = parts;
        }

        const formatted = `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;

        console.log(formatted);

        elModal.modalProductionDate.querySelector("#productiondate").value =
          formatted;
        handlerShowBSModal("modalProductionDate");
      }
    } catch (error) {
      const msg = `Event spanEl: ${error.message}`;
      catchMessages(msg);
    }
  });
});

const modalHandlers = {
  modalChangeStatus: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("change", (e) => {
        e.target.classList.remove("is-invalid");

        if (e.target.id === "status") {
          const status = e.target.value;
          displayElmodalChangeStatus(status);
        }
      });

      modal.addEventListener("input", (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", (e) => {
        const id = e.target.id;

        if (id === "tooltipDescription") {
          const status = modal.querySelector("#status")?.value;

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
  },

  modalSendMailQuote: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("input", (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", (e) => {
        if (e.target.id === "btnSendMailQuote") {
          submitSendMailQuote();
        }
      });
    },
  },

  modalQuoteDisc: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("input", (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", (e) => {
        if (e.target.id === "btnSubmitOverrideDisc") {
          modal
            .querySelectorAll(".form-control, .form-select")
            .forEach((el) => {
              el.classList.remove("is-invalid");
            });

          submitOverrideDisc(e.target.id);
        }
      });
    },
  },

  modalProductionDate: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("change", (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", (e) => {
        if (e.target.id === "btnSubmitProductionDate") {
          modal
            .querySelectorAll(".form-control, .form-select")
            .forEach((el) => {
              el.classList.remove("is-invalid");
            });

          submitChangeProductionDate(e.target.id);
        }
      });
    },
  },

  modalAddItem: {
    init: (modal) => {
      const divProduction = modal.querySelector("#divProduction");
      divProduction.classList.add("d-none");
    },

    events: (modal) => {
      modal.addEventListener("change", async (e) => {
        e.target.classList.remove("is-invalid");
        const id = e.target.id;
        if (id === "designid") {
          const divProduction = modal.querySelector("#divProduction");
          divProduction.classList.add("d-none");
          const selectedOption = e.target.selectedOptions[0];
          const designname = selectedOption?.dataset.name;
          await bindProduction(designname);
        }
      });

      modal.addEventListener("click", async (e) => {
        const id = e.target.id;
        if (id === "submitAddItem") {
          const designid = modal.querySelector("#designid").value;
          const production = modal.querySelector("#production").value;
          const action = "AddItem";

          await submitSelectProduct(designid, production, action, id);
        }
      });
    },
  },

  modalEditPricing: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("input", async (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", async (e) => {
        const id = e.target.id;
        if (id === "btnSubmitEditPricing") {
          await submitEditPricing(id);
        }
      });
    },
  },
};
Object.entries(elModal).forEach(([key, modal]) => {
  if (!modal) return;

  const handler = modalHandlers[key];
  if (!handler) return;

  if (handler.init) {
    handler.init(modal);
  }

  if (handler.events) {
    handler.events(modal);
  }
});

document.querySelector("#tableAjax").addEventListener("click", async (e) => {
  const id = e.target.id;
  if (e.target.id === "btnDetailItem") {
    const itemid = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const production = e.target.dataset.production;
    const headerid = e.target.dataset.headerid;
    const ordertype = ORDERTYPE;
    handlerFindProductForm(
      itemid,
      headerid,
      ordertype,
      "ViewItem",
      designid,
      production,
    );
  }

  if (id === "btnEditItem") {
    const itemid = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const production = e.target.dataset.production;
    const headerid = e.target.dataset.headerid;
    const ordertype = ORDERTYPE;
    handlerFindProductForm(
      itemid,
      headerid,
      ordertype,
      "EditItem",
      designid,
      production,
    );
  }

  if (id === "btnCopyItem") {
    const itemid = e.target.dataset.id;
    const headerid = e.target.dataset.headerid;
    const product = e.target.dataset.product;
    handlerCopyItem(itemid, headerid, product);
  }

  if (id === "btnDeleteItem") {
    const itemid = e.target.dataset.id;
    const product = e.target.dataset.product;
    handlerDeleteItem(itemid, product);
  }

  if (e.target.id === "btnEditPricingItem") {
    const id = e.target.dataset.id;
    const qty = e.target.dataset.qty;
    await handlerEditPricing(id, qty);
    handlerShowBSModal("modalEditPricing");
  }

  if (id === "btnPricingItem") {
    const itemid = e.target.dataset.id;
    await bindPricingItem(itemid);
    elModal.modalPricingItem.querySelector("#modalPricingItemLabel").innerHTML =
      `Cost Details - ${itemid}`;
    handlerShowBSModal("modalPricingItem");
  }

  if (id === "btnDownloadBarcodeItem") {
    const itemid = e.target.dataset.id;
    handlerDownloadBarcode(HEADERID, itemid);
  }

  if (e.target.id === "btnNextItem") {
    const itemid = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const headerid = e.target.dataset.headerid;
    const ordertype = ORDERTYPE;
    const production = e.target.dataset.production;
    const msgbody = e.target.dataset.next;
    handlerFindProductForm(
      itemid,
      headerid,
      ordertype,
      "NextItem",
      designid,
      production,
      msgbody,
    );
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

    console.log(data.header);

    handlerHeaderInfo(data.header);
    bindDetails(data.detail);
    displayElOverall(data.header, data.detail);
    handlerCheckOrder(data.other.ResCheckOrder);
    handlerChangeStatus(data.header);
    setValmodalQuoteDisc(data.header);
    setValmodalSendMailQuote(data.other.SendMailQuote);
    setValmodalLogs(data.other);
    bindProduct(data.other.Designs.list);
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
      responsive: false,
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

const bindProduct = (data) => {
  const sel = document.querySelector("#modalAddItem #designid");
  sel.innerHTML = ""; // reset

  if (data.length > 0) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindProduction = async (designname) => {
  if (!designname) return;
  try {
    const response = await fetch(`${URIMETHOD}/BindProduction`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designname, rolename: ROLENAME }),
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

    generateOption("#modalAddItem #production", data.OptProd, 1);
    if (data.VisProd) {
      elModal.modalAddItem
        .querySelector("#divProduction")
        .classList.remove("d-none");
    }

    console.log(data);
  } catch (error) {
    const msg = `bindProduction: ${error.message}`;
    catchMessage(msg);
  }
};

const bindPricingItem = async (id) => {
  try {
    const response = await fetch(`${URIMETHOD}/BindPricingItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, rolename: ROLENAME }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const { d: data } = await response.json();

    if (!data || data.length === 0) return;
    if (data.error) {
      throw new Error(data.message);
    }

    const dt = data.price;
    console.log(dt);

    let html = `
      <table class="table table-bordered table-vcenter" width="100%">
        <thead>
          <tr>
            <th class="text-center">#</th>
            <th class="h3 text-center">Qty</th>
            <th class="h3">Descpription</th>
            <th class="h3">Cost / Qty</th>
            <th class="h3">POA / Qty</th>
            <th class="h3">Discount / Qty</th>
            <th class="h3 ">Sub Total</th>
          </tr>
        </thead>
        <tbody>
    `;

    dt.forEach((item, index) => {
      if (item.isContinue) {
        return;
      }
      html += `
          <tr class="${item.isOpacity}">
            <td class="text-center">${index + 1}</td>
            <td class="text-center">${item.Qty}</td>
            <td class="text-left">${item.Description}</td>
            <td class="text-center">${item.Cost}</td>
            <td class="text-center">${item.Poa}</td>
            <td class="text-center">${item.Discount}</td>
            <td class="text-center">${item.FinalCost}</td>
          </tr>
        `;
    });

    html += `</tbody></table>`;

    const container = elModal.modalPricingItem.querySelector(
      "#modalPricingItemBody",
    );
    container.innerHTML = html;

    const popoverTriggerList = container.querySelectorAll(
      '[data-bs-toggle="popover"]',
    );
    popoverTriggerList.forEach((popoverTriggerEl) => {
      new bootstrap.Popover(popoverTriggerEl);
    });
  } catch (error) {
    const msg = `bindPricingItem: ${error.message}`;
    catchMessages(msg);
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
    spanEl.productionDate.setAttribute("data-date", item.JobDate);
    spanEl.completedDate.textContent = formatDate(item.CompletedDate);
    spanEl.canceledDate.textContent = formatDate(item.CanceledDate);

    let JoNumVal = item.JoNumberId
      ? `<span class="badge badge-outline text-red">${item.JoNumberId}</span>`
      : "-";
    spanEl.joNumber.innerHTML = JoNumVal;
    spanEl.joNumber.setAttribute("data-number", item.JoNumberId);
    spanEl.joNumberMsg.classList.add("d-none");

    setText(spanEl.orderType, item.OrderType);

    spanEl.total.innerHTML = formatCurrency(item.SumPrice);
    spanEl.gst.innerHTML = formatCurrency(item.Gst);
    spanEl.final.innerHTML = formatCurrency(item.FinalTotal);
  } catch (error) {
    const msg = `handlerHeaderInfo : ${error.message}`;
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
    setValmodalChangeStatus(header);
    displayElmodalChangeStatus(header.Status);
  } catch (error) {
    const msg = `handlerChangeStatus: ${error.message}`;
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

const handlerExactSlip = async () => {
  swalLoadingShow("Please wait while we exact the slip.");
  try {
    const response = await fetch(`${URIMETHOD}/ExactSlip`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid: HEADERID, ordertype: ORDERTYPE }),
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
      location.reload();
    }
  } catch (error) {
    const msg = `handlerExactSlip: ${error.message}`;
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

const handlerFindProductForm = async (
  id,
  headerid,
  ordertype,
  action,
  designid,
  production,
  msgbody = "",
) => {
  if (action === "NextItem") {
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
  }
  try {
    const response = await fetch(`${URIMETHOD}/FindProductForm`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          id,
          rolename: ROLENAME,
          headerid,
          ordertype,
          action,
          designid,
          production,
        },
      }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message?.toUpperCase());
      if (res.field) {
        const field = document.querySelector(res.field);
        field.classList.add("is-invalid");
      }
    } else if (res.success) {
      const findPage = res.page.replace("~", "");
      window.location.href = findPage;
    }
  } catch (error) {
    const msg = `handlerFindProductForm: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerCopyItem = async (id, headerid, product) => {
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

  swalLoadingShow("Please wait while we copy the item.");

  try {
    const response = await fetch(`${URIMETHOD}/CopyItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, headerid, loginid: LOGINID }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    // Swal.close();

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);
      location.reload();
    }
  } catch (error) {
    const msg = `handlerCopyItem: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerDeleteItem = async (id, product) => {
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

  swalLoadingShow("Please wait while we delete the item.");

  try {
    const response = await fetch(`${URIMETHOD}/DeleteItem`, {
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
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message.toUpperCase());
    } else if (res.success) {
      await isSuccess(res.message);
      location.reload();
    }
  } catch (error) {
    const msg = `handlerDeleteItem: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerEditPricing = async (id, qty) => {
  try {
    const response = await fetch(`${URIMETHOD}/BindOrderDetailPrice`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ itemid: id }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const { d: res } = await response.json();

    if (!res) {
      throw new Error("No data");
    }
    if (res.error) {
      throw new Error(res.message);
    }

    const data = res.odp;

    let html = `
      <input type="number" min="1" name="itemid" id="itemid" value="${id}" class="form-control " readonly hidden />
      <input type="number" min="1" name="qty" id="qty" value="${qty}" class="form-control " readonly hidden />
      <table class="table table-bordered table-vcenter" width="100%">
        <thead>
          <tr>
            <th>No</th>
            <th>Qty</th>
            <th>Description</th>
            <th>Cost/Qty</th>
            <th>POA/Qty</th>
          </tr>
        </thead>
        <tbody>
    `;

    data.forEach((item, index) => {
      html += `
          <tr>
            <td>${index + 1}</td>
            <td>${item.Qty}</td>
            <td>${item.Description}</td>
            <td>${item.Cost}</td>
            <td>
              <div class="input-group">
                <input type="text" 
                  value="${item.Poa}" 
                  class="form-control input-poa" 
                  data-id="${item.Id}" data-type="${item.Type}"
                  placeholder="Example: 10.00" />
              </div>
            </td>
          </tr>
        `;
    });

    html += `</tbody></table>`;

    elModal.modalEditPricing.querySelector("#modalBody").innerHTML = html;
  } catch (error) {
    const msg = `handlerEditPricing: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| Display Functions ||-------------------------------------
const displayElOverall = (header, detail) => {
  try {
    Object.values(btnEl).forEach((el) => toggleShow(el, false));

    if (!header || !detail) return;

    // Reset visibilitas kolom data table
    DataTableDetails?.columns(5)?.visible(false);
    DataTableDetails?.columns(6)?.visible(false);

    // Button bawaan (Selalu muncul jika header & detail ada)
    toggleShow(btnEl.btnFinish, true);
    toggleShow(btnEl.btnPreviewPrint, true);
    toggleShow(btnEl.btnPreviewPDF, true);
    toggleShow(btnEl.btnConvertToJob, true);
    toggleShow(btnEl.dividerPrintQuote, true);
    toggleShow(btnEl.btnPrintQuote, true);
    toggleShow(btnEl.btnEmailQuote, true);

    if (header.JoNumberId) {
      toggleShow(btnEl.btnReprintJobSheet, true);
    }

    // Variabel Penentu Status & Peran
    const isAdmin = ROLENAME === "Administrator";
    const isPpicCs = ["PPIC & DE", "Customer Service"].includes(ROLENAME);
    const isCustomer = ROLENAME === "Customer";

    const isDraftOrPending = ["Draft", "Pending Price Approval"].includes(
      header.Status,
    );
    const isNotCompleted = header.Status !== "Completed";
    const isNotCanceled = header.Status !== "Canceled";
    const isOrderActive = [
      "New Order",
      "In Production",
      "Completed",
      "On Hold",
    ].includes(header.Status);

    // --- LOGIKA PERMISSION UNTUK ADMIN ---
    if (isAdmin) {
      toggleShow(btnEl.btnJobSheet, true);

      if (isDraftOrPending) {
        toggleShow(btnEl.btnSubmit, true);
        toggleShow(btnEl.btnEditHeader, true);
        toggleShow(btnEl.btnDeleteHeader, true);
        toggleShow(btnEl.btnAddItem, true);
      }

      if (isNotCompleted) {
        toggleShow(btnEl.btnAddSurcharge, true);
        toggleShow(btnEl.btnQuoteDisc, true);
      }

      toggleShow(btnEl.btnDownloadBarcode, true);
      toggleShow(btnEl.btnExactSlip, true);
      toggleShow(btnEl.btnQuote, true);
      toggleShow(btnEl.btnQuoteDetail, true);
      toggleShow(btnEl.btnDownloadQuote, true);

      if (isOrderActive) {
        toggleShow(btnEl.btnChangeStatus, true);
        toggleShow(btnEl.btnSendOrderMail, true);
        toggleShow(btnEl.btnAddItem, true);
      }

      toggleShow(btnEl.btnMoreAction, true);
      toggleShow(btnEl.btnEmailDeposit, true);
      toggleShow(btnEl.dividerEmailDeposit, true);

      if (isNotCanceled) {
        toggleShow(btnEl.btnReloadPricing, true);
      }

      toggleShow(btnEl.dividerLogs, true);
      toggleShow(btnEl.btnLogs, true);
    }

    // --- LOGIKA PERMISSION UNTUK PPIC & CS ---
    if (isPpicCs) {
      toggleShow(btnEl.btnJobSheet, true);

      if (isDraftOrPending) {
        toggleShow(btnEl.btnSubmit, true);
        toggleShow(btnEl.btnEditHeader, true);
        toggleShow(btnEl.btnDeleteHeader, true);
        toggleShow(btnEl.btnAddItem, true);
      }

      if (isNotCompleted) {
        toggleShow(btnEl.btnAddSurcharge, true);
        toggleShow(btnEl.btnQuoteDisc, true);
      }

      toggleShow(btnEl.btnDownloadBarcode, true);
      toggleShow(btnEl.btnExactSlip, true);

      if (isOrderActive) {
        toggleShow(btnEl.btnChangeStatus, true);
      }

      toggleShow(btnEl.btnMoreAction, true);

      if (isNotCanceled) {
        toggleShow(btnEl.btnReloadPricing, true);
      }
    }

    // --- LOGIKA PERMISSION UNTUK CUSTOMER ---
    if (isCustomer) {
      if (isDraftOrPending) {
        toggleShow(btnEl.btnSubmit, true);
        toggleShow(btnEl.btnEditHeader, true);
        toggleShow(btnEl.btnDeleteHeader, true);
        toggleShow(btnEl.btnAddItem, true);
      }

      toggleShow(btnEl.btnQuote, true);
      toggleShow(btnEl.btnQuoteDetail, true);
      toggleShow(btnEl.btnDownloadQuote, true);
    }

    // ----------------------------------------------|| Hide Button Datatable ||---------------------------------------

    // Reset semua elemen di liEl
    Object.values(liEl).forEach((el) => {
      Array.from(el).forEach((li) => toggleShow(li, false));
    });

    // Default visibilitas item detail
    toggleShowList(["liDetailItem"], true);

    if (isDraftOrPending) {
      toggleShowList(["liEditItem", "liCopyItem", "liDeleteItem"], true);
      toggleShowList(["liDetailItem"], false);

      const tempRole = ["PPIC & DE", "Customer Service", "Manager", "Account"];
      const isOtherUser =
        header.CreatedBy.toUpperCase() !== LOGINID.toUpperCase();

      if (tempRole.includes(ROLENAME) && isOtherUser) {
        toggleShowList(["liEditItem", "liCopyItem", "liDeleteItem"], false);
        toggleShowList(["liDetailItem"], true);
      }
    }

    if (["Additional", "Surcharge"].includes(detail.DesignName)) {
      toggleShowList(["liDeleteItem"], true);
    }

    let hideEditPricing = "True";
    let hidePricing = "True";

    if (["True", "1"].includes(PRICEACCESS)) {
      DataTableDetails?.columns(5).visible(true);
      toggleShow(btnEl.thPrice, true);
      toggleShow(btnEl.divPrice, true);

      if (
        ["Administrator", "PPIC & DE", "Customer Service"].includes(ROLENAME)
      ) {
        toggleShowList(
          ["liEditPricingItem", "liDividerBarcode", "liDownloadBarcodeItem"],
          true,
        );
        hideEditPricing = "False";
      }

      toggleShowList(["liPricingItem"], true);
      hidePricing = "False";
    }

    if (["True", "1"].includes(MARKUPACCESS)) {
      DataTableDetails?.columns(6).visible(true);
      toggleShow(btnEl.thMarkUp, true);
    }

    if (hideEditPricing === "False" && hidePricing === "False") {
      toggleShowList(["liDivider"], true);
    }
  } catch (error) {
    const msg = `displayElOverall: ${error.message}`;
    catchMessages(msg);
  }
};
const displayElmodalChangeStatus = (status) => {
  elModal.modalChangeStatus
    .querySelector("#divDescription")
    .classList.add("d-none");
  elModal.modalChangeStatus
    .querySelector("#divSubmittedDate")
    .classList.add("d-none");
  elModal.modalChangeStatus
    .querySelector("#divCompletedDate")
    .classList.add("d-none");
  elModal.modalChangeStatus
    .querySelector("#divCanceledDate")
    .classList.add("d-none");

  if (status) {
    elModal.modalChangeStatus
      .querySelector("#divDescription")
      .classList.remove("d-none");
    switch (status) {
      case "New Order":
        elModal.modalChangeStatus
          .querySelector("#divSubmittedDate")
          .classList.remove("d-none");
        break;
      case "Completed":
        elModal.modalChangeStatus
          .querySelector("#divCompletedDate")
          .classList.remove("d-none");
        break;
      case "Canceled":
        elModal.modalChangeStatus
          .querySelector("#divCanceledDate")
          .classList.remove("d-none");
        break;
    }
  }
};
// ----------------------------------------------|| SetVal Functions ||--------------------------------------
const setValmodalChangeStatus = (itemData) => {
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

const setValmodalQuoteDisc = (header) => {
  const disc = header.QuoteDisc.replace(",", ".");
  elModal.modalQuoteDisc.querySelector("#discount").value = disc || 0;
  console.log(disc);
};

const setValmodalSendMailQuote = (other) => {
  document.querySelector("#modalSendMailQuote #id").value = other.MailId;
  document.querySelector("#modalSendMailQuote #from").value = other.MailFrom;
  document.querySelector("#modalSendMailQuote #mailto").value = other.MailTo;
};

const setValmodalLogs = (other) => {
  const table = document.querySelector("#modalLogs #table-logs tbody");
  table.innerHTML = "";

  const logs = other?.Logs.LogsData || [];
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

const submitChangeProductionDate = async (button) => {
  try {
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = ["productiondate"];

    const formData = {
      headerid: HEADERID,
      loginid: LOGINID,
      rolename: ROLENAME,
    };

    fields.forEach((field) => {
      formData[field] = document.querySelector(
        `#modalProductionDate #${field}`,
      ).value;
    });

    // swal.close();
    // return console.table(formData);

    const response = await fetch(URIMETHOD + "/SubmitChangeProductionDate", {
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
      const field = document.getElementById(res.field);
      if (field) {
        field.classList.add("is-invalid");
      }
    } else if (res.success) {
      await isSuccess(res.message);
      location.reload();
    }
  } catch (error) {
    const msg = `submitChangeProductionDate: ${error.message}`;
    catchMessages(msg);
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

const submitSelectProduct = async (designid, production, action, button) => {
  document.getElementById(button).innerHTML = "Proccessing...";
  try {
    const response = await fetch(URIMETHOD + "/FindProductForm", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          id: "",
          rolename: ROLENAME,
          headerid: HEADERID,
          ordertype: ORDERTYPE,
          action: action,
          designid: designid,
          production: production,
        },
      }),
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
      if (res.field) {
        const field = document.querySelector(res.field);
        field.classList.add("is-invalid");
      }
    } else if (res.success) {
      const findPage = res.page.replace("~", "");
      window.location.href = findPage;
    }
  } catch (error) {
    const msg = `submitSelectProduct: ${error.message}`;
    catchMessages(msg);
  } finally {
    document.getElementById(button).innerHTML = "Next";
  }
};

const submitEditPricing = async (button) => {
  const md = elModal.modalEditPricing;
  md.querySelectorAll(".form-control").forEach((e) =>
    e.classList.remove("is-invalid"),
  );

  const poaInputs = md.querySelectorAll(".input-poa");

  let detailList = [];

  poaInputs.forEach((input) => {
    detailList.push({
      id: input.dataset.id,
      type: input.dataset.type,
      poa: input.value.replace(",", "."),
    });
  });

  let itemid = md.querySelector("#itemid").value;
  let qty = md.querySelector("#qty").value;

  const Params = {
    loginid: LOGINID,
    username: USERNAME,
    rolename: ROLENAME,
    headerid: HEADERID,
    itemid: itemid,
    qty: qty,
    customerid: document.querySelector("#spanRetailerId").innerHTML,
    details: detailList,
  };

  // return console.table(Params);

  try {
    button.innerHTML = "Proccessing...";
    swalLoadingShow("Please wait...");

    const response = await fetch(`${URIMETHOD}/OverwritePricing`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: Params }),
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
    } else if (res.success) {
      handlerHideBSModal("modalEditPricing");
      await isSuccess(res.message);
      location.reload();
    }
  } catch (error) {
    const msg = `submitEditPricing: ${error.message}`;
    catchMessages(msg);
  } finally {
    button.innerHTML = "Save Changes";
  }
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const orderDetailPageLoaded = async () => {
  try {
    if (!ULTRON || !ORDERTYPE) window.location.href = "/order";

    if (CUSTOMERID == "LS-A224") window.location.href = "/order"; // JPM Direct

    if (CUSTOMERID == "DEFAULT" && USERNAME == "galih") {
      window.location.href = "/order";
    }

    // if (!["Administrator"].includes(ROLENAME)) {
    //   window.location.href = "/order";
    // }

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

const copyToClipboard = async (text) => {
  try {
    if (navigator.clipboard && navigator.clipboard.writeText) {
      await navigator.clipboard.writeText(text);
      return true;
    }

    const ta = document.createElement("textarea");
    ta.value = text;
    ta.style.position = "fixed";
    ta.style.opacity = "0";
    document.body.appendChild(ta);
    ta.focus();
    ta.select();

    document.execCommand("copy");
    ta.remove();

    return true;
  } catch (err) {
    console.error("Copy failed:", err);
    return false;
  }
};

const showCopyMessage = () => {
  const el = spanEl.joNumberMsg;
  if (!el) return;

  el.classList.remove("d-none"); // tampilkan

  setTimeout(() => {
    el.classList.add("d-none"); // sembunyikan lagi
  }, 1500); // 1.5 detik
};

const toggleShow = (el, show) => {
  if (!el) return;
  el.classList.toggle("d-none", !show);
};

const toggleShowList = (keys, show) => {
  keys.forEach((key) => {
    if (liEl[key]) {
      Array.from(liEl[key]).forEach((li) =>
        li.classList.toggle("d-none", !show),
      );
    }
  });
};
const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
