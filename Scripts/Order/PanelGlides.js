document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Vertical.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  panelGlideGlobalPageLoaded();
});
// ==============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindColours(DESIGNID, blindtype);
    }

    // if (e.target.id === "tubetype") {
    //   const blindtype = document.getElementById("blindtype").value;
    //   const tubetype = e.target.value;
    //   await handlerElementVisibility(blindtype, tubetype);
    //   await bindControls(DESIGNID, blindtype, tubetype);
    // }

    if (e.target.id === "colourtype") {
      const blind = document.getElementById("blindtype");
      const blindtype = blind.value;
      const blindname = blind.selectedOptions[0].dataset.name;
      const colourtype = e.target.value;
      await Promise.all([bindMounting()]);
      await bindFabrics(DESIGNID);
      await Promise.all([
        bindLayoutCode(),
        bindNoPanel(),
        bindTrackType(),
        bindWandPosition(),
        bindWandColour(),
        bindBottomRail(),
        bindBattenColour(),
      ]);
      await handlerElementVisibility(blindtype, colourtype);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    if (e.target.id === "tracktype") {
      const tracktype = e.target.value;
      await Promise.all([bindTrackColour(tracktype)]);
    }

    if (e.target.id === "batten") {
      const divBattenColour = document.getElementById("divBattenColour");
      divBattenColour.classList.add("d-none");
      const batten = e.target.value;
      if (batten === "Yes") {
        divBattenColour.classList.remove("d-none");
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
// ==============================================================FUNCTIONS=====================================================================
// ----------------------------------------------------------- || Binding Funtions ||----------------------------------------------------------
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

const bindColours = async (designid, blindid) => {
  const select = document.getElementById("colourtype");
  select.innerHTML = "";

  if (!designid || !blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindColourType`, {
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
        const blindname = await getItemData(
          `SELECT Name FROM Blinds WHERE Id = '${blindid}'`,
        );
        const colourtype = select.value;
        await Promise.all([bindMounting()]);
        await bindFabrics(designid);
        await Promise.all([
          bindLayoutCode(),
          bindNoPanel(),
          bindTrackType(),
          bindWandPosition(),
          bindWandColour(),
          bindBottomRail(),
          bindBattenColour(),
        ]);
        await handlerElementVisibility(blindid, colourtype);
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

const bindMounting = () => {
  const sel = document.getElementById("mounting");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Make Size", text: "Make Size" },
    { value: "Face Fit", text: "Face Fit" },
    { value: "Reveal fit", text: "Reveal fit" },
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

const bindFabrics = async (designid) => {
  const select = document.getElementById("fabrictype");
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

const bindFabricColours = async (designid, fabrictype) => {
  const select = document.getElementById("fabriccolour");
  select.innerHTML = "";

  if (!designid || !fabrictype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
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

const bindLayoutCode = () => {
  const sel = document.getElementById("layoutcode");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "A", text: "A" },
    { value: "B", text: "B" },
    { value: "C", text: "C" },
    { value: "D", text: "D" },
    { value: "E", text: "E" },
    { value: "F", text: "F" },
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

const bindNoPanel = () => {
  const sel = document.getElementById("nopanel");
  sel.innerHTML = ""; //reset

  let data = [];
  for (let i = 2; i <= 9; i++) {
    data.push({ value: i, text: i });
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
    option.text = item.text;
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindTrackType = () => {
  const sel = document.getElementById("tracktype");
  document.getElementById("trackcolour").innerHTML = "";
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "3 Channel Track", text: "3 Channel Track" },
    { value: "4 Channel Track", text: "4 Channel Track" },
    { value: "5 Channel Track", text: "5 Channel Track" },
    { value: "6 Channel Track", text: "6 Channel Track" },
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

const bindTrackColour = (tracktype) => {
  const sel = document.getElementById("trackcolour");
  sel.innerHTML = ""; //reset

  if (!tracktype) return;

  let data = [];
  data.push(
    { value: "Black", text: "Black" },
    { value: "Grey", text: "Grey" },
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

const bindWandPosition = () => {
  const sel = document.getElementById("wandposition");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push({ value: "Back", text: "Back" }, { value: "Front", text: "Front" });

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

const bindWandColour = () => {
  const sel = document.getElementById("wandcolour");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Black", text: "Black" },
    { value: "Grey", text: "Grey" },
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

const bindBattenColour = () => {
  const sel = document.getElementById("battencolour");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Aluminium", text: "Aluminium" },
    { value: "Timber - Alabaster", text: "Timber - Alabaster" },
    { value: "Timber - Batlic", text: "Timber - Batlic" },
    { value: "Timber - Black", text: "Timber - Black" },
    { value: "Timber - Brown", text: "Timber - Brown" },
    { value: "Timber - Cherry", text: "Timber - Cherry" },
    { value: "Timber - Natural", text: "Timber - Natural" },
    { value: "Timber - Teak", text: "Timber - Teak" },
    { value: "Timber - White", text: "Timber - White" },
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

const bindBottomRail = () => {
  const sel = document.getElementById("bottomrail");
  sel.innerHTML = ""; //reset

  let data = [];
  let list = [];

  list = [
    "Standard (Plain Pocket)",
    "Small Flat Rail",
    "Large Flat Rail",
    "Fabric Inserted Rail",
    "Oval Rail",
  ];

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

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
      await Promise.all([bindMounting()]);
      await bindFabrics(item.DesignId);
      await bindFabricColours(item.DesignId, item.FabricType);
      await Promise.all([
        bindLayoutCode(),
        bindNoPanel(),
        bindTrackType(),
        bindTrackColour(item.TrackType),
        bindWandPosition(),
        bindWandColour(),
        bindBottomRail(),
        bindBattenColour(),
        handlerSetElementValues(item),
      ]);
      await handlerElementVisibility(item.BlindId, item.KitId, item);
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};
// ----------------------------------------------------------- || Handler Funtions ||------------------------------------------------------------
const handlerElementVisibility = async (blindtype, colourtype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divColourType = document.getElementById("divColourType");
    const divFormDetail = document.getElementById("divFormDetail");
    const lblWidthDrop = document.getElementById("lblWidthDrop");
    const divWidth = document.getElementById("divWidth");
    const divDrop = document.getElementById("divDrop");
    const divFabric = document.getElementById("divFabric");
    const divLayoutCode = document.getElementById("divLayoutCode");
    const divNoPanel = document.getElementById("divNoPanel");
    const divTrack = document.getElementById("divTrack");
    const divWandPosition = document.getElementById("divWandPosition");
    const divWand = document.getElementById("divWand");
    const divBottomRail = document.getElementById("divBottomRail");
    // const divBatten = document.getElementById("divBatten");
    // const divBattenColour = document.getElementById("divBattenColour");
    const divFitting = document.getElementById("divFitting");

    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divColourType.classList.add("d-none");
    lblWidthDrop.innerHTML = "width x drop";
    divWidth.classList.add("d-none");
    divDrop.classList.add("d-none");
    divFabric.classList.add("d-none");
    divLayoutCode.classList.add("d-none");
    divNoPanel.classList.add("d-none");
    divTrack.classList.add("d-none");
    divWandPosition.classList.add("d-none");
    divWand.classList.add("d-none");
    divBottomRail.classList.add("d-none");
    // divBatten.classList.add("d-none");
    // divBattenColour.classList.add("d-none");
    divFitting.classList.add("d-none");

    divFormDetail.classList.add("d-none");

    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divColourType.classList.remove("d-none");

    if (!colourtype) return;
    const colourname = await getItemData(
      `SELECT ColourType FROM HardwareKits WHERE Id = '${colourtype}'`,
    );
    if (colourname == "N/A") {
      divColourType.classList.add("d-none");
    }
    divFormDetail.classList.remove("d-none");

    if (["Completed"].includes(blindname)) {
      divWidth.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divLayoutCode.classList.remove("d-none");
      divNoPanel.classList.remove("d-none");
      divTrack.classList.remove("d-none");
      divWandPosition.classList.remove("d-none");
      divWand.classList.remove("d-none");
      divBottomRail.classList.remove("d-none");
      divFitting.classList.remove("d-none");
    }

    if (["Panel Only"].includes(blindname)) {
      divWidth.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divLayoutCode.classList.remove("d-none");
      divNoPanel.classList.remove("d-none");
      divBottomRail.classList.remove("d-none");
    }

    if (["Track Only"].includes(blindname)) {
      lblWidthDrop.innerHTML = "width";
      divWidth.classList.remove("d-none");
      divLayoutCode.classList.remove("d-none");
      divNoPanel.classList.remove("d-none");
      divTrack.classList.remove("d-none");
      divWandPosition.classList.remove("d-none");
      divWand.classList.remove("d-none");
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
      console.log(dataResult.error.message);
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

  // Set nilai ke input sesuai mapping
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[key];
    if (id === "markup" && value === 0) value = "";

    const maxLength = 1000;
    const notesLength = (itemData["Notes"] || "").length;
    document.querySelector("#notescount").textContent =
      `${notesLength}/${maxLength}`;

    el.value = value ?? ""; // fallback ke string kosong

    // jika nilainya "0" → kosong
    if (el.value === "0") el.value = "";
  });
};
// ----------------------------------------------------------- || Other Funtions ||------------------------------------------------------------
const panelGlideGlobalPageLoaded = async () => {
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
    await handlerElementVisibility();
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
      body: JSON.stringify({ query: query }),
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};
