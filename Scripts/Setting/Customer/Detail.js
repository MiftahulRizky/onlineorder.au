document.addEventListener("DOMContentLoaded", () => {
  console.log("Detail.js loaded successfully");

  checkSessionCusDetail();
});
// =================================================||EVENTS||==================================================
// change tabs
document.querySelectorAll(".tab-click").forEach((tab) => {
  tab.addEventListener("click", () => {
    const tabHeaderActive = tab.id;
    const tabHeaderKey = "active";
    setState("customer_detail_tab_header_active_val", tabHeaderActive);
    setState("customer_detail_tab_header_active_key", tabHeaderKey);

    const tabActive = tab.dataset.tabs;
    const tabKey = "active show";
    setState("customer_detail_tab_active_val", tabActive);
    setState("customer_detail_tab_active_key", tabKey);
  });
});
// ==============================================||FUNCTION ||==================================================
// ----------------------------------------------|| Binding Function ||-----------------------------------------
const bindCustomerDetail = async (id) => {
  try {
    if (!id) return;

    const res = await fetch(`${uriMethod}/bindCustomerDetail`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id }),
    });

    if (!res.ok) {
      const msg =
        roleName === "Administrator"
          ? `${res.status} - ${res.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : bindCustomerDetail"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    for (const item of data) {
      await binLabelValue(item);
      await bindContact(item.Id, "#tabs-contact #data-table");
      await bindAddresses(item.Id, "#tabs-address #data-table");
      await bindLogins(item.Id, "#tabs-logins #data-table");
      await bindDiscounts(item.Id, "#tabs-discount #data-table");
      await bindProductAccess(item.Id, "#tabs-product-access #data-table");
      await bindQuotes(item.Id, "#tabs-quotes #data-table");
      await loaderFadeOut();
    }

    return true;
  } catch (error) {
    console.error("bindCustomerDetail error:", error);
    throw error;
  }
};

const binLabelValue = (itemData) => {
  const mapping = {
    lblCustomerName: "Name",
    lblAccount: "Account",
    lblMasterCustomer: "MasterCustomer",
    lblCustomerType: "Type",
    lblSalesPerson: "SalesPerson",
    lblWebId: "Id",
    lblExactId: "ExactId",
    lblCustomerGroup: "CustomerGroup",
    lblCustomerPriceGroup: "CustomerPriceGroup",
    lblOnStop: "OnStop",
    lblCashSale: "CashSale",
    lblNewsletter: "Newsletter",
    lblMinOrderSurcharge: "MinimumOrderSurcharge",
  };

  // Set nilai ke input sesuai mapping
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[key];
    if (id === "lblOnStop") {
      if (value === "True" || value === "1") {
        value = `<i class="ti text-success fs-2 me-1 ti-square-rounded-check"></i>Yes`;
      } else if (value === "False" || value === "0") {
        value = `<i class="ti text-danger fs-2 me-1 ti-square-rounded-x"></i>No`;
      }
    }

    if (id === "lblCashSale") {
      if (value === "True" || value === "1") {
        value = `<i class="ti text-success fs-2 me-1 ti-square-rounded-check"></i>Yes`;
      } else if (value === "False" || value === "0") {
        value = `<i class="ti text-danger fs-2 me-1 ti-square-rounded-x"></i>No`;
      }
    }

    if (id === "lblNewsletter") {
      if (value === "True" || value === "1") {
        value = `<i class="ti text-success fs-2 me-1 ti-square-rounded-check"></i>Yes`;
      } else if (value === "False" || value === "0") {
        value = `<i class="ti text-danger fs-2 me-1 ti-square-rounded-x"></i>No`;
      }
    }

    if (id === "lblMinOrderSurcharge") {
      if (value === "True" || value === "1") {
        value = `<i class="ti text-success fs-2 me-1 ti-square-rounded-check"></i>Yes`;
      } else if (value === "False" || value === "0") {
        value = `<i class="ti text-danger fs-2 me-1 ti-square-rounded-x"></i>No`;
      }
    }

    el.innerHTML = value ?? ""; // fallback ke string kosong
  });
};

let ContactServerSide;
const bindContact = (customerid, params) => {
  if (ContactServerSide) {
    ContactServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  ContactServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/ContactServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      {
        data: "No",
        width: "5%",
        orderable: false,
        render: function (data, type, row, meta) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      { data: "Name", width: "10%", orderable: false },
      { data: "Salutation", width: "10%", orderable: false },
      { data: "Role", width: "10%", orderable: false },
      { data: "Email", width: "10%", orderable: false },
      { data: "Phone", width: "10%", orderable: false },
      { data: "Mobile", width: "10%", orderable: false },
      { data: "Tags", width: "10%", orderable: false },
      { data: "Note", width: "10%", orderable: false },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let icn = `<i class="ti fs-2 text-success ti-circle-check"></i>`;
          if (row.Primary == "False" || row.Primary == "0") {
            icn = `<i class="ti fs-2 text-danger ti-circle-x"></i>`;
          }
          return `<div class="text-center">${icn}</div>`;
        },
      },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

let AdressesServerSide;
const bindAddresses = (customerid, params) => {
  if (AdressesServerSide) {
    AdressesServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  AdressesServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/AdressesServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      {
        data: "No",
        width: "5%",
        orderable: false,
        render: function (data, type, row, meta) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      { data: "Description", width: "10%", orderable: false },
      { data: "Address", width: "40%", orderable: false },
      { data: "Port", width: "10%", orderable: false },
      { data: "Tags", width: "10%", orderable: false },
      { data: "Instruction", width: "10%", orderable: false },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let icn = `<i class="ti fs-2 text-success ti-circle-check"></i>`;
          if (row.Primary == "False" || row.Primary == "0") {
            icn = `<i class="ti fs-2 text-danger ti-circle-x"></i>`;
          }
          return `<div class="text-center">${icn}</div>`;
        },
      },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

let LoginsServerSide;
const bindLogins = (customerid, params) => {
  if (LoginsServerSide) {
    LoginsServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  LoginsServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/LoginsServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      {
        data: "No",
        width: "5%",
        orderable: false,
        render: function (data, type, row, meta) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      { data: "Application", width: "20%", orderable: false },
      { data: "Role", width: "10%", orderable: false },
      { data: "Username", width: "10%", orderable: false },
      { data: "Name", width: "20%", orderable: false },
      {
        data: null,
        width: "15%",
        orderable: false,
        render: function (data, type, row) {
          const rawDate = row.LastLogin;
          if (!rawDate) return `<div class="text-center">-</div>`;

          const date = parseDateFlexible(rawDate);
          if (!date) return `<div class="text-center">Invalid date</div>`;

          const day = String(date.getDate()).padStart(2, "0");
          const month = date.toLocaleString("en-US", { month: "short" }); // Oct
          const year = String(date.getFullYear()).slice(-2); // 25
          const time = date.toLocaleTimeString("en-GB"); // 09:05:00

          const formatted = `${day} ${month} ${year} ${time}`;
          return formatted;
        },
      },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let icn = `<i class="ti fs-2 text-success ti-circle-check"></i>`;
          if (row.Active == "False" || row.Active == "0") {
            icn = `<i class="ti fs-2 text-danger ti-circle-x"></i>`;
          }
          return `<div class="text-center">${icn}</div>`;
        },
      },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

let DiscountsServerSide;
const bindDiscounts = (customerid, params) => {
  if (DiscountsServerSide) {
    DiscountsServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  DiscountsServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/DiscountsServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      {
        data: "No",
        width: "5%",
        orderable: false,
        render: function (data, type, row, meta) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      { data: "Id", width: "10%", orderable: false },
      { data: "Title", width: "40%", orderable: false },
      { data: "Discount", width: "10%", orderable: false },
      { data: "StartDate", width: "10%", orderable: false },
      { data: "EndDate", width: "10%", orderable: false },
      { data: "FinalDiscount", width: "10%", orderable: false },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

let ProductAccessServerSide;
const bindProductAccess = (customerid, params) => {
  if (ProductAccessServerSide) {
    ProductAccessServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  ProductAccessServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/ProductAccessServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      { data: "Product", width: "95%", orderable: false },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

let QuotesServerSide;
const bindQuotes = (customerid, params) => {
  if (QuotesServerSide) {
    QuotesServerSide.destroy();
  }

  const paramData = {
    customerid: customerid,
  };

  QuotesServerSide = $(params).DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 50,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      return stylingColumnSearchAndPaging(params);
    },
    ajax: {
      url: uriMethod + "/QuotesServerSide",
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
        // console.table(json.d.data);
        return json.d.data;
      },
      complete: function () {
        // loaderFadeOut();
      },
      error: function (xhr, thrownError, textStatus) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },

    columns: [
      { data: "Logo", width: "50%", orderable: false },
      { data: "Terms", width: "45%", orderable: false },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

// ----------------------------------------------|| Other Function ||-------------------------------------------
const checkSessionCusDetail = () => {
  if (!customerDetail) window.location.href = "/setting/customer";

  const headerDefaultActive = "contact";
  const headerDefaultKey = "active";

  const tabDefaultActive = "tabs-contact";
  const tabDefaultKey = "active show";

  const headerActive = getState("customer_detail_tab_header_active_val");
  const headerKey = getState("customer_detail_tab_header_active_key");
  const tabActive = getState("customer_detail_tab_active_val");
  const tabKey = getState("customer_detail_tab_active_key");

  const headerActiveFinal = headerActive ? headerActive : headerDefaultActive;
  const headerKeyFinal = headerKey ? headerKey : headerDefaultKey;

  const elHeader = document.querySelector(`#${headerActiveFinal}`);
  if (elHeader) {
    elHeader.classList.add(...headerKeyFinal.split(" "));
  }

  const tabActiveFinal = tabActive ? tabActive : tabDefaultActive;
  const tabKeyFinal = tabKey ? tabKey : tabDefaultKey;

  const el = document.querySelector(`#${tabActiveFinal}`);
  if (el) {
    el.classList.add(...tabKeyFinal.split(" "));
  }

  bindCustomerDetail(customerDetail);
};

const parseDateFlexible = (value) => {
  if (!value) return null;

  // Jika format mengandung tanda "/" → dd/MM/yyyy HH:mm:ss
  if (value.includes("/")) {
    const [d, m, yAndTime] = value.split("/");
    const [y, time] = yAndTime.split(" ");
    const formatted = `${y.trim()}-${m.trim()}-${d.trim()}T${time.trim()}`;
    const date = new Date(formatted);
    return isNaN(date) ? null : date;
  }

  // Jika format seperti "2025-03-28 15:37:24.000"
  if (value.includes("-")) {
    const formatted = value.replace(" ", "T").replace(".000", "");
    const date = new Date(formatted);
    return isNaN(date) ? null : date;
  }

  return null;
};

// --------------------------------------------------||Additional data table styling ||-------------------------------------------
const dropdownActionButton = (data, type, row, params) => {
  let act = `
  <div class="dropdown text-center">
            <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
            </button>
              <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
              `;

  if (params == "#tabs-contact #data-table") {
    act += `
                  <li>
                    <a class="dropdown-item" href="javascript:void(0)" id="btn-detail" data-id="${row.Id}">
                      <i class="ti ti-edit me-1 opacity-50 fs-2" ></i>Edit / Detail
                    </a>
                  </li>
                  <li>
                    <a class="dropdown-item text-danger" href="javascript:void(0)" id="btn-danger" data-id="${row.Id}">
                      <i class="ti ti-trash-x me-1 opacity-50 fs-2" ></i>Delete
                    </a>
                  </li>
  
                  <div class="dropdown-divider"></div>
                  <li>
                    <a class="dropdown-item " href="javascript:void(0)" id="btn-set-primary" data-id="${row.Id}">
                      <i class="ti ti-key me-1 opacity-50 fs-2"></i>Set As Primary Contact
                    </a>
                  </li>
                  <li>
                    <a class="dropdown-item " href="javascript:void(0)" id="btn-log" data-id="${row.Id}">
                      <i class="ti ti-logout me-1 opacity-50 fs-2"></i>Logs
                    </a>
                  </li>
                  `;
  } else if (params == "#tabs-logins #data-table") {
    act += `
                  <li>
                    <a class="dropdown-item" href="javascript:void(0)" id="btn-detail" data-id="${row.Id}">
                      <i class="ti ti-edit me-1 opacity-50 fs-2" ></i>Edit / Detail
                    </a>
                  </li>
                  <li>
                    <a class="dropdown-item" href="javascript:void(0)" id="btn-swtich" data-id="${row.Id}">
                      <i class="ti ti-switch-horizontal me-1 opacity-50 fs-2" ></i>Switch Activation
                    </a>
                  </li>
                  <li>
                    <a class="dropdown-item text-danger" href="javascript:void(0)" id="btn-danger" data-id="${row.Id}">
                      <i class="ti ti-trash-x me-1 opacity-50 fs-2" ></i>Delete
                    </a>
                  </li>

                  <div class="dropdown-divider"></div>
                  <li>
                    <a class="dropdown-item " href="javascript:void(0)" id="btn-reset-password" data-id="${row.Id}">
                      <i class="ti ti-password me-1 opacity-50 fs-2"></i>Reset Password
                    </a>
                  </li>

                  <li>
                    <a class="dropdown-item " href="javascript:void(0)" id="btn-show-password" data-id="${row.Id}">
                      <i class="ti ti-password-user me-1 opacity-50 fs-2"></i>Show Password
                    </a>
                  </li>

                  <div class="dropdown-divider"></div>
                  <li>
                    <a class="dropdown-item " href="javascript:void(0)" id="btn-log" data-id="${row.Id}">
                      <i class="ti ti-logout me-1 opacity-50 fs-2"></i>Logs
                    </a>
                  </li>
                  `;
  } else {
    act += ` <li class="text-danger">Something went wrong</li>`;
  }

  act += `</ul>
          </div>`;
  return act;
};

const stylingColumnSearchAndPaging = (params) => {
  const filterWrapper = $(params + "_filter");
  const lengthSelect = $(params + "_length select");

  switch (params) {
    case "#tabs-contact #data-table":
    case "#tabs-address #data-table":
    case "#tabs-discount #data-table":
    case "#tabs-product-access #data-table":
    case "#tabs-quotes #data-table":
      filterWrapper.hide();
      lengthSelect.hide();
      return;
  }

  // search
  const input = filterWrapper.find("input");
  input
    .addClass("form-control form-control-sm")
    .attr("placeholder", "🔍 Type here to search...")
    .css({
      width: "250px",
      height: "40px",
      fontSize: "15px",
    });

  //select show entries
  lengthSelect.addClass("form-select form-select-sm").css({
    width: "65px",
    fontSize: "15px",
    height: "40px",
  });
};
