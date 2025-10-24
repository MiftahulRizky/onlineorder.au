document.addEventListener("DOMContentLoaded", () => {
  console.log("aluminium.js loaded successfully");
});
// ===========================EVENTS===========================
//MSG ERROR
const msgError = document.getElementById("MainContent_divError");

// INPUT QTY
const inputQTY = document.getElementById("MainContent_txtQty");
if (inputQTY) {
  // keyup reset error
  inputQTY.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT LOCATION/ROOM INSTALL
const inputLocation = document.getElementById("MainContent_txtLocation");
if (inputLocation) {
  // keyup reset error
  inputLocation.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT MOUNTING
const inputMounting = document.getElementById("MainContent_ddlMounting");
if (inputMounting) {
  // change reset error
  inputMounting.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// WIDTH
const inputWidth = document.getElementById("MainContent_txtWidth");
if (inputWidth) {
  // keyup reset error
  inputWidth.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

//DROP
const inputDrop = document.getElementById("MainContent_txtDrop");
if (inputDrop) {
  // keyup reset error
  inputDrop.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CONTROL POSITION
const inputPosition = document.getElementById("MainContent_ddlControl");
if (inputPosition) {
  // change reset error
  inputPosition.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CONTROL LENGTH
const inputLength = document.getElementById("MainContent_txtControlLength");
if (inputLength) {
  // keyup reset error
  inputLength.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// BRACKET
const inputBracket = document.getElementById("MainContent_ddlBracket");
if (inputBracket) {
  // change reset error
  inputBracket.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}
// ==========================FUNCTIONS=========================
function resetError() {
  if (msgError) {
    msgError.classList.add("d-none");
  }
}
