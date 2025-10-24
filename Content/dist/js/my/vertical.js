document.addEventListener("DOMContentLoaded", () => {
  console.log("venetian.js loaded successfully");
  // select2FaricType();
});
// ===========================EVENTS===========================
//MSG ERROR
const msgError = document.getElementById("MainContent_divError");

// INPUT QTY
const inputQTY = document.getElementById("MainContent_txtQty");
if (inputQTY) {
  // input reset error
  inputQTY.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT LOCATION/ROOM INSTALL
const inputLocation = document.getElementById("MainContent_txtLocation");
if (inputLocation) {
  // input reset error
  inputLocation.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// MOUNTING
const inputMounting = document.getElementById("MainContent_ddlMounting");
if (inputMounting) {
  // input reset error
  inputMounting.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// IMPUT WIDTH
const inputWidth = document.getElementById("MainContent_txtWidth");
if (inputWidth) {
  // input reset error
  inputWidth.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT DROP
const inputDrop = document.getElementById("MainContent_txtDrop");
if (inputDrop) {
  // input reset error
  inputDrop.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// TRACK COLOUR
const inputTrackColour = document.getElementById("MainContent_ddlTrackColour");
if (inputTrackColour) {
  // input reset error
  inputTrackColour.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// STACK POSITION
const inputStack = document.getElementById("MainContent_ddlStackPosition");
if (inputStack) {
  // input reset error
  inputStack.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CONTROL POSITION
const inputControlPosition = document.getElementById(
  "MainContent_ddlControlPosition"
);
if (inputControlPosition) {
  // input reset error
  inputControlPosition.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CHAIN COLOUR
const inputChainColour = document.getElementById("MainContent_ddlChainColour");
if (inputChainColour) {
  // input reset error
  inputChainColour.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT CHAIN LENGTH
const inputChainLength = document.getElementById("MainContent_txtChainLength");
if (inputChainLength) {
  // input reset error
  inputChainLength.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

const customWandLength = document.getElementById(
  "MainContent_txtWandCustomLength"
);
if (customWandLength) {
  // input reset error
  customWandLength.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// BRACKETS
const inputBrackets = document.getElementById("MainContent_ddlBrackets");
if (inputBrackets) {
  // input reset error
  inputBrackets.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// BRACKET COLOUR
const inputBracketColour = document.getElementById(
  "MainContent_ddlBracketColour"
);
if (inputBracketColour) {
  // input reset error
  inputBracketColour.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// HANGER TYPE
const inputHangerType = document.getElementById("MainContent_ddlHangerType");
if (inputHangerType) {
  // input reset error
  inputHangerType.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// BOTTOM
const inputBottom = document.getElementById("MainContent_ddlBottom");
if (inputBottom) {
  // input reset error
  inputBottom.addEventListener("input", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}
// ============================FUNCTIONS===========================
function resetError() {
  if (msgError) {
    msgError.classList.add("d-none");
  }
}
function select2FaricType() {
  const fabricType = document.getElementById("MainContent_ddlFabricType");
  if (fabricType) {
    $("#MainContent_ddlFabricType").select2({
      theme: "bootstrap-5",
      // placeholder: "Pilih opsi",
      allowClear: true,
    });
  }
}
