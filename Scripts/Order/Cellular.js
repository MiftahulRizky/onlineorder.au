document.addEventListener("DOMContentLoaded", () => {
  if (roleName === "Administrator") {
    console.log("Cellora.js loaded successfully");
    console.log("roleName: " + roleName);
    console.log("itemaction: " + itemAction);
    console.log("itemId: " + itemId);
    console.log("userId: " + userId);
    console.log("uriMethod: " + uriMethod);
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
  // const fabricType = document.querySelector("#fabrictype").value;
  // const fabricType2 = document.querySelector("#fabrictype2").value;

  bindBrackets(designId, blindId);
  // bindControls(designId, blindId);
  // bindFabrics(designId);
  // bindFabricColours(designId, fabricType);
  // bindFabrics2(designId);
  // bindFabricColours2(designId, fabricType2);
});

// change brackets
document.querySelector("#brackettype").addEventListener("change", (e) => {
  const divFormDetail = document.querySelector("#divFormDetail");
  divFormDetail.setAttribute("hidden", true);

  const blindId = document.querySelector("#blindtype").value;
  const bracketType = e.target.value;

  const fabricType = document.querySelector("#fabrictype").value;
  const fabricType2 = document.querySelector("#fabrictype2").value;

  bindControls(designId, blindId, bracketType);
  bindFabrics(designId);
  bindFabricColours(designId, fabricType);
  bindFabrics2(designId);
  bindFabricColours2(designId, fabricType2);
});

// // change controls
document.querySelector("#controltype").addEventListener("change", (e) => {
  const select = document.querySelector("#blindtype");
  const blindName = select.options[select.selectedIndex].dataset.name;

  const controlType = e.target.selectedOptions[0].dataset.name;

  bindCordType(controlType);
  handlerElementVisibility(controlType, blindName);
});

// change fabrics
document.querySelector("#fabrictype").addEventListener("change", (e) => {
  bindFabricColours(designId, e.target.value);
});

document.querySelector("#fabrictype2").addEventListener("change", (e) => {
  bindFabricColours2(designId, e.target.value);
});

// input notes count length
document.querySelector("#notes").addEventListener("input", (e) => {
  let maxLength = 1000;
  let currentLength = e.target.value.length;
  document.querySelector(
    "#notescount"
  ).textContent = `${currentLength}/${maxLength}`;
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
  handlerSubmit(buttonSubmit.form, e.target);
});

// =========================================||FUNCTIONS||=====================================

// --------------------------------------|| handler Functions ||--------------------------------------
const handlerSubmit = async (formEl, button) => {
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
      headerid: headerId,
      itemaction: itemAction,
      itemid: itemId,
      designid: designId,
      loginid: loginId,
    };

    // gabungkan
    const finalData = { ...formObject, ...extraData };

    // debug konsisten
    // console.group("Submit Debug");
    // console.log("FormData snapshot:", [...formData.entries()]);
    // console.table(formObject);
    // console.table(extraData);
    // return console.table(finalData);
    // console.groupEnd();

    // before send
    button.setAttribute("disabled", "disabled");
    button.innerHTML = '<i class="ti ti-loader fs-2 me-1"></i> Processing...';

    // fetch POST
    const response = await fetch(uriMethod + "/SubmitForm", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: finalData }),
    });

    // restore button
    button.removeAttribute("disabled");
    button.innerHTML = "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit";

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        roleName === "Administrator"
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
      window.location.href = "/order/detail";
    }
  } catch (err) {
    await isError(err.message);
  }
};

const handlerElementVisibility = (controltype, blindname) => {
  const btnSubmit = document.querySelector("#btnSubmit");

  const divFormDetail = document.getElementById("divFormDetail");
  const divMarkUp = document.getElementById("divMarkUp");

  const divFabricNight = document.getElementById("divFabricNight");
  const lblFabricDay = document.getElementById("lblFabricDay");
  const lblFabricNight = document.getElementById("lblFabricNight");
  const divCordType = document.getElementById("divCordType");

  // set default hide
  btnSubmit.setAttribute("hidden", true);
  divFormDetail.setAttribute("hidden", true);
  divMarkUp.setAttribute("hidden", true);
  divFabricNight.setAttribute("hidden", true);
  divCordType.setAttribute("hidden", true);

  lblFabricDay.innerHTML = "fabric type x colour";
  lblFabricNight.innerHTML = "fabric type x colour";
  if (controltype) divFormDetail.removeAttribute("hidden");

  if (
    blindname == "Galaxy" &&
    (controltype == "DN Corded" || controltype == "DN Cordless")
  ) {
    divFabricNight.removeAttribute("hidden");
    lblFabricDay.innerHTML = "fabric type x colour day";
    lblFabricNight.innerHTML = "fabric type x colour night";
  }

  if (blindname == "Galaxy" && controltype.includes("Corded")) {
    divCordType.removeAttribute("hidden");
  }

  // markup
  if (markupAccess === "True") divMarkUp.removeAttribute("hidden");

  btnSubmit.innerHTML =
    "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit";

  if (["AddItem", "EditItem", "CopyItem"].includes(itemAction)) {
    btnSubmit.removeAttribute("hidden");
  } else if (itemAction === "ViewItem") {
    btnSubmit.removeAttribute("hidden");
    if (roleName !== "Administrator") btnSubmit.setAttribute("hidden", true);
  }
};

// --------------------------------------|| Binding Functions ||--------------------------------------
const bindDesigns = async () => {
  try {
    const response = await fetch(`${uriMethod}/GetDesignType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
          ? "No data returned from server : bindDesigns"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    document.getElementById("pageTitle").innerHTML = data.designName;
    document.getElementById("pageAction").innerHTML = itemAction;
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindHeaders = async () => {
  try {
    const response = await fetch(`${uriMethod}/GetHeaderData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
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
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFormAction = (itemAction) => {
  const cardTitle = document.getElementById("cardTitle");
  // if (!cardTitle) return console.warn("Elemen 'cardTitle' tidak ditemukan.");

  const actionMap = {
    AddItem: "ADD ITEM",
    EditItem: "EDIT ITEM",
    ViewItem: "VIEW ITEM",
    CopyItem: "COPY ITEM",
  };
  cardTitle.innerText = actionMap[itemAction] || "";
};

const bindBlinds = async () => {
  const blindtype = document.getElementById("blindtype");
  blindtype.innerHTML = ""; //reset

  if (!designId) return;

  try {
    const response = await fetch(`${uriMethod}/BindBlindType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
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
        bindControls(designId, blindtype.value);
      }

      const blindId = blindtype.value;
      //   bindControls(designId, blindId);
    }

    if (itemAction === "AddItem") loaderFadeOut();
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindBrackets = async (designId, blindId) => {
  const brackettype = document.getElementById("brackettype");
  brackettype.innerHTML = ""; //reset

  if (!blindId) return;

  try {
    const response = await fetch(`${uriMethod}/BindBracketType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId, blindId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
          ? "No data returned from server : bindBrackets"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      brackettype.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.value = "";
        defaultOption.text = "";
        brackettype.appendChild(defaultOption);
      }

      data.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        brackettype.appendChild(option);
        brackettype.classList.add("fw-bold");
      });

      if (data.length === 1) {
        brackettype.selectedIndex = 0;
        bindControls(designId, blindId, brackettype.value);
      }

      // const sel = document.getElementById("blindtype");
      // const blindName = sel.selectedOptions[0].getAttribute("data-name");

      // handlerElementVisibility(brackettype.value, blindName);
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindControls = async (designId, blindId, bracketType) => {
  const controltype = document.getElementById("controltype");
  controltype.innerHTML = ""; //reset

  if (!blindId) return;

  try {
    const response = await fetch(`${uriMethod}/BindControlType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId, blindId, bracketType }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
          ? "No data returned from server : bindControls"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      controltype.innerHTML = ""; //reset

      if (data.length > 0) {
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

      if (data.length === 1) {
        controltype.selectedIndex = 0;
        visibleElementForm(blindName, controltype.value);
      }

      const sel = document.getElementById("blindtype");
      const blindName = sel.selectedOptions[0].getAttribute("data-name");

      handlerElementVisibility(controltype.value, blindName);
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabrics = async (designId) => {
  const sel = document.getElementById("fabrictype");
  sel.innerHTML = ""; //reset

  if (!designId) return;

  try {
    const response = await fetch(`${uriMethod}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
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
        bindFabricColours(designId, sel.value);
      }
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};
const bindFabrics2 = async (designId) => {
  const sel = document.getElementById("fabrictype2");
  sel.innerHTML = ""; //reset

  if (!designId) return;

  try {
    const response = await fetch(`${uriMethod}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
          ? "No data returned from server : bindFabrics2"
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
        bindFabricColours2(designId, sel.value);
      }
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindFabricColours = async (designId, fabricType) => {
  const sel = document.getElementById("fabriccolour");
  sel.innerHTML = ""; //reset

  if (!fabricType || !designId) return;

  try {
    const response = await fetch(`${uriMethod}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId, fabricType }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
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
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};
const bindFabricColours2 = async (designId, fabricType) => {
  const sel = document.getElementById("fabriccolour2");
  sel.innerHTML = ""; //reset

  if (!fabricType || !designId) return;

  try {
    const response = await fetch(`${uriMethod}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designId, fabricType }),
    });

    // cek status HTTP (400, 500, dsb.)
    if (!response.ok) {
      const text = await response.text();
      const msg =
        roleName === "Administrator"
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
        roleName === "Administrator"
          ? "No data returned from server : bindFabricColours2"
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
      roleName === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindCordType = (controltype) => {
  const sel = document.getElementById("cordtype");
  sel.innerHTML = ""; //reset

  if (!controltype) return;

  let data = [];
  data = [
    { value: "Cord Standard", text: "Cord Standard" },
    { value: "Continous Cord", text: "Continous Cord" },
  ];

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
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindItemOrders = async (itemId) => {
  try {
    if (!itemId) return;

    const res = await fetch(`${uriMethod}/BindItemOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ itemId }),
    });

    if (!res.ok) {
      const msg =
        roleName === "Administrator"
          ? `${res.status} - ${res.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    const response = await res.json();
    const data = response.d;

    if (!data || data.length === 0) {
      const msg =
        roleName === "Administrator"
          ? "No data returned from server : bindItemOrders"
          : "Please contact our IT team at support@onlineorder.au";
      throw isError(msg);
    }

    for (const item of data) {
      await bindBlinds(item.DesignId);
      await bindBrackets(item.DesignId, item.BlindId);
      await bindControls(item.DesignId, item.BlindId, item.BracketType);
      await bindFabrics(item.DesignId);
      await bindFabricColours(item.DesignId, item.FabricType);
      await bindFabrics2(item.DesignId);
      await bindFabricColours2(item.DesignId, item.FabricTypeB);
      await bindCordType(item.ControlType);
      await handlerElementVisibility(item.ControlType, item.BlindName);
      await handlerSetElementValues(item);
      if (itemAction !== "AddItem") await loaderFadeOut();
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
    brackettype: "BracketType",
    controltype: "KitId",
    qty: "Qty",
    room: "Location",
    mounting: "Mounting",
    fabrictype: "FabricType",
    fabriccolour: "FabricId",
    fabrictype2: "FabricTypeB",
    fabriccolour2: "FabricIdB",
    width: "Width",
    drop: "Drop",
    cordtype: "MaterialCord",
    controlposition: "ControlPosition",
    chainlength: "ChainLength",
    holddown: "BottomHoldDown",
    cutout: "DoorCutOut",
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
  if (itemAction === "CopyItem") {
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

  setSessionAlive();

  bindDesigns();
  bindHeaders();
  bindFormAction(itemAction);

  if (itemAction === "AddItem") {
    handlerElementVisibility();
    bindBlinds(designId);
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
    bindItemOrders(itemId);
  }
};
