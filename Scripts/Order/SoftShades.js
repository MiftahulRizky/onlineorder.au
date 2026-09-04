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
  softshadesPageLoaded();
});

// ==========================================================INITIALIZATION=====================================================================
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const selectorEl = (el) => document.querySelector(el);
const selectorElAll = (el) => document.querySelectorAll(el);

// ===============================================================EVENTS========================================================================
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "blindtype") {
      const blindtype = e.target.value;
      const blindname = e.target.selectedOptions[0].dataset.name;
      await handlerElementVisibility(blindname);
      await bindTubes(DESIGNID, blindtype);
    }

    if (e.target.id === "tubetype") {
      const blinds = document.getElementById("blindtype");
      const blindtype = blinds.value;
      const blindname = blinds.selectedOptions[0].dataset.name;
      const tubetype = e.target.value;
      await handlerElementVisibility(blindname, tubetype);
      await bindControls(DESIGNID, blindtype, tubetype);
    }

    if (e.target.id === "controltype") {
      const blind = document.getElementById("blindtype");
      const blindtype = blind.value;
      const blindname = blind.selectedOptions[0].dataset.name;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      const controlname = e.target.selectedOptions[0].dataset.name;
      await bindFabrics(DESIGNID);
      await Promise.all([
        bindSizeType(),
        bindMounting(),
        bindDropFloor(),
        bindSlatSize(),
        bindTrackColour(tubetype),
        bindStackPosition(),
        bindControlPosition(),
        bindChains(),
        bindWandLength(),
        bindBracketType(),
        bindBracketColour(tubetype),
        bindHanger(blindname, tubetype),
        bindBottom(),
      ]);
      await handlerElementVisibility(blindname, tubetype, controlname);
    }

    if (e.target.id === "sizetype") {
      const sizetype = e.target.value;
      const mounting = document.getElementById("mounting").value;
      const divDropFloor = document.getElementById("divDropFloor");
      divDropFloor.classList.add("d-none");
      if (sizetype == "Opening Size" && mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }
      bindDropFloor();
    }

    if (e.target.id === "mounting") {
      const sizetype = getById("sizetype").value;
      const mounting = e.target.value;

      const divDropFloor = getById("divDropFloor");
      divDropFloor.classList.add("d-none");
      if (sizetype == "Opening Size" && mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }
      bindDropFloor();
    }

    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      const tubetype = getById("tubetype").value;
      getById("fabriclength").innerHTML = "";
      getById("fabriccolour").innerHTML = "";
      await bindFabricLength(DESIGNID, tubetype, fabrictype);
    }

    if (e.target.id === "fabriclength") {
      const fabrictype = selectorEl("#fabrictype").value;
      const fabriclength = e.target.value;
      getById("fabriccolour").innerHTML = "";
      await bindFabricColours(DESIGNID, fabrictype, fabriclength);
    }

    if (e.target.id === "wandlength") {
      const tubetype = getById("tubetype").value;
      const divWandCustomLength = selectorEl("#divWandCustomLength");
      const wandlength = e.target.value;
      divWandCustomLength.classList.add("d-none");
      if (wandlength === "custom") {
        divWandCustomLength.classList.remove("d-none");
      }
      bindWandColour(wandlength);
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

document.querySelectorAll(".btn-information").forEach((el) => {
  el.addEventListener("click", async (e) => {
    const id = e.currentTarget.id;
    let msg = "";

    switch (id) {
      case "btnInfoQty":
        msg =
          "Please pay attention to the quantity you want to order, because the quantity you enter will be processed automatically.";
        break;
      case "btnInfoWD":
        msg =
          "Very long tracks are not recommended. Butting shorter tracks will work more effectively.";
        break;
      case "btnInfoSlatQty":
        msg = "If left blank, the system will calculate it.";
        break;
      case "btnInfoCustomLength":
        msg =
          "Custom wand length is available in white color only with maximum length 3000mm.";
        break;
    }

    if (msg) {
      isInfo(msg);
    }
  });
});

document.querySelector("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  document.querySelectorAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  // handlerSubmit(e.target.form, e.target.id);
  handlerSubmit(e.target.id);
});

document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// =============================================================FUNCTIONS========================================================================

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
    const cardTitle = document.getElementById("cardTitle");
    const actionMap = {
      AddItem: "ADD ITEM",
      NextItem: "NEXT ITEM",
      EditItem: "EDIT ITEM ID: " + ITEMID,
      ViewItem: "VIEW ITEM ID: " + ITEMID,
      CopyItem: "COPY ITEM",
    };
    cardTitle.innerText = actionMap[itemaction] || "";
  } catch (error) {
    const msg = `bindActionInfo: ${error.message}`;
    catchMessages(msg);
  }
};

const bindTubes = async (designid, blindtype) => {
  if (!designid || !blindtype) return;

  await bindListData({
    elementId: "tubetype",
    field: "tubetype",
    params: { designid, blindtype },
    withDefaultOption: true,
    lengthDefaultOption: 0,
  });
};

const bindControls = async (designid, blindtype, tubetype) => {
  if (!designid || !blindtype || !tubetype) return;

  await bindListData({
    elementId: "controltype",
    field: "controltype",
    params: { designid, blindtype, tubetype },
    withDefaultOption: true,
    lengthDefaultOption: 1,
    onSingle: async (item, select) => {
      const blind = document.getElementById("blindtype");
      const blindname = blind.selectedOptions[0].dataset.name;
      const controltype = item.value;
      const controlname = item.text;

      await bindFabrics(designid);
      await Promise.all([
        bindSizeType(),
        bindMounting(),
        bindDropFloor(),
        bindSlatSize(),
        bindTrackColour(tubetype),
        bindStackPosition(),
        bindControlPosition(),
        bindChains(),
        bindWandLength(),
        bindBracketType(),
        bindBracketColour(tubetype),
        bindHanger(blindname, tubetype),
        bindBottom(),
      ]);
      await handlerElementVisibility(blindname, tubetype, controlname);
    },
  });
};

const bindSizeType = () => {
  generateOption("sizetype", ["Opening Size", "Make Size"]);
};

const bindMounting = () => {
  generateOption("mounting", ["Face Fit", "Reveal Fit"]);
};

const bindDropFloor = () => {
  generateOption("dropfloor", ["No", "Yes"]);
};

const bindFabrics = async (designid) => {
  if (!designid) return;
  getById("fabriclength").innerHTML = "";
  getById("fabriccolour").innerHTML = "";
  await bindListData({
    elementId: "fabrictype",
    field: "fabrictype",
    params: { designid },
    withDefaultOption: true,
    lengthDefaultOption: 1,
  });
};

const bindFabricLength = async (designid, tubetype, fabrictype) => {
  if (!designid || !tubetype || !fabrictype) return;

  await bindListData({
    elementId: "fabriclength",
    field: "fabriclength",
    params: { designid, tubetype, fabrictype },
    withDefaultOption: true,
    lengthDefaultOption: 1,
    onSingle: async (item, select) => {
      const fabriclength = item.value;
      await bindFabricColours(designid, fabrictype, fabriclength);
    },
  });
};

const bindFabricColours = async (designid, fabrictype, fabriclength) => {
  if (!designid || !fabrictype || !fabriclength) return;

  await bindListData({
    elementId: "fabriccolour",
    field: "fabriccolour",
    params: { designid, fabrictype, fabriclength },
    withDefaultOption: true,
    lengthDefaultOption: 1,
  });
};

const bindSlatSize = () => {
  generateOption("slatsize", ["127mm", "100mm", "89mm", "63mm"]);
};

const bindTrackColour = (tubetype) => {
  if (!tubetype) return;
  let list = [];
  if (["Louvolite"].includes(tubetype)) {
    list.push("Black", "White");
  }

  if (["Standard"].includes(tubetype)) {
    list.push("Beige", "Black", "Ivory", "Silver Anodised", "White");
  }
  generateOption("trackcolour", list);
};

const bindStackPosition = () => {
  generateOption("stackposition", [
    "Fix",
    "Left",
    "Right",
    "Center",
    "Split / Centre Open",
  ]);
};

const bindControlPosition = () => {
  generateOption("controlposition", [
    "Tilt Only",
    "Left",
    "Right",
    "Twin Wand",
  ]);
};

const bindChains = () => {
  generateOption("chaincolour", [
    "Beige",
    "Birch White",
    "Black",
    "Grey",
    "Stainless Steel",
    "White",
  ]);
};

const bindWandLength = () => {
  getById("wandcolour").innerHTML = "";
  generateOption("wandlength", [
    "custom",
    "500",
    "750",
    "800",
    "1000",
    "1100",
    "1250",
    "1500",
    "2000",
  ]);
};

const bindWandColour = (wandlength) => {
  if (!wandlength) return;
  let list = ["White"];
  if (wandlength !== "custom") {
    list.push("Black", "White Birch");
  }
  generateOption("wandcolour", list, 1);
};

const bindBracketType = () => {
  generateOption("bracket", [
    "127mm F/Fit",
    "100mm F/Fit",
    "89mm F/Fit",
    "C/Fit",
    "Ext F/Fit",
    "Ext C/Fit",
    "Ext",
  ]);
};

const bindBracketColour = (tubetype) => {
  if (!tubetype) return;
  let list = [];

  if (["Louvolite"].includes(tubetype)) {
    list.push("Birch White", "Black", "White", "Grey");
  } else {
    list.push("Birch White", "Black", "White", "Grey", "Silver");
  }

  generateOption("bracketcolour", list);
};

const bindHanger = (blindname, tubetype) => {
  if (!blindname || !tubetype) return;
  let list = [];

  if (["Slat Only"].includes(blindname)) {
    list.push("Standard", "Peghook", "Tiltrack 28mm");
  }

  if (["Complete", "Track Only"].includes(blindname)) {
    if (["Louvolite"].includes(tubetype)) {
      list.push("Opaque", "White");
    }
    if (["Standard"].includes(tubetype)) {
      list.push("Standard");
    }
  }

  generateOption("hangertype", list, 1);
};

const bindBottom = () => {
  generateOption("bottom", [
    "Chained (White)",
    "Fully Sewn In",
    "Plastic Chainless",
    "Plastic Chainless (White)",
    "Top Hanger Only",
  ]);
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

    bindSelectPayload(data.Tubes, "tubetype", true, 1);
    bindSelectPayload(data.Controls, "controltype", true, 1);
    bindSelectPayload(data.Fabrics, "fabrictype", true, 1);
    bindSelectPayload(data.FabricLength, "fabriclength", true, 1);
    bindSelectPayload(data.FabricColour, "fabriccolour", true, 1);

    console.log(data.DetailData);

    bindSizeType();
    bindDropFloor();
    bindMounting();
    bindDropFloor();
    bindSlatSize();
    bindTrackColour(data.DetailData.TubeType);
    bindStackPosition();
    bindControlPosition();
    bindChains();
    bindWandLength();
    bindWandColour(data.DetailData.WandLength);
    bindBracketType();
    bindBracketColour(data.DetailData.TubeType);
    bindHanger(data.DetailData.BlindName, data.DetailData.TubeType);
    bindBottom();
    handlerSetElementValues(data.DetailData);
    handlerElementVisibility(
      data.DetailData.BlindName,
      data.DetailData.TubeType,
      data.DetailData.ControlType,
      data.DetailData,
    );

    console.log(data);
  } catch (err) {
    const msg = `bindItemOrders: ${err.message}`;
    catchMessages(msg);
  }
};

// ----------------------------------------------|| Hanlder Functions ||---------------------------------------
const handlerElementVisibility = (blindname, tubetype, controlname, item) => {
  try {
    const lblItemId = document.getElementById("lblItemId");
    const divTubeType = document.getElementById("divTubeType");
    const divControlType = document.getElementById("divControlType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divSizeType = document.getElementById("divSizeType");
    const divDropFloor = document.getElementById("divDropFloor");
    const divMounting = document.getElementById("divMounting");
    const lblWd = document.getElementById("lblWd");
    const divWidth = document.getElementById("divWidth");
    const divDrop = document.getElementById("divDrop");
    const divInfoWD = document.getElementById("divInfoWD");
    const divSlatSize = document.getElementById("divSlatSize");
    const divSlatQty = document.getElementById("divSlatQty");
    const divFabric = document.getElementById("divFabric");
    const divTrackColour = document.getElementById("divTrackColour");
    const divStackPosition = document.getElementById("divStackPosition");
    const divControlPosition = document.getElementById("divControlPosition");
    const divChain = document.getElementById("divChain");
    const divWand = document.getElementById("divWand");
    const divWandCustomLength = document.getElementById("divWandCustomLength");
    const divBrackets = document.getElementById("divBrackets");
    const divHangerType = document.getElementById("divHangerType");
    const divBottom = document.getElementById("divBottom");
    const divInsertInTrack = document.getElementById("divInsertInTrack");
    const divSloper = document.getElementById("divSloper");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");
    // return;
    divTubeType.classList.add("d-none");
    divControlType.classList.add("d-none");

    divFormDetail.classList.add("d-none");
    divSizeType.classList.add("d-none");
    divDropFloor.classList.add("d-none");
    divMounting.classList.add("d-none");
    lblWd.innerHTML = "width x drop";
    divWidth.classList.add("d-none");
    divDrop.classList.add("d-none");
    divInfoWD.classList.add("d-none");
    divSlatSize.classList.add("d-none");
    divSlatQty.classList.add("d-none");
    divFabric.classList.add("d-none");
    divTrackColour.classList.add("d-none");
    divStackPosition.classList.add("d-none");
    divControlPosition.classList.add("d-none");
    divChain.classList.add("d-none");
    divWand.classList.add("d-none");
    divWandCustomLength.classList.add("d-none");
    divBrackets.classList.add("d-none");
    divHangerType.classList.add("d-none");
    divBottom.classList.add("d-none");
    divInsertInTrack.classList.add("d-none");
    divSloper.classList.add("d-none");
    divMarkUp.classList.add("d-none");
    btnSubmit.classList.add("d-none");

    if (!blindname) return;
    divTubeType.classList.remove("d-none");

    if (!tubetype) return;
    if (["Complete", "Track Only"].includes(blindname)) {
      divControlType.classList.remove("d-none");
    }

    if (!controlname) return;
    divFormDetail.classList.remove("d-none");

    if (blindname === "Complete") {
      // divSizeType.classList.remove("d-none");
      divMounting.classList.remove("d-none");
      divWidth.classList.remove("d-none");
      divDrop.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divTrackColour.classList.remove("d-none");
      divStackPosition.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      divBrackets.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divBottom.classList.remove("d-none");
      divSloper.classList.remove("d-none");

      if (tubetype === "Fairline") {
        divInsertInTrack.classList.remove("d-none");
      }
      if (controlname === "Chain") {
        divChain.classList.remove("d-none");
      }
      if (controlname === "Wand") {
        divWand.classList.remove("d-none");
      }
    }

    if (blindname === "Slat Only") {
      lblWd.innerHTML = "drop exact";
      divDrop.classList.remove("d-none");
      divSlatQty.classList.remove("d-none");
      divFabric.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divBottom.classList.remove("d-none");
    }

    if (blindname === "Track Only") {
      lblWd.innerHTML = "width";
      divMounting.classList.remove("d-none");
      divWidth.classList.remove("d-none");
      divInfoWD.classList.remove("d-none");
      divSlatSize.classList.remove("d-none");
      // divSlatQty.classList.remove("d-none");
      divTrackColour.classList.remove("d-none");
      divStackPosition.classList.remove("d-none");
      divControlPosition.classList.remove("d-none");
      // divChain.classList.remove("d-none");
      divBrackets.classList.remove("d-none");
      divHangerType.classList.remove("d-none");
      divSloper.classList.remove("d-none");

      if (tubetype === "Fairline") {
        divInsertInTrack.classList.remove("d-none");
      }

      if (controlname === "Chain") {
        divChain.classList.remove("d-none");
      }
      if (controlname === "Wand") {
        divWand.classList.remove("d-none");
      }
    }

    if (item) {
      if (item.LouvreSize == "Opening Size" && item.Mounting == "Face Fit") {
        divDropFloor.classList.remove("d-none");
      }

      const WandLengthKey = ["", "500", "750", "1100", "1250", "1500", "2000"];
      const WandLengthVal = item.WandLength;
      if (!WandLengthKey.includes(WandLengthVal)) {
        divWandCustomLength.classList.remove("d-none");
      } else {
        divWandCustomLength.classList.add("d-none");
      }
    }

    if (MARKUPACCESS === "True") divMarkUp.classList.remove("d-none");

    if (["AddItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
      btnSubmit.classList.remove("d-none");
    } else if (ITEMACTION === "ViewItem") {
      btnSubmit.classList.remove("d-none");
      if (
        !["Administrator", "PPIC & DE", "Customer Service"].includes(ROLENAME)
      ) {
        btnSubmit.classList.add("d-none");
      }
    }
  } catch (error) {
    const msg = `handlerElementVisibility: ${error.message}`;
    catchMessages(msg);
  }
};

const handlerSubmit = async (button) => {
  try {
    // return alert(button);
    document.getElementById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = [
      "blindtype",
      "tubetype",
      "controltype",
      "qty",
      "room",
      "sizetype",
      "dropfloor",
      "mounting",
      "width",
      "drop",
      "slatsize",
      "slatqty",
      "fabrictype",
      "fabriclength",
      "fabriccolour",
      "trackcolour",
      "stackposition",
      "controlposition",
      "chaincolour",
      "chainlength",
      "wandlength",
      "wandcolour",
      "wandcustomlength",
      "bracket",
      "bracketcolour",
      "hangertype",
      "bottom",
      "inserttrack",
      "sloper",
      "notes",
      "markup",
    ];

    const formData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
      rolename: ROLENAME,
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

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    } else if (res.warning) {
      await isWarning(res.message?.toUpperCase());
      const field = document.getElementById(res.field);
      if (field) {
        // field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        // field.focus();
        field.classList.add("is-invalid");
      }
    } else if (res.success) {
      await isSuccess(res.message);
      window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    }
  } catch (error) {
    const msg = `handlerSubmit: ${error.message}`;
    catchMessages(msg);
  } finally {
    document.getElementById(button).innerHTML = "Save Changes";
  }
};

const handlerSetElementValues = (itemData) => {
  try {
    const mapping = {
      blindtype: "BlindId",
      tubetype: "TubeType",
      controltype: "KitId",
      qty: "Qty",
      room: "Location",
      sizetype: "LouvreSize",
      dropfloor: "LouvrePosition",
      mounting: "Mounting",
      width: "Width",
      drop: "Drop",
      slatsize: "SlatSize",
      slatqty: "SlatQty",
      fabrictype: "FabricType",
      fabriclength: "FabricWidth",
      fabriccolour: "FabricId",
      trackcolour: "TrackColour",
      stackposition: "StackPosition",
      controlposition: "ControlPosition",
      chaincolour: "ChainColour",
      chainlength: "ChainLength",
      wandcolour: "WandColour",
      wandlength: "WandLength",
      wandcustomlength: "WandLength",
      bracket: "BracketOption",
      bracketcolour: "BracketColour",
      hangertype: "HangerType",
      bottom: "BottomHoldDown",
      inserttrack: "InsertInTrack",
      sloper: "Sloper",
      notes: "Notes",
      markup: "MarkUp",
    };

    // console.log("itemData: ", itemData);
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

      const WandLengthKey = ["", "500", "750", "1100", "1250", "1500", "2000"];
      const WandLengthVal = itemData["WandLength"];
      if (!WandLengthKey.includes(WandLengthVal)) {
        if (id === "wandlength") {
          el.value = "custom";
        }
        if (id === "wandcustomlength") {
          el.value = WandLengthVal;
        }
      } else {
        if (id === "wandlength") {
          el.value = WandLengthVal;
        }
      }

      if (["inserttrack", "sloper"].includes(id)) {
        if (["0", "False"].includes(value)) {
          el.value = "0";
        } else {
          el.value = "1";
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
  } catch (error) {
    const msg = `handlerSetElementValues: ${error.message}`;
    catchMessages(msg);
  }
};
// ----------------------------------------------|| Other Functions ||---------------------------------------
const softshadesPageLoaded = async () => {
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
    await handlerElementVisibility();
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    await bindItemOrders();
    loaderFadeOut();
  }
};

const generateOption = (elementId, list = [], lengthDefaultOption = 0) => {
  const sel = document.getElementById(elementId);
  if (!sel) return;
  sel.innerHTML = ""; // reset

  // Short A-Z
  if (!["wandlength"].includes(elementId)) {
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

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
