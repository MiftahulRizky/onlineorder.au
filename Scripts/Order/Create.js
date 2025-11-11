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

    if (e.target.id !== "ordertype") return;
    const formDetail = document.getElementById("formDetail");
    const divCreatedBy = document.getElementById("divCreatedBy");
    const divCreatedDate = document.getElementById("divCreatedDate");
    const divOrderId = document.getElementById("divOrderId");
    const divDelivery = document.getElementById("divDelivery");
    const divJobId = document.getElementById("divJobId");
    const divJobDate = document.getElementById("divJobDate");
    const divShipmentId = document.getElementById("divShipmentId");
    const divShipping = document.getElementById("divShipping");

    formDetail.setAttribute("hidden", true);
    divCreatedBy.setAttribute("hidden", true);
    divCreatedDate.setAttribute("hidden", true);
    divOrderId.setAttribute("hidden", true);
    divDelivery.setAttribute("hidden", true);
    divJobId.setAttribute("hidden", true);
    divJobDate.setAttribute("hidden", true);
    divShipmentId.setAttribute("hidden", true);
    divShipping.setAttribute("hidden", true);

    const value = e.target.value;
    if (!value) return;

    await Promise.all([handlerSelCustomer(value, "#customer")]);

    formDetail.removeAttribute("hidden");
    if (value == "Blinds") {
      divCreatedBy.setAttribute("hidden", true);
      divCreatedDate.setAttribute("hidden", true);
      divDelivery.removeAttribute("hidden");
    } else if (value == "Panorama") {
      handlerSelUser(value, "#createdby");
      divCreatedBy.removeAttribute("hidden");
      divCreatedDate.removeAttribute("hidden");
      divDelivery.setAttribute("hidden", true);
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

// button submit
document.querySelector("#btn-submit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
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
    window.location.href = `/order/detail?ultron=${ID}&infinity=${ORDERTYPE.toLowerCase()}`;
  }

  if (ACTION == "edit" && ORDERTYPE == "panorama") {
    window.location.href = `/order/loop/detail?ultron=${ID}&infinity=${ORDERTYPE.toLowerCase()}`;
  }
});

// ==============================================|| FUNCTIONS ||=============================================
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
      "designid",
      "blindid",
      "data-table_length",
    ];

    formObject = Object.fromEntries(
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key))
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
          : "Something went wrong, please try again!"
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
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${response.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerSelCustomer"
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
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
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message || error
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const handlerSelUser = async (ordertype, params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!ordertype) return;

  try {
    const response = await fetch(URIMETHOD + "/BindUser", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      //   body: JSON.stringify({ ordertype: ordertype, rolename: ROLENAME }),
    });

    if (!response.ok) {
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${response.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerSelUser"
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
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
      const msg =
        ROLENAME === "Administrator"
          ? `${res.status} - ${res.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerEdit"
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    for (const item of data) {
      await handlerSelCustomer(item.OrderType, "#customer");
      await handlerSelUser(item.OrderType, "#createdby");
      await visibleElementForm(item);
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

    if (id === "createddate") {
      value = new Date(value).toLocaleDateString("en-CA");
    }

    el.value = value ?? "";
  });
};
// --------------------------------------------||Other Functions ||-------------------------------------------
const checkSessionCreateHeader = async () => {
  if (!ACTION) window.location.href = "/order";

  if (ACTION === "add") {
    await Promise.all([visibleElementForm()]);
    loaderFadeOut();
  } else if (ACTION === "edit" && ID && ORDERTYPE) {
    handlerEdit(ID, ORDERTYPE);
    // loaderFadeOut();
  } else {
    window.location.href = "/order";
  }
};

const visibleElementForm = (item) => {
  const divOrderType = document.getElementById("divOrderType");
  const formDetail = document.getElementById("formDetail");
  const divCreatedBy = document.getElementById("divCreatedBy");
  const divCreatedDate = document.getElementById("divCreatedDate");
  const divOrderId = document.getElementById("divOrderId");
  const divDelivery = document.getElementById("divDelivery");
  const divJobId = document.getElementById("divJobId");
  const divJobDate = document.getElementById("divJobDate");
  const divShipmentId = document.getElementById("divShipmentId");
  const divShipping = document.getElementById("divShipping");

  const createddate = (document.getElementById("createddate").value =
    new Date().toLocaleDateString("en-CA"));

  formDetail.setAttribute("hidden", true);
  divCreatedBy.setAttribute("hidden", true);
  divCreatedDate.setAttribute("hidden", true);
  divOrderId.setAttribute("hidden", true);
  divDelivery.setAttribute("hidden", true);
  divJobId.setAttribute("hidden", true);
  divJobDate.setAttribute("hidden", true);
  divShipmentId.setAttribute("hidden", true);
  divShipping.setAttribute("hidden", true);

  if (!item) return;

  divOrderType.setAttribute("hidden", true); // edit
  formDetail.removeAttribute("hidden");
  if (item.OrderType == "Blinds") {
    divCreatedBy.setAttribute("hidden", true);
    divCreatedDate.setAttribute("hidden", true);
    divDelivery.removeAttribute("hidden");
  }

  if (item.OrderType == "Panorama") {
    divCreatedBy.removeAttribute("hidden");
    divCreatedDate.removeAttribute("hidden");
    divDelivery.setAttribute("hidden", true);
  }
};
