document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Window.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  windowPageLoaded();
});

// ==================================================EVENTS==================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    // ---------------------------------||blindtype||---------------------------------
    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindColours(DESIGNID, blindtype);
    }

    // ---------------------------------||colourtype||---------------------------------
    if (e.target.id === "colourtype") {
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

// button submit
document.querySelector("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  // handlerSubmit(e.target.form, e.target.id);
  handlerSubmit(e.target.id);
});

// button cancel
document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// ==================================================FUNCTION================================================
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

      if (data.length > 0) {
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
        // select.selectedIndex = 0;
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
      body: JSON.stringify({
        designid,
        blindid,
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
      throw new Error("No data returned from server : bindColours");
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
          ` SELECT Name FROM Blinds WHERE Id = '${blindid}' AND Active=1 `,
        );
        if (!blindname) {
          throw new Error("Blind name not found : bindColours");
        }
        await Promise.all([
          bindMounting(),
          bindMesh(blindname),
          bindFrameColour(blindname),
          bindFrameSize(blindname),
          bindBrace(),
          bindAngleType(),
          bindPortHole(),
          bindPlunger(),
          bindSwivalColour(),
        ]);
        await handlerElementVisibility(blindid, select.value);
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
    { value: "Opening Size", text: "Opening Size" },
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

const bindMesh = (blindname) => {
  const sel = document.getElementById("meshtype");
  sel.innerHTML = ""; //reset

  let data = [];

  if (["Flyscreen", "Standard"].includes(blindname)) {
    data.push(
      { value: "Fiberglass", text: "Fiberglass" },
      { value: "Aluminium", text: "Aluminium" },
      { value: "Stainless Steel", text: "Stainless Steel" },
      { value: "Pet Mesh", text: "Pet Mesh" },
    );
  }
  if (["Safety"].includes(blindname)) {
    data.push({ value: "304 SS Mesh", text: "304 SS Mesh" });
  }
  if (["Security"].includes(blindname)) {
    data.push({ value: "316 SS Mesh", text: "316 SS Mesh" });
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

const bindFrameColour = (blindname) => {
  const sel = document.getElementById("framecolour");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];

  if (blindname == "Flyscreen") {
    data.push(
      { value: "Apo Grey", text: "Apo Grey" },
      { value: "Beige", text: "Beige" },
      { value: "Birch White", text: "Birch White" },
      { value: "Black", text: "Black" },
      { value: "Bronze", text: "Bronze" },
      { value: "Brown", text: "Brown" },
      { value: "Charcoal", text: "Charcoal" },
      { value: "Deep Ocean", text: "Deep Ocean" },
      { value: "Dune", text: "Dune" },
      { value: "Hawthorne Green", text: "Hawthorne Green" },
      { value: "Jasper", text: "Jasper" },
      { value: "Monument", text: "Monument" },
      { value: "Notre Dame", text: "Notre Dame" },
      { value: "Primrose", text: "Primrose" },
      { value: "Silver", text: "Silver" },
      { value: "Surf Mist", text: "Surf Mist" },
      { value: "White", text: "White" },
      { value: "Woodland Grey", text: "Woodland Grey" },
    );
  } else {
    data.push(
      { value: "Apo Grey", text: "Apo Grey" },
      { value: "Beige", text: "Beige" },
      { value: "Birch White", text: "Birch White" },
      { value: "Black", text: "Black" },
      { value: "Bronze", text: "Bronze" },
      { value: "Brown", text: "Brown" },
      { value: "Charcoal", text: "Charcoal" },
      { value: "Deep Ocean", text: "Deep Ocean" },
      { value: "Dune", text: "Dune" },
      { value: "Hawthorne Green", text: "Hawthorne Green" },
      { value: "Jasper", text: "Jasper" },
      { value: "Monument", text: "Monument" },
      { value: "Notre Dame", text: "Notre Dame" },
      { value: "Primrose", text: "Primrose" },
      { value: "Silver", text: "Silver" },
      { value: "Surf Mist", text: "Surf Mist" },
      { value: "White", text: "White" },
      { value: "Woodland Grey", text: "Woodland Grey" },
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

const bindFrameSize = (blindname) => {
  const sel = document.getElementById("framesize");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  if (blindname == "Flyscreen") {
    data.push(
      { value: "21x9", text: "21x9" },
      { value: "25x11", text: "25x11" },
      { value: "35x11 Grille", text: "35x11 Grille" },
    );
  } else {
    data.push({ value: "35x11 Grille", text: "35x11 Grille" });
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

const bindBrace = () => {
  const sel = document.getElementById("brace");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Centre of Horizontal", text: "Centre of Horizontal" },
    { value: "Centre of Vertical", text: "Centre of Vertical" },
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

const bindAngleType = () => {
  const sel = document.getElementById("angletype");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "12x12mm", text: "12x12mm" },
    { value: "12x20mm", text: "12x20mm" },
    { value: "12x25mm", text: "12x25mm" },
    { value: "20x20mm", text: "20x20mm" },
    { value: "20x25mm", text: "20x25mm" },
    { value: "20x40mm", text: "20x40mm" },
    { value: "25x50mm", text: "25x50mm" },
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

const bindPortHole = () => {
  const sel = document.getElementById("porthole");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Supply Loose", text: "Supply Loose" },
    { value: "Fitted (Diagram)", text: "Fitted (Diagram)" },
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

const bindPlunger = () => {
  const sel = document.getElementById("plungerpin");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Metal Loose (4)", text: "Metal Loose (4)" },
    { value: "Metal Loose (6)", text: "Metal Loose (6)" },
    { value: "Plain Loose (4)", text: "Plain Loose (4)" },
    { value: "Plain Loose (6)", text: "Plain Loose (6)" },
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

const bindSwivalColour = () => {
  const sel = document.getElementById("swivelcolour");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Black", text: "Black" },
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
      await Promise.all([
        bindMounting(),
        bindMesh(item.BlindName),
        bindFrameColour(item.BlindName),
        bindFrameSize(item.BlindName),
        bindBrace(),
        bindAngleType(),
        bindPortHole(),
        bindPlunger(),
        bindSwivalColour(),
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
// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, colourtype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divColourType = document.getElementById("divColourType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divMounting = document.getElementById("divMounting");
    const divMesh = document.getElementById("divMesh");
    const divFrameColour = document.getElementById("divFrameColour");
    const divBrace = document.getElementById("divBrace");
    const divAngle = document.getElementById("divAngle");
    const divPortHole = document.getElementById("divPortHole");
    const divPlungerPin = document.getElementById("divPlungerPin");
    const divSwivalColour = document.getElementById("divSwivalColour");
    const divSwivalQty = document.getElementById("divSwivalQty");
    const divSpringQty = document.getElementById("divSpringQty");
    const divTopPlasticQty = document.getElementById("divTopPlasticQty");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");
    lblItemId.classList.add("d-none");
    divColourType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divMounting.classList.add("d-none");
    divMesh.classList.add("d-none");
    divFrameColour.classList.add("d-none");
    divBrace.classList.add("d-none");
    divAngle.classList.add("d-none");
    divPortHole.classList.add("d-none");
    divPlungerPin.classList.add("d-none");
    divSwivalColour.classList.add("d-none");
    divSwivalQty.classList.add("d-none");
    divSpringQty.classList.add("d-none");
    divTopPlasticQty.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}' AND Active=1 `,
    );
    // divColourType.classList.remove("d-none"); // unhide if isset colourtype

    if (!colourtype) return;
    const colourname = await getItemData(
      `SELECT ColourType FROM HardwareKits WHERE Id = '${colourtype}' AND Active=1 `,
    );
    divColourType.classList.add("d-none");
    if (!["N/A"].includes(colourname)) {
      divColourType.classList.remove("d-none");
    }

    divFormDetail.classList.remove("d-none");

    if (blindname === "Flyscreen") {
      divMounting.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      divAngle.classList.remove("d-none");
      divPortHole.classList.remove("d-none");
      divPlungerPin.classList.remove("d-none");
      divSwivalColour.classList.remove("d-none");
      divSwivalQty.classList.remove("d-none");
      divSpringQty.classList.remove("d-none");
      divTopPlasticQty.classList.remove("d-none");
    }
    if (blindname === "Safety") {
      divMounting.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      divAngle.classList.remove("d-none");
    }
    if (blindname === "Security") {
      divMounting.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divAngle.classList.remove("d-none");
    }
    if (blindname === "Standard") {
      divMounting.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      divAngle.classList.remove("d-none");
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
      "colourtype",
      "qty",
      "room",
      "mounting",
      "width",
      "drop",
      "meshtype",
      "framecolour",
      "framesize",
      "brace",
      "angletype",
      "anglelength",
      "angleqty",
      "porthole",
      "plungerpin",
      "swivelcolour",
      "swivelqty",
      "swivelqtyb",
      "springqty",
      "topplasticqty",
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
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    meshtype: "MeshType",
    framecolour: "FrameColour",
    framesize: "FrameType",
    brace: "Brace",
    angletype: "AngleType",
    anglelength: "AngleLength",
    angleqty: "AngleQty",
    porthole: "PortHole",
    plungerpin: "PlungerPin",
    swivelcolour: "SwipelColour",
    swivelqty: "SwipelQty",
    swivelqtyb: "SwipelQtyB",
    springqty: "SpringQty",
    topplasticqty: "TopPlasticQty",
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

    el.value = value ?? ""; // fallback ke string kosong

    // jika nilainya "0" → kosong
    if (el.value === "0") el.value = "";
  });
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const windowPageLoaded = async () => {
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
