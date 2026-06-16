window.addEventListener("DOMContentLoaded", function () {
  if (
    ROLENAME == "Administrator" ||
    ROLENAME == "Customer Service" ||
    ROLENAME == "PPIC & DE" ||
    ROLENAME == "Data Entry"
  ) {
    console.log("Detail.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("CUSTOMERID: " + CUSTOMERID);
    console.log("USERNAME: " + USERNAME);
    console.log("HEADERID: " + HEADERID);
    console.log("PRICEACCESS: " + PRICEACCESS);
    console.log("CUSTOMERCONTACTID: " + CUSTOMERCONTACTID);
    console.log("PREVIEWACCESS: " + PREVIEWACCESS);
    console.log("REPRINT: " + REPRINT);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  detailPageLoaded();
});
// ==================================================EVENTS==================================================
// ------------------------------------------||Card 1 Event ||-------------------------------------------
// BUTTON FINISH
document.querySelector("#btnFinish").addEventListener("click", () => {
  window.location.href = "/order";
});

// BUTTON PREVIEW PRINT
document.querySelector("#btnPreviewPrint").addEventListener("click", () => {
  handlerCreatePDFOrder(
    HEADERID,
    "preview",
    "Please wait while we generate the document.",
  );
});

// BUTTON PREVIEW PDF
document.querySelector("#btnPreviewPDF").addEventListener("click", () => {
  handlerCreatePDFOrder(
    HEADERID,
    "download",
    "Please wait while we generate the document.",
  );
});

// BUTTON CONVERT TO JOB
document.querySelector("#btnConvertToJob").addEventListener("click", () => {
  handlerConvertToJob(HEADERID, "convert", "Please wait while we convert...");
});

// BUTTON RE PRINT JOB SHEET
document.querySelector("#btnReprintJobSheet").addEventListener("click", () => {
  handlerCreateJOBOrder(HEADERID, "reprint", "Please wait while we reprint...");
});

// BUTTON SUBMIT ORDER
document.querySelector("#btnSubmit").addEventListener("click", () => {
  handlerSubmitOrder(
    HEADERID,
    "submit",
    "Please wait while we submit the order.",
  );
});

// BUTTON EDIT HEADER
document.querySelector("#btnEditHeader").addEventListener("click", () => {
  // handlerEditHeader(HEADERID);
  window.location.href = `/order/header?action=edit&param=${HEADERID}&ordertype=${ORDERTYPE}`;
});

// BUTTON DELETE HEADER
document.querySelector("#btnDeleteHeader").addEventListener("click", () => {
  handlerDeleteHeader(HEADERID);
});

// BUTTON QUOTE DETAIL
document.querySelector("#btnQuoteDetail").addEventListener("click", () => {
  handlerCreatePDFCustomerQuote(
    HEADERID,
    USERNAME,
    "preview",
    "Please wait while we generate the document.",
  );
});

// BUTTON DOWNLOAD QUOTE
document.querySelector("#btnDownloadQuote").addEventListener("click", () => {
  handlerCreatePDFCustomerQuote(
    HEADERID,
    USERNAME,
    "download",
    "Please wait while we generate the document.",
  );
});

// BUTTON CHANGE STSTUS
document.querySelector("#btnChangeStatus").addEventListener("click", () => {
  document
    .querySelectorAll(
      "#modalChangeStatus .form-control, #modalChangeStatus .form-select",
    )
    .forEach((e) => {
      e.classList.remove("is-invalid");
    });
  handlerChangeStatus(HEADERID);
});

// BUTTON SEND MANUAL ORDER
document.querySelector("#btnSendOrderMail").addEventListener("click", () => {
  handlerCreatePDFOrder(
    HEADERID,
    "mail",
    "Please wait while we generate the document.",
  );
});

// BTN RELOAD PRICING
document.querySelector("#btnReloadPricing").addEventListener("click", () => {
  const statusOrder = document.getElementById("spanStatusOrder").innerHTML;
  handlerReloadPricing(HEADERID, statusOrder, "click");
});

// BTN DOWNLOAD BARCODE
document.querySelector("#btnDownloadBarcode").addEventListener("click", () => {
  handlerDownloadBarcode(HEADERID);
});

// BTN PRINT QUOTE
document.querySelector("#btnPrintQuote").addEventListener("click", () => {
  handlerPrintQuote(HEADERID, "preview");
});

// BTN SEND MAIL QUOTE
document.querySelector("#btnEmailQuote").addEventListener("click", async () => {
  try {
    document
      .querySelectorAll(
        "#modalSendMailQuote .form-control, #modalSendMailQuote .form-select",
      )
      .forEach((e) => {
        e.classList.remove("is-invalid");
        e.value = "";
      });

    const id = await getItemData(
      `SELECT Id FROM Mailings WHERE ApplicationId ='${APPLICATIONID}' AND Name = 'Quote Order Shutters' AND Active = 1`,
    );
    const from = await getItemData(
      `SELECT Server FROM Mailings WHERE ApplicationId ='${APPLICATIONID}' AND Name = 'Quote Order Shutters' AND Active = 1`,
    );
    if (!from || !id) {
      throw new Error("Server Mailings Not Found");
    }
    const customerid = document.querySelector("#spanRetailerId").innerHTML;
    const to = await getItemData(
      `SELECT Email FROM CustomerContacts WHERE CustomerId = '${customerid}' AND [Primary]=1`,
    );
    if (!to) {
      throw new Error("Please Setup Customer Email");
    }

    document.querySelector("#modalSendMailQuote #id").value = id;
    document.querySelector("#modalSendMailQuote #from").value = from;
    document.querySelector("#modalSendMailQuote #mailto").value = to;

    handlerShowBSModal("modalSendMailQuote");
  } catch (error) {
    var msg = error.message ? error.message : error;
    if (ROLENAME !== "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    await isError(msg);
  }
});

// BTN LOGS
document.querySelector("#btnLogs").addEventListener("click", () => {
  const id = HEADERID;
  const ordertype = ORDERTYPE;
  handlerLogs(id, ordertype);
});

// BTN COPY JO NUMBER
document.addEventListener("click", (e) => {
  const btn = e.target.closest("#btnCopyJoNumber, [data-jonumber]"); // id atau attribute
  if (!btn) return;

  const jonumber = btn.dataset.jonumber;
  copyToClipboard(jonumber);
});

// BUTTON ADD ITEMS
document.querySelector("#btnAddItem").addEventListener("click", async (el) => {
  document
    .querySelectorAll("#modalAddItem .form-control, #modalAddItem .form-select")
    .forEach((e) => {
      e.classList.remove("is-invalid");
    });
  const divProduction = document.getElementById("divProduction");
  divProduction.classList.add("d-none");
  const production = document.querySelector("#spanProduction");

  await handlerSelDesignType("#modalAddItem #designid", production);
  handlerShowBSModal("modalAddItem");
});

// BUTTON ADD SERVICE
document.querySelector("#btnAddService").addEventListener("click", () => {
  // reset form
  document
    .querySelectorAll(
      "#modalAddService .form-control, #modalAddService .form-select",
    )
    .forEach((e) => {
      e.classList.remove("is-invalid");
      e.value = "";
    });

  // binding
  handlerSelService("#modalAddService #category");

  // visible element
  const divType = document.getElementById("divType");
  const lblType = document.getElementById("lblType");
  const modalLabel = document.getElementById("modalAddServiceLabel");

  modalLabel.innerHTML = "Add New Surcharge";
  divType.setAttribute("hidden", true);
  lblType.innerHTML = "Catgory Type";

  handlerShowBSModal("modalAddService");
});

// ------------------------------------------||modalSendMailQuote Event ||------------------------------------
document
  .querySelectorAll(
    "#modalSendMailQuote .form-control, #modalSendMailQuote .form-select",
  )
  .forEach((e) => {
    e.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

document
  .querySelector("#modalSendMailQuote #btnSendMailQuote")
  .addEventListener("click", () => {
    submitSendMailQuote();
  });
// ------------------------------------------||modalAddItem Event ||------------------------------------
// CHANGE DESIGN TYPE
document.querySelectorAll("#modalAddItem .form-select").forEach((e) => {
  e.addEventListener("change", async (e) => {
    e.target.classList.remove("is-invalid");

    if (e.target.id === "designid") {
      const divProduction = document.getElementById("divProduction");
      divProduction.classList.add("d-none");
      const designid = e.target.value;
      const designname = await getItemData(
        `SELECT Name FROM Designs WHERE Id = '${designid}'`,
      );

      if (
        [
          "Roller Blinds",
          "Panel Glides",
          "Roman Blinds",
          "Vertical Blinds",
        ].includes(designname)
      ) {
        let env = "";
        if (["Customer"].includes(ROLENAME)) {
          env = "AND Description = 'Environment : Production'";
        }
        if (["PPIC & DE", "Manager", "Customer Service"].includes(ROLENAME)) {
          env =
            "AND Description IN ('Environment : Production', 'Environment : Testing')";
        }
        const designs = await getItemData(
          `SELECT Id FROM Designs WHERE Name = 'Global ${designname}' ${env} AND Active = 1`,
        );

        if (designs) {
          divProduction.classList.remove("d-none");
        }
      }
      await bindProduction(designname);
    }
  });
});

// BUTTON SUBMIT ADD ITEM
document
  .querySelector("#modalAddItem #submitAddItem")
  .addEventListener("click", () => {
    const designId = document.querySelector("#modalAddItem #designid").value;
    const production = document.querySelector(
      "#modalAddItem #production",
    )?.value;
    const action = "AddItem";
    submitSelectProduct(HEADERID, ORDERTYPE, action, designId, production);
  });

// ------------------------------------------||modalAddService Event ||-------------------------------------
document
  .querySelectorAll(
    "#modalAddService .form-control, #modalAddService .form-select",
  )
  .forEach((e) => {
    e.addEventListener("change", async (e) => {
      e.target.classList.remove("is-invalid");

      // change category
      if (e.target.id == "category") {
        const selectedOption = e.target.options[e.target.selectedIndex];
        const id = selectedOption.value;
        const category = selectedOption.dataset.name;

        // binding
        await bindHardwareKit("#modalAddService #type", id);

        const divType = document.getElementById("divType");
        const lblType = document.getElementById("lblType");

        if (["Long Length Surcharge", "Powder Coating"].includes(category)) {
          lblType.innerHTML = "Type";
        }
      }
    });
    e.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

document
  .querySelector("#modalAddService #btnSubmitService")
  .addEventListener("click", () => {
    submitService();
  });

// ------------------------------------------||modalChangeStatus Event ||------------------------------------
// CHANGE STATUS
document
  .querySelector("#modalChangeStatus #status")
  .addEventListener("change", (e) => {
    const status = e.target.value;
    hanlderDisplayElementModalChangeStatus(status);
  });

document
  .querySelectorAll(
    "#modalChangeStatus .form-control, #modalChangeStatus .form-select",
  )
  .forEach((e) => {
    e.addEventListener("change", (e) => {
      e.target.classList.remove("is-invalid");
    });
    e.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

// TOOLTIP DESCRIPTION CLICK
document
  .querySelector("#modalChangeStatus #tooltipDescription")
  .addEventListener("click", () => {
    const status = document.querySelector("#modalChangeStatus #status").value;
    handlerTooltip("modalChangeStatus", status);
  });

// BUTTON SUBMIT CHANGE STATUS
document
  .querySelector("#modalChangeStatus #submitChangeStatus")
  .addEventListener("click", () => {
    submitChangeStatus();
  });

// ------------------------------------------||modalEditPricingItem Event ||------------------------------------
document
  .querySelectorAll("#modalEditPricingItem .form-control")
  .forEach((e) => {
    e.addEventListener("change", (e) => {
      e.target.classList.remove("is-invalid");
    });
    e.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });

document
  .querySelector("#modalEditPricingItem #submitEditPricingItem")
  .addEventListener("click", () => {
    submitEditPricing();
  });

// ------------------------------------------||tableAjax Event ||------------------------------------
// BUTTON DETAIL ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDetailItem") {
    const id = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const production = e.target.dataset.production;
    const headerid = e.target.dataset.headerid;
    const ordertype = ORDERTYPE;
    const designname = e.target.dataset.designname;
    handlerEditItem(
      id,
      headerid,
      ordertype,
      "ViewItem",
      designid,
      production,
      designname,
    );
  }
});

document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnEditItem") {
    const id = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const production = e.target.dataset.production;
    const headerid = e.target.dataset.headerid;
    const ordertype = ORDERTYPE;
    const designname = e.target.dataset.designname;
    handlerEditItem(
      id,
      headerid,
      ordertype,
      "EditItem",
      designid,
      production,
      designname,
    );
  }
});

// BUTTON COPY ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnCopyItem") {
    const id = e.target.dataset.id;
    const headerid = e.target.dataset.headerid;
    const product = e.target.dataset.product;
    handlerCopyItem(
      id,
      headerid,
      product,
      "Please wait while we copy the item...",
    );
  }
});

// BUTTON DELETE ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnDeleteItem") {
    const id = e.target.dataset.id;
    const product = e.target.dataset.product;
    handlerDeleteItem(id, product, "Please wait while we delete the item...");
  }
});

// BUTTON EDIT PRICING ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnEditPricingItem") {
    const id = e.target.dataset.id;
    const cost = e.target.dataset.cost || "0.00";
    const designid = e.target.dataset.designid;
    const blindid = e.target.dataset.blindid;

    document
      .querySelectorAll("#modalEditPricingItem .form-control")
      .forEach((e) => {
        e.classList.remove("is-invalid");
        e.value = "";
      });

    document.querySelector("#modalEditPricingItem #id").value = id;
    document.querySelector("#modalEditPricingItem #cost").value = cost;
    document.querySelector("#modalEditPricingItem #designid").value = designid;
    document.querySelector("#modalEditPricingItem #blindid").value = blindid;
    handlerShowBSModal("modalEditPricingItem");
    // handlerShowBSModal("modalEditPricingAllItem");
  }
});

// BUTTON PRICING ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnPricingItem") {
    const id = e.target.dataset.id;
    handlerPricingItem(id);
    handlerShowBSModal("modalPricingItem");
  }
});

// HANDLER NEXT ITEM
document.querySelector("#tableAjax").addEventListener("click", (e) => {
  if (e.target.id === "btnNextItem") {
    const id = e.target.dataset.id;
    const designid = e.target.dataset.designid;
    const production = e.target.dataset.production;
    const msgBody = e.target.dataset.next;
    handlerNextItem(
      id,
      HEADERID,
      ORDERTYPE,
      "NextItem",
      designid,
      production,
      msgBody,
    );
  }
});

// ==================================================FUNCTION================================================
// --------------------------------------------||Submit Function ||-------------------------------------------
// SUBMIT CHANGE STATUS
const submitChangeStatus = async () => {
  // Hapus semua tanda invalid di awal
  document
    .querySelectorAll("#modalChangeStatus .form-control")
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalChangeStatus #submitChangeStatus",
  );

  const fields = [
    "id",
    "status",
    "statusOld",
    "submitteddate",
    "completeddate",
    "canceleddate",
    "description",
  ];

  const paramsChangeStatus = { username: USERNAME, loginid: LOGINID };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalChangeStatus #${field}`);
    paramsChangeStatus[field] = el ? el.value : "";
  });

  try {
    // === Sebelum request ===
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait while we update the status.");

    // === Kirim request ===
    const response = await fetch(`${URIMETHOD}/UpdateStatusOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: paramsChangeStatus }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    // === Setelah sukses ===
    if (data.error) {
      await isError(data.error.message.toUpperCase());
      const fieldElement = document.querySelector(data.error.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      await isSuccess(data.success.message);
      handlerHideBSModal("modalChangeStatus");
      location.reload();
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  } finally {
    // === Setelah request selesai (sukses atau error) ===
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit`;
  }

  return false;
};

// SUBMIT SELECT PRODUCT
const submitSelectProduct = async (
  headerid,
  ordertype,
  action,
  designid,
  production,
) => {
  const btn = document.querySelector("#modalAddItem #submitAddItem");
  btn.innerHTML = "Proccessing...";
  try {
    const response = await fetch(URIMETHOD + "/SetSessionOpenPageInputItem", {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        id: "",
        rolename: ROLENAME,
        headerid: headerid,
        ordertype: ordertype,
        action: action,
        designid: designid,
        production: production,
      }),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`${response.status}\n${errorText}`);
    }

    const result = await response.json();
    const dataResult = result.d || result;

    if (dataResult.error) {
      await isWarning(dataResult.error.message?.toUpperCase());
      if (dataResult.error.field) {
        const field = document.querySelector(dataResult.error.field);
        // field.closest("[aria-hidden='true']")?.removeAttribute("aria-hidden");
        // field.focus();
        field.classList.add("is-invalid");
      }
    } else {
      // await isSuccess(dataResult.success.message);
      // window.location.href = dataResult.success.message;
      var finePage = dataResult.success.message.replace("~", "");
      window.location.href = finePage;
    }
  } catch (error) {
    var msg = error.message;
    if (ROLENAME !== "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg);
  } finally {
    btn.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i>Submit`;
  }
};

// SUBMIT SERVICE
const submitService = async () => {
  // Hapus semua tanda invalid di awal
  document
    .querySelectorAll(
      "#modalAddService .form-control, #modalAddService .form-select",
    )
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalAddService #btnSubmitService",
  );

  const fields = ["id", "category", "type"];

  const additionalParams = {
    username: USERNAME,
    headerid: HEADERID,
    loginid: LOGINID,
  };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalAddService #${field}`);
    additionalParams[field] = el ? el.value : "";
  });

  // return console.table(additionalParams);

  try {
    // === Sebelum request ===
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait a moment");

    // === Kirim request ===
    const response = await fetch(`${URIMETHOD}/SubmitService`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: additionalParams }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    // === Setelah sukses ===
    if (data.error) {
      await isError(data.error.message.toUpperCase());
      const fieldElement = document.querySelector(data.error.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      handlerHideBSModal("modalAddService");
      await isSuccess(data.success.message);
      location.reload();
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  } finally {
    // === Setelah request selesai (sukses atau error) ===
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `Submit`;
  }

  return false;
};

// SUBMIT SEND MAIL QUOTE
const submitSendMailQuote = async () => {
  // Hapus semua tanda invalid di awal
  document
    .querySelectorAll(
      "#modalSendMailQuote .form-control, #modalSendMailQuote .form-select",
    )
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalSendMailQuote #btnSendMailQuote",
  );

  const fields = ["id", "from", "mailto", "cc"];

  const additionalParams = {
    username: USERNAME,
    headerid: HEADERID,
    loginid: LOGINID,
  };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalSendMailQuote #${field}`);
    additionalParams[field] = el ? el.value : "";
  });

  try {
    // === Sebelum request ===
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait a moment");

    // === Kirim request ===
    const response = await fetch(`${URIMETHOD}/SubmitSendMailQuote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: additionalParams }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    // === Setelah sukses ===
    if (data.error) {
      await isError(data.error.message.toUpperCase());
      const fieldElement = document.querySelector(data.error.field);
      if (fieldElement) {
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      handlerHideBSModal("modalSendMailQuote");
      await isSuccess(data.success.message);
      tableData.ajax.reload();
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  } finally {
    // === Setelah request selesai (sukses atau error) ===
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `Send`;
  }

  return false;
};

// SUBMIT EDIT PRICING
const submitEditPricing = async () => {
  document
    .querySelectorAll("#modalEditPricingItem .form-control")
    .forEach((e) => e.classList.remove("is-invalid"));

  const btnSubmit = document.querySelector(
    "#modalEditPricingItem #submitEditPricingItem",
  );

  const fields = ["id", "cost", "newcost", "designid", "blindid"];

  const Params = {
    username: USERNAME,
    headerid: HEADERID,
    customerid: document.querySelector("#spanRetailerId").innerHTML,
  };

  fields.forEach((field) => {
    const el = document.querySelector(`#modalEditPricingItem #${field}`);
    Params[field] = el ? el.value : "";
  });

  try {
    btnSubmit.setAttribute("disabled", "disabled");
    btnSubmit.innerHTML = '<i class="fa fa-spin fa-spinner"></i>';
    swalLoadingShow("Please wait...");

    const response = await fetch(`${URIMETHOD}/OverridePricing`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ data: Params }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    if (data.error) {
      await isError(data.error.message.toUpperCase());
      if (data.error.field) {
        const fieldElement = document.querySelector(data.error.field);
        fieldElement.focus();
        fieldElement.classList.add("is-invalid");
      }
    } else {
      handlerHideBSModal("modalEditPricingItem");
      await isSuccess(data.success.message);
      window.location.href = `/order/detail?param=${HEADERID}&ordertype=${ORDERTYPE}`;
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? "submitEditPricing : " + error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  } finally {
    btnSubmit.removeAttribute("disabled");
    btnSubmit.innerHTML = `<i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit`;
  }
};

// ------------------------------------------||Handler Function ||-------------------------------------------
// HANDLER HIDE BOOTSTRAP MODAL
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

// HANDLER DISPLAY ELEMENT
const handlerDisplayElement = (item) => {
  // INITIALIZE ELEMENTS
  const btnJobSheet = document.getElementById("btnJobSheet");
  const btnReprintJobSheet = document.getElementById("btnReprintJobSheet");
  const btnChangeJobStatus = document.getElementById("btnChangeJobStatus");
  const btnSubmit = document.getElementById("btnSubmit");
  const btnEditHeader = document.getElementById("btnEditHeader");
  const btnDeleteHeader = document.getElementById("btnDeleteHeader");
  const btnQuote = document.getElementById("btnQuote");
  const btnQuoteDetail = document.getElementById("btnQuoteDetail");
  const btnDownloadQuote = document.getElementById("btnDownloadQuote");
  const btnMoreAction = document.getElementById("btnMoreAction");
  const btnEmailDeposit = document.getElementById("btnEmailDeposit");
  const dividerEmailDeposit = document.getElementById("dividerEmailDeposit");
  const btnChangeStatus = document.getElementById("btnChangeStatus");
  const btnSendOrderMail = document.getElementById("btnSendOrderMail");
  const btnReloadPricing = document.getElementById("btnReloadPricing");
  const btnAddItem = document.getElementById("btnAddItem");
  const btnAddService = document.getElementById("btnAddService");
  const divPrice = document.getElementById("divPrice");
  const msgThanks = document.getElementById("msgThanks");

  const thMarkUp = document.querySelector(".thMarkUp");
  const thPrice = document.querySelector(".thPrice");

  //SET DEFAULT HIDE ELEMENT
  btnJobSheet.setAttribute("hidden", true);
  btnReprintJobSheet.setAttribute("hidden", true);
  btnChangeJobStatus.setAttribute("hidden", true);
  btnSubmit.setAttribute("hidden", true);
  btnEditHeader.setAttribute("hidden", true);
  btnDeleteHeader.setAttribute("hidden", true);
  btnQuote.setAttribute("hidden", true);
  btnQuoteDetail.setAttribute("hidden", true);
  btnDownloadQuote.setAttribute("hidden", true);
  btnMoreAction.setAttribute("hidden", true);
  btnEmailDeposit.setAttribute("hidden", true);
  dividerEmailDeposit.setAttribute("hidden", true);
  btnChangeStatus.setAttribute("hidden", true);
  btnSendOrderMail.setAttribute("hidden", true);
  btnReloadPricing.setAttribute("hidden", true);
  btnAddItem.setAttribute("hidden", true);
  btnAddService.setAttribute("hidden", true);
  divPrice.setAttribute("hidden", true);
  msgThanks.setAttribute("hidden", true);

  thMarkUp.setAttribute("hidden", true);
  thPrice.setAttribute("hidden", true);

  tableData.columns(5).visible(false);
  tableData.columns(6).visible(false);

  if (!item) return;

  // btnJobSheet
  btnJobSheet.removeAttribute("hidden");
  if (ROLENAME !== "Administrator" && ROLENAME !== "PPIC & DE") {
    btnJobSheet.setAttribute("hidden", true);
  }

  // btnReprintJobSheet & btnChangeJobStatus
  if (item.JoNumberId) {
    btnReprintJobSheet.removeAttribute("hidden");
    // btnChangeJobStatus.removeAttribute("hidden");
  }

  // btnSubmit, btnEditHeader, btnDeleteHeader, & btnAddItem
  if (item.Status === "Draft" || item.Status === "Pending Price Approval") {
    switch (ROLENAME) {
      case "Customer":
        btnSubmit.removeAttribute("hidden");
        btnEditHeader.removeAttribute("hidden");
        btnDeleteHeader.removeAttribute("hidden");
        btnAddItem.removeAttribute("hidden");
        break;
      case "PPIC & DE":
      case "Manager":
      case "Customer Service":
        if (item.CreatedBy.toUpperCase() === LOGINID) {
          btnEditHeader.removeAttribute("hidden");
          btnDeleteHeader.removeAttribute("hidden");
          btnAddItem.removeAttribute("hidden");
        }
        break;
      case "Administrator":
        btnSubmit.removeAttribute("hidden");
        btnEditHeader.removeAttribute("hidden");
        btnDeleteHeader.removeAttribute("hidden");
        btnAddItem.removeAttribute("hidden");
        break;
    }
  }

  // btnAddService
  if (
    ROLENAME === "Administrator" ||
    ROLENAME === "PPIC & DE" ||
    ROLENAME === "Customer Service"
  ) {
    // if (ROLENAME === "Administrator") {
    if (item.Status !== "Completed") {
      btnAddService.removeAttribute("hidden");
    }
  }

  // btnQuote, btnQuoteDetail, & btnDownloadQuote
  if (ROLENAME === "Administrator" || ROLENAME === "Customer") {
    btnQuote.removeAttribute("hidden");
    btnQuoteDetail.removeAttribute("hidden");
    btnDownloadQuote.removeAttribute("hidden");
  }

  //  btnChangeStatus, btnAddItem, & btnSendOrderMail
  switch (item.Status) {
    case "New Order":
    case "In Production":
    case "Completed":
    case "On Hold":
      msgThanks.removeAttribute("hidden");
      if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
        btnChangeStatus.removeAttribute("hidden");
        if (ROLENAME === "Administrator") {
          btnSendOrderMail.removeAttribute("hidden");
          btnAddItem.removeAttribute("hidden");
        }
      }
      break;
  }

  // btnMoreAction
  if (
    ROLENAME === "Administrator" ||
    ROLENAME === "PPIC & DE" ||
    ROLENAME === "Customer Service"
  ) {
    btnMoreAction.removeAttribute("hidden");
  }

  // btnEmailDeposit & dividerEmailDeposit
  if (ROLENAME === "Administrator") {
    btnEmailDeposit.removeAttribute("hidden");
    dividerEmailDeposit.removeAttribute("hidden");
  }

  // btnReloadPricing
  if (item.Status !== "Canceled") {
    btnReloadPricing.removeAttribute("hidden");
    if (
      !["Administrator", "PPIC & DE", "Customer Service"].includes(ROLENAME)
    ) {
      btnReloadPricing.setAttribute("hidden", true);
    }
  }

  if (PRICEACCESS == "True" || PRICEACCESS == "1") {
    tableData.columns(5).visible(true);
    thPrice.removeAttribute("hidden");
    divPrice.removeAttribute("hidden");
  }

  if (MARKUPACCESS == "True" || MARKUPACCESS == "1") {
    tableData.columns(6).visible(true);
    thMarkUp.removeAttribute("hidden");
  }
};

// HANDLER HEADER INFO
const handlerHeaderInfo = async (item) => {
  // INITIALIZE ELEMENTS
  // CARD 1
  const spanRetailerName = document.getElementById("spanRetailerName");
  const spanRetailerId = document.getElementById("spanRetailerId");
  const spanOrderId = document.getElementById("spanOrderId");
  const spanJoNumber = document.getElementById("spanJoNumber");
  const spanOrderProductType = document.getElementById("spanOrderProductType");
  const spanOrderNo = document.getElementById("spanOrderNo");
  const spanOrderCust = document.getElementById("spanOrderCust");
  const spanCreatedDate = document.getElementById("spanCreatedDate");
  const spanCreatedBy = document.getElementById("spanCreatedBy");
  const spanNote = document.getElementById("spanNote");
  const spanStatusNote = document.getElementById("spanStatusNote");
  const spanStatusOrder = document.getElementById("spanStatusOrder");
  const spanDelivery = document.getElementById("spanDelivery");

  // CARD 2
  const spanSubmittedDate = document.getElementById("spanSubmittedDate");
  const spanCompletedDate = document.getElementById("spanCompletedDate");
  const spanCanceledDate = document.getElementById("spanCanceledDate");
  const spanTotal = document.getElementById("spanTotal");
  const spanGST = document.getElementById("spanGST");
  const spanFinalTotal = document.getElementById("spanFinalTotal");

  // SET INFORMATION OR VALUES
  if (item) {
    // INITIALIZE DATE FORMATTER
    const us = {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    };
    const indo = {
      year: "numeric",
      month: "long",
      day: "2-digit",
    };

    spanRetailerName.innerHTML = item.CustomerName;
    spanRetailerId.innerHTML = item.CustomerId;
    spanOrderId.innerHTML = item.OrderId;

    // CARD 1
    spanJoNumber.innerHTML = item.JoNumberId
      ? `<span class="badge badge-outline text-red">${item.JoNumberId}</span> <a href="javascript:void(0);" id="btnCopyJoNumber" class="btn btn-sm  border-0 bg-transparent" data-jonumber="${item.JoNumberId}"><i class="ti ti-copy fs-2 opacity-50"></i></a>`
      : "-";
    spanOrderProductType.innerHTML = item.OrderType;
    spanOrderNo.innerHTML = item.OrderNumber;
    spanOrderCust.innerHTML = item.OrderName;

    // CreatedDate
    const customDate = parseCustomDate(item.CreatedDate);
    if (!customDate || isNaN(customDate.getTime())) {
      console.warn("Tanggal tidak valid:", item.CreatedDate);
      spanCreatedDate.innerHTML = "-";
      return;
    }
    if (ROLENAME === "Administrator") {
      spanCreatedDate.innerHTML = customDate
        .toLocaleDateString("id-ID", us)
        .replace(/\./g, ":");
    } else {
      spanCreatedDate.innerHTML = customDate.toLocaleDateString("en-US", indo);
    }

    spanNote.innerHTML = item.OrderNote ? item.OrderNote : "-";
    spanStatusNote.innerHTML = item.StatusAdditional
      ? item.StatusAdditional
      : "-";
    spanStatusOrder.innerHTML = item.Status;
    spanDelivery.innerHTML = item.Delivery;

    // CARD 2
    // SubmittedDate
    if (!item.SubmittedDate) spanSubmittedDate.innerHTML = "-";
    if (item.SubmittedDate) {
      const cardPrice = document.getElementById("cardPrice");
      // cardPrice.classList.add("mb-3", "mt-1");

      const customDate = parseCustomDate(item.SubmittedDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.SubmittedDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (ROLENAME === "Administrator") {
        spanSubmittedDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanSubmittedDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo,
        );
      }
    }

    // CompletedDate
    if (!item.CompletedDate) spanCompletedDate.innerHTML = "-";
    if (item.CompletedDate) {
      const customDate = parseCustomDate(item.CompletedDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.CompletedDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (ROLENAME === "Administrator") {
        spanCompletedDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanCompletedDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo,
        );
      }
    }

    // CanceledDate
    if (!item.CanceledDate) spanCanceledDate.innerHTML = "-";
    if (item.CanceledDate) {
      const customDate = parseCustomDate(item.CanceledDate);
      if (!customDate || isNaN(customDate.getTime())) {
        console.warn("Tanggal tidak valid:", item.CanceledDate);
        spanCreatedDate.innerHTML = "-";
        return;
      }
      if (ROLENAME === "Administrator") {
        spanCanceledDate.innerHTML = customDate
          .toLocaleDateString("id-ID", us)
          .replace(/\./g, ":");
      } else {
        spanCanceledDate.innerHTML = customDate.toLocaleDateString(
          "en-US",
          indo,
        );
      }
    }

    try {
      const CreatedBy = await getItemData(
        `SELECT FullName FROM CustomerLogins WHERE Id='${item.CreatedBy}'`,
      );
      spanCreatedBy.innerHTML = CreatedBy || "-";

      let SumPrice = 0;
      let Gst = 0;
      let FinalTotal = 0;

      if (PRICEACCESS == "1" || PRICEACCESS == "True") {
        SumPrice = await getItemData(
          `SELECT SUM(TotalMatrix + TotalCharge) AS SumPrice FROM OrderDetails WHERE HeaderId = '${item.Id}' AND Active=1`,
        );

        // convert string "556,40" -> number 556.40
        SumPrice = parseFloat(SumPrice.toString().replace(",", ".")) || 0;

        if (SumPrice) {
          Gst = (SumPrice * 10) / 100;
          FinalTotal = SumPrice + Gst;
        }
      }

      const formatCurrency = (num) => {
        return num.toLocaleString("en-US", {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2,
        });
      };

      // Render
      spanTotal.innerHTML = SumPrice
        ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${formatCurrency(SumPrice)}</span>`
        : `<span style="font-size:larger;">-</span>`;

      spanGST.innerHTML = Gst
        ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${formatCurrency(Gst)}</span>`
        : `<span style="font-size:larger;">-</span>`;

      spanFinalTotal.innerHTML = FinalTotal
        ? `<span class="badge badge-outline text-green" style="font-size:larger;">$${formatCurrency(FinalTotal)}</span>`
        : `<span style="font-size:larger;">-</span>`;
    } catch (error) {
      const msg =
        ROLENAME === "Administrator"
          ? error.message
          : "Please contact our IT team at support@onlineorder.au";
      isError(msg);
    }
  }
};

// HANDLER PREVIEW PRINT ORDER
const handlerCreatePDFOrder = async (headerid, action, msgloading) => {
  // Tampilkan loading SweetAlert
  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${URIMETHOD}/CreatePDFOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: headerid,
        action: action,
      }),
    });

    Swal.close(); // Tutup loading Swal setelah respons diterima

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      await isError(result.error.message.toUpperCase());
      // location.reload();
    } else {
      await isSuccess(result.success.message);

      if (action === "download") {
        window.location.href = result.success.url;
      } else if (action === "preview") {
        window.open(result.success.url, "_blank");
      } else if (action === "submit" || action === "mail") {
        location.reload();
      }
    }
  } catch (error) {
    Swal.close(); // Tutup loading Swal saat error
    const msg = error.message || "Something went wrong while creating the PDF.";
    isError(msg);
  }
};

// HANDLER SUBMIT ORDER HEADER
const handlerSubmitOrder = async (headerid, action, msgloading) => {
  Swal.fire({
    title: "Are you sure?",
    html:
      "Sure to " +
      action +
      " this order? <br/>You won't be able to revert this!",
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, " + action + " it!",
  }).then(async (result) => {
    if (result.isConfirmed) {
      try {
        const response = await fetch(`${URIMETHOD}/SubmitOrder`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json; charset=utf-8",
          },
          body: JSON.stringify({
            headerid: headerid,
            loginid: LOGINID,
          }),
        });

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();
        const resultData = data.d || data;

        if (resultData.error) {
          isError(
            resultData.error.message.toUpperCase(),
            resultData.error.field,
          );
        } else {
          handlerCreatePDFOrder(headerid, action, msgloading);
        }
      } catch (error) {
        const msg =
          error.message || "Something went wrong while submitting the order.";
        isError(msg);
      }
    }
  });
};

// HANDLER CONVERT TO JOB
const handlerConvertToJob = async (headerid, action, msgloading) => {
  const result = await Swal.fire({
    title: "Are you sure?",
    html: "Sure to convert this order to a job? <br /> This action cannot be undone.",
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, convert it!",
  });

  if (!result.isConfirmed) return;

  const statusOrder = document
    .getElementById("spanStatusOrder")
    ?.innerHTML?.trim();

  if (!statusOrder) {
    return isError("Order status not found.");
  }

  if (["Draft", "Completed", "Canceled"].includes(statusOrder)) {
    await isError(
      `Cannot convert this order as the status is <b>${statusOrder}</b>`,
    );
    return;
  }

  // Jika semua valid → lanjut buat job
  await handlerCreateJOBOrder(headerid, action, msgloading);
};

// HANDLER CREATE JOB
const handlerCreateJOBOrder = async (headerid, action, msgloading) => {
  // Tampilkan loading SweetAlert sebelum request
  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${URIMETHOD}/CreateJOBOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid: headerid,
        action: action,
      }),
    });

    // Tutup loading setelah menerima response
    Swal.close();

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      isError(result.error.message.toUpperCase(), result.error.field);
    } else {
      await isSuccess(result.success.message);

      if (action === "download") {
        window.location.href = result.success.url;
      } else if (action === "reprint" || action === "preview") {
        window.open(result.success.url, "_blank");
      } else if (action === "convert") {
        location.reload();
      }
    }
  } catch (error) {
    Swal.close();
    const msg =
      error.message || "Something went wrong while creating JOB Order.";
    isError(msg);
  }
};

// HANDLER EDIT HEADER
const handlerEditHeader = async (headerid) => {
  try {
    const response = await fetch(`${URIMETHOD}/SetSessionOpenEditOrderHeader`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    // Jika sukses, arahkan ke halaman edit order
    window.location.href = "/order/header";
  } catch (error) {
    isError("Gagal menyetel session: " + error.message);
  }
};

// HANDLER DELETE HEADER
const handlerDeleteHeader = async (headerid) => {
  const result = await Swal.fire({
    title: "Are you sure?",
    html: "Sure to delete this order?",
    icon: "warning",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  });

  if (!result.isConfirmed) return;

  swalLoadingShow("Please wait while we delete the order.");

  try {
    const response = await fetch(`${URIMETHOD}/DeleteOrderHeader`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id: headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(
        resultData.error.message.toUpperCase(),
        resultData.error.field,
      );
    } else {
      await isSuccess(resultData.success.message);
      window.location.href = "/order";
    }
  } catch (error) {
    const msg =
      error.message || "Something went wrong while deleting the order.";
    isError(msg);
  }
};

// HANDLER CREATE PDF QUOTE
const handlerCreatePDFCustomerQuote = async (
  headerid,
  username,
  action,
  msgloading,
) => {
  try {
    // Tampilkan loading SweetAlert
    swalLoadingShow(msgloading);

    const response = await fetch(`${URIMETHOD}/CreatePDFCustomerQuote`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid, username, action }),
    });

    Swal.close(); // Tutup loading Swal setelah respons diterima

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const result = await response.json();
    const data = result.d || result;

    if (data.error) {
      isError(data.error.message.toUpperCase(), data.error.field);
    } else {
      await isSuccess(data.success.message);

      if (action === "download") {
        window.location.href = data.success.url;
      } else if (action === "preview") {
        window.open(data.success.url, "_blank");
      }
    }
  } catch (error) {
    Swal.close(); // Tutup loading Swal jika error
    isError(`Gagal membuat PDF Customer Quote: ${error.message}`);
  }
};

// HANDLER CHANGE STATUS
const handlerChangeStatus = async (headerid) => {
  try {
    if (!headerid) return;

    const response = await fetch(`${URIMETHOD}/BindOrderHeaderByID`, {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify({ headerid, ordertype: ORDERTYPE }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }

    const result = await response.json();
    const data = result.d;

    if (!data || data.length === 0) {
      const msg =
        ROLENAME === "Administrator"
          ? "No data returned from server : handlerChangeStatus"
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    for (const item of data) {
      await handlerSelStatus("#modalChangeStatus #status", item.Status);
      await setValueModalChangeStatus(item);
      await hanlderDisplayElementModalChangeStatus(item.Status);
      await handlerShowBSModal("modalChangeStatus");
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// HANDLER CELECT STSTUS
const handlerSelStatus = async (params, statusNow) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!params) return;

  let data = [];

  // for cardChangeStatus => status
  if (params === "#modalChangeStatus #status" && statusNow) {
    switch (statusNow) {
      case "Pending Price Approval":
        data = [{ value: "Draft", text: "Draft" }];
        break;
      case "Draft":
        data = [
          { value: "New Order", text: "New Order" },
          { value: "Canceled", text: "Canceled" },
        ];
        break;

      case "New Order":
        data = [
          { value: "New Order", text: "New Order" },
          { value: "In Production", text: "In Production" },
          { value: "On Hold", text: "On Hold" },
          { value: "Canceled", text: "Canceled" },
        ];
        break;

      case "In Production":
        data = [
          { value: "In Production", text: "In Production" },
          { value: "Completed", text: "Completed" },
          { value: "Canceled", text: "Canceled" },
        ];
        break;
    }

    if (ROLENAME !== "Customer") {
      data.unshift({ value: "Draft", text: "Draft" });
    }
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    sel.appendChild(option);
  });
};

// SET VALUE MODAL CHANGE STATUS
const setValueModalChangeStatus = (itemData) => {
  const mapping = {
    id: "Id",
    status: "Status",
    statusOld: "Status",
    submitteddate: "SubmittedDate",
    completeddate: "CompletedDate",
    canceleddate: "CanceledDate",
    description: "StatusDescription",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.querySelector("#modalChangeStatus #" + id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]];

    // Jika kosong/null/undefined, langsung set sebagai empty string
    if (!value) {
      el.value = "";
      return;
    }

    // Jika input type date dan format tanggal dd/mm/yyyy hh:mm:ss
    if (el.type === "date" && typeof value === "string") {
      const datePart = value.split(" ")[0]; // contoh: "16/07/2025"
      const parts = datePart.split("/"); // hasil: ["16", "07", "2025"]
      if (parts.length === 3) {
        value = `${parts[2]}-${parts[1]}-${parts[0]}`;
      } else {
        console.warn(`Format tanggal tidak sesuai: ${value}`);
        value = ""; // fallback kosong agar tidak error
      }
    }

    // Khusus untuk description, potong setelah "Notes from the office:<br />"
    if (id === "description" && typeof value === "string") {
      const marker = "Notes from the office:<br />";
      if (value.includes(marker)) {
        value = value.split(marker)[1] || "";
        // Hapus tag HTML jika masih ada
        value = value.replace(/<[^>]*>/g, "");
      }
    }

    el.value = value;
  });
};

// HANDLER DISPLAY ELEMENT MODAL CHANGE STATUS
const hanlderDisplayElementModalChangeStatus = (status) => {
  // INITIALIZE ELEMENT
  const divSubmittedDate = document.getElementById("divSubmittedDate");
  const divCompletedDate = document.getElementById("divCompletedDate");
  const divCanceledDate = document.getElementById("divCanceledDate");
  const divDescription = document.getElementById("divDescription");

  // SET DEFAULT HIDE ELEMENT
  divSubmittedDate.setAttribute("hidden", true);
  divCompletedDate.setAttribute("hidden", true);
  divCanceledDate.setAttribute("hidden", true);
  divDescription.setAttribute("hidden", true);

  if (status) {
    divDescription.removeAttribute("hidden");
    switch (status) {
      case "New Order":
        divSubmittedDate.removeAttribute("hidden");
        break;
      case "Completed":
        divCompletedDate.removeAttribute("hidden");
        break;
      case "Canceled":
        divCanceledDate.removeAttribute("hidden");
        break;
    }
  }
};

// HANDLER TOOLTIP
const handlerTooltip = (modalName, params) => {
  // INITIALIZE MESSAGE
  let title = "Tooltip";
  let msg = "This message is a tooltip";
  if (modalName === "modalChangeStatus") {
    switch (params) {
      case "New Order":
      case "Canceled":
        title = params + " Description";
        msg =
          "explain and write why you changed it to <b>" +
          params +
          "</b> status";
        break;
    }
  }

  Swal.fire({
    title: title,
    html: msg,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    icon: "question",
  });
};

// HANDLER RELOAD PRICING
const handlerReloadPricing = async (headerid, status, action) => {
  try {
    const result = await Swal.fire({
      title: "Are you sure?",
      text: "Sure to reload the pricing?",
      icon: "warning",
      showCancelButton: true,
      customClass: {
        popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
      },
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, reload it!",
    });

    if (result.isConfirmed) {
      await handlerReloadPricingOnReadyPage(headerid, status, action);
    }
  } catch (error) {
    console.error("Error reloading pricing:", error);
    isError("Failed to reload pricing: " + error.message);
  }
};

// HANDLER RELOAD PRICING ON READY PAGE
const handlerReloadPricingOnReadyPage = async (headerid, status, action) => {
  if (!headerid) return;

  if (
    action === "binding" &&
    (status !== "Draft" || status !== "Pending Price Approval")
  ) {
    return;
  }

  if (action === "click") {
    swalLoadingShow("Please wait while we reload the pricing.");
  }

  try {
    const response = await fetch(`${URIMETHOD}/ReloadPricing`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      throw new Error(result.error.message);
    } else {
      if (action === "binding") {
        if (ROLENAME === "Administrator") {
          console.log(result.success.message);
        }
      } else if (action === "click") {
        await isSuccess(result.success.message);
        location.reload();
      }
    }
  } catch (error) {
    console.error("Reload pricing failed:", error);
    isError(error.message);
  }
};

// HANDLER DOWNLOAD BARCODE
const handlerDownloadBarcode = async (headerid) => {
  swalLoadingShow("Please wait while we download the barcode.");
  try {
    const response = await fetch(`${URIMETHOD}/DownloadBarcode`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;
    if (result.error) {
      throw new Error(result.error.message);
    } else {
      isSuccess(result.success.message).then(() => {
        window.open(result.success.url, "_blank");
      });
    }
  } catch (error) {
    var msg = error.message;
    if (ROLENAME != "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg);
  }
};

// HANDLER DOWNLOAD BARCODE
const handlerPrintQuote = async (headerid, action) => {
  swalLoadingShow("Please wait while we print the quote.");
  try {
    const response = await fetch(`${URIMETHOD}/CreatePDFQuote`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, action, username: USERNAME }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d || data;
    if (result.error) {
      throw new Error(result.error.message);
    } else {
      isSuccess(result.success.message).then(() => {
        window.open(result.success.url, "_blank");
      });
    }
  } catch (error) {
    var msg = error.message;
    if (ROLENAME != "Administrator") {
      msg = "Please contact our IT team at support@onlineorder.au";
    }
    isError(msg);
  }
};

// HANLDER LOGS
const handlerLogs = async (id, ordertype) => {
  try {
    const response = await fetch(`${URIMETHOD}/Logs`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id: id, ordertype: ordertype }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      await isError(resultData.error.message.toUpperCase());
    } else {
      const table = document.querySelector("#modalLogs #table-logs tbody");
      table.innerHTML = "";

      const logs =
        typeof resultData === "string" ? JSON.parse(resultData) : resultData;

      if (logs.length === 0) {
        const tr = document.createElement("tr");
        tr.innerHTML = `
        <td  class="text-center">
          No logs found
        </td>
        `;
        table.appendChild(tr);
      }

      logs.forEach((log) => {
        const formattedDate = formatDotNetDate(log.ActionDate);
        const tr = document.createElement("tr");
        tr.innerHTML = `
        <td >
          <b>${log.FullName}</b> on ${formattedDate}. Action: ${log.Description} 
        </td>
        `;
        table.appendChild(tr);
      });

      await handlerShowBSModal("modalLogs");
    }
    // await handlerShowBSModal("modalLogs");
  } catch (error) {
    const msg = `${error.message || error}`;
    await isError(msg);
  }
};

// HANDLER SELECT DESIGN TYPE
const handlerSelDesignType = async (params, production) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  try {
    const response = await fetch(`${URIMETHOD}/BindDesignType`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        customerid: CUSTOMERID,
        ordertype: ORDERTYPE,
        production,
        rolename: ROLENAME,
      }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d;

    if (!result || result.length === 0) {
      throw new Error("No data returned from server : handlerSelDesignType");
    }

    if (Array.isArray(result)) {
      sel.innerHTML = ""; // reset ulang

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      result.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        sel.add(option);
      });
    }

    return Promise.resolve();
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

// HANDLER SELECT DESIGN TYPE
const handlerSelService = async (params) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  try {
    const response = await fetch(`${URIMETHOD}/BindService`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      // body: JSON.stringify({
      //   customerid: CUSTOMERID,
      //   ordertype: ORDERTYPE,
      // }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d;

    if (!result || result.length === 0) {
      throw new Error("No data returned from server : handlerSelService");
    }

    if (Array.isArray(result)) {
      sel.innerHTML = ""; // reset ulang

      const defaultOption = document.createElement("option");
      defaultOption.text = "";
      defaultOption.value = "";
      sel.add(defaultOption);

      result.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        sel.add(option);
      });
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

const bindHardwareKit = async (params, blindid) => {
  const sel = document.querySelector(params);
  sel.innerHTML = ""; // reset

  if (!blindid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindHardwareKit`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ blindid }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const data = await response.json();
    const result = data.d;

    if (!result || result.length === 0) {
      throw new Error("No data returned from server : bindHardwareKit");
    }

    if (Array.isArray(result)) {
      sel.innerHTML = ""; // reset ulang

      if (result.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "";
        defaultOption.value = "";
        sel.add(defaultOption);
      }

      result.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        sel.add(option);
      });

      if (result.length === 1) {
        sel.selectedIndex = 0;
      }

      if (result.length > 1) {
        divType.removeAttribute("hidden");
      }
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    await isError(msg);
  }
};

// HANDLER EDIT ITEM
const handlerEditItem = async (
  id,
  headerid,
  ordertype,
  action,
  designid,
  production,
  designname,
) => {
  if (designname == "Additional") {
    swalLoadingShow("Please wait while we prepare the data.");
    try {
      const response = await fetch(`${URIMETHOD}/BindOrderDetailsByID`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify({ itemid: id }),
      });

      if (!response.ok) {
        const msg = `${response.status} - ${response.statusText}`;
        throw new Error(msg);
      }

      const dataResponse = await response.json();
      const data = dataResponse.d;

      if (!data || data.length === 0) {
        throw new Error("No data returned from server : handlerEditItem");
      }

      for (const item of data) {
        await handlerSelService("#modalAddService #category");
        await bindHardwareKit("#modalAddService #type", item.BlindId);
        await setFormValues(item);
        await visibleFormService(item);
        await Swal.close();
        await handlerShowBSModal("modalAddService");
      }
    } catch (error) {
      const msg =
        ROLENAME === "Administrator"
          ? error.message
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
    }
  } else {
    try {
      const response = await fetch(`${URIMETHOD}/SetSessionOpenPageInputItem`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify({
          id,
          rolename: ROLENAME,
          headerid,
          ordertype,
          action,
          designid,
          production,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const data = await response.json();
      const result = data.d || data;

      const finePage = result.success.message.replace("~", "");
      window.location.href = finePage;
    } catch (error) {
      const msg =
        ROLENAME === "Administrator"
          ? "Gagal menyetel session: " + error.message
          : "Please contact our IT team at support@onlineorder.au";
      await isError(msg);
    }
  }
};

const setFormValues = (itemData) => {
  const mapping = {
    id: "Id",
    category: "BlindId",
    type: "KitId",
  };

  Object.keys(mapping).forEach((id) => {
    const el = document.querySelector("#modalAddService #" + id);
    if (!el) {
      console.warn(`Elemen '${id}' tidak ditemukan.`);
      return;
    }

    let value = itemData[mapping[id]];
    if (id === "markup" && value === 0) value = "";
    el.value = value || "";

    // Set value to empty if value is 0
    if (el) el.value = el.value === "0" ? "" : el.value;
  });
};

const visibleFormService = (itemData) => {
  const divType = document.getElementById("divType");
  const lblType = document.getElementById("lblType");
  const modalLabel = document.getElementById("modalAddServiceLabel");

  divType.setAttribute("hidden", true);
  lblType.innerHTML = "Type";
  modalLabel.innerHTML = "Edit Ervice Service";

  if (
    ["Long Length Surcharge", "Powder Coating"].includes(itemData.BlindName)
  ) {
    divType.removeAttribute("hidden");
  }
};

// HANDLER COPY ITEM
const handlerCopyItem = async (id, headerid, product, msgloading) => {
  const result = await Swal.fire({
    title: "Copy this item?",
    html: product,
    icon: "question",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, copy it!",
  });

  if (!result.isConfirmed) return;

  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${URIMETHOD}/CopyItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id, headerid, loginid: LOGINID }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    Swal.close();

    if (resultData.error) {
      throw new Error(resultData.error.message);
    } else {
      await isSuccess(resultData.success.message);
      location.reload();
    }
  } catch (error) {
    Swal.close();
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Something went wrong, please try again!";
    await isError(msg);
  }
};

// HANDLER DELETE ITEM
async function handlerDeleteItem(id, product, msgloading) {
  const result = await Swal.fire({
    title: "Sure delete this item ?",
    html: product,
    icon: "warning",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  });

  if (!result.isConfirmed) return;

  swalLoadingShow(msgloading);

  try {
    const response = await fetch(`${URIMETHOD}/DeleteItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ id }),
    });

    if (!response.ok) {
      const msg = `${response.status}\n${response.statusText}`;
      throw new Error(msg);
    }

    const data = await response.json();
    const resultData = data.d || data;

    if (resultData.error) {
      isError(resultData.error.message.toUpperCase(), resultData.error.field);
    } else {
      await isSuccess(resultData.success.message);
      location.reload();
    }
  } catch (error) {
    isError(error.message || "An unexpected error occurred");
  }
}

// HANDLER PRICING ITEM
const handlerPricingItem = (id) => {
  const paramData = { id: id };

  const columnDefs = [
    {
      width: "5%",
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { width: "100%", orderable: false, data: "Description" },
    {
      width: "15%",
      orderable: false,
      data: null,
      render: (row) => {
        let cs = "";
        if (
          ["Powder Coating", "Tracking & Interloock"].includes(
            row.Description,
          ) ||
          row.Description.includes("Powder Coating")
        ) {
          cs = "text-decoration-line-through";
        }

        let Cost = `<span class="${cs}">${row.Cost}</span>`;
        // if (row.Description.includes("POA")) {
        //   Cost = `<span class="badge bg-warning">${row.Cost}</span>`;
        // }
        if (row.CostB) {
          Cost += `<br/>${row.CostB}`;
        }
        return Cost;
      },
    },
    { width: "15%", orderable: false, data: "Poa" },
    {
      width: "15%",
      orderable: false,
      data: null,
      render: (row) => {
        let DisInPercent = row.Discount;
        // if (
        //   ["Administrator", "PPIC & DE", "Customer Service"].includes(
        //     ROLENAME,
        //   ) &&
        //   row.Discount
        // ) {
        // }
        DisInPercent = `
        <button type="button" class="border-0 bg-transparent" data-bs-container="body" data-bs-toggle="popover" data-bs-trigger="hover focus" data-bs-placement="bottom" data-bs-content="Discount in ${row.DiscountInPercent}%">
           ${row.Discount}
        </button>
      `;
        if (row.DiscountB) {
          DisInPercent += `
            <br/><button type="button" class="border-0 bg-transparent" data-bs-container="body" data-bs-toggle="popover" data-bs-trigger="hover focus" data-bs-placement="bottom" data-bs-content="Discount in ${row.DiscountInPercentB}%">
              ${row.DiscountB}
            </button>
          `;
        }
        return DisInPercent;
      },
    },
    {
      width: "15%",
      orderable: false,
      data: null,
      render: (row) => {
        let cs = "";
        if (
          ["Powder Coating", "Tracking & Interloock"].includes(
            row.Description,
          ) ||
          row.Description.includes("Powder Coating")
        ) {
          cs = "text-decoration-line-through";
        }
        let FinalCost = `<span class="${cs}">${row.FinalCost}</span>`;
        if (row.FinalCostB) {
          FinalCost += `<br/>${row.FinalCostB}`;
        }
        return FinalCost;
      },
    },
  ];

  tablePricingData = $("#tablePricingDetail").DataTable({
    processing: true,
    serverSide: true, // <<< INI KUNCI PENTINGNYA
    order: [], // Tetap bisa set default order di sini
    pageLength: 25,
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      $("#tablePricingDetail_filter").hide();
      $("#tablePricingDetail_length").hide();
      $("#tablePricingDetail_info").hide();
      $("#tablePricingDetail_paginate").hide();
    },
    ajax: {
      url: URIMETHOD + "/BindOrderPricingDetails",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      data: function (d) {
        return JSON.stringify({
          params: {
            ...paramData,
            draw: d.draw,
            start: d.start,
            length: d.length,
            order: d.order,
            columns: d.columns,
            search: d.search,
          },
        });
      },
      dataSrc: function (json) {
        json.recordsTotal = json.d.recordsTotal;
        json.recordsFiltered = json.d.recordsFiltered;
        // console.log(json);
        return json.d.data;
      },
      error: function (xhr, thrownError, ajaxOptions) {
        var msg = xhr.status + "\n" + xhr.responseText + "\n" + thrownError;
        isError(msg);
      },
    },
    drawCallback: function () {
      document.querySelectorAll('[data-bs-toggle="popover"]').forEach((el) => {
        new bootstrap.Popover(el);
      });
    },
    columns: columnDefs,
    createdRow: (row, data, dataIndex) => {
      if (
        ["Powder Coating", "Tracking & Interloock"].includes(
          data.Description,
        ) ||
        row.Description.includes("Powder Coating")
      ) {
        $(row).addClass("opacity-50");
      }
    },
  });
};

// HANDLER NEXT ITEM
const handlerNextItem = async (
  id,
  headerid,
  ordertype,
  action,
  designid,
  production,
  msgbody,
) => {
  const result = await Swal.fire({
    title: "Information",
    html: msgbody,
    icon: "info",
    showCancelButton: true,
    customClass: {
      popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
    },
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, do it!",
  });

  if (!result.isConfirmed) return;

  try {
    const response = await fetch(`${URIMETHOD}/SetSessionOpenPageInputItem`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        id,
        rolename: ROLENAME,
        headerid,
        ordertype,
        action,
        designid,
        production,
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const data = await response.json();
    const resultData = data.d || data;

    const finePage = resultData.success.message.replace("~", "");
    window.location.href = finePage;
  } catch (error) {
    var msg = "Please contact our IT team at support@onlineorder.au";
    if (ROLENAME === "Administrator") {
      msg = "Gagal menyetel session: " + error.message;
    }
    isError(msg);
  }
};

// HANDLER CHEKC ORDER
const handlerCheckOrder = async (headerid, status, loginid) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${URIMETHOD}/CheckOrder`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        headerid,
        status,
        loginid,
      }),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const data = await response.json();
    const result = data.d || data;

    if (result.error) {
      console.log(result.error.message);
      return;
    }

    if (result.success.url === "Yes") {
      await Swal.fire({
        title: "Order Information",
        html: result.success.message,
        icon: "info",
        showClass: {
          popup: `
            animate__animated
            animate__fadeInUp
            animate__faster
          `,
        },
        hideClass: {
          popup: `
            animate__animated
            animate__fadeOutDown
            animate__faster
          `,
        },
        customClass: {
          popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
        },
      });
    }
  } catch (error) {
    isError(`${error.message}`);
  }
};

// ------------------------------------------||Binding Function ||-------------------------------------------
// BIND ORDER HEADER
const bindOrderHeaderByID = async (headerid, ordertype) => {
  if (!headerid) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindOrderHeaderByID`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({ headerid, ordertype }),
    });

    if (!response.ok) {
      const msg =
        ROLENAME === "Administrator"
          ? `${response.status} - ${response.statusText}`
          : "Please contact our IT team at support@onlineorder.au";
      throw new Error(msg);
    }

    const dataResponse = await response.json();
    const data = dataResponse.d;

    if (!data || data.length === 0) {
      window.location.href = "/order";
    }

    for (const item of data) {
      // await handlerReloadPricingOnReadyPage(item.Id, item.Status, "binding");
      await handlerHeaderInfo(item);
      await bindDetails(item.Id, item.Status, item.CreatedBy);
      await handlerDisplayElement(item);
      await handlerCheckOrder(item.Id, item.Status, item.CreatedBy);
      await loaderFadeOut();
    }
  } catch (error) {
    const msg =
      ROLENAME === "Administrator"
        ? error.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

// BIND ORDER DETAILS
let tableData;
const bindDetails = async (headerid, status, createdby) => {
  const paramData = {
    headerid: headerid,
    status: status,
    createdby: createdby,
  };

  // render: function (data, type, row, meta)
  const columnDefs = [
    {
      width: "5%",
      data: "No",
      orderable: false,
      render: (data) => `<div class="text-center">${data}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: true,
      render: (row) => `<div class="text-center">${row.Id}</div>`,
    },
    {
      width: "5%",
      data: null,
      orderable: false,
      render: (row) => `<div class="text-center">${row.Qty}</div>`,
    },
    { width: "20%", data: "Location" },
    {
      width: "60%",
      data: null,
      orderable: false,
      render: (row) => {
        // HIDE BUTTON NEXT
        let brNext = row.HideNext !== "hidden" ? "</br>" : "";
        return `
          ${row.Product}
          ${brNext}
          <button type="button" class="btn btn-sm btn-outline-success mt-1" id="btnNextItem" data-id="${row.Id}" data-designid="${row.DesignId}" data-next="${row.TextNext}" data-production="${row.Production}" ${row.HideNext}>
            <i class="bi bi-node-plus me-1"></i>
            Next Item
          </button>
          `;
      },
    },
    { width: "5%", data: "Cost" },
    { width: "5%", data: "MarkUp" },
    {
      width: "5%",
      data: null,
      orderable: false,
      render: (row) => {
        return dropdownActionButton(row, createdby);
      },
    },
  ];

  tableData = $("#tableAjax").DataTable({
    serverSide: true,
    pageLength: 100,
    bPaginate: true,
    bInfo: true,
    bFilter: true,
    bDestroy: true,
    autoWidth: false,
    initComplete: function () {
      $("#tableAjax_filter").hide();
      $("#tableAjax_length").hide();
      $("#tableAjax_info").hide();
      $("#tableAjax_paginate").hide();
    },
    ajax: async function (data, callback) {
      try {
        const response = await fetch(URIMETHOD + "/BindOrderDetails", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            params: {
              ...paramData,
              draw: data.draw,
              start: data.start,
              length: data.length,
              order: data.order,
              columns: data.columns,
              search: data.search,
            },
          }),
        });

        const json = await response.json();

        callback({
          draw: json.d.draw,
          recordsTotal: json.d.recordsTotal,
          recordsFiltered: json.d.recordsFiltered,
          data: json.d.data,
        });
      } catch (err) {
        console.error(err);
        isError(err);
      }
    },
    columns: columnDefs,
  });
};

const bindProduction = async (designname) => {
  const sel = document.querySelector("#modalAddItem #production");
  sel.innerHTML = ""; //reset

  if (!designname) return;

  let data = [{ value: "Sunlight", text: "Sunlight" }];
  if (
    [
      "Roller Blinds",
      "Panel Glides",
      "Roman Blinds",
      "Vertical Blinds",
    ].includes(designname)
  ) {
    let env = "";
    if (["Customer"].includes(ROLENAME)) {
      env = "AND Description = 'Environment : Production'";
    }
    if (["PPIC & DE", "Manager", "Customer Service"].includes(ROLENAME)) {
      env =
        "AND Description IN ('Environment : Production', 'Environment : Testing')";
    }
    const designs = await getItemData(
      `SELECT Id FROM Designs WHERE Name = 'Global ${designname}' ${env} AND Active = 1`,
    );

    if (designs) {
      data.push({ value: "Global", text: "Global" });
    }
  } else {
    data.push({ value: "Sunlight", text: "Sunlight" });
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

// --------------------------------------------||Other Function ||-------------------------------------------
// CHECK SESSION
const detailPageLoaded = () => {
  if (!ULTRON || !ORDERTYPE) window.location.href = "/order";

  if (CUSTOMERID == "LS-A224") window.location.href = "/order"; // JPM Direct

  if (CUSTOMERID == "DEFAULT" && USERNAME == "galih") {
    window.location.href = "/order";
  }

  bindOrderHeaderByID(HEADERID, ORDERTYPE);
};

const copyToClipboard = (text) => {
  // Jika Clipboard API tersedia
  if (navigator.clipboard && navigator.clipboard.writeText) {
    navigator.clipboard
      .writeText(text)
      .catch((err) => console.error("Clipboard API Error:", err));
    return;
  }

  // Fallback (aman untuk HTTP)
  const ta = document.createElement("textarea");
  ta.value = text;
  ta.style.position = "fixed"; // hindari scroll
  ta.style.opacity = "0";
  document.body.appendChild(ta);
  ta.focus();
  ta.select();

  try {
    document.execCommand("copy");
  } catch (err) {
    console.error("Fallback copy error:", err);
  }

  ta.remove();
};

// FORMAT DATE TIME
const parseCustomDate = (value) => {
  if (!value || typeof value !== "string") return null;

  // Format ISO: 2025-07-10 08:42:01.653
  if (/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}/.test(value)) {
    return new Date(value.replace(" ", "T"));
  }

  // Format: 10/07/2025 08:42:01 (24 jam)
  const match24 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{2}):(\d{2}):(\d{2})$/,
  );
  if (match24) {
    const [_, day, month, year, hour, minute, second] = match24;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(
        2,
        "0",
      )}T${hour}:${minute}:${second}`,
    );
  }

  // Format: 13/07/2025 1:06:07 PM (12 jam)
  const match12 = value.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4}) (\d{1,2}):(\d{2}):(\d{2}) (\w{2})$/,
  );
  if (match12) {
    let [_, day, month, year, hour, minute, second, period] = match12;
    hour = parseInt(hour, 10);
    if (period === "PM" && hour < 12) hour += 12;
    if (period === "AM" && hour === 12) hour = 0;
    return new Date(
      `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}T${hour
        .toString()
        .padStart(2, "0")}:${minute}:${second}`,
    );
  }

  return null;
};

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

const formatDotNetDate = (value) => {
  if (!value) return "";

  // Ambil angka di dalam /Date(XXXXX)/
  const timestamp = parseInt(value.replace("/Date(", "").replace(")/", ""));

  const date = new Date(timestamp);

  const day = String(date.getDate()).padStart(2, "0");
  const monthNames = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];
  const month = monthNames[date.getMonth()];
  const year = date.getFullYear();

  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");

  return `${day} ${month} ${year} ${hours}:${minutes}`;
};

// --------------------------------------------||Additional Serverside ||-------------------------------------------
const dropdownActionButton = (row, createdby) => {
  // HIDE BUTTON DETAIL
  let hideDetail = "";
  let hideEdit = "hidden";
  let hideCopy = "hidden";
  let hideDelete = "hidden";
  if (["Draft", "Pending Price Approval"].includes(row.StatusHeader)) {
    hideDetail = "hidden";
    hideEdit = "";
    hideCopy = "";
    hideDelete = "";
    if (
      ["PPIC & DE", "Customer Service"].includes(ROLENAME) &&
      createdby.toUpperCase() !== LOGINID.toUpperCase()
    ) {
      hideDetail = "";
      hideEdit = "hidden";
      hideCopy = "hidden";
      hideDelete = "hidden";
    } else if (["Manager", "Account"].includes(ROLENAME)) {
      hideCopy = "hidden";
      hideDelete = "hidden";
    }
  }

  if (row.DesignName == "Additional") {
    hideDelete = "";
  }

  // HIDE BUTTON EDIT PRICING
  let hideEditPricing = "hidden";
  if (row.Group === "POA" || row.PriceGroupName.includes("POA")) {
    hideEditPricing = "";
  }

  //  HIDE BUTTON PRICING
  let hidePricing = "hidden";
  if (PRICEACCESS == "True" || PRICEACCESS == "1") {
    hidePricing = "";
  }

  // HIDE DIVIDER
  let hideDivider = "";
  if (hideEditPricing == "hidden" && hidePricing == "hidden") {
    hideDivider = "hidden";
  }

  return `
      <div class="dropdown text-center">
        <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
          <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
        </button>
        <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
          <li ${hideDetail}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnDetailItem" data-id="${row.Id}"" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}" data-designname="${row.DesignName}" data-production='${row.Production}'>
              <i class="ti ti-info-square-rounded me-1 opacity-50 fs-2"></i>Detail
            </a>
          </li>
          <li ${hideEdit}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnEditItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-designid="${row.DesignId}" data-designname="${row.DesignName}" data-production='${row.Production}'>
            <i class="ti ti-edit me-1 opacity-50 fs-2"></i>Edit
            </a>
          </li>
          <li ${hideCopy}>
            <a class="dropdown-item" href="javascript:void(0);" id="btnCopyItem" data-id="${row.Id}" data-headerid="${row.HeaderId}" data-product="${row.Product}" >
              <i class="ti ti-copy-plus me-1 opacity-50 fs-2"></i>Copy
            </a>
          </li>
          <li ${hideDelete}>
            <a class="dropdown-item text-danger" href="javascript:void(0);" id="btnDeleteItem" data-id="${row.Id}" data-product="${row.Product}">
              <i class="ti ti-trash-x me-1 opacity-50 fs-2"></i>Delete
            </a>
          </li>
          <div ${hideDivider} class="dropdown-divider"></div>
          <li ${hideEditPricing}>
            <a class="dropdown-item " href="javascript:void(0);" id="btnEditPricingItem" data-id="${row.Id}" data-cost="${row.RealCost}" data-designid="${row.DesignId}" data-blindid="${row.BlindId}">
              <i class="ti ti-pencil-dollar text-success fs-1 me-1 opacity-50"></i>Edit Pricing
            </a>
          </li>
          <li>
            <a ${hidePricing} class="dropdown-item " href="javascript:void(0);" id="btnPricingItem" data-id="${row.Id}">
              <i class="ti ti-tags me-1 opacity-50 fs-2"></i>Pricing
            </a>
          </li>
        </ul>
      </div>
    `;
};
const stylingColumnSearchAndPaging = () => {
  $("#tableAjax_filter").hide();
  $("#tableAjax_length").hide();
  $("#tableAjax_info").hide();
  $("#tableAjax_paginate").hide();
};
