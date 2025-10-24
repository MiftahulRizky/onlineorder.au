document.addEventListener("DOMContentLoaded", () => {
  console.log("venetian.js loaded successfully");
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

// MOUNTING
const inputMounting = document.getElementById("MainContent_ddlMounting");
if (inputMounting) {
  // change reset error
  inputMounting.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// FABRIC COLOUR
const inputFabricColour = document.getElementById(
  "MainContent_ddlFabricColour"
);
if (inputFabricColour) {
  // change reset error
  inputFabricColour.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

//INPUT WIDTH
const inputWidth = document.getElementById("MainContent_txtWidth");
if (inputWidth) {
  // keyup reset error
  inputWidth.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT DROP
const inputDrop = document.getElementById("MainContent_txtDrop");
if (inputDrop) {
  // keyup reset error
  inputDrop.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// STACK POSITION
const inputStack = document.getElementById("MainContent_ddlStackPosition");
if (inputStack) {
  // change reset error
  inputStack.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// TRACK TYPE
const inputTrack = document.getElementById("MainContent_ddlTrackType");
if (inputTrack) {
  // change reset error
  inputTrack.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// TRACK COLOUR
const inputTrackColour = document.getElementById("MainContent_ddlTrackColour");
if (inputTrackColour) {
  // change reset error
  inputTrackColour.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// WAND COLOUR
const inputWandColour = document.getElementById("MainContent_ddlWandColour");
if (inputWandColour) {
  // change reset error
  inputWandColour.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// WAND SIZE
const inputWandSize = document.getElementById("MainContent_ddlWandSize");
if (inputWandSize) {
  // change reset error
  inputWandSize.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

const inputWandCustomSize = document.getElementById(
  "MainContent_txtWandCustomLength"
);
if (inputWandCustomSize) {
  // change reset error
  inputWandCustomSize.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}
// ===========================FUNCTIONS=========================
function resetError() {
  if (msgError) {
    msgError.classList.add("d-none");
  }
}
