document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("RomanBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  romanPageLoaded();
});

// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindControls(DESIGNID, blindtype);
    }

    if (e.target.id === "controltype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controltype = e.target.value;
      await handlerElementVisibility(blindtype, controltype);
      await bindFabrics(DESIGNID, blindtype, controltype);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
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
});

document.querySelector("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  // handlerSubmit(e.target.form, e.target.id);
  // handlerSubmit(e.target.id);
});

document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});

// ============================================================FUNCTIONS========================================================================
// ----------------------------------------------|| Binding Functions ||---------------------------------------
const bindDesigns = async (designid) => {
  try {
    const Name = await getItemData(
      `SELECT Name FROM Designs WHERE Id = '${designid}'`,
    );

    document.getElementById("pageTitle").innerHTML = Name;
    document.getElementById("pageAction").innerHTML = ITEMACTION;
  } catch (error) {
    console.error(error.message);
  }
};

const bindHeaders = async (headerid) => {
  try {
    const OrderId = await getItemData(
      `SELECT OrderId FROM view_order_headers WHERE OrderType IN ('Blinds', 'Door and Window') AND Id = '${headerid}'`,
    );
    const OrderNumber = await getItemData(
      `SELECT OrderNumber FROM view_order_headers WHERE OrderType IN ('Blinds', 'Door and Window') AND Id = '${headerid}'`,
    );
    const OrderName = await getItemData(
      `SELECT OrderName FROM view_order_headers WHERE OrderType IN ('Blinds', 'Door and Window') AND Id = '${headerid}'`,
    );

    const lblOrder = document.getElementById("lblOrder");
    const lblItemId = document.getElementById("lblItemId");
    const lblOrderNumber = document.getElementById("lblOrderNumber");
    const lblOrderName = document.getElementById("lblOrderName");

    lblOrder.innerHTML = OrderId;
    lblOrder.classList.add("fw-bold");

    lblItemId.innerHTML = ITEMID;
    lblItemId.classList.add("fw-bold");

    lblOrderNumber.innerHTML = OrderNumber;
    lblOrderNumber.classList.add("fw-bold");

    lblOrderName.innerHTML = OrderName;
    lblOrderName.classList.add("fw-bold");
  } catch (error) {
    console.error(error.message);
  }
};

const bindFormAction = (itemaction, id) => {
  const cardTitle = document.getElementById("cardTitle");
  const actionMap = {
    AddItem: "ADD ITEM",
    NextItem: "NEXT ITEM",
    EditItem: "EDIT ITEM ID: " + id,
    ViewItem: "VIEW ITEM ID: " + id,
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemaction] || "";
};

const bindBlinds = async () => {
  if (!DESIGNID) return;

  await bindSelect({
    elementId: "blindtype",
    field: "blindtype",
    params: { designid: DESIGNID },
  });
};

const bindControls = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "controltype",
    field: "controltype",
    params: { designid, blindtype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const controltype = item.value;

    //   // await handlerElementVisibility(blindtype, controltype);
    //   // await bindControls(designid, blindtype, controltype);
    // },
  });
};

const bindMounting = () => {
  generateOption("mounting", ["Make Size", "Face Fit", "Reveal Fit"]);
};

const bindFabrics = async (designid, blindtype, controltype) => {
  document.getElementById("fabriccolour").innerHTML = "";
  if (!designid || !blindtype || !controltype) return;

  await bindSelect({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid, blindtype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const fabrictype = item.value;

    // },
  });
};

const bindFabricColours = async (designid, fabrictype) => {
  if (!designid || !fabrictype) return;

  await bindSelect({
    elementId: "fabriccolour",
    field: "fabriccolour",
    params: { designid, fabrictype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const fabriccolour = item.value;

    // },
  });
};

// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, controltype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divControlType = document.getElementById("divControlType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divChained = document.getElementById("divChained");
    const divCordlock = document.getElementById("divCordlock");
    const divBattenColour = document.getElementById("divBattenColour");
    const divPlasticColour = document.getElementById("divPlasticColour");
    const divCleat = document.getElementById("divCleat");

    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divControlType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divChained.classList.add("d-none");
    divCordlock.classList.add("d-none");
    divBattenColour.classList.add("d-none");
    divPlasticColour.classList.add("d-none");
    divCleat.classList.add("d-none");

    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divControlType.classList.remove("d-none");

    if (!controltype) return;
    const controlname = await getItemData(
      `SELECT ControlType FROM HardwareKits WHERE Id = '${controltype}'`,
    );
    divFormDetail.classList.remove("d-none");

    if (["Classic"].includes(blindname)) {
      if (["Cord"].includes(controlname)) {
        divCordlock.classList.remove("d-none");
        divPlasticColour.classList.remove("d-none");
        divCleat.classList.remove("d-none");
      }
      if (["Chain"].includes(controlname)) {
        divChained.classList.remove("d-none");
      }
    }

    if (["Plantation"].includes(blindname)) {
      if (["Cord"].includes(controlname)) {
        divCordlock.classList.remove("d-none");
        divPlasticColour.classList.remove("d-none");
        divCleat.classList.remove("d-none");
      }
      if (["Chain"].includes(controlname)) {
        divChained.classList.remove("d-none");
      }
      divBattenColour.classList.remove("d-none");
    }

    if (["Sewless"].includes(blindname)) {
      if (["Cord"].includes(controlname)) {
        divCordlock.classList.remove("d-none");
        divPlasticColour.classList.remove("d-none");
        divCleat.classList.remove("d-none");
      }
      if (["Chain"].includes(controlname)) {
        divChained.classList.remove("d-none");
      }
      divBattenColour.classList.remove("d-none");
    }

    if (MARKUPACCESS === "True") divMarkUp.classList.remove("d-none");

    if (["AddItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
      btnSubmit.classList.remove("d-none");
    } else if (ITEMACTION === "ViewItem") {
      btnSubmit.classList.remove("d-none");
      if (ROLENAME !== "Administrator") btnSubmit.classList.add("d-none");
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const romanPageLoaded = async () => {
  if (!HEADERID) {
    window.location.href = "/order";
    return;
  }

  if (!ORDERTYPE) {
    window.location.href = "/order";
    return;
  }

  if (!ITEMACTION || !DESIGNID) {
    window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    return;
  }

  if (DESIGNID.toUpperCase() !== DESIGNIDORI) {
    window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    return;
  }

  await bindDesigns(DESIGNID);
  await bindHeaders(HEADERID);
  bindFormAction(ITEMACTION, ITEMID);

  if (ITEMACTION === "AddItem") {
    await bindBlinds(DESIGNID);
    handlerElementVisibility();
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    // await bindItemOrders(ITEMID);
    loaderFadeOut();
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

const bindSelect = async ({
  elementId,
  field,
  params = {},
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  const select = document.getElementById(elementId);
  select.innerHTML = "";

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
    const data = result.d;

    if (!Array.isArray(data)) {
      throw new Error(`No data returned from server : ${field}`);
    }

    select.innerHTML = "";

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
      select.add(option);
    });

    select.classList.add("fw-bold");

    // callback setelah render
    if (afterRender) {
      await afterRender(data, select);
    }

    // kalau cuma 1 data
    if (data.length === 1 && onSingle) {
      select.selectedIndex = 0;
      await onSingle(data[0], select);
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  list.sort();

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
