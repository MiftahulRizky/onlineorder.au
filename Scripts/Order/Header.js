document.addEventListener("DOMContentLoaded", function () {
  console.log("Create.js loaded successfully");
  checkSessionCreateHeader();
});
// ==================================================EVENTS==================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
  });
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id == "ordertype") {
      const ordertype = e.target.value;
      // await visibleElementFormOnChange(ordertype);
      // alert(ordertype);
      await visibleElementForm(ordertype);
      await handlerSelCustomer(ordertype, "#customer");
      if (["Panorama", "Evolve"].includes(ordertype)) {
        await Promise.all([handlerSelUser("#createdby")]);
      }
    }

    if (e.target.id == "customer") {
      const ordertype = document.getElementById("ordertype").value;
      const customer = e.target.value;

      await Promise.all([visibleElementForm(ordertype, customer)]);
    }
  });
});

document.querySelector("#btnInfoOrderNumber").addEventListener("click", (e) => {
  let msg = "Please do not use the following characters:";
  msg += '<br/> [ / ], [  ], [ & ], [ # ], [ ` ], [ , ], AND [ " ]';
  msg += "<br/> Maximum 20 characters for retailer order number.";
  isInfo(msg);
});

document.querySelector("#btnInfoOrderName").addEventListener("click", (e) => {
  let msg = "Please do not use the following characters:";
  msg += '<br/> [ / ], [  ], [ & ], [ # ], [ ` ], [ , ], AND [ " ]';
  isInfo(msg);
});

document.querySelector("#btnInfoDelivery").addEventListener("click", (e) => {
  let msg = "This applies to blinds only!";
  msg +=
    "<br/> This only applies if the customer does not have default delivery/pickup settings.";
  isInfo(msg);
});

// click shipping
document.querySelector("#shipping").addEventListener("click", (e) => {
  document
    .querySelectorAll(
      "#modalShipping, #modalShipping .form-control, #modalShipping .form-select",
    )
    .forEach((el) => {
      el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
      el.classList.remove("is-invalid");
    });
  handlerShowBSModal("modalShipping");
});

// button submit
document.querySelector("#btn-submit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    // el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
    el.classList.remove("is-invalid");
  });

  handlerSubmit(e.target.form, e.target, e.target.innerHTML);
});

// button cancel
document.querySelector("#btn-cancel").addEventListener("click", (e) => {
  e.preventDefault();

  if (ACTION == "add") {
    window.location.href = "/order";
  }

  if (ACTION == "edit" && ORDERTYPE == "blinds") {
    window.location.href = `/order/detail?param=${ID}&ordertype=${ORDERTYPE.toLowerCase()}`;
  }

  if (ACTION == "edit" && (ORDERTYPE == "panorama" || ORDERTYPE == "evolve")) {
    // let ortype = ORDERTYPE;
    window.location.href = `/order/shutters/detail?param=${ID}&ordertype=${ORDERTYPE}`;
  }
});

// --------------------------------------------|| modalShipping ||-----------------------------------------

document
  .querySelectorAll("#modalShipping .form-control, #modalShipping .form-select")
  .forEach((el) => {
    el.addEventListener("change", () => {
      el.classList.remove("is-invalid");
    });
    el.addEventListener("input", () => {
      el.classList.remove("is-invalid");
    });
  });

// button submit
document
  .querySelector("#modalShipping #btn-submit")
  .addEventListener("click", (e) => {
    e.preventDefault();

    document
      .querySelectorAll(
        "#modalShipping, #modalShipping .form-control, #modalShipping .form-select",
      )
      .forEach((el) => {
        el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        el.classList.remove("is-invalid");
      });

    handlerSubmitShipping(e.target.form, e.target, e.target.innerHTML);
  });

// ==============================================|| FUNCTIONS ||=============================================
const handlerHideBSModal = (id) => {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
};

const handlerShowBSModal = (params) => {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
};
// --------------------------------------------||Handler Functions ||-----------------------------------------
const handlerSubmit = async (formEl, button, htmlButton) => {
  try {
    const formData = new FormData(formEl);

    let formObject = Object.fromEntries(formData.entries());
    const excludeKeys = [
      "__EVENTTARGET",
      "__EVENTARGUMENT",
      "__VIEWSTATE",
      "__VIEWSTATEGENERATOR",
      "__SCROLLPOSITIONX",
      "__SCROLLPOSITIONY",
      "__EVENTVALIDATION",
      "ctl00$txtSearchMaster",
    ];

    formObject = Object.fromEntries(
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key)),
    );

    const additionalData = {
      loginid: LOGINID,
      actions: ACTION,
    };

    const finalData = {
      ...formObject,
      ...additionalData,
    };

    // debug konsisten
    // return console.table(finalData);

    // before send
    button.setAttribute("disabled", "disabled");
    button.innerHTML = '<i class="ti ti-loader fs-2 me-1"></i> Processing...';

    const response = await fetch(URIMETHOD + "/Submit", {
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
        ROLENAME === "Administrator"
          ? `${response.status}\n${errorText}`
          : "Something went wrong, please try again!",
      );
    }

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isWarning(dataResult.error.message.toUpperCase());
      const field = document.getElementById(dataResult.error.field);
      if (field) {
        field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      await isSuccess(dataResult.success.message);
      window.location.href = dataResult.success.url;
    }
  } catch (err) {
    await isError(err.message);
  }
};

const handlerSubmitShipping = async (formEl, button, htmlButton) => {
  try {
    const formData = new FormData(formEl);

    let formObject = Object.fromEntries(formData.entries());
    const excludeKeys = [
      "__EVENTTARGET",
      "__EVENTARGUMENT",
      "__VIEWSTATE",
      "__VIEWSTATEGENERATOR",
      "__SCROLLPOSITIONX",
      "__SCROLLPOSITIONY",
      "__EVENTVALIDATION",
      "ctl00$txtSearchMaster",
    ];

    formObject = Object.fromEntries(
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key)),
    );

    const additionalData = {
      loginid: LOGINID,
    };

    const finalData = {
      ...formObject,
      ...additionalData,
    };

    // debug konsisten
    // return console.table(finalData);

    // before send
    button.setAttribute("disabled", "disabled");
    button.innerHTML = '<i class="ti ti-loader fs-2 me-1"></i> Processing...';

    const response = await fetch(URIMETHOD + "/SubmitShipping", {
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
        ROLENAME === "Administrator"
          ? `${response.status}\n${errorText}`
          : "Something went wrong, please try again!",
      );
    }

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isWarning(dataResult.error.message.toUpperCase());
      const field = document.getElementById(dataResult.error.field);
      if (field) {
        field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      await isSuccess(dataResult.success.message);
      handlerHideBSModal("modalShipping");
      location.reload();
    }
  } catch (err) {
    await isError(err.message);
  }
};

const handlerSelCustomer = async (ordertype, params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!ordertype) return;

  try {
    const response = await fetch(URIMETHOD + "/BindCustomer", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ ordertype: ordertype, rolename: ROLENAME }), // aktifkan jika butuh kirim data
    });

    if (!response.ok) {
      const msg = `${response.status}\n${response.statusText}`;
      throw new Error(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      throw new Error("No data returned from server : handlerSelCustomer");
    }

    if (Array.isArray(data)) {
      sel.innerHTML = "";

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      data.forEach((item) => {
        const option = document.createElement("option");

        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-delivery", item.delivery);
        sel.add(option);
      });

      sel.value = CUSTOMERID;
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message || error
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const handlerSelUser = async (params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!params) return;

  try {
    const response = await fetch(URIMETHOD + "/BindUser", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      //   body: JSON.stringify({ ordertype: ordertype, rolename: ROLENAME }),
    });

    if (!response.ok) {
      const msg = `${response.status}\n${response.statusText}`;
      throw new Error(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      throw new Error("No data returned from server : handlerSelUser");
    }

    if (Array.isArray(data)) {
      sel.innerHTML = "";

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      data.forEach((item) => {
        const option = document.createElement("option");

        option.value = item.value;
        option.text = item.text.toUpperCase();
        // option.setAttribute("data-name", item.text);
        sel.add(option);
      });

      sel.value = LOGINID;
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message || error
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const handlerSelShipment = async (params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!params) return;

  try {
    const response = await fetch(URIMETHOD + "/BindShipment", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      //   body: JSON.stringify({ ordertype: ordertype, rolename: ROLENAME }),
    });

    if (!response.ok) {
      const msg = `${response.status}\n${response.statusText}`;
      throw new Error(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      throw new Error("No data returned from server : handlerSelShipment");
    }

    if (Array.isArray(data)) {
      sel.innerHTML = "";

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      data.forEach((item) => {
        const option = document.createElement("option");

        option.value = item.value;
        option.text = item.text.toUpperCase();
        // option.setAttribute("data-name", item.text);
        sel.add(option);
      });

      sel.value = LOGINID;
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message || error
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const handlerShipping = async (customerid) => {
  try {
    if (!customerid) return;

    const res = await fetch(`${URIMETHOD}/BindShipping`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ customerid }),
    });

    if (!res.ok) {
      const msg = `${res.status} - ${res.statusText}`;
      throw new Error(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      return true;
    }

    for (const item of data) {
      const shipping = document.querySelector("#shipping");
      const modalShippingLabel = document.querySelector(
        "#modalShipping #modalShippingLabel",
      );
      const customer = document.querySelector("#customer");

      document.querySelector("#modalShipping #customer").value = customer.value;
      modalShippingLabel.innerHTML = "Add Primary Address";

      if (!item) return;
      modalShippingLabel.innerHTML = "Edit Primary Address";
      document.querySelector("#modalShipping #id").value = item.Id;
      document.querySelector("#modalShipping #customer").value =
        item.CustomerId;
      document.querySelector("#modalShipping #unitnumber").value =
        item.UnitNumber;
      document.querySelector("#modalShipping #streetaddress").value =
        item.Street;
      document.querySelector("#modalShipping #suburb").value = item.Suburb;
      document.querySelector("#modalShipping #states").value = item.States;
      document.querySelector("#modalShipping #postcode").value = item.PostCode;
      document.querySelector("#modalShipping #addressport").value = item.Port;
      shipping.value = `${item.UnitNumber} ${item.Street}, ${item.Suburb}, ${item.States} ${item.PostCode}`;
    }

    return true; // ✅ success
  } catch (error) {
    console.error("BindShipping error:", error);
    throw error;
  }
};

const handlerEdit = async (id, ordertype) => {
  try {
    if (!id) return;

    const res = await fetch(`${URIMETHOD}/Find`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, ordertype }),
    });

    if (!res.ok) {
      const msg = `${res.status} - ${res.statusText}`;
      throw new Error(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      throw new Error("No data returned from server : handlerEdit");
    }

    for (const item of data) {
      // Panorama
      if (item.OrderType == "Panorama") {
        if (
          (ROLENAME == "Customer" || ROLENAME == "Representative") &&
          (item.Status == "In Production" ||
            item.Status == "Canceled" ||
            item.Status == "Completed")
        ) {
          window.location.href = `/order/shutters/detail?ultron=${item.Id}&infinity=panorama`;
          return;
        }
      }
      await handlerSelCustomer(item.OrderType, "#customer");
      await handlerSelUser("#createdby");
      await handlerSelShipment("#shipmentid");
      await handlerShipping(item.CustomerId);
      await visibleElementForm(item.OrderType, item.CustomerId);
      await handlerSetElementValues(item);
      await loaderFadeOut();
    }

    return true; // ✅ success
  } catch (error) {
    console.error("handlerEdit error:", error);
    throw error;
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    ordertype: "OrderType",
    id: "Id",
    customer: "CustomerId",
    createdby: "CreatedBy",
    createddate: "CreatedDate",
    ordernumber: "OrderNumber",
    ordername: "OrderName",
    delivery: "Delivery",
    jobid: "JoNumberId",
    jobdate: "JobDate",
    orderid: "OrderId",
    shipmentid: "ShipmentId",
    note: "OrderNote",
  };

  console.table(itemData);

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
      // value = value ? new Date(value).toLocaleDateString("en-CA") : "";
      const parsed = parseDDMMYYYYToDate(value);
      value = parsed ? parsed.toISOString().split("T")[0] : "";
    }

    el.value = value ?? "";
  });
};

const parseDDMMYYYYToDate = (value) => {
  if (!value) return null;

  const [datePart] = value.split(" "); // ambil "24/11/2025"
  const [day, month, year] = datePart.split("/");

  return new Date(`${year}-${month}-${day}`); // format valid
};

const handlerSelOrderType = async (params) => {
  if (!params) return;

  const sel = document.querySelector(params);
  if (!sel) return;
  sel.innerHTML = ""; // Reset options

  let data = [];
  data = [
    { value: "", text: "" },
    { value: "Blinds", text: "Blinds" },
    { value: "Panorama", text: "Panorama" },
    { value: "Evolve", text: "Evolve" },
  ];

  if (CUSTOMERID == "LS-A224") {
    // JPM Direct
    data = [
      { value: "", text: "" },
      { value: "Panorama", text: "Panorama" },
    ];
  }

  if (CUSTOMERID == "DEFAULT" && USERNAME == "galih") {
    data = [
      { value: "", text: "" },
      { value: "Panorama", text: "Panorama" },
      { value: "Evolve", text: "Evolve" },
    ];
  }

  for (const { value, text } of data) {
    const option = document.createElement("option");
    option.value = value;
    option.textContent = text.toUpperCase();
    sel.appendChild(option);
  }
};

// --------------------------------------------||Other Functions ||-------------------------------------------
const checkSessionCreateHeader = async () => {
  if (!ACTION) window.location.href = "/order";
  await handlerSelOrderType("#ordertype");
  if (ACTION === "add") {
    visibleElementForm();
    await loaderFadeOut();
    document.getElementById("titleCard").textContent = "Create New Order";
  } else if (ACTION === "edit" && ID && ORDERTYPE) {
    handlerEdit(ID, ORDERTYPE);
    document.getElementById("titleCard").textContent = "Edit Order";
  } else {
    window.location.href = "/order";
  }
};

const visibleElementForm = async (ordertype, customer) => {
  try {
    const ordertypeEl = document.getElementById("ordertype");
    const formDetail = document.getElementById("formDetail");
    const divOrderType = document.getElementById("divOrderType");
    const divCustomer = document.getElementById("divCustomer");
    const divCreatedBy = document.getElementById("divCreatedBy");
    const divCreatedDate = document.getElementById("divCreatedDate");
    const divOrderId = document.getElementById("divOrderId");
    const divDelivery = document.getElementById("divDelivery");
    const divJobId = document.getElementById("divJobId");
    const divJobDate = document.getElementById("divJobDate");
    const divShipmentId = document.getElementById("divShipmentId");
    const divShipping = document.getElementById("divShipping");

    const customerEl = document.getElementById("customer");
    const createdbyEL = document.getElementById("createdby");
    const createddateEl = document.getElementById("createddate");
    createddateEl.value = new Date().toLocaleDateString("en-CA");
    const orderidEl = document.getElementById("orderid");
    const jobidEl = document.getElementById("jobid");
    const jobdateEl = document.getElementById("jobdate");

    formDetail.setAttribute("hidden", true);
    divCustomer.setAttribute("hidden", true);
    divCreatedBy.setAttribute("hidden", true);
    divCreatedDate.setAttribute("hidden", true);
    divOrderId.setAttribute("hidden", true);
    divDelivery.setAttribute("hidden", true);
    divJobId.setAttribute("hidden", true);
    divJobDate.setAttribute("hidden", true);
    divShipmentId.setAttribute("hidden", true);
    divShipping.setAttribute("hidden", true);

    if (!ordertype) return; //throw new Error(ordertype);
    formDetail.removeAttribute("hidden");

    if (ordertype === "Blinds") {
      const cus = document.getElementById("customer");
      const customerDelivery =
        cus?.options?.[cus.selectedIndex]?.dataset?.delivery;
      if (!customerDelivery) {
        divDelivery.removeAttribute("hidden");
      }
    }

    if (["Panorama", "Evolve"].includes(ordertype)) {
      if (
        [
          "Administrator",
          "Customer Service",
          "Data Entry",
          "PPIC & DE",
        ].includes(ROLENAME)
      ) {
        divCustomer.removeAttribute("hidden");
      }

      if (ROLENAME == "Administrator") {
        divCreatedBy.removeAttribute("hidden");
        divCreatedDate.removeAttribute("hidden");
      }
    }

    if (!customer) return;
    const customerDelivery = await getItemData(
      `SELECT Delivery FROM Customers WHERE Id = '${customer}'`,
    );

    if (customerDelivery) {
      divDelivery.setAttribute("hidden", true);
    }

    // if (CUSTOMERID == "LS-A224") {
    //   divOrderType.setAttribute("hidden", true);
    //   visibleElementFormOnChange(ordertype.value);
    //   return;
    // }

    if (ACTION !== "edit") return;

    divOrderType.setAttribute("hidden", true);
    formDetail.removeAttribute("hidden");
    divOrderId.removeAttribute("hidden");

    orderidEl.setAttribute("readonly", true);
    orderidEl.classList.add("bg-body-secondary");
    orderidEl.classList.add("text-secondary");

    if (ordertype == "Panorama" || ordertype == "Evolve") {
      // divShipmentId.removeAttribute("hidden");

      customerEl.setAttribute("readonly", true);
      createdbyEL.setAttribute("readonly", true);
      createddateEl.setAttribute("readonly", true);

      orderidEl.setAttribute("readonly", true);
      jobdateEl.setAttribute("readonly", true);

      if (ROLENAME == "Administrator") {
        divCustomer.removeAttribute("hidden");
        divCreatedBy.removeAttribute("hidden");
        divCreatedDate.removeAttribute("hidden");
        divShipping.removeAttribute("hidden");
        divDelivery.setAttribute("hidden", true);

        const status = await getItemData(
          `SELECT Status FROM view_order_headers WHERE Id = '${ID}' AND OrderType = '${ordertype}'`,
        );
        if (status == "In Production") {
          divJobId.removeAttribute("hidden");
          divJobDate.removeAttribute("hidden");
        }

        customerEl.removeAttribute("readonly");
        createdbyEL.removeAttribute("readonly");
        createddateEl.removeAttribute("readonly");

        if (LEVELNAME == "Leader" || "Super Admin") {
          orderidEl.removeAttribute("readonly");
          jobidEl.removeAttribute("readonly");
          jobdateEl.removeAttribute("readonly");
        }
      }

      if (
        ROLENAME == "Customer Service" ||
        ROLENAME == "Data Entry" ||
        ROLENAME == "PPIC & DE"
      ) {
        divCustomer.removeAttribute("hidden");
        divCreatedBy.removeAttribute("hidden");
        divCreatedDate.removeAttribute("hidden");
        divShipping.removeAttribute("hidden");
      }
    }
  } catch (error) {
    if (ROLENAME === "Administrator") {
      isError(error.message);
    }
  }
};

const getItemData = async (query) => {
  try {
    const response = await fetch(`${URIMETHOD}/GetItemData`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: query }), // ✅ FIX
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};
