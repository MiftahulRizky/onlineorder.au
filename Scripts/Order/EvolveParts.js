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
    const category = $(this).val();
    const component = $("#component").val();

    bindComponent(category);
    bindColour(category, component);
    visibleColour(category, component);
  });

  $("#component").on("change", function () {
    const category = $("#category").val();
    const component = $(this).val();

    bindColour(category, component);
    visibleColour(category, component);
    visibleLength(category, component);
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
    console.error(error);
  }
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

function isError(msg) {
  $("#modalError").modal("show");
  document.getElementById("errorMsg").innerHTML = msg;
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
        const designName = response.d.trim();
        pageTitle.textContent = designName;
        resolve();
      },
      error: function (error) {
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
          document.getElementById("orderid").innerText = data.OrderId;
          document.getElementById("orderno").innerText = data.OrderNumber;
          document.getElementById("ordername").innerText = data.OrderName;
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
      error: function (error) {
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
      error: function (error) {
        reject(error);
      },
    });
  });
}

function bindComponent(category) {
  const component = document.getElementById("component");
  component.innerHTML = "";

  if (!category) return;

  const componentOptions = {
    Louvres: [
      "63mm Louvre",
      "89mm Louvre",
      "Louvre Pin - Easy Tilt",
      "Louvre Pin - Standard/Repair",
    ],
    "Framing | Hinged": [
      "Beaded L Frame 49mm",
      "Insert L Frame 49mm",
      "Small Bullnose Z Frame (38mm)",
      "Large Bullnose Z Frame (50mm)",
      "Bullnose Z Sill Plate",
      "13mm Frame Extension",
    ],
    "Framing | Bi-fold or Sliding": [
      "92mm x 22mm Header/Sideboard",
      "152mm x 22mm Header/Sideboard",
      "185mm x 22mm Header/Sideboard",
      "32mm x 10mm Fascia-Sideboard",
      "67mm x 13mm Top Fascia",
      "95mm x 13mm Top Fascia",
      "107mm x 13mm Top Fascia",
      "135mm x 13mm Top Fascia",
      "Fascia H Clip",
      "Fascia Return Connector",
      "Header Mounting Strip (F Strip)",
      "Sideboard Mounting Strip (L Strip)",
      "106mm Header Support Bracket",
      "142mm Header Support Bracket",
      "19mm x 19mm Light Strip",
      "32mm x 25mm Light Strip",
    ],
    "Framing | Fixed": [
      "Top U Channel",
      "Bottom U Channel",
      "19mm x 19mm Light Block",
    ],
    Posts: [
      "T Post",
      "90° Corner Post",
      "135° Bay Post",
      "T Post Mounting Block",
      "90° Corner Post Mounting Block",
      "135° Bay Post Mounting Block",
      "T Post Mounting Bracket",
      "Bay/Corner Post Mounting Bracket",
    ],
    Extrusion: ["19mm x 19mm", "32mm x 9.5mm", "44.5mm x 25mm"],
    Hinges: ["60mm Non-Mortise Hinge", "60mm Rabbet Hinge", "Hinge Spacer"],
    Catches: ["Roller Catch", "Roller Catch Ramp"],
    "Track Hardware": [
      "Top Track",
      "Bottom M Track",
      "Top Pivot + Bracket",
      "Wheel Carrier + Bolt",
      "Bottom Pivot + Bracket",
      "Spring Loaded Guide",
      "Track Stop",
      "Adjustment Spanners",
    ],
    Misc: ["Small Corner Block", "Medium Corner Block", "Large Corner Block"],
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
    component.appendChild(optionElement);
  });
}

function bindColour(category, component) {
  return new Promise((resolve) => {
    const colour = document.getElementById("colour");
    colour.innerHTML = "";

    if (!category || !component) {
      resolve();
      return;
    }

    let options = [{ value: "", text: "" }];

    if (category === "Louvres") {
      if (component === "63mm Louvre" || component === "89mm Louvre") {
        options = [
          { value: "", text: "" },
          { value: "Cotton", text: "COTTON" },
        ];
      }
      if (
        component === "Louvre Pin - Easy Tilt" ||
        component === "Louvre Pin - Standard/Repair"
      ) {
        options = [
          { value: "", text: "" },
          { value: "White", text: "WHITE" },
        ];
      }
    } else if (category === "Framing | Hinged") {
      options = [
        { value: "", text: "" },
        { value: "Cotton", text: "COTTON" },
      ];
    } else if (category === "Framing | Bi-fold or Sliding") {
      if (
        component === "92mm x 22mm Header/Sideboard" ||
        component === "152mm x 22mm Header/Sideboard" ||
        component === "185mm x 22mm Header/Sideboard" ||
        component === "32mm x 10mm Fascia-Sideboard" ||
        component === "67mm x 13mm Top Fascia" ||
        component === "95mm x 13mm Top Fascia" ||
        component === "107mm x 13mm Top Fascia" ||
        component === "135mm x 13mm Top Fascia" ||
        component === "19mm x 19mm Light Strip" ||
        component === "32mm x 25mm Light Strip"
      ) {
        options = [
          { value: "", text: "" },
          { value: "Cotton", text: "COTTON" },
        ];
      } else if (
        component === "Header Mounting Strip (F Strip)" ||
        component === "Sideboard Mounting Strip (L Strip)" ||
        component === "106mm Header Support Bracket" ||
        component === "142mm Header Support Bracket"
      ) {
        options = [
          { value: "", text: "" },
          { value: "White", text: "WHITE" },
        ];
      } else {
        options = [{ value: "", text: "" }];
      }
    } else if (category === "Framing | Fixed") {
      options = [
        { value: "", text: "" },
        { value: "Cotton", text: "COTTON" },
      ];
    } else if (category === "Posts") {
      if (
        component === "T Post" ||
        component === "90° Corner Post" ||
        component === "135° Bay Post"
      ) {
        options = [
          { value: "", text: "" },
          { value: "Cotton", text: "COTTON" },
        ];
      } else if (
        component === "T Post Mounting Block" ||
        component === "90° Corner Post Mounting Block" ||
        component === "135° Bay Post Mounting Block" ||
        component === "T Post Mounting Bracket" ||
        component === "Bay/Corner Post Mounting Bracket"
      ) {
        options = [
          { value: "", text: "" },
          { value: "White", text: "WHITE" },
        ];
      } else {
        options = [{ value: "", text: "" }];
      }
    } else if (category === "Extrusion") {
      options = [
        { value: "", text: "" },
        { value: "Cotton", text: "COTTON" },
      ];
    } else if (category === "Hinges") {
      if (
        component === "60mm Non-Mortise Hinge" ||
        component === "60mm Rabbet Hinge"
      ) {
        options = [
          { value: "", text: "" },
          { value: "White", text: "WHITE" },
          { value: "Stainless Steel", text: "STAINLESS STEEL" },
        ];
      }
    } else if (category === "Track Hardware") {
      if (component === "Top Track") {
        options = [
          { value: "", text: "" },
          { value: "White", text: "WHITE" },
        ];
      } else if (component === "Bottom M Track") {
        options = [
          { value: "", text: "" },
          { value: "Silver", text: "SILVER" },
          { value: "White", text: "WHITE" },
        ];
      } else {
        options = [{ value: "", text: "" }];
      }
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      colour.appendChild(optionElement);
    });
    resolve();
  });
}

function bindComponentForm(Data) {
  const divDetail = document.getElementById("divDetail");
  divDetail.style.display = Data ? "" : "none";

  if (Data) {
    const category = document.getElementById("category").value;
    const component = document.getElementById("component").value;

    visibleColour(category, component);
    visibleLength(category, component);
  }
}

function visibleColour(category, component) {
  return new Promise((resolve) => {
    const divColour = document.getElementById("divColour");

    divColour.style.display = "none";

    if (!category || !component) {
      resolve();
      return;
    }

    if (
      category === "Louvres" ||
      category === "Framing | Hinged" ||
      category === "Framing | Bi-fold or Sliding" ||
      category === "Framing | Fixed" ||
      category === "Posts" ||
      category === "Extrusion" ||
      category === "Hinges"
    ) {
      divColour.style.display = "";
      if (
        component === "Fascia H Clip" ||
        component === "Fascia Return Connector" ||
        component === "Hinge Spacer"
      ) {
        divColour.style.display = "none";
      }
    }
    if (
      category === "Track Hardware" &&
      (component === "Top Track" || component === "Bottom M Track")
    ) {
      divColour.style.display = "";
    }

    resolve();
  });
}

function visibleLength(category, component) {
  return new Promise((resolve) => {
    const divLength = document.getElementById("divLength");

    divLength.style.display = "none";

    if (!category || !component) {
      resolve();
      return;
    }

    if (
      category === "Louvres" &&
      (component === "63mm Louvre" || component === "89mm Louvre")
    ) {
      divLength.style.display = "";
    }
    if (
      category === "Framing | Hinged" ||
      category === "Framing | Fixed" ||
      category === "Extrusion"
    ) {
      divLength.style.display = "";
    }
    if (category === "Framing | Bi-fold or Sliding") {
      divLength.style.display = "";
      if (
        component === "Fascia H Clip" ||
        component === "Fascia Return Connector" ||
        component === "106mm Header Support Bracket" ||
        component === "142mm Header Support Bracket"
      ) {
        divLength.style.display = "none";
      }
    }
    if (
      category === "Posts" &&
      (component === "T Post" ||
        component === "90° Corner Post" ||
        component === "135° Bay Post")
    ) {
      divLength.style.display = "";
    }

    if (
      category === "Track Hardware" &&
      (component === "Top Track" || component === "Bottom M Track")
    ) {
      divLength.style.display = "";
    }

    resolve();
  });
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
      const msg = xhr.responseText;
      isError(msg);
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
