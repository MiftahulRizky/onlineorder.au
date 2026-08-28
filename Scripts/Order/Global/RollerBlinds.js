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
selectorElAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      const blindname = e.target.selectedOptions[0].dataset.name;
      await handlerElementVisibility(blindname);
      await bindBrackets(DESIGNID, blindtype);
    }

    if (e.target.id === "brackettype") {
      const blinds = getById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const brackettype = e.target.value;
      await handlerElementVisibility(blindname, brackettype);
      await bindTubes(DESIGNID, blindtype, brackettype);
    }

    if (e.target.id === "tubetype") {
      const blinds = getById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const brackettype = getById("brackettype").value;
      const tubetype = e.target.value;
      await handlerElementVisibility(blindname, brackettype, tubetype);
      await bindControls(DESIGNID, blindtype, brackettype, tubetype);
    }

    if (e.target.id === "controltype") {
      alert(e.target.value);
      const blinds = getById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const brackettype = getById("brackettype").value;
      const tubetype = getById("tubetype").value;
      const controltype = e.target.value;
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
      );
      await bindColours(
        DESIGNID,
        blindtype,
        brackettype,
        tubetype,
        controltype,
      );
    }

    if (e.target.id === "colourtype") {
      const blinds = getById("blindtype");
      const blindname = blinds.options[blinds.selectedIndex].dataset.name;
      const brackettype = getById("brackettype").value;
      const tubetype = getById("tubetype").value;
      const controltype = getById("controltype").value;
      const colourtype = e.target.value;
      const width = getById("width").value;
      const drop = getById("drop").value;

      await bindFabrics(DESIGNID);
      await Promise.all([
        bindMounting(),
        bindRoll(),
        bindControlPosition(),
        bindChains(),
        bindTrims(),
        bindChildSafe(),
        bindAccessory(),
        bindBracketCover(),
      ]);
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
        colourtype,
      );
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    if (e.target.id === "trim") {
      toggleShow(elForm.divBottomRail, false);
      toggleShow(elForm.btnInfoTrim, false);
      toggleShow(elForm.divAccessory, false);
      toggleShow(elForm.divRailColour, false);
      toggleShow(elForm.lblBotomRail, false);
      elForm.lblBotomRail.innerHTM = "bottom rail type x colour";

      if (!e.target.value) return;
      const blindtype = getById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const brackettype = getById("brackettype").value;
      const trim = e.target.value;

      await bindRailType(brackettype, trim);
      if (
        ["Gear Reduction"].includes(blindname) &&
        ["Bottom Rail", "Decorative"].includes(trim)
      ) {
        toggleShow(elForm.divBottomRail, true);
        toggleShow(elForm.lblBotomRail, true);
        elForm.lblBotomRail.innerHTML = "Decorative Trim";
        if (trim == "Bottom Rail") {
          elForm.lblBotomRail.innerHTML = "bottom rail type x colour";
          toggleShow(elForm.divRailColour, true);
        }
        if (trim == "Decorative") {
          toggleShow(elForm.btnInfoTrim, true);
          // divRailColour.classList.remove("d-none");
        }
        toggleShow(elForm.divAccessory, true);
      }
    }

    if (e.target.id === "railtype") {
      const brackettype = getById("brackettype").value;
      const railtype = e.target.value;
      const trim = getById("trim").value;

      await bindRailColour(brackettype, railtype, trim);
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");

    // if (e.target.id === "width") {
    //   const blinds = document.getElementById("blindtype");
    //   const blindname = blinds.selectedOptions[0].dataset.name;
    //   const tubetype = document.getElementById("tubetype").value;
    //   const width = e.target.value;
    //   const drop = document.getElementById("drop").value;
    //   bindTubeSize(blindname, tubetype, width, drop);
    // }

    // if (e.target.id === "drop") {
    //   const blinds = document.getElementById("blindtype");
    //   const blindname = blinds.selectedOptions[0].dataset.name;
    //   const tubetype = document.getElementById("tubetype").value;
    //   const width = document.getElementById("width").value;
    //   const drop = e.target.value;
    //   bindTubeSize(blindname, tubetype, width, drop);
    // }

    if (e.target.id === "notes") {
      let maxLength = 1000;
      let currentLength = e.target.value.length;
      document.querySelector("#notescount").textContent =
        `${currentLength}/${maxLength}`;
    }
  });
});

selectorEl("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  selectorElAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  // handlerSubmit(e.target.form, e.target.id);
  handlerSubmit(e.target.id);
});

const btnInfo = selectorElAll(".btn-information");
if (btnInfo) {
  btnInfo.forEach((el) => {
    el.addEventListener("click", (e) => {
      try {
        let text = "";
        let id = e.currentTarget.id;

        switch (id) {
          case "btnInfoControlType":
            const blind = selectorEl("#blindtype");
            const blindName = blind.options[blind.selectedIndex].dataset.name;
            const tubetype = selectorEl("#tubetype").value;

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
            text = "Trim";
            break;
          case "btnInfoChain":
            text =
              "If you enter 0 or leave blank, the system will automatically apply the factory default (standard chain length)";
            break;
          case "btnInfoTubeSize":
            text = "TubeSize";
            break;
          case "btnInfoControlPosition":
            const brackettype = selectorEl("#brackettype").value;
            const lblBlindNo = getById("lblBlindNo");
            if (["Linked 2 Blinds (Dep)"].includes(brackettype)) {
              text =
                "Linked 2 Dependent: Control allowed only on Blind 1 (Blind 2 empty) or Blind 2 (Blind 1 empty).";
            }

            if (["Linked 3 Blinds (Dep)"].includes(brackettype)) {
              if (["Blind 1", "Blind 3"].includes(lblBlindNo.innerHTML)) {
                text =
                  "Linked 3 Dependent: Control allowed only on Blind 1 (Blind 2 & 3 empty) or Blind 3 (Blind 1 & 2 empty).";
              }
            }

            if (["Linked 3 Blinds (Ind)"].includes(brackettype)) {
              if (["Blind 2"].includes(lblBlindNo.innerHTML)) {
                text =
                  "If Blind 2 has the same control side as Blind 1, then Blind 2 is dependent on Blind 1. If Blind 2 has the same control side as Blind 3 (meaning Line 3 is opposite to Blind 1), then Blind 2 is dependent on Blind 3.";
              }
            }

            break;
        }

        if (text && !["Trim", "TubeSize"].includes(text)) {
          isInfo(text);
        }

        switch (text) {
          case "Trim":
            handlerShowBSModal("modalInfoTrim");
            break;

          case "TubeSize":
            handlerShowBSModal("modalInfoTube");
            break;

          default:
            break;
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
selectorEl("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
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

    // console.log(data);

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

const bindBrackets = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindListData({
    elementId: "brackettype",
    field: "brackettype",
    params: { designid, blindtype },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindTubes = async (designid, blindtype, brackettype) => {
  if (!designid || !blindtype || !brackettype) return;

  await bindListData({
    elementId: "tubetype",
    field: "tubetype",
    params: { designid, blindtype, brackettype },
    withDefaultOption: true,
    lengthDefaultOption: 1,

    onSingle: async (item, select) => {
      const blinds = getById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = item.value;
      await handlerElementVisibility(blindname, brackettype, tubetype);
      await bindControls(designid, blindtype, brackettype, tubetype);
    },
  });
};

const bindControls = async (designid, blindtype, brackettype, tubetype) => {
  if (!designid || !blindtype || !brackettype || !tubetype) return;

  await bindListData({
    elementId: "controltype",
    field: "controltype",
    params: { designid, blindtype, brackettype, tubetype },
    withDefaultOption: true,
    lengthDefaultOption: 1,

    onSingle: async (item, select) => {
      const blinds = getById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controltype = item.value;
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
      );
      await bindColours(
        designid,
        blindtype,
        brackettype,
        tubetype,
        controltype,
      );
    },
  });
};

const bindColours = async (
  designid,
  blindtype,
  brackettype,
  tubetype,
  controltype,
) => {
  if (!designid || !blindtype || !brackettype || !tubetype || !controltype)
    return;

  await bindListData({
    elementId: "colourtype",
    field: "colourtype",
    params: { designid, blindtype, brackettype, tubetype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 0,

    onSingle: async (item, select) => {
      const blinds = getById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const colourtype = item.value;
      const width = getById("width").value;
      const drop = getById("drop").value;

      // await bindFabrics(designid);
      // if (blindname == "Motorised") {
      //   await Promise.all([
      //     bindMotorStyle(controltype),
      //     bindMotorRemote(controltype),
      //   ]);
      // }
      // await Promise.all([
      //   bindChains(designid),
      //   bindTrims(blindname, brackettype, tubetype),
      //   bindTubeSize(blindname, tubetype, width, drop),
      //   bindChildSafe(),
      //   bindAccessory(),
      // ]);
      await handlerElementVisibility(
        blindname,
        brackettype,
        tubetype,
        controltype,
        colourtype,
      );
    },
  });
};

const bindMounting = () => {
  generateOption("mounting", ["Face Fit", "Reveal Fit"]);
};

const bindFabrics = async (designid) => {
  getById("fabriccolour").innerHTML = "";
  if (!designid) return;

  await bindListData({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindFabricColours = async (designid, fabrictype) => {
  if (!designid || !fabrictype) return;

  await bindListData({
    elementId: "fabriccolour",
    field: "fabriccolour",
    params: { designid, fabrictype },
    withDefaultOption: true,
    lengthDefaultOption: 1,
  });
};

const bindMotorStyle = (controltype) => {
  if (!controltype) return;
  let list = [];

  if (controltype == "Somfy RTS") {
    list.push(
      "Altus 40 RTS",
      "Altus 50 RTS",
      "Sonesse 40 RTS",
      "Son 40 RTS ZB",
    );
  }

  if (controltype == "Somfy WF") {
    list.push(
      "Altus 28 WF",
      "Altus 28 EXB",
      "Son 28 WF ZB",
      "Son 28 WF ZBEXB",
      "Sonesse 30 WF",
      "Sonesse 40 WF",
      "Son 40 WF ZB",
    );
  }

  if (controltype == "Somfy WS") {
    list.push("Mecure LS 40", "Sonesse 40 WT");
  }

  if (controltype == "Alpha RTS") {
    list.push("WSER 40 Universal");
  }

  if (controltype == "Alpha WF") {
    list.push("Alpha 1NM Sml", "Alpha 2NM Std", "Alpha 3NM HD");
  }

  if (controltype == "Alpha WS") {
    list.push("WSEC 40 Universal", "WSS40 Allen Key");
  }

  generateOption("motorstyle", list);
};

const bindMotorRemote = (controltype) => {
  if (!controltype) return;
  let list = [];

  if (controltype == "Somfy RTS" || controltype == "Somfy WF") {
    list.push("1 Situo (1 ch)", "4 Situo (5ch)", "Telis (16 ch)");

    if (controltype == "Somfy RTS") {
      list.push("Sm O (w+frame)", "Sm O 2ch (w+frame)", "Sm O 4ch (w+frame)");
    }
    list.push(
      "Ysia ZB (1 ch)",
      "Ysia ZB (5 ch)",
      "Connexoon",
      "Tahoma Wifi Box",
      "E-Adaptor Tahoma",
    );
  }

  if (controltype == "Somfy WS") {
    list.push("Sm Uno (+frame)", "Sm Duo (+frame)", "Triple Toggle Switch");
  }

  if (controltype == "Alpha RTS" || controltype == "Alpha WF") {
    list.push(
      "Pioneer 1 Channel",
      "Pioneer 4 Channels",
      "Pioneer 16 Channels",
      "Navigator 1 Channel",
      "Navigator 5 Channels",
      "Navigator 16 Channels",
      "1 Ch Wall",
      "8 Ch Wall",
      "Neo Link Box",
    );
  }

  if (controltype == "Alpha WS") {
    list.push("Mt Paddle (4c)", "Neo Link Box");
  }

  generateOption("motorremote", list);
};

const bindExternalBattery = () => {
  generateOption("externalbattery", ["Yes"], 1);
};

const bindMotorCharger = (controltype, motorstyle) => {
  if (!controltype || !motorstyle) return;
  let list = [];
  if (controltype == "Somfy WF") {
    if (motorstyle.includes("ZB")) {
      list.push("USB-C");
    } else {
      list.push("Yes");
    }
  }
  if (controltype == "Alpha WF") {
    if (motorstyle == "Alpha 1NM Sml") {
      list.push("Alpha");
    }
    if (motorstyle == "Alpha 2NM Std") {
      list.push("Alpha 2NM (C)");
    }
    if (motorstyle == "Alpha 3NM HD") {
      list.push("Alpha 3NM (old)");
    }
  }
  generateOption("charger", list);
};

const bindCableExitPoint = () => {
  generateOption("cableexitpoint", ["Side", "Top"]);
};

const bindConnector = () => {
  generateOption("connector", ["Yes"]);
};

const bindRoll = () => {
  generateOption("roll", ["Reverse Roll", "Standard"]);
};

const bindControlPosition = () => {
  generateOption("controlposition", ["Left", "Right"]);
};

const bindChains = () => {
  generateOption("chaincolour", [
    "Chrome",
    "Birch White",
    "Black",
    "Grey",
    "Stainless Steel",
    "White",
  ]);
};

const bindTrims = () => {
  generateOption("trim", ["Plain", "Bottom Rail", "Decorative"]);
};

const bindRailType = async (brackettype, trim) => {
  getById("railcolour").innerHTML = "";
  if (!brackettype || !trim) return;

  await bindListData({
    elementId: "railtype",
    field: "railtype",
    params: { brackettype, trim },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindRailColour = async (brackettype, railtype, trim) => {
  if (!brackettype || !railtype || !trim) return;

  await bindListData({
    elementId: "railcolour",
    field: "railcolour",
    params: { brackettype, railtype, trim },
    withDefaultOption: true,
    lengthDefaultOption: 1,
  });
};

const bindChildSafe = () => {
  generateOption("childsafe", ["Clear Loop (Standard)"]);
};

const bindAccessory = () => {
  generateOption("accessory", [
    "Crochet Ring Pull",
    "Metal Ring Pull",
    "Tassle Pull",
    "Plastic Ring & Tab",
    "Timber Ring & Tab",
    "Silver Ring",
    "Gold Ring",
    "Match Metal Ring",
  ]);
};

const bindBracketCover = () => {
  generateOption("bracketcovers", ["Yes"]);
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

const bindItemOrders = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/BindItemOrder`, {
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

    const { d: data } = await response.json();

    if (!data) {
      throw new Error("No data");
    }

    if (data.error) {
      throw new Error(data.message);
    }
    // console.log(data);
    const detail = data.DetailData;
    bindSelectPayload(data.Brackets, "brackettype", true, 1);
    bindSelectPayload(data.Tubes, "tubetype", true, 1);
    bindSelectPayload(data.Controls, "controltype", true, 1);
    bindSelectPayload(data.Colours, "colourtype", true, 1);
    bindSelectPayload(data.Fabrics, "fabrictype", true, 1);
    bindSelectPayload(data.FabricColours, "fabriccolour", true, 1);
    bindSelectPayload(data.Rails, "railtype", true, 1);
    bindSelectPayload(data.RailColours, "railcolour", true, 1);

    await Promise.all([
      bindMounting(),
      bindRoll(),
      bindControlPosition(),
      bindChains(),
      bindTrims(),
      bindChildSafe(),
      bindAccessory(),
      bindBracketCover(),
      handlerSetElementValues(detail),
    ]);

    await handlerElementVisibility(
      detail.BlindName,
      detail.BracketType,
      detail.TubeType,
      detail.ControlType,
      detail.ColourType,
      detail,
    );
  } catch (err) {
    const msg = `bindItemOrders: ${err.message}`;
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
    Object.values(elForm).forEach((el) => toggleShow(el, false));
    const isGearReduction = blindname === "Gear Reduction";
    const isDouble = brackettype === "Double";
    const isLinked2Ind = brackettype === "Linked 2 Blinds (Ind)";
    const isLinked2Dep = brackettype === "Linked 2 Blinds (Dep)";
    const isLinked3Ind = brackettype === "Linked 3 Blinds (Ind)";
    const isLinked3Dep = brackettype === "Linked 3 Blinds (Dep)";
    const isBlind1 = elForm.lblBlindNo.innerHTML === "Blind 1";
    const isBlind2 = elForm.lblBlindNo.innerHTML === "Blind 2";
    const isBlind3 = elForm.lblBlindNo.innerHTML === "Blind 3";
    const isBlind4 = elForm.lblBlindNo.innerHTML === "Blind 4";

    if (ROLENAME === "Administrator" && LEVELNAME === "Super Admin") {
      // lblItemId.classList.remove("d-none");
      // lblBlindNo.classList.remove("d-none");
      // lblUniqueId.classList.remove("d-none");
    }

    if (!blindname) return;
    if (isGearReduction) {
      elForm.lblColourType.innerHTML = "colour type";
      toggleShow(elForm.lblColourType, true);
      toggleShow(elForm.divAdditional, true);
    }
    toggleShow(elForm.lblBracketType, true);
    toggleShow(elForm.divBracketType, true);

    if (!brackettype) return;
    if (isGearReduction) {
      toggleShow(elForm.divTubeType, true);
    }

    if (isLinked2Dep) {
      toggleShow(elForm.btnInfoControlPosition, true);
    }

    if (isLinked3Dep) {
      if (isBlind1 || isBlind3) {
        toggleShow(elForm.btnInfoControlPosition, true);
      }
    }

    if (isLinked3Ind) {
      if (isBlind2) {
        toggleShow(elForm.btnInfoControlPosition, true);
      }
    }

    if (!tubetype) return;
    if (isGearReduction) {
      toggleShow(elForm.divControlType, true);
    }

    if (!controltype) return;
    if (isGearReduction) {
      toggleShow(elForm.divColourType, true);
    }

    if (!colourtype) return;
    toggleShow(elForm.divFormDetail, true);

    if (isGearReduction) {
      toggleShow(elForm.divRoll, true);
      toggleShow(elForm.lblControlPosition, true);
      toggleShow(elForm.divControlPosition, true);
      toggleShow(elForm.divChain, true);
      toggleShow(elForm.divBracketCover, true);
      if (isDouble) {
        toggleShow(elForm.divBracketExt, false);
      }
      // divTubeSize.classList.remove("d-none");
      toggleShow(elForm.divChildSafe, true);
      // divAccessory.classList.remove("d-none");

      if (isLinked3Ind) {
        if (isBlind2) {
          toggleShow(elForm.divChain, true);
        }
      }
    }

    if (item) {
      toggleShow(elForm.divAttention, item.NextDescVisible);
      toggleShow(elForm.lblNextDesc, item.NextDescVisible);
      elForm.lblNextDesc.innerHTML = item.NextDescText;

      if (["Bottom Rail", "Decorative"].includes(item.Trim)) {
        elForm.lblBotomRail.innerHTML = item.Trim;
        toggleShow(elForm.lblBotomRail, true);
        toggleShow(elForm.divBottomRail, true);
        if (item.Trim == "Bottom Rail") {
          elForm.lblBotomRail.innerHTML = "bottom rail type x colour";
          toggleShow(elForm.divRailColour, true);
        }
        if (item.Trim == "Decorative") {
          toggleShow(elForm.btnInfoTrim, true);
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

const handlerSubmit = async (button, isConfirmed = false) => {
  try {
    // return alert(button);
    getById(button).innerHTML = "Processing...";
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
      "chainlength",
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
      isConfirmed: isConfirmed,
      blindno: getById("lblBlindNo")?.innerHTML,
      uniqueid: getById("lblUniqueId")?.innerHTML,
    };

    fields.forEach((field) => {
      formData[field] = getById(field).value;
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

    const data = await response.json();
    const res = data.d || data;

    if (res.confirm) {
      const yes = await isConfirm(res.message?.toUpperCase());
      if (yes) return handlerSubmit(button, true);
      return;
    }

    if (res.error) {
      await isWarning(res.message?.toUpperCase());
      const field = getById(res.field);
      if (field) {
        field.classList.add("is-invalid");
      }
    }

    if (res.success) {
      await isSuccess(res.message);
      window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    }
  } catch (error) {
    const msg = `handlerSubmit: ${error.message}`;
    catchMessages(msg);
  } finally {
    getById(button).innerHTML = "Save Changes";
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

  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = document.getElementById("notescount");
  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
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
    elForm.lblBlindNo.innerHTML = "Blind 1";
    handlerElementVisibility();
    loaderFadeOut();
  } else if (
    ["NextItem", "EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)
  ) {
    await bindItemOrders();
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

const handlerHideBSModal = (id) => {
  var modalEl = document.getElementById(id);
  var modalInstance = bootstrap.Modal.getInstance(modalEl);

  if (modalInstance) {
    modalInstance.hide();
  } else {
    // Jika modal belum pernah di-show dan belum punya instance, buat dan langsung hide
    modalInstance = new bootstrap.Modal(modalEl);
    modalInstance.hide();
  }
};

// HANDLER SHOW BOOTSTRAP MODAL
const handlerShowBSModal = (params) => {
  var myModal = new bootstrap.Modal(document.getElementById(params), {
    keyboard: false,
  });
  myModal.show();
};

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
