$(document).ready(function () {
  if (roleName == "Administrator") {
    console.log("CassetteExtra/Default.js loaded successfully");
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
$("#btnInsert").on("click", function (e) {
  // reset
  $(".form-control").val("");
  // set
  handlerSelectPriceGroup("#modalInsert #pricegroupid");
  $("#modalInsert #modalInsertLabel").html("Add a new list");
  // result
  handlerShowBSModal("modalInsert");
});

// BUTTON SUBMIT INSERT
$("#modalInsert #submitInsert").on("click", function () {
  console.log("submit insert");
});

// BUTTON IMPORT CSV
$("#btnImport").on("click", function (e) {
  // reset
  $(".form-control").val("");
  // set
  handlerSelectPriceGroup("#modalImport #pricegroupid");
  $("#modalImport #modalImportLabel").html("Import csv file");
  // result
  handlerShowBSModal("modalImport");
});

// BUTTON SUBMIT IMPORT
$("#modalImport #submitImport").on("click", submitFormImport);

// BUTTON SEARCH FILTER
$("#btnSearchFilter").on("click", function (e) {
  var pricegroupid = $("#pricegroupid").val();
  var width = $("#width").val();
  var drop = $("#drop").val();
  bindCassetteExtra(pricegroupid, width, drop);
  handlerCanvas("hide");
});

// BUTTON DELETE
$("#tableCassetteExtra").on("click", "#btn-delete", function () {
  handlerDelete(
    $(this).data("id"),
    $(this).data("name"),
    $(this).data("width"),
    $(this).data("drop"),
    $(this).data("cost")
  );
});

// #-------------------------|| Input Event ||-------------------------#
$("#modalImport #fileupload").on("change", function () {
  $(this).removeClass("is-invalid");
});
$("#modalImport #pricegroupid").on("change", function () {
  $(this).removeClass("is-invalid");
});

// ==================================================FUNCTIONS===============================================
// #-------------------------|| Submit Function ||-----------------------#
function submitFormImport() {
  uploadExcelFile(function (filename) {
    const formData = {
      filePath: filename,
      pricegroupid: document.querySelector("#modalImport #pricegroupid").value,
    };

    $.ajax({
      type: "POST",
      url: uriMethod + "/CassetteExtraMethod.aspx/ImportCSV",
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
          isError(result.error.message.toUpperCase(), result.error.field);
        } else {
          isSuccess(result.success);
          handlerHideBSModal("modalImport");
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

function uploadExcelFile(callback) {
  const fileInput = document.querySelector("#modalImport #fileupload");
  const pricegroupid = document.querySelector("#modalImport #pricegroupid");

  const fieldError = document.querySelectorAll("#modalImport .is-invalid");

  for (let i = 0; i < fieldError.length; i++) {
    fieldError[i].classList.remove("is-invalid");
  }

  if (!fileInput || !fileInput.files.length) {
    return isError("Please select a file !", "#modalImport #fileupload");
  }

  if (pricegroupid.value === "") {
    return isError(
      "Please select a price group !",
      "#modalImport #pricegroupid"
    );
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
function bindCassetteExtra(pricegroupid, width, drop) {
  tableData = $("#tableCassetteExtra").DataTable({
    processing: true,
    order: [],
    ajax: {
      url: uriMethod + "/CassetteExtraMethod.aspx/BindCassetteExtra",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      async: true,
      dataType: "json",
      data: function () {
        return JSON.stringify({
          pricegroupid: pricegroupid,
          width: width,
          drop: drop,
        });
      },
      dataSrc: function (json) {
        return json.d.data; // gunakan 'd.data' karena ASP.NET meng-serialize WebMethod dalam 'd'
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    columns: [
      {
        data: "No",
        render: function (data, type, row) {
          return `<div class="text-center">${data}</div>`;
        },
      },
      { data: "PriceGroupName" },
      { data: "Width" },
      { data: "Drop" },
      { data: "Cost" },
      {
        data: null,
        render: function (data, type, row) {
          return `
        <div class="dropdown text-center">
          <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
            <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
          </button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li>
              <a class="dropdown-item" href="javascript:void(0)" id="btn-edit" data-id="${row.Id}">
                <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
              </a>
            </li>
            <li>
              <a class="dropdown-item text-danger" href="javascript:void(0)" id="btn-delete"
                 data-id="${row.Id}"
                 data-name="${row.PriceGroupName}"
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

// BIND PRICE GROUP NAME
function handlerSelectPriceGroup(params) {
  return new Promise((resolve, reject) => {
    const sel = document.querySelector(params);
    sel.innerHTML = ""; //reset

    $.ajax({
      type: "POST",
      url: uriMethod + "/CassetteExtraMethod.aspx/BindPriceGroup",
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
              ? "No data returned from server : bindBlindType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          if (data.length > 1) {
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
// HANDLER DELETE
function handlerDelete(id, name, width, drop, cost) {
  Swal.fire({
    title: name,
    html:
      "Sure to delete this data? <br/><br/> <b>Width :</b>" +
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
      //   $.ajax({
      //     type: "post",
      //     url: "<?= base_url('administrator/clients/vendor-delete') ?>",
      //     data: {
      //       id: id,
      //     }, //$(this).serialize(),
      //     dataType: "json",

      //     success: function (response) {
      //       if (response.success) {
      //         isSuccess(response.success);
      //       } else if (response.warning) {
      //         isError(response.warning);
      //       }
      //     },
      //     error: function (xhr, ajaxOptions, thrownError) {
      //       var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
      //       isError(msg);
      //     },
      //   });
      isSuccess("Data has been deleted");
    }
  });
}

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
// #-------------------------|| Other Function ||-------------------------#
// FUNCTION CHECK SESSION
function checkSession() {
  loader();
  setSessionAlive();

  const pricegroupid = "";
  const width = "";
  const drop = "";
  bindCassetteExtra(pricegroupid, width, drop);
  handlerSelectPriceGroup("#canvasFilter #pricegroupid");
}

// LOADER
function loader() {
  const overlay = document.getElementById("loading-overlay");
  overlay.classList.add("fade-out"); // mulai fade-out
  setTimeout(() => {
    overlay.classList.add("d-none");
    overlay.classList.remove("d-flex", "fade-out");
  }, 500); // waktu harus sama dengan di CSS
}

// SET SESSION ALIVE
function setSessionAlive() {
  setInterval(() => {
    fetch("/Account/KeepSessionAlive.aspx", {
      method: "POST",
      credentials: "include",
    }).then((res) => {
      if (res.status === 401) {
        // session expired, redirect to login?
        window.location.href = "/Account/Login.aspx";
      }
      console.log(res.status);
    });
  }, 180000); // 3 menit
}

// RESPONSES MESSAGE
function isError(msg, field) {
  Swal.fire({
    icon: "error",
    title: "Oops...",
    html: msg,
  }).then(() => {
    setTimeout(() => {
      const fieldElement = document.querySelector(field);
      if (fieldElement && !fieldElement.classList.contains("is-invalid")) {
        fieldElement.classList.add("is-invalid");
      }
      fieldElement?.focus();
    }, 100); // beri jeda setelah SweetAlert menutup modal
  });
}
function isSuccess(msg, url) {
  Swal.fire({
    icon: "success",
    title: "Success",
    html: msg,
  }).then(() => {
    if (url) {
      setTimeout(() => {
        window.location.href = url;
      }, 100); // beri jeda setelah SweetAlert menutup modal
    } else {
      tableData.ajax.reload();
    }
  });
}
