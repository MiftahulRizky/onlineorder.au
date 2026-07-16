document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("CellularBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  cellularBlindPageLoaded();
});

// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindBrackets(DESIGNID, blindtype);
    }

    if (e.target.id === "brackettype") {
      const blindtype = document.getElementById("blindtype").value;
      const brackettype = e.target.value;
      await handlerElementVisibility(blindtype, brackettype);
      await bindControls(DESIGNID, blindtype, brackettype);
    }

    if (e.target.id === "controltype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.options[blinds.selectedIndex].text;
      const brackettype = document.getElementById("brackettype").value;
      const controltype = e.target.value;
      await handlerElementVisibility(blindtype, brackettype, controltype);
      await bindFabrics(DESIGNID, blindtype, brackettype, controltype);
      await bindFabrics2(DESIGNID, blindtype, brackettype, controltype);
      await bindControlSystem(controltype);
      await Promise.all([
        bindMounting(),
        bindCordType(),
        bindControlPosition(),
        bindMotorType(),
        bindMotorExtra(),
        bindHoldDown(),
        bindCutOut(),
        bindAdditional(),
      ]);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    if (e.target.id === "fabrictype2") {
      const fabrictype = e.target.value;
      await bindFabricColours2(DESIGNID, fabrictype);
    }

    if (e.target.id === "controlsystem") {
      const controlsystem = e.target.tomselect.getValue();

      const divMotor = document.getElementById("divMotor");
      divMotor.classList.add("d-none");
      if (controlsystem.includes("Motorised")) {
        divMotor.classList.remove("d-none");
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

const bindBlinds = async () => {
  if (!DESIGNID) return;

  await bindSelect({
    elementId: "blindtype",
    field: "blindtype",
    params: { designid: DESIGNID },
  });
};

const bindBrackets = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "brackettype",
    field: "brackettype",
    params: { designid, blindtype },
    withDefaultOption: true,
    lengthDefaultOption: 1,

    onSingle: async (item, select) => {
      const brackettype = item.value;

      await handlerElementVisibility(blindtype, brackettype);
      await bindControls(designid, blindtype, brackettype);
    },
  });
};

const bindControls = async (designid, blindtype, brackettype) => {
  if (!designid || !blindtype || !brackettype) return;

  await bindSelect({
    elementId: "controltype",
    field: "controltype",
    params: { designid, blindtype, brackettype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const controltype = item.value;

    // },
  });
};

const bindMounting = () => {
  generateOption("mounting", ["Reveal Fit", "Face Fit", "Make Size"]);
};

const bindFabrics = async (designid, blindtype, brackettype, controltype) => {
  document.getElementById("fabriccolour").innerHTML = "";
  if (!designid || !blindtype || !brackettype || !controltype) return;

  await bindSelect({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid, blindtype, brackettype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const fabrictype = item.value;

    // },
  });
};

const bindFabrics2 = async (designid, blindtype, brackettype, controltype) => {
  document.getElementById("fabriccolour2").innerHTML = "";
  if (!designid || !blindtype || !brackettype || !controltype) return;

  await bindSelect({
    elementId: "fabrictype2",
    field: "fabrictype2",
    params: { designid, blindtype, brackettype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const fabrictype2 = item.value;

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

const bindFabricColours2 = async (designid, fabrictype) => {
  if (!designid || !fabrictype) return;

  await bindSelect({
    elementId: "fabriccolour2",
    field: "fabriccolour2",
    params: { designid, fabrictype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const fabriccolour2 = item.value;

    // },
  });
};

const bindControlSystem = async (controltype) => {
  if (!controltype) return;

  if (!controlSystemTS) {
    tomSelectPlug("controlsystem");
  }

  controlSystemTS.clear(); // clear selected
  controlSystemTS.clearOptions(); // clear dropdown

  await bindSelect({
    elementId: "controlsystem",
    field: "controlsystem",
    params: { controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const controlsystem = item.value;

    // },
  });
};

const bindCordType = () => {
  generateOption("cordtype", ["Standard Cord", "Continous Cord"]);
};

const bindControlPosition = () => {
  generateOption("controlposition", ["L", "R", "LR"]);
};

const bindMotorType = () => {
  generateOption("motortype", [
    "STD 36W",
    "STD Rechargable",
    "DBU 36W",
    "TDBU Rechargable",
    "D&N 36W",
    "D&N Rechargable",
  ]);
};

const bindMotorExtra = () => {
  generateOption("motorextra", [
    "36W Adapter",
    "Ext. Cable for PowerBar",
    "Corded PowerBar",
    "Cordess PowerBar",
    "Ext.Rod-910mm",
    "Remote With Holder",
    "Additional Remote Holder",
    "Additional Remote Holder",
    "G2 SmartDial Remote",
    "G2 SmartDial Colour Ring",
    "G2 ShadeAuto Hub",
    "Repeater",
  ]);
};

const bindHoldDown = () => {
  generateOption("holddown", ["No", "Yes"], 2);
};

const bindCutOut = () => {
  let list = [];
  for (let i = 1; i <= 4; i++) {
    list.push(i.toString());
  }
  generateOption("cutout", list);
};

const bindAdditional = () => {
  generateOption("additional", [
    "Dual Shade (2 on 1)",
    "Decoflex Fram Colour",
    "Pre-drilled Frames",
    "Specialty shapes",
  ]);
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
      await bindBrackets(item.DesignId, item.BlindId);
      await bindControls(item.DesignId, item.BlindId, item.BracketType);
      await handlerElementVisibility(
        item.BlindId,
        item.BracketType,
        item.KitId,
        item,
      );
      await bindFabrics(
        item.DesignId,
        item.BlindId,
        item.BracketType,
        item.KitId,
      );
      await bindFabricColours(item.DesignId, item.FabricType);
      await bindFabrics2(
        item.DesignId,
        item.BlindId,
        item.BracketType,
        item.KitId,
      );
      await bindFabricColours2(item.DesignId, item.FabricTypeB);
      await bindControlSystem(item.KitId);
      await Promise.all([
        bindMounting(),
        bindCordType(),
        bindControlPosition(),
        bindMotorType(),
        bindMotorExtra(),
        bindHoldDown(),
        bindCutOut(),
        bindAdditional(),
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
const handlerElementVisibility = async (
  blindtype,
  brackettype,
  controltype,
  item,
) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divBracketType = document.getElementById("divBracketType");
    const lblBracketType = document.getElementById("lblBracketType");
    const divControlType = document.getElementById("divControlType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divFabricDay = document.getElementById("divFabricDay");
    const lblFabricDay = document.getElementById("lblFabricDay");
    const divFabricNight = document.getElementById("divFabricNight");
    const lblFabricNight = document.getElementById("lblFabricNight");
    const divControlSystem = document.getElementById("divControlSystem");
    const divCordType = document.getElementById("divCordType");
    const divMotor = document.getElementById("divMotor");
    const divAdditional = document.getElementById("divAdditional");

    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divBracketType.classList.add("d-none");
    lblBracketType.innerHTML = "cell type";
    divControlType.classList.add("d-none");
    lblControlType.innerHTML = "control type";
    divFormDetail.classList.add("d-none");
    divFabricDay.classList.add("d-none");
    lblFabricDay.innerHTML = "fabric type x colour";
    divFabricNight.classList.add("d-none");
    lblFabricNight.innerHTML = "fabric type x colour";
    divControlSystem.classList.add("d-none");
    divCordType.classList.add("d-none");
    divMotor.classList.add("d-none");
    divAdditional.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    if (!["Cellora"].includes(blindname)) {
      divBracketType.classList.remove("d-none");
    }

    if (!brackettype) return;
    divControlType.classList.remove("d-none");

    if (["Potrait"].includes(blindname)) {
      lblControlType.innerHTML = "system type";
    }

    if (!controltype) return;
    const controlname = await getItemData(
      `SELECT ControlType FROM HardwareKits WHERE Id = '${controltype}'`,
    );

    divFormDetail.classList.remove("d-none");

    if (["Cellora"].includes(blindname)) {
      divFabricDay.classList.remove("d-none");
    }

    if (["Galaxy"].includes(blindname)) {
      divFabricDay.classList.remove("d-none");
      if (controlname.includes("DN")) {
        divFabricNight.classList.remove("d-none");
        lblFabricDay.innerHTML = "fabric day";
        lblFabricNight.innerHTML = "fabric night";
      }

      if (controlname.includes("Corded")) {
        divCordType.classList.remove("d-none");
      }
    }

    if (["Potrait"].includes(blindname)) {
      divFabricDay.classList.remove("d-none");
      divControlSystem.classList.remove("d-none");
      divAdditional.classList.remove("d-none");
    }

    if (item) {
      if (item.HangerType.includes("Motorised")) {
        divMotor.classList.remove("d-none");
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
      "brackettype",
      "controltype",
      "qty",
      "room",
      "mounting",
      "width",
      "drop",
      "fabrictype",
      "fabriccolour",
      "fabrictype2",
      "fabriccolour2",
      "controlsystem",
      "cordtype",
      "controlposition",
      "chainlength",
      "motortype",
      "motorextra",
      "holddown",
      "cutout",
      "additional",
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
      if (field === "controlsystem") {
        formData[field] = controlSystemTS
          ? controlSystemTS.getValue() // array
          : [];
      } else {
        const el = document.getElementById(field);
        formData[field] = el ? el.value : "";
      }
    });

    // swal.close();
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
    brackettype: "BracketType",
    controltype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    fabrictype2: "FabricTypeB",
    fabriccolour2: "FabricIdB",
    width: "Width",
    drop: "Drop",
    cordtype: "MaterialCord",
    controlposition: "ControlPosition",
    chainlength: "ChainLength",
    motortype: "MotorStyle",
    motorextra: "AdditionalMotor",
    holddown: "BottomHoldDown",
    cutout: "DoorCutOut",
    additional: "Accessory",
    notes: "Notes",
    markup: "MarkUp",
  };

  // 1. set normal fields
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    if (id === "controlsystem") {
      const values = value ? value.split(",") : [];

      if (el.tomselect) {
        el.tomselect.setValue(values); // 🔥 penting
      } else {
        el.value = values;
      }

      return;
    }

    el.value = value ?? "";

    if (el.value === "0") el.value = "";
  });

  const controlSystemEl = document.getElementById("controlsystem");

  if (controlSystemEl?.tomselect) {
    let csValue = itemData["HangerType"];

    // normalisasi → array
    if (typeof csValue === "string") {
      csValue = csValue
        .split(",")
        .map((v) => v.trim())
        .filter(Boolean);
    }

    if (Array.isArray(csValue)) {
      // pastikan option sudah ada
      controlSystemEl.tomselect.clear();
      controlSystemEl.tomselect.setValue(csValue, true);
    }
  }

  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = document.getElementById("notescount");
  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
  }
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const cellularBlindPageLoaded = async () => {
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

    if (elementId == "controlsystem") {
      controlSystemTS.addOptions(data);
      controlSystemTS.refreshOptions(false);
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

let controlSystemTS;
const tomSelectPlug = (param) => {
  const el = document.getElementById(param);
  if (!el) return;
  controlSystemTS = new TomSelect(el, {
    copyClassesToDropdown: false,
    dropdownParent: "body",
    controlInput: "<input>",
    render: {
      item: function (data, escape) {
        if (data.customProperties) {
          return (
            '<div><span class="dropdown-item-indicator">' +
            data.customProperties +
            "</span>" +
            escape(data.text.toUpperCase()) +
            "</div>"
          );
        }
        return "<div>" + escape(data.text.toUpperCase()) + "</div>";
      },
      option: function (data, escape) {
        if (data.customProperties) {
          return (
            '<div><span class="dropdown-item-indicator">' +
            data.customProperties +
            "</span>" +
            escape(data.text.toUpperCase()) +
            "</div>"
          );
        }
        return "<div>" + escape(data.text.toUpperCase()) + "</div>";
      },
    },
  });
};
