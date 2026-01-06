document.addEventListener("DOMContentLoaded", function () {
  if (ROLENAME == "Administrator" || ROLENAME == "Customer Service") {
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
  checkSession();
});
// ==================================================EVENTS==================================================

// --------------------------------------------||cardOrder Event ||-------------------------------------------
// BUTTON DAILY MAIL
document.querySelector("#aDailyMail").addEventListener("click", (e) => {
  e.preventDefault();
  handlerSendProductionOrder();
});

// CHANGE FILTER STATUS
document.querySelector("#cardOrder #status").addEventListener("change", (e) => {
  const status = e.target.value;
  const ordertype = document.querySelector("#cardOrder #ordertype").value;
  const active = document.querySelector("#cardOrder #active").value;
  const storetype = document.querySelector("#cardOrder #storetype").value;

  // Simpan ke localStorage
  setState("filter_orders_status", status);
  setState("filter_orders_ordertype", ordertype);
  setState("filter_orders_active", active);
  setState("filter_orders_storetype", storetype);

  bindOrders(status, ordertype, active, storetype, "#cardOrder #tableAjax");
});

// CHANGE FILTER ORDERTYPE
document
  .querySelector("#cardOrder #ordertype")
  .addEventListener("change", (e) => {
    const status = document.querySelector("#cardOrder #status").value;
    const ordertype = e.target.value;
    const active = document.querySelector("#cardOrder #active").value;
    const storetype = document.querySelector("#cardOrder #storetype").value;

    // Simpan ke localStorage
    setState("filter_orders_status", status);
    setState("filter_orders_ordertype", ordertype);
    setState("filter_orders_active", active);
    setState("filter_orders_storetype", storetype);

    bindOrders(status, ordertype, active, storetype, "#cardOrder #tableAjax");
  });

// BUTTON CREATE ORDER
document
  .querySelector("#cardOrder #btnCreateNewOrder")
  .addEventListener("click", () => {
    // handlerCreateNewOrder();
    window.location.href = "/order/header?action=add";
  });

// CHANGE FILTER ACTIVE
document.querySelector("#cardOrder #active").addEventListener("change", (e) => {
  const status = document.querySelector("#cardOrder #status").value;
  const ordertype = document.querySelector("#cardOrder #ordertype").value;
  const active = e.target.value;
  const storetype = document.querySelector("#cardOrder #storetype").value;

  // Simpan ke localStorage
  setState("filter_orders_status", status);
  setState("filter_orders_ordertype", ordertype);
  setState("filter_orders_active", active);
  setState("filter_orders_storetype", storetype);

  bindOrders(status, ordertype, active, storetype, "#cardOrder #tableAjax");
});

// CHANGE FILTER STORE TYPE
document
  .querySelector("#cardOrder #storetype")
  .addEventListener("change", (e) => {
    const status = document.querySelector("#cardOrder #status").value;
    const ordertype = document.querySelector("#cardOrder #ordertype").value;
    const active = document.querySelector("#cardOrder #active").value;
    const storetype = e.target.value;

    // Simpan ke localStorage
    setState("filter_orders_status", status);
    setState("filter_orders_ordertype", ordertype);
    setState("filter_orders_storetype", storetype);

    bindOrders(status, ordertype, active, storetype, "#cardOrder #tableAjax");
  });

// --------------------------------------------|| tableAjax Event ||-------------------------------------------
// BUTTON DETAIL ORDER
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDetailOrder") {
    const id = e.target.dataset.id;
    const type = e.target.dataset.type;
    // handlerOpenDetailOrder(id);
    if (type == "Blinds") {
      window.location.href = `/order/detail?param=${id}&ordertype=${type.toLowerCase()}`;
    } else if (type == "Panorama" || type == "Evolve") {
      window.location.href = `/order/shutters/detail?param=${id}&ordertype=${type.toLowerCase()}`;
    } else {
      window.location.href = `/order`;
    }
  }
});

// BUTTON DATE INFO
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDateInfo") {
    const id = e.target.dataset.id;
    handlerDateInfo(id);
  }
});

// BUTTON CHANGE STATUS
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnChangeStatus") {
    document
      .querySelectorAll(
        "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
      )
      .forEach((el) => {
        el.classList.remove("is-invalid");
      });
    const id = e.target.dataset.id;
    handlerChangeStatus(id);
  }
});

// BUTTON DELLETE ORDER
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDeleteOrder") {
    const id = e.target.dataset.id;
    const name = e.target.dataset.name;
    const order = e.target.dataset.order;
    const ref = e.target.dataset.ref;
    const del = e.target.dataset.del;
    const type = e.target.dataset.type;
    handlerSwitch(id, name, order, ref, del, type, "delete");
  }
});

// BUTTON RESTORE ORDER
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnRestoreOrder") {
    const id = e.target.dataset.id;
    const name = e.target.dataset.name;
    const order = e.target.dataset.order;
    const ref = e.target.dataset.ref;
    const del = e.target.dataset.del;
    const type = e.target.dataset.type;
    handlerSwitch(id, name, order, ref, del, type, "restore");
  }
});

// BUTTON DOWNLOAD CSV
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDownloadCsv") {
    const id = e.target.dataset.id;
    if (ROLENAME !== "Administrator") {
      //  buat sweetalert jika bukan admin dengan pesan, "aksi ini akan segera hadir"
      Swal.fire({
        icon: "info",
        title: "Information",
        text: "This feature will be available soon",
        customClass: {
          popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
        },
      });
      return;
    }
    handlerDownloadCSV(id);
  }
});

// BUTTON LOGS
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnLogs") {
    const id = e.target.dataset.id;
    const ordertype = e.target.dataset.type;
    handlerLogs(id, ordertype);
  }
});
// --------------------------------------------||modalChangeStatus Event ||-------------------------------------------
// CHANGE STATUS
document
  .querySelector("#modalChangeStatus #status")
  .addEventListener("change", (e) => {
    document
      .querySelectorAll(
        "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
      )
      .forEach((el) => {
        el.classList.remove("is-invalid");
      });
    const status = e.target.value;
    hanlderDisplayElementModalChangeStatus(status);
  });

document
  .querySelectorAll(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  )
  .forEach((el) => {
    el.addEventListener("change", () => {
      el.classList.remove("is-invalid");
    });
    el.addEventListener("input", () => {
      el.classList.remove("is-invalid");
    });
  });

// TOOLTIP DESCRIPTION CLICK
document
  .querySelector("#modalChangeStatus #tooltipDescription")
  .addEventListener("click", (e) => {
    const status = document.querySelector("#modalChangeStatus #status").value;
    handlerTooltip("modalChangeStatus", status);
  });
// BUTTON SUBMIT CHANGE STATUS
document
  .querySelector("#modalChangeStatus #submitChangeStatus")
  .addEventListener("click", () => {
    submitChangeStatus();
  });
// ==================================================FUNCTIONS===============================================
// --------------------------------------------||Submit Function ||-------------------------------------------
const submitChangeStatus = async () => {
  // Hapus class invalid saat user ubah input
  document
    .querySelectorAll(
      "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
    )
    .forEach((el) => {
      ["change", "input"].forEach((evt) =>
        el.addEventListener(evt, () => el.classList.remove("is-invalid"))
      );
    });

  const fields = [
    "id",
    "status",
    "statusOld",
    "submitteddate",
    "completeddate",
    "canceleddate",
    "description",
  ];

  const paramsChangeStatus = { username: USERNAME };
  for (const field of fields) {
    const el = document.querySelector(`#modalChangeStatus #${field}`);
    paramsChangeStatus[field] = el ? el.value : "";
  }

  const btnSubmit = document.querySelector(
    "#modalChangeStatus #submitChangeStatus"
  );

  try {
    // === sebelum kirim ===
    btnSubmit.disabled = true;
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait while we update the status.");

    // === kirim request ===
    const response = await fetch(`${URIMETHOD}/UpdateStatusOrder`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ data: paramsChangeStatus }),
    });

    const result = await response.json();
    const resData = result.d || result;

    // === tangani hasil ===
    if (resData.error) {
      await isError(resData.error.message.toUpperCase());
      const fieldElement = document.querySelector(resData.error.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      await isSuccess(resData.success);
      handlerHideBSModal("modalChangeStatus");
      tableData.ajax.reload();
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? `${err.message}`
        : "Something went wrong, please try again!";
    isError(msg);
  } finally {
    // === setelah selesai ===
    btnSubmit.disabled = false;
    btnSubmit.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit`;
  }

  return false;
};

// --------------------------------------------||Binding Function ||-------------------------------------------
// BIND ORDERS
const bindOrders = async (status, ordertype, active, storetype, params) => {
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

  tableData = $(params).DataTable({
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
      } catch (err) {
        const msg = `${err.message}`;
        isError(msg);
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
      { width: "5%", data: "Id", orderable: false },
      { width: "10%", data: "OrderId", orderable: false },
      { width: "20%", data: "CustomerName", orderable: false },
      { width: "10%", data: "OrderNumber", orderable: false },
      { width: "10%", data: "OrderName", orderable: false },
      { width: "5%", data: "OrderType", orderable: false },
      {
        width: "10%",
        data: null,
        orderable: false,
        render: function (data, type, row) {
          let findDelivery = "";
          if (row.Delivery === "Pick Up") {
            findDelivery = `<span class='badge bg-pink-lt'><i class='bi bi-truck-front'></i> ${row.Delivery}</span>`;
          } else if (row.Delivery === "Delivery") {
            findDelivery = `<span class='badge bg-cyan-lt'><i class='bi bi-box-seam'></i> ${row.Delivery}</span>`;
          }
          return `<div class="text-center">${findDelivery}</div>`;
        },
      },
      {
        width: "12%",
        data: null,
        orderable: false,
        render: function (data, type, row) {
          let icon = "";
          let addStat = "";
          if (row.OrderType == "Panorama") addStat = row.StatusAdditional;
          switch (row.Status) {
            case "Draft":
            case "Unsubmitted":
              icon = `<i class="bi opacity-50 bi-stopwatch"></i>`;
              break;
            case "New Order":
              icon = `<i class="bi opacity-50 bi-clipboard-check"></i>`;
              break;
            case "In Production":
              icon = `<i class="bi opacity-50 bi-hourglass-split"></i>`;
              break;
            case "On Hold":
              icon = `<i class="bi opacity-50 bi-pause-circle"></i>`;
              break;
            case "Canceled":
              icon = `<i class="bi opacity-50 bi-x-circle"></i>`;
              break;
            case "Completed":
              icon = `<i class="bi opacity-50 bi-check-circle"></i>`;
              break;
          }
          return `${icon} ${row.Status} <br> <span class="text-secondary">${addStat}</span>`;
        },
      },
      { width: "5%", data: "CreatedDate", orderable: false },
      { width: "5%", data: "SubmittedDate", orderable: false },
      {
        width: "3%",
        data: null,
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row, params);
        },
      },
    ],
  });
};

// --------------------------------------------||Handler Function ||-------------------------------------------
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

// HANDLER SEND PRODUCTION ORDER
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
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(resultData.error.message.toUpperCase());
    } else {
      await isSuccess(resultData.success);
      tableData.ajax.reload();
    }
  } catch (error) {
    var msg = `${error.message || error}`;
    if (ROLENAME != "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    await isError(msg);
  }
};

// HANDLER CELECT STSTUS
const handlerSelStatus = (params, statusNow) => {
  if (!params) return;

  const sel = document.querySelector(params);
  if (!sel) return;

  sel.innerHTML = ""; // Reset options

  let data = [];

  // === cardOrder => status ===
  if (params === "#cardOrder #status") {
    if (ROLENAME === "PPIC & DE") {
      data = [
        { value: "all", text: "All" },
        { value: "New Order", text: "New Order" },
        { value: "In Production", text: "In Production" },
        { value: "On Hold", text: "On Hold" },
        { value: "Completed", text: "Completed" },
        { value: "Canceled", text: "Canceled" },
      ];
    } else if (ROLENAME == "Sunlight Product" || ROLENAME == "Account") {
      data = [
        { value: "all", text: "All" },
        { value: "New Order", text: "New Order" },
        { value: "In Production", text: "In Production" },
        { value: "On Hold", text: "On Hold" },
        { value: "Completed", text: "Completed" },
      ];
    } else if (ROLENAME == "Customer Service") {
      data = [
        { value: "all", text: "All" },
        { value: "Draft", text: "Draft / Unsubmitted" },
        { value: "New Order", text: "New Order" },
        { value: "In Production", text: "In Production" },
        { value: "Completed", text: "Completed" },
        { value: "Canceled", text: "Canceled" },
      ];
    } else {
      data = [
        { value: "all", text: "All" },
        { value: "Draft", text: "Draft / Unsubmitted" },
        { value: "New Order", text: "New Order" },
        { value: "In Production", text: "In Production" },
        { value: "On Hold", text: "On Hold" },
        { value: "Completed", text: "Completed" },
        { value: "Canceled", text: "Canceled" },
      ];
    }
  }

  // === modalChangeStatus => status ===
  if (params === "#modalChangeStatus #status" && statusNow) {
    switch (statusNow) {
      case "Draft":
        data = [
          { value: "New Order", text: "New Order" },
          { value: "Canceled", text: "Canceled" },
        ];
        if (ROLENAME !== "Administrator") {
          data.unshift({ value: "Draft", text: "Draft / Unsubmitted" });
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

    if (ROLENAME === "Administrator") {
      data.unshift({ value: "Draft", text: "Draft / Unsubmitted" });
    }
  }

  // === render option ===
  for (const { value, text } of data) {
    const option = document.createElement("option");
    option.value = value;
    option.textContent = text.toUpperCase();
    sel.appendChild(option);
  }

  // === cardOrder behavior ===
  if (params === "#cardOrder #status") {
    const uiStatus = sel.options[sel.selectedIndex]?.value || "";
    const uiOrderType =
      document.querySelector("#cardOrder #ordertype")?.value || "";
    const uiActive = document.querySelector("#cardOrder #active")?.value || "";
    const uiStoreType =
      document.querySelector("#cardOrder #storetype")?.value || "";

    const statusToUse = getState("filter_orders_status") || uiStatus;
    const orderTypeToUse = getState("filter_orders_ordertype") || uiOrderType;
    const activeToUse = getState("filter_orders_active") || uiActive;
    const storeTypeToUse = getState("filter_orders_storetype") || uiStoreType;

    // Update filter UI
    setFilterValues(statusToUse, orderTypeToUse, activeToUse, storeTypeToUse);

    // Jika bindOrders adalah fungsi async, kita tunggu dulu
    bindOrders(
      statusToUse,
      orderTypeToUse,
      activeToUse,
      storeTypeToUse,
      "#cardOrder #tableAjax"
    );
  }
};

// HANDLER OPEN DETAIL ORDER
const handlerOpenDetailOrder = async (headerid) => {
  try {
    const response = await fetch(`${URIMETHOD}/SetSessionOpenOrderDetail`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    // Jika sukses, arahkan ke halaman detail order
    window.location.href = "/order/detail";
  } catch (error) {
    isError("Gagal menyetel session: " + error.message);
  }
};

// HANDLER CHANGE STATUS
const handlerChangeStatus = async (headerid) => {
  if (!headerid) return;

  try {
    let timer = setTimeout(() => {
      swalLoadingShow("Please wait ...");
    }, 3000);

    const response = await fetch(`${URIMETHOD}/BindOrderId`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid }),
    });

    clearTimeout(timer);

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerChangeStatus"
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
      return;
    }

    // Jalankan tiap item secara berurutan
    for (const item of data) {
      handlerSelStatus("#modalChangeStatus #status", item.Status);
      setValueModalChangeStatus(item);
      hanlderDisplayElementModalChangeStatus(item.Status);
      await Swal.close();
      handlerShowBSModal("modalChangeStatus");
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

const setValueModalChangeStatus = async (itemData) => {
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

// HANDLER DATE INFORMATION
const handlerDateInfo = async (headerid) => {
  if (!headerid) return;

  try {
    let timer = setTimeout(() => {
      swalLoadingShow("Please wait ...");
    }, 3000);

    const response = await fetch(`${URIMETHOD}/BindOrderId`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid }),
    });

    clearTimeout(timer);

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerDateInfo"
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
      return;
    }

    for (const item of data) {
      await setValueModalDateInfo(item);
      await Swal.close();
      await handlerShowBSModal("modalDateInfo");
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

const setValueModalDateInfo = (itemData) => {
  const mapping = {
    createddate: "CreatedDate",
    submitteddate: "SubmittedDate",
    completeddate: "CompletedDate",
    canceleddate: "CanceledDate",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.querySelector("#modalDateInfo #" + id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]];
    if (value) {
      const date = parseCustomDate(value);

      if (!date || isNaN(date.getTime())) {
        console.warn(`Tanggal tidak valid untuk '${mapping[id]}':`, value);
        el.value = "-";
        return;
      }

      if (ROLENAME === "Administrator") {
        const options = {
          weekday: "long",
          year: "numeric",
          month: "long",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
          hour12: false,
        };
        value = date.toLocaleDateString("id-ID", options).replace(/\./g, ":");
      } else {
        const options = {
          year: "numeric",
          month: "long",
          day: "2-digit",
        };
        value = date.toLocaleDateString("en-US", options);
      }
    }

    el.value = value;
  });
};

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

// HANDLER DELETE & RESTORE ORDER
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
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(resultData.error.message.toUpperCase());
    } else {
      await isSuccess(resultData.success);
      tableData.ajax.reload();
    }
  } catch (error) {
    const msg = `${error.message || error}`;
    await isError(msg);
  }
};

// HANDLER DOWLOAD CSV ORDER
const handlerDownloadCSV = async (headerId) => {
  let timerInterval;

  const result = await Swal.fire({
    title: "Downloading...",
    html: "Is downloading, I will close in <b></b> milliseconds.",
    timer: 2000,
    timerProgressBar: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    didOpen: () => {
      Swal.showLoading();
      const timer = Swal.getPopup().querySelector("b");
      timerInterval = setInterval(() => {
        timer.textContent = `${Swal.getTimerLeft()}`;
      }, 100);
    },
    willClose: () => {
      clearInterval(timerInterval);
    },
  });

  // hanya lanjut download jika Swal ditutup oleh timer
  if (result.dismiss === Swal.DismissReason.timer) {
    try {
      const response = await fetch(`${URIMETHOD}/DownloadCSVOrder`, {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify({ HeaderId: headerId }), // huruf besar 'H' sesuai server
      });

      if (!response.ok) {
        throw new Error(`HTTP error ${response.status}`);
      }

      // jika response JSON, ambil nilai string CSV
      const resultData = await response.json();
      const csvString = resultData.d || resultData;

      // buat blob file CSV
      const blob = new Blob([csvString], {
        type: "text/csv;charset=utf-8;",
      });

      // buat URL blob sementara
      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `-SPOD-ID-${headerId}.csv`;
      document.body.appendChild(link);
      link.click();

      // bersihkan URL blob dan elemen link
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    } catch (error) {
      const msg =
        ROLENAME === "Administrator"
          ? error.message
          : "Something went wrong while downloading the CSV.";
      await isError(msg);
    }
  }
};

// HANLDER LOGS
const handlerLogs = async (id, ordertype) => {
  try {
    const response = await fetch(`${URIMETHOD}/Logs`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id: id, ordertype: ordertype }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(resultData.error.message.toUpperCase());
    } else {
      const table = document.querySelector("#modalLogs #table-logs tbody");
      table.innerHTML = "";

      const logs =
        typeof resultData === "string" ? JSON.parse(resultData) : resultData;

      if (logs.length === 0) {
        const tr = document.createElement("tr");
        tr.innerHTML = `
        <td  class="text-center">
          No logs found
        </td>
        `;
        table.appendChild(tr);
      }

      logs.forEach((log) => {
        const formattedDate = formatDotNetDate(log.ActionDate);
        const tr = document.createElement("tr");
        tr.innerHTML = `
        <td >
          <b>${log.FullName}</b> on ${formattedDate}. Action: ${log.Description} 
        </td>
        `;
        table.appendChild(tr);
      });

      await handlerShowBSModal("modalLogs");
    }
    // await handlerShowBSModal("modalLogs");
  } catch (error) {
    const msg = `${error.message || error}`;
    await isError(msg);
  }
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
    icon: "question",
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
  });
};
// --------------------------------------------||Other Function ||-------------------------------------------
// CHECK SESSION
const checkSession = () => {
  handlerSelStatus("#cardOrder #status", null);
  visibleColumnServerside();
};

const setState = (name, value) => {
  if (!name && !value) return console.warn("setState: name and value required");
  localStorage.setItem(name, value);
};

const getState = (name) => {
  if (!name) return console.warn("getState: name required");
  return localStorage.getItem(name);
};

const setFilterValues = (status, ordertype, active, storeType) => {
  document.querySelector("#cardOrder #status").value = status;
  document.querySelector("#cardOrder #ordertype").value = ordertype;
  document.querySelector("#cardOrder #active").value = active;
  document.querySelector("#cardOrder #storetype").value = storeType;
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

// --------------------------------------------||Additional Datatable Function ||-------------------------------------------
const dropdownActionButton = (data, type, row, params) => {
  // --------------------|| Visible Button ||--------------------#
  let act;
  let displayEditOrDetail = "d-none";
  let displayDelete = "d-none";
  let displayChangeStatus = "d-none";
  let displayDownloadCSV = "d-none";
  let displayRestore = "d-none";

  //...............................|| Display Edit / Detail Button ||...............................//
  if (row.Status === "Draft" || row.Status === "Unsubmitted") {
    displayEditOrDetail = "";

    if (ROLENAME === "Customer" || ROLENAME === "Representative") {
    }
  }
  //...............................|| Display Delete Button ||...............................//
  if (row.Status === "Draft" || row.Status === "Unsubmitted") {
    if (ROLENAME == "Administrator") displayDelete = "";

    if (
      (ROLENAME === "PPIC & DE" ||
        ROLENAME === "Data Entry" ||
        ROLENAME === "Customer Service") &&
      CUSTOMERID !== row.CustomerId
    ) {
      displayDelete = "d-none";
    }

    if (ROLENAME === "Customer" || ROLENAME === "Representative") {
      displayDelete = "";
    }
  }

  if (row.Active === "False" || row.Active === "0") displayDelete = "d-none";

  //...............................|| Display Change Status Button ||...............................//
  if (
    ROLENAME === "Administrator" ||
    ROLENAME === "PPIC & DE" ||
    ROLENAME === "Customer Service"
  ) {
    displayChangeStatus = "";
  }

  if (row.Status === "Completed" || row.Status === "Canceled") {
    displayChangeStatus = "d-none";

    if (ROLENAME === "Administrator" && LEVELNAME === "Super Admin") {
      displayChangeStatus = "";
    }
  }

  if (row.Active === "False" || row.Active === "0")
    displayChangeStatus = "d-none";

  //...............................|| Display Download CSV Button ||...............................//
  // if (ROLENAME === "Administrator" && LEVELNAME === "Super Admin") {
  //   if (row.Status !== "Draft" && row.Status !== "Canceled") {
  //     displayDownloadCSV = "";
  //   }
  // }

  //...............................|| Display Restore Button ||...............................//
  if (
    ROLENAME === "Administrator" &&
    LEVELNAME === "Super Admin" &&
    (row.Active === "False" || row.Active === "0")
  ) {
    displayRestore = "";
  }

  act = `<div class="dropdown text-center">
            <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
            </button>
            <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">`;
  act += `<li>
            <a class="dropdown-item" href="javascript:void(0)" id="btnDetailOrder" data-id="${row.Id}" data-type="${row.OrderType}">
             <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-alert-square-rounded"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>Detail
            </a>
          </li>
          <li class="${displayDelete}">
            <a class="dropdown-item text-danger" href="javascript:void(0)" id="btnDeleteOrder" data-id="${row.Id}" data-name="${row.CustomerName}" data-order="${row.OrderNumber}" data-ref="${row.OrderName}" data-del="${row.Delivery}" data-type="${row.OrderType}">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-trash"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 7l16 0" /><path d="M10 11l0 6" /><path d="M14 11l0 6" /><path d="M5 7l1 12a2 2 0 0 0 2 2h8a2 2 0 0 0 2 -2l1 -12" /><path d="M9 7v-3a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v3" /></svg>Delete
            </a>
          </li>`;

  if (row.OrderType == "Blinds") {
    act += `<li class="${displayRestore}">
              <a class="dropdown-item" href="javascript:void(0)" id="btnRestoreOrder" data-id="${row.Id}" data-name="${row.StoreName}" data-order="${row.OrderNo}" data-ref="${row.OrderCust}" data-del="${row.Delivery}" data-type="${row.OrderType}">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-restore"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3.06 13a9 9 0 1 0 .49 -4.087" /><path d="M3 4.001v5h5" /><path d="M12 12m-1 0a1 1 0 1 0 2 0a1 1 0 1 0 -2 0" /></svg>Restore 
              </a>
            </li>
            <div class="dropdown-divider"></div>
            <li>
              <a class="dropdown-item" href="javascript:void(0)" id="btnDateInfo" data-id="${row.Id}">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-calendar-week"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 7a2 2 0 0 1 2 -2h12a2 2 0 0 1 2 2v12a2 2 0 0 1 -2 2h-12a2 2 0 0 1 -2 -2v-12z" /><path d="M16 3v4" /><path d="M8 3v4" /><path d="M4 11h16" /><path d="M7 14h.013" /><path d="M10.01 14h.005" /><path d="M13.01 14h.005" /><path d="M16.015 14h.005" /><path d="M13.015 17h.005" /><path d="M7.01 17h.005" /><path d="M10.01 17h.005" /></svg>Date Information
              </a>
            </li>
            
            <li class="${displayChangeStatus}">
              <a class="dropdown-item" href="javascript:void(0)" id="btnChangeStatus" data-id="${row.Id}">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-checkup-list"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 5h-2a2 2 0 0 0 -2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-12a2 2 0 0 0 -2 -2h-2" /><path d="M9 3m0 2a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v0a2 2 0 0 1 -2 2h-2a2 2 0 0 1 -2 -2z" /><path d="M9 14h.01" /><path d="M9 17h.01" /><path d="M12 16l1 1l3 -3" /></svg>Change Status
              </a>
            </li>
            <li class="${displayDownloadCSV}">
              <a class="dropdown-item" href="javascript:void(0)" id="btnDownloadCsv" data-id="${row.Id}">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-file-type-csv"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M14 3v4a1 1 0 0 0 1 1h4" /><path d="M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4" /><path d="M7 16.5a1.5 1.5 0 0 0 -3 0v3a1.5 1.5 0 0 0 3 0" /><path d="M10 20.25c0 .414 .336 .75 .75 .75h1.25a1 1 0 0 0 1 -1v-1a1 1 0 0 0 -1 -1h-1a1 1 0 0 1 -1 -1v-1a1 1 0 0 1 1 -1h1.25a.75 .75 0 0 1 .75 .75" /><path d="M16 15l2 6l2 -6" /></svg>Download CSV Order 
              </a>
            </li>`;
  }

  act += `<div class="dropdown-divider"></div>
          <li>
            <a class="dropdown-item" href="javascript:void(0)" id="btnLogs" data-id="${row.Id}" data-type="${row.OrderType}">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler opacity-50 me-1 icons-tabler-outline icon-tabler-logout"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M14 8v-2a2 2 0 0 0 -2 -2h-7a2 2 0 0 0 -2 2v12a2 2 0 0 0 2 2h7a2 2 0 0 0 2 -2v-2" /><path d="M9 12h12l-3 -3" /><path d="M18 15l3 -3" /></svg>Logs
            </a>
          </li>`;
  // if (row.OrderType == "Panorama") {
  // }

  act += `</ul>
            </div>`;

  return act;
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

const visibleColumnServerside = () => {
  const id = document.querySelectorAll("#tableAjax .column-id");
  const retailer = document.querySelectorAll("#tableAjax .column-retailer");
  const ordertype = document.querySelectorAll("#tableAjax .column-type");
  const filterOrderType = document.querySelector("#cardOrder #ordertype");

  tableData.columns(1).visible(false); // ID
  tableData.columns(3).visible(false); // RETAILER
  tableData.columns(6).visible(true); // ORDER TYPE
  id.forEach((item) => item.setAttribute("hidden", true));
  retailer.forEach((item) => item.setAttribute("hidden", true));
  ordertype.forEach((item) => item.removeAttribute("hidden"));
  filterOrderType.setAttribute("hidden", true);

  if (
    ROLENAME == "Administrator" &&
    (LEVELNAME == "Leader" || LEVELNAME == "Super Admin")
  ) {
    tableData.columns(1).visible(true); // ID
    id.forEach((item) => item.removeAttribute("hidden"));
    filterOrderType.removeAttribute("hidden");
  }

  if (
    ROLENAME == "Administrator" ||
    ROLENAME == "Customer Service" ||
    ROLENAME == "Data Entry" ||
    ROLENAME == "PPIC & DE"
  ) {
    tableData.columns(3).visible(true); // RETAILER;
    retailer.forEach((item) => item.removeAttribute("hidden"));
  }

  if (
    ROLENAME == "Customer" &&
    (CUSTOMERID == "LS-A012" || CUSTOMERID == "LS-A333")
  ) {
    tableData.columns(3).visible(true); // RETAILER
    retailer.forEach((item) => item.removeAttribute("hidden"));
  }

  if (ROLENAME == "Customer" || ROLENAME == "Representative" || SESSION_SP) {
    tableData.columns(6).visible(false); // ORDER TYPE
    ordertype.forEach((item) => item.setAttribute("hidden", true));
  }

  const aDailyMail = document.querySelector("#aDailyMail");
  const aOtorisasi = document.querySelector("#aOtorisasi");
  const btnCreateNewOrder = document.querySelector("#btnCreateNewOrder");

  aDailyMail.setAttribute("hidden", true);
  aOtorisasi.setAttribute("hidden", true);
  btnCreateNewOrder.setAttribute("hidden", true);

  if (
    ROLENAME == "Administrator" &&
    (LEVELNAME == "Leader" || LEVELNAME == "Super Admin")
  ) {
    aDailyMail.removeAttribute("hidden");
  }

  if (ROLENAME == "Administrator") {
    // || ROLENAME == "Customer Service" ||
    // ROLENAME == "Data Entry" ||
    // ROLENAME == "PPIC & DE"
    aOtorisasi.removeAttribute("hidden");
  }

  if (
    ROLENAME == "Administrator" ||
    ROLENAME == "Customer Service" ||
    ROLENAME == "Data Entry" ||
    ROLENAME == "PPIC & DE" ||
    ROLENAME == "Representative"
  ) {
    btnCreateNewOrder.removeAttribute("hidden");
  }

  if (ROLENAME == "Customer" && ONSTOP == "False") {
    btnCreateNewOrder.removeAttribute("hidden");
  }
};
