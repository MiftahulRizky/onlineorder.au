document.addEventListener("DOMContentLoaded", function () {
  console.log("Create.js loaded successfully");
  createOrderPageLoaded();
});

// ================================================================INITIALIZE====================================================================
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const selectorEl = (el) => document.querySelector(el);
const selectorElAll = (el) => document.querySelectorAll(el);

const elForm = {
  divOrderType: getById("divOrderType"),
  // ordertype: getById("ordertype"),
  formDetail: getById("formDetail"),
  divCustomer: getById("divCustomer"),
  divCreatedBy: getById("divCreatedBy"),
  divCreatedDate: getById("divCreatedDate"),
  divOrderId: getById("divOrderId"),
  divCreateType: getById("divCreateType"),
  divFiles: getById("divFiles"),
  divOrderNumber: getById("divOrderNumber"),
  divOrderName: getById("divOrderName"),
  divDeliveryType: getById("divDeliveryType"),
  divDelivery: getById("divDelivery"),
  divSource: getById("divSource"),
  divJobId: getById("divJobId"),
  divJobDate: getById("divJobDate"),
  divShipmentId: getById("divShipmentId"),
  divShipping: getById("divShipping"),
  // customer: getById("customer"),
  // createdby: getById("createdby"),
  // createddate: getById("createddate"),
  // orderid: getById("orderid"),
  // jobid: getById("jobid"),
  // jobdate: getById("jobdate"),
};
const elModal = {
  modalShipping: getById("modalShipping"),
};

// ================================================================EVENTS========================================================================
selectorElAll(".form-control, .form-select").forEach((el) => {
  try {
    el.addEventListener("change", async (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "ordertype") {
        const ordertype = e.target.value;
        await bindCustomers(ordertype);
        if (["Panorama", "Evolve"].includes(ordertype)) {
          await bindUsers();
        }
        getById("createtype").value = "Create New";
        getById("files").value = "";

        displayElOverall(ordertype);
      }

      if (e.target.id === "customer") {
        const customer = e.target.value;
        const ordertype = getById("ordertype").value;
        getById("createtype").value = "Create New";
        getById("files").value = "";
        displayElOverall(ordertype);
      }

      if (e.target.id === "createtype") {
        const createtype = e.target.value;

        toggleShow(elForm.divOrderName, false);
        toggleShow(elForm.divOrderNumber, false);
        toggleShow(elForm.divFiles, false);
        if (createtype === "Upload CSV") {
          toggleShow(elForm.divFiles, true);
        }

        if (createtype === "Create New") {
          toggleShow(elForm.divOrderName, true);
          toggleShow(elForm.divOrderNumber, true);
        }

        getById("files").value = "";
      }

      if (e.target.id === "delivery") {
        const delivery = e.target.value;
        toggleShow(elForm.divDeliveryType, false);

        if (delivery === "Delivery") {
          toggleShow(elForm.divDeliveryType, true);
        }
      }
    });

    el.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "notes") {
        let maxLength = 1000;
        let currentLength = e.target.value.length;
        document.querySelector("#notescount").textContent =
          `${currentLength}/${maxLength}`;
      }
    });

    el.addEventListener("click", (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "shippingaddress") {
        selectorElAll(
          "#modalShipping .form-control, #modalShipping .form-select",
        ).forEach((el) => {
          el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
          el.classList.remove("is-invalid");
        });

        showBSModal("modalShipping");
      }
    });

    el.addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        handleSaveData(e);
      }
    });
  } catch (error) {
    const msg = `form event: ${error}`;
    catchMessages(msg);
  }
});

selectorElAll(".btn-information").forEach((el) => {
  el.addEventListener("click", (e) => {
    e.preventDefault();
    const id = e.currentTarget.id;
    let msg = "Oops!";

    switch (id) {
      case "btnInfoOrderNumber":
        msg = "Please do not use the following characters:";
        msg += '<br/> [ / ], [  ], [ & ], [ # ], [ ` ], [ , ], AND [ " ]';
        msg += "<br/> Maximum 20 characters for retailer order number.";
        break;

      case "btnInfoOrderName":
        msg = "Please do not use the following characters:";
        msg += '<br/> [ / ], [  ], [ & ], [ # ], [ ` ], [ , ], AND [ " ]';
        break;

      case "btnInfoDelivery":
        msg = "This applies to blinds only!";
        msg +=
          "<br/> This only applies if the customer does not have default delivery/pickup settings.";
        break;

      case "btnInfoDeliveryType":
        msg =
          "This determines the delivery type and affects certain surcharges.";
        msg += "<br/> If left blank, standard delivery applies.";
        break;

      case "btnInfoSourceType":
        msg =
          "This will determine whether the order is <b>NEW</b> or a <b>REMAKE</b>.";
        msg +=
          "<br/> If <b>REMAKE</b> is selected, the order will not generate a price.";
        break;
    }

    if (msg) {
      isInfo(msg);
    }
  });
});

selectorEl("#btn-cancel").addEventListener("click", (e) => {
  e.preventDefault();

  if (ACTION == "add") {
    window.location.href = "/order";
  }

  if (
    ACTION == "edit" &&
    ["blinds", "door", "window", "door and window"].includes(ORDERTYPE)
  ) {
    window.location.href = `/order/orderdetails?param=${ID}&ordertype=${ORDERTYPE.toLowerCase()}`;
  }

  if (ACTION == "edit" && ["panorama", "evolve"].includes(ORDERTYPE)) {
    window.location.href = `/order/shutters/detail?param=${ID}&ordertype=${ORDERTYPE}`;
  }
});

selectorEl("#btn-submit-order").addEventListener("click", (e) => {
  e.preventDefault();
  handleSaveData(e);
});

const modalEvents = {
  modalShipping: {
    init: (modal) => {},

    events: (modal) => {
      modal.addEventListener("input", async (e) => {
        e.target.classList.remove("is-invalid");
      });

      modal.addEventListener("click", async (e) => {
        const id = e.target.id;
        if (id === "btn-submit-shipping") {
          await saveDataShipping(id);
          // alert("shipping");
        }
      });
    },
  },
};

// ==============================================================FUNCTION========================================================================
// ----------------------------------------------|| Binding Functions ||---------------------------------------
const bindProductType = async () => {
  await bindListData({
    elementId: "ordertype",
    field: "ordertype",
    params: { customerid: CUSTOMERID, username: USERNAME, rolename: ROLENAME },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindCustomers = async (ordertype) => {
  if (!ordertype) return;

  await bindListData({
    elementId: "customer",
    field: "customer",
    params: { ordertype, rolename: ROLENAME },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });

  getById("customer").value = CUSTOMERID;
};

const bindUsers = async () => {
  await bindListData({
    elementId: "createdby",
    field: "createdby",
    params: { ordertype, rolename: ROLENAME },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });

  getById("createdby").value = LOGINID;
};

const bindOrderAggregate = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/BindOrderAggregate`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          headerid: ID,
          ordertype: ORDERTYPE,
          loginid: LOGINID,
          rolename: ROLENAME,
        },
      }),
    });
    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }
    const { d: data } = await response.json();
    if (!data) {
      throw new Error("No data");
    }
    if (data.error) {
      throw new Error(data.message);
    }

    console.log(data);
    const header = data.HeaderData;
    const isCustomer = ["Customer", "Representative"].includes(ROLENAME);
    const isProduction = ["In Production", "Canceled", "Completed"].includes(
      header.Status,
    );
    if (["Panorama", "Evolve"].includes(header.OrderType)) {
      if (isCustomer && isProduction) {
        window.location.href = `/order/shutters/detail?ultron=${header.Id}&infinity=panorama`;
        return;
      }
    }
    bindSelectPayload(data.Customers, "customer", true, 1);
    bindSelectPayload(data.Users, "createdby", true, 1);
    bindSelectPayload(data.Shipment, "shipmentid", true, 1);

    displayElOverall(header.OrderType, header);
    setValHeader(header);
    setValShipping(data.Shipping);
  } catch (error) {
    let msg = `bindOrderAggregate: ${error.message}`;
    catchMessages(msg);
  }
};

const bindSelect = ({
  data,
  elementId,
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  const select = document.getElementById(elementId);
  select.innerHTML = "";

  try {
    // default option
    if (withDefaultOption && data.length > lengthDefaultOption) {
      const opt = document.createElement("option");
      opt.value = "";
      opt.text = "";
      select.add(opt);
    }

    // render options
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      option.setAttribute("data-name", item.text);
      option.setAttribute("data-delivery", item.delivery);
      option.setAttribute("data-company", item.company);
      option.setAttribute("data-upload", item.upload);
      select.add(option);
    });

    select.classList.add("fw-bold");

    // callback setelah render
    if (afterRender) {
      afterRender(data, select);
    }

    // kalau cuma 1 data
    if (data.length === 1 && onSingle) {
      select.selectedIndex = 0;
      onSingle(data[0], select);
    }
  } catch (err) {
    const msg = `bindSelect: ${err.message}`;
    catchMessages(msg);
  }
};

const bindListData = async ({
  elementId,
  field,
  params = {},
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field,
          ...params,
        },
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`${response.status}\n${text}`);
    }

    const result = await response.json();
    const data = result.d.list;

    if (!Array.isArray(data)) {
      throw new Error(`No data returned from server : ${field}`);
    }

    bindSelect({
      data,
      elementId,
      withDefaultOption,
      lengthDefaultOption,
      onSingle,
      afterRender,
    });
  } catch (err) {
    const msg = `bindListData: ${err.message}`;
    catchMessages(msg);
  }
};

const bindSelectPayload = (data, el, def = true, leng = 0) => {
  try {
    bindSelect({
      data: data,
      elementId: el,
      withDefaultOption: def,
      lengthDefaultOption: leng,
    });
  } catch (error) {
    const msg = `bindSelectPayload: ${error.message}`;
    catchMessages(msg);
  }
};

// --------------------------------------------||Handle Functions ||----------------------------------------
const handleSaveData = (e) => {
  e.preventDefault();

  selectorElAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  saveData("btn-submit-order");
};

// ----------------------------------------------|| Saving Functions ||---------------------------------------
const saveData = async (button) => {
  try {
    getById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");

    const fields = [
      "ordertype",
      "headerid",
      "customer",
      "customerold",
      "createdby",
      "createddate",
      "orderid",
      "createtype",
      "ordernumber",
      "ordername",
      "delivery",
      "deliverytype",
      "sourcetype",
      "notes",
      "jobid",
      "jobdate",
      "shipmentid",
      "shippingaddress",
    ];

    const formData = {
      loginid: LOGINID,
      action: ACTION,
      rolename: ROLENAME,
      csvContent: "", // Penampung isi teks CSV
      fileName: "", // Optional: jika butuh nama filenya
    };

    // Ambil data dari form input standar
    fields.forEach((field) => {
      const el = document.getElementById(field);
      if (el) {
        if (el.type === "checkbox") {
          formData[field] = el.checked;
        } else {
          formData[field] = el.value;
        }
      }
    });

    // Proses Pembacaan File CSV
    const fileInput = document.getElementById("files");
    if (fileInput && fileInput.files.length > 0) {
      const file = fileInput.files[0];
      formData.fileName = file.name;

      // Baca isi file teks CSV menggunakan FileReader (dikemas dalam Promise)
      formData.csvContent = await new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = (e) => resolve(e.target.result);
        reader.onerror = (e) => reject(new Error("Failed to read file"));
        reader.readAsText(file); // Membaca file sebagai string teks
      });
    }

    // return console.log(formData);

    // Kirim data via fetch WebMethod
    const response = await fetch(URIMETHOD + "/Save", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: formData }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`${response.status}\n${errorText}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.debug) {
      console.table(res.list);
      throw new Error(res.message);
    }

    if (res.error) {
      throw new Error(res.message);
    }

    if (res.warning) {
      await isWarning(res.message?.toUpperCase());
      const field = getById(res.field);
      if (field) {
        field.classList.add("is-invalid");
      }
    }

    if (res.success) {
      if (formData.createtype === "Upload CSV") {
        if (ACTION === "add") {
          const alertReponse = await Swal.fire({
            title: "Success!",
            html: `Your CSV file has been processed. Would you like to upload another file or finish?`,
            icon: "success",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#6c757d",
            confirmButtonText: "Finish & View",
            cancelButtonText: "Upload Another",
            reverseButtons: true,
            customClass: {
              popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
            },
          });

          if (alertReponse.isConfirmed) {
            window.location.href = res.url;
          } else {
            getById("files").value = "";
          }
        }

        if (ACTION === "edit") {
          await isSuccess(res.message);
          window.location.href = res.url;
        }
      } else {
        await isSuccess(res.message);
        window.location.href = res.url;
      }
    }
  } catch (error) {
    const msg = `saveData: ${error.message}`;
    catchMessages(msg);
  } finally {
    getById(button).innerHTML = "Next";
  }
};

const saveDataShipping = async (button) => {
  try {
    button.innerHTML = "Proccessing...";
    swalLoadingShow("Please wait while we change the pricing.");

    const md = elModal.modalShipping;
    md.querySelectorAll(".form-control").forEach((e) =>
      e.classList.remove("is-invalid"),
    );

    const fields = [
      "unitnumber",
      "customer",
      "id",
      "streetaddress",
      "suburb",
      "states",
      "postcode",
      "addressport",
    ];

    const formData = {
      loginid: LOGINID,
      rolename: ROLENAME,
    };

    fields.forEach((field) => {
      formData[field] = selectorEl(`#modalShipping #${field}`).value;
    });

    // return console.log(formData);

    const response = await fetch(`${URIMETHOD}/SaveShipping`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: formData }),
    });

    if (!response.ok) {
      throw new Error(
        `HTTP error: ${response.status} || ${response.statusText}`,
      );
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    }

    if (res.warning) {
      await isWarning(res.message.toUpperCase());
      const field = selectorEl(`#modalShipping #${res.field}`);
      if (field) {
        field.classList.add("is-invalid");
      }
    }

    if (res.success) {
      hideBSModal("modalShipping");
      await isSuccess(res.message);
      location.reload();
    }
  } catch (error) {
    const msg = `saveDataShipping: ${error.message}`;
    catchMessages(msg);
  } finally {
    button.innerHTML = "Save Changes";
  }
};
// ----------------------------------------------|| Display Functions ||---------------------------------------
const displayElOverall = async (ordertype, header) => {
  try {
    Object.values(elForm).forEach((el) => toggleShow(el, false));
    const isAdministrator = ROLENAME === "Administrator";
    const isPpicDeCs = ["PPIC & DE", "Data Entry", "Customer Service"].includes(
      ROLENAME,
    );
    const isCustomer = ROLENAME === "Customer";
    const isSuperAdmin = LEVELNAME === "Super Admin";
    const isLeader = LEVELNAME === "Leader";
    const isBlinds = ordertype === "Blinds";
    const isDoorWindows = ordertype === "Door and Window";
    const isShutters = ["Panorama", "Evolve"].includes(ordertype);
    const isEdit = ACTION === "edit";
    const isAdd = ACTION === "add";

    getById("createddate").value = new Date().toLocaleDateString("en-CA");
    toggleShow(elForm.divOrderType, true);

    if (!ordertype) return;
    toggleShow(elForm.formDetail, true);

    if (!isCustomer) {
      toggleShow(elForm.divCustomer, true);
    }

    if (isBlinds) {
      if (isAdd) {
        // toggleShow(elForm.divCreateType, true);
        const createtype = getById("createtype").value;
        if (createtype === "Create New") {
          toggleShow(elForm.divOrderNumber, true);
          toggleShow(elForm.divOrderName, true);
        }
        if (createtype === "Upload CSV") {
          toggleShow(elForm.divFiles, true);
        }
      }
    }

    if (isDoorWindows) {
      toggleShow(elForm.divOrderNumber, true);
      toggleShow(elForm.divOrderName, true);
    }

    if (isShutters) {
      toggleShow(elForm.divOrderNumber, true);
      toggleShow(elForm.divOrderName, true);
      if (isAdministrator) {
        toggleShow(elForm.divCreatedBy, true);
        toggleShow(elForm.divCreatedDate, true);
      }
    }

    const isDelivery = getById("customer").selectedOptions[0]?.dataset.delivery;
    const isUpload = getById("customer").selectedOptions[0]?.dataset.upload;
    if (!isDelivery && isBlinds) {
      getById("delivery").value = "";
      getById("deliverytype").value = "";
      toggleShow(elForm.divDelivery, true);
    }

    if (isUpload === "True" && isBlinds) {
      if (isAdd) {
        getById("createtype").value = "Create New";
        toggleShow(elForm.divCreateType, true);
      }
    }

    if (isEdit) {
      getById("btn-submit-order").innerHTML = "Save Changes";

      toggleShow(elForm.divOrderType, false);
      toggleShow(elForm.divOrderId, true);
      getById("orderid").setAttribute("disabled", true);

      if (isBlinds) {
        toggleShow(elForm.divOrderNumber, true);
        toggleShow(elForm.divOrderName, true);
        toggleShow(elForm.divDelivery, true);
        if (!isCustomer) {
          if (header.Delivery === "Delivery") {
            toggleShow(elForm.divDeliveryType, true);
          }
          toggleShow(elForm.divSource, true);
        }
      }

      if (isShutters) {
        selectorEl("#formDetail #customer").setAttribute("disabled", true);
        getById("createdby").setAttribute("disabled", true);
        getById("createddate").setAttribute("disabled", true);

        getById("jobid").setAttribute("disabled", true);
        getById("jobdate").setAttribute("disabled", true);
        if (isAdministrator) {
          toggleShow(elForm.divShipping, true);

          const status = header.Status;
          if (status === "In Production") {
            toggleShow(elForm.divJobId, true);
            toggleShow(elForm.divJobDate, true);
          }

          selectorEl("#formDetail #customer").removeAttribute("disabled");
          getById("createdby").removeAttribute("disabled");
          getById("createddate").removeAttribute("disabled");

          if (isLeader || isSuperAdmin) {
            getById("orderid").setAttribute("disabled", true);
            getById("jobid").setAttribute("disabled", true);
            getById("jobdate").setAttribute("disabled", true);
          }
        }
      }

      if (isPpicDeCs) {
        toggleShow(elForm.divCreatedBy, true);
        toggleShow(elForm.divCreatedDate, true);
        toggleShow(elForm.divShipping, true);
      }
    }

    if (isAdd) {
      getById("btn-submit-order").innerHTML = "Next";
    }
  } catch (error) {
    const msg = `displayElOverall: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| SetVal Functions ||---------------------------------------
const setValShipping = (item) => {
  try {
    const shipping = selectorEl("#shippingaddress");
    const modalShippingLabel = selectorEl("#modalShipping #modalShippingLabel");
    const customer = selectorEl("#formDetail #customer");

    console;

    selectorEl("#modalShipping #customer").value = customer.value;
    modalShippingLabel.innerHTML = "Add Primary Address";

    if (!item || Object.keys(item).length === 0) return;

    modalShippingLabel.innerHTML = "Edit Primary Address";
    selectorEl("#modalShipping #id").value = item.Id;
    selectorEl("#modalShipping #customer").value = item.CustomerId;
    selectorEl("#modalShipping #unitnumber").value = item.UnitNumber;
    selectorEl("#modalShipping #streetaddress").value = item.Street;
    selectorEl("#modalShipping #suburb").value = item.Suburb;
    selectorEl("#modalShipping #states").value = item.States;
    selectorEl("#modalShipping #postcode").value = item.PostCode;
    selectorEl("#modalShipping #addressport").value = item.Port;
    shipping.value = `${item.UnitNumber} ${item.Street}, ${item.Suburb}, ${item.States} ${item.PostCode}`;
  } catch (error) {
    const msg = `setValShipping: ${error.message}`;
    catchMessages(msg);
  }
};

const setValHeader = (itemData) => {
  try {
    const mapping = {
      ordertype: "OrderType",
      headerid: "Id",
      customer: "CustomerId",
      customerold: "CustomerId",
      createdby: "CreatedBy",
      createddate: "CreatedDate",
      orderid: "OrderId",
      createtype: "CreateType",
      files: "",
      ordernumber: "OrderNumber",
      ordername: "OrderName",
      delivery: "Delivery",
      deliverytype: "DeliveryType",
      sourcetype: "OrderSourceType",
      sourcetypeold: "OrderSourceType",
      notes: "OrderNote",
      jobid: "JoNumberId",
      jobdate: "JobDate",
      shipmentid: "ShipmentId",
    };

    Object.entries(mapping).forEach(([id, key]) => {
      const el = document.getElementById(id);
      if (!el) {
        console.warn(`Elemen '${id}' tidak ditemukan.`);
        return;
      }

      let value = itemData[key];

      if (id === "customer") {
        value = value ? value.toUpperCase() : "";
      }

      if (id === "createdby") {
        value = value ? value.toUpperCase() : "";
      }

      if (id === "createddate" || id === "jobdate") {
        if (value) {
          // Tangani format /Date(123456789)/ dari ASP.NET
          if (typeof value === "string" && value.includes("/Date(")) {
            const timestamp = parseInt(
              value.replace(/\/Date\((.*?)\)\//, "$1"),
              10,
            );
            value = isNaN(timestamp)
              ? ""
              : new Date(timestamp).toISOString().split("T")[0];
          } else {
            // Fallback ke parser bawaan kamu jika formatnya beda
            const parsed = parseDDMMYYYYToDate(value);
            value = parsed ? parsed.toISOString().split("T")[0] : "";
          }
        } else {
          value = "";
        }
      }

      el.value = value ?? "";
    });
  } catch (error) {
    const msg = `setValHeader: ${error.message}`;
    catchMessages(msg);
  }
};
// --------------------------------------------||Other Functions ||-------------------------------------------
const createOrderPageLoaded = async () => {
  if (!ACTION) window.location.href = "/order";
  await bindProductType();

  if (ACTION === "add") {
    getById("titleCard").textContent = "Create New Order";
    await displayElOverall();
    await loaderFadeOut();
  } else if (ACTION === "edit" && ID && ORDERTYPE) {
    getById("titleCard").textContent = "Edit Order";
    await bindOrderAggregate();
    await loaderFadeOut();
  } else {
    window.location.href = "/order";
  }
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  if (!["trim"].includes(elementId)) {
    list.sort();
  }

  // default option kalau lebih dari 1 data
  if (list.length > lengthDefaultOption) {
    const defaultOption = new Option("", "");
    sel.add(defaultOption);
  }

  list.forEach((item) => {
    const option = new Option(item.toUpperCase(), item);
    option.setAttribute("data-name", item);
    sel.add(option);
  });
};

const toggleShow = (el, show) => {
  if (!el) return;
  el.classList.toggle("d-none", !show);
};

const toggleShowList = (keys, show) => {
  keys.forEach((key) => {
    if (liEl[key]) {
      Array.from(liEl[key]).forEach((li) =>
        li.classList.toggle("d-none", !show),
      );
    }
  });
};

const hideBSModal = (id) => {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
};

const showBSModal = (params) => {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
};

Object.entries(elModal).forEach(([key, modal]) => {
  if (!modal) return;

  const handle = modalEvents[key];
  if (!handle) return;

  if (handle.init) {
    handle.init(modal);
  }

  if (handle.events) {
    handle.events(modal);
  }
});

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
