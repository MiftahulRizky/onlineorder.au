document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Softshades.js loaded successfully");
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
// ==========================================================INITIALIZATION=============================================================
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const selectorEl = (el) => document.querySelector(el);
const selectorElAll = (el) => document.querySelectorAll(el);

const elForm = {
  lblItemId: getById("lblItemId"),
  lblBlindNo: getById("lblBlindNo"),
  lblUniqueId: getById("lblUniqueId"),
  lblBracketType: getById("lblBracketType"),
  divBracketType: getById("divBracketType"),
  divTubeType: getById("divTubeType"),
  divControlType: getById("divControlType"),
  btnInfoControlType: getById("btnInfoControlType"),
  lblColourType: getById("lblColourType"),
  divColourType: getById("divColourType"),
  divFormDetail: getById("divFormDetail"),
  divAttention: getById("divAttention"),
  lblNextDesc: getById("lblNextDesc"),
  divMotorStyle: getById("divMotorStyle"),
  btnInfoMotorStyle: getById("btnInfoMotorStyle"),
  divMotorRemote: getById("divMotorRemote"),
  btnInfoMotorRemote: getById("btnInfoMotorRemote"),
  divMotorBattery: getById("divMotorBattery"),
  divMotorCharger: getById("divMotorCharger"),
  divCableExitPoint: getById("divCableExitPoint"),
  divConnector: getById("divConnector"),
  divRoll: getById("divRoll"),
  divControlPosition: getById("divControlPosition"),
  lblControlPosition: getById("lblControlPosition"),
  btnInfoControlPosition: getById("btnInfoControlPosition"),
  lblIndBlind: getById("lblIndBlind"),
  divChain: getById("divChain"),
  divBottomRail: getById("divBottomRail"),
  lblBotomRail: getById("lblBotomRail"),
  btnInfoTrim: getById("btnInfoTrim"),
  divRailColour: getById("divRailColour"),
  divTubeSize: getById("divTubeSize"),
  divAdditional: getById("divAdditional"),
  divChildSafe: getById("divChildSafe"),
  divAccessory: getById("divAccessory"),
  divExtras: getById("divExtras"),
  divBracketCover: getById("divBracketCover"),
  divBracketExt: getById("divBracketExt"),
  divMarkUp: getById("divMarkUp"),
  btnSubmit: selectorEl("#btnSubmit"),
};

// ==========================================================EVENTS=====================================================================

// ==========================================================FUNCTIONS==================================================================
// ----------------------------------------------|| Binding Functions ||---------------------------------------
const bindFormAggregate = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/BindFormAggregate`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          headerid: HEADERID,
          ordertype: ORDERTYPE,
          designid: DESIGNID,
          itemid: ITEMID,
          itemaction: ITEMACTION,
        },
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const { d: data } = await response.json();

    if (!data) {
      throw new Error("No data");
    }

    if (data.error) {
      throw new Error(data.message);
    }

    console.log(data);

    bindPageTitle(data.DesignName);
    bindHeaderInfo(data.HeaderData);
    bindActionInfo(ITEMACTION);
    bindSelect({
      data: data.Blinds,
      elementId: "blindtype",
      withDefaultOption: true,
      lengthDefaultOption: 0,
    });
  } catch (error) {
    const msg = `bindFormAggregate: ${error.message}`;
    catchMessages(msg);
  }
};

const bindPageTitle = (name) => {
  try {
    document.getElementById("pageTitle").innerHTML = name;
    document.getElementById("pageAction").innerHTML = ITEMACTION;
  } catch (error) {
    const msg = `bindPageTitle: ${error.message}`;
    catchMessages(msg);
  }
};

const bindHeaderInfo = (header) => {
  try {
    const lblOrder = document.getElementById("lblOrder");
    const lblItemId = document.getElementById("lblItemId");
    const lblOrderNumber = document.getElementById("lblOrderNumber");
    const lblOrderName = document.getElementById("lblOrderName");

    lblOrder.innerHTML = header.OrderId;
    lblOrder.classList.add("fw-bold");

    lblItemId.innerHTML = ITEMID;
    lblItemId.classList.add("fw-bold");

    lblOrderNumber.innerHTML = header.OrderNumber;
    lblOrderNumber.classList.add("fw-bold");

    lblOrderName.innerHTML = header.OrderName;
    lblOrderName.classList.add("fw-bold");
  } catch (error) {
    const msg = `bindHeaderInfo: ${error.message}`;
    catchMessages(msg);
  }
};

const bindActionInfo = (itemaction) => {
  try {
    const cardTitle = getById("cardTitle");
    const actionMap = {
      AddItem: "ADD ITEM",
      NextItem: "NEXT ITEM",
      EditItem: "EDIT ITEM ID: " + ITEMID,
      ViewItem: "VIEW ITEM ID: " + ITEMID,
      CopyItem: "COPY ITEM",
    };
    cardTitle.innerText = actionMap[itemaction] || "";

    if (["NextItem", "EditItem", "ViewItem"].includes(itemaction)) {
      const blindtype = getById("blindtype");
      const brackettype = getById("brackettype");

      blindtype.setAttribute("disabled", true);
      brackettype.setAttribute("disabled", true);
    }
  } catch (error) {
    const msg = `bindActionInfo: ${error.message}`;
    catchMessages(msg);
  }
};

const bindSelect = ({
  data,
  elementId,
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  const select = document.getElementById(elementId);
  select.innerHTML = "";

  try {
    // default option
    if (withDefaultOption && data.length > lengthDefaultOption) {
      const opt = document.createElement("option");
      opt.value = "";
      opt.text = "";
      select.add(opt);
    }

    // render options
    data.forEach((item) => {
      const option = document.createElement("option");
      option.value = item.value;
      option.text = item.text.toUpperCase();
      option.setAttribute("data-name", item.text);
      select.add(option);
    });

    select.classList.add("fw-bold");

    // callback setelah render
    if (afterRender) {
      afterRender(data, select);
    }

    // kalau cuma 1 data
    if (data.length === 1 && onSingle) {
      select.selectedIndex = 0;
      onSingle(data[0], select);
    }
  } catch (err) {
    const msg = `bindSelect: ${err.message}`;
    catchMessages(msg);
  }
};

const bindListData = async ({
  elementId,
  field,
  params = {},
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field,
          ...params,
        },
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      throw new Error(`${response.status}\n${text}`);
    }

    const result = await response.json();
    const data = result.d.list;

    if (!Array.isArray(data)) {
      throw new Error(`No data returned from server : ${field}`);
    }

    bindSelect({
      data,
      elementId,
      withDefaultOption,
      lengthDefaultOption,
      onSingle,
      afterRender,
    });
  } catch (err) {
    const msg = `bindListData: ${err.message}`;
    catchMessages(msg);
  }
};

const bindSelectPayload = (data, el, def = true, leng = 0) => {
  try {
    bindSelect({
      data: data,
      elementId: el,
      withDefaultOption: def,
      lengthDefaultOption: leng,
    });
  } catch (error) {
    const msg = `bindSelectPayload: ${error.message}`;
    catchMessages(msg);
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
    const btnInfoControlPosition = document.getElementById(
      "btnInfoControlPosition",
    );
    const lblIndBlind = document.getElementById("lblIndBlind");
    const divChain = document.getElementById("divChain");
    const divBottomRail = document.getElementById("divBottomRail");
    const lblBotomRail = document.getElementById("lblBotomRail");
    const btnInfoTrim = document.getElementById("btnInfoTrim");
    const divRailColour = document.getElementById("divRailColour");
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
    lblIndBlind.classList.add("d-none");
    btnInfoControlPosition.classList.add("d-none");
    divChain.classList.add("d-none");
    divBottomRail.classList.add("d-none");
    lblBotomRail.innerHTML = "bottom rail type x colour";
    btnInfoTrim.classList.add("d-none");
    divRailColour.classList.add("d-none");
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

    if (["Gear Reduction"].includes(blindname)) {
      lblColourType.innerHTML = "colour type";
    }

    if (["Gear Reduction"].includes(blindname)) {
      divAdditional.classList.remove("d-none");
    }

    divBracketType.classList.remove("d-none");

    // ---------------------------------|| on change brackettype ||---------------------------------
    if (!brackettype) return;
    if (["Gear Reduction"].includes(blindname)) {
      divTubeType.classList.remove("d-none");
    }

    if (["Linked 2 Blinds (Dep)"].includes(brackettype)) {
      btnInfoControlPosition.classList.remove("d-none");
    }

    if (["Linked 3 Blinds (Dep)"].includes(brackettype)) {
      if (["Blind 1", "Blind 3"].includes(lblBlindNo.innerHTML)) {
        btnInfoControlPosition.classList.remove("d-none");
      }
    }

    if (["Linked 3 Blinds (Ind)"].includes(brackettype)) {
      if (["Blind 2"].includes(lblBlindNo.innerHTML)) {
        btnInfoControlPosition.classList.remove("d-none");
      }
    }

    // ---------------------------------|| on change tubetype ||---------------------------------
    if (!tubetype) return;

    if (["Gear Reduction"].includes(blindname)) {
      divControlType.classList.remove("d-none");
    }

    // ---------------------------------|| on change controltype ||---------------------------------
    if (!controltype) return;
    if (["Gear Reduction"].includes(blindname)) {
      divColourType.classList.remove("d-none");
    }

    // ---------------------------------|| on change colourtype ||---------------------------------
    if (!colourtype) return;
    divFormDetail.classList.remove("d-none");

    if (blindname === "Gear Reduction") {
      divRoll.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      divChain.classList.remove("d-none");
      divBracketCover.classList.remove("d-none");
      if (brackettype === "Double") {
        divBracketExt.classList.add("d-none");
      }
      // divTubeSize.classList.remove("d-none");
      divChildSafe.classList.remove("d-none");
      // divAccessory.classList.remove("d-none");

      if (["Linked 3 Blinds (Ind)"].includes(brackettype)) {
        if (["Blind 2"].includes(lblBlindNo.innerHTML)) {
          divChain.classList.add("d-none");
        }
      }
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

      if (["Bottom Rail", "Decorative"].includes(item.Trim)) {
        lblBotomRail.innerHTML = item.Trim;
        divBottomRail.classList.remove("d-none");
        if (item.Trim == "Bottom Rail") {
          lblBotomRail.innerHTML = "bottom rail type x colour";
          divRailColour.classList.remove("d-none");
        }
        if (item.Trim == "Decorative") {
          btnInfoTrim.classList.remove("d-none");
        }
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
    const msg = `handlerElementVisibility: ${error.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
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

  await bindFormAggregate();
  if (ITEMACTION === "AddItem") {
    // elForm.lblBlindNo.innerHTML = "Blind 1";
    // await handlerElementVisibility();
    loaderFadeOut();
  } else if (
    ["NextItem", "EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)
  ) {
    // await bindItemOrders();
    loaderFadeOut();
  }
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  if (!["trim"].includes(elementId)) {
    list.sort();
  }

  // default option kalau lebih dari 1 data
  if (list.length > lengthDefaultOption) {
    const defaultOption = new Option("", "");
    sel.add(defaultOption);
  }

  list.forEach((item) => {
    const option = new Option(item.toUpperCase(), item);
    option.setAttribute("data-name", item);
    sel.add(option);
  });
};

const toggleShow = (el, show) => {
  if (!el) return;
  el.classList.toggle("d-none", !show);
};

const toggleShowList = (keys, show) => {
  keys.forEach((key) => {
    if (liEl[key]) {
      Array.from(liEl[key]).forEach((li) =>
        li.classList.toggle("d-none", !show),
      );
    }
  });
};

const isConfirm = async (message) => {
  return new Promise((resolve) => {
    Swal.fire({
      title: "Confirmation !",
      html: message,
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Yes",
      cancelButtonText: "No",
      customClass: {
        popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
      },
    }).then((result) => {
      resolve(result.isConfirmed);
    });
  });
};

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
