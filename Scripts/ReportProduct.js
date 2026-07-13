document.addEventListener("DOMContentLoaded", () => {
  console.log("ReportProduct.js loaded successfully");
  reportProductPageLoaded();
});
// =================================================EVENTS==================================================
document
  .querySelectorAll("#cardFind .form-control, #cardFind .form-select")
  .forEach((el) => {
    el.addEventListener("change", async (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "findby") {
        const finedlabel = document.getElementById("finedLabel");
        const findby = e.target.value;
        finedlabel.innerText = findby || "Fined";
        await bindFined(findby);
      }
    });
    el.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

document.querySelector("#btnFind").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  handlerFindReport(e.target.id);
});
// =================================================FUNCTION================================================
// --------------------------------------------||Binding Function||-----------------------------------------
const bindFindBy = () => {
  const sel = document.getElementById("findby");
  sel.innerHTML = ""; //reset

  let data = [];
  let list = [];

  list = ["product", "customer"];

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindStatus = () => {
  const sel = document.getElementById("status");
  sel.innerHTML = ""; //reset

  let data = [];
  let list = [];

  list = ["New Order", "In Production"];

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  // if (data.length > 1) {
  //   const defaultOption = document.createElement("option");
  //   defaultOption.text = "";
  //   defaultOption.value = "";
  //   sel.add(defaultOption);
  // }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindDate = () => {
  const fromdate = document.getElementById("fromdate");
  const todate = document.getElementById("todate");

  fromdate.value = new Date().toISOString().split("T")[0];
  todate.value = new Date().toISOString().split("T")[0];
};

const bindFined = async (findby) => {
  const select = document.getElementById("fined");
  select.innerHTML = "";

  if (!findby) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field: "fined",
          findby,
          rolename: ROLENAME,
        },
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindFined");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "ALL " + findby.toUpperCase();
        defaultOption.value = "all";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// --------------------------------------------||Handler Function||---------------------------------------

// Variabel global untuk menyimpan instance datatable agar bisa di-reset/refresh
let dataTableInstance = null;

const handlerFindReport = async (button) => {
  try {
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");

    const fields = ["findby", "fined", "status", "fromdate", "todate"];
    const formData = { rolename: ROLENAME };

    fields.forEach((field) => {
      formData[field] = document.getElementById(field).value;
    });

    console.table(formData);

    // 1. Panggil WebMethod menggunakan Fetch API
    const response = await fetch(`${URIMETHOD}/MyReport`, {
      // <--- Ganti NamaHalamanAnda.aspx sesuai file Anda
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: formData }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const res = await response.json();
    const result = res.d; // ASP.NET membungkus return value di dalam properti .d

    // 2. Cek Jika Mengembalikan Error dari Server (Validasi Server-Side Gagal)
    if (result.error) {
      const err = result.error;

      await isWarning(err.message?.toUpperCase());
      if (err.field) {
        const inputEl = document.getElementById(err.field);
        if (inputEl) {
          inputEl.classList.add("is-invalid");
        }
      }
    }

    if (result.success) {
      const dataBagiTabel = result.reportData || [];

      const findby = formData.findby;
      // masukan ke
      if (findby == "customer") {
        renderTable(dataBagiTabel);
      }
      if (findby == "product") {
        renderProductTable(dataBagiTabel);
      }

      Swal.close();
    }
  } catch (error) {
    let msg = error.message;
    if (ROLENAME !== "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg); // Memanggil fungsi penampil error Anda
  } finally {
    document.getElementById(button).innerHTML = "Show";
    // swal.close(); // Menutup loading sweetalert
  }
};
// --------------------------------------------||Other Function||-----------------------------------------
const renderTable = (data) => {
  const container = document.getElementById("cardResult");
  container.innerHTML = "";

  if (!data.length) {
    container.innerHTML = "<p>No data found</p>";
    return;
  }

  // Ambil semua kolom dari object pertama
  const columns = Object.keys(data[0]);

  let html = `<div class="table-responsive">
                <table class="table table-bordered table-striped table-hover">
                  <thead>
                    <tr>`;

  // HEADER
  columns.forEach((col) => {
    html += `<th>${col}</th>`;
  });

  html += `</tr></thead><tbody>`;

  // BODY
  data.forEach((row) => {
    html += `<tr>`;
    columns.forEach((col) => {
      html += `<td>${row[col] ?? 0}</td>`;
    });
    html += `</tr>`;
  });

  html += `</tbody></table></div>`;

  container.innerHTML = html;
};

const renderProductTable = (data) => {
  const container = document.getElementById("cardResult");
  container.innerHTML = "";

  if (!data.length) {
    container.innerHTML = "<p>No data found</p>";
    return;
  }

  let html = "";

  // Group berdasarkan bulan + tahun
  const grouped = data.reduce((acc, row) => {
    const key = `${row.MonthName} ${row.YearNum}`;

    if (!acc[key]) {
      acc[key] = [];
    }

    acc[key].push(row);

    return acc;
  }, {});

  Object.keys(grouped).forEach((monthYear) => {
    html += `
      <h4 class="mt-4">
        ${monthYear}
      </h4>

      <div class="table-responsive">
        <table class="table table-bordered table-striped table-hover">

          <thead>
            <tr>
              <th>No</th>
              <th>Product</th>
              <th>Qty</th>
            </tr>
          </thead>

          <tbody>
    `;

    grouped[monthYear].forEach((row, index) => {
      html += `
        <tr>
          <td>${index + 1}</td>
          <td>${row.DesignName}</td>
          <td>${row.Qty ?? 0}</td>
        </tr>
      `;
    });

    html += `
          </tbody>
        </table>
      </div>
    `;
  });

  container.innerHTML = html;
};
const reportProductPageLoaded = async () => {
  await Promise.all([bindFindBy(), bindStatus(), bindDate()]);

  await loaderFadeOut();
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
