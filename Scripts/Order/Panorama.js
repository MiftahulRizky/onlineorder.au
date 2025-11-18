$("#divOrder").hide();

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

document
  .getElementById("modalInfo")
  .addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
  });

$(document).ready(function () {
  loaderFadeOut();
  checkSession();

  $("#submit").on("click", proccess);
  $("#cancel").on("click", () => (window.location.href = "/order/detail/"));
  $("#vieworder").on("click", () => (window.location.href = "/order/detail"));

  $("#blindtype").on("change", function () {
    resetForm();

    const blindtype = $(this).val();
    const mounting = document.getElementById("mounting").value;
    const louvreposition = document.getElementById("louvreposition").value;
    const louvresize = document.getElementById("louvresize").value;
    const framebottom = document.getElementById("framebottom").value;
    const midrailheight1 =
      parseFloat(document.getElementById("midrailheight1").value) || 0;

    bindColourType(blindtype);
    bindMounting(blindtype);
    bindLayoutCode(blindtype);
    bindFrameType(blindtype, mounting, louvresize, louvreposition);
    bindBottomTrack(blindtype, framebottom);
    bindTiltrodSplit(midrailheight1);
    visibleSemiInside(blindtype, mounting);
  });

  $("#colourtype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    bindComponentForm(blindtype, $(this).val());
  });

  $("#mounting").on("change", function () {
    const mounting = $(this).val();
    const blindtype = document.getElementById("blindtype").value;
    const louvreposition = document.getElementById("louvreposition").value;
    const louvresize = document.getElementById("louvresize").value;

    bindFrameType(blindtype, mounting, louvresize, louvreposition);
    visibleSemiInside(blindtype, mounting);
  });

  $("#louvresize").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const mounting = document.getElementById("mounting").value;
    const louvresize = $(this).val();
    const louvreposition = document.getElementById("louvreposition").value;

    getBlindName(blindtype)
      .then((blindName) => {
        if (blindName === "Panel Only") {
          const dropInput = document.getElementById("drop").value;
          const drop = parseFloat(dropInput) || 0;

          if (dropInput.length < 4) return;
          if (drop.length < 4) return;

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
      })
      .catch((error) => {
        console.error(error);
      });

    bindFrameType(blindtype, mounting, louvresize, louvreposition);
  });

  $("#louvreposition").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const mounting = document.getElementById("mounting").value;
    const louvresize = document.getElementById("louvresize").value;
    const louvreposition = $(this).val();

    bindFrameType(blindtype, mounting, louvresize, louvreposition);
  });

  $("#midrailheight1").on("input", function () {
    const midrailheight1 =
      parseFloat(document.getElementById("midrailheight1").value) || 0;
    const midrailheight2 =
      parseFloat(document.getElementById("midrailheight2").value) || 0;

    bindMidrailCritical(midrailheight1, midrailheight2);
    bindTiltrodSplit(midrailheight1);
  });

  $("#midrailheight2").on("input", function () {
    const midrailheight1 = document.getElementById("midrailheight1").value || 0;
    const midrailheight2 = document.getElementById("midrailheight2").value || 0;
    bindMidrailCritical(midrailheight1, midrailheight2);
  });

  $("#joinedpanels").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const hingecolour = document.getElementById("hingecolour").value;
    visibleHingeColour(blindtype, $(this).val());
    visibleHingesLoose(blindtype, hingecolour, $(this).val());
  });

  $("#hingecolour").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const joinedPanels = document.getElementById("joinedpanels").value;
    visibleHingesLoose(blindtype, $(this).val(), joinedPanels);
  });

  $("#layoutcode").on("change", function () {
    $("#layoutcodecustom").val("");
    $("#samesizepanel").val("");
    const blindtype = document.getElementById("blindtype").value;
    const layoutcode = $(this).val();
    visibleLayoutCustom(layoutcode);
    visibleSameSize(blindtype, layoutcode);
    visibleGap(blindtype, "", layoutcode);
  });

  $("#layoutcodecustom").on("input", function () {
    $("#samesizepanel").val("");
    const blindtype = document.getElementById("blindtype").value;
    const layoutcode = $(this).val();
    visibleSameSize(blindtype, layoutcode);
    visibleGap(blindtype, "", layoutcode);
  });

  $("#samesizepanel").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const layout = document.getElementById("layoutcode").value;
    const layoutcustom = document.getElementById("layoutcodecustom").value;

    let layoutcode = layout;
    if (layout === "Other") layoutcode = layoutcustom;

    visibleGap(blindtype, $(this).val(), layoutcode);
  });

  $("#frametype").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const frametype = $(this).val();
    const mounting = document.getElementById("mounting").value;
    const buildout = document.getElementById("buildout").value;

    bindLeftFrame(frametype);
    bindRightFrame(frametype);
    bindTopFrame(frametype);
    bindBottomFrame(frametype);
    visibleFrameDetail(frametype);
    visibleBuildout(blindtype, frametype);
    visibleBuildoutPosition(blindtype, frametype, buildout);
  });

  $("#framebottom").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const framebottom = $(this).val();
    bindBottomTrack(blindtype, framebottom);
  });

  $("#buildout").on("change", function () {
    const blindtype = document.getElementById("blindtype").value;
    const frametype = document.getElementById("frametype").value;
    visibleBuildoutPosition(blindtype, frametype, $(this).val());
  });

  $("#bottomtracktype").on("change", function () {
    visibleBottomTrackReccess($(this).val());
  });

  $("#horizontaltpostheight").on("input", function () {
    const value = parseFloat($(this).val()) || 0;

    visibleHorizontalRequired(value);
  });

  $("#tiltrodsplit").on("change", function () {
    visibleSplitHeight($(this).val());
  });

  $("#specialshape").on("change", function () {
    visibleTemplateProvided($(this).val());
  });

  $("#width").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;

    getBlindName(blindtype)
      .then((blindName) => {
        if (blindName !== "Panel Only") return;
        const widthInput = $(this).val();
        const width = parseFloat(widthInput) || 0;

        if (widthInput.length < 3) return;
        if (width < 200 || width > 900) {
          isError("PANEL WIDTH MUST BE BETWEEN 200MM & 900MM !");
          $(this).val("");
        }
      })
      .catch((error) => {
        console.error(error);
      });
  });

  $("#drop").on("input", function () {
    const blindtype = document.getElementById("blindtype").value;

    getBlindName(blindtype)
      .then((blindName) => {
        if (blindName === "Panel Only") {
          const drop = parseFloat($(this).val()) || 0;
          const louvresize = document.getElementById("louvresize").value;

          if ($(this).val().length < 4) return;
          if (louvresize === "63" && drop < 282) {
            isError("MINIMUM PANEL HEIGHT IS 282MM !");
            $(this).val("");
          } else if (louvresize === "89" && drop < 333) {
            isError("MINIMUM PANEL HEIGHT IS 333MM !");
            $(this).val("");
          } else if (louvresize === "114" && drop < 384) {
            isError("MINIMUM PANEL HEIGHT IS 384MM !");
            $(this).val("");
          } else if (drop > 2500) {
            isError("MAXIMUM PANEL HEIGHT IS 2500MM !");
            $(this).val("");
          }
        }
      })
      .catch((error) => {
        console.error(error);
      });
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
      bindComponentForm("", "");
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

function getBlindName(blindId) {
  if (!blindId) return;

  const type = "BlindName";
  return new Promise((resolve, reject) => {
    $.ajax({
      type: "POST",
      url: "Method.aspx/StringData",
      data: JSON.stringify({ type: type, dataId: blindId }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        resolve(response.d);
      },
      error: function (error) {
        reject(error);
      },
    });
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

    const listData = { type: "BlindTypeShutters", designtype: designId };

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
          Promise.all([
            bindColourType(selectedValue),
            bindMounting(selectedValue),
          ])
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

function bindColourType(blindType) {
  return new Promise((resolve, reject) => {
    const colourtype = document.getElementById("colourtype");
    colourtype.innerHTML = "";

    if (!blindType) {
      resolve();
      return;
    }

    const listData = {
      type: "ColourType",
      blindtype: blindType,
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

          let selectedValue = "";

          if (response.d.length === 1) {
            colourtype.selectedIndex = 0;
            selectedValue = colourtype.value;
          }

          Promise.resolve(bindComponentForm(blindType, selectedValue))
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

function bindMounting(blindId) {
  return new Promise((resolve, reject) => {
    const mounting = document.getElementById("mounting");

    if (!blindId) {
      resolve();
      return;
    }

    const listData = { type: "Mounting", blindtype: blindId };

    $.ajax({
      type: "POST",
      url: "Method.aspx/ListData",
      data: JSON.stringify({ data: listData }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        if (Array.isArray(response.d)) {
          mounting.innerHTML = "";

          if (response.d.length > 1) {
            const defaultOption = document.createElement("option");
            defaultOption.text = "";
            defaultOption.value = "";
            mounting.add(defaultOption);
          }

          response.d.forEach(function (item) {
            const option = document.createElement("option");
            option.value = item.Value;
            option.text = item.Text;
            mounting.add(option);
          });

          if (response.d.length === 1) {
            mounting.selectedIndex = 0;
          }
        }
        resolve();
      },
      error: function (error) {
        reject(error);
      },
    });
  });
}

function bindMidrailCritical(height1, height2) {
  return new Promise((resolve) => {
    const midrailcritical = document.getElementById("midrailcritical");
    midrailcritical.innerHTML = "";

    visibleMidrail(height1);

    let options = [{ value: "", text: "" }];

    if (height1 > 0 && height2 > 0) {
      options = [
        { value: "", text: "" },
        { value: "Yes - Top Only", text: "YES - TOP ONLY" },
        { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
      ];
    } else if (height1 > 0) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
      ];
    } else if (height2 > 0) {
      options = [
        { value: "", text: "" },
        { value: "Yes - Top Only", text: "YES - TOP ONLY" },
        { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
      ];
    }

    options.forEach((opt) => {
      const optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      midrailcritical.appendChild(optionElement);
    });

    resolve();
  });
}

function bindLayoutCode(blindType) {
  return new Promise((resolve, reject) => {
    const layoutcode = document.getElementById("layoutcode");
    layoutcode.innerHTML = "";

    if (!blindType) {
      resolve();
      return;
    }

    getBlindName(blindType)
      .then((blindName) => {
        let options = [{ value: "", text: "" }];

        switch (blindName) {
          case "Hinged":
            options = [
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
            options = [
              { value: "", text: "" },
              { value: "LL", text: "LL" },
              { value: "RR", text: "RR" },
              { value: "LLRR", text: "LLRR" },
              { value: "Other", text: "OTHER" },
            ];
            break;
          case "Track Bi-fold":
            options = [
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
            options = [
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
            options = [
              { value: "", text: "" },
              { value: "F", text: "F" },
              { value: "FF", text: "FF" },
              { value: "FFF", text: "FFF" },
              { value: "FFFF", text: "FFFF" },
              { value: "Other", text: "OTHER" },
            ];
            break;
          case "Fixed":
            options = [
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

        options.forEach((opt) => {
          let optionElement = document.createElement("option");
          optionElement.value = opt.value;
          optionElement.textContent = opt.text;
          layoutcode.appendChild(optionElement);
        });
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function bindFrameType(blindType, mounting, louvreSize, louvrePosition) {
  return new Promise((resolve, reject) => {
    const frametype = document.getElementById("frametype");
    const buildout = document.getElementById("buildout");
    frametype.innerHTML = "";

    visibleFrameDetail(frametype.value);
    visibleBuildout(blindType, frametype.value);
    visibleBuildoutPosition(blindType, frametype.value, buildout.value);

    if (!blindType) {
      resolve();
      return;
    }

    getBlindName(blindType)
      .then((blindName) => {
        let options = [{ value: "", text: "" }];

        if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
          options = [
            { value: "", text: "" },
            { value: "Beaded L 48mm", text: "BEADED L 48MM" },
            { value: "Insert L 50mm", text: "INSERT L 50MM" },
            { value: "Insert L 63mm", text: "INSERT L 63MM" },
            { value: "Flat L 48mm", text: "FLAT L 48MM" },
            { value: "No Frame", text: "NO FRAME" },
          ];
          if (mounting === "Inside") {
            options = [
              { value: "", text: "" },
              { value: "Beaded L 48mm", text: "BEADED L 48MM" },
              { value: "Insert L 50mm", text: "INSERT L 50MM" },
              { value: "Insert L 63mm", text: "INSERT L 63MM" },
              { value: "Flat L 48mm", text: "FLAT L 48MM" },
              {
                value: "Small Bullnose Z Frame",
                text: "SMALL BULLNOSE Z FRAME",
              },
              {
                value: "Large Bullnose Z Frame",
                text: "LARGE BULLNOSE Z FRAME",
              },
              { value: "Colonial Z Frame", text: "COLONIAL Z FRAME" },
              { value: "No Frame", text: "NO FRAME" },
            ];
          }
        } else if (blindName === "Track Bi-fold") {
          options = [
            { value: "", text: "" },
            { value: "100mm", text: "100MM" },
            { value: "160mm", text: "160MM" },
          ];
        } else if (blindName === "Track Sliding") {
          options = [
            { value: "", text: "" },
            { value: "100mm", text: "100MM" },
            { value: "160mm", text: "160MM" },
            { value: "200mm", text: "200MM" },
          ];
          if (louvrePosition === "Open") {
            options = [
              { value: "", text: "" },
              { value: "160mm", text: "160MM" },
              { value: "200mm", text: "200MM" },
            ];
          }
          if (
            louvrePosition === "Open" &&
            (louvreSize === "89" || louvreSize === "114")
          ) {
            options = [
              { value: "", text: "" },
              { value: "100mm", text: "100MM" },
              { value: "200mm", text: "200MM" },
            ];
          }
        } else if (blindName === "Track Sliding Single Track") {
          options = [{ value: "100mm", text: "100MM" }];
        } else if (blindName === "Fixed") {
          options = [
            { value: "", text: "" },
            { value: "U Channel", text: "U CHANNEL" },
            { value: "19x19 Light Block", text: "19X19 LIGHT BLOCK" },
          ];
        }

        options.forEach((opt) => {
          let optionElement = document.createElement("option");
          optionElement.value = opt.value;
          optionElement.textContent = opt.text;
          frametype.appendChild(optionElement);
        });

        if (frametype.options.length === 1) {
          bindLeftFrame(frametype.value);
          bindRightFrame(frametype.value);
          bindTopFrame(frametype.value);
          bindBottomFrame(frametype.value);

          visibleFrameDetail(frametype.value);
          visibleBuildout(blindType, frametype.value);
          visibleBuildoutPosition(blindType, frametype.value, buildout.value);
        }

        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function bindLeftFrame(frameType) {
  return new Promise((resolve) => {
    const frameleft = document.getElementById("frameleft");
    frameleft.innerHTML = "";

    if (!frameType) {
      resolve();
      return;
    }

    let options = [{ value: "", text: "" }];

    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
      ];
    } else if (
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
      ];
    } else if (frameType === "Large Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
      ];
    } else if (frameType === "Colonial Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
    } else if (frameType === "No Frame") {
      options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      options = [
        { value: "", text: "" },
        { value: "No", text: "NO" },
        { value: "L Strip", text: "L STRIP" },
      ];
    } else if (frameType === "19x19 Light Block") {
      options = [{ value: "No", text: "NO" }];
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      frameleft.appendChild(optionElement);
    });
    resolve();
  });
}

function bindRightFrame(frameType) {
  return new Promise((resolve, reject) => {
    const frameright = document.getElementById("frameright");
    frameright.innerHTML = "";

    if (!frameType) {
      resolve();
      return;
    }

    let options = [{ value: "", text: "" }];

    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm" ||
      frameType === "Flat L 48mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
      ];
    } else if (
      frameType === "Small Bullnose Z Frame" ||
      frameType === "Large Bullnose Z Frame" ||
      frameType === "Colonial Z Frame"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
      ];
    } else if (frameType === "Large Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
      ];
    } else if (frameType === "Colonial Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
      ];
    } else if (frameType === "No Frame") {
      options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      options = [
        { value: "", text: "" },
        { value: "No", text: "NO" },
        { value: "L Strip", text: "L STRIP" },
      ];
    } else if (frameType === "19x19 Light Block") {
      options = [{ value: "No", text: "NO" }];
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      frameright.appendChild(optionElement);
    });
    resolve();
  });
}

function bindTopFrame(frameType) {
  return new Promise((resolve, reject) => {
    const frametop = document.getElementById("frametop");
    frametop.innerHTML = "";

    if (!frameType) {
      resolve();
      return;
    }

    let options = [{ value: "", text: "" }];

    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Flat L 48mm", text: "FLAT L 48MM" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Flat L 48mm") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Small Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Large Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Colonial Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "No Frame") {
      options = [
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      options = [{ value: "Yes", text: "YES" }];
    } else if (frameType === "19x19 Light Block") {
      options = [{ value: "No", text: "NO" }];
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      frametop.appendChild(optionElement);
    });
    resolve();
  });
}

function bindBottomFrame(frameType) {
  return new Promise((resolve, reject) => {
    const framebottom = document.getElementById("framebottom");
    framebottom.innerHTML = "";
    bindBottomTrack("", "");

    if (!frameType) {
      resolve();
      return;
    }

    let options = [{ value: "", text: "" }];

    if (
      frameType === "Beaded L 48mm" ||
      frameType === "Insert L 50mm" ||
      frameType === "Insert L 63mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Flat L 48mm", text: "FLAT L 48MM" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Flat L 48mm") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Small Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Small Bullnose Z Sill Plate",
          text: "SMALL BULLNOSE Z SILL PLATE",
        },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Large Bullnose Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        {
          value: "Large Bullnose Z Sill Plate",
          text: "LARGE BULLNOSE Z SILL PLATE",
        },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "Colonial Z Frame") {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "9.5mm Sill Plate", text: "9.5MM SILL PLATE" },
        { value: "Colonial Z Sill Plate", text: "COLONIAL Z SILL PLATE" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (frameType === "No Frame") {
      options = [
        { value: "", text: "" },
        { value: "Light Block", text: "LIGHT BLOCK" },
        { value: "L Striker Plate", text: "L STRIKER PLATE" },
      ];
    } else if (
      frameType === "100mm" ||
      frameType === "160mm" ||
      frameType === "200mm"
    ) {
      options = [
        { value: "", text: "" },
        { value: "Yes", text: "YES" },
        { value: "No", text: "NO" },
      ];
    } else if (frameType === "U Channel") {
      options = [{ value: "Yes", text: "YES" }];
    } else if (frameType === "19x19 Light Block") {
      options = [{ value: "No", text: "NO" }];
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      framebottom.appendChild(optionElement);
    });
    resolve();
  });
}

function bindBottomTrack(blindType, bottomFrame) {
  return new Promise((resolve, reject) => {
    const bottomtracktype = document.getElementById("bottomtracktype");
    bottomtracktype.innerHTML = "";

    visibleBottomTrackReccess(bottomtracktype.value);

    if (!blindType) {
      resolve();
      return;
    }

    getBlindName(blindType)
      .then((blindName) => {
        let options = [{ value: "", text: "" }];

        if (
          blindName === "Track Bi-fold" ||
          blindName === "Track Sliding" ||
          blindName === "Track Sliding Single Track"
        ) {
          options = [
            { value: "", text: "" },
            { value: "M Track", text: "M TRACK" },
            { value: "U Track", text: "U TRACK" },
          ];
          if (bottomFrame === "Yes") {
            options = [{ value: "U Track", text: "U TRACK" }];
          }
        }

        options.forEach((opt) => {
          let optionElement = document.createElement("option");
          optionElement.value = opt.value;
          optionElement.textContent = opt.text;
          bottomtracktype.appendChild(optionElement);
        });

        if (bottomtracktype.options.length === 0) {
          bottomtracktype.selectedIndex = 0;
          visibleBottomTrackReccess(bottomtracktype.value);
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function bindTiltrodSplit(height1) {
  return new Promise((resolve, reject) => {
    const tiltrodsplit = document.getElementById("tiltrodsplit");
    tiltrodsplit.innerHTML = "";

    visibleSplitHeight(tiltrodsplit.value);

    let options = [{ value: "", text: "" }];

    if (height1 === 0) {
      options = [
        { value: "", text: "" },
        { value: "None", text: "NONE" },
        { value: "Split Halfway", text: "SPLIT HALFWAY" },
        { value: "Other", text: "OTHER" },
      ];
    } else if (height1 > 0) {
      options = [
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
          text: "SPLIT HALFWAY ABOVE & BELOW MIDRAIL",
        },
        { value: "Other", text: "OTHER" },
      ];
    }

    options.forEach((opt) => {
      let optionElement = document.createElement("option");
      optionElement.value = opt.value;
      optionElement.textContent = opt.text;
      tiltrodsplit.appendChild(optionElement);
    });

    if (tiltrodsplit.options.length === 0) {
      tiltrodsplit.selectedIndex = 0;
      visibleSplitHeight(tiltrodsplit.value);
    }

    resolve();
  });
}

function bindComponentForm(blindType, colourType) {
  const divDetail = document.getElementById("divDetail");
  divDetail.style.display = "none";

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

  if (colourType) {
    divDetail.style.display = "";

    divLouvrePosition.style.display = "none";
    divMidrailHeight2.style.display = "none";
    divMidrailCritical.style.display = "none";
    divPanelQty.style.display = "none";
    divJoinedPanels.style.display = "none";
    divHingeColour.style.display = "none";
    divSemiInsideMount.style.display = "none";
    divCustomHeaderLength.style.display = "none";
    divLayoutCode.style.display = "none";
    divLayoutCodeCustom.style.display = "none";
    divFrameType.style.display = "none";
    divFrameLeft.style.display = "none";
    divFrameRight.style.display = "none";
    divFrameTop.style.display = "none";
    divFrameBottom.style.display = "none";
    divBottomTrackType.style.display = "none";
    divBottomTrackRecess.style.display = "none";
    divBuildout.style.display = "none";
    divBuildoutPosition.style.display = "none";
    divSameSize.style.display = "none";
    divGapPost.style.display = "none";
    divHorizontalTPost.style.display = "none";
    divHorizontalTPostRequired.style.display = "none";
    divTiltrodType.style.display = "none";
    divTiltrodSplit.style.display = "none";
    divTiltrodHeight.style.display = "none";
    divReverseHinged.style.display = "none";
    divPelmetFlat.style.display = "none";
    divExtraFascia.style.display = "none";
    divHingesLoose.style.display = "none";
    divCutOut.style.display = "none";
    divSpecialShape.style.display = "none";
    divTemplateProvided.style.display = "none";

    getBlindName(blindType)
      .then((blindName) => {
        if (blindName === "Panel Only") {
          divPanelQty.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
        } else if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
          divHingeColour.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
          divSpecialShape.style.display = "";
          divLayoutCode.style.display = "";
          divFrameType.style.display = "";
          divHorizontalTPost.style.display = "";
        } else if (blindName === "Track Bi-fold") {
          divHingeColour.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
          divLayoutCode.style.display = "";
          divFrameType.style.display = "";
          divBottomTrackType.style.display = "";
          divReverseHinged.style.display = "";
          divPelmetFlat.style.display = "";
          divExtraFascia.style.display = "";
        } else if (blindName === "Track Sliding") {
          divLouvrePosition.style.display = "";
          divJoinedPanels.style.display = "";
          divCustomHeaderLength.style.display = "";
          divLayoutCode.style.display = "";
          divFrameType.style.display = "";
          divBottomTrackType.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
          divPelmetFlat.style.display = "";
          divExtraFascia.style.display = "";
        } else if (blindName === "Track Sliding Single Track") {
          divJoinedPanels.style.display = "";
          divCustomHeaderLength.style.display = "";
          divLayoutCode.style.display = "";
          divFrameType.style.display = "";
          divFrameLeft.style.display = "";
          divFrameRight.style.display = "";
          divFrameTop.style.display = "";
          divFrameBottom.style.display = "";
          divBottomTrackType.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
          divPelmetFlat.style.display = "";
          divExtraFascia.style.display = "";
        } else if (blindName === "Fixed") {
          divLayoutCode.style.display = "";
          divFrameType.style.display = "";
          divTiltrodType.style.display = "";
          divTiltrodSplit.style.display = "";
          divSpecialShape.style.display = "";
        }
      })
      .catch((error) => {
        reject(error);
      });
  }
}

function visibleMidrail(height1) {
  return new Promise((resolve) => {
    const divMidrailHeight2 = document.getElementById("divMidrailHeight2");
    const divMidrailCritical = document.getElementById("divMidrailCritical");

    divMidrailHeight2.style.display = "none";
    divMidrailCritical.style.display = "none";

    if (height1 > 0) {
      divMidrailHeight2.style.display = "";
      divMidrailCritical.style.display = "";
    }

    resolve();
  });
}

function visibleHingeColour(blindType, joinedPanels) {
  return new Promise((resolve, reject) => {
    const divHingeColour = document.getElementById("divHingeColour");
    divHingeColour.style.display = "none";

    if (!blindType) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (
          blindName === "Hinged" ||
          blindName === "Hinged Bi-fold" ||
          blindName === "Track Bi-fold"
        ) {
          divHingeColour.style.display = "";
        } else if (
          blindName === "Track Sliding" ||
          blindName === "Track Sliding Single Track"
        ) {
          if (joinedPanels === "Yes") {
            divHingeColour.style.display = "";
          }
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleLayoutCustom(layout) {
  return new Promise((resolve) => {
    const divLayoutCodeCustom = document.getElementById("divLayoutCodeCustom");
    divLayoutCodeCustom.style.display = "none";
    if (layout === "Other") {
      divLayoutCodeCustom.style.display = "";
    }

    resolve();
  });
}

function visibleFrameDetail(frameType) {
  return new Promise((resolve) => {
    const frameElements = [
      document.getElementById("divFrameLeft"),
      document.getElementById("divFrameRight"),
      document.getElementById("divFrameTop"),
      document.getElementById("divFrameBottom"),
    ];

    const displayValue = frameType !== "" ? "" : "none";

    frameElements.forEach((element) => {
      element.style.display = displayValue;
    });

    resolve();
  });
}

function visibleBuildout(blindType, frameType) {
  return new Promise((resolve, reject) => {
    const divBuildout = document.getElementById("divBuildout");
    divBuildout.style.display = "none";

    if (!blindType || !frameType) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
          divBuildout.style.display = "";
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleBuildoutPosition(blindType, frameType, buildout) {
  return new Promise((resolve, reject) => {
    const divBuildoutPosition = document.getElementById("divBuildoutPosition");
    divBuildoutPosition.style.display = "none";

    if (!blindType || !frameType) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
          if (
            (frameType === "Small Bullnose Z Frame" ||
              frameType === "Large Bullnose Z Frame" ||
              frameType === "Colonial Z Frame") &&
            buildout !== ""
          ) {
            divBuildoutPosition.style.display = "";
          }
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleSameSize(blindType, layoutCode) {
  return new Promise((resolve, reject) => {
    const divSameSize = document.getElementById("divSameSize");
    divSameSize.style.display = "none";

    if (!blindType && !layoutCode) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (
          (blindName === "Hinged" || blindName === "Hinged Bi-fold") &&
          cekSameSizePanels(layoutCode)
        ) {
          divSameSize.style.display = "";
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleGap(blindType, sameSize, layoutCode) {
  return new Promise((resolve, reject) => {
    const divGapPost = document.getElementById("divGapPost");
    divGapPost.style.display = "none";

    const divGap1 = document.getElementById("divGap1");
    const divGap2 = document.getElementById("divGap2");
    const divGap3 = document.getElementById("divGap3");
    const divGap4 = document.getElementById("divGap4");
    const divGap5 = document.getElementById("divGap5");

    divGap1.style.display = "none";
    divGap2.style.display = "none";
    divGap3.style.display = "none";
    divGap4.style.display = "none";
    divGap5.style.display = "none";

    if (!blindType) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
          if (cekSameSizePanels(layoutCode)) {
            if (sameSize === "Yes") {
              divGapPost.style.display = "none";
            } else {
              let countT = 0;
              for (let char of layoutCode) {
                if (
                  char === "T" ||
                  char === "B" ||
                  char === "C" ||
                  char === "G"
                )
                  countT++;
              }
              if (countT > 0) divGapPost.style.display = "";
              if (countT > 0) divGap1.style.display = "";
              if (countT > 1) divGap2.style.display = "";
              if (countT > 2) divGap3.style.display = "";
              if (countT > 3) divGap4.style.display = "";
              if (countT > 4) divGap5.style.display = "";
            }
          } else {
            let countT = 0;
            for (let char of layoutCode) {
              if (char === "T" || char === "B" || char === "C" || char === "G")
                countT++;
            }
            if (countT > 0) divGapPost.style.display = "";
            if (countT > 0) divGap1.style.display = "";
            if (countT > 1) divGap2.style.display = "";
            if (countT > 2) divGap3.style.display = "";
            if (countT > 3) divGap4.style.display = "";
            if (countT > 4) divGap5.style.display = "";
          }
        } else if (blindName === "Track Bi-fold") {
          let countT = 0;
          for (let char of layoutCode) {
            if (char === "T" || char === "B" || char === "C" || char === "G")
              countT++;
          }
          if (countT > 0) divGapPost.style.display = "";
          if (countT > 0) divGap1.style.display = "";
          if (countT > 1) divGap2.style.display = "";
          if (countT > 2) divGap3.style.display = "";
          if (countT > 3) divGap4.style.display = "";
          if (countT > 4) divGap5.style.display = "";
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleHingesLoose(blindType, hingeColour, joinedPanels) {
  return new Promise((resolve, reject) => {
    const divHingesLoose = document.getElementById("divHingesLoose");
    divHingesLoose.style.display = "none";

    if (!hingeColour) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (
          blindName === "Hinged" ||
          blindName === "Hinged Bi-fold" ||
          blindName === "Track Bi-fold"
        ) {
          divHingesLoose.style.display = "";
        } else if (blindName === "Track Sliding" && joinedPanels === "Yes") {
          divHingesLoose.style.display = "";
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleSemiInside(blindType, mounting) {
  return new Promise((resolve, reject) => {
    const divSemiInside = document.getElementById("divSemiInsideMount");
    divSemiInside.style.display = "none";

    if (!blindType) return resolve();

    getBlindName(blindType)
      .then((blindName) => {
        if (
          (blindName === "Track Bi-fold" ||
            blindName === "Track Sliding" ||
            blindName === "Track Sliding Single Track") &&
          mounting === "Inside"
        ) {
          divSemiInside.style.display = "";
        }
        resolve();
      })
      .catch((error) => {
        reject(error);
      });
  });
}

function visibleBottomTrackReccess(bottomTrack) {
  return new Promise((resolve) => {
    const divBottomTrackRecess = document.getElementById(
      "divBottomTrackRecess"
    );
    divBottomTrackRecess.style.display = "none";

    const bottomtrackrecess = document.getElementById("bottomtrackrecess");

    if (!bottomTrack) return resolve();

    if (bottomTrack === "M Track") divBottomTrackRecess.style.display = "";
    if (bottomTrack === "U Track") bottomtrackrecess.value = "Yes";

    resolve();
  });
}

function visibleSplitHeight(tiltrodSplit) {
  return new Promise((resolve) => {
    const tiltrodHeight = document.getElementById("divTiltrodHeight");
    tiltrodHeight.style.display = "none";

    if (tiltrodSplit === "Other") tiltrodHeight.style.display = "";

    resolve();
  });
}

function visibleTemplateProvided(spesialShape) {
  return new Promise((resolve) => {
    const divTemplateProvided = document.getElementById("divTemplateProvided");
    divTemplateProvided.style.display = "none";

    if (!specialshape) return resolve();

    if (spesialShape === "Yes") divTemplateProvided.style.display = "";

    resolve();
  });
}

function visibleHorizontalRequired(horizontalHeigth) {
  return new Promise((resolve) => {
    const thisDiv = document.getElementById("divHorizontalTPostRequired");
    thisDiv.style.display = "none";

    if (horizontalHeigth > 0) thisDiv.style.display = "";

    resolve();
  });
}

function proccess() {
  toggleButtonState(true, "Processing...");

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
  };

  fields.forEach((id) => {
    formData[id] = document.getElementById(id).value;
  });

  $.ajax({
    type: "POST",
    url: "Method.aspx/PanoramaProccess",
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
    error: function () {
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

function bindItemOrder(itemId) {
  document.getElementById("divLoader").style.display = "";

  return new Promise((resolve, reject) => {
    $.ajax({
      type: "POST",
      url: "Method.aspx/ShutterDetail",
      data: JSON.stringify({ itemId }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (response) {
        const data = response.d;

        if (!data.length) {
          document.getElementById("divLoader").style.display = "none";
          reject("No data found");
          return;
        }

        const itemData = data[0];

        const {
          BlindType: blindtype,
          ColourType: colourtype,
          Mounting: mounting,
          MidrailHeight1: height1,
          MidrailHeight2: height2,
          LouvreSize: louvreSize,
          LouvrePosition: louvrePosition,
          Buildout: buildout,
          FrameType: frameType,
          FrameBottom: bottomFrame,
          JoinedPanels: joinedPanels,
          LayoutCode: layoutCode,
          LayoutCodeCustom: layoutCodeCustom,
          SamePanelSize: sameSize,
          HingeColour: hingeColour,
          BottomTrackType: bottomTrack,
          SpecialShape: specialShape,
          TiltrodSplit: tiltrodSplit,
          HorizontalTPostHeight: HorizontalTPostHeight,
        } = itemData;

        let layoutCodeFinal =
          layoutCode === "Other" ? layoutCodeCustom : layoutCode;

        Promise.resolve()
          .then(() => bindBlindType(designId))
          .then(() => bindColourType(blindtype))
          .then(() => bindMounting(blindtype))
          .then(() => bindMidrailCritical(height1, height2))
          .then(() => bindLayoutCode(blindtype))
          .then(() =>
            bindFrameType(blindtype, mounting, louvreSize, louvrePosition)
          )
          .then(() => bindLeftFrame(frameType))
          .then(() => bindRightFrame(frameType))
          .then(() => bindTopFrame(frameType))
          .then(() => bindBottomFrame(frameType))
          .then(() => bindBottomTrack(blindtype, bottomFrame))
          .then(() => bindTiltrodSplit(height1))
          .then(() => setFormValues(itemData))
          .then(() => bindComponentForm(blindtype, colourtype))
          .then(() => {
            return Promise.all([
              visibleMidrail(height1),
              visibleHingeColour(blindtype, joinedPanels),
              visibleLayoutCustom(layoutCode),
              visibleFrameDetail(frameType),
              visibleBuildout(blindtype, frameType),
              visibleBuildoutPosition(blindtype, frameType, buildout),
              visibleSameSize(blindtype, layoutCodeFinal),
              visibleGap(blindtype, sameSize, layoutCodeFinal),
              visibleHingesLoose(blindtype, hingeColour, joinedPanels),
              visibleSemiInside(blindtype, mounting),
              visibleBottomTrackReccess(bottomTrack),
              visibleSplitHeight(tiltrodSplit),
              visibleTemplateProvided(specialShape),
              visibleHorizontalRequired(HorizontalTPostHeight),
            ]);
          })
          .then(() => {
            document.getElementById("divLoader").style.display = "none";
            document.getElementById("divOrder").style.display = "";
            resolve();
          })
          .catch((error) => {
            console.error("Promise chain error:", error);
            document.getElementById("divLoader").style.display = "none";
            reject(error);
          });
      },
      error: function (error) {
        document.getElementById("divLoader").style.display = "none";
        reject(error);
      },
    });
  });
}

function setFormValues(itemData) {
  const mapping = {
    blindtype: "BlindType",
    colourtype: "ColourType",
    qty: "Qty",
    room: "Room",
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
    layoutcode: "LayoutCode",
    layoutcodecustom: "LayoutCodeCustom",
    frametype: "FrameType",
    frameleft: "FrameLeft",
    frameright: "FrameRight",
    frametop: "FrameTop",
    framebottom: "FrameBottom",
    bottomtracktype: "BottomTrackType",
    bottomtrackrecess: "BottomTrackRecess",
    buildout: "Buildout",
    buildoutposition: "BuildoutPosition",
    samesizepanel: "SamePanelSize",
    gap1: "Gap1",
    gap2: "Gap2",
    gap3: "Gap3",
    gap4: "Gap4",
    gap5: "Gap5",
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
    const resetFields = ["room", "width", "drop", "notes"];
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

  const submit = document.getElementById("submit");
  if (status === true) {
    submit.style.display = "none";
  } else {
    submit.style.display = "";
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

  function resetFields() {
    fields.forEach((id) => {
      let element = document.getElementById(id);
      if (element) {
        if (element.tagName === "SELECT") {
          element.selectedIndex = 0;
        } else {
          element.value = "";
        }
      }
    });
  }
  resetFields();
}

function showInfo(type) {
  let title;
  let info;

  if (type === "Tiltrod Type") {
    title = "Tiltrod Type Information";
    info =
      "<b>Easy Tilt</b>: Internal rack and pinion.<br /><b>Clearview</b>: Metal rod on back edge of louvres.";
  } else if (type === "Gap") {
    title = "T-Post / Gap / Bay / Corner Location Information";
    info =
      "The factory will make all panels within an opening the same width unless otherwise indicated. If specific T-post locations are required, enter the measurement from the far left-hand side to the centre of the T-post measurement. The factory will make the panels to fit in between these posts.";
  }
  document.getElementById("modalTitle").innerHTML = title;
  document.getElementById("spanInfo").innerHTML = info;
}
