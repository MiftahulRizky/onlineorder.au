document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Global/VerticalBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  golobalVerticalPageLoaded();
});

// ==========================================================INITIALIZATION=============================================================
const getById = (id) => document.getElementById(id);
const getByClass = (cls) => document.getElementsByClassName(cls);
const selectorEl = (el) => document.querySelector(el);
const selectorElAll = (el) => document.querySelectorAll(el);

const elForm = {
  divTubeType: getById("divTubeType"),
  lblTubeType: getById("lblTubeType"),
  divColourType: getById("divColourType"),
  divControlType: getById("divControlType"),
  divFormDetail: getById("divFormDetail"),
  divMounting: getById("divMounting"),
  divFabricInsert: getById("divFabricInsert"),
  divFabric: getById("divFabric"),
  divBladeQty: getById("divBladeQty"),
  lblWd: getById("lblWd"),
  divWidth: getById("divWidth"),
  divDrop: getById("divDrop"),
  divStackPosition: getById("divStackPosition"),
  divControlPosition: getById("divControlPosition"),
  divChain: getById("divChain"),
  divCustomChainLength: getById("divCustomChainLength"),
  divWand: getById("divWand"),
  divWandCustomLength: getById("divWandCustomLength"),
  divBottomJoining: getById("divBottomJoining"),
  divExtBracket: getById("divExtBracket"),
  divSloping: getById("divSloping"),
  divMarkUp: getById("divMarkUp"),
  btnSubmit: getById("btnSubmit"),
};

// ==========================================================EVENTS=====================================================================
selectorElAll(".form-control, .form-select").forEach((el) => {
  try {
    el.addEventListener("change", async (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "blindtype") {
        const blindtype = e.target.value;
        const blindname = e.target.selectedOptions[0].dataset.name;
        await handlerElementVisibility(blindname);
        await bindTubes(DESIGNID, blindtype);
      }

      if (e.target.id === "tubetype") {
        const blinds = getById("blindtype");
        const blindtype = blinds.value;
        const blindname = blinds.selectedOptions[0].dataset.name;
        const tubetype = e.target.value;
        await handlerElementVisibility(blindname, tubetype);
        await bindControls(DESIGNID, blindtype, tubetype);
      }

      if (e.target.id === "controltype") {
        const blinds = getById("blindtype");
        const blindtype = blinds.value;
        const blindname = blinds.selectedOptions[0].dataset.name;
        const tubetype = getById("tubetype").value;
        const controltype = e.target.value;
        await handlerElementVisibility(blindname, tubetype, controltype);
        await bindColours(DESIGNID, blindtype, tubetype, controltype);
      }

      if (e.target.id === "colourtype") {
        const blinds = getById("blindtype");
        const blindtype = blinds.value;
        const blindname = blinds.selectedOptions[0].dataset.name;
        const tubetype = getById("tubetype").value;
        const controltype = getById("controltype").value;
        const colourtype = e.target.value;
        bindMounting();
        bindFabricInsert();
        bindFabrics(DESIGNID);
        bindStackPosition();
        bindControlPosition();
        bindControlColour(controltype);
        bindBottomJoining();
        bindExtBracket();
        bindSloping();
        await handlerElementVisibility(
          blindname,
          tubetype,
          controltype,
          colourtype,
        );
      }

      if (e.target.id === "fabricinsert") {
        const blinds = getById("blindtype");
        const blindname = blinds.selectedOptions[0].dataset.name;
        const isTrackOnly = blindname === "Track Only";
        const fabricinsert = e.target.value;
        const isInsert = fabricinsert === "Yes";
        if (isTrackOnly) {
          toggleShow(elForm.divFabric, false);
          getById("fabrictype").innerHTML = "";
          getById("fabriccolour").innerHTML = "";
          if (isInsert) {
            toggleShow(elForm.divFabric, true);
            bindFabrics(DESIGNID);
          }
        }
      }

      if (e.target.id === "fabrictype") {
        const fabrictype = e.target.value;
        await bindFabricColours(DESIGNID, fabrictype);
      }

      if (["chaincolour", "wandcolour"].includes(e.target.id)) {
        toggleShow(elForm.divCustomChainLength, false);
        toggleShow(elForm.divWandCustomLength, false);
        getById("chainlength").innerHTML = "";
        getById("wandlength").innerHTML = "";
        const controltype = getById("controltype").value;
        const val = e.target.value;
        if (!val) return;

        bindControlLength(controltype);
      }

      if (["chainlength", "wandlength"].includes(e.target.id)) {
        const val = e.target.value;
        const isStandard = val === "Standard";
        const isCustom = val === "Custom";
        const controltype = getById("controltype").value;
        const isChain = controltype === "Chain";
        const isWand = controltype === "Wand";
        toggleShow(elForm.divCustomChainLength, false);
        toggleShow(elForm.divWandCustomLength, false);
        getById("customchainlength").value = "";
        getById("customwandlength").value = "";

        if (isChain && isCustom) {
          toggleShow(elForm.divCustomChainLength, true);
        }

        if (isWand && isCustom) {
          toggleShow(elForm.divWandCustomLength, true);
        }
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
  } catch (error) {
    const msg = `form event: ${error}`;
    catchMessages(msg);
  }
});

selectorEl("#btnSubmit").addEventListener("click", (e) => {
  e.preventDefault();

  selectorElAll(".form-control, .form-select").forEach((el) => {
    el.classList.remove("is-invalid");
  });

  handlerSubmit(e.target.id);
});

document.querySelector("#btnCancel").addEventListener("click", (e) => {
  const id = e.target.id;
  handlerCancel(id);
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
    getById("pageTitle").innerHTML = name;
    getById("pageAction").innerHTML = ITEMACTION;
  } catch (error) {
    const msg = `bindPageTitle: ${error.message}`;
    catchMessages(msg);
  }
};

const bindHeaderInfo = (header) => {
  try {
    const lblOrder = getById("lblOrder");
    const lblItemId = getById("lblItemId");
    const lblOrderNumber = getById("lblOrderNumber");
    const lblOrderName = getById("lblOrderName");

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
      // const blindtype = getById("blindtype");
      // const brackettype = getById("brackettype");
      // blindtype.setAttribute("disabled", true);
      // brackettype.setAttribute("disabled", true);
    }
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
      const blinds = getById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const controltype = item.value;
      await handlerElementVisibility(blindname, tubetype, controltype);
      await bindColours(designid, blindtype, tubetype, controltype);
    },
  });
};

const bindColours = async (designid, blindtype, tubetype, controltype) => {
  if (!designid || !blindtype || !tubetype || !controltype) return;

  await bindListData({
    elementId: "colourtype",
    field: "colourtype",
    params: { designid, blindtype, tubetype, controltype },
    withDefaultOption: true,
    lengthDefaultOption: 1,

    onSingle: async (item, select) => {
      const blinds = getById("blindtype");
      const blindname = blinds.selectedOptions[0].dataset.name;
      const colourtype = item.value;
      bindMounting();
      bindFabricInsert();
      bindFabrics(designid);
      bindStackPosition();
      bindControlPosition();
      bindControlColour(controltype);
      bindBottomJoining();
      bindExtBracket();
      bindSloping();
      await handlerElementVisibility(
        blindname,
        tubetype,
        controltype,
        colourtype,
      );
    },
  });
};

const bindMounting = () => {
  generateOption("mounting", [
    "Make Size Face Fit",
    "Make Size Reveal Fit",
    "Opening Size Face Fit",
    "Opening Size Reveal Fit",
  ]);
};

const bindFabricInsert = () => {
  generateOption("fabricinsert", ["Yes"]);
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

const bindStackPosition = () => {
  generateOption("stackposition", ["Left", "Right", "Centre", "Split"]);
};

const bindControlPosition = () => {
  generateOption("controlposition", ["Left", "Right"]);
};

const bindControlColour = (controltype) => {
  if (!controltype) return;
  getById("chainlength").innerHTML = "";
  getById("wandlength").innerHTML = "";
  const isChain = controltype === "Chain";
  const isWand = controltype === "Wand";

  let list = [];
  let field = "";
  if (isChain) {
    list.push("Chromed Metal");
    field = "chaincolour";
  }
  if (isWand) {
    list.push("Beige", "Black", "Grey", "Ivory", "White");
    field = "wandcolour";
  }
  generateOption(field, list, 1);
  if (list.length == 1) {
    bindControlLength(controltype);
  }
};
const bindControlLength = (controltype) => {
  if (!controltype) return;
  const isChain = controltype === "Chain";
  const isWand = controltype === "Wand";

  let list = ["Standard", "Custom"];
  let field = "";
  if (isChain) {
    field = "chainlength";
  }
  if (isWand) {
    field = "wandlength";
  }
  generateOption(field, list, 1);
};

const bindBottomJoining = () => {
  generateOption("bottomjoining", ["Chainless", "With Chain", "Sewn In"]);
};

const bindExtBracket = () => {
  generateOption("extbracket", ["Yes"]);
};

const bindSloping = () => {
  generateOption("sloping", ["Top", "Bottom"]);
};

const bindSelect = ({
  data,
  elementId,
  withDefaultOption = true,
  lengthDefaultOption = 0,
  onSingle = null,
  afterRender = null,
}) => {
  const select = getById(elementId);
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

    bindSelectPayload(data.Tubes, "tubetype", true, 1);
    bindSelectPayload(data.Controls, "controltype", true, 1);
    bindSelectPayload(data.Colours, "colourtype", true, 1);
    bindSelectPayload(data.Fabrics, "fabrictype", true, 1);
    bindSelectPayload(data.FabricColours, "fabriccolour", true, 1);

    await Promise.all([
      bindMounting(),
      bindFabricInsert(),
      bindStackPosition(),
      bindControlPosition(),
      bindControlColour(detail.ControlType),
      bindControlLength(detail.ControlType),
      bindBottomJoining(),
      bindExtBracket(),
      bindSloping(),
      handlerSetElementValues(detail),
    ]);

    await handlerElementVisibility(
      detail.BlindName,
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
  tubetype,
  controltype,
  colourtype,
  item,
) => {
  try {
    Object.values(elForm).forEach((el) => toggleShow(el, false));
    const isComplete = blindname === "Complete Set";
    const isSlatOnly = blindname === "Slat Only";
    const isTrackOnly = blindname === "Track Only";
    const is127mm = tubetype === "127mm";
    const is89mm = tubetype === "89mm";
    const isChain = controltype === "Chain";
    const isWand = controltype === "Wand";
    const isMarkup = ["True", "1"].includes(MARKUPACCESS);
    const isCreate = ["AddItem", "NextItem", "EditItem", "CopyItem"].includes(
      ITEMACTION,
    );
    const isView = ["ViewItem"].includes(ITEMACTION);
    const isInternal = [
      "Administrator",
      "PPIC & DE",
      "Customer Service",
    ].includes(ROLENAME);

    if (!blindname) return;
    toggleShow(elForm.lblTubeType, true);
    toggleShow(elForm.divTubeType, true);

    if (!tubetype) return;
    if (!isSlatOnly) {
      toggleShow(elForm.divControlType, true);
    }

    if (!controltype) return;
    if (!isSlatOnly) {
      toggleShow(elForm.divColourType, true);
    }

    if (!colourtype) return;
    toggleShow(elForm.divFormDetail, true);
    toggleShow(elForm.divMounting, true);

    if (isComplete) {
      toggleShow(elForm.divFabricInsert, true);
      toggleShow(elForm.divFabric, true);
      toggleShow(elForm.divWidth, true);
      toggleShow(elForm.divDrop, true);
      toggleShow(elForm.lblWd, true);
      elForm.lblWd.innerHTML = "Width x Drop";
      toggleShow(elForm.divStackPosition, true);
      if (isChain) {
        toggleShow(elForm.divControlPosition, true);
        toggleShow(elForm.divChain, true);
      }
      if (isWand) {
        toggleShow(elForm.divWand, true);
      }
      toggleShow(elForm.divBottomJoining, true);
      toggleShow(elForm.divExtBracket, true);
      toggleShow(elForm.divSloping, true);
    }

    if (isSlatOnly) {
      toggleShow(elForm.divFabric, true);
      toggleShow(elForm.divBladeQty, true);
      toggleShow(elForm.lblWd, true);
      toggleShow(elForm.divDrop, true);
      elForm.lblWd.innerHTML = "finished Drop </br> (Fabric to fabric)";
      toggleShow(elForm.divBottomJoining, true);
    }

    if (isTrackOnly) {
      toggleShow(elForm.divFabricInsert, true);
      toggleShow(elForm.divBladeQty, true);
      toggleShow(elForm.lblWd, true);
      toggleShow(elForm.divWidth, true);
      elForm.lblWd.innerHTML = "width";
      toggleShow(elForm.divStackPosition, true);
      if (isChain) {
        toggleShow(elForm.divControlPosition, true);
        toggleShow(elForm.divChain, true);
      }
      if (isWand) {
        toggleShow(elForm.divWand, true);
      }
      toggleShow(elForm.divExtBracket, true);
    }

    if (item) {
      if (isChain) {
        const data = JSON.parse(item.MaterialChain);
        if (data.name === "Custom") {
          toggleShow(elForm.divCustomChainLength, true);
        }
      }
      if (isWand) {
        const data = JSON.parse(item.MaterialCord);
        if (data.name === "Custom") {
          toggleShow(elForm.divWandCustomLength, true);
        }
      }

      if (isTrackOnly) {
        if (item.InsertInTrack) {
          toggleShow(elForm.divFabric, true);
        }
      }
    }

    if (isMarkup) {
      toggleShow(elForm.divMarkUp, true);
    }

    if (isCreate) {
      toggleShow(elForm.btnSubmit, true);
    }

    if (isView) {
      toggleShow(elForm.btnSubmit, true);
      if (!isInternal) {
        toggleShow(elForm.btnSubmit, false);
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
    getById(button).innerHTML = "Processing...";
    swalLoadingShow("Please wait while we save the data.");
    const fields = [
      "blindtype",
      "tubetype",
      "controltype",
      "colourtype",
      "qty",
      "room",
      "mounting",
      "fabricinsert",
      "fabrictype",
      "fabriccolour",
      "bladeqty",
      "width",
      "drop",
      "stackposition",
      "controlposition",
      "chaincolour",
      "chainlength",
      "customchainlength",
      "wandcolour",
      "wandlength",
      "customwandlength",
      "bottomjoining",
      "extbracket",
      "sloping",
      "notes",
      "markup",
    ];

    const formData = {
      headerid: HEADERID,
      ordertype: ORDERTYPE,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
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

    if (res.error) {
      throw new Error(res.message);
    }

    if (res.warning) {
      await isWarning(res.message?.toUpperCase());
      const field = getById(res.field);
      if (field) {
        field.classList.add("is-invalid");
      }
    }

    if (res.success) {
      await isSuccess(res.message);
      window.location.href = res.url;
    }
  } catch (error) {
    const msg = `handlerSubmit: ${error.message}`;
    catchMessages(msg);
  } finally {
    getById(button).innerHTML = "Save Changes";
  }
};

const handlerCancel = async (button = "btnCancel") => {
  try {
    getById(button).innerHTML = "Waiting...";

    const response = await fetch(URIMETHOD + "/Cancel", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: HEADERID,
        ordertype: ORDERTYPE,
        rolename: ROLENAME,
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`${response.status}\n${errorText}`);
    }

    const data = await response.json();
    const res = data.d || data;

    if (res.error) {
      throw new Error(res.message);
    }

    if (res.warning) {
      await isWarning(res.message?.toUpperCase());
      const field = getById(res.field);
      if (field) {
        field.classList.add("is-invalid");
      }
    }

    if (res.success) {
      // await isSuccess(res.message);
      window.location.href = res.url;
    }
  } catch (error) {
    const msg = `handlerCancel: ${error.message}`;
    catchMessages(msg);
  } finally {
    getById(button).innerHTML = "Cancel";
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    blindtype: "BlindId",
    tubetype: "TubeType",
    controltype: "ControlType",
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabricinsert: "InsertInTrack",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    bladeqty: "SlatQty",
    width: "Width",
    drop: "Drop",
    stackposition: "StackPosition",
    controlposition: "ControlPosition",
    chaincolour: "ChainColour",
    wandcolour: "WandColour",
    bottomjoining: "BottomHoldDown",
    extbracket: "BracketExtension",
    sloping: "Cleat",
    notes: "Notes",
    markup: "MarkUp",
  };

  // Mapping kolom biasa
  Object.entries(mapping).forEach(([id, key]) => {
    const el = document.getElementById(id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[key];

    if (id === "markup" && value === 0) value = "";

    if (id === "fabricinsert") {
      value = value == 1 || value == "True" ? "Yes" : "";
    }

    el.value = value ?? "";

    if (el.value === "0") el.value = "";
  });

  // Helper untuk mengisi MaterialChain dan MaterialCord
  const setMaterialLength = (source, nameId, customId) => {
    let data = itemData[source];

    const nameEl = getById(nameId);
    const customEl = getById(customId);

    // Parse JSON jika data masih berupa string
    if (typeof data === "string") {
      try {
        data = JSON.parse(data);
      } catch (error) {
        // console.warn(`Invalid JSON ${source}:`, data);
        data = null;
      }
    }

    if (!data) {
      if (nameEl) nameEl.value = "";
      if (customEl) customEl.value = "";
      return;
    }

    if (nameEl) nameEl.value = data.name ?? "";
    if (customEl) customEl.value = data.value ?? "";
  };

  // Chain Length
  setMaterialLength("MaterialChain", "chainlength", "customchainlength");

  // Wand Length
  setMaterialLength("MaterialCord", "wandlength", "customwandlength");

  // Notes counter
  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = getById("notescount");

  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const golobalVerticalPageLoaded = async () => {
  if (!HEADERID) {
    window.location.href = "/order";
    return;
  }

  if (!ORDERTYPE) {
    window.location.href = "/order";
    return;
  }

  if (!ITEMACTION || !DESIGNID) {
    handlerCancel();
    return;
  }

  if (DESIGNID.toUpperCase() !== DESIGNIDORI) {
    handlerCancel();
    return;
  }

  await bindFormAggregate();
  if (ITEMACTION === "AddItem") {
    await handlerElementVisibility();
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

const catchMessages = (msg) => {
  if (!["Administrator"].includes(ROLENAME))
    msg = "Please contact our IT team at support@onlineorder.au";
  isError(msg);
  console.error(msg);
};
