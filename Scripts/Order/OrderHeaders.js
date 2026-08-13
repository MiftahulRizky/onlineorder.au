document.addEventListener("DOMContentLoaded", function () {
  if (ROLENAME == "Administrator" || ROLENAME == "Customer") {
    console.log("Default.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("CUSTOMERID: " + CUSTOMERID);
    console.log("USERNAME: " + USERNAME);
    console.log("FULLNAME: " + FULLNAME);
    console.log("ONSTOP: " + ONSTOP);
    console.log("CUSTOMERCOMPANY: " + CUSTOMERCOMPANY);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("URIMETHOD: " + URIMETHOD);
    console.log("LOGINID: " + LOGINID);
    console.log("CUSTOMERACCOUNT: " + CUSTOMERACCOUNT);
  }
  orderHeadersPageLoaded();
});
// ==========================================INITIALIZATION=================================================
let DataTableOrders;
// =================================================EVENTS==================================================
// =================================================FUNCTION================================================
// ------------------------------------------||Binding Function ||-------------------------------------------
const bindOrderHeaderAggregate = async () => {
  try {
    const status = document.querySelector("#cardOrder #status").value || "ALL";
    const ordertype =
      document.querySelector("#cardOrder #ordertype").value || "ALL";
    const active = document.querySelector("#cardOrder #active").value;
    const storetype = document.querySelector("#cardOrder #storetype").value;

    const response = await fetch(`${URIMETHOD}/BindOrderHeaderAggregate`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          status: status,
          ordertype: ordertype,
          active: active,
          storetype: storetype,
          search: "",
          loginid: LOGINID,
          customerid: CUSTOMERID,
          customeraccount: CUSTOMERACCOUNT,
          customercompany: CUSTOMERCOMPANY,
          rolename: ROLENAME,
          levelname: LEVELNAME,
          username: USERNAME,
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

    generateOption("#ordertype", data.ProductType, 10);
    bindStatus("#cardOrder #status");
    bindOrders(data.Orders);
  } catch (error) {
    let msg = `bindOrderHeaderAggregate: ${error.message}`;
    catchMessages(msg);
  } finally {
    loaderFadeOut();
  }
};

const bindStatus = (params, statusNow = null) => {
  if (!params) return;

  let data = [];
  if (params === "#cardOrder #status") {
    data.push(
      "ALL",
      "Pending Price Approval",
      "Draft",
      "New Order",
      "In Production",
      "On Hold",
      "Completed",
      "Canceled",
    );
  }

  if (params === "#modalChangeStatus #status" && statusNow) {
    data.push({ value: "Draft", text: "Draft" });

    if (["Pending Price Approval"].includes(statusNow)) {
      data.push("Pending Price Approval");
    }

    if (["Draft"].includes(statusNow)) {
      data.push("Canceled");
    }

    if (["New Order", "On Hold"].includes(statusNow)) {
      data.push("New Order", "In Production", "On Hold", "Canceled");
    }

    if (["In Production"].includes(statusNow)) {
      data.push("In Production", "On Hold", "Completed", "Canceled");
    }
  }

  generateOption(params, data, 10);
};

const bindOrders = (orders) => {
  try {
    if (!orders || orders.length === 0) return;

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
      { width: "10%", data: "OrderId", className: "text-center" },
      { width: "20%", data: "CustomerName" },
      { width: "10%", data: "OrderNumber" },
      { width: "10%", data: "OrderName" },
      { width: "5%", data: "OrderType" },
      { width: "10%", data: "Delivery" },
      { width: "12%", data: "Status" },
      { width: "5%", data: "CreatedDate" },
      { width: "5%", data: "SubmittedDate" },
      {
        width: "3%",
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

                <li class="liDetailOrder">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnDetailOrder" data-id="${row.Id}" data-type="${row.OrderType}">
                  <i class="ti ti-info-square-rounded me-1 fs-2 opacity-50"></i>Detail
                  </a>
                </li>

                <li class="liDeleteOrder">
                  <a class="dropdown-item text-danger" href="javascript:void(0)" id="btnDeleteOrder" data-id="${row.Id}" data-name="${row.CustomerName}" data-order="${row.OrderNumber}" data-ref="${row.OrderName}" data-del="${row.Delivery}" data-type="${row.OrderType}">
                    <i class="ti ti-trash-x me-1 fs-2 opacity-50"></i>Delete
                  </a>
                </li>

                <li class="liRestoreOrder">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnRestoreOrder" data-id="${row.Id}" data-name="${row.StoreName}" data-order="${row.OrderNo}" data-ref="${row.OrderCust}" data-del="${row.Delivery}" data-type="${row.OrderType}">
                    <i class="ti ti-restore me-1 fs-2 opacity-50"></i>Restore 
                  </a>
                </li>

                <li class="liChangeStatus">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnChangeStatus" data-id="${row.Id}">
                    <i class="ti ti-exchange me-1 fs-2 opacity-50"></i>Change Status
                  </a>
                </li>

                <div class="dropdown-divider liDividerLogs"></div>
                <li class="liLogs">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnLogs" data-id="${row.Id}" data-type="${row.OrderType}">
                    <i class="ti ti-logout me-1 fs-2 opacity-50"></i>Logs
                  </a>
                </li>

              </ul>
            </div>
          `;
        },
      },
    ];

    DataTableOrders = $("#tableOrders").DataTable({
      data: orders,
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
        stylingColumnSearchAndPaging("#tableOrders");
      },
    });
  } catch (error) {
    const msg = `bindOrders: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const orderHeadersPageLoaded = async () => {
  await bindOrderHeaderAggregate();
  //   await bindProductType();
  //   await Promise.all([
  //     handlerSelStatus("#cardOrder #status", null),
  //     visibleColumnServerside(),
  //   ]);
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  // const sel = document.getElementById(elementId);
  const sel = document.querySelector(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  if (
    !["#modalChangeStatus #status", "#cardOrder #status"].includes(elementId)
  ) {
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

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
