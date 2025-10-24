document.addEventListener("DOMContentLoaded", () => {
  console.log("Default.js loaded successfully");

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
  checkSessionFabric();
});
// ==================================================|| EVENTS ||==================================================
// --------------------------------------------------|| card-table ||-----------------------------------------
// change designs
document
  .querySelector("#card-table #designid")
  .addEventListener("change", (e) => {
    const designid = e.target.value;
    const active = document.querySelector("#card-table #active").value;
    setState("filter_fabric_designid", designid);
    bindFabrics(designid, active, "#card-table #data-table");
  });
// change active
document
  .querySelector("#card-table #active")
  .addEventListener("change", (e) => {
    const designid = document.querySelector("#card-table #designid").value;
    const active = e.target.value;
    setState("filter_fabric_active", designid);
    bindFabrics(designid, active, "#card-table #data-table");
  });

// button add
document.querySelector("#card-table #btn-add").addEventListener("click", () => {
  document
    .querySelectorAll("#modalSubmit .form-control, #modalSubmit .form-select")
    .forEach((el) => {
      el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
      el.value = "";
      el.classList.remove("is-invalid");
    });

  document.querySelector("#modalSubmit #action").value = "create";
  document.querySelector("#modalSubmit #activate").value = 1;

  document.querySelector("#modalSubmit #modalSubmitLabel").innerHTML =
    "Create Fabric";
  handlerSelDesigns("#modalSubmit #designtype");
});
// --------------------------------------------------||#card-table #data-table ||-----------------------------------
// edit / detail
document
  .querySelector("#card-table #data-table")
  .addEventListener("click", (e) => {
    if (e.target.matches("#btn-edit")) {
      document
        .querySelectorAll(
          "#modalSubmit .form-control, #modalSubmit .form-select"
        )
        .forEach((el) => {
          el.classList.remove("is-invalid");
        });

      document.querySelector("#modalSubmit #action").value = "update";
      swalLoadingShow("Please wait while we load the data.");

      const fabricid = e.target.getAttribute("data-id");
      handlerEdit(fabricid);
    }
  });

// delete
document
  .querySelector("#card-table #data-table")
  .addEventListener("click", (e) => {
    if (e.target.matches("#btn-delete")) {
      const surchargeId = e.target.getAttribute("data-id");
      const surchargeName = e.target.getAttribute("data-name");
      Swal.fire({
        customClass: {
          popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
        },
        title: surchargeName,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!",
      }).then((result) => {
        if (result.isConfirmed) {
          handlerDelete(surchargeId);
        }
      });
    }
  });

// switch
document
  .querySelector("#card-table #data-table")
  .addEventListener("click", (e) => {
    if (e.target.matches("#btn-switch")) {
      const surchargeId = e.target.getAttribute("data-id");
      const active = e.target.getAttribute("data-active");
      const surchargeName = e.target.getAttribute("data-name");
      Swal.fire({
        customClass: {
          popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
        },
        title: surchargeName,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, switch it!",
      }).then((result) => {
        if (result.isConfirmed) {
          handlerSwitch(surchargeId, active);
        }
      });
    }
  });

// --------------------------------------------------||modalSubmit ||-------------------------------------------
// input or chenge  remove class is-invalid
document
  .querySelectorAll("#modalSubmit .form-control, #modalSubmit .form-select")
  .forEach((el) => {
    el.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
    el.addEventListener("change", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

// submit
document
  .querySelector("#modalSubmit #btn-submit")
  .addEventListener("click", (e) => {
    e.preventDefault();

    // reset error state
    document
      .querySelectorAll("#modalSubmit .form-control, #modalSubmit .form-select")
      .forEach((el) => {
        el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        el.classList.remove("is-invalid");
      });

    handlerSubmit(e.target.form, e.target, e.target.innerHTML);
  });
// ==================================================|| FUNCTIONS ||==================================================
// --------------------------------------------------|| Handler Functions ||-----------------------------------------
const handlerShowBSModal = (id) => {
  const modalEl = document.getElementById(id);

  // Gunakan instance yang sudah ada, jangan buat baru tiap kali
  let modalInstance = bootstrap.Modal.getInstance(modalEl);
  if (!modalInstance) {
    modalInstance = new bootstrap.Modal(modalEl, { keyboard: false });
  }

  modalInstance.show();
};

const handlerHideBSModal = (id) => {
  const modalEl = document.getElementById(id);
  const modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  }

  // Tunggu sedikit agar event internal Bootstrap selesai (fade out backdrop)
  setTimeout(() => {
    // Bersihkan sisa backdrop dan atribut modal
    document.querySelectorAll(".modal-backdrop").forEach((el) => el.remove());
    modalEl.classList.remove("show");
    modalEl.style.display = "none";
    modalEl.removeAttribute("aria-modal");
    modalEl.setAttribute("aria-hidden", "true");

    // Pastikan body kembali normal
    document.body.classList.remove("modal-open");
    document.body.style.overflow = "auto";
    document.body.style.removeProperty("padding-right");

    // Pastikan tidak ada focus di dalam modal
    if (document.activeElement && modalEl.contains(document.activeElement)) {
      document.activeElement.blur();
    }
  }, 250); // waktu sesuai animasi fade Bootstrap
};

const handlerSelDesigns = async (params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  try {
    const response = await fetch(uriMethod + "/BindDesignType", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      // body: JSON.stringify({ designId: designId }), // aktifkan jika butuh kirim data
    });

    if (!response.ok) {
      const msg =
        roleName === "Administrator"
          ? `${response.status}\n${response.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerSelDesigns"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    if (Array.isArray(data)) {
      sel.innerHTML = ""; // reset lagi untuk jaga-jaga

      // bukan untuk card table
      if (params !== "#card-table #designid" && data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        sel.add(defaultOption);
      }

      data.forEach((item) => {
        const option = document.createElement("option");

        if (String(item.active).toLowerCase() === "false") {
          option.classList.add("text-danger");
        }
        option.value = item.value;
        option.text = item.text.toUpperCase();
        // option.setAttribute("data-name", item.text);
        sel.add(option);
      });

      if (params === "#card-table #designid") {
        const designid = sel.options[sel.selectedIndex].value;
        const active = document.querySelector("#card-table #active").value;
        const currentDesigns = getState("filter_fabric_designid") || designid;
        sel.value = currentDesigns;
        bindFabrics(currentDesigns, active, "#card-table #data-table");
      } else {
        handlerShowBSModal("modalSubmit");
      }
    }
  } catch (error) {
    const msg =
      roleName === "Administrator"
        ? error.message || error
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg); // langsung tampilkan error, tidak perlu reject
  }
};

const handlerEdit = async (id) => {
  try {
    if (!id) return;

    const res = await fetch(`${uriMethod}/Find`, {
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
          ? "No data returned from server : handlerEdit"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    document.querySelector("#modalSubmit #modalSubmitLabel").innerHTML =
      "Edit Fabric";
    for (const item of data) {
      await handlerSelDesigns("#modalSubmit #designtype");
      await handlerSetElementValues(item);
      await swal.close();
      await handlerShowBSModal("modalSubmit");
    }

    return true; // ✅ success
  } catch (error) {
    console.error("handlerEdit error:", error);
    throw error;
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    id: "Id",
    designtype: "DesignId",
    name: "Name",
    type: "Type",
    colour: "Colour",
    width: "Width",
    group: "Group",
    activate: "Active",
  };

  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[key];
    if (id === "activate" && value === "True") value = 1;
    if (id === "activate" && value === "False") value = 0;

    el.value = value ?? "";
  });
};

const handlerSubmit = async (formEl, button, htmlButton) => {
  try {
    const formData = new FormData(formEl);

    // ubah FormData menjadi object
    let formObject = Object.fromEntries(formData.entries());

    // filter field ASP.NET yang tidak dibutuhkan
    const excludeKeys = [
      "__EVENTTARGET",
      "__EVENTARGUMENT",
      "__VIEWSTATE",
      "__VIEWSTATEGENERATOR",
      "__SCROLLPOSITIONX",
      "__SCROLLPOSITIONY",
      "__EVENTVALIDATION",
      "ctl00$txtSearchMaster",
      "designid",
      "data-table_length",
      "active",
    ];

    formObject = Object.fromEntries(
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key))
    );

    // gabungkan
    const finalData = formObject;

    // debug konsisten
    // console.group("Submit Debug");
    // console.log("FormData snapshot:", [...formData.entries()]);
    // console.table(formObject);
    // console.table(extraData);
    // return console.table(finalData);
    // console.groupEnd();

    // before send
    button.setAttribute("disabled", "disabled");
    button.innerHTML = '<i class="ti ti-loader fs-2 me-1"></i> Processing...';

    // fetch POST
    const response = await fetch(uriMethod + "/Submit", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: finalData }),
    });

    // restore button
    button.removeAttribute("disabled");
    button.innerHTML = htmlButton;

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        roleName === "Administrator"
          ? `${response.status}\n${errorText}`
          : "Something went wrong, please try again!"
      );
    }

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isError(dataResult.error.message.toUpperCase());
      const field = document.getElementById(dataResult.error.field);
      if (field) {
        field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      await isSuccess(dataResult.success);
      handlerHideBSModal("modalSubmit");
      tableData.ajax.reload();
    }
  } catch (err) {
    await isError(err.message);
  }
};

const handlerSwitch = async (id, active) => {
  try {
    if (!id) return;

    const res = await fetch(`${uriMethod}/Switch`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, active }),
    });

    if (!res.ok) {
      const msg =
        roleName === "Administrator"
          ? `${res.status} - ${res.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    const response = await res.json();
    const data = response.d || response;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerSwitch"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    if (data.error) {
      await isError(data.error.message.toUpperCase());
    } else {
      await isSuccess(data.success);
      tableData.ajax.reload();
    }
  } catch (error) {
    console.error("handlerSwitch error:", error);
    throw error;
  }
};

const handlerDelete = async (id) => {
  try {
    if (!id) return;

    const res = await fetch(`${uriMethod}/Delete`, {
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
    const data = response.d || response;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : handlerDelete"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    if (data.error) {
      await isError(data.error.message.toUpperCase());
    } else {
      await isSuccess(data.success);
      tableData.ajax.reload();
    }
  } catch (error) {
    console.error("handlerDelete error:", error);
    throw error;
  }
};
// --------------------------------------------------|| Binding Functions ||-----------------------------------------
let tableData;
const bindFabrics = (designid, active, params) => {
  if (tableData) {
    tableData.destroy();
  }

  const paramData = {
    designid: designid,
    active: active,
  };

  tableData = $(params).DataTable({
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
      url: uriMethod + "/Bind",
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
      { data: "Id", width: "5%" },
      { data: "DesignName", width: "15%" },
      {
        data: "FabricName",
        width: "45%",
        orderable: false,
        render: function (data, type, row) {
          let icn = "ti-circle-check";
          let color = "text-success";
          if (row.Active == "False") {
            icn = "ti-circle-x";
            color = "text-danger";
          }
          return `<i class="ti fs-3 me-1 ${icn} ${color}"></i>${data}`;
        },
      },
      { data: "Width", width: "5%" },
      { data: "Group", width: "20%" },
      {
        data: null,
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          return dropdownActionButton(data, type, row);
        },
      },
    ],
  });
};
// --------------------------------------------------|| Other Functions ||-------------------------------------------

const checkSessionFabric = () => {
  setSessionAlive();
  handlerSelDesigns("#card-table #designid");
};

const setState = (name, value) => {
  if (!name && !value) return console.warn("setState: name and value required");
  localStorage.setItem(name, value);
};

const getState = (name) => {
  if (!name) return console.warn("getState: name required");
  return localStorage.getItem(name);
};
// --------------------------------------------------||Additional data table styling ||-------------------------------------------
const dropdownActionButton = (data, type, row) => {
  return `<div class="dropdown text-center">
            <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
            </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <span class="dropdown-header">Basic Action</span>
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btn-edit" data-id="${row.Id}">
                    <i class="ti ti-edit me-1 opacity-50 fs-2" ></i>Edit / Detail
                  </a>
                </li>
                <li>
                  <a class="dropdown-item text-danger" href="javascript:void(0)" id="btn-delete" data-id="${row.Id}" data-name="${row.FabricName}">
                    <i class="ti ti-trash-x me-1 opacity-50 fs-2"></i>Delete
                  </a>
                </li>

                <div class="dropdown-divider"></div>
                <span class="dropdown-header">configuration</span>
                <li>
                  <a class="dropdown-item " href="javascript:void(0)" id="btn-switch" data-id="${row.Id}" data-name="${row.FabricName}" data-active="${row.Active}">
                    <i class="ti ti-switch-horizontal me-1 opacity-50 fs-2"></i>Switch Activation
                  </a>
                </li>
                
              </ul>
          </div>`;
};

const stylingColumnSearchAndPaging = (params) => {
  // 1. Styling kolom search
  const input = $(params + "_filter input");
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
  const lengthSelect = $(params + "_length select");
  lengthSelect.addClass("form-select form-select-sm").css({
    width: "65px",
    fontSize: "15px",
    height: "40px",
  });
};
