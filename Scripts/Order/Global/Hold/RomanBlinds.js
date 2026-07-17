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
// ---------------------------------------------|| Input Event ||---------------------------------------
// BUTTON CANCEL
$("#btnCancel").on(
  "click",
  () =>
    (window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`),
);

// BUTTON SUBMIT
$("#btnSubmit").on("click", submitForm);
// ---------------------------------------------|| Input Event ||---------------------------------------
// CHANGE BLIND TYPE
$("#blindtype").on("change", function (e) {
  $(this).removeClass("is-invalid");

  const blindid = $(this).val();

  const blindname = $(this).find("option:selected").data("name");
  const controlname = $("#controltype option:selected").data("name");
  const fabrictype = $(this).find("option:selected").data("type");

  bindControlType(DESIGNID, blindid);
  // bindMounting(blindname);
  // bindFabricType(DESIGNID, blindname, controlname);
  // bindFabricColour(DESIGNID, fabrictype);
  // bindControlPosition(blindname);
  // //for chained
  // bindMaterialChain(blindname);
  // bindChainColour(blindname);
  // //for cordlock
  // bindCordColour(blindname);
  // bindBattenColour(blindname);
  // bindPlasticColour(blindname);
  // bindCleat(blindname);
});

// CHANGE CONTROL TYPE
$("#controltype").on("change", function (e) {
  $(this).removeClass("is-invalid");

  const blindname = $("#blindtype option:selected").data("name");
  const controlname = $(this).find("option:selected").data("name");
  const fabrictype = $(this).find("option:selected").data("type");
  const materialchain = $("#materialchain").val();

  bindMounting(blindname);
  bindFabricType(DESIGNID, blindname, controlname);
  bindFabricColour(DESIGNID, fabrictype);
  bindControlPosition(blindname);
  //for chained
  bindMaterialChain(blindname);
  bindChainColour(blindname, materialchain);
  //for cordlock
  bindCordColour(blindname);
  bindBattenColour(blindname);
  bindPlasticColour(blindname);
  bindCleat(blindname);
  handlerDisplayElemets(blindname, controlname);
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

// CHANGE CONTROL POSITION
$("#controlposition").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE MATERIAL CHAIN
$("#materialchain").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindname = $("#blindtype option:selected").data("name");
  const materialchain = $(this).val();
  bindChainColour(blindname, materialchain);
});

// CHANGE CHAIN COLOUR
$("#chaincolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT CHAIN LENGTH
$("#chainlength").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE CORD COLOUR
$("#cordcolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT CORD LENGTH
$("#cordlength").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE BATTEN COLOUR
$("#battencolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE PLASTIC COLOUR
$("#plasticcolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE CLEAT
$("#cleat").on("change", function (e) {
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

// ==================================================FUNCTION================================================
// ---------------------------------------------|| Submit Function ||---------------------------------------
// SUBMIT FORM
function submitForm() {
  if (ITEMACTION === "AddItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Save Changes";
  }
  if (ITEMACTION === "EditItem" || ITEMACTION === "CopyItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Save Changes";
  }

  handlerResetFormError();

  const fields = [
    "blindtype", // as Kit Id
    "controltype", // as Kit Id
    "qty", // as Qty
    "room", // as Location
    "mounting", // as Mounting
    "fabrictype", // as FabricId
    "fabriccolour", // as FabricId
    "width", // as Width
    "drop", // as Drop
    "controlposition", // as ControlPosition
    "materialchain", // as New MaterialChain
    "chaincolour", // as ChainId
    "chainlength", // as ChainLength
    "cordcolour", // as New CordColour
    "cordlength", // as New CordLength
    "battencolour", // as BattenColour
    "plasticcolour", // as New AcornPlasticColour
    "cleat", // as New Cleat
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
    url: URIMETHOD + "/SaveData",
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
}
// ---------------------------------------------|| Binding Function ||---------------------------------------
// BIND DATA BLIND TYPE
function bindBlindType(designid) {
  return new Promise((resolve, reject) => {
    const blindtype = document.getElementById("blindtype");
    blindtype.innerHTML = ""; //reset

    if (!designid) return resolve();

    bindControlType(designid, blindtype.value);

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
            bindControlType(designid, blindtype.value);
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
function bindControlType(designid, blindid) {
  return new Promise((resolve, reject) => {
    const controltype = document.getElementById("controltype");
    controltype.innerHTML = ""; //reset

    if (!blindid) return resolve();

    const sel = document.getElementById("blindtype");
    const blindName = sel.selectedOptions[0].getAttribute("data-name");
    handlerDisplayElemets(blindName, controltype.value);

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
              ? "No data returned from server : bindControlType"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }
        if (Array.isArray(data)) {
          controltype.innerHTML = ""; //reset

          if (data.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.value = "";
            defaultOption.text = "";
            controltype.appendChild(defaultOption);
          }

          data.forEach((item) => {
            const option = document.createElement("option");
            option.value = item.value;
            option.text = item.text.toUpperCase();
            option.setAttribute("data-name", item.text);
            controltype.appendChild(option);
            controltype.classList.add("fw-bold");
          });

          handlerDisplayElemets(
            blindName,
            controltype.selectedOptions[0].getAttribute("data-name"),
          );

          if (data.length === 1) {
            controltype.selectedIndex = 0;
            const controlname =
              controltype.selectedOptions[0].getAttribute("data-name");

            const fabrictype = $(this).find("option:selected").data("type");
            bindMounting(blindName);
            bindFabricType(designid, blindName, controlname);
            bindFabricColour(designid, fabrictype);
            bindControlPosition(blindName);
            //for chained
            bindMaterialChain(blindName);
            bindChainColour(blindName);
            //for cordlock
            bindCordColour(blindName);
            bindBattenColour(blindName);
            bindPlasticColour(blindName);
            bindCleat(blindName);
            handlerDisplayElemets(blindName, controlname);
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
      case "Classic":
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
function bindFabricType(designid, blindname, controlname) {
  return new Promise((resolve, reject) => {
    const sel = document.getElementById("fabrictype");
    sel.innerHTML = ""; //reset

    if (!designid || !blindname || !controlname) return resolve();

    bindFabricColour(designid, sel.value);
    console.log(designid, blindname, controlname);

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/BindFabricType",
      data: JSON.stringify({
        designid: designid,
        blindname: blindname,
        controlname: controlname,
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

// BIND CONTROL POSITION
function bindControlPosition(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("controlposition");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "RHC", label: "RHC" },
          { value: "LHC", label: "LHC" },
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

// BIND BATTEN COLOUR
function bindBattenColour(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("battencolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Plantation":
        data = [
          { value: "Alabaster", label: "Alabaster" },
          { value: "Batlic", label: "Batlic" },
          { value: "Black", label: "Black" },
          { value: "Brown", label: "Brown" },
          { value: "Cherry", label: "Cherry" },
          { value: "Natural", label: "Natural" },
          { value: "Teak", label: "Teak" },
          { value: "White", label: "White" },
        ];
        break;
      case "Sewless":
        data = [{ value: "Aluminium-Ivory", label: "Aluminium-Ivory" }];
        break;
    }

    if (data.length > 1) {
      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      select.add(defaultOption);
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

// BIND MATERIAL CHAIN
function bindMaterialChain(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("materialchain");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Chrome", label: "Chrome" },
          { value: "Plastic", label: "Plastic" },
          { value: "Stailess Steel", label: "Stailess Steel" },
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

// BIND CHAIN COLOUR
function bindChainColour(blindName, materialChain) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("chaincolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName || !materialChain) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        if (["Chrome"].includes(materialChain)) {
          data.push({
            value: "Chrome",
            label: "Chrome",
          });
        }

        if (["Plastic"].includes(materialChain)) {
          data.push(
            { value: "Ivory", label: "Ivory" },
            { value: "White", label: "White" },
            { value: "Black", label: "Black" },
            { value: "Beige", label: "Beige" },
            { value: "Grey", label: "Grey" },
          );
        }
        if (["Stailess Steel"].includes(materialChain)) {
          data.push({
            value: "Stailess Steel",
            label: "Stailess Steel",
          });
        }
        break;
    }

    if (data.length > 1) {
      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      select.add(defaultOption);
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

// BIND CORD COLOUR
function bindCordColour(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("cordcolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Alabaster", label: "Alabaster" },
          { value: "Mahogany", label: "Mahogany" },
          { value: "Teak", label: "Teak" },
          { value: "White", label: "White" },
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

// BIND ACORN PLACTIC COLOUR
function bindPlasticColour(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("plasticcolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Alabaster", label: "Alabaster" },
          { value: "Mahogany", label: "Mahogany" },
          { value: "Teak", label: "Teak" },
          { value: "White", label: "White" },
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

// BIND CLEAT
function bindCleat(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("cleat");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Plastic", label: "Plastic" },
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
            .then(() => bindControlType(item.DesignId, item.BlindId))
            .then(() => bindMounting(item.BlindName))
            .then(() =>
              bindFabricType(item.DesignId, item.BlindName, item.ControlType),
            )
            .then(() => bindFabricColour(item.DesignId, item.FabricType))
            .then(() => bindBattenColour(item.BlindName))
            .then(() => bindControlPosition(item.BlindName))
            .then(() => bindMaterialChain(item.BlindName))
            .then(() => bindChainColour(item.BlindName, item.MaterialChain))
            .then(() => bindCordColour(item.BlindName))
            .then(() => bindPlasticColour(item.BlindName))
            .then(() => bindCleat(item.BlindName))
            .then(() => setFormValues(item))
            .then(() =>
              handlerDisplayElemets(item.BlindName, item.ControlType),
            );
          // .then(() => {
          //   return Promise.all([visibleBattenColour(item.Batten)])
          //     .then(resolve)
          //     .catch(reject);
          // });
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
    controltype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    width: "Width",
    drop: "Drop",
    materialchain: "MaterialChain",
    controlposition: "ControlPosition",
    chaincolour: "ChainColour",
    chainlength: "ChainLength",
    cordcolour: "CordColour",
    cordlength: "CordLength",
    battencolour: "BattenColour",
    plasticcolour: "AcornPlasticColour",
    cleat: "Cleat",
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
// ---------------------------------------------|| Handler Function ||---------------------------------------
// HANDLER PAGE TITLE
function handlerPageTitle(designid) {
  return new Promise((resolve, reject) => {
    if (!designid) return resolve();

    $.ajax({
      type: "POST",
      url: URIMETHOD + "/GetDesignName",
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
              ? "No data returned from server : handlerPageTitle"
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

// HANDLER HEADER INFO
function handlerHeaderInfo(headerid) {
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
              ? "No data returned from server : handlerHeaderInfo"
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

// HANDLER INFO ACTION
function handlerInfoAction(itemaction) {
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

// HANDLER DISPLAY ELEMENTS
function handlerDisplayElemets(blindname, controlname) {
  // DEFINE ELEMENTS
  const btnSubmit = document.getElementById("btnSubmit");
  const divFormDetail = document.getElementById("divFormDetail");
  divFormDetail.setAttribute("hidden", true);

  const divChained = document.getElementById("divChained");
  const divCordlock = document.getElementById("divCordlock");
  const divBattenColour = document.getElementById("divBattenColour");
  const divPlasticColour = document.getElementById("divPlasticColour");
  const divCleat = document.getElementById("divCleat");

  //   console.log("blindname :" + blindname);
  //   console.log("controlname :" + controlname);

  if (controlname) {
    divFormDetail.removeAttribute("hidden");
    // SET DEFAULT HIDE ELEMENT
    divChained.setAttribute("hidden", true);
    divCordlock.setAttribute("hidden", true);
    divBattenColour.setAttribute("hidden", true);
    divPlasticColour.setAttribute("hidden", true);
    divCleat.setAttribute("hidden", true);
    switch (controlname) {
      case "Chain":
        divChained.removeAttribute("hidden");
        divBattenColour.removeAttribute("hidden");
        break;
      case "Cord":
        if (blindname !== "Classic") {
          divBattenColour.removeAttribute("hidden");
        }
        divCordlock.removeAttribute("hidden");
        divPlasticColour.removeAttribute("hidden");
        divCleat.removeAttribute("hidden");
        break;
    }
  }

  if (ITEMACTION == "AddItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Save Changes";
  } else if (ITEMACTION == "EditItem" || ITEMACTION == "CopyItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Save Changes";
  } else if (ITEMACTION == "ViewItem") {
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Save Changes";
    if (ROLENAME !== "Administrator") btnSubmit.setAttribute("hidden", true);
  }
}

// RESET FORM IS INVALID
function handlerResetFormError() {
  document
    .querySelectorAll(".form-control, .form-select")
    .forEach((element) => {
      element.classList.remove("is-invalid");
    });
}
// ---------------------------------------------|| Other Function ||-----------------------------------------
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

  await handlerPageTitle(DESIGNID);
  await handlerHeaderInfo(HEADERID);
  await handlerInfoAction(ITEMACTION);

  if (ITEMACTION === "AddItem") {
    await handlerDisplayElemets(); //blindname, controltype
    await bindBlindType(DESIGNID);
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    await bindItemOrder(ITEMID);
    loaderFadeOut();
  }
};
