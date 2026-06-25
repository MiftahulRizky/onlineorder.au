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
  verticalPageLoaded();
});

// =======================================================|| EVENT LISTENERS ||=======================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blindtype = document.getElementById("blindtype").value;
      const tubetype = e.target.value;
      await handlerElementVisibility(blindtype, tubetype);
      await bindControls(DESIGNID, blindtype, tubetype);
    }

    if (e.target.id === "controltype") {
      const blind = document.getElementById("blindtype");
      const blindtype = blind.value;
      const blindname = blind.selectedOptions[0].dataset.name;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      await bindFabrics(DESIGNID);
      await Promise.all([
        bindSlatSize(),
        bindTrackColour(tubetype),
        bindStackPosition(),
        bindChains(),
        bindWandLength(),
        bindBracketType(),
        bindBracketColour(tubetype),
        bindHanger(blindname, tubetype),
        bindBottom(),
      ]);
      await handlerElementVisibility(blindtype, tubetype, controltype);
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      const tubetype = document.getElementById("tubetype").value;
      await bindFabricLength(DESIGNID, tubetype, fabrictype);
    }

    if (e.target.id === "fabriclength") {
      const fabrictype = document.querySelector("#fabrictype").value;
      const fabriclength = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype, fabriclength);
    }

    if (e.target.id === "wandlength") {
      const tubetype = document.getElementById("tubetype").value;
      const divWandCustomLength = document.querySelector(
        "#divWandCustomLength",
      );
      const wandlength = e.target.value;
      divWandCustomLength.classList.add("d-none");
      if (wandlength === "custom") {
        divWandCustomLength.classList.remove("d-none");
      }
      await Promise.all([bindWandColour(tubetype, wandlength)]);
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
    let msg = "";

    switch (id) {
      case "btnInfoQty":
        msg =
          "Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.";
        break;
      case "btnInfoWD":
        msg =
          "Very long tracks are not recommended. Butting shorter tracks will work more effectively.";
        break;
      case "btnInfoSlatQty":
        msg = "If left blank, the system will calculate it.";
        break;
      case "btnInfoCustomLength":
        msg =
          "Custom wand length is available in white color only with maximum length 3000mm.";
        break;
    }

    if (msg) {
      isInfo(msg);
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
        const blind = document.getElementById("blindtype");
        const blindtype = blind.value;
        const blindname = blind.selectedOptions[0].dataset.name;
        const tubetype = document.getElementById("tubetype").value;
        const controltype = select.value;
        await bindFabrics(designid);
        await Promise.all([
          bindSlatSize(),
          bindTrackColour(tubetype),
          bindStackPosition(),
          bindChains(),
          bindWandLength(),
          bindBracketType(),
          bindBracketColour(tubetype),
          bindHanger(blindname, tubetype),
          bindBottom(),
        ]);
        await handlerElementVisibility(blindtype, tubetype, controltype);
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

const bindFabricLength = async (designid, tubetype, fabrictype) => {
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
        tubetype,
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
        const fabriclength = select.value;
        await bindFabricColours(designid, fabrictype, fabriclength);
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

const bindFabricColours = async (designid, fabrictype, fabriclength) => {
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

const bindSlatSize = () => {
  const sel = document.getElementById("slatsize");
  sel.innerHTML = ""; //reset

  if (!tubetype) return;

  let data = [];
  data.push(
    { value: "127mm", text: "127mm" },
    { value: "100mm", text: "100mm" },
    { value: "89mm", text: "89mm" },
    { value: "63mm", text: "63mm" },
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

const bindTrackColour = (tubetype) => {
  const sel = document.getElementById("trackcolour");
  sel.innerHTML = ""; //reset

  if (!tubetype) return;

  let data = [];
  if (tubetype === "28mm Tiltrack") {
    data.push({ value: "Primrose", text: "Primrose" });
  }
  if (["Fairline", "Javaline"].includes(tubetype)) {
    data.push(
      { value: "Beige", text: "Beige" },
      // { value: "Birch White", text: "Birch White" },
      { value: "Black", text: "Black" },
      { value: "Silver", text: "Silver" },
    );
  }
  if (tubetype === "Louvolite") {
    data.push({ value: "Birch White", text: "Birch White" });
    data.push({ value: "White", text: "White" });
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
    { value: "1000", text: "1000mm" },
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

const bindWandColour = (tubetype, wandlength) => {
  const sel = document.getElementById("wandcolour");
  sel.innerHTML = ""; //reset

  if (!wandlength) return;

  let data = [];
  if (wandlength === "custom") {
    data.push({ value: "White", text: "White" });
  } else {
    if (tubetype === "Louvolite") {
      data.push({ value: "White", text: "White" });
    } else {
      data.push(
        { value: "Birch", text: "Birch" },
        { value: "Black", text: "Black" },
        { value: "Beige", text: "Beige" },
        { value: "White", text: "White" },
      );
    }
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

const bindBracketType = () => {
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

const bindBracketColour = (tubetype) => {
  const sel = document.getElementById("bracketcolour");
  sel.innerHTML = ""; //reset

  let data = [];
  if (tubetype === "Louvolite") {
    data.push(
      { value: "Birch White", text: "Birch White" },
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

const bindHanger = (blindname, tubetype) => {
  const sel = document.getElementById("hangertype");
  sel.innerHTML = ""; //reset

  if (!blindname || !tubetype) return;

  let data = [];
  if (["Slat Only"].includes(blindname)) {
    data.push(
      { value: "Standard", text: "Standard" },
      { value: "Peghook", text: "Peghook" },
      { value: "Tiltrack 28mm", text: "Tiltrack 28mm" },
    );
  }

  if (["Complete", "Track Only"].includes(blindname)) {
    if (["Louvolite"].includes(tubetype)) {
      data.push(
        { value: "Opaque", text: "Opaque" },
        { value: "White", text: "White" },
      );
    }
    if (["Fairline", "Javaline"].includes(tubetype)) {
      data.push({ value: "Standard", text: "Standard" });
    }
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

  if (["Louvolite"].includes(tubetype)) {
    sel.value = "White";
  }
};

const bindBottom = () => {
  const sel = document.getElementById("bottom");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Chained (Black)", text: "Chained (Black)" },
    { value: "Chained (White)", text: "Chained (White)" },
    { value: "Fully Sewn In", text: "Fully Sewn In" },
    { value: "Plastic Chainless", text: "Plastic Chainless" },
    { value: "Plastic Chainless (Black)", text: "Plastic Chainless (Black)" },
    { value: "Plastic Chainless (White)", text: "Plastic Chainless (White)" },
    { value: "Top Hanger Only", text: "Top Hanger Only" },
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
      await bindTubes(item.DesignId, item.BlindId);
      await bindControls(item.DesignId, item.BlindId, item.TubeType);
      await bindFabrics(item.DesignId);
      await bindFabricLength(item.DesignId, item.TubeType, item.FabricType);
      await bindFabricColours(item.DesignId, item.FabricType, item.FabricWidth);
      await Promise.all([
        bindSlatSize(),
        bindTrackColour(item.TubeType),
        bindStackPosition(),
        bindChains(),
        bindWandLength(),
        bindWandColour(item.TubeType, item.WandLength),
        bindBracketType(),
        bindBracketColour(item.TubeType),
        bindHanger(item.BlindName, item.TubeType),
        bindBottom(),
        handlerSetElementValues(item),
      ]);
      await handlerElementVisibility(
        item.BlindId,
        item.TubeType,
        item.KitId,
        item,
      );
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};
// ----------------------------------------------------------- || Handler Funtions ||------------------------------------------------------------
const handlerElementVisibility = async (
  blindtype,
  tubetype,
  controltype,
  item,
) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const divControlType = document.getElementById("divControlType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divMounting = document.getElementById("divMounting");
    const lblWd = document.getElementById("lblWd");
    const divWidth = document.getElementById("divWidth");
    const divDrop = document.getElementById("divDrop");
    const divInfoWD = document.getElementById("divInfoWD");
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
    // return;
    divTubeType.classList.add("d-none");
    divControlType.classList.add("d-none");

    divFormDetail.classList.add("d-none");
    divMounting.classList.add("d-none");
    lblWd.innerHTML = "width x drop";
    divWidth.classList.add("d-none");
    divDrop.classList.add("d-none");
    divInfoWD.classList.add("d-none");
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
      divMounting.classList.remove("d-none");
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
      divMounting.classList.remove("d-none");
      divWidth.classList.remove("d-none");
      divInfoWD.classList.remove("d-none");
      divSlatSize.classList.remove("d-none");
      // divSlatQty.classList.remove("d-none");
      divTrackColour.classList.remove("d-none");
      divStackPosition.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      // divChain.classList.remove("d-none");
      divBrackets.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
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

    if (item) {
      const WandLengthKey = ["", "500", "750", "1100", "1250", "1500", "2000"];
      const WandLengthVal = item.WandLength;
      if (!WandLengthKey.includes(WandLengthVal)) {
        divWandCustomLength.classList.remove("d-none");
      } else {
        divWandCustomLength.classList.add("d-none");
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

const handlerSubmit = async (button) => {
  try {
    // return alert(button);
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = [
      "blindtype",
      "tubetype",
      "controltype",
      "qty",
      "room",
      "mounting",
      "width",
      "drop",
      "slatsize",
      "slatqty",
      "fabrictype",
      "fabriclength",
      "fabriccolour",
      "trackcolour",
      "stackposition",
      "controlposition",
      "chaincolour",
      "chainlength",
      "wandlength",
      "wandcolour",
      "wandcustomlength",
      "bracket",
      "bracketcolour",
      "hangertype",
      "bottom",
      "inserttrack",
      "sloper",
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
    tubetype: "TubeType",
    controltype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    slatsize: "SlatSize",
    slatqty: "SlatQty",
    fabrictype: "FabricType",
    fabriclength: "FabricWidth",
    fabriccolour: "FabricId",
    trackcolour: "TrackColour",
    stackposition: "StackPosition",
    controlposition: "ControlPosition",
    chaincolour: "ChainColour",
    chainlength: "ChainLength",
    wandcolour: "WandColour",
    wandlength: "WandLength",
    wandlength: "WandLength",
    wandcustomlength: "WandLength",
    bracket: "BracketOption",
    bracketcolour: "BracketColour",
    hangertype: "HangerType",
    bottom: "BottomHoldDown",
    inserttrack: "InsertInTrack",
    sloper: "Sloper",
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

    const WandLengthKey = ["", "500", "750", "1100", "1250", "1500", "2000"];
    const WandLengthVal = itemData["WandLength"];
    if (!WandLengthKey.includes(WandLengthVal)) {
      if (id === "wandlength") {
        el.value = "custom";
      }
      if (id === "wandcustomlength") {
        el.value = WandLengthVal;
      }
    } else {
      if (id === "wandlength") {
        el.value = WandLengthVal;
      }
    }

    if (["inserttrack", "sloper"].includes(id)) {
      if (["0", "False"].includes(value)) {
        el.value = "0";
      } else {
        el.value = "1";
      }
    }

    // jika nilainya "0" → kosong
    if (el.value === "0") el.value = "";
  });
};
// ----------------------------------------------------------- || Other Funtions ||------------------------------------------------------------
const verticalPageLoaded = async () => {
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
