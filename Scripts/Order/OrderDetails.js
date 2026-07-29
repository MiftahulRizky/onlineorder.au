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
const bindHeader = async (headerid, ordertype) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindOrderHeaderByID`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: { headerid, ordertype, loginid: LOGINID, rolename: ROLENAME },
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
    console.table(data);

    handlerHeaderInfo(data.header); // langsung 1 object, bukan array
    handlerDisplayElement(data.header);
    handlerCheckOrder(data.header.ResCheckOrder);
  } catch (error) {
    let msg = "Please contact our IT team at support@onlineorder.au";
    if (["Administrator"].includes(ROLENAME)) {
      msg = error.message;
    }
    isError(msg);
  }
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
const handlerDisplayElement = (item) => {
  Object.values(btnEl).forEach((el) => {
    if (el) el.classList.add("d-none");
  });

  if (!item) return;

  if (item.JoNumberId) {
    btnEl.btnReprintJobSheet.classList.remove("d-none");
  }

  if (["Administrator"].includes(ROLENAME)) {
    btnEl.btnJobSheet.classList.remove("d-none");

    if (["Draft", "Pending Price Approval"].includes(item.Status)) {
      btnEl.btnSubmit.classList.remove("d-none");
      btnEl.btnEditHeader.classList.remove("d-none");
      btnEl.btnDeleteHeader.classList.remove("d-none");
      btnEl.btnAddItem.classList.remove("d-none");
    }

    if (!["Completed"].includes(item.Status)) {
      btnEl.btnAddService.classList.remove("d-none");
      btnEl.btnQuoteDisc.classList.remove("d-none");
    }
    btnEl.btnDownloadBarcode.classList.remove("d-none");

    btnEl.btnQuote.classList.remove("d-none");
    btnEl.btnQuoteDetail.classList.remove("d-none");
    btnEl.btnDownloadQuote.classList.remove("d-none");

    if (
      ["New Order", "In Production", "Completed", "On Hold"].includes(
        item.Status,
      )
    ) {
      btnEl.btnChangeStatus.classList.remove("d-none");
      btnEl.btnSendOrderMail.classList.remove("d-none");
    }

    btnEl.btnMoreAction.classList.remove("d-none");

    btnEl.btnEmailDeposit.classList.remove("d-none");
    btnEl.dividerEmailDeposit.classList.remove("d-none");

    if (!["Canceled"].includes(item.Status)) {
      btnEl.btnReloadPricing.classList.remove("d-none");
    }
  }

  if (["PPIC & DE", "Customer Service"].includes(ROLENAME)) {
    btnEl.btnJobSheet.classList.remove("d-none");

    if (["Draft", "Pending Price Approval"].includes(item.Status)) {
      btnEl.btnSubmit.classList.remove("d-none");
      btnEl.btnEditHeader.classList.remove("d-none");
      btnEl.btnDeleteHeader.classList.remove("d-none");
      btnEl.btnAddItem.classList.remove("d-none");
    }

    if (!["Completed"].includes(item.Status)) {
      btnEl.btnAddService.classList.remove("d-none");
      btnEl.btnQuoteDisc.classList.remove("d-none");
    }
    btnEl.btnDownloadBarcode.classList.remove("d-none");

    if (
      ["New Order", "In Production", "Completed", "On Hold"].includes(
        item.Status,
      )
    ) {
      btnEl.btnChangeStatus.classList.remove("d-none");
    }

    btnEl.btnMoreAction.classList.remove("d-none");

    if (!["Canceled"].includes(item.Status)) {
      btnEl.btnReloadPricing.classList.remove("d-none");
    }
  }

  if (["Administrator"].includes(ROLENAME)) {
    if (["Draft", "Pending Price Approval"].includes(item.Status)) {
      btnEl.btnSubmit.classList.remove("d-none");
      btnEl.btnEditHeader.classList.remove("d-none");
      btnEl.btnDeleteHeader.classList.remove("d-none");
      btnEl.btnAddItem.classList.remove("d-none");
    }

    btnEl.btnQuote.classList.remove("d-none");
    btnEl.btnQuoteDetail.classList.remove("d-none");
    btnEl.btnDownloadQuote.classList.remove("d-none");
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

  await bindHeader(HEADERID, ORDERTYPE);
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
