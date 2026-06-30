document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("SupplyOnly.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  supplyOnlyPageLoaded();
});
// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const tubetype = e.target.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubename = e.target.selectedOptions[0].dataset.name;
      await handlerElementVisibility(blindtype, tubetype);
      await Promise.all([
        bindSize(),
        bindWidth(tubename),
        bindColour(blindname),
      ]);
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
  handlerSubmit(e.target.id);
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
      `SELECT OrderId FROM view_order_headers WHERE Id = '${headerid}'`,
    );
    const OrderNumber = await getItemData(
      `SELECT OrderNumber FROM view_order_headers WHERE Id = '${headerid}'`,
    );
    const OrderName = await getItemData(
      `SELECT OrderName FROM view_order_headers WHERE Id = '${headerid}'`,
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

const bindTubes = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "tubetype",
    field: "tubetype",
    params: { designid, blindtype },
    withDefaultOption: true,

    onSingle: async (item, select) => {
      const tubetype = item.value;

      // await handlerElementVisibility(blindtype, tubetype);
      // await bindControls(designid, blindtype, tubetype);
    },
  });
};

const bindSize = () => {
  generateOption("size", [
    "750×2000",
    "750×2400",
    "950×2000",
    "950×2400",
    "1200×2000",
    "1200×2400",
  ]);
};

const bindWidth = (tubename) => {
  if (!tubename) return;
  let list = [];
  if (["Fibreglass Mesh"].includes(tubename)) {
    list.push("810", "1220", "1520", "1830");
  }
  if (["Aluminium Mesh"].includes(tubename)) {
    list.push("810", "1220", "1500");
  }
  if (["Paw Proof Mesh"].includes(tubename)) {
    list.push("910", "1220", "1520");
  }
  if (["Stainless Steel Mesh"].includes(tubename)) {
    list.push("810", "1220", "1520");
  }
  generateOption("widthselect", list);
};

const bindColour = (blindname) => {
  if (!blindname) return;
  let list = [];
  if (["Mesh Only"].includes(blindname)) {
    list.push("black");
  }
  if (["Frame Only"].includes(blindname)) {
    list.push(
      "Apo Grey",
      "Beige",
      "Birch White",
      "Black",
      "Bronze",
      "Dune",
      "Monument",
      "Primrose",
      "Silver",
      "Surf Mist",
      "White",
      "Woodland Grey",
    );
  }
  generateOption("colour", list);
};

const bindItemOrders = async (itemid) => {
  try {
    if (!itemid) return;

    const res = await fetch(`${URIMETHOD}/BindItemOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ itemid }),
    });

    if (!res.ok) {
      const msg =
        ROLENAME === "Administrator"
          ? `${res.status} - ${res.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      throw isError("No data returned from server : bindItemOrders");
    }

    for (const item of data) {
      await bindBlinds(item.DesignId);
      await bindTubes(item.DesignId, item.BlindId);
      await handlerElementVisibility(item.BlindId, item.KitId, item);
      await Promise.all([
        bindSize(),
        bindWidth(item.TubeType),
        bindColour(item.BlindName),
      ]);
      await Promise.all([handlerSetElementValues(item)]);
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};
// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, tubetype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const lblTubeType = document.getElementById("lblTubeType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divRoom = document.getElementById("divRoom");
    const divSize = document.getElementById("divSize");
    const divWidthInput = document.getElementById("divWidthInput");
    const divWidthSelect = document.getElementById("divWidthSelect");
    const divDrop = document.getElementById("divDrop");
    const divLength = document.getElementById("divLength");
    const divColour = document.getElementById("divColour");
    const divCutOut = document.getElementById("divCutOut");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divTubeType.classList.add("d-none");
    lblTubeType.innerHTML = "product type";
    divFormDetail.classList.add("d-none");
    divRoom.classList.add("d-none");
    divSize.classList.add("d-none");
    divWidthInput.classList.add("d-none");
    divWidthSelect.classList.add("d-none");
    divDrop.classList.add("d-none");
    divLength.classList.add("d-none");
    divColour.classList.add("d-none");
    divCutOut.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divTubeType.classList.remove("d-none");
    if (blindname.includes("Mesh")) {
      lblTubeType.innerHTML = "MESH TYPE";
    }
    if (blindname.includes("Frame")) {
      lblTubeType.innerHTML = "FRAME TYPE";
    }

    if (!tubetype) return;
    const tubename = await getItemData(
      `SELECT TubeType FROM HardwareKits WHERE Id = '${tubetype}'`,
    );
    divFormDetail.classList.remove("d-none");

    if (
      [
        "Fibreglass Mesh",
        "Aluminium Mesh",
        "Stainless Steel Mesh",
        "Paw Proof Mesh",
      ].includes(tubename)
    ) {
      divWidthSelect.classList.remove("d-none");
      divLength.classList.remove("d-none");
      divColour.classList.remove("d-none");
    }

    if (["Ultra Barrier Mesh"].includes(tubename)) {
      divSize.classList.remove("d-none");
      divWidthInput.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divColour.classList.remove("d-none");
    }

    if (["Frame Only"].includes(blindname)) {
      divWidthInput.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divColour.classList.remove("d-none");
      divCutOut.classList.remove("d-none");
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

const handlerSubmit = async (button) => {
  try {
    // return alert(button);
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = [
      "blindtype",
      "tubetype",
      "qty",
      "room",
      "size",
      "widthinput",
      "widthselect",
      "drop",
      "length",
      "colour",
      "cutout",
      "notes",
      "markup",
    ];

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
    };

    fields.forEach((field) => {
      formData[field] = document.getElementById(field).value;
    });

    // return console.table(formData);

    const response = await fetch(URIMETHOD + "/Submit", {
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

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isWarning(dataResult.error.message?.toUpperCase());
      const field = document.getElementById(dataResult.error.field);
      if (field) {
        // field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        // field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      await isSuccess(dataResult.success);
      window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    }
  } catch (error) {
    var msg = error.message;
    if (ROLENAME !== "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg);
  } finally {
    document.getElementById(button).innerHTML = "Submit";
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    blindtype: "BlindId",
    tubetype: "KitId",
    controltype: "KitId",
    qty: "Qty",
    room: "Location",
    length: "PanelSize",
    widthinput: "Width",
    widthselect: "Width",
    drop: "Drop",
    colour: "SwipelColour",
    cutout: "CutOut_LeftTop",
    trackless: "TilterPosition",
    notes: "Notes",
    markup: "MarkUp",
  };

  // 1. set normal fields
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    el.value = value ?? "";

    if (el.value === "0") el.value = "";
  });
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const supplyOnlyPageLoaded = async () => {
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
    await bindItemOrders(ITEMID);
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
    if (withDefaultOption && data.length > 0) {
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

const generateOption = (elementId, list = []) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  let validateLength = 1;
  switch (elementId) {
    case "trackless":
    case "frametype":
    case "handleheight":
      validateLength = 0;
      break;
  }

  // Short A-Z
  list.sort();

  // default option kalau lebih dari 1 data
  if (list.length > validateLength) {
    const defaultOption = new Option("", "");
    sel.add(defaultOption);
  }

  list.forEach((item) => {
    const option = new Option(item.toUpperCase(), item);
    option.setAttribute("data-name", item);
    sel.add(option);
  });
};
