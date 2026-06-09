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
      const tubename = e.target.selectedOptions[0].dataset.name;
      const width = document.getElementById("width").value;
      await Promise.all([
        bindMounting(),
        bindMesh(blindname, width),
        bindSlidingType(blindname),
        bindStacking(blindname),
        bindTrackless(blindname),
        bindFrameType(blindname, tubename),
        bindBrace(blindname),
        bindInstall(blindname),
        bindFitting(blindname),
        bindRemove(blindname),
        bindHandle(blindname),
        bindPullCord(blindname),
        bindCutOut(blindname),
        bindExtras(blindname, tubename),
      ]);
      if (["Retractable Flyscreen Pleated"].includes(tubename)) {
        await Promise.all([
          bindFrameColour(blindname, tubename, "colour only"),
        ]);
      }
      await Promise.all([handlerSetDefaultValues()]);
      await handlerElementVisibility(blindtype, tubetype);
    }

    if (e.target.id === "frametype") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubes = document.getElementById("tubetype");
      const tubename = tubes.selectedOptions[0].dataset.name;
      const frametype = e.target.value;
      bindFrameColour(blindname, tubename, frametype);
    }

    if (e.target.id === "framecolour") {
      const framecolour = e.target.value;
      const divCoating = document.getElementById("divCoating");
      document.getElementById("coatingcolour").value = "";
      divCoating.classList.add("d-none");
      if (["Powder Coating"].includes(framecolour)) {
        divCoating.classList.remove("d-none");
      }
      bindCoatingType();
    }

    if (e.target.id === "brace") {
      const divBraceLength = document.getElementById("divBraceLength");
      document.getElementById("bracelength").value = "";
      divBraceLength.classList.add("d-none");
      const brace = e.target.value;
      if (
        !["Horizontal Centre Brace", "Vertical Centre Brace", ""].includes(
          brace,
        )
      ) {
        divBraceLength.classList.remove("d-none");
      }
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
        const tubename = select.selectedOptions[0].dataset.name;
        const width = document.getElementById("width").value;

        await Promise.all([
          bindMounting(),
          bindMesh(blindname, width),
          bindSlidingType(blindname),
          bindStacking(blindname),
          bindTrackless(blindname),
          bindFrameType(blindname, tubename),
          bindBrace(blindname),
          bindInstall(blindname),
          bindFitting(blindname),
          bindRemove(blindname),
          bindHandle(blindname),
          bindPullCord(blindname),
          bindCutOut(blindname),
          bindExtras(blindname, tubename),
        ]);
        if (["Retractable Flyscreen Pleated"].includes(tubename)) {
          await Promise.all([
            bindFrameColour(blindname, tubename, "colour only"),
          ]);
        }
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
  generateOption("mounting", ["Make Size", "Opening Size"]);
};

const bindMesh = (blindname, width) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Window", "Safety Window"].includes(blindname)) {
    list.push("Fibreglass", "Aluminium", "Stainless Steel", "Pawproof");
  }

  generateOption("meshtype", list);
};

const bindSlidingType = (blindname) => {
  if (!blindname) return;

  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list.push("Single Sliding Pleated", "Double Sliding Pleated");
  }

  generateOption("slidingtype", list);
};

const bindStacking = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list.push("Stacking - Right", "Stacking - Left", "Stacking - Split");
  }

  generateOption("stacking", list);
};

const bindTrackless = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list.push("Trackless - No");
  }

  generateOption("trackless", list);
};

const bindFrameType = (blindname, tubename) => {
  document.getElementById("framecolour").innerHTML = "";

  if (!blindname || !tubename) return;
  let list = [];

  if (["Safety Window", "Security Window"].includes(blindname)) {
    list.push("Grille Frame", "Door Frame");
  }

  if (["Basic Window"].includes(blindname)) {
    if (["Flyscreens"].includes(tubename)) {
      list.push("21x9 Frame", "25x11 Frame", "35x11 Frame");
    }
    if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
      list.push("Door", "Window");
    }
  }

  generateOption("frametype", list);
};

const bindFrameColour = (blindname, tubename, frametype) => {
  if (!blindname || !tubename || !frametype) return;

  let list = [];

  if (["Safety Window"].includes(blindname)) {
    list.push(
      "Monument",
      "Apo Grey",
      "Black",
      "Bronze",
      "Brown",
      "Charcoal",
      "Dune",
      "Hawtorn green",
      "Powder Coating",
      "Primrose",
      "Silver (Anodised)",
      "Beige",
      "Surf Mist",
      "White",
      "White Birch",
      "Woodland Grey",
    );
  }

  if (["Security Window"].includes(blindname)) {
    if (["Ultra Guard"].includes(tubename)) {
      list.push(
        "TBC",
        "Apo Grey",
        "Custom Black",
        "Charcoal Satin",
        "Monument Matt",
        "Primrose",
        "Surf Mist",
        "Paperbark",
        "Pearl White",
        "White Birch",
        "Woodland Grey",
      );
    }

    if (["Ultra Wedge"].includes(tubename)) {
      list.push(
        "Powder Coating",
        "TBC",
        "Apo Grey",
        "Bicrh White",
        "Black",
        "Chharcoal",
        "Monument",
        "Primrose",
        "Silver",
        "Surfmist",
        "White",
        "Woodland Grey",
      );
    }

    if (["SSS"].includes(tubename)) {
      list.push(
        "Powder Coating",
        "TBC",
        "Apo Grey",
        "Anotec Off White",
        "Bronze",
        "Brown",
        "Cedar",
        "Charcoal",
        "Claret",
        "Clear Anodised",
        "Deep Ocean",
        "Dune",
        "Hawthorn Green",
        "Notre Dame",
        "Primrose",
        "Stone Beige",
        "White Birch",
        "Woodland Grey",
        "Surmist",
        "Paperbark",
        "Jasper",
      );
    }
  }

  if (["Basic Window"].includes(blindname)) {
    if (["Flyscreens"].includes(tubename)) {
      list.push(
        "Apo Grey",
        "Beige",
        "Black",
        "Bronze",
        "Brown",
        "Charcoal",
        "Deep Ocean",
        "Dune",
        "Hawthorne Green",
        "Jasper",
        "Monument",
        "Notre Dame",
        "Powder Coating",
        "Primrose",
        "Silver (Anodised)",
        "Surf Mist",
        "TBA",
        "White",
        "White Birch",
        "Woodland Grey",
      );
    }
    if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
      list.push("White", "Black", "Powder Coating");
    }
    if (["Retractable Flyscreen Pleated"].includes(tubename)) {
      list.push(
        "White",
        "Black",
        "Clear Anodised",
        "Powder Coating",
        "White Birch",
        "Primrose",
        "Monument",
      );
    }
  }

  generateOption("framecolour", list);
};

const bindCoatingType = () => {
  let data = [];

  data.push(
    "Dulux Standard / Duralloy / Surreal Effect",
    "Dulux Precious / D1000 / Duratec Zeus",
    "Dulux Alphatec",
    "Dulux Duratec Eternity / Electro",
    "Dulux Duratec Elements",
    "Dulux Duratex Intensity",
  );
  generateOption("coatingtype", data);
};

const bindBrace = (blindname) => {
  if (!blindname) return;

  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list.push(
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Horizontal Brace/s Specify",
      "Vertical Brace/ Specify",
    );
  }
  if (["Safety Window"].includes(blindname)) {
    list.push(
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Vertical Brace Specify",
      "Horizontal Brace Specify",
    );
  }

  if (["Security Window"].includes(blindname)) {
    list.push(
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Horizontal Brace Specify",
      "Vertical Brace Specify",
    );
  }

  generateOption("brace", list);
};

const bindInstall = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Window", "Safety Window"].includes(blindname)) {
    list.push("Pick Up");
  }

  generateOption("install", list);
};

const bindFitting = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list.push("Screen Port / Trap Door");
  }

  generateOption("fitting", list);
};

const bindRemove = (blindname) => {
  if (!blindname) return;
  let list = ["Removal Only", "Removal and Disposal"];
  generateOption("remove", list);
};

const bindHandle = (blindname) => {
  if (!blindname) return;
  let list = [];
  if (["Basic Window"].includes(blindname)) {
    list.push("Handle - Front", "Handle - Back", "Handle - Dual");
  }

  generateOption("handle", list);
};

const bindPullCord = (blindname) => {
  if (!blindname) return;
  let list = [];
  if (["Basic Window"].includes(blindname)) {
    list.push("Pullcord - Yes", "Pullcord - No");
  }

  generateOption("pullcord", list);
};

let cutOutState = [];
const cutOutRef = { current: null };
const bindCutOut = (blindname) => {
  if (!blindname) return;
  let list = [];

  if (["Safety Window", "Security Window"].includes(blindname)) {
    list.push(
      { name: "Cutout Side 1", unit: "mm" },
      { name: "Cutout Width 1", unit: "mm" },
      { name: "Bottom Cutout 1", unit: "mm" },
      { name: "Top Cutout 1", unit: "mm" },
      { name: "Cutout Side 2", unit: "mm" },
      { name: "Bottom Cutout 2", unit: "mm" },
      { name: "Cutout Width 2", unit: "mm" },
      { name: "Top Cutout 2", unit: "mm" },
    );
  }

  applyToSelect({
    selector: "#cutout",
    list,
    getState: () => cutOutState,
    setState: (val) => (cutOutState = val),
    render: (state) =>
      renderDynamic({
        containerId: "cutoutContainer",
        state,
        setState: (val) => (cutOutState = val),
      }),
    instanceRef: cutOutRef,
  });
};

let extrasState = [];
const extrasRef = { current: null };
const bindExtras = (blindname, tubename) => {
  if (!blindname) return;

  let list = [];

  if (
    ["Safety Window", "Basic Window", "Security Window"].includes(blindname)
  ) {
    if (!["SSS"].includes(tubename) && !["Flyscreens"].includes(tubename)) {
      list.push(
        { name: "Closer Hingled", unit: "Qty" },
        { name: "Bug Seal with Short Fur", unit: "mm" },
        { name: "Bug Seal with Long Fur", unit: "mm" },
        { name: "Bead Stop", unit: "mm" },
        { name: "Magnetic Sliding Latch", unit: "Qty" },
        { name: "Latch - Hinged", unit: "Qty" },
        { name: "Lock - Hinged", unit: "Qty" },
        { name: "Lock - Sliding", unit: "Qty" },
        { name: "Barrel Lock with Keys", unit: "Qty" },
        { name: "Barrel (Short) Lock with Keys", unit: "Qty" },
        { name: "Triple Lock Upgrade", unit: "Qty" },
        { name: "Bolt Patio Lockable", unit: "Qty" },
        { name: "Doggie Door - Perspex (190mm x 240mm)", unit: "Qty" },
        { name: "Doggie Door - Perspex (225mm x 350mm)", unit: "Qty" },
        { name: "Doggie Door - Perspex (260mm x 400mm)", unit: "Qty" },
        { name: "Mesh Magic Restorer", unit: "Qty" },
        { name: "Striker Plate Extension", unit: "Qty" },
        { name: "Hinge", unit: "Qty" },
        { name: "Wheels, Standard", unit: "Qty" },
        { name: "Wheels, Security", unit: "Qty" },
        { name: "Track - Pip (ST11)", unit: "mm" },
        { name: "Track - U Channel (20x25x20)", unit: "mm" },
        { name: "Track - J (HD1) - 20mm x 13mm", unit: "mm" },
        { name: "Track - W (ST8) - 25mm x 25mm", unit: "mm" },
        { name: "Track - H Offset (ST4) - 28mm x 28mm", unit: "mm" },
        { name: "Jamb Adaptor - Standard Leg", unit: "mm" },
        { name: "Jamb Adaptor  - Long Leg", unit: "mm" },
        { name: "Interlock Type 1", unit: "mm" },
        { name: "Interlock Type 2", unit: "mm" },
        { name: "Interlock Type 3", unit: "mm" },
        { name: "Interlock Type F", unit: "mm" },
        { name: "Angle - 25 x 50mm", unit: "mm" },
        { name: "Angle - 20 x 40mm", unit: "mm" },
        { name: "Angle - 20 x 25mm", unit: "mm" },
        { name: "Angle - 12 x 25mm", unit: "mm" },
        { name: "Angle - 12 x 20mm", unit: "mm" },
        { name: "Angle - 12 x 12mm", unit: "mm" },
        { name: "J Bead ", unit: "mm" },
        { name: "Packer Hinged - Aluminium 1mm or 2mm", unit: "Qty" },
        { name: "Packer Hinged - Plastic 2mm or 4mm", unit: "Qty" },
        { name: "Single Sliding Track - Top", unit: "mm" },
        { name: "Single Sliding Track - Bottom", unit: "mm" },
        { name: "Double Sliding Track - Top", unit: "mm" },
        { name: "Double Sliding Track - Bottom", unit: "mm" },
      );
    }

    if (["Flyscreens"].includes(tubename)) {
      list.push(
        { name: "Flyscreen Plunger Pins", unit: "Qty" },
        { name: "Flyscreen Top Clips", unit: "Qty" },
        { name: "Flyscreen Turn Buttons", unit: "Qty" },
        { name: "Flyscreen Beading", unit: "Qty" },
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Pull Tab", unit: "Qty" },
        { name: "Single Sliding Track - Top", unit: "mm" },
        { name: "Single Sliding Track - Bottom", unit: "mm" },
        { name: "Double Sliding Track - Top", unit: "mm" },
        { name: "Double Sliding Track - Bottom", unit: "mm" },
      );
    }
  }

  if (["Security Window"].includes(blindname)) {
    if (["SSS"].includes(tubename)) {
      list.push(
        { name: "Closer Hingled", unit: "Qty" },
        { name: "Bug Seal with Short Fur", unit: "mm" },
        { name: "Bug Seal with Long Fur", unit: "mm" },
        { name: "Bead Stop", unit: "mm" },
        { name: "Bolt Patio Lockable", unit: "Qty" },
        { name: "Doggie Door - Perspex (190mm x 240mm)", unit: "Qty" },
        { name: "Doggie Door - Perspex (225mm x 350mm)", unit: "Qty" },
        { name: "Doggie Door - Perspex (260mm x 400mm)", unit: "Qty" },
        { name: "Mesh Magic Restorer", unit: "Qty" },
        { name: "Striker Plate Extension", unit: "Qty" },
        { name: "Track - Pip (ST11)", unit: "mm" },
        { name: "Track - U Channel (20x25x20)", unit: "mm" },
        { name: "Track - J (HD1) - 20mm x 13mm", unit: "mm" },
        { name: "Track - W (ST8) - 25mm x 25mm", unit: "mm" },
        { name: "Track - H Offset (ST4) - 28mm x 28mm", unit: "mm" },
        { name: "Jamb Adaptor - Standard Leg", unit: "mm" },
        { name: "Jamb Adaptor  - Long Leg", unit: "mm" },
        { name: "Interlock Type 1", unit: "mm" },
        { name: "Interlock Type 2", unit: "mm" },
        { name: "Interlock Type 3", unit: "mm" },
        { name: "Interlock Type F", unit: "mm" },
        { name: "Angle - 25 x 50mm", unit: "mm" },
        { name: "Angle - 20 x 40mm", unit: "mm" },
        { name: "Angle - 20 x 25mm", unit: "mm" },
        { name: "Angle - 12 x 25mm", unit: "mm" },
        { name: "Angle - 12 x 20mm", unit: "mm" },
        { name: "Angle - 12 x 12mm", unit: "mm" },
        { name: "Packer Hinged - Aluminium 1mm or 2mm", unit: "Qty" },
        { name: "Packer Hinged - Plastic 2mm or 4mm", unit: "Qty" },
        { name: "Single Sliding Track - Top", unit: "mm" },
        { name: "Single Sliding Track - Bottom", unit: "mm" },
        { name: "Double Sliding Track - Top", unit: "mm" },
        { name: "Double Sliding Track - Bottom", unit: "mm" },
      );
    }
  }

  applyToSelect({
    selector: "#extras",
    list,
    getState: () => extrasState,
    setState: (val) => (extrasState = val),
    render: (state) =>
      renderDynamic({
        containerId: "extrasContainer",
        state,
        setState: (val) => (extrasState = val),
      }),
    instanceRef: extrasRef,
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
      await Promise.all([
        bindMounting(),
        bindMesh(item.BlindName, item.Width),
        bindSlidingType(item.BlindName),
        bindStacking(item.BlindName),
        bindTrackless(item.BlindName),
        bindFrameType(item.BlindName, item.TubeType),
        bindFrameColour(item.BlindName, item.TubeType, item.FrameType),
        bindCoatingType(),
        bindBrace(item.BlindName),
        bindInstall(item.BlindName),
        bindFitting(item.BlindName),
        bindRemove(item.BlindName),
        bindHandle(item.BlindName),
        bindPullCord(item.BlindName),
        bindCutOut(item.BlindName),
        bindExtras(item.BlindName, item.TubeType),
      ]);
      if (["Retractable Flyscreen Pleated"].includes(item.TubeType)) {
        await Promise.all([
          bindFrameColour(item.BlindName, item.TubeType, "colour only"),
        ]);
      }
      await Promise.all([handlerSetElementValues(item)]);
      await handlerElementVisibility(item.BlindId, item.KitId, item);
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};
// ----------------------------------------------|| Handler Functions ||---------------------------------------
const handlerSetDefaultValues = () => {
  if (ITEMACTION == "AddItem") {
    document.getElementById("meshtype").value = "Fibreglass";
  }
};
const handlerElementVisibility = async (blindtype, tubetype, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const divFormDetail = document.getElementById("divFormDetail");
    const divMounting = document.getElementById("divMounting");
    const divMesh = document.getElementById("divMesh");
    const divSlidingType = document.getElementById("divSlidingType");
    const divStacking = document.getElementById("divStacking");
    const divTrackless = document.getElementById("divTrackless");
    const divFrameType = document.getElementById("divFrameType");
    const divFrameColour = document.getElementById("divFrameColour");
    const divCoating = document.getElementById("divCoating");
    const divBrace = document.getElementById("divBrace");
    const divBraceLength = document.getElementById("divBraceLength");
    const divDualHinges = document.getElementById("divDualHinges");
    const divInstall = document.getElementById("divInstall");
    const divFitting = document.getElementById("divFitting");
    const divRemove = document.getElementById("divRemove");
    const divHandle = document.getElementById("divHandle");
    const divPullCord = document.getElementById("divPullCord");
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
    divSlidingType.classList.add("d-none");
    divStacking.classList.add("d-none");
    divTrackless.classList.add("d-none");
    divFrameType.classList.add("d-none");
    divFrameColour.classList.add("d-none");
    divCoating.classList.add("d-none");
    divBrace.classList.add("d-none");
    divBraceLength.classList.add("d-none");
    divDualHinges.classList.add("d-none");
    divInstall.classList.add("d-none");
    divFitting.classList.add("d-none");
    divRemove.classList.add("d-none");
    divHandle.classList.add("d-none");
    divPullCord.classList.add("d-none");
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
      if (["Heavy Duty Diamond"].includes(tubename)) {
        divMesh.classList.remove("d-none");
      }
      // divDualHinges.classList.remove("d-none");
      // divInstall.classList.remove("d-none");
      // divCutOut.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (["Security Window"].includes(blindname)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      // divRemove.classList.remove("d-none");
      // divCutOut.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (["Basic Window"].includes(blindname)) {
      if (["Flyscreens"].includes(tubename)) {
        divFrameType.classList.remove("d-none");
        divFrameColour.classList.remove("d-none");
        divMesh.classList.remove("d-none");
        divBrace.classList.remove("d-none");
        // divInstall.classList.remove("d-none");
        divFitting.classList.remove("d-none");
        divExtras.classList.remove("d-none");
      }
      if (["Retractable Flyscreen Pleated"].includes(tubename)) {
        divSlidingType.classList.remove("d-none");
        divStacking.classList.remove("d-none");
        divTrackless.classList.remove("d-none");
        divFrameColour.classList.remove("d-none");
      }
      if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
        divFrameType.classList.remove("d-none");
        divFrameColour.classList.remove("d-none");
        divHandle.classList.remove("d-none");
        divPullCord.classList.remove("d-none");
      }
      divExtras.classList.remove("d-none");
    }

    if (item) {
      if (["Powder Coating"].includes(item.FrameColour)) {
        divCoating.classList.remove("d-none");
      }
      if (
        !["Horizontal Centre Brace", "Vertical Centre Brace", ""].includes(
          item.Brace,
        )
      ) {
        divBraceLength.classList.remove("d-none");
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
      "mounting",
      "width",
      "drop",
      "meshtype",
      "slidingtype",
      "stacking",
      "trackless",
      "frametype",
      "framecolour",
      "coatingtype",
      "coatingcolour",
      "brace",
      "bracelength",
      "dualhinges",
      "install",
      "fitting",
      "remove",
      "handle",
      "pullcord",
      "cutout",
      "extras",
      "notes",
      "markup",
    ];

    // Sebelum submit
    console.log("extrasState:", extrasState);
    console.log("cutOutState:", cutOutState);
    // return;

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
    };

    formData["extras"] = JSON.stringify(extrasState || []);
    formData["cutout"] = JSON.stringify(cutOutState || []);

    fields.forEach((field) => {
      if (field === "extras" || field === "cutout") return; // skip
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
    tubetype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    meshtype: "MeshType",
    slidingtype: "BottomTrackType",
    stacking: "StackPosition",
    trackless: "TilterPosition",
    frametype: "FrameType",
    framecolour: "FrameColour",
    coatingtype: "FrameLeft",
    coatingcolour: "FrameRight",
    brace: "Brace",
    bracelength: "TrackLength",
    dualhinges: "BracketOption",
    install: "BracketCover",
    fitting: "Fitting",
    remove: "BracketExtension",
    handle: "PortHole",
    pullcord: "PlungerPin",
    cutout: "FlatType",
    notes: "Notes",
    markup: "MarkUp",
  };

  // ===============================
  // 1. SET NORMAL INPUT
  // ===============================
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";
    if (value === "0") value = "";

    el.value = value ?? "";
  });

  // ===============================
  // 2. HELPER DYNAMIC (GENERIC)
  // ===============================
  const applyDynamicFromData = ({
    jsonString,
    setState,
    selectId,
    containerId,
  }) => {
    let data = [];

    try {
      data = jsonString ? JSON.parse(jsonString) : [];
    } catch (e) {
      console.error("Invalid JSON", e);
      data = [];
    }

    // set state
    setState(data);

    // set TomSelect value (trigger change)
    const selectEl = document.getElementById(selectId);
    if (selectEl && selectEl.tomselect) {
      selectEl.tomselect.setValue(
        data.map((x) => x.name),
        true,
      );
    }

    // render ulang (biar pasti sinkron)
    renderDynamic({
      containerId: containerId,
      state: data,
    });
  };

  // ===============================
  // 3. CONFIG SEMUA DYNAMIC FIELD
  // ===============================
  const dynamicFields = [
    {
      key: "AdditionalMotor",
      setState: (val) => (extrasState = val),
      selectId: "extras",
      containerId: "extrasContainer",
    },
    {
      key: "FlatType",
      setState: (val) => (cutOutState = val),
      selectId: "cutout",
      containerId: "cutoutContainer",
    },
  ];

  // ===============================
  // 4. APPLY SEMUA DYNAMIC FIELD
  // ===============================
  dynamicFields.forEach((field) => {
    applyDynamicFromData({
      jsonString: itemData[field.key],
      setState: field.setState,
      selectId: field.selectId,
      containerId: field.containerId,
    });
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

const generateOption = (elementId, list = []) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  let validateLength = 1;
  switch (elementId) {
    case "trackless":
    case "frametype":
    case "fitting":
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

const applyToSelect = ({
  selector,
  list,
  getState,
  setState,
  render,
  instanceRef,
}) => {
  const data = list
    .map((ls) => ({
      value: ls.name,
      text: ls.name,
      unit: ls.unit,
    }))
    .sort((a, b) => a.text.localeCompare(b.text));

  if (!instanceRef.current) {
    instanceRef.current = new TomSelect(selector, {
      maxItems: null,
      create: false,
    });

    instanceRef.current.on("change", (value) => {
      const selected = value ? (Array.isArray(value) ? value : [value]) : [];

      let currentState = getState();

      let newState = currentState.filter((x) => selected.includes(x.name));

      selected.forEach((name) => {
        if (!newState.find((x) => x.name === name)) {
          const option = instanceRef.current.options[name];

          newState.push({
            name,
            unit: option?.unit || "Qty",
            value: "",
          });
        }
      });

      setState(newState);
      render(newState);
    });
  }

  const ts = instanceRef.current;

  ts.clear();
  ts.clearOptions();

  data.forEach((item) => {
    ts.addOption({
      value: item.value,
      text: item.text.toUpperCase(),
      unit: item.unit,
    });
  });

  ts.refreshOptions(false);

  // reset
  setState([]);
  render([]);
};

const renderDynamic = ({ containerId, state, setState }) => {
  const container = document.getElementById(containerId);
  if (!container) return;

  container.innerHTML = "";

  state.forEach((item, index) => {
    container.innerHTML += `
      <div class="row mb-2 dynamic-row">

        <div class="col-7">
          <input type="text"
                 class="form-control"
                 value="${item.name}"
                 readonly />
        </div>

        <div class="col-5">
          <div class="input-group">
            <input type="number"
                   class="form-control dynamic-value"
                   data-index="${index}"
                   value="${item.value || ""}"
                   placeholder="Enter ${item.unit}" />
            <span class="input-group-text">${item.unit}</span>
          </div>
        </div>

      </div>
    `;
  });

  container.querySelectorAll(".dynamic-value").forEach((input) => {
    input.addEventListener("input", (e) => {
      const index = e.target.dataset.index;
      const value = e.target.value;

      state[index].value = value;

      setState([...state]); // trigger update
    });
  });
};
