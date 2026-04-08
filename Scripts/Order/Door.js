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
  doorPageLoaded();
});

// ==================================================EVENTS==================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blind = document.getElementById("blindtype");
      const blindtype = blind.value;
      const blindname = blind.selectedOptions[0].dataset.name;
      const tubetype = e.target.value;
      await Promise.all([
        bindMounting(),
        bindFrameColour(blindname),
        bindMeshType(),
        bindLayout(),
        bindHandlePosition(),
        bindHandleMeasure(),
        bindMidrailPosition(),
        bindPetDoorType(),
        bindPetDoorPosition(),
        bindTripleLock(),
        bindLatchBass(),
        bindBugseal(),
        bindDoorCloser(),
      ]);
      await handlerElementVisibility(blindtype, tubetype);
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

// button cancel
document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// =================================================FUNCTIONS================================================
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
        const blindname = await getItemData(
          `SELECT Name FROM Blinds WHERE Id = ${blindid}`,
        );
        const tubetype = select.value;
        await Promise.all([
          bindMounting(),
          bindFrameType(blindname),
          bindFrameColour(blindname),
          bindMeshType(),
          bindLayout(),
          bindHandlePosition(),
          bindHandleMeasure(),
          bindMidrailPosition(),
          bindPetDoorType(),
          bindPetDoorPosition(),
          bindTripleLock(),
          bindLatchBass(),
          bindBugseal(),
          bindDoorCloser(),
        ]);
        await handlerElementVisibility(blindid, tubetype);
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
    { value: "In", text: "In" },
    { value: "Out", text: "Out" },
    { value: "Make Size", text: "Make Size" },
    { value: "Inswing", text: "Inswing" },
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

const bindFrameType = (blindname) => {
  const sel = document.getElementById("frametype");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  data.push(
    { value: "Door Frame", text: "Door Frame" },
    { value: "Grille Frame", text: "Grille Frame" },
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

const bindFrameColour = (blindname) => {
  const sel = document.getElementById("framecolour");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  data.push(
    { value: "Bronze", text: "Bronze" },
    { value: "Apo Grey", text: "Apo Grey" },
    { value: "Black", text: "Black" },
    { value: "Bronze Anodized", text: "Bronze Anodized" },
    { value: "Brown", text: "Brown" },
    { value: "Charcoal", text: "Charcoal" },
    { value: "Claret", text: "Claret" },
    { value: "Deep Ocean", text: "Deep Ocean" },
    { value: "Dune", text: "Dune" },
    { value: "Hawthorne Green", text: "Hawthorne Green" },
    { value: "Mist Green", text: "Mist Green" },
    { value: "Monument", text: "Monument" },
    { value: "Notre Dame", text: "Notre Dame" },
    { value: "Paperbank", text: "Paperbank" },
    { value: "Primrose", text: "Primrose" },
    { value: "Silver/Clear Anodize", text: "Silver/Clear Anodize" },
    { value: "Stone Beige", text: "Stone Beige" },
    { value: "Surf Mist", text: "Surf Mist" },
    { value: "White", text: "White" },
    { value: "White Birch", text: "White Birch" },
    { value: "Woodland Grey", text: "Woodland Grey" },
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

const bindMeshType = () => {
  const sel = document.getElementById("meshtype");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "HD Diamond", text: "HD Diamond" },
    { value: "Fiberglass", text: "Fiberglass" },
    { value: "Pawproof", text: "Pawproof" },
    { value: "Stainless Steel", text: "Stainless Steel" },
    { value: "SS304 0.7mm", text: "SS304 0.7mm" },
    { value: "SS316 0.8mm", text: "SS316 0.8mm" },
    { value: "SS316 0.9mm", text: "SS316 0.9mm" },
    { value: "HD Diamond+Fiberglass", text: "HD Diamond+Fiberglass" },
    { value: "HD Diamond+Pawproof", text: "HD Diamond+Pawproof" },
    { value: "HD Diamond+Stainless Steel", text: "HD Diamond+Stainless Steel" },
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

const bindLayout = () => {
  const sel = document.getElementById("layoutcode");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "L", text: "L" },
    { value: "R", text: "R" },
    { value: "L-RA", text: "L-RA" },
    { value: "A-LR", text: "A-LR" },
    { value: "AL", text: "AL" },
    { value: "RA", text: "RA" },
    { value: "RA-L", text: "RA-L" },
    { value: "R-AL", text: "R-AL" },
    { value: "ALL", text: "ALL" },
    { value: "RRA", text: "RRA" },
    { value: "FRA", text: "FRA" },
    { value: "ALF", text: "ALF" },
    { value: "RRRA", text: "RRRA" },
    { value: "ALLL", text: "ALLL" },
    { value: "FRRA", text: "FRRA" },
    { value: "ALLF", text: "ALLF" },
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

const bindHandlePosition = () => {
  const sel = document.getElementById("handleposition");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Left", text: "Left" },
    { value: "Right", text: "Right" },
    { value: "No Handle", text: "No Handle" },
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

const bindHandleMeasure = () => {
  const sel = document.getElementById("handlemeasure");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Lock Height", text: "Lock Height" },
    { value: "Centre of Handle", text: "Centre of Handle" },
    { value: "Bottom of Tongue", text: "Bottom of Tongue" },
    { value: "Centre of Tongue", text: "Centre of Tongue" },
    { value: "Bottom of Lock body", text: "Bottom of Lock body" },
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

const bindMidrailPosition = () => {
  const sel = document.getElementById("midrailposition");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "By Request", text: "By Request" },
    { value: "Centre of Vertical", text: "Centre of Vertical" },
    { value: "No Midrail", text: "No Midrail" },
    { value: "Centre of Horizontal", text: "Centre of Horizontal" },
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

const bindPetDoorType = () => {
  const sel = document.getElementById("petdoortype");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Small 190x240", text: "Small 190x240" },
    { value: "Medium 255x305", text: "Medium 255x305" },
    { value: "Large 260x400", text: "Large 260x400" },
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

const bindPetDoorPosition = () => {
  const sel = document.getElementById("petdoorposition");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Left", text: "Left" },
    { value: "Centre", text: "Centre" },
    { value: "Right", text: "Right" },
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

const bindTripleLock = () => {
  const sel = document.getElementById("triplelock");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push({ value: "No", text: "No" }, { value: "Yes", text: "Yes" });

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

const bindLatchBass = () => {
  const sel = document.getElementById("latchbass");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Outer Pull", text: "Outer Pull" },
    { value: "Bass Standard", text: "Bass Standard" },
    { value: "Bass Hinged", text: "Bass Hinged" },
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

const bindBugseal = () => {
  const sel = document.getElementById("bugseal");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "No", text: "No" },
    { value: "Yes", text: "Yes" },
    { value: "Yes (Long Fur)", text: "Yes (Long Fur)" },
    { value: "Yes (Short Fur)", text: "Yes (Short Fur)" },
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

const bindDoorCloser = () => {
  const sel = document.getElementById("doorcloser");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "No", text: "No" },
    { value: "Black", text: "Black" },
    { value: "Primrose", text: "Primrose" },
    { value: "White", text: "White" },
    { value: "White Birch", text: "White Birch" },
    { value: "Bronze Anodize", text: "Bronze Anodize" },
    { value: "Silver/Clear Anodize", text: "Silver/Clear Anodize" },
    { value: "Stone Beige", text: "Stone Beige" },
    { value: "Apo Grey", text: "Apo Grey" },
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
// ----------------------------------------------|| Other Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, tubetype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divMounting = document.getElementById("divMounting");

    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    divTubeType.classList.add("d-none");

    divFormDetail.classList.add("d-none");

    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    divTubeType.classList.remove("d-none");

    if (!tubetype) return;
    divFormDetail.classList.remove("d-none");

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
// ----------------------------------------------|| Other Functions ||---------------------------------------
const doorPageLoaded = async () => {
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
