document.addEventListener("DOMContentLoaded", () => {
  checkSessionCustomer();
});
// ==================================================|| EVENTS ||==================================================
// ==================================================|| FUNCTIONS ||===============================================
// ----------------------------------------------|| Binding Functions ||--------------------------------------------
let tableData;
const bindCustomer = (params) => {
  if (tableData) {
    tableData.destroy();
  }

  const paramData = {
    // designid: designid,
    // blindid: blindid,
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
      { data: "Id", width: "5%" },
      { data: "ExactId", width: "10%" },
      { data: "Name", width: "30%" },
      // {
      //   data: "Name",
      //   width: "55%",
      //   orderable: false,
      //   render: function (data, type, row) {
      //     let icn = "ti-circle-check";
      //     let color = "text-success";
      //     if (row.Active == "False") {
      //       icn = "ti-circle-x";
      //       color = "text-danger";
      //     }
      //     return `<i class="ti fs-3 me-1 ${icn} ${color}"></i>${data}`;
      //   },
      // },
      { data: "CustomerGroup", width: "15%" },
      { data: "CustomerCashSale", width: "15%" },
      { data: "CustomerOnStop", width: "15%" },
      { data: "CustomerMinSurcharge", width: "15%" },
      { data: "DataActive", width: "15%" },
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
// ----------------------------------------------|| Other Functions ||----------------------------------------------
const checkSessionCustomer = () => {
  // loaderFadeOut();

  bindCustomer("#card-table #data-table");
};

// --------------------------------------------------||Additional data table styling ||-------------------------------------------
const dropdownActionButton = (data, type, row) => {
  return `<div class="dropdown text-center">
            <button class="btn btn-sm btn-default dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
              action
            </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <span class="dropdown-header">Basic Action</span>
                <li>
                  <a class="dropdown-item" href="javascript:void(0)" id="btn-edit" data-id="${row.Id}">
                    <i class="ti ti-edit me-1 opacity-50 fs-2" ></i>Edit / Detail
                  </a>
                </li>
                <li>
                  <a class="dropdown-item text-danger" href="javascript:void(0)" id="btn-delete" data-id="${row.Id}" data-name="${row.Name}">
                    <i class="ti ti-trash-x me-1 opacity-50 fs-2"></i>Delete
                  </a>
                </li>

                <div class="dropdown-divider"></div>
                <span class="dropdown-header">configuration</span>
                <li>
                  <a class="dropdown-item " href="javascript:void(0)" id="btn-switch" data-id="${row.Id}" data-name="${row.Name}" data-active="${row.DataActive}">
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
