$(document).ready(function () {
  if (roleName === "Administrator") {
    console.log("panelglides.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("itemaction: " + itemAction);
    console.log("itemId: " + itemId);
    console.log("userId: " + userId);
    console.log("uriMethod: " + uriMethod);
  }
  checkSession();
});

// ==================================================EVENTS==================================================
// ---------------------------------------------|| Input Event ||---------------------------------------
// BUTTON CANCEL
$("#btnCancel").on("click", () => (window.location.href = "/order/detail"));

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

  bindControlType(designId, blindid);
  bindMounting(blindname);
  bindFabricType(designId, blindname, controlname);
  bindFabricColour(designId, fabrictype);
  bindControlPosition(blindname);
  //for chained
  bindMaterialChain(blindname);
  bindChainColour(blindname);
  //for cordlock
  bindCordColour(blindname);
  bindBattenColour(blindname);
  bindPlasticColour(blindname);
  bindCleat(blindname);
});

// CHANGE CONTROL TYPE
$("#controltype").on("change", function (e) {
  $(this).removeClass("is-invalid");

  const blindname = $("#blindtype option:selected").data("name");
  const controlname = $(this).find("option:selected").data("name");
  bindFabricType(designId, blindname, controlname);
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
  bindFabricColour(designId, fabrictype);
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
  if (itemAction === "AddItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)";
  }
  if (itemAction === "EditItem" || itemAction === "CopyItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Edit Item)";
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
    headerid: headerId,
    itemaction: itemAction,
    itemid: itemId,
    designid: designId,
    loginid: loginId,
  };

  fields.forEach((field) => {
    formData[field] = document.getElementById(field).value;
  });

  $.ajax({
    type: "post",
    url: uriMethod + "/SaveData",
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
          window.location.href = "/order/detail";
        });
      }
    },
    error: function (xhr, ajaxOptions, thrownError) {
      var msg =
        roleName === "Administrator"
          ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
          : "Something went wrong, please try again!";
      isError(msg);
    },
  });
  return false;
}
// ---------------------------------------------|| Binding Function ||---------------------------------------
// BIND DATA BLIND TYPE
function bindBlindType(designId) {
  return new Promise((resolve, reject) => {
    const blindtype = document.getElementById("blindtype");
    blindtype.innerHTML = ""; //reset

    if (!designId) return resolve();

    bindControlType(designId, blindtype.value);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindBlindType",
      data: JSON.stringify({
        designId: designId,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
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
            bindControlType(designId, blindtype.value);
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// BIND COLOUR TYPE
function bindControlType(designId, blindId) {
  return new Promise((resolve, reject) => {
    const controltype = document.getElementById("controltype");
    controltype.innerHTML = ""; //reset

    if (!blindId) return resolve();

    const sel = document.getElementById("blindtype");
    const blindName = sel.selectedOptions[0].getAttribute("data-name");
    handlerDisplayElemets(blindName, controltype.value);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindColourType",
      data: JSON.stringify({
        designId: designId,
        blindId: blindId,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
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
            controltype.selectedOptions[0].getAttribute("data-name")
          );

          if (data.length === 1) {
            controltype.selectedIndex = 0;
            const controlname =
              controltype.selectedOptions[0].getAttribute("data-name");
            bindFabricType(designId, blindName, controlname);
            handlerDisplayElemets(blindName, controlname);
          }
        }
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
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
      url: uriMethod + "/BindFabricType",
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
            roleName === "Administrator"
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
          roleName === "Administrator"
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
      url: uriMethod + "/BindFabricColour",
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
            roleName === "Administrator"
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
          roleName === "Administrator"
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
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
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
function bindChainColour(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("chaincolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!blindName) return resolve();

    let data = [];
    switch (blindName) {
      case "Classic":
      case "Plantation":
      case "Sewless":
        data = [
          { value: "", label: "" },
          { value: "Beige", label: "Beige" },
          { value: "Birch White", label: "Birch White" },
          { value: "Black", label: "Black" },
          { value: "Grey", label: "Grey" },
          { value: "Stainless Steel", label: "Stainless Steel" },
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
function bindItemOrder(itemId) {
  return new Promise((resolve, reject) => {
    if (!itemId) return resolve();
    // console.log("bindItemOrder", itemId);

    $.ajax({
      type: "POST",
      url: uriMethod + "/BindItemOrder",
      data: JSON.stringify({
        itemId: itemId,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
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
              bindFabricType(item.DesignId, item.BlindName, item.ControlType)
            )
            .then(() => bindFabricColour(item.DesignId, item.FabricType))
            .then(() => bindBattenColour(item.BlindName))
            .then(() => bindControlPosition(item.BlindName))
            .then(() => bindMaterialChain(item.BlindName))
            .then(() => bindChainColour(item.BlindName))
            .then(() => bindCordColour(item.BlindName))
            .then(() => bindPlasticColour(item.BlindName))
            .then(() => bindCleat(item.BlindName))
            .then(() => setFormValues(item))
            .then(() =>
              handlerDisplayElemets(item.BlindName, item.ControlType)
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
          roleName === "Administrator"
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

  if (itemAction === "CopyItem") {
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
function handlerPageTitle(designId) {
  return new Promise((resolve, reject) => {
    if (!designId) return resolve();

    $.ajax({
      type: "POST",
      url: uriMethod + "/GetDesignName",
      data: JSON.stringify({
        designId: designId,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (!data) {
          var msg =
            roleName === "Administrator"
              ? "No data returned from server : handlerPageTitle"
              : "Please contact our IT team at support@onlineorder.au";
          reject(isError(msg));
          return;
        }

        document.getElementById("pageTitle").innerHTML = data.designName;
        document.getElementById("pageAction").innerHTML = itemAction;
        resolve();
      },
      error: function (xhr, status, error, thrownError) {
        var msg =
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// HANDLER HEADER INFO
function handlerHeaderInfo(headerId) {
  return new Promise((resolve, reject) => {
    if (!headerId) return resolve();

    $.ajax({
      type: "POST",
      url: uriMethod + "/GetHeaderData",
      data: JSON.stringify({
        headerId: headerId,
      }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;
        if (!data) {
          var msg =
            roleName === "Administrator"
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
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// HANDLER INFO ACTION
function handlerInfoAction(itemAction) {
  const cardTitle = document.getElementById("cardTitle");
  // if (!cardTitle) return console.warn("Elemen 'cardTitle' tidak ditemukan.");

  const actionMap = {
    AddItem: "ADD ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemAction] || "";
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

  if (itemAction == "AddItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)";
  } else if (itemAction == "EditItem" || itemAction == "CopyItem") {
    //SET DEFAULT TEXT BUTTON SUBMIT
    btnSubmit.innerHTML =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Edit Item)";
  } else if (itemAction == "ViewItem") {
    if (roleName !== "Administrator") btnSubmit.setAttribute("hidden", true);
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
function checkSession() {
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

  loader(itemAction);

  setSessionAlive();

  handlerPageTitle(designId);
  handlerHeaderInfo(headerId);
  handlerInfoAction(itemAction);

  if (itemAction === "AddItem") {
    handlerDisplayElemets(); //blindname, controltype
    bindBlindType(designId);
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
    bindItemOrder(itemId);
  }
}

// LOADER FUNCTION
function loader(action) {
  const overlay = document.getElementById("loading-overlay");

  if (action === "AddItem") {
    overlay.classList.add("fade-out"); // mulai fade-out
    setTimeout(() => {
      overlay.classList.add("d-none");
      overlay.classList.remove("d-flex", "fade-out");
    }, 1000); // waktu harus sama dengan di CSS
  } else {
    overlay.classList.remove("d-none");
    overlay.classList.add("d-flex");
    overlay.classList.remove("fade-out"); // pastikan tidak dalam fade-out

    setTimeout(() => {
      overlay.classList.add("fade-out");
      setTimeout(() => {
        overlay.classList.add("d-none");
        overlay.classList.remove("d-flex", "fade-out");
      }, 500); // fade-out durasi
    }, 2000);
  }
}
