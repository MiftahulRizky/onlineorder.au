document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("RollerBlinds.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  pageLoaded();
});

// =================================================EVENTS==================================================
// input or chenge  remove class is-invalid & event
document.querySelectorAll(".form-control, .form-select").forEach((el) => {
  el.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    // ---------------------------------||blindtype||---------------------------------
    if (e.target.id === "blindtype") {
      const lblBracketType = document.getElementById("lblBracketType");
      const divBracketType = document.getElementById("divBracketType");
      const divTubeType = document.getElementById("divTubeType");
      const divControlType = document.getElementById("divControlType");
      const divColourType = document.getElementById("divColourType");
      const lblColourType = document.getElementById("lblColourType");
      const divFormDetail = document.getElementById("divFormDetail");
      const lblControlPosition = document.getElementById("lblControlPosition");

      lblBracketType.innerHTML = "bracket type";
      divBracketType.setAttribute("hidden", true);
      divTubeType.setAttribute("hidden", true);
      divControlType.setAttribute("hidden", true);
      divColourType.setAttribute("hidden", true);
      lblColourType.innerHTML = "control colour";
      divFormDetail.setAttribute("hidden", true);
      lblControlPosition.innerHTML = "control position";

      if (!e.target.value) return;

      const blindid = e.target.value;
      const blindname = e.target.selectedOptions[0].dataset.name;
      await bindBrackets(DESIGNID, blindid);

      if (blindname === "Cassette") {
        lblBracketType.innerHTML = "cassette type";
        lblColourType.innerHTML = "cassette colour";
        lblControlPosition.innerHTML = "control side";
      }

      divBracketType.removeAttribute("hidden");
    }

    // ---------------------------------||brackettype||---------------------------------
    if (e.target.id === "brackettype") {
      const divTubeType = document.getElementById("divTubeType");
      const divControlType = document.getElementById("divControlType");
      const divColourType = document.getElementById("divColourType");
      const divFormDetail = document.getElementById("divFormDetail");

      divTubeType.setAttribute("hidden", true);
      divControlType.setAttribute("hidden", true);
      divColourType.setAttribute("hidden", true);
      divFormDetail.setAttribute("hidden", true);

      if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = e.target.value;
      await bindTubes(DESIGNID, blindid, brackettype);

      if (blindname !== "Skin Only") {
        divTubeType.removeAttribute("hidden");
      }
    }

    // ---------------------------------||tubetype||---------------------------------
    if (e.target.id === "tubetype") {
      const divControlType = document.getElementById("divControlType");
      const divColourType = document.getElementById("divColourType");
      const divFormDetail = document.getElementById("divFormDetail");

      divControlType.setAttribute("hidden", true);
      divColourType.setAttribute("hidden", true);
      divFormDetail.setAttribute("hidden", true);

      if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = e.target.value;
      await bindControls(DESIGNID, blindid, brackettype, tubetype);

      if (blindname !== "Skin Only") {
        divControlType.removeAttribute("hidden");
      }
    }

    // ---------------------------------||controltype||---------------------------------
    if (e.target.id === "controltype") {
      const divColourType = document.getElementById("divColourType");
      const divFormDetail = document.getElementById("divFormDetail");

      divColourType.setAttribute("hidden", true);
      divFormDetail.setAttribute("hidden", true);

      if (!e.target.value) return;

      const blindtype = document.getElementById("blindtype");
      const blindid = blindtype.value;
      const blindname = blindtype.selectedOptions[0].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = e.target.value;
      await bindColours(DESIGNID, blindid, brackettype, tubetype, controltype);

      if (blindname !== "Skin Only") {
        divColourType.removeAttribute("hidden");
      }
    }

    // ---------------------------------||colourtype||---------------------------------
    if (e.target.id === "colourtype") {
      const divFormDetail = document.getElementById("divFormDetail");
      const divMotorStyle = document.getElementById("divMotorStyle");
      const divMotorRemote = document.getElementById("divMotorRemote");
      const divMotorBattery = document.getElementById("divMotorBattery");
      const divMotorCharger = document.getElementById("divMotorCharger");
      const divCableExitPoint = document.getElementById("divCableExitPoint");
      const divConnector = document.getElementById("divConnector");
      const divBottomRail = document.getElementById("divBottomRail");
      const divChildSafe = document.getElementById("divChildSafe");
      const divAccessory = document.getElementById("divAccessory");
      const divExtras = document.getElementById("divExtras");

      divFormDetail.setAttribute("hidden", true);
      divMotorStyle.setAttribute("hidden", true);
      divMotorRemote.setAttribute("hidden", true);
      divMotorBattery.setAttribute("hidden", true);
      divMotorCharger.setAttribute("hidden", true);
      divCableExitPoint.setAttribute("hidden", true);
      divConnector.setAttribute("hidden", true);
      divBottomRail.setAttribute("hidden", true);
      divChildSafe.setAttribute("hidden", true);
      divAccessory.setAttribute("hidden", true);
      divExtras.setAttribute("hidden", true);

      if (!e.target.value) return;
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const tubetype = document.getElementById("tubetype").value;
      const controltype = document.getElementById("controltype").value;

      await bindFabrics(DESIGNID);
      if (blindname == "Motorised") {
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

      divFormDetail.removeAttribute("hidden");
      if (blindname === "Cassette") {
        if (tubetype === "Motorised") {
          divMotorStyle.removeAttribute("hidden");
          divMotorRemote.removeAttribute("hidden");
          divMotorCharger.removeAttribute("hidden");
          divAccessory.removeAttribute("hidden");
          divExtras.removeAttribute("hidden");
        }
        if (tubetype == "JAI Geared") {
          divAccessory.removeAttribute("hidden");
          divChildSafe.removeAttribute("hidden");
        }
      }

      if (blindname === "Motorised") {
        divMotorStyle.removeAttribute("hidden");
        divMotorRemote.removeAttribute("hidden");
        divMotorCharger.removeAttribute("hidden");
        divAccessory.removeAttribute("hidden");
        divExtras.removeAttribute("hidden");
      }
    }

    // ---------------------------------||fabrictype||---------------------------------
    if (e.target.id === "fabrictype") {
      const fabrictype = e.target.value;
      await bindFabricColours(DESIGNID, fabrictype);
    }

    // ---------------------------------||motorstyle||---------------------------------
    if (e.target.id === "motorstyle") {
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const controltype = document.getElementById("controltype").value;
      const motorstyle = e.target.value;

      await Promise.all([
        bindMotorCharger(controltype, motorstyle),
        bindExtras(blindname, controltype, motorstyle),
      ]);
    }

    // ---------------------------------||trim||---------------------------------
    if (e.target.id === "trim") {
      const divBottomRail = document.getElementById("divBottomRail");

      divBottomRail.setAttribute("hidden", true);

      if (!e.target.value) return;
      const blindtype = document.getElementById("blindtype");
      const blindname = blindtype.options[blindtype.selectedIndex].dataset.name;
      const brackettype = document.getElementById("brackettype").value;
      const trim = e.target.value;
      bindRailType(brackettype);

      if (blindname == "Skin Only" && trim == "1F") {
        divBottomRail.removeAttribute("hidden");
      }
      if (
        (blindname == "Roller Blind" ||
          blindname == "Motorised" ||
          blindname == "Cassette") &&
        trim == "1F"
      ) {
        divBottomRail.removeAttribute("hidden");
      }
    }

    // ---------------------------------||railtype||---------------------------------
    if (e.target.id === "railtype") {
      const brackettype = document.getElementById("brackettype").value;
      const railtype = e.target.value;

      bindRailColour(brackettype, railtype);
    }
  });
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
  });
});

// button cancel
document.querySelector("#btnCancel").addEventListener("click", (e) => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});
// ===============================================FUNCTION==================================================
const pageLoaded = async () => {
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
  bindFormAction(ITEMACTION);

  if (ITEMACTION === "AddItem") {
    await bindBlinds(DESIGNID);
    handlerElementVisibility();
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    loaderFadeOut();
  }
};

// ------------------------------------------------------|| Handler Functions ||--------------------------------------
const handlerElementVisibility = () => {
  try {
    const divBracketType = document.getElementById("divBracketType");
    const divTubeType = document.getElementById("divTubeType");
    const divControlType = document.getElementById("divControlType");
    const btnInfoControlType = document.getElementById("btnInfoControlType");
    const divColourType = document.getElementById("divColourType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divAttention = document.getElementById("divAttention");
    const divMotorStyle = document.getElementById("divMotorStyle");
    const btnInfoMotorStyle = document.getElementById("btnInfoMotorStyle");
    const divMotorRemote = document.getElementById("divMotorRemote");
    const btnInfoMotorRemote = document.getElementById("btnInfoMotorRemote");
    const divMotorBattery = document.getElementById("divMotorBattery");
    const divMotorCharger = document.getElementById("divMotorCharger");
    const divCableExitPoint = document.getElementById("divCableExitPoint");
    const divConnector = document.getElementById("divConnector");
    const divBottomRail = document.getElementById("divBottomRail");
    const divTubeSize = document.getElementById("divTubeSize");
    const divChildSafe = document.getElementById("divChildSafe");
    const divAccessory = document.getElementById("divAccessory");
    const divExtras = document.getElementById("divExtras");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");

    divBracketType.setAttribute("hidden", true);
    divTubeType.setAttribute("hidden", true);
    divControlType.setAttribute("hidden", true);
    btnInfoControlType.setAttribute("hidden", true);
    divColourType.setAttribute("hidden", true);

    divFormDetail.setAttribute("hidden", true);
    divAttention.setAttribute("hidden", true);
    divMotorStyle.setAttribute("hidden", true);
    btnInfoMotorStyle.setAttribute("hidden", true);
    divMotorRemote.setAttribute("hidden", true);
    btnInfoMotorRemote.setAttribute("hidden", true);
    divMotorBattery.setAttribute("hidden", true);
    divMotorCharger.setAttribute("hidden", true);
    divCableExitPoint.setAttribute("hidden", true);
    divConnector.setAttribute("hidden", true);
    divBottomRail.setAttribute("hidden", true);
    divTubeSize.setAttribute("hidden", true);
    divChildSafe.setAttribute("hidden", true);
    divAccessory.setAttribute("hidden", true);
    divExtras.setAttribute("hidden", true);
    btnSubmit.setAttribute("hidden", true);

    if (MARKUPACCESS === "True") divMarkUp.removeAttribute("hidden");

    if (["AddItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
      btnSubmit.removeAttribute("hidden");
    } else if (ITEMACTION === "ViewItem") {
      btnSubmit.removeAttribute("hidden");
      if (ROLENAME !== "Administrator") btnSubmit.setAttribute("hidden", true);
    }
  } catch (error) {
    console.error(error.message);
  }
};
// ------------------------------------------------------|| Binding Functions ||--------------------------------------
const bindFormAction = (itemaction) => {
  const cardTitle = document.getElementById("cardTitle");
  const actionMap = {
    AddItem: "ADD ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemaction] || "";
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
    const lblOrderNumber = document.getElementById("lblOrderNumber");
    const lblOrderName = document.getElementById("lblOrderName");

    lblOrder.innerHTML = OrderId;
    lblOrder.classList.add("fw-bold");

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
        const divControlType = document.getElementById("divControlType");
        const blindname =
          document.getElementById("blindtype").selectedOptions[0].dataset.name;

        await bindControls(designid, blindid, brackettype, select.value);

        if (blindname !== "Skin Only") {
          divControlType.removeAttribute("hidden");
        }
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
        const divColourType = document.getElementById("divColourType");
        const blindname =
          document.getElementById("blindtype").selectedOptions[0].dataset.name;

        await bindColours(
          designid,
          blindid,
          brackettype,
          tubetype,
          select.value,
        );

        if (blindname !== "Skin Only") {
          divColourType.removeAttribute("hidden");
        }
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
        const divFormDetail = document.getElementById("divFormDetail");
        const blindname = await getItemData(
          `SELECT Name FROM Blinds WHERE Id = '${blindid}' AND Active = 1`,
        );

        if (!blindname) {
          throw new Error("Blind name not found : bindColours");
        }

        await bindFabrics(designid);
        if (blindname == "motorised") {
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

        divFormDetail.removeAttribute("hidden");
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

const bindTrims = (blindname, brackettype, tubetype) => {
  const sel = document.getElementById("trim");
  sel.innerHTML = ""; //reset

  if (!blindname || !brackettype || !tubetype) return;

  let data = [];
  if (blindname == "Roller Blind" || blindname == "Motorised" || "Cassette") {
    data.push(
      { value: "1P", text: "1P" },
      { value: "1F", text: "1F" },
      { value: "5F", text: "5F" },
      { value: "6F", text: "6F" },
      { value: "7F", text: "7F" },
      { value: "9F", text: "9F" },
      { value: "10F", text: "10F" },
      { value: "12F", text: "12F" },
      { value: "15F", text: "15F" },
      { value: "17F", text: "17F" },
      { value: "18F", text: "18F" },
      { value: "19F", text: "19F" },
      { value: "20F", text: "20F" },
      { value: "22F", text: "22F" },
      { value: "23F", text: "23F" },
      { value: "24F", text: "24F" },
      { value: "25F", text: "25F" },
      { value: "26F", text: "26F" },
    );
  }

  if (blindname == "Skin Only") {
    if (brackettype == "Excluded" || brackettype == "With Tube Included") {
      data.push(
        { value: "1P", text: "1P" },
        { value: "Spline", text: "Spline" },
      );
    }
    if (brackettype == "Excluded") {
      data.push(
        { value: "Pocket", text: "Pocket" },
        { value: "1RS", text: "1RS" },
        { value: "1OS", text: "1OS" },
        { value: "Added Trim", text: "Added Trim" },
      );
    }
    if (
      brackettype == "With Tube & Bottom Included" ||
      brackettype == "With Bottom Included"
    ) {
      data.push(
        { value: "1P", text: "1P" },
        { value: "1F", text: "1F" },
        { value: "5F", text: "5F" },
        { value: "7F", text: "7F" },
        { value: "9F", text: "9F" },
        { value: "10F", text: "10F" },
        { value: "12F", text: "12F" },
        { value: "15F", text: "15F" },
        { value: "17F", text: "17F" },
        { value: "18F", text: "18F" },
        { value: "19F", text: "19F" },
        { value: "20F", text: "20F" },
        { value: "20F", text: "20F" },
        { value: "22F", text: "22F" },
        { value: "23F", text: "23F" },
        { value: "24F", text: "24F" },
        { value: "25F", text: "25F" },
        { value: "26F", text: "26F" },
      );
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

const bindRailType = async (brackettype) => {
  const select = document.getElementById("railtype");
  select.innerHTML = "";

  if (!brackettype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindRailType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        brackettype,
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

const bindRailColour = async (brackettype, railtype) => {
  const select = document.getElementById("railcolour");
  select.innerHTML = "";

  if (!brackettype || !railtype) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindRailColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        brackettype,
        railtype,
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

const bindTubeSize = (blindname, tubetype) => {
  const sel = document.getElementById("tubesize");
  sel.innerHTML = ""; //reset

  if (!blindname || !tubetype) return;

  let data = [];
  if (blindname == "Roller Blind") {
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
    if (controltype == "Somfy WF" && motorstyle.includes("ZB")) {
      data.push(
        { value: "WF Li Solar Panel Kit", text: "WF Li Solar Panel Kit" },
        { value: "Adaptor Mg V2 Li", text: "Adaptor Mg V2 Li" },
        { value: "Cable Mg Rigid", text: "Cable Mg Rigid" },
      );
    }

    if (blindname == "Cassette") {
      data.push({
        value: "Cable Ex 20cm Cassette",
        text: "Cable Ex 20cm Cassette",
      });
    }

    if (motorstyle.includes("ZB")) {
      if (controltype == "Somfy WF") {
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

    if (controltype.includes("Alpha")) {
      data.push({
        value: "Lead Ex 3M ALDC Charger",
        text: "Lead Ex 3M ALDC Charger",
      });
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
