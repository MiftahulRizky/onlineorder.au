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
      const divCustomFrameColour = document.getElementById(
        "divCustomFrameColour",
      );
      document.getElementById("customframecolour").value = "";
      divCustomFrameColour.classList.add("d-none");
      if (["Powder Coating"].includes(framecolour)) {
        divCustomFrameColour.classList.remove("d-none");
      }
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
  let list = [];

  if (["Safety Window"].includes(blindname)) {
    list = ["Fiberglass"];
    if (width && width <= 1000) {
      list = ["Fiberglass", "Stainless Steel"];
    }
  }

  if (["Basic Window"].includes(blindname)) {
    list = [
      "Fibreglass Mesh",
      "Alum (std)",
      "Stainless (1000)",
      "Stainless (1300)",
      "Pawproof",
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

const bindSlidingType = (blindname) => {
  const sel = document.getElementById("slidingtype");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Single Sliding Pleated", "Double Sliding Pleated"];
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

const bindStacking = (blindname) => {
  const sel = document.getElementById("stacking");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Stacking - Right", "Stacking - Left", "Stacking - Split"];
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

const bindTrackless = (blindname) => {
  const sel = document.getElementById("trackless");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Trackless - No"];
  }

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  if (data.length > 0) {
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

const bindFrameType = (blindname, tubename) => {
  const sel = document.getElementById("frametype");
  document.getElementById("framecolour").innerHTML = "";
  sel.innerHTML = ""; //reset

  if (!blindname || !tubename) return;

  let data = [];
  let list = [];

  if (["Safety Window", "Security Window"].includes(blindname)) {
    list = ["Grille Frame", "Door Frame"];
  }

  if (["Basic Window"].includes(blindname)) {
    if (["Flyscreens"].includes(tubename)) {
      list = ["21x9 Frame", "25x11 Frame", "35x11 Frame"];
    }
    if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
      list = ["Door", "Window"];
    }
  }

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  if (data.length > 0) {
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

const bindFrameColour = (blindname, tubename, frametype) => {
  const sel = document.getElementById("framecolour");
  sel.innerHTML = ""; //reset

  if (!blindname || !tubename || !frametype) return;

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
      "Beige",
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
      "Monument Matt",
      "Primrose",
      "Surf Mist",
      "Paperbark",
      "Pearl White",
      "White Birch",
      "Woodland Grey",
    ];
  }

  if (["Basic Window"].includes(blindname)) {
    if (["Flyscreens"].includes(tubename)) {
      list = [
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
      ];
    }
    if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
      list = ["White", "Black", "Powder Coating"];
    }
    if (["Retractable Flyscreen Pleated"].includes(tubename)) {
      list = [
        "White",
        "Black",
        "Clear Anodised",
        "Powder Coating",
        "White Birch",
        "Primrose",
        "Monument",
      ];
    }
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

  if (["Basic Window"].includes(blindname)) {
    list = [
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Horizontal Brace/s Specify",
      "Vertical Brace/ Specify",
    ];
  }
  if (["Safety Window"].includes(blindname)) {
    list = [
      "Horizontal Centre Brace",
      "Vertical Centre Brace",
      "Vertical Brace Specify",
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
  let list = [];

  if (["Basic Window", "Safety Window"].includes(blindname)) {
    list = ["Pick Up"];
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

const bindFitting = (blindname) => {
  const sel = document.getElementById("fitting");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Screen Port / Trap Door"];
  }

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  if (data.length > 0) {
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
    const list = ["Removal Only", "Removal and Disposal"];

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

const bindHandle = (blindname) => {
  const sel = document.getElementById("handle");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Handle - Front", "Handle - Back", "Handle - Dual"];
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

const bindPullCord = (blindname) => {
  const sel = document.getElementById("pullcord");
  sel.innerHTML = ""; //reset

  if (!blindname) return;

  let data = [];
  let list = [];

  if (["Basic Window"].includes(blindname)) {
    list = ["Pullcord - Yes", "Pullcord - No"];
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

let cutoutState = [];
const bindCutOut = (blindname) => {
  const sel = document.getElementById("cutout");
  sel.innerHTML = ""; //reset

  if (!blindname) return;
  if (tomCutout) {
    tomCutout.destroy();
    tomCutout = null;
  }

  let data = [];
  let list = [];

  if (["Safety Window", "Security Window"].includes(blindname)) {
    // list = [
    //   "Cutout Side 1",
    //   "Cutout Width 1",
    //   "Bottom Cutout 1",
    //   "Top Cutout 1",
    //   "Cutout Side 2",
    //   "Bottom Cutout 2",
    //   "Cutout Width 2",
    //   "Top Cutout 2",
    // ];

    list = [
      { name: "Cutout Side 1", unit: "mm" },
      { name: "Cutout Width 1", unit: "mm" },
      { name: "Bottom Cutout 1", unit: "mm" },
      { name: "Top Cutout 1", unit: "mm" },
      { name: "Cutout Side 2", unit: "mm" },
      { name: "Bottom Cutout 2", unit: "mm" },
      { name: "Cutout Width 2", unit: "mm" },
      { name: "Top Cutout 2", unit: "mm" },
    ];
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
    cutoutState = cutoutState.filter((x) => selected.includes(x.name));

    // 2. TAMBAH ITEM BARU
    selected.forEach((name) => {
      if (!cutoutState.find((x) => x.name === name)) {
        const option = this.querySelector(`option[value="${name}"]`);

        cutoutState.push({
          name: name,
          unit: option?.dataset?.unit || "Qty",
          value: "",
        });
      }
    });

    // 3. RENDER ULANG (INI YANG MENCEGAH RESET)
    renderCutOut();
  });

  initTomSelect();
};

let extrasState = [];
const bindExtras = (blindname, tubename) => {
  const sel = document.getElementById("extras");
  sel.innerHTML = ""; //reset

  if (!blindname) return;
  if (tomExtras) {
    tomExtras.destroy();
    tomExtras = null;
  }

  let data = [];
  let list = [];

  if (["Safety Window"].includes(blindname)) {
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

  if (["Security Window"].includes(blindname)) {
    list = [
      { name: "Angle 12 x 12mm", unit: "mm" },
      { name: "Angle 25 x 70", unit: "mm" },
      { name: "Bugseal Additional Hinged", unit: "mm" },
      { name: "Doggie Door - Perspex 190mm x 260mm", unit: "Qty" },
      { name: "Doggie Door - Perspex 260mm x 400mm", unit: "Qty" },
      { name: "Door Interlock Additional", unit: "mm" },
      { name: "Patio Bolt", unit: "Qty" },
      { name: "Angle 12 x 20mm", unit: "mm" },
      { name: "Angle 20 x 20mm", unit: "mm" },
      { name: "Angle 20 x 40mm", unit: "mm" },
      { name: "Angle 25 x 20mm", unit: "mm" },
      { name: "Angle 50 x 50mm", unit: "mm" },
      { name: "Chain Winder Lockable", unit: "Qty" },
      { name: "Door Posts 19 x 19 (for frame work)", unit: "mm" },
      { name: "Door Posts 25 x 25 (for frame work)", unit: "mm" },
      { name: "Door Posts 50 x 50 (for frame work)", unit: "mm" },
      { name: "Door Track H ST4", unit: "mm" },
      { name: "Door Track J HD1", unit: "mm" },
      { name: "Door Track P ST11", unit: "mm" },
      { name: "Door Track U Frame 20mm sidesx 25mm wide", unit: "mm" },
      { name: "Door Track W ST8", unit: "mm" },
      { name: "Powder Coating Minimum", unit: "Qty" },
      { name: "Stop Bead Additional", unit: "Qty" },
      { name: "Whitco Winder Strip", unit: "Qty" },

      { name: "Door Interlock Type 1", unit: "mm" },
      { name: "Door Interlock Type 2", unit: "mm" },
      { name: "Door Interlock Type 3", unit: "mm" },
      { name: "Door Interlock Type F", unit: "mm" },
      { name: "Double Sliding Track", unit: "mm" },
      { name: "Fit Flyscreen Track per pair", unit: "Qty" },
      { name: "Miscellaneous", unit: "Qty" },
      { name: "Miscellaneous Scaffold", unit: "Qty" },
      { name: "Miscellaneous Security", unit: "Qty" },
      { name: "Miscellaneous Timber", unit: "Qty" },
      { name: "Single Sliding Track", unit: "mm" },

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

  if (["Basic Window"].includes(blindname)) {
    if (["Flyscreens"].includes(tubename)) {
      list = [
        { name: "Flyscreen Plunger Pins", unit: "Qty" },
        { name: "Flyscreen Top Clips", unit: "Qty" },
        { name: "Flyscreen Turn Buttons", unit: "Qty" },
        { name: "Single Sliding Track Top", unit: "mm" },
        { name: "Single Sliding Track Bottom", unit: "mm" },
        { name: "Double Sliding Track Top", unit: "mm" },
        { name: "Double Sliding Track Bottom", unit: "mm" },
        { name: "Flyscreen Beading", unit: "mm" },
        { name: "Bugseal Additional Hinged", unit: "mm" },
        { name: "Chain Winder Lockable", unit: "Qty" },
        { name: "Powdercoating Minimum Flyscreens", unit: "Qty" },
        { name: "Door Track U Frame 20mm sidesx 25mm wide", unit: "mm" },
        { name: "Angle 12 x 12mm", unit: "mm" },
        { name: "Angle 12 x 20mm", unit: "mm" },
        { name: "Angle 12 x 25mm", unit: "mm" },
        { name: "Angle 20 x 40mm", unit: "mm" },
        { name: "Angle 25 x 20mm", unit: "mm" },
        { name: "Angle 50 x 25mm", unit: "mm" },
        { name: "Whitco Winder Strip", unit: "Qty" },
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

    if (["Retractable Flyscreen Pleated"].includes(tubename)) {
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

    if (["Retractable Flyscreen Roll-Up Down"].includes(tubename)) {
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
      await Promise.all([
        bindMounting(),
        bindMesh(item.BlindName, item.Width),
        bindSlidingType(item.BlindName),
        bindStacking(item.BlindName),
        bindTrackless(item.BlindName),
        bindFrameType(item.BlindName, item.TubeType),
        bindFrameColour(item.BlindName, item.TubeType, item.FrameType),
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
    const divCustomFrameColour = document.getElementById(
      "divCustomFrameColour",
    );
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
    divCustomFrameColour.classList.add("d-none");
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
      divDualHinges.classList.remove("d-none");
      // divInstall.classList.remove("d-none");
      divCutOut.classList.remove("d-none");
      divExtras.classList.remove("d-none");
    }

    if (["Security Window"].includes(blindname)) {
      divFrameType.classList.remove("d-none");
      divFrameColour.classList.remove("d-none");
      divBrace.classList.remove("d-none");
      // divRemove.classList.remove("d-none");
      divCutOut.classList.remove("d-none");
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
        divCustomFrameColour.classList.remove("d-none");
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
      "customframecolour",
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

    const cutout = [];
    document.querySelectorAll(".cutout-row").forEach((row) => {
      const name = row.querySelector("input[readonly]").value;
      const value = row.querySelector(".cutout-value").value;
      const unit = row
        .querySelector(".cutout-value")
        .getAttribute("placeholder")
        .replace("Enter ", "");

      cutout.push({
        name: name,
        unit: unit,
        value: value,
      });
    });

    formData["extras"] = JSON.stringify(extras);
    formData["cutout"] = JSON.stringify(cutout);

    fields.forEach((field) => {
      if (field === "extras") return; // skip
      if (field === "cutout") return; // skip

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
    customframecolour: "FrameLeft",
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
  let cutoutData = [];

  try {
    extrasData = itemData.AdditionalMotor
      ? JSON.parse(itemData.AdditionalMotor)
      : [];

    cutoutData = itemData.FlatType ? JSON.parse(itemData.FlatType) : [];
  } catch (e) {
    console.error("Invalid JSON", e);
    extrasData = [];
    cutoutData = [];
  }
  extrasState = extrasData;
  cutoutState = cutoutData;

  // 3. SET TOM SELECT VALUE
  const extrasSelect = document.getElementById("extras");
  const cutoutSelect = document.getElementById("cutout");
  if (extrasSelect && extrasSelect.tomselect) {
    extrasSelect.tomselect.setValue(extrasData.map((x) => x.name));
  }
  if (cutoutSelect && cutoutSelect.tomselect) {
    cutoutSelect.tomselect.setValue(cutoutData.map((x) => x.name));
  }

  // 4. REBUILD DYNAMIC ROWS
  const extrasContainer = document.getElementById("extrasContainer");
  const cutoutContainer = document.getElementById("cutoutContainer");
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

  if (cutoutContainer) {
    cutoutContainer.innerHTML = "";

    cutoutData.forEach((item) => {
      cutoutContainer.innerHTML += `
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

let tomExtras = null;
let tomCutout = null;
const initTomSelect = () => {
  if (tomExtras) {
    tomExtras.destroy();
  }
  if (tomCutout) {
    tomCutout.destroy();
  }

  tomExtras = new TomSelect("#extras", {
    // plugins: ["remove_button"],
    // placeholder: "Select Extras",
    maxItems: null,
    create: false,
  });

  tomCutout = new TomSelect("#cutout", {
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

const renderCutOut = () => {
  const container = document.getElementById("cutoutContainer");
  if (!container) return;

  container.innerHTML = "";

  cutoutState.forEach((item) => {
    container.innerHTML += `
      <div class="row mb-2 cutout-row">

          <div class="col-7">
              <input type="text"
                     class="form-control"
                     value="${item.name}"
                     readonly />
          </div>

          <div class="col-5">
              <div class="input-group">
                <input type="number"
                      class="form-control cutout-value"
                      value="${item.value || ""}"
                      placeholder="Enter ${item.unit}" />
                <span class="input-group-text">${item.unit}</span>
              </div>
          </div>

      </div>
    `;
  });
};
