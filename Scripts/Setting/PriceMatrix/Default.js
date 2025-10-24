$(document).ready(function () {
  if (roleName == "Administrator") {
    console.log("Default.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("userId: " + userId);
    console.log("uriMethod: " + uriMethod);
  }
  checkSession();
});
// ==================================================EVENTS==================================================
// #-------------------------|| Button Event ||-------------------------#
// BUTTON FILTER
$("#btnFilter").on("click", () => handlerCanvas("show"));

// BUTTON INSERT
$("#btnInsert").on("click", function () {
  // reset
  $("#modalSaveData .form-control").val("");
  $("#modalSaveData .form-select").val("");
  // set
  $("#modalSaveData #modalSaveDataLabel").html("Add a new list");
  handlerSelDesignType("#modalSaveData #designid");
  // result
  handlerShowBSModal("modalSaveData");
});

// BUTTON IMPORT
// $("#btnImport").on("click", function () {
//   // reset
//   $("#modalImport .form-control").val("");
//   $("#modalImport .form-select").val("");
//   // set
//   $("#modalImport #modalImportLabel").html("Import a new list");
//   handlerSelDesignType("#modalImport #designid");
//   // result
//   handlerShowBSModal("modalImport");
// });

// BUTTON EDIT
$("#tableAjax").on("click", "#btnEdit", function () {
  handlerEdit($(this).data("id"));
});

// BUTTON Delete
$("#tableAjax").on("click", "#btnDelete", function () {
  handlerDelete(
    $(this).data("id"),
    $(this).data("name"),
    $(this).data("type"),
    $(this).data("width"),
    $(this).data("drop"),
    $(this).data("cost")
  );
});

// #-------------------------|| Filter Event ||-------------------------#
// CHANGE DESIGN TYPE
$("#canvasFilter #designid").on("change", function () {
  handlerSelPriceGroup($(this).val(), "#canvasFilter #pricegroupid");
});

// CHANGE PRICE GROUP
$("#canvasFilter #pricegroupid").on("change", function () {
  const pricegroupid = $(this).val();
  const type = $("#canvasFilter #type").val();
  const width = $("#canvasFilter #width").val();
  const drop = $("#canvasFilter #drop").val();
  bindPriceMatrix(pricegroupid, type, width, drop);
});

// CHANGE TYPE
$("#canvasFilter #type").on("change", function () {
  const pricegroupid = $("#canvasFilter #pricegroupid").val();
  const type = $(this).val();
  const width = $("#canvasFilter #width").val();
  const drop = $("#canvasFilter #drop").val();
  bindPriceMatrix(pricegroupid, type, width, drop);
});

// BUTTON SEARCH FILTER
$("#canvasFilter #btnSearchFilter").on("click", function () {
  const pricegroupid = $("#canvasFilter #pricegroupid").val();
  const type = $("#canvasFilter #type").val();
  const width = $("#canvasFilter #width").val();
  const drop = $("#canvasFilter #drop").val();
  bindPriceMatrix(pricegroupid, type, width, drop);
  handlerCanvas("hide");
});

// BUTTON IMPORT
$("#canvasFilter #btnImport").on("click", function () {
  // reset
  $("#modalImport .form-control").val("");
  $("#modalImport .form-select").val("");
  // set
  $("#modalImport #modalImportLabel").html("Import a new list");
  // result
  handlerShowBSModal("modalImport");
});

// button delete
document
  .querySelector("#canvasFilter #btnDelete")
  .addEventListener("click", () => {
    const select = document.querySelector("#canvasFilter #pricegroupid");

    const groupid = select.value;
    const htmlgroupid = select.selectedOptions[0].text;
    const type = document.querySelector("#canvasFilter #type").value;

    handlerDeleteByGroupAndType(groupid, type, htmlgroupid);
  });

// #-------------------------|| modalImport Event ||-------------------------#
$("#modalImport").on("keydown", function (e) {
  if (e.key === "Enter") {
    e.preventDefault();
  }
});
// CHANGE FILE UPLOAD
$("#modalImport #fileupload").on("change", function () {
  $(this).removeClass("is-invalid");
});
// CHANGE DESIGN TYPE
$("#modalImport #designid").on("change", function () {
  $(this).removeClass("is-invalid");
  handlerSelPriceGroup($(this).val(), "#modalImport #pricegroupid");
});
// CHANGE GROUP NAME
$("#modalImport #pricegroupid").on("change", function () {
  $(this).removeClass("is-invalid");
});
// CHANGE TYPE
$("#modalImport #type").on("change", function () {
  $(this).removeClass("is-invalid");
});

// BUTTON SUBMIT IMPORT
$("#modalImport #submitImport").on("click", submitCSV);

// #-------------------------|| modalSaveData Event ||-------------------------#
$("#modalSaveData").on("keydown", function (e) {
  if (e.key === "Enter") {
    e.preventDefault();
  }
});
// CHANGE FILE UPLOAD
$("#modalSaveData #fileupload").on("change", function () {
  $(this).removeClass("is-invalid");
});
// CHANGE DESIGN TYPE
$("#modalSaveData #designid").on("change", function () {
  $(this).removeClass("is-invalid");
  handlerSelPriceGroup($(this).val(), "#modalSaveData #pricegroupid");
});
// CHANGE GROUP NAME
$("#modalSaveData #pricegroupid").on("change", function () {
  $(this).removeClass("is-invalid");
});
// CHANGE TYPE
$("#modalSaveData #type").on("input", function () {
  $(this).removeClass("is-invalid");
});
// INPUT WIDTH
$("#modalSaveData #width").on("input", function () {
  $(this).removeClass("is-invalid");
});
// INPUT DROP
$("#modalSaveData #drop").on("input", function () {
  $(this).removeClass("is-invalid");
});
// INPUT COST
$("#modalSaveData #cost").on("input", function () {
  $(this).removeClass("is-invalid");
});

// BUTTON SUBMIT INSERT
$("#modalSaveData #submitSave").on("click", submitSave);

// ==================================================FUNCTIONS===============================================
// #-------------------------|| Submit Function ||-------------------------#
// SUBMIT SAVE
function submitSave() {
  handlerResetFormSaveDataError();

  const fields = [
    "id",
    "designid",
    "pricegroupid",
    "type",
    "width",
    "drop",
    "cost",
  ];
  const paramsSave = {};

  fields.forEach((field) => {
    paramsSave[field] = document.querySelector(
      "#modalSaveData #" + field
    ).value;
  });

  $.ajax({
    type: "post",
    url: uriMethod + "/PriceMatrixMethod.aspx/SaveData",
    data: JSON.stringify({ data: paramsSave }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    beforeSend: function () {
      $("#modalSaveData #submitSave").attr("disable", "disable");
      $("#modalSaveData #submitSave").html(
        '<i class="fa fa-spin fa-spinner"</i>'
      );
    },
    complete: function () {
      $("#modalSaveData #submitSave").removeAttr("disable");
      $("#modalSaveData #submitSave").html(
        `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit `
      );
    },
    success: function (response) {
      const result = response.d || response;
      if (result.error) {
        isError(result.error.message.toUpperCase()).then(() => {
          const el = document.getElementById(result.error.field);
          if (el) {
            // el.focus();
            el.classList.add("is-invalid");
          }
        });
      } else {
        isSuccess(result.success).then(() => {
          handlerHideBSModal("modalSaveData");
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
// SUBMIT CSV
function submitCSV() {
  uploadCSVFile(function (filename) {
    const formData = {
      filePath: filename,
      pricegroupid: document.querySelector("#canvasFilter #pricegroupid").value,
      type: document.querySelector("#canvasFilter #type").value,
    };

    $.ajax({
      type: "POST",
      url: uriMethod + "/PriceMatrixMethod.aspx/SubmitCSV",
      data: JSON.stringify({ data: formData }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: function () {
        $("#modalImport #submitImport").attr("disable", "disable");
        $("#modalImport #submitImport").html(
          '<i class="fa fa-spin fa-spinner"</i>'
        );
      },
      complete: function () {
        $("#modalImport #submitImport").removeAttr("disable");
        $("#modalImport #submitImport").html(
          "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit"
        );
      },
      success: function (response) {
        const result = response.d || response;
        if (result.error) {
          isError(result.error.message.toUpperCase()).then(() => {
            const el = document.getElementById(result.error.field);
            if (el) {
              // el.focus();
              el.classList.add("is-invalid");
            }
          });
        } else {
          isSuccess(result.success).then(() => {
            handlerHideBSModal("modalImport");
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
  });
  return false;
}
function uploadCSVFile(callback) {
  const fileInput = document.querySelector("#modalImport #fileupload");
  const fieldError = document.querySelectorAll("#modalImport .is-invalid");

  for (let i = 0; i < fieldError.length; i++) {
    fieldError[i].classList.remove("is-invalid");
  }

  if (!fileInput || !fileInput.files.length) {
    isError("PLEASE SELECT A FILE !").then(() => {
      const el = document.querySelector("#modalImport #fileupload");
      if (el) {
        // el.focus();
        el.classList.add("is-invalid");
      }
    });
  }

  const formData = new FormData();
  formData.append("file", fileInput.files[0]);

  $.ajax({
    url: uriMethod + "/UploadHandler.ashx",
    type: "POST",
    data: formData,
    contentType: false,
    processData: false,
    success: function (res) {
      if (callback) callback(res); // ← file path/nama dikirim balik
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var msg =
        roleName === "Administrator"
          ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
          : "Something went wrong, please try again!";
      isError(msg);
    },
  });
}
// #-------------------------|| Bind Function ||-------------------------#
// BIND CASSETTE EXTRA

function bindPriceMatrix(pricegroupid, type, width, drop) {
  if ($.fn.DataTable.isDataTable("#tableAjax")) {
    $("#tableAjax").DataTable().destroy(); // Hancurkan instance DataTables yang ada
  }

  const paramData = {
    pricegroupid: pricegroupid,
    type: type,
    width: width,
    drop: drop,
  };

  tableData = $("#tableAjax").DataTable({
    processing: true,
    serverSide: true, // <<< INI KUNCI PENTINGNYA
    order: [], // Tetap bisa set default order di sini
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
      url: uriMethod + "/PriceMatrixMethod.aspx/BindPriceMatrix",
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
    bPaginate: true,
    bInfo: true,
    bFilter: true, // Ini mengaktifkan global search box
    bDestroy: true,
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
      { data: "PriceGroupName" },
      { data: "Type" },
      { data: "Width" },
      { data: "Drop" },
      { data: "Cost" },
      {
        data: null,
        orderable: false, // Kolom aksi tidak bisa diurutkan
        render: function (data, type, row) {
          return `
                  <div class="dropdown text-center">
                    <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                      <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                      <li>
                        <a class="dropdown-item" href="javascript:void(0)" id="btnEdit" data-id="${row.Id}">
                          <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
                        </a>
                      </li>
                      <li>
                        <a class="dropdown-item text-danger" href="javascript:void(0)" id="btnDelete"
                            data-id="${row.Id}"
                            data-name="${row.PriceGroupName}"
                            data-type="${row.Type}"
                            data-width="${row.Width}"
                            data-drop="${row.Drop}"
                            data-cost="${row.Cost}">
                          <i class="bi bi-trash3 me-2"></i>Delete
                        </a>
                      </li>
                    </ul>
                  </div>`;
        },
      },
    ],
  });
}

function bindDesignByPriceGroupId(pricegroupid, params) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.value = ""; //reset

    $.ajax({
      type: "POST",
      url: uriMethod + "/PriceMatrixMethod.aspx/BindDesignByPriceGroupId",
      data: JSON.stringify({
        pricegroupid: pricegroupid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : bindDesignByPriceGroupId"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (sel) {
          sel.value = data.value;
          handlerSelPriceGroup(data.value, "#modalSaveData #pricegroupid");
        }
        resolve();
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

// #-------------------------|| Handler Function ||-------------------------#
// HANDLER CANVAS
function handlerCanvas(params) {
  var offcanvas = bootstrap.Offcanvas.getOrCreateInstance(
    document.getElementById("canvasFilter")
  );
  if (params === "hide") {
    offcanvas.hide();
  } else {
    offcanvas.show();
  }
}

// HANDLER SELECT DESIGN TYPE
function handlerSelDesignType(params) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    $.ajax({
      type: "POST",
      url: uriMethod + "/PriceMatrixMethod.aspx/BindDesignType",
      //   data: JSON.stringify({
      //     designId: designId,
      //   }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerSelDesignType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          // tidak untuk canvas filter
          if (params !== "#canvasFilter #designid" && data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            sel.add(defaultOption);
          }

          data.forEach(function (item) {
            const option = document.createElement("option");

            if (String(item.active).toLowerCase() === "false") {
              option.classList.add("text-danger");
            }

            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            sel.add(option);
          });

          // hanya untuk canvas filter
          if (params === "#canvasFilter #designid") {
            const designid = sel.options[sel.selectedIndex].value;
            handlerSelPriceGroup(designid, "#canvasFilter #pricegroupid");
          }
        }

        resolve();
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

// BIND PRICE GROUP NAME
function handlerSelPriceGroup(designid, params) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    $.ajax({
      type: "POST",
      url: uriMethod + "/PriceMatrixMethod.aspx/BindPriceGroup",
      data: JSON.stringify({
        designid: designid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerSelPriceGroup"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          // tidak untuk canvas filter
          if (params !== "#canvasFilter #pricegroupid" && data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            sel.add(defaultOption);
          }

          data.forEach(function (item) {
            const option = document.createElement("option");

            if (String(item.active).toLowerCase() === "false") {
              option.classList.add("text-danger");
            }

            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            sel.add(option);
          });

          // hanya untuk canvas filter
          if (params === "#canvasFilter #pricegroupid") {
            const pricegroupid = sel.options[sel.selectedIndex].value;
            const type = document.querySelector("#canvasFilter #type").value;
            const width = document.querySelector("#canvasFilter #width").value;
            const drop = document.querySelector("#canvasFilter #drop").value;

            bindPriceMatrix(pricegroupid, type, width, drop);
          }
        }

        resolve();
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

// RESET FORM IS INVALID
function handlerResetFormSaveDataError() {
  document
    .querySelectorAll(
      "#modalSaveData .form-control, #modalSaveData .form-select"
    )
    .forEach((element) => {
      element.classList.remove("is-invalid");
    });
}

// HANDLER EDIT
function handlerEdit(id) {
  return new Promise((resolve, reject) => {
    if (!id) return resolve();
    // console.log("bindItemOrder", id);

    $.ajax({
      type: "POST",
      url: uriMethod + "/PriceMatrixMethod.aspx/BindPriceMatrixId",
      data: JSON.stringify({
        id: id,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerEdit"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return Promise.resolve()
            .then(() => handlerSelDesignType("#modalSaveData #designid"))
            .then(() =>
              bindDesignByPriceGroupId(
                item.PriceGroupId,
                "#modalSaveData #designid"
              )
            )
            .then(() => setFormValues(item))
            .then(() => {
              return Promise.all([handlerShowBSModal("modalSaveData")])
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

function setFormValues(itemData) {
  const mapping = {
    id: "Id",
    pricegroupid: "PriceGroupId",
    type: "Type",
    width: "Width",
    drop: "Drop",
    cost: "Cost",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.querySelector("#modalSaveData #" + id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    if (id === "pricegroupid") {
      const val = itemData[mapping[id]];
      console.log("pricegroupid", val);

      const trySetValue = () => {
        const found = [...el.options].some((opt) => opt.value === val);
        if (found) {
          el.value = val;
        } else {
          setTimeout(trySetValue, 100);
        }
      };

      trySetValue();
      return;
    }

    let value = itemData[mapping[id]];

    // 🟡 Format nilai cost: ganti koma jadi titik
    if (id === "cost" && typeof value === "string") {
      value = value.replace(",", ".");
    }

    el.value = value;
  });
}

// HANDLER DELETE
function handlerDelete(id, name, type, width, drop, cost) {
  Swal.fire({
    title: name,
    html:
      "Sure to delete this data? <br/><br/> <b>Type :</b>" +
      type +
      " <b>Width :</b>" +
      width +
      " <b>Drop :</b>" +
      drop +
      " <b>Cost :</b>" +
      cost,
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        type: "post",
        url: uriMethod + "/PriceMatrixMethod.aspx/DeletePriceMatrix",
        data: JSON.stringify({
          id: id,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase()).then(() => {
              const el = document.getElementById(result.error.field);
              if (el) {
                // el.focus();
                el.classList.add("is-invalid");
              }
            });
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

const handlerDeleteByGroupAndType = (groupid, type, htmlgroupid) => {
  Swal.fire({
    title: htmlgroupid + " (" + type + ")",
    html: "Sure to delete this data?",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        type: "post",
        url: uriMethod + "/PriceMatrixMethod.aspx/DeletePriceMatrixByGroup",
        data: JSON.stringify({
          groupid: groupid,
          type: type,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (response) {
          const result = response.d || response;
          if (result.error) {
            isError(result.error.message.toUpperCase()).then(() => {
              const el = document.getElementById(result.error.field);
              if (el) {
                // el.focus();
                el.classList.add("is-invalid");
              }
            });
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
};

// #-------------------------|| Other Function ||-------------------------#
// FUNCTION CHECK SESSION
function checkSession() {
  // loaderFadeOut();
  setSessionAlive();

  handlerSelDesignType("#canvasFilter #designid");
}
