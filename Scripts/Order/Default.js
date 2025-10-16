document.addEventListener("DOMContentLoaded", function () {
  if (roleName == "Administrator") {
    console.log("Default.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("userId: " + userId);
    console.log("userName: " + userName);
    console.log("storeId: " + storeId);
    console.log("storeCompany: " + storeCompany);
    console.log("levelName: " + levelName);
    console.log("uriMethod: " + uriMethod);
  }
  checkSession();
});
// ==================================================EVENTS==================================================

// --------------------------------------------||cardOrder Event ||-------------------------------------------
// CHANGE FILTER STATUS
document.querySelector("#cardOrder #status").addEventListener("change", (e) => {
  const status = e.target.value;
  const active = document.querySelector("#cardOrder #active").value;
  const storetype = document.querySelector("#cardOrder #storetype").value;

  // Simpan ke localStorage
  setState("filter_orders_status", status);
  setState("filter_orders_active", active);
  setState("filter_orders_storetype", storetype);

  bindOrders(status, active, storetype);
});

// BUTTON CREATE ORDER
document
  .querySelector("#cardOrder #btnCreateNewOrder")
  .addEventListener("click", () => {
    handlerCreateNewOrder();
  });

// CHANGE FILTER ACTIVE
document.querySelector("#cardOrder #active").addEventListener("change", (e) => {
  const status = document.querySelector("#cardOrder #status").value;
  const active = e.target.value;
  const storetype = document.querySelector("#cardOrder #storetype").value;

  // Simpan ke localStorage
  setState("filter_orders_status", status);
  setState("filter_orders_active", active);
  setState("filter_orders_storetype", storetype);

  bindOrders(status, active, storetype);
});

// CHANGE FILTER STORE TYPE
document
  .querySelector("#cardOrder #storetype")
  .addEventListener("change", (e) => {
    const status = document.querySelector("#cardOrder #status").value;
    const active = document.querySelector("#cardOrder #active").value;
    const storetype = e.target.value;

    // Simpan ke localStorage
    setState("filter_orders_status", status);
    setState("filter_orders_active", active);
    setState("filter_orders_storetype", storetype);

    bindOrders(status, active, storetype);
  });

// BUTTON DETAIL ORDER
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDetailOrder") {
    const id = e.target.dataset.id;
    handlerOpenDetailOrder(id);
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
    handlerSwitch(id, name, order, ref, del, "delete");
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
    handlerSwitch(id, name, order, ref, del, "restore");
  }
});

// BUTTON DOWNLOAD CSV
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDownloadCsv") {
    const id = e.target.dataset.id;
    if (roleName !== "Administrator") {
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

  const paramsChangeStatus = { username: userName };
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
    const response = await fetch(`${uriMethod}/UpdateStatusOrder`, {
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
    }
  } catch (err) {
    const msg =
      roleName === "Administrator"
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
let tableData;
const bindOrders = (status, active, storetype) => {
  const paramData = {
    storeid: storeId,
    storecompany: storeCompany,
    userid: userId,
    rolename: roleName,
    levelname: levelName,
    status: status,
    active: active,
    storetype: storetype,
  };

  tableData = $("#tableAjax").DataTable({
    processing: true,
    serverSide: true,
    order: [],
    stateSave: true,
    stateDuration: -1,
    pageLength: 10,
    language: {
      search: "",
      lengthMenu: "_MENU_",
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    initComplete: function () {
      const input = $("#tableAjax_filter input");
      input
        .addClass("form-control form-control-sm")
        .attr("placeholder", "🔍 Type here to search...")
        .css({
          width: "250px",
          height: "40px",
          fontSize: "15px",
          display: "inline-block",
        });

      const lengthSelect = $("#tableAjax_length select");
      lengthSelect.addClass("form-select form-select-sm").css({
        width: "65px",
        fontSize: "15px",
        height: "40px",
      });
    },
    ajax: {
      url: uriMethod + "/BindOrders",
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
        return json.d.data;
      },
      complete: function () {
        loaderFadeOut();
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },
    columns: [
      {
        data: "No",
        orderable: false,
        render: function (data, type, row, meta) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      {
        data: null,
        render: function (data, type, row, meta) {
          let jobno = "";
          if (row.JoNumber)
            jobno = `<span class="badge badge-outline text-red">${row.JoNumber}</span>`;
          return `<div class="text-center">${jobno}</div>`;
        },
      },
      { data: "StoreName" },
      { data: "OrderNo" },
      { data: "OrderCust" },
      {
        data: null,
        orderable: false,
        render: function (data, type, row) {
          let findDelivery = `<span class='badge bg-cyan-lt'><i class='bi bi-box-seam'></i> ${row.Delivery}</span>`;
          if (row.Delivery === "Pick Up") {
            findDelivery = `<span class='badge bg-pink-lt'><i class='bi bi-truck-front'></i> ${row.Delivery}</span>`;
          }
          return `<div class="text-center">${findDelivery}</div>`;
        },
      },
      {
        data: null,
        orderable: false,
        render: function (data, type, row) {
          let icon;
          switch (row.Status) {
            case "Draft":
              icon = `<i class="bi bi-stopwatch"></i>`;
              break;
            case "New Order":
              icon = `<i class="bi bi-clipboard-check"></i>`;
              break;
            case "In Production":
              icon = `<i class="bi bi-hourglass-split"></i>`;
              break;
            case "On Hold":
              icon = `<i class="bi bi-pause-circle"></i>`;
              break;
            case "Canceled":
              icon = `<i class="bi bi-x-circle"></i>`;
              break;
            case "Completed":
              icon = `<i class="bi bi-check-circle"></i>`;
              break;
          }

          return `<span class="badge bg-blue text-blue-fg">${icon} ${row.Status}</span></div>`;
        },
      },
      {
        data: null,
        orderable: false,
        render: function (data, type, row) {
          // --------------------|| Visible Button ||--------------------#
          let displayDelete = "d-none";
          let displayChangeStatus = "d-none";
          let displayDownloadCSV = "d-none";
          let displayRestore = "d-none";

          // DISPLAY BUTTON DELETE
          if (row.Status === "Draft") {
            displayDelete = "";
            if (roleName === "PPIC & DE" && userId !== row.UserId) {
              displayDelete = "d-none";
            }
          }
          // if (roleName === "Administrator" && row.Status !== "Canceled") {
          if (roleName === "Administrator") displayDelete = "";

          if (roleName === "Manager" || roleName === "Account") {
            displayDelete = "d-none";
          }

          if (row.Active === "False" || row.Active === "0")
            displayDelete = "d-none";

          // DISPLAY BUTTON CHANGE STATUS
          if (roleName === "Administrator" || roleName === "PPIC & DE") {
            displayChangeStatus = "";
          }
          if (row.Status === "Completed" || row.Status === "Canceled") {
            displayChangeStatus = "d-none";
            if (roleName === "Administrator") {
              displayChangeStatus = "";
            }
          }
          if (row.Active === "False" || row.Active === "0")
            displayChangeStatus = "d-none";

          // DISPLAY BUTTON DOWNLOAD CSV
          if (roleName === "Administrator" || roleName === "PPIC & DE") {
            if (row.Status !== "Draft" && row.Status !== "Canceled") {
              displayDownloadCSV = "";
            }
          }

          // DISPLAY BUTTON RESTORE
          if (
            roleName === "Administrator" &&
            (row.Active === "False" || row.Active === "0")
          ) {
            displayRestore = "";
          }
          return `
            <div class="dropdown text-center">
              <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
              </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btnDetailOrder" data-id="${row.Id}">
                    <i class="bi bi-info-circle me-2 opacity-50"></i>Detail
                  </a>
                </li>
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btnDateInfo" data-id="${row.Id}">
                    <i class="bi bi-calendar4 me-2 opacity-50"></i>Date Information
                  </a>
                </li>
                <li class="${displayDelete}">
                  <a class="dropdown-item text-danger" href="javascript:void(0)" id="btnDeleteOrder" data-id="${row.Id}" data-name="${row.StoreName}" data-order="${row.OrderNo}" data-ref="${row.OrderCust}" data-del="${row.Delivery}">
                    <i class="bi bi-trash3 me-2"></i>Delete
                  </a>
                </li>
                <li class="${displayChangeStatus}">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnChangeStatus" data-id="${row.Id}">
                    <i class="bi bi-clipboard-check me-2 opacity-50"></i> Change Status
                  </a>
                </li>
                <li class="${displayDownloadCSV}">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnDownloadCsv" data-id="${row.Id}">
                    <i class="bi bi-file-earmark-arrow-down me-2 opacity-50"></i> Download CSV Order 
                  </a>
                </li>
                <li class="${displayRestore}">
                  <a class="dropdown-item" href="javascript:void(0)" id="btnRestoreOrder" data-id="${row.Id}" data-name="${row.StoreName}" data-order="${row.OrderNo}" data-ref="${row.OrderCust}" data-del="${row.Delivery}">
                    <i class="bi bi-arrow-repeat me-2 opacity-50"></i>Restore 
                  </a>
                </li>
              </ul>
            </div>`;
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

// HANDLER CELECT STSTUS
const handlerSelStatus = async (params, statusNow) => {
  if (!params) return;

  const sel = document.querySelector(params);
  if (!sel) return;

  sel.innerHTML = ""; // Reset options

  let data = [];

  // === cardOrder => status ===
  if (params === "#cardOrder #status") {
    if (roleName === "PPIC & DE") {
      data = [
        { value: "all", text: "All" },
        { value: "New Order", text: "New Order" },
        { value: "In Production", text: "In Production" },
        { value: "On Hold", text: "On Hold" },
        { value: "Completed", text: "Completed" },
        { value: "Canceled", text: "Canceled" },
      ];
    } else {
      data = [
        { value: "all", text: "All" },
        { value: "Draft", text: "Draft" },
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
    const uiActive = document.querySelector("#cardOrder #active")?.value || "";
    const uiStoreType =
      document.querySelector("#cardOrder #storetype")?.value || "";

    const statusToUse = getState("filter_orders_status") || uiStatus;
    const activeToUse = getState("filter_orders_active") || uiActive;
    const storeTypeToUse = getState("filter_orders_storetype") || uiStoreType;

    // Update filter UI
    setFilterValues(statusToUse, activeToUse, storeTypeToUse);

    // Jika bindOrders adalah fungsi async, kita tunggu dulu
    await bindOrders(statusToUse, activeToUse, storeTypeToUse);
  }
};

// HANDLER CREATE NEW ORDER
const handlerCreateNewOrder = async () => {
  try {
    const response = await fetch(`${uriMethod}/SetHeaderAction`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ action: "AddHeader" }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    // Jika sukses, langsung arahkan ke halaman order header
    window.location.href = "/order/header";
  } catch (error) {
    isError("Gagal menyetel session: " + error.message);
  }
};

// HANDLER OPEN DETAIL ORDER
const handlerOpenDetailOrder = async (headerid) => {
  try {
    const response = await fetch(`${uriMethod}/SetSessionOpenOrderDetail`, {
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
    const response = await fetch(`${uriMethod}/BindOrderId`, {
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
      await isError(msg);
      return;
    }

    // Jalankan tiap item secara berurutan
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
    const response = await fetch(`${uriMethod}/BindOrderId`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerDateInfo"
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
      return;
    }

    for (const item of data) {
      await setValueModalDateInfo(item);
      await handlerShowBSModal("modalDateInfo");
    }
  } catch (error) {
    const msg =
      roleName === "Administrator"
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

      if (roleName === "Administrator") {
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
const handlerSwitch = async (id, name, order, ref, del, act) => {
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
    const response = await fetch(`${uriMethod}/SwitchOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, action: act }),
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
      const response = await fetch(`${uriMethod}/DownloadCSVOrder`, {
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
        roleName === "Administrator"
          ? error.message
          : "Something went wrong while downloading the CSV.";
      await isError(msg);
    }
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
};

const setState = (name, value) => {
  if (!name && !value) return console.warn("setState: name and value required");
  localStorage.setItem(name, value);
};

const getState = (name) => {
  if (!name) return console.warn("getState: name required");
  return localStorage.getItem(name);
};

const setFilterValues = (status, active, storeType) => {
  document.querySelector("#cardOrder #status").value = status;
  document.querySelector("#cardOrder #active").value = active;
  document.querySelector("#cardOrder #storetype").value = storeType;
};
