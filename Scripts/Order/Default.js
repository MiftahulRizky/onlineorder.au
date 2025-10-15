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
$("#cardOrder #status").on("change", function () {
  const status = $(this).val();
  const active = $("#cardOrder #active").val();
  const storetype = $("#cardOrder #storetype").val();

  // Simpan ke localStorage
  setState("filter_status", status);
  setState("filter_active", active);
  setState("filter_storetype", storetype);

  bindOrders(status, active, storetype);
});

// BUTTON CREATE ORDER
$("#cardOrder #btnCreateNewOrder").on("click", function () {
  handlerCreateNewOrder();
});

// CHANGE FILTER ACTIVE
$("#cardOrder #active").on("change", function () {
  const status = $("#cardOrder #status").val();
  const active = $(this).val();
  const storetype = $("#cardOrder #storetype").val();

  // Simpan ke localStorage
  setState("filter_status", status);
  setState("filter_active", active);
  setState("filter_storetype", storetype);

  bindOrders(status, active, storetype);
});

// CHANGE FILTER STORE TYPE
$("#cardOrder #storetype").on("change", function () {
  const status = $("#cardOrder #status").val();
  const active = $("#cardOrder #active").val();
  const storetype = $(this).val();

  // Simpan ke localStorage
  setState("filter_status", status);
  setState("filter_active", active);
  setState("filter_storetype", storetype);

  bindOrders(status, active, storetype);
});

// BUTTON DETAIL ORDER
$("#tableAjax").on("click", "#btnDetailOrder", function () {
  const id = $(this).data("id");
  handlerOpenDetailOrder(id);
});

// BUTTON DATE INFO
$("#tableAjax").on("click", "#btnDateInfo", function () {
  const id = $(this).data("id");
  handlerDateInfo(id);
});

// BUTTON CHANGE STATUS
$("#tableAjax").on("click", "#btnChangeStatus", function () {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  );
  const id = $(this).data("id");
  handlerChangeStatus(id);
});

// BUTTON DELLETE ORDER
$("#tableAjax").on("click", "#btnDeleteOrder", function () {
  const id = $(this).data("id");
  const name = $(this).data("name");
  const order = $(this).data("order");
  const ref = $(this).data("ref");
  const del = $(this).data("del");
  handlerSwitch(id, name, order, ref, del, "delete");
});

// BUTTON RESTORE ORDER
$("#tableAjax").on("click", "#btnRestoreOrder", function () {
  const id = $(this).data("id");
  const name = $(this).data("name");
  const order = $(this).data("order");
  const ref = $(this).data("ref");
  const del = $(this).data("del");
  handlerSwitch(id, name, order, ref, del, "restore");
});

// BUTTON DOWNLOAD CSV
$("#tableAjax").on("click", "#btnDownloadCsv", function () {
  const id = $(this).data("id");
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
});
// --------------------------------------------||modalChangeStatus Event ||-------------------------------------------
// CHANGE STATUS
$("#modalChangeStatus #status").on("change", function () {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
  );
  const status = $(this).val();
  hanlderDisplayElementModalChangeStatus(status);
});
// CHANGE SUBMITTED DATE
$("#modalChangeStatus #submitteddate").on("change", function () {
  $(this).removeClass("is-invalid");
});
// CHANGE CANCELED DATE
$("#modalChangeStatus #canceleddate").on("change", function () {
  $(this).removeClass("is-invalid");
});
// INPUT DESCRIPTION
$("#modalChangeStatus #description").on("change", function () {
  $(this).removeClass("is-invalid");
});

// TOOLTIP DESCRIPTION CLICK
$("#modalChangeStatus #tooltipDescription").on("click", function () {
  const status = $("#modalChangeStatus #status").val();
  handlerTooltip("modalChangeStatus", status);
});
// BUTTON SUBMIT CHANGE STATUS
$("#modalChangeStatus #submitChangeStatus").on("click", submitChangeStatus);
// ==================================================FUNCTIONS===============================================
// --------------------------------------------||Submit Function ||-------------------------------------------
function submitChangeStatus() {
  handlerResetFormError(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select"
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
  const paramsChangeStatus = { username: userName };

  fields.forEach((field) => {
    paramsChangeStatus[field] = document.querySelector(
      "#modalChangeStatus #" + field
    ).value;
  });

  $.ajax({
    type: "post",
    url: uriMethod + "/UpdateStatusOrder",
    data: JSON.stringify({ data: paramsChangeStatus }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    beforeSend: function () {
      $("#modalChangeStatus #submitChangeStatus").attr("disable", "disable");
      $("#modalChangeStatus #submitChangeStatus").html(
        '<i class="fa fa-spin fa-spinner"</i>'
      );
      swalLoadingShow("Please wait while we update the status.");
    },
    complete: function () {
      $("#modalChangeStatus #submitChangeStatus").removeAttr("disable");
      $("#modalChangeStatus #submitChangeStatus").html(
        `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit `
      );
    },
    success: function (response) {
      const result = response.d || response;
      // Swal.close();
      if (result.error) {
        isError(result.error.message.toUpperCase()).then(() => {
          const fieldElement = document.querySelector(result.error.field);
          if (fieldElement) {
            fieldElement.focus();
            fieldElement.classList.add("is-invalid");
          }
        });
      } else {
        isSuccess(result.success).then(() => {
          handlerHideBSModal("modalChangeStatus");
        });
      }
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var msg =
        roleName === "Administrator"
          ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
          : "Something went wrong, please try again!";
      isError(msg);
    },
  });
  return false;
}
// --------------------------------------------||Binding Function ||-------------------------------------------
// BIND ORDERS
function bindOrders(status, active, storetype) {
  if ($.fn.DataTable.isDataTable("#tableAjax")) {
    $("#tableAjax").DataTable().destroy(); // Hancurkan instance DataTables yang ada
  }

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
    serverSide: true, // <<< INI KUNCI PENTINGNYA
    order: [], // Tetap bisa set default order di sini
    // -----------------|| Save State Default DataTables ||-----------------//
    stateSave: true,
    stateDuration: -1,
    pageLength: 10,
    // -----------------|| /Save State ||-----------------//
    language: {
      search: "",
      lengthMenu: "_MENU_", // hanya dropdown, tanpa "Show entries"
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true, // Ini mengaktifkan global search box
    bDestroy: true,
    initComplete: function () {
      // 1. Styling kolom search
      const input = $("#tableAjax_filter input");
      input
        .addClass("form-control form-control-sm") // ganti lg -> sm
        .attr("placeholder", "🔍 Type here to search...")
        .css({
          width: "250px",
          height: "40px",
          fontSize: "15px",
          display: "inline-block",
        });

      // 2. Styling dropdown "Show entries"
      const lengthSelect = $("#tableAjax_length select");
      lengthSelect.addClass("form-select form-select-sm").css({
        width: "65px",
        fontSize: "15px",
        height: "40px",
      }); // ganti lg -> sm
    },
    ajax: {
      url: uriMethod + "/BindOrders",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: function (d) {
        // 'd' adalah objek parameter DataTables
        // Gabungkan parameter Anda dengan parameter DataTables
        return JSON.stringify({
          params: {
            ...paramData, // Parameter filter Anda
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
        // Pastikan struktur respons cocok dengan DataTableResponse Anda
        // json.d.data = array data
        // json.d.recordsTotal = total records tanpa filter
        // json.d.recordsFiltered = total records setelah filter
        json.recordsTotal = json.d.recordsTotal;
        json.recordsFiltered = json.d.recordsFiltered;
        return json.d.data;
      },
      complete: function () {
        loaderFadeOut(); // Loader disembunyikan setelah data Ajax berhasil
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg); // Asumsikan isError adalah fungsi penanganan error Anda
      },
    },
    columns: [
      {
        data: "No",
        orderable: false, // Kolom nomor urut biasanya tidak bisa diurutkan
        render: function (data, type, row, meta) {
          // Jika Anda ingin nomor urut dihitung di client
          // return meta.row + meta.settings._iDisplayStart + 1;
          // Jika Anda mengambil 'No' dari server:
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
      { data: "OrderCust" }, // As Reference
      {
        data: null,
        orderable: false, // Kolom aksi tidak bisa diurutkan
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
        orderable: false, // Kolom aksi tidak bisa diurutkan
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
        orderable: false, // Kolom aksi tidak bisa diurutkan
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
          if (roleName === "Administrator") {
            displayDelete = "";
          }

          if (roleName === "Manager" || roleName === "Account") {
            displayDelete = "d-none";
          }

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
}
// --------------------------------------------||Handler Function ||-------------------------------------------
// HANDLER HIDE BOOTSTRAP MODAL
function handlerHideBSModal(id) {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    // Jika modal belum pernah di-show dan belum punya instance, buat dan langsung hide
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
}

// HANDLER SHOW BOOTSTRAP MODAL
function handlerShowBSModal(params) {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
}

// HANDLER CELECT STSTUS
function handlerSelStatus(params, statusNow) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    if (!params) return resolve();

    let data = [];

    // for cardOrder => status
    if (params === "#cardOrder #status") {
      if (roleName === "PPIC & DE") {
        data = [
          { value: "all", text: "All" },
          // { value: "Draft", text: "Draft" },
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

    // for cardChangeStatus => status
    if (params === "#modalChangeStatus #status" && statusNow) {
      switch (statusNow) {
        case "Draft":
          data = [
            { value: "New Order", text: "New Order" },
            { value: "Canceled", text: "Canceled" },
          ];

          if (roleName !== "Administrator") {
            // Tambahkan Draft di awal (unshift) atau akhir (push)
            data.unshift({ value: "Draft", text: "Draft" }); // Menambahkan di awal
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
        // Tambahkan Draft di awal (unshift) atau akhir (push)
        data.unshift({ value: "Draft", text: "Draft" }); // Menambahkan di awal
      }
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      sel.appendChild(option);
    });

    // for cardOrder => status
    if (params === "#cardOrder #status") {
      // ambil value filter default ketika load
      const uiStatus = sel.options[sel.selectedIndex].value;
      const uiActive = document.querySelector("#cardOrder #active").value;
      const uiStoreType = document.querySelector("#cardOrder #storetype").value;

      // jika ada state makan gunakan state
      let statusToUse = getState("filter_status") || uiStatus;
      let activeToUse = getState("filter_active") || uiActive;
      let storeTypeToUse = getState("filter_storetype") || uiStoreType;

      // set value filter
      setFilterValues(statusToUse, activeToUse, storeTypeToUse);

      // bind orders
      bindOrders(statusToUse, activeToUse, storeTypeToUse);
    }

    resolve();
  });
}

// HANDLER CREATE NEW ORDER
function handlerCreateNewOrder() {
  $.ajax({
    type: "POST",
    url: uriMethod + "/SetHeaderAction",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({ action: "AddHeader" }),
    success: function () {
      window.location.href = "/order/header";
    },
    error: function (xhr, status, error) {
      isError("Gagal menyetel session: " + error);
    },
  });
}

// HANDLER OPEN DETAIL ORDER
function handlerOpenDetailOrder(headerid) {
  $.ajax({
    type: "POST",
    url: uriMethod + "/SetSessionOpenOrderDetail",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({ headerid: headerid }),
    success: function () {
      window.location.href = "/order/detail";
    },
    error: function (xhr, status, error) {
      isError("Gagal menyetel session: " + error);
    },
  });
}

// HANDLER CHANGE STATUS
function handlerChangeStatus(headerid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();
    // console.log("bindItemOrder", headerid);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindOrderId",
      data: JSON.stringify({
        headerid: headerid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerChangeStatus"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return Promise.resolve()
            .then(() =>
              handlerSelStatus("#modalChangeStatus #status", item.Status)
            )
            .then(() => setValueModalChangeStatus(item))
            .then(() => hanlderDisplayElementModalChangeStatus(item.Status))
            .then(() => {
              return Promise.all([handlerShowBSModal("modalChangeStatus")])
                .then(resolve)
                .catch(reject);
            });
        });

        Promise.all(promises)
          .then(() => resolve())
          .catch((error) => reject(error));
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

function setValueModalChangeStatus(itemData) {
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
}

function handlerResetFormError(params) {
  // params : "#modalSaveData .form-control, #modalSaveData .form-select"
  document.querySelectorAll(params).forEach((element) => {
    element.classList.remove("is-invalid");
  });
}

// HANDLER DATE INFORMATION
function handlerDateInfo(headerid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();
    // console.log("bindItemOrder", headerid);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindOrderId",
      data: JSON.stringify({
        headerid: headerid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerChangeStatus"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return Promise.resolve()
            .then(() => setValueModalDateInfo(item))
            .then(() => {
              return Promise.all([handlerShowBSModal("modalDateInfo")])
                .then(resolve)
                .catch(reject);
            });
        });

        Promise.all(promises)
          .then(() => resolve())
          .catch((error) => reject(error));
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

function setValueModalDateInfo(itemData) {
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
}

function parseCustomDate(value) {
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
}

// HANDLER DELETE & RESTORE ORDER
function handlerSwitch(id, name, order, ref, del, act) {
  let title = act === "delete" ? "delete" : "restore";
  let textButton = act === "delete" ? "Yes, delete it!" : "Yes, restore it!";
  let icon = act === "delete" ? "warning" : "question";
  Swal.fire({
    title: name,
    html:
      "Sure to " +
      title +
      " this data? <br/><br/> <b>Order No :</b>" +
      order +
      " <b>Ref :</b>" +
      ref +
      " <b>Del :</b>" +
      del,
    icon: icon,
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: textButton,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        type: "post",
        url: uriMethod + "/SwitchOrder",
        data: JSON.stringify({
          id: id,
          action: act,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase());
          } else {
            isSuccess(result.success);
          }
        },
        error: function (xhr, ajaxOptions, thrownError) {
          var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
          isError(msg);
        },
      });
    }
  });
}

// HANDLER DOWLOAD CSV ORDER
function handlerDownloadCSV(headerId) {
  return new Promise((resolve, reject) => {
    let timerInterval;
    Swal.fire({
      title: "Downloading...",
      html: "Is downloading, i will close in <b></b> milliseconds.",
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
    }).then((result) => {
      /* Read more about handling dismissals below */
      if (result.dismiss === Swal.DismissReason.timer) {
        $.ajax({
          type: "POST",
          url: uriMethod + "/DownloadCSVOrder",
          data: JSON.stringify({
            HeaderId: headerId, // harus huruf besar H agar cocok dengan server-side
          }),
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (response) {
            // Membuat Blob dari string CSV
            const csvString = response.d;
            const blob = new Blob([csvString], {
              type: "text/csv;charset=utf-8;",
            });

            // Buat URL blob
            const url = URL.createObjectURL(blob);

            // Buat elemen link sementara
            const link = document.createElement("a");
            link.setAttribute("href", url);
            link.setAttribute("download", "-SPOD-ID-" + headerId + ".csv");
            document.body.appendChild(link);
            link.click();

            // Bersihkan
            document.body.removeChild(link);
            URL.revokeObjectURL(url);

            resolve();
          },
          error: function (xhr, status, error, thrownError) {
            reject(
              new Error(
                xhr.responseJSON?.error
                  ? xhr.responseJSON.error
                  : xhr.status + "\n" + xhr.responseText + "\n" + thrownError
              )
            );
          },
        });
      }
    });
  });
}

// HANDLER DISPLAY ELEMENT MODAL CHANGE STATUS
function hanlderDisplayElementModalChangeStatus(status) {
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
}

// HANDLER TOOLTIP
function handlerTooltip(modalName, params) {
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
}
// --------------------------------------------||Other Function ||-------------------------------------------
// CHECK SESSION
function checkSession() {
  setSessionAlive();

  handlerSelStatus("#cardOrder #status", null);
}

function setState(name, value) {
  if (!name && !value) return console.warn("setState: name and value required");
  localStorage.setItem(name, value);
}

function getState(name) {
  if (!name) return console.warn("getState: name required");
  return localStorage.getItem(name);
}

function setFilterValues(status, active, storeType) {
  document.querySelector("#cardOrder #status").value = status;
  document.querySelector("#cardOrder #active").value = active;
  document.querySelector("#cardOrder #storetype").value = storeType;
}
