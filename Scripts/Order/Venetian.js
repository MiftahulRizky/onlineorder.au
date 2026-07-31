document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Venetian.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  venetianPageLoaded();
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
      const blindtype = document.getElementById("blindtype").value;
      const controltype = e.target.value;
      await handlerElementVisibility(blindtype, controltype);
      await bindColours(DESIGNID, blindtype, controltype);
    }

    if (e.target.id === "colourtype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controltype = document.getElementById("blindtype").value;
      const colourtype = e.target.value;
      await handlerElementVisibility(blindtype, controltype, colourtype);
      await Promise.all([
        bindNotes(blindname),
        bindSizeType(),
        bindDropFloor(),
        bindMounting(),
        bindControlPosition(),
        bindControlLift(),
        bindControlTilt(),
        bindBracket(blindname),
        bindBottomHoldDown(blindname),
        bindHoldDownBracket(blindname),
        bindPelmetType(blindname),
        bindPelmetSize(),
      ]);
    }

    if (e.target.id === "sizetype") {
      const sizetype = e.target.value;
      const divDropFloor = document.getElementById("divDropFloor");
      divDropFloor.classList.add("d-none");
      if (["Opening Size"].includes(sizetype)) {
        divDropFloor.classList.remove("d-none");
      }
      bindDropFloor();
    }
    if (e.target.id === "mounting") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const mounting = e.target.value;
      bindPelmetType(blindname, mounting);
    }

    if (e.target.id === "pelmettype") {
      const pelmettype = e.target.value;
      const divReturnLength = document.getElementById("divReturnLength");
      const divReturnLeft = document.getElementById("divReturnLeft");
      const divReturnRight = document.getElementById("divReturnRight");
      divReturnLength.classList.add("d-none");
      divReturnLeft.classList.add("d-none");
      divReturnRight.classList.add("d-none");

      if (pelmettype == "With Return") {
        divReturnLength.classList.remove("d-none");
        divReturnLeft.classList.remove("d-none");
        divReturnRight.classList.remove("d-none");
      }

      if (pelmettype == "Single Left Return") {
        divReturnLength.classList.remove("d-none");
        divReturnLeft.classList.remove("d-none");
      }

      if (pelmettype == "Single Right Return") {
        divReturnLength.classList.remove("d-none");
        divReturnRight.classList.remove("d-none");
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

document.querySelectorAll(".btn-information").forEach((el) => {
  el.addEventListener("click", async (e) => {
    const id = e.currentTarget.id;
    const blinds = document.getElementById("blindtype");
    const blindname = blinds.selectedOptions[0].dataset.name;
    let msg = "";

    switch (id) {
      case "btnInfoControlLength":
        msg =
          "If you leave this blank or 0, it will automatically follow the factory default (standard).";
        break;
      case "btnInfoPelmetWidth":
        msg = "If you leave this blank, it will use the factory default.";
        msg += "<br><br>";
        msg += "Our Standar Pelmet Width:";
        msg += "<br>";
        msg += "Reveal fit is width + 10";
        msg += "<br>";
        msg += "Face fit is width + 20";
        break;
      case "btnInfoReturnLength":
        let mm = "100mm";
        switch (blindname) {
          case "50mm Timberstyle":
          case "63mm Timberstyle":
            mm = "67mm";
            break;
          case "50mm Wooden":
          case "63mm Wooden":
            mm = "70mm";
            break;
          case "50mm Mockwood":
          case "63mm Mockwood":
            mm = "77mm";
            break;
        }
        msg = `If you leave this blank, it will use the factory default, which is ${mm} !`;
        break;
    }

    if (msg) {
      isInfo(msg);
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
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindControls = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindSelect({
    elementId: "controltype",
    field: "controltype",
    params: { designid, blindtype },
    withDefaultOption: true,
    lengthDefaultOption: 1,

    onSingle: async (item, select) => {
      const controltype = item.value;

      await handlerElementVisibility(blindtype, controltype);
      await bindColours(designid, blindtype, controltype);
    },
  });
};

const bindColours = async (designid, blindtype, controltype) => {
  if (!designid || !blindtype || !controltype) return;

  await bindSelect({
    elementId: "colourtype",
    field: "colourtype",
    params: { designid, blindtype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    // onSingle: async (item, select) => {
    //   const colourtype = item.value;

    //   await handlerElementVisibility(blindtype, controltype, colourtype);
    //   // await bindControls(designid, blindtype, controltype, colourtype);
    // },
  });
};

const bindNotes = (blindname) => {
  if (!blindname) return;

  const pNotes = document.getElementById("pNotes");
  pNotes.innerHTML = "";

  let text = `<h3>${blindname}</h3>`;
  text += "<br />";
  text += "- Minimum width available is 180mm";
  text += "<br />";
  text +=
    "- Width between 180mm & 260mm -> Tilter only in the middle and surcharge of $25";
  text += "<br />";
  text += "<br />";
  text +=
    "- Width between 261mm & 399mm -> Tilter and Cord lock are on opposite ends - Surcharge of $25 applies";

  pNotes.innerHTML = text;
};

const bindSizeType = () => {
  generateOption("sizetype", ["Opening Size", "Make Size"]);
};

const bindDropFloor = () => {
  generateOption("dropfloor", ["No", "Yes"]);
};

const bindMounting = () => {
  generateOption("mounting", ["Face Fit", "Reveal Fit"]);
};

const bindControlPosition = () => {
  generateOption("controlposition", ["LHC", "RHC"]);
};

const bindControlLift = () => {
  generateOption("controllift", ["Left", "Right", "Left Right"]);
};
const bindControlTilt = () => {
  generateOption("controltilt", ["Left", "Right", "Left Right"]);
};

const bindBracket = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["25mm Aluminium"].includes(blindname)) {
    list.push("Spring");
  }

  if (["50mm Aluminium"].includes(blindname)) {
    list.push("Spring", "End Mounting");
  }

  if (blindname.includes("Timberstyle")) {
    list.push("Spring", "End Mounting");
  }

  generateOption("bracket", list, 1);
};

const bindBottomHoldDown = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (
    ["50mm Aluminium"].includes(blindname) ||
    blindname.includes("Timberstyle")
  ) {
    list.push("Silver", "Gold");
  }

  if (
    blindname.includes("Mockwood") ||
    blindname.includes("Wooden") ||
    ["25mm Aluminium"].includes(blindname)
  ) {
    list.push("No", "Yes");
  }
  generateOption("bottom", list);
};

const bindHoldDownBracket = (blindname) => {
  if (!blindname) return;
  let list = [];
  if (
    blindname.includes("Mockwood") ||
    blindname.includes("Wooden") ||
    blindname.includes("Aluminium")
  ) {
    list.push("No", "Yes");
  }

  generateOption("holdbracket", list);
};

const bindPelmetType = (blindname, mounting) => {
  if (!blindname) return;
  let list = [];

  if (blindname.includes("Mockwood") || blindname.includes("Wooden")) {
    if (mounting == "Face Fit") {
      list.push("With Return");
    } else if (mounting == "Reveal Fit") {
      list.push("No Return");
    } else {
      list.push(
        "Bay Left",
        "Bay Right",
        "Main Bay",
        "Common (1 Pelmet Cover 2 or 3 blinds)",
        "Single Left Return",
        "Single Right Return",
      );
    }
  } else {
    list.push(
      "With Return",
      "No Return",
      "Bay Left",
      "Bay Right",
      "Main Bay",
      "Common (1 Pelmet Cover 2 or 3 blinds)",
      "Single Left Return",
      "Single Right Return",
    );
  }

  generateOption("pelmettype", list);
};

const bindPelmetSize = () => {
  generateOption("pelmetsize", ["63mm", "90mm"]);
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
      await bindControls(item.DesignId, item.BlindId);
      await bindColours(item.DesignId, item.BlindId, item.ControlType);
      await handlerElementVisibility(
        item.BlindId,
        item.ControlType,
        item.KitId,
        item,
      );
      await Promise.all([
        bindNotes(item.BlindName),
        bindSizeType(),
        bindDropFloor(),
        bindMounting(),
        bindControlPosition(),
        bindControlLift(),
        bindControlTilt(),
        bindBracket(item.BlindName),
        bindBottomHoldDown(item.BlindName),
        bindHoldDownBracket(item.BlindName),
        bindPelmetType(item.BlindName, item.Mounting),
        bindPelmetSize(),
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
  controltype,
  colourtype,
  item,
) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divControlType = document.getElementById("divControlType");
    const divColourType = document.getElementById("divColourType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divSizeType = document.getElementById("divSizeType");
    const divDropFloor = document.getElementById("divDropFloor");
    const divControl = document.getElementById("divControl");
    const divControlMock = document.getElementById("divControlMock");
    const divBracket = document.getElementById("divBracket");
    const divBottom = document.getElementById("divBottom");
    const divHoldBracket = document.getElementById("divHoldBracket");
    const div2on1Headreal = document.getElementById("div2on1Headreal");
    const divPelmetDetail = document.getElementById("divPelmetDetail");
    const divPelmetSize = document.getElementById("divPelmetSize");
    const divReturnLength = document.getElementById("divReturnLength");
    const divReturnLeft = document.getElementById("divReturnLeft");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.getElementById("btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divControlType.classList.add("d-none");
    divColourType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divSizeType.classList.add("d-none");
    divDropFloor.classList.add("d-none");
    divControl.classList.add("d-none");
    divControlMock.classList.add("d-none");
    divBracket.classList.add("d-none");
    divBottom.classList.add("d-none");
    divHoldBracket.classList.add("d-none");
    div2on1Headreal.classList.add("d-none");
    divPelmetDetail.classList.add("d-none");
    divPelmetSize.classList.add("d-none");
    divReturnLength.classList.add("d-none");
    divReturnLeft.classList.add("d-none");
    divReturnRight.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    if (["50mm Timberstyle", "63mm Timberstyle"].includes(blindname)) {
      divControlType.classList.remove("d-none");
    }

    if (!controltype) return;
    divColourType.classList.remove("d-none");

    if (!colourtype) return;
    divFormDetail.classList.remove("d-none");

    if (["25mm Aluminium", "50mm Aluminium"].includes(blindname)) {
      if (["50mm Aluminium"].includes(blindname)) {
        divSizeType.classList.remove("d-none");
      }
      divControl.classList.remove("d-none");
      divBracket.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      // divHoldBracket.classList.remove("d-none");
      div2on1Headreal.classList.remove("d-none");
    }

    if (["50mm Mockwood", "63mm Mockwood"].includes(blindname)) {
      divControlMock.classList.remove("d-none");
      // divHoldBracket.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      div2on1Headreal.classList.remove("d-none");
      divPelmetDetail.classList.remove("d-none");
    }

    if (["50mm Timberstyle", "63mm Timberstyle"].includes(blindname)) {
      divSizeType.classList.remove("d-none");
      divControl.classList.remove("d-none");
      divBracket.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      divPelmetDetail.classList.remove("d-none");
      divPelmetSize.classList.remove("d-none");
    }

    if (["50mm Wooden", "63mm Wooden"].includes(blindname)) {
      divControlMock.classList.remove("d-none");
      // divHoldBracket.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      div2on1Headreal.classList.remove("d-none");
      divPelmetDetail.classList.remove("d-none");
    }

    if (item) {
      if (["Opening Size"].includes(item.LouvreSize)) {
        divDropFloor.classList.remove("d-none");
      }
      if (item.PelmetType == "With Return") {
        divReturnLength.classList.remove("d-none");
        divReturnLeft.classList.remove("d-none");
        divReturnRight.classList.remove("d-none");
      }

      if (item.PelmetType == "Single Left Return") {
        divReturnLength.classList.remove("d-none");
        divReturnLeft.classList.remove("d-none");
      }

      if (item.PelmetType == "Single Right Return") {
        divReturnLength.classList.remove("d-none");
        divReturnRight.classList.remove("d-none");
      }
    }

    if (MARKUPACCESS === "True") divMarkUp.classList.remove("d-none");

    if (["AddItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
      btnSubmit.classList.remove("d-none");
    } else if (ITEMACTION === "ViewItem") {
      btnSubmit.classList.remove("d-none");
      if (!["Administrator", "PPIC & DE"].includes(ROLENAME))
        btnSubmit.classList.add("d-none");
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
      "controltype",
      "colourtype",
      "qty",
      "room",
      "sizetype",
      "dropfloor",
      "mounting",
      "width",
      "drop",
      "controlposition",
      "controllength",
      "controllift",
      "controltilt",
      "bracket",
      "bottom",
      "holdbracket",
      "twoheadrail",
      "pelmettype",
      "pelmetsize",
      "pelmetwidth",
      "returnleft",
      "returnright",
      "toplhswidth",
      "toplhsheight",
      "toprhswidth",
      "toprhsheight",
      "toprhsheight",
      "botlhswidth",
      "botlhsheight",
      "botrhswidth",
      "botrhsheight",
      "botrhsheight",
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
    document.getElementById(button).innerHTML = "Save Changes";
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    blindtype: "BlindId",
    controltype: "ControlType",
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    sizetype: "LouvreSize",
    dropfloor: "LouvrePosition",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    controlposition: "ControlPosition",
    controllength: "ControlLength",
    controllift: "ControlPosition",
    controltilt: "ControlPosition",
    bracket: "BracketOption",
    bottom: "BottomHoldDown",
    holdbracket: "BracketColour",
    twoheadrail: "DoorCutOut",
    pelmettype: "PelmetType",
    pelmetsize: "PelmetSize",
    pelmetwidth: "PelmetWidth",
    returnleft: "PelmetReturnSize",
    returnright: "PelmetReturnSize2",
    toplhswidth: "LHSWidth_Top",
    toplhsheight: "LHSHeight_Top",
    toprhswidth: "RHSWidth_Top",
    toprhsheight: "RHSHeight_Top",
    botlhswidth: "LHSWidth_Bottom",
    botlhsheight: "LHSHeight_Bottom",
    botrhswidth: "RHSWidth_Bottom",
    botrhsheight: "RHSHeight_Bottom",
    notes: "Notes",
    markup: "MarkUp",
  };

  const controlValue = itemData["ControlPosition"];
  if (controlValue) {
    if (controlValue.includes("|")) {
      const [lift, tilt] = controlValue.split("|");

      const elLift = document.getElementById("controllift");
      const elTilt = document.getElementById("controltilt");

      if (elLift) elLift.value = lift ?? "";
      if (elTilt) elTilt.value = tilt ?? "";
    } else {
      const elPosition = document.getElementById("controlposition");
      if (elPosition) elPosition.value = controlValue;
    }
  }

  // 1. set normal fields
  Object.entries(mapping).forEach(([id, key]) => {
    if (["controlposition", "controllift", "controltilt"].includes(id)) return;
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    el.value = value ?? "";

    if (el.value === "0") el.value = "";
  });

  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = document.getElementById("notescount");
  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const venetianPageLoaded = async () => {
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
