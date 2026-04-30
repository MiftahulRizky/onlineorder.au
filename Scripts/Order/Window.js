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

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      await handlerElementVisibility(blindtype);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = e.target.value;
      await Promise.all([
        bindMounting(),
        bindMesh(blindname),
        bindFrameType(blindname),
        bindBrace(blindname),
        bindInstall(blindname),
        bindRemove(blindname),
        bindCutOut(blindname),
        bindExtras(blindname),
      ]);
      await handlerElementVisibility(blindtype, tubetype);
    }

    if (e.target.id === "frametype") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const frametype = e.target.value;
      bindFrameColour(blindname, frametype);
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "width") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const width = e.target.value;
      bindMesh(blindname, width);
    }

    if (e.target.id === "notes") {
      let maxLength = 1000;
      let currentLength = e.target.value.length;
      document.querySelector("#notescount").textContent =
        `${currentLength}/${maxLength}`;
    }
  });
});
// ================================================FUNCTION==================================================
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
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        field: "blindtype",
        designid: DESIGNID,
        blindtype: "",
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

const bindTubes = async (designid, blindtype) => {
  const select = document.getElementById("tubetype");
  select.innerHTML = "";

  if (!designid || !blindtype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        field: "tubetype",
        designid,
        blindtype,
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
          `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
        );
        const tubetype = select.value;

        await Promise.all([
          bindMounting(),
          bindMesh(blindname),
          bindFrameType(blindname),
          bindBrace(blindname),
          bindInstall(blindname),
          bindRemove(blindname),
          bindCutOut(blindname),
          bindExtras(blindname),
        ]);
        await handlerElementVisibility(blindtype, tubetype);
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

const bindMesh = (blindname, width) => {
  const sel = document.getElementById("meshtype");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];

  if (["Safety Window"].includes(blindname)) {
    data.push({ value: "Fiberglass", text: "Fiberglass" });
    if (width && width <= 1000) {
      data.push({ value: "Stainless Steel", text: "Stainless Steel" });
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

const bindFrameType = (blindname) => {
  const sel = document.getElementById("frametype");
  document.getElementById("framecolour").innerHTML = "";
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  if (["Safety Window", "Security Window"].includes(blindname)) {
    const list = ["Door Frame", "Grille Frame"];

    list.forEach((ls) => {
      data.push({ value: ls, text: ls });
    });
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

const bindFrameColour = (blindname, frametype) => {
  const sel = document.getElementById("framecolour");
  sel.innerHTML = ""; //reset

  if ((!blindname, !frametype)) return;

  let data = [];
  let list = [];

  if (["Safety Window"].includes(blindname)) {
    list = [
      "Monument",
      "Apo Grey",
      "Paperbark",
      "Black",
      "Bronze",
      "Brown",
      "Charcoal",
      "Dune",
      "Powder Coating",
      "Primrose",
      "Silver (Anodised)",
      "Stone Beige",
      "Surf Mist",
      "White",
      "White Birch",
      "Woodland Grey",
    ];
  }

  if (["Security Window"].includes(blindname)) {
    list = [
      "TBC",
      "Apo Grey",
      "Custom Black",
      "Charcoal Satin",
      "Dune",
      "Monument Matt",
      "Primrose",
      "Silver Anodised",
      "Stone Beige",
      "Surf Mist",
      "Pearl White",
      "Woodland Grey",
    ];
  }

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

const bindBrace = (blindname) => {
  const sel = document.getElementById("brace");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Safety Window"].includes(blindname)) {
    list = [
      "Horizontal Centre Brace",
      "Vertical Centre",
      "Vertical Centre Brace",
      "Vertical Brace Specify",
      "Vertical Specify",
      "Horizontal Brace Specify",
    ];
  }

  if (["Security Window"].includes(blindname)) {
    list = [
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Horizontal Brace Specify",
      "Vertical Brace Specify",
    ];
  }

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

const bindInstall = (blindname) => {
  const sel = document.getElementById("install");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];

  if (["Safety Window"].includes(blindname)) {
    const list = ["Installation", "Pick Up"];

    list.forEach((ls) => {
      data.push({ value: ls, text: ls });
    });
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

const bindRemove = (blindname) => {
  const sel = document.getElementById("remove");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];

  if (["Security Window"].includes(blindname)) {
    const list = ["No Removal", "Removal Only", "Removal and Disposal"];

    list.forEach((ls) => {
      data.push({ value: ls, text: ls });
    });
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

const bindCutOut = (blindname) => {
  const sel = document.getElementById("cutout");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Safety Window", "Security Window"].includes(blindname)) {
    list = [
      "Cutout Side 1",
      "Cutout Width 1",
      "Bottom Cutout 1",
      "Top Cutout 1",
      "Cutout Side 2",
      "Bottom Cutout 2",
      "Cutout Width 2",
      "Top Cutout 2",
    ];
  }

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

const bindExtras = (blindname) => {
  const sel = document.getElementById("extras");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Safety Window"].includes(blindname)) {
    list = [
      "Angle 12 x 12mm",
      "Doggie Door - Perspex 190mm x 260mm",
      "Doggie Door - Perspex 260mm x 400mm",
      "Angle 25 x 70",
      "Angle 12 x 20mm",
      "Angle 12 x 25mm",
      "Angle 20 x 40mm",
      "Angle 25 x 20mm",
      "Angle 50 x 25mm",
      "Casement Bolt",
      "Chain Winder Lockable",
      "Door Frame (Infill for Sliding Door Receiver)",
      "Door Interlock HD10 (LRG 2)",
      "Door Interlock HD2 (FLAT 3)",
      "Door Interlock HD3 (SML 1)",
      "Door Interlock HD9 Type F (4)",
      "Door Posts 19 x 19 (for frame work)",
      "Door Posts 25 x 25 (for frame work)",
      "Door Posts 50 x 50 (for frame work)",
      "Door Track Powdercoating (in addition to std track price)",
      "Door Track J",
      "Door Track P",
      "Door Track ST4",
      "Door Track W",
      "Double Sliding Track Bottom",
      "Double Sliding Track Top",
      "Efi Non Specific",
      "Fit Flyscreen Track per pair",
      "Fit Tim/Alum per piece",
      "Grill Frame for Infill",
      "H Channel in Door to add 30mm to width or drop",
      "Lock Barrel supply only",
      "Lock Barrell Installed",
      "Miscellaneous",
      "Miscellaneous Timber",
      "Patio Bolt",
      "Posts 50mm x 50mm",
      "Powder Coating Minimum",
      "Single Sliding Track Bottom",
      "Single Sliding Track Top",
      "Square Tube 20x20",
      "Stop Bead Additional",
      "Timber Frame 19 x 13mm Finished",
      "Timber Frame 19 x 7mm Finished",
      "Timber Frame 30 x 13mm Finished",
      "Timber Frame 30 x 7mm Finished",
      "Timber Frame 41 x 13mm Finished",
      "Timber Frame 41 x 7mm Finished",
      "Timber Frame 66 x 7mm Finished",
      "Timber Frame 91 x 7mm Finished",
      "Timber Frames 19 x 19 Finished",
      "Timber Frames 19 x 30mm Finished",
      "Timber Frames 19 x 41mm Finished",
      "Timber Frames 19 x 66mm Finished",
      "Timber Frames 19 x 91mm Finished",
      "Timber Frames 30 x 19mm Finished",
      "Timber Frames 30 x 30mm Finished",
      "Timber Frames 30 x 41 Finished",
      "Timber Frames 30 x 66mm Finished",
      "Timber Frames 30 x 91mm Finished",
      "Timber Frames 41 x 19mm Finished",
      "Timber Frames 41 x 30mm Finished",
      "Timber Frames 41 x 41 Finished",
      "Timber Frames 41 x 66mm Finished",
      "Timber Frames 41 x 91mm Finished",
      "Timber Frames 66 x 13 Finished",
      "Timber Frames 66 x 19mm Finished",
      "Timber Frames 66 x 30mm Finished",
      "Timber Frames 66 x 41mm Finished",
      "Timber Frames 66 x 91mm Finished",
      "Timber Frames 91 x 13mm Finished",
      "Timber Frames 91 x 19mm Finished",
      "Timber Frames 91 x 30mm Finished",
      "Timber Frames 91 x 41mm Finished",
      "Timber Frames 91 x 66mm Finished",
      "Timber Frames 91 x 91mm Finished",
      "Track",
      "Track Jamb Adaptor Long",
      "Track Jamb Adaptor Short",
      "U Frame 20 mm sides x 25 mm wide",
      "Whitco Winder Strip",
      "Window Lock",
    ];
  }

  if (["Security Window"].includes(blindname)) {
    list = [
      "Angle 12 x 12mm",
      "Angle 25 x 70",
      "Bugseal Additional Hinged",
      "Doggie Door - Perspex 190mm x 260mm",
      "Doggie Door - Perspex 260mm x 400mm",
      "Door Interlock Additional",
      "Patio Bolt",
      "Angle 12 x 20mm",
      "Angle 20 x 20mm",
      "Angle 20 x 40mm",
      "Angle 25 x 20mm",
      "Angle 50 x 50mm",
      "Chain Winder Lockable",
      "Door Posts 19 x 19 (for frame work)",
      "Door Posts 25 x 25 (for frame work)",
      "Door Posts 50 x 50 (for frame work)",
      "Door Track H ST4",
      "Door Track J HD1",
      "Door Track P ST11",
      "Door Track U Frame 20mm sides x 25mm wide",
      "Door Track W ST8",
      "Powder Coating Minimum",
      "Stop Bead Additional",
      "Whitco Winder Strip",
      "Door Interlock Type 1",
      "Door Interlock Type 2",
      "Door Interlock Type 3",
      "Door Interlock Type F",
      "Double Sliding Track",
      "Fit Flyscreen Track per pair",
      "Miscellaneous",
      "Miscellaneous Scaffold",
      "Miscellaneous Security",
      "Miscellaneous Timber",
      "Single Sliding Track",
      "Timber Frame 19 x 13mm Finished",
      "Timber Frame 19 x 7mm Finished",
      "Timber Frame 30 x 13mm Finished",
      "Timber Frame 30 x 7mm Finished",
      "Timber Frame 41 x 13mm Finished",
      "Timber Frame 41 x 7mm Finished",
      "Timber Frame 66 x 7mm Finished",
      "Timber Frame 91 x 7mm Finished",
      "Timber Frames 19 x 19 Finished",
      "Timber Frames 19 x 30mm Finished",
      "Timber Frames 19 x 41mm Finished",
      "Timber Frames 19 x 66mm Finished",
      "Timber Frames 19 x 91mm Finished",
      "Timber Frames 30 x 19mm Finished",
      "Timber Frames 30 x 30mm Finished",
      "Timber Frames 30 x 41 Finished",
      "Timber Frames 30 x 66mm Finished",
      "Timber Frames 30 x 91mm Finished",
      "Timber Frames 41 x 19mm Finished",
      "Timber Frames 41 x 30mm Finished",
      "Timber Frames 41 x 41 Finished",
      "Timber Frames 41 x 66mm Finished",
      "Timber Frames 41 x 91mm Finished",
      "Timber Frames 66 x 13 Finished",
      "Timber Frames 66 x 19mm Finished",
      "Timber Frames 66 x 30mm Finished",
      "Timber Frames 66 x 41mm Finished",
      "Timber Frames 66 x 91mm Finished",
      "Timber Frames 91 x 13mm Finished",
      "Timber Frames 91 x 19mm Finished",
      "Timber Frames 91 x 30mm Finished",
      "Timber Frames 91 x 41mm Finished",
      "Timber Frames 91 x 66mm Finished",
      "Timber Frames 91 x 91mm Finished",
    ];
  }

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
// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerElementVisibility = async (blindtype, tubetype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divMounting = document.getElementById("divMounting");
    const divMesh = document.getElementById("divMesh");
    const divFrameType = document.getElementById("divFrameType");
    const divFrameColour = document.getElementById("divFrameColour");
    const divBrace = document.getElementById("divBrace");
    const divDualHinges = document.getElementById("divDualHinges");
    const divInstall = document.getElementById("divInstall");
    const divRemove = document.getElementById("divRemove");
    const divCutOut = document.getElementById("divCutOut");
    const divExtras = document.getElementById("divExtras");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divTubeType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divMounting.classList.add("d-none");
    divMesh.classList.add("d-none");
    divFrameType.classList.add("d-none");
    divFrameColour.classList.add("d-none");
    divBrace.classList.add("d-none");
    divDualHinges.classList.add("d-none");
    divInstall.classList.add("d-none");
    divRemove.classList.add("d-none");
    divCutOut.classList.add("d-none");
    divExtras.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id = '${blindtype}'`,
    );
    divTubeType.classList.remove("d-none");

    if (!tubetype) return;
    const tubename = await getItemData(
      `SELECT TubeType FROM Hardwarekits WHERE Id = '${tubetype}'`,
    );
    divFormDetail.classList.remove("d-none");

    if (["Safety Window"].includes(blindname)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      if (["Heavy Duty Diamond"].includes(blindname)) {
        divMesh.classList.remove("d-none");
      }
      divDualHinges.classList.remove("d-none");
      divInstall.classList.remove("d-none");
      divCutOut.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (["Security Window"].includes(blindname)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      divRemove.classList.remove("d-none");
      divCutOut.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (["Basic Window"].includes(blindname)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divExtras.classList.remove("d-none");
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
