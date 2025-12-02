document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Cellora.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  checkSessionCellora();
});
// =========================================||EVENTS||========================================
// input or chenge  remove class is-invalid
document.querySelectorAll(".form-control").forEach((el) => {
  el.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
  });
  el.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
  });
});

// change blinds
document.querySelector("#blindtype").addEventListener("change", (e) => {
  const divFormDetail = document.querySelector("#divFormDetail");
  divFormDetail.setAttribute("hidden", true);

  const blindId = e.target.value;
  const fabricType = document.querySelector("#fabrictype").value;

  bindColours(DESIGNID, blindId);
  bindFabrics(DESIGNID);
  bindFabricColours(DESIGNID, fabricType);
});

// // change colours
document.querySelector("#colourtype").addEventListener("change", (e) => {
  handlerElementVisibility(e.target.value);
});

// change fabrics
document.querySelector("#fabrictype").addEventListener("change", (e) => {
  bindFabricColours(DESIGNID, e.target.value);
});

// input notes count length
document.querySelector("#notes").addEventListener("input", (e) => {
  let maxLength = 1000;
  let currentLength = e.target.value.length;
  document.querySelector(
    "#notescount"
  ).textContent = `${currentLength}/${maxLength}`;
});

// btn cancel
const buttonCancel = document.querySelector("#btnCancel");
buttonCancel.addEventListener("click", () => {
  window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
});

// submit form
const buttonSubmit = document.querySelector("#btnSubmit");
buttonSubmit.addEventListener("click", (e) => {
  e.preventDefault();

  // reset error state
  document.querySelectorAll(".form-control").forEach((el) => {
    el.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
    el.classList.remove("is-invalid");
  });

  // kirim form element & button ke handler
  handlerSubmit(buttonSubmit.form, e.target, e.target.innerHTML);
});

// =========================================||FUNCTIONS||=====================================

// --------------------------------------|| handler Functions ||--------------------------------------
const handlerSubmit = async (formEl, button, htmlButton) => {
  try {
    // create FormData
    const formData = new FormData(formEl);

    // loop semua elemen form
    [...formEl.elements].forEach((el) => {
      if (!el.name) return; // skip elemen tanpa name

      // khusus number → pakai value langsung, jangan biarkan kosong
      if (el.type === "number") {
        formData.append(el.name, el.value ?? "");
      } else {
        formData.append(el.name, el.value);
      }
    });

    // ubah FormData menjadi object
    let formObject = Object.fromEntries(formData.entries());

    // filter field ASP.NET yang tidak dibutuhkan
    const excludeKeys = [
      "__EVENTTARGET",
      "__EVENTARGUMENT",
      "__VIEWSTATE",
      "__VIEWSTATEGENERATOR",
      "__SCROLLPOSITIONX",
      "__SCROLLPOSITIONY",
      "__EVENTVALIDATION",
      "ctl00$txtSearchMaster",
    ];

    formObject = Object.fromEntries(
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key))
    );

    // data tambahan
    const extraData = {
      headerid: HEADERID,
      itemaction: ITEMACTION,
      itemid: ITEMID,
      designid: DESIGNID,
      loginid: LOGINID,
    };

    // gabungkan
    const finalData = { ...formObject, ...extraData };

    // debug konsisten
    // console.group("Submit Debug");
    // console.log("FormData snapshot:", [...formData.entries()]);
    // console.table(formObject);
    // console.table(extraData);
    // console.table(finalData);
    // console.groupEnd();

    // before send
    button.setAttribute("disabled", "disabled");
    button.innerHTML = '<i class="ti ti-loader fs-2 me-1"></i> Processing...';

    // fetch POST
    const response = await fetch(URIMETHOD + "/SubmitForm", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: finalData }),
    });

    // restore button
    button.removeAttribute("disabled");
    button.innerHTML = htmlButton;

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        ROLENAME === "Administrator"
          ? `${response.status}\n${errorText}`
          : "Something went wrong, please try again!"
      );
    }

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isError(dataResult.error.message.toUpperCase());
      const field = document.getElementById(dataResult.error.field);
      if (field) {
        field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      await isSuccess(dataResult.success);
      window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    }
  } catch (err) {
    await isError(err.message);
  }
};

const handlerElementVisibility = (colourtype) => {
  const btnSubmit = document.querySelector("#btnSubmit");

  const divFormDetail = document.getElementById("divFormDetail");
  const divMarkUp = document.getElementById("divMarkUp");

  // set default hide
  btnSubmit.setAttribute("hidden", true);
  divFormDetail.setAttribute("hidden", true);
  divMarkUp.setAttribute("hidden", true);
  if (colourtype) divFormDetail.removeAttribute("hidden");

  // markup
  if (MARKUPACCESS === "True") divMarkUp.removeAttribute("hidden");

  if (["AddItem", "EditItem", "CopyItem"].includes(ITEMACTION)) {
    btnSubmit.removeAttribute("hidden");
  } else if (ITEMACTION === "ViewItem") {
    btnSubmit.removeAttribute("hidden");
    if (ROLENAME !== "Administrator") btnSubmit.setAttribute("hidden", true);
  }
};

// --------------------------------------|| Binding Functions ||--------------------------------------
const bindDesigns = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/GetDesignType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid: DESIGNID }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindDesigns"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    document.getElementById("pageTitle").innerHTML = data.designName;
    document.getElementById("pageAction").innerHTML = ITEMACTION;
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindHeaders = async () => {
  try {
    const response = await fetch(`${URIMETHOD}/GetHeaderData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerId: HEADERID }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindHeaders"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    const divOrderNo = document.getElementById("divOrderNo");
    const divOrderCust = document.getElementById("divOrderCust");

    divOrderNo.innerHTML = data.orderNo;
    divOrderNo.classList.add("fw-bold");

    divOrderCust.innerHTML = data.orderCust;
    divOrderCust.classList.add("fw-bold");
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFormAction = (itemaction) => {
  const cardTitle = document.getElementById("cardTitle");
  // if (!cardTitle) return console.warn("Elemen 'cardTitle' tidak ditemukan.");

  const actionMap = {
    AddItem: "ADD ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemaction] || "";
};

const bindBlinds = async () => {
  const blindtype = document.getElementById("blindtype");
  blindtype.innerHTML = ""; //reset

  if (!DESIGNID) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindBlindType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid: DESIGNID }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindBlinds"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
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
        bindColours(DESIGNID, blindtype.value);
      }

      const blindId = blindtype.value;
    }

    if (ITEMACTION === "AddItem") loaderFadeOut();
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindColours = async (designid, blindid) => {
  const colourtype = document.getElementById("colourtype");
  colourtype.innerHTML = ""; //reset

  if (!blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindColourType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindColours"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
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

      const sel = document.getElementById("blindtype");
      const blindName = sel.selectedOptions[0].getAttribute("data-name");

      handlerElementVisibility(colourtype.value);
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabrics = async (designid) => {
  const sel = document.getElementById("fabrictype");
  sel.innerHTML = ""; //reset

  if (!designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid: DESIGNID }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindFabrics"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
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
        bindFabricColours(designid, sel.value);
      }
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabricColours = async (designid, fabricType) => {
  const sel = document.getElementById("fabriccolour");
  sel.innerHTML = ""; //reset

  if (!fabricType || !designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, fabricType }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status}\n${text}`
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindFabricColours"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
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
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
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
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : bindItemOrders"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    for (const item of data) {
      await bindBlinds(item.DesignId);
      await bindColours(item.DesignId, item.BlindId);
      await bindFabrics(item.DesignId);
      await bindFabricColours(item.DesignId, item.FabricType);
      await handlerElementVisibility(item.BlindId);
      await handlerSetElementValues(item);
      if (ITEMACTION !== "AddItem") await loaderFadeOut();
    }

    return true; // ✅ success
  } catch (error) {
    console.error("bindItemOrder error:", error);
    throw error;
  }
};

const handlerSetElementValues = (itemData) => {
  const mapping = {
    blindtype: "BlindId",
    colourtype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    width: "Width",
    drop: "Drop",
    controlposition: "ControlPosition",
    chainlength: "ChainLength",
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

    // jika nilainya "0" → kosong
    if (el.value === "0") el.value = "";
  });

  // Update counter untuk Notes
  const maxLength = 1000;
  const notesLength = (itemData["Notes"] || "").length;
  const notesCountEl = document.getElementById("notescount");
  if (notesCountEl) {
    notesCountEl.textContent = `${notesLength}/${maxLength}`;
  }

  // Kalau mode copy item → reset beberapa field
  if (ITEMACTION === "CopyItem") {
    const resetFields = ["room", "width", "drop", "notes"];
    resetFields.forEach((id) => {
      const el = document.getElementById(id);
      if (el) el.value = "";
    });

    if (notesCountEl) {
      notesCountEl.textContent = `0/${maxLength}`;
    }
  }
};

// --------------------------------------|| Other Functions ||--------------------------------------
const checkSessionCellora = () => {
  if (!HEADERID) {
    window.location.href = "/order";
    return;
  }

  if (!ORDERTYPE) {
    window.location.href = "/order";
    return;
  }

  if (!ITEMACTION || !DESIGNID) {
    window.location.href = `/order/detail`;
    return;
  }

  if (DESIGNID.toUpperCase() !== DESIGNIDORI) {
    window.location.href = `/order/detail`;
    return;
  }

  setSessionAlive();

  bindDesigns();
  bindHeaders();
  bindFormAction(ITEMACTION);

  if (ITEMACTION === "AddItem") {
    handlerElementVisibility();
    bindBlinds(DESIGNID);
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    bindItemOrders(ITEMID);
  }
};
