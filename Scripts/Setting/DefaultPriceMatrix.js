$(document).ready(function () {});

// ==================================================EVENTS==================================================
// #-------------------------|| Input Event ||-------------------------#
// PRICE GROUP
const priceGroup = document.getElementById("MainContent_ddlPriceGroup");
if (priceGroup) {
  tomSelectType("MainContent_ddlPriceGroup");
}
// ==================================================FUNCTIONS===============================================
// #-------------------------|| Other Functions ||-------------------------#
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
