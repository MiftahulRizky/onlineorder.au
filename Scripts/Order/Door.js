document.addEventListener("DOMContentLoaded", () => {
  if (ROLENAME === "Administrator" || ROLENAME === "PPIC & DE") {
    console.log("Window.js loaded successfully");
    console.log("ROLENAME: " + ROLENAME);
    console.log("LEVELNAME: " + LEVELNAME);
    console.log("ITEMACTION: " + ITEMACTION);
    console.log("ITEMID: " + ITEMID);
    console.log("HEADERID: " + HEADERID);
    console.log("ORDERTYPE: " + ORDERTYPE);
    console.log("URIMETHOD: " + URIMETHOD);
  }
  doorPageLoaded();
});

// ==================================================EVENTS==================================================
// =================================================FUNCTIONS================================================
// ----------------------------------------------|| Binding Functions ||---------------------------------------
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
    const lblItemId = document.getElementById("lblItemId");
    const lblOrderNumber = document.getElementById("lblOrderNumber");
    const lblOrderName = document.getElementById("lblOrderName");

    lblOrder.innerHTML = OrderId;
    lblOrder.classList.add("fw-bold");

    lblItemId.innerHTML = ITEMID;
    lblItemId.classList.add("fw-bold");

    lblOrderNumber.innerHTML = OrderNumber;
    lblOrderNumber.classList.add("fw-bold");

    lblOrderName.innerHTML = OrderName;
    lblOrderName.classList.add("fw-bold");
  } catch (error) {
    console.error(error.message);
  }
};

// ----------------------------------------------|| Other Functions ||---------------------------------------
const doorPageLoaded = async () => {
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
  //   bindFormAction(ITEMACTION, ITEMID);

  if (ITEMACTION === "AddItem") {
    // await bindBlinds(DESIGNID);
    // handlerElementVisibility();
    loaderFadeOut();
  } else if (["EditItem", "ViewItem", "CopyItem"].includes(ITEMACTION)) {
    // await bindItemOrders(ITEMID);
    loaderFadeOut();
  }
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
