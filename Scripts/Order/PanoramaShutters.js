$(document).ready(function () {
  if (roleName === "Administrator") {
    console.log("panorama.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("itemaction: " + itemAction);
    console.log("itemId: " + itemId);
    console.log("userId: " + userId);
  }
  checkSession();
});

// ==================================================EVENTS==================================================
// #-------------------------|| Button Event ||-------------------------#
// BUTTON CANCEL
$("#btnCancel").on("click", () => (window.location.href = "/order/detail"));

// BUTTON INFO GAP
$(".btn-show-info").on("click", function (e) {
  showInfo($(this).data("params"));
});

// BUTTON SUBMIT
$("#btnSubmit").on("click", submitForm);

// #-------------------------|| Input Event ||-------------------------#
// CHANGE BLIND TYPE
$("#blindtype").on("change", function (e) {
  $(this).removeClass("is-invalid");
  $("#divFormDetail").attr("hidden", true);

  resetForm();

  const blindId = $(this).val();
  const blindName = $(this).find("option:selected").data("name");
  const mounting = document.getElementById("mounting").value;
  const louvreposition = document.getElementById("louvreposition").value;
  const louvresize = document.getElementById("louvresize").value;
  const framebottom = document.getElementById("framebottom").value;
  const midrailheight1 =
    parseFloat(document.getElementById("midrailheight1").value) || 0;

  bindColourType(designId, blindId); //designId, blindId, params as kitId
  bindMounting(blindName); //blindName, params as id
  bindLouvreSize(); //params as id
  bindPanelQty();
  bindLouvrePosition(louvresize); //louvresize, params as id
  bindHingeColour(); //params as id
  bindLayoutCode(blindName); //blindName, params as id
  bindFrameType(blindName, mounting, louvresize, louvreposition); //blindName, mounting, louvreSize, louvrePosition, params as id
  bindBottomTrackType(blindName, framebottom); //blindName, framebottom, params
  bindTiltrodType(); //params as id
  bindTiltrodSplit(midrailheight1); //midrailheight1, params
  visibleSemiInsideMount(blindName, mounting); //blindName, mounting, params
});

// CHANGE COLOUR TYPE
$("#colourtype").on("change", function (e) {
  $(this).removeClass("is-invalid");

  const blindName = $("#blindtype option:selected").data("name");
  const colourtype = $(this).val();
  visibleElementForm(blindName, colourtype); //blindName, colourtype
});

// INPUT QTY
$("#qty").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT ROOM TO INSTALL
$("#room").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE MOUNTING
$("#mounting").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const mounting = $(this).val();
  const louvreposition = $("#louvreposition option:selected").val();
  const louvresize = $("#louvresize option:selected").val();

  bindFrameType(blindName, mounting, louvresize, louvreposition); //blindName, mounting, louvreSize, louvrePosition, params as id
  visibleSemiInsideMount(blindName, mounting); //blindName, mounting, params
});

// INPUT WIDTH
$("#width").on("input", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");

  if (blindName !== "Panel Only") return;
  const widthInput = $(this).val();
  const width = parseFloat(widthInput) || 0;

  if (widthInput.length < 3) return;
  if (width < 200 || width > 900) {
    isError("MINIMUM WIDTH IS 200MM AND MAXIMUM WIDTH IS 900MM !", "width");
  }
});

// INPUT DROP
$("#drop").on("input", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");

  if (blindName === "Panel Only") {
    const drop = parseFloat($(this).val()) || 0;
    const louvresize = document.getElementById("louvresize").value;

    if ($(this).val().length < 3) return;
    if (louvresize === "63" && drop < 282) {
      isError("MINIMUM PANEL HEIGHT IS 282MM !");
    } else if (louvresize === "89" && drop < 333) {
      isError("MINIMUM PANEL HEIGHT IS 333MM !");
    } else if (louvresize === "114" && drop < 384) {
      isError("MINIMUM PANEL HEIGHT IS 384MM !");
    } else if (drop > 2500) {
      isError("MAXIMUM PANEL HEIGHT IS 2500MM !");
    }
  }
});

// CHANGE LOUVRE SIZE
$("#louvresize").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const mounting = $("#mounting option:selected").val();
  const louvresize = $(this).val();
  const louvreposition = $("#louvreposition option:selected").val();

  if (blindName == "Panel Only") {
    const dropInput = $("#drop").val();
    const drop = dropInput ? parseFloat(dropInput) : 0;

    // Default enable this code
    if (dropInput.length < 3) return;
    // if (drop.length < 4) return;
    if (louvresize === "63" && drop < 282) {
      isError("MINIMUM PANEL HEIGHT IS 282MM !", "drop");
    } else if (louvresize === "89" && drop < 333) {
      isError("MINIMUM PANEL HEIGHT IS 333MM !", "drop");
    } else if (louvresize === "114" && drop < 384) {
      isError("MINIMUM PANEL HEIGHT IS 384MM !", "drop");
    } else if (drop > 2500) {
      isError("MAXIMUM PANEL HEIGHT IS 2500MM !", "drop");
    }
  }

  bindLouvrePosition(louvresize);
  bindFrameType(blindName, mounting, louvresize, louvreposition); //blindName, mounting, louvreSize, louvrePosition, params as id
});

// CHANGE LOUVRE POSITION
$("#louvreposition").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const mounting = $("#mounting option:selected").val();
  const louvresize = $("#louvresize option:selected").val();
  const louvreposition = $(this).val();

  bindFrameType(blindName, mounting, louvresize, louvreposition); //blindName, mounting, louvreSize, louvrePosition, params as id
});

// INPUT MIDRAIL HEIGHT 1
$("#midrailheight1").on("input", function (e) {
  $(this).removeClass("is-invalid");
  const midrailheight1 = parseFloat($(this).val()) || 0;
  const midrailheight2 = parseFloat($("#midrailheight2").val()) || 0;

  bindMidrailCritical(midrailheight1, midrailheight2); //midrailheight1, midrailheight2, params
  bindTiltrodSplit(midrailheight1); //midrailheight1, params
});

// INPUT MIDRAIL HEIGHT 2
$("#midrailheight2").on("input", function (e) {
  $(this).removeClass("is-invalid");
  const midrailheight1 = parseFloat($("#midrailheight1").val()) || 0;
  const midrailheight2 = parseFloat($(this).val()) || 0;

  bindMidrailCritical(midrailheight1, midrailheight2); //midrailheight1, midrailheight2, params
});

// CHANGE PANEL QTY
$("#panelqty").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE JOINED PANELS
$("#joinedpanels").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const hingecolour = $("#hingecolour option:selected").val();
  visibleHingeColour(blindName, $(this).val()); //blindName, joinedpanels, params
  visibleHingesLoose(blindName, hingecolour, $(this).val());
});

// CHANGE HINGE COLOUR
$("#hingecolour").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const joinedpanels = $("#joinedpanels option:selected").val();
  visibleHingesLoose(blindName, $(this).val(), joinedpanels);
});

// INPUT CUSTOM HEADER LENGTH
$("#customheaderlength").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE LAYOUT CODE
$("#layoutcode").on("change", function (e) {
  $(this).removeClass("is-invalid");
  $("#layoutcodecustom").val(""); // reset value
  $("#samesizepanel").val(""); // reset value
  const blindName = $("#blindtype option:selected").data("name");
  const layoutCode = $(this).val();
  visibleLayoutCustom(layoutCode); //layoutCode, params as id
  visibleSameSizePanel(blindName, layoutCode); //blindName, layoutCode, params
  visibleGap(blindName, null, layoutCode); //layoutCode, samesizepanel, layoutCode, params
});

// INPUT LAYOUT CODE CUSTOM
$("#layoutcodecustom").on("input", function (e) {
  $(this).removeClass("is-invalid");
  $(".gaps").removeClass("is-invalid");
  $("#samesizepanel").val(""); // reset value
  const blindName = $("#blindtype option:selected").data("name");
  const layoutCode = $(this).val();
  visibleSameSizePanel(blindName, layoutCode); //blindName, layoutCode, params
  visibleGap(blindName, null, layoutCode); //layoutCode, samesizepanel, layoutCode, params
});

// CHANGE FRAME TYPE
$("#frametype").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const frameType = $(this).val();
  const mounting = $("#mounting option:selected").val();
  const buildout = $("#buildout option:selected").val();
  bindFrameLeft(frameType, mounting); //frameType, mounting, params as id
  bindFrameRight(frameType, mounting); //frameType, mounting, params as id
  bindFrameTop(frameType, mounting); //frameType, mounting, params as id
  bindFrameBottom(frameType, mounting); //frameType, mounting, params as id
  bindBuildout(blindName, frameType); //blindName, frameType, params
  bindBuildoutPosition(blindName, frameType, buildout); //blindName, frameType, buildout, params
});

// CHANGE FRAME LEFT
$("#frameleft").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE FRAME RIGHT
$("#frameright").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE FRAME TOP
$("#frametop").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE FRAME BOTTOM
$("#framebottom").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const framebottom = $(this).val();
  const bottomTrackType = $("#bottomtracktype option:selected").val();
  bindBottomTrackType(blindName, framebottom);
});

// CHANGE BOTTOM TRACK TYPE
$("#bottomtracktype").on("change", function (e) {
  $(this).removeClass("is-invalid");
  visibleBottomTrackReccess($(this).val()); //bottomtracktype, params
});

// CHANGE BUILDOUT
$("#buildout").on("change", function (e) {
  $("#buildoutposition").removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const frameType = $("#frametype option:selected").val();
  const buildout = $(this).val();
  bindBuildoutPosition(blindName, buildout, frameType); //blindName, buildout, frameType, params
});

// CHANGE BUILDOUT POSITION
$("#buildoutposition").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE SAME SIZE PANEL
$("#samesizepanel").on("change", function (e) {
  $(this).removeClass("is-invalid");
  const blindName = $("#blindtype option:selected").data("name");
  const layout = $("#layoutcode option:selected").val();
  const layoutCustom = $("#layoutcodecustom").val();

  let layoutCode = layout;
  if (layout === "Other") layoutCode = layoutCustom;

  visibleGap(blindName, $(this).val(), layoutCode); //samesizepanel, layoutCode, layoutCode, params
});

// INPUT GAP 1
$("#gap1").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT GAP 2
$("#gap2").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT GAP 3
$("#gap3").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT GAP 4
$("#gap4").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT GAP 5
$("#gap5").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// INPUT HORIZONTAL POST HEIGHT
$("#horizontaltpostheight").on("input", function (e) {
  $(this).removeClass("is-invalid");
  $("#horizontaltpost").removeClass("is-invalid");
  const thisValue = parseFloat($(this).val()) || 0;
  const divHorizontalTPostRequired = document.getElementById(
    "divHorizontalTPostRequired"
  );
  divHorizontalTPostRequired.setAttribute("hidden", true); // visible false

  if (thisValue > 0) divHorizontalTPostRequired.removeAttribute("hidden");
});

// CHANGE HORIZONTAL POST REQUIRED
$("#horizontaltpost").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE TILTROD TYPE
$("#tiltrodtype").on("change", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE TILTROD ROTATION
$("#tiltrodsplit").on("change", function (e) {
  $(this).removeClass("is-invalid");
  visibleSplitHeight($(this).val());
});

// INPUT SPLIT HEIGHT 1
$("#splitheight1").on("input", function (e) {
  $(this).removeClass("is-invalid");
});
// INPUT SPLIT HEIGHT 2
$("#splitheight2").on("input", function (e) {
  $(this).removeClass("is-invalid");
});

// CHANGE SPECIAL SHAPE
$("#specialshape").on("change", function (e) {
  visibleTemplateProvided($(this).val()); //specialshape, params
});

// INPUT NOTES
$("#notes").on("input", function (e) {
  let maxLength = 1000;
  let currentLength = $(this).val().length;
  $("#notescount").text(`${currentLength}/${maxLength}`);
});
// ==================================================FUNCTIONS===============================================
// #...............................|| Submit Functions ||..........................#
function submitForm() {
  if (itemAction === "AddItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)";
  }
  if (itemAction === "EditItem" || itemAction === "CopyItem") {
    var htmlButtonSubmit =
      "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Edit Item)";
  }

  resetFormError();

  const fields = [
    "blindtype",
    "colourtype",
    "qty",
    "room",
    "mounting",
    "width",
    "drop",
    "louvresize",
    "louvreposition",
    "midrailheight1",
    "midrailheight2",
    "midrailcritical",
    "panelqty",
    "joinedpanels",
    "hingecolour",
    "semiinsidemount",
    "customheaderlength",
    "layoutcode",
    "layoutcodecustom",
    "frametype",
    "frameleft",
    "frameright",
    "frametop",
    "framebottom",
    "bottomtracktype",
    "bottomtrackrecess",
    "buildout",
    "buildoutposition",
    "samesizepanel",
    "gap1",
    "gap2",
    "gap3",
    "gap4",
    "gap5",
    "horizontaltpostheight",
    "horizontaltpost",
    "tiltrodtype",
    "tiltrodsplit",
    "splitheight1",
    "splitheight2",
    "reversehinged",
    "pelmetflat",
    "extrafascia",
    "hingesloose",
    "cutout",
    "specialshape",
    "templateprovided",
    "markup",
    "notes",
  ];

  const formData = {
    headerid: headerId,
    itemaction: itemAction,
    itemid: itemId,
    designid: designId,
    loginid: loginId,
    rolename: roleName,
  };

  fields.forEach((field) => {
    formData[field] = $(`#${field}`).val();
  });

  $.ajax({
    type: "post",
    url: uriMethod + "/SubmitForm",
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
        isError(result.error.message.toUpperCase(), result.error.field);
      } else {
        isSuccess(result.success, "/order/detail");
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
//#...............................|| Binding Functions ||..........................#
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
          let layoutCodeFinal =
            item.Layout === "Other" ? item.LayoutSpecial : item.Layout;

          return Promise.resolve()
            .then(() => bindBlindType(item.DesignId))
            .then(() => bindColourType(item.DesignId, item.BlindId))

            .then(() => bindMounting(item.BlindName))
            .then(() =>
              bindMidrailCritical(item.MidrailHeight1, item.MidrailHeight2)
            )
            .then(() => bindLouvreSize())
            .then(() => bindPanelQty())
            .then(() => bindHingeColour())
            .then(() => bindLayoutCode(item.BlindName))
            .then(() => bindHorizontalTpost())
            .then(() =>
              bindFrameType(
                item.BlindName,
                item.Mounting,
                item.LouvreSize,
                item.LouvrePosition
              )
            )
            .then(() => bindFrameLeft(item.FrameType, item.Mounting))
            .then(() => bindFrameRight(item.FrameType, item.Mounting))
            .then(() => bindFrameTop(item.FrameType, item.Mounting))
            .then(() => bindFrameBottom(item.FrameType, item.Mounting))
            .then(() => bindBottomTrackType(item.BlindName, item.FrameBottom))
            .then(() => bindTiltrodType())
            .then(() => bindTiltrodSplit(item.MidrailHeight1))
            .then(() => bindBuildout(item.BlindName, item.FrameType))
            .then(() =>
              bindBuildoutPosition(
                item.BlindName,
                item.Buildout,
                item.FrameType
              )
            )
            .then(() => setFormValues(item))
            .then(() => visibleElementForm(item.BlindName, item.KitId))
            .then(() => {
              return Promise.all([
                visibleMidrail(item.MidrailHeight1),
                visibleHingeColour(item.BlindName, item.JoinedPanels),
                visibleLayoutCustom(item.Layout),
                visibleFrameDetail(item.FrameType),
                visibleBuildout(item.BlindName, item.FrameType),
                visibleBuildoutPosition(
                  item.BlindName,
                  item.FrameType,
                  item.Buildout
                ),
                visibleSameSizePanel(item.BlindName, layoutCodeFinal),
                visibleGap(item.BlindName, item.PanelSize, layoutCodeFinal),
                visibleHingesLoose(
                  item.BlindName,
                  item.HingeColour,
                  item.JoinedPanels
                ),
                visibleSemiInsideMount(item.BlindName, item.Mounting),
                visibleBottomTrackReccess(item.BottomTrackType),
                visibleSplitHeight(item.TiltrodSplit),
                visibleTemplateProvided(item.SpecialShape),
              ])
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
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    louvresize: "LouvreSize",
    louvreposition: "LouvrePosition",
    midrailheight1: "MidrailHeight1",
    midrailheight2: "MidrailHeight2",
    midrailcritical: "MidrailCritical",
    panelqty: "PanelQty",
    joinedpanels: "JoinedPanels",
    hingecolour: "HingeColour",
    semiinsidemount: "SemiInsideMount",
    customheaderlength: "CustomHeaderLength",
    layoutcode: "Layout",
    layoutcodecustom: "LayoutSpecial",
    frametype: "FrameType",
    frameleft: "FrameLeft",
    frameright: "FrameRight",
    frametop: "FrameTop",
    framebottom: "FrameBottom",
    bottomtracktype: "BottomTrackType",
    bottomtrackrecess: "BottomTrackRecess",
    buildout: "Buildout",
    buildoutposition: "BuildoutPosition",
    samesizepanel: "PanelSize",
    gap1: "LocationTPost1",
    gap2: "LocationTPost2",
    gap3: "LocationTPost3",
    gap4: "LocationTPost4",
    gap5: "LocationTPost5",
    horizontaltpostheight: "HorizontalTPostHeight",
    horizontaltpost: "HorizontalTPost",
    tiltrodtype: "TiltrodType",
    tiltrodsplit: "TiltrodSplit",
    splitheight1: "SplitHeight1",
    splitheight2: "SplitHeight2",
    reversehinged: "ReverseHinged",
    pelmetflat: "PelmetFlat",
    extrafascia: "ExtraFascia",
    hingesloose: "HingesLoose",
    cutout: "DoorCutOut",
    specialshape: "SpecialShape",
    templateprovided: "TemplateProvided",
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

// GET HEADER DATA
function bindDataHeader(headerId) {
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
        if (!data || data.length === 0) {
          var msg =
            roleName === "Administrator"
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
          roleName === "Administrator"
            ? xhr.status + "\n" + xhr.responseText + "\n" + thrownError
            : "Please contact our IT team at support@onlineorder.au";
        reject(isError(msg));
      },
    });
  });
}

// GET FORM ACTION
function getFormAction(itemAction) {
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

// GET DESIGN TYPE
function bindDesignType(designId) {
  return new Promise((resolve, reject) => {
    if (!designId) return resolve();

    $.ajax({
      type: "POST",
      url: uriMethod + "/GetDesignType",
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
              ? "No data returned from server : bindDataHeader"
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

// BIND DATA BLIND TYPE
function bindBlindType(designId) {
  return new Promise((resolve, reject) => {
    const blindtype = document.getElementById("blindtype");
    blindtype.innerHTML = ""; //reset

    if (!designId) return resolve();

    bindColourType(designId, blindtype.value);

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
            bindColourType(designId, blindtype.value); //
            bindMounting(
              blindtype.selectedOptions[0].getAttribute("data-name")
            );
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
function bindColourType(designId, blindId) {
  return new Promise((resolve, reject) => {
    const colourtype = document.getElementById("colourtype");
    colourtype.innerHTML = ""; //reset

    if (!blindId) return resolve();

    const sel = document.getElementById("blindtype");
    const blindName = sel.selectedOptions[0].getAttribute("data-name");

    visibleElementForm(blindName, colourtype.value);

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
      case "Hinged":
      case "Hinged Bi-fold":
      case "Track Bi-fold":
      case "Track Sliding":
      case "Track Sliding Single Track":
      case "Fixed":
      case "Panel Only":
        data = [
          { value: "", label: "" },
          { value: "Inside", label: "Inside" },
          { value: "Out side", label: "Out side" },
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

// BIND LOUVRE SIZE
function bindLouvreSize() {
  return new Promise((resolve) => {
    const select = document.getElementById("louvresize");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const data = [
      { value: "", label: "" },
      { value: "63", label: "63mm" },
      { value: "89", label: "89mm" },
      { value: "114", label: "114mm" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND LOUVRE POSITION
function bindLouvrePosition(louvresize) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("louvreposition");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    if (!louvresize) return resolve();

    let data = [];
    if (louvresize === "114") {
      data = [
        { value: "", label: "" },
        { value: "Closed", label: "Closed" },
      ];
    } else {
      data = [
        { value: "", label: "" },
        { value: "Open", label: "Open" },
        { value: "Closed", label: "Closed" },
      ];
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

// BIND MIDRAIL CRITICAL
function bindMidrailCritical(height1, height2) {
  return new Promise((resolve) => {
    const select = document.getElementById("midrailcritical");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    visibleMidrail(height1);

    let data = [];
    if (height1 > 0 && height2 > 0) {
      data = [
        { value: "", text: "" },
        { value: "Yes - Top Only", text: "YES - TOP ONLY" },
        { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
      ];
    } else if (height1 > 0) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
      ];
    } else if (height2 > 0) {
      data = [
        { value: "", text: "" },
        { value: "Yes - Top Only", text: "YES - TOP ONLY" },
        { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
      ];
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND PANEL QTY
function bindPanelQty() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("panelqty");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const data = [{ value: "", label: "" }];
    for (let i = 1; i <= 20; i++) {
      data.push({ value: i, label: i });
    }

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label;
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND HINGE COLOUR
function bindHingeColour() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("hingecolour");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const data = [
      { value: "", label: "" },
      { value: "Default", label: "Default" },
      { value: "White", label: "White" },
      { value: "Off White", label: "Off White" },
      { value: "Stainless Steel", label: "Stainless Steel" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND LAYOUT CODE
function bindLayoutCode(blindName) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("layoutcode");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    // console.log("bindLayoutCode :" + blindName);
    if (!blindName) return resolve();

    let data = [{ value: "", label: "" }];
    switch (blindName) {
      case "Hinged":
        data = [
          { value: "", text: "" },
          { value: "L", text: "L" },
          { value: "R", text: "R" },
          { value: "LR", text: "LR" },
          { value: "LD-R", text: "LD-R" },
          { value: "L-DR", text: "L-DR" },
          { value: "LTLR", text: "LTLR" },
          { value: "LRTR", text: "LRTR" },
          { value: "LRTLR", text: "LRTLR" },
          { value: "LTLRTR", text: "LTLRTR" },
          { value: "LD-RTLD-R", text: "LD-RTLD-R" },
          { value: "L-DRTL-DR", text: "L-DRTL-DR" },
          { value: "Other", text: "OTHER" },
        ];
        break;
      case "Hinged Bi-fold":
        data = [
          { value: "", text: "" },
          { value: "LL", text: "LL" },
          { value: "RR", text: "RR" },
          { value: "LLRR", text: "LLRR" },
          { value: "Other", text: "OTHER" },
        ];
        break;
      case "Track Bi-fold":
        data = [
          { value: "", text: "" },
          { value: "LL", text: "LL" },
          { value: "RR", text: "RR" },
          { value: "LLRR", text: "LLRR" },
          { value: "LLLL", text: "LLLL" },
          { value: "RRRR", text: "RRRR" },
          { value: "LLRRRR", text: "LLRRRR" },
          { value: "LLLLRR", text: "LLLLRR" },
          { value: "LLLLLL", text: "LLLLLL" },
          { value: "RRRRRR", text: "RRRRRR" },
          { value: "LLRRRRRR", text: "LLRRRRRR" },
          { value: "LLLLRRRR", text: "LLLLRRRR" },
          { value: "LLLLLLRR", text: "LLLLLLRR" },
          { value: "LLLLLLLL", text: "LLLLLLLL" },
          { value: "RRRRRRRR", text: "RRRRRRRR" },
          { value: "Other", text: "OTHER" },
        ];
        break;
      case "Track Sliding":
        data = [
          { value: "", text: "" },
          { value: "BF", text: "BF" },
          { value: "FB", text: "FB" },
          { value: "BFB", text: "BFB" },
          { value: "FBF", text: "FBF" },
          { value: "BFFB", text: "BFFB" },
          { value: "FBBF", text: "FBBF" },
          { value: "BBFF", text: "BBFF" },
          { value: "FFBB", text: "FFBB" },
          { value: "Other", text: "OTHER" },
        ];
        break;
      case "Track Sliding Single Track":
        data = [
          { value: "", text: "" },
          { value: "F", text: "F" },
          { value: "FF", text: "FF" },
          { value: "FFF", text: "FFF" },
          { value: "FFFF", text: "FFFF" },
          { value: "Other", text: "OTHER" },
        ];
        break;
      case "Fixed":
        data = [
          { value: "", text: "" },
          { value: "F", text: "F" },
          { value: "FF", text: "FF" },
          { value: "FFF", text: "FFF" },
          { value: "FFFF", text: "FFFF" },
          { value: "FFFFFF", text: "FFFFFF" },
          { value: "Other", text: "OTHER" },
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

//BIND FRAME TYPE
function bindFrameType(blindName, mounting, louvreSize, louvrePosition) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("frametype");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const buildout = document.getElementById("buildout");

    if (!blindName) return resolve();

    visibleFrameDetail(select.value);
    visibleBuildout(blindName, select.value);
    visibleBuildoutPosition(blindName, select.value, buildout.value);

    let data = [];
    if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
      data = [
        { value: "", text: "" },
        { value: "Beaded L 48mm", text: "BEADED L 48MM" },
        { value: "Insert L 50mm", text: "INSERT L 50MM" },
        { value: "Insert L 63mm", text: "INSERT L 63MM" },
        { value: "Flat L 48mm", text: "FLAT L 48MM" },
      ];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Beaded L 48mm", text: "BEADED L 48MM" },
          { value: "Insert L 50mm", text: "INSERT L 50MM" },
          { value: "Insert L 63mm", text: "INSERT L 63MM" },
          { value: "Flat L 48mm", text: "FLAT L 48MM" },
          { value: "Small Bullnose Z Frame", text: "SMALL BULLNOSE Z FRAME" },
          { value: "Large Bullnose Z Frame", text: "LARGE BULLNOSE Z FRAME" },
          { value: "Colonial Z Frame", text: "COLONIAL Z FRAME" },
          { value: "No Frame", text: "NO FRAME" },
        ];
      }
    } else if (blindName === "Track Bi-fold") {
      data = [
        { value: "", text: "" },
        { value: "100mm", text: "100MM" },
        { value: "160mm", text: "160MM" },
      ];
    } else if (blindName === "Track Sliding") {
      data = [
        { value: "", text: "" },
        { value: "100mm", text: "100MM" },
        { value: "160mm", text: "160MM" },
        { value: "200mm", text: "200MM" },
      ];
      if (louvrePosition === "Open") {
        data = [
          { value: "", text: "" },
          { value: "160mm", text: "160MM" },
          { value: "200mm", text: "200MM" },
        ];
      }
      if (
        louvrePosition === "Open" &&
        (louvreSize === "89" || louvreSize === "114")
      ) {
        data = [
          { value: "", text: "" },
          { value: "100mm", text: "100MM" },
          { value: "200mm", text: "200MM" },
        ];
      }
    } else if (blindName === "Track Sliding Single Track") {
      data = [{ value: "100mm", text: "100MM" }];
    } else if (blindName === "Fixed") {
      data = [
        { value: "", text: "" },
        { value: "U Channel", text: "U CHANNEL" },
        { value: "19x19 Light Block", text: "19X19 LIGHT BLOCK" },
      ];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text;
      select.appendChild(option);
    });

    if (select.options.length === 1) {
      bindFrameLeft(select.value, mounting);
      bindFrameRight(select.value, mounting);
      bindFrameTop(select.value, mounting);
      bindFrameBottom(select.value, mounting);

      visibleFrameDetail(select.value);
      visibleBuildout(blindName, select.value);
      visibleBuildoutPosition(blindName, select.value, buildout.value);
    }

    resolve();
  });
}

// BIND FRAME LEFT
function bindFrameLeft(frameType, mounting) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("frameleft");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    const divFrameLeft = document.getElementById("divFrameLeft");
    divFrameLeft.setAttribute("hidden", true); // visible false

    if (!frameType) return resolve();

    divFrameLeft.removeAttribute("hidden"); // visible true
    let data = [];
    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
      ];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Yes", text: "YES" },
          { value: "No", text: "NO" },
          { value: "Light Block", text: "LIGHT BLOCK" },
          { value: "L Striker Plate", text: "L STRIKER PLATE" },
        ];
      }
    } else if (
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
    } else if (frameType === "No Frame") {
      data = [{ value: "Light Block", text: "LIGHT BLOCK" }];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      data = [
        { value: "", text: "" },
        { value: "No", text: "NO" },
        { value: "L Strip", text: "L STRIP" },
      ];
    } else if (frameType === "19x19 Light Block") {
      data = [{ value: "No", text: "NO" }];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    resolve();
  });
}

// BIND FRAME RIGHT
function bindFrameRight(frameType, mounting) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("frameright");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    document.getElementById("divFrameRight").setAttribute("hidden", true); // visible false

    if (!frameType) return resolve();

    document.getElementById("divFrameRight").removeAttribute("hidden"); // visible true
    let data = [];
    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
      ];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Yes", text: "YES" },
          { value: "No", text: "NO" },
          { value: "Light Block", text: "LIGHT BLOCK" },
          { value: "L Striker Plate", text: "L STRIKER PLATE" },
        ];
      }
    } else if (
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
    } else if (frameType === "No Frame") {
      data = [{ value: "Light Block", text: "LIGHT BLOCK" }];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      data = [
        { value: "", text: "" },
        { value: "No", text: "NO" },
        { value: "L Strip", text: "L STRIP" },
      ];
    } else if (frameType === "19x19 Light Block") {
      data = [{ value: "No", text: "NO" }];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });
    resolve();
  });
}

// BIND FRAME TOP
function bindFrameTop(frameType, mounting) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("frametop");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    document.getElementById("divFrameTop").setAttribute("hidden", true); // visible false

    if (!frameType) return resolve();

    document.getElementById("divFrameTop").removeAttribute("hidden"); // visible true
    let data = [];
    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm" ||
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Yes", text: "YES" },
          { value: "No", text: "NO" },
          { value: "Light Block", text: "LIGHT BLOCK" },
          { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
          {
            value: "Small Bullnose Z Sill Plate",
            text: "SMALL BULLNOSE Z SILL PLATE",
          },
          {
            value: "Large Bullnose Z Sill Plate",
            text: "LARGE BULLNOSE Z SILL PLATE",
          },
          { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
          { value: "L Striker Plate", text: "L STRIKER PLATE" },
        ];
      }
    } else if (frameType === "No Frame") {
      data = [
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      data = [{ value: "Yes", text: "YES" }];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Yes", text: "YES" },
          { value: "No", text: "NO" },
        ];
      }
    } else if (frameType === "U Channel") {
      data = [{ value: "Yes", text: "YES" }];
    } else if (frameType === "19x19 Light Block") {
      data = [{ value: "No", text: "NO" }];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });
    resolve();
  });
}

// BIND FRAME BOTTOM
function bindFrameBottom(frameType, mounting) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("framebottom");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    document.getElementById("divFrameBottom").setAttribute("hidden", true); // visible false

    if (!frameType) return resolve();

    document.getElementById("divFrameBottom").removeAttribute("hidden"); // visible true
    let data = [];
    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
      if (mounting === "Inside") {
        data = [
          { value: "", text: "" },
          { value: "Yes", text: "YES" },
          { value: "No", text: "NO" },
          { value: "Light Block", text: "LIGHT BLOCK" },
          { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
          {
            value: "Small Bullnose Z Sill Plate",
            text: "SMALL BULLNOSE Z SILL PLATE",
          },
          {
            value: "Large Bullnose Z Sill Plate",
            text: "LARGE BULLNOSE Z SILL PLATE",
          },
          { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
          { value: "L Striker Plate", text: "L STRIKER PLATE" },
        ];
      }
    } else if (
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
    } else if (frameType === "No Frame") {
      data = [
        { value: "", text: "" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      data = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      data = [{ value: "Yes", text: "YES" }];
    } else if (frameType === "19x19 Light Block") {
      data = [{ value: "No", text: "NO" }];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });
    resolve();
  });
}

// BIND BOTTOM TRACK TYPE
function bindBottomTrackType(blindName, bottomFrame) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("bottomtracktype");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    document.getElementById("divBottomTrackType").setAttribute("hidden", true); // visible false

    if (!blindName) return resolve();

    visibleBottomTrackReccess(select.value);

    let data = [];
    if (
      blindName === "Track Bi-fold" ||
      blindName === "Track Sliding" ||
      blindName === "Track Sliding Single Track"
    ) {
      document.getElementById("divBottomTrackType").removeAttribute("hidden"); // visible true
      data = [
        { value: "", text: "" },
        { value: "M Track", text: "M TRACK" },
        { value: "U Track", text: "U TRACK" },
      ];
      if (bottomFrame === "Yes") {
        data = [{ value: "U Track", text: "U TRACK" }];
      }
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });
    if (select.options.length === 0) {
      select.selectedIndex = 0;
      visibleBottomTrackReccess(select.value);
    }
    resolve();
  });
}

//  BIND BUILDOUT
function bindBuildout(blindName, frameType) {
  return new Promise((resolve) => {
    const select = document.getElementById("buildout");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const divBuildout = document.getElementById("divBuildout");
    divBuildout.setAttribute("hidden", true); // visible false

    if (!blindName) return resolve();

    if (
      frameType &&
      (blindName === "Hinged" || blindName === "Hinged Bi-fold")
    ) {
      divBuildout.removeAttribute("hidden"); // visible true
      const data = [
        { value: "", label: "" },
        { value: "9.5mm Buildout", label: "9.5mm Buildout" },
        { value: "25mm Buildout", label: "25mm Buildout" },
      ];

      data.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.label.toUpperCase();
        select.appendChild(option);
      });
    }
    resolve();
  });
}

// BIND BUILDOUT POSITION
function bindBuildoutPosition(blindName, buildout, frameType) {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("buildoutposition");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    document.getElementById("divBuildoutPosition").setAttribute("hidden", true); // visible false
    if (!blindName || !frameType || !buildout) return resolve();

    if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
      const show = () => {
        document
          .getElementById("divBuildoutPosition")
          .removeAttribute("hidden"); // visible true
      };
      let data = [];
      switch (frameType) {
        case "Small Bullnose Z Frame":
          data = [
            { value: "", label: "" },
            { value: "Back of Frame", label: "Back of Frame" },
            { value: "Back of Lip", label: "Back of Lip" },
          ];
          show();
          break;
        case "Large Bullnose Z Frame":
          data = [
            { value: "", label: "" },
            { value: "Back of Frame", label: "Back of Frame" },
            { value: "Back of Lip", label: "Back of Lip" },
          ];
          show();
          break;
        case "Colonial Z Frame":
          data = [
            { value: "", label: "" },
            { value: "Back of Frame", label: "Back of Frame" },
            { value: "Back of Lip", label: "Back of Lip" },
          ];
          show();
          break;
        case "Regal Z Frame":
          data = [
            { value: "", label: "" },
            { value: "Back of Frame", label: "Back of Frame" },
            { value: "Back of Lip", label: "Back of Lip" },
          ];
          show();
          break;
      }
      data.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.label.toUpperCase();
        select.appendChild(option);
      });

      resolve();
    }
  });
}

// BIND HORIZONTAL TPOST REQUIRED
function bindHorizontalTpost() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("horizontaltpost");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const data = [
      { value: "", label: "" },
      { value: "Yes", label: "Yes" },
      { value: "No Post", label: "No Post" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label.toUpperCase();
      select.appendChild(option);
    });
    resolve();
  });
}

// BIND TILTROD TYPE
function bindTiltrodType() {
  return new Promise((resolve, reject) => {
    const select = document.getElementById("tiltrodtype");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset
    const data = [
      { value: "", label: "" },
      { value: "Easy Tilt", label: "Easy Tilt" },
      { value: "Clearview", label: "Clearview" },
    ];

    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.label.toUpperCase();
      select.appendChild(option);
    });
    resolve();
  });
}

// BIND TILTROD ROTATION
function bindTiltrodSplit(midrailheight1) {
  return new Promise((resolve) => {
    const select = document.getElementById("tiltrodsplit");
    select.innerHTML = ""; // kosongkan dulu jika ingin reset

    visibleSplitHeight(select.value); //tilrodsplit, tiltrodheight1, tiltrodheight2
    let data = [{ value: "debug", text: "debug" }];
    if (parseFloat(midrailheight1) === 0) {
      data = [
        { value: "", text: "" },
        { value: "None", text: "NONE" },
        { value: "Split Halfway", text: "SPLIT HALFWAY" },

        { value: "Other", text: "OTHER" },
      ];
    } else if (parseFloat(midrailheight1) > 0) {
      data = [
        { value: "", text: "" },
        { value: "None", text: "NONE" },
        {
          value: "Split Halfway Above Midrail",
          text: "SPLIT HALFWAY ABOVE MIDRAIL",
        },
        {
          value: "Split Halfway Below Midrail",
          text: "SPLIT HALFWAY BELOW MIDRAIL",
        },
        {
          value: "Split Halfway Above and Below Midrail",
          text: "SPLIT HALFWAY  ABOVE & BELOW MIDRAIL",
        },
        { value: "Other", text: "OTHER" },
      ];
    }
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      select.appendChild(option);
    });

    if (select.options.length === 0) {
      select.selectedIndex = 0;
      visibleSplitHeight(select.value);
    }

    resolve();
  });
}

// #.........................|| Visible Function ||...........................#

// VISIBLE SEMI INSIDE MOUNT
function visibleSemiInsideMount(blindName, mounting) {
  return new Promise((resolve, reject) => {
    const element = document.getElementById("semiinsidemount");
    document.getElementById("divSemiInsideMount").setAttribute("hidden", true); // visible false

    if (!blindName || !mounting) return resolve();

    if (
      mounting === "Inside" &&
      (blindName === "Track Bi-fold" ||
        blindName === "Track Sliding" ||
        blindName === "Track Sliding Single Track")
    ) {
      document.getElementById("divSemiInsideMount").removeAttribute("hidden"); // visible true
    }
    resolve();
  });
}

// VISIBLE SPLIT HEIGHT
function visibleSplitHeight(tiltrodsplit) {
  return new Promise((resolve) => {
    const divElement = document.getElementById("divTiltrodHeight");
    divElement.setAttribute("hidden", true); // visible false
    if (tiltrodsplit === "Other") divElement.removeAttribute("hidden");

    resolve();
  });
}

// VISIBLE MIDRAIL CRITICAL
function visibleMidrail(height1) {
  return new Promise((resolve) => {
    const divMidrailHeight2 = document.getElementById("divMidrailHeight2");
    const divMidrailCritical = document.getElementById("divMidrailCritical");

    divMidrailHeight2.setAttribute("hidden", true); // visible false
    divMidrailCritical.setAttribute("hidden", true); // visible false
    if (height1 > 0) {
      divMidrailHeight2.removeAttribute("hidden"); // visible true
      divMidrailCritical.removeAttribute("hidden"); // visible true
    }
    resolve();
  });
}

// VISIBLE HINGE COLOUR
function visibleHingeColour(blindName, joinedPanels) {
  return new Promise((resolve) => {
    const divHingeColour = document.getElementById("divHingeColour");
    divHingeColour.setAttribute("hidden", true); // visible false

    if (!blindName) return resolve();
    if (
      blindName === "Hinged" ||
      blindName === "Hinged Bi-fold" ||
      blindName === "Track Bi-fold"
    ) {
      divHingeColour.removeAttribute("hidden");
    } else if (
      blindName === "Track Sliding" ||
      blindName === "Track Sliding Single Track"
    ) {
      if (joinedPanels === "Yes") divHingeColour.removeAttribute("hidden");
    }
    resolve();
  });
}

// VISIBLE HINGES LOOSE
function visibleHingesLoose(blindName, hingecolour, joinedPanels) {
  return new Promise((resolve) => {
    const divHingesLoose = document.getElementById("divHingesLoose");
    divHingesLoose.setAttribute("hidden", true); // visible false
    if (!hingecolour) return resolve();
    if (
      blindName === "Hinged" ||
      blindName === "Hinged Bi-fold" ||
      blindName === "Track Bi-fold"
    ) {
      divHingesLoose.removeAttribute("hidden");
    } else if (blindName === "Track Sliding" && joinedPanels === "Yes") {
      divHingesLoose.removeAttribute("hidden");
    }
    resolve();
  });
}

// VISIBLE LAYOUT CUSTOM
function visibleLayoutCustom(layoutcode) {
  return new Promise((resolve) => {
    const divLayoutCodeCustom = document.getElementById("divLayoutCodeCustom");
    divLayoutCodeCustom.setAttribute("hidden", true); // visible false
    if (layoutcode === "Other") divLayoutCodeCustom.removeAttribute("hidden");
    resolve();
  });
}

// VISIBLE SAME SIZE PANEL
function visibleSameSizePanel(blindName, layoutCode) {
  return new Promise((resolve) => {
    const divSameSize = document.getElementById("divSameSize");
    divSameSize.setAttribute("hidden", true); // visible false

    if (!blindName && !layoutCode) return resolve();
    if (
      (blindName === "Hinged" || blindName === "Hinged Bi-fold") &&
      cekSameSizePanels(layoutCode)
    ) {
      divSameSize.removeAttribute("hidden");
    }
    resolve();
  });
}

function cekSameSizePanels(layoutCode) {
  if (layoutCode.length === 0) return false;
  if (layoutCode.includes("T")) {
    if (
      layoutCode.includes("B") ||
      layoutCode.includes("C") ||
      layoutCode.includes("G")
    ) {
      return false;
    }
  }
  if (layoutCode.includes("B")) {
    if (
      layoutCode.includes("T") ||
      layoutCode.includes("C") ||
      layoutCode.includes("G")
    ) {
      return false;
    }
  }
  if (layoutCode.includes("C")) {
    if (
      layoutCode.includes("T") ||
      layoutCode.includes("B") ||
      layoutCode.includes("G")
    ) {
      return false;
    }
  }
  if (layoutCode.includes("G")) {
    if (
      layoutCode.includes("T") ||
      layoutCode.includes("B") ||
      layoutCode.includes("C")
    ) {
      return false;
    }
  }
  return (
    layoutCode.includes("T") ||
    layoutCode.includes("B") ||
    layoutCode.includes("C") ||
    layoutCode.includes("G")
  );
}

// VISIBLE GAP
function visibleGap(blindName, sameSize, layoutCode) {
  return new Promise((resolve) => {
    const divGapPost = document.getElementById("divGapPost");
    divGapPost.setAttribute("hidden", true); // visible false

    const divGap1 = document.getElementById("divGap1");
    const divGap2 = document.getElementById("divGap2");
    const divGap3 = document.getElementById("divGap3");
    const divGap4 = document.getElementById("divGap4");
    const divGap5 = document.getElementById("divGap5");

    divGap1.setAttribute("hidden", true); // visible false
    divGap2.setAttribute("hidden", true); // visible false
    divGap3.setAttribute("hidden", true); // visible false
    divGap4.setAttribute("hidden", true); // visible false
    divGap5.setAttribute("hidden", true); // visible false

    if (!blindName) return resolve();

    if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
      if (cekSameSizePanels(layoutCode)) {
        if (sameSize === "Yes") {
          divGapPost.setAttribute("hidden", true);
        } else {
          let countT = 0;
          for (let char of layoutCode) {
            if (char === "T" || char === "B" || char === "C" || char === "G")
              countT++;
          }
          if (countT > 0) divGapPost.removeAttribute("hidden"); // visible true
          if (countT > 0) divGap1.removeAttribute("hidden"); // visible true
          if (countT > 1) divGap2.removeAttribute("hidden"); // visible true
          if (countT > 2) divGap3.removeAttribute("hidden"); // visible true
          if (countT > 3) divGap4.removeAttribute("hidden"); // visible true
          if (countT > 4) divGap5.removeAttribute("hidden"); // visible true
        }
      } else {
        let countT = 0;
        for (let char of layoutCode) {
          if (char === "T" || char === "B" || char === "C" || char === "G")
            countT++;
        }
        if (countT > 0) divGapPost.removeAttribute("hidden"); // visible true
        if (countT > 0) divGap1.removeAttribute("hidden"); // visible true
        if (countT > 1) divGap2.removeAttribute("hidden"); // visible true
        if (countT > 2) divGap3.removeAttribute("hidden"); // visible true
        if (countT > 3) divGap4.removeAttribute("hidden"); // visible true
        if (countT > 4) divGap5.removeAttribute("hidden"); // visible true
      }
    } else if (blindName === "Track Bi-fold") {
      let countT = 0;
      for (let char of layoutCode) {
        if (char === "T" || char === "B" || char === "C" || char === "G")
          countT++;
      }
      if (countT > 0) divGapPost.removeAttribute("hidden"); // visible true
      if (countT > 0) divGap1.removeAttribute("hidden"); // visible true
      if (countT > 1) divGap2.removeAttribute("hidden"); // visible true
      if (countT > 2) divGap3.removeAttribute("hidden"); // visible true
      if (countT > 3) divGap4.removeAttribute("hidden"); // visible true
      if (countT > 4) divGap5.removeAttribute("hidden"); // visible true
    }
    resolve();
  });
}

// VISIBLE FRAME DETAIL
function visibleFrameDetail(frameType) {
  return new Promise((resolve) => {
    const frameElements = [
      document.getElementById("divFrameLeft"),
      document.getElementById("divFrameRight"),
      document.getElementById("divFrameTop"),
      document.getElementById("divFrameBottom"),
    ];

    frameElements.forEach((element) => {
      if (frameType) {
        element.removeAttribute("hidden");
      } else {
        element.setAttribute("hidden", true);
      }
    });

    resolve();
  });
}

function visibleBuildout(blindName, frameType) {
  return new Promise((resolve) => {
    const divBuildout = document.getElementById("divBuildout");
    divBuildout.setAttribute("hidden", true);

    if (!blindName || !frameType) return resolve();

    if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
      divBuildout.removeAttribute("hidden");
    }
    resolve();
  });
}

function visibleBuildoutPosition(blindName, frameType, buildout) {
  return new Promise((resolve) => {
    const divBuildoutPosition = document.getElementById("divBuildoutPosition");
    divBuildoutPosition.setAttribute("hidden", true);

    if (!blindName || !frameType) return resolve();

    if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
      if (
        (frameType === "Small Bullnose Z Frame" ||
          frameType === "Large Bullnose Z Frame" ||
          frameType === "Colonial Z Frame") &&
        buildout !== ""
      ) {
        divBuildoutPosition.removeAttribute("hidden");
      }
    }
    resolve();
  });
}

// VISIBLE BOTTOM TRACK RECESS
function visibleBottomTrackReccess(bottomTrack) {
  return new Promise((resolve) => {
    const divBottomTrackRecess = document.getElementById(
      "divBottomTrackRecess"
    );
    divBottomTrackRecess.setAttribute("hidden", true); // visible false

    const bottomtrackrecess = document.getElementById("bottomtrackrecess");
    if (!bottomTrack) return resolve();

    if (bottomTrack === "M Track")
      divBottomTrackRecess.removeAttribute("hidden");
    if (bottomTrack === "U Track") bottomtrackrecess.value = "Yes";

    resolve();
  });
}

// VISIBLE TEMPLATE PROVIDED
function visibleTemplateProvided(specialshape) {
  return new Promise((resolve) => {
    const divTemplateProvided = document.getElementById("divTemplateProvided");
    divTemplateProvided.setAttribute("hidden", true); // visible false
    if (!specialshape) return resolve();

    if (specialshape === "Yes") divTemplateProvided.removeAttribute("hidden");
    resolve();
  });
}

// .........................|| Other Function ||...........................#
// CHECK SESSION
function checkSession() {
  if (!headerId) {
    window.location.href = "/order";
    return;
  }
  if (!itemAction || !designId) {
    window.location.href = "/order/detail";
    return;
  }
  if (designId.toUpperCase() !== designIdShutters) {
    window.location.href = "/order/detail";
    return;
  }

  loader(itemAction);

  setSessionAlive();

  bindDataHeader(headerId);
  bindDesignType(designId);
  getFormAction(itemAction);

  if (itemAction === "AddItem") {
    controlForm(false);
    visibleElementForm();
    bindBlindType(designId); //designId, blindId
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
    controlForm(
      itemAction === "ViewItem",
      itemAction === "EditItem",
      itemAction === "CopyItem"
    );
    bindItemOrder(itemId);
  }
}

// CONTROL FORM
function controlForm(status, isEditItem, isCopyItem) {
  if (isEditItem === undefined) {
    isEditItem = false;
  }
  if (isCopyItem === undefined) {
    isCopyItem = false;
  }

  const submit = document.getElementById("btnSubmit");
  if (status === true) {
    submit.setAttribute("hidden", true);
  } else {
    submit.removeAttribute("hidden");
  }

  const inputs = [
    "blindtype",
    "colourtype",
    "qty",
    "room",
    "mounting",
    "width",
    "drop",
    "louvresize",
    "louvreposition",
    "midrailheight1",
    "midrailheight2",
    "midrailcritical",
    "panelqty",
    "joinedpanels",
    "hingecolour",
    "semiinsidemount",
    "customheaderlength",
    "layoutcode",
    "layoutcodecustom",
    "frametype",
    "frameleft",
    "frameright",
    "frametop",
    "framebottom",
    "bottomtracktype",
    "bottomtrackrecess",
    "buildout",
    "buildoutposition",
    "samesizepanel",
    "gap1",
    "gap2",
    "gap3",
    "gap4",
    "gap5",
    "horizontaltpostheight",
    "horizontaltpost",
    "tiltrodtype",
    "tiltrodsplit",
    "splitheight1",
    "splitheight2",
    "reversehinged",
    "pelmetflat",
    "extrafascia",
    "hingesloose",
    "cutout",
    "specialshape",
    "templateprovided",
    "notes",
    "markup",
  ];

  inputs.forEach((id) => {
    const inputElement = document.getElementById(id);
    if (inputElement) {
      if (isCopyItem) {
        inputElement.disabled = id === "blindtype";
      } else if (isEditItem && (id === "qty" || id === "blindtype")) {
        inputElement.disabled = true;
      } else {
        inputElement.disabled = status;
      }
    }
  });
}

// LOADER
function loader(action) {
  const overlay = document.getElementById("loading-overlay");

  if (action === "AddItem") {
    overlay.classList.add("fade-out"); // mulai fade-out
    setTimeout(() => {
      overlay.classList.add("d-none");
      overlay.classList.remove("d-flex", "fade-out");
    }, 500); // waktu harus sama dengan di CSS
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

// SET SESSION ALIVE
function setSessionAlive() {
  setInterval(() => {
    fetch("/Account/KeepSessionAlive.aspx", {
      method: "POST",
      credentials: "include",
    }).then((res) => {
      if (res.status === 401) {
        // session expired, redirect to login?
        window.location.href = "/Account/Login.aspx";
      }
      console.log(res.status);
    });
  }, 180000); // 3 menit
}

// SHOW INFO INPUT
function showInfo(params) {
  var title;
  var info;
  if (params == "gap") {
    title = "T-Post / Gap / Bay / Corner Location Information";
    info =
      "The factory will make all panels within an opening the same width unless otherwise indicated. If specific T-post locations are required, enter the measurement from the far left-hand side to the centre of the T-post measurement. The factory will make the panels to fit in between these posts.";
  } else if (params == "tiltrodtype") {
    title = "Tiltrod Type Information";
    info =
      "<b>Easy Tilt</b>: Internal rack and pinion.<br /><b>Clearview</b>: Metal rod on back edge of louvres.";
  } else if (params == "frametype") {
    title = "Frame Type Information";
    info =
      "Beaded L 48mm and Flat L 48mm have been developed for use in an inside mount opening.<br>Insert L 50mm and Insert L 63mm have been developed for use in an outside mount opening.";
  }

  $("#modalTitle").html(title);
  $("#spanInfo").html(info);
  $("#modalInfo").modal("show");
}

// RESET FORM
function visibleElementForm(blindName, colourtype) {
  //SET DEFAULT TEXT BUTTON SUBMIT
  const btnSubmit = document.getElementById("btnSubmit");

  // HIDE UNHIDE ELEMENTS
  const divFormDetail = document.getElementById("divFormDetail");
  divFormDetail.setAttribute("hidden", true);

  const divLouvrePosition = document.getElementById("divLouvrePosition");
  const divMidrailHeight2 = document.getElementById("divMidrailHeight2");
  const divMidrailCritical = document.getElementById("divMidrailCritical");
  const divPanelQty = document.getElementById("divPanelQty");
  const divJoinedPanels = document.getElementById("divJoinedPanels");
  const divHingeColour = document.getElementById("divHingeColour");
  const divSemiInsideMount = document.getElementById("divSemiInsideMount");
  const divCustomHeaderLength = document.getElementById(
    "divCustomHeaderLength"
  );
  const divLayoutCode = document.getElementById("divLayoutCode");
  const divLayoutCodeCustom = document.getElementById("divLayoutCodeCustom");
  const divFrameType = document.getElementById("divFrameType");
  const divFrameLeft = document.getElementById("divFrameLeft");
  const divFrameRight = document.getElementById("divFrameRight");
  const divFrameTop = document.getElementById("divFrameTop");
  const divFrameBottom = document.getElementById("divFrameBottom");
  const divBottomTrackType = document.getElementById("divBottomTrackType");
  const divBottomTrackRecess = document.getElementById("divBottomTrackRecess");
  const divBuildout = document.getElementById("divBuildout");
  const divBuildoutPosition = document.getElementById("divBuildoutPosition");
  const divSameSize = document.getElementById("divSameSize");
  const divGapPost = document.getElementById("divGapPost");
  // const divGap1 = document.getElementById("divGap1");
  // const divGap2 = document.getElementById("divGap2");
  // const divGap3 = document.getElementById("divGap3");
  // const divGap4 = document.getElementById("divGap4");
  // const divGap5 = document.getElementById("divGap5");
  const divHorizontalTPost = document.getElementById("divHorizontalTPost");
  const divHorizontalTPostRequired = document.getElementById(
    "divHorizontalTPostRequired"
  );
  const divTiltrodType = document.getElementById("divTiltrodType");
  const divTiltrodSplit = document.getElementById("divTiltrodSplit");
  const divTiltrodHeight = document.getElementById("divTiltrodHeight");
  const divReverseHinged = document.getElementById("divReverseHinged");
  const divPelmetFlat = document.getElementById("divPelmetFlat");
  const divExtraFascia = document.getElementById("divExtraFascia");
  const divHingesLoose = document.getElementById("divHingesLoose");
  const divCutOut = document.getElementById("divCutOut");
  const divSpecialShape = document.getElementById("divSpecialShape");
  const divTemplateProvided = document.getElementById("divTemplateProvided");
  const divMarkUp = document.getElementById("divMarkUp");

  // if (!colourtype) console.log("no colourtype");

  if (colourtype) {
    divFormDetail.removeAttribute("hidden");

    divLouvrePosition.setAttribute("hidden", true);
    divMidrailHeight2.setAttribute("hidden", true);
    divMidrailCritical.setAttribute("hidden", true);
    divPanelQty.setAttribute("hidden", true);
    divJoinedPanels.setAttribute("hidden", true);
    divHingeColour.setAttribute("hidden", true);
    divSemiInsideMount.setAttribute("hidden", true);
    divCustomHeaderLength.setAttribute("hidden", true);
    divLayoutCode.setAttribute("hidden", true);
    divLayoutCodeCustom.setAttribute("hidden", true);
    divFrameType.setAttribute("hidden", true);
    divFrameLeft.setAttribute("hidden", true);
    divFrameRight.setAttribute("hidden", true);
    divFrameTop.setAttribute("hidden", true);
    divFrameBottom.setAttribute("hidden", true);
    divBottomTrackType.setAttribute("hidden", true);
    divBottomTrackRecess.setAttribute("hidden", true);
    divBuildout.setAttribute("hidden", true);
    divBuildoutPosition.setAttribute("hidden", true);
    divSameSize.setAttribute("hidden", true);
    divGapPost.setAttribute("hidden", true);
    // divGap1.setAttribute("hidden", true);
    // divGap2.setAttribute("hidden", true);
    // divGap3.setAttribute("hidden", true);
    // divGap4.setAttribute("hidden", true);
    // divGap5.setAttribute("hidden", true);
    divHorizontalTPost.setAttribute("hidden", true);
    divHorizontalTPostRequired.setAttribute("hidden", true);
    divTiltrodType.setAttribute("hidden", true);
    divTiltrodSplit.setAttribute("hidden", true);
    divTiltrodHeight.setAttribute("hidden", true);
    divReverseHinged.setAttribute("hidden", true);
    divPelmetFlat.setAttribute("hidden", true);
    divExtraFascia.setAttribute("hidden", true);
    divHingesLoose.setAttribute("hidden", true);
    divCutOut.setAttribute("hidden", true);
    divSpecialShape.setAttribute("hidden", true);
    divTemplateProvided.setAttribute("hidden", true);
    divMarkUp.setAttribute("hidden", true);

    if (markupAccess === "True") divMarkUp.removeAttribute("hidden");

    // HIDE UNHIDE ELEMENTS
    if (!blindName) console.log("no blindName");

    if (blindName) {
      switch (blindName) {
        case "Panel Only":
          divPanelQty.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          break;

        case "Hinged":
        case "Hinged Bi-fold":
          divHingeColour.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          divLayoutCode.removeAttribute("hidden");
          divFrameType.removeAttribute("hidden");
          divHorizontalTPost.removeAttribute("hidden");
          break;

        case "Track Bi-fold":
          divHingeColour.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          divLayoutCode.removeAttribute("hidden");
          divFrameType.removeAttribute("hidden");
          divGapPost.removeAttribute("hidden");
          divBottomTrackType.removeAttribute("hidden");
          divReverseHinged.removeAttribute("hidden");
          divPelmetFlat.removeAttribute("hidden");
          divExtraFascia.removeAttribute("hidden");
          break;

        case "Track Sliding":
          divLouvrePosition.removeAttribute("hidden");
          divJoinedPanels.removeAttribute("hidden");
          divCustomHeaderLength.removeAttribute("hidden");
          divLayoutCode.removeAttribute("hidden");
          divFrameType.removeAttribute("hidden");
          divFrameLeft.removeAttribute("hidden");
          divFrameRight.removeAttribute("hidden");
          divFrameTop.removeAttribute("hidden");
          divFrameBottom.removeAttribute("hidden");
          divBottomTrackType.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divReverseHinged.removeAttribute("hidden");
          divPelmetFlat.removeAttribute("hidden");
          divExtraFascia.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          divTemplateProvided.removeAttribute("hidden");
          break;

        case "Track Sliding Single Track":
          divJoinedPanels.removeAttribute("hidden");
          divCustomHeaderLength.removeAttribute("hidden");
          divLayoutCode.removeAttribute("hidden");
          divFrameType.removeAttribute("hidden");
          divFrameLeft.removeAttribute("hidden");
          divFrameRight.removeAttribute("hidden");
          divFrameTop.removeAttribute("hidden");
          divFrameBottom.removeAttribute("hidden");
          divBottomTrackType.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divPelmetFlat.removeAttribute("hidden");
          divExtraFascia.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          divTemplateProvided.removeAttribute("hidden");
          break;
        case "Fixed":
          divLayoutCode.removeAttribute("hidden");
          divFrameType.removeAttribute("hidden");
          divTiltrodType.removeAttribute("hidden");
          divTiltrodSplit.removeAttribute("hidden");
          divCutOut.removeAttribute("hidden");
          divSpecialShape.removeAttribute("hidden");
          divTemplateProvided.removeAttribute("hidden");
          break;
      }
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

// RESET FORM VALUES
function resetForm() {
  const fields = [
    "room",
    "mounting",
    "width",
    "drop",
    "louvresize",
    "louvreposition",
    "midrailheight1",
    "midrailheight2",
    "midrailcritical",
    "panelqty",
    "joinedpanels",
    "hingecolour",
    "semiinsidemount",
    "customheaderlength",
    "layoutcode",
    "layoutcodecustom",
    "frametype",
    "frameleft",
    "frameright",
    "frametop",
    "framebottom",
    "bottomtracktype",
    "bottomtrackrecess",
    "buildout",
    "buildoutposition",
    "samesizepanel",
    "gap1",
    "gap2",
    "gap3",
    "gap4",
    "gap5",
    "horizontaltpostheight",
    "horizontaltpost",
    "tiltrodtype",
    "tiltrodsplit",
    "splitheight1",
    "splitheight2",
    "reversehinged",
    "pelmetflat",
    "extrafascia",
    "hingesloose",
    "cutout",
    "specialshape",
    "templateprovided",
  ];

  return fields.forEach((field) => {
    const fieldElement = document.getElementById(field);
    if (fieldElement) {
      if (fieldElement.tagName === "SELECT") {
        fieldElement.selectedIndex = 0;
      } else {
        fieldElement.value = "";
      }
    }
  });
}

// RESET FORM IS INVALID
function resetFormError() {
  document
    .querySelectorAll(".form-control, .form-select")
    .forEach((element) => {
      element.classList.remove("is-invalid");
    });
}

// RESPONSES MESSAGE
function isError(msg, field) {
  Swal.fire({
    icon: "error",
    title: "Oops...",
    html: msg,
  }).then(() => {
    setTimeout(() => {
      const fieldElement = document.getElementById(field);
      if (fieldElement && !fieldElement.classList.contains("is-invalid")) {
        fieldElement.classList.add("is-invalid");
      }
      fieldElement?.focus();
    }, 100); // beri jeda setelah SweetAlert menutup modal
  });
}
function isSuccess(msg, url) {
  Swal.fire({
    icon: "success",
    title: "Success",
    html: msg,
  }).then(() => {
    setTimeout(() => {
      window.location.href = url;
    }, 100); // beri jeda setelah SweetAlert menutup modal
  });
}
