document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("RollerBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  globalRollerPageLoaded();
});

// =================================================EVENTS==================================================
// input or chenge  remove class is-invalid & event
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    // ---------------------------------||blindtype||---------------------------------
    if (e.target.id === "blindtype") {
      // if (!e.target.value) return;

      const blindid = e.target.value;
      const blindname = e.target.selectedOptions[0].dataset.name;
      await handlerElementVisibility(blindname);
      await bindBrackets(DESIGNID, blindid);
    }

    // ---------------------------------||brackettype||---------------------------------
    if (e.target.id === "brackettype") {
      // if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = e.target.value;
      await handlerElementVisibility(blindname, brackettype);
      await bindTubes(DESIGNID, blindid, brackettype);
    }

    // ---------------------------------||tubetype||---------------------------------
    if (e.target.id === "tubetype") {
      // if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = e.target.value;
      await handlerElementVisibility(blindname, brackettype, tubetype);
      await bindControls(DESIGNID, blindid, brackettype, tubetype);
    }

    // ---------------------------------||controltype||---------------------------------
    if (e.target.id === "controltype") {
      // if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
      );
      await bindColours(DESIGNID, blindid, brackettype, tubetype, controltype);
    }

    // ---------------------------------||colourtype||---------------------------------
    if (e.target.id === "colourtype") {
      // if (!e.target.value) return;
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = document.getElementById("controltype").value;
      const colourtype = e.target.value;

      await bindFabrics(DESIGNID);
      if (
        blindname == "Motorised" ||
        (blindname == "Cassette" && tubetype == "Motorised")
      ) {
        await Promise.all([
          bindMotorStyle(controltype),
          bindMotorRemote(controltype),
        ]);
      }
      await Promise.all([
        bindChains(DESIGNID),
        bindTrims(blindname, brackettype, tubetype),
        bindTubeSize(blindname, tubetype),
        bindChildSafe(),
        bindAccessory(),
      ]);
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
        colourtype,
      );
    }

    // ---------------------------------||fabrictype||---------------------------------
    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    // ---------------------------------||motorstyle||---------------------------------
    if (e.target.id === "motorstyle") {
      const divMotorBattery = document.getElementById("divMotorBattery");
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const controltype = document.getElementById("controltype").value;
      const motorstyle = e.target.value;

      divMotorBattery.classList.add("d-none");

      await Promise.all([
        bindExternalBattery(),
        bindMotorCharger(controltype, motorstyle),
        bindExtras(blindname, controltype, motorstyle),
      ]);

      if (motorstyle.includes("EXB")) {
        divMotorBattery.classList.remove("d-none");
      }
    }

    // ---------------------------------||trim||---------------------------------
    if (e.target.id === "trim") {
      const divBottomRail = document.getElementById("divBottomRail");
      const divAccessory = document.getElementById("divAccessory");
      const lblBotomRail = document.getElementById("lblBotomRail");
      const divRailColour = document.getElementById("divRailColour");

      divBottomRail.classList.add("d-none");
      divAccessory.classList.add("d-none");
      divRailColour.classList.add("d-none");
      lblBotomRail.innerHTM = "bottom rail type x colour";

      if (!e.target.value) return;
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const trim = e.target.value;

      if (trim == "Decorative") {
        lblBotomRail.innerHTML = "Decorative Trim";
      }

      bindRailType(brackettype, trim);

      if (blindname == "Skin Only" && trim == "1F") {
        divBottomRail.classList.remove("d-none");
        divRailColour.classList.remove("d-none");
      }
      if (
        ["Cassette", "Motorised", "Gear Reduction"].includes(blindname) &&
        ["Bottom Rail", "Decorative"].includes(trim)
      ) {
        divBottomRail.classList.remove("d-none");
        if (trim == "Bottom Rail") {
          divRailColour.classList.remove("d-none");
        }
        divBottomRail.classList.remove("d-none");
        divAccessory.classList.remove("d-none");
      }
    }

    // ---------------------------------||railtype||---------------------------------
    if (e.target.id === "railtype") {
      const brackettype = document.getElementById("brackettype").value;
      const railtype = e.target.value;
      const trim = document.getElementById("trim").value;

      bindRailColour(brackettype, railtype, trim);
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "notes") {
      let maxLength = 1000;
      let currentLength = e.target.value.length;
      document.querySelector("#notescount").textContent =
        `${currentLength}/${maxLength}`;
    }
  });
});

// button submit
document.querySelector("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  // handlerSubmit(e.target.form, e.target.id);
  handlerSubmit(e.target.id);
});

const btnInfo = document.querySelectorAll(".btn-information");
if (btnInfo) {
  btnInfo.forEach((el) => {
    el.addEventListener("click", (e) => {
      try {
        let text = "";
        let id = e.currentTarget.id;

        switch (id) {
          case "btnInfoControlType":
            const blind = document.querySelector("#blindtype");
            const blindName = blind.options[blind.selectedIndex].dataset.name;
            const tubetype = document.querySelector("#tubetype").value;

            if (
              blindName == "Motorised" ||
              (blindName == "Cassette" && tubetype == "Motorised")
            ) {
              text =
                " RTS - Wired Motors <br/> WS – Battery RTS Motors <br/> WS – Switch Motors";
            }
            break;
          case "btnInfoQty":
            text =
              "Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.";
            break;
          case "btnInfoMotorStyle":
            text =
              "If any another blind (Double or linked) <br /> If you change this MOTOR STYLE then the other motor style will follow this motor style.";
            break;
          case "btnInfoMotorRemote":
            text =
              "If any another blind (Double or linked) <br /> If you change this MOTOR REMOTE then the other motor remote will follow this motor remote.";
            break;
          case "btnInfoTrim":
            text =
              "If you want to use the BOTTOM RAIL.<br />Please select <b>Base Bar Trim</b> Or <b>Decorative Trim</b>.";
            break;
          case "btnInfoTubeSize":
            text =
              "Our standard tube size <br /><br /> 1. If the width or drop are below 2400 then the tube size uses 40 <br /> 2. If the width or drop are more than 2400 then the tube size uses 45 <br /> 3. If the width or drop are more than 2600 then the tube size uses 45H";
            break;
        }

        if (text) {
          isInfo(text);
        }
      } catch (error) {
        var msg = error.message;
        if (ROLENAME != "Administrator") {
          msg = "Please contact our IT team at support@onlineorder.au";
        }
      }
    });
  });
}

// button cancel
document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// ===============================================FUNCTION==================================================
const globalRollerPageLoaded = async () => {
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
    document.getElementById("lblBlindNo").innerHTML = "Blind 1";
    await handlerElementVisibility();
    loaderFadeOut();
  } else if (
    ["NextItem", "EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)
  ) {
    await bindItemOrders(ITEMID);
    loaderFadeOut();
  }
};

// ------------------------------------------------------|| Handler Functions ||--------------------------------------
const handlerElementVisibility = async (
  blindname,
  brackettype,
  tubetype,
  controltype,
  colourtype,
  item,
) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const lblBlindNo = document.getElementById("lblBlindNo");
    const lblUniqueId = document.getElementById("lblUniqueId");
    const lblBracketType = document.getElementById("lblBracketType");
    const divBracketType = document.getElementById("divBracketType");
    const divTubeType = document.getElementById("divTubeType");
    const divControlType = document.getElementById("divControlType");
    const btnInfoControlType = document.getElementById("btnInfoControlType");
    const lblColourType = document.getElementById("lblColourType");
    const divColourType = document.getElementById("divColourType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divAttention = document.getElementById("divAttention");
    const lblNextDesc = document.getElementById("lblNextDesc");
    const divMotorStyle = document.getElementById("divMotorStyle");
    const btnInfoMotorStyle = document.getElementById("btnInfoMotorStyle");
    const divMotorRemote = document.getElementById("divMotorRemote");
    const btnInfoMotorRemote = document.getElementById("btnInfoMotorRemote");
    const divMotorBattery = document.getElementById("divMotorBattery");
    const divMotorCharger = document.getElementById("divMotorCharger");
    const divCableExitPoint = document.getElementById("divCableExitPoint");
    const divConnector = document.getElementById("divConnector");
    const divRoll = document.getElementById("divRoll");
    const divControlPosition = document.getElementById("divControlPosition");
    const lblControlPosition = document.getElementById("lblControlPosition");
    const divChain = document.getElementById("divChain");
    const divBottomRail = document.getElementById("divBottomRail");
    const divTubeSize = document.getElementById("divTubeSize");
    const divAdditional = document.getElementById("divAdditional");
    const divChildSafe = document.getElementById("divChildSafe");
    const divAccessory = document.getElementById("divAccessory");
    const divExtras = document.getElementById("divExtras");
    const divBracketCover = document.getElementById("divBracketCover");
    const divBracketExt = document.getElementById("divBracketExt");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    lblItemId.classList.add("d-none");
    lblBlindNo.classList.add("d-none");
    lblUniqueId.classList.add("d-none");
    lblBracketType.innerHTML = "bracket type";
    divBracketType.classList.add("d-none");
    divTubeType.classList.add("d-none");
    divControlType.classList.add("d-none");
    btnInfoControlType.classList.add("d-none");
    lblColourType.innerHTML = "control colour";
    divColourType.classList.add("d-none");
    divControlPosition.classList.add("d-none");
    divFormDetail.classList.add("d-none");
    divAttention.classList.add("d-none");
    divMotorStyle.classList.add("d-none");
    btnInfoMotorStyle.classList.add("d-none");
    divMotorRemote.classList.add("d-none");
    btnInfoMotorRemote.classList.add("d-none");
    divMotorBattery.classList.add("d-none");
    divMotorCharger.classList.add("d-none");
    divCableExitPoint.classList.add("d-none");
    divConnector.classList.add("d-none");
    divRoll.classList.add("d-none");

    lblControlPosition.innerHTML = "control position";
    divChain.classList.add("d-none");
    divBottomRail.classList.add("d-none");
    divTubeSize.classList.add("d-none");
    divAdditional.classList.add("d-none");
    divChildSafe.classList.add("d-none");
    divAccessory.classList.add("d-none");
    divExtras.classList.add("d-none");
    divBracketCover.classList.add("d-none");
    divBracketExt.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (ROLENAME === "Administrator" && LEVELNAME === "Super Admin") {
      // lblItemId.classList.remove("d-none");
      // lblBlindNo.classList.remove("d-none");
      // lblUniqueId.classList.remove("d-none");
    }

    // -------------------------------|| on change blindtype ||---------------------------------
    if (!blindname) return;
    if (blindname === "Cassette") {
      lblBracketType.innerHTML = "cassette type";
      lblColourType.innerHTML = "cassette colour";
    }

    if (["Motorised", "Gear Reduction"].includes(blindname)) {
      lblColourType.innerHTML = "colour type";
    }

    if (["Cassette", "Motorised", "Gear Reduction"].includes(blindname)) {
      divAdditional.classList.remove("d-none");
    }

    divBracketType.classList.remove("d-none");

    // ---------------------------------|| on change brackettype ||---------------------------------
    if (!brackettype) return;
    if (["Cassette", "Motorised", "Gear Reduction"].includes(blindname)) {
      divTubeType.classList.remove("d-none");
    }

    if (blindname === "Skin Only") {
      if (
        ["With Tube & Bottom Included", "With Tube Included"].includes(
          brackettype,
        )
      ) {
        divTubeSize.classList.remove("d-none");
      }
    }

    // ---------------------------------|| on change tubetype ||---------------------------------
    if (!tubetype) return;
    if (
      blindname == "Motorised" ||
      (blindname == "Cassette" && tubetype == "Motorised")
    ) {
      btnInfoControlType.classList.remove("d-none");
    }

    if (["Cassette", "Motorised", "Gear Reduction"].includes(blindname)) {
      if (tubetype !== "Spring Operated") {
        divControlType.classList.remove("d-none");
      }
    }

    // ---------------------------------|| on change controltype ||---------------------------------
    if (!controltype) return;
    if (["Cassette", "Motorised", "Gear Reduction"].includes(blindname)) {
      if (tubetype !== "Spring Operated") {
        divColourType.classList.remove("d-none");
      }
    }

    // ---------------------------------|| on change colourtype ||---------------------------------
    if (!colourtype) return;
    divFormDetail.classList.remove("d-none");
    if (blindname === "Cassette") {
      if (tubetype === "Motorised") {
        divMotorStyle.classList.remove("d-none");
        divMotorRemote.classList.remove("d-none");
        if (["Alpha WF", "Somfy WF"].includes(controltype)) {
          divMotorCharger.classList.remove("d-none");
        }
        if (!["Alpha WF", "Somfy WF"].includes(controltype)) {
          divConnector.classList.remove("d-none");
        }
        if (
          ["Alpha RTS", "Alpha WS", "Somfy RTS", "Somfy WS"].includes(
            controltype,
          )
        ) {
          divCableExitPoint.classList.remove("d-none");
        }
        divAccessory.classList.remove("d-none");
        divExtras.classList.remove("d-none");
      }
      if (tubetype == "JAI Geared") {
        divChain.classList.remove("d-none");
        divChildSafe.classList.remove("d-none");
        divAccessory.classList.remove("d-none");
      }
      divRoll.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      lblControlPosition.innerHTML = "control side";
      divBracketCover.classList.remove("d-none");
      if (brackettype === "Double") {
        divBracketExt.classList.add("d-none");
      }
    }

    if (blindname === "Motorised") {
      divMotorStyle.classList.remove("d-none");
      divMotorRemote.classList.remove("d-none");
      if (["Alpha WF", "Somfy WF"].includes(controltype)) {
        divMotorCharger.classList.remove("d-none");
      }
      if (!["Alpha WF", "Somfy WF"].includes(controltype)) {
        divConnector.classList.remove("d-none");
      }
      divRoll.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      lblControlPosition.innerHTML = "motor side";
      divExtras.classList.remove("d-none");
      divBracketCover.classList.remove("d-none");
      if (brackettype === "Double") {
        divBracketExt.classList.add("d-none");
      }
    }

    if (blindname === "Gear Reduction") {
      if (tubetype !== "Spring Operated") {
        divRoll.classList.remove("d-none");
        divControlPosition.classList.remove("d-none");
        divChain.classList.remove("d-none");
        divBracketCover.classList.remove("d-none");
        if (brackettype === "Double") {
          divBracketExt.classList.add("d-none");
        }
      }
      divTubeSize.classList.remove("d-none");
      divChildSafe.classList.remove("d-none");
      divAccessory.classList.remove("d-none");
    }

    if (item) {
      if (ITEMACTION === "EditItem") {
        let blinds = "first blind";
        if (item.BlindNo === "Blind 2") blinds = "second blind";
        if (item.BlindNo === "Blind 3") blinds = "third blind";
        if (item.BlindNo === "Blind 4") blinds = "fourth blind";

        let totalBlind = await getItemData(
          `SELECT COUNT(*) FROM OrderDetails WHERE UniqueId = '${item.UniqueId}' AND Active = 1`,
        );

        // ------------------------------------|| Double, Linked 2 Blinds (Dep), Linked 2 Blinds (Ind) ||------------------------------------
        if (
          ["Double", "Linked 2 Blinds (Dep)", "Linked 2 Blinds (Ind)"].includes(
            item.BracketType,
          )
        ) {
          if (totalBlind > 1) {
            divAttention.classList.remove("d-none");
            let connectId = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
            );
            if (item.BlindNo === "Blind 2") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
            }

            lblNextDesc.innerHTML = `This is the <b><u>${blinds}</u></b> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, then the data on the <b><u>ITEM ID ${connectId}</u></b>  blind will automatically be changed according to this data.`;
          }
        }

        // ------------------------------------|| Linked 3 Blinds (Dep), Linked 3 Blinds (Ind) ||------------------------------------
        if (
          ["Linked 3 Blinds (Dep)", "Linked 3 Blinds (Ind)"].includes(
            item.BracketType,
          )
        ) {
          if (totalBlind > 1) {
            divAttention.classList.remove("d-none");
            let connectId = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
            );
            let connectId2 = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
            );

            let blindid = connectId;
            if (connectId2) {
              blindid = `${blindid} AND ITEM ID ${connectId2}`;
            }

            if (item.BlindNo === "Blind 2") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId2 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
              );

              blindid = connectId;
              if (connectId2) {
                blindid = `${blindid} AND ITEM ID ${connectId2}`;
              }
            }

            if (item.BlindNo === "Blind 3") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId2 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
              );

              blindid = `${connectId} AND ITEM ID ${connectId2}`;
            }

            lblNextDesc.innerHTML = `This is the <b><u>${blinds}</u></b> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, bracket cover and bracket extension,  then the data on the <b><u>ITEM ID ${blindid}</u></b>  blind will automatically be changed according to this data.`;
          }
        }

        // ------------------------------------|| Double and Link System Dep, Double and Link System Ind ||------------------------------------
        if (
          ["Linked 3 Blinds (Dep)", "Linked 3 Blinds (Ind)"].includes(
            item.BracketType,
          )
        ) {
          if (totalBlind > 1) {
            divAttention.classList.remove("d-none");
            let connectId = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
            );
            let connectId2 = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
            );
            let connectId3 = await getItemData(
              `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId ='${item.UniqueId}'`,
            );

            let blindid = connectId;
            if (connectId2) {
              blindid = `${blindid} AND ITEM ID ${connectId2}`;
            }
            if (connectId3) {
              blindid = `${blindid}, ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
            }

            if (item.BlindNo === "Blind 2") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId2 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId3 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId ='${item.UniqueId}'`,
              );

              blindid = connectId;
              if (connectId2) {
                blindid = `${blindid} AND ITEM ID ${connectId2}`;
              }
              if (connectId3) {
                blindid = `${blindid}, ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
              }
            }

            if (item.BlindNo === "Blind 3") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId2 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId3 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId ='${item.UniqueId}'`,
              );

              blindid = connectId;
              if (connectId2) {
                blindid = `${blindid} AND ITEM ID ${connectId2}`;
              }
              if (connectId3) {
                blindid = `${blindid}, ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
              }
            }

            if (item.BlindNo === "Blind 4") {
              connectId = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId2 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
              );
              connectId3 = await getItemData(
                `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
              );

              blindid = connectId;
              if (connectId3) {
                blindid = `${blindid}, ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
              }
            }

            lblNextDesc.innerHTML = `This is the <b><u>${blinds}</u></b> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, bracket cover and bracket extension,  then the data on the <b><u>ITEM ID ${blindid}</u></b>  blind will automatically be changed according to this data.`;
          }
        }
      }

      if (ITEMACTION === "NextItem") {
        divAttention.classList.remove("d-none");

        let blinds = "second blind";
        if (item.BlindNo === "Blind 3") blinds = "third blind";
        if (item.BlindNo === "Blind 4") blinds = "fourth blind";

        let connectId = await getItemData(
          `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
        );
        let connectId2 = await getItemData(
          `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
        );
        let connectId3 = await getItemData(
          `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId ='${item.UniqueId}'`,
        );

        let blindid = connectId;
        if (connectId2) {
          blindid = `${blindid} AND ITEM ID ${connectId2}`;
        }
        if (connectId3) {
          blindid = `${blindid},ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
        }

        if (item.BlindNo === "Blind 3") {
          connectId = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
          );
          connectId2 = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
          );
          connectId3 = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId ='${item.UniqueId}'`,
          );
          blindid = `${connectId} ADD ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
        }

        if (item.BlindNo === "Blind 4") {
          connectId = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId ='${item.UniqueId}'`,
          );
          connectId2 = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId ='${item.UniqueId}'`,
          );
          connectId3 = await getItemData(
            `SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId ='${item.UniqueId}'`,
          );
          blindid = `${connectId} ADD ITEM ID ${connectId2} AND ITEM ID ${connectId3}`;
        }

        lblNextDesc.innerHTML = `This is the <b><u>${blinds}</b></u> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, then the data on the <b><u>ITEM ID ${connectId}</u></b>  blind will automatically be changed according to this data.`;
      }

      if (item.Trim === "1F") {
        divBottomRail.classList.remove("d-none");
      }
    }
    if (MARKUPACCESS === "True") divMarkUp.classList.remove("d-none");

    if (["AddItem", "NextItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
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
      "brackettype",
      "tubetype",
      "controltype",
      "colourtype",
      "qty",
      "room",
      "mounting",
      "width",
      "drop",
      "fabrictype",
      "fabriccolour",
      "motorstyle",
      "motorremote",
      "externalbattery",
      "charger",
      "cableexitpoint",
      "connector",
      "roll",
      "controlposition",
      "chaincolour",
      "trim",
      "railtype",
      "railcolour",
      "tubesize",
      "childsafe",
      "accessory",
      "extras",
      "bracketcovers",
      "bracketext",
      "notes",
      "markup",
    ];

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
      blindno: document.getElementById("lblBlindNo")?.innerHTML,
      uniqueid: document.getElementById("lblUniqueId")?.innerHTML,
    };

    fields.forEach((field) => {
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
    lblBlindNo: "BlindNo",
    lblUniqueId: "UniqueId",
    blindtype: "BlindId",
    brackettype: "BracketType",
    tubetype: "TubeType",
    controltype: "ControlType",
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    width: "Width",
    drop: "Drop",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    motorstyle: "MotorStyle",
    motorremote: "MotorRemote",
    externalbattery: "MotorBattery",
    charger: "MotorCharger",
    cableexitpoint: "CableExitPoint",
    connector: "Connector",
    roll: "RollDirection",
    controlposition: "ControlPosition",
    chaincolour: "ChainColour",
    chainlength: "ChainLength",
    trim: "Trim",
    railtype: "BottomType",
    railcolour: "BottomRailId",
    tubesize: "TubeSize",
    childsafe: "ChildSafe",
    accessory: "Accessory",
    extras: "AdditionalMotor",
    bracketcovers: "BracketCover",
    bracketext: "BracketExtension",
    notes: "Notes",
    markup: "MarkUp",
  };

  // Set nilai ke input sesuai mapping
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[key];
    if (id === "markup" && value === 0) value = "";

    el.value = value ?? ""; // fallback ke string kosong

    if (["lblBlindNo", "lblUniqueId"].includes(id)) {
      el.innerHTML = value;

      if (id === "lblBlindNo" && ITEMACTION === "NextItem") {
        if (
          value === "Blind 1" &&
          ["Double", "Linked 2 Blinds (Dep)", "Linked 2 Blinds (Ind)"].includes(
            itemData["BracketType"],
          )
        ) {
          el.innerHTML = "Blind 2";
        }

        if (
          ["Linked 3 Blinds (Dep)", "Linked 3 Blinds (Ind)"].includes(
            itemData["BracketType"],
          )
        ) {
          if (value === "Blind 1") {
            el.innerHTML = "Blind 2";
          }
          if (value === "Blind 2") {
            el.innerHTML = "Blind 3";
          }
        }

        if (
          ["Double and Link System Dep", "Double and Link System Ind"].includes(
            itemData["BracketType"],
          )
        ) {
          if (value === "Blind 1") {
            el.innerHTML = "Blind 2";
          }
          if (value === "Blind 2") {
            el.innerHTML = "Blind 3";
          }
          if (value === "Blind 3") {
            el.innerHTML = "Blind 4";
          }
        }
      }
    }

    // jika nilainya "0" → kosong
    if (el.value === "0") el.value = "";
  });
};
// ------------------------------------------------------|| Binding Functions ||--------------------------------------
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

  if (["NextItem", "EditItem", "ViewItem"].includes(itemaction)) {
    const blindtype = document.getElementById("blindtype");
    const brackettype = document.getElementById("brackettype");

    blindtype.setAttribute("disabled", true);
    brackettype.setAttribute("disabled", true);
  }
};

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

const bindBlinds = async () => {
  const select = document.getElementById("blindtype");
  select.innerHTML = "";

  if (!DESIGNID) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindBlindType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid: DESIGNID }),
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
        select.selectedIndex = 0;
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

const bindBrackets = async (designid, blindid) => {
  const select = document.getElementById("brackettype");
  select.innerHTML = "";

  if (!designid || !blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindBracketType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid }),
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
      throw new Error("No data returned from server : bindBrackets");
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

const bindTubes = async (designid, blindid, brackettype) => {
  const select = document.getElementById("tubetype");
  select.innerHTML = "";

  if (!designid || !blindid || !brackettype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindTubeType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid, brackettype }),
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
        const blindname =
          document.getElementById("blindtype").selectedOptions[0].dataset.name;
        await handlerElementVisibility(blindname, brackettype, select.value);
        await bindControls(designid, blindid, brackettype, select.value);
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

const bindControls = async (designid, blindid, brackettype, tubetype) => {
  const select = document.getElementById("controltype");
  select.innerHTML = "";

  if (!designid || !blindid || !brackettype || !tubetype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindControlType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid, brackettype, tubetype }),
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
      throw new Error("No data returned from server : bindControls");
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
        const blindname =
          document.getElementById("blindtype").selectedOptions[0].dataset.name;

        await handlerElementVisibility(
          blindname,
          brackettype,
          tubetype,
          select.value,
        );
        await bindColours(
          designid,
          blindid,
          brackettype,
          tubetype,
          select.value,
        );
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

const bindColours = async (
  designid,
  blindid,
  brackettype,
  tubetype,
  controltype,
) => {
  const select = document.getElementById("colourtype");
  select.innerHTML = "";

  if (!designid || !blindid || !brackettype || !tubetype || !controltype)
    return;

  try {
    const response = await fetch(`${URIMETHOD}/BindColourType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
        blindid,
        brackettype,
        tubetype,
        controltype,
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
      throw new Error("No data returned from server : bindColours");
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
          ` SELECT Name FROM Blinds WHERE Id = '${blindid}' AND Active=1 `,
        );
        if (!blindname) {
          throw new Error("Blind name not found : bindColours");
        }

        await bindFabrics(designid);
        if (blindname == "Motorised") {
          await Promise.all([
            bindMotorStyle(controltype),
            bindMotorRemote(controltype),
          ]);
        }
        await Promise.all([
          bindChains(designid),
          bindTrims(blindname, brackettype, tubetype),
          bindTubeSize(blindname, tubetype),
          bindChildSafe(),
          bindAccessory(),
        ]);
        await handlerElementVisibility(
          blindname,
          brackettype,
          tubetype,
          controltype,
          select.value,
        );
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

const bindFabrics = async (designid) => {
  const select = document.getElementById("fabrictype");
  document.getElementById("fabriccolour").innerHTML = "";
  select.innerHTML = "";

  if (!designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
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
      throw new Error("No data returned from server : bindFabrics");
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
        // bindControls(DESIGNID, select.value);
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

const bindFabricColours = async (designid, fabrictype) => {
  const select = document.getElementById("fabriccolour");
  select.innerHTML = "";

  if (!designid || !fabrictype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        designid,
        fabrictype,
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
      throw new Error("No data returned from server : bindFabricColours");
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
        // bindControls(DESIGNID, select.value);
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

const bindChains = () => {
  const sel = document.getElementById("chaincolour");
  sel.innerHTML = ""; //reset

  let data = [];
  data.push(
    { value: "Beige", text: "Beige" },
    { value: "Birch White", text: "Birch White" },
    { value: "Black", text: "Black" },
    { value: "Grey", text: "Grey" },
    { value: "Stainless Steel", text: "Stainless Steel" },
    { value: "White", text: "White" },
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

const bindMotorStyle = (controltype) => {
  const sel = document.getElementById("motorstyle");
  sel.innerHTML = ""; //reset

  if (!controltype) return;

  let data = [];
  if (controltype == "Somfy RTS") {
    data.push(
      { value: "Altus 40 RTS", text: "Altus 40 RTS" },
      { value: "Altus 50 RTS", text: "Altus 50 RTS" },
      { value: "Sonesse 40 RTS", text: "Sonesse 40 RTS" },
      { value: "Son 40 RTS ZB", text: "Son 40 RTS ZB" },
    );
  }

  if (controltype == "Somfy WF") {
    data.push(
      { value: "Altus 28 WF", text: "Altus 28 WF" },
      { value: "Altus 28 EXB", text: "Altus 28 EXB" },
      { value: "Son 28 WF ZB", text: "Son 28 WF ZB" },
      { value: "Son 28 WF ZBEXB", text: "Son 28 WF ZBEXB" },
      { value: "Sonesse 30 WF", text: "Sonesse 30 WF" },
      { value: "Sonesse 40 WF", text: "Sonesse 40 WF" },
      { value: "Son 40 WF ZB", text: "Son 40 WF ZB" },
    );
  }

  if (controltype == "Somfy WS") {
    data.push(
      { value: "Mecure LS 40", text: "Mecure LS 40" },
      { value: "Sonesse 40 WT", text: "Sonesse 40 WT" },
    );
  }

  if (controltype == "Alpha RTS") {
    data.push({ value: "WSER 40 Universal", text: "WSER 40 Universal" });
  }

  if (controltype == "Alpha WF") {
    data.push(
      { value: "Alpha 1NM Sml", text: "Alpha 1NM Sml" },
      { value: "Alpha 2NM Std", text: "Alpha 2NM Std" },
      { value: "Alpha 3NM HD", text: "Alpha 3NM HD" },
    );
  }

  if (controltype == "Alpha WS") {
    data.push(
      { value: "WSEC 40 Universal", text: "WSEC 40 Universal" },
      { value: "WSS40 Allen Key", text: "WSS40 Allen Key" },
    );
  }

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

const bindMotorRemote = (controltype) => {
  const sel = document.getElementById("motorremote");
  sel.innerHTML = ""; //reset

  if (!controltype) return;

  let data = [];
  if (controltype == "Somfy RTS" || controltype == "Somfy WF") {
    data.push(
      { value: "1 Situo (1 ch)", text: "1 Situo (1 ch)" },
      { value: "4 Situo (5ch)", text: "4 Situo (5ch)" },
      { value: "Telis (16 ch)", text: "Telis (16 ch)" },
    );
    if (controltype == "Somfy RTS") {
      data.push(
        { value: "Sm O (w+frame)", text: "Sm O (w+frame)" },
        { value: "Sm O 2ch (w+frame)", text: "Sm O 2ch (w+frame)" },
        { value: "Sm O 4ch (w+frame)", text: "Sm O 4ch (w+frame)" },
      );
    }
    data.push(
      { value: "Ysia ZB (1 ch)", text: "Ysia ZB (1 ch)" },
      { value: "Ysia ZB (5 ch)", text: "Ysia ZB (5 ch)" },
      { value: "Connexoon", text: "Connexoon" },
      { value: "Tahoma Wifi Box", text: "Tahoma Wifi Box" },
      { value: "E-Adaptor Tahoma", text: "E-Adaptor Tahoma" },
    );
  }

  if (controltype == "Somfy WS") {
    data.push(
      { value: "Sm Uno (+frame)", text: "Sm Uno (+frame)" },
      { value: "Sm Duo (+frame)", text: "Sm Duo (+frame)" },
      { value: "Triple Toggle Switch", text: "Triple Toggle Switch" },
    );
  }

  if (controltype == "Alpha RTS" || controltype == "Alpha WF") {
    data.push(
      { value: "Pioneer 1 Channel", text: "Pioneer 1 Channel" },
      { value: "Pioneer 4 Channels", text: "Pioneer 4 Channels" },
      { value: "Pioneer 16 Channels", text: "Pioneer 16 Channels" },
      { value: "Navigator 1 Channel", text: "Navigator 1 Channel" },
      { value: "Navigator 5 Channels", text: "Navigator 5 Channels" },
      { value: "Navigator 16 Channels", text: "Navigator 16 Channels" },
      { value: "1 Ch Wall", text: "1 Ch Wall" },
      { value: "8 Ch Wall", text: "8 Ch Wall" },
      { value: "Neo Link Box", text: "Neo Link Box" },
    );
  }

  if (controltype == "Alpha WS") {
    data.push(
      { value: "Mt Paddle (4c)", text: "Mt Paddle (4c)" },
      { value: "Neo Link Box", text: "Neo Link Box" },
    );
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

const bindExternalBattery = () => {
  const sel = document.getElementById("externalbattery");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push({ value: "Yes", text: "Yes" });

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

const bindMotorCharger = (controltype, motorstyle) => {
  const sel = document.getElementById("charger");
  sel.innerHTML = ""; //reset

  if (!controltype || !motorstyle) return;

  let data = [];
  if (controltype == "Somfy WF") {
    if (motorstyle.includes("ZB")) {
      data.push({ value: "USB-C", text: "USB-C" });
    } else {
      data.push({ value: "Yes", text: "Yes" });
    }
  }

  if (controltype == "Alpha WF") {
    if (motorstyle == "Alpha 1NM Sml") {
      data.push({ value: "Alpha", text: "Alpha" });
    }
    if (motorstyle == "Alpha 2NM Std") {
      data.push({ value: "Alpha 2NM (C)", text: "Alpha 2NM (C)" });
    }
    if (motorstyle == "Alpha 3NM HD") {
      data.push({ value: "Alpha 3NM (old)", text: "Alpha 3NM (old)" });
    }
  }

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

const bindTrims = (blindname, brackettype, tubetype) => {
  const sel = document.getElementById("trim");
  sel.innerHTML = ""; //reset

  if (!blindname || !brackettype || !tubetype) return;

  let data = [];
  data.push(
    { value: "Plain", text: "Plain" },
    { value: "Bottom Rail", text: "Bottom Rail" },
    { value: "Decorative", text: "Decorative" },
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

const bindRailType = async (brackettype, trim) => {
  const select = document.getElementById("railtype");
  document.getElementById("railcolour").innerHTML = "";
  select.innerHTML = "";

  if (!brackettype || !trim) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindRailType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        brackettype,
        trim,
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
      throw new Error("No data returned from server : bindRailType");
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
        // select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
        // bindControls(DESIGNID, select.value);
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

const bindRailColour = async (brackettype, railtype, trim) => {
  const select = document.getElementById("railcolour");
  select.innerHTML = "";

  if (!brackettype || !railtype || !trim) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindRailColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        brackettype,
        railtype,
        trim,
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
      throw new Error("No data returned from server : bindRailColour");
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
        // select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
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

const bindTubeSize = (blindname, tubetype) => {
  const sel = document.getElementById("tubesize");
  sel.innerHTML = ""; //reset

  if (!blindname || !tubetype) return;

  let data = [];
  if (blindname == "Gear Reduction") {
    if (
      tubetype == "JAI Standard" ||
      tubetype == "JAI Geared" ||
      tubetype == "LOV Standard" ||
      tubetype == "LOV Geared" ||
      tubetype == "Spring Operated"
    ) {
      data.push(
        { value: "40", text: "40" },
        { value: "45", text: "45" },
        { value: "45H", text: "45H" },
      );
    }
    if (tubetype == "Gear Reduction") {
      data.push(
        { value: "38", text: "38" },
        { value: "45", text: "45" },
        { value: "49", text: "49" },
      );
    }
  }

  if (blindname == "Skin Only") {
    data.push(
      { value: "40", text: "40" },
      { value: "45", text: "45" },
      { value: "45H", text: "45H" },
    );
  }

  if (blindname == "Cassette") {
    data.push(
      { value: "40", text: "40" },
      { value: "45", text: "45" },
      { value: "45H", text: "45H" },
    );
  }

  if (blindname == "Motorised") {
    switch (tubetype) {
      case "45 JAI":
      case "45 LOV":
        data.push({ value: "45", text: "45" });
        break;
      case "45H JAI":
        data.push({ value: "45H", text: "45H" });
        break;
      case "63 Acmeda":
        data.push({ value: "63", text: "63" });
        break;
    }
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

const bindChildSafe = () => {
  const sel = document.getElementById("childsafe");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Clear Loop (Standard)", text: "Clear Loop (Standard)" },
    { value: "Black - Deluxe", text: "Black - Deluxe" },
    { value: "Grey - Deluxe", text: "Grey - Deluxe" },
    { value: "Birch White - Deluxe", text: "Birch White - Deluxe" },
    { value: "White - Deluxe", text: "White - Deluxe" },
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

const bindAccessory = () => {
  const sel = document.getElementById("accessory");
  sel.innerHTML = ""; //reset

  let data = [];

  data.push(
    { value: "Crochet Ring Pull", text: "Crochet Ring Pull" },
    { value: "Metal Ring Pull", text: "Metal Ring Pull" },
    { value: "Tassle Pull", text: "Tassle Pull" },
    { value: "Plastic Ring & Tab", text: "Plastic Ring & Tab" },
    { value: "Timber Ring & Tab", text: "Timber Ring & Tab" },
    { value: "Silver Ring", text: "Silver Ring" },
    { value: "Gold Ring", text: "Gold Ring" },
    { value: "Match Metal Ring", text: "Match Metal Ring" },
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

const bindExtras = (blindname, controltype, motorstyle) => {
  const sel = document.getElementById("extras");
  sel.innerHTML = ""; //reset

  if (!blindname || !controltype || !motorstyle) return;

  let data = [];

  if (controltype.includes("Somfy")) {
    if (controltype === "Somfy WF" && !motorstyle.includes("ZB")) {
      data.push(
        { value: "WF Li Solar Panel Kit", text: "WF Li Solar Panel Kit" },
        { value: "Adaptor Mg V2 Li", text: "Adaptor Mg V2 Li" },
        { value: "Cable Mg Rigid", text: "Cable Mg Rigid" },
      );
    }

    if (blindname === "Cassette") {
      data.push({
        value: "Cable Ex 20cm Cassette",
        text: "Cable Ex 20cm Cassette",
      });
    }

    if (motorstyle.includes("ZB")) {
      if (controltype === "Somfy WF") {
        data.push({
          value: "WF Li ZB Solar Panel Kit",
          text: "WF Li ZB Solar Panel Kit",
        });
      }
      data.push(
        { value: "Cable ZB Ex 20cm USB-C", text: "Cable ZB Ex 20cm USB-C" },
        {
          value: "Adaptor Mg ZB USB-C Charger",
          text: "Adaptor Mg ZB USB-C Charger",
        },
        {
          value: "Cable Mg Rg ZB USB-C Charger",
          text: "Cable Mg Rg ZB USB-C Charger",
        },
      );
    }
  }

  if (controltype.includes("Alpha")) {
    data.push({
      value: "Lead Ex 3M ALDC Charger",
      text: "Lead Ex 3M ALDC Charger",
    });
  }

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
      await bindBrackets(item.DesignId, item.BlindId);
      await bindTubes(item.DesignId, item.BlindId, item.BracketType);
      await bindControls(
        item.DesignId,
        item.BlindId,
        item.BracketType,
        item.TubeType,
      );
      await bindColours(
        item.DesignId,
        item.BlindId,
        item.BracketType,
        item.TubeType,
        item.ControlType,
      );
      await bindFabrics(item.DesignId);
      await bindFabricColours(item.DesignId, item.FabricType);
      if (item.BlindName == "Motorised") {
        await Promise.all([
          bindMotorStyle(item.ControlType),
          bindMotorRemote(item.ControlType),
          bindExternalBattery(),
          bindMotorCharger(item.ControlType, item.MotorStyle),
          bindExtras(item.BlindName, item.ControlType, item.MotorStyle),
        ]);
      }
      await Promise.all([
        bindChains(item.DesignId),
        bindTrims(item.BlindName, item.BracketType, item.TubeType),
      ]);
      await bindRailType(item.BracketType, item.Trim);
      await bindRailColour(item.BracketType, item.BottomType, item.Trim);
      await Promise.all([
        bindTubeSize(item.BlindName, item.TubeType),
        bindChildSafe(),
        bindAccessory(),
        handlerSetElementValues(item),
      ]);
      await handlerElementVisibility(
        item.BlindName,
        item.BracketType,
        item.TubeType,
        item.ControlType,
        item.ColourType,
        item,
      );
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};
// ------------------------------------------------------|| Other Functions ||--------------------------------------
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
