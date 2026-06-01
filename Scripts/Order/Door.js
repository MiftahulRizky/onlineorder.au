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
        bindExtras(blindname, controlname),
      ]);
      if (["Security Door"].includes(blindname)) {
        await Promise.all(bindFrameColour(blindname, controlname));
      }
    }

    if (e.target.id === "frametype") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const frametype = e.target.value;
      Promise.all([
        bindFrameColour(blindname, controlname),
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

    if (e.target.id === "handleside") {
      const blinds = document.getElementById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controls = document.getElementById("controltype");
      const controlname = controls.selectedOptions[0].dataset.name;
      const frametype = document.getElementById("frametype").value;
      const handleside = e.target.value;
      document.getElementById("handleheight").innerHTML = "";
      if (handleside) {
        bindHandleHeight(blindname, controlname, frametype);
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
        bindFrameColour(blindname, controlname);
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
          bindExtras(blindname, controlname),
        ]);
        if (
          ["Security Door"].includes(blindname) ||
          ["N/A"].includes(controlname)
        ) {
          await Promise.all([bindFrameColour(blindname, controlname)]);
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
        data.push(
          "Screen Door (up to 865mm)",
          "Screen Door (up to 1035mm)",
          "Screen Door (up to 1315mm & more)",
        );
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

const bindFrameColour = (blindname, controlname) => {
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
        "Paperbark",
        "Primrose",
        "Silver",
        "Stone Beige",
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
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Apo Grey",
        "Custom Black",
        "Charcoal Satin",
        "Dune",
        "Monument Matt",
        "Powder Coating",
        "Primrose",
        "Surfmist",
        "Silver",
        "White",
        "Stone Beige",
        "White Birch",
        "Woodland Grey",
      );
    }
  }
  generateOption("framecolour", data);
};

const bindMesh = (blindname, controlname, frametype) => {
  if (!blindname) return;
  let data = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door", "Hinged Door"].includes(controlname)) {
      data.push(
        "Fibreglass Mesh",
        "Sunlight Security Mesh",
        "Alum (Std)",
        "Pawproof (1000)",
        "Pawproof (1520)",
        "Stainless (1000)",
        "Stainless (1300)",
      );

      if (!frametype.includes("Screen Door (up to")) {
        data.push(
          "Ultra Barrier Mesh (1010x2110)",
          "Ultra Barrier Mesh (1010x2500)",
          "Ultra Barrier Mesh (1310x2110)",
          "Ultra Barrier Mesh (1310x2500)",
          "Ultra Barrier Mesh (865x2110)",
          "Ultra Barrier Mesh (865x2500)",
        );
      }
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

      if (frametype.includes("Screen Door (up to")) {
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

      if (frametype.includes("Screen Door (up to")) {
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

      if (!frametype.includes("Screen Door (up to")) {
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

      if (frametype.includes("Screen Door (up to")) {
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
const bindExtras = (blindname, controlname) => {
  const sel = document.getElementById("extras");
  sel.innerHTML = ""; //reset

  if (!blindname) return;
  if (tomExtras) {
    tomExtras.destroy();
    tomExtras = null;
  }

  let data = [];
  let list = [];

  if (["Basic Door", "Safety Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      list = [
        { name: "Angle 25 x 70", unit: "mm" },
        { name: "Bugseal Additional Sliding", unit: "mm" },
        { name: "Door Frame (Infill for Sliding Door Receiver)", unit: "mm" },
        { name: "Door Posts 19 x 19 (for frame work)", unit: "mm" },
        { name: "Door Posts 50 x 25 (for frame work)", unit: "mm" },
        {
          name: "Safety Door Deadlock - Without Barrel - Supply Only",
          unit: "Qty",
        },
        { name: "Safety Door Deadlock With Barrel (Supply Only)", unit: "Qty" },
        { name: "Grill Frame for Infill", unit: "mm" },
        { name: "H Channel in Door to add 30mm to width or drop", unit: "Qty" },
        { name: "U Frame 20mm sides x 25 wide", unit: "mm" },
        { name: "Door Posts 25 x 25 (for frame work)", unit: "mm" },
        { name: "Door Posts 50 x 50 (for frame work)", unit: "mm" },
        { name: "Stop Bead Additional", unit: "Qty" },
        { name: "Door Track H ST4", unit: "mm" },
        { name: "Door Track J HD1", unit: "mm" },
        { name: "Door Track P- ST11", unit: "mm" },
        { name: "Door Track W- ST8", unit: "mm" },
        {
          name: "Door Track Powdercoating (in addition to std track price)",
          unit: "mm",
        },
        { name: "Powder Coating Minimum", unit: "Qty" },
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Lock Barrell Installed", unit: "Qty" },
        { name: "Lock Barrel supply only", unit: "Qty" },
        { name: "Lock Barrel supplied by customer", unit: "Qty" },
        { name: "Patio Bolt", unit: "Qty" },
        { name: "Miscellaneous Doors", unit: "Qty" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Miscellaneous Timber", unit: "Qty" },
        { name: "Timber Frame 19 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 19 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 19 Finished", unit: "mm" },
        { name: "Timber Frames 19 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 30 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 41 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 66 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 13 Finished", unit: "mm" },
        { name: "Timber Frames 66 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 91 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 91mm Finished", unit: "mm" },
      ];
    }

    if (["Hinged Door"].includes(controlname)) {
      list = [
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Door Posts 19 x 19 (for frame work)", unit: "mm" },
        { name: "Door Posts 25 x 25 (for frame work)", unit: "mm" },
        { name: "Grill Frame for Infill", unit: "mm" },
        { name: "Lock Barrel Supplied by customer", unit: "Qty" },
        { name: "Lock Barrel supply only", unit: "Qty" },
        { name: "Lock Barrell Installed", unit: "Qty" },
        { name: "Powder Coating Minimum", unit: "Qty" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Door Frame (Infill for Sliding Door Receiver)", unit: "mm" },
        { name: "Door Posts 50 x 50 (for frame work)", unit: "mm" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Patio Bolt", unit: "Qty" },
        { name: "Timber Frame 19 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 19 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 66 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 91 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 19 Finished", unit: "mm" },
        { name: "Timber Frames 19 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 30 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 41 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 13 Finished", unit: "mm" },
        { name: "Timber Frames 66 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 91mm Finished", unit: "mm" },
      ];
    }

    if (["N/A"].includes(controlname)) {
      list = [
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Doggie Door - Perspex 190mm x 260mm", unit: "Qty" },
        { name: "Doggie Door - Perspex 260mm x 400mm", unit: "Qty" },
        { name: "Angle 25 x 70", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Casement Bolt", unit: "Qty" },
        { name: "Chain Winder Lockable", unit: "Qty" },
        { name: "Door Frame (Infill for Sliding Door Receiver)", unit: "mm" },
        { name: "Door Interlock HD10 (LRG 2)", unit: "Qty" },
        { name: "Door Interlock HD2 (FLAT 3)", unit: "Qty" },
        { name: "Door Interlock HD3(SML 1)", unit: "Qty" },
        { name: "Door Interlock HD9 Type F (4)", unit: "Qty" },
        { name: "Door Posts 19 x 19 (for frame work)", unit: "mm" },
        { name: "Door Posts 25 x 25 (for frame work)", unit: "mm" },
        { name: "Door Posts 50 x 50 (for frame work)", unit: "mm" },
        {
          name: "Door Track Powdercoating (in addition to std track price)",
          unit: "mm",
        },
        { name: "Door Track J", unit: "mm" },
        { name: "Door Track P", unit: "mm" },
        { name: "Door Track ST4", unit: "mm" },
        { name: "Door Track W", unit: "mm" },
        { name: "Double Sliding Track Bottom", unit: "mm" },
        { name: "Double Sliding Track Top", unit: "mm" },
        { name: "Efi Non Specific", unit: "Qty" },
        { name: "Fit Flyscreen Track per pair", unit: "Qty" },
        { name: "Fit Tim/Alum per piece", unit: "Qty" },
        { name: "Grill Frame for Infill", unit: "mm" },
        { name: "H Channel in Door to add 30mm to width or drop", unit: "Qty" },
        { name: "Lock Barrel supply only", unit: "Qty" },
        { name: "Lock Barrell Installed", unit: "Qty" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Miscellaneous Timber", unit: "Qty" },
        { name: "Patio Bolt", unit: "Qty" },
        { name: "Posts 50mm x 50mm", unit: "mm" },
        { name: "Powder Coating Minimum", unit: "Qty" },
        { name: "Single Sliding Track Bottom", unit: "mm" },
        { name: "Single Sliding Track Top", unit: "mm" },
        { name: "Square Tube 20x20", unit: "mm" },
        { name: "Stop Bead Additional", unit: "Qty" },
        { name: "Timber Frame 19 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 19 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 66 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 91 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 19 Finished", unit: "mm" },
        { name: "Timber Frames 19 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 30 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 41 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 13 Finished", unit: "mm" },
        { name: "Timber Frames 66 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 91mm Finished", unit: "mm" },
        { name: "Track", unit: "mm" },
        { name: "Track Jamb Adaptor Long", unit: "mm" },
        { name: "Track Jamb Adaptor Short", unit: "mm" },
        { name: "U Frame 20 mm sides x 25 mm wide", unit: "mm" },
        { name: "Whitco Winder Strip", unit: "Qty" },
        { name: "Window Lock", unit: "Qty" },
      ];
    }
  }

  if (["Security Door"].includes(blindname)) {
    if (["Sliding Door"].includes(controlname)) {
      list = [
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 25 x 70", unit: "mm" },
        { name: "Angle 25 x 75mm", unit: "mm" },
        { name: "Angle 40 x 40mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Door Frame (Infill for Sliding Door Receiver)", unit: "mm" },
        { name: "Door Posts 19 x 19 (for frame work)", unit: "mm" },
        { name: "Door Posts 25 x 25 (for frame work)", unit: "mm" },
        { name: "Door Posts 50 x 25 (for frame work)", unit: "mm" },
        { name: "Door Posts 50 x 50 (for frame work)", unit: "mm" },
        { name: "Door Track J HD1", unit: "mm" },
        { name: "Door Track P ST11", unit: "mm" },
        { name: "Door Track U Frame 20mm sidesx 25mm wide", unit: "mm" },
        { name: "Door Track W ST8", unit: "mm" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Powder Coating Minimum", unit: "Qty" },
        { name: "Stop Bead Additional", unit: "Qty" },
        { name: "Timber Frame 19 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 19 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 13mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 66 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 91 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 19 Finished", unit: "mm" },
        { name: "Timber Frames 19 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 30 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 41 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 13 Finished", unit: "mm" },
        { name: "Timber Frames 66 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 91mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 91mm Finished", unit: "mm" },
      ];
    }
    if (["Hinged Door"].includes(controlname)) {
      list = [
        { name: "Angle 25 x 70", unit: "mm" },
        { name: "Bugseal Additional Sliding", unit: "mm" },
        { name: "Bugseal Additional Hinged", unit: "mm" },
        { name: "Door Closer", unit: "mm" },
        { name: "Door Frame (Infill for Sliding Door Receiver)", unit: "mm" },
        {
          name: "Safety Door Deadlock - Without Barrel - Supply Only",
          unit: "Qty",
        },
        { name: "Safety Door Deadlock With Barrel (Supply Only)", unit: "Qty" },
        { name: "H Channel in Door to add 30mm to width or drop", unit: "Qty" },
        { name: "Grill Frame for Infill", unit: "mm" },
        { name: "Stop Bead Additional", unit: "Qty" },
        { name: "Powder Coating Minimum", unit: "Qty" },
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Lock Barrell Installed", unit: "Qty" },
        { name: "Lock Barrel (Customer to Supply)", unit: "Qty" },
        { name: "Lock Barrel supply only", unit: "Qty" },
        { name: "Patio Bolt", unit: "Qty" },
        { name: "Miscellaneous", unit: "Qty" },
        { name: "Timber Frame 19 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 19 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 19 Finished", unit: "mm" },
        { name: "Timber Frames 19 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 19 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 30 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 30 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 30 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 7mm Finished", unit: "mm" },
        { name: "Timber Frame 41 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 41 Finished", unit: "mm" },
        { name: "Timber Frames 41 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 41 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 66 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 13 Finished", unit: "mm" },
        { name: "Timber Frames 66 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 66 x 91mm Finished", unit: "mm" },
        { name: "Timber Frame 91 x 7mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 13mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 19mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 30mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 41mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 66mm Finished", unit: "mm" },
        { name: "Timber Frames 91 x 91mm Finished", unit: "mm" },
      ];
    }
  }

  list.forEach((ls) => {
    data.push({
      value: ls.name,
      text: ls.name,
      unit: ls.unit,
    });
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
    option.setAttribute("data-unit", item.unit);
    sel.add(option);
  });

  sel.addEventListener("change", function () {
    const selected = Array.from(this.selectedOptions).map((x) => x.value);

    // 1. HAPUS ITEM YANG DI UNSELECT
    extrasState = extrasState.filter((x) => selected.includes(x.name));

    // 2. TAMBAH ITEM BARU
    selected.forEach((name) => {
      if (!extrasState.find((x) => x.name === name)) {
        const option = this.querySelector(`option[value="${name}"]`);

        extrasState.push({
          name: name,
          unit: option?.dataset?.unit || "Qty",
          value: "",
        });
      }
    });

    // 3. RENDER ULANG (INI YANG MENCEGAH RESET)
    renderExtras();
  });

  initTomSelect();
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
        bindFrameColour(item.BlindName, item.ControlType),
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
        bindExtras(item.BlindName, item.ControlType),
      ]);
      if (["Security Door"].includes(item.BlindName)) {
        await Promise.all(bindFrameColour(item.BlindName, item.ControlType));
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
    const divMesh = document.getElementById("divMesh");
    const divHandle = document.getElementById("divHandle");
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
    divMesh.classList.add("d-none");
    divHandle.classList.add("d-none");
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
      divHalf.classList.remove("d-none");
      divInstall.classList.remove("d-none");
      divInterlock.classList.remove("d-none");
      divExtras.classList.remove("d-none");

      if (["Hinged Door"].includes(controlname)) {
        divWidthMid.classList.remove("d-none");
        divWidthBot.classList.remove("d-none");
        lblInterlock.innerHTML = "Adaptors and options";
        divInswing.classList.remove("d-none");
        divLockColour.classList.remove("d-none");
        divCloser.classList.remove("d-none");
        divFixing.classList.remove("d-none");
        divFitted.classList.remove("d-none");
        divRemove.classList.remove("d-none");
      }
    }

    if (["Ultra Guard"].includes(tubetype)) {
      lblFrame.innerHTML = "Frame Colour";
      divFrameColour.classList.remove("d-none");
      divHandle.classList.remove("d-none");
      divKeyed.classList.remove("d-none");
      divMidrail.classList.remove("d-none");
      divBugseal.classList.remove("d-none");
      lblInterlock.innerHTML = "Interlocks";
      divInterlock.classList.remove("d-none");
      divExtras.classList.remove("d-none");

      if (["Hinged Door"].includes(controlname)) {
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
      "drop",
      "sliding",
      "stacking",
      "trackless",
      "frametype",
      "framecolour",
      "meshtype",
      "handleside",
      "handleheight",
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
      "half",
      "interlock",
      "extras",
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

    const extras = [];
    document.querySelectorAll(".extra-row").forEach((row) => {
      const name = row.querySelector("input[readonly]").value;
      const value = row.querySelector(".extra-value").value;
      const unit = row
        .querySelector(".extra-value")
        .getAttribute("placeholder")
        .replace("Enter ", "");

      extras.push({
        name: name,
        unit: unit,
        value: value,
      });
    });

    formData["extras"] = JSON.stringify(extras);

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
    drop: "Drop",
    sliding: "BottomTrackType",
    stacking: "StackPosition",
    trackless: "TilterPosition",
    frametype: "FrameType",
    framecolour: "FrameColour",
    // customframecolour: "FrameLeft",
    meshtype: "MeshType",
    handleside: "Brace",
    handleheight: "BracketOption",
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

  // ===============================
  // 2. HANDLE EXTRAS (INI TARUH DI SINI)
  // ===============================

  let extrasData = [];

  try {
    extrasData = itemData.AdditionalMotor
      ? JSON.parse(itemData.AdditionalMotor)
      : [];
  } catch (e) {
    console.error("Invalid JSON", e);
    extrasData = [];
  }
  extrasState = extrasData;

  // 3. SET TOM SELECT VALUE
  const extrasSelect = document.getElementById("extras");
  if (extrasSelect && extrasSelect.tomselect) {
    extrasSelect.tomselect.setValue(extrasData.map((x) => x.name));
  }

  // 4. REBUILD DYNAMIC ROWS
  const extrasContainer = document.getElementById("extrasContainer");
  if (extrasContainer) {
    extrasContainer.innerHTML = "";

    extrasData.forEach((item) => {
      extrasContainer.innerHTML += `
        <div class="row mb-2 extra-row">

            <div class="col-7">
                <input type="text"
                       class="form-control"
                       value="${item.name}"
                       readonly />
            </div>

            <div class="col-5">
                <div class="input-group">
                  <input type="number"
                        class="form-control extra-value"
                        value="${item.value || ""}"
                        placeholder="Enter ${item.unit}" />
                  <span class="input-group-text ">${item.unit}</span>
                </div>
            </div>

        </div>
      `;
    });
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
      validateLength = 0;
      break;
  }

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

let tomExtras = null;
const initTomSelect = () => {
  if (tomExtras) {
    tomExtras.destroy();
  }

  tomExtras = new TomSelect("#extras", {
    // plugins: ["remove_button"],
    // placeholder: "Select Extras",
    maxItems: null,
    create: false,
  });
};

const renderExtras = () => {
  const container = document.getElementById("extrasContainer");
  if (!container) return;

  container.innerHTML = "";

  extrasState.forEach((item) => {
    container.innerHTML += `
      <div class="row mb-2 extra-row">

          <div class="col-7">
              <input type="text"
                     class="form-control"
                     value="${item.name}"
                     readonly />
          </div>

          <div class="col-5">
              <div class="input-group">
                <input type="number"
                      class="form-control extra-value"
                      value="${item.value || ""}"
                      placeholder="Enter ${item.unit}" />
                <span class="input-group-text">${item.unit}</span>
              </div>
          </div>

      </div>
    `;
  });
};
