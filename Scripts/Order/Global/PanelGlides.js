document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Global PG.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  pgGlobalPageLoaded();
});

// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindColours(DESIGNID, blindtype);
    }

    if (e.target.id === "colourtype") {
      const blindtype = document.getElementById("blindtype").value;
      const colourtype = e.target.value;
      await handlerElementVisibility(blindtype, colourtype);
      await bindFabrics(DESIGNID, blindtype);
      await Promise.all([
        bindMounting(),
        bindLayoutCode(),
        bindNoPanel(),
        bindTrackType(),
        bindTrackColour(),
        bindWandPosition(),
        bindWandColour(),
        bindBottomRail(),
        bindBattenColour(),
        bindFitting(),
      ]);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    if (e.target.id === "tracktype") {
      const tracktype = e.target.value;
      document.getElementById("trackcolour").value = "";
      await bindTrackColour(tracktype);
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

const bindBlinds = async (designid) => {
  if (!designid) return;

  await bindSelect({
    elementId: "blindtype",
    field: "blindtype",
    params: { designid },
    withDefaultOption: true,
  });
};

const bindColours = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "colourtype",
    field: "colourtype",
    params: { designid, blindtype },
    withDefaultOption: true,
  });
};

const bindMounting = () => {
  generateOption("mounting", ["Make Size", "Face Fit", "Reveal fit"]);
};

const bindFabrics = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid, blindtype },
    withDefaultOption: true,
  });
};

const bindFabricColours = async (designid, fabrictype) => {
  if (!designid || !fabrictype) return;

  await bindSelect({
    elementId: "fabriccolour",
    field: "fabriccolour",
    params: { designid, fabrictype },
    withDefaultOption: true,
  });
};

const bindLayoutCode = () => {
  generateOption("layoutcode", ["A", "B", "C", "D", "E", "F"]);
};

const bindNoPanel = () => {
  let list = [];

  for (let i = 2; i <= 9; i++) {
    list.push(i.toString());
  }

  generateOption("nopanel", list);
};

const bindTrackType = () => {
  generateOption("tracktype", [
    "2 Channel Track",
    "3 Channel Track",
    "4 Channel Track",
    "5 Channel Track",
    "6 Channel Track",
  ]);
};

const bindTrackColour = (tracktype) => {
  if (!tracktype) return;
  generateOption("trackcolour", ["Black", "Grey", "White"]);
};

const bindWandPosition = () => {
  generateOption("wandposition", ["Back", "Front"]);
};

const bindWandColour = () => {
  generateOption("wandcolour", ["Black", "Grey", "White"]);
};

const bindBottomRail = () => {
  generateOption("bottomrail", ["Standard (Plain Pocket)", "Fabric Covered"]);
};

const bindBattenColour = () => {
  generateOption("battencolour", [
    "Aluminium",
    "Timber - Alabaster",
    "Timber - Batlic",
    "Timber - Black",
    "Timber - Brown",
    "Timber - Cherry",
    "Timber - Natural",
    "Timber - Teak",
    "Timber - White",
  ]);
};

const bindFitting = () => {
  generateOption("fitting", ["Face", "Reveal"]);
};
// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, colourtype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divColourType = document.getElementById("divColourType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divBatten = document.getElementById("divBatten");
    const divBattenColour = document.getElementById("divBattenColour");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divColourType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divBatten.classList.add("d-none");
    divBattenColour.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divColourType.classList.remove("d-none");

    if (!colourtype) return;
    divFormDetail.classList.remove("d-none");

    if (["Plantation", "Sewless"].includes(blindname)) {
      divBatten.classList.remove("d-none");
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
      "blindtype", // as Kit Id
      "colourtype", // as Kit Id
      "qty", // as Qty
      "room", // as Location
      "mounting", // as Mounting
      "fabrictype", // as FabricId
      "fabriccolour", // as FabricId
      "width", // as Width
      "drop", // as Drop
      "layoutcode", // as LayoutCode
      "nopanel", // as New NoPanel
      "tracktype", // as TrackType
      "trackcolour", // as TrackColour
      "wandposition", // as New WandPosition
      "wandlength", // as WandLength
      "wandcolour", // as WandColour
      "bottomrail", // as BottomHoldDown
      "batten", // as New Batten
      "battencolour", // as New BattenColour
      "fitting", // as New Fitting
      "notes", // as Notes
      "markup", // as Markup
    ];

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
      rolename: ROLENAME,
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
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    width: "Width",
    drop: "Drop",
    layoutcode: "Layout",
    nopanel: "NumOfPanel",
    tracktype: "TrackType",
    trackcolour: "TrackColour",
    wandposition: "WandPosition",
    wandlength: "WandLength",
    wandcolour: "WandColour",
    bottomrail: "BottomHoldDown",
    batten: "Batten",
    battencolour: "BattenColour",
    fitting: "Fitting",
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
      await bindColours(item.DesignId, item.BlindId);
      await handlerElementVisibility(item.BlindId, item.KitId, item);
      await bindFabrics(item.DesignId, item.BlindId);
      await bindFabricColours(item.DesignId, item.FabricType);
      await Promise.all([
        bindMounting(),
        bindLayoutCode(),
        bindNoPanel(),
        bindTrackType(),
        bindWandPosition(),
        bindWandColour(),
        bindBottomRail(),
        bindBattenColour(),
        bindFitting(),
      ]);
      await Promise.all([handlerSetElementValues(item)]);
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const pgGlobalPageLoaded = async () => {
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
  await bindFormAction(ITEMACTION, ITEMID);

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
