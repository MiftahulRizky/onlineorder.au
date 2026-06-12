document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Dooe.js loaded successfully");
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
// ===============================================================EVENTS========================================================================
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
      const tubetype = e.target.value;
      await handlerElementVisibility(blindtype, tubetype);
      await bindControls(DESIGNID, blindtype, tubetype);
    }

    if (e.target.id === "controltype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      const controlname = e.target.selectedOptions[0].dataset.name;
      const width = document.getElementById("width").value;
      const frametype = document.getElementById("frametype").value;
      await handlerElementVisibility(blindtype, tubetype, controltype);
      await Promise.all([
        bindSliding(),
        bindStacking(),
        bindTrackless(),
        bindFrameType(blindname, tubetype, controlname, width),
        bindMesh(blindname, controlname, frametype),
        bindHandleSide(blindname, controlname, frametype),
        bindLock(blindname, controlname, frametype),
        bindMidrail(blindname, controlname, frametype),
        bindBugseal(blindname, controlname, frametype),
        bindCloser(blindname, controlname, frametype),
        bindHalf(blindname, controlname, frametype),
        bindInstall(blindname, controlname, frametype),
        bindFixing(blindname, controlname, frametype),
        bindFitted(blindname, controlname, frametype),
        bindRemoval(blindname, controlname, frametype),
        bindPetDoorType(blindname, controlname, frametype),
        bindHalf(blindname, controlname, frametype),
        bindInterlock(blindname, controlname, frametype),
        bindExtras(blindname, tubetype, controlname),
      ]);
      if (["Security Door"].includes(blindname)) {
        await Promise.all([bindFrameColour(blindname, tubetype, controlname)]);
      }
      await Promise.all([handlerSetDefaultValues()]);
    }

    if (e.target.id === "frametype") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = document.getElementById("tubetype").value;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const frametype = e.target.value;
      Promise.all([
        bindFrameColour(blindname, tubetype, controlname),
        bindMesh(blindname, controlname, frametype),
        bindHandleSide(blindname, controlname, frametype),
        bindMidrail(blindname, controlname, frametype),
      ]);

      const divMesh = document.getElementById("divMesh");
      const divMidrail = document.getElementById("divMidrail");
      divMesh.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
      if (["Ultra Barrier Screen Door"].includes(frametype)) {
        divMesh.classList.add("d-none");
      }
      if (["Heavy Duty Diamond"].includes(frametype)) {
        divMidrail.classList.add("d-none");
      }
    }

    if (e.target.id === "framecolour") {
      const framecolour = e.target.value;
      const divCoating = document.getElementById("divCoating");
      divCoating.classList.add("d-none");
      document.getElementById("coatingcolour").value = "";
      if (["Powder Coating"].includes(framecolour)) {
        divCoating.classList.remove("d-none");
      }
      bindCoatingType();
    }

    if (e.target.id === "handleside") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const frametype = document.getElementById("frametype").value;
      const handleside = e.target.value;
      const divHandleHeight = document.getElementById("divHandleHeight");
      const divHandleHeightMM = document.getElementById("divHandleHeightMM");
      divHandleHeight.classList.add("d-none");
      divHandleHeightMM.classList.add("d-none");
      document.getElementById("handleheight").innerHTML = "";
      if (!["Sidelight", ""].includes(handleside)) {
        divHandleHeight.classList.remove("d-none");
      }
      bindHandleHeight(blindname, controlname, frametype);
    }

    if (e.target.id === "handleheight") {
      const handleheight = e.target.value;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;

      const divHandleHeightMM = document.getElementById("divHandleHeightMM");
      document.getElementById("handleheightmm").value = "";
      divHandleHeightMM.classList.add("d-none");
      if (["Hinged Door"].includes(controlname)) {
        if (!["Tulip A Latch", ""].includes(handleheight)) {
          divHandleHeightMM.classList.remove("d-none");
        }
      }

      if (["Sliding Door"].includes(controlname)) {
        if (["Lock Height", "Specify"].includes(handleheight)) {
          divHandleHeightMM.classList.remove("d-none");
        }
      }
    }

    if (e.target.id === "petdoortype") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const frametype = document.getElementById("frametype").value;
      bindPetDoorPosition(blindname, controlname, frametype);
    }

    if (e.target.id === "petdoorposition") {
      const petdoorposition = e.target.value;
      const divPetDorPositionW = document.getElementById("divPetDorPositionW");
      divPetDorPositionW.classList.add("d-none");
      document.getElementById("petdoorpositionw").value = "";
      if (["Specify"].includes(petdoorposition)) {
        divPetDorPositionW.classList.remove("d-none");
      }
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "width") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = document.getElementById("tubetype").value;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const width = e.target.value;
      bindFrameType(blindname, tubetype, controlname, width);
      if (
        ["Security Door"].includes(blindname) ||
        ["N/A"].includes(controlname)
      ) {
        bindFrameColour(blindname, tubetype, controlname);
      }
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
        data: {
          field: "blindtype",
          designid: DESIGNID,
        },
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
        data: {
          field: "tubetype",
          designid,
          blindtype,
        },
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
        const tubetype = select.options[select.selectedIndex].value;
        await handlerElementVisibility(blindtype, tubetype);
        await bindControls(designid, blindtype, tubetype);
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

const bindControls = async (designid, blindtype, tubetype) => {
  const select = document.getElementById("controltype");
  select.innerHTML = "";

  if (!designid || !blindtype || !tubetype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field: "controltype",
          designid,
          blindtype,
          tubetype,
        },
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
        const controltype = select.options[select.selectedIndex].value;
        const controlname = select.options[select.selectedIndex].dataset.name;
        const width = document.getElementById("width").value;
        const frametype = document.getElementById("frametype").value;
        await handlerElementVisibility(blindtype, tubetype, controltype);
        await Promise.all([
          bindSliding(),
          bindStacking(),
          bindTrackless(),
          bindFrameType(blindname, tubetype, controlname, width),
          bindMesh(blindname, controlname, frametype),
          bindHandleSide(blindname, controlname, frametype),
          bindLock(blindname, controlname, frametype),
          bindMidrail(blindname, controlname, frametype),
          bindBugseal(blindname, controlname, frametype),
          bindCloser(blindname, controlname, frametype),
          bindHalf(blindname, controlname, frametype),
          bindInstall(blindname, controlname, frametype),
          bindFixing(blindname, controlname, frametype),
          bindFitted(blindname, controlname, frametype),
          bindRemoval(blindname, controlname, frametype),
          bindPetDoorType(blindname, controlname, frametype),
          bindHalf(blindname, controlname, frametype),
          bindInterlock(blindname, controlname, frametype),
          bindExtras(blindname, tubetype, controlname),
        ]);
        if (
          ["Security Door"].includes(blindname) ||
          ["N/A"].includes(controlname)
        ) {
          await Promise.all([
            bindFrameColour(blindname, tubetype, controlname),
          ]);
        }
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

const bindSliding = () => {
  generateOption("sliding", [
    "Single Sliding Pleated",
    "Double Sliding Pleated",
  ]);
};

const bindStacking = () => {
  generateOption("stacking", [
    "Stacking - Right",
    "Stacking - Left",
    "Stacking - Split",
  ]);
};

const bindTrackless = () => {
  generateOption("trackless", ["Trackless - No"]);
};

const bindFrameType = (blindname, tubetype, controlname, width) => {
  document.getElementById("framecolour").innerHTML = "";
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Flydoor"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push("Screen Door");
      }
    }

    if (["Heavy Duty Diamond"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push("Heavy Duty Diamond");
      }
    }

    if (["Ultra Barrier"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push("Ultra Barrier Screen Door");
      }
    }
  }

  generateOption("frametype", data);
};

const bindFrameColour = (blindname, tubetype, controlname) => {
  if (!blindname) return;
  let data = [];
  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Powder Coating",
        "TBA",
        "Apo Grey",
        "Black",
        "Bronze",
        "Brown",
        "Charcoal",
        "Dune",
        "Hawthorn Green",
        "Monument",
        "Primrose",
        "Silver",
        "Beige",
        "Surf Mist",
        "White",
        "White Birch",
        "Woodland Grey",
      );
    }

    if (["N/A"].includes(controlname)) {
      data.push(
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

  if (["Security Door"].includes(blindname)) {
    if (["Ultra Guard"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push(
          "Apo Grey",
          "Custom Black",
          "Charcoal Satin",
          "Monument Matt",
          "Paperbark",
          "Pearl White",
          "Powder Coating",
          "Primrose",
          "Surfmist",
          "White Birch",
          "Woodland Grey",
        );
      }
    }

    if (["Ultra Wedge"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push(
          "Powder Coating",
          "TBC",
          "Apo Grey",
          "Beige",
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
    }

    if (["SSS"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
        data.push(
          "Powder Coating",
          "TBC",
          "Apo Grey",
          "Bronze",
          "Brown",
          "Charcoal",
          "Clear Anodised",
          "Dune",
          "Mist Green",
          "Pebble",
          "Primrose",
          "Stone Beige",
          "White Birch",
          "Woodland Grey",
          "Paperbark",
        );
      }
    }
  }
  generateOption("framecolour", data);
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

const bindMesh = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Fibreglass Mesh",
        "Aluminium Mesh",
        "Pawproof Mesh  ",
        "Stainless Steel Mesh",
        "Ultra Barrier Mesh",
      );
    }
  }
  generateOption("meshtype", data);
};

const bindHandleSide = (blindname, controlname, frametype) => {
  document.getElementById("handleheight").innerHTML = "";
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "Left",
        "Right",
        "Left - Reciever",
        "Right - Reciever",
        "Stacker (Left Slide)",
        "Stacker (Right Slide)",
      );
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Left",
        "Right",
        "Left - Reciever",
        "Right - Reciever",
        "Sidelight",
      );
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "Left",
        "Right",
        "Left - Reciever",
        "Right - Reciever",
        "Stacker (Left Slide)",
        "Stacker (Right Slide)",
      );
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push("Left", "Right", "Left - Reciever", "Right - Reciever");
    }
  }
  generateOption("handleside", data);
};

const bindHandleHeight = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push("Lock Height", "Handle Height to Centre", "Specify");

      if (frametype.includes("Screen Door")) {
        data.push("Bass Latch", "Batman Morticeed SNIB");
      }
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Lock Height",
        "To Centre of Handle",
        "To Bottom of Tongue",
        "To Centre of Tongue",
        "Specify",
      );

      if (frametype.includes("Screen Door")) {
        data.push("Tulip A Latch");
      }
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push("Lock Height", "Handle Height to Centre", "Specify");
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Lock Height",
        "Centre of Handle",
        "Bot of Tongue",
        "Centre of Tongue",
        "Specify",
      );
    }
  }
  generateOption("handleheight", data);
};

const bindLock = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Black",
        "Bronze",
        "Brown",
        "Hawthorne Green",
        "Primrose",
        "Stone Beige",
        "White",
        "White Birch",
      );
    }
  }
  generateOption("lockcolour", data);
};

const bindMidrail = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "No Midrail",
        "Standard Midrail to Centre",
        "Standard Mid Rail Specify",
      );

      if (!frametype.includes("Screen Door")) {
        data.push(
          "Ultra Barrier Mid Rail to Centre",
          "Ultra Barrier Mid Rail Specify",
        );
      }
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "No Midrail",
        "Vista Mid Rail Specify",
        "Vista Mid Rail to Centre",
      );

      if (frametype.includes("Screen Door")) {
        data.push("Standard Mid rail Specify", "Standard Midrail to Centre");
      }
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push("No Midrail", "Specify", "Midrail to Centre");
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push("Midrail to Centre", "No Midrail", "Specify");
    }
  }
  generateOption("midrail", data);
};

const bindBugseal = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door", "Security Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push("No Bug Seal", "Bug Seal - Thin", "Bug Seal - Wide");
    }
  }
  generateOption("bugseal", data);
};

const bindCloser = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door", "Security Door"].includes(blindname)) {
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "No Closer",
        "Closer - Black",
        "Closer - Bronze",
        "Closer - Brown",
        "Closer - Green",
        "Closer - Primrose",
        "Closer - Stone Beige",
        "Closer - White",
        "Closer - White Birch",
      );
    }
  }
  generateOption("closer", data);
};

const bindInstall = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Installation Safety Door",
        "Installation Screen Door",
        "Pick up",
      );
    }
  }
  generateOption("install", data);
};

const bindFixing = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Hinged Door"].includes(controlname)) {
      data.push("Timber", "Concrete", "Steel", "Aluminium", "Not Specified");
    }
  }
  generateOption("fixing", data);
};

const bindFitted = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Standard Fit",
        "Standard Fit (C)",
        "Standard Fit 3 PT (C)",
        "Standard Fit 3 PT (T)",
        "Supply Only",
      );
    }
  }
  generateOption("fitted", data);
};

const bindRemoval = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Hinged Door"].includes(controlname)) {
      data.push("No Removal", "Removal Only", "Removal and Disposal");
    }
  }
  generateOption("remove", data);
};

const bindPetDoorType = (blindname, controlname, frametype) => {
  document.getElementById("petdoorposition").innerHTML = "";
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "Pet Dr Large - Stone beige",
        "Pet Dr Medium - Stone Beige",
        "Pet Dr Small - Black",
        "Pet Dr Small - Stone Beige",
        "Pet Dr Small - Bronze",
        "Pet Dr Small - Brown",
        "Pet Dr Small - Primrose",
        "Pet Dr Small - White",
        "Pet Dr Medium - Black",
        "Pet Dr Medium - Bronze",
        "Pet Dr Medium - Brown",
        "Pet Dr Medium - Primrose",
        "Pet Dr Medium - White",
        "Pet Dr Large - Black",
        "Pet Dr Large - Bronze",
        "Pet Dr Large - Brown",
        "Pet Dr Large - Primrose",
        "Pet Dr Large - White",
      );
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Pet Dr Small - Black",
        "Pet Dr Small - Bronze",
        "Pet Dr Small - Brown",
        "Pet Dr Small - Primrose",
        "Pet Dr Small - White",
        "Pet Dr Medium - Black",
        "Pet Dr Medium - Bronze",
        "Pet Dr Medium - Brown",
        "Pet Dr Medium - Primrose",
        "Pet Dr Medium - Stone Beige",
        "Pet Dr Medium - White",
        "Pet Dr Large - Black",
        "Pet Dr Large - Bronze",
        "Pet Dr Large - Brown",
        "Pet Dr Large - Primrose",
        "Pet Dr Large - White",
      );
    }
  }
  generateOption("petdoortype", data);
};

const bindPetDoorPosition = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push("Left", "Centre", "Right", "Specify");
    }
  }
  generateOption("petdoorposition", data);
};

const bindHalf = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Black - 1/2 Panel",
        "Bronze - 1/2 Panel",
        "Brown - 1/2 Panel",
        "Hawthorn Green - 1/2 Panel",
        "Primrose - 1/2 Panel",
        "Silver - 1/2 Panel",
        "Stone Beige - 1/2 Panel",
        "White - 1/2 Panel",
        "White Birch - 1/2 Panel",
      );
    }
  }
  generateOption("half", data);
};

const bindInterlock = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "Door Interlock Large (2)",
        "Door Interlock Flat (3)",
        "Door Interlock Small (1)",
        "Door Interlock F (4)",
        "Triple lock Slider Charge (Fitted in Factory)",
      );
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Hinges X 3",
        "Hinges x 4",
        "Stop Bead Additional",
        "Track Jamb Adaptor Long",
        "Track Jamb Adaptor Short",
        "Safety Door Deadlock - Without Barrel - Supply Only",
        "Safety Door Deadlock With Barrel (Supply Only)",
        "Triple Lock Hinged Charge",
      );
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      data.push(
        "Door Interlock Large (2)",
        "Door Interlock Flat (3)",
        "Door Interlock Small (1)",
        "Door Interlock F (4)",
      );
    }
    if (["Hinged Door"].includes(controlname)) {
      data.push(
        "Stop Bead Additional",
        "Track Jamb Adaptor Long",
        "Track Jamb Adaptor Short",
      );
    }
  }
  generateOption("interlock", data);
};

let extrasState = [];
const extrasRef = { current: null };
const bindExtras = (blindname, tubetype, controlname) => {
  if (!blindname) return;
  let list = [];

  if (["Basic Door", "Safety Door", "Security Door"].includes(blindname)) {
    if (!["SSS"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door", "N/A"].includes(controlname)) {
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
          { name: "Angle 25 x 50mm", unit: "mm" },
          { name: "Angle 20 x 40mm", unit: "mm" },
          { name: "Angle 20 x 25mm", unit: "mm" },
          { name: "Angle 12 x 25mm", unit: "mm" },
          { name: "Angle 12 x 20mm", unit: "mm" },
          { name: "Angle 12 x 12mm", unit: "mm" },
          { name: "J Bead ", unit: "mm" },
          { name: "Packer Hinged - Aluminium 1mm or 2mm", unit: "Qty" },
          { name: "Packer Hinged - Plastic 2mm or 4mm", unit: "Qty" },
        );
      }
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["SSS"].includes(tubetype)) {
      if (["Sliding Door", "Hinged Door"].includes(controlname)) {
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
          { name: "Angle 25 x 50mm", unit: "mm" },
          { name: "Angle 20 x 40mm", unit: "mm" },
          { name: "Angle 20 x 25mm", unit: "mm" },
          { name: "Angle 12 x 25mm", unit: "mm" },
          { name: "Angle 12 x 20mm", unit: "mm" },
          { name: "Angle 12 x 12mm", unit: "mm" },
          { name: "Packer Hinged - Aluminium 1mm or 2mm", unit: "Qty" },
          { name: "Packer Hinged - Plastic 2mm or 4mm", unit: "Qty" },
        );
      }
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
      await bindControls(item.DesignId, item.BlindId, item.TubeType);
      await handlerElementVisibility(
        item.BlindId,
        item.TubeType,
        item.KitId,
        item,
      );
      await Promise.all([
        bindSliding(),
        bindStacking(),
        bindTrackless(),
        bindFrameType(
          item.BlindName,
          item.TubeType,
          item.ControlType,
          item.Width,
        ),
        bindFrameColour(item.BlindName, item.TubeType, item.ControlType),
        bindCoatingType(),
        bindMesh(item.BlindName, item.ControlType, item.FrameType),
        bindHandleSide(item.BlindName, item.ControlType, item.FrameType),
        bindHandleHeight(item.BlindName, item.ControlType, item.FrameType),
        bindLock(item.BlindName, item.ControlType, item.FrameType),
        bindMidrail(item.BlindName, item.ControlType, item.FrameType),
        bindBugseal(item.BlindName, item.ControlType, item.FrameType),
        bindCloser(item.BlindName, item.ControlType, item.FrameType),
        bindHalf(item.BlindName, item.ControlType, item.FrameType),
        bindInstall(item.BlindName, item.ControlType, item.FrameType),
        bindFixing(item.BlindName, item.ControlType, item.FrameType),
        bindFitted(item.BlindName, item.ControlType, item.FrameType),
        bindRemoval(item.BlindName, item.ControlType, item.FrameType),
        bindPetDoorType(item.BlindName, item.ControlType, item.FrameType),
        bindPetDoorPosition(item.BlindName, item.ControlType, item.FrameType),
        bindHalf(item.BlindName, item.ControlType, item.FrameType),
        bindInterlock(item.BlindName, item.ControlType, item.FrameType),
        bindExtras(item.BlindName, item.TubeType, item.ControlType),
      ]);
      if (["Security Door"].includes(item.BlindName)) {
        await Promise.all([
          bindFrameColour(item.BlindName, item.TubeType, item.ControlType),
        ]);
      }
      await Promise.all([handlerSetElementValues(item)]);
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
    document.getElementById("meshtype").value = "Fibreglass Mesh";
  }
};
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
    const divWidthMid = document.getElementById("divWidthMid");
    const divWidthBot = document.getElementById("divWidthBot");
    const divSliding = document.getElementById("divSliding");
    const divStacking = document.getElementById("divStacking");
    const divTrackless = document.getElementById("divTrackless");
    const lblFrame = document.getElementById("lblFrame");
    const divFrameType = document.getElementById("divFrameType");
    const divFrameColour = document.getElementById("divFrameColour");
    const divCoating = document.getElementById("divCoating");
    const divMesh = document.getElementById("divMesh");
    const divHandle = document.getElementById("divHandle");
    const divHandleHeight = document.getElementById("divHandleHeight");
    const divHandleHeightMM = document.getElementById("divHandleHeightMM");
    const divInswing = document.getElementById("divInswing");
    const divLockColour = document.getElementById("divLockColour");
    const divKeyed = document.getElementById("divKeyed");
    const divMidrail = document.getElementById("divMidrail");
    const divBugseal = document.getElementById("divBugseal");
    const divCloser = document.getElementById("divCloser");
    const lblCloser = document.getElementById("lblCloser");
    const divInstall = document.getElementById("divInstall");
    const divFixing = document.getElementById("divFixing");
    const divFitted = document.getElementById("divFitted");
    const divRemove = document.getElementById("divRemove");
    const divPetDor = document.getElementById("divPetDor");
    const divPetDorPositionW = document.getElementById("divPetDorPositionW");
    const divHalf = document.getElementById("divHalf");
    const divInterlock = document.getElementById("divInterlock");
    const lblInterlock = document.getElementById("lblInterlock");
    const divExtras = document.getElementById("divExtras");
    const divMarkUp = document.getElementById("divMarkUp");
    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    divTubeType.classList.add("d-none");
    divControlType.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divMounting.classList.add("d-none");
    divWidthMid.classList.add("d-none");
    divWidthBot.classList.add("d-none");
    divSliding.classList.add("d-none");
    divStacking.classList.add("d-none");
    divTrackless.classList.add("d-none");
    lblFrame.innerHTML = "Grille";
    divFrameType.classList.add("d-none");
    divFrameColour.classList.add("d-none");
    divCoating.classList.add("d-none");
    divMesh.classList.add("d-none");
    divHandle.classList.add("d-none");
    divHandleHeight.classList.add("d-none");
    divHandleHeightMM.classList.add("d-none");
    divInswing.classList.add("d-none");
    divLockColour.classList.add("d-none");
    divKeyed.classList.add("d-none");
    divMidrail.classList.add("d-none");
    divBugseal.classList.add("d-none");
    divCloser.classList.add("d-none");
    lblCloser.innerHTML = "Closer";
    divInstall.classList.add("d-none");
    divFixing.classList.add("d-none");
    divFitted.classList.add("d-none");
    divRemove.classList.add("d-none");
    divPetDor.classList.add("d-none");
    divPetDorPositionW.classList.add("d-none");
    divHalf.classList.add("d-none");
    divInterlock.classList.add("d-none");
    lblInterlock.innerHTML = "interlocks and options";
    divExtras.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindtype) return;
    divTubeType.classList.remove("d-none");
    const blindname = await getItemData(
      `SELECT Name FROM Blinds WHERE Id='${blindtype}'`,
    );

    if (!tubetype) return;
    divControlType.classList.remove("d-none");

    if (!controltype) return;
    divFormDetail.classList.remove("d-none");
    const controlname = await getItemData(
      `SELECT ControlType FROM HardwareKits WHERE Id='${controltype}'`,
    );

    if (controlname == "N/A") {
      divControlType.classList.add("d-none");
    }

    if (["Flydoor", "Heavy Duty Diamond", "Ultra Barrier"].includes(tubetype)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divMesh.classList.remove("d-none");
      divHandle.classList.remove("d-none");
      divKeyed.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
      divPetDor.classList.remove("d-none");
      divBugseal.classList.remove("d-none");
      // divHalf.classList.remove("d-none");
      // divInstall.classList.remove("d-none");
      divExtras.classList.remove("d-none");

      if (["Hinged Door"].includes(controlname)) {
        // divInterlock.classList.remove("d-none");
        divWidthMid.classList.remove("d-none");
        divWidthBot.classList.remove("d-none");
        lblInterlock.innerHTML = "Adaptors and options";
        divInswing.classList.remove("d-none");
        divLockColour.classList.remove("d-none");
        divCloser.classList.remove("d-none");
        // divFixing.classList.remove("d-none");
        // divFitted.classList.remove("d-none");
        // divRemove.classList.remove("d-none");
      }
    }

    if (["Ultra Guard", "Ultra Wedge", "SSS"].includes(tubetype)) {
      lblFrame.innerHTML = "Frame Colour";
      divFrameColour.classList.remove("d-none");
      divHandle.classList.remove("d-none");
      divKeyed.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
      divBugseal.classList.remove("d-none");
      lblInterlock.innerHTML = "Interlocks";
      divExtras.classList.remove("d-none");

      if (["Hinged Door"].includes(controlname)) {
        // divInterlock.classList.remove("d-none");
        divWidthMid.classList.remove("d-none");
        divWidthBot.classList.remove("d-none");
        lblInterlock.innerHTML = "Adaptors";
        divCloser.classList.remove("d-none");
        lblCloser.innerHTML = "Door Closure";
      }
    }

    if (["Retractable Pleated"].includes(tubetype)) {
      divSliding.classList.remove("d-none");
      divStacking.classList.remove("d-none");
      divTrackless.classList.remove("d-none");
      lblFrame.innerHTML = "Frame Colour";
      divFrameColour.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (item) {
      if (["Ultra Barrier Screen Door"].includes(item.FrameType)) {
        divMesh.classList.add("d-none");
      }

      if (["Heavy Duty Diamond"].includes(item.FrameType)) {
        divMidrail.classList.add("d-none");
      }

      if (["Powder Coating"].includes(item.FrameColour)) {
        divCoating.classList.remove("d-none");
      }

      if (!["Sidelight", ""].includes(item.Brace)) {
        divHandleHeight.classList.remove("d-none");
      }

      if (["Hinged Door"].includes(item.ControlType)) {
        if (!["Tulip A Latch", ""].includes(item.SlatSize)) {
          divHandleHeightMM.classList.remove("d-none");
        }
      }

      if (["Sliding Door"].includes(item.ControlType)) {
        if (["Lock Height", "Specify"].includes(item.SlatSize)) {
          divHandleHeightMM.classList.remove("d-none");
        }
      }

      if (["Specify"].includes(item.TrackColour)) {
        divPetDorPositionW.classList.remove("d-none");
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
      "controltype",
      "qty",
      "room",
      "mounting",
      "width",
      "widthmid",
      "widthbot",
      "drop",
      "sliding",
      "stacking",
      "trackless",
      "frametype",
      "framecolour",
      "coatingtype",
      "coatingcolour",
      "meshtype",
      "handleside",
      "handleheight",
      "handleheightmm",
      "inswing",
      "lockcolour",
      "keyed",
      "midrail",
      "bugseal",
      "closer",
      "install",
      "fixing",
      "fitted",
      "remove",
      "petdoortype",
      "petdoorposition",
      "petdoorpositionw",
      "half",
      "interlock",
      "extras",
      "notes",
      "markup",
    ];

    console.log("extrasState:", extrasState);

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
    };

    formData["extras"] = JSON.stringify(extrasState || []);

    fields.forEach((field) => {
      if (field === "extras") return; // skip

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
    widthmid: "WidthMiddle",
    widthbot: "WidthBottom",
    drop: "Drop",
    sliding: "BottomTrackType",
    stacking: "StackPosition",
    trackless: "TilterPosition",
    frametype: "FrameType",
    framecolour: "FrameColour",
    coatingtype: "FrameLeft",
    coatingcolour: "FrameRight",
    meshtype: "MeshType",
    handleside: "Brace",
    handleheight: "SlatSize",
    handleheightmm: "SlatQty",
    inswing: "PortHole",
    lockcolour: "PlungerPin",
    keyed: "Batten",
    midrail: "MidrailCritical",
    bugseal: "FlatType",
    closer: "ChildSafe",
    install: "BracketCover",
    fixing: "Fitting",
    fitted: "Cleat",
    remove: "BracketExtension",
    petdoortype: "TrackType",
    petdoorposition: "TrackColour",
    petdoorpositionw: "WandPosition",
    half: "AcornPlasticColour",
    interlock: "Accessory",
    notes: "Notes",
    markup: "MarkUp",
  };

  // 1. set normal fields
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) return;

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    el.value = value ?? "";

    if (el.value === "0") el.value = "";
  });

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

    const selectEl = document.getElementById(selectId);
    if (selectEl && selectEl.tomselect) {
      selectEl.tomselect.setValue(
        data.map((x) => x.name),
        true,
      );
    }

    renderDynamic({
      containerId: containerId,
      state: data,
    });
  };

  const dynamicFields = [
    {
      key: "AdditionalMotor",
      setState: (val) => (extrasState = val),
      selectId: "extras",
      containerId: "extrasContainer",
    },
    // {
    //   key: "FlatType",
    //   setState: (val) => (cutOutState = val),
    //   selectId: "cutout",
    //   containerId: "cutoutContainer",
    // },
  ];

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
