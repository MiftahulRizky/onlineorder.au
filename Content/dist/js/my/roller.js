document.addEventListener("DOMContentLoaded", () => {
  console.log("roller.js loaded successfully");
  // select2FaricType();
});
// ===========================EVENTS===========================
//MSG ERROR
const msgError = document.getElementById("MainContent_divError");
// console.log(msgError);

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

// FABRIC TYPE
const fabricType = document.getElementById("MainContent_ddlFabricType");
if (fabricType) {
  tomSelectType("MainContent_ddlFabricType");
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
  if (fabricType.value != "") {
    tomSelectColour(inputFabricColour.id);
  }
}

// ROLL DIRECTION
const inputRoll = document.getElementById("MainContent_ddlRoll");
if (inputRoll) {
  // change reset error
  inputRoll.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CONTROL POSITION
const inputControlPosition = document.getElementById(
  "MainContent_ddlControlPosition"
);
if (inputControlPosition) {
  // change reset error
  inputControlPosition.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    ("");
    resetError();
  });
}

// CHAIN COLOUR
const inputChainColour = document.getElementById("MainContent_ddlChainColour");
if (inputChainColour) {
  // change reset error
  inputChainColour.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT CHAIN LENGTH
const inputChainLength = document.getElementById("MainContent_txtChainLength");
if (inputChainLength) {
  // keyup reset error
  inputChainLength.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// INPUT WIDTH
const inputWidth = document.getElementById("MainContent_txtWidth");
if (inputWidth) {
  // keyup reset error
  inputWidth.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

//INPUT DROP
const inputDrop = document.getElementById("MainContent_txtDrop");
if (inputDrop) {
  // keyup reset error
  inputDrop.addEventListener("keyup", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// TUBE SIZE
const inputTubeSize = document.getElementById("MainContent_ddlTubeSize");
if (inputTubeSize) {
  // change reset error
  inputTubeSize.addEventListener("change", (e) => {
    e.target.classList.remove("is-invalid");
    resetError();
  });
}

// CHILD SAFE
const inputChildSafe = document.getElementById("MainContent_ddlChildSafe");
if (inputChildSafe) {
  // change reset error
  inputChildSafe.addEventListener("change", (e) => {
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

function tomSelectType(id) {
  var el;
  window.TomSelect &&
    new TomSelect((el = document.getElementById(id)), {
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
              escape(data.text) +
              "</div>"
            );
          }
          return "<div>" + escape(data.text) + "</div>";
        },
        option: function (data, escape) {
          if (data.customProperties) {
            return (
              '<div><span class="dropdown-item-indicator">' +
              data.customProperties +
              "</span>" +
              escape(data.text) +
              "</div>"
            );
          }
          return "<div>" + escape(data.text) + "</div>";
        },
      },
    });
}

function tomSelectColour(id) {
  var el;
  window.TomSelect &&
    new TomSelect((el = document.getElementById(id)), {
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
              escape(data.text) +
              "</div>"
            );
          }
          return "<div>" + escape(data.text) + "</div>";
        },
        option: function (data, escape) {
          if (data.customProperties) {
            return (
              '<div><span class="dropdown-item-indicator">' +
              data.customProperties +
              "</span>" +
              escape(data.text) +
              "</div>"
            );
          }
          return "<div>" + escape(data.text) + "</div>";
        },
      },
    });
}
