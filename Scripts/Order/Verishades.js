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
  verishadesPageLoaded();
});

// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      const blindname = e.target.selectedOptions[0].dataset.name;
      await handlerElementVisibility(blindtype);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    if (e.target.id === "fabriccolour") {
      const fabriccolour = e.target.value;
      await bindTape(fabriccolour);
    }

    if (e.target.id === "tracktype") {
      const tracktype = e.target.value;
      document.getElementById("trackcolour").innerHTML = "";
      document.getElementById("bracket").innerHTML = "";
      await Promise.all([bindTrackColour(tracktype), bindBracket(tracktype)]);
    }

    if (e.target.id === "wandsize") {
      const wandsize = e.target.value;
      const divWandCustomSize = document.getElementById("divWandCustomSize");
      document.getElementById("wandcolour").innerHTML = "";
      divWandCustomSize.classList.add("d-none");
      if (["Custom"].includes(wandsize)) {
        divWandCustomSize.classList.remove("d-none");
      }
      bindWandColour(wandsize);
    }

    if (e.target.id === "sizetype") {
      const sizetype = e.target.value;
      const mounting = document.getElementById("mounting").value;
      const divDropFloor = document.getElementById("divDropFloor");
      divDropFloor.classList.add("d-none");
      if (sizetype == "Opening Size" && mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }
      bindDropFloor();
    }

    if (e.target.id === "mounting") {
      const sizetype = document.getElementById("sizetype").value;
      const mounting = e.target.value;

      const divDropFloor = document.getElementById("divDropFloor");
      divDropFloor.classList.add("d-none");
      if (sizetype == "Opening Size" && mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }
      bindDropFloor();
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

document.querySelectorAll(".form-check-input").forEach((el) => {
  el.addEventListener("click", (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id.endsWith("override")) {
      const targetId = e.target.id.replace("override", "");
      document.getElementById(targetId).value = "";
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

const bindBlinds = async (designid) => {
  if (!designid) return;

  await bindSelect({
    elementId: "blindtype",
    field: "blindtype",
    params: { designid },
    withDefaultOption: true,
  });
};

const bindTubes = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "tubetype",
    field: "tubetype",
    params: { designid, blindtype },
    withDefaultOption: false,

    onSingle: async (item, select) => {
      const blindname = await getItemData(
        `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
      );
      const tubetype = item.value;
      const fabriccolour = document.getElementById("fabriccolour").value;
      await handlerElementVisibility(blindtype, tubetype);
      await bindFabrics(DESIGNID);
      await bindTape(fabriccolour);
      await Promise.all([
        bindSizeType(),
        bindDropFloor(),
        bindStack(),
        bindTrackType(),
        bindWandSize(),
      ]);
    },
  });
};

const bindSizeType = () => {
  generateOption("sizetype", ["Opening Size", "Make Size"]);
};

const bindDropFloor = () => {
  generateOption("dropfloor", ["No", "Yes"]);
};

const bindFabrics = async (designid) => {
  if (!designid) return;

  await bindSelect({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid },
    withDefaultOption: true,

    onSingle: async (item, select) => {
      const fabrictype = item.value;

      // await handlerElementVisibility(fabrictype, tubetype);
      // await bindControls(designid, fabrictype, tubetype);
    },
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

const bindStack = () => {
  let list = [];
  list.push("Left", "Right", "Centre Stack", "Centre Split");
  generateOption("stack", list);
};

const bindTrackType = () => {
  let list = [];
  list.push("Cube", "Decorative (Flat)", "Decorative (Round)", "Standard");
  generateOption("tracktype", list);
};

const bindTrackColour = (type) => {
  if (!type) return;
  let list = [];
  list.push("Birch", "Black", "White");
  generateOption("trackcolour", list);
};

const bindWandSize = () => {
  let list = [];
  list.push("Custom", "500", "750", "1100", "1500", "2000");
  generateOption("wandsize", list);
};

const bindWandColour = (size) => {
  if (!size) return;
  let list = [];
  list.push("White");
  if (!["Custom"].includes(size)) {
    list.push("Birch", "Black");
  }
  generateOption("wandcolour", list);
};

const bindBracket = (track) => {
  if (!track) return;
  let list = [];

  switch (track) {
    case "Cube":
    case "Standard":
      list.push("Ceiling clip", "Ext Face", "Ext Rev", "L Bkt w Clip ff");
      break;
    case "Decorative (Flat)":
    case "Decorative (Round)":
      list.push("Deco Ceiling", "Deco Face fit");
      break;
  }

  generateOption("bracket", list);
};

const bindTape = async (fabricid) => {
  let list = [];

  const allColors = [
    "Antique White",
    "Black",
    "Charcoal",
    "Dark Grey",
    "Fawn",
    "Herringbone",
    "Indigo",
    "Ivory",
    "Pure White",
    "Shell",
    "Soft Grey",
    "Soft White",
    "Strike Grey",
  ];

  const fabricname = await getItemData(
    `SELECT Name FROM Fabrics WHERE Id = '${fabricid}'`,
  );

  let found = true;
  switch (fabricname) {
    case "Alpine White":
    case "Autumn White":
    case "Net White":
    case "Standard White":
    case "Eclipse Cotton":
    case "Verlux Cotton":
    case "Classic Snow":
    case "Classic Pearl":
      list.push("Pure White");
      break;

    case "Alpine Beige":
    case "Autumn Beige":
    case "Net Beige":
    case "Net Ivory":
    case "Standard Beige":
    case "Standard Ivory":
      list.push("Ivory");
      break;

    case "Alpine Charcoal":
    case "Autumn Charcoal":
    case "Net Charcoal":
    case "Standard Charcoal":
    case "Mist Black":
    case "Classic Onyx":
      list.push("Black");
      break;

    case "Alpine Light Grey":
    case "Autumn Light Grey":
    case "Mist Soft Grey":
    case "Net Soft White": // tetap sesuai mapping kamu
    case "Standard Soft White":
    case "Eclipse Soft Grey":
    case "Verlux Soft Grey":
      list.push("Soft Grey");
      break;

    case "Alpine Slate":
    case "Autumn Slate":
    case "Net Slate":
    case "Standard Slate":
    case "Mist Dark Grey":
    case "Eclipse Dark Grey":
    case "Eclipse Night":
    case "Classic Ash":
      list.push("Dark Grey");
      break;

    case "Alpine Soft White":
    case "Autumn Soft White":
    case "Net Soft White":
    case "Standard Soft White":
      list.push("Soft White");
      break;

    case "Mist Antique White":
    case "Verlux Flax":
    case "Verlux Lucent":
    case "Classic Sand":
      list.push("Antique White");
      break;

    case "Mist Fawn":
      list.push("Fawn");
      break;

    case "Mist Pure White":
      list.push("Pure White");
      break;

    case "Mist Shell":
    case "Verlux Sable":
    case "Classic Dusk":
      list.push("Shell");
      break;

    case "Eclipse Hygge":
    case "Verlux Strike Grey":
    case "Classic Thunder":
      list.push("Strike Grey");
      break;

    case "Eclipse Iron":
    case "Verlux Indigo":
      list.push("Indigo");
      break;

    case "Verlux Herringbone":
      list.push("Herringbone");
      break;

    default:
      // optional fallback
      found = false;
      break;
  }

  if (!found) {
    list.push(...allColors);
  }

  generateOption("tape", list);
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
      await bindFabrics(item.DesignId);
      await bindFabricColours(item.DesignId, item.FabricType);
      await bindTape(item.FabricId);
      await handlerElementVisibility(item.BlindId, item.TubeType, item);
      await Promise.all([
        bindSizeType(),
        bindDropFloor(),
        bindStack(),
        bindTrackType(),
        bindTrackColour(item.TrackType),
        bindBracket(item.TrackType),
        bindWandSize(),
        bindWandColour(item.WandLength),
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
    const divFormDetail = document.getElementById("divFormDetail");
    const divSizeType = document.getElementById("divSizeType");
    const divDropFloor = document.getElementById("divDropFloor");
    const lblWd = document.getElementById("lblWd");
    const divWidth = document.getElementById("divWidth");
    const divDrop = document.getElementById("divDrop");
    const divFabric = document.getElementById("divFabric");
    const divBlindSize = document.getElementById("divBlindSize");
    const divStack = document.getElementById("divStack");
    const divTrack = document.getElementById("divTrack");
    const divWand = document.getElementById("divWand");
    const divWandCustomSize = document.getElementById("divWandCustomSize");
    const divBracket = document.getElementById("divBracket");
    const divTape = document.getElementById("divTape");
    const divCarriers = document.getElementById("divCarriers");
    const divSpacer = document.getElementById("divSpacer");
    const divSlat = document.getElementById("divSlat");
    const divSlatQty = document.getElementById("divSlatQty");
    const divEndSlat = document.getElementById("divEndSlat");
    const divTotalSlat = document.getElementById("divTotalSlat");
    const divFabricQty = document.getElementById("divFabricQty");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divTubeType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divSizeType.classList.add("d-none");
    divDropFloor.classList.add("d-none");
    lblWd.innerHTML = "width x drop";
    divWidth.classList.add("d-none");
    divDrop.classList.add("d-none");
    divFabric.classList.add("d-none");
    divBlindSize.classList.add("d-none");
    divStack.classList.add("d-none");
    divTrack.classList.add("d-none");
    divWand.classList.add("d-none");
    divWandCustomSize.classList.add("d-none");
    divBracket.classList.add("d-none");
    divTape.classList.add("d-none");
    divCarriers.classList.add("d-none");
    divSpacer.classList.add("d-none");
    divSlat.classList.add("d-none");
    divSlatQty.classList.add("d-none");
    divEndSlat.classList.add("d-none");
    divTotalSlat.classList.add("d-none");
    divFabricQty.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );

    if (!tubetype) return;
    divFormDetail.classList.remove("d-none");

    if (["Single"].includes(blindname)) {
      lblWd.innerHTML = "width x drop";
      // divSizeType.classList.remove("d-none");
      divWidth.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divStack.classList.remove("d-none");
      divTrack.classList.remove("d-none");
      divWand.classList.remove("d-none");
      divBracket.classList.remove("d-none");
      divTape.classList.remove("d-none");
      divCarriers.classList.remove("d-none");
      divSpacer.classList.remove("d-none");
      divSlat.classList.remove("d-none");
      divSlatQty.classList.remove("d-none");
      divEndSlat.classList.remove("d-none");
      divTotalSlat.classList.remove("d-none");
      divFabricQty.classList.remove("d-none");
    }

    if (["Slat Only"].includes(blindname)) {
      lblWd.innerHTML = "drop";
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divBlindSize.classList.remove("d-none");
      divStack.classList.remove("d-none");
      divTrack.classList.remove("d-none");
      divTape.classList.remove("d-none");
      divCarriers.classList.remove("d-none");
      divSpacer.classList.remove("d-none");
      divSlat.classList.remove("d-none");
      divSlatQty.classList.remove("d-none");
      divEndSlat.classList.remove("d-none");
      divTotalSlat.classList.remove("d-none");
      divFabricQty.classList.remove("d-none");
    }

    if (["Track Only"].includes(blindname)) {
      lblWd.innerHTML = "width";
      divWidth.classList.remove("d-none");
      divStack.classList.remove("d-none");
      divTrack.classList.remove("d-none");
      divWand.classList.remove("d-none");
      divBracket.classList.remove("d-none");
      divTape.classList.remove("d-none");
      divCarriers.classList.remove("d-none");
      divSpacer.classList.remove("d-none");
    }

    if (item) {
      if (item.LouvreSize == "Opening Size" && item.Mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }

      if (!["500", "750", "1100", "1500", "2000"].includes(item.WandLength)) {
        divWandCustomSize.classList.remove("d-none");
      }
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
      "sizetype",
      "dropfloor",
      "mounting",
      "width",
      "drop",
      "fabrictype",
      "fabriccolour",
      "blindsize",
      "stack",
      "tracktype",
      "trackcolour",
      "wandsize",
      "wandcolour",
      "customsize",
      "bracket",
      "tape",
      "carrier",
      "carrieroverride",
      "spacer",
      "spaceroverride",
      "slat",
      "slatoverride",
      "slatqty",
      "slatqtyoverride",
      "endslats",
      "endslatsoverride",
      "totalslats",
      "fabricqty",
      "fabricqtyoverride",
      "notes",
      "markup",
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
      const el = document.getElementById(field);

      if (el.type === "checkbox") {
        formData[field] = el.checked; // true / false
      } else {
        formData[field] = el.value;
      }
    });

    // Swal.close();
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
    document.getElementById(button).innerHTML = "Save Changes";
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    blindtype: "BlindId",
    tubetype: "KitId",
    qty: "Qty",
    room: "Location",
    sizetype: "LouvreSize",
    dropfloor: "LouvrePosition",
    mounting: "Mounting",
    widthinput: "Width",
    width: "Width",
    drop: "Drop",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    blindsize: "BlindSize",
    stack: "StackPosition",
    tracktype: "TrackType",
    trackcolour: "TrackColour",
    wandsize: "WandLength",
    wandcolour: "WandColour",
    customsize: "WandLength",
    bracket: "BracketOption",
    tape: "BracketColour",
    totalslats: "SlatQty",
    notes: "Notes",
    markup: "MarkUp",
  };

  const jsonFields = [
    {
      inputId: "carrier",
      checkboxId: "carrieroverride",
      dataKey: "FrameType",
    },
    {
      inputId: "spacer",
      checkboxId: "spaceroverride",
      dataKey: "FrameLeft",
    },
    {
      inputId: "slat",
      checkboxId: "slatoverride",
      dataKey: "Layout",
    },
    {
      inputId: "slatqty",
      checkboxId: "slatqtyoverride",
      dataKey: "LayoutSpecial",
    },
    {
      inputId: "endslats",
      checkboxId: "endslatsoverride",
      dataKey: "SlatSize",
    },
    {
      inputId: "fabricqty",
      checkboxId: "fabricqtyoverride",
      dataKey: "TubeSize",
    },
    // tinggal tambah di sini nanti
  ];

  //Normal fields
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    if (
      ["wandsize"].includes(id) &&
      !["500", "750", "1100", "1500", "2000"].includes(value)
    ) {
      value = "Custom";
    }

    el.value = value ?? "";
    if (el.value === "0") el.value = "";
  });

  // JSON fields (reusable)
  jsonFields.forEach(({ inputId, checkboxId, dataKey }) => {
    setJsonField({
      inputId,
      checkboxId,
      rawValue: itemData[dataKey],
    });
  });

  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = document.getElementById("notescount");
  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
  }
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const verishadesPageLoaded = async () => {
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

const setJsonField = ({ inputId, checkboxId, rawValue }) => {
  const inputEl = document.getElementById(inputId);
  const checkboxEl = document.getElementById(checkboxId);

  if (!inputEl || !checkboxEl || !rawValue) return;

  try {
    const parsed = JSON.parse(rawValue);

    if (Array.isArray(parsed) && parsed.length > 0) {
      const { value, checked } = parsed[0];

      inputEl.value = value ?? "";
      checkboxEl.checked = checked ?? false;
    }
  } catch (err) {
    console.error(`JSON parse error (${inputId})`, err);
  }
};
