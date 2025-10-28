document.addEventListener("DOMContentLoaded", () => {
  checkSessionCustomer();
  console.log(uriMethod);
});
// ==================================================|| EVENTS ||==================================================
// button detail
document
  .querySelector("#card-table #data-table")
  .addEventListener("click", (e) => {
    const btn = e.target.closest("#btn-detail");
    if (btn) {
      const id = btn.dataset.id;
      handlerOpenDetail(id);
    }
  });
// ==================================================|| FUNCTIONS ||===============================================
// ----------------------------------------------|| Binding Functions ||-------------------------------------------
let tableData;
const bindCustomer = (params) => {
  if (tableData) {
    tableData.destroy();
  }

  const paramData = {
    // designid: designid,
    // blindid: blindid,
    rolename: roleName,
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
      url: uriMethod + "/CustomerServerSide",
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
      { data: "Id", width: "10%" },
      { data: "ExactId", width: "10%" },
      { data: "Name", width: "35%" },
      { data: "CustomerGroup", width: "10%" },
      {
        data: "CustomerCashSale",
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let retVal = "No";
          if (row.CustomerCashSale == "1" || row.CustomerCashSale == "True")
            retVal = "Yes";
          return `<div class="text-center">${retVal}</div>`;
        },
      },
      {
        data: "CustomerOnStop",
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let retVal = "No";
          if (row.CustomerOnStop == "1" || row.CustomerOnStop == "True")
            retVal = "Yes";
          return `<div class="text-center">${retVal}</div>`;
        },
      },
      {
        data: "CustomerMinSurcharge",
        width: "10%",
        orderable: false,
        render: function (data, type, row) {
          let retVal = "No";
          if (
            row.CustomerMinSurcharge == "1" ||
            row.CustomerMinSurcharge == "True"
          )
            retVal = "Yes";
          return `<div class="text-center">${retVal}</div>`;
        },
      },
      {
        data: "DataActive",
        width: "5%",
        orderable: false,
        render: function (data, type, row) {
          let retVal = "No";
          if (row.DataActive == "1" || row.DataActive == "True") retVal = "Yes";
          return `<div class="text-center">${retVal}</div>`;
        },
      },
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
// ----------------------------------------------|| Handler Functions ||--------------------------------------------
const handlerOpenDetail = async (id) => {
  try {
    const response = await fetch(`${uriMethod}/SetSessionOpenCustomerDetail`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ id }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    // Jika sukses, arahkan ke halaman detail order
    window.location.href = "/setting/customer/detail";
  } catch (error) {
    isError("Gagal menyetel session: " + error.message);
  }
};
// ----------------------------------------------|| Other Functions ||----------------------------------------------
const checkSessionCustomer = () => {
  // loaderFadeOut();

  bindCustomer("#card-table #data-table");
};

// --------------------------------------------------||Additional data table styling ||-------------------------------------------
const dropdownActionButton = (data, type, row) => {
  return `<div class="dropdown text-center">
            <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
            </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btn-detail" data-id="${row.Id}">
                    <i class="ti ti-info-square-rounded me-1 opacity-50 fs-2" ></i>Detail
                  </a>
                </li>

                <div class="dropdown-divider"></div>
                <li>
                  <a class="dropdown-item " href="javascript:void(0)" id="btn-logs" data-id="${row.Id}">
                    <i class="ti ti-logout me-1 opacity-50 fs-2"></i>Logs
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
