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
          `SELECT Name FROM Blinds WHERE Id = '${blindid}'`,
        );
        console.log(blindname);
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
  if (blindname.includes("Steel")) {
    data.push(
      { value: "SD 1", text: "SD 1" },
      { value: "SD 2", text: "SD 2" },
      { value: "SD 3A", text: "SD 3A" },
      { value: "SD 4C", text: "SD 4C" },
      { value: "SD 5B", text: "SD 5B" },
      { value: "SD 6A1", text: "SD 6A1" },
      { value: "SD 7C", text: "SD 7C" },
      { value: "SD 8A1", text: "SD 8A1" },
      { value: "SD 9B", text: "SD 9B" },
    );
    for (let i = 10; i <= 27; i++) {
      data.push({ value: `SD ${i}`, text: `SD ${i}` });
    }
    data.push(
      { value: "SG 1", text: "SG 1" },
      { value: "SG 2", text: "SG 2" },
      { value: "SG 3A", text: "SG 3A" },
      { value: "SG 4C", text: "SG 4C" },
      { value: "SG 5", text: "SG 5" },
      { value: "SG 5A", text: "SG 5A" },
      { value: "SG 6A1", text: "SG 6A1" },
      { value: "SG 7C", text: "SG 7C" },
      { value: "SG 8A1", text: "SG 8A1" },
    );
  } else {
    data.push(
      { value: "Door Frame", text: "Door Frame" },
      { value: "Grille Frame", text: "Grille Frame" },
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
    const lblWidth = document.getElementById("lblWidth");
    const divWidthMiddle = document.getElementById("divWidthMiddle");
    const divWidthBotMin = document.getElementById("divWidthBotMin");
    const divFrameType = document.getElementById("divFrameType");
    const lblFrameType = document.getElementById("lblFrameType");
    const divFrameColour = document.getElementById("divFrameColour");
    const lblFrameColour = document.getElementById("lblFrameColour");
    const divFitted = document.getElementById("divFitted");
    const divMesh = document.getElementById("divMesh");
    const lblMesh = document.getElementById("lblMesh");
    const divFixing = document.getElementById("divFixing");
    const divTop = document.getElementById("divTop");
    const divHingeType = document.getElementById("divHingeType");
    const divLockType = document.getElementById("divLockType");
    const divLockHandling = document.getElementById("divLockHandling");
    const divFrameSize = document.getElementById("divFrameSize");
    const divExtended = document.getElementById("divExtended");
    const divSlamBar = document.getElementById("divSlamBar");
    const divLeverHandleType = document.getElementById("divLeverHandleType");
    const divLock = document.getElementById("divLock");
    const divLayoutCode = document.getElementById("divLayoutCode");
    const divHandle = document.getElementById("divHandle");
    const divMidrail = document.getElementById("divMidrail");
    const divPetDor = document.getElementById("divPetDor");
    const divTripleLock = document.getElementById("divTripleLock");
    const divLatchBass = document.getElementById("divLatchBass");
    const divBugSeal = document.getElementById("divBugSeal");
    const divDoorCloser = document.getElementById("divDoorCloser");
    const divBoldPatio = document.getElementById("divBoldPatio");
    const divCrossBrace = document.getElementById("divCrossBrace");

    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    divTubeType.classList.add("d-none");

    divFormDetail.classList.add("d-none");
    lblWidth.innerHTML = "width top x middle";
    divWidthMiddle.classList.add("d-none");
    divWidthBotMin.classList.add("d-none");
    divFrameType.classList.add("d-none");
    lblFrameType.innerHTML = "frame type";
    divFrameColour.classList.add("d-none");
    lblFrameColour.innerHTML = "frame colour";
    divFitted.classList.add("d-none");
    divMesh.classList.add("d-none");
    lblMesh.innerHTML = "mesh type";
    divFixing.classList.add("d-none");
    divTop.classList.add("d-none");
    divHingeType.classList.add("d-none");
    divLockType.classList.add("d-none");
    divLockHandling.classList.add("d-none");
    divFrameSize.classList.add("d-none");
    divExtended.classList.add("d-none");
    divSlamBar.classList.add("d-none");
    divLeverHandleType.classList.add("d-none");
    divLock.classList.add("d-none");
    divLayoutCode.classList.add("d-none");
    divHandle.classList.add("d-none");
    divMidrail.classList.add("d-none");
    divPetDor.classList.add("d-none");
    divTripleLock.classList.add("d-none");
    divLatchBass.classList.add("d-none");
    divBugSeal.classList.add("d-none");
    divDoorCloser.classList.add("d-none");
    divBoldPatio.classList.add("d-none");
    divCrossBrace.classList.add("d-none");

    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divTubeType.classList.remove("d-none");

    if (!tubetype) return;
    divFormDetail.classList.remove("d-none");
    // DOOR
    if (blindname.includes("Door") && !blindname.includes("Steel")) {
      lblWidth.innerHTML = "width top x middle";
      divWidthMiddle.classList.remove("d-none");
      divWidthBotMin.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divLayoutCode.classList.remove("d-none");
      divHandle.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
      divPetDor.classList.remove("d-none");
      divTripleLock.classList.remove("d-none");
      divLatchBass.classList.remove("d-none");
      divBugSeal.classList.remove("d-none");
      divDoorCloser.classList.remove("d-none");
      divBoldPatio.classList.remove("d-none");
    }

    if (blindname.includes("Grile") && !blindname.includes("Steel")) {
      lblWidth.innerHTML = "width";
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
    }

    if (blindname.includes("Flyscreen")) {
      lblWidth.innerHTML = "width";
      divFrameColour.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divCrossBrace.classList.remove("d-none");
    }

    if (blindname.includes("Steel")) {
      lblWidth.innerHTML = "width top x middle";
      divWidthMiddle.classList.remove("d-none");
      divWidthBotMin.classList.remove("d-none");
      divFrameType.classList.remove("d-none");
      lblFrameType.innerHTML = "type";
      divFrameColour.classList.remove("d-none");
      lblFrameColour.innerHTML = "colour";
      divFitted.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      lblMesh.innerHTML = "mesh";
      divFixing.classList.remove("d-none");
      divTop.classList.remove("d-none");
      divHingeType.classList.remove("d-none");
      divLockType.classList.remove("d-none");
      divLockHandling.classList.remove("d-none");
      divFrameSize.classList.remove("d-none");
      divExtended.classList.remove("d-none");
      divSlamBar.classList.remove("d-none");
      divLeverHandleType.classList.remove("d-none");
      divLock.classList.remove("d-none");
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
