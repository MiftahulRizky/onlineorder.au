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

// ==============================================|| EVENTS ||================================================
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
        },
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const { d: data } = await response.json();

    if (!data) {
      window.location.replace("/order");
      return;
    }
    console.log(data.detail);

    handlerHeaderInfo(data.header);
    bindDetails(data.detail);
    handlerDisplayElement(data.header, data.detail);
    handlerCheckOrder(data.header.ResCheckOrder);
  } catch (error) {
    let msg = "Please contact our IT team at support@onlineorder.au";
    if (!["Administrator"].includes(ROLENAME)) {
      msg = error.message;
    }
    isError(msg);
  }
};

let DataTableDetails;
const bindDetails = (details) => {
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
};
// ----------------------------------------------|| Handler Functions ||-------------------------------------
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
const handlerHeaderInfo = (item) => {
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
};

const btnEl = {
  btnJobSheet: document.getElementById("btnJobSheet"),
  btnReprintJobSheet: document.getElementById("btnReprintJobSheet"),
  btnChangeJobStatus: document.getElementById("btnChangeJobStatus"),
  btnSubmit: document.getElementById("btnSubmit"),
  btnEditHeader: document.getElementById("btnEditHeader"),
  btnDeleteHeader: document.getElementById("btnDeleteHeader"),
  btnQuote: document.getElementById("btnQuote"),
  btnQuoteDetail: document.getElementById("btnQuoteDetail"),
  btnDownloadQuote: document.getElementById("btnDownloadQuote"),
  btnMoreAction: document.getElementById("btnMoreAction"),
  btnEmailDeposit: document.getElementById("btnEmailDeposit"),
  dividerEmailDeposit: document.getElementById("dividerEmailDeposit"),
  btnChangeStatus: document.getElementById("btnChangeStatus"),
  btnSendOrderMail: document.getElementById("btnSendOrderMail"),
  btnDownloadBarcode: document.getElementById("btnDownloadBarcode"),
  btnQuoteDisc: document.getElementById("btnQuoteDisc"),
  btnReloadPricing: document.getElementById("btnReloadPricing"),
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
};

const handlerDisplayElement = (header, detail) => {
  Object.values(btnEl).forEach((el) => {
    if (el) el.classList.add("d-none");
  });
  DataTableDetails.columns(5).visible(false);
  DataTableDetails.columns(6).visible(false);

  if (!header || !detail) return;

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
    DataTableDetails.columns(5).visible(true);
    btnEl.thPrice.classList.remove("d-none");
    btnEl.divPrice.classList.remove("d-none");

    if (["Administrator", "PPIC & DE", "Customer Service"].includes(ROLENAME)) {
      ["liEditPricingItem"].forEach((key) => {
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
    DataTableDetails.columns(6).visible(true);
    btnEl.thMarkUp.classList.remove("d-none");
  }

  if (hideEditPricing == "False" && hidePricing == "False") {
    ["liDivider"].forEach((key) => {
      Array.from(liEl[key]).forEach((li) => {
        li.classList.remove("d-none");
      });
    });
  }
};

const handlerCheckOrder = (res) => {
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
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const orderDetailPageLoaded = async () => {
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
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  list.sort();

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
