$(document).ready(function () {
  console.log("importCsv.js loaded successfully");
  console.log("roleName: " + roleName);
  console.log("userId: " + userId);
  checkSession();
});

// ==================================================EVENTS==================================================
// #-------------------------|| Button Event ||-------------------------#
// BUTTON CANCEL
$("#btnCancel").on(
  "click",
  () => (window.location.href = "/setting/pricematrix")
);

// BUTTON SUBMIT
$("#btnSubmit").on("click", submitForm);

// BUTTON DOWNLOAD EXAMPLE
$("#btnDownloadExample").on("click", downloadExample);
// #-------------------------|| Input Event ||-------------------------#
// CHANGE FILE
$("#fileupload").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE DESIGN TYPE
$("#designid").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const designid = $(this).val();
  bindPriceGroup(designid.toUpperCase());
});

// CHANGE PRICE GROUP
$("#pricegroupid").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE TYPE
$("#type").on("change", function (e) {
  $(this).removeClass("is-invalid");
});
// ==================================================FUNCTIONS===============================================
// #-------------------------|| Binding Function ||-------------------------#
// BIND DESIGN TYPE
function bindDesignType() {
  return new Promise((resolve, reject) => {
    const sel = document.getElementById("designid");
    sel.innerHTML = ""; //reset

    $.ajax({
      type: "POST",
      url: "/Methods/SettingPage/PriceMatrix/ImportMatrixMethod.aspx/BindDesignType",
      // data: JSON.stringify({
      //   designId: designId,
      // }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
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
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            sel.add(option);
          });

          if (data.length === 1) {
            sel.selectedIndex = 0;
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

// BIND DESIGN TYPE
function bindPriceGroup(designid) {
  return new Promise((resolve, reject) => {
    const sel = document.getElementById("pricegroupid");
    sel.innerHTML = ""; //reset

    if (!designid) return resolve();

    $.ajax({
      type: "POST",
      url: "/Methods/SettingPage/PriceMatrix/ImportMatrixMethod.aspx/BindPriceGroup",
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
              ? "No data returned from server : bindPriceGroup"
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
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            sel.add(option);
          });

          if (data.length === 1) {
            sel.selectedIndex = 0;
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

// BIND BATTEN COLOUR
function bindType() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("type");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "Pick Up", text: "Pick Up" },
      { value: "Delivery", text: "Delivery" },
      { value: "INT-PU", text: "INT-PU" },
      { value: "INT-FIS", text: "INT-FIS" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// -------------------------|| Submit Function ||-------------------------#

function uploadExcelFile(callback) {
  const fileInput = document.getElementById("fileupload");
  const designid = document.getElementById("designid");
  const pricegroupid = document.getElementById("pricegroupid");
  const type = document.getElementById("type");

  const fieldError = document.querySelectorAll(".is-invalid");

  for (let i = 0; i < fieldError.length; i++) {
    fieldError[i].classList.remove("is-invalid");
  }

  if (!fileInput || !fileInput.files.length) {
    return isError("Please select a file !", "fileupload");
  }

  if (designid.value === "") {
    return isError("Please select a design type !", "designid");
  }

  if (pricegroupid.value === "") {
    return isError("Please select a price group !", "pricegroupid");
  }

  if (type.value === "") {
    return isError("Please select a type !", "type");
  }

  const formData = new FormData();
  formData.append("file", fileInput.files[0]);

  $.ajax({
    url: "/Methods/SettingPage/PriceMatrix/UploadHandler.ashx",
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

function submitForm() {
  uploadExcelFile(function (filename) {
    const formData = {
      filePath: filename,
      pricegroupid: document.getElementById("pricegroupid").value,
      type: document.getElementById("type").value,
    };

    $.ajax({
      type: "POST",
      url: "/Methods/SettingPage/PriceMatrix/ImportMatrixMethod.aspx/SubmitForm",
      data: JSON.stringify({ data: formData }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      beforeSend: function () {
        $("#btnSubmit").attr("disable", "disable");
        $("#btnSubmit").html('<i class="fa fa-spin fa-spinner"</i>');
      },
      complete: function () {
        $("#btnSubmit").removeAttr("disable");
        $("#btnSubmit").html(
          "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit"
        );
      },
      success: function (response) {
        const result = response.d || response;
        if (result.error) {
          isError(result.error.message.toUpperCase(), result.error.field);
        } else {
          isSuccess(result.success, null);
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
// #-------------------------|| Other Function ||-------------------------#
// FUNCTION CHECK SESSION
function checkSession() {
  const designid = document.getElementById("designid").value;

  loader();
  setSessionAlive();
  bindDesignType();
  bindPriceGroup(designid);
  bindType();
}

// FUNCTION LOADER
function loader() {
  const overlay = document.getElementById("loading-overlay");

  overlay.classList.add("fade-out"); // mulai fade-out
  setTimeout(() => {
    overlay.classList.add("d-none");
    overlay.classList.remove("d-flex", "fade-out");
  }, 1000); // waktu harus sama dengan di CSS
}

// FUNCTION DOWNLOAD EXAMPLE
function downloadExample() {
  return new Promise((resolve, reject) => {
    let timerInterval;
    Swal.fire({
      title: "Downloading...",
      html: "Is downloading, i will close in <b></b> milliseconds.",
      timer: 2000,
      timerProgressBar: true,
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
        window.location.href =
          "/Setting/PriceMatrix/DownloadExampleCSV.aspx?downloadExample=true";
        resolve();
      }
    });
  });
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

// FUNCTION MESSAGE
function isError(msg, field) {
  Swal.fire({
    icon: "error",
    title: "Oops...",
    html: msg,
  }).then(() => {
    setTimeout(() => {
      const fieldElement = document.getElementById(field);
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
    setTimeout(() => {
      if (url) {
        window.location.href = url;
      } else {
        window.location.reload();
      }
    }, 100); // beri jeda setelah SweetAlert menutup modal
  });
}
