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
    const divColourType = document.getElementById("divColourType");

    const divFormDetail = document.getElementById("divFormDetail");
    const divMarkUp = document.getElementById("divMarkUp");

    const btnSubmit = document.querySelector("#btnSubmit");

    divBracketType.setAttribute("hidden", true);
    divTubeType.setAttribute("hidden", true);
    divControlType.setAttribute("hidden", true);
    divColourType.setAttribute("hidden", true);

    divFormDetail.setAttribute("hidden", true);
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
