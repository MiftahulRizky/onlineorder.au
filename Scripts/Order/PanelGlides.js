$(document).ready(function () {
  if (ROLENAME === "Administrator") {
    console.log("panelglides.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  checkSession();
});

// ==================================================EVENTS==================================================
// #-------------------------|| Button Event ||-------------------------#
// BUTTON CANCEL
$("#btnCancel").on(
  "click",
  () =>
    (window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`)
);

// BUTTON SUBMIT
$("#btnSubmit").on("click", submitForm);

// #-------------------------|| Input Event ||-------------------------#
// CHANGE BLIND TYPE
$("#blindtype").on("change", function (e) {
  $(this).removeClass("is-invalid");
  $("#divFormDetail").attr("hidden", true);

  const blindId = $(this).val();

  const blindName = $(this).find("option:selected").data("name");
  const fabrictype = $(this).find("option:selected").data("type");
  console.log("fabrictype: " + fabrictype);

  bindColourType(DESIGNID, blindId);
  bindMounting(blindName);
  bindFabricType(DESIGNID, blindName);
  bindFabricColour(DESIGNID, fabrictype);
  bindLayoutCode(blindName);
  bindNoPanel();
  bindTrackType();
  bindTrackColour();
  bindWandPosition();
  bindWandColour();
  bindBattenColour();
});

// CHANGE COLOUR TYPE
$("#colourtype").on("change", function (e) {
  $(this).removeClass("is-invalid");

  const blindName = $("#blindtype option:selected").data("name");
  const colourtype = $(this).val();
  visibleElementForm(blindName, colourtype);
});

// INPUT QTY
$("#qty").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT ROOM
$("#room").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT MOUNTING
$("#mounting").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE FABRIC TYPE
$("#fabrictype").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const fabrictype = $(this).find("option:selected").data("type");
  bindFabricColour(DESIGNID, fabrictype);
});

// CHANGE FABRIC COLOUR
$("#fabriccolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT WIDTH
$("#width").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT DROP
$("#drop").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE LAYOUT CODE
$("#layoutcode").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE NO PANEL
$("#nopanel").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE TRACK TYPE
$("#tracktype").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE TRACK COLOUR
$("#trackcolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE WAND POSITION
$("#wandposition").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT WAND LENGTH
$("#wandlength").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE WAND COLOUR
$("#wandcolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE BATTEN
$("#batten").on("change", function (e) {
  $(this).removeClass("is-invalid");
  visibleBattenColour($(this).val());
});

// CHANGE BATTEN COLOUR
$("#battencolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE FITTING
$("#fitting").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT NOTES
$("#notes").on("change", function (e) {
  $(this).removeClass("is-invalid");
  let maxLength = 1000;
  let currentLength = $(this).val().length;
  $("#notescount").text(`${currentLength}/${maxLength}`);
});

// CHANGE MARKUP
$("#markup").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// ==================================================FUNCTIONS===============================================
// #-------------------------|| Binding Function ||-------------------------#
// BIND DESIGN TYPE
function bindDesignType(designid) {
  return new Promise((resolve, reject) => {
    if (!designid) return resolve();

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/GetDesignType",
      data: JSON.stringify({
        designid: designid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (!data) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindDesignType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        document.getElementById("pageTitle").innerHTML = data.designName;
        document.getElementById("pageAction").innerHTML = ITEMACTION;
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND ITEM ORDER
function bindFormAction(itemaction) {
  const cardTitle = document.getElementById("cardTitle");
  // if (!cardTitle) return console.warn("Elemen 'cardTitle' tidak ditemukan.");

  const actionMap = {
    AddItem: "ADD ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemaction] || "";
}

// BIND DATA HEADER
function bindDataHeader(headerid) {
  return new Promise((resolve, reject) => {
    if (!headerid) return resolve();

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/GetHeaderData",
      data: JSON.stringify({
        headerid: headerid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (!data) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindDataHeader"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const divOrderNo = document.getElementById("divOrderNo");
        const divOrderCust = document.getElementById("divOrderCust");

        divOrderNo.innerHTML = data.orderNo;
        divOrderNo.classList.add("fw-bold");

        divOrderCust.innerHTML = data.orderCust;
        divOrderCust.classList.add("fw-bold");

        resolve(data);
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND DATA BLIND TYPE
function bindBlindType(designid) {
  return new Promise((resolve, reject) => {
    const blindtype = document.getElementById("blindtype");
    blindtype.innerHTML = ""; //reset

    if (!designid) return resolve();

    bindColourType(designid, blindtype.value);

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindBlindType",
      data: JSON.stringify({
        designid: designid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindBlindType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          blindtype.innerHTML = ""; //reset

          if (data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            blindtype.add(defaultOption);
          }

          data.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            blindtype.add(option);
            blindtype.classList.add("fw-bold");
          });

          if (data.length === 1) {
            blindtype.selectedIndex = 0;
            bindColourType(designid, blindtype.value);
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND COLOUR TYPE
function bindColourType(designid, blindid) {
  return new Promise((resolve, reject) => {
    const colourtype = document.getElementById("colourtype");
    colourtype.innerHTML = ""; //reset

    if (!blindid) return resolve();

    const sel = document.getElementById("blindtype");
    const blindName = sel.selectedOptions[0].getAttribute("data-name");

    visibleElementForm(blindName, colourtype.value);

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindColourType",
      data: JSON.stringify({
        designid: designid,
        blindid: blindid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindColourType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }
        if (Array.isArray(data)) {
          colourtype.innerHTML = ""; //reset

          if (data.length > 0) {
            const defaultOption = document.createElement("option");
            defaultOption.value = "";
            defaultOption.text = "";
            colourtype.appendChild(defaultOption);
          }

          data.forEach((item) => {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            colourtype.appendChild(option);
            colourtype.classList.add("fw-bold");
          });

          if (data.length === 1) {
            colourtype.selectedIndex = 0;
            visibleElementForm(blindName, colourtype.value);
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND MOUNTING
function bindMounting(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("mounting");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Plain":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Make Size", label: "Make Size" },
          { value: "Face Fit", label: "Face Fit" },
          { value: "Reveal fit", label: "Reveal fit" },
        ];
        break;
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND FABRIC TYPE
function bindFabricType(designid, blindname) {
  return new Promise((resolve, reject) => {
    const sel = document.getElementById("fabrictype");
    sel.innerHTML = ""; //reset

    if (!designid || !blindname) return resolve();

    bindFabricColour(designid, sel.value);

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindFabricType",
      data: JSON.stringify({
        designid: designid,
        blindname: blindname,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindFabricType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          if (data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            sel.add(defaultOption);
          }

          data.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-type", item.text);
            sel.add(option);
          });

          if (data.length === 1) {
            sel.selectedIndex = 0;
            bindFabricColour(designid, sel.value);
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND FABRIC TYPE
function bindFabricColour(designid, fabrictype) {
  return new Promise((resolve, reject) => {
    const sel = document.getElementById("fabriccolour");
    sel.innerHTML = ""; //reset

    if (!designid || !fabrictype) return resolve();

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindFabricColour",
      data: JSON.stringify({
        designid: designid,
        fabrictype: fabrictype,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindFabricColour"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        if (Array.isArray(data)) {
          sel.innerHTML = ""; //reset

          if (data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            sel.add(defaultOption);
          }

          data.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-colour", item.text);
            sel.add(option);
          });

          if (data.length === 1) {
            sel.selectedIndex = 0;
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND LAYOUT CODE
function bindLayoutCode(blindname) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("layoutcode");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    if (!blindname) return resolve();

    let data = [];
    switch (blindname) {
      case "Plain":
      case "Sewless":
      case "Plantation":
        data = [
          { value: "", text: "" },
          { value: "A", text: "A" },
          { value: "B", text: "B" },
          { value: "C", text: "C" },
          { value: "D", text: "D" },
          { value: "E", text: "E" },
          { value: "F", text: "F" },
        ];
        break;
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text;
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND NO OF PANEL
function bindNoPanel() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("nopanel");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data.push({ value: "", text: "" });
    for (let i = 2; i <= 9; i++) {
      data.push({ value: i, text: i });
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text;
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND TRACK TYPE
function bindTrackType() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("tracktype");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "2 Channel Track", text: "2 Channel Track" },
      { value: "3 Channel Track", text: "3 Channel Track" },
      { value: "4 Channel Track", text: "4 Channel Track" },
      { value: "5 Channel Track", text: "5 Channel Track" },
      { value: "6 Channel Track", text: "6 Channel Track" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND TRACK COLOUR
function bindTrackColour() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("trackcolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "Black", text: "Black" },
      { value: "Grey", text: "Grey" },
      { value: "White", text: "White" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND WAND POSITION
function bindWandPosition() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("wandposition");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "Back", text: "Back" },
      { value: "Front", text: "Front" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND WAND COLOUR
function bindWandColour() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("wandcolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "Black", text: "Black" },
      { value: "Grey", text: "Grey" },
      { value: "White", text: "White" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND BATTEN COLOUR
function bindBattenColour() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("battencolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    let data = [];
    data = [
      { value: "", text: "" },
      { value: "Aluminium", text: "Aluminium" },
      { value: "Timber - Alabaster", text: "Timber - Alabaster" },
      { value: "Timber - Batlic", text: "Timber - Batlic" },
      { value: "Timber - Black", text: "Timber - Black" },
      { value: "Timber - Brown", text: "Timber - Brown" },
      { value: "Timber - Cherry", text: "Timber - Cherry" },
      { value: "Timber - Natural", text: "Timber - Natural" },
      { value: "Timber - Teak", text: "Timber - Teak" },
      { value: "Timber - White", text: "Timber - White" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND ITEM ORDER FOR EDIT ONLY
function bindItemOrder(itemid) {
  return new Promise((resolve, reject) => {
    if (!itemid) return resolve();
    // console.log("bindItemOrder", itemid);

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindItemOrder",
      data: JSON.stringify({
        itemid: itemid,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            ROLENAME === "Administrator"
              ? "No data returned from server : bindItemOrder"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        const promises = data.map((item) => {
          return Promise.resolve()
            .then(() => bindBlindType(item.DesignId))
            .then(() => bindColourType(item.DesignId, item.BlindId))
            .then(() => bindMounting(item.BlindName))
            .then(() => bindFabricType(item.DesignId, item.BlindName))
            .then(() => bindFabricColour(item.DesignId, item.FabricType))
            .then(() => bindLayoutCode(item.BlindName))
            .then(() => bindNoPanel())
            .then(() => bindTrackType())
            .then(() => bindTrackColour())
            .then(() => bindWandPosition())
            .then(() => bindWandColour())
            .then(() => bindBattenColour())
            .then(() => setFormValues(item))
            .then(() => visibleElementForm(item.BlindName, item.KitId))
            .then(() => {
              return Promise.all([visibleBattenColour(item.Batten)])
                .then(resolve)
                .catch(reject);
            });
        });

        Promise.all(promises)
          .then(() => resolve())
          .catch((error) => reject(error));
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          ROLENAME === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

function setFormValues(itemData) {
  const mapping = {
    blindtype: "BlindId",
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    width: "Width",
    drop: "Drop",
    layoutcode: "Layout",
    nopanel: "NumOfPanel",
    tracktype: "TrackType",
    trackcolour: "TrackColour",
    wandposition: "WandPosition",
    wandlength: "WandLength",
    wandcolour: "WandColour",
    batten: "Batten",
    battencolour: "BattenColour",
    fitting: "Fitting",
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

    // Set value to empty if value is 0
    if (el) el.value = el.value === "0" ? "" : el.value;
  });
  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  $("#notescount").text(`${notesLength}/${maxLength}`);

  if (ITEMACTION === "CopyItem") {
    const resetFields = ["room", "width", "drop", "notes"];
    resetFields.forEach((id) => {
      const el = document.getElementById(id);
      if (el) el.value = "";
    });

    $("#notescount").text(`0/${maxLength}`);
  }
}

// #-------------------------|| Submit Function ||-------------------------#
function submitForm() {
  if (ITEMACTION === "AddItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit";
  }
  if (ITEMACTION === "EditItem" || ITEMACTION === "CopyItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit";
  }

  resetFormError();

  const fields = [
    "blindtype", // as Kit Id
    "colourtype", // as Kit Id
    "qty", // as Qty
    "room", // as Location
    "mounting", // as Mounting
    "fabrictype", // as FabricId
    "fabriccolour", // as FabricId
    "width", // as Width
    "drop", // as Drop
    "layoutcode", // as LayoutCode
    "nopanel", // as New NoPanel
    "tracktype", // as TrackType
    "trackcolour", // as TrackColour
    "wandposition", // as New WandPosition
    "wandlength", // as WandLength
    "wandcolour", // as WandColour
    "batten", // as New Batten
    "battencolour", // as New BattenColour
    "fitting", // as New Fitting
    "notes", // as Notes
    "markup", // as Markup
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

  $.ajax({
    type: "post",
    url: URIMETHOD + "/SubmitForm",
    data: JSON.stringify({ data: formData }),
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    beforeSend: function () {
      $("#btnSubmit").attr("disable", "disable");
      $("#btnSubmit").html('<i class="fa fa-spin fa-spinner"</i>');
    },
    complete: function () {
      $("#btnSubmit").removeAttr("disable");
      $("#btnSubmit").html(htmlButtonSubmit);
    },
    success: function (response) {
      const result = response.d || response;
      if (result.error) {
        isError(result.error.message.toUpperCase()).then(() => {
          const el = document.getElementById(result.error.field);
          if (el) {
            // el.focus();
            el.classList.add("is-invalid");
          }
        });
      } else {
        isSuccess(result.success).then(() => {
          window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
        });
      }
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var msg =
        ROLENAME === "Administrator"
          ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
          : "Something went wrong, please try again!";
      isError(msg);
    },
  });
  return false;
  // console.log(formData);
}
// #-------------------------|| Visible Function ||-----------------------#
// VISIBLE ELEMENT FORM FUNCTION
function visibleElementForm(blindname, colourtype) {
  //DEFIND ELEMENTS
  const btnSubmit = document.getElementById("btnSubmit");
  const divFormDetail = document.getElementById("divFormDetail");
  const divBattenColour = document.getElementById("divBattenColour");
  const divMarkUp = document.getElementById("divMarkUp");

  // SET DEFAULT HIDE ELEMENT
  divFormDetail.setAttribute("hidden", true);
  divBattenColour.setAttribute("hidden", true);
  divMarkUp.setAttribute("hidden", true);
  if (colourtype) {
    divFormDetail.removeAttribute("hidden");
  }

  if (MARKUPACCESS === "True") divMarkUp.removeAttribute("hidden");

  if (ITEMACTION == "AddItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit";
  } else if (ITEMACTION == "EditItem" || ITEMACTION == "CopyItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit";
  } else if (ITEMACTION == "ViewItem") {
    if (ROLENAME !== "Administrator") btnSubmit.setAttribute("hidden", true);
  }
}

// VISIBLE BATTEN COLOUR
function visibleBattenColour(batten) {
  const divBattenColour = document.getElementById("divBattenColour");
  divBattenColour.removeAttribute("hidden");
  if (batten === "No" || batten === "") {
    divBattenColour.setAttribute("hidden", true);
  }
}
// #-------------------------|| Other Function ||-------------------------#
// SESSION FUNCTION
const checkSession = async () => {
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

  await bindDesignType(DESIGNID);
  await bindDataHeader(HEADERID);
  bindFormAction(ITEMACTION);

  if (ITEMACTION === "AddItem") {
    visibleElementForm();
    await bindBlindType(DESIGNID);
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    await bindItemOrder(ITEMID);
    loaderFadeOut();
  }
};

// RESET FORM IS INVALID
function resetFormError() {
  document
    .querySelectorAll(".form-control, .form-select")
    .forEach((element) => {
      element.classList.remove("is-invalid");
    });
}
