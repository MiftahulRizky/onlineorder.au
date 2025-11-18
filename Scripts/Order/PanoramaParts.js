document
  .getElementById("modalSuccess")
  .addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
  });

document
  .getElementById("modalError")
  .addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
  });

$(document).ready(function () {
  checkSession();

  $("#submit").on("click", proccess);
  $("#cancel").on("click", () => (window.location.href = "/order/detail/"));
  $("#vieworder").on("click", () => (window.location.href = "/order/detail"));

  $("#category").on("change", function () {
    const ddlCategory = $(this).val();
    bindComponent(ddlCategory);
    const component = $("#component").val();
    bindColour(ddlCategory, component);

    toggleVisibility("divColour", isColourVisible(category, component));
    toggleVisibility("divLength", isLengthVisible(category, component));
  });

  $("#component").on("change", function () {
    const category = $("#category").val();
    const component = $(this).val();

    bindColour(category, component);

    toggleVisibility("divColour", isColourVisible(category, component));
    toggleVisibility("divLength", isLengthVisible(category, component));
  });

  $("#notes").on("input", function () {
    let maxLength = 1000;
    let currentLength = $(this).val().length;
    $("#notescount").text(`${currentLength}/${maxLength}`);
  });
});

async function checkSession() {
  if (!headerId) {
    window.location.href = "/order";
    return;
  }
  if (!itemAction || !designId) {
    window.location.href = "/order/detail";
    return;
  }
  if (designId.toUpperCase() !== designIdOri) {
    window.location.href = "/order/detail";
    return;
  }

  try {
    await getDesignName(designId);
    await getDataHeader(headerId);
    await getFormAction(itemAction);
    await loader(itemAction);

    if (itemAction === "AddItem") {
      bindComponentForm("");
      controlForm(false);
      await bindBlindType(designId);
    } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
      await bindItemOrder(itemId);
      controlForm(
        itemAction === "ViewItem",
        itemAction === "EditItem",
        itemAction === "CopyItem"
      );
    }
  } catch (error) {
    console.error("Terjadi kesalahan saat proses checkSession:", error);
  }
}

function isError(msg) {
  $("#modalError").modal("show");
  document.getElementById("errorMsg").innerHTML = msg;
}

function loader(itemAction) {
  return new Promise((resolve) => {
    if (itemAction === "AddItem") {
      document.getElementById("divLoader").style.display = "none";
      document.getElementById("divOrder").style.display = "";
    }
    resolve();
  });
}

function getDesignName(designId) {
  return new Promise((resolve, reject) => {
    const pageTitle = document.getElementById("pageTitle");
    pageTitle.textContent = "";

    if (!designId) {
      resolve();
      return;
    }

    const type = "DesignName";
    $.ajax({
      type: "POST",
      url: "Method.aspx/StringData",
      data: JSON.stringify({ type: type, dataId: designId }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const designName = response.d.trim() || "Nama desain tidak ditemukan";
        pageTitle.textContent = designName;
        resolve();
      },
      error: function (xhr, status, error) {
        console.error("AJAX Error:", error, xhr.responseText);
        reject(error);
      },
    });
  });
}

function getDataHeader(headerId) {
  return new Promise((resolve, reject) => {
    $.ajax({
      type: "POST",
      url: "Method.aspx/GetDataHeader",
      data: JSON.stringify({ headerId }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (data) {
          document.getElementById("orderid").innerText = data.OrderId || "-";
          document.getElementById("orderno").innerText =
            data.OrderNumber || "-";
          document.getElementById("ordername").innerText =
            data.OrderName || "-";
        }
        resolve();
      },
      error: function (error) {
        reject(error);
      },
    });
  });
}

function getFormAction(itemAction) {
  return new Promise((resolve) => {
    const cardTitle = document.getElementById("cardTitle");
    if (!cardTitle) {
      console.warn("Elemen 'cardTitle' tidak ditemukan.");
      resolve();
      return;
    }

    const actionMap = {
      AddItem: "ADD ITEM",
      EditItem: "EDIT ITEM",
      ViewItem: "VIEW ITEM",
      CopyItem: "COPY ITEM",
    };
    cardTitle.innerText = actionMap[itemAction] || "";
    resolve();
  });
}

function bindBlindType(designId) {
  return new Promise((resolve, reject) => {
    const blindtype = document.getElementById("blindtype");
    blindtype.innerHTML = "";

    if (!designId) {
      resolve();
      return;
    }

    const listData = { type: "BlindType", designtype: designId };
    $.ajax({
      type: "POST",
      url: "Method.aspx/ListData",
      data: JSON.stringify({ data: listData }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        if (Array.isArray(response.d)) {
          blindtype.innerHTML = "";

          if (response.d.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            blindtype.add(defaultOption);
          }

          response.d.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.Value;
            option.text = item.Text;
            blindtype.add(option);
          });

          if (response.d.length === 1) {
            blindtype.selectedIndex = 0;
          }

          const selectedValue = blindtype.value;
          Promise.all([bindColourType(selectedValue)])
            .then(resolve)
            .catch(reject);
        }
        resolve();
      },
      error: function (xhr, status, error) {
        reject(error);
      },
    });
  });
}

function bindColourType(blindId) {
  return new Promise((resolve, reject) => {
    const colourtype = document.getElementById("colourtype");
    colourtype.innerHTML = "";

    if (!designId) {
      resolve();
      return;
    }

    const listData = {
      type: "ColourType",
      blindtype: blindId,
      tubetype: "N/A",
      controltype: "N/A",
    };
    $.ajax({
      type: "POST",
      url: "Method.aspx/ListData",
      data: JSON.stringify({ data: listData }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        if (Array.isArray(response.d)) {
          colourtype.innerHTML = "";

          if (response.d.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            colourtype.add(defaultOption);
          }

          response.d.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.Value;
            option.text = item.Text;
            colourtype.add(option);
          });

          if (response.d.length === 1) {
            colourtype.selectedIndex = 0;
          }

          const selectedValue = colourtype.value;
          Promise.all([bindComponentForm(selectedValue)])
            .then(resolve)
            .catch(reject);
        }
        resolve();
      },
      error: function (xhr, status, error) {
        console.log("Terjadi kesalahan saat memanggil WebMethod: " + error);
        reject(error);
      },
    });
  });
}

function bindComponent(category) {
  const ddlComponent = document.getElementById("component");
  ddlComponent.innerHTML = "";

  if (!category) return;

  const componentOptions = {
    Louvres: [
      "63mm Louvre",
      "89mm Louvre",
      "114mm Louvre",
      "Standard Louvre Pin",
      "Louvre Repair Pin",
    ],
    "Framing | Hinged": [
      "Beaded L Frame 48mm",
      "Insert L Frame 50mm",
      "Insert L Frame 63mm",
      "Flat L Frame",
      "Small Bullnose Z Frame",
      "Large Bullnose Z Frame",
      "Colonial Z Frame",
      "Small Bullnose Sill Plate",
      "Large Bullnose Sill Plate",
      "Colonial Bullnose Sill Plate",
      // "Top U Channel",
      // "Bottom U Channel",
    ],
    "Framing | Bi-fold or Sliding": [
      "101.6mm x 19mm Header",
      "161.6mm x 19mm Header",
      "201.6mm x 19mm Header",
      "100mm x 19mm Sideboard/Bottom Board",
      "160mm x 19mm Sideboard/Bottom Board",
      "200mm x 19mm Sideboard/Bottom Board",
      "63.5mm x 9.5mm Fascia",
      "100mm x 9.5mm Fascia",
      "140mm x 9.5mm Fascia",
      "31.8mm x 9.5mm Side Fascia",
      "25mm x 44.5mm Header Support Strip",
      "19mm x 19mm Header Support Strip",
      "19mm x 25mm Light Strip",
      "25mm x 25mm Light Strip",
      "30mm x 25mm Light Strip",
      "40mm x 25mm Light Strip",
    ],
    "Framing | Fixed": ["Top U Channel", "Bottom U Channel"],
    Posts: [
      "T-Post",
      "90° Corner Post",
      "135° Bay Post",
      "Post Mounting Bracket",
      "L Strip",
    ],
    Extrusion: [
      "19mm x 19mm Light Block",
      "9.5mm x 31.8mm Frame Buildout",
      "25mm x 44.5mm Frame Buildout",
    ],
    Hinges: ["76mm Non-Mortise Hinge", "76mm Rabbet Hinge", "Hinge Spacer"],
    "Magnets and Strikers": [
      "Magnet",
      "Standard Striker Plate",
      "L Shape Striker Plate",
    ],
    "Track Hardware": [
      "Top Track",
      "Bottom M Track",
      "Bottom U Track",
      "Top Pivot Set",
      "Bottom Pivot (Incl Bracket)",
      "Wheel Carrier",
      "Spring Loaded Guide",
      "Adjustment Spanner",
      "Track Stop",
    ],
    Misc: ["Hoffman Key"],
  };

  const options = [{ value: "", text: "" }];

  if (componentOptions[category]) {
    componentOptions[category].forEach((text) => {
      options.push({ value: text, text: text.toUpperCase() });
    });
  }

  options.forEach((opt) => {
    const optionElement = document.createElement("option");
    optionElement.value = opt.value;
    optionElement.textContent = opt.text;
    ddlComponent.appendChild(optionElement);
  });
}

function bindColour(category, component) {
  const ddlColour = document.getElementById("colour");
  ddlColour.innerHTML = "";

  if (!category) return;

  const defaultOptions = [
    { value: "", text: "" },
    { value: "Off White", text: "OFF WHITE" },
    { value: "Snow White", text: "SNOW WHITE" },
    { value: "Bright White", text: "BRIGHT WHITE" },
    // { value: "Alabaster", text: "ALABASTER" },
    { value: "Classic White", text: "WHITE" },
  ];

  const colourOptions = {
    Louvres:
      component === "Standard Louvre Pin" || component === "Louvre Repair Pin"
        ? [{ value: "White", text: "WHITE" }]
        : defaultOptions,
    Hinges: [
      { value: "", text: "" },
      { value: "Off White", text: "OFF WHITE" },
      { value: "White", text: "WHITE" },
      { value: "Stainless Steel", text: "STAINLESS STEEL" },
    ],
    Misc: [{ value: "White", text: "WHITE" }],
  };

  const options = colourOptions[category] || defaultOptions;

  options.forEach((opt) => {
    const optionElement = document.createElement("option");
    optionElement.value = opt.value;
    optionElement.textContent = opt.text;
    ddlColour.appendChild(optionElement);
  });
}

function bindComponentForm(Data) {
  const divDetail = document.getElementById("divDetail");
  divDetail.style.display = Data ? "" : "none";

  if (Data) {
    const ddlCategory = document.getElementById("category").value;
    const ddlComponent = document.getElementById("component").value;
    toggleVisibility("divColour", isColourVisible(ddlCategory, ddlComponent));
    toggleVisibility("divLength", isLengthVisible(ddlCategory, ddlComponent));
  }
}

function isColourVisible(category, component) {
  const colourCategories = new Set([
    "Louvres",
    "Framing | Hinged",
    "Framing | Bi-fold or Sliding",
    "Framing | Fixed",
    "Posts",
    "Extrusion",
    "Hinges",
    "Magnets and Strikers",
    "Misc",
  ]);
  const hiddenComponents = new Set([
    "Post Mounting Brack",
    "Hinge Spacer",
    "Magnet",
  ]);
  return colourCategories.has(category) && !hiddenComponents.has(component);
}

function isLengthVisible(category, component) {
  const lengthCategories = new Set([
    "Louvres",
    "Framing | Hinged",
    "Framing | Bi-fold or Sliding",
    "Framing | Fixed",
    "Posts",
    "Extrusion",
    "Track Hardware",
  ]);
  const hiddenComponents = new Set([
    "Standard Louvre Pin",
    "Louvre Repair Pin",
    "Post Mounting Bracket",
    "Top Pivot Set",
    "Bottom Pivot (Incl Bracket)",
    "Wheel Carrier",
    "Spring Loaded Guide",
    "Adjustment Spanner",
    "Track Stop",
  ]);
  return lengthCategories.has(category) && !hiddenComponents.has(component);
}

function toggleVisibility(elementId, isVisible) {
  document.getElementById(elementId).style.display = isVisible ? "" : "none";
}

function proccess() {
  toggleButtonState(true, "Processing...");

  const fields = [
    "blindtype",
    "colourtype",
    "qty",
    "category",
    "component",
    "colour",
    "length",
    "notes",
    "markup",
  ];

  const formData = {
    headerid: headerId,
    itemaction: itemAction,
    itemid: itemId,
    designid: designId,
    loginid: loginId,
  };

  fields.forEach((id) => {
    formData[id] = document.getElementById(id).value;
  });

  $.ajax({
    type: "POST",
    url: "Method.aspx/PartsProccess",
    data: JSON.stringify({ data: formData }),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (response) {
      const result = response.d.trim();
      if (result === "Success") {
        setTimeout(() => {
          $("#modalSuccess").modal("show");
          startCountdown(3);
        }, 1000);
      } else {
        isError(result);
        toggleButtonState(false, "Submit");
      }
    },
    error: function (xhr, status, error) {
      console.error("AJAX Error:", error, xhr.responseText);
      alert("Error: " + error);
      toggleButtonState(false, "Submit");
    },
  });
}

function toggleButtonState(disabled, text) {
  $("#submit")
    .prop("disabled", disabled)
    .css("pointer-events", disabled ? "none" : "auto")
    .text(text);

  $("#cancel")
    .prop("disabled", disabled)
    .css("pointer-events", disabled ? "none" : "auto");
}

function startCountdown(seconds) {
  let countdown = seconds;
  const button = $("#vieworder");

  function updateButton() {
    button.text(`View Order (${countdown}s)`);
    countdown--;

    if (countdown >= 0) {
      setTimeout(updateButton, 1000);
    } else {
      window.location.href = "/order/detail";
    }
  }
  updateButton();
}

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function bindItemOrder(itemId) {
  try {
    const response = await $.ajax({
      type: "POST",
      url: "Method.aspx/Detail",
      data: JSON.stringify({ itemId }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
    });

    const data = response.d;
    if (!data.length) return;

    const itemData = data[0];
    const blindtype = itemData["BlindType"];
    const colourtype = itemData["ColourType"];
    const category = itemData["Category"];
    const component = itemData["Component"];

    document.getElementById("divLoader").style.display = "";

    bindBlindType(designId);
    await delay(150);

    bindColourType(blindtype);
    await delay(200);

    await Promise.all([
      bindComponent(category),
      bindColour(category, component),
    ]);
    await delay(400);

    setFormValues(itemData);
    bindComponentForm(colourtype);

    document.getElementById("divLoader").style.display = "none";
    document.getElementById("divOrder").style.display = "";
  } catch (err) {
    console.error("Error:", err);
  }
}

function setFormValues(itemData) {
  const mapping = {
    blindtype: "BlindType",
    colourtype: "ColourType",
    qty: "Qty",
    category: "Category",
    component: "Component",
    colour: "Colour",
    length: "Length",
    notes: "Notes",
    markup: "MarkUp",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]];
    if (id === "markup" && value === 0) value = "";
    el.value = value || "";
  });
  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  $("#notescount").text(`${notesLength}/${maxLength}`);

  if (itemAction === "CopyItem") {
    const resetFields = ["length", "notes"];
    resetFields.forEach((id) => {
      const el = document.getElementById(id);
      if (el) el.value = "";
    });

    $("#notescount").text(`0/${maxLength}`);
  }
}

function controlForm(status, isEditItem, isCopyItem) {
  if (isEditItem === undefined) {
    isEditItem = false;
  }
  if (isCopyItem === undefined) {
    isCopyItem = false;
  }

  document.getElementById("submit").style.display = status ? "none" : "";

  const inputs = [
    "blindtype",
    "colourtype",
    "qty",
    "category",
    "component",
    "colour",
    "length",
    "notes",
    "markup",
  ];

  inputs.forEach((id) => {
    const inputElement = document.getElementById(id);
    if (inputElement) {
      if (isCopyItem) {
        inputElement.disabled = id === "blindtype" || id === "colourtype";
      } else if (isEditItem && (id === "blindtype" || id === "colourtype")) {
        inputElement.disabled = true;
      } else {
        inputElement.disabled = status;
      }
    }
  });
}
