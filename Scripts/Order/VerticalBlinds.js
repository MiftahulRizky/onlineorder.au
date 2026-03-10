document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("RollerBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  pageLoaded();
});

// =======================================================|| EVENT LISTENERS ||=======================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await Promise.all([handlerElementVisibility(blindtype)]);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blindtype = document.getElementById("blindtype").value;
      const tubetype = e.target.value;
      await Promise.all([handlerElementVisibility(blindtype, tubetype)]);
      await bindControls(DESIGNID, blindtype, tubetype);
    }

    if (e.target.id === "controltype") {
      const blindtype = document.getElementById("blindtype").value;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      await bindFabrics(DESIGNID);
      await Promise.all([
        bindTrackColour(tubetype),
        bindStackPosition(),
        bindChains(),
        bindWandLength(),
        BindBracketType(),
        BindBracketColour(tubetype),
        handlerElementVisibility(blindtype, tubetype, controltype),
      ]);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricLength(DESIGNID, fabrictype);
    }

    if (e.target.id === "fabriclength") {
      const fabrictype = document.querySelector("#fabrictype").value;
      const fabriclength = e.target.value;
      await bindFabricColour(DESIGNID, fabrictype, fabriclength);
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
  });
});

// button cancel
document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// ==========================================================|| FUNCTIONS ||==========================================================

// ----------------------------------------------------------- || Binding Funtions ||------------------------------------------------------------
const bindFormAction = (itemaction) => {
  const cardTitle = document.getElementById("cardTitle");
  const actionMap = {
    AddItem: "ADD ITEM",
    NextItem: "NEXT ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemaction] || "";
};

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

const bindBlinds = async () => {
  const select = document.getElementById("blindtype");
  select.innerHTML = "";

  if (!DESIGNID) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindBlindType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid: DESIGNID }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindBlinds");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindTubes = async (designid, blindid) => {
  const select = document.getElementById("tubetype");
  select.innerHTML = "";

  if (!designid || !blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindTubeType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindTubes");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
        // const blindname =
        //   document.getElementById("blindtype").selectedOptions[0].dataset.name;
        // await Promise.all([
        //   handlerElementVisibility(blindname, brackettype, select.value),
        // ]);
        // await bindControls(designid, blindid, brackettype, select.value);
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindControls = async (designid, blindid, tubetype) => {
  const select = document.getElementById("controltype");
  select.innerHTML = "";

  if (!designid || !blindid || !tubetype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindControlType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid, tubetype }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindControls");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
        const blindtype = document.getElementById("blindtype").value;
        const tubetype = document.getElementById("tubetype").value;
        const controltype = select.value;
        await Promise.all([
          handlerElementVisibility(blindtype, tubetype, controltype),
        ]);
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabrics = async (designid) => {
  const select = document.getElementById("fabrictype");
  document.getElementById("fabriclength").innerHTML = "";
  document.getElementById("fabriccolour").innerHTML = "";
  select.innerHTML = "";

  if (!designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindFabrics");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabricLength = async (designid, fabrictype) => {
  const select = document.getElementById("fabriclength");
  document.getElementById("fabriccolour").innerHTML = "";
  select.innerHTML = "";

  if (!designid || !fabrictype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricLength`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
        tubetype: document.getElementById("tubetype").value,
        fabrictype,
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindFabricLength");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabricColour = async (designid, fabrictype, fabriclength) => {
  const select = document.getElementById("fabriccolour");
  select.innerHTML = "";

  if (!designid || !fabrictype || !fabriclength) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
        fabrictype,
        fabriclength,
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindFabricLength");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindTrackColour = (tubetype) => {
  const sel = document.getElementById("trackcolour");
  sel.innerHTML = ""; //reset

  let data = [];
  if (tubetype === "28mm Tiltrack") {
    data.push({ value: "Primrose", text: "Primrose" });
  }
  if (["Fairline", "Javaline"].includes(tubetype)) {
    data.push(
      { value: "Beige", text: "Beige" },
      { value: "Birch White", text: "Birch White" },
      { value: "Black", text: "Black" },
      { value: "Silver", text: "Silver" },
    );
  }
  if (tubetype === "Louvolite") {
    data.push(
      { value: "Black", text: "Black" },
      { value: "White", text: "White" },
      { value: "Grey", text: "Grey" },
    );
  }

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindStackPosition = () => {
  const sel = document.getElementById("stackposition");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Left", text: "Left" },
    { value: "Right", text: "Right" },
    { value: "Center", text: "Center" },
    { value: "Split / Centre Open", text: "Split / Centre Open" },
  );

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindChains = () => {
  const sel = document.getElementById("chaincolour");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Beige", text: "Beige" },
    { value: "Birch White", text: "Birch White" },
    { value: "Black", text: "Black" },
    { value: "Grey", text: "Grey" },
    { value: "Stainless Steel", text: "Stainless Steel" },
    { value: "White", text: "White" },
  );

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindWandLength = () => {
  const sel = document.getElementById("wandlength");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "custom", text: "Custom (White Only)" },
    { value: "500", text: "500mm" },
    { value: "750", text: "750mm" },
    { value: "800", text: "800mm" },
    { value: "1100", text: "1100mm" },
    { value: "1250", text: "1250mm" },
    { value: "1500", text: "1500mm" },
    { value: "2000", text: "2000mm" },
  );

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindWandColour = (wandlength) => {
  const sel = document.getElementById("wandcolour");
  sel.innerHTML = ""; //reset

  let data = [];
  if (wandlength === "custom") {
    data.push({ value: "White", text: "White" });
  } else {
    data.push(
      { value: "Birch", text: "Birch" },
      { value: "Black", text: "Black" },
      { value: "Beige", text: "Beige" },
      { value: "White", text: "White" },
    );
  }

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const BindBracketType = () => {
  const sel = document.getElementById("bracket");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "127mm F/Fit", text: "127mm F/Fit" },
    { value: "100mm F/Fit", text: "100mm F/Fit" },
    { value: "89mm F/Fit", text: "89mm F/Fit" },
    { value: "C/Fit", text: "C/Fit" },
    { value: "Ext F/Fit", text: "Ext F/Fit" },
    { value: "Ext C/Fit", text: "Ext C/Fit" },
    { value: "Ext", text: "Ext" },
  );

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const BindBracketColour = (tubetype) => {
  const sel = document.getElementById("bracketcolour");
  sel.innerHTML = ""; //reset

  let data = [];
  if (tubetype === "Louvolite") {
    data.push(
      { value: "Black", text: "Black" },
      { value: "White", text: "White" },
      { value: "Grey", text: "Grey" },
    );
  } else {
    data.push(
      { value: "Black", text: "Black" },
      { value: "White", text: "White" },
      { value: "Silver", text: "Silver" },
    );
  }

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};
// ----------------------------------------------------------- || Handler Funtions ||------------------------------------------------------------
const handlerElementVisibility = async (blindtype, tubetype, controltype) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const divControlType = document.getElementById("divControlType");

    const divFormDetail = document.getElementById("divFormDetail");
    const lblWd = document.getElementById("lblWd");
    const divWidth = document.getElementById("divWidth");
    const divDrop = document.getElementById("divDrop");
    const divSlatSize = document.getElementById("divSlatSize");
    const divSlatQty = document.getElementById("divSlatQty");
    const divFabric = document.getElementById("divFabric");
    const divTrackColour = document.getElementById("divTrackColour");
    const divStackPosition = document.getElementById("divStackPosition");
    const divControlPosition = document.getElementById("divControlPosition");
    const divChain = document.getElementById("divChain");
    const divWand = document.getElementById("divWand");
    const divWandCustomLength = document.getElementById("divWandCustomLength");
    const divBrackets = document.getElementById("divBrackets");
    const divHangerType = document.getElementById("divHangerType");
    const divBottom = document.getElementById("divBottom");
    const divInsertInTrack = document.getElementById("divInsertInTrack");
    const divSloper = document.getElementById("divSloper");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");

    divTubeType.classList.add("d-none");
    divControlType.classList.add("d-none");

    divFormDetail.classList.add("d-none");
    lblWd.innerHTML = "width x drop";
    divWidth.classList.add("d-none");
    divDrop.classList.add("d-none");
    divSlatSize.classList.add("d-none");
    divSlatQty.classList.add("d-none");
    divFabric.classList.add("d-none");
    divTrackColour.classList.add("d-none");
    divStackPosition.classList.add("d-none");
    divControlPosition.classList.add("d-none");
    divChain.classList.add("d-none");
    divWand.classList.add("d-none");
    divWandCustomLength.classList.add("d-none");
    divBrackets.classList.add("d-none");
    divHangerType.classList.add("d-none");
    divBottom.classList.add("d-none");
    divInsertInTrack.classList.add("d-none");
    divSloper.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}' AND Active = 1`,
    );
    divTubeType.classList.remove("d-none");

    if (!tubetype) return;
    if (["Complete", "Track Only"].includes(blindname)) {
      divControlType.classList.remove("d-none");
    }

    if (!controltype) return;
    const controlname = await getItemData(
      `SELECT ControlType FROM HardwareKits WHERE Id = '${controltype}' AND Active = 1`,
    );

    divFormDetail.classList.remove("d-none");

    if (blindname === "Complete") {
      divWidth.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divTrackColour.classList.remove("d-none");
      divStackPosition.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      divBrackets.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      divSloper.classList.remove("d-none");

      if (tubetype === "Fairline") {
        divInsertInTrack.classList.remove("d-none");
      }
      if (controlname === "Chain") {
        divChain.classList.remove("d-none");
      }
      if (controlname === "Wand") {
        divWand.classList.remove("d-none");
      }
    }

    if (blindname === "Slat Only") {
      lblWd.innerHTML = "drop exact";
      divDrop.classList.remove("d-none");
      divSlatQty.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divBottom.classList.remove("d-none");
    }

    if (blindname === "Track Only") {
      lblWd.innerHTML = "width";
      divSlatSize.classList.remove("d-none");
      divSlatQty.classList.remove("d-none");
      divTrackColour.classList.remove("d-none");
      divStackPosition.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      divChain.classList.remove("d-none");
      divBrackets.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divSloper.classList.remove("d-none");

      if (tubetype === "Fairline") {
        divInsertInTrack.classList.remove("d-none");
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
    var msg = error.message;
    if (ROLENAME !== "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg);
  }
};
// ----------------------------------------------------------- || Other Funtions ||------------------------------------------------------------
const pageLoaded = async () => {
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
  bindFormAction(ITEMACTION);

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
      body: JSON.stringify({ query: query }),
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};
