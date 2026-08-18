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
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const selectorEl = (el) => document.querySelector(el);
const selectorElAll = (el) => document.querySelectorAll(el);

const mainEl = {
  aDailyMail: getById("aDailyMail"),
  aOtorisasi: getById("aOtorisasi"),
  status: selectorEl("#cardOrder #status"),
  ordertype: getById("ordertype"),
  btnCreateNewOrder: getById("btnCreateNewOrder"),
  active: getById("active"),
  lblactive: getById("lblactive"),
  storetype: getById("storetype"),
  lblstoretype: getById("lblstoretype"),
};

const liEl = {
  liDetailOrder: getByClass("liDetailOrder"),
  liDeleteOrder: getByClass("liDeleteOrder"),
  liRestoreOrder: getByClass("liRestoreOrder"),
  liDividerChangeStatus: getByClass("liDividerChangeStatus"),
  liChangeStatus: getByClass("liChangeStatus"),
  liDividerLogs: getByClass("liDividerLogs"),
  liLogs: getByClass("liLogs"),
};

const modalEl = {
  modalChangeStatus: getById("modalChangeStatus"),
  modalLogs: getById("modalLogs"),
};

// =================================================EVENTS==================================================
Object.values(mainEl).forEach((el) => {
  if (!el) return;

  el.addEventListener("click", (e) => {
    try {
      const id = e.currentTarget.id;
      if (id === "aDailyMail") {
        handlerSendProductionOrder();
      }
      if (id === "aOtorisasi") {
        // alert("aOtorisasi");
      }
      if (id === "btnCreateNewOrder") {
        window.location.href = "/order/header?action=add";
      }
    } catch (error) {
      const msg = `mainEl Click: ${error.message}`;
      catchMessages(msg);
    }
  });

  el.addEventListener("change", (e) => {
    try {
      const id = e.currentTarget.id;
      if (id === "status") {
        const status = e.target.value;
        const ordertype = mainEl.ordertype.value;
        const active = mainEl.active.value;
        const storetype = mainEl.storetype.value;

        setState("filter_orders_status", status);
        setState("filter_orders_ordertype", ordertype);
        setState("filter_orders_active", active);
        setState("filter_orders_storetype", storetype);

        bindOrders(
          status,
          ordertype,
          active,
          storetype,
          "#cardOrder #tableOrders",
        );
      }

      if (id === "ordertype") {
        const status = mainEl.status.value;
        const ordertype = e.target.value;
        const active = mainEl.active.value;
        const storetype = mainEl.storetype.value;

        setState("filter_orders_status", status);
        setState("filter_orders_ordertype", ordertype);
        setState("filter_orders_active", active);
        setState("filter_orders_storetype", storetype);

        bindOrders(
          status,
          ordertype,
          active,
          storetype,
          "#cardOrder #tableOrders",
        );
      }

      if (id === "active") {
        const status = mainEl.status.value;
        const ordertype = mainEl.ordertype.value;
        const active = e.target.value;
        const storetype = mainEl.storetype.value;

        setState("filter_orders_status", status);
        setState("filter_orders_ordertype", ordertype);
        setState("filter_orders_active", active);
        setState("filter_orders_storetype", storetype);

        bindOrders(
          status,
          ordertype,
          active,
          storetype,
          "#cardOrder #tableOrders",
        );
      }

      if (id === "storetype") {
        const status = mainEl.status.value;
        const ordertype = mainEl.ordertype.value;
        const active = mainEl.active.value;
        const storetype = e.target.value;

        setState("filter_orders_status", status);
        setState("filter_orders_ordertype", ordertype);
        setState("filter_orders_active", active);
        setState("filter_orders_storetype", storetype);

        bindOrders(
          status,
          ordertype,
          active,
          storetype,
          "#cardOrder #tableOrders",
        );
      }
    } catch (error) {
      const msg = `mainEl Change: ${error.message}`;
      catchMessages(msg);
    }
  });
});

selectorEl("#tableOrders").addEventListener("click", async (e) => {
  try {
    const key = e.target.id;
    if (key === "btnDetailOrder") {
      const id = e.target.dataset.id;
      const type = e.target.dataset.type;
      const isSp = ["Blinds", "Door", "Window", "Door and Window"].includes(
        type,
      );
      const isShutters = ["Panorama", "Evolve"].includes(type);
      let page = "/order";

      if (isSp) {
        page = `/order/detail`;
      }
      if (isShutters) {
        page = `/order/shutters/detail`;
      }

      const live = `${page}?param=${id}&ordertype=${type.toLowerCase()}`;
      const dev = `/order/orderdetails?param=${id}&ordertype=${type.toLowerCase()}`;
      window.location.href = live;
    }

    if (key === "btnChangeStatus") {
      selectorElAll(
        "#modalChangeStatus .form-control, #modalChangeStatus .form-select",
      ).forEach((el) => {
        el.classList.remove("is-invalid");
      });

      const id = e.target.dataset.id;
      const type = e.target.dataset.type;
      handlerChangeStatus(id, type);
    }

    if (key === "btnDeleteOrder") {
      const id = e.target.dataset.id;
      const name = e.target.dataset.name;
      const order = e.target.dataset.order;
      const ref = e.target.dataset.ref;
      const del = e.target.dataset.del;
      const type = e.target.dataset.type;
      handlerSwitch(id, name, order, ref, del, type, "delete");
    }

    if (key === "btnRestoreOrder") {
      const id = e.target.dataset.id;
      const name = e.target.dataset.name;
      const order = e.target.dataset.order;
      const ref = e.target.dataset.ref;
      const del = e.target.dataset.del;
      const type = e.target.dataset.type;
      handlerSwitch(id, name, order, ref, del, type, "restore");
    }

    if (key === "btnLogs") {
      const id = e.target.dataset.id;
      const ordertype = e.target.dataset.type;
      await bindlogs(id, ordertype);
      handlerShowBSModal("modalLogs");
    }
  } catch (error) {
    const msg = `DataTableOrders Click: ${error.message}`;
    catchMessages(msg);
  }
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
};

Object.entries(modalEl).forEach(([key, modal]) => {
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
// =================================================FUNCTION================================================
// ------------------------------------------||Binding Function ||-------------------------------------------
const bindProductType = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/BindProductType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        customerid: CUSTOMERID,
        username: USERNAME,
        rolename: ROLENAME,
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
    generateOption("#ordertype", data.list, 10);
  } catch (error) {
    let msg = `bindProductType: ${error.message}`;
    catchMessages(msg);
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
    data.push("Draft");

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

const bindOrders = async (status, ordertype, active, storetype, params) => {
  try {
    const paramData = {
      loginid: LOGINID,
      customerid: CUSTOMERID,
      customeraccount: CUSTOMERACCOUNT,
      customercompany: CUSTOMERCOMPANY,
      rolename: ROLENAME,
      levelname: LEVELNAME,
      username: USERNAME,
      status,
      ordertype,
      active,
      customeraccountfilter: storetype,
    };

    DataTableOrders = $(params).DataTable({
      processing: true,
      serverSide: true,
      stateSave: true,
      stateDuration: -1,
      order: [],
      pageLength: 25,
      autoWidth: false,
      bPaginate: true,
      bInfo: true,
      bFilter: true,
      bDestroy: true,
      language: {
        search: "",
        lengthMenu: "_MENU_",
      },
      initComplete: () => {
        stylingColumnSearchAndPaging(params);
      },
      drawCallback: function (settings) {
        const api = this.api();
        displayElOverall(api);
      },

      ajax: async (data, callback) => {
        try {
          const response = await fetch(`${URIMETHOD}/BindOrders`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify({
              params: {
                ...paramData,
                draw: data.draw,
                start: data.start,
                length: data.length,
                order: data.order,
                columns: data.columns,
                search: data.search,
              },
            }),
          });

          if (!response.ok) {
            throw new Error(`${response.status} ${response.statusText}`);
          }

          const json = await response.json();

          callback({
            draw: json.d.draw,
            recordsTotal: json.d.recordsTotal,
            recordsFiltered: json.d.recordsFiltered,
            data: json.d.data,
          });
        } catch (error) {
          const msg = `bindOrders callback: ${error.message}`;
          catchMessages(msg);
        } finally {
          loaderFadeOut();
        }
      },
      columns: [
        {
          width: "5%",
          data: "No",
          orderable: false,
          render: function (data, type, row, meta) {
            return `<div class="text-center">${data}</div>`;
          },
        },
        { width: "5%", className: "columnId", data: "Id", orderable: false },
        { width: "10%", data: "OrderId", orderable: false },
        {
          width: "20%",
          className: "columnRetailer",
          data: "CustomerName",
          orderable: false,
        },
        { width: "10%", data: "OrderNumber", orderable: false },
        { width: "10%", data: "OrderName", orderable: false },
        {
          width: "5%",
          className: "columnType",
          data: "OrderType",
          orderable: false,
        },
        { width: "10%", data: "Delivery", orderable: false },
        { width: "12%", data: "Status", orderable: false },
        { width: "5%", data: "CreatedDate", orderable: false },
        { width: "5%", data: "SubmittedDate", orderable: false },
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
                  <a class="dropdown-item" href="javascript:void(0)" id="btnRestoreOrder" data-id="${row.Id}" data-name="${row.CustomerName}" data-order="${row.OrderNo}" data-ref="${row.OrderCust}" data-del="${row.Delivery}" data-type="${row.OrderType}">
                    <i class="ti ti-restore me-1 fs-2 opacity-50"></i>Restore 
                  </a>
                </li>

                <div class="dropdown-divider liDividerChangeStatus"></div>
                <li class="liChangeStatus">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnChangeStatus" data-id="${row.Id}" data-type="${row.OrderType}">
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
      ],
    });
  } catch (error) {
    const msg = `bindOrders: ${error.message}`;
    catchMessages(msg);
  }
};

const bindlogs = async (headerid, ordertype) => {
  try {
    const response = await fetch(`${URIMETHOD}/BindLogs`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, ordertype }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    }

    const table = document.querySelector("#modalLogs #table-logs tbody");
    table.innerHTML = "";
    if (res.length === 0) {
      const tr = document.createElement("tr");
      tr.innerHTML = `
      <td class="text-center">
        No logs found
      </td>
    `;
      table.appendChild(tr);
    }

    if (res.length > 0) {
      res.forEach((log) => {
        const formattedDate = formatDotNetDate(log.ActionDate);

        const tr = document.createElement("tr");
        let itemid = `ID: <b>${log.ItemId}</b>`;
        if (!log.ItemId || log.ItemId == "0") {
          itemid = "";
        }
        tr.innerHTML = `
          <td>
            <b>${log.FullName}</b> on ${formattedDate}. Action: ${log.Description} ${itemid}
          </td>
        `;
        table.appendChild(tr);
      });
    }

    console.log(res);
  } catch (error) {
    const msg = `bindlogs: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| handler Functions ||---------------------------------------
const handlerSendProductionOrder = async () => {
  const result = await Swal.fire({
    title: "Send Production Order",
    html: "Are you sure you would like to do this?",
    icon: "question",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, send it!",
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
  });

  if (!result.isConfirmed) return;

  swalLoadingShow("Please wait ...");

  try {
    const response = await fetch(`${URIMETHOD}/SendProductionOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      // body: JSON.stringify({ id, action: act }),
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
      await isSuccess(res.message);
      DataTableOrders.ajax.reload();
    }
  } catch (error) {
    const msg = `handlerSendProductionOrder: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerChangeStatus = async (headerid, ordertype) => {
  try {
    if (!headerid || !ordertype) throw new Error("param not found");
    let timer = setTimeout(() => {
      swalLoadingShow("Please wait ...");
    }, 3000);

    const response = await fetch(`${URIMETHOD}/BindOrderByID`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid, ordertype }),
    });

    clearTimeout(timer);

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    const { d: data } = await response.json();
    if (data.error) {
      throw new Error(data.message);
    }
    if (!data || data.length === 0) {
      throw new Error("No data returned from server");
    }

    console.log(data);

    bindStatus("#modalChangeStatus #status", data.Status);
    setValModalChangeStatus(data);
    displayElmodalChangeStatus(data.Status);
    await Swal.close();
    handlerShowBSModal("modalChangeStatus");
  } catch (error) {
    const msg = `handlerChangeStatus: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerSwitch = async (id, name, order, ref, del, type, act) => {
  const title = act === "delete" ? "delete" : "restore";
  const textButton = act === "delete" ? "Yes, delete it!" : "Yes, restore it!";
  const icon = act === "delete" ? "warning" : "question";

  const result = await Swal.fire({
    title: name,
    html: `
      Sure to ${title} this data?<br/><br/>
      <b>Order No :</b> ${order}
      <b>Ref :</b> ${ref}
      <b>Del :</b> ${del}
    `,
    icon: icon,
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: textButton,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
  });

  if (!result.isConfirmed) return;

  try {
    const response = await fetch(`${URIMETHOD}/SwitchOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, action: act, type }),
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
      await isSuccess(res.message);
      DataTableOrders.ajax.reload();
    }
  } catch (error) {
    const msg = `handlerSwitch: ${error.message}`;
    catchMessages(msg);
  }
};
// ----------------------------------------------|| Display Functions ||---------------------------------------
const displayElOverall = (api) => {
  try {
    Object.values(mainEl).forEach((el) => toggleShow(el, false));

    Object.values(liEl).forEach((el) => {
      Array.from(el).forEach((col) => toggleShow(col, false));
    });

    // 1 Id, 3 CustomerName, 6 OrderType
    DataTableOrders.columns([1, 3, 6]).visible(false);

    const isAdmin = ROLENAME === "Administrator";
    const isPpicCsDe = ["PPIC & DE", "Data Entry", "Customer Service"].includes(
      ROLENAME,
    );
    const isCustomer = ROLENAME === "Customer";
    const isRepres = ROLENAME === "Representative";
    const isManager = ROLENAME === "Manager";

    const isLeadSup = LEVELNAME === "Leader" || LEVELNAME === "Super Admin";
    const isSupAdmin = LEVELNAME === "Super Admin";
    const isOnStop = ONSTOP === "False";

    api.rows({ page: "current" }).every(function () {
      const rowData = this.data();
      const rowNode = this.node();

      const liDetailOrder = rowNode.querySelector(".liDetailOrder");
      const liDeleteOrder = rowNode.querySelector(".liDeleteOrder");
      const liDividerChangeStatus = rowNode.querySelector(
        ".liDividerChangeStatus",
      );
      const liChangeStatus = rowNode.querySelector(".liChangeStatus");
      const liRestoreOrder = rowNode.querySelector(".liRestoreOrder");
      const liDividerLogs = rowNode.querySelector(".liDividerLogs");
      const liLogs = rowNode.querySelector(".liLogs");

      const isDraft = ["Draft", "Unsubmitted"].includes(rowData.StatusVal);
      const isFinish = ["Completed", "Canceled"].includes(rowData.StatusVal);
      const isActive = ["1", "True"].includes(rowData.Active);
      const isShutters = ["Panorama", "Evolve"].includes(rowData.OrderType);

      toggleShow(liDetailOrder, true);

      if (isDraft && isActive) {
        toggleShow(liDeleteOrder, true);
      }

      if (isAdmin) {
        if (isSupAdmin) {
          if (!isShutters && isActive) {
            toggleShow(liDividerChangeStatus, true);
            toggleShow(liChangeStatus, true);
          }
          toggleShow(liDividerLogs, true);
          toggleShow(liLogs, true);
        }

        if (!isActive && !isShutters) {
          toggleShow(liRestoreOrder, true);
        }
      }

      if (isPpicCsDe) {
        if (!isFinish && isActive && !isShutters) {
          toggleShow(liDividerChangeStatus, true);
          toggleShow(liChangeStatus, true);
        }
      }
    });

    if (isAdmin) {
      Object.values(mainEl).forEach((el) => toggleShow(el, true));
      DataTableOrders.columns([1, 3, 6]).visible(true);
    }

    if (isPpicCsDe) {
      toggleShow(mainEl.status, true);
      toggleShow(mainEl.ordertype, true);
      toggleShow(mainEl.btnCreateNewOrder, true);
      DataTableOrders.columns([3, 6]).visible(true);
    }

    if (isCustomer) {
      toggleShow(mainEl.status, true);
      toggleShow(mainEl.ordertype, true);
      toggleShow(mainEl.btnCreateNewOrder, true);
      DataTableOrders.columns(6).visible(true);
    }
  } catch (error) {
    const msg = `displayElOverall: ${error.message}`;
    catchMessages(msg);
  }
};
const displayElmodalChangeStatus = (status) => {
  modalEl.modalChangeStatus
    .querySelector("#divDescription")
    .classList.add("d-none");
  modalEl.modalChangeStatus
    .querySelector("#divSubmittedDate")
    .classList.add("d-none");
  modalEl.modalChangeStatus
    .querySelector("#divCompletedDate")
    .classList.add("d-none");
  modalEl.modalChangeStatus
    .querySelector("#divCanceledDate")
    .classList.add("d-none");

  if (status) {
    modalEl.modalChangeStatus
      .querySelector("#divDescription")
      .classList.remove("d-none");
    switch (status) {
      case "New Order":
        modalEl.modalChangeStatus
          .querySelector("#divSubmittedDate")
          .classList.remove("d-none");
        break;
      case "Completed":
        modalEl.modalChangeStatus
          .querySelector("#divCompletedDate")
          .classList.remove("d-none");
        break;
      case "Canceled":
        modalEl.modalChangeStatus
          .querySelector("#divCanceledDate")
          .classList.remove("d-none");
        break;
    }
  }
};
// ----------------------------------------------|| SetVal Functions ||---------------------------------------
const setValFilter = () => {
  try {
    const uiStatus = selectorEl("#cardOrder #status")?.value || "";
    const uiOrderType = selectorEl("#cardOrder #ordertype")?.value || "";
    const uiActive = selectorEl("#cardOrder #active")?.value || "";
    const uiStoreType = selectorEl("#cardOrder #storetype")?.value || "";

    const statusToUse = getState("filter_orders_status") || uiStatus;
    const orderTypeToUse = getState("filter_orders_ordertype") || uiOrderType;
    const activeToUse = getState("filter_orders_active") || uiActive;
    const storeTypeToUse = getState("filter_orders_storetype") || uiStoreType;

    selectorEl("#cardOrder #status").value = statusToUse;
    selectorEl("#cardOrder #ordertype").value = orderTypeToUse;
    selectorEl("#cardOrder #active").value = activeToUse;
    selectorEl("#cardOrder #storetype").value = storeTypeToUse;

    return {
      status: statusToUse,
      ordertype: orderTypeToUse,
      active: activeToUse,
      storetype: storeTypeToUse,
    };
  } catch (error) {
    const msg = `setValFilter: ${error.message}`;
    catchMessages(msg);
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
      DataTableOrders.ajax.reload();
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

const setValModalChangeStatus = async (itemData) => {
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
    const el = document.querySelector(`#modalChangeStatus #${id}`);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]] ?? "";

    // Jika input bertipe date dan format dd/mm/yyyy hh:mm:ss → ubah ke yyyy-mm-dd
    if (el.type === "date" && typeof value === "string") {
      const [datePart] = value.split(" "); // contoh: "16/07/2025"
      const parts = datePart.split("/"); // hasil: ["16", "07", "2025"]
      if (parts.length === 3) {
        value = `${parts[2]}-${parts[1]}-${parts[0]}`;
      } else {
        console.warn(`Format tanggal tidak sesuai: ${value}`);
        value = "";
      }
    }

    // Jika description: ambil teks setelah "Notes from the office:<br />" dan bersihkan tag HTML
    if (id === "description" && typeof value === "string") {
      const marker = "Notes from the office:<br />";
      if (value.includes(marker)) {
        value = value.split(marker)[1] || "";
      }
      // Hapus tag HTML dengan regex agar tidak ikut tampil
      value = value.replace(/<[^>]*>/g, "").trim();
    }

    // Set nilai ke elemen input / textarea
    el.value = value;
  });
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const orderHeadersPageLoaded = async () => {
  try {
    await bindProductType();
    bindStatus("#cardOrder #status", null);
    const { status, ordertype, active, storetype } = setValFilter() || {};
    await bindOrders(
      status,
      ordertype,
      active,
      storetype,
      "#cardOrder #tableOrders",
    );
  } catch (error) {
    const msg = `orderHeadersPageLoaded: ${error.message}`;
    catchMessages(msg);
  }
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

const setState = (name, value) => {
  if (!name && !value) return console.warn("setState: name and value required");
  localStorage.setItem(name, value);
};

const getState = (name) => {
  if (!name) return console.warn("getState: name required");
  return localStorage.getItem(name);
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
  console.error(msg);
};
