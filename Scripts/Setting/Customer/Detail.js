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
      await bindLogins(item.Id, "#tabs-logins #data-table");
      // await bindFabrics(item.DesignId);
      // await bindFabricColours(item.DesignId, item.FabricType);
      // await handlerElementVisibility(item.BlindId);
      // await handlerSetElementValues(item);
      await loaderFadeOut();
    }

    return true; // ✅ success
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
      { data: "Name", width: "10%" },
      { data: "Solution", width: "10%" },
      { data: "Role", width: "10%" },
      { data: "Email", width: "10%" },
      { data: "Phone", width: "10%" },
      { data: "Mobile", width: "10%" },
      { data: "Tags", width: "10%" },
      { data: "Note", width: "10%" },
      { data: "Primary", width: "10%" },
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
      { data: "Application", width: "10%" },
      { data: "Role", width: "10%" },
      { data: "Username", width: "10%" },
      { data: "Name", width: "40%" },
      { data: "LastLogin", width: "10%" },
      { data: "Active", width: "10%" },
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

// --------------------------------------------------||Additional data table styling ||-------------------------------------------
const dropdownActionButton = (data, type, row, params) => {
  return `<div class="dropdown text-center">
            <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
            </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btn-detail" data-id="${row.Id}">
                    <i class="ti ti-info-square-rounded me-1 opacity-50 fs-2" ></i>Detail
                  </a>
                </li>

                <div class="dropdown-divider"></div>
                <li>
                  <a class="dropdown-item " href="javascript:void(0)" id="btn-logs" data-id="${row.Id}">
                    <i class="ti ti-logout me-1 opacity-50 fs-2"></i>Logs
                  </a>
                </li>
                
              </ul>
          </div>`;
};

const stylingColumnSearchAndPaging = (params) => {
  const filterWrapper = $(params + "_filter");
  const lengthSelect = $(params + "_length select");

  if (params == "#tabs-contact #data-table") {
    filterWrapper.hide();
    lengthSelect.hide();
    return;
  }

  // search
  const input = filterWrapper.find("input");
  input
    .addClass("form-control form-control-sm") // ganti lg -> sm
    .attr("placeholder", "🔍 Type here to search...")
    .css({
      width: "250px",
      height: "40px",
      fontSize: "15px",
    });

  // if (filterWrapper.find(".btn-clear-search").length === 0) {
  //   // button add
  //   const btnNewContact = $(
  //     '<button type="button" class="btn btn-primary" id="btnNewContact">New Contact</button>'
  //   ).css({
  //     marginBottom: "5px",
  //     marginLeft: "5px",
  //   });
  //   const btnResetPrimaryContact = $(
  //     '<button type="button" class="btn btn-danger" id="btnResetPrimaryContact">Reset Primary Contact</button>'
  //   ).css({
  //     marginBottom: "5px",
  //     marginLeft: "5px",
  //   });
  //   const btnResetPrimaryContact2 = $(
  //     '<button type="button" class="btn btn-danger" id="btnResetPrimaryContact">Reset Primary Contact</button>'
  //   ).css({
  //     marginBottom: "5px",
  //     marginLeft: "5px",
  //   });

  //   // Masukkan setelah input
  //   input.after(btnNewContact, btnResetPrimaryContact, btnResetPrimaryContact2);
  // }

  //select show entries

  lengthSelect.addClass("form-select form-select-sm").css({
    width: "65px",
    fontSize: "15px",
    height: "40px",
  });
};
