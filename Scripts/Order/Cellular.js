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
  const blindName = e.target.selectedOptions[0].dataset.name;

  // const lblBracketType = document.querySelector("#lblBracketType");
  // lblBracketType.innerHTML = "cell type";

  const lblControlType = document.querySelector("#lblControlType");
  lblControlType.innerHTML = "control type";

  const divBracketType = document.querySelector("#divBracketType");
  // const divControlType = document.querySelector("#divControlType");
  divBracketType.setAttribute("hidden", true);
  // divControlType.setAttribute("hidden", true);

  if (blindName == "Cellora") {
    // divControlType.removeAttribute("hidden");
  }
  if (blindName == "Galaxy") {
    divBracketType.removeAttribute("hidden");
    // divControlType.removeAttribute("hidden");
    const bracketType = document.querySelector("#brackettype").value;
    bindControls(DESIGNID, blindId, bracketType); // for reset
  }
  if (blindName == "Potrait") {
    divBracketType.removeAttribute("hidden");
    // lblBracketType.innerHTML = "system type";
    lblControlType.innerHTML = "system type";
  }

  bindBrackets(DESIGNID, blindId);
});

// change brackets
document.querySelector("#brackettype").addEventListener("change", (e) => {
  const divFormDetail = document.querySelector("#divFormDetail");
  divFormDetail.setAttribute("hidden", true);

  const blinds = document.querySelector("#blindtype");
  const blindName = blinds.options[blinds.selectedIndex].dataset.name;

  const controls = document.querySelector("#controltype");
  const controlName =
    controls?.options?.[controls.selectedIndex]?.dataset?.name;

  const blindId = blinds.value;
  const bracketType = e.target.value;
  const bracketName = e.target.selectedOptions[0].dataset.name;

  const fabricType = document.querySelector("#fabrictype").value;
  const fabricType2 = document.querySelector("#fabrictype2").value;

  bindControls(DESIGNID, blindId, bracketType);
  bindFabrics(DESIGNID, blindName, bracketName, controlName);
  bindFabricColours(DESIGNID, fabricType);
  bindFabrics2(DESIGNID, blindName, bracketName, controlName);
  bindFabricColours2(DESIGNID, fabricType2);
});

// change controls
document.querySelector("#controltype").addEventListener("change", (e) => {
  const blinds = document.querySelector("#blindtype");
  const blindName = blinds.options[blinds.selectedIndex].dataset.name;

  const brackets = document.querySelector("#brackettype");
  const bracketName = brackets.options[brackets.selectedIndex].dataset.name;

  const controlName = e.target.selectedOptions[0].dataset.name;

  const fabricType = document.querySelector("#fabrictype").value;
  const fabricType2 = document.querySelector("#fabrictype2").value;

  bindFabrics(DESIGNID, blindName, bracketName, controlName);
  bindFabricColours(DESIGNID, fabricType);
  bindFabrics2(DESIGNID, blindName, bracketName, controlName);
  bindFabricColours2(DESIGNID, fabricType2);
  bindControlSystem(controlName);
  bindMotorType();
  bindMotorExtra();
  bindAdditional();
  bindCordType(controlName);
  handlerElementVisibility(controlName, blindName);
});

// change fabrics
document.querySelector("#fabrictype").addEventListener("change", (e) => {
  bindFabricColours(DESIGNID, e.target.value);
});
document.querySelector("#fabrictype2").addEventListener("change", (e) => {
  bindFabricColours2(DESIGNID, e.target.value);
});

// change control system
const controlSelect = document.querySelector("#controlsystem");
controlSelect.addEventListener("change", () => {
  const values = controlSelect.tomselect.getValue(); // array

  const divMotor = document.querySelector("#divMotor");
  divMotor.setAttribute("hidden", true);

  if (values.includes("Motorised")) {
    divMotor.removeAttribute("hidden");
  }
});

// input notes count length
document.querySelector("#notes").addEventListener("input", (e) => {
  let maxLength = 1000;
  let currentLength = e.target.value.length;
  document.querySelector("#notescount").textContent =
    `${currentLength}/${maxLength}`;
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
  handlerSubmit(buttonSubmit.form, e.target);
});

// =========================================||FUNCTIONS||=====================================

// --------------------------------------|| handler Functions ||--------------------------------------
const handlerSubmit = async (formEl, button) => {
  try {
    // create FormData
    const formData = new FormData(formEl);
    let formObject = {};

    // loop semua elemen form
    // [...formEl.elements].forEach((el) => {
    //   if (!el.name) return; // skip elemen tanpa name

    //   // khusus number → pakai value langsung, jangan biarkan kosong
    //   if (el.type === "number") {
    //     formData.append(el.name, el.value ?? "");
    //   } else {
    //     formData.append(el.name, el.value);
    //   }
    // });

    for (const key of formData.keys()) {
      const values = formData.getAll(key);

      // jika lebih dari satu → array
      // jika satu → single value
      formObject[key] = values.length > 1 ? values : values[0];
    }

    // ubah FormData menjadi object
    // let formObject = Object.fromEntries(formData.entries());

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
      Object.entries(formObject).filter(([key]) => !excludeKeys.includes(key)),
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
    button.innerHTML = "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit";

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        ROLENAME === "Administrator"
          ? `${response.status}\n${errorText}`
          : "Something went wrong, please try again!",
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

const handlerElementVisibility = (controltype, blindname, controlsystem) => {
  const btnSubmit = document.querySelector("#btnSubmit");

  const divFormDetail = document.getElementById("divFormDetail");
  const divMarkUp = document.getElementById("divMarkUp");

  const divBracketType = document.getElementById("divBracketType");
  const divControlType = document.getElementById("divControlType");
  const lblControlType = document.getElementById("lblControlType");

  const divFabricNight = document.getElementById("divFabricNight");

  const lblFabricDay = document.getElementById("lblFabricDay");
  const lblFabricNight = document.getElementById("lblFabricNight");

  const divFabricDayType = document.getElementById("divFabricDayType");
  const divFabricDayColour = document.getElementById("divFabricDayColour");

  const divControlSystem = document.getElementById("divControlSystem");
  const divAdditional = document.getElementById("divAdditional");

  const divCordType = document.getElementById("divCordType");
  const divMotor = document.getElementById("divMotor");

  // set default hide
  btnSubmit.setAttribute("hidden", true);
  divFormDetail.setAttribute("hidden", true);
  divBracketType.setAttribute("hidden", true);
  // divControlType.setAttribute("hidden", true);
  divFabricNight.setAttribute("hidden", true);
  divControlSystem.setAttribute("hidden", true);
  divAdditional.setAttribute("hidden", true);
  divCordType.setAttribute("hidden", true);
  divMotor.setAttribute("hidden", true);
  divMarkUp.setAttribute("hidden", true);

  lblControlType.innerHTML = "control type";
  lblFabricDay.innerHTML = "fabric type x colour";
  lblFabricNight.innerHTML = "fabric type x colour";

  // divFabricDayType.classList.remove("col-lg-8");
  // divFabricDayType.classList.add("col-lg-4");
  // divFabricDayColour.removeAttribute("hidden");
  if (controltype) divFormDetail.removeAttribute("hidden");

  if (blindname == "Cellora") {
    // divControlType.removeAttribute("hidden");
  }

  if (blindname == "Galaxy") {
    divBracketType.removeAttribute("hidden");
    // divControlType.removeAttribute("hidden");

    if (controltype == "DN Corded" || controltype == "DN Cordless") {
      divFabricNight.removeAttribute("hidden");
      lblFabricDay.innerHTML = "fabric type x colour day";
      lblFabricNight.innerHTML = "fabric type x colour night";
    }

    if (controltype.includes("Corded")) {
      divCordType.removeAttribute("hidden");
    }
  }

  if (blindname == "Potrait") {
    divBracketType.removeAttribute("hidden");
    divControlSystem.removeAttribute("hidden");
    divAdditional.removeAttribute("hidden");
    lblControlType.innerHTML = "system type";
    // lblFabricDay.innerHTML = "fabric";
    // divFabricDayType.classList.remove("col-lg-4");
    // divFabricDayType.classList.add("col-lg-8");
    // divFabricDayColour.setAttribute("hidden", true);
  }

  if (controlsystem && controlsystem.includes("Motorised")) {
    divMotor.removeAttribute("hidden");
  }

  // markup
  if (MARKUPACCESS === "True") divMarkUp.removeAttribute("hidden");

  btnSubmit.innerHTML =
    "<i class='fa-solid fa-cloud-arrow-up me-2'></i> Submit";

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
        bindControls(DESIGNID, blindtype.value);
      }

      const blindId = blindtype.value;
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

const bindBrackets = async (designid, blindid) => {
  const brackettype = document.getElementById("brackettype");
  brackettype.innerHTML = ""; //reset

  if (!blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindBracketType`, {
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

        const fabricType = document.querySelector("#fabrictype").value;
        const fabricType2 = document.querySelector("#fabrictype2").value;

        const blinds = document.querySelector("#blindtype");
        const blindName = blinds.options[blinds.selectedIndex].dataset.name;

        const bracketName = brackettype.value;

        const controls = document.querySelector("#controltype");
        const controlName =
          controls?.options?.[controls.selectedIndex]?.dataset?.name;

        bindControls(designid, blindid, brackettype.value);
        bindFabrics(designid, blindName, bracketName, controlName);
        bindFabricColours(designid, fabricType);
        bindFabrics2(designid, blindName, bracketName, controlName);
        bindFabricColours2(designid, fabricType2);
      }
    }
  } catch (err) {
    // error karena jaringan / parsing JSON
    const msg =
      ROLENAME === "Administrator"
        ? "bindBrackets : " + err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindControls = async (designid, blindid, brackettype) => {
  const controltype = document.getElementById("controltype");
  controltype.innerHTML = ""; //reset

  if (!blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindControlType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindid, brackettype }),
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
          ? "No data returned from server : bindControls"
          : "Please contact our IT team at support@onlineorder.au";
      return isError(msg);
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      controltype.innerHTML = ""; //reset

      if (data.length > 1) {
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
        const blinds = document.getElementById("blindtype");
        const blindName = blinds.options[blinds.selectedIndex].dataset.name;

        const brackets = document.getElementById("brackettype");
        const bracketName =
          brackets.options[brackets.selectedIndex].dataset.name;

        const controlName =
          controltype.options[controltype.selectedIndex].dataset.name;

        const fabricType = document.querySelector("#fabrictype").value;
        const fabricType2 = document.querySelector("#fabrictype2").value;

        bindFabrics(designid, blindName, bracketName, controlName);
        bindFabricColours(designid, fabricType);
        bindFabrics2(designid, blindName, bracketName, controlName);
        bindFabricColours2(designid, fabricType2);
        bindControlSystem(controlName);
        bindCordType(controlName);
        handlerElementVisibility(controlName, blindName);
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

const bindFabrics = async (designid, blindname, bracketname, controlname) => {
  const sel = document.getElementById("fabrictype");
  sel.innerHTML = ""; //reset

  if (!designid || !blindname || !bracketname || !controlname) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindname, bracketname, controlname }),
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
const bindFabrics2 = async (designid, blindname, bracketname, controlname) => {
  const sel = document.getElementById("fabrictype2");
  sel.innerHTML = ""; //reset

  if (!designid || !blindname || !bracketname || !controlname) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, blindname, bracketname, controlname }),
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

const bindFabricColours = async (designid, fabrictype) => {
  const sel = document.getElementById("fabriccolour");
  sel.innerHTML = ""; //reset

  if (!fabrictype || !designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, fabrictype }),
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
const bindFabricColours2 = async (designid, fabrictype) => {
  const sel = document.getElementById("fabriccolour2");
  sel.innerHTML = ""; //reset

  if (!fabrictype || !designid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindFabricColour`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ designid, fabrictype }),
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

const bindControlSystem = async (controlname) => {
  const sel = document.getElementById("controlsystem");
  sel.innerHTML = ""; //reset

  controlSystemTS.clear(); // clear selected
  controlSystemTS.clearOptions(); // clear dropdown

  if (!controlname) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindControlSystem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ controlname }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      return isError(msg);
    }

    const result = await response.json();
    const data = result.d;

    if (!data) {
      return isError("No data returned from server : bindControlSystem");
    }

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
        option.setAttribute("data-name", item.text);
        sel.add(option);
      });

      if (data.length === 1) {
        sel.selectedIndex = 0;
      }

      controlSystemTS.addOptions(data);
      controlSystemTS.refreshOptions(false);
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const bindCordType = (controlname) => {
  const sel = document.getElementById("cordtype");
  sel.innerHTML = ""; //reset

  if (!controlname) return;

  let data = [];
  data = [
    { value: "Standard Cord", text: "Standard Cord" },
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

const bindMotorType = () => {
  const sel = document.getElementById("motortype");
  sel.innerHTML = ""; //reset

  // if (!controlsystem) return;

  let data = [];
  data = [
    { value: "STD 36W", text: "STD 36W" },
    { value: "STD Rechargable", text: "STD Rechargable" },
    { value: "TDBU 36W", text: "TDBU 36W" },
    { value: "TDBU Rechargable", text: "TDBU Rechargable" },
    { value: "D&N 36W", text: "D&N 36W" },
    { value: "D&N Rechargable", text: "D&N Rechargable" },
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

const bindMotorExtra = () => {
  const sel = document.getElementById("motorextra");
  sel.innerHTML = ""; //reset

  // if (!controlsystem) return;

  let data = [];
  data = [
    { value: "36W Adapter", text: "36W Adapter" },
    { value: "Ext. Cable for PowerBar", text: "Ext. Cable for PowerBar" },
    { value: "Corded PowerBar", text: "Corded PowerBar" },
    { value: "Cordess PowerBar", text: "Cordess PowerBar" },
    { value: "Ext.Rod-910mm", text: "Ext.Rod-910mm" },
    { value: "Remote With Holder", text: "Remote With Holder" },
    { value: "Additional Remote Holder", text: "Additional Remote Holder" },
    { value: "G2 SmartDial Remote", text: "G2 SmartDial Remote" },
    { value: "G2 SmartDial Colour Ring", text: "G2 SmartDial Colour Ring" },
    { value: "G2 ShadeAuto Hub", text: "G2 ShadeAuto Hub" },
    { value: "Repeater", text: "Repeater" },
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

const bindAdditional = () => {
  const sel = document.getElementById("additional");
  sel.innerHTML = ""; //reset

  // if (!controlsystem) return;

  let data = [];
  data = [
    {
      value: "Dual Shade (2 on 1)",
      text: "Dual Shade (2 on 1)",
    },
    {
      value: "Decoflex Fram Colour",
      text: "Decoflex Fram Colour",
    },
    {
      value: "Pre-drilled Frames",
      text: "Pre-drilled Frames",
    },
    { value: "Specialty shapes", text: "Specialty shapes" },
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
      await bindBrackets(item.DesignId, item.BlindId);
      await bindControls(item.DesignId, item.BlindId, item.BracketType);
      await bindFabrics(
        item.DesignId,
        item.BlindName,
        item.BracketType,
        item.ControlType,
      );
      await bindFabricColours(item.DesignId, item.FabricType);
      await bindFabrics2(
        item.DesignId,
        item.BlindName,
        item.BracketType,
        item.ControlType,
      );
      await bindFabricColours2(item.DesignId, item.FabricTypeB);
      await bindControlSystem(item.ControlType);
      await bindMotorType();
      await bindMotorExtra();
      await bindAdditional();
      await bindCordType(item.ControlType);
      await handlerElementVisibility(
        item.ControlType,
        item.BlindName,
        item.HangerType,
      );
      await handlerSetElementValues(item);
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
    motortype: "MotorStyle",
    motorextra: "AdditionalMotor",
    holddown: "BottomHoldDown",
    cutout: "DoorCutOut",
    additional: "Accessory",
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

  const controlSystemEl = document.getElementById("controlsystem");

  if (controlSystemEl?.tomselect) {
    let csValue = itemData["HangerType"];

    // normalisasi → array
    if (typeof csValue === "string") {
      csValue = csValue
        .split(",")
        .map((v) => v.trim())
        .filter(Boolean);
    }

    if (Array.isArray(csValue)) {
      // pastikan option sudah ada
      controlSystemEl.tomselect.clear();
      controlSystemEl.tomselect.setValue(csValue, true);
    }
  }

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
const checkSessionCellora = async () => {
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

  await bindDesigns();
  await bindHeaders();
  bindFormAction(ITEMACTION);
  tomSelectPlug("controlsystem");

  if (ITEMACTION === "AddItem") {
    handlerElementVisibility();
    await bindBlinds(DESIGNID);
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    await bindItemOrders(ITEMID);
    loaderFadeOut();
  }
};

let controlSystemTS;
const tomSelectPlug = (param) => {
  const el = document.getElementById(param);
  if (!el) return;
  controlSystemTS = new TomSelect(el, {
    copyClassesToDropdown: false,
    dropdownParent: "body",
    controlInput: "<input>",
    render: {
      item: function (data, escape) {
        if (data.customProperties) {
          return (
            '<div><span class="dropdown-item-indicator">' +
            data.customProperties +
            "</span>" +
            escape(data.text.toUpperCase()) +
            "</div>"
          );
        }
        return "<div>" + escape(data.text.toUpperCase()) + "</div>";
      },
      option: function (data, escape) {
        if (data.customProperties) {
          return (
            '<div><span class="dropdown-item-indicator">' +
            data.customProperties +
            "</span>" +
            escape(data.text.toUpperCase()) +
            "</div>"
          );
        }
        return "<div>" + escape(data.text.toUpperCase()) + "</div>";
      },
    },
  });
};
