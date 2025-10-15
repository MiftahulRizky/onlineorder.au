document.addEventListener("DOMContentLoaded", () => {
  console.log("header.js loaded successfully");
});

// ===========================EVENTS===========================
//MSG ERROR
const msgError = document.getElementById("MainContent_divError");

// #store name
const storeName = document.getElementById("MainContent_ddlStore");
if (storeName) {
  // change reset error
  storeName.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// order number
const orderNumber = document.getElementById("MainContent_txtOrderNo");
if (orderNumber) {
  // keyup reset error
  orderNumber.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// #reference
const reference = document.getElementById("MainContent_txtReference");
if (reference) {
  // keyup reset error
  reference.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// delivery
const delivery = document.getElementById("MainContent_ddlDelivery");
if (delivery) {
  // change reset error
  delivery.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// ===========================FUNCTIONS========================
function resetError() {
  if (msgError) {
    msgError.classList.add("d-none");
  }
}
